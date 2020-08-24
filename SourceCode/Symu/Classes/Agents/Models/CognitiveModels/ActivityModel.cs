#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using Symu.Repository.Entity;
using Symu.Repository.Networks;
using Symu.Repository.Networks.Activities;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace Symu.Classes.Agents.Models.CognitiveModels
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
        private readonly ActivityNetwork _networkActivities;

        /// <summary>
        ///     Initialize influence model :
        ///     update networkInfluences
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="cognitiveArchitecture"></param>
        /// <param name="network"></param>
        public ActivityModel(AgentId agentId, CognitiveArchitecture cognitiveArchitecture, MetaNetwork network)
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
            _networkActivities = network.Activities;
        }

        /// <summary>
        ///     Get all the activities of an agent
        /// </summary>
        public IEnumerable<IActivity> Activities => _networkActivities.GetGroupActivities(_agentId);

        /// <summary>
        ///     Add a list of activities an agent can perform
        /// </summary>
        /// <param name="activities"></param>
        public void AddActivities(IEnumerable<Activity> activities)
        {
            _networkActivities.AddActivities(activities, _agentId);
        }

        /// <summary>
        ///     List of the activities on which an agent is working, filtered by groupId
        /// </summary>
        public IEnumerable<IActivity> GetGroupActivities(AgentId groupId)
        {
            return _networkActivities.GetAgentActivities(_agentId, groupId);
        }

        /// <summary>
        ///     Add all the groupId's activities to the AgentId, filtered by the agentId's knowledges
        /// </summary>
        /// <param name="activities"></param>
        /// <param name="groupId"></param>
        public void AddActivities(AgentId groupId, IEnumerable<IActivity> activities)
        {
            _networkActivities.AddActivities(_agentId, groupId, activities);
        }

        /// <summary>
        ///     Get the all the knowledges for all the activities of an agent
        /// </summary>
        /// <returns></returns>
        public IDictionary<IActivity, List<IKnowledge>>
            GetActivitiesKnowledgesByActivity()
        {
            return _networkActivities.GetActivitiesKnowledgesByActivity(_agentId);
        }
    }
}