#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Agent;
using SymuEngine.Classes.Agent.Models.CognitiveArchitecture.Knowledge;
using SymuEngine.Repository.Networks.Belief;

#endregion

namespace SymuEngineTests.Repository.Networks.Belief
{
    [TestClass]
    public class NetworkBeliefsTests
    {
        private readonly AgentId _agentId = new AgentId(1, 1);

        private readonly SymuEngine.Repository.Networks.Knowledge.Repository.Knowledge _knowledge =
            new SymuEngine.Repository.Networks.Knowledge.Repository.Knowledge(1, "1", 1);

        private readonly KnowledgeModel _model = new KnowledgeModel();
        private readonly NetworkBeliefs _network = new NetworkBeliefs();
        private SymuEngine.Repository.Networks.Belief.Repository.Belief _belief;

        [TestInitialize]
        public void Initialize()
        {
            _belief = new SymuEngine.Repository.Networks.Belief.Repository.Belief(_knowledge.Id, 1, _model);
        }

        [TestMethod]
        public void AddBeliefTest()
        {
            Assert.IsFalse(_network.Exists(_belief));
            _network.AddBelief(_belief);
            Assert.IsTrue(_network.Exists(_belief));
        }

        [TestMethod]
        public void AddBeliefTest1()
        {
            Assert.IsFalse(_network.Exists(_knowledge.Id));
            _network.AddBelief(_knowledge);
            Assert.IsTrue(_network.Exists(_knowledge.Id));
        }

        [TestMethod]
        public void AnyTest()
        {
            Assert.IsFalse(_network.Any());
            _network.AddBelief(_belief);
            Assert.IsFalse(_network.Any());
            _network.Add(_agentId, _belief.Id);
            Assert.IsTrue(_network.Any());
        }

        [TestMethod]
        public void ClearTest()
        {
            _network.AddBelief(_belief);
            _network.AddAgentId(_agentId);
            _network.Clear();
            Assert.IsFalse(_network.Any());
        }

        [TestMethod]
        public void GetBeliefTest()
        {
            Assert.IsNull(_network.GetBelief(_belief.Id));
            _network.AddBelief(_belief);
            Assert.IsNotNull(_network.GetBelief(_belief.Id));
        }

        [TestMethod]
        public void AddBeliefsTest()
        {
            var knowledges = new List<SymuEngine.Repository.Networks.Knowledge.Repository.Knowledge> {_knowledge};
            _network.AddBeliefs(knowledges);
            Assert.IsTrue(_network.Exists(_knowledge.Id));
        }

        [TestMethod]
        public void AddTest()
        {
            Assert.IsFalse(_network.Exists(_agentId));
            _network.Add(_agentId, _belief.Id);
            Assert.IsTrue(_network.Exists(_agentId));
            Assert.IsNotNull(_network.GetAgentBelief(_agentId, _belief.Id));
        }

        [TestMethod]
        public void AddTest1()
        {
            Assert.IsFalse(_network.Exists(_agentId));
            _network.Add(_agentId, _belief);
            Assert.IsTrue(_network.Exists(_agentId));
            Assert.IsNotNull(_network.GetAgentBelief(_agentId, _belief.Id));
        }

        [TestMethod]
        public void AddAgentIdTest()
        {
            Assert.IsFalse(_network.Exists(_agentId));
            _network.AddAgentId(_agentId);
            Assert.IsTrue(_network.Exists(_agentId));
        }

        /// <summary>
        ///     Non passing tests
        /// </summary>
        [TestMethod]
        public void NullInitializeBeliefsTest()
        {
            // AgentId
            Assert.ThrowsException<NullReferenceException>(() => _network.InitializeBeliefs(_agentId, true));
            _network.Add(_agentId, 0);
            // Belief
            Assert.ThrowsException<NullReferenceException>(() => _network.InitializeBeliefs(_agentId, true));
        }

        /// <summary>
        ///     neutral initialization
        /// </summary>
        [TestMethod]
        public void InitializeBeliefsTest()
        {
            _network.AddBelief(_belief);
            _network.Add(_agentId, _belief);
            _network.InitializeBeliefs(_agentId, true);
            var agentBelief = _network.GetAgentBelief(_agentId, _belief.Id);
            Assert.IsNotNull(agentBelief.BeliefBits);
            Assert.AreEqual(0, agentBelief.BeliefBits.GetBit(0));
        }

        /// <summary>
        ///     non neutral initialization
        /// </summary>
        [TestMethod]
        public void InitializeBeliefsTest1()
        {
            _network.AddBelief(_belief);
            _network.Add(_agentId, _belief);
            _network.InitializeBeliefs(_agentId, false);
            var agentBelief = _network.GetAgentBelief(_agentId, _belief.Id);
            Assert.IsNotNull(agentBelief.BeliefBits);
            var t = Convert.ToInt32(agentBelief.BeliefBits.GetBit(0));
            Assert.IsTrue(t == 0 || t == 1 || t == -1);
        }

        [TestMethod]
        public void RemoveAgentTest()
        {
            // no agent Id
            _network.RemoveAgent(_agentId);
            _network.Add(_agentId, _belief);
            _network.RemoveAgent(_agentId);
            Assert.IsFalse(_network.Exists(_belief));
        }

        [TestMethod]
        public void NullGetAgentBeliefsTest()
        {
            Assert.ThrowsException<NullReferenceException>(() => _network.GetAgentBeliefs(_agentId));
        }

        [TestMethod]
        public void GetAgentBeliefsTest()
        {
            _network.Add(_agentId, _belief);
            var agentBeliefs = _network.GetAgentBeliefs(_agentId);
            Assert.AreEqual(1, agentBeliefs.Count);
        }

        [TestMethod]
        public void NullGetAgentBeliefTest()
        {
            Assert.ThrowsException<NullReferenceException>(() => _network.GetAgentBelief(_agentId, _belief.Id));
        }

        [TestMethod]
        public void GetAgentBeliefTest()
        {
            _network.Add(_agentId, _belief);
            Assert.IsNull(_network.GetAgentBelief(_agentId, 0));
            Assert.IsNotNull(_network.GetAgentBelief(_agentId, _belief.Id));
        }

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void NullLearnTest()
        {
            Assert.ThrowsException<NullReferenceException>(() => _network.Learn(_agentId, _belief.Id, null, 1));
        }

        /// <summary>
        ///     Passing test, with an agent which don't have still a belief
        /// </summary>
        [TestMethod]
        public void LearnNewBeliefTest()
        {
            _network.AddBelief(_belief);
            _network.LearnNewBelief(_agentId, _belief.Id);
            Assert.IsNotNull(_network.GetAgentBelief(_agentId, _belief.Id));
        }
    }
}