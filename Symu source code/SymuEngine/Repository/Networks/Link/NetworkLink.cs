#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using SymuEngine.Classes.Agent;

#endregion

namespace SymuEngine.Repository.Networks.Link
{
    /// <summary>
    ///     Sphere of interaction
    /// </summary>
    public class NetworkLink
    {
        public NetworkLink(AgentId agentId1, AgentId agentId2)
        {
            AgentId1 = agentId1;
            AgentId2 = agentId2;
        }

        /// <summary>
        ///     Unique key of the agent
        /// </summary>
        public AgentId AgentId1 { get; }

        /// <summary>
        ///     Unique key of the agent
        /// </summary>
        public AgentId AgentId2 { get; }

        /// <summary>
        ///     State of the Link : Active / Passive
        /// </summary>
        public NetworkLinkState State { get; private set; } = NetworkLinkState.Active;

        public bool IsActive => State == NetworkLinkState.Active;
        public bool IsPassive => State == NetworkLinkState.Passive;

        public void Activate()
        {
            State = NetworkLinkState.Active;
        }

        public void Deactivate()
        {
            State = NetworkLinkState.Passive;
        }

        public bool HasActiveLink(AgentId agentId1, AgentId agentId2)
        {
            return HasLink(agentId1, agentId2) && IsActive;
        }

        public bool HasPassiveLink(AgentId agentId1, AgentId agentId2)
        {
            return HasLink(agentId1, agentId2) && IsPassive;
        }

        public bool HasLink(AgentId agentId1, AgentId agentId2)
        {
            return AgentId1.Equals(agentId1) && AgentId2.Equals(agentId2) ||
                   AgentId1.Equals(agentId2) && AgentId2.Equals(agentId1);
        }

        public override bool Equals(object obj)
        {
            return obj is NetworkLink link &&
                   AgentId1.Equals(link.AgentId1) && AgentId2.Equals(link.AgentId2);
        }

        public bool HasActiveLinks(AgentId agentId)
        {
            return IsActive && AgentId1.Equals(agentId);
        }

        public bool HasActiveLinks(AgentId agentId, byte groupClassKey)
        {
            return AgentId2.ClassKey == groupClassKey && HasActiveLinks(agentId);
        }
    }
}