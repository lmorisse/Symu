#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

#region using directives

#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Classes.Organization;
using Symu.Common;
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.Engine;
using SymuTests.Helpers;

#endregion

#endregion

#endregion

namespace SymuTests.Repository
{
    [TestClass]
    public class WhitePagesTests
    {
        private const byte ClassName1 = 11;
        private readonly TestEnvironment _environment = new TestEnvironment();
        private readonly OrganizationEntity _organizationEntity = new OrganizationEntity("1");
        private readonly SymuEngine _symu = new SymuEngine();

        private TestReactiveAgent _agent;

        [TestInitialize]
        public void Initialize()
        {
            _environment.SetOrganization(_organizationEntity);
            _environment.InitializeIteration();
            _symu.SetEnvironment(_environment);
            _agent = TestReactiveAgent.CreateInstance(_environment.Organization.NextEntityId(), _environment);
        }

        [TestMethod]
        public void ExistAgentTests()
        {
            Assert.IsTrue(_environment.WhitePages.ExistsAgent(_agent.AgentId));
        }

        [TestMethod]
        public void GetAgentTests()
        {
            Assert.AreEqual(_agent, _environment.WhitePages.GetAgent(_agent.AgentId));
        }

        [TestMethod]
        public void ExistAndStartedAgentTests()
        {
            var agentId = new AgentId(1, ClassName1);
            Assert.IsFalse(_environment.WhitePages.ExistsAndStarted(agentId));
            Assert.IsFalse(_environment.WhitePages.ExistsAndStarted(_agent.AgentId));
            Assert.IsFalse(_environment.WhitePages.ExistsAndStarted(_agent.AgentId));
            _agent.Start();
            _environment.WhitePages.WaitingForStart(_agent.AgentId);
            Assert.IsTrue(_environment.WhitePages.ExistsAndStarted(_agent.AgentId));
        }

        [TestMethod]
        public void WaitingForAgentTests()
        {
            _agent.Start();
            _environment.WhitePages.WaitingForStart(_agent.AgentId);
            Assert.IsTrue(_environment.WhitePages.ExistsAndStarted(_agent.AgentId));
        }

        [TestMethod]
        public void TestAgentTest()
        {
            Assert.AreEqual(1, _environment.WhitePages.FilteredAgentsByClassCount(TestCognitiveAgent.ClassId));
        }

        /// <summary>
        ///     2 Agents With The Same Name
        /// </summary>
        [TestMethod]
        public void TestAgentTest1()
        {
            Assert.ThrowsException<ArgumentException>(() => TestReactiveAgent.CreateInstance(_agent.AgentId.Id, _environment));
        }

        [TestMethod]
        public void SetEnvironment()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _symu.SetEnvironment(null));
        }

        [TestMethod]
        public void ClearAgentsTest()
        {
            _environment.Start();
            _environment.WhitePages.WaitingForStart(_agent.AgentId);
            _agent.State = AgentState.Stopping;
            _environment.StopAgents();
            _environment.WhitePages.WaitingForStop(_agent.AgentId);
            _environment.InitializeIteration();
            //Assert
            Assert.IsFalse(_environment.WhitePages.StoppedAgents.Any());
            Assert.AreEqual(0, _environment.WhitePages.MetaNetwork.Agents.Count);
        }

        [TestMethod]
        public void RemoveASingleAgentTests()
        {
            _environment.WhitePages.RemoveAgent(_agent);

            Assert.AreEqual(0, _environment.WhitePages.FilteredAgentsByClassCount(_agent.AgentId.ClassId));
            Assert.AreEqual(1, _environment.WhitePages.StoppedAgents.Count);
        }

        [TestMethod]
        public void ManageAgentsToStopTest()
        {
            _agent.State = AgentState.Stopping;

            _environment.StopAgents();

            Assert.AreEqual(1, _environment.WhitePages.StoppedAgents.Count);
            Assert.AreEqual(0, _environment.WhitePages.FilteredAgentsByClassCount(_agent.AgentId.ClassId));
        }

        /// <summary>
        ///     With one agent
        /// </summary>
        [TestMethod]
        public void WaitingForStartTest()
        {
            _environment.Start();
            _environment.WaitingForStart();
            Assert.IsTrue(_environment.WhitePages.ExistsAndStarted(_agent.AgentId));
        }

        /// <summary>
        ///     With multiple agents
        /// </summary>
        [TestMethod]
        public void WaitingForStartTest1()
        {
            for (byte i = 10; i < 20; i++)
            {
                _ = TestReactiveAgent.CreateInstance(new UId(i), _environment);
            }

            _environment.Start();
            _environment.WaitingForStart();
            foreach (var agentId in _environment.WhitePages.AllAgentIds())
            {
                Assert.IsTrue(_environment.WhitePages.ExistsAndStarted(agentId));
            }
        }

        [TestMethod]
        public void GetFilteredAgentIdsWithExclusionListTest()
        {
            _environment.WhitePages.Clear();
            for (byte i = 0; i < 10; i++)
            {
                _ = TestReactiveAgent.CreateInstance(_environment.Organization.NextEntityId(), _environment);
            }

            var excludeIds = new List<IAgentId>();
            for (byte i = 10; i < 20; i++)
            {
                var agent = TestReactiveAgent.CreateInstance(_environment.Organization.NextEntityId(), _environment);
                excludeIds.Add(agent.AgentId);
            }

            Assert.AreEqual(10,
                _environment.WhitePages.GetFilteredAgentIdsWithExclusionList(TestCognitiveAgent.ClassId, excludeIds).Count);
        }

        [TestMethod]
        public void GetFilteredAgentsWithExclusionListTest()
        {
            _environment.WhitePages.Clear();
            for (byte i = 0; i < 10; i++)
            {
                _ = TestReactiveAgent.CreateInstance(_environment.Organization.NextEntityId(), _environment);
            }

            var excludeIds = new List<IAgentId>();
            for (byte i = 10; i < 20; i++)
            {
                var agent = TestReactiveAgent.CreateInstance(_environment.Organization.NextEntityId(), _environment);
                excludeIds.Add(agent.AgentId);
            }

            Assert.AreEqual(10,
                _environment.WhitePages.GetFilteredAgentsWithExclusionList(TestCognitiveAgent.ClassId, excludeIds).Count());
        }
    }
}