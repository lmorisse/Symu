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

#endregion

namespace SymuTests.Repository.Networks
{
    [TestClass]
    public class NetworkTests
    {
        private const byte IsWorkingOn = 1;
        private const byte IsSupportOn = 2;
        private readonly Activity _activity = new Activity("a");
        private readonly AgentId _componentId = new AgentId(6, 4);

        private readonly Knowledge _knowledge =
            new Knowledge(1, "1", 1);

        private readonly AgentId _managerId = new AgentId(3, 2);
        private readonly MetaNetwork _network = new MetaNetwork(new OrganizationModels());
        private readonly AgentId _teamId = new AgentId(1, 1);
        private readonly AgentId _teamId2 = new AgentId(2, 1);
        private readonly AgentId _teammateId = new AgentId(4, SymuYellowPages.Actor);
        private readonly AgentId _teammateId2 = new AgentId(5, SymuYellowPages.Actor);
        private Belief _belief;
        private NetworkRole _networkRole;

        [TestInitialize]
        public void Initialize()
        {
            _belief = new Belief(1, "1", 1,
                _network.NetworkKnowledges.Model, BeliefWeightLevel.RandomWeight);
            _network.NetworkActivities.AddActivities(new List<Activity> {_activity}, _teamId);
            _networkRole = new NetworkRole(_managerId, _teamId, 1);
        }

        [TestMethod]
        public void ClearTest()
        {
            _network.NetworkLinks.AddLink(_teammateId, _managerId);
            _network.NetworkGroups.AddGroup(_teamId);
            _network.NetworkRoles.Add(_networkRole);
            _network.NetworkPortfolios.AddPortfolio(_teammateId, _componentId, IsWorkingOn, 100);
            _network.AddKnowledge(_knowledge);
            _network.NetworkKnowledges.Add(_teammateId, _knowledge.Id, KnowledgeLevel.Expert, 0, -1);
            _network.NetworkBeliefs.Add(_teammateId, _belief, BeliefLevel.NeitherAgreeNorDisagree);
            _network.NetworkActivities.AddActivities(_teammateId, _teamId, new List<string> {"a1"});
            _network.Clear();
            Assert.IsFalse(_network.NetworkLinks.Any());
            Assert.IsFalse(_network.NetworkGroups.Any());
            Assert.IsFalse(_network.NetworkRoles.Any());
            Assert.IsFalse(_network.NetworkPortfolios.Any());
            Assert.IsFalse(_network.NetworkKnowledges.Any());
            Assert.IsFalse(_network.NetworkKnowledges.Any());
            Assert.IsFalse(_network.NetworkActivities.Any());
            Assert.IsFalse(_network.NetworkBeliefs.Any());
        }

        [TestMethod]
        public void AddTeamTest()
        {
            _network.AddGroup(_teamId);
            Assert.IsTrue(_network.NetworkGroups.Any());
        }

        /// <summary>
        ///     With network started
        /// </summary>
        [TestMethod]
        public void AddMemberToGroupTest()
        {
            //_network.State = AgentState.Started;
            _network.AddGroup(_teamId);
            _network.NetworkPortfolios.AddPortfolio(_teamId, _componentId, IsSupportOn, 100);
            // Method to test
            _network.AddMemberToGroup(_teammateId, 100, _teamId, true);
            _network.AddMemberToGroup(_teammateId2, 100, _teamId, true);
            // Test link teammates
            Assert.IsTrue(_network.NetworkLinks.HasActiveLink(_teammateId, _teammateId2));
            // Test group
            Assert.IsTrue(_network.IsMemberOfGroup(_teammateId, _teamId));
            // Portfolio
            Assert.IsTrue(_network.HasObject(_teammateId, IsSupportOn));
        }

        /// <summary>
        ///     With network starting
        /// </summary>
        [TestMethod]
        public void AddMemberToGroupTest2()
        {
            //_network.State = AgentState.Starting;
            _network.AddGroup(_teamId);
            _network.NetworkPortfolios.AddPortfolio(_teamId, _componentId, IsSupportOn, 100);
            // Method to test
            _network.AddMemberToGroup(_teammateId, 100, _teamId, false);
            _network.AddMemberToGroup(_teammateId2, 100, _teamId, false);
            // Test link teammates
            Assert.IsFalse(_network.NetworkLinks.HasActiveLink(_teammateId, _teammateId2));
        }

        [TestMethod]
        public void RemoveMemberFromGroupTest()
        {
            _network.AddGroup(_teamId);
            _network.AddMemberToGroup(_teammateId, 100, _teamId, false);
            _network.AddMemberToGroup(_teammateId2, 100, _teamId, false);
            _network.NetworkPortfolios.AddPortfolio(_teamId, _componentId, IsSupportOn, 100);
            // Method to test
            _network.RemoveMemberFromGroup(_teammateId, _teamId);
            Assert.IsFalse(_network.IsMemberOfGroup(_teammateId, _teamId));
            // Test link teammates
            Assert.IsFalse(_network.NetworkLinks.HasActiveLink(_teammateId, _teammateId2));
            // Test group
            Assert.IsFalse(_network.IsMemberOfGroup(_teammateId, _teamId));
            // Test link subordinates
            Assert.IsFalse(_network.NetworkLinks.HasActiveLink(_teammateId, _managerId));
            // Portfolio
            Assert.IsFalse(_network.HasObject(_teammateId, IsSupportOn));
        }

        [TestMethod]
        public void RemoveAgentTest()
        {
            _network.NetworkLinks.AddLink(_teammateId, _managerId);
            _network.NetworkGroups.AddMember(_teammateId, 100, _teamId);
            _network.NetworkRoles.Add(_networkRole);
            _network.NetworkPortfolios.AddPortfolio(_teammateId, _componentId, IsWorkingOn, 100);
            _network.AddKnowledge(_knowledge);
            _network.NetworkKnowledges.Add(_teammateId, _knowledge.Id, KnowledgeLevel.Expert, 0, -1);
            _network.NetworkActivities.AddActivities(_teammateId, _teamId, new List<string> {_activity.Name});
            _network.RemoveAgent(_teammateId);
            //Assert.IsFalse(network.AgentIdExists(teammateId));
            Assert.IsFalse(_network.NetworkLinks.Any());
            Assert.AreEqual(0, _network.NetworkGroups.GetMembersCount(_teamId, _teammateId.ClassKey));
            Assert.IsFalse(_network.NetworkRoles.IsMember(_teammateId, _teamId.ClassKey));
            Assert.IsFalse(_network.NetworkPortfolios.Exists(_teammateId, _componentId, IsWorkingOn));
            Assert.IsFalse(_network.NetworkKnowledges.Any());
            Assert.IsFalse(_network.NetworkActivities.AgentHasActivitiesOn(_teammateId, _teamId));
        }

        [TestMethod]
        public void GetMainGroupTest()
        {
            // Default test
            var agentId = _network.GetMainGroupOrDefault(_teammateId, _teamId.ClassKey);
            Assert.AreEqual(0, agentId.Key);
            Assert.AreEqual(0, agentId.ClassKey);
            // Normal test
            _network.AddMemberToGroup(_teammateId, 100, _teamId, false);
            Assert.AreEqual(_teamId, _network.GetMainGroupOrDefault(_teammateId, _teamId.ClassKey));
            _network.AddMemberToGroup(_teammateId, 10, _teamId2, false);
            Assert.AreEqual(_teamId, _network.GetMainGroupOrDefault(_teammateId, _teamId.ClassKey));
        }

        #region Knowledge

        [TestMethod]
        public void AddKnowledgeTest()
        {
            var knowledge = new Knowledge(1, "1", 1);
            _network.AddKnowledge(knowledge);
            Assert.IsNotNull(_network.NetworkKnowledges.GetKnowledge(knowledge.Id));
            Assert.IsNotNull(_network.NetworkBeliefs.GetBelief(knowledge.Id));
        }

        [TestMethod]
        public void AddKnowledgesTest()
        {
            var knowledge = new Knowledge(1, "1", 1);
            var knowledges = new List<Knowledge> {knowledge};
            _network.AddKnowledges(knowledges);
            Assert.IsNotNull(_network.NetworkKnowledges.GetKnowledge(knowledge.Id));
            Assert.IsNotNull(_network.NetworkBeliefs.GetBelief(knowledge.Id));
        }

        #endregion
    }
}