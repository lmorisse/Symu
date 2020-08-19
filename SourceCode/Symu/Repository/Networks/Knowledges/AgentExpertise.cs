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
        public List<AgentKnowledge> List { get; } = new List<AgentKnowledge>();

        public int Count => List.Count;

        ///// <summary>
        /////     EventHandler triggered after learning a new information
        ///// </summary>
        //public event EventHandler<LearningEventArgs> OnAfterLearning;


        public bool Any()
        {
            return List.Any();
        }

        public void Clear()
        {
            List.Clear();
        }
        /// <summary>
        /// Add method should be called only by NetworkKnowledge, KnowledgeModel and LearningModel
        /// </summary>
        /// <param name="agentKnowledge"></param>
        public void Add(AgentKnowledge agentKnowledge)
        {
            if (agentKnowledge == null)
            {
                throw new ArgumentNullException(nameof(agentKnowledge));
            }

            if (Contains(agentKnowledge))
            {
                return;
            }

            //agentKnowledge.OnAfterLearning += AfterLearning;
            List.Add(agentKnowledge);
        }

        public void Add(ushort knowledgeId, KnowledgeLevel level, float minimumKnowledge, short timeToLive)
        {
            var agentKnowledge = new AgentKnowledge(knowledgeId, level, minimumKnowledge, timeToLive);
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
        ///     Get all the knowledge of an agent
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ushort> GetKnowledgeIds()
        {
            return List.Select(x => x.KnowledgeId);
        }

        ///// <summary>
        /////     OnAfterLearning event is triggered if learning occurs,
        /////     you can subscribe to this event to treat the new learning
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //public void AfterLearning(object sender, LearningEventArgs e)
        //{
        //    OnAfterLearning?.Invoke(this, e);
        //}
    }
}