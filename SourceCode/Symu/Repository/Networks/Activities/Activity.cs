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
using Symu.Repository.Networks.Knowledges;

#endregion

namespace Symu.Repository.Networks.Activities
{
    /// <summary>
    ///     Define an activity by its name and the list of knowledgeIds required by the activity
    /// </summary>
    public class Activity
    {
        public Activity(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        /// <summary>
        ///     List of knowledges required to work on this activity
        /// </summary>
        public List<Knowledge> Knowledges { get; } = new List<Knowledge>();

        public void AddKnowledge(Knowledge knowledge)
        {
            if (Knowledges.Contains(knowledge))
            {
                return;
            }

            Knowledges.Add(knowledge);
        }

        /// <summary>
        ///     Check that agent has the required knowledges to work on the activity
        /// </summary>
        /// <param name="agentKnowledgeIds"></param>
        /// <returns></returns>
        public bool CheckKnowledgeIds(List<ushort> agentKnowledgeIds)
        {
            if (agentKnowledgeIds is null)
            {
                throw new ArgumentNullException(nameof(agentKnowledgeIds));
            }

            return Knowledges.Any(knowledge => agentKnowledgeIds.Contains(knowledge.Id));
        }
    }
}