#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using Symu.Messaging.Messages;

#endregion

namespace Symu.Messaging.Reply
{
    /// <summary>
    ///     Manage the waiting replies sent with SendToClass method
    ///     when the agent need to wait for all the answers before taking an action
    ///     Inherits from WaitingReply with an additional parameter
    ///     Stock all the messages sent
    /// </summary>
    public class WaitingMessageSent : WaitingReply
    {
        public WaitingMessageSent(byte content, object param, ushort messagesSent) : base(content, messagesSent)
        {
            Param = param;
        }

        public object Param { get; }

        public List<Message> Messages { get; } = new List<Message>();

        public void Increment(Message message)
        {
            Increment();
            Messages.Add(message);
        }

        internal override void Initialize(ushort messageSent)
        {
            base.Initialize(messageSent);
            Messages.Clear();
        }

        /// <summary>
        ///     Call NoWaitingMessage to check if the agent has received all the replies
        ///     The number replies expected is not set but it is a parameter
        /// </summary>
        /// <param name="expectedReplies"></param>
        /// <returns></returns>
        public bool NoWaitingMessage(ushort expectedReplies)
        {
            return MessagesSent == expectedReplies;
        }

        public void Reset()
        {
            Initialize(0);
        }
    }
}