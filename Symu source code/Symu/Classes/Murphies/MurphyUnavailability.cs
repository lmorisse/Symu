#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

using System;

namespace Symu.Classes.Murphies
{
    /// <summary>
    /// This murphy defines unplanned :index:`unavailability` of agent such as illness, ...
    /// This murphy has an impact on the agent's initial capacity.
    /// It should not be confused with Agent.Cognitive.InteractionPatterns.AgentCanBeIsolated which deals with plannable unavailability(even if it can be randomly generated, such as holidays).
    /// </summary>
    public class MurphyUnAvailability : Murphy
    {
        private float _threshold = 0.1F;

        /// <summary>
        ///     Unavailability Threshold is linked to worker's initial capacity.
        ///     As capacity is already a stochastic function, we choose to fix the threshold
        ///     When initial capacity is below this threshold,
        ///     the worker is considered not available for the day
        /// </summary>
        /// <example>
        ///     UnavailabilityThreshold = 0.1 by default
        ///     if initial capacity = 0.05, worker is not available today
        /// </example>
        public float Threshold
        {
            get => _threshold;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("Threshold should be between [0;1]");
                }

                _threshold = value;
            }
        }

        /// <summary>
        ///     Unavailability has an impact on the daily availability or sub optimal work
        /// </summary>
        /// <param name="capacity"></param>
        /// <returns>0 if capacity is below Threshold, capacity if above</returns>
        public float Next(float capacity)
        {
            if (!IsAgentOn())
            {
                return capacity;
            }

            // Below a certain threshold, we can presume the agent is not available
            return capacity < Threshold ? 0 : capacity;
        }
    }
}