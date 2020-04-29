#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuTools.Classes.ProbabilityDistributions;

#endregion

namespace SymuEngine.Classes.Agent.Models
{
    /// <summary>
    ///     Base Class to a simulation model
    /// </summary>
    /// <example>
    ///     Multi tasking
    ///     Prioritization
    ///     ...
    /// </example>
    public class ModelEntity
    {
        public ModelEntity()
        {
        }

        public ModelEntity(ModelEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.CopyTo(this);
        }

        /// <summary>
        ///     If (On) the model is active globally
        /// </summary>
        public bool On { get; set; }

        /// <summary>
        ///     If model is On, individuals may use or not the model
        ///     RateOfAgentsOn define the rate of agent that will use effectively the model
        ///     [0; 1] default 1
        /// </summary>
        /// <example>if RateOfAgentsOn = 1, every agent will use the model</example>
        /// <example>if RateOfAgentsOn = 0, no agent will use the model, equivalent to Model Off</example>
        /// <example>if RateOfAgentsOn = 0.8F, 80% of agents will use the model</example>
        public float RateOfAgentsOn { get; set; } = 0.8F;

        public void CopyTo(ModelEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.On = On;
            entity.RateOfAgentsOn = RateOfAgentsOn;
        }

        /// <summary>
        ///     When Model is On, individuals may use or not the model
        ///     IsAgentOn set the agent parameter to On or Off
        /// </summary>
        /// <returns>A random bool if model is On, false otherwise</returns>
        public bool IsAgentOn()
        {
            return On && Bernoulli.Sample(RateOfAgentsOn);
        }
    }
}