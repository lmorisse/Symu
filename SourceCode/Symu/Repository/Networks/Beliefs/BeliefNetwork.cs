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
using Symu.DNA.Beliefs;
using Symu.DNA.Knowledges;
using Symu.Repository.Entity;

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
    public class BeliefNetwork
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

        public IBelief GetBelief(IId beliefId)
        {
            return Repository.GetBelief(beliefId);
        }
        public TBelief GetBelief<TBelief>(IId beliefId) where TBelief : IBelief
        {
            return (TBelief)GetBelief(beliefId);
        }

        /// <summary>
        ///     Add a Belief to the repository
        /// </summary>
        public void AddBelief(IBelief belief)
        {
            if (Exists(belief))
            {
                return;
            }

            Repository.Add(belief);
        }

        public bool Exists(IBelief belief)
        {
            return Repository.Contains(belief);
        }

        public bool Exists(IId beliefId)
        {
            return Repository.Exists(beliefId);
        }

        #endregion

        #region Agent Beliefs

        public bool Exists(IAgentId agentId)
        {
            return AgentsRepository.ContainsKey(agentId);
        }

        public bool Exists(IAgentId agentId, IId beliefId)
        {
            return Exists(agentId) && AgentsRepository[agentId].Contains(beliefId);
        }

        public void Add(IAgentId agentId, IAgentBelief agentBelief)
        {
            AddAgentId(agentId);
            AddBelief(agentId, agentBelief);
        }

        /// <summary>
        ///     Add a Belief to an AgentId
        ///     AgentId is supposed to be already present in the collection.
        ///     if not use Add method
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="agentBelief"></param>
        public void AddBelief(IAgentId agentId, IAgentBelief agentBelief)
        {
            if (agentBelief == null)
            {
                throw new ArgumentNullException(nameof(agentBelief));
            }

            if (!AgentsRepository[agentId].Contains(agentBelief.BeliefId))
            {
                AgentsRepository[agentId].Add(agentBelief);
            }
        }

        public void AddAgentId(IAgentId agentId)
        {
            if (!Exists(agentId))
            {
                AgentsRepository.TryAdd(agentId, new AgentBeliefs());
            }
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

        public IEnumerable<IId> GetBeliefIds(IAgentId agentId)
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
        public IAgentBelief GetAgentBelief(IAgentId agentId, IId beliefId)
        {
            var agentBeliefs = GetAgentBeliefs(agentId);
            if (agentBeliefs is null)
            {
                throw new NullReferenceException(nameof(agentBeliefs));
            }

            return agentBeliefs.GetAgentBelief(beliefId);
        }  
        
        /// <summary>
        ///     Get Agent belief
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="beliefId"></param>
        /// <returns>null if agentId don't Exists, AgentBelief otherwise</returns>
        public TAgentBelief GetAgentBelief<TAgentBelief>(IAgentId agentId, IId beliefId) where TAgentBelief : IAgentBelief
        {
            return (TAgentBelief) GetAgentBelief(agentId, beliefId);
        }
        #endregion
    }
}