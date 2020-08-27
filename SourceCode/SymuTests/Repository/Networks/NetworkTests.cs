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
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.DNA.Activities;
using Symu.Repository;
using Symu.Repository.Entity;
using Symu.Repository.Networks;
using Symu.Repository.Networks.Beliefs;
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
        private readonly TestResource _component = new TestResource(6);

        private readonly Knowledge _knowledge =
            new Knowledge(1, "1", 1);

        private MetaNetwork _network ;
        private readonly AgentId _teamId = new AgentId(1, 1);
        private readonly AgentId _teamId2 = new AgentId(2, 1);
        private readonly AgentId _teammateId = new AgentId(4, SymuYellowPages.Actor);
        private readonly AgentId _teammateId2 = new AgentId(5, SymuYellowPages.Actor);
        private readonly AgentId _managerId = new AgentId(3, 2);
        private Belief _belief;
        private TestAgentRole _testAgentRole;
        private TestAgentResource _agentResource;
        private AgentGroup _agentGroup1;
        private AgentGroup _agentGroup2;
        private AgentKnowledge _agentKnowledge;

        [TestInitialize]
        public void Initialize()
        {
            var models = new OrganizationModels();
            _network = new MetaNetwork(models.InteractionSphere);
            _belief = new Belief(1, "1", 1,
                _network.Knowledge.Model, BeliefWeightLevel.RandomWeight);
            _network.Activities.AddActivities(new List<Activity> {_activity}, _teamId);
            _testAgentRole = new TestAgentRole(_managerId, _teamId, 1);

            _agentResource = new TestAgentResource(_component.Id, new ResourceUsage(IsSupportOn), 100);
            _agentGroup1 = new AgentGroup(_teammateId, 100);
            _agentGroup2 = new AgentGroup(_teammateId2, 100);
            _agentKnowledge = new AgentKnowledge(_knowledge.Id, KnowledgeLevel.Expert, 0, -1);
        }

        [TestMethod]
        public void ClearTest()
        {
            var interaction = new Interaction(_teammateId, _managerId);
            _network.Interactions.AddInteraction(interaction);
            _network.Groups.AddGroup(_teamId);
            _network.Roles.Add(_testAgentRole);
            _network.Resources.Add(_component);
            _network.AddKnowledge(_knowledge, BeliefWeightLevel.NoWeight);
            _network.Knowledge.Add(_teammateId, _agentKnowledge);
            var agentBelief = new AgentBelief(_belief.Id, BeliefLevel.NeitherAgreeNorDisagree);
            _network.Beliefs.Add(_teammateId, agentBelief);
            _network.Activities.AddActivities(_teammateId, _teamId, new List<IAgentActivity> { new AgentActivity(_teammateId, _activity) });
            _network.Clear();
            Assert.IsFalse(_network.Interactions.Any());
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
            _network.Resources.Add(_teamId, _agentResource);
            // Method to test
            _network.AddAgentToGroup(_agentGroup1, _teamId);
            _network.AddAgentToGroup(_agentGroup2, _teamId);
            // Test group
            Assert.IsTrue(_network.Groups.IsMemberOfGroup(_teammateId, _teamId));
            // Resource
            Assert.IsTrue(_network.Resources.HasResource(_teammateId, new ResourceUsage(IsSupportOn)));
        }

        /// <summary>
        ///     With network starting
        /// </summary>
        [TestMethod]
        public void AddMemberToGroupTest2()
        {
            //_network.State = AgentState.Starting;
            _network.Groups.AddGroup(_teamId);
            _network.Resources.Add(_teamId, _agentResource);
            // Method to test
            _network.AddAgentToGroup(_agentGroup1, _teamId);
            _network.AddAgentToGroup(_agentGroup2, _teamId);
            // Test link teammates
            Assert.IsFalse(_network.Interactions.HasActiveInteraction(_teammateId, _teammateId2));
        }

        [TestMethod]
        public void RemoveMemberFromGroupTest()
        {
            _network.Groups.AddGroup(_teamId);
            _network.AddAgentToGroup(_agentGroup1, _teamId);
            _network.AddAgentToGroup(_agentGroup2, _teamId);
            _network.Resources.Add(_teamId, _agentResource);
            // Method to test
            _network.RemoveAgentFromGroup(_teammateId, _teamId);
            Assert.IsFalse(_network.Groups.IsMemberOfGroup(_teammateId, _teamId));
            // Test link teammates
            Assert.IsFalse(_network.Interactions.HasActiveInteraction(_teammateId, _teammateId2));
            // Test group
            Assert.IsFalse(_network.Groups.IsMemberOfGroup(_teammateId, _teamId));
            // Test link subordinates
            Assert.IsFalse(_network.Interactions.HasActiveInteraction(_teammateId, _managerId));
            // Portfolio
            Assert.IsFalse(_network.Resources.HasResource(_teammateId, new ResourceUsage(IsSupportOn)));
        }

        [TestMethod]
        public void RemoveAgentTest()
        {
            var interaction = new Interaction(_teammateId, _managerId);
            _network.Interactions.AddInteraction(interaction);
            _network.Groups.AddAgent(_agentGroup1, _teamId);
            _network.Roles.Add(_testAgentRole);
            _network.Resources.Add(_teammateId, _agentResource);
            _network.AddKnowledge(_knowledge, BeliefWeightLevel.NoWeight);
            _network.Knowledge.Add(_teammateId, _agentKnowledge);
            _network.Activities.AddActivities(_teammateId, _teamId, new List<IAgentActivity> {new AgentActivity(_teammateId, _activity)});
            _network.RemoveAgent(_teammateId);
            //Assert.IsFalse(network.AgentIdExists(teammateId));
            Assert.IsFalse(_network.Interactions.Any());
            Assert.AreEqual(0, _network.Groups.GetAgentsCount(_teamId, _teammateId.ClassId));
            Assert.IsFalse(_network.Roles.IsMember(_teammateId, _teamId.ClassId));
            Assert.IsFalse(_network.Resources.HasResource(_teammateId, _component.Id, new ResourceUsage(IsWorkingOn)));
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
            _network.AddAgentToGroup(_agentGroup1, _teamId);
            Assert.AreEqual(_teamId, _network.Groups.GetMainGroupOrDefault(_teammateId, _teamId.ClassId));
            _network.AddAgentToGroup(_agentGroup1, _teamId2);
            Assert.AreEqual(_teamId, _network.Groups.GetMainGroupOrDefault(_teammateId, _teamId.ClassId));
        }

        #region Knowledge

        [TestMethod]
        public void AddKnowledgeTest()
        {
            var knowledge = new Knowledge(1, "1", 1);
            _network.AddKnowledge(knowledge, BeliefWeightLevel.NoWeight);
            Assert.IsNotNull(_network.Knowledge.GetKnowledge(knowledge.Id));
            Assert.IsNotNull(_network.Beliefs.GetBelief(knowledge.Id));
        }

        [TestMethod]
        public void AddKnowledgesTest()
        {
            var knowledge = new Knowledge(1, "1", 1);
            var knowledges = new List<Knowledge> {knowledge};
            _network.AddKnowledges(knowledges, BeliefWeightLevel.NoWeight);
            Assert.IsNotNull(_network.Knowledge.GetKnowledge(knowledge.Id));
            Assert.IsNotNull(_network.Beliefs.GetBelief(knowledge.Id));
        }

        #endregion
    }
}