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
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.DNA.Activities;
using Symu.DNA.Groups;
using Symu.Repository.Networks.Beliefs;
using Symu.Repository.Networks.Enculturation;
using Symu.Repository.Networks.Influences;
using Symu.Repository.Networks.Knowledges;
using Symu.Repository.Networks.Link;
using Symu.Repository.Networks.Resources;
using Symu.Repository.Networks.Roles;
using Symu.Repository.Networks.Sphere;

#endregion

namespace Symu.Repository.Networks
{
    /// <summary>
    ///     MetaNetwork: referential of networks for social and organizational network analysis
    /// </summary>
    public class MetaNetwork
    {
        public MetaNetwork(InteractionSphereModel interactionSphere, BeliefWeightLevel beliefWeightLevel)
        {
            InteractionSphere = new InteractionSphere(interactionSphere);
            Beliefs = new BeliefNetwork(beliefWeightLevel);
        }

        /// <summary>
        ///     Directory of social links between AgentIds, with their interaction type
        ///     Who report/communicate to who
        ///     Sphere of interaction of agents
        /// </summary>
        public LinkNetwork Links { get; } = new LinkNetwork();

        /// <summary>
        ///     Directory of the groups of the organizationEntity :
        ///     Team, task force, workgroup, circles, community of practices, ...
        /// </summary>
        public GroupNetwork Groups { get; } = new GroupNetwork();

        /// <summary>
        ///     Directory of the roles the agent are playing in the organizationEntity
        /// </summary>
        public RoleNetwork Roles { get; } = new RoleNetwork();

        /// <summary>
        ///     Directory of objects used by the agentIds
        ///     using, working, support
        /// </summary>
        public ResourceNetwork Resources { get; } = new ResourceNetwork();

        /// <summary>
        ///     Knowledge network
        ///     Who (agentId) knows what (Information)
        /// </summary>
        public KnowledgeNetwork Knowledge { get; } = new KnowledgeNetwork();

        /// <summary>
        ///     Belief network
        ///     Who (agentId) believes what (Information)
        /// </summary>
        public BeliefNetwork Beliefs { get; }

        /// <summary>
        ///     Kanban activities network
        ///     Who (agentId) works on what activities (Kanban)
        /// </summary>
        public ActivityNetwork Activities { get; } = new ActivityNetwork();

        /// <summary>
        ///     Agent enculturation level network
        /// </summary>
        public EnculturationNetwork Enculturation { get; } = new EnculturationNetwork();

        /// <summary>
        ///     Agent influences network
        /// </summary>
        public InfluenceNetwork Influences { get; } = new InfluenceNetwork();

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
        }

        public void RemoveAgent(IAgentId agentId)
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
        }

        #endregion

        #region Methods having crossed impacts on networks

        /// <summary>
        ///     Add an agent to a group
        ///     It doesn't handle roles' impact
        /// </summary>
        /// <param name="agentGroup"></param>
        /// <param name="groupId"></param>
        /// <param name="addLink">
        ///     If true, a link is created with the members of the group and the new member.
        ///     AddLink is done during the simulation. During the initialization, use InitializeNetworkLinks, for performance
        ///     issues
        /// </param>
        public void AddAgentToGroup(IAgentGroup agentGroup, IAgentId groupId, bool addLink)
        {
            if (agentGroup == null)
            {
                throw new ArgumentNullException(nameof(agentGroup));
            }

            lock (Groups)
            {
                Groups.AddGroup(groupId);
                if (addLink)
                {
                    foreach (var newAgentId in Groups.GetAgents(groupId, agentGroup.AgentId.ClassId))
                    {
                        Links.AddLink(agentGroup.AgentId, newAgentId);
                    }
                }

                Groups.AddAgent(agentGroup, groupId);
            }

            Resources.AddMemberToGroup(agentGroup.AgentId, groupId);
        }

        /// <summary>
        ///     Remove an agent to a group
        ///     It doesn't handle roles
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupId"></param>
        public void RemoveAgentFromGroup(IAgentId agentId, IAgentId groupId)
        {
            if (agentId == null)
            {
                throw new ArgumentNullException(nameof(agentId));
            }

            if (!Groups.Exists(groupId))
            {
                return;
            }

            foreach (var agentIdToRemove in Groups.GetAgents(groupId, agentId.ClassId))
            {
                Links.DeactivateLink(agentId, agentIdToRemove);
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
        public void AddKnowledges(IEnumerable<IKnowledge> knowledge)
        {
            var knowledges = knowledge.ToList();
            Knowledge.AddKnowledges(knowledges);
            Beliefs.AddBeliefs(knowledges);
        }

        #endregion
    }
}