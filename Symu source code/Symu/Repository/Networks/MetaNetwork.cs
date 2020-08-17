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
using Symu.Classes.Agents;
using Symu.Classes.Organization;
using Symu.Common;
using Symu.Messaging.Templates;
using Symu.Repository.Networks.Activities;
using Symu.Repository.Networks.Beliefs;
using Symu.Repository.Networks.Databases;
using Symu.Repository.Networks.Enculturation;
using Symu.Repository.Networks.Group;
using Symu.Repository.Networks.Influences;
using Symu.Repository.Networks.Knowledges;
using Symu.Repository.Networks.Link;
using Symu.Repository.Networks.Portfolio;
using Symu.Repository.Networks.Role;
using Symu.Repository.Networks.Sphere;

#endregion

namespace Symu.Repository.Networks
{
    /// <summary>
    ///     MetaNetwork: referential of networks for social and organizational network analysis
    /// </summary>
    public class MetaNetwork
    {
        public MetaNetwork(InteractionSphereModel interactionSphere , BeliefWeightLevel beliefWeightLevel)
        {
            InteractionSphere = new InteractionSphere(interactionSphere);
            Beliefs = new NetworkBeliefs(beliefWeightLevel);
        }

        /// <summary>
        ///     Directory of social links between AgentIds, with their interaction type
        ///     Who report/communicate to who
        ///     Sphere of interaction of agents
        /// </summary>
        public NetworkLinks Links { get; } = new NetworkLinks();

        /// <summary>
        ///     Directory of the groups of the organizationEntity :
        ///     Team, task force, workgroup, circles, community of practices, ...
        /// </summary>
        public NetworkGroups Groups { get; } = new NetworkGroups();

        /// <summary>
        ///     Directory of the roles the agent are playing in the organizationEntity
        /// </summary>
        public NetworkRoles Roles { get; } = new NetworkRoles();

        /// <summary>
        ///     Directory of objects used by the agentIds
        ///     using, working, support
        /// </summary>
        public NetworkPortfolios Resources { get; } = new NetworkPortfolios();

        /// <summary>
        ///     Knowledge network
        ///     Who (agentId) knows what (Information)
        /// </summary>
        public NetworkKnowledges Knowledge { get; } = new NetworkKnowledges();

        /// <summary>
        ///     Belief network
        ///     Who (agentId) believes what (Information)
        /// </summary>
        public NetworkBeliefs Beliefs { get; }

        /// <summary>
        ///     Kanban activities network
        ///     Who (agentId) works on what activities (Kanban)
        /// </summary>
        public NetworkActivities Activities { get; } = new NetworkActivities();

        /// <summary>
        ///     Agent enculturation level network
        /// </summary>
        public NetworkEnculturation Enculturation { get; } = new NetworkEnculturation();

        /// <summary>
        ///     Agent influences network
        /// </summary>
        public NetworkInfluences Influences { get; } = new NetworkInfluences();

        /// <summary>
        ///     Communication network
        /// </summary>
        // todo should be part of resources
        public NetworkDatabases Databases { get; } = new NetworkDatabases();

        /// <summary>
        ///     Derived Parameters from others networks.
        ///     these parameters are use indirectly to change agent behavior.
        /// </summary>
        public InteractionSphere InteractionSphere { get; }

        #region Initialize & remove Agents

        public void Clear()
        {
            Links.Clear();
            Groups.Clear();
            Roles.Clear();
            Resources.Clear();
            Knowledge.Clear();
            Beliefs.Clear();
            Activities.Clear();
            Enculturation.Clear();
            Influences.Clear();
            Databases.Clear();
        }

        public void RemoveAgent(AgentId agentId)
        {
            Links.RemoveAgent(agentId);
            Groups.RemoveAgent(agentId);
            Roles.RemoveAgent(agentId);
            Resources.RemoveAgent(agentId);
            Knowledge.RemoveAgent(agentId);
            Activities.RemoveAgent(agentId);
            Beliefs.RemoveAgent(agentId);
            Enculturation.RemoveAgent(agentId);
            Influences.RemoveAgent(agentId);
            Databases.RemoveAgent(agentId);
        }

        #endregion

        #region Methods having crossed impacts on networks

        /// <summary>
        ///     Add an agent to a group
        ///     It doesn't handle roles
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="allocation"></param>
        /// <param name="groupId"></param>
        /// <param name="addLink">If true, a link is created with the members of the group and the new member.
        /// AddLink is done during the simulation. During the initialization, use InitializeNetworkLinks, for performance issues</param>
        public void AddAgentToGroup(AgentId agentId, float allocation, AgentId groupId, bool addLink)
        {
            lock (Groups)
            {
                Groups.AddGroup(groupId);
                if (addLink)
                {
                    foreach (var newTeammateId in Groups.GetAgents(groupId, agentId.ClassKey))
                    {
                        Links.AddLink(agentId, newTeammateId);
                    }
                }

                Groups.AddAgent(agentId, allocation, groupId);
            }

            Resources.AddMemberToGroup(agentId, groupId);
        }

        /// <summary>
        ///     Initialize the network links.
        ///     For performance it is not done in AddMemberToGroup at initialization
        /// </summary>
        public void InitializeNetworkLinks()
        {
            foreach (var groupId in Groups.GetGroups().ToList())
            {
                Links.AddLinks(Groups.GetAgents(groupId, SymuYellowPages.Actor).ToList());
            }
        }

        /// <summary>
        ///     Remove an agent to a group
        ///     It doesn't handle roles
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupId"></param>
        public void RemoveAgentFromGroup(AgentId agentId, AgentId groupId)
        {
            if (!Groups.Exists(groupId))
            {
                return;
            }

            foreach (var oldTeammateId in Groups.GetAgents(groupId, agentId.ClassKey))
            {
                Links.DeactivateLink(agentId, oldTeammateId);
            }

            Groups.RemoveMember(agentId, groupId);
            Roles.RemoveMember(agentId, groupId);
            Resources.RemoveMemberFromGroup(agentId, groupId);

            // Remove all the groupId activities to the AgentId
            Activities.RemoveMember(agentId, groupId);
        }

        /// <summary>
        ///     Add a Knowledge to the repository
        /// </summary>
        /// <param name="knowledge"></param>
        public void AddKnowledge(Knowledge knowledge)
        {
            Knowledge.AddKnowledge(knowledge);
            Beliefs.AddBelief(knowledge);
        }

        /// <summary>
        ///     Add a set of Knowledge to the repository
        /// </summary>
        public void AddKnowledges(IEnumerable<Knowledge> knowledge)
        {
            var knowledges = knowledge.ToList();
            Knowledge.AddKnowledges(knowledges);
            Beliefs.AddBeliefs(knowledges);
        }

        #endregion


    }
}