#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;

#endregion

namespace SymuEngine.Messaging.Reply
{
    /// <summary>
    ///     Manage the list of waiting messages sent with SendToClass method
    ///     when the agent need to wait for all the answers before taking an action
    /// </summary>
    public class WaitingMessagesSent
    {
        public List<WaitingMessageSent> WaitingMessages { get; } = new List<WaitingMessageSent>();

        private WaitingMessageSent GetWaitingMessage(byte content, object param)
        {
            return WaitingMessages.Find(w => w.Content == content && w.Param == param);
        }

        private void AddWaitingMessages(byte content, object param, ushort messagesSentCount)
        {
            WaitingMessages.Add(new WaitingMessageSent(content, param, messagesSentCount));
        }

        public WaitingMessageSent WaitingMessage(byte content, object param)
        {
            if (!Exists(content, param))
            {
                AddWaitingMessages(content, param, 0);
            }

            return GetWaitingMessage(content, param);
        }

        private bool Exists(byte content, object param)
        {
            return WaitingMessages.Exists(w => w.Content == content && w.Param == param);
        }
    }
}