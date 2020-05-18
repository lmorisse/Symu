﻿#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Repository.Networks;
using SymuEngine.Repository.Networks.Beliefs;
using SymuEngine.Repository.Networks.Influences;
using SymuEngine.Repository.Networks.Knowledges;
using SymuTools.Math.ProbabilityDistributions;

#endregion

namespace SymuEngine.Classes.Agents.Models.CognitiveModel
{
    /// <summary>
    ///     CognitiveArchitecture define how an actor will influence or be influenced
    ///     Entity enable or not this mechanism for all the agents during the simulation
    ///     The InfluenceModel initialize the real value of the agent's influence parameters
    /// </summary>
    /// <remarks>From Construct Software</remarks>
    public class InfluenceModel : ModelEntity
    {
        private readonly AgentId _agentId;
        private readonly NetworkBeliefs _networkBeliefs;
        private readonly NetworkInfluences _networkInfluences;
        /// <summary>
        /// Initialize influence model :
        /// update networkInfluences
        /// </summary>
        /// <param name="agentAgentId"></param>
        /// <param name="entity"></param>
        /// <param name="internalCharacteristics"></param>
        /// <param name="network"></param>
        public InfluenceModel(AgentId agentAgentId, ModelEntity entity, InternalCharacteristics internalCharacteristics, Network network)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (internalCharacteristics == null)
            {
                throw new ArgumentNullException(nameof(internalCharacteristics));
            }

            if (network == null)
            {
                throw new ArgumentNullException(nameof(network));
            }

            _agentId = agentAgentId;
            _networkInfluences = network.NetworkInfluences;
            _networkBeliefs = network.NetworkBeliefs;

            if (internalCharacteristics.CanInfluenceOrBeInfluence && entity.IsAgentOn())
            {
                _networkInfluences.Add(agentAgentId,
                    NextInfluenceability(internalCharacteristics),
                    NextInfluentialness(internalCharacteristics));
            }
            else
            {
                _networkInfluences.Add(agentAgentId,0,0);
            }

        }

        /// <summary>
        ///     Set the Influentialness for a specific agent with a random value between [InfluentialnessRateMin,
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
        ///     Set the Influentialness for a specific agent with a random value between [InfluentialnessRateMin,
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
        public void BeInfluenced(ushort beliefId, Bits beliefBits, AgentId agentId, BeliefLevel beliefLevel)
        {
            //if (beliefId == 0 || beliefBits == null)
            //{
            //    return;
            //}

            if (beliefBits == null)// && beliefId > 0)
            {
                return;
                //throw new ArgumentNullException(nameof(beliefBits));
            }

            // Learning From agent
            var influentialness = _networkInfluences.GetInfluentialness(agentId);
            // to Learner
            var influenceability = _networkInfluences.GetInfluenceability(_agentId);
            // Learner learn beliefId from agentAgentId with a weight of influenceability * influentialness
            _networkBeliefs.Learn(_agentId, beliefId, beliefBits, influenceability * influentialness, beliefLevel);
        }

        public void ReinforcementByDoing(ushort beliefId, byte beliefBit, BeliefLevel beliefLevel)
        {
            if (!_networkBeliefs.Exists(_agentId, beliefId))
            {
                _networkBeliefs.LearnNewBelief(_agentId, beliefId, beliefLevel);
            }

            var agentBelief = _networkBeliefs.GetAgentBelief(_agentId, beliefId);
            agentBelief.Learn(_networkBeliefs.Model, beliefBit);
        }
    }
}