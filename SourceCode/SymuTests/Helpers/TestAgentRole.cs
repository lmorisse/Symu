#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents;
using Symu.Common.Interfaces;
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.DNA.Resources;
using Symu.DNA.Roles;
using Symu.Environment;
using Symu.Repository;

#endregion

namespace SymuTests.Helpers
{
    /// <summary>
    ///     Class for tests
    /// </summary>
    internal sealed class TestAgentRole : IAgentRole
    {
        public TestAgentRole(IAgentId agentId, IAgentId groupId, byte role)
        {
            AgentId = agentId;
            GroupId = groupId;
            Role = new TestRole(role);
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
        public IRole Role { get; set; }
        public bool IsMemberOfGroups(IAgentId teammateId, IClassId groupClassId)
        {
            return GroupId.Equals(groupClassId) && IsAgent(teammateId);
        }

        /// <summary>
        ///     CHeck that there is a role of roleType for that groupId
        /// </summary>
        /// <param name="role"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool HasRoleInGroup(IRole role, IAgentId groupId)
        {
            return Role.Equals(role) && IsGroup(groupId);
        }

        public bool HasRoleInGroup(IAgentId agentId, IRole role, IAgentId groupId)
        {
            return Role.Equals(role) && IsAgent(agentId) && IsGroup(groupId);
        }

        public bool HasRoleInGroup(IAgentId agentId, IAgentId groupId)
        {
            return IsAgent(agentId) && IsGroup(groupId);
        }

        /// <summary>
        ///     CHeck that there is a role of roleType for that groupId
        /// </summary>
        /// <param name="role"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bool HasRole(IAgentId agentId, IRole role)
        {
            return Role.Equals(role) && IsAgent(agentId);
        }

        public bool HasRole(IRole role)
        {
            return Role.Equals(role);
        }

        public bool IsGroup(IAgentId groupId)
        {
            return GroupId.Equals(groupId);
        }

        public bool IsAgent(IAgentId agentId)
        {
            return AgentId.Equals(agentId);
        }
        public IAgentRole Clone()
        {
            return new TestAgentRole(AgentId, GroupId, ((TestRole)Role).Role);
        }
    }
}