#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Diagnostics.CodeAnalysis;

#endregion

namespace Symu.Messaging.Messages
{
    /// <summary>
    ///     Message life cycle
    /// </summary>
    [SuppressMessage("Design", "CA1028:Le stockage d'enum doit être de type Int32", Justification = "<En attente>")]
    public enum MessageState : byte
    {
        Created,
        Sent,
        Received,

        /// <summary>
        ///     Receiver don't exist, so the message is lost
        /// </summary>
        Lost
    }
}