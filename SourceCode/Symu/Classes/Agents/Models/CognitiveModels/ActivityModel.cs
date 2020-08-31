#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.DNA;
using Symu.DNA.Activities;
using Symu.DNA.Knowledges;
using Symu.Repository.Entity;
using Symu.Repository.Networks;

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
        private readonly IAgentId _agentId;
        private readonly ActivityNetwork _networkActivities;

        /// <summary>
        ///     Initialize influence model :
        ///     update networkInfluences
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="cognitiveArchitecture"></param>
        /// <param name="network"></param>
        public ActivityModel(IAgentId agentId, CognitiveArchitecture cognitiveArchitecture, MetaNetwork network)
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
        public void AddActivities(IEnumerable<IActivity> activities)
        {
            _networkActivities.AddActivities(activities, _agentId);
        }

        /// <summary>
        ///     List of the activities on which an agent is working, filtered by groupId
        /// </summary>
        public IEnumerable<IActivity> GetGroupActivities(IAgentId groupId)
        {
            return _networkActivities.GetActivities(_agentId, groupId);
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