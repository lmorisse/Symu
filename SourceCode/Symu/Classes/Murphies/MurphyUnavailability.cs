#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Tools.Math.ProbabilityDistributions;

#endregion

namespace Symu.Classes.Murphies
{
    /// <summary>
    ///     This murphy defines unplanned :index:`unavailability` of agent such as illness, ...
    ///     This murphy has an impact on the agent's initial capacity.
    ///     It should not be confused with Agent.Cognitive.InteractionPatterns.AgentCanBeIsolated which deals with plannable
    ///     unavailability(even if it can be randomly generated, such as holidays).
    /// </summary>
    public class MurphyUnAvailability : Murphy
    {
        private float _rateOfUnavailability = 0.1F;

        /// <summary>
        ///     Rate of unavailability:
        ///     If rate = 0, everyone is available
        ///     If rate = 1, no one is available
        /// </summary>
        public float RateOfUnavailability
        {
            get => _rateOfUnavailability;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("RateOfUnavailability should be between [0;1]");
                }

                _rateOfUnavailability = value;
            }
        }

        /// <summary>
        ///     Unavailability has an impact on the daily availability or sub optimal work
        /// </summary>
        /// <returns>true if unavailable</returns>
        public bool Next()
        {
            return IsAgentOn() && Bernoulli.Sample(RateOfUnavailability);
        }
    }
}