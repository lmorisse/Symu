using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Agents;
using SymuEngine.Classes.Agents.Models;
using SymuEngine.Classes.Agents.Models.CognitiveModel;
using SymuEngine.Classes.Agents.Models.Templates.Communication;
using SymuEngine.Classes.Organization;
using SymuEngine.Classes.Task;
using SymuEngine.Repository.Networks;
using SymuEngine.Repository.Networks.Knowledges;

namespace SymuEngineTests.Classes.Agents.Models.CognitiveModel
{
    [TestClass]
    public class KnowledgeModelTests
    {
        private readonly EmailTemplate _emailTemplate = new EmailTemplate();
        private readonly AgentId _agentId = new AgentId(1, 1);
        private Network _network;
        private CognitiveArchitecture _cognitiveArchitecture;
        private Knowledge _knowledge;
        private KnowledgeModel _knowledgeModel;
        private readonly AgentExpertise _expertise = new AgentExpertise();
        private readonly TaskKnowledgeBits _taskBits = new TaskKnowledgeBits();
        private readonly float[] _knowledge1FBits = new[] { 1, 0.5F, 0.3F, 0 };
        private readonly float[] _knowledge1Bits = new float[] { 1, 1, 1, 1 };

        [TestInitialize]
        public void Initialize()
        {
            _network = new Network(new AgentTemplates(), new OrganizationModels());
            _cognitiveArchitecture = new CognitiveArchitecture()
            {
                KnowledgeAndBeliefs = { HasBelief = true, HasKnowledge = true },
                MessageContent = { CanReceiveBeliefs = true, CanReceiveKnowledge = true },
                InternalCharacteristics = { CanLearn = true, CanForget = true, CanInfluenceOrBeInfluence = true }
            };
            var modelEntity = new ModelEntity();
            _knowledgeModel = new KnowledgeModel(_agentId, modelEntity, _cognitiveArchitecture, _network);
            _knowledge = new Knowledge(1, "1", 1);
            _network.NetworkKnowledges.AddKnowledge(_knowledge);
            _network.NetworkKnowledges.Add(_agentId, _expertise);
            _taskBits.SetMandatory(new byte[] { 0 });
            _taskBits.SetRequired(new byte[] { 0 });
        }


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
            _knowledgeModel.CheckKnowledge(1, _taskBits, ref mandatoryCheck, ref requiredCheck, ref mandatoryIndex,
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
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] { 0 }, 0, -1, 0);
            _expertise.Add(agentKnowledge);
            _network.NetworkKnowledges.Add(_agentId, _expertise);
            _knowledgeModel.CheckKnowledge(1, _taskBits, ref mandatoryCheck, ref requiredCheck, ref mandatoryIndex,
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
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] { 1 }, 0, -1, 0);
            _expertise.Add(agentKnowledge);
            _network.NetworkKnowledges.Add(_agentId, _expertise);
            _knowledgeModel.CheckKnowledge(1, _taskBits, ref mandatoryCheck, ref requiredCheck, ref mandatoryIndex,
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
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] { 0 }, 0, -1, 0);
            _expertise.Add(agentKnowledge);
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasKnowledge = false;
            _knowledgeModel.AddExpertise(_expertise);
            Assert.AreEqual(0, _network.NetworkKnowledges.GetAgentExpertise(_agentId).Count);
            Assert.IsFalse(_network.NetworkBeliefs.Exists(_agentId));
        }

        /// <summary>
        ///     Passing test
        /// </summary>
        [TestMethod]
        public void AddExpertiseTest1()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasKnowledge = true;
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasBelief = true;
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] { 0 }, 0, -1, 0);
            _expertise.Add(agentKnowledge);
            _knowledgeModel.AddExpertise(_expertise);
            Assert.AreEqual(1, _network.NetworkKnowledges.GetAgentExpertise(_agentId).Count);
        }

        /// <summary>
        ///     Don't have initial Knowledge
        /// </summary>
        [TestMethod]
        public void InitializeExpertiseTest()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasInitialKnowledge = false;
            _knowledgeModel.InitializeExpertise(0);
            Assert.IsTrue(_network.NetworkKnowledges.Exists(_agentId));
        }

        /// <summary>
        ///     Have initial Knowledge
        /// </summary>
        [TestMethod]
        public void InitializeExpertiseTest1()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasInitialKnowledge = true;
            _network.AddKnowledge(_knowledge);
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] { 0 }, 0, -1, 0);
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
                _network.NetworkKnowledges.GetAgentKnowledge(_agentId, _knowledge.Id));
        }

        /// <summary>
        ///     model on
        /// </summary>
        [TestMethod]
        public void AddKnowledgeTest1()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasKnowledge = true;
            _knowledgeModel.AddKnowledge(_knowledge.Id, KnowledgeLevel.Expert, 0, -1);
            var agentKnowledge = _network.NetworkKnowledges.GetAgentKnowledge(_agentId, _knowledge.Id);
            Assert.IsNotNull(agentKnowledge);
        }

        /// <summary>
        ///     WIth stochastic effect - non passing
        /// </summary>
        [TestMethod]
        public void AskKnowledgeToSendTest()
        {
            Assert.IsNull(_knowledgeModel.FilterKnowledgeToSend(1, 0, _emailTemplate, 0, out _));
        }

        /// <summary>
        ///     Passing test
        ///     Can't send belief
        /// </summary>
        [TestMethod]
        public void GetFilteredKnowledgeToSendTest()
        {
            _cognitiveArchitecture.MessageContent.CanSendKnowledge = false;
            Assert.IsNull(_knowledgeModel.FilterKnowledgeToSend(1, 0, _emailTemplate, 0, out _));
        }

        /// <summary>
        ///     Passing test
        ///     no belief asked
        /// </summary>
        [TestMethod]
        public void GetFilteredKnowledgeToSendTest1()
        {
            _cognitiveArchitecture.MessageContent.CanSendKnowledge = true;
            Assert.IsNull(_knowledgeModel.FilterKnowledgeToSend(0, 0, _emailTemplate, 0, out _));
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
            _knowledgeModel.GetKnowledge(_knowledge.Id).SetKnowledgeBit(0, 1, 0);
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
            _emailTemplate.Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend = 0;
            _emailTemplate.Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 0;
            _knowledgeModel.AddKnowledge(_knowledge.Id, KnowledgeLevel.Expert, 0, -1);
            _knowledgeModel.InitializeExpertise(0);
            _knowledgeModel.GetKnowledge(_knowledge.Id).SetKnowledgeBits(_knowledge1FBits, 0);
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
            _emailTemplate.Cognitive.MessageContent.MinimumKnowledgeToSendPerBit = 1;
            _emailTemplate.Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 2;
            _emailTemplate.Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend = 2;
            _knowledgeModel.AddKnowledge(_knowledge.Id, KnowledgeLevel.Expert, 0, -1);
            _knowledgeModel.InitializeExpertise(0);
            _knowledgeModel.GetKnowledge(_knowledge.Id).SetKnowledgeBits(_knowledge1FBits, 0);
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
            _emailTemplate.Cognitive.MessageContent.MinimumKnowledgeToSendPerBit = 0.4F;
            _emailTemplate.Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 4;
            _emailTemplate.Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend = 4;
            _knowledgeModel.AddKnowledge(_knowledge.Id, KnowledgeLevel.Expert, 0, -1);
            _knowledgeModel.InitializeExpertise(0);
            _knowledgeModel.GetKnowledge(_knowledge.Id).SetKnowledgeBits(_knowledge1FBits,0);
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
            _knowledgeModel.GetKnowledge(_knowledge.Id).SetKnowledgeBits(_knowledge1Bits, 0);
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
            _emailTemplate.Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend = 0;
            _emailTemplate.Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 0;
            bits = _knowledgeModel.FilterKnowledgeToSend(_knowledge.Id, 0, _emailTemplate, 0, out _);
            Assert.IsNull(bits);
        }
    }
}