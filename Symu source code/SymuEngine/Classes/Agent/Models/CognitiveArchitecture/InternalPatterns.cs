#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Common;
using SymuTools.Classes.ProbabilityDistributions;

#endregion

namespace SymuEngine.Classes.Agent.Models.CognitiveArchitecture
{
    /// <summary>
    ///     InteractionPatterns from Construct Software
    ///     Sphere of interaction
    ///     Isolation
    ///     Interactions patterns
    /// </summary>
    /// <remarks>Interaction Patterns from Construct Software</remarks>
    public class InteractionPatterns
    {
        public void CopyTo(InteractionPatterns interactionPatterns)
        {
            if (interactionPatterns is null)
            {
                throw new ArgumentNullException(nameof(interactionPatterns));
            }

            interactionPatterns.AgentCanBeIsolated = AgentCanBeIsolated;
            interactionPatterns.IsolationIsCyclical = IsolationIsCyclical;
            interactionPatterns.IsolationIsRandom = IsolationIsRandom;
            interactionPatterns.InteractionsDeliberateSearch = InteractionsDeliberateSearch;
            interactionPatterns.InteractionsUsingHomophily = InteractionsUsingHomophily;
            interactionPatterns.InteractionsWithCoWorkers = InteractionsWithCoWorkers;
        }

        #region Sphere of interactions

        #endregion

        #region Isolation

        /// <summary>
        ///     This parameter specify whether agent must be isolated or whether they are active during the entire simulation
        /// </summary>
        public Frequency AgentCanBeIsolated { get; set; }

        public bool IsolationIsCyclical { get; set; }
        public bool IsolationIsRandom { get; set; }

        /// <summary>
        ///     Impact of isolation parameter on the capacity to work of the agent
        /// </summary>
        /// <returns>true if agent is isolated, false otherwise</returns>
        public bool IsIsolated()
        {
            if (!IsolationIsRandom)
            {
                return false;
            }

            float isolationThreshold;
            switch (AgentCanBeIsolated)
            {
                case Frequency.Never:
                    isolationThreshold = 0;
                    break;
                case Frequency.VeryRarely:
                    isolationThreshold = 0.1F;
                    break;
                case Frequency.Rarely:
                    isolationThreshold = 0.3F;
                    break;
                case Frequency.Medium:
                    isolationThreshold = 0.5F;
                    break;
                case Frequency.Often:
                    isolationThreshold = 0.7F;
                    break;
                case Frequency.VeryOften:
                    isolationThreshold = 0.9F;
                    break;
                case Frequency.Always:
                    isolationThreshold = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Bernoulli.Sample(isolationThreshold);
        }

        #endregion

        #region Interactions strategy

        /// <summary>
        ///     An agent that acts via homophily attempts to ﬁnd an interaction partner that shares its characteristics.
        ///     When searching for suitable partners, the agent will stress agents who have similar socio-demographic parameters,
        ///     similar knowledge, and similar beliefs
        ///     Range [0;1]
        /// </summary>
        /// <remarks>InteractionsUsingHomophily+InteractionsDeliberateSearch+InteractionsWithCoWorkers = 100</remarks>
        public float InteractionsUsingHomophily { get; set; }

        /// <summary>
        ///     An agent that acts via deliberate search will attempt to ﬁnd an interaction partner that knows a particular piece
        ///     of information.
        ///     When searching for suitable partners, the agent will stress knowledge and will ignore most other parameters.
        ///     Agents who lack the piece of information will be ignored by the seeker.
        ///     Range [0;1]
        /// </summary>
        /// <remarks>InteractionsUsingHomophily+InteractionsDeliberateSearch+InteractionsWithCoWorkers = 100</remarks>
        public float InteractionsDeliberateSearch { get; set; }

        /// <summary>
        ///     An agent that acts with its co-workers will interact with those agents that are performing a similar task.
        ///     When searching for interaction partners, the agent will stress tasks primarily and will ignore most other
        ///     parameters.
        ///     Agents assigned to other tasks will be ignored by the seeker.
        ///     Range [0;1]
        /// </summary>
        /// <remarks>InteractionsUsingHomophily+InteractionsDeliberateSearch+InteractionsWithCoWorkers = 100</remarks>
        public float InteractionsWithCoWorkers { get; set; }

        /// <summary>
        ///     return a random value of the InteractionStrategy based on InteractionsUsingHomophily, InteractionsDeliberateSearch,
        ///     InteractionsWithCoWorkers probabilities
        /// </summary>
        /// <returns>a random value of InteractionStrategy</returns>
        public InteractionStrategy NextInteractionStrategy()
        {
            var index = Categorical.SampleIndex(InteractionsUsingHomophily, InteractionsDeliberateSearch,
                InteractionsWithCoWorkers);
            switch (index)
            {
                default:
                    return InteractionStrategy.Homophily;
                case 1:
                    return InteractionStrategy.DeliberateSearch;
                case 2:
                    return InteractionStrategy.CoWorkers;
            }
        }

        #endregion
    }
}