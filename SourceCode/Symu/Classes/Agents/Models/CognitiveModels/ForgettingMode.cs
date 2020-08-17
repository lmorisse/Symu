#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace Symu.Classes.Agents.Models.CognitiveModels
{
    /// <summary>
    ///     Collection of forgetting modes
    /// </summary>
    public enum ForgettingSelectingMode
    {
        /// <summary>
        ///     Random mode
        /// </summary>
        Random,

        /// <summary>
        ///     Oldest knowledge first, based on the KnowledgeBit.LastTouched attribute
        /// </summary>
        Oldest
    }
}