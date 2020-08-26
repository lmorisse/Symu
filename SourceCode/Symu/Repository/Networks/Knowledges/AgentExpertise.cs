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

namespace Symu.Repository.Networks.Knowledges
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
        public List<IAgentKnowledge> List { get; } = new List<IAgentKnowledge>();

        public int Count => List.Count;

        public IEnumerable<TAgentKnowledge> GetAgentKnowledges<TAgentKnowledge>() where TAgentKnowledge : IAgentKnowledge
        {
            return List.Cast<TAgentKnowledge>();
        }
        public bool Any()
        {
            return List.Any();
        }

        public void Clear()
        {
            List.Clear();
        }

        public void Add(IAgentKnowledge agentKnowledge)
        {
            if (agentKnowledge == null)
            {
                throw new ArgumentNullException(nameof(agentKnowledge));
            }

            if (Contains(agentKnowledge))
            {
                return;
            }

            List.Add(agentKnowledge);
        }

        public bool Contains(IAgentKnowledge agentKnowledge)
        {
            if (agentKnowledge is null)
            {
                throw new ArgumentNullException(nameof(agentKnowledge));
            }

            return Contains(agentKnowledge.KnowledgeId);
        }

        public bool Contains(IId knowledgeId)
        {
            return List.Exists(x => x.KnowledgeId.Equals(knowledgeId));
        }

        public IAgentKnowledge GetAgentKnowledge(IId knowledgeId)
        {
            return List.Find(x => x.KnowledgeId.Equals(knowledgeId));
        }

        public TAgentKnowledge GetAgentKnowledge<TAgentKnowledge>(IId knowledgeId) where TAgentKnowledge : IAgentKnowledge
        {
            return (TAgentKnowledge)GetAgentKnowledge(knowledgeId);
        }

        /// <summary>
        ///     Get all the knowledge of an agent
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IId> GetKnowledgeIds()
        {
            return List.Select(x => x.KnowledgeId);
        }
    }
}