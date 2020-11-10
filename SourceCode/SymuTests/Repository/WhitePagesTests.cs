#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

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

using Symu.Common.Interfaces;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Common;
using SymuTests.Helpers;

#endregion

#endregion

#endregion

#endregion

namespace SymuTests.Repository
{
    [TestClass]
    public class WhitePagesTests : BaseTestClass
    {
        private const byte ClassName1 = 11;

        private TestReactiveAgent _agent;

        [TestInitialize]
        public void Initialize()
        {
            Environment.SetOrganization(MainOrganization);
            Simulation.Initialize(Environment);
        }

        [TestMethod]
        public void ExistAgentTests()
        {
            _agent = TestReactiveAgent.CreateInstance(Environment);
            Assert.IsTrue(Environment.AgentNetwork.ExistsAgent(_agent.AgentId));
        }

        [TestMethod]
        public void GetAgentTests()
        {
            _agent = TestReactiveAgent.CreateInstance(Environment);
            Assert.AreEqual(_agent, Environment.AgentNetwork.GetAgent(_agent.AgentId));
        }

        [TestMethod]
        public void ExistAndStartedAgentTests()
        {
            var agentId = new AgentId(1, ClassName1);
            Assert.IsFalse(Environment.AgentNetwork.ExistsAndStarted(agentId));
            _agent = TestReactiveAgent.CreateInstance(Environment);
            _agent.Start();
            Environment.AgentNetwork.WaitingForStart(_agent.AgentId);
            Assert.IsTrue(Environment.AgentNetwork.ExistsAndStarted(_agent.AgentId));
        }

        [TestMethod]
        public void WaitingForAgentTests()
        {
            _agent = TestReactiveAgent.CreateInstance(Environment);
            _agent.Start();
            Environment.AgentNetwork.WaitingForStart(_agent.AgentId);
            Assert.IsTrue(Environment.AgentNetwork.ExistsAndStarted(_agent.AgentId));
        }

        [TestMethod]
        public void ClearTest()
        {
            _agent = TestReactiveAgent.CreateInstance(Environment);
            Environment.AgentNetwork.StoppedAgents.Add(_agent);
            //_environment.Organization.MetaNetwork.Actor.Add(new ActorEntity(_environment.Organization.MetaNetwork));
            Environment.AgentNetwork.Clear();
            Assert.AreEqual(1, Environment.AgentNetwork.Agents.Count);
            Assert.IsFalse(Environment.AgentNetwork.StoppedAgents.Any());
            //Assert.IsFalse(_environment.Organization.MetaNetwork.Actor.Any());
        }

        [TestMethod]
        public void SetEnvironment()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Simulation.SetEnvironment(null));
        }

        [TestMethod]
        public void InitializeIterationTest()
        {
            _agent = TestReactiveAgent.CreateInstance(Environment);
            Environment.Start();
            Environment.AgentNetwork.WaitingForStart(_agent.AgentId);
            _agent.State = AgentState.Stopping;
            Environment.StopAgents();
            Environment.AgentNetwork.WaitingForStop(_agent.AgentId);
            Environment.InitializeIteration();
            //Assert
            Assert.IsFalse(Environment.AgentNetwork.StoppedAgents.Any());
            Assert.AreEqual(0, Environment.MainOrganization.ArtifactNetwork.Actor.Count);
        }

        [TestMethod]
        public void RemoveASingleAgentTests()
        {
            _agent = TestReactiveAgent.CreateInstance(Environment);
            Environment.AgentNetwork.RemoveAgent(_agent);

            Assert.AreEqual(0, Environment.AgentNetwork.FilteredAgentsByClassCount(_agent.AgentId.ClassId));
            Assert.AreEqual(1, Environment.AgentNetwork.StoppedAgents.Count);
        }

        [TestMethod]
        public void ManageAgentsToStopTest()
        {
            _agent = TestReactiveAgent.CreateInstance(Environment);
            _agent.State = AgentState.Stopping;

            Environment.StopAgents();

            Assert.AreEqual(1, Environment.AgentNetwork.StoppedAgents.Count);
            Assert.AreEqual(0, Environment.AgentNetwork.FilteredAgentsByClassCount(_agent.AgentId.ClassId));
        }

        /// <summary>
        ///     With one agent
        /// </summary>
        [TestMethod]
        public void WaitingForStartTest()
        {
            _agent = TestReactiveAgent.CreateInstance(Environment);
            Environment.Start();
            Environment.WaitingForStart();
            Assert.IsTrue(Environment.AgentNetwork.ExistsAndStarted(_agent.AgentId));
        }

        /// <summary>
        ///     With multiple agents
        /// </summary>
        [TestMethod]
        public void WaitingForStartTest1()
        {
            for (byte i = 0; i < 20; i++)
            {
                _ = TestReactiveAgent.CreateInstance(Environment);
            }

            Environment.Start();
            Environment.WaitingForStart();
            foreach (var agentId in Environment.AgentNetwork.AllAgentIds())
            {
                Assert.IsTrue(Environment.AgentNetwork.ExistsAndStarted(agentId));
            }
        }

        [TestMethod]
        public void GetFilteredAgentIdsWithExclusionListTest()
        {
            for (byte i = 0; i < 10; i++)
            {
                TestReactiveAgent.CreateInstance(Environment);
            }

            var excludeIds = new List<IAgentId>();
            for (byte i = 10; i < 20; i++)
            {
                var agent = TestReactiveAgent.CreateInstance(Environment);
                excludeIds.Add(agent.AgentId);
            }

            Assert.AreEqual(10,
                Environment.AgentNetwork.GetFilteredAgentIdsWithExclusionList(TestCognitiveAgent.ClassId, excludeIds)
                    .Count);
        }

        [TestMethod]
        public void GetFilteredAgentsWithExclusionListTest()
        {
            Environment.AgentNetwork.Clear();
            for (byte i = 0; i < 10; i++)
            {
                _ = TestReactiveAgent.CreateInstance(Environment);
            }

            var excludeIds = new List<IAgentId>();
            for (byte i = 10; i < 20; i++)
            {
                var agent = TestReactiveAgent.CreateInstance(Environment);
                excludeIds.Add(agent.AgentId);
            }

            Assert.AreEqual(10,
                Environment.AgentNetwork.GetFilteredAgentsWithExclusionList(TestCognitiveAgent.ClassId, excludeIds)
                    .Count());
        }
    }
}