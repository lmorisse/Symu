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
using Symu.Common.Interfaces.Entity;
using Symu.Repository.Entity;

#endregion

namespace Symu.Repository.Networks.Beliefs
{
    /// <summary>
    ///     Expertise of an agent is defined by the list of all its knowledge (hard skills)  x KnowledgeLevel
    /// </summary>
    /// <example>Dev Java, test, project management, sociology, ...</example>
    public class AgentBeliefs
    {
        /// <summary>
        ///     Key => ComponentId
        ///     Values => List of Knowledge
        /// </summary>
        public List<IAgentBelief> List { get; } = new List<IAgentBelief>();

        public int Count => List.Count;

        public void Add(IAgentBelief agentBelief)
        {
            if (!Contains(agentBelief))
            {
                List.Add(agentBelief);
            }
        }

        public bool Contains(IAgentBelief agentBelief)
        {
            if (agentBelief is null)
            {
                throw new ArgumentNullException(nameof(agentBelief));
            }

            return Contains(agentBelief.BeliefId);
        }

        public bool Contains(IId beliefId)
        {
            return List.Exists(x => x.BeliefId.Equals(beliefId));
        }

        public IAgentBelief GetAgentBelief(IId beliefId)
        {
            return List.Find(x => x.BeliefId.Equals(beliefId));
        }

        public TAgentBelief GetAgentBelief<TAgentBelief>(IId beliefId) where TAgentBelief : IAgentBelief
        {
            return (TAgentBelief)GetAgentBelief(beliefId);
        }

        public IEnumerable<TAgentBelief> GetAgentBeliefs<TAgentBelief>() where TAgentBelief : IAgentBelief
        {
            return List.Cast<TAgentBelief>();
        }

        /// <summary>
        ///     Get all the belief Ids of an agent
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IId> GetBeliefIds()
        {
            return List.Select(x => x.BeliefId);
        }
    }
}