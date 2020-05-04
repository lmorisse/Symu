#region Licence

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
using SymuEngine.Repository.Networks.Knowledges;

#endregion

namespace SymuEngine.Repository.Networks.Activities
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
        public Dictionary<AgentId, List<Activity>> Repository { get; } = new Dictionary<AgentId, List<Activity>>();

        /// <summary>
        ///     Key => GroupId
        ///     Value => list of AgentActivity : AgentId, activity
        /// </summary>
        public Dictionary<AgentId, List<AgentActivity>> AgentActivities { get; } =
            new Dictionary<AgentId, List<AgentActivity>>();

        public bool Any()
        {
            return Repository.Any();
        }

        public void Clear()
        {
            Repository.Clear();
            AgentActivities.Clear();
        }

        /// <summary>
        ///     Remove agent from network,
        ///     either it is a Group or an agent
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

        #region for Group

        public IEnumerable<AgentId> GetGroupIds()
        {
            return Repository.Any() ? Repository.Keys : null;
        }

        /// <summary>
        ///     Check that group exist
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool Exists(AgentId groupId)
        {
            return Repository.ContainsKey(groupId);
        }

        public void RemoveGroup(AgentId groupId)
        {
            Repository.Remove(groupId);
            AgentActivities.Remove(groupId);
        }

        /// <summary>
        ///     Add Group and initialize the dictionaries
        ///     To add the activities of the group, directly use AddActivities or AddActivity
        ///     which call AddGroup
        /// </summary>
        /// <param name="groupId"></param>
        public void AddGroup(AgentId groupId)
        {
            if (Exists(groupId))
            {
                return;
            }

            Repository.Add(groupId, new List<Activity>());
            AgentActivities.Add(groupId, new List<AgentActivity>());
        }

        /// <summary>
        ///     Add an activity to a group
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="groupId"></param>
        public void AddActivity(Activity activity, AgentId groupId)
        {
            AddGroup(groupId);
            if (!Repository[groupId].Contains(activity))
            {
                Repository[groupId].Add(activity);
            }
        }

        /// <summary>
        ///     Add an activity to a group
        /// </summary>
        /// <param name="activities"></param>
        /// <param name="groupId"></param>
        public void AddActivities(IEnumerable<Activity> activities, AgentId groupId)
        {
            if (activities is null)
            {
                throw new ArgumentNullException(nameof(activities));
            }

            AddGroup(groupId);
            foreach (var activity in activities)
            {
                if (!Repository[groupId].Contains(activity))
                {
                    Repository[groupId].Add(activity);
                }
            }
        }

        /// <summary>
        ///     check if a group has activities
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool HasActivities(AgentId groupId)
        {
            return Exists(groupId) && Repository[groupId].Any();
        }

        /// <summary>
        ///     Get all the activities of a Group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public IEnumerable<string> GetActivities(AgentId groupId)
        {
            return Exists(groupId) ? Repository[groupId].Select(x => x.Name) : new List<string>();
        }

        /// <summary>
        ///     Get the all the knowledges of a group categorize by activity
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns>Dictionary</returns>
        public IDictionary<string, List<Knowledge>> GetActivitiesKnowledgesByActivity(
            AgentId groupId)
        {
            if (!Exists(groupId))
            {
                return null;
            }

            var activitiesKnowledges = new Dictionary<string, List<Knowledge>>();
            foreach (var activity in Repository[groupId])
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
        public IEnumerable<ushort> GetActivitiesKnowledgeIds(AgentId groupId)
        {
            if (!Exists(groupId))
            {
                return null;
            }

            var activitiesKnowledges = new List<ushort>();
            foreach (var activity in Repository[groupId])
            {
                activitiesKnowledges.AddRange(activity.Knowledges.Select(x => x.Id));
            }

            return activitiesKnowledges.Distinct();
        }

        #endregion

        #region for agent

        public void RemoveMember(AgentId agentId)
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

        public void RemoveMember(AgentId agentId, AgentId groupId)
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
        public void AddActivity(AgentId agentId, string activity, AgentId groupId)
        {
            AddGroup(groupId);
            if (HasAnActivityOn(agentId, groupId, activity))
            {
                return;
            }

            var agentActivity = new AgentActivity(agentId, activity);
            AgentActivities[groupId].Add(agentActivity);
        }

        public bool HasAnActivityOn(AgentId agentId, AgentId groupId, string activity)
        {
            return Exists(groupId) && AgentActivities[groupId]
                .Exists(g => g.AgentId.Equals(agentId) && g.Activity == activity);
        }

        public bool HasActivitiesOn(AgentId agentId, AgentId groupId)
        {
            return Exists(groupId) && AgentActivities[groupId].Exists(g => g.AgentId.Equals(agentId));
        }


        /// <summary>
        ///     Get all the activities of a groupId and an agentId is working on
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public IEnumerable<string> GetActivities(AgentId agentId, AgentId groupId)
        {
            return Exists(groupId)
                ? AgentActivities[groupId].FindAll(g => g.AgentId.Equals(agentId)).Select(x => x.Activity)
                : new List<string>();
        }

        public IEnumerable<AgentId> FilterAgentIdsWithActivity(IEnumerable<AgentId> agentIds, AgentId groupId,
            string activity)
        {
            if (agentIds is null)
            {
                throw new ArgumentNullException(nameof(agentIds));
            }

            return agentIds.Where(agentId => HasAnActivityOn(agentId, groupId, activity)).ToList();
        }

        /// <summary>
        ///     Add a list of activities to AgentId for the groupId
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupId"></param>
        /// <param name="activities"></param>
        public void AddActivities(AgentId agentId, AgentId groupId, IEnumerable<string> activities)
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
        public void AddActivities(AgentId agentId, AgentId groupId, List<ushort> agentKnowledgeIds)
        {
            AddActivities(agentId, groupId,
                Repository[groupId].Where(a => a.CheckKnowledgeIds(agentKnowledgeIds)).Select(x => x.Name));
        }

        /// <summary>
        ///     Transfer the agentId activities from groupSourceId to groupTargetId
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupSourceId"></param>
        /// <param name="groupTargetId"></param>
        public void TransferTo(AgentId agentId, AgentId groupSourceId, AgentId groupTargetId)
        {
            AddActivities(agentId, groupTargetId, GetActivities(agentId, groupSourceId));
            RemoveMember(agentId, groupSourceId);
        }

        /// <summary>
        ///     Check if an agent has some activities in any group
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bool HasAgentActivities(AgentId agentId)
        {
            return AgentActivities.Any(a => a.Value.Exists(v => v.AgentId.Equals(agentId)));
        }

        #endregion
    }
}