#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Common.Interfaces.Agent;
using Symu.DNA;
using Symu.DNA.Activities;
using Symu.DNA.Agent;
using Symu.DNA.Beliefs;
using Symu.DNA.Groups;
using Symu.DNA.Knowledges;
using Symu.DNA.Resources;
using Symu.DNA.Roles;
using Symu.DNA.TwoModesNetworks.Interactions;
using Symu.DNA.TwoModesNetworks.Sphere;
using Symu.Repository.Networks.Enculturation;
using Symu.Repository.Networks.Events;

#endregion

namespace Symu.Repository.Networks
{
    /// <summary>
    ///     MetaNetwork: referential of networks for social and organizational network analysis
    /// </summary>
    public class SymuMetaNetwork
    {
        public SymuMetaNetwork(InteractionSphereModel interactionSphere)
        {
            Network = new MetaNetwork(interactionSphere);
        }

        public MetaNetwork Network { get; }

        /// <summary>
        /// occurrences or phenomena that happen
        /// </summary>
        public EventNetwork Events => Network.Events;


        /// <summary>
        ///     Agent enculturation level network
        /// </summary>
        public EnculturationNetwork Enculturation { get; } = new EnculturationNetwork();

        /// <summary>
        ///     Agent enculturation level network
        /// </summary>
        public AgentNetwork Agents => Network.Agents;

        /// <summary>
        ///     Directory of social links between AgentIds, with their interaction type
        ///     Who report/communicate to who
        ///     Sphere of interaction of agents
        /// </summary>
        public InteractionNetwork Interactions => Network.Interactions;

        /// <summary>
        ///     Directory of the groups of the organizationEntity :
        ///     Team, task force, workgroup, circles, community of practices, ...
        /// </summary>
        public GroupNetwork Groups => Network.Groups;

        /// <summary>
        ///     Directory of the roles the agent are playing in the organizationEntity
        /// </summary>
        public RoleNetwork Roles => Network.Roles;

        /// <summary>
        ///     Directory of objects used by the agentIds
        ///     using, working, support
        /// </summary>
        public ResourceNetwork Resources => Network.Resources;

        /// <summary>
        ///     Knowledge network
        ///     Who (agentId) knows what (Information)
        /// </summary>
        public KnowledgeNetwork Knowledge => Network.Knowledge;

        /// <summary>
        ///     Belief network
        ///     Who (agentId) believes what (Information)
        /// </summary>
        public BeliefNetwork Beliefs => Network.Beliefs;

        /// <summary>
        ///     Kanban activities network
        ///     Who (agentId) works on what activities (Kanban)
        /// </summary>
        public ActivityNetwork Activities => Network.Activities;

        /// <summary>
        ///     Derived Parameters from others networks.
        ///     these parameters are use indirectly to change agent behavior.
        /// </summary>
        public InteractionSphere InteractionSphere => Network.InteractionSphere;


        public void Clear()
        {
            Network.Clear();
            Enculturation.Clear();
        }

        public void RemoveAgent(IAgentId agentId)
        {
            Network.RemoveAgent(agentId);
            Enculturation.RemoveAgent(agentId);
        }
    }
}