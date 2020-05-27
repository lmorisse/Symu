#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using System.Linq;
using Symu.Classes.Agents;

#endregion

namespace Symu.Repository.Networks.Role
{
    public class NetworkRoles
    {
        public List<NetworkRole> List { get; } = new List<NetworkRole>();

        /// <summary>
        ///     Gets or sets the element at the specified index
        /// </summary>
        /// <param name="index">0 based</param>
        /// <returns></returns>
        public NetworkRole this[int index]
        {
            get => List[index];
            set => List[index] = value;
        }

        public void Clear()
        {
            List.Clear();
        }

        public void RemoveAgent(AgentId agentId)
        {
            // Remove agentId as an Agent
            List.RemoveAll(l => l.AgentId.Equals(agentId));
            // Remove agentId as a Group
            List.RemoveAll(l => l.GroupId.Equals(agentId));
        }

        public bool Any()
        {
            return List.Any();
        }

        /// <summary>
        ///     List of groupIds teammate is member of
        /// </summary>
        /// <param name="teammateId"></param>
        /// <param name="groupClassKey"></param>
        /// <returns></returns>
        public IEnumerable<AgentId> IsMemberOfGroups(AgentId teammateId, byte groupClassKey)
        {
            return List.FindAll(r => r.IsMemberOfGroups(teammateId, groupClassKey)).Select(x => x.GroupId);
        }

        /// <summary>
        ///     List of groupIds teammate is member of
        /// </summary>
        /// <param name="teammateId"></param>
        /// <param name="groupClassKey"></param>
        /// <returns></returns>
        public bool IsMember(AgentId teammateId, byte groupClassKey)
        {
            return List.Exists(r => r.IsMemberOfGroups(teammateId, groupClassKey));
        }

        public bool ExistAgentForRoleType(byte roleType, AgentId groupId)
        {
            return List.Exists(l => l.HasRoleInGroup(roleType, groupId));
        }

        public AgentId GetAgentIdForRoleType(byte roleType, AgentId groupId)
        {
            return List.Find(l => l.HasRoleInGroup(roleType, groupId)).AgentId;
        }

        public IEnumerable<AgentId> GetGroups(AgentId agentId, byte roleType)
        {
            return List.FindAll(r => r.HasRole(agentId, roleType)).Select(x => x.GroupId);
        }

        /// <summary>
        ///     Check if agentId has a role in a team
        /// </summary>
        public bool HasRoles(AgentId agentId)
        {
            return List.Exists(r => r.AgentId.Equals(agentId));
        }

        public bool HasRole(AgentId agentId, byte roleType)
        {
            return List.Exists(l => l.HasRole(agentId, roleType));
        }

        public bool HasARoleIn(AgentId agentId, byte roleType, AgentId groupId)
        {
            return List.Exists(l => l.HasRoleInGroup(agentId, roleType, groupId));
        }

        public bool HasARoleIn(AgentId agentId, AgentId groupId)
        {
            return List.Exists(l => l.HasRoleInGroup(agentId, groupId));
        }

        public void RemoveMember(AgentId agentId, AgentId groupId)
        {
            List.RemoveAll(r => r.HasRoleInGroup(agentId, groupId));
        }

        /// <summary>
        ///     Get the roles of the agentId for the groupId
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public IEnumerable<NetworkRole> GetRoles(AgentId agentId, AgentId groupId)
        {
            return List.Where(r => r.HasRoleInGroup(agentId, groupId));
        }

        /// <summary>
        ///     Get all the roles for the groupId
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public IEnumerable<NetworkRole> GetRoles(AgentId groupId)
        {
            return List.Where(r => r.IsGroup(groupId));
        }

        /// <summary>
        ///     Get the roles of the agentId for the groupId
        /// </summary>
        /// <param name="roleType"></param>
        /// <returns></returns>
        public IEnumerable<AgentId> GetAgents(byte roleType)
        {
            return List.Where(r => r.HasRole(roleType)).Select(x => x.AgentId);
        }

        /// <summary>
        ///     Transfer characteristics of the agentId roles with the groupSourceId to groupTargetId
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupSourceId"></param>
        /// <param name="groupTargetId"></param>
        public void TransferTo(AgentId agentId, AgentId groupSourceId, AgentId groupTargetId)
        {
            var roles = GetRoles(agentId, groupSourceId).ToList();
            foreach (var role in roles)
            {
                List.Add(new NetworkRole(agentId, groupTargetId, role.RoleType));
            }

            RemoveMember(agentId, groupSourceId);
        }

        public void Add(NetworkRole role)
        {
            if (Exists(role))
            {
                return;
            }

            List.Add(role);
        }

        public bool Exists(NetworkRole role)
        {
            return List.Contains(role);
        }

        public void RemoveMembersByRoleTypeFromGroup(byte roleType, AgentId groupId)
        {
            List.RemoveAll(l => l.HasRoleInGroup(roleType, groupId));
        }
    }
}