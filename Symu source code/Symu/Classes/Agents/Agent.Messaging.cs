#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using Symu.Classes.Task.Manager;
using Symu.Common;
using Symu.Messaging.Manager;
using Symu.Messaging.Messages;
using Symu.Tools;
using Symu.Tools.Math.ProbabilityDistributions;

#endregion

namespace Symu.Classes.Agents
{
    /// <summary>
    ///     An abstract base class for agents.
    ///     You must define your own agent derived classes derived
    /// </summary>
    public abstract partial class Agent
    {
        /// <summary>
        ///     Messaging of the agent
        /// </summary>
        public MessageProcessor MessageProcessor { get; set; }

        /// <summary>
        ///     Tasks manager for the agent
        ///     Null if !Cognitive.TasksAndPerformance.CanPerformTask
        /// </summary>
        public TaskProcessor TaskProcessor { get; private set; }

        /// <summary>
        ///     Communication medium used by the agent for the next message based on its
        ///     Cognitive.InteractionCharacteristics.PreferredCommunicationMediums
        /// </summary>
        public CommunicationMediums NextMedium => CommunicationMediumsModel.NextMedium(Cognitive
            .InteractionCharacteristics
            .PreferredCommunicationMediums);

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

        private void TrackMissedMessages(Message message)
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
        public void PostMessage(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            switch (message.Medium)
            {
                case CommunicationMediums.System:
                    MessageProcessor.Post(message);
                    break;
                default:
                    // The agent may have received too much messages for the step
                    if (IsMessagesPerPeriodBelowLimit(message.Medium) &&
                        IsMessagesReceivedPerPeriodBelowLimit(message.Medium))
                    {
                        if (AcceptNewInteraction(message.Sender))
                        {
                            OnBeforePostMessage(message);
                            MessageProcessor.Post(message);
                            OnAfterPostMessage(message);
                        }
                        else
                        {
                            Environment.Messages.Result.NotAcceptedMessagesCount++;
                            MessageProcessor.AddNotAcceptedMessages(message, Environment.Debug);
                        }
                    }
                    else
                    {
                        TrackMissedMessages(message);
                    }

                    break;
            }
        }

        /// <summary>
        ///     The message may be accepted or not depending if it's in its interaction sphere :
        ///     Or does he accept a new interaction :
        /// </summary>
        /// <param name="senderId"></param>
        /// <returns>True if the new interaction has been accepted</returns>
        public bool AcceptNewInteraction(AgentId senderId)
        {
            if (Id.Equals(senderId))
            {
                // for unit test
                return true;
            }

            var sender = Environment.WhitePages.GetAgent(senderId);
            // sender may be stopped since he accept this new interaction
            if (!Cognitive.InteractionPatterns.IsPartOfInteractionSphere ||
                !sender.Cognitive.InteractionPatterns.IsPartOfInteractionSphere)
            {
                return true;
            }

            if (Environment.WhitePages.Network.NetworkLinks.HasActiveLink(Id, senderId))
            {
                return true;
            }

            if (!Cognitive.InteractionPatterns.AllowNewInteractions)
            {
                return false;
            }

            if (Cognitive.InteractionPatterns.LimitNumberOfNewInteractions && _newInteractionCounter >=
                Cognitive.InteractionPatterns.MaxNumberOfNewInteractions)
            {
                return false;
            }

            // Not in its sphere of interaction (with an active link)
            if (!Bernoulli.Sample(Cognitive.InteractionPatterns.ThresholdForNewInteraction))
            {
                return false;
            }

            _newInteractionCounter++;

            // Decide to positively answer to this new interaction
            if (Environment.Organization.Models.InteractionSphere.SphereUpdateOverTime)
            {
                // Message.Sender is now part of agent interaction sphere
                Environment.WhitePages.Network.NetworkLinks.AddLink(Id, senderId);
            }

            return true;
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
                Environment.Organization.Communication.TimeSpent(message.Medium, true,
                    Environment.Organization.Models.RandomLevelValue);
            Capacity.Decrement(cost);
            Environment.Messages.TrackMessageSent(message, cost);
        }

        /// <summary>
        ///     Triggered after message post in the mailbox
        /// </summary>
        /// <remarks>Impact of the cost of the message on TimeSpent is done in Agent.TaskManagement</remarks>
        /// <remarks>Impact of the cost of the message on Capacity and Message result is done when message is converted into a task</remarks>
        public virtual void OnAfterPostMessage(Message message)
        {
            if (message is null || message.Medium == CommunicationMediums.System || !message.HasAttachments)
            {
                return;
            }

            LearnKnowledgesFromPostMessage(message);
            LearnBeliefsFromPostMessage(message);
        }

        /// <summary>
        ///     Learning Knowledge from posted message
        ///     Agent learn about knowledgeId from other agent with KnowledgeBits
        ///     Number of KnowledgeBits is defined by Cognitive.MessageContent.MaximumBitsOfKnowledge
        ///     If Agent must learn only the blocked KnowledgeBit set MaximumBitsOfKnowledge to 1
        /// </summary>
        /// <param name="message"></param>
        public void LearnKnowledgesFromPostMessage(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message.Medium == CommunicationMediums.System || !Cognitive.MessageContent.CanReceiveKnowledge ||
                message.Attachments.KnowledgeBits is null)
            {
                return;
            }

            var communication =
                Environment.Organization.Communication.TemplateFromChannel(message.Medium);
            LearningModel.Learn(message.Attachments.KnowledgeId,
                message.Attachments.KnowledgeBits, communication.MaxRateLearnable, Schedule.Step);
            if (message.Medium == CommunicationMediums.Email && HasEmail)
            {
                Email.StoreKnowledge(message.Attachments.KnowledgeId, message.Attachments.KnowledgeBits,
                    communication.MaxRateLearnable, Schedule.Step);
            }
        }

        /// <summary>
        ///     Learning Beliefs from posted message
        ///     Agent get other agent's beliefs with beliefBits
        ///     Number of beliefBits is defined by Cognitive.MessageContent.MaximumBitsOfBelief
        ///     Depending on influentialness and influenceability, agent can change its belief or not
        /// </summary>
        /// <param name="message"></param>
        public void LearnBeliefsFromPostMessage(Message message)
        {
            if (message is null || !message.HasAttachments)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message.Medium == CommunicationMediums.System || !Cognitive.MessageContent.CanReceiveBeliefs)
            {
                return;
            }

            InfluenceModel.BeInfluenced(message.Attachments.KnowledgeId, message.Attachments.BeliefBits,
                message.Sender, Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel);
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
        public void Send(AgentId receiverId, MessageAction action, byte content)
        {
            var message = new Message(Id, receiverId, action, content);
            Send(message);
        }

        public void Send(AgentId receiverId, MessageAction action, byte content, object parameter)
        {
            var message = new Message(Id, receiverId, action, content, parameter);
            Send(message);
        }

        public void Send(AgentId receiverId, MessageAction action, byte content, CommunicationMediums mediums)
        {
            var message = new Message(Id, receiverId, action, content, mediums);
            Send(message);
        }

        public void Send(AgentId receiverId, MessageAction action, byte content, MessageAttachments parameter)
        {
            var message = new Message(Id, receiverId, action, content, parameter);
            Send(message);
        }

        public void Send(AgentId receiverId, MessageAction action, byte content, MessageAttachments parameter,
            CommunicationMediums communicationMedium)
        {
            var message = new Message(Id, receiverId, action, content, parameter, communicationMedium);
            Send(message);
        }

        /// <summary>
        ///     Send a message to another agent define by the message.Receiver
        ///     It count in the Mailbox.NumberMessagesPerStep
        ///     It will be effectively sent only if IsMessages is above Limits
        /// </summary>
        /// <param name="message"></param>
        public void Send(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            switch (message.Medium)
            {
                case CommunicationMediums.System:
                    Environment.SendAgent(message);
                    break;
                default:
                {
                    if (Status == AgentStatus.Offline ||
                        !IsMessagesPerPeriodBelowLimit(message.Medium) ||
                        !IsMessagesSendPerPeriodBelowLimit(message.Medium))
                    {
                        return;
                    }

                    OnBeforeSendMessage(message);
                    Environment.SendAgent(message);
                    OnAfterSendMessage(message);
                    break;
                }
            }
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

            var receiver = Environment.WhitePages.GetAgent(message.Receiver);
            if (receiver == null)
            {
                // receiver is already stopped
                return;
            }

            switch (receiver.State)
            {
                case AgentState.Stopping:
                    return;
                case AgentState.Starting:
                case AgentState.Started:
                    SendDelayed(message, step);
                    break;
            }
        }

        private void SendDelayed(Message message, ushort step)
        {
            Environment.SendDelayedMessage(message, step);
        }

        public void SendToMany(IEnumerable<AgentId> receivers, MessageAction action, byte content)
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

        public void SendToMany(IEnumerable<AgentId> receivers, MessageAction action, byte content, object parameter)
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

        public void SendToMany(IEnumerable<AgentId> receivers, MessageAction action, byte content,
            MessageAttachments parameter)
        {
            SendToMany(receivers, action, content, parameter, CommunicationMediums.System);
        }

        public void SendToMany(IEnumerable<AgentId> receivers, MessageAction action, byte content,
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
        public void Reply(Message message, bool delayed, ushort delay)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!IsMessagesPerPeriodBelowLimit(message.Medium))
            {
                return;
            }

            if (message.HasAttachments && message.Medium != CommunicationMediums.System)
            {
                var ma = message.Attachments;
                var communication =
                    Environment.Organization.Communication.TemplateFromChannel(message.Medium);
                ma.KnowledgeBits = KnowledgeModel.FilterKnowledgeToSend(ma.KnowledgeId, ma.KnowledgeBit, communication,
                    Schedule.Step, out var knowledgeIndexToSend);
                ma.BeliefBits = BeliefsModel.FilterBeliefToSend(ma.KnowledgeId, ma.KnowledgeBit, communication);
                // The agent is asked for his knowledge, so he can't forget it
                if (ma.KnowledgeBits != null)
                {
                    ForgettingModel.UpdateForgettingProcess(ma.KnowledgeId, knowledgeIndexToSend);
                }
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
        public void Reply(AgentId receiverId, MessageAction action, byte content)
        {
            var message = new Message(Id, receiverId, action, content);
            Reply(message);
        }

        public void Reply(AgentId receiverId, MessageAction action, byte content, object parameter)
        {
            var message = new Message(Id, receiverId, action, content, parameter);
            Reply(message);
        }

        public void Reply(AgentId receiverId, MessageAction action, byte content, MessageAttachments parameter)
        {
            var message = new Message(Id, receiverId, action, content, parameter);
            Reply(message);
        }

        public void Reply(AgentId receiverId, MessageAction action, byte content, MessageAttachments parameter,
            CommunicationMediums communicationMedium)
        {
            var message = new Message(Id, receiverId, action, content, parameter, communicationMedium);
            Reply(message);
        }

        #endregion

        #region Limit MessagesManager par period

        /// <summary>
        ///     If Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod is set to true,
        ///     check that numberMessagesPerPeriod is below MaximumMessagesPerPeriod
        /// </summary>
        /// <returns>true if numberMessagesPerPeriod is below the maximum, a new message can be send or receive</returns>
        public bool IsMessagesPerPeriodBelowLimit(CommunicationMediums medium)
        {
            // System messages are not concerned by the limits
            if (medium == CommunicationMediums.System)
            {
                return true;
            }

            var noLimit = !Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod &&
                          MessageProcessor.NumberMessagesPerStep < ushort.MaxValue;
            var limit = Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod &&
                        MessageProcessor.NumberMessagesPerStep <
                        Cognitive.InteractionCharacteristics.MaximumMessagesPerPeriod;
            return limit | noLimit;
        }

        /// <summary>
        ///     If Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod is set to true,
        ///     check that NumberPostPerPeriod is below MaximumInitiationsPerPeriod
        /// </summary>
        /// <returns>true if numberMessagesPerPeriod is below the maximum, a new message can be send or receive</returns>
        public bool IsMessagesSendPerPeriodBelowLimit(CommunicationMediums medium)
        {
            // System messages are not concerned by the limits
            if (medium == CommunicationMediums.System)
            {
                return true;
            }

            var noLimit = !Cognitive.InteractionCharacteristics.LimitMessagesSentPerPeriod &&
                          MessageProcessor.NumberSentPerPeriod < byte.MaxValue;
            var limit = Cognitive.InteractionCharacteristics.LimitMessagesSentPerPeriod &&
                        MessageProcessor.NumberSentPerPeriod <
                        Cognitive.InteractionCharacteristics.MaximumMessagesSentPerPeriod;
            return limit | noLimit;
        }

        /// <summary>
        ///     If Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod is set to true,
        ///     check that NumberPostPerPeriod is below MaximumInitiationsPerPeriod
        /// </summary>
        /// <returns>true if numberMessagesPerPeriod is below the maximum, a new message can be send or receive</returns>
        public bool IsMessagesReceivedPerPeriodBelowLimit(CommunicationMediums medium)
        {
            // System messages are not concerned by the limits
            if (medium == CommunicationMediums.System)
            {
                return true;
            }

            var noLimit = !Cognitive.InteractionCharacteristics.LimitReceptionsPerPeriod &&
                          MessageProcessor.NumberReceivedPerPeriod < byte.MaxValue;
            var limit = Cognitive.InteractionCharacteristics.LimitReceptionsPerPeriod &&
                        MessageProcessor.NumberReceivedPerPeriod <
                        Cognitive.InteractionCharacteristics.MaximumReceptionsPerPeriod;
            return limit | noLimit;
        }

        #endregion
    }
}