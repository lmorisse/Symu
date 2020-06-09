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

#endregion

namespace Symu.Repository.Networks.Group
{
    /// <summary>
    ///     Dictionary of all the group of the network
    ///     for every group, the list of all the local AgentIds
    ///     Key => Group Id
    ///     Value => List of AgentIds
    /// </summary>
    /// <example>Groups : team, task force, quality circle, community of practices, committees, ....</example>
    public class NetworkGroups
    {
        /// <summary>
        ///     Key => groupId
        ///     Value => list of group allocation : AgentId, Allocation of the agentId to the groupId
        /// </summary>
        public Dictionary<AgentId, List<GroupAllocation>> List { get; } =
            new Dictionary<AgentId, List<GroupAllocation>>();

        /// <summary>
        ///     Remove agent from network,
        ///     either it is a group or a member of a group
        /// </summary>
        /// <param name="agentId"></param>
        public void RemoveAgent(AgentId agentId)
        {
            if (Exists(agentId))
            {
                RemoveGroup(agentId);
            }
            else
            {
                RemoveMember(agentId);
            }
        }

        public void RemoveMember(AgentId agentId)
        {
            foreach (var groupId in GetGroups().ToList())
            {
                RemoveMember(agentId, groupId);
            }
        }

        public void RemoveMember(AgentId agentId, AgentId groupId)
        {
            if (Exists(groupId))
            {
                List[groupId].RemoveAll(g => g.AgentId.Equals(agentId));
            }
        }

        public IEnumerable<AgentId> GetGroups()
        {
            return List.Any() ? (IEnumerable<AgentId>) List.Keys : new List<AgentId>();
        }

        /// <summary>
        ///     Check that Group exist, either a team, a kanban, ...
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool Exists(AgentId groupId)
        {
            return List.ContainsKey(groupId);
        }

        public void RemoveGroup(AgentId groupId)
        {
            List.Remove(groupId);
        }

        public bool Any()
        {
            return List.Any();
        }

        public void Clear()
        {
            List.Clear();
        }

        public void AddGroup(AgentId groupId)
        {
            if (!Exists(groupId))
            {
                List.Add(groupId, new List<GroupAllocation>());
            }
        }

        /// <summary>
        ///     Add teammate to a group
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="allocation"></param>
        /// <param name="groupId"></param>
        public void AddMember(AgentId agentId, float allocation, AgentId groupId)
        {
            AddGroup(groupId);
            if (!IsMemberOfGroup(agentId, groupId))
            {
                var groupAllocation = new GroupAllocation(agentId, allocation);
                List[groupId].Add(groupAllocation);
            }
            else
            {
                var groupAllocation = GetGroupAllocation(agentId, groupId);
                groupAllocation.Allocation = allocation;
            }

            UpdateGroupAllocations(agentId, groupId.ClassKey, false);
        }

        /// <summary>
        ///     Get members of a group filtered by classKey
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="classKey"></param>
        /// <returns></returns>
        public IEnumerable<AgentId> GetMembers(AgentId groupId, byte classKey)
        {
            return Exists(groupId)
                ? List[groupId].FindAll(x => x.AgentId.ClassKey == classKey).Select(x => x.AgentId)
                : null;
        }

        /// <summary>
        ///     Get members count of a group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public byte GetMembersCount(AgentId groupId)
        {
            return Exists(groupId) ? Convert.ToByte(List[groupId].Count) : (byte) 0;
        }

        /// <summary>
        ///     Get members count of a group
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="classKey"></param>
        /// <returns></returns>
        public byte GetMembersCount(AgentId groupId, byte classKey)
        {
            return Exists(groupId)
                ? Convert.ToByte(List[groupId].Count(x => x.AgentId.ClassKey == classKey))
                : (byte) 0;
        }

        public bool IsMemberOfGroup(AgentId agentId, AgentId groupId)
        {
            return Exists(groupId) && List[groupId].Exists(g => g.AgentId.Equals(agentId));
        }

        public GroupAllocation GetGroupAllocation(AgentId agentId, AgentId groupId)
        {
            return Exists(groupId) ? List[groupId].Find(g => g.AgentId.Equals(agentId)) : null;
        }

        public float GetAllocation(AgentId agentId, AgentId groupId)
        {
            if (IsMemberOfGroup(agentId, groupId))
            {
                return List[groupId].Find(g => g.AgentId.Equals(agentId)).Allocation;
            }

            return 0;
        }

        /// <summary>
        ///     Get the list of the groupIds of a agentId, filtered by group.ClassKey
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupClassKey"></param>
        /// <returns>List of groupIds</returns>
        public IEnumerable<AgentId> GetGroups(AgentId agentId, byte groupClassKey)
        {
            var groupIds = new List<AgentId>();
            if (!List.Any())
            {
                return groupIds;
            }

            groupIds.AddRange(GetGroups().Where(g => g.ClassKey == groupClassKey && IsMemberOfGroup(agentId, g)));

            return groupIds;
        }

        /// <summary>
        ///     Get the list of the members of all the groups of a agentId, filtered by group.ClassKey
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupClassKey"></param>
        /// <returns>List of groupIds</returns>
        public IEnumerable<AgentId> GetCoMemberIds(AgentId agentId, byte groupClassKey)
        {
            var coMemberIds = new List<AgentId>();
            var groupIds = GetGroups(agentId, groupClassKey).ToList();
            if (!groupIds.Any())
            {
                return coMemberIds;
            }

            foreach (var groupId in groupIds)
            {
                coMemberIds.AddRange(List[groupId].FindAll(x => !x.AgentId.Equals(agentId)).Select(x => x.AgentId));
            }

            return coMemberIds.Distinct();
        }

        /// <summary>
        ///     Get the list of the group allocations of a agentId, filtered by group.ClassKey
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="classKey"></param>
        /// <returns>List of groupAllocations (groupId, Allocation)</returns>
        public IEnumerable<GroupAllocation> GetGroupAllocations(AgentId agentId, byte classKey)
        {
            var groupAllocations = new List<GroupAllocation>();
            if (!List.Any())
            {
                return groupAllocations;
            }

            groupAllocations.AddRange(GetGroups().Where(g => g.ClassKey == classKey && IsMemberOfGroup(agentId, g))
                .Select(groupId => new GroupAllocation(groupId, GetAllocation(agentId, groupId))));

            return groupAllocations;
        }

        /// <summary>
        ///     Get the total allocation of a groupId
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns>allocation</returns>
        public float GetMemberAllocations(AgentId groupId)
        {
            return Exists(groupId) ? List[groupId].Sum(a => a.Allocation) : 0;
        }

        /// <summary>
        ///     Update GroupAllocation in a delta mode
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupId"></param>
        /// <param name="allocation"></param>
        /// <param name="capacityThreshold"></param>
        /// <example>allocation = 50 & groupAllocation = 20 => updated groupAllocation =50+20=70</example>
        public void UpdateGroupAllocation(AgentId agentId, AgentId groupId, float allocation, float capacityThreshold)
        {
            var groupAllocation = GetGroupAllocation(agentId, groupId);
            if (groupAllocation is null)
            {
                throw new NullReferenceException(nameof(groupAllocation));
            }

            groupAllocation.Allocation = Math.Max(groupAllocation.Allocation + allocation, capacityThreshold);
        }

        /// <summary>
        ///     Update all groupAllocation of the agentId filtered by the groupId.ClassKey
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="classKey">groupId.ClassKey</param>
        /// <param name="fullAlloc">true if all groupAllocations are added, false if we are in modeling phase</param>
        public void UpdateGroupAllocations(AgentId agentId, byte classKey, bool fullAlloc)
        {
            var groupAllocations = GetGroupAllocations(agentId, classKey).ToList();
            var totalCapacityAllocation = groupAllocations.Sum(ga => ga.Allocation);

            if (!fullAlloc && totalCapacityAllocation <= 100)
            {
                return;
            }

            if (totalCapacityAllocation <= 0)
            {
                throw new ArgumentOutOfRangeException("totalCapacityAllocation should be strictly positif");
            }

            foreach (var groupAllocation in groupAllocations)
            {
                // groupAllocation come from an IEnumerable which is readonly
                var updatedGroupAllocation = GetGroupAllocation(agentId, groupAllocation.AgentId);
                updatedGroupAllocation.Allocation =
                    Math.Min(100F, groupAllocation.Allocation * 100F / totalCapacityAllocation);
            }
        }

        /// <summary>
        ///     Copy all groupAllocations of a groupSourceId into groupTargetId
        /// </summary>
        /// <param name="groupSourceId"></param>
        /// <param name="groupTargetId"></param>
        public void CopyTo(AgentId groupSourceId, AgentId groupTargetId)
        {
            AddGroup(groupTargetId);
            foreach (var groupAllocation in List[groupSourceId])
            {
                List[groupTargetId].Add(groupAllocation);
            }
        }
    }
}