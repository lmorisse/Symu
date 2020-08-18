#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Common.Interfaces;

#endregion

namespace Symu.Repository.Networks.Role
{
    public class NetworkRole
    {
        public NetworkRole(IAgentId agentId, IAgentId groupId, byte roleType)
        {
            AgentId = agentId;
            GroupId = groupId;
            RoleType = roleType;
        }

        /// <summary>
        ///     Unique key of the agent
        /// </summary>
        public IAgentId AgentId { get; }

        /// <summary>
        ///     Unique key of the group
        /// </summary>
        public IAgentId GroupId { get; set; }

        /// <summary>
        ///     An agent may have different role type in a group
        /// </summary>
        public byte RoleType { get; set; }

        public bool IsMemberOfGroups(IAgentId teammateId, IClassId groupClassId)
        {
            return GroupId.Equals(groupClassId) && IsAgent(teammateId);
        }

        /// <summary>
        ///     CHeck that there is a role of roleType for that groupId
        /// </summary>
        /// <param name="roleType"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool HasRoleInGroup(byte roleType, IAgentId groupId)
        {
            return RoleType == roleType && IsGroup(groupId);
        }

        public bool HasRoleInGroup(IAgentId agentId, byte roleType, IAgentId groupId)
        {
            return RoleType == roleType && IsAgent(agentId) && IsGroup(groupId);
        }

        public bool HasRoleInGroup(IAgentId agentId, IAgentId groupId)
        {
            return IsAgent(agentId) && IsGroup(groupId);
        }

        /// <summary>
        ///     CHeck that there is a role of roleType for that groupId
        /// </summary>
        /// <param name="roleType"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bool HasRole(IAgentId agentId, byte roleType)
        {
            return RoleType == roleType && IsAgent(agentId);
        }

        public bool HasRole(byte roleType)
        {
            return RoleType == roleType;
        }

        public bool IsGroup(IAgentId groupId)
        {
            return GroupId.Equals(groupId);
        }

        public bool IsAgent(IAgentId agentId)
        {
            return AgentId.Equals(agentId);
        }
    }
}