#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Symu.Common;
using Symu.Common.Interfaces;
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.Repository.Entity;

#endregion

namespace Symu.Repository.Networks.Knowledges
{
    /// <summary>
    ///     Knowledge network
    ///     Who (agentId) knows what (knowledge)
    ///     Key => the agentId
    ///     Value : the list of NetworkInformation the agent knows
    /// </summary>
    /// <example></example>
    public class KnowledgeNetwork
    {
        /// <summary>
        ///     Describe the knowledge model :
        ///     How to generate knowledge Network
        /// </summary>
        public RandomGenerator Model { get; set; } = RandomGenerator.RandomBinary;

        /// <summary>
        ///     Repository of all the knowledges used during the simulation
        /// </summary>
        public KnowledgeCollection Repository { get; } = new KnowledgeCollection();

        /// <summary>
        ///     List
        ///     Key => ComponentId
        ///     Values => AgentExpertise : list of KnowledgeIds/KnowledgeBits/KnowledgeLevel of an agent
        /// </summary>
        public ConcurrentDictionary<IAgentId, AgentExpertise> AgentKnowledgeNetwork { get; } =
            new ConcurrentDictionary<IAgentId, AgentExpertise>();

        public bool Any()
        {
            return AgentKnowledgeNetwork.Any();
        }

        public void Clear()
        {
            Repository.Clear();
            AgentKnowledgeNetwork.Clear();
        }

        #region Knowledge repository

        public IKnowledge GetKnowledge(IId knowledgeId)
        {
            return Repository.GetKnowledge(knowledgeId);
        }
        public TKnowledge GetKnowledge<TKnowledge>(IId knowledgeId) where TKnowledge : IKnowledge
        {
            return (TKnowledge) GetKnowledge(knowledgeId);
        }

        /// <summary>
        ///     Add a Knowledge to the repository
        ///     Should be called only by NetWork, not directly to add belief in parallel
        /// </summary>
        public void AddKnowledge(IKnowledge knowledge)
        {
            if (Repository.Contains(knowledge))
            {
                return;
            }

            Repository.Add(knowledge);
        }

        /// <summary>
        ///     Add a set of Knowledge to the repository
        /// </summary>
        public void AddKnowledges(IEnumerable<IKnowledge> knowledges)
        {
            if (knowledges is null)
            {
                throw new ArgumentNullException(nameof(knowledges));
            }

            foreach (var knowledge in knowledges)
            {
                AddKnowledge(knowledge);
            }
        }

        #endregion

        #region Agent Knowledge

        public bool Exists(IAgentId agentId, IId knowledgeId)
        {
            return Exists(agentId) && AgentKnowledgeNetwork[agentId].Contains(knowledgeId);
        }

        public bool Exists(IAgentId agentId)
        {
            return AgentKnowledgeNetwork.ContainsKey(agentId);
        }

        public void Add(IAgentId agentId, AgentExpertise expertise)
        {
            if (expertise is null)
            {
                throw new ArgumentNullException(nameof(expertise));
            }

            AddAgentId(agentId, expertise);


            foreach (var agentKnowledge in expertise.List.Where(a => !AgentKnowledgeNetwork[agentId].Contains(a)))
            {
                AgentKnowledgeNetwork[agentId].Add(agentKnowledge);
            }
        }

        public void Add(IAgentId agentId, IAgentKnowledge agentKnowledge)
        {
            AddAgentId(agentId);
            AddKnowledge(agentId, agentKnowledge);
        }

        /// <summary>
        ///     Add a knowledge to an AgentId
        ///     AgentId is supposed to be already present in the collection.
        ///     if not use Add method
        /// </summary>
        /// <param name="agentKnowledge"></param>
        /// <param name="agentId"></param>
        public void AddKnowledge(IAgentId agentId, IAgentKnowledge agentKnowledge)
        {
            if (agentKnowledge == null)
            {
                throw new ArgumentNullException(nameof(agentKnowledge));
            }

            if (!Exists(agentId, agentKnowledge.KnowledgeId))
            {
                AgentKnowledgeNetwork[agentId].Add(agentKnowledge);
            }
        }

        public void AddAgentId(IAgentId agentId, AgentExpertise agentExpertise)
        {
            if (!Exists(agentId))
            {
                AgentKnowledgeNetwork.TryAdd(agentId, agentExpertise);
            }
        }

        public void AddAgentId(IAgentId agentId)
        {
            if (!Exists(agentId))
            {
                AgentKnowledgeNetwork.TryAdd(agentId, new AgentExpertise());
            }
        }

        public IEnumerable<IAgentId> FilterAgentsWithKnowledge(IEnumerable<IAgentId> agentIds, IId knowledgeId)
        {
            if (agentIds is null)
            {
                throw new ArgumentNullException(nameof(agentIds));
            }

            return agentIds.Where(agentId => Exists(agentId) && AgentKnowledgeNetwork[agentId].Contains(knowledgeId))
                .ToList();
        }

        public IEnumerable<IId> GetKnowledgeIds(IAgentId agentId)
        {
            if (!Exists(agentId))
            {
                throw new NullReferenceException(nameof(agentId));
            }

            return AgentKnowledgeNetwork[agentId].GetKnowledgeIds();
        }

        public void RemoveAgent(IAgentId agentId)
        {
            AgentKnowledgeNetwork.TryRemove(agentId, out _);
        }

        /// <summary>
        ///     Get Agent Expertise
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns>NullReferenceException if agentId don't Exists, AgentExpertise otherwise</returns>
        public AgentExpertise GetAgentExpertise(IAgentId agentId)
        {
            if (!Exists(agentId))
            {
                throw new NullReferenceException(nameof(agentId));
            }

            return AgentKnowledgeNetwork[agentId];
        }

        /// <summary>
        ///     Get Agent Knowledge
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="knowledgeId"></param>
        /// <returns>null if agentId don't Exists, AgentExpertise otherwise</returns>
        public IAgentKnowledge GetAgentKnowledge(IAgentId agentId, IId knowledgeId)
        {
            if (!Exists(agentId, knowledgeId))
            {
                throw new NullReferenceException(nameof(agentId));
            }

            return AgentKnowledgeNetwork[agentId].GetAgentKnowledge(knowledgeId);
        }

        /// <summary>
        ///     Get Agent Knowledge
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="knowledgeId"></param>
        /// <returns>null if agentId don't Exists, AgentExpertise otherwise</returns>
        public TAgentKnowledge GetAgentKnowledge<TAgentKnowledge>(IAgentId agentId, IId knowledgeId) where TAgentKnowledge : IAgentKnowledge
        {
            return (TAgentKnowledge)GetAgentKnowledge(agentId, knowledgeId);
        }

        #endregion
    }
}