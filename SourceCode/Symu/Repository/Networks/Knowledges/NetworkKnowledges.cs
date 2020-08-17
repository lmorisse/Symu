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
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Common;
using Symu.Common.Interfaces;

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
    public class NetworkKnowledges
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
        public ConcurrentDictionary<IAgentId, AgentExpertise> AgentsRepository { get; } =
            new ConcurrentDictionary<IAgentId, AgentExpertise>();

        public bool Any()
        {
            return AgentsRepository.Any();
        }

        public void Clear()
        {
            Repository.Clear();
            AgentsRepository.Clear();
        }

        #region Knowledge repository

        public Knowledge GetKnowledge(ushort knowledgeId)
        {
            return Repository.GetKnowledge(knowledgeId);
        }

        /// <summary>
        ///     Add a Knowledge to the repository
        ///     Should be called only by NetWork, not directly to add belief in parallel
        /// </summary>
        public void AddKnowledge(Knowledge knowledge)
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
        public void AddKnowledges(IEnumerable<Knowledge> knowledges)
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

        public bool Exists(IAgentId agentId, ushort knowledgeId)
        {
            return Exists(agentId) && AgentsRepository[agentId].Contains(knowledgeId);
        }

        public bool Exists(IAgentId agentId)
        {
            return AgentsRepository.ContainsKey(agentId);
        }

        public void Add(IAgentId agentId, AgentExpertise expertise)
        {
            if (expertise is null)
            {
                throw new ArgumentNullException(nameof(expertise));
            }

            AddAgentId(agentId);

            foreach (var agentKnowledge in expertise.List.Where(a => !AgentsRepository[agentId].Contains(a)))
            {
                AgentsRepository[agentId].Add(agentKnowledge);
            }
        }

        public void Add(IAgentId agentId, ushort knowledgeId, KnowledgeLevel level,
            InternalCharacteristics internalCharacteristics)
        {
            if (internalCharacteristics == null)
            {
                throw new ArgumentNullException(nameof(internalCharacteristics));
            }

            AddAgentId(agentId);
            AddKnowledge(agentId, knowledgeId, level, internalCharacteristics.MinimumRemainingKnowledge,
                internalCharacteristics.TimeToLive);
        }

        public void Add(IAgentId agentId, ushort knowledgeId, KnowledgeLevel level, float minimumKnowledge,
            short timeToLive)
        {
            AddAgentId(agentId);
            AddKnowledge(agentId, knowledgeId, level, minimumKnowledge, timeToLive);
        }

        /// <summary>
        ///     Add a knowledge to an AgentId
        ///     AgentId is supposed to be already present in the collection.
        ///     if not use Add method
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="knowledgeId"></param>
        /// <param name="level"></param>
        /// <param name="minimumKnowledge"></param>
        /// <param name="timeToLive"></param>
        public void AddKnowledge(IAgentId agentId, ushort knowledgeId, KnowledgeLevel level, float minimumKnowledge,
            short timeToLive)
        {
            if (!Exists(agentId, knowledgeId))
            {
                AgentsRepository[agentId].Add(knowledgeId, level, minimumKnowledge, timeToLive);
            }
        }

        public void AddAgentId(IAgentId agentId)
        {
            if (!Exists(agentId))
            {
                AgentsRepository.TryAdd(agentId, new AgentExpertise());
            }
        }

        /// <summary>
        ///     Initialize AgentExpertise with a stochastic process based on the agent knowledge level
        /// </summary>
        /// <param name="agentId">agentId's expertise to initialize</param>
        /// <param name="neutral">if !HasInitialKnowledge, then a neutral (KnowledgeLevel.NoKnowledge) initialization is done</param>
        /// <param name="step"></param>
        public void InitializeExpertise(IAgentId agentId, bool neutral, ushort step)
        {
            if (!Exists(agentId))
            {
                throw new NullReferenceException(nameof(agentId));
            }

            foreach (var agentKnowledge in AgentsRepository[agentId].List)
            {
                InitializeAgentKnowledge(agentKnowledge, neutral, step);
            }
        }

        /// <summary>
        ///     Initialize AgentExpertise with a stochastic process based on the agent knowledge level
        /// </summary>
        /// <param name="agentKnowledge">AgentKnowledge to initialize</param>
        /// <param name="neutral">if !HasInitialKnowledge, then a neutral (KnowledgeLevel.NoKnowledge) initialization is done</param>
        /// <param name="step"></param>
        public void InitializeAgentKnowledge(AgentKnowledge agentKnowledge, bool neutral, ushort step)
        {
            if (agentKnowledge is null)
            {
                throw new ArgumentNullException(nameof(agentKnowledge));
            }

            var knowledge = GetKnowledge(agentKnowledge.KnowledgeId);
            if (knowledge == null)
            {
                throw new ArgumentNullException(nameof(knowledge));
            }

            var level = neutral ? KnowledgeLevel.NoKnowledge : agentKnowledge.KnowledgeLevel;
            var knowledgeBits = knowledge.InitializeBits(Model, level);
            agentKnowledge.SetKnowledgeBits(knowledgeBits, step);
        }

        /// <summary>
        ///     Agent don't have still this Knowledge, it's time to create one
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="knowledgeId"></param>
        /// <param name="minimumKnowledge"></param>
        /// <param name="timeToLive"></param>
        /// <param name="step"></param>
        public void LearnNewKnowledge(IAgentId agentId, ushort knowledgeId, float minimumKnowledge, short timeToLive,
            ushort step)
        {
            if (Exists(agentId, knowledgeId))
            {
                return;
            }

            AddAgentId(agentId);
            AddKnowledge(agentId, knowledgeId, KnowledgeLevel.NoKnowledge, minimumKnowledge, timeToLive);
            var agentKnowledge = GetAgentKnowledge(agentId, knowledgeId);
            InitializeAgentKnowledge(agentKnowledge, true, step);
        }

        public IEnumerable<IAgentId> FilterAgentsWithKnowledge(IEnumerable<IAgentId> agentIds, ushort knowledgeId)
        {
            if (agentIds is null)
            {
                throw new ArgumentNullException(nameof(agentIds));
            }

            return agentIds.Where(agentId => Exists(agentId) && AgentsRepository[agentId].Contains(knowledgeId))
                .ToList();
        }

        public IEnumerable<ushort> GetKnowledgeIds(IAgentId agentId)
        {
            if (!Exists(agentId))
            {
                throw new NullReferenceException(nameof(agentId));
            }

            return AgentsRepository[agentId].GetKnowledgeIds();
        }

        public void RemoveAgent(IAgentId agentId)
        {
            AgentsRepository.TryRemove(agentId, out _);
        }

        /// <summary>
        ///     Get Agent Expertise
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns>null if agentId don't Exists, AgentExpertise otherwise</returns>
        public AgentExpertise GetAgentExpertise(IAgentId agentId)
        {
            if (!Exists(agentId))
            {
                throw new NullReferenceException(nameof(agentId));
            }

            return AgentsRepository[agentId];
        }

        /// <summary>
        ///     Get Agent Knowledge
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="knowledgeId"></param>
        /// <returns>null if agentId don't Exists, AgentExpertise otherwise</returns>
        public AgentKnowledge GetAgentKnowledge(IAgentId agentId, ushort knowledgeId)
        {
            if (!Exists(agentId, knowledgeId))
            {
                throw new NullReferenceException(nameof(agentId));
            }

            return AgentsRepository[agentId].GetKnowledge(knowledgeId);
        }

        #endregion
    }
}