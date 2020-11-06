#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using Symu.Common;
using Symu.Common.Classes;
using Symu.Common.Interfaces;
using Symu.Messaging.Manager;
using Symu.Messaging.Messages;

#endregion

namespace Symu.Classes.Agents
{
    /// The default implementation of IAgent
    /// You can define your own class agent by inheritance or implementing directly IAgent
    /// This partial class focus on messaging methods
    public partial class ReactiveAgent
    {
        /// <summary>
        ///     Messaging of the agent
        /// </summary>
        public MessageProcessor MessageProcessor { get; set; }

        #region Post message

        public void Post(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (MessageProcessor is null)
            {
                throw new ArgumentNullException(nameof(MessageProcessor));
            }

            switch (message.Medium)
            {
                case CommunicationMediums.Irc:
                case CommunicationMediums.Email:
                case CommunicationMediums.ViaAPlatform:
                    if (Status == AgentStatus.Offline)
                    {
                        // If receiver is offline, the message is postponed until the next interaction
                        PostAsADelayedMessage(message, (ushort) (Schedule.Step + 1));
                    }
                    else
                    {
                        // The message is posted
                        PostMessage(message);
                    }

                    break;
                case CommunicationMediums.Phone:
                case CommunicationMediums.Meeting:
                case CommunicationMediums.FaceToFace:
                    if (Status == AgentStatus.Offline)
                    {
                        // message is Missed
                        TrackMissedMessages(message);
                    }
                    else
                    {
                        // The message is posted
                        PostMessage(message);
                    }

                    break;
                //case MessageType.System:
                default:
                    switch (State)
                    {
                        case AgentState.NotStarted:
                        case AgentState.Starting:
                            // If receiver is offline, the message is postponed until the next interaction
                            PostAsADelayedMessage(message, (ushort) (Schedule.Step + 1));
                            break;
                        case AgentState.Stopped:
                            break;
                        default:
                            PostMessage(message);
                            break;
                    }

                    break;
            }
        }

        protected void TrackMissedMessages(Message message)
        {
            MessageProcessor.AddMissedMessage(message, Environment.Debug);
            Environment.Messages.Result.MissedMessagesCount++;
        }

        /// <summary>
        ///     Message post is postponed until another step, because agent were offline
        /// </summary>
        /// <param name="message">the message to delay</param>
        /// <param name="step">the step to which the message will be post</param>
        public void PostAsADelayedMessage(Message message, ushort step)
        {
            MessageProcessor.PostAsADelayed(message, step);
        }

        /// <summary>
        ///     Message is post this step
        /// </summary>
        /// <param name="message"></param>
        public virtual void PostMessage(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            OnBeforePostMessage(message);
            MessageProcessor.Post(message);
            OnAfterPostMessage(message);
        }

        /// <summary>
        ///     Triggered before post message in the MessageManager
        /// </summary>
        /// <param name="message"></param>
        public virtual void OnBeforePostMessage(Message message)
        {
        }

        /// <summary>
        ///     Triggered before message send in the mailbox
        ///     Use to track message (non system)
        ///     and impact the cost of the message on the agent capacity and TimeSpent
        /// </summary>
        /// <param name="message"></param>
        /// <remarks>Impact on TimeSpent is done in Agent.TaskManagement</remarks>
        public virtual void OnBeforeSendMessage(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            MessageProcessor.IncrementMessagesPerPeriod(message.Medium, true);
            // Impact of the Communication channels on the remaining capacity
            var cost =
                Environment.MainOrganization.Communication.TimeSpent(message.Medium, true,
                    Environment.RandomLevelValue);
            Environment.Messages.TrackMessageSent(message, cost);
        }

        /// <summary>
        ///     Triggered after message post in the mailbox
        /// </summary>
        /// <remarks>Impact of the cost of the message on TimeSpent is done in Agent.TaskManagement</remarks>
        /// <remarks>Impact of the cost of the message on Capacity and Message result is done when message is converted into a task</remarks>
        public virtual void OnAfterPostMessage(Message message)
        {
        }

        public void MessageOnBeforePost(object sender, MessageEventArgs e)
        {
            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            Environment.Messages.EnQueueWaitingMessage(e.Message, Schedule.Step);
        }

        /// <summary>
        ///     Post all delayed messages of this current interaction step
        /// </summary>
        public void PostDelayedMessages()
        {
            while (MessageProcessor.NextDelayedMessages(Schedule.Step) is Message message)
            {
                PostMessage(message);
            }
        }

        #endregion

        #region Send messages

        /// <summary>
        ///     Sends a message to a specific agent, identified by name.
        /// </summary>
        /// <param name="receiverId">The agent that will receive the message</param>
        /// <param name="content">The content of the message</param>
        /// <param name="action">
        ///     A conversation identifier, for the cases when a conversation involves multiple messages
        ///     that refer to the same topic
        /// </param>
        public void Send(IAgentId receiverId, MessageAction action, byte content)
        {
            var message = new Message(AgentId, receiverId, action, content);
            Send(message);
        }

        public void Send(IAgentId receiverId, MessageAction action, byte content, object parameter)
        {
            var message = new Message(AgentId, receiverId, action, content, parameter);
            Send(message);
        }

        public void Send(IAgentId receiverId, MessageAction action, byte content, CommunicationMediums mediums)
        {
            var message = new Message(AgentId, receiverId, action, content, mediums);
            Send(message);
        }

        public void Send(IAgentId receiverId, MessageAction action, byte content, MessageAttachments parameter)
        {
            var message = new Message(AgentId, receiverId, action, content, parameter);
            Send(message);
        }

        public void Send(IAgentId receiverId, MessageAction action, byte content, MessageAttachments parameter,
            CommunicationMediums communicationMedium)
        {
            var message = new Message(AgentId, receiverId, action, content, parameter, communicationMedium);
            Send(message);
        }

        /// <summary>
        ///     Send a message to another agent define by the message.Receiver
        ///     It count in the Mailbox.NumberMessagesPerStep
        ///     It will be effectively sent only if IsMessages is above Limits
        /// </summary>
        /// <param name="message"></param>
        public virtual void Send(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            OnBeforeSendMessage(message);
            Environment.SendAgent(message);
            OnAfterSendMessage(message);
        }

        /// <summary>
        ///     Triggered after the message is send
        /// </summary>
        /// <param name="message"></param>
        public virtual void OnAfterSendMessage(Message message)
        {
        }

        /// <summary>
        ///     Agent try send a message to another agent
        ///     if the receiver is stopping or stopped, the message is not sent
        ///     otherwise, the message is send to the next step
        /// </summary>
        /// <param name="message"></param>
        public void TrySendDelayed(Message message)
        {
            TrySendDelayed(message, Schedule.Step);
        }

        /// <summary>
        ///     Agent try send a message to another agent
        ///     if the receiver is stopping or stopped, the message is not sent
        ///     otherwise, the message is send with delay
        /// </summary>
        /// <param name="message"></param>
        /// <param name="step"></param>
        public void TrySendDelayed(Message message, ushort step)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var receiver = Environment.AgentNetwork.GetAgent(message.Receiver);
            if (receiver == null || receiver.State == AgentState.Stopping)
            {
                // receiver is already stopped
                return;
            }

            SendDelayed(message, step);
        }

        private void SendDelayed(Message message, ushort step)
        {
            Environment.SendDelayedMessage(message, step);
        }

        public void SendToMany(IEnumerable<IAgentId> receivers, MessageAction action, byte content)
        {
            if (receivers is null)
            {
                return;
            }

            foreach (var a in receivers.Shuffle())
            {
                Send(a, action, content);
            }
        }

        public void SendToMany(IEnumerable<IAgentId> receivers, MessageAction action, byte content, object parameter)
        {
            if (receivers is null)
            {
                return;
            }

            foreach (var a in receivers.Shuffle())
            {
                Send(a, action, content, parameter);
            }
        }

        public void SendToMany(IEnumerable<IAgentId> receivers, MessageAction action, byte content,
            MessageAttachments parameter)
        {
            SendToMany(receivers, action, content, parameter, CommunicationMediums.System);
        }

        public void SendToMany(IEnumerable<IAgentId> receivers, MessageAction action, byte content,
            MessageAttachments parameter, CommunicationMediums communicationMedium)
        {
            if (receivers is null)
            {
                return;
            }

            foreach (var a in receivers.Shuffle())
            {
                Send(a, action, content, parameter, communicationMedium);
            }
        }

        #endregion

        #region Reply message

        /// <summary>
        ///     Reply to a message from another agent
        ///     It does count in the Mailbox.NumberMessagesPerStep
        ///     It doesn't count in the Mailbox.NumberSentPerPeriod
        ///     It will be effectively sent only if IsMessages is above Limits
        /// </summary>
        /// <param name="message"></param>
        /// <param name="delayed"></param>
        /// <param name="delay"></param>
        public virtual void Reply(Message message, bool delayed, ushort delay)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            OnBeforeSendMessage(message);
            if (delayed)
            {
                SendDelayed(message, delay);
            }
            else
            {
                Environment.SendAgent(message);
            }

            OnAfterSendMessage(message);
        }

        /// <summary>
        ///     Reply to a message from another agent
        ///     It does count in the Mailbox.NumberMessagesPerStep
        ///     It doesn't count in the Mailbox.NumberSentPerPeriod
        ///     It will be effectively sent only if IsMessages is above Limits
        /// </summary>
        /// <param name="message"></param>
        public void Reply(Message message)
        {
            Reply(message, false, 0);
        }

        /// <summary>
        ///     Reply to a message from another agent
        ///     It does count in the Mailbox.NumberMessagesPerStep
        ///     It doesn't count in the Mailbox.NumberSentPerPeriod
        ///     It will be effectively sent only if IsMessages is above Limits
        /// </summary>
        /// <param name="message"></param>
        /// <param name="delay"></param>
        public void ReplyDelayed(Message message, ushort delay)
        {
            Reply(message, true, delay);
        }

        /// <summary>
        ///     Sends a message to a specific agent, identified by name.
        /// </summary>
        /// <param name="receiverId">The agent that will receive the message</param>
        /// <param name="content">The content of the message</param>
        /// <param name="action">
        ///     A conversation identifier, for the cases when a conversation involves multiple messages
        ///     that refer to the same topic
        /// </param>
        public void Reply(IAgentId receiverId, MessageAction action, byte content)
        {
            var message = new Message(AgentId, receiverId, action, content);
            Reply(message);
        }

        public void Reply(IAgentId receiverId, MessageAction action, byte content, object parameter)
        {
            var message = new Message(AgentId, receiverId, action, content, parameter);
            Reply(message);
        }

        public void Reply(IAgentId receiverId, MessageAction action, byte content, MessageAttachments parameter)
        {
            var message = new Message(AgentId, receiverId, action, content, parameter);
            Reply(message);
        }

        public void Reply(IAgentId receiverId, MessageAction action, byte content, MessageAttachments parameter,
            CommunicationMediums communicationMedium)
        {
            var message = new Message(AgentId, receiverId, action, content, parameter, communicationMedium);
            Reply(message);
        }

        #endregion
    }
}