#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Classes.Organization;
using Symu.Common;
using Symu.Common.Interfaces;
using Symu.Common.Math.ProbabilityDistributions;
using Symu.Repository.Networks.Activities;
using Symu.Repository.Networks.Beliefs;
using Symu.Repository.Networks.Knowledges;
using Symu.Repository.Networks.Link;

#endregion

namespace Symu.Repository.Networks.Sphere
{
    /// <summary>
    ///     Array Agent * Agent of derived parameters from all networks.
    ///     Those parameters are relative parameters of an agent fro another agent.
    ///     these parameters are use indirectly to change agent behavior.
    /// </summary>
    public class InteractionSphere
    {
        private readonly InteractionSphereModel _model;
        private Dictionary<IAgentId, int> _agentIndex;
        private Dictionary<int, IAgentId> _indexAgent;
        private DerivedParameter _lastAverage;

        public InteractionSphere(InteractionSphereModel model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        /// <summary>
        ///     List of all agentId and their enculturation information
        /// </summary>
        public DerivedParameter[,] Sphere { get; private set; }

        public void SetSphere(bool initialization, List<IAgentId> agentIds, MetaNetwork network)
        {
            if (agentIds == null)
            {
                throw new ArgumentNullException(nameof(agentIds));
            }

            if (network == null)
            {
                throw new ArgumentNullException(nameof(network));
            }

            var modelOn = _model.On;
            var sphereChange = initialization || _model.SphereUpdateOverTime;
            if (!modelOn || !sphereChange)
            {
                return;
            }

            network.Links.SetMaxLinksCount();
            if (_model.RandomlyGeneratedSphere)
            {
                if (initialization)
                {
                    SetSphereRandomly(agentIds);
                }
                else
                {
                    UpdateSphereRandomly(agentIds, network);
                }
            }
            else
            {
                SetSphereWithSimilarityMatching(agentIds, network);
            }

            _lastAverage = InteractionMatrix.GetAverageInteractionMatrix(Sphere);
        }

        /// <summary>
        ///     Clone sphere randomly based on InteractionPatterns
        /// </summary>
        /// <param name="agentIds"></param>
        public void SetSphereRandomly(IReadOnlyList<IAgentId> agentIds)
        {
            if (agentIds == null)
            {
                throw new ArgumentNullException(nameof(agentIds));
            }

            var count = agentIds.Count;
            _agentIndex = new Dictionary<IAgentId, int>();
            _indexAgent = new Dictionary<int, IAgentId>();
            Sphere = new DerivedParameter[count, count];
            // for the moment the matrix is symmetrical
            for (var i = 0; i < count; i++)
            {
                var agentI = agentIds[i];
                _agentIndex[agentI] = i;
                _indexAgent[i] = agentI;

                var density = ContinuousUniform.Sample(_model.MinSphereDensity, _model.MaxSphereDensity);
                for (var j = i + 1; j < count; j++)
                {
                    var value = Bernoulli.Sample(density) ? ContinuousUniform.Sample(0F, 1F) : 0F;
                    Sphere[i, j] =
                        new DerivedParameter(_model, value, value, value, value);
                    Sphere[j, i] = Sphere[i, j];
                }
            }
        }

        /// <summary>
        ///     Update sphere randomly based on InteractionPatterns with new agent
        /// </summary>
        /// <param name="agentIds"></param>
        /// <param name="network"></param>
        public void UpdateSphereRandomly(IReadOnlyList<IAgentId> agentIds, MetaNetwork network)
        {
            if (agentIds == null)
            {
                throw new ArgumentNullException(nameof(agentIds));
            }

            if (network == null)
            {
                throw new ArgumentNullException(nameof(network));
            }

            var count = agentIds.Count;
            var tempAgentIndex = new Dictionary<IAgentId, int>();
            var tempIndexAgent = new Dictionary<int, IAgentId>();
            var tempSphere = new DerivedParameter[count, count];
            for (var i = 0; i < count; i++)
            {
                var agentI = agentIds[i];
                tempAgentIndex[agentI] = i;
                tempIndexAgent[i] = agentI;
                if (_agentIndex.ContainsKey(agentI))
                {
                    var oldIndexI = _agentIndex[agentI];
                    for (var j = i + 1; j < count; j++)
                    {
                        var agentJ = agentIds[j];
                        if (_agentIndex.ContainsKey(agentJ))
                        {
                            var oldIndexJ = _agentIndex[agentJ];

                            var tempDerivedParameter = SetDerivedParameter(network, agentI, agentJ);
                            var socialProximity = Math.Max(tempDerivedParameter.SocialDemographic,
                                Sphere[oldIndexI, oldIndexJ].SocialDemographic);
                            var relativeBelief = Math.Max(tempDerivedParameter.RelativeBelief,
                                Sphere[oldIndexI, oldIndexJ].RelativeBelief);
                            var relativeKnowledge = Math.Max(tempDerivedParameter.RelativeKnowledge,
                                Sphere[oldIndexI, oldIndexJ].RelativeKnowledge);
                            var relativeActivity = Math.Max(tempDerivedParameter.RelativeActivity,
                                Sphere[oldIndexI, oldIndexJ].RelativeActivity);

                            tempSphere[i, j] = new DerivedParameter(socialProximity, relativeBelief, relativeKnowledge,
                                relativeActivity);
                        }
                        else
                        {
                            //new agent
                            var density = ContinuousUniform.Sample(_model.MinSphereDensity, _model.MaxSphereDensity);
                            var value = Bernoulli.Sample(density) ? ContinuousUniform.Sample(0F, 1F) : 0F;
                            tempSphere[i, j] =
                                new DerivedParameter(_model, value, value, value, value);
                        }

                        tempSphere[j, i] = tempSphere[i, j];
                    }
                }
                else
                {
                    // new agent
                    var density = ContinuousUniform.Sample(_model.MinSphereDensity, _model.MaxSphereDensity);
                    for (var j = i + 1; j < count; j++)
                    {
                        var value = Bernoulli.Sample(density) ? ContinuousUniform.Sample(0F, 1F) : 0F;
                        tempSphere[i, j] =
                            new DerivedParameter(_model, value, value,
                                value, value);
                        tempSphere[j, i] = tempSphere[i, j];
                    }
                }
            }

            _agentIndex = tempAgentIndex;
            _indexAgent = tempIndexAgent;
            Sphere = tempSphere;
        }

        private void SetSphereWithSimilarityMatching(IReadOnlyList<IAgentId> agentIds, MetaNetwork network)
        {
            var count = agentIds.Count;
            _agentIndex = new Dictionary<IAgentId, int>();
            _indexAgent = new Dictionary<int, IAgentId>();
            Sphere = new DerivedParameter[count, count];
            // for the moment it is a symmetrical matrix
            for (var i = 0; i < count; i++)
            {
                var agentI = agentIds[i];
                _agentIndex[agentI] = i;
                _indexAgent[i] = agentI;
                for (var j = i + 1; j < count; j++)
                {
                    var agentJ = agentIds[j];
                    Sphere[i, j] = SetDerivedParameter(network, agentI, agentJ);
                    Sphere[j, i] = Sphere[i, j];
                }
            }
        }

        private DerivedParameter SetDerivedParameter(MetaNetwork network, IAgentId agentI, IAgentId agentJ)
        {
            var socialProximity = _model.SocialDemographicWeight > Constants.Tolerance
                ? SetSocialProximity(agentI, agentJ, network.Links)
                : 0;

            var relativeBelief = _model.RelativeBeliefWeight > Constants.Tolerance
                ? SetRelativeBelief(agentI, agentJ, network.Beliefs)
                : 0;
            var relativeKnowledge = _model.RelativeKnowledgeWeight > Constants.Tolerance
                ? SetRelativeKnowledge(agentI, agentJ, network.Knowledge)
                : 0;
            var relativeActivity = _model.RelativeActivityWeight > Constants.Tolerance
                ? SetRelativeActivity(agentI, agentJ, network.Activities)
                : 0;

            var derivedParameter =
                new DerivedParameter(_model, socialProximity, relativeBelief, relativeKnowledge, relativeActivity);
            return derivedParameter;
        }

        /// <summary>
        ///     The closer two agents are in the belief area, the more likely they will be to interact.
        /// </summary>
        public static float SetRelativeBelief(IAgentId agentId1, IAgentId agentId2, NetworkBeliefs networkBeliefs)
        {
            if (networkBeliefs == null)
            {
                throw new ArgumentNullException(nameof(networkBeliefs));
            }

            if (!networkBeliefs.Exists(agentId1) || !networkBeliefs.Exists(agentId2))
            {
                return 0;
            }

            var relativeBelief = 0F;
            var beliefIds = networkBeliefs.GetBeliefIds(agentId1).ToList();
            foreach (var beliefId in beliefIds)
            {
                var bits1 = networkBeliefs.GetAgentBelief(agentId1, beliefId).BeliefBits;
                if (!networkBeliefs.Exists(agentId2, beliefId))
                {
                    continue;
                }

                var bits2 = networkBeliefs.GetAgentBelief(agentId2, beliefId).BeliefBits;
                relativeBelief += Bits.GetRelativeBits(bits1, bits2);
            }

            return beliefIds.Any() ? relativeBelief / beliefIds.Count : 0;
        }

        /// <summary>
        ///     The closer two agents are in the knowledge area, the more likely they will be to interact.
        /// </summary>
        public static float SetRelativeKnowledge(IAgentId agentId1, IAgentId agentId2,
            NetworkKnowledges networkKnowledges)
        {
            if (networkKnowledges == null)
            {
                throw new ArgumentNullException(nameof(networkKnowledges));
            }

            if (!networkKnowledges.Exists(agentId1) || !networkKnowledges.Exists(agentId2))
            {
                return 0;
            }

            var relativeExpertise = 0F;
            var knowledgeIds = networkKnowledges.GetKnowledgeIds(agentId1).ToList();
            foreach (var knowledgeId in knowledgeIds)
            {
                var knowledgeBits1 = networkKnowledges.GetAgentKnowledge(agentId1, knowledgeId).KnowledgeBits;
                if (!networkKnowledges.Exists(agentId2, knowledgeId))
                {
                    continue;
                }

                var knowledgeBits2 = networkKnowledges.GetAgentKnowledge(agentId2, knowledgeId).KnowledgeBits;
                if (!knowledgeBits2.IsNull)
                {
                    relativeExpertise += Bits.GetRelativeBits(knowledgeBits1, knowledgeBits2);
                }
            }

            return knowledgeIds.Any() ? relativeExpertise / knowledgeIds.Count : 0;
        }

        /// <summary>
        ///     The closer two agents are, the more likely they will be to interact.
        /// </summary>
        /// <param name="agentId1"></param>
        /// <param name="agentId2"></param>
        /// <param name="networkLinks"></param>
        /// <returns></returns>
        public static float SetSocialProximity(IAgentId agentId1, IAgentId agentId2, NetworkLinks networkLinks)
        {
            //todo graph : number of nodes between agentId1 and agentId2
            if (networkLinks == null)
            {
                throw new ArgumentNullException(nameof(networkLinks));
            }

            return networkLinks.NormalizedCountLinks(agentId1, agentId2);
        }

        /// <summary>
        ///     The closer two agents are on this area, the more likely they will be to interact.
        /// </summary>
        /// <param name="agentId1"></param>
        /// <param name="agentId2"></param>
        /// <param name="networkActivities"></param>
        /// <returns></returns>
        public static float SetRelativeActivity(IAgentId agentId1, IAgentId agentId2, NetworkActivities networkActivities)
        {
            if (networkActivities == null)
            {
                throw new ArgumentNullException(nameof(networkActivities));
            }

            var activity1 = networkActivities.GetAgentActivities(agentId1).ToList();
            var activity2 = networkActivities.GetAgentActivities(agentId2).ToList();
            var relativeActivity = activity1.Count(activity => activity2.Contains(activity));
            return activity1.Any() ? relativeActivity / activity1.Count : 0;
        }

        /// <summary>
        ///     List of AgentId for interactions, interactions that are below above interactions (difference with
        ///     GetAgentIdsForNewInteractions)
        ///     based on the interaction strategy of the interaction patterns :
        ///     Filtered with interactionStrategy and limit with number of new interactions
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="interactionStrategy">can come from InteractionPatterns, but passed in parameter for unit test</param>
        /// <param name="interactionPatterns"></param>
        /// <returns></returns>
        public IEnumerable<IAgentId> GetAgentIdsForInteractions(IAgentId agentId, InteractionStrategy interactionStrategy,
            InteractionPatterns interactionPatterns)
        {
            if (interactionPatterns is null)
            {
                throw new ArgumentNullException(nameof(interactionPatterns));
            }

            if (!_model.On || _agentIndex is null || !_agentIndex.ContainsKey(agentId))
            {
                return new List<IAgentId>();
            }

            var agentIdDerivedParameters = new Dictionary<IAgentId, DerivedParameter>();
            var agentIndex = _agentIndex[agentId];
            for (var i = 0; i < _agentIndex.Count; i++)
            {
                if (i == agentIndex)
                {
                    continue;
                }

                switch (interactionStrategy)
                {
                    case InteractionStrategy.Homophily:
                        if (Sphere[agentIndex, i].Homophily < _lastAverage.Homophily - Constants.Tolerance ||
                            Math.Abs(Sphere[agentIndex, i].Homophily) < Constants.Tolerance)
                        {
                            continue;
                        }

                        break;
                    case InteractionStrategy.Knowledge:
                        if (Sphere[agentIndex, i].RelativeKnowledge <
                            _lastAverage.RelativeKnowledge - Constants.Tolerance ||
                            Math.Abs(Sphere[agentIndex, i].RelativeKnowledge) < Constants.Tolerance)
                        {
                            continue;
                        }

                        break;
                    case InteractionStrategy.Activities:
                        if (Sphere[agentIndex, i].RelativeActivity <
                            _lastAverage.RelativeActivity - Constants.Tolerance ||
                            Math.Abs(Sphere[agentIndex, i].RelativeActivity) < Constants.Tolerance)
                        {
                            continue;
                        }

                        break;
                    case InteractionStrategy.Beliefs:
                        if (Sphere[agentIndex, i].RelativeBelief < _lastAverage.RelativeBelief - Constants.Tolerance ||
                            Math.Abs(Sphere[agentIndex, i].RelativeBelief) < Constants.Tolerance)
                        {
                            continue;
                        }

                        break;
                    case InteractionStrategy.SocialDemographics:
                        if (Sphere[agentIndex, i].SocialDemographic <
                            _lastAverage.SocialDemographic - Constants.Tolerance ||
                            Math.Abs(Sphere[agentIndex, i].SocialDemographic) < Constants.Tolerance)
                        {
                            continue;
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(interactionStrategy), interactionStrategy, null);
                }

                agentIdDerivedParameters.Add(_indexAgent[i], Sphere[agentIndex, i]);
            }

            return OrderAgentIdsForInteractions(interactionStrategy, agentIdDerivedParameters);
        }

        private static IEnumerable<IAgentId> OrderAgentIdsForInteractions(InteractionStrategy interactionStrategy,
            Dictionary<IAgentId, DerivedParameter> agentIdDerivedParameters)
        {
            List<IAgentId> orderedAgentIds;
            switch (interactionStrategy)
            {
                case InteractionStrategy.Homophily:
                    orderedAgentIds = agentIdDerivedParameters.OrderByDescending(x => x.Value.Homophily)
                        .Select(x => x.Key)
                        .ToList();
                    break;
                case InteractionStrategy.Knowledge:
                    orderedAgentIds = agentIdDerivedParameters.OrderByDescending(x => x.Value.RelativeKnowledge)
                        .Select(x => x.Key).ToList();
                    break;
                case InteractionStrategy.Activities:
                    orderedAgentIds = agentIdDerivedParameters.OrderByDescending(x => x.Value.RelativeActivity)
                        .Select(x => x.Key).ToList();
                    break;
                case InteractionStrategy.Beliefs:
                    orderedAgentIds = agentIdDerivedParameters.OrderByDescending(x => x.Value.RelativeBelief)
                        .Select(x => x.Key)
                        .ToList();
                    break;
                case InteractionStrategy.SocialDemographics:
                    orderedAgentIds = agentIdDerivedParameters.OrderByDescending(x => x.Value.SocialDemographic)
                        .Select(x => x.Key).ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(interactionStrategy), interactionStrategy, null);
            }

            return orderedAgentIds;
        }

        /// <summary>
        ///     List of AgentId for new interactions, interactions that are below average interactions (difference with
        ///     GetAgentIdsForInteractions)
        ///     based on the interaction strategy of the interaction patterns :
        ///     Filtered with interactionStrategy and limit with number of new interactions
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="interactionStrategy">can come from InteractionPatterns, but passed in parameter for unit test</param>
        /// <returns></returns>
        public IEnumerable<IAgentId> GetAgentIdsForNewInteractions(IAgentId agentId,
            InteractionStrategy interactionStrategy)
        {
            if (_agentIndex is null)
            {
                throw new NullReferenceException("Sphere is not Setted");
            }

            if (!_model.On || !_model.SphereUpdateOverTime || !_agentIndex.ContainsKey(agentId))
            {
                return new List<IAgentId>();
            }

            var agentIdDerivedParameters = new Dictionary<IAgentId, DerivedParameter>();
            var agentIndex = _agentIndex[agentId];
            for (var i = 0; i < _agentIndex.Count; i++)
            {
                if (i == agentIndex)
                {
                    continue;
                }

                switch (interactionStrategy)
                {
                    case InteractionStrategy.Homophily:
                        if (Sphere[agentIndex, i].Homophily > _lastAverage.Homophily + Constants.Tolerance ||
                            Math.Abs(Sphere[agentIndex, i].Homophily - 1) < Constants.Tolerance)
                        {
                            continue;
                        }

                        break;
                    case InteractionStrategy.Knowledge:
                        if (Sphere[agentIndex, i].RelativeKnowledge >
                            _lastAverage.RelativeKnowledge + Constants.Tolerance ||
                            Math.Abs(Sphere[agentIndex, i].RelativeKnowledge - 1) < Constants.Tolerance)
                        {
                            continue;
                        }

                        break;
                    case InteractionStrategy.Activities:
                        if (Sphere[agentIndex, i].RelativeActivity >
                            _lastAverage.RelativeActivity + Constants.Tolerance ||
                            Math.Abs(Sphere[agentIndex, i].RelativeActivity - 1) < Constants.Tolerance)
                        {
                            continue;
                        }

                        break;
                    case InteractionStrategy.Beliefs:
                        if (Sphere[agentIndex, i].RelativeBelief > _lastAverage.RelativeBelief + Constants.Tolerance ||
                            Math.Abs(Sphere[agentIndex, i].RelativeBelief - 1) < Constants.Tolerance)
                        {
                            continue;
                        }

                        break;
                    case InteractionStrategy.SocialDemographics:
                        if (Sphere[agentIndex, i].SocialDemographic >
                            _lastAverage.SocialDemographic + Constants.Tolerance ||
                            Math.Abs(Sphere[agentIndex, i].SocialDemographic - 1) < Constants.Tolerance)
                        {
                            continue;
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(interactionStrategy), interactionStrategy, null);
                }

                agentIdDerivedParameters.Add(_indexAgent[i], Sphere[agentIndex, i]);
            }

            return OrderAgentIdsForInteractions(interactionStrategy, agentIdDerivedParameters);
        }

        /// <summary>
        ///     Get the homophily value of the AgentId2 for the AgentId1
        /// </summary>
        /// <param name="agentId1"></param>
        /// <param name="agentId2"></param>
        /// <returns>
        ///     0 if one of the agent is not yet present in the network
        ///     The homophily value if both exist.
        /// </returns>
        /// <remarks>
        ///     An agent that acts via homophily attempts to ﬁnd an _model partner that shares its characteristics.
        ///     When searching for suitable partners, the agent will stress agents who have similar socio-demographic parameters,
        ///     similar knowledge, and similar beliefs.
        ///     This process utilizes the derived parameters
        /// </remarks>
        public float GetHomophily(IAgentId agentId1, IAgentId agentId2)
        {
            if (_agentIndex == null)
            {
                throw new NullReferenceException(nameof(_agentIndex));
            }

            if (!_model.On || !_agentIndex.ContainsKey(agentId1) || !_agentIndex.ContainsKey(agentId2))
            {
                return 0;
            }

            var index1 = _agentIndex[agentId1];
            var index2 = _agentIndex[agentId2];
            return Sphere[index1, index2].Homophily;
        }

        public float GetSphereWeight()
        {
            if (!_model.On)
            {
                return 0;
            }

            // for the moment it is a symmetrical matrix
            var weight = 0F;
            for (var i = 0; i < Sphere.GetLength(0); i++)
            {
                for (var j = i + 1; j < Sphere.GetLength(1); j++)
                {
                    weight += Sphere[i, j].Homophily;
                }
            }

            return weight * 2;
        }

        public float GetMaxSphereWeight()
        {
            if (!_model.On)
            {
                return 0;
            }

            return (_model.SocialDemographicWeight + _model.RelativeBeliefWeight + _model.RelativeKnowledgeWeight +
                    _model.RelativeActivityWeight) * Sphere.GetLength(0) * (Sphere.GetLength(0) - 1);
        }
    }
}