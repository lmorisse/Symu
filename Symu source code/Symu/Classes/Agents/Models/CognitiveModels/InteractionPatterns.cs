#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Common;
using Symu.Tools.Math.ProbabilityDistributions;

#endregion

namespace Symu.Classes.Agents.Models.CognitiveModels
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
        /// <summary>
        ///     If set to true, agent is computed in the interaction sphere
        /// </summary>
        public bool IsPartOfInteractionSphere { get; set; }

        public void CopyTo(InteractionPatterns interactionPatterns)
        {
            if (interactionPatterns is null)
            {
                throw new ArgumentNullException(nameof(interactionPatterns));
            }

            interactionPatterns.AgentCanBeIsolated = AgentCanBeIsolated;
            interactionPatterns.IsolationCyclicity = IsolationCyclicity;
            interactionPatterns.InteractionsBasedOnKnowledge = InteractionsBasedOnKnowledge;
            interactionPatterns.InteractionsBasedOnHomophily = InteractionsBasedOnHomophily;
            interactionPatterns.InteractionsBasedOnActivities = InteractionsBasedOnActivities;
            interactionPatterns.InteractionsBasedOnBeliefs = InteractionsBasedOnBeliefs;
            interactionPatterns.InteractionsBasedOnSocialDemographics = InteractionsBasedOnSocialDemographics;
            interactionPatterns.MaxNumberOfNewInteractions = MaxNumberOfNewInteractions;
            interactionPatterns.LimitNumberOfNewInteractions = LimitNumberOfNewInteractions;
            interactionPatterns.IsPartOfInteractionSphere = IsPartOfInteractionSphere;
            interactionPatterns.AllowNewInteractions = AllowNewInteractions;
            interactionPatterns.ThresholdForNewInteraction = ThresholdForNewInteraction;
        }

        #region Isolation

        /// <summary>
        ///     This parameter specify whether agent must be isolated or whether they are active during the entire symu
        /// </summary>
        public Frequency AgentCanBeIsolated { get; set; }

        //public bool IsolationIsCyclical { get; set; }
        /// <summary>
        ///     Isolation can be random, cylclic, ...
        /// </summary>
        public Cyclicity IsolationCyclicity { get; set; }

        /// <summary>
        ///     Check if Agent is Isolated at this step based on IsolationCyclicity or IsolationIsCyclical
        /// </summary>
        /// <param name="step"></param>
        /// <returns>true if agent is isolated, false otherwise</returns>
        public bool IsIsolated(ushort step)
        {
            switch (IsolationCyclicity)
            {
                case Cyclicity.Cyclical:
                    return IsCyclicallyIsolated(step);
                case Cyclicity.Random:
                    return IsRandomlyIsolated();
                case Cyclicity.Always:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        ///     Check if Agent is randomly Isolated
        /// </summary>
        /// <returns>true if agent is isolated, false otherwise</returns>
        private bool IsRandomlyIsolated()
        {
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

        /// <summary>
        ///     Check if Agent is cyclically Isolated at this step
        /// </summary>
        /// <param name="step"></param>
        /// <returns>true if agent is isolated, false otherwise</returns>
        public bool IsCyclicallyIsolated(ushort step)
        {
            switch (AgentCanBeIsolated)
            {
                case Frequency.Never:
                    return false;
                case Frequency.VeryRarely:
                    return step % 6 == 0;
                case Frequency.Rarely:
                    return step % 5 == 0;
                case Frequency.Medium:
                    return step % 4 == 0;
                case Frequency.Often:
                    return step % 3 == 0;
                case Frequency.VeryOften:
                    return step % 2 == 0;
                case Frequency.Always:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Interactions strategy

        private float _interactionsBasedOnHomophily = 1;

        /// <summary>
        ///     An agent that acts via homophily attempts to ﬁnd an interaction partner that shares its characteristics.
        ///     When searching for suitable partners, the agent will stress agents who have similar socio-demographic parameters,
        ///     similar knowledge, and similar beliefs
        ///     Range [0;1]
        ///     Default interaction
        /// </summary>
        /// <remarks>
        ///     InteractionsBasedOnHomophily+InteractionsBasedOnKnowledge+InteractionsBasedOnActivities+
        ///     InteractionsBasedOnBelief+InteractionsBasedOnSocialDemographics= 1
        /// </remarks>
        public float InteractionsBasedOnHomophily
        {
            get => _interactionsBasedOnHomophily;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("InteractionsBasedOnHomophily should be between [0;1]");
                }

                _interactionsBasedOnHomophily = value;
            }
        }

        private float _interactionsBasedOnKnowledge;

        /// <summary>
        ///     An agent that acts via deliberate search will attempt to ﬁnd an interaction partner that knows a particular piece
        ///     of information.
        ///     When searching for suitable partners, the agent will stress knowledge and will ignore most other parameters.
        ///     Agents who lack the piece of information will be ignored by the seeker.
        ///     Range [0;1]
        /// </summary>
        /// <remarks>
        ///     InteractionsBasedOnHomophily+InteractionsBasedOnKnowledge+InteractionsBasedOnActivities+
        ///     InteractionsBasedOnBelief+InteractionsBasedOnSocialDemographics= 1
        /// </remarks>
        public float InteractionsBasedOnKnowledge
        {
            get => _interactionsBasedOnKnowledge;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("InteractionsBasedOnKnowledge should be between [0;1]");
                }

                _interactionsBasedOnKnowledge = value;
            }
        }

        private float _interactionsBasedOnActivities;

        /// <summary>
        ///     An agent that acts with its co-workers will interact with those agents that are performing a similar task.
        ///     When searching for interaction partners, the agent will stress tasks primarily and will ignore most other
        ///     parameters.
        ///     Agents assigned to other tasks will be ignored by the seeker.
        ///     Range [0;1]
        /// </summary>
        /// <remarks>
        ///     InteractionsBasedOnHomophily+InteractionsBasedOnKnowledge+InteractionsBasedOnActivities+
        ///     InteractionsBasedOnBelief+InteractionsBasedOnSocialDemographics= 1
        /// </remarks>
        public float InteractionsBasedOnActivities
        {
            get => _interactionsBasedOnActivities;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("InteractionsBasedOnActivities should be between [0;1]");
                }

                _interactionsBasedOnActivities = value;
            }
        }

        private float _interactionsBasedOnBeliefs;

        /// <summary>
        ///     An agent that acts with its co-workers will interact with those agents that have the same beliefs.
        ///     When searching for interaction partners, the agent will stress tasks primarily and will ignore most other
        ///     parameters.
        ///     Range [0;1]
        /// </summary>
        /// <remarks>
        ///     InteractionsBasedOnHomophily+InteractionsBasedOnKnowledge+InteractionsBasedOnActivities+
        ///     InteractionsBasedOnBelief+InteractionsBasedOnSocialDemographics= 1
        /// </remarks>
        public float InteractionsBasedOnBeliefs
        {
            get => _interactionsBasedOnBeliefs;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("InteractionsBasedOnBeliefs should be between [0;1]");
                }

                _interactionsBasedOnBeliefs = value;
            }
        }

        /// <summary>
        ///     An agent that acts with its co-workers will interact with those agents that have the same beliefs.
        ///     When searching for interaction partners, the agent will stress tasks primarily and will ignore most other
        ///     parameters.
        ///     Range [0;1]
        /// </summary>
        /// <remarks>
        ///     InteractionsBasedOnHomophily+InteractionsBasedOnKnowledge+InteractionsBasedOnActivities+
        ///     InteractionsBasedOnBelief+InteractionsBasedOnSocialDemographics= 1
        /// </remarks>
        private float _interactionsBasedOnSocialDemographics;

        public float InteractionsBasedOnSocialDemographics
        {
            get => _interactionsBasedOnSocialDemographics;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException(
                        "InteractionsBasedOnSocialDemographics should be between [0;1]");
                }

                _interactionsBasedOnSocialDemographics = value;
            }
        }

        /// <summary>
        ///     Clone binary interaction pattern based on InteractionStrategy
        /// </summary>
        /// <param name="strategy"></param>
        public void SetInteractionPatterns(InteractionStrategy strategy)
        {
            switch (strategy)
            {
                case InteractionStrategy.Homophily:
                    InteractionsBasedOnHomophily = 1;
                    InteractionsBasedOnKnowledge = 0;
                    InteractionsBasedOnActivities = 0;
                    InteractionsBasedOnBeliefs = 0;
                    InteractionsBasedOnSocialDemographics = 0;
                    break;
                case InteractionStrategy.Knowledge:
                    InteractionsBasedOnHomophily = 0;
                    InteractionsBasedOnKnowledge = 1;
                    InteractionsBasedOnActivities = 0;
                    InteractionsBasedOnBeliefs = 0;
                    InteractionsBasedOnSocialDemographics = 0;
                    break;
                case InteractionStrategy.Activities:
                    InteractionsBasedOnHomophily = 0;
                    InteractionsBasedOnKnowledge = 0;
                    InteractionsBasedOnActivities = 1;
                    InteractionsBasedOnBeliefs = 0;
                    InteractionsBasedOnSocialDemographics = 0;
                    break;
                case InteractionStrategy.Beliefs:
                    InteractionsBasedOnHomophily = 0;
                    InteractionsBasedOnKnowledge = 0;
                    InteractionsBasedOnActivities = 0;
                    InteractionsBasedOnBeliefs = 1;
                    InteractionsBasedOnSocialDemographics = 0;
                    break;
                case InteractionStrategy.SocialDemographics:
                    InteractionsBasedOnHomophily = 0;
                    InteractionsBasedOnKnowledge = 0;
                    InteractionsBasedOnActivities = 0;
                    InteractionsBasedOnBeliefs = 0;
                    InteractionsBasedOnSocialDemographics = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null);
            }
        }

        #endregion

        #region new interactions

        /// <summary>
        ///     If set to true, agent can initiate new interactions.
        /// </summary>
        public bool AllowNewInteractions { get; set; }

        /// <summary>
        ///     If set to true, agent can initiate a limited number of new interactions.
        ///     It is associated with MaxNumberOfNewInteractions.
        /// </summary>
        public bool LimitNumberOfNewInteractions { get; set; } = true;

        private byte _maxNumberOfNewInteractions = 5;

        /// <summary>
        ///     Maximum number of new interactions per step:
        ///     a new interaction is a new agent in the sphere of interaction
        /// </summary>
        public byte MaxNumberOfNewInteractions
        {
            get => _maxNumberOfNewInteractions;
            set
            {
                if (value > 5)
                {
                    throw new ArgumentOutOfRangeException("MaxNumberOfNewInteractions should be <= 5");
                }

                _maxNumberOfNewInteractions = value;
            }
        }

        private float _thresholdForNewInteraction = 0.2F;

        public float ThresholdForNewInteraction
        {
            get => _thresholdForNewInteraction;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("ThresholdForNewInteraction should be between [0;1]");
                }

                _thresholdForNewInteraction = value;
            }
        }


        /// <summary>
        ///     return a random value of the InteractionStrategy based on InteractionsBasedOnHomophily,
        ///     InteractionsBasedOnKnowledge,
        ///     InteractionsBasedOnActivities probabilities
        /// </summary>
        /// <returns>a random value of InteractionStrategy</returns>
        public InteractionStrategy NextInteractionStrategy()
        {
            var index = Categorical.SampleIndex(InteractionsBasedOnHomophily, InteractionsBasedOnKnowledge,
                InteractionsBasedOnActivities, InteractionsBasedOnBeliefs, InteractionsBasedOnSocialDemographics);
            switch (index)
            {
                case 0:
                    return InteractionStrategy.Homophily;
                case 1:
                    return InteractionStrategy.Knowledge;
                case 2:
                    return InteractionStrategy.Activities;
                case 3:
                    return InteractionStrategy.Beliefs;
                case 4:
                    return InteractionStrategy.SocialDemographics;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}