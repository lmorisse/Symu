#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Diagnostics.CodeAnalysis;

#endregion

namespace SymuEngine.Messaging.Messages
{
    [SuppressMessage("Design", "CA1028:Le stockage d'enum doit être de type Int32", Justification = "<En attente>")]
    public enum MessageAction : byte
    {
        Handle,
        Ask,
        Reply,
        Close,
        Stop,
        Cancel,
        Update,
        Remove,
        Add
    }
}