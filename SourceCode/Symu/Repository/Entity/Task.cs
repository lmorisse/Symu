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
using Symu.DNA.Networks.OneModeNetworks;
using Symu.DNA.Networks.TwoModesNetworks;

#endregion

namespace Symu.Repository.Entity
{
    /// <summary>
    ///     Default implementation of IActivity
    ///     Define an activity by its name and the list of knowledgeIds required by the activity
    /// </summary>
    public class Task : ITask
    {
        private readonly TaskKnowledgeNetwork _taskKnowledgeNetwork;
        public Task(ushort id, string name, TaskKnowledgeNetwork taskKnowledgeNetwork)
        {
            Id = new UId(id);
            Name = name;
            _taskKnowledgeNetwork = taskKnowledgeNetwork;
        }
        public Task(IId id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Name { get; set; }

        /// <summary>
        ///     Unique identifier of the activity
        /// </summary>
        public IId Id { get; }

        /// <summary>
        ///     List of knowledges required to work on this activity
        /// </summary>
        public List<IKnowledge> Knowledges => _taskKnowledgeNetwork.GetValues(Id).ToList();//{ get; } = new List<IKnowledge>();


        public void AddKnowledge(IKnowledge knowledge)
        {
            //if (Knowledges.Contains(knowledge))
            //{
            //    return;
            //}

            //Knowledges.Add(knowledge);
            _taskKnowledgeNetwork.Add(Id, knowledge);
        }

        /// <summary>
        ///     Check that agent has the required knowledges to work on the activity
        /// </summary>
        /// <param name="agentKnowledgeIds"></param>
        /// <returns></returns>
        public bool CheckKnowledgeIds(List<IId> agentKnowledgeIds)
        {
            if (agentKnowledgeIds is null)
            {
                throw new ArgumentNullException(nameof(agentKnowledgeIds));
            }

            return Knowledges.Any(knowledge => agentKnowledgeIds.Contains(knowledge.Id));
        }

        public bool Equals(ITask task)
        {
            return task is Task act &&
                   Id.Equals(act.Id);
        }
    }
}