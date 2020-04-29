#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using SymuEngine.Repository.Networks.Knowledge.Bits;

#endregion

namespace SymuEngine.Repository.Networks.Knowledge.Agent
{
    /// <summary>
    ///     Expertise of an agent is defined by the list of all its knowledge (hard skills)  x KnowledgeLevel
    /// </summary>
    /// <example>Dev Java, test, project management, sociology, ...</example>
    public class AgentExpertise
    {
        /// <summary>
        ///     Key => ComponentId
        ///     Values => List of Knowledge
        /// </summary>
        public List<AgentKnowledge> List { get; } = new List<AgentKnowledge>();

        public int Count => List.Count;

        public bool Any()
        {
            return List.Any();
        }

        public void Clear()
        {
            List.Clear();
        }

        public void Add(AgentKnowledge agentKnowledge)
        {
            if (!Contains(agentKnowledge))
            {
                List.Add(agentKnowledge);
            }
        }

        public void Add(ushort knowledgeId, KnowledgeLevel level)
        {
            var agentKnowledge = new AgentKnowledge(knowledgeId, level);
            Add(agentKnowledge);
        }

        public bool Contains(AgentKnowledge agentKnowledge)
        {
            if (agentKnowledge is null)
            {
                throw new ArgumentNullException(nameof(agentKnowledge));
            }

            return Contains(agentKnowledge.KnowledgeId);
        }

        public bool Contains(ushort knowledgeId)
        {
            return List.Exists(x => x.KnowledgeId == knowledgeId);
        }

        public AgentKnowledge GetKnowledge(ushort knowledgeId)
        {
            return List.Find(x => x.KnowledgeId == knowledgeId);
        }

        /// <summary>
        ///     Get the sum of all the knowledges
        /// </summary>
        /// <returns></returns>
        public float GetKnowledgesSum()
        {
            return List.Sum(l => l.GetKnowledgeSum());
        }

        /// <summary>
        ///     Get all the knowledge of an agent
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ushort> GetKnowledgeIds()
        {
            return List.Select(x => x.KnowledgeId);
        }

        /// <summary>
        ///     Check that agent has the knowledgeId[knowledgeBit] == 1
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="knowledgeBit"></param>
        /// <param name="knowledgeThreshHoldForAnswer"></param>
        /// <param name="step"></param>
        /// <returns>true if the agent has the knowledge</returns>
        public bool KnowsEnough(ushort knowledgeId, byte knowledgeBit, float knowledgeThreshHoldForAnswer, ushort step)
        {
            if (!Contains(knowledgeId))
            {
                return false;
            }

            var knowledge = GetKnowledge(knowledgeId);
            return knowledge.KnowsEnough(knowledgeBit, knowledgeThreshHoldForAnswer, step);
        }

        /// <summary>
        ///     Forget knowledges from the expertise based on knowledgeBits.LastTouched and timeToLive value
        /// </summary>
        public void ForgettingProcess(short timeToLive, float forgettingRate, float minimumRemainingLevel, ushort step)
        {
            List.ForEach(x => x.ForgettingProcess(timeToLive, forgettingRate, minimumRemainingLevel, step));
        }
    }
}