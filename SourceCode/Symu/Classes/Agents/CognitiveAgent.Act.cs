#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Symu.Classes.Task;
using Symu.Common.Classes;
using Symu.Messaging.Messages;
using Symu.Repository;

#endregion

namespace Symu.Classes.Agents
{
    /// <summary>
    ///     An abstract base class for agents.
    ///     You must define your own agent derived classes derived
    ///     This partial class focus on Act methods
    /// </summary>
    public abstract partial class CognitiveAgent
    {
        /// <summary>
        ///     This is the method that is called when the agent receives a message and is activated.
        ///     When Schedule.Type is Intraday, messages are treated as tasks and stored in task.Parent attribute
        /// </summary>
        /// <param name="message">The message that the agent has received and should respond to</param>
        public override void Act(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            // agent ask Environment to be in a SplitStep mode
            // message is managed directly 
            if (message.Subject == SymuYellowPages.SplitStep)
            {
                var splitStep = message.Attachments.First as SplitStep;
                splitStep?.Step();
                return;
            }

            if (Cognitive.TasksAndPerformance.CanPerformTask && message.Medium != CommunicationMediums.System)
            {
                var task = ConvertMessageIntoTask(message);
                Post(task);
            }
            else
            {
                ActMessage(message);
            }
        }

        /// <summary>
        ///     Convert message into a task to be perform in the task manager
        ///     MessageResult.ReceivedMessagesCost is also updated
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private SymuTask ConvertMessageIntoTask(Message message)
        {
            var communication =
                Environment.MainOrganization.Communication.TemplateFromChannel(message.Medium);
            var task = new SymuTask(Schedule.Step)
            {
                Type = message.Medium.ToString(),
                TimeToLive = communication.TimeToLive,
                Parent = message,
                Weight = Environment.MainOrganization.Communication.TimeSpent(message.Medium, false,
                    Environment.RandomLevelValue),
                Assigned = message.Receiver
                //todo maybe define a specific KeyActivity to follow the time spent on messaging?
            };

            Environment.Messages.Result.ReceivedMessagesCost += task.Weight;
            return task;
        }

        /// <summary>
        ///     This is where the main logic of the agent should be placed.
        /// </summary>
        /// <param name="message"></param>
        public override void ActMessage(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            base.ActMessage(message);

            switch (message.Subject)
            {
                case SymuYellowPages.Help:
                    ActHelp(message);
                    break;
            }
        }

        /// <summary>
        ///     Trigger every event before the new step
        ///     Do not send messages, use NextStep for that
        /// </summary>
        public override async void PreStep()
        {
            MessageProcessor?.ClearMessagesPerPeriod();
            ForgettingModel?.InitializeForgettingProcess();

            // Databases
            if (HasEmail)
            {
                Email.ForgettingProcess(Schedule.Step);
            }

            _newInteractionCounter = 0;

            var isolated = Cognitive.InteractionPatterns.IsIsolated(Schedule.Step);
            HandleStatus(isolated);
            // intentionally after Status
            HandleCapacity(isolated, true);
            // Task manager
            if (!Cognitive.TasksAndPerformance.CanPerformTask)
            {
                return;
            }

            async Task<bool> ProcessWorkInProgress()
            {
                while (Capacity.HasCapacity && Status != AgentStatus.Offline)
                {
                    try
                    {
                        var task = await TaskProcessor.Receive(Schedule.Step).ConfigureAwait(false);
                        switch (task.Parent)
                        {
                            case Message message:
                                // When Schedule.Type is Intraday, messages are treated as tasks and stored in task.Parent attribute
                                // Once a message (as a task) is receive it is treated as a message
                                if (!task.IsStarted)
                                {
                                    ActMessage(message);
                                }

                                WorkOnTask(task);
                                break;
                            default:
                                WorkInProgress(task);
                                break;
                        }
                    }
                    catch (Exception exception)
                    {
                        var exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                        exceptionDispatchInfo.Throw();
                    }
                }

                // If we didn't deschedule then run the continuation immediately
                return true;
            }

            await ProcessWorkInProgress().ConfigureAwait(false);

            if (Schedule.Type <= TimeStepType.Daily)
            {
                ActEndOfDay();
            }
        }

        /// <summary>
        ///     Trigger every event after the actual step,
        ///     Do not send messages
        /// </summary>
        public override void PostStep()
        {
            base.PostStep();
            ForgettingModel?.FinalizeForgettingProcess(Schedule.Step);
        }

        /// <summary>
        ///     Trigger at the end of day,
        ///     agent can still send message
        /// </summary>
        public virtual void ActEndOfDay()
        {
            SendNewInteractions();
            TaskProcessor?.TasksManager.TasksCheck(Schedule.Step);
        }

        /// <summary>
        ///     Send new interactions to augment its sphere of interaction if possible
        ///     Depends on Cognitive.InteractionPatterns && Cognitive.InteractionCharacteristics
        /// </summary>
        public void SendNewInteractions()
        {
            var agents = GetAgentIdsForNewInteractions().ToList();
            if (!agents.Any())
            {
                return;
            }

            // Send new interactions
            SendToMany(agents, MessageAction.Ask, SymuYellowPages.Actor, null, NextMedium);
        }

        /// <summary>
        ///     Start the working day, by asking new tasks
        /// </summary>
        public override void ActWorkingDay()
        {
            base.ActWorkingDay();
            // update ActWeekEnd to have the same behaviour
            if (!Cognitive.TasksAndPerformance.CanPerformTask
                || TaskProcessor.TasksManager.HasReachedTotalMaximumLimit)
            {
                return;
            }

            ImpactOfBlockersOnCapacity();
            GetNewTasks();
        }

        /// <summary>
        ///     Start a weekend, by asking new tasks if agent perform tasks on weekends
        /// </summary>
        public override void ActWeekEnd()
        {
            // update ActWorkingDay to have the same behaviour
            if (!Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds
                || TaskProcessor.TasksManager.HasReachedTotalMaximumLimit)
            {
                return;
            }

            ImpactOfBlockersOnCapacity();
            GetNewTasks();
        }

        /// <summary>
        ///     Check if agent is performing task today depending on its settings or if agent is active
        /// </summary>
        /// <returns>true if agent is performing task, false if agent is not</returns>
        public bool IsPerformingTask(bool isolated)
        {
            // Agent can be temporary isolated
            return !isolated && (Cognitive.TasksAndPerformance.CanPerformTask && Schedule.IsWorkingDay ||
                                 Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds &&
                                 !Schedule.IsWorkingDay);
        }
    }
}