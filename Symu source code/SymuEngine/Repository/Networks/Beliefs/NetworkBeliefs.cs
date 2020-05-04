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
using SymuEngine.Common;
using SymuEngine.Repository.Networks.Knowledges;

#endregion

namespace SymuEngine.Repository.Networks.Beliefs
{
    /// <summary>
    ///     Belief network
    ///     Who (agentId) knows what (Belief)
    ///     Key => the agentId
    ///     Value : the list of NetworkInformation the agent knows
    /// </summary>
    /// <example></example>
    public class NetworkBeliefs
    {
        public RandomGenerator Model { get; set; } = new RandomGenerator();

        /// <summary>
        ///     Repository of all the Beliefs used during the simulation
        /// </summary>
        public BeliefCollection Repository { get; } = new BeliefCollection();

        /// <summary>
        ///     List
        ///     Key => ComponentId
        ///     Values => AgentBelief : list of BeliefIds/BeliefBits/BeliefLevel of an agent
        /// </summary>
        public ConcurrentDictionary<AgentId, AgentBeliefs> AgentsRepository { get; } =
            new ConcurrentDictionary<AgentId, AgentBeliefs>();

        public int Count => AgentsRepository.Count;

        public bool Any()
        {
            return AgentsRepository.Any();
        }

        public void Clear()
        {
            Repository.Clear();
            AgentsRepository.Clear();
        }

        #region Beliefs repository

        public Belief GetBelief(ushort beliefId)
        {
            return Repository.GetBelief(beliefId);
        }

        /// <summary>
        ///     Add a Belief to the repository based on a knowledge
        /// </summary>
        public void AddBelief(Knowledge knowledge)
        {
            if (knowledge is null)
            {
                throw new ArgumentNullException(nameof(knowledge));
            }

            var belief = new Belief(knowledge.Id, knowledge.Length, Model);
            if (Exists(belief))
            {
                return;
            }

            Repository.Add(belief);
        }

        /// <summary>
        ///     Add a Belief to the repository
        /// </summary>
        public void AddBelief(Belief belief)
        {
            if (Exists(belief))
            {
                return;
            }

            Repository.Add(belief);
        }

        /// <summary>
        ///     Add a set of Beliefs to the repository
        /// </summary>
        public void AddBeliefs(IEnumerable<Knowledge> knowledges)
        {
            if (knowledges is null)
            {
                throw new ArgumentNullException(nameof(knowledges));
            }

            foreach (var knowledge in knowledges)
            {
                AddBelief(knowledge);
            }
        }

        public bool Exists(Belief belief)
        {
            return Repository.Contains(belief);
        }

        public bool Exists(ushort beliefId)
        {
            return Repository.Exists(beliefId);
        }

        #endregion

        #region Agent Beliefs

        public bool Exists(AgentId agentId)
        {
            return AgentsRepository.ContainsKey(agentId);
        }

        public bool Exists(AgentId agentId, ushort beliefId)
        {
            return Exists(agentId) && AgentsRepository[agentId].Contains(beliefId);
        }

        public void Add(AgentId agentId, Belief belief)
        {
            if (belief is null)
            {
                throw new ArgumentNullException(nameof(belief));
            }

            AddAgentId(agentId);
            AddBelief(agentId, belief.Id);
        }

        public void Add(AgentId agentId, ushort beliefId)
        {
            AddAgentId(agentId);
            AddBelief(agentId, beliefId);
        }

        /// <summary>
        ///     Add a Belief to an AgentId
        ///     AgentId is supposed to be already present in the collection.
        ///     if not use Add method
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="beliefId"></param>
        public void AddBelief(AgentId agentId, ushort beliefId)
        {
            if (!AgentsRepository[agentId].Contains(beliefId))
            {
                AgentsRepository[agentId].Add(beliefId);
            }
        }

        public void Add(AgentId agentId, AgentExpertise expertise)
        {
            if (expertise is null)
            {
                throw new ArgumentNullException(nameof(expertise));
            }

            AddAgentId(agentId);

            foreach (var agentKnowledge in expertise.List)
            {
                AddBelief(agentId, agentKnowledge.KnowledgeId);
            }
        }

        public void AddAgentId(AgentId agentId)
        {
            if (!Exists(agentId))
            {
                AgentsRepository.TryAdd(agentId, new AgentBeliefs());
            }
        }

        /// <summary>
        ///     Initialize AgentBelief with a stochastic process
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="neutral"></param>
        public void InitializeBeliefs(AgentId agentId, bool neutral)
        {
            if (!Exists(agentId))
            {
                throw new NullReferenceException(nameof(agentId));
            }

            foreach (var agentBelief in AgentsRepository[agentId].List)
            {
                var belief = GetBelief(agentBelief.BeliefId);
                if (belief == null)
                {
                    throw new NullReferenceException(nameof(belief));
                }

                agentBelief.InitializeBeliefBits(Model, belief.Length, neutral);
            }
        }

        public void RemoveAgent(AgentId agentId)
        {
            AgentsRepository.TryRemove(agentId, out _);
        }

        /// <summary>
        ///     Get Agent beliefs
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns>null if agentId don't Exists, AgentBelief otherwise</returns>
        public AgentBeliefs GetAgentBeliefs(AgentId agentId)
        {
            if (!Exists(agentId))
            {
                throw new NullReferenceException(nameof(agentId));
            }

            return AgentsRepository[agentId];
        }

        /// <summary>
        ///     Get Agent belief
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="beliefId"></param>
        /// <returns>null if agentId don't Exists, AgentBelief otherwise</returns>
        public AgentBelief GetAgentBelief(AgentId agentId, ushort beliefId)
        {
            var agentBeliefs = GetAgentBeliefs(agentId);
            if (agentBeliefs is null)
            {
                throw new NullReferenceException(nameof(agentBeliefs));
            }

            return agentBeliefs.GetBelief(beliefId);
        }

        /// <summary>
        ///     agent learn beliefId with a weight of influenceability * influentialness
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="beliefId"></param>
        /// <param name="beliefBits"></param>
        /// <param name="influenceWeight"></param>
        public void Learn(AgentId agentId, ushort beliefId, Bits beliefBits, float influenceWeight)
        {
            LearnNewBelief(agentId, beliefId);
            GetAgentBelief(agentId, beliefId).Learn(beliefBits, influenceWeight);
        }

        /// <summary>
        ///     Agent don't have still this belief, it's time to learn a new one
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="beliefId"></param>
        public void LearnNewBelief(AgentId agentId, ushort beliefId)
        {
            if (Exists(agentId, beliefId))
            {
                return;
            }

            Add(agentId, beliefId);
            InitializeBeliefs(agentId, true);
        }

        #endregion
    }
}