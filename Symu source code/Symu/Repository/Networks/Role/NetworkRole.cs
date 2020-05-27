#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Classes.Agents;

#endregion

namespace Symu.Repository.Networks.Role
{
    public class NetworkRole
    {
        public NetworkRole(AgentId agentId, AgentId groupId, byte roleType)
        {
            AgentId = agentId;
            GroupId = groupId;
            RoleType = roleType;
        }

        /// <summary>
        ///     Unique key of the agent
        /// </summary>
        public AgentId AgentId { get; }

        /// <summary>
        ///     Unique key of the group
        /// </summary>
        public AgentId GroupId { get; set; }

        /// <summary>
        ///     An agent may have different role type in a group
        /// </summary>
        public byte RoleType { get; set; }

        public bool IsMemberOfGroups(AgentId teammateId, byte groupClassKey)
        {
            return GroupId.ClassKey == groupClassKey && IsAgent(teammateId);
        }

        /// <summary>
        ///     CHeck that there is a role of roleType for that groupId
        /// </summary>
        /// <param name="roleType"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool HasRoleInGroup(byte roleType, AgentId groupId)
        {
            return RoleType == roleType && IsGroup(groupId);
        }

        public bool HasRoleInGroup(AgentId agentId, byte roleType, AgentId groupId)
        {
            return RoleType == roleType && IsAgent(agentId) && IsGroup(groupId);
        }

        public bool HasRoleInGroup(AgentId agentId, AgentId groupId)
        {
            return IsAgent(agentId) && IsGroup(groupId);
        }

        /// <summary>
        ///     CHeck that there is a role of roleType for that groupId
        /// </summary>
        /// <param name="roleType"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bool HasRole(AgentId agentId, byte roleType)
        {
            return RoleType == roleType && IsAgent(agentId);
        }

        public bool HasRole(byte roleType)
        {
            return RoleType == roleType;
        }

        public bool IsGroup(AgentId groupId)
        {
            return GroupId.Equals(groupId);
        }

        public bool IsAgent(AgentId agentId)
        {
            return AgentId.Equals(agentId);
        }
    }
}