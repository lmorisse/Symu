#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace SymuEngine.Classes.Agents.Models.CognitiveModel
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