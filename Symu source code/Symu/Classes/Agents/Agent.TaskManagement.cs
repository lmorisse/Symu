#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Symu.Classes.Agents.Models;
using Symu.Classes.Agents.Models.CognitiveModel;
using Symu.Classes.Blockers;
using Symu.Classes.Murphies;
using Symu.Classes.Task;
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.Repository;
using Symu.Repository.Networks.Knowledges;
using Symu.Results.Blocker;
using static SymuTools.Constants;

#endregion

namespace Symu.Classes.Agents
{
    /// <summary>
    ///     An abstract base class for agents.
    ///     You must define your own agent derived classes derived
    ///     This partial class focus on tasks management methods
    /// </summary>
    public abstract partial class Agent
    {
        #region Work on task
        /// <summary>
        ///     Override this method to specify how an agent will get new tasks to complete
        ///     By default, if worker can't perform task or has reached the maximum number of tasks,
        ///     he can't ask for more tasks, just finished the tasks in the taskManager
        /// </summary>
        public virtual void GetNewTasks()
        {
        }

        /// <summary>
        ///     Work on the next task
        /// </summary>
        public void WorkInProgress(SymuTask task)
        {
            if (task == null)
            {
                Status = AgentStatus.Available;
                return;
            }

            // The task may be blocked, try to unlock it
            TryRecoverBlockedTask(task);
            // Agent may discover new blockers
            CheckBlockers(task);
            // Task may have been blocked
            // Capacity may have been used for blockers
            if (!task.IsBlocked && Capacity.HasCapacity)
            {
                WorkOnTask(task);
            }

            if (Capacity.HasCapacity)
                // We start a new loop on the current tasks of the agent
            {
                SwitchingContextModel();
            }
        }

        /// <summary>
        ///     Simulate the work on a specific task
        /// </summary>
        /// <param name="task"></param>
        public virtual float WorkOnTask(SymuTask task)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            float timeSpent;
            if (Schedule.Type == TimeStepType.Intraday)
            {
                timeSpent = Math.Min(Environment.Organization.Models.Intraday, Capacity.Actual);
            }
            else
            {
                timeSpent = Cognitive.TasksAndPerformance.TasksLimit.LimitSimultaneousTasks
                    // Mono tasking
                    ? Math.Min(task.Weight, Capacity.Actual)
                    // Multi tasking
                    : Math.Min(task.Weight / 2, Capacity.Actual);
            }

            timeSpent = Math.Min(task.WorkToDo, timeSpent);
            task.WorkToDo -= timeSpent;
            if (task.WorkToDo < Tolerance)
            {
                SetTaskDone(task);
            }
            else
            {
                UpdateTask(task);
            }

            // As the agent work on task that requires knowledge, the agent can't forget the associate knowledge today
            foreach (var knowledgeId in task.KnowledgesBits.KnowledgeIds)
            {
                ForgettingModel.UpdateForgettingProcess(knowledgeId, task.KnowledgesBits.GetBits(knowledgeId));
            }

            Capacity.Decrement(timeSpent);
            return timeSpent;
        }

        /// <summary>
        ///     Set the task done in task manager
        /// </summary>
        /// <param name="task"></param>
        public void SetTaskDone(SymuTask task)
        {
            TaskProcessor.PushDone(task);
        }

        /// <summary>
        ///     Update the task as the agent has worked on it, but not complete it
        /// </summary>
        /// <param name="task"></param>
        public void UpdateTask(SymuTask task)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            task.Update(Schedule.Step);
        }
        #endregion

        #region Capacity

        /// <summary>
        ///     Describe the agent capacity
        /// </summary>
        public AgentCapacity Capacity { get; } = new AgentCapacity();

        /// <summary>
        ///     Set the initial capacity for the new step based on SetInitialCapacity, working day,
        ///     By default = Initial capacity if it's a working day, 0 otherwise
        ///     If resetRemainingCapacity set to true, Remaining capacity is reset to Initial Capacity value
        /// </summary>
        public void HandleCapacity(bool resetRemainingCapacity)
        {
            // Intentionally no test on Agent that must be able to perform tasks
            // && Cognitive.TasksAndPerformance.CanPerformTask
            // Example : internet access don't perform task, but is online
            if (IsPerformingTask())
            {
                SetInitialCapacity();
                // Intentionally after SetInitialCapacity
                MurphiesImpactsOnCapacity();
                if (Cognitive.TasksAndPerformance.CanPerformTask)
                {
                    Environment.IterationResult.Capacity += Capacity.Initial;
                }
            }
            else
            {
                Capacity.Initial = 0;
            }

            if (resetRemainingCapacity)
            {
                Capacity.Reset();
            }
        }

        /// <summary>
        ///     Use to set the baseline value of the initial capacity
        /// </summary>
        /// <returns></returns>
        public virtual void SetInitialCapacity()
        {
            Capacity.Initial = 1;
        }

        /// <summary>
        ///     Murphies impacts on the capacity
        /// </summary>
        /// <returns></returns>
        public virtual void MurphiesImpactsOnCapacity()
        {
            // Unavailability
            if (Environment.Organization.Murphies.UnAvailability.Next())
            {
                Capacity.Initial = 0;
                Status = AgentStatus.Offline;
            }
        }

        /// <summary>
        ///     Switching context may have an impact on the agent capacity
        /// </summary>
        public virtual void SwitchingContextModel()
        {
        }
        #endregion

        #region TimeSpent
        /// <summary>
        ///     Daily Track of the TimeSPent for each keyActivity
        ///     Key => step
        ///     Value => time spent
        /// </summary>
        public Dictionary<ushort, float> TimeSpent { get; } = new Dictionary<ushort, float>();

        /// <summary>
        ///     Impact of the Communication channels on the remaining capacity
        ///     Allocate this time on the keyActivity
        /// </summary>
        /// <param name="medium"></param>
        /// <param name="keyActivity">the keyActivity activity of the task, to track TimeSpent</param>
        /// <param name="send">If set, it is an ask help task, otherwise it is a reply help task</param>
        public void ImpactOfTheCommunicationMediumOnTimeSpent(CommunicationMediums medium, bool send, ushort keyActivity)
        {
            if (keyActivity == 0)
            {
                return;
            }

            var impact =
                Environment.WhitePages.Network.NetworkCommunications.TimeSpent(medium, send,
                    Environment.Organization.Models.RandomLevelValue);
            AddTimeSpent(keyActivity, impact);
        }

        /// <summary>
        ///     Manage TimeSpent of the agent, by keyActivity
        /// </summary>
        /// <param name="keyActivity"></param>
        /// <param name="timeSpent"></param>
        public void AddTimeSpent(ushort keyActivity, float timeSpent)
        {
            if (!TimeSpent.ContainsKey(keyActivity))
            {
                TimeSpent.Add(keyActivity, 0);
            }

            TimeSpent[keyActivity] += timeSpent;
        }
        #endregion

        #region Post task

        /// <summary>
        ///     Post a task in the TasksProcessor
        /// </summary>
        /// <param name="task"></param>
        /// <remarks>Don't use TaskProcessor.Post directly to handle the OnBeforeTaskPost event</remarks>
        public void Post(SymuTask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            if (!Cognitive.TasksAndPerformance.CanPerformTask)
            {
                return;
            }
            OnBeforePostTask(task);
            if (!task.IsBlocked)
            {
                TaskProcessor.Post(task);
            }
        }

        /// <summary>
        ///     Post a task in the TasksProcessor
        /// </summary>
        /// <param name="tasks"></param>
        /// <remarks>Don't use TaskProcessor.Post directly to handle the OnBeforeTaskPost event</remarks>
        public void Post(IEnumerable<SymuTask> tasks)
        {
            if (tasks is null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }
            if (!Cognitive.TasksAndPerformance.CanPerformTask)
            {
                return;
            }
            foreach (var task in tasks)
            {
                OnBeforePostTask(task);
                if (!task.IsBlocked)
                {
                    TaskProcessor.Post(task);
                }
            }
        }

        /// <summary>
        ///     EventHandler triggered before the event TaskProcessor.Post(task)
        ///     By default CheckBlockerBeliefs
        ///     If task must be posted, use task.Blockers
        /// </summary>
        /// <param name="task"></param>
        protected virtual void OnBeforePostTask(SymuTask task)
        {
            CheckBlockerBeliefs(task);
        }

        #endregion

        #region Common Blocker
        private void ActHelp(Message message)
        {
            switch (message.Action)
            {
                case MessageAction.Ask:
                    // From worker
                    AskHelp(message);
                    break;
                case MessageAction.Reply:
                    // From worker
                    ReplyHelp(message);
                    break;
            }
        }
        /// <summary>
        ///     A teammate ask help because of a murphy
        /// </summary>
        /// <param name="message"></param>
        public virtual void AskHelp(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!(message.Attachments.First is Blocker blocker))
            {
                return;
            }

            switch (blocker.Type)
            {
                case Murphy.IncompleteKnowledge:
                    AskHelpIncompleteKnowledge(message);
                    break;
                case Murphy.IncompleteBelief:
                    AskHelpIncompleteBelief(message);
                    break;
            }
        }
        /// <summary>
        ///     After asking for help, an agent help finally the worker.
        ///     Task can be unlocked
        /// </summary>
        /// <param name="message"></param>
        public virtual void ReplyHelp(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!(message.Attachments.Second is SymuTask task))
            {
                throw new ArgumentNullException(nameof(task));
            }

            // Some repliers arrive a bit late, blocker has already been removed
            if (!task.Blockers.IsBlocked)
            {
                return;
            }

            // Remove blocker
            // TODO should use blockerId but for the moment blocker is not in the message.Parameter and blocker don't have Id
            var blocker = (Blocker)message.Attachments.First;
            switch (blocker.Type)
            {
                case Murphy.IncompleteKnowledge:
                    ReplyHelpIncompleteKnowledge(task, blocker, message.Attachments.KnowledgeBits, message.Medium,
                        message.Sender.ClassKey == SymuYellowPages.Actor);
                    break;
                case Murphy.IncompleteBelief:
                    ReplyHelpIncompleteBelief(task, blocker, message.Attachments.BeliefBits);
                    break;
            }
        }

        /// <summary>
        ///     Launch a recovery for all the blockers
        /// </summary>
        /// <param name="task"></param>
        private void TryRecoverBlockedTask(SymuTask task)
        {
            foreach (var blocker in task.Blockers.FilterBlockers(Schedule.Step))
            {
                blocker.Update(Schedule.Step);
                TryRecoverBlocker(task, blocker);
            }
        }

        /// <summary>
        ///     Launch a recovery for the blocker
        /// </summary>
        /// <param name="task"></param>
        /// <param name="blocker"></param>
        protected virtual void TryRecoverBlocker(SymuTask task, Blocker blocker)
        {
            if (blocker is null)
            {
                throw new ArgumentNullException(nameof(blocker));
            }

            switch (blocker.Type)
            {
                case Murphy.IncompleteKnowledge:
                    TryRecoverBlockerIncompleteKnowledge(task, blocker);
                    break;
                case Murphy.IncompleteBelief:
                    TryRecoverBlockerIncompleteBelief(task, blocker);
                    break;
            }
        }

        /// <summary>
        ///     Check if there are  blockers today on the task
        /// </summary>
        /// <param name="task"></param>
        private void CheckBlockers(SymuTask task)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (!Environment.Organization.Murphies.MultipleBlockers && task.IsBlocked)
                // One blocker at a time
            {
                return;
            }

            CheckNewBlockers(task);
        }

        /// <summary>
        ///     Launch a recovery for the blocker
        /// </summary>
        /// <param name="task"></param>
        public virtual void CheckNewBlockers(SymuTask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (task.Parent is Message)
            {
                return;
            }

            CheckBlockerIncompleteKnowledge(task);
        }

        /// <summary>
        ///     Impact of blockers on the remaining capacity
        ///     Blockers may create idle time
        /// </summary>
        public virtual void ImpactOfBlockersOnCapacity()
        {
        }

        #endregion

        #region Incomplete Knowledge

        /// <summary>
        ///     Check Task.KnowledgesBits against WorkerAgent.expertise
        ///     If Has expertise Task will be complete
        ///     If has expertise for mandatoryKnowledgesBits but not for requiredKnowledgesBits, he will guess, and possibly
        ///     complete the task incorrectly
        ///     If hasn't expertise for mandatoryKnowledgesBits && for requiredKnowledgesBits, he will ask for help (co workers,
        ///     internet forum, ...) && learn
        /// </summary>
        /// <param name="task"></param>
        public void CheckBlockerIncompleteKnowledge(SymuTask task)
        {
            if (!Environment.Organization.Murphies.IncompleteKnowledge.On)
            {
                return;
            }

            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            foreach (var knowledgeId in task.KnowledgesBits.KnowledgeIds)
            {
                CheckBlockerIncompleteKnowledge(task, knowledgeId);
            }
        }

        /// <summary>
        ///     Check Task.KnowledgesBits for a specific KnowledgeBit against WorkerAgent.expertise
        ///     If Has expertise Task will be complete
        ///     If has expertise for mandatoryKnowledgesBits but not for requiredKnowledgesBits, he will guess, and possibly
        ///     complete the task incorrectly
        ///     If hasn't expertise for mandatoryKnowledgesBits && for requiredKnowledgesBits, he will ask for help (co workers,
        ///     internet forum, ...) && learn
        /// </summary>
        /// <param name="task"></param>
        /// <param name="knowledgeId"></param>
        protected virtual void CheckBlockerIncompleteKnowledge(SymuTask task, ushort knowledgeId)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            var taskBits = task.KnowledgesBits.GetBits(knowledgeId);
            // If taskBits.Mandatory.Any => mandatoryCheck is false unless workerKnowledge has the good knowledge or there is no mandatory knowledge
            var mandatoryOk = taskBits.GetMandatory().Length == 0;
            // If taskBits.Required.Any => RequiredCheck is false unless workerKnowledge has the good knowledge or there is no required knowledge
            var requiredOk = taskBits.GetRequired().Length == 0;
            byte mandatoryIndex = 0;
            byte requiredIndex = 0;
            Environment.Organization.Murphies.IncompleteKnowledge.CheckKnowledge(knowledgeId, taskBits, KnowledgeModel.Expertise, ref mandatoryOk, ref requiredOk,
                ref mandatoryIndex, ref requiredIndex, Schedule.Step);
            if (!mandatoryOk)
            {
                // mandatoryCheck is false => Task is blocked
                var blocker = task.Blockers.Add(Murphy.IncompleteKnowledge, Schedule.Step, knowledgeId, mandatoryIndex);
                TryRecoverBlockerIncompleteKnowledge(task, blocker);
            }
            else if (!requiredOk)
            {
                RecoverBlockerKnowledgeByDoing(task, null, knowledgeId, requiredIndex, BlockerResolution.Guessing);
            }
        }

        /// <summary>
        ///     Missing knowledge is guessed or searched in agent's databases
        ///     The worker possibly complete the task incorrectly
        ///     and learn by doing
        /// </summary>
        /// <param name="task"></param>
        /// <param name="knowledgeId"></param>
        /// <param name="knowledgeBit"></param>
        /// <param name="blocker"></param>
        /// <param name="resolution">guessing or searched</param>
        public void RecoverBlockerKnowledgeByDoing(SymuTask task, Blocker blocker, ushort knowledgeId,
            byte knowledgeBit, BlockerResolution resolution)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            var impact = Environment.Organization.Murphies.IncompleteKnowledge.NextGuess();
            if (impact > task.Incorrect)
            {
                task.Incorrect = impact;
            }

            task.Weight *= Cognitive.TasksAndPerformance.CostFactorOfLearningByDoing;
            LearningModel.LearnByDoing(knowledgeId, knowledgeBit,
                Schedule.Step);
            switch (blocker)
            {
                // No blocker, it's a required knowledgeBit
                case null:
                    task.KnowledgesBits.RemoveFirstRequired(knowledgeId);
                    // Blockers Management - no blocker has been created
                    // We create a fake one to follow the impact of the murphy
                    task.Blockers.SetBlockerInProgress();
                    break;
                // blocker, it's a mandatory knowledgeBit
                default:
                    task.KnowledgesBits.RemoveFirstMandatory(knowledgeId);
                    break;
            }

            task.Blockers.Recover(blocker, resolution, Schedule.Step);
        }

        /// <summary>
        ///     Try different strategy to unblocked the blocker from incomplete knowledge
        ///     Default strategies:
        ///         search in its databases the information
        ///         ask internally of the organization if other agents have the adequate knowledge
        ///         Do the by itself, with the risk of performing incorrectly the task
        ///         asking externally of the organization (to override)
        /// </summary>
        /// <param name="task"></param>
        /// <param name="blocker"></param>
        public virtual void TryRecoverBlockerIncompleteKnowledge(SymuTask task, Blocker blocker)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (blocker is null)
            {
                throw new ArgumentNullException(nameof(blocker));
            }

            var knowledgeId = (ushort) blocker.Parameter;
            var knowledgeBit = (byte) blocker.Parameter2;
            // Check if he has the right to receive knowledge from others agents
            if (!Cognitive.MessageContent.CanReceiveKnowledge)
            {
                RecoverBlockerKnowledgeByDoing(task, blocker, knowledgeId, knowledgeBit, BlockerResolution.Guessing);
                return;
            }
            // He has the right
            // first search in the databases of the actor
            if (HasEmail && Email.SearchKnowledge(knowledgeId, knowledgeBit,
                Cognitive.TasksAndPerformance.LearningRate))
            {
                RecoverBlockerKnowledgeByDoing(task, blocker, knowledgeId, knowledgeBit, BlockerResolution.Searching);
            }

            var teammates = GetAgentIdsForInteractions(InteractionStrategy.Knowledge).ToList();
            if (teammates.Any() &&
                Environment.Organization.Murphies.IncompleteKnowledge.AskInternally(Schedule.Step, blocker.InitialStep))
            {
                var attachments = new MessageAttachments();
                attachments.Add(blocker);
                attachments.Add(task);
                attachments.KnowledgeId = knowledgeId;
                attachments.KnowledgeBit = knowledgeBit;
                var messageType =
                    Environment.Organization.Murphies.IncompleteKnowledge.AskOnWhichChannel(Cognitive.InteractionCharacteristics
                        .PreferredCommunicationMediums);
                ImpactOfTheCommunicationMediumOnTimeSpent(messageType, true, task.KeyActivity);
                SendToMany(teammates, MessageAction.Ask, SymuYellowPages.Help, attachments, messageType);
            }
            else if (!Environment.Organization.Murphies.IncompleteKnowledge.ShouldGuess(blocker.NumberOfTries))
            {
                TryRecoverBlockerIncompleteKnowledgeExternally(task, blocker, knowledgeId, knowledgeBit);
            }
            else
            {
                // blocker must be unblocked in a way or another
                RecoverBlockerKnowledgeByDoing(task, blocker, knowledgeId, knowledgeBit, BlockerResolution.Guessing);
            }
        }

        /// <summary>
        ///     Try different strategy to unblocked the blocker from incomplete knowledge
        ///     Other strategies failed. Asking externally of the organization (to override)
        ///     Don't forget to call RecoverBlockerKnowledgeByDoing as the last chance to unblock the task
        /// </summary>
        /// <param name="task"></param>
        /// <param name="blocker"></param>
        /// <param name="knowledgeId"></param>
        /// <param name="knowledgeBit"></param>
        public virtual void TryRecoverBlockerIncompleteKnowledgeExternally(SymuTask task, Blocker blocker, ushort knowledgeId,
            byte knowledgeBit)
        {
            RecoverBlockerKnowledgeByDoing(task, blocker, knowledgeId, knowledgeBit, BlockerResolution.Guessing);
        }

        public void ReplyHelpIncompleteKnowledge(SymuTask task, Blocker blocker, Bits knowledgeBits,
            CommunicationMediums medium, bool internalHelp)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (blocker is null)
            {
                throw new ArgumentNullException(nameof(blocker));
            }

            if (knowledgeBits is null)
            {
                return;
            }

            task.KnowledgesBits.RemoveFirstMandatory((ushort)blocker.Parameter);
            task.Blockers.Recover(blocker, internalHelp ? BlockerResolution.Internal : BlockerResolution.External, Schedule.Step);

            // Take some time to learn, allocate this time on KeyActivity
            ImpactOfTheCommunicationMediumOnTimeSpent(medium, false, task.KeyActivity);
        }
        /// <summary>
        ///     An agent have ask help on the knowledgeId, and the bit knowledgeBit
        /// </summary>
        /// <param name="message"></param>
        public void AskHelpIncompleteKnowledge(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var isReplayingIn = Environment.Organization.Murphies.IncompleteKnowledge.DelayToReplyToHelp();
            // If isReplayingIn = -1 , no reply
            if (isReplayingIn == -1)
            {
                return;
            }

            var replyMessage = Message.ReplyMessage(message);
            if (isReplayingIn == 0)
            {
                // Receive the question
                if (replyMessage.Attachments.Second is SymuTask task)
                {
                    ImpactOfTheCommunicationMediumOnTimeSpent(message.Medium, false, task.KeyActivity);
                    // Send the answer
                    ImpactOfTheCommunicationMediumOnTimeSpent(message.Medium, true, task.KeyActivity);
                }

                Reply(replyMessage);
            }
            else
            {
                // TODO reply should take time and impact the day he will reply his initial or remaining capacity, and is part of the multitasking
                ReplyDelayed(replyMessage, (ushort)(Environment.Schedule.Step + isReplayingIn));
            }
        }
        #endregion

        #region Incomplete beliefs

        /// <summary>
        ///     Check Task.BeliefBits against Agent.Beliefs
        ///     Prevent the agent from acting on a particular belief
        ///     Task may be blocked if it is the case
        /// </summary>
        public void CheckBlockerBeliefs(SymuTask task)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (task.Parent is Message)
            {
                return;
            }

            if (!Environment.Organization.Murphies.IncompleteBelief.On)
            {
                return;
            }

            foreach (var knowledgeId in task.KnowledgesBits.KnowledgeIds)
            {
                CheckBlockerBelief(task, knowledgeId);
            }
        }

        /// <summary>
        ///     Check a particular beliefId from Task.BeliefBits against Agent.Beliefs
        ///     Prevent the agent from acting on a particular belief
        ///     Task may be blocked if it is the case
        /// </summary>
        public void CheckBlockerBelief(SymuTask task, ushort knowledgeId)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            var taskBits = task.KnowledgesBits.GetBits(knowledgeId);
            float mandatoryScore = 0;
            float requiredScore = 0;
            byte mandatoryIndex = 0;
            byte requiredIndex = 0;

            var belief = Environment.WhitePages.Network.NetworkBeliefs.GetBelief(knowledgeId);
            Environment.Organization.Murphies.IncompleteBelief.CheckBelief(belief, taskBits, BeliefsModel.Beliefs, ref mandatoryScore, ref requiredScore,
                ref mandatoryIndex, ref requiredIndex);
            CheckBlockerBelief(task, knowledgeId, mandatoryScore, requiredScore, mandatoryIndex, requiredIndex);
        }

        protected virtual void CheckBlockerBelief(SymuTask task, ushort knowledgeId, float mandatoryScore,
            float requiredScore, byte mandatoryIndex, byte requiredIndex)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (!(mandatoryScore <= -Cognitive.InternalCharacteristics.RiskAversionThreshold))
            {
                return;
            }

            // Prevent the agent from acting on a particular belief
            // mandatoryScore is not enough => agent don't want to do the task, the task is blocked
            var blocker = task.Blockers.Add(Murphy.IncompleteBelief, Schedule.Step, knowledgeId, mandatoryIndex);
            TryRecoverBlockerIncompleteBelief(task, blocker);
        }

        /// <summary>
        ///     Ask Help to teammates about it belief
        ///     when task is blocked because of a lack of belief
        /// </summary>
        /// <param name="task"></param>
        /// <param name="blocker"></param>
        public virtual void TryRecoverBlockerIncompleteBelief(SymuTask task, Blocker blocker)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (blocker is null)
            {
                throw new ArgumentNullException(nameof(blocker));
            }

            // Check if he has the right to receive knowledge from others agents
            if (!Cognitive.MessageContent.CanReceiveBeliefs)
            {
                // If agent has no other strategy 
                // Blocker must be unblocked in a way or another
                RecoverBlockerBeliefByGuessing(task, blocker);
            }
        }

        /// <summary>
        ///     Missing belief is guessed
        ///     The worker possibly complete the task incorrectly
        ///     and learn by doing
        /// </summary>
        /// <param name="task"></param>
        /// <param name="blocker"></param>
        public void RecoverBlockerBeliefByGuessing(SymuTask task, Blocker blocker)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (blocker == null)
            {
                throw new ArgumentNullException(nameof(blocker));
            }

            var beliefId = (ushort) blocker.Parameter;
            var beliefBit = (byte) blocker.Parameter2;

            var impact = Environment.Organization.Murphies.IncompleteBelief.NextGuess();
            if (impact > task.Incorrect)
            {
                task.Incorrect = impact;
            }

            task.Weight += Environment.Organization.Murphies.IncompleteBelief.NextImpactOnTimeSpent();
            InfluenceModel.ReinforcementByDoing(beliefId, beliefBit, Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel);
            task.Blockers.Recover(blocker, BlockerResolution.Guessing, Schedule.Step);
        }


        public void ReplyHelpIncompleteBelief(SymuTask task, Blocker blocker, Bits beliefBits)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (beliefBits is null)
            {
                return;
            }
            task.Blockers.Recover(blocker, BlockerResolution.Internal, Schedule.Step);
        }
        /// <summary>
        ///     An agent have ask help on the beliefId, and the bit beliefBit
        /// </summary>
        /// <param name="message"></param>
        public void AskHelpIncompleteBelief(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var isReplayingIn = Environment.Organization.Murphies.IncompleteBelief.DelayToReplyToHelp();
            // If isReplayingIn = -1 , no reply
            if (isReplayingIn == -1)
            {
                return;
            }

            var replyMessage = Message.ReplyMessage(message);
            if (isReplayingIn == 0)
            {
                // Receive the question
                if (replyMessage.Attachments.Second is SymuTask task)
                {
                    ImpactOfTheCommunicationMediumOnTimeSpent(message.Medium, false, task.KeyActivity);
                    // Send the answer
                    ImpactOfTheCommunicationMediumOnTimeSpent(message.Medium, true, task.KeyActivity);
                }

                Reply(replyMessage);
            }
            else
            {
                // TODO reply should take time and impact the day he will reply his initial or remaining capacity, and is part of the multitasking
                ReplyDelayed(replyMessage, (ushort)(Environment.Schedule.Step + isReplayingIn));
            }
        }
        #endregion
    }
}