#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Agent;
using SymuEngine.Classes.Agent.Models.CognitiveArchitecture;
using SymuEngine.Classes.Agent.Models.CognitiveArchitecture.Knowledge;
using SymuEngine.Classes.Task.Knowledge;
using SymuEngine.Repository.Networks;
using SymuEngine.Repository.Networks.Knowledge.Agent;
using SymuEngine.Repository.Networks.Knowledge.Repository;

#endregion

namespace SymuEngineTests.Classes.Agent.Models.CognitiveArchitecture
{
    [TestClass]
    public class KnowledgeAndBeliefsTests
    {
        private readonly AgentId _agentId = new AgentId(1, 1);
        private readonly AgentExpertise _expertise = new AgentExpertise();
        private readonly Knowledge _knowledge = new Knowledge(1, "1", 1);
        private readonly KnowledgeModel _model = new KnowledgeModel();
        private readonly Network _network = new Network();
        private readonly TaskKnowledgeBits _taskBits = new TaskKnowledgeBits();
        private KnowledgeAndBeliefs _knowledgeAndBeliefs;

        [TestInitialize]
        public void Initialize()
        {
            _network.NetworkKnowledges.Add(_agentId, _expertise);
            _knowledgeAndBeliefs = new KnowledgeAndBeliefs(_network, _agentId)
            {
                HasKnowledge = true,
                HasBelief = true
            };
            _taskBits.SetMandatory(new byte[] {0});
            _taskBits.SetRequired(new byte[] {0});
        }

        #region Knowledge

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void CheckKnowledgeTest()
        {
            var mandatoryCheck = false;
            var requiredCheck = false;
            byte mandatoryIndex = 0;
            byte requiredIndex = 0;
            _knowledgeAndBeliefs.CheckKnowledge(1, _taskBits, ref mandatoryCheck, ref requiredCheck, ref mandatoryIndex,
                ref requiredIndex, 0);
            Assert.IsFalse(mandatoryCheck && requiredCheck);
        }

        /// <summary>
        ///     Without the good knowledge
        /// </summary>
        [TestMethod]
        public void CheckKnowledgeTest1()
        {
            var mandatoryCheck = false;
            var requiredCheck = false;
            byte mandatoryIndex = 0;
            byte requiredIndex = 0;
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {0}, 0);
            _expertise.Add(agentKnowledge);
            _network.NetworkKnowledges.Add(_agentId, _expertise);
            _knowledgeAndBeliefs.CheckKnowledge(1, _taskBits, ref mandatoryCheck, ref requiredCheck, ref mandatoryIndex,
                ref requiredIndex, 0);
            Assert.IsFalse(mandatoryCheck && requiredCheck);
        }

        /// <summary>
        ///     With the good knowledge
        /// </summary>
        [TestMethod]
        public void CheckKnowledgeTest2()
        {
            var mandatoryCheck = false;
            var requiredCheck = false;
            byte mandatoryIndex = 0;
            byte requiredIndex = 0;
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {1}, 0);
            _expertise.Add(agentKnowledge);
            _network.NetworkKnowledges.Add(_agentId, _expertise);
            _knowledgeAndBeliefs.CheckKnowledge(1, _taskBits, ref mandatoryCheck, ref requiredCheck, ref mandatoryIndex,
                ref requiredIndex, 0);
            Assert.IsTrue(mandatoryCheck && requiredCheck);
            Assert.AreEqual(0, mandatoryIndex);
            Assert.AreEqual(0, requiredIndex);
        }

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void AddExpertiseTest()
        {
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {0}, 0);
            _expertise.Add(agentKnowledge);
            _knowledgeAndBeliefs.HasKnowledge = false;
            _knowledgeAndBeliefs.AddExpertise(_expertise);
            Assert.AreEqual(0, _network.NetworkKnowledges.GetAgentExpertise(_agentId).Count);
            Assert.IsFalse(_network.NetworkBeliefs.Exists(_agentId));
        }

        /// <summary>
        ///     Passing test
        /// </summary>
        [TestMethod]
        public void AddExpertiseTest1()
        {
            _knowledgeAndBeliefs.HasKnowledge = true;
            _knowledgeAndBeliefs.HasBelief = true;
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {0}, 0);
            _expertise.Add(agentKnowledge);
            _knowledgeAndBeliefs.AddExpertise(_expertise);
            Assert.AreEqual(1, _network.NetworkKnowledges.GetAgentExpertise(_agentId).Count);
            Assert.IsTrue(_network.NetworkBeliefs.Exists(_agentId));
        }

        /// <summary>
        ///     Don't have initial Knowledge
        /// </summary>
        [TestMethod]
        public void InitializeExpertiseTest()
        {
            _knowledgeAndBeliefs.HasInitialKnowledge = false;
            _knowledgeAndBeliefs.InitializeExpertise(0);
            Assert.IsTrue(_network.NetworkKnowledges.Exists(_agentId));
        }

        /// <summary>
        ///     Have initial Knowledge
        /// </summary>
        [TestMethod]
        public void InitializeExpertiseTest1()
        {
            _knowledgeAndBeliefs.HasInitialKnowledge = true;
            _network.AddKnowledge(_knowledge);
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {0}, 0);
            _expertise.Add(agentKnowledge);
            _knowledgeAndBeliefs.AddExpertise(_expertise);
            _knowledgeAndBeliefs.InitializeExpertise(0);
            Assert.IsNotNull(agentKnowledge.KnowledgeBits);
        }

        #endregion

        #region Beliefs

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void AddBeliefsTest()
        {
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {0}, 0);
            _expertise.Add(agentKnowledge);
            _knowledgeAndBeliefs.HasBelief = false;
            _knowledgeAndBeliefs.AddBeliefs(_expertise);
            Assert.IsFalse(_network.NetworkBeliefs.Exists(_agentId));
        }

        /// <summary>
        ///     Passing test
        /// </summary>
        [TestMethod]
        public void AddBeliefsTest1()
        {
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {0}, 0);
            _expertise.Add(agentKnowledge);
            _knowledgeAndBeliefs.HasBelief = true;
            _knowledgeAndBeliefs.AddBeliefs(_expertise);
            Assert.IsTrue(_network.NetworkBeliefs.Exists(_agentId));
            Assert.AreEqual(1, _network.NetworkBeliefs.GetAgentBeliefs(_agentId).Count);
        }

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void NullCheckBeliefTest()
        {
            float mandatoryCheck = 0;
            float requiredCheck = 0;
            byte mandatoryIndex = 0;
            byte requiredIndex = 0;
            Assert.ThrowsException<ArgumentNullException>(() =>
                _knowledgeAndBeliefs.CheckBelief(1, null, ref mandatoryCheck, ref requiredCheck, ref mandatoryIndex,
                    ref requiredIndex));
            // no belief
            Assert.ThrowsException<NullReferenceException>(() => _knowledgeAndBeliefs.CheckBelief(1, _taskBits,
                ref mandatoryCheck, ref requiredCheck, ref mandatoryIndex, ref requiredIndex));
        }

        [TestMethod]
        public void CheckBeliefTest()
        {
            float mandatoryCheck = 0;
            float requiredCheck = 0;
            byte mandatoryIndex = 0;
            byte requiredIndex = 0;
            _network.NetworkBeliefs.AddBelief(_knowledge);
            _network.NetworkBeliefs.Add(_agentId, _knowledge.Id);
            var workerBelief = _network.NetworkBeliefs.GetAgentBeliefs(_agentId).GetBelief(_knowledge.Id);
            workerBelief.InitializeBeliefBits(_model, 1, false);
            // Force beliefBits
            workerBelief.BeliefBits.SetBit(0, 1);
            _network.NetworkBeliefs.GetBelief(_knowledge.Id).Weights.SetBit(0, 1);
            _knowledgeAndBeliefs.CheckBelief(1, _taskBits, ref mandatoryCheck, ref requiredCheck, ref mandatoryIndex,
                ref requiredIndex);
            Assert.AreEqual(1, mandatoryCheck);
            Assert.AreEqual(1, requiredCheck);
        }

        /// <summary>
        ///     Don't have initial belief
        /// </summary>
        [TestMethod]
        public void InitializeBeliefTest()
        {
            _knowledgeAndBeliefs.HasInitialBelief = false;
            _knowledgeAndBeliefs.InitializeBeliefs();
            Assert.IsTrue(_network.NetworkBeliefs.Exists(_agentId));
        }

        /// <summary>
        ///     Have initial Knowledge
        /// </summary>
        [TestMethod]
        public void InitializeBeliefTest1()
        {
            _knowledgeAndBeliefs.HasInitialBelief = true;
            _network.AddKnowledge(_knowledge);
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {0}, 0);
            _expertise.Add(agentKnowledge);
            _knowledgeAndBeliefs.AddExpertise(_expertise);
            _knowledgeAndBeliefs.InitializeBeliefs();
            var agentBelief = _network.NetworkBeliefs.GetAgentBelief(_agentId, _knowledge.Id);
            Assert.IsNotNull(agentBelief);
            Assert.IsNotNull(agentBelief.BeliefBits);
        }

        #endregion
    }
}