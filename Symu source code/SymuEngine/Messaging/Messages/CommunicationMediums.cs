#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace SymuEngine.Messaging.Messages
{
    /// <summary>
    ///     A message is an interaction between tow agents
    ///     The interaction can be done on different communication channels : IRC, mail, meeting, ....
    /// </summary>
    /// <remarks>value should be 2exp(n) 0, 1, 2, 4, 8, 16, ...</remarks>
    [SuppressMessage("Design", "CA1028:Le stockage d'enum doit être de type Int32", Justification = "<En attente>")]
    [Flags]
    public enum CommunicationMediums : byte
    {
        /// <summary>
        ///     a system message is used to send information like a new interaction step, information from the timer, ...
        /// </summary>
        System = 0,

        /// <summary>
        ///     Slack, Teams, ...
        /// </summary>
        Irc = 1,
        Email = 2,
        Phone = 4,
        Meeting = 8,
        FaceToFace = 16,

        /// <summary>
        ///     Via an online platform like Jira, ...
        /// </summary>
        ViaAPlatform = 32
    }
}