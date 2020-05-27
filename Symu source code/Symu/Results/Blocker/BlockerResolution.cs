#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace Symu.Results.Blocker
{
    /// <summary>
    ///     Type of blocker resolution
    /// </summary>
    public enum BlockerResolution
    {
        /// <summary>
        ///     Blocker is resolved by asking help to internal agents
        /// </summary>
        Internal,

        /// <summary>
        ///     Blocker is resolved by searching or asking help to external agents
        /// </summary>
        External,

        /// <summary>
        ///     Blocker is resolved by guessing the answer
        /// </summary>
        Guessing,

        /// <summary>
        ///     Blocker is resolved by searching in agent's databases
        /// </summary>
        Searching
    }
}