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
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Classes.Organization;
using Symu.Classes.Task;
using Symu.Common.Classes;
using Symu.Common.Interfaces;
using Symu.Messaging.Templates;
using Symu.OrgMod.Edges;
using Symu.Repository.Entities;
using SymuTests.Helpers;
using ActorKnowledge = Symu.Repository.Edges.ActorKnowledge;

#endregion

namespace SymuTests.Classes.Agents.Models.CognitiveModels
{
    [TestClass]
    public class KnowledgeModelTests : BaseTestClass
    {
        private const float Threshold = 0.1F;
        private readonly byte[] _taskKnowledge0 = { 0 };
        private readonly byte[] _taskKnowledge1 = { 1 };
        private readonly float[] _knowledgeFloatBits = { 0.1F, 0.1F }; 
        private readonly float[] _knowledge01Bits = { 0, 1 };
        private readonly float[] _knowledge0Bits = { 0, 0 };
        private ActorKnowledge _actorKnowledge0;
        private ActorKnowledge _actorKnowledge01;
        private ActorKnowledge _actorKnowledge1;
        private ActorKnowledge _actorKnowledgeFloat;
        private readonly AgentId _agentId = new AgentId(1, 1);
        private readonly EmailTemplate _emailTemplate = new EmailTemplate();
        private readonly float[] _knowledge1Bits = {1, 1, 1, 1};
        private readonly float[] _knowledge1FBits = {1, 0.5F, 0.3F, 0};
        private readonly TaskKnowledgeBits _taskBits = new TaskKnowledgeBits();
        private CognitiveArchitecture _cognitiveArchitecture;
        private Knowledge _knowledge;
        private Knowledge _knowledge1;
        private Knowledge _knowledge2;
        private Knowledge _knowledge3;
        private KnowledgeModel _knowledgeModel;

        [TestInitialize]
        public void Initialize()
        {
            _cognitiveArchitecture = new CognitiveArchitecture
            {
                KnowledgeAndBeliefs = {HasBelief = true, HasKnowledge = true},
                MessageContent = {CanReceiveBeliefs = true, CanReceiveKnowledge = true},
                InternalCharacteristics = {CanLearn = true, CanForget = true, CanInfluenceOrBeInfluence = true}
            };
            var modelEntity = new KnowledgeModelEntity {On = true};
            _knowledgeModel = new KnowledgeModel(_agentId, modelEntity, _cognitiveArchitecture, Network, RandomGenerator.RandomBinary);
            _knowledge = new Knowledge(Network, Organization.Models, "1", 1);
            _knowledge1 = new Knowledge(Network, Organization.Models, "1", 1);
            _knowledge2 = new Knowledge(Network, Organization.Models, "1", 1);
            _knowledge3 = new Knowledge(Network, Organization.Models, "1", 1);
            _taskBits.SetMandatory(new byte[] {0});
            _taskBits.SetRequired(new byte[] {0});
            _actorKnowledge0 = new ActorKnowledge(_agentId, _knowledge.EntityId, _knowledge0Bits, 0, -1, 0);
            _actorKnowledge1 = new ActorKnowledge(_agentId, _knowledge1.EntityId, _knowledge1Bits, 0, -1, 0);
            _actorKnowledge01 = new ActorKnowledge(_agentId, _knowledge2.EntityId, _knowledge01Bits, 0, -1, 0);
            _actorKnowledgeFloat = new ActorKnowledge(_agentId, _knowledge3.EntityId, _knowledgeFloatBits, 0, -1, 0);
        }

        /// <summary>
        ///     Passing test
        /// </summary>
        [TestMethod]
        public void AddExpertiseTest1()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasKnowledge = true;
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasBelief = true;
            var actorKnowledge = new ActorKnowledge(_agentId, _knowledge.EntityId, new float[] {0}, 0, -1, 0);
            var expertise = new List<IEntityKnowledge>{ actorKnowledge};
            _knowledgeModel.AddExpertise(expertise);
            Assert.AreEqual(1, Network.ActorKnowledge.EdgesFilteredBySource(_agentId).Count());
        }

        /// <summary>
        ///     Don't have initial Knowledge
        /// </summary>
        [TestMethod]
        public void InitializeExpertiseTest()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasInitialKnowledge = false;
            _knowledgeModel.InitializeExpertise(0);
            Assert.IsFalse(Network.ActorKnowledge.ExistsSource(_agentId));
        }

        [TestMethod]
        public void NullInitializeAgentKnowledgeTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _knowledgeModel.InitializeActorKnowledge(null, false, 0));
        }

        [TestMethod]
        public void InitializeAgentKnowledgeTest()
        {
            var actorKnowledgeNo = new ActorKnowledge(_agentId, _knowledge.EntityId, KnowledgeLevel.NoKnowledge, 0, -1);
            Assert.IsTrue(actorKnowledgeNo.KnowledgeBits.IsNull);
            _knowledgeModel.InitializeActorKnowledge(actorKnowledgeNo, false, 0);
            Assert.IsFalse(actorKnowledgeNo.KnowledgeBits.IsNull);
            Assert.AreEqual(0, actorKnowledgeNo.KnowledgeBits.GetBit(0));
        }

        /// <summary>
        ///     Have initial Knowledge
        /// </summary>
        [TestMethod]
        public void InitializeExpertiseTest1()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasInitialKnowledge = true;
            Network.Knowledge.Add(_knowledge);
            _knowledgeModel.AddKnowledge(_knowledge.EntityId, new float[] { 0 }, 0, -1);
            _knowledgeModel.InitializeExpertise(0);
            Assert.IsNotNull(_knowledgeModel.GetActorKnowledge(_knowledge.EntityId).KnowledgeBits);
        }
        /// <summary>
        ///     Non passing test - agent !HasKnowledge
        /// </summary>
        [TestMethod]
        public void AddKnowledgeTest()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasKnowledge = false;
            _knowledgeModel.AddKnowledge(_knowledge.EntityId, KnowledgeLevel.Expert, 0, -1);
            Assert.IsNull(Network.ActorKnowledge.Edge(_agentId, _knowledge.EntityId));
            Assert.IsNull(_knowledgeModel.GetActorKnowledge(_knowledge.EntityId));
        }

        /// <summary>
        ///     model on
        /// </summary>
        [TestMethod]
        public void AddKnowledgeTest1()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasKnowledge = true;
            _knowledgeModel.AddKnowledge(_knowledge.EntityId, KnowledgeLevel.Expert, 0, -1);
            var actorKnowledge = Network.ActorKnowledge.Edge(_agentId, _knowledge.EntityId);
            Assert.IsNotNull(actorKnowledge);
        }

        /// <summary>
        ///     WIth stochastic effect - non passing
        /// </summary>
        [TestMethod]
        public void AskKnowledgeToSendTest()
        {
            Assert.IsNull(_knowledgeModel.FilterKnowledgeToSend(_knowledge.EntityId, 0, _emailTemplate, 0, out _));
        }

        /// <summary>
        ///     Passing test
        ///     Can't send belief
        /// </summary>
        [TestMethod]
        public void GetFilteredKnowledgeToSendTest()
        {
            _cognitiveArchitecture.MessageContent.CanSendKnowledge = false;
            Assert.IsNull(_knowledgeModel.FilterKnowledgeToSend(_knowledge.EntityId, 0, _emailTemplate, 0, out _));
        }

        /// <summary>
        ///     Passing test
        ///     no belief asked
        /// </summary>
        [TestMethod]
        public void GetFilteredKnowledgeToSendTest1()
        {
            _cognitiveArchitecture.MessageContent.CanSendKnowledge = true;
            Assert.IsNull(_knowledgeModel.FilterKnowledgeToSend(new AgentId(0,0), 0, _emailTemplate, 0, out _));
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
            Assert.IsNull(_knowledgeModel.FilterKnowledgeToSend(_knowledge.EntityId, 0, _emailTemplate, 0, out _));
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

            _knowledgeModel.AddKnowledge(_knowledge.EntityId, KnowledgeLevel.Expert, 0, -1);
            _knowledgeModel.InitializeExpertise(0);
            _knowledgeModel.SetKnowledge(_knowledge.EntityId,0, 1, 0);
            var bits = _knowledgeModel.FilterKnowledgeToSend(_knowledge.EntityId, 0, _emailTemplate, 0, out _);
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
            _knowledgeModel.AddKnowledge(_knowledge.EntityId, KnowledgeLevel.Expert, 0, -1);
            _knowledgeModel.InitializeExpertise(0);
            _knowledgeModel.SetKnowledge(_knowledge.EntityId,_knowledge1FBits, 0);
            var bits = _knowledgeModel.FilterKnowledgeToSend(_knowledge.EntityId, 0, _emailTemplate, 0, out _);
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
            _knowledgeModel.AddKnowledge(_knowledge.EntityId, KnowledgeLevel.Expert, 0, -1);
            _knowledgeModel.InitializeExpertise(0);
            _knowledgeModel.SetKnowledge(_knowledge.EntityId,_knowledge1FBits, 0);
            var bits = _knowledgeModel.FilterKnowledgeToSend(_knowledge.EntityId, 0, _emailTemplate, 0, out _);
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
            _knowledgeModel.AddKnowledge(_knowledge.EntityId, KnowledgeLevel.Expert, 0, -1);
            _knowledgeModel.InitializeExpertise(0);
            _knowledgeModel.SetKnowledge(_knowledge.EntityId,_knowledge1FBits, 0);
            var bits = _knowledgeModel.FilterKnowledgeToSend(_knowledge.EntityId, 0, _emailTemplate, 0, out _);
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
            _knowledgeModel.AddKnowledge(_knowledge.EntityId, KnowledgeLevel.Expert, 0, -1);
            _knowledgeModel.InitializeExpertise(0);
            _knowledgeModel.SetKnowledge(_knowledge.EntityId,_knowledge1Bits, 0);
            var bits = _knowledgeModel.FilterKnowledgeToSend(_knowledge.EntityId, 0, _emailTemplate, 0, out _);
            Assert.IsNotNull(bits);
            Assert.IsTrue(1F <= bits.GetSum());
            Assert.IsTrue(bits.GetSum() <= 3);
            _cognitiveArchitecture.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 2;
            bits = _knowledgeModel.FilterKnowledgeToSend(_knowledge.EntityId, 0, _emailTemplate, 0, out _);
            Assert.IsNotNull(bits);
            Assert.IsTrue(1F <= bits.GetSum());
            Assert.IsTrue(bits.GetSum() <= 2);
            _cognitiveArchitecture.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 1;
            bits = _knowledgeModel.FilterKnowledgeToSend(_knowledge.EntityId, 0, _emailTemplate, 0, out _);
            Assert.IsNotNull(bits);
            Assert.AreEqual(1F, bits.GetSum());
            _cognitiveArchitecture.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 0;
            _emailTemplate.MinimumNumberOfBitsOfKnowledgeToSend = 0;
            _emailTemplate.MaximumNumberOfBitsOfKnowledgeToSend = 0;
            bits = _knowledgeModel.FilterKnowledgeToSend(_knowledge.EntityId, 0, _emailTemplate, 0, out _);
            Assert.IsNull(bits);
        }

        [TestMethod]
        public void InitializeKnowledgeTest()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasInitialKnowledge = true;
            _knowledgeModel.AddKnowledge(_knowledge.EntityId, KnowledgeLevel.FullKnowledge, 0, -1);
            _knowledgeModel.InitializeKnowledge(_knowledge.EntityId, 0);
            Assert.IsTrue(Network.ActorKnowledge.Exists(_agentId, _knowledge.EntityId));
            var actorKnowledge = _knowledgeModel.GetActorKnowledge(_knowledge.EntityId);
            Assert.AreEqual(1, actorKnowledge.KnowledgeBits.GetBit(0));
        }

        #region KnowsEnough

        [TestMethod]
        public void CheckTest()
        {
            Assert.IsFalse(_knowledgeModel.Check(_actorKnowledge0, _taskKnowledge0, out var index, Threshold, 0));
            Assert.AreEqual(0, index);
            Assert.IsFalse(_knowledgeModel.Check(_actorKnowledge0, _taskKnowledge1, out index, Threshold, 0));
            Assert.AreEqual(1, index);
            Assert.IsTrue(_knowledgeModel.Check(_actorKnowledge1, _taskKnowledge0, out _, Threshold, 0));
            Assert.IsTrue(_knowledgeModel.Check(_actorKnowledge1, _taskKnowledge1, out _, Threshold, 0));
            Assert.IsFalse(_knowledgeModel.Check(_actorKnowledge01, _taskKnowledge0, out index, Threshold, 0));
            Assert.AreEqual(0, index);
            Assert.IsTrue(_knowledgeModel.Check(_actorKnowledge01, _taskKnowledge1, out _, Threshold, 0));
            Assert.IsTrue(_knowledgeModel.Check(_actorKnowledgeFloat, _taskKnowledge0, out _, Threshold, 0));
            Assert.IsTrue(_knowledgeModel.Check(_actorKnowledgeFloat, _taskKnowledge1, out _, Threshold, 0));
        }

        [TestMethod]
        public void KnowsEnoughTest()
        {
            var knowledge4 = new Knowledge(Network, Organization.Models, "1", 1);
            var actorKnowledge = new ActorKnowledge(_agentId, knowledge4.EntityId, KnowledgeLevel.BasicKnowledge, 0, -1);
            // Non passing test
            Assert.IsFalse(KnowledgeModel.KnowsEnough(actorKnowledge, 0, Threshold, 0));
            // Passing tests
            Assert.IsTrue(KnowledgeModel.KnowsEnough(_actorKnowledge0, 0, 0, 0));
            Assert.IsFalse(KnowledgeModel.KnowsEnough(_actorKnowledge0, 0, Threshold, 0));
            Assert.IsFalse(KnowledgeModel.KnowsEnough(_actorKnowledge0, 1, Threshold, 0));
            Assert.IsTrue(KnowledgeModel.KnowsEnough(_actorKnowledge1, 0, Threshold, 0));
            Assert.IsTrue(KnowledgeModel.KnowsEnough(_actorKnowledge1, 1, Threshold, 0));
            Assert.IsFalse(KnowledgeModel.KnowsEnough(_actorKnowledge01, 0, Threshold, 0));
            Assert.IsTrue(KnowledgeModel.KnowsEnough(_actorKnowledge01, 1, Threshold, 0));
            Assert.IsTrue(KnowledgeModel.KnowsEnough(_actorKnowledgeFloat, 0, Threshold, 0));
            Assert.IsTrue(KnowledgeModel.KnowsEnough(_actorKnowledgeFloat, 1, Threshold, 0));
        }


        [TestMethod]
        public void HasTest()
        {
            float[] bits = { 0, 1 };
            var knowledgeBits = new KnowledgeBits(bits, 0, -1);
            var actorKnowledge = new ActorKnowledge(_agentId, _knowledge.EntityId, knowledgeBits);
            Network.ActorKnowledge.Add(actorKnowledge);
            Assert.IsFalse(_knowledgeModel.KnowsEnough(_knowledge.EntityId, 0, 0.1F, 0));
            Assert.IsTrue(_knowledgeModel.KnowsEnough(_knowledge.EntityId, 1, 0.1F, 0));
        }
        #endregion
    }
}