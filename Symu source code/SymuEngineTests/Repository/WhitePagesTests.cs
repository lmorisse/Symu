#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Agent;
using SymuEngine.Classes.Organization;
using SymuEngine.Common;
using SymuEngine.Engine;
using SymuEngineTests.Helpers;

#endregion

namespace SymuEngineTests.Repository
{
    [TestClass]
    public class WhitePagesTests
    {
        private const byte ClassName1 = 11;
        private readonly TestEnvironment _environment = new TestEnvironment();
        private readonly OrganizationEntity _organizationEntity = new OrganizationEntity("1");
        private readonly SimulationEngine _simulation = new SimulationEngine();

        private TestAgent _agent;

        [TestInitialize]
        public void Initialize()
        {
            _environment.SetOrganization(_organizationEntity);
            _environment.InitializeIteration();
            _simulation.SetEnvironment(_environment);
            _agent = new TestAgent(1, _environment);
        }

        [TestMethod]
        public void ExistAgentTests()
        {
            var agentId = new AgentId(1, ClassName1);
            Assert.IsFalse(_environment.WhitePages.ExistsAgent(agentId));
            Assert.IsTrue(_environment.WhitePages.ExistsAgent(_agent.Id));
        }

        [TestMethod]
        public void ExistWithEnvironmentIdAgentTests()
        {
            var agentId = new AgentId(1, ClassName1);
            Assert.IsFalse(_environment.WhitePages.ExistsAgent(agentId));
        }

        [TestMethod]
        public void GetAgentTests()
        {
            var agentId = new AgentId(1, ClassName1);
            Assert.IsNull(_environment.WhitePages.GetAgent(agentId));
            Assert.AreEqual(_agent, _environment.WhitePages.GetAgent(_agent.Id));
        }

        [TestMethod]
        public void ExistAndStartedAgentTests()
        {
            var agentId = new AgentId(1, ClassName1);
            Assert.IsFalse(_environment.WhitePages.ExistsAndStarted(agentId));
            Assert.IsFalse(_environment.WhitePages.ExistsAndStarted(_agent.Id));
            Assert.IsFalse(_environment.WhitePages.ExistsAndStarted(_agent.Id));
            _agent.Start();
            _environment.WhitePages.WaitingForStart(_agent.Id);
            Assert.IsTrue(_environment.WhitePages.ExistsAndStarted(_agent.Id));
        }

        [TestMethod]
        public void WaitingForAgentTests()
        {
            _agent.Start();
            _environment.WhitePages.WaitingForStart(_agent.Id);
            Assert.IsTrue(_environment.WhitePages.ExistsAndStarted(_agent.Id));
        }

        [TestMethod]
        public void TestAgentTest()
        {
            Assert.AreEqual(1, _environment.WhitePages.FilteredAgentsByClassCount(TestAgent.ClassKey));
        }

        /// <summary>
        ///     2 Agents With The Same Name
        /// </summary>
        [TestMethod]
        public void TestAgentTest1()
        {
            Assert.ThrowsException<ArgumentException>(() => new TestAgent(1, _environment));
        }

        /// <summary>
        ///     2 Agents With different Names
        /// </summary>
        [TestMethod]
        public void TestAgentTest2()
        {
            _ = new TestAgent(1, ClassName1, _environment);
            Assert.AreEqual(1, _environment.WhitePages.FilteredAgentsByClassCount(ClassName1));
        }

        [TestMethod]
        public void SetEnvironment()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _simulation.SetEnvironment(null));
        }

        [TestMethod]
        public void ClearAgentsTest()
        {
            var agent = new TestAgent(1, ClassName1, _environment);
            _environment.Start();
            _environment.WhitePages.WaitingForStart(agent.Id);
            agent.State = AgentState.Stopping;
            _environment.ManageAgentsToStop();
            _environment.WhitePages.WaitingForStop(agent.Id);
            _environment.InitializeIteration();
            //Assert
            Assert.IsFalse(_environment.WhitePages.StoppedAgents.Any());
            Assert.AreEqual(0, _environment.WhitePages.Agents.Count);
        }

        [TestMethod]
        public void RemoveASingleAgentTests()
        {
            _environment.WhitePages.RemoveAgent(_agent.Id);

            Assert.AreEqual(0, _environment.WhitePages.FilteredAgentsByClassCount(_agent.Id.ClassKey));
            Assert.AreEqual(1, _environment.WhitePages.StoppedAgents.Count);
        }

        [TestMethod]
        public void ManageAgentsToStopTest()
        {
            _agent.State = AgentState.Stopping;

            _environment.ManageAgentsToStop();

            Assert.AreEqual(1, _environment.WhitePages.StoppedAgents.Count);
            Assert.AreEqual(0, _environment.WhitePages.FilteredAgentsByClassCount(_agent.Id.ClassKey));
        }

        /// <summary>
        ///     With one agent
        /// </summary>
        [TestMethod]
        public void WaitingForStartTest()
        {
            _environment.Start();
            _environment.WaitingForStart();
            Assert.IsTrue(_environment.WhitePages.ExistsAndStarted(_agent.Id));
        }

        /// <summary>
        ///     With multiple agents
        /// </summary>
        [TestMethod]
        public void WaitingForStartTest1()
        {
            for (byte i = 10; i < 20; i++)
            {
                _ = new TestAgent(i, _environment);
            }

            _environment.Start();
            _environment.WaitingForStart();
            foreach (var agentId in _environment.WhitePages.AllAgentIds())
            {
                Assert.IsTrue(_environment.WhitePages.ExistsAndStarted(agentId));
            }
        }
    }
}