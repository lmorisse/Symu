#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace Symu.Classes.Agents
{
    /// <summary>
    ///     Status of the agent during a interaction
    ///     The State of the agent is started
    /// </summary>
    public enum AgentStatus
    {
        /// <summary>
        ///     Agent is available
        /// </summary>
        Available,

        /// <summary>
        ///     Agent is Working
        /// </summary>
        Busy,

        /// <summary>
        ///     Agent is not available during the interaction
        /// </summary>
        Offline
    }
}