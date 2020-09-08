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
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.DNA;
using Symu.DNA.Networks;
using Symu.DNA.Networks.OneModeNetworks;
using Symu.DNA.Networks.TwoModesNetworks;
using Symu.Repository.Entity;

#endregion

namespace Symu.Classes.Agents.Models.CognitiveModels
{
    /// <summary>
    ///     CognitiveArchitecture define how an actor will perform task
    ///     Entity enable or not this mechanism for all the agents during the simulation
    ///     The ActivityModel initialize the real value of the agent's activity parameters
    /// </summary>
    /// <remarks>From Construct Software</remarks>
    public class ResourceTaskModel
    {
        private readonly IId _resourceId;
        private readonly ResourceTaskNetwork _resourceTaskNetwork;

        /// <summary>
        ///     Initialize influence model :
        ///     update networkInfluences
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="network"></param>
        public ResourceTaskModel(IId resourceId, MetaNetwork network)
        {
            if (network == null)
            {
                throw new ArgumentNullException(nameof(network));
            }

            _resourceId = resourceId;
            _resourceTaskNetwork = network.ResourceTask;
        }

        /// <summary>
        ///     Get all the tasks (activities) that an agent can do
        /// </summary>
        public IEnumerable<ITask> Tasks => _resourceTaskNetwork.GetValues(_resourceId);
        /// <summary>
        ///     Get the all the knowledges for all the tasks of an agent
        /// </summary>
        /// <returns></returns>
        public IDictionary<ITask, IEnumerable<IKnowledge>> Knowledge
        {
            get
            {
                var knowledge = new Dictionary<ITask, IEnumerable<IKnowledge>>();
                foreach (var task in Tasks)
                {
                    knowledge.Add(task,task.Knowledges);
                }

                return knowledge;
            }
        }

        /// <summary>
        ///     Add an activity to an agent can perform
        /// </summary>
        /// <param name="task"></param>
        public void AddResourceTask(ITask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            _resourceTaskNetwork.Add(_resourceId, task);
        }

        /// <summary>
        ///     Add a list of activities an agent can perform
        /// </summary>
        /// <param name="tasks"></param>
        public void AddResourceTasks(IEnumerable<ITask> tasks)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            foreach (var task in tasks)
            {
                _resourceTaskNetwork.Add(_resourceId, task);
            }
        }
    }
}