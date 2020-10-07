#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Organization;
using Symu.Common.Classes;
using Symu.Common.Interfaces;
using Symu.Common.Math.ProbabilityDistributions;
using Symu.Repository;
using Symu.Repository.Entities;

#endregion

namespace Symu.Classes.Agents.Models.CognitiveModels
{
    /// <summary>
    ///     CognitiveArchitecture define how an actor will influence or be influenced
    ///     Entity enable or not this mechanism for all the agents during the simulation
    ///     The InfluenceModel initialize the real value of the agent's influence parameters
    /// </summary>
    /// <remarks>From Construct Software</remarks>
    public class InfluenceModel
    {
        private readonly BeliefsModel _beliefsModel;

        private readonly RandomGenerator _model;

        //private readonly ActorNetwork _actorNetwork;
        private readonly WhitePages _whitePages;

        /// <summary>
        ///     Initialize influence model :
        ///     update networkInfluences
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cognitiveArchitecture"></param>
        /// <param name="whitePages"></param>
        /// <param name="beliefsModel"></param>
        /// <param name="model"></param>
        public InfluenceModel(InfluenceModelEntity entity, CognitiveArchitecture cognitiveArchitecture,
            WhitePages whitePages, BeliefsModel beliefsModel, RandomGenerator model)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (cognitiveArchitecture == null)
            {
                throw new ArgumentNullException(nameof(cognitiveArchitecture));
            }

            // In case of turning On the model during the simulation, champs must be initialized
            _whitePages = whitePages ?? throw new ArgumentNullException(nameof(whitePages));
            _beliefsModel = beliefsModel;
            _model = model;

            On = cognitiveArchitecture.KnowledgeAndBeliefs.HasBelief && entity.IsAgentOn();
            if (!On)
            {
                return;
            }

            if (cognitiveArchitecture.InternalCharacteristics.CanInfluenceOrBeInfluence && On)
            {
                Influenceability = NextInfluenceability(cognitiveArchitecture.InternalCharacteristics);
                Influentialness = NextInfluentialness(cognitiveArchitecture.InternalCharacteristics);
            }
            else
            {
                Influenceability = 0;
                Influentialness = 0;
            }
        }

        /// <summary>
        ///     how susceptible an agent will be to the influentialness of another agent
        /// </summary>
        public float Influenceability { get; set; }

        /// <summary>
        ///     how influential an agent will be
        /// </summary>
        public float Influentialness { get; set; }

        public bool On { get; set; }

        /// <summary>
        ///     Clone the Influentialness for a specific agent with a random value between [InfluentialnessRateMin,
        ///     InfluentialnessRateMax]
        /// </summary>
        /// <returns></returns>
        public static float NextInfluentialness(InternalCharacteristics internalCharacteristics)
        {
            if (internalCharacteristics == null)
            {
                throw new ArgumentNullException(nameof(internalCharacteristics));
            }

            return ContinuousUniform.Sample(internalCharacteristics.InfluentialnessRateMin,
                internalCharacteristics.InfluentialnessRateMax);
        }

        /// <summary>
        ///     Clone the Influentialness for a specific agent with a random value between [InfluentialnessRateMin,
        ///     InfluentialnessRateMax]
        /// </summary>
        /// <returns></returns>
        public static float NextInfluenceability(InternalCharacteristics internalCharacteristics)
        {
            if (internalCharacteristics == null)
            {
                throw new ArgumentNullException(nameof(internalCharacteristics));
            }

            return ContinuousUniform.Sample(internalCharacteristics.InfluenceabilityRateMin,
                internalCharacteristics.InfluenceabilityRateMax);
        }

        /// <summary>
        ///     Be influenced beliefId from agentAgentId
        /// </summary>
        /// <param name="beliefId"></param>
        /// <param name="beliefBits">from agentAgentId beliefBits</param>
        /// <param name="agentId"></param>
        /// <param name="beliefLevel"></param>
        public void BeInfluenced(IAgentId beliefId, Bits beliefBits, IAgentId agentId, BeliefLevel beliefLevel)
        {
            if (!On || beliefBits == null)
            {
                return;
            }

            // Learning From agent
            if (!_whitePages.ExistsAgent(agentId))
            {
                return;
            }

            var influentialness = _whitePages.GetAgent<CognitiveAgent>(agentId).InfluenceModel.Influentialness;
            // to Learner
            // Learner learn beliefId from agentAgentId with a weight of influenceability * influentialness
            _beliefsModel.Learn(beliefId, beliefBits, Influenceability * influentialness, beliefLevel);
        }

        public void ReinforcementByDoing(IAgentId beliefId, byte beliefBit, BeliefLevel beliefLevel)
        {
            if (!On || !_beliefsModel.On)
            {
                return;
            }

            _beliefsModel.LearnNewBelief(beliefId, beliefLevel);
            var agentBelief = _beliefsModel.GetActorBelief(beliefId);
            agentBelief.Learn(_model, beliefBit);
        }
    }
}