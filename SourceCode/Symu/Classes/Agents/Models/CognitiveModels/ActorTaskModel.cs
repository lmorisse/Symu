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
using Symu.Common.Interfaces;
using Symu.DNA;
using Symu.OrgMod.Edges;
using Symu.OrgMod.Entities;
using Symu.OrgMod.GraphNetworks;
using Symu.OrgMod.GraphNetworks.TwoModesNetworks;

#endregion

namespace Symu.Classes.Agents.Models.CognitiveModels
{
    /// <summary>
    ///     CognitiveArchitecture define how an actor will perform task
    ///     Entity enable or not this mechanism for all the agents during the simulation
    ///     The ActivityModel initialize the real value of the agent's activity parameters
    /// </summary>
    /// <remarks>From Construct Software</remarks>
    public class ActorTaskModel
    {
        private readonly IAgentId _agentId;
        private readonly ActorTaskNetwork _actorTaskNetwork;
        private readonly OneModeNetwork _taskNetwork;

        /// <summary>
        ///     Initialize influence model :
        ///     update networkInfluences
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="cognitiveArchitecture"></param>
        /// <param name="network"></param>
        public ActorTaskModel(IAgentId agentId, CognitiveArchitecture cognitiveArchitecture, GraphMetaNetwork network)
        {
            if (cognitiveArchitecture == null)
            {
                throw new ArgumentNullException(nameof(cognitiveArchitecture));
            }

            if (network == null)
            {
                throw new ArgumentNullException(nameof(network));
            }

            _agentId = agentId;
            _actorTaskNetwork = network.ActorTask;
            _taskNetwork = network.Task;
        }

        /// <summary>
        ///     Get all the tasks (activities) that an actor can do
        /// </summary>
        public IEnumerable<IAgentId> TaskIds => _actorTaskNetwork.TargetsFilteredBySource(_agentId);

        /// <summary>
        ///     Get all the tasks (activities) that an agent can do
        /// </summary>
        public IEnumerable<ITask> Tasks => TaskIds.Select(taskId => _taskNetwork.GetEntity<ITask>(taskId)).ToList();

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
                    knowledge.Add(task, task.Knowledge);
                }

                return knowledge;
            }
        }

        /// <summary>
        ///     Add an activity to an actor can perform
        /// </summary>
        /// <param name="taskId"></param>
        public void AddActorTask(IAgentId taskId)
        {
            var actorTask = new ActorTask(_agentId, taskId);
            _actorTaskNetwork.Add(actorTask);
        }

        /// <summary>
        ///     Add a list of activities an actor can perform
        /// </summary>
        /// <param name="taskIds"></param>
        public void AddActorTasks(IEnumerable<IAgentId> taskIds)
        {
            if (taskIds == null)
            {
                throw new ArgumentNullException(nameof(taskIds));
            }

            foreach (var taskId in taskIds)
            {
                AddActorTask(taskId);
            }
        }
    }
}