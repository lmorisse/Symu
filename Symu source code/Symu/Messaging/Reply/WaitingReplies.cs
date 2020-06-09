#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;

#endregion

namespace Symu.Messaging.Reply
{
    /// <summary>
    ///     Manage the list of waiting messages sent with SendToClass method
    ///     when the agent need to wait for all the answers before taking an action
    /// </summary>
    public class WaitingReplies
    {
        public List<WaitingReply> WaitingMessages { get; } = new List<WaitingReply>();

        /// <summary>
        ///     Call Enqueue when the initial message is sent
        ///     Enqueue must be before the Send Method
        /// </summary>
        public void Reset(byte content)
        {
            if (!Exists(content))
            {
                WaitingMessages.Add(new WaitingReply(content, 0));
            }
            else
            {
                var waitingMessage = GetWaitingMessage(content);
                waitingMessage.Initialize(0);
            }
        }

        private WaitingReply GetWaitingMessage(byte content)
        {
            return WaitingMessages.Find(w => w.Content == content);
        }

        /// <summary>
        ///     Enqueue initialize for the good content
        ///     MessagesSent to messagesSentCount
        ///     and RepliesReceived to 0
        ///     Call Enqueue before the initial message is sent
        /// </summary>
        public void Enqueue(byte content, ushort messagesSentCount)
        {
            if (!Exists(content))
            {
                AddWaitingMessages(content, messagesSentCount);
            }
            else
            {
                var waitingMessage = GetWaitingMessage(content);
                waitingMessage.Initialize(messagesSentCount);
            }
        }

        private void AddWaitingMessages(byte content, ushort messagesSentCount)
        {
            WaitingMessages.Add(new WaitingReply(content, messagesSentCount));
        }

        /// <summary>
        ///     Dequeue increment RepliesReceived
        ///     Call Dequeue when a reply to the initial message is received
        /// </summary>
        /// <param name="content"></param>
        public void Dequeue(byte content)
        {
            var waitingMessage = GetWaitingMessage(content);
            // For unit tests
            if (waitingMessage == null)
            {
                return;
            }

            waitingMessage.RepliesReceived++;
            if (waitingMessage.Error)
            {
                throw new IndexOutOfRangeException("Error waiting messages");
            }
        }

        /// <summary>
        ///     Call NoWaitingMessage to check if the agent has received all the replies
        ///     set with Enqueue or increment
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool NoWaitingMessage(byte content)
        {
            if (!Exists(content))
            {
                return true;
            }

            var waitingMessage = GetWaitingMessage(content);
            return waitingMessage.NoWaitingReply;
        }

        private bool Exists(byte content)
        {
            return WaitingMessages.Exists(w => w.Content == content);
        }
    }
}