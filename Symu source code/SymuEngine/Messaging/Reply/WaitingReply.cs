#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace SymuEngine.Messaging.Reply
{
    /// <summary>
    ///     Manage the waiting replies sent with SendToClass method
    ///     when the agent need to wait for all the answers before taking an action
    /// </summary>
    public class WaitingReply
    {
        public WaitingReply(byte content, ushort messagesSent)
        {
            Content = content;
            MessagesSent = messagesSent;
        }

        public byte Content { get; }
        public ushort MessagesSent { get; set; }
        public ushort RepliesReceived { get; set; }
        public bool NoWaitingReply => MessagesSent == RepliesReceived;
        public bool Error => MessagesSent < RepliesReceived;

        internal virtual void Initialize(ushort messageSent)
        {
            MessagesSent = messageSent;
            RepliesReceived = 0;
        }

        internal void Increment()
        {
            MessagesSent++;
        }
    }
}