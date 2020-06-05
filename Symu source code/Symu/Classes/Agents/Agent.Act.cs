#region Licence

// Description: Symu - Symu
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
using Symu.Common;
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
    public abstract partial class Agent
    {
        /// <summary>
        ///     This is the method that is called when the agent receives a message and is activated.
        ///     When Schedule.Type is Intraday, messages are treated as tasks and stored in task.Parent attribute
        /// </summary>
        /// <param name="message">The message that the agent has received and should respond to</param>
        public void Act(Message message)
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
                Environment.WhitePages.Network.NetworkCommunications.TemplateFromChannel(message.Medium);
            var task = new SymuTask(Schedule.Step)
            {
                Type = message.Medium.ToString(),
                TimeToLive = communication.TimeToLive,
                Parent = message,
                Weight = Environment.WhitePages.Network.NetworkCommunications.TimeSpent(message.Medium, false,
                    Environment.Organization.Models.RandomLevelValue)
            };

            Environment.Messages.Result.ReceivedMessagesCost += task.Weight;
            return task;
        }

        /// <summary>
        ///     This is where the main logic of the agent should be placed.
        /// </summary>
        /// <param name="message"></param>
        public virtual void ActMessage(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            switch (message.Subject)
            {
                case SymuYellowPages.Stop:
                    State = AgentState.Stopping;
                    break;
                case SymuYellowPages.Subscribe:
                    ActSubscribe(message);
                    break;
                case SymuYellowPages.Help:
                    ActHelp(message);
                    break;
                default:
                    ActClassKey(message);
                    break;
            }
        }

        /// <summary>
        ///     Trigger every event before the new step
        ///     Do not send messages, use NextStep for that
        /// </summary>
        public virtual async void PreStep()
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

            ActEndOfDay();
        }

        /// <summary>
        ///     Trigger event after the taskManager is started.
        ///     Used by the agent to subscribe to AfterSetTaskDone event
        /// </summary>
        /// <example>TaskManager.AfterSetTaskDone += AfterSetTaskDone;</example>
        public virtual void OnAfterTaskProcessorStart()
        {
        }

        /// <summary>
        ///     Trigger every event after the actual step,
        ///     Do not send messages
        /// </summary>
        public virtual void PostStep()
        {
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
            SendToMany(agents, MessageAction.Ask, SymuYellowPages.Actor, CommunicationMediums.FaceToFace);
        }

        public virtual void ActCadence()
        {
        }

        /// <summary>
        ///     Start the working day, by asking new tasks
        /// </summary>
        public virtual void ActWorkingDay()
        {
            // update ActWeekEnd to have the same behaviour
            if (!Cognitive.TasksAndPerformance.CanPerformTask
                || TaskProcessor.TasksManager.HasReachedTotalMaximumLimit
                || Status == AgentStatus.Offline)
            {
                return;
            }

            Status = AgentStatus.Busy;
            ImpactOfBlockersOnCapacity();
            GetNewTasks();
        }

        /// <summary>
        ///     Start a weekend, by asking new tasks if agent perform tasks on weekends
        /// </summary>
        public virtual void ActWeekEnd()
        {
            // update ActWorkingDay to have the same behaviour
            if (!Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds
                || TaskProcessor.TasksManager.HasReachedTotalMaximumLimit
                || Status == AgentStatus.Offline)
            {
                return;
            }

            ImpactOfBlockersOnCapacity();
            GetNewTasks();
        }

        /// <summary>
        ///     Event that occur on friday to end the work week
        /// </summary>
        public virtual void ActEndOfWeek()
        {
        }

        public virtual void ActEndOfMonth()
        {
        }

        public virtual void ActEndOfYear()
        {
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

        /// <summary>
        ///     CopyTo the Status to available if agent as InitialCapacity, Offline otherwise
        /// </summary>
        public virtual void HandleStatus(bool isolated)
        {
            Status = !isolated ? AgentStatus.Available : AgentStatus.Offline;
            if (Status != AgentStatus.Offline)
                // Open the agent mailbox with all the waiting messages
            {
                PostDelayedMessages();
            }
        }

        protected virtual void ActClassKey(Message message)
        {
        }
    }
}