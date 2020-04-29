#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SymuEngine.Classes.Agent;
using SymuEngine.Classes.Agent.Models.CognitiveArchitecture.Knowledge;
using SymuEngine.Repository.Networks.Knowledge.Agent;
using SymuEngine.Repository.Networks.Knowledge.Bits;
using SymuEngine.Repository.Networks.Knowledge.Repository;

#endregion

namespace SymuEngine.Repository.Networks.Knowledge
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
        public KnowledgeModel Model { get; set; } = new KnowledgeModel();

        /// <summary>
        ///     Repository of all the knowledges used during the simulation
        /// </summary>
        public Knowledges Repository { get; } = new Knowledges();

        /// <summary>
        ///     List
        ///     Key => ComponentId
        ///     Values => AgentExpertise : list of KnowledgeIds/KnowledgeBits/KnowledgeLevel of an agent
        /// </summary>
        public ConcurrentDictionary<AgentId, AgentExpertise> AgentsRepository { get; } =
            new ConcurrentDictionary<AgentId, AgentExpertise>();

        public bool Any()
        {
            return AgentsRepository.Any();
        }

        public void Clear()
        {
            Repository.Clear();
            AgentsRepository.Clear();
        }

        #region Knowledges repository

        public Repository.Knowledge GetKnowledge(ushort knowledgeId)
        {
            return Repository.GetKnowledge(knowledgeId);
        }

        /// <summary>
        ///     Add a Knowledge to the repository
        ///     Should be called only by NetWork, not directly to add belief in parallel
        /// </summary>
        public void AddKnowledge(Repository.Knowledge knowledge)
        {
            if (Repository.Contains(knowledge))
            {
                return;
            }

            Repository.Add(knowledge);
        }

        /// <summary>
        ///     Add a set of Knowledges to the repository
        /// </summary>
        public void AddKnowledges(IEnumerable<Repository.Knowledge> knowledges)
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

        #region Agent Knowledges

        public bool Exists(AgentId agentId, ushort knowledgeId)
        {
            return Exists(agentId) && AgentsRepository[agentId].Contains(knowledgeId);
        }

        public bool Exists(AgentId agentId)
        {
            return AgentsRepository.ContainsKey(agentId);
        }

        public void Add(AgentId agentId, AgentExpertise expertise)
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

        public void Add(AgentId agentId, Repository.Knowledge knowledge, KnowledgeLevel level)
        {
            if (knowledge is null)
            {
                throw new ArgumentNullException(nameof(knowledge));
            }

            AddAgentId(agentId);
            AddKnowledge(agentId, knowledge.Id, level);
        }

        /// <summary>
        ///     Add a knowledge to an AgentId
        ///     AgentId is supposed to be already present in the collection.
        ///     if not use Add method
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="knowledgeId"></param>
        /// <param name="level"></param>
        public void AddKnowledge(AgentId agentId, ushort knowledgeId, KnowledgeLevel level)
        {
            if (!Exists(agentId, knowledgeId))
            {
                AgentsRepository[agentId].Add(knowledgeId, level);
            }
        }

        public void AddAgentId(AgentId agentId)
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
        public void InitializeExpertise(AgentId agentId, bool neutral, ushort step)
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
            var knowledgeBits = knowledge.GetAgentBits(Model, level);
            agentKnowledge.SetKnowledgeBits(knowledgeBits, step);
        }

        /// <summary>
        ///     Agent don't have still this Knowledge, it's time to create one
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="knowledgeId"></param>
        /// <param name="step"></param>
        public void LearnNewKnowledge(AgentId agentId, ushort knowledgeId, ushort step)
        {
            if (Exists(agentId, knowledgeId))
            {
                return;
            }

            AddAgentId(agentId);
            AddKnowledge(agentId, knowledgeId, KnowledgeLevel.NoKnowledge);
            var agentKnowledge = GetAgentKnowledge(agentId, knowledgeId);
            InitializeAgentKnowledge(agentKnowledge, true, step);
        }

        public IEnumerable<AgentId> FilterAgentsWithKnowledge(IEnumerable<AgentId> agentIds, ushort knowledgeId)
        {
            if (agentIds is null)
            {
                throw new ArgumentNullException(nameof(agentIds));
            }

            return agentIds.Where(agentId => Exists(agentId) && AgentsRepository[agentId].Contains(knowledgeId))
                .ToList();
        }

        public IEnumerable<ushort> GetKnowledgeIds(AgentId agentId)
        {
            if (!Exists(agentId))
            {
                throw new NullReferenceException(nameof(agentId));
            }

            //return new List<ushort>();
            return AgentsRepository[agentId].GetKnowledgeIds();
        }

        public void RemoveAgent(AgentId agentId)
        {
            AgentsRepository.TryRemove(agentId, out _);
        }

        /// <summary>
        ///     Get Agent Expertise
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns>null if agentId don't Exists, AgentExpertise otherwise</returns>
        public AgentExpertise GetAgentExpertise(AgentId agentId)
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
        public AgentKnowledge GetAgentKnowledge(AgentId agentId, ushort knowledgeId)
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