#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Agent;
using SymuEngine.Repository.Networks.Knowledge;
using SymuEngine.Repository.Networks.Knowledge.Agent;
using SymuEngine.Repository.Networks.Knowledge.Bits;

#endregion

namespace SymuEngineTests.Repository.Networks.Knowledge
{
    [TestClass]
    public class NetworkKnowledgesTests
    {
        private readonly AgentId _agentId = new AgentId(1, 1);

        private readonly SymuEngine.Repository.Networks.Knowledge.Repository.Knowledge _knowledge =
            new SymuEngine.Repository.Networks.Knowledge.Repository.Knowledge(1, "1", 10);

        private readonly NetworkKnowledges _network = new NetworkKnowledges();

        [TestInitialize]
        public void Initialize()
        {
            _network.AddKnowledge(_knowledge);
        }

        [TestMethod]
        public void InitializeExpertiseTest()
        {
            _network.Add(_agentId, _knowledge, KnowledgeLevel.Intermediate);
            _network.InitializeExpertise(_agentId, false, 0);
            Assert.AreEqual(10, _network.AgentsRepository[_agentId].List[0].Length);
        }

        [TestMethod]
        public void FilterAgentsWithKnowledgeTest()
        {
            var agentIds = new List<AgentId>
            {
                _agentId
            };
            // Non passing tests
            var filteredAgents = _network.FilterAgentsWithKnowledge(agentIds, 0);
            Assert.AreEqual(0, filteredAgents.Count());
            filteredAgents = _network.FilterAgentsWithKnowledge(agentIds, _knowledge.Id);
            Assert.AreEqual(0, filteredAgents.Count());
            // Passing test
            _network.Add(_agentId, _knowledge, KnowledgeLevel.Intermediate);
            filteredAgents = _network.FilterAgentsWithKnowledge(agentIds, _knowledge.Id);
            Assert.AreEqual(1, filteredAgents.Count());
        }

        [TestMethod]
        public void AddAgentIdTest()
        {
            var agentId2 = new AgentId(2, 1);
            Assert.IsFalse(_network.Exists(agentId2));
            _network.AddAgentId(agentId2);
            Assert.IsTrue(_network.Exists(agentId2));
        }

        /// <summary>
        ///     Add expertise
        /// </summary>
        [TestMethod]
        public void AddTest()
        {
            Assert.IsFalse(_network.Exists(_agentId));
            Assert.IsFalse(_network.Exists(_agentId, _knowledge.Id));
            _network.Add(_agentId, _knowledge, KnowledgeLevel.Intermediate);
            Assert.IsTrue(_network.Exists(_agentId));
            Assert.IsTrue(_network.Exists(_agentId, _knowledge.Id));
        }

        [TestMethod]
        public void AddKnowledgeTest()
        {
            _network.AddAgentId(_agentId);
            Assert.IsFalse(_network.Exists(_agentId, _knowledge.Id));
            _network.AddKnowledge(_agentId, _knowledge.Id, KnowledgeLevel.BasicKnowledge);
            Assert.IsTrue(_network.Exists(_agentId, _knowledge.Id));
        }

        [TestMethod]
        public void GetKnowledgeIdsTest()
        {
            _network.AddAgentId(_agentId);
            Assert.AreEqual(0, _network.GetKnowledgeIds(_agentId).Count());
            _network.AddKnowledge(_agentId, _knowledge.Id, KnowledgeLevel.BasicKnowledge);
            Assert.AreEqual(1, _network.GetKnowledgeIds(_agentId).Count());
        }

        [TestMethod]
        public void RemoveAgentTest()
        {
            _network.AddAgentId(_agentId);
            _network.RemoveAgent(_agentId);
            Assert.IsFalse(_network.Exists(_agentId));
        }

        [TestMethod]
        public void ClearTest()
        {
            _network.Add(_agentId, _knowledge, KnowledgeLevel.Expert);
            _network.Clear();
            Assert.IsFalse(_network.Any());
        }

        [TestMethod]
        public void ExistsTest()
        {
            Assert.IsFalse(_network.Exists(_agentId));
            _network.Add(_agentId, _knowledge, KnowledgeLevel.Expert);
            Assert.IsTrue(_network.Exists(_agentId));
        }

        [TestMethod]
        public void ExistsAgentTest()
        {
            Assert.IsFalse(_network.Exists(_agentId, _knowledge.Id));
            _network.Add(_agentId, _knowledge, KnowledgeLevel.Expert);
            Assert.IsTrue(_network.Exists(_agentId, _knowledge.Id));
        }

        /// <summary>
        ///     Add knowledge
        /// </summary>
        [TestMethod]
        public void Add1Test()
        {
            Assert.IsFalse(_network.Any());
            _network.Add(_agentId, _knowledge, KnowledgeLevel.Expert);
            Assert.IsTrue(_network.Any());
        }

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void GetAgentExpertiseTest()
        {
            Assert.ThrowsException<NullReferenceException>(() => _network.GetAgentExpertise(_agentId));
        }

        /// <summary>
        ///     Passing test
        /// </summary>
        [TestMethod]
        public void GetAgentExpertiseTest1()
        {
            _network.Add(_agentId, _knowledge, KnowledgeLevel.Expert);
            var agentExpertise = _network.GetAgentExpertise(_agentId);
            Assert.IsNotNull(agentExpertise);
        }

        [TestMethod]
        public void AddTest1()
        {
            var agentExpertise = new AgentExpertise();
            var agentKnowledge = new AgentKnowledge(1, new float[] {1}, KnowledgeLevel.Expert, 0);
            agentExpertise.Add(agentKnowledge);
            _network.Add(_agentId, agentExpertise);
            agentExpertise = _network.GetAgentExpertise(_agentId);
            Assert.IsNotNull(agentExpertise);
            Assert.AreEqual(1, agentExpertise.GetKnowledgesSum());
        }

        [TestMethod]
        public void NullInitializeAgentKnowledgeTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _network.InitializeAgentKnowledge(null, false, 0));
        }

        [TestMethod]
        public void InitializeAgentKnowledgeTest()
        {
            var agentKnowledge = new AgentKnowledge(1, KnowledgeLevel.NoKnowledge);
            Assert.IsTrue(agentKnowledge.KnowledgeBits.IsNull);
            _network.InitializeAgentKnowledge(agentKnowledge, false, 0);
            Assert.IsFalse(agentKnowledge.KnowledgeBits.IsNull);
            Assert.AreEqual(0, agentKnowledge.KnowledgeBits.GetBit(0));
        }

        [TestMethod]
        public void GetAgentKnowledgeTest()
        {
            Assert.ThrowsException<NullReferenceException>(() => _network.GetAgentKnowledge(_agentId, _knowledge.Id));
            _network.Add(_agentId, _knowledge, KnowledgeLevel.NoKnowledge);
            Assert.IsNotNull(_network.GetAgentKnowledge(_agentId, _knowledge.Id));
        }

        [TestMethod]
        public void LearnNewKnowledgeTest()
        {
            _network.AddAgentId(_agentId);
            _network.LearnNewKnowledge(_agentId, _knowledge.Id, 0);
            var agentKnowledge = _network.GetAgentKnowledge(_agentId, _knowledge.Id);
            Assert.IsNotNull(agentKnowledge);
            Assert.IsFalse(agentKnowledge.KnowledgeBits.IsNull);
            Assert.AreEqual(0, agentKnowledge.KnowledgeBits.GetBit(0));
        }
    }
}