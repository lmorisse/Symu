#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Classes.Agents.Models;
using SymuEngine.Common;
using SymuEngine.Engine;
using SymuEngine.Repository.Networks.Beliefs;

#endregion

namespace SymuEngine.Classes.Organization
{
    /// <summary>
    ///     List of the models used by the organizationEntity
    /// </summary>
    public class OrganizationModels
    {
        /// <summary>
        ///     If set, the organizationEntity flexibility performance will be followed and stored during the simulation
        /// </summary>
        //TODO should be with IterationResult as Results Settings with cadence of feeds
        public bool FollowGroupFlexibility { get; set; }

        /// <summary>
        ///     If set, the organizationEntity knowledge and belief performance will be followed and stored during the simulation
        /// </summary>
        //TODO should be with IterationResult as Results Settings with cadence of feeds
        public bool FollowGroupKnowledge { get; set; }

        /// <summary>
        ///     If set, the organizationEntity tasks metrics will be followed and stored during the simulation
        /// </summary>
        //TODO should be with IterationResult as Results Settings with cadence of feeds
        public bool FollowTasks { get; set; }

        /// <summary>
        ///     If set, the organizationEntity blockers metrics will be followed and stored during the simulation
        /// </summary>
        //TODO should be with IterationResult as Results Settings with cadence of feeds
        public bool FollowBlockers { get; set; }

        /// <summary>
        ///     If true, allow multiple blockers at the same time
        ///     If false, will check new blockers only if there is no blocker
        /// </summary>
        public bool MultipleBlockers { get; set; }

        /// <summary>
        ///     Agent knowledge learning model
        /// </summary>
        public ModelEntity Learning { get; set; } = new ModelEntity();

        /// <summary>
        ///     Agent knowledge forgetting model
        /// </summary>
        public ModelEntity Forgetting { get; set; } = new ModelEntity();
        /// <summary>
        ///     Agent influence model
        /// </summary>
        public ModelEntity Influence { get; set; } = new ModelEntity();
        /// <summary>
        ///     Agent influence model
        /// </summary>
        public ModelEntity Beliefs { get; set; } = new ModelEntity();
        /// <summary>
        ///     Agent knowledge model
        /// </summary>
        public ModelEntity Knowledge { get; set; } = new ModelEntity();
        /// <summary>
        /// Impact level of agent's belief on how agent will accept to do the task
        /// </summary>
        public BeliefWeightLevel ImpactOfBeliefOnTask { get; set; } = BeliefWeightLevel.RandomWeight;

        public InteractionSphereModel InteractionSphere { get; set; } = new InteractionSphereModel();

        /// <summary>
        ///     Random generator modes in order to create random network
        /// </summary>
        public RandomGenerator Generator { get; set; } = RandomGenerator.RandomUniform;

        /// <summary>
        ///     Define level of random for the simulation.
        /// </summary>
        public SimulationRandom RandomLevel { get; set; } = SimulationRandom.NoRandom;

        public byte RandomLevelValue => (byte) RandomLevel;

        /// <summary>
        ///     Define the ratio of task splitting in intraday mode
        /// </summary>
        public float Intraday { get; set; } = 0.01F;

        public void SetRandomLevel(int level)
        {
            switch (level)
            {
                case 1:
                    RandomLevel = SimulationRandom.Simple;
                    break;
                case 2:
                    RandomLevel = SimulationRandom.Double;
                    break;
                case 3:
                    RandomLevel = SimulationRandom.Triple;
                    break;
                default:
                    RandomLevel = SimulationRandom.NoRandom;
                    break;
            }
        }

        public void CopyTo(OrganizationModels entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Learning.CopyTo(entity.Learning);
            Forgetting.CopyTo(entity.Forgetting);
            Influence.CopyTo(entity.Influence);
            Beliefs.CopyTo(entity.Beliefs);
            InteractionSphere.CopyTo(entity.InteractionSphere);
            entity.FollowGroupFlexibility = FollowGroupFlexibility;
            entity.FollowGroupKnowledge = FollowGroupKnowledge;
            entity.MultipleBlockers = MultipleBlockers;
            entity.Generator = Generator;
            entity.ImpactOfBeliefOnTask = ImpactOfBeliefOnTask;
        }
    }
}