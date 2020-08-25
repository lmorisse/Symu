#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

using Symu.Common.Interfaces.Agent;

namespace Symu.Repository.Networks.Roles
{
    public interface IAgentRole
    {
        /// <summary>
        ///     Unique key of the agent
        /// </summary>
        IAgentId AgentId { get; }

        /// <summary>
        ///     Unique key of the group
        /// </summary>
        IAgentId GroupId { get; set; }

        /// <summary>
        ///     An agent may have different role type in a group
        /// </summary>
        IRole Role { get; set; }

        bool IsMemberOfGroups(IAgentId teammateId, IClassId groupClassId);

        /// <summary>
        ///     CHeck that there is a role of roleType for that groupId
        /// </summary>
        /// <param name="role"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        bool HasRoleInGroup(IRole role, IAgentId groupId);

        bool HasRoleInGroup(IAgentId agentId, IRole role, IAgentId groupId);
        bool HasRoleInGroup(IAgentId agentId, IAgentId groupId);

        /// <summary>
        ///     CHeck that there is a role of roleType for that groupId
        /// </summary>
        /// <param name="role"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        bool HasRole(IAgentId agentId, IRole role);

        bool HasRole(IRole role);
        bool IsGroup(IAgentId groupId);
        bool IsAgent(IAgentId agentId);
        IAgentRole Clone();
    }
}