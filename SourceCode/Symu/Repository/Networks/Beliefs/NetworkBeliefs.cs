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
using Symu.Repository.Networks.Knowledges;

#endregion

namespace Symu.Repository.Networks.Beliefs
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
        public NetworkBeliefs(BeliefWeightLevel beliefWeightLevel)
        {
            BeliefWeightLevel = beliefWeightLevel;
        }

        /// <summary>
        ///     Impact level of agent's belief on how agent will accept to do the task
        /// </summary>
        public BeliefWeightLevel BeliefWeightLevel { get; set; }

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
        public ConcurrentDictionary<IAgentId, AgentBeliefs> AgentsRepository { get; } =
            new ConcurrentDictionary<IAgentId, AgentBeliefs>();

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

            var belief = new Belief(knowledge.Id, knowledge.Name, knowledge.Length, Model, BeliefWeightLevel);
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

        public bool Exists(IAgentId agentId)
        {
            return AgentsRepository.ContainsKey(agentId);
        }

        public bool Exists(IAgentId agentId, ushort beliefId)
        {
            return Exists(agentId) && AgentsRepository[agentId].Contains(beliefId);
        }

        public void Add(IAgentId agentId, Belief belief, BeliefLevel beliefLevel)
        {
            if (belief is null)
            {
                throw new ArgumentNullException(nameof(belief));
            }

            AddAgentId(agentId);
            AddBelief(agentId, belief.Id, beliefLevel);
        }

        public void Add(IAgentId agentId, ushort beliefId, BeliefLevel beliefLevel)
        {
            AddAgentId(agentId);
            AddBelief(agentId, beliefId, beliefLevel);
        }

        /// <summary>
        ///     Add a Belief to an AgentId
        ///     AgentId is supposed to be already present in the collection.
        ///     if not use Add method
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="beliefId"></param>
        /// <param name="beliefLevel"></param>
        public void AddBelief(IAgentId agentId, ushort beliefId, BeliefLevel beliefLevel)
        {
            if (!AgentsRepository[agentId].Contains(beliefId))
            {
                AgentsRepository[agentId].Add(beliefId, beliefLevel);
            }
        }

        public void Add(IAgentId agentId, AgentExpertise expertise, BeliefLevel beliefLevel)
        {
            if (expertise is null)
            {
                throw new ArgumentNullException(nameof(expertise));
            }

            AddAgentId(agentId);

            foreach (var agentKnowledge in expertise.List)
            {
                AddBelief(agentId, agentKnowledge.KnowledgeId, beliefLevel);
            }
        }

        public void AddAgentId(IAgentId agentId)
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
        public void InitializeBeliefs(IAgentId agentId, bool neutral)
        {
            if (!Exists(agentId))
            {
                throw new NullReferenceException(nameof(agentId));
            }

            foreach (var agentBelief in AgentsRepository[agentId].List)
            {
                InitializeAgentBelief(agentBelief, neutral);
            }
        }

        /// <summary>
        ///     Initialize AgentBelief with a stochastic process based on the agent belief level
        /// </summary>
        /// <param name="agentBelief">agentBelief to initialize</param>
        /// <param name="neutral">if !HasInitialBelief, then a neutral initialization is done</param>
        public void InitializeAgentBelief(AgentBelief agentBelief, bool neutral)
        {
            if (agentBelief == null)
            {
                throw new ArgumentNullException(nameof(agentBelief));
            }

            var belief = GetBelief(agentBelief.BeliefId);
            if (belief == null)
            {
                throw new NullReferenceException(nameof(belief));
            }

            var level = neutral ? BeliefLevel.NoBelief : agentBelief.BeliefLevel;
            var beliefBits = belief.InitializeBits(Model, level);
            agentBelief.SetBeliefBits(beliefBits);
        }

        public void RemoveAgent(IAgentId agentId)
        {
            AgentsRepository.TryRemove(agentId, out _);
        }

        /// <summary>
        ///     Get Agent beliefs
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns>null if agentId don't Exists, AgentBelief otherwise</returns>
        public AgentBeliefs GetAgentBeliefs(IAgentId agentId)
        {
            if (!Exists(agentId))
            {
                throw new NullReferenceException(nameof(agentId));
            }

            return AgentsRepository[agentId];
        }

        public IEnumerable<ushort> GetBeliefIds(IAgentId agentId)
        {
            if (!Exists(agentId))
            {
                throw new NullReferenceException(nameof(agentId));
            }

            return AgentsRepository[agentId].GetBeliefIds();
        }

        /// <summary>
        ///     Get Agent belief
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="beliefId"></param>
        /// <returns>null if agentId don't Exists, AgentBelief otherwise</returns>
        public AgentBelief GetAgentBelief(IAgentId agentId, ushort beliefId)
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
        /// <param name="beliefLevel"></param>
        public void Learn(IAgentId agentId, ushort beliefId, Bits beliefBits, float influenceWeight,
            BeliefLevel beliefLevel)
        {
            LearnNewBelief(agentId, beliefId, beliefLevel);
            GetAgentBelief(agentId, beliefId).Learn(beliefBits, influenceWeight);
        }

        /// <summary>
        ///     Agent don't have still this belief, it's time to learn a new one
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="beliefId"></param>
        /// <param name="beliefLevel"></param>
        public void LearnNewBelief(IAgentId agentId, ushort beliefId, BeliefLevel beliefLevel)
        {
            if (Exists(agentId, beliefId))
            {
                return;
            }

            Add(agentId, beliefId, beliefLevel);
            InitializeBeliefs(agentId, true);
        }

        #endregion
    }
}