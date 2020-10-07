#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Messaging.Messages;
using Symu.Repository;

#endregion

namespace Symu.Classes.Agents
{
    /// <summary>
    ///     The default implementation of IAgent
    ///     You can define your own class agent by inheritance or implementing directly IAgent
    ///     This partial class focus on Act methods
    /// </summary>
    public partial class ReactiveAgent
    {
        /// <summary>
        ///     This is the method that is called when the agent receives a message and is activated.
        ///     When Schedule.Type is Intraday, messages are treated as tasks and stored in task.Parent attribute
        /// </summary>
        /// <param name="message">The message that the agent has received and should respond to</param>
        public virtual void Act(Message message)
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

            ActMessage(message);
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
                    Stop();
                    break;
                case SymuYellowPages.Subscribe:
                    ActSubscribe(message);
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
            HandleStatus(false);
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
            if (Status == AgentStatus.Offline)
            {
                return;
            }

            Status = AgentStatus.Busy;
        }

        /// <summary>
        ///     Start a weekend, by asking new tasks if agent perform tasks on weekends
        /// </summary>
        public virtual void ActWeekEnd()
        {
            // update ActWorkingDay to have the same behaviour
            if (Status == AgentStatus.Offline)
            {
                return;
            }

            Status = AgentStatus.Busy;
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
        ///     Clone the Status to available if agent as InitialCapacity, Offline otherwise
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
    }
}