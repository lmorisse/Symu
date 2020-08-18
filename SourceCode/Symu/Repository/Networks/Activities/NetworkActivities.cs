#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Symu.Common.Interfaces;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace Symu.Repository.Networks.Activities
{
    /// <summary>
    ///     Dictionary of all the activities of the network
    ///     for every groupId, the list of all the AgentActivity having activities in a group
    ///     Key => GroupId
    ///     Value => List of AgentActivity : AgentId, activity
    /// </summary>
    public class NetworkActivities
    {
        /// <summary>
        ///     List of all GroupIds and their activities
        /// </summary>
        private readonly ConcurrentDictionary<IAgentId, List<Activity>> _repository =
            new ConcurrentDictionary<IAgentId, List<Activity>>();

        /// <summary>
        ///     Key => GroupId
        ///     Value => list of AgentActivity : AgentId, activity
        /// </summary>
        public ConcurrentDictionary<IAgentId, List<AgentActivity>> AgentActivities { get; } =
            new ConcurrentDictionary<IAgentId, List<AgentActivity>>();

        public bool Any()
        {
            return _repository.Any();
        }

        public void Clear()
        {
            _repository.Clear();
            AgentActivities.Clear();
        }

        /// <summary>
        ///     Remove agent from network,
        ///     either it is a Group or an agent
        /// </summary>
        /// <param name="agentId"></param>
        public void RemoveAgent(IAgentId agentId)
        {
            if (Exists(agentId))
            {
                RemoveGroup(agentId);
            }

            RemoveMember(agentId);
        }

        #region for Group

        public IEnumerable<IAgentId> GetGroupIds()
        {
            return _repository.Any() ? _repository.Keys : null;
        }

        /// <summary>
        ///     Check that group exist
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool Exists(IAgentId groupId)
        {
            return _repository.ContainsKey(groupId);
        }

        public void RemoveGroup(IAgentId groupId)
        {
            _repository.TryRemove(groupId, out _);
            AgentActivities.TryRemove(groupId, out _);
        }

        /// <summary>
        ///     Add Group and initialize the dictionaries
        ///     To add the activities of the group, directly use AddActivities or AddActivity
        ///     which call AddGroup
        /// </summary>
        /// <param name="groupId"></param>
        public void AddGroup(IAgentId groupId)
        {
            if (Exists(groupId))
            {
                return;
            }

            _repository.TryAdd(groupId, new List<Activity>());
            AgentActivities.TryAdd(groupId, new List<AgentActivity>());
        }

        /// <summary>
        ///     Add an activity to a group
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="groupId"></param>
        public void AddActivity(Activity activity, IAgentId groupId)
        {
            AddGroup(groupId);
            if (!_repository[groupId].Contains(activity))
            {
                _repository[groupId].Add(activity);
            }
        }

        /// <summary>
        ///     Remove an activity from a group
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="groupId"></param>
        public void RemoveActivity(IAgentId groupId, Activity activity)
        {
            if (Exists(groupId))
            {
                _repository[groupId].Remove(activity);
            }
        }

        /// <summary>
        ///     Add an activity to a group
        /// </summary>
        /// <param name="activities"></param>
        /// <param name="groupId"></param>
        public void AddActivities(IEnumerable<Activity> activities, IAgentId groupId)
        {
            if (activities is null)
            {
                throw new ArgumentNullException(nameof(activities));
            }

            AddGroup(groupId);
            foreach (var activity in activities)
            {
                if (!_repository[groupId].Contains(activity))
                {
                    _repository[groupId].Add(activity);
                }
            }
        }

        /// <summary>
        ///     check if a group has activities
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool GroupHasActivities(IAgentId groupId)
        {
            return Exists(groupId) && _repository[groupId].Any();
        }

        /// <summary>
        ///     Get all the activities of a Group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public IEnumerable<string> GetGroupActivities(IAgentId groupId)
        {
            return Exists(groupId) ? _repository[groupId].Select(x => x.Name) : new List<string>();
        }

        /// <summary>
        ///     Get the all the knowledges of a group categorize by activity
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns>Dictionary</returns>
        public IDictionary<string, List<Knowledge>> GetActivitiesKnowledgesByActivity(
            IAgentId groupId)
        {
            if (!Exists(groupId))
            {
                return null;
            }

            var activitiesKnowledges = new Dictionary<string, List<Knowledge>>();
            foreach (var activity in _repository[groupId])
            {
                activitiesKnowledges[activity.Name] = activity.Knowledges;
            }

            return activitiesKnowledges;
        }

        /// <summary>
        ///     Get the all the knowledges for all the activities of a group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns>Dictionary</returns>
        public IEnumerable<ushort> GetActivitiesKnowledgeIds(IAgentId groupId)
        {
            if (!Exists(groupId))
            {
                return null;
            }

            var activitiesKnowledges = new List<ushort>();
            foreach (var activity in _repository[groupId])
            {
                activitiesKnowledges.AddRange(activity.Knowledges.Select(x => x.Id));
            }

            return activitiesKnowledges.Distinct();
        }

        #endregion

        #region for agent

        public void RemoveMember(IAgentId agentId)
        {
            var groupIds = GetGroupIds();
            if (groupIds == null)
            {
                return;
            }

            foreach (var groupId in groupIds)
            {
                RemoveMember(agentId, groupId);
            }
        }

        public void RemoveMember(IAgentId agentId, IAgentId groupId)
        {
            if (Exists(groupId))
            {
                AgentActivities[groupId].RemoveAll(g => g.AgentId.Equals(agentId));
            }
        }

        /// <summary>
        ///     Add a worker to an activity of a group
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="activity"></param>
        /// <param name="groupId"></param>
        public void AddActivity(IAgentId agentId, string activity, IAgentId groupId)
        {
            AddGroup(groupId);
            if (AgentHasAnActivityOn(agentId, groupId, activity))
            {
                return;
            }

            var agentActivity = new AgentActivity(agentId, activity);
            AgentActivities[groupId].Add(agentActivity);
        }

        public bool AgentHasAnActivityOn(IAgentId agentId, IAgentId groupId, string activity)
        {
            return Exists(groupId) && AgentActivities[groupId]
                .Exists(g => g.AgentId.Equals(agentId) && g.Activity == activity);
        }

        public bool AgentHasActivitiesOn(IAgentId agentId, IAgentId groupId)
        {
            return Exists(groupId) && AgentActivities[groupId].Exists(g => g.AgentId.Equals(agentId));
        }


        /// <summary>
        ///     Get all the activities of a groupId and an agentId is working on
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public IEnumerable<string> GetAgentActivities(IAgentId agentId, IAgentId groupId)
        {
            return Exists(groupId)
                ? AgentActivities[groupId].FindAll(g => g.AgentId.Equals(agentId)).Select(x => x.Activity)
                : new List<string>();
        }

        /// <summary>
        ///     Get all the activities of an agentId
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public IEnumerable<string> GetAgentActivities(IAgentId agentId)
        {
            var activities = new List<string>();
            foreach (var agentActivities in AgentActivities)
            {
                activities.AddRange(agentActivities.Value.Where(x => x.AgentId.Equals(agentId))
                    .Select(agentActivity => agentActivity.Activity));
            }

            return activities;
        }

        public IEnumerable<IAgentId> FilterAgentIdsWithActivity(IEnumerable<IAgentId> agentIds, IAgentId groupId,
            string activity)
        {
            if (agentIds is null)
            {
                throw new ArgumentNullException(nameof(agentIds));
            }

            return agentIds.Where(agentId => AgentHasAnActivityOn(agentId, groupId, activity)).ToList();
        }

        /// <summary>
        ///     Add a list of activities to AgentId for the groupId
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupId"></param>
        /// <param name="activities"></param>
        public void AddActivities(IAgentId agentId, IAgentId groupId, IEnumerable<string> activities)
        {
            if (activities is null)
            {
                throw new ArgumentNullException(nameof(activities));
            }

            foreach (var activity in activities)
            {
                AddActivity(agentId, activity, groupId);
            }
        }

        /// <summary>
        ///     Add Activities from groupId if the agentId has the knowledge for
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupId"></param>
        /// <param name="agentKnowledgeIds"></param>
        public void AddActivities(IAgentId agentId, IAgentId groupId, List<ushort> agentKnowledgeIds)
        {
            AddActivities(agentId, groupId,
                _repository[groupId].Where(a => a.CheckKnowledgeIds(agentKnowledgeIds)).Select(x => x.Name));
        }

        /// <summary>
        ///     Transfer the agentId activities from groupSourceId to groupTargetId
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupSourceId"></param>
        /// <param name="groupTargetId"></param>
        public void TransferTo(IAgentId agentId, IAgentId groupSourceId, IAgentId groupTargetId)
        {
            AddActivities(agentId, groupTargetId, GetAgentActivities(agentId, groupSourceId));
            RemoveMember(agentId, groupSourceId);
        }

        /// <summary>
        ///     Check if an agent has some activities in any group
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bool HasAgentActivities(IAgentId agentId)
        {
            return AgentActivities.Any(a => a.Value.Exists(v => v != null && v.AgentId.Equals(agentId)));
        }

        #endregion
    }
}