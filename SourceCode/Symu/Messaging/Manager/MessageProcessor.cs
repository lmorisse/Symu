#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Symu.Messaging.Delayed;
using Symu.Messaging.Messages;
using Symu.Messaging.Reply;
using Symu.Messaging.Subscription;

#endregion

namespace Symu.Messaging.Manager
{
    public sealed class MessageProcessor : IDisposable
    {
        private readonly Func<MessageProcessor, Task> _body;
        private readonly MessagesManager _messagesManager;
        private bool _started;

        public MessageProcessor(Func<MessageProcessor, Task> body)
        {
            _body = body;
            _messagesManager = new MessagesManager();
            _started = false;
        }

        /// <summary>
        ///     Actual number of messages of the step
        ///     As it is a send and receive parameter, it can be in MailBox
        /// </summary>
        public ushort NumberMessagesPerStep { get; set; }

        #region Waiting replies

        /// <summary>
        ///     Manage message that are waiting replies
        /// </summary>
        public WaitingReplies WaitingReplies { get; } = new WaitingReplies();

        #endregion

        #region Message subscription

        /// <summary>
        ///     Manage subscriptions/unsubscriptions to an agent's messages
        ///     The subscription is based on the subject of the message
        ///     If an agent subscribe, the agent will receive all messages sent on the subject
        /// </summary>
        public MessageSubscriptions Subscriptions { get; } = new MessageSubscriptions();

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _messagesManager.Dispose();
        }

        #endregion

        public void Start()
        {
            if (_started)
            {
                throw new InvalidOperationException("MessageProcessor already started");
            }

            _started = true;

            Task.Run(async () =>
            {
                try
                {
                    await _body(this).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    var exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                    exceptionDispatchInfo.Throw();
                }
            }, Task.Factory.CancellationToken);
        }

        #region Post / receive message

        /// <summary>
        /// </summary>
        public byte NumberSentPerPeriod { get; set; }

        /// <summary>
        /// </summary>
        public byte NumberReceivedPerPeriod { get; set; }

        /// <summary>
        ///     EventHandler use to update the form after each step
        /// </summary>
        public event EventHandler<MessageEventArgs> OnBeforePostEvent;

        public void Post(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            IncrementMessagesPerPeriod(message.Medium, false);
            var eventArgs = new MessageEventArgs(message);
            OnBeforePostEvent?.Invoke(this, eventArgs);
            _messagesManager.Post(message);
        }

        /// <summary>
        ///     Increment numberMessagesPerPeriod if the message is not a system message
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="sentMessage">if set message is a posted message, otherwise it's a received message</param>
        public void IncrementMessagesPerPeriod(CommunicationMediums messageType, bool sentMessage)
        {
            if (messageType == CommunicationMediums.System || NumberMessagesPerStep >= ushort.MaxValue)
            {
                return;
            }

            NumberMessagesPerStep++;
            if (sentMessage)
            {
                NumberSentPerPeriod++;
            }
            else
            {
                NumberReceivedPerPeriod++;
            }
        }

        /// <summary>
        ///     Initialize numberMessagesPerPeriod done at the beginning of every new period (interaction step)
        /// </summary>
        public void ClearMessagesPerPeriod()
        {
            NumberMessagesPerStep = 0;
            NumberSentPerPeriod = 0;
            NumberReceivedPerPeriod = 0;
        }

        public Task<Message> Receive()
        {
            return _messagesManager.Receive();
        }

        #endregion

        #region Delayed MessagesManager

        /// <summary>
        ///     Postponed MessagesManager until the next interaction step
        /// </summary>
        public DelayedMessages DelayedMessages { get; } = new DelayedMessages();

        public void PostAsADelayed(Message message, ushort step)
        {
            DelayedMessages.Enqueue(message, step);
        }

        /// <summary>
        ///     Get the next delayed message of this step
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public Message NextDelayedMessages(ushort step)
        {
            return DelayedMessages.Dequeue(step);
        }

        #endregion

        #region Missed Message

        /// <summary>
        ///     An sender agent has send a message of type phone, or meeting, ... but the receiver was offline, so he missed the
        ///     message
        ///     Missed messages trace those missed messages in debug mode
        /// </summary>
        public List<Message> MissedMessages { get; } = new List<Message>();

        public void AddMissedMessage(Message message, bool debug)
        {
            if (debug)
            {
                MissedMessages.Add(message);
            }
        }

        #endregion

        #region Not accepted Message

        /// <summary>
        ///     An sender agent has send a message of type phone, or meeting, ... but the sender was not in the interaction sphere
        ///     of the receiver and did not accept the message
        ///     NotAcceptedMessages trace those not accepted message in debug
        /// </summary>
        public List<Message> NotAcceptedMessages { get; } = new List<Message>();

        public void AddNotAcceptedMessages(Message message, bool debug)
        {
            if (debug)
            {
                NotAcceptedMessages.Add(message);
            }
        }

        #endregion
    }
}