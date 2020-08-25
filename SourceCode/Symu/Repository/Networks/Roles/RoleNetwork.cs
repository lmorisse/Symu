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
using Symu.Repository.Entity;

#endregion

namespace Symu.Repository.Networks.Roles
{
    public class RoleNetwork
    {
        /// <summary>
        ///     Repository of all the resources used during the simulation
        /// </summary>
        public RoleCollection Repository { get; } = new RoleCollection();
        public List<IAgentRole> AgentRoles { get; } = new List<IAgentRole>();

        /// <summary>
        ///     Gets or sets the element at the specified index
        /// </summary>
        /// <param name="index">0 based</param>
        /// <returns></returns>
        public IAgentRole this[int index]
        {
            get => AgentRoles[index];
            set => AgentRoles[index] = value;
        }

        public void Clear()
        {
            AgentRoles.Clear();
        }

        public void RemoveAgent(IAgentId agentId)
        {
            // Remove agentId as an Agent
            AgentRoles.RemoveAll(l => l.AgentId.Equals(agentId));
            // Remove agentId as a Group
            AgentRoles.RemoveAll(l => l.GroupId.Equals(agentId));
        }

        public bool Any()
        {
            return AgentRoles.Any();
        }

        /// <summary>
        ///     List of groupIds teammate is member of
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupClassId"></param>
        /// <returns></returns>
        public IEnumerable<IAgentId> IsMemberOfGroups(IAgentId agentId, IClassId groupClassId)
        {
            return AgentRoles.FindAll(l => l != null && l.IsMemberOfGroups(agentId, groupClassId)).Select(x => x.GroupId);
        }

        /// <summary>
        ///     List of groupIds teammate is member of
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupClassId"></param>
        /// <returns></returns>
        public bool IsMember(IAgentId agentId, IClassId groupClassId)
        {
            return AgentRoles.Exists(l => l != null && l.IsMemberOfGroups(agentId, groupClassId));
        }

        public bool ExistAgentForRoleType(IRole role, IAgentId groupId)
        {
            return AgentRoles.Exists(l => l != null && l.HasRoleInGroup(role, groupId));
        }

        public IAgentId GetAgentIdForRoleType(IRole role, IAgentId groupId)
        {
            var group = AgentRoles.Find(l => l != null && l.HasRoleInGroup(role, groupId));
            return group?.AgentId;
        }

        public IEnumerable<IAgentId> GetGroups(IAgentId agentId, IRole role)
        {
            return AgentRoles.FindAll(l => l != null && l.HasRole(agentId, role)).Select(x => x.GroupId);
        }

        /// <summary>
        ///     Check if agentId has a role in a team
        /// </summary>
        public bool HasRoles(IAgentId agentId)
        {
            return AgentRoles.Exists(l => l != null && l.AgentId.Equals(agentId));
        }

        public bool HasRole(IAgentId agentId, IRole role)
        {
            return AgentRoles.Exists(l => l != null && l.HasRole(agentId, role));
        }

        public bool HasARoleIn(IAgentId agentId, IRole role, IAgentId groupId)
        {
            return AgentRoles.Exists(l => l != null && l.HasRoleInGroup(agentId, role, groupId));
        }

        public bool HasARoleIn(IAgentId agentId, IAgentId groupId)
        {
            return AgentRoles.Exists(l => l != null && l.HasRoleInGroup(agentId, groupId));
        }

        public void RemoveMember(IAgentId agentId, IAgentId groupId)
        {
            AgentRoles.RemoveAll(l => l == null || l.HasRoleInGroup(agentId, groupId));
        }

        /// <summary>
        ///     Get the roles of the agentId for the groupId
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public IEnumerable<IAgentRole> GetRoles(IAgentId agentId, IAgentId groupId)
        {
            lock (AgentRoles)
            {
                return AgentRoles.Where(r => r.HasRoleInGroup(agentId, groupId));
            }
        }

        /// <summary>
        ///     Get all the roles for the groupId
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public IEnumerable<IAgentRole> GetRoles(IAgentId groupId)
        {
            return AgentRoles.Where(r => r.IsGroup(groupId));
        }

        /// <summary>
        ///     Get the roles of the agentId for the groupId
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public IEnumerable<IAgentId> GetAgents(IRole role)
        {
            return AgentRoles.Where(r => r.HasRole(role)).Select(x => x.AgentId);
        }

        /// <summary>
        ///     Transfer characteristics of the agentId roles with the groupSourceId to groupTargetId
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupSourceId"></param>
        /// <param name="groupTargetId"></param>
        public void TransferTo(IAgentId agentId, IAgentId groupSourceId, IAgentId groupTargetId)
        {
            if (groupSourceId == null)
            {
                throw new ArgumentNullException(nameof(groupSourceId));
            }

            if (groupSourceId.Equals(groupTargetId))
            {
                return;
            }

            lock (AgentRoles)
            {
                var roles = GetRoles(agentId, groupSourceId).ToList();
                foreach (var role in roles)
                {
                    var agentRole = role.Clone();
                    agentRole.GroupId = groupTargetId;
                    AgentRoles.Add(agentRole);
                }

                RemoveMember(agentId, groupSourceId);
            }
        }

        public void Add(IAgentRole agentRole)
        {
            if (Exists(agentRole))
            {
                return;
            }

            AgentRoles.Add(agentRole);
        }

        public bool Exists(IAgentRole agentRole)
        {
            return AgentRoles.Contains(agentRole);
        }

        public void RemoveMembersByRoleTypeFromGroup(IRole role, IAgentId groupId)
        {
            AgentRoles.RemoveAll(l => l.HasRoleInGroup(role, groupId));
        }
    }
}