#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace SymuEngine.Classes.Murphies
{
    /// <summary>
    ///     agent unavailability do to unplannable events
    ///     such as bottleneck, illness, ...
    ///     Holidays is not a murphy, it's plannable
    ///     MurphyUnAvailability has an impact on the worker's initial capacity
    ///     This happens in addition to Agent.Cognitive.InteractionPatterns.AgentCanBeIsolated
    /// </summary>
    public class MurphyUnAvailability : Murphy
    {
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
        public float Threshold { get; set; } = 0.1F;

        /// <summary>
        ///     Unavailability has an impact on the daily availability or sub optimal work
        /// </summary>
        /// <param name="capacity"></param>
        /// <returns>0 if capacity is below Threshold, capacity if above</returns>
        public float Next(float capacity)
        {
            if (!On)
            {
                return capacity;
            }

            // Below a certain threshold, we can presume the worker is not available
            if (capacity < Threshold)
            {
                capacity = 0;
            }

            return capacity;
        }
    }
}