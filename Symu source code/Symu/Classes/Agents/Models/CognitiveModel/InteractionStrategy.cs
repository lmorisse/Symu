#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace Symu.Classes.Agents.Models.CognitiveModel
{
    /// <summary>
    ///     Enum the different interaction strategy used in InteractionPatterns
    /// </summary>
    public enum InteractionStrategy
    {
        /// <summary>
        ///     among the interaction sphere, with best Homophily score (Knowledge, beliefs, socio-demographics, activity)
        /// </summary>
        Homophily,

        /// <summary>
        ///     among the interaction sphere, with best Knowledge score
        /// </summary>
        Knowledge,

        /// <summary>
        ///     among the interaction sphere, with best Activities score
        /// </summary>
        Activities,

        /// <summary>
        ///     among the interaction sphere, with best Beliefs score
        /// </summary>
        Beliefs,

        /// <summary>
        ///     among the interaction sphere, with best SocialDemographics score
        /// </summary>
        SocialDemographics
    }
}