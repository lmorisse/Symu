#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using SymuEngine.Repository.Networks;
using SymuEngine.Repository.Networks.Activities;
using SymuEngine.Repository.Networks.Beliefs;
using SymuEngine.Repository.Networks.Influences;
using SymuEngine.Repository.Networks.Knowledges;
using SymuTools.Math.ProbabilityDistributions;

#endregion

namespace SymuEngine.Classes.Agents.Models.CognitiveModel
{
    /// <summary>
    ///     CognitiveArchitecture define how an actor will perform task
    ///     Entity enable or not this mechanism for all the agents during the simulation
    ///     The ActivityModel initialize the real value of the agent's activity parameters
    /// </summary>
    /// <remarks>From Construct Software</remarks>
    public class ActivityModel
    {
        private readonly AgentId _agentId;
        private readonly NetworkActivities _networkActivities;
        private readonly TasksAndPerformance _tasksAndPerformance;
        /// <summary>
        /// Initialize influence model :
        /// update networkInfluences
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="cognitiveArchitecture"></param>
        /// <param name="network"></param>
        public ActivityModel(AgentId agentId, CognitiveArchitecture cognitiveArchitecture, Network network) 
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
            _networkActivities = network.NetworkActivities;
            _tasksAndPerformance = cognitiveArchitecture.TasksAndPerformance;
        }

        /// <summary>
        ///     Get all the activities of an agent
        /// </summary>
        public IEnumerable<string> Activities => _networkActivities.GetGroupActivities(_agentId);

        /// <summary>
        ///     Add a list of activities an agent can perform
        /// </summary>
        /// <param name="activities"></param>
        public void AddActivities(IEnumerable<Activity> activities)
        {
            if (!_tasksAndPerformance.CanPerformTask)
            {
                return;
            }

            _networkActivities.AddActivities(activities, _agentId);
        }

        /// <summary>
        ///     List of the activities on which an agent is working, filtered by groupId
        /// </summary>
        public IEnumerable<string> GetGroupActivities(AgentId groupId)
        {
            return _networkActivities.GetAgentActivities(_agentId, groupId);
        }

        /// <summary>
        ///     Add all the groupId's activities to the AgentId, filtered by the agentId's knowledges
        /// </summary>
        /// <param name="activities"></param>
        /// <param name="groupId"></param>
        public void AddActivities(AgentId groupId, IEnumerable<string> activities)
        {
            if (!_tasksAndPerformance.CanPerformTask)
            {
                return;
            }

            _networkActivities.AddActivities(_agentId, groupId, activities);
        }

        /// <summary>
        ///     Get the all the knowledges for all the activities of an agent
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, List<Knowledge>>
            GetActivitiesKnowledgesByActivity()
        {
            return _networkActivities.GetActivitiesKnowledgesByActivity(_agentId);
        }
    }
}