﻿#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using SymuEngine.Classes.Agent;
using SymuEngine.Classes.Agent.Models.Templates.Communication;
using SymuEngine.Repository.Networks.Activities;
using SymuEngine.Repository.Networks.Belief;
using SymuEngine.Repository.Networks.Communication;
using SymuEngine.Repository.Networks.Databases;
using SymuEngine.Repository.Networks.Databases.Repository;
using SymuEngine.Repository.Networks.Enculturation;
using SymuEngine.Repository.Networks.Group;
using SymuEngine.Repository.Networks.Influence;
using SymuEngine.Repository.Networks.Knowledge;
using SymuEngine.Repository.Networks.Link;
using SymuEngine.Repository.Networks.Portfolio;
using SymuEngine.Repository.Networks.Role;

#endregion

namespace SymuEngine.Repository.Networks
{
    /// <summary>
    ///     Network: referential for social, communication, knowledge, authority
    /// </summary>
    public class Network
    {
        /// <summary>
        ///     Directory of social links between AgentIds, with their interaction type
        ///     Who report/communicate to who
        ///     Sphere of interaction of agents
        /// </summary>
        public NetworkLinks NetworkLinks { get; } = new NetworkLinks();

        /// <summary>
        ///     Directory of the groups of the organizationEntity :
        ///     Team, task force, workgroup, circles, community of practices, ...
        /// </summary>
        public NetworkGroups NetworkGroups { get; } = new NetworkGroups();

        /// <summary>
        ///     Directory of the roles the agent are playing in the organizationEntity
        /// </summary>
        public NetworkRoles NetworkRoles { get; } = new NetworkRoles();

        /// <summary>
        ///     Directory of objects used by the agentIds
        ///     using, working, support
        /// </summary>
        public NetworkPortfolios NetworkPortfolios { get; } = new NetworkPortfolios();

        /// <summary>
        ///     Knowledge network
        ///     Who (agentId) knows what (Informations)
        /// </summary>
        public NetworkKnowledges NetworkKnowledges { get; } = new NetworkKnowledges();

        /// <summary>
        ///     Belief network
        ///     Who (agentId) believes what (Informations)
        /// </summary>
        public NetworkBeliefs NetworkBeliefs { get; } = new NetworkBeliefs();

        /// <summary>
        ///     Kanban activities network
        ///     Who (agentId) works on what activities (Kanban)
        /// </summary>
        public NetworkActivities NetworkActivities { get; } = new NetworkActivities();

        /// <summary>
        ///     Agent enculturation level network
        /// </summary>
        public NetworkEnculturation NetworkEnculturation { get; } = new NetworkEnculturation();

        /// <summary>
        ///     Agent enculturation level network
        /// </summary>
        public NetworkInfluences NetworkInfluences { get; } = new NetworkInfluences();

        /// <summary>
        ///     Communication network
        /// </summary>
        public NetworkCommunications NetworkCommunications { get; } = new NetworkCommunications();

        /// <summary>
        ///     Communication network
        /// </summary>
        public NetworkDatabases NetworkDatabases { get; } = new NetworkDatabases();

        #region Clear & remove Agents

        public void Clear()
        {
            NetworkLinks.Clear();
            NetworkGroups.Clear();
            NetworkRoles.Clear();
            NetworkPortfolios.Clear();
            NetworkKnowledges.Clear();
            NetworkBeliefs.Clear();
            NetworkActivities.Clear();
            NetworkEnculturation.Clear();
            NetworkInfluences.Clear();
            NetworkDatabases.Clear();
        }

        public void RemoveAgent(AgentId agentId)
        {
            NetworkLinks.RemoveAgent(agentId);
            NetworkGroups.RemoveAgent(agentId);
            NetworkRoles.RemoveAgent(agentId);
            NetworkPortfolios.RemoveAgent(agentId);
            NetworkKnowledges.RemoveAgent(agentId);
            NetworkActivities.RemoveAgent(agentId);
            NetworkBeliefs.RemoveAgent(agentId);
            NetworkEnculturation.RemoveAgent(agentId);
            NetworkInfluences.RemoveAgent(agentId);
            NetworkDatabases.RemoveAgent(agentId);
        }

        #endregion

        #region Shortcuts to Networks

        /// <summary>
        ///     Add a group to the networkGroup
        /// </summary>
        /// <param name="groupId"></param>
        public void AddGroup(AgentId groupId)
        {
            NetworkGroups.AddGroup(groupId);
        }

        /// <summary>
        ///     Get the list of the group allocations of a agentId
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="classKey"></param>
        /// <returns>List of groupAllocations (groupId, Allocation)</returns>
        public IEnumerable<GroupAllocation> GetGroupAllocations(AgentId agentId, byte classKey)
        {
            return NetworkGroups.GetGroupAllocations(agentId, classKey);
        }

        /// <summary>
        ///     Get the total allocation of a groupId
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public float GetMemberAllocations(AgentId groupId)
        {
            return NetworkGroups.GetMemberAllocations(groupId);
        }

        /// <summary>
        ///     Update the agentId allocation for the groupId
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupId"></param>
        /// <param name="allocation"></param>
        /// <param name="capacityThreshold"></param>
        public void UpdateGroupAllocation(AgentId agentId, AgentId groupId, float allocation, float capacityThreshold)
        {
            NetworkGroups.UpdateGroupAllocation(agentId, groupId, allocation, capacityThreshold);
        }

        /// <summary>
        ///     Update all agentId's groupIds filtered by groupId.ClassKey
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="classKey">groupId.ClassKey</param>
        public void UpdateGroupAllocations(AgentId agentId, byte classKey)
        {
            NetworkGroups.UpdateGroupAllocations(agentId, classKey, true);
        }

        /// <summary>
        ///     Get the main group of the agentId filter by the group.ClassKey
        ///     The main group is defined by the maximum GroupAllocation
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="classKey"></param>
        /// <returns>
        ///     return AgentId of the main group is exists, default Agent if don't exist, so check the result when using this
        ///     method
        /// </returns>
        public AgentId GetMainGroupOrDefault(AgentId agentId, byte classKey)
        {
            var groups = GetGroupAllocations(agentId, classKey);
            return groups.Any()
                ? GetGroupAllocations(agentId, classKey).OrderByDescending(ga => ga.Allocation).First().AgentId
                : new AgentId();
        }

        /// <summary>
        ///     Get the group allocation of the agentIf for the groupId
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public float GetGroupAllocation(AgentId agentId, AgentId groupId)
        {
            return NetworkGroups.GetAllocation(agentId, groupId);
        }

        /// <summary>
        ///     Check if an Agent has roles in any group
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bool HasRoles(AgentId agentId)
        {
            return NetworkRoles.HasRoles(agentId);
        }

        /// <summary>
        ///     Add an agent to a group
        ///     It doesn't handle roles
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="allocation"></param>
        /// <param name="groupId"></param>
        public void AddMemberToGroup(AgentId agentId, float allocation, AgentId groupId)
        {
            lock (NetworkGroups)
            {
                NetworkGroups.AddGroup(groupId);
                foreach (var newTeammateId in NetworkGroups.GetMembers(groupId, agentId.ClassKey))
                {
                    NetworkLinks.AddMembers(agentId, newTeammateId, groupId);
                }

                // intentionally after Links.AddMembers
                NetworkGroups.AddMember(agentId, allocation, groupId);
            }

            NetworkPortfolios.AddMemberToGroup(agentId, groupId);
        }

        /// <summary>
        ///     Remove an agent to a group
        ///     It doesn't handle roles
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupId"></param>
        public void RemoveMemberFromGroup(AgentId agentId, AgentId groupId)
        {
            foreach (var oldTeammateId in NetworkGroups.GetMembers(groupId, agentId.ClassKey))
            {
                NetworkLinks.DeactivateTeammates(agentId, oldTeammateId, groupId);
            }

            NetworkGroups.RemoveMember(agentId, groupId);
            NetworkRoles.RemoveMember(agentId, groupId);
            NetworkPortfolios.RemoveMemberFromGroup(agentId, groupId);

            // Remove all the groupId activities to the AgentId
            NetworkActivities.RemoveMember(agentId, groupId);
        }

        /// <summary>
        ///     Check if an agent is member of a group
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool IsMemberOfGroup(AgentId agentId, AgentId groupId)
        {
            return NetworkGroups.IsMemberOfGroup(agentId, groupId);
        }

        /// <summary>
        ///     Get all the active links of an agent
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public IEnumerable<AgentId> GetActiveLinks(AgentId agentId)
        {
            return NetworkLinks.GetActiveLinks(agentId);
        }

        /// <summary>
        ///     Add a Knowledge to the repository
        /// </summary>
        /// <param name="knowledge"></param>
        public void AddKnowledge(Knowledge.Repository.Knowledge knowledge)
        {
            NetworkKnowledges.AddKnowledge(knowledge);
            NetworkBeliefs.AddBelief(knowledge);
        }

        /// <summary>
        ///     Add a set of Knowledges to the repository
        /// </summary>
        public void AddKnowledges(IEnumerable<Knowledge.Repository.Knowledge> knowledge)
        {
            var knowledges = knowledge.ToList();
            NetworkKnowledges.AddKnowledges(knowledges);
            NetworkBeliefs.AddBeliefs(knowledges);
        }

        #endregion

        #region NetworkPortfolio

        public IEnumerable<AgentId> GetObjectIds(AgentId agentId, byte type)
        {
            return NetworkPortfolios.GetObjectIds(agentId, type);
        }

        /// <summary>
        ///     Copy characteristics of a group to another
        /// </summary>
        /// <param name="groupSourceId"></param>
        /// <param name="groupTargetId"></param>
        public void GroupCopyTo(AgentId groupSourceId, AgentId groupTargetId)
        {
            NetworkPortfolios.CopyTo(groupSourceId, groupTargetId);
        }

        public bool HasObject(AgentId agentId, byte typeOfUse)
        {
            return NetworkPortfolios.HasObject(agentId, typeOfUse);
        }

        #endregion

        #region Database

        public void AddDatabase(AgentId agentId, ushort databaseId)
        {
            NetworkDatabases.Add(agentId, databaseId);
        }

        public void AddEmail(AgentId agentId, CommunicationTemplate communication)
        {
            if (communication is null)
            {
                throw new ArgumentNullException(nameof(communication));
            }

            var email = new Database(agentId.Key, communication.Cognitive.TasksAndPerformance,
                communication.TimeToLive);
            NetworkDatabases.Add(agentId, email);
        }

        #endregion
    }
}