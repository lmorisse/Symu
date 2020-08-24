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
using Symu.Common.Interfaces;
using Symu.Common.Interfaces.Agent;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace SymuTests.Repository.Networks.Knowledges
{
    [TestClass]
    public class NetworkKnowledgesTests
    {
        private readonly AgentId _agentId = new AgentId(1, 1);

        private readonly Knowledge _knowledge =
            new Knowledge(1, "1", 10);

        private readonly KnowledgeNetwork _knowledgeNetwork = new KnowledgeNetwork();

        [TestInitialize]
        public void Initialize()
        {
            _knowledgeNetwork.AddKnowledge(_knowledge);
        }

        [TestMethod]
        public void InitializeExpertiseTest()
        {
            _knowledgeNetwork.Add(_agentId, _knowledge.Id, KnowledgeLevel.Intermediate, 0, -1);
            _knowledgeNetwork.InitializeExpertise(_agentId, false, 0);
            Assert.AreEqual(10, _knowledgeNetwork.AgentsRepository[_agentId].List[0].Length);
        }

        [TestMethod]
        public void FilterAgentsWithKnowledgeTest()
        {
            var agentIds = new List<IAgentId>
            {
                _agentId
            };
            // Non passing tests
            var filteredAgents = _knowledgeNetwork.FilterAgentsWithKnowledge(agentIds, 0);
            Assert.AreEqual(0, filteredAgents.Count());
            filteredAgents = _knowledgeNetwork.FilterAgentsWithKnowledge(agentIds, _knowledge.Id);
            Assert.AreEqual(0, filteredAgents.Count());
            // Passing test
            _knowledgeNetwork.Add(_agentId, _knowledge.Id, KnowledgeLevel.Intermediate, 0, -1);
            filteredAgents = _knowledgeNetwork.FilterAgentsWithKnowledge(agentIds, _knowledge.Id);
            Assert.AreEqual(1, filteredAgents.Count());
        }

        [TestMethod]
        public void AddAgentIdTest()
        {
            var agentId2 = new AgentId(2, 1);
            Assert.IsFalse(_knowledgeNetwork.Exists(agentId2));
            _knowledgeNetwork.AddAgentId(agentId2);
            Assert.IsTrue(_knowledgeNetwork.Exists(agentId2));
        }

        /// <summary>
        ///     Add expertise
        /// </summary>
        [TestMethod]
        public void AddTest()
        {
            Assert.IsFalse(_knowledgeNetwork.Exists(_agentId));
            Assert.IsFalse(_knowledgeNetwork.Exists(_agentId, _knowledge.Id));
            _knowledgeNetwork.Add(_agentId, _knowledge.Id, KnowledgeLevel.Intermediate, 0, -1);
            Assert.IsTrue(_knowledgeNetwork.Exists(_agentId));
            Assert.IsTrue(_knowledgeNetwork.Exists(_agentId, _knowledge.Id));
        }

        [TestMethod]
        public void AddKnowledgeTest()
        {
            _knowledgeNetwork.AddAgentId(_agentId);
            Assert.IsFalse(_knowledgeNetwork.Exists(_agentId, _knowledge.Id));
            _knowledgeNetwork.AddKnowledge(_agentId, _knowledge.Id, KnowledgeLevel.BasicKnowledge, 0, -1);
            Assert.IsTrue(_knowledgeNetwork.Exists(_agentId, _knowledge.Id));
        }

        [TestMethod]
        public void GetKnowledgeIdsTest()
        {
            _knowledgeNetwork.AddAgentId(_agentId);
            Assert.AreEqual(0, _knowledgeNetwork.GetKnowledgeIds(_agentId).Count());
            _knowledgeNetwork.AddKnowledge(_agentId, _knowledge.Id, KnowledgeLevel.BasicKnowledge, 0, -1);
            Assert.AreEqual(1, _knowledgeNetwork.GetKnowledgeIds(_agentId).Count());
        }

        [TestMethod]
        public void RemoveAgentTest()
        {
            _knowledgeNetwork.AddAgentId(_agentId);
            _knowledgeNetwork.RemoveAgent(_agentId);
            Assert.IsFalse(_knowledgeNetwork.Exists(_agentId));
        }

        [TestMethod]
        public void ClearTest()
        {
            _knowledgeNetwork.Add(_agentId, _knowledge.Id, KnowledgeLevel.Expert, 0, -1);
            _knowledgeNetwork.Clear();
            Assert.IsFalse(_knowledgeNetwork.Any());
        }

        [TestMethod]
        public void ExistsTest()
        {
            Assert.IsFalse(_knowledgeNetwork.Exists(_agentId));
            _knowledgeNetwork.Add(_agentId, _knowledge.Id, KnowledgeLevel.Expert, 0, -1);
            Assert.IsTrue(_knowledgeNetwork.Exists(_agentId));
        }

        [TestMethod]
        public void ExistsAgentTest()
        {
            Assert.IsFalse(_knowledgeNetwork.Exists(_agentId, _knowledge.Id));
            _knowledgeNetwork.Add(_agentId, _knowledge.Id, KnowledgeLevel.Expert, 0, -1);
            Assert.IsTrue(_knowledgeNetwork.Exists(_agentId, _knowledge.Id));
        }

        /// <summary>
        ///     Add knowledge
        /// </summary>
        [TestMethod]
        public void Add1Test()
        {
            Assert.IsFalse(_knowledgeNetwork.Any());
            _knowledgeNetwork.Add(_agentId, _knowledge.Id, KnowledgeLevel.Expert, 0, -1);
            Assert.IsTrue(_knowledgeNetwork.Any());
        }

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void GetAgentExpertiseTest()
        {
            Assert.ThrowsException<NullReferenceException>(() => _knowledgeNetwork.GetAgentExpertise(_agentId));
        }

        /// <summary>
        ///     Passing test
        /// </summary>
        [TestMethod]
        public void GetAgentExpertiseTest1()
        {
            _knowledgeNetwork.Add(_agentId, _knowledge.Id, KnowledgeLevel.Expert, 0, -1);
            var agentExpertise = _knowledgeNetwork.GetAgentExpertise(_agentId);
            Assert.IsNotNull(agentExpertise);
        }

        [TestMethod]
        public void AddTest1()
        {
            var agentExpertise = new AgentExpertise();
            var agentKnowledge = new AgentKnowledge(1, new float[] {1}, KnowledgeLevel.Expert, 0, -1, 0);
            agentExpertise.Add(agentKnowledge);
            _knowledgeNetwork.Add(_agentId, agentExpertise);
            agentExpertise = _knowledgeNetwork.GetAgentExpertise(_agentId);
            Assert.IsNotNull(agentExpertise);
        }

        [TestMethod]
        public void NullInitializeAgentKnowledgeTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _knowledgeNetwork.InitializeAgentKnowledge(null, false, 0));
        }

        [TestMethod]
        public void InitializeAgentKnowledgeTest()
        {
            var agentKnowledge = new AgentKnowledge(1, KnowledgeLevel.NoKnowledge, 0, -1);
            Assert.IsTrue(agentKnowledge.KnowledgeBits.IsNull);
            _knowledgeNetwork.InitializeAgentKnowledge(agentKnowledge, false, 0);
            Assert.IsFalse(agentKnowledge.KnowledgeBits.IsNull);
            Assert.AreEqual(0, agentKnowledge.KnowledgeBits.GetBit(0));
        }

        [TestMethod]
        public void GetAgentKnowledgeTest()
        {
            Assert.ThrowsException<NullReferenceException>(() => _knowledgeNetwork.GetAgentKnowledge(_agentId, _knowledge.Id));
            _knowledgeNetwork.Add(_agentId, _knowledge.Id, KnowledgeLevel.NoKnowledge, 0, -1);
            Assert.IsNotNull(_knowledgeNetwork.GetAgentKnowledge(_agentId, _knowledge.Id));
        }

        [TestMethod]
        public void LearnNewKnowledgeTest()
        {
            _knowledgeNetwork.AddAgentId(_agentId);
            _knowledgeNetwork.LearnNewKnowledge(_agentId, _knowledge.Id, 0, -1, 0);
            var agentKnowledge = _knowledgeNetwork.GetAgentKnowledge(_agentId, _knowledge.Id);
            Assert.IsNotNull(agentKnowledge);
            Assert.IsFalse(agentKnowledge.KnowledgeBits.IsNull);
            Assert.AreEqual(0, agentKnowledge.KnowledgeBits.GetBit(0));
        }
    }
}