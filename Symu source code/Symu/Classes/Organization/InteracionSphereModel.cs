#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents.Models;
using Symu.Classes.Agents.Models.CognitiveModel;
using Symu.Environment;

#endregion

namespace Symu.Classes.Organization
{
    /// <summary>
    ///     Model for interaction sphere settings
    /// </summary>
    public class InteractionSphereModel : ModelEntity
    {
        private float _maxSphereDensity = 1;

        private float _minSphereDensity;

        /// <summary>
        ///     If set to true, sphere of interaction will be re calculate at the FrequencyOfSphereUpdate.
        ///     If set to true, agent can initiate new interaction to add an agent in his sphere of interaction.
        ///     Otherwise, it will be initialized once
        /// </summary>
        public bool SphereUpdateOverTime { get; set; }

        /// <summary>
        ///     Define the frequency of sphere change if sphere change overtime
        /// </summary>
        public TimeStepType FrequencyOfSphereUpdate { get; set; } = TimeStepType.Monthly;

        /// <summary>
        ///     If set to true, sphere of interaction is randomly generated, min/max sphere density are associated.
        ///     Is set to false, sphere is generated based on similarity matching
        /// </summary>
        public bool RandomlyGeneratedSphere { get; set; }

        /// <summary>
        ///     If sphere of interaction is randomly generated,
        ///     min sphere density is used as range min for the random generation
        /// </summary>
        public float MinSphereDensity
        {
            get => _minSphereDensity;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("MinSphereDensity should be between [0;1]");
                }

                _minSphereDensity = value;
            }
        }

        /// <summary>
        ///     If sphere of interaction is randomly generated,
        ///     max sphere density is used as range max for the random generation
        /// </summary>
        public float MaxSphereDensity
        {
            get => _maxSphereDensity;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("MaxSphereDensity should be between [0;1]");
                }

                _maxSphereDensity = value;
            }
        }

        /// <summary>
        ///     Weight of SocialDemographic in the calculus of DerivedParameter
        ///     Range[0;1]
        /// </summary>
        public float SocialDemographicWeight { get; set; } = 1;

        /// <summary>
        ///     Weight of RelativeBelief in the calculus of DerivedParameter
        ///     Range[0;1]
        /// </summary>
        public float RelativeBeliefWeight { get; set; } = 1;

        /// <summary>
        ///     Weight of RelativeKnowledge in the calculus of DerivedParameter
        ///     Range[0;1]
        /// </summary>
        public float RelativeKnowledgeWeight { get; set; } = 1;

        /// <summary>
        ///     Weight of RelativeBeliefs in the calculus of DerivedParameter
        ///     Range[0;1]
        /// </summary>
        public float RelativeActivityWeight { get; set; } = 1;

        public void CopyTo(InteractionSphereModel entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            base.CopyTo(entity);
            entity.SphereUpdateOverTime = SphereUpdateOverTime;
            entity.FrequencyOfSphereUpdate = FrequencyOfSphereUpdate;
            entity.MaxSphereDensity = MaxSphereDensity;
            entity.MinSphereDensity = MinSphereDensity;
            entity.RandomlyGeneratedSphere = RandomlyGeneratedSphere;
            entity.RelativeActivityWeight = RelativeActivityWeight;
            entity.RelativeBeliefWeight = RelativeBeliefWeight;
            entity.RelativeKnowledgeWeight = RelativeKnowledgeWeight;
            entity.SocialDemographicWeight = SocialDemographicWeight;
        }

        /// <summary>
        ///     Set binary interaction pattern based on InteractionStrategy
        /// </summary>
        /// <param name="strategy"></param>
        public void SetInteractionPatterns(InteractionStrategy strategy)
        {
            switch (strategy)
            {
                case InteractionStrategy.Homophily:
                    RelativeActivityWeight = 0.25F;
                    RelativeBeliefWeight = 0.25F;
                    RelativeKnowledgeWeight = 0.25F;
                    SocialDemographicWeight = 0.25F;
                    break;
                case InteractionStrategy.Knowledge:
                    RelativeActivityWeight = 0F;
                    RelativeBeliefWeight = 0F;
                    RelativeKnowledgeWeight = 1F;
                    SocialDemographicWeight = 0F;
                    break;
                case InteractionStrategy.Activities:
                    RelativeActivityWeight = 1F;
                    RelativeBeliefWeight = 0F;
                    RelativeKnowledgeWeight = 0F;
                    SocialDemographicWeight = 0F;
                    break;
                case InteractionStrategy.Beliefs:
                    RelativeActivityWeight = 0F;
                    RelativeBeliefWeight = 1F;
                    RelativeKnowledgeWeight = 0F;
                    SocialDemographicWeight = 0F;
                    break;
                case InteractionStrategy.SocialDemographics:
                    RelativeActivityWeight = 0F;
                    RelativeBeliefWeight = 0F;
                    RelativeKnowledgeWeight = 0F;
                    SocialDemographicWeight = 1F;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null);
            }
        }
    }
}