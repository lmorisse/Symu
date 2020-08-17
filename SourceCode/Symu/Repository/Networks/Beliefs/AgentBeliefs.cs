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
        public List<AgentBelief> List { get; } = new List<AgentBelief>();

        public int Count => List.Count;

        public void Add(AgentBelief agentBelief)
        {
            if (!Contains(agentBelief))
            {
                List.Add(agentBelief);
            }
        }

        public void Add(ushort beliefId, BeliefLevel beliefLevel)
        {
            var agentBelief = new AgentBelief(beliefId, beliefLevel);
            Add(agentBelief);
        }

        public bool Contains(AgentBelief agentBelief)
        {
            if (agentBelief is null)
            {
                throw new ArgumentNullException(nameof(agentBelief));
            }

            return Contains(agentBelief.BeliefId);
        }

        public bool Contains(ushort beliefId)
        {
            return List.Exists(x => x.BeliefId == beliefId);
        }

        public AgentBelief GetBelief(ushort beliefId)
        {
            return List.Find(x => x.BeliefId == beliefId);
        }

        /// <summary>
        ///     Check that agent has the BeliefId[knowledgeBit] == 1
        /// </summary>
        /// <param name="beliefId"></param>
        /// <param name="beliefBit"></param>
        /// <param name="beliefThreshHoldForAnswer"></param>
        /// <returns>true if the agent has the knowledge</returns>
        public bool BelievesEnough(ushort beliefId, byte beliefBit, float beliefThreshHoldForAnswer)
        {
            if (!Contains(beliefId))
            {
                return false;
            }

            var belief = GetBelief(beliefId);
            return belief.BelievesEnough(beliefBit, beliefThreshHoldForAnswer);
        }

        /// <summary>
        ///     Get the sum of all the beliefs
        /// </summary>
        /// <returns></returns>
        public float GetBeliefsSum()
        {
            return List.Sum(l => l.GetBeliefSum());
        }

        /// <summary>
        ///     Get the maximum potential of all the beliefs
        /// </summary>
        /// <returns></returns>
        public float GetBeliefsPotential()
        {
            return List.Sum(l => l.GetBeliefPotential());
        }


        /// <summary>
        ///     Get all the belief Ids of an agent
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ushort> GetBeliefIds()
        {
            return List.Select(x => x.BeliefId);
        }
    }
}