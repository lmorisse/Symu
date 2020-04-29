#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Classes.Agent;

#endregion

namespace SymuEngine.Messaging.Message
{
    /// <summary>
    ///     A message that the agents use to communicate. In an agent-based system, the communication between the agents is
    ///     exclusively performed by exchanging messages.
    /// </summary>
    public class Message
    {
        /// <summary>
        ///     The subject of the message.
        /// </summary>
        public byte Subject { get; set; }

        /// <summary>
        ///     The action of the message.
        /// </summary>
        public MessageAction Action { get; set; }

        /// <summary>
        ///     The name of the agent that sends the message
        /// </summary>
        public AgentId Sender { get; set; }

        /// <summary>
        ///     The name of the agent that needs to receive the message
        /// </summary>
        public AgentId Receiver { get; set; }

        /// <summary>
        ///     The State of the message
        /// </summary>
        public MessageState State { get; set; } = MessageState.Created;

        /// <summary>
        ///     The communication medium of the message
        /// </summary>
        public CommunicationMediums Medium { get; set; } = CommunicationMediums.System;

        /// <summary>
        ///     The attachments associated with the content
        ///     null if the message has no attachment
        ///     use HasAttachments to check
        /// </summary>
        public MessageAttachments Attachments { get; set; }

        public bool HasAttachments => Attachments != null;

        /// <summary>
        ///     The Id of the feed to follow a conversation
        /// </summary>
        //TODO public byte FeedId { get; set; }
        public override bool Equals(object obj)
        {
            return obj is Message message
                   && message.Sender.Equals(Sender)
                   && message.Receiver.Equals(Receiver)
                   && message.Action == Action
                   && message.Subject == Subject;
        }

        #region Reply message

        /// <summary>
        ///     Should
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Message ReplyMessage(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var reply = new Message
            {
                Sender = message.Receiver,
                Receiver = message.Sender,
                Action = MessageAction.Reply,
                Subject = message.Subject,
                Medium = message.Medium
            };
            if (!message.HasAttachments)
            {
                return reply;
            }

            reply.Attachments = new MessageAttachments();
            reply.Attachments.Copy(message.Attachments);
            return reply;
        }

        #endregion

        #region constructor

        /// <summary>
        ///     Initializes a new instance of the Message class with an empty message.
        /// </summary>
        public Message()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the Message class.
        /// </summary>
        /// <param name="senderId">The name of the agent that sends the message</param>
        /// <param name="receiverId">The name of the agent that needs to receive the message</param>
        /// <param name="action"></param>
        /// <param name="subject">The subject of the message</param>
        public Message(AgentId senderId, AgentId receiverId, MessageAction action, byte subject) : this()
        {
            Sender = senderId;
            Receiver = receiverId;
            Action = action;
            Subject = subject;
        }

        /// <summary>
        ///     Initializes a new instance of the Message class.
        /// </summary>
        /// <param name="senderId">The name of the agent that sends the message</param>
        /// <param name="receiverId">The name of the agent that needs to receive the message</param>
        /// <param name="action"></param>
        /// <param name="subject">The subject of the message</param>
        /// <param name="medium"></param>
        public Message(AgentId senderId, AgentId receiverId, MessageAction action, byte subject,
            CommunicationMediums medium) : this(senderId, receiverId, action, subject)
        {
            Medium = medium;
        }

        public Message(AgentId senderId, AgentId receiverId, MessageAction action, byte subject, object attachment)
            : this(senderId, receiverId, action, subject)
        {
            if (attachment == null)
            {
                return;
            }

            Attachments = new MessageAttachments();
            Attachments.Add(attachment);
        }

        public Message(AgentId senderId, AgentId receiverId, MessageAction action, byte subject, object attachment,
            CommunicationMediums medium)
            : this(senderId, receiverId, action, subject, medium)
        {
            if (attachment == null)
            {
                return;
            }

            Attachments = new MessageAttachments();
            Attachments.Add(attachment);
        }

        public Message(AgentId senderId, AgentId receiverId, MessageAction action, byte subject,
            MessageAttachments attachments)
            : this(senderId, receiverId, action, subject)
        {
            if (attachments == null)
            {
                return;
            }

            Attachments = attachments;
        }

        public Message(AgentId senderId, AgentId receiverId, MessageAction action, byte subject,
            MessageAttachments attachments, CommunicationMediums medium)
            : this(senderId, receiverId, action, subject, medium)
        {
            if (attachments == null)
            {
                return;
            }

            Attachments = attachments;
        }

        #endregion
    }
}