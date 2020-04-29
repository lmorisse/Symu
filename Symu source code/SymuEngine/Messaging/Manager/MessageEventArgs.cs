#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;

#endregion

namespace SymuEngine.Messaging.Manager
{
    /// <summary>
    ///     The eventArg class for Message processor
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(Message.Message message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public Message.Message Message { get; set; }
    }
}