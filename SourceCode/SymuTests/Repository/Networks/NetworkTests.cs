#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Classes.Organization;
using Symu.Common;
using Symu.Repository;
using Symu.Repository.Networks;
using Symu.Repository.Networks.Activities;
using Symu.Repository.Networks.Beliefs;
using Symu.Repository.Networks.Knowledges;
using Symu.Repository.Networks.Role;
using SymuTests.Helpers;

#endregion

namespace SymuTests.Repository.Networks
{
    [TestClass]
    public class NetworkTests
    {
        private const byte IsWorkingOn = 1;
        private const byte IsSupportOn = 2;
        private readonly Activity _activity = new Activity("a");
        private readonly TestResource _component = new TestResource(6, 4);

        private readonly Knowledge _knowledge =
            new Knowledge(1, "1", 1);

        private MetaNetwork _network ;
        private readonly TestAgentId _teamId = new TestAgentId(1, 1);
        private readonly TestAgentId _teamId2 = new TestAgentId(2, 1);
        private readonly TestAgentId _teammateId = new TestAgentId(4, SymuYellowPages.Actor);
        private readonly TestAgentId _teammateId2 = new TestAgentId(5, SymuYellowPages.Actor);
        private readonly TestAgentId _managerId = new TestAgentId(3, 2);
        private Belief _belief;
        private NetworkRole _networkRole;

        [TestInitialize]
        public void Initialize()
        {
            var models = new OrganizationModels();
            _network = new MetaNetwork(models.InteractionSphere, models.ImpactOfBeliefOnTask);
            _belief = new Belief(1, "1", 1,
                _network.Knowledge.Model, BeliefWeightLevel.RandomWeight);
            _network.Activities.AddActivities(new List<Activity> {_activity}, _teamId);
            _networkRole = new NetworkRole(_managerId, _teamId, 1);
        }

        [TestMethod]
        public void ClearTest()
        {
            _network.Links.AddLink(_teammateId, _managerId);
            _network.Groups.AddGroup(_teamId);
            _network.Roles.Add(_networkRole);
            _network.Resources.Add(_component);
            _network.AddKnowledge(_knowledge);
            _network.Knowledge.Add(_teammateId, _knowledge.Id, KnowledgeLevel.Expert, 0, -1);
            _network.Beliefs.Add(_teammateId, _belief, BeliefLevel.NeitherAgreeNorDisagree);
            _network.Activities.AddActivities(_teammateId, _teamId, new List<string> {"a1"});
            _network.Clear();
            Assert.IsFalse(_network.Links.Any());
            Assert.IsFalse(_network.Groups.Any());
            Assert.IsFalse(_network.Roles.Any());
            Assert.IsFalse(_network.Resources.Any());
            Assert.IsFalse(_network.Knowledge.Any());
            Assert.IsFalse(_network.Knowledge.Any());
            Assert.IsFalse(_network.Activities.Any());
            Assert.IsFalse(_network.Beliefs.Any());
        }

        [TestMethod]
        public void AddTeamTest()
        {
            _network.Groups.AddGroup(_teamId);
            Assert.IsTrue(_network.Groups.Any());
        }

        /// <summary>
        ///     With network started
        /// </summary>
        [TestMethod]
        public void AddMemberToGroupTest()
        {
            //_network.State = AgentState.Started;
            _network.Groups.AddGroup(_teamId);
            _network.Resources.Add(_teamId, _component, IsSupportOn, 100);
            // Method to test
            _network.AddAgentToGroup(_teammateId, 100, _teamId, true);
            _network.AddAgentToGroup(_teammateId2, 100, _teamId, true);
            // Test link teammates
            Assert.IsTrue(_network.Links.HasActiveLink(_teammateId, _teammateId2));
            // Test group
            Assert.IsTrue(_network.Groups.IsMemberOfGroup(_teammateId, _teamId));
            // Resource
            Assert.IsTrue(_network.Resources.HasResource(_teammateId, IsSupportOn));
        }

        /// <summary>
        ///     With network starting
        /// </summary>
        [TestMethod]
        public void AddMemberToGroupTest2()
        {
            //_network.State = AgentState.Starting;
            _network.Groups.AddGroup(_teamId);
            _network.Resources.Add(_teamId, _component, IsSupportOn, 100);
            // Method to test
            _network.AddAgentToGroup(_teammateId, 100, _teamId, false);
            _network.AddAgentToGroup(_teammateId2, 100, _teamId, false);
            // Test link teammates
            Assert.IsFalse(_network.Links.HasActiveLink(_teammateId, _teammateId2));
        }

        [TestMethod]
        public void RemoveMemberFromGroupTest()
        {
            _network.Groups.AddGroup(_teamId);
            _network.AddAgentToGroup(_teammateId, 100, _teamId, false);
            _network.AddAgentToGroup(_teammateId2, 100, _teamId, false);
            _network.Resources.Add(_teamId, _component, IsSupportOn, 100);
            // Method to test
            _network.RemoveAgentFromGroup(_teammateId, _teamId);
            Assert.IsFalse(_network.Groups.IsMemberOfGroup(_teammateId, _teamId));
            // Test link teammates
            Assert.IsFalse(_network.Links.HasActiveLink(_teammateId, _teammateId2));
            // Test group
            Assert.IsFalse(_network.Groups.IsMemberOfGroup(_teammateId, _teamId));
            // Test link subordinates
            Assert.IsFalse(_network.Links.HasActiveLink(_teammateId, _managerId));
            // Portfolio
            Assert.IsFalse(_network.Resources.HasResource(_teammateId, IsSupportOn));
        }

        [TestMethod]
        public void RemoveAgentTest()
        {
            _network.Links.AddLink(_teammateId, _managerId);
            _network.Groups.AddAgent(_teammateId, 100, _teamId);
            _network.Roles.Add(_networkRole);
            _network.Resources.Add(_teammateId, _component, IsWorkingOn, 100);
            _network.AddKnowledge(_knowledge);
            _network.Knowledge.Add(_teammateId, _knowledge.Id, KnowledgeLevel.Expert, 0, -1);
            _network.Activities.AddActivities(_teammateId, _teamId, new List<string> {_activity.Name});
            _network.RemoveAgent(_teammateId);
            //Assert.IsFalse(network.AgentIdExists(teammateId));
            Assert.IsFalse(_network.Links.Any());
            Assert.AreEqual(0, _network.Groups.GetAgentsCount(_teamId, _teammateId.ClassId));
            Assert.IsFalse(_network.Roles.IsMember(_teammateId, _teamId.ClassId));
            Assert.IsFalse(_network.Resources.HasResource(_teammateId, _component.Id, IsWorkingOn));
            Assert.IsFalse(_network.Knowledge.Any());
            Assert.IsFalse(_network.Activities.AgentHasActivitiesOn(_teammateId, _teamId));
        }

        [TestMethod]
        public void GetMainGroupTest()
        {
            // Default test
            var agentId = _network.Groups.GetMainGroupOrDefault(_teammateId, _teamId.ClassId);
            Assert.IsNull(agentId);
            // Normal test
            _network.AddAgentToGroup(_teammateId, 100, _teamId, false);
            Assert.AreEqual(_teamId, _network.Groups.GetMainGroupOrDefault(_teammateId, _teamId.ClassId));
            _network.AddAgentToGroup(_teammateId, 10, _teamId2, false);
            Assert.AreEqual(_teamId, _network.Groups.GetMainGroupOrDefault(_teammateId, _teamId.ClassId));
        }

        #region Knowledge

        [TestMethod]
        public void AddKnowledgeTest()
        {
            var knowledge = new Knowledge(1, "1", 1);
            _network.AddKnowledge(knowledge);
            Assert.IsNotNull(_network.Knowledge.GetKnowledge(knowledge.Id));
            Assert.IsNotNull(_network.Beliefs.GetBelief(knowledge.Id));
        }

        [TestMethod]
        public void AddKnowledgesTest()
        {
            var knowledge = new Knowledge(1, "1", 1);
            var knowledges = new List<Knowledge> {knowledge};
            _network.AddKnowledges(knowledges);
            Assert.IsNotNull(_network.Knowledge.GetKnowledge(knowledge.Id));
            Assert.IsNotNull(_network.Beliefs.GetBelief(knowledge.Id));
        }

        #endregion
    }
}