#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Classes.Organization;
using Symu.Classes.Task;
using Symu.Common.Classes;
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.DNA;
using Symu.DNA.TwoModesNetworks.AgentKnowledge;
using Symu.Messaging.Templates;
using Symu.Repository.Entity;

#endregion

namespace SymuTests.Classes.Agents.Models.CognitiveModels
{
    [TestClass]
    public class KnowledgeModelTests
    {
        private const float Threshold = 0.1F;
        private readonly byte[] _taskKnowledge0 = { 0 };
        private readonly byte[] _taskKnowledge1 = { 1 };
        private readonly float[] _knowledgeFloatBits = { 0.1F, 0.1F }; 
        private readonly float[] _knowledge01Bits = { 0, 1 };
        private readonly float[] _knowledge0Bits = { 0, 0 };
        //private readonly float[] _knowledge1Bits = { 1, 1 };
        private AgentKnowledge _agentKnowledge0;
        private AgentKnowledge _agentKnowledge01;
        private AgentKnowledge _agentKnowledge1;
        private AgentKnowledge _agentKnowledgeFloat;
        private readonly AgentId _agentId = new AgentId(1, 1);
        private readonly EmailTemplate _emailTemplate = new EmailTemplate();
        private AgentExpertise _expertise;
        private readonly float[] _knowledge1Bits = {1, 1, 1, 1};
        private readonly float[] _knowledge1FBits = {1, 0.5F, 0.3F, 0};
        private readonly TaskKnowledgeBits _taskBits = new TaskKnowledgeBits();
        private CognitiveArchitecture _cognitiveArchitecture;
        private Knowledge _knowledge;
        private KnowledgeModel _knowledgeModel;
        private MetaNetwork _network;

        [TestInitialize]
        public void Initialize()
        {
            var models = new OrganizationModels();
            _network = new MetaNetwork(models.InteractionSphere);
            _cognitiveArchitecture = new CognitiveArchitecture
            {
                KnowledgeAndBeliefs = {HasBelief = true, HasKnowledge = true},
                MessageContent = {CanReceiveBeliefs = true, CanReceiveKnowledge = true},
                InternalCharacteristics = {CanLearn = true, CanForget = true, CanInfluenceOrBeInfluence = true}
            };
            var modelEntity = new ModelEntity();
            _knowledgeModel = new KnowledgeModel(_agentId, modelEntity, _cognitiveArchitecture, _network, RandomGenerator.RandomBinary);
            _expertise = _knowledgeModel.Expertise;
            _knowledge = new Knowledge(1, "1", 1);
            _network.Knowledge.AddKnowledge(_knowledge);
            _network.AgentKnowledge.Add(_agentId, _expertise);
            _taskBits.SetMandatory(new byte[] {0});
            _taskBits.SetRequired(new byte[] {0});
            _agentKnowledgeFloat = new AgentKnowledge(3, _knowledgeFloatBits, 0, -1, 0);
            _agentKnowledge0 = new AgentKnowledge(0, _knowledge0Bits, 0, -1, 0);
            _agentKnowledge1 = new AgentKnowledge(1, _knowledge1Bits, 0, -1, 0);
            _agentKnowledge01 = new AgentKnowledge(2, _knowledge01Bits, 0, -1, 0);
        }

        /// <summary>
        ///     Non passing test - agent !HasKnowledge
        /// </summary>
        [TestMethod]
        public void AddExpertiseTest()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasKnowledge = false;
            _knowledgeModel.AddKnowledge(_knowledge.Id, KnowledgeLevel.Expert, 0, -1);
            Assert.AreEqual(0, _network.AgentKnowledge.GetAgentExpertise(_agentId).Count);
            Assert.IsFalse(_network.AgentBelief.Exists(_agentId));
        }

        /// <summary>
        ///     Passing test
        /// </summary>
        [TestMethod]
        public void AddExpertiseTest1()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasKnowledge = true;
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasBelief = true;
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {0}, 0, -1, 0);
            _expertise.Add(agentKnowledge);
            _knowledgeModel.AddExpertise(_expertise);
            Assert.AreEqual(1, _network.AgentKnowledge.GetAgentExpertise(_agentId).Count);
        }

        /// <summary>
        ///     Don't have initial Knowledge
        /// </summary>
        [TestMethod]
        public void InitializeExpertiseTest()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasInitialKnowledge = false;
            _knowledgeModel.InitializeExpertise(0);
            Assert.IsTrue(_network.AgentKnowledge.Exists(_agentId));
        }

        [TestMethod]
        public void NullInitializeAgentKnowledgeTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _knowledgeModel.InitializeAgentKnowledge(null, false, 0));
        }

        [TestMethod]
        public void InitializeAgentKnowledgeTest()
        {
            var agentKnowledgeNo = new AgentKnowledge(_knowledge.Id, KnowledgeLevel.NoKnowledge, 0, -1);
            Assert.IsTrue(agentKnowledgeNo.KnowledgeBits.IsNull);
            _knowledgeModel.InitializeAgentKnowledge(agentKnowledgeNo, false, 0);
            Assert.IsFalse(agentKnowledgeNo.KnowledgeBits.IsNull);
            Assert.AreEqual(0, agentKnowledgeNo.KnowledgeBits.GetBit(0));
        }

        /// <summary>
        ///     Have initial Knowledge
        /// </summary>
        [TestMethod]
        public void InitializeExpertiseTest1()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasInitialKnowledge = true;
            _network.Knowledge.AddKnowledge(_knowledge);
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {0}, 0, -1, 0);
            _expertise.Add(agentKnowledge);
            _knowledgeModel.AddExpertise(_expertise);
            _knowledgeModel.InitializeExpertise(0);
            Assert.IsNotNull(agentKnowledge.KnowledgeBits);
        }

        /// <summary>
        ///     Model off
        /// </summary>
        [TestMethod]
        public void AddKnowledgeTest()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasKnowledge = false;
            _knowledgeModel.AddKnowledge(_knowledge.Id, KnowledgeLevel.Expert, 0, -1);
            Assert.ThrowsException<NullReferenceException>(() =>
                _network.AgentKnowledge.GetAgentKnowledge(_agentId, _knowledge.Id));
        }

        /// <summary>
        ///     model on
        /// </summary>
        [TestMethod]
        public void AddKnowledgeTest1()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasKnowledge = true;
            _knowledgeModel.AddKnowledge(_knowledge.Id, KnowledgeLevel.Expert, 0, -1);
            var agentKnowledge = _network.AgentKnowledge.GetAgentKnowledge(_agentId, _knowledge.Id);
            Assert.IsNotNull(agentKnowledge);
        }

        /// <summary>
        ///     WIth stochastic effect - non passing
        /// </summary>
        [TestMethod]
        public void AskKnowledgeToSendTest()
        {
            Assert.IsNull(_knowledgeModel.FilterKnowledgeToSend(_knowledge.Id, 0, _emailTemplate, 0, out _));
        }

        /// <summary>
        ///     Passing test
        ///     Can't send belief
        /// </summary>
        [TestMethod]
        public void GetFilteredKnowledgeToSendTest()
        {
            _cognitiveArchitecture.MessageContent.CanSendKnowledge = false;
            Assert.IsNull(_knowledgeModel.FilterKnowledgeToSend(_knowledge.Id, 0, _emailTemplate, 0, out _));
        }

        /// <summary>
        ///     Passing test
        ///     no belief asked
        /// </summary>
        [TestMethod]
        public void GetFilteredKnowledgeToSendTest1()
        {
            _cognitiveArchitecture.MessageContent.CanSendKnowledge = true;
            Assert.IsNull(_knowledgeModel.FilterKnowledgeToSend(new UId(0), 0, _emailTemplate, 0, out _));
        }

        /// <summary>
        ///     Passing test
        ///     don't BelievesEnough
        /// </summary>
        [TestMethod]
        public void GetFilteredKnowledgeToSendTest2()
        {
            _cognitiveArchitecture.MessageContent.CanSendKnowledge = true;
            _cognitiveArchitecture.MessageContent.MinimumKnowledgeToSendPerBit = 1;
            Assert.IsNull(_knowledgeModel.FilterKnowledgeToSend(_knowledge.Id, 0, _emailTemplate, 0, out _));
        }

        /// <summary>
        ///     Passing test
        ///     enough belief
        /// </summary>
        [TestMethod]
        public void GetFilteredKnowledgeToSendTest3()
        {
            _cognitiveArchitecture.MessageContent.CanSendKnowledge = true;
            _cognitiveArchitecture.MessageContent.MinimumKnowledgeToSendPerBit = 0;

            _knowledgeModel.AddKnowledge(_knowledge.Id, KnowledgeLevel.Expert, 0, -1);
            _knowledgeModel.InitializeExpertise(0);
            _knowledgeModel.GetAgentKnowledge(_knowledge.Id).SetKnowledgeBit(0, 1, 0);
            var bits = _knowledgeModel.FilterKnowledgeToSend(_knowledge.Id, 0, _emailTemplate, 0, out _);
            Assert.IsNotNull(bits);
            Assert.AreEqual(1, bits.GetSum());
        }

        /// <summary>
        ///     WIth stochastic effect
        ///     Passing test
        ///     length to send == 0
        /// </summary>
        [TestMethod]
        public void AskKnowledgeToSendTest1()
        {
            _cognitiveArchitecture.MessageContent.CanSendKnowledge = true;
            _cognitiveArchitecture.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend = 0;
            _cognitiveArchitecture.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 0;
            _emailTemplate.MinimumNumberOfBitsOfKnowledgeToSend = 0;
            _emailTemplate.MaximumNumberOfBitsOfKnowledgeToSend = 0;
            _knowledgeModel.AddKnowledge(_knowledge.Id, KnowledgeLevel.Expert, 0, -1);
            _knowledgeModel.InitializeExpertise(0);
            _knowledgeModel.GetAgentKnowledge(_knowledge.Id).SetKnowledgeBits(_knowledge1FBits, 0);
            var bits = _knowledgeModel.FilterKnowledgeToSend(_knowledge.Id, 0, _emailTemplate, 0, out _);
            Assert.IsNull(bits);
        }

        /// <summary>
        ///     WIth stochastic effect
        ///     Passing test
        ///     MinimumLengthToSendPerBit = 1
        /// </summary>
        [TestMethod]
        public void AskKnowledgeToSendTest3()
        {
            _cognitiveArchitecture.MessageContent.CanSendKnowledge = true;
            _cognitiveArchitecture.MessageContent.MinimumKnowledgeToSendPerBit = 1;
            _cognitiveArchitecture.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 2;
            _cognitiveArchitecture.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend = 2;
            _emailTemplate.MinimumKnowledgeToSendPerBit = 1;
            _emailTemplate.MaximumNumberOfBitsOfKnowledgeToSend = 2;
            _emailTemplate.MinimumNumberOfBitsOfKnowledgeToSend = 2;
            _knowledgeModel.AddKnowledge(_knowledge.Id, KnowledgeLevel.Expert, 0, -1);
            _knowledgeModel.InitializeExpertise(0);
            _knowledgeModel.GetAgentKnowledge(_knowledge.Id).SetKnowledgeBits(_knowledge1FBits, 0);
            var bits = _knowledgeModel.FilterKnowledgeToSend(_knowledge.Id, 0, _emailTemplate, 0, out _);
            Assert.IsNotNull(bits);
            Assert.AreEqual(1, bits.GetSum());
        }

        /// <summary>
        ///     WIth stochastic effect
        ///     Passing test
        ///     MinimumLengthToSendPerBit = 0.5F
        /// </summary>
        [TestMethod]
        public void AskKnowledgeToSendTest4()
        {
            _cognitiveArchitecture.MessageContent.CanSendKnowledge = true;
            _cognitiveArchitecture.MessageContent.MinimumKnowledgeToSendPerBit = 0.4F;
            _cognitiveArchitecture.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 4;
            _cognitiveArchitecture.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend = 4;
            _emailTemplate.MinimumKnowledgeToSendPerBit = 0.4F;
            _emailTemplate.MaximumNumberOfBitsOfKnowledgeToSend = 4;
            _emailTemplate.MinimumNumberOfBitsOfKnowledgeToSend = 4;
            _knowledgeModel.AddKnowledge(_knowledge.Id, KnowledgeLevel.Expert, 0, -1);
            _knowledgeModel.InitializeExpertise(0);
            _knowledgeModel.GetAgentKnowledge(_knowledge.Id).SetKnowledgeBits(_knowledge1FBits, 0);
            var bits = _knowledgeModel.FilterKnowledgeToSend(_knowledge.Id, 0, _emailTemplate, 0, out _);
            Assert.IsNotNull(bits);
            Assert.IsTrue(1F <= bits.GetSum());
        }

        /// <summary>
        ///     Without stochastic effect
        /// </summary>
        [TestMethod]
        public void AskKnowledgeToSend1Test1()
        {
            _cognitiveArchitecture.MessageContent.CanSendKnowledge = true;
            _cognitiveArchitecture.MessageContent.MinimumKnowledgeToSendPerBit = 0.4F;
            _cognitiveArchitecture.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend = 0;
            _cognitiveArchitecture.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 3;
            _knowledgeModel.AddKnowledge(_knowledge.Id, KnowledgeLevel.Expert, 0, -1);
            _knowledgeModel.InitializeExpertise(0);
            _knowledgeModel.GetAgentKnowledge(_knowledge.Id).SetKnowledgeBits(_knowledge1Bits, 0);
            var bits = _knowledgeModel.FilterKnowledgeToSend(_knowledge.Id, 0, _emailTemplate, 0, out _);
            Assert.IsNotNull(bits);
            Assert.IsTrue(1F <= bits.GetSum());
            Assert.IsTrue(bits.GetSum() <= 3);
            _cognitiveArchitecture.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 2;
            bits = _knowledgeModel.FilterKnowledgeToSend(_knowledge.Id, 0, _emailTemplate, 0, out _);
            Assert.IsNotNull(bits);
            Assert.IsTrue(1F <= bits.GetSum());
            Assert.IsTrue(bits.GetSum() <= 2);
            _cognitiveArchitecture.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 1;
            bits = _knowledgeModel.FilterKnowledgeToSend(_knowledge.Id, 0, _emailTemplate, 0, out _);
            Assert.IsNotNull(bits);
            Assert.AreEqual(1F, bits.GetSum());
            _cognitiveArchitecture.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 0;
            _emailTemplate.MinimumNumberOfBitsOfKnowledgeToSend = 0;
            _emailTemplate.MaximumNumberOfBitsOfKnowledgeToSend = 0;
            bits = _knowledgeModel.FilterKnowledgeToSend(_knowledge.Id, 0, _emailTemplate, 0, out _);
            Assert.IsNull(bits);
        }

        [TestMethod]
        public void InitializeKnowledgeTest()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasInitialKnowledge = true;
            _knowledgeModel.AddKnowledge(_knowledge.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            _knowledgeModel.InitializeKnowledge(_knowledge.Id, 0);
            Assert.IsTrue(_network.AgentKnowledge.Exists(_agentId, _knowledge.Id));
            var agentKnowledge = _knowledgeModel.Expertise.GetAgentKnowledge<AgentKnowledge>(_knowledge.Id);
            Assert.AreEqual(1, agentKnowledge.KnowledgeBits.GetBit(0));
        }

        #region KnowsEnough

        [TestMethod]
        public void CheckTest()
        {
            Assert.IsFalse(_knowledgeModel.Check(_agentKnowledge0, _taskKnowledge0, out var index, Threshold, 0));
            Assert.AreEqual(0, index);
            Assert.IsFalse(_knowledgeModel.Check(_agentKnowledge0, _taskKnowledge1, out index, Threshold, 0));
            Assert.AreEqual(1, index);
            Assert.IsTrue(_knowledgeModel.Check(_agentKnowledge1, _taskKnowledge0, out _, Threshold, 0));
            Assert.IsTrue(_knowledgeModel.Check(_agentKnowledge1, _taskKnowledge1, out _, Threshold, 0));
            Assert.IsFalse(_knowledgeModel.Check(_agentKnowledge01, _taskKnowledge0, out index, Threshold, 0));
            Assert.AreEqual(0, index);
            Assert.IsTrue(_knowledgeModel.Check(_agentKnowledge01, _taskKnowledge1, out _, Threshold, 0));
            Assert.IsTrue(_knowledgeModel.Check(_agentKnowledgeFloat, _taskKnowledge0, out _, Threshold, 0));
            Assert.IsTrue(_knowledgeModel.Check(_agentKnowledgeFloat, _taskKnowledge1, out _, Threshold, 0));
        }

        [TestMethod]
        public void KnowsEnoughTest()
        {
            var agentKnowledge = new AgentKnowledge(4, KnowledgeLevel.BasicKnowledge, 0, -1);
            // Non passing test
            Assert.IsFalse(KnowledgeModel.KnowsEnough(agentKnowledge, 0, Threshold, 0));
            // Passing tests
            Assert.IsTrue(KnowledgeModel.KnowsEnough(_agentKnowledge0, 0, 0, 0));
            Assert.IsFalse(KnowledgeModel.KnowsEnough(_agentKnowledge0, 0, Threshold, 0));
            Assert.IsFalse(KnowledgeModel.KnowsEnough(_agentKnowledge0, 1, Threshold, 0));
            Assert.IsTrue(KnowledgeModel.KnowsEnough(_agentKnowledge1, 0, Threshold, 0));
            Assert.IsTrue(KnowledgeModel.KnowsEnough(_agentKnowledge1, 1, Threshold, 0));
            Assert.IsFalse(KnowledgeModel.KnowsEnough(_agentKnowledge01, 0, Threshold, 0));
            Assert.IsTrue(KnowledgeModel.KnowsEnough(_agentKnowledge01, 1, Threshold, 0));
            Assert.IsTrue(KnowledgeModel.KnowsEnough(_agentKnowledgeFloat, 0, Threshold, 0));
            Assert.IsTrue(KnowledgeModel.KnowsEnough(_agentKnowledgeFloat, 1, Threshold, 0));
        }


        [TestMethod]
        public void HasTest()
        {
            float[] bits = { 0, 1 };
            var knowledgeBits = new KnowledgeBits(bits, 0, -1);
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, knowledgeBits);
            _expertise.Add(agentKnowledge);
            Assert.IsFalse(_knowledgeModel.KnowsEnough(_knowledge.Id, 0, 0.1F, 0));
            Assert.IsTrue(_knowledgeModel.KnowsEnough(_knowledge.Id, 1, 0.1F, 0));
        }
        #endregion
    }
}