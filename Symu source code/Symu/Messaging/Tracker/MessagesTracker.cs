#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using Symu.Classes.Agents;
using Symu.Messaging.Delayed;
using Symu.Messaging.Messages;
using Symu.Results.Blocker;
using Symu.Results.Messaging;

#endregion

namespace Symu.Messaging.Tracker
{
    /// <summary>
    ///     Use to manage timeBased interaction Step :
    ///     It is based on the flow of messages. When there is no more messages in the step, the step is over
    ///     Symu can trigger the next interaction step
    ///     It is also use to trace messages in the simulation, to debug
    /// </summary>
    public class MessagesTracker
    {
        /// <summary>
        ///     Give the count of the waiting messages
        /// </summary>
        private byte _waitingMessagesCount;

        /// <summary>
        ///     MessagesManager that are postponed to the next interaction step
        /// </summary>
        public DelayedMessages DelayedMessages { get; } = new DelayedMessages();

        public MessageResult Result { get; } = new MessageResult();

        public bool Debug { get; set; } = true;

        /// <summary>
        ///     Last messages sent during the last NumberOfMessages
        /// </summary>
        public TimeStampedMessages LastSentMessages { get; } = new TimeStampedMessages();

        /// <summary>
        ///     Lost messages sent
        /// </summary>
        public List<Message> LostMessages { get; } = new List<Message>();

        /// <summary>
        ///     MessagesManager sent but still not read by the receiver
        /// </summary>
        public List<Message> WaitingMessages { get; } = new List<Message>();

        /// <summary>
        ///     number of steps to retain the MessagesSent
        ///     to optimize performance memory usage
        ///     By default 7 = a week
        ///     NumberOfMessages = -1 => Don't clear messages / debug mode
        /// </summary>
        public sbyte NumberOfSteps { get; set; } = 7;

        /// <summary>
        ///     Check that there is no lost messages and no waiting messages
        /// </summary>
        /// <returns></returns>
        public bool CheckMessages => Result.LostMessagesCount == 0 && _waitingMessagesCount == 0;

        #region Lost MessagesManager

        /// <summary>
        ///     Lost message is a message whose Sender was not found
        /// </summary>
        /// <param name="message">The state of the message is changed to Lost</param>
        public void EnqueueLostMessage(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            message.State = MessageState.Lost;
            Result.LostMessagesCount++;
            if (Debug)
            {
                LostMessages.Add(message);
            }
        }

        #endregion

        #region LastSentMessages

        /// <summary>
        ///     Initialize all MessagesSent before stepNumber
        /// </summary>
        /// <param name="stepNumber"></param>
        public void ClearMessagesSent(ushort stepNumber)
        {
            if (NumberOfSteps == -1)
            {
                return;
            }

            LastSentMessages.ClearSteps(stepNumber - NumberOfSteps);
        }

        public List<Message> MessagesReceivedByAgent(ushort step, AgentId agentId)
        {
            return LastSentMessages.ReceivedByAgent(step, agentId);
        }

        public List<Message> MessagesSentByAgent(ushort step, AgentId agentId)
        {
            return LastSentMessages.SentByAgent(step, agentId);
        }

        /// <summary>
        ///     Initialize All MessagesSent
        /// </summary>
        public void Clear()
        {
            LastSentMessages.Clear();
            DelayedMessages.Clear();
            _waitingMessagesCount = 0;
            Result.Clear();
            LostMessages.Clear();
            WaitingMessages.Clear();
        }

        #endregion

        #region Waiting MessagesManager

        /// <summary>
        ///     Track all messages that are posting
        ///     Must be call before Mailbox.Post()
        /// </summary>
        /// <param name="message"></param>
        /// <param name="step"></param>
        public void EnQueueWaitingMessage(Message message, ushort step)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (Debug)
            {
                lock (WaitingMessages)
                {
                    WaitingMessages.Add(message);
                    LastSentMessages.Enqueue(message, step);
                }
            }

            _waitingMessagesCount++;
            message.State = MessageState.Sent;
        }

        public void TrackMessageSent(Message message, float cost)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            Result.SentMessagesCount++;
            switch (message.Medium)
            {
                case CommunicationMediums.Irc:
                    Result.SentMessagesByIrc++;
                    break;
                case CommunicationMediums.Email:
                    Result.SentMessagesByEmail++;
                    break;
                case CommunicationMediums.Phone:
                    Result.SentMessagesByPhone++;
                    break;
                case CommunicationMediums.Meeting:
                    Result.SentMessagesByMeeting++;
                    break;
                case CommunicationMediums.FaceToFace:
                    Result.SentMessagesByFaceToFace++;
                    break;
                case CommunicationMediums.ViaAPlatform:
                    Result.SentMessagesByPlatform++;
                    break;
                case CommunicationMediums.System:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Result.SentMessagesCost += cost;
        }

        public void DeQueueWaitingMessage(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (Debug)
            {
                lock (WaitingMessages)
                {
                    WaitingMessages.Remove(message);
                }
            }

            _waitingMessagesCount--;
            Result.ReceivedMessagesCount++;
            message.State = MessageState.Received;
        }

        public bool IsThereAnyWaitingMessages()
        {
            return _waitingMessagesCount != 0;
        }

        public void WaitingToClearAllMessages()
        {
            while (IsThereAnyWaitingMessages())
            {
                //Sleep
            }
        }

        #endregion
    }
}