#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Common.Interfaces;
using Symu.Common.Interfaces.Agent;
using Symu.Repository.Networks.Activities;
using Symu.Repository.Networks.Knowledges;

#endregion


namespace SymuTests.Repository.Networks.Activities
{
    [TestClass]
    public class NetworkActivitiesTests
    {
        private const string StrActivity1 = "a1";
        private const string StrActivity2 = "a2";
        private readonly List<string> _activities = new List<string> {StrActivity1, StrActivity2};
        private readonly Activity _activity1 = new Activity(StrActivity1);
        private readonly Activity _activity2 = new Activity(StrActivity2);
        private readonly AgentId _agentId = new AgentId(2, 2);
        private readonly AgentId _kanbanId = new AgentId(1, 1);
        private readonly AgentId _kanbanId2 = new AgentId(2, 1);

        private readonly Knowledge _knowledge1 =
            new Knowledge(1, "1", 1);

        private readonly Knowledge _knowledge2 =
            new Knowledge(2, "2", 1);

        private readonly ActivityNetwork _network = new ActivityNetwork();

        [TestInitialize]
        public void Initialize()
        {
            _activity1.AddKnowledge(_knowledge1);
            _activity2.AddKnowledge(_knowledge2);
        }

        /// <summary>
        ///     With a kanban
        /// </summary>
        [TestMethod]
        public void RemoveAgentTest()
        {
            _network.AddGroup(_kanbanId);
            _network.AddActivities(_agentId, _kanbanId, _activities);
            _network.RemoveAgent(_kanbanId);
            Assert.IsFalse(_network.Exists(_kanbanId));
        }

        /// <summary>
        ///     With an agent
        /// </summary>
        [TestMethod]
        public void RemoveAgentTest1()
        {
            _network.AddGroup(_kanbanId);
            _network.AddActivities(_agentId, _kanbanId, _activities);
            _network.RemoveAgent(_agentId);
            Assert.IsFalse(_network.AgentHasActivitiesOn(_agentId, _kanbanId));
        }

        [TestMethod]
        public void RemoveMemberTest()
        {
            _network.AddGroup(_kanbanId);
            _network.AddGroup(_kanbanId2);
            _network.AddActivities(_agentId, _kanbanId, _activities);
            _network.AddActivities(_agentId, _kanbanId2, _activities);
            _network.RemoveMember(_agentId, _kanbanId);
            Assert.IsFalse(_network.AgentHasActivitiesOn(_agentId, _kanbanId));
            Assert.IsTrue(_network.AgentHasActivitiesOn(_agentId, _kanbanId2));
        }

        [TestMethod]
        public void RemoveMemberTest1()
        {
            _network.AddGroup(_kanbanId);
            _network.AddGroup(_kanbanId2);
            _network.AddActivities(_agentId, _kanbanId, _activities);
            _network.AddActivities(_agentId, _kanbanId2, _activities);
            _network.RemoveMember(_agentId);
            Assert.IsFalse(_network.AgentHasActivitiesOn(_agentId, _kanbanId));
            Assert.IsFalse(_network.AgentHasActivitiesOn(_agentId, _kanbanId2));
        }

        [TestMethod]
        public void GetKanbansTest()
        {
            Assert.IsNull(_network.GetGroupIds());
            _network.AddGroup(_kanbanId);
            Assert.IsNotNull(_network.GetGroupIds());
        }

        [TestMethod]
        public void ExistsTest()
        {
            Assert.IsFalse(_network.Exists(_kanbanId));
            _network.AddGroup(_kanbanId);
            Assert.IsTrue(_network.Exists(_kanbanId));
        }

        [TestMethod]
        public void RemoveKanbanTest()
        {
            _network.AddGroup(_kanbanId);
            _network.RemoveGroup(_kanbanId);
            Assert.IsFalse(_network.Exists(_kanbanId));
        }

        [TestMethod]
        public void AnyTest()
        {
            Assert.IsFalse(_network.Any());
            _network.AddGroup(_kanbanId);
            Assert.IsTrue(_network.Any());
        }

        [TestMethod]
        public void ClearTest()
        {
            _network.AddGroup(_kanbanId);
            _network.AddGroup(_kanbanId2);
            _network.Clear();
            Assert.IsFalse(_network.Any());
        }

        [TestMethod]
        public void AddKanbanTest()
        {
            Assert.IsFalse(_network.Exists(_kanbanId));
            _network.AddGroup(_kanbanId);
            Assert.IsTrue(_network.Exists(_kanbanId));
        }

        [TestMethod]
        public void AddAndGetActivitiesTest()
        {
            _network.AddGroup(_kanbanId);
            Assert.AreEqual(0, _network.GetAgentActivities(_agentId, _kanbanId).Count());
            _network.AddActivities(_agentId, _kanbanId, _activities);
            Assert.AreEqual(_activities.Count, _network.GetAgentActivities(_agentId, _kanbanId).Count());
        }

        [TestMethod]
        public void IsWorkingOnActivityTest()
        {
            _network.AddGroup(_kanbanId);
            Assert.IsFalse(_network.AgentHasAnActivityOn(_agentId, _kanbanId, StrActivity1));
            _network.AddActivities(_agentId, _kanbanId, _activities);
            Assert.IsTrue(_network.AgentHasAnActivityOn(_agentId, _kanbanId, StrActivity1));
            Assert.IsTrue(_network.AgentHasAnActivityOn(_agentId, _kanbanId, StrActivity2));
        }

        [TestMethod]
        public void GetActivitiesTest()
        {
            Assert.AreEqual(0, _network.GetGroupActivities(_kanbanId).Count());
            _network.AddActivity(_activity1, _kanbanId);
            Assert.AreEqual(1, _network.GetGroupActivities(_kanbanId).Count());
        }

        [TestMethod]
        public void GetActivitiesKnowledgesTest()
        {
            Assert.IsNull(_network.GetActivitiesKnowledgesByActivity(_kanbanId));
            _network.AddActivity(_activity1, _kanbanId);
            Assert.AreEqual(1, _network.GetActivitiesKnowledgesByActivity(_kanbanId).Count);
            _network.AddActivity(_activity2, _kanbanId);
            Assert.AreEqual(2, _network.GetActivitiesKnowledgesByActivity(_kanbanId).Count);
        }

        [TestMethod]
        public void GetActivitiesKnowledgeIdsTest()
        {
            Assert.IsNull(_network.GetActivitiesKnowledgeIds(_kanbanId));
            _network.AddActivity(_activity1, _kanbanId);
            Assert.AreEqual(1, _network.GetActivitiesKnowledgeIds(_kanbanId).Count());
            _network.AddActivity(_activity2, _kanbanId);
            Assert.AreEqual(2, _network.GetActivitiesKnowledgeIds(_kanbanId).Count());
        }

        /// <summary>
        ///     Add activity to a kanban
        /// </summary>
        [TestMethod]
        public void AddActivityTest()
        {
            Assert.IsFalse(_network.GroupHasActivities(_kanbanId));
            _network.AddActivity(_activity1, _kanbanId);
            Assert.IsTrue(_network.GroupHasActivities(_kanbanId));
            _network.AddActivity(_activity1, _kanbanId); //Duplicate
            Assert.AreEqual(1, _network.GetGroupActivities(_kanbanId).Count());
        }

        /// <summary>
        ///     Add activity to an agentId
        /// </summary>
        [TestMethod]
        public void AddActivityTest1()
        {
            _network.AddGroup(_kanbanId);
            Assert.IsFalse(_network.AgentHasAnActivityOn(_agentId, _kanbanId, StrActivity1));
            _network.AddActivity(_agentId, StrActivity1, _kanbanId);
            Assert.IsTrue(_network.AgentHasAnActivityOn(_agentId, _kanbanId, StrActivity1));
        }

        /// <summary>
        ///     Add a list of activities to AgentId for the KanbanId
        /// </summary>
        [TestMethod]
        public void AddActivitiesTest()
        {
            _network.AddGroup(_kanbanId);
            Assert.IsFalse(_network.AgentHasActivitiesOn(_agentId, _kanbanId));
            _network.AddActivities(_agentId, _kanbanId, _activities);
            Assert.IsTrue(_network.AgentHasActivitiesOn(_agentId, _kanbanId));
        }

        /// <summary>
        ///     Add Activities from KanbanId if the agentId has the knowledge for
        /// </summary>
        [TestMethod]
        public void AddActivitiesTest1()
        {
            _network.AddGroup(_kanbanId);
            _network.AddActivities(_agentId, _kanbanId, new List<ushort> {_knowledge1.Id});
            Assert.AreEqual(0, _network.GetAgentActivities(_agentId, _kanbanId).Count());
            // Has the knowledge
            _network.AddActivity(_activity1, _kanbanId);
            _network.AddActivities(_agentId, _kanbanId, new List<ushort> {_knowledge1.Id});
            Assert.AreEqual(1, _network.GetAgentActivities(_agentId, _kanbanId).Count());
            // Hasn't the knowledge
            _network.AddActivity(_activity2, _kanbanId);
            _network.AddActivities(_agentId, _kanbanId, new List<ushort> {_knowledge1.Id});
            Assert.AreEqual(1, _network.GetAgentActivities(_agentId, _kanbanId).Count());
        }

        [TestMethod]
        public void TransferToTest()
        {
            _network.AddActivities(_agentId, _kanbanId, _activities);
            _network.TransferTo(_agentId, _kanbanId, _kanbanId2);
            Assert.IsTrue(_network.AgentHasActivitiesOn(_agentId, _kanbanId2));
            Assert.IsFalse(_network.AgentHasActivitiesOn(_agentId, _kanbanId));
        }

        [TestMethod]
        public void AddActivitiesTest2()
        {
            Assert.IsFalse(_network.GroupHasActivities(_kanbanId));
            _network.AddActivities(new List<Activity> {_activity1}, _kanbanId);
            Assert.IsTrue(_network.GroupHasActivities(_kanbanId));
        }

        [TestMethod]
        public void HasAgentActivitiesTest()
        {
            Assert.IsFalse(_network.HasAgentActivities(_agentId));
            _network.AddActivities(_agentId, _kanbanId, _activities);
            Assert.IsTrue(_network.HasAgentActivities(_agentId));
        }

        [TestMethod]
        public void FilterAgentIdsWithActivityTest()
        {
            // Empty AgentIds
            var agentIds = new List<IAgentId>();
            Assert.AreEqual(0, _network.FilterAgentIdsWithActivity(agentIds, _kanbanId, _activity1.Name).Count());
            // One agent not working on activity
            agentIds.Add(_agentId);
            Assert.AreEqual(0, _network.FilterAgentIdsWithActivity(agentIds, _kanbanId, _activity1.Name).Count());
            // One agent working on activity
            _network.AddActivity(_agentId, _activity1.Name, _kanbanId);
            Assert.AreEqual(1, _network.FilterAgentIdsWithActivity(agentIds, _kanbanId, _activity1.Name).Count());
        }

        /// <summary>
        ///     With no activities
        /// </summary>
        [TestMethod]
        public void GetAgentActivitiesTest()
        {
            Assert.AreEqual(0, _network.GetAgentActivities(_agentId).Count());
        }

        /// <summary>
        ///     With activities
        /// </summary>
        [TestMethod]
        public void GetAgentActivitiesTest1()
        {
            _network.AddActivity(_agentId, _activity1.Name, _kanbanId);
            Assert.AreEqual(1, _network.GetAgentActivities(_agentId).Count());
            _network.AddActivity(_agentId, _activity2.Name, _kanbanId);
            Assert.AreEqual(2, _network.GetAgentActivities(_agentId).Count());
        }
    }
}