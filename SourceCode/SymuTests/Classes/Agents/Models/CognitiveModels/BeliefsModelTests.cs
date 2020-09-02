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
using Symu.DNA.Networks;
using Symu.DNA.Networks.TwoModesNetworks.AgentKnowledge;
using Symu.Messaging.Templates;
using Symu.Repository.Entity;

#endregion

namespace SymuTests.Classes.Agents.Models.CognitiveModels
{
    [TestClass]
    public class BeliefsModelTests
    {
        private readonly AgentId _agentId = new AgentId(1, 1);
        private readonly EmailTemplate _emailTemplate = new EmailTemplate();
        private readonly AgentExpertise _expertise = new AgentExpertise();
        private readonly float[] _knowledge1FBits = {1, 0.5F, 0.3F, 0};
        private readonly TaskKnowledgeBits _taskBits = new TaskKnowledgeBits();
        private Belief _belief;
        private BeliefsModel _beliefsModel;
        private CognitiveArchitecture _cognitiveArchitecture;
        private MetaNetwork _network;
        private AgentBelief _agentBelief;
        private float[] _beliefBitsNeutral;
        private float[] _beliefBitsNonNeutral;

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
            _beliefsModel = new BeliefsModel(_agentId, modelEntity, _cognitiveArchitecture, _network, models.Generator) {On = true};
            _belief = new Belief(1, "1", 1, RandomGenerator.RandomUniform, BeliefWeightLevel.RandomWeight);
            _beliefBitsNonNeutral = _belief.InitializeBits(models.Generator, BeliefLevel.NeitherAgreeNorDisagree);
            _beliefBitsNeutral = _belief.InitializeBits(models.Generator, BeliefLevel.NoBelief);

            _network.Belief.Add(_belief);
            _agentBelief = new AgentBelief(_belief.Id, BeliefLevel.NeitherAgreeNorDisagree);
            _network.AgentBelief.Add(_agentId, _agentBelief);

            _taskBits.SetMandatory(new byte[] {0});
            _taskBits.SetRequired(new byte[] {0});
        }

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void AddBeliefsTest()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasBelief = false;
            var agentKnowledge = new AgentKnowledge(2, new float[] {0}, 0, -1, 0);
            _expertise.Add(agentKnowledge);
            _beliefsModel.AddBeliefs(_expertise);
            Assert.IsFalse(_network.AgentBelief.Exists(_agentId, agentKnowledge.KnowledgeId));
        }

        /// <summary>
        ///     Passing test
        /// </summary>
        [TestMethod]
        public void AddBeliefsTest1()
        {
            var agentKnowledge = new AgentKnowledge(2, new float[] {0}, 0, -1, 0);
            _expertise.Add(agentKnowledge);
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasBelief = true;
            _beliefsModel.AddBeliefs(_expertise);
            Assert.IsTrue(_network.AgentBelief.Exists(_agentId));
            Assert.AreEqual(2, _network.AgentBelief.GetAgentBeliefs(_agentId).Count);
        }

        /// <summary>
        ///     Don't have initial belief
        /// </summary>
        [TestMethod]
        public void InitializeBeliefTest()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasInitialBelief = false;
            _beliefsModel.InitializeBeliefs();
            Assert.IsTrue(_network.AgentBelief.Exists(_agentId));
        }

        /// <summary>
        ///     Have initial Knowledge
        /// </summary>
        [TestMethod]
        public void InitializeBeliefTest1()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasInitialBelief = true;

            _beliefsModel.AddBelief(_belief.Id, BeliefLevel.NeitherAgreeNorDisagree);
            _beliefsModel.InitializeBeliefs();
            var agentBelief = _network.AgentBelief.GetAgentBelief<AgentBelief>(_agentId, _belief.Id);
            Assert.IsNotNull(agentBelief);
            Assert.IsNotNull(agentBelief.BeliefBits);
        }

        /// <summary>
        ///     Model off
        /// </summary>
        [TestMethod]
        public void AddBeliefTest()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasBelief = false;
            _beliefsModel.AddBelief(_belief.Id);
            Assert.ThrowsException<NullReferenceException>(() =>
                _beliefsModel.GetAgentBelief(_belief.Id));
        }

        /// <summary>
        ///     model on
        /// </summary>
        [TestMethod]
        public void AddBeliefTest1()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasBelief = true;
            _beliefsModel.AddBelief(_belief.Id);
            var agentBelief = _beliefsModel.GetAgentBelief(_belief.Id);
            Assert.IsNotNull(agentBelief);
        }

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void NullGetFilteredBeliefToSendTest()
        {
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(_belief.Id, 0, _emailTemplate));
            _cognitiveArchitecture.MessageContent.CanSendBeliefs = true;
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(_belief.Id, 0, _emailTemplate));
        }

        /// <summary>
        ///     Non passing test
        ///     Model Off
        /// </summary>
        [TestMethod]
        public void GetFilteredBeliefToSendTest0()
        {
            _beliefsModel.On = false;
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(_belief.Id, 0, _emailTemplate));
        }

        /// <summary>
        ///     Non passing test
        ///     Can't send belief
        /// </summary>
        [TestMethod]
        public void GetFilteredBeliefToSendTest()
        {
            _cognitiveArchitecture.MessageContent.CanSendBeliefs = false;
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(_belief.Id, 0, _emailTemplate));
        }

        /// <summary>
        ///     Non passing test
        ///     no belief asked
        /// </summary>
        [TestMethod]
        public void GetFilteredBeliefToSendTest1()
        {
            _cognitiveArchitecture.MessageContent.CanSendBeliefs = true;
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(new UId(0), 0, _emailTemplate));
        }

        /// <summary>
        ///     Non passing test
        ///     don't BelievesEnough
        /// </summary>
        [TestMethod]
        public void GetFilteredBeliefToSendTest2()
        {
            _beliefsModel.InitializeBeliefs(true);
            _cognitiveArchitecture.MessageContent.CanSendBeliefs = true;
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(_belief.Id, 0, _emailTemplate));
        }

        /// <summary>
        ///     Non passing test
        ///     MinimumBeliefToSendPerBit too high
        /// </summary>
        [TestMethod]
        public void GetFilteredBeliefToSendTest3()
        {
            _beliefsModel.InitializeBeliefs(false);
            _cognitiveArchitecture.MessageContent.CanSendBeliefs = true;
            _cognitiveArchitecture.MessageContent.MinimumBeliefToSendPerBit = 1;
            var bits = _beliefsModel.FilterBeliefToSend(_belief.Id, 0, _emailTemplate);
            Assert.IsNull(bits);
        }

        /// <summary>
        ///     Passing test
        ///     enough belief
        /// </summary>
        [TestMethod]
        public void GetFilteredBeliefToSendTest4()
        {
            _beliefsModel.InitializeBeliefs(false);
            _network.AgentBelief.GetAgentBelief<AgentBelief>(_agentId, _belief.Id).BeliefBits
                .SetBit(0, 1);
            _cognitiveArchitecture.MessageContent.CanSendBeliefs = true;
            _cognitiveArchitecture.MessageContent.MinimumBeliefToSendPerBit = 0;
            var bits = _beliefsModel.FilterBeliefToSend(_belief.Id, 0, _emailTemplate);
            Assert.IsNotNull(bits);
            Assert.AreEqual(1, bits.GetSum());
        }

        #region Belief

        /// <summary>
        ///     WIth stochastic effect - non passing
        /// </summary>
        [TestMethod]
        public void AskBeliefToSendTest()
        {
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(_belief.Id, 0, _emailTemplate));
        }

        /// <summary>
        ///     WIth stochastic effect
        ///     Passing test
        ///     length to send == 0
        /// </summary>
        [TestMethod]
        public void AskBeliefToSendTest1()
        {
            _cognitiveArchitecture.MessageContent.CanSendBeliefs = true;
            _cognitiveArchitecture.MessageContent.MinimumNumberOfBitsOfBeliefToSend = 0;
            _cognitiveArchitecture.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 0;
            _emailTemplate.MinimumNumberOfBitsOfBeliefToSend = 0;
            _emailTemplate.MaximumNumberOfBitsOfBeliefToSend = 0;
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(_belief.Id, 0, _emailTemplate));
        }

        /// <summary>
        ///     WIth stochastic effect
        ///     Passing test
        ///     MinimumBeliefToSendPerBit > _agentBeliefF
        /// </summary>
        [TestMethod]
        public void AskBeliefToSendTest2()
        {
            _cognitiveArchitecture.MessageContent.CanSendBeliefs = true;
            _cognitiveArchitecture.MessageContent.MinimumBeliefToSendPerBit = 1;
            _cognitiveArchitecture.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 1;
            _cognitiveArchitecture.MessageContent.MinimumNumberOfBitsOfBeliefToSend = 1;
            _emailTemplate.MinimumBeliefToSendPerBit = 1;
            _emailTemplate.MaximumNumberOfBitsOfBeliefToSend = 1;
            _emailTemplate.MinimumNumberOfBitsOfBeliefToSend = 1;
            _beliefsModel.AddBelief(_belief.Id);
            _beliefsModel.InitializeBeliefs();
            _beliefsModel.GetAgentBelief(_belief.Id).BeliefBits.SetBit(0, 0.5F);
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(_belief.Id, 0, _emailTemplate));
        }

        /// <summary>
        ///     WIth stochastic effect
        ///     Passing test
        ///     MinimumBeliefToSendPerBit = 1
        /// </summary>
        [TestMethod]
        public void AskBeliefToSendTest3()
        {
            _cognitiveArchitecture.MessageContent.CanSendBeliefs = true;
            _cognitiveArchitecture.MessageContent.MinimumBeliefToSendPerBit = 1;
            _cognitiveArchitecture.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 2;
            _cognitiveArchitecture.MessageContent.MinimumNumberOfBitsOfBeliefToSend = 2;
            _emailTemplate.MinimumBeliefToSendPerBit = 1;
            _emailTemplate.MaximumNumberOfBitsOfBeliefToSend = 2;
            _emailTemplate.MinimumNumberOfBitsOfBeliefToSend = 2;
            _beliefsModel.AddBelief(_belief.Id);
            _beliefsModel.InitializeBeliefs();
            _beliefsModel.GetAgentBelief(_belief.Id).SetBeliefBits(_knowledge1FBits);
            Assert.AreEqual(1F, _beliefsModel.FilterBeliefToSend(_belief.Id, 0, _emailTemplate).GetSum());
        }

        /// <summary>
        ///     WIth stochastic effect
        ///     Passing test
        ///     MinimumBeliefToSendPerBit = 0.5F
        /// </summary>
        [TestMethod]
        public void AskBeliefToSendTest4()
        {
            _cognitiveArchitecture.MessageContent.CanSendBeliefs = true;
            _cognitiveArchitecture.MessageContent.MinimumBeliefToSendPerBit = 0.4F;
            _cognitiveArchitecture.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 4;
            _cognitiveArchitecture.MessageContent.MinimumNumberOfBitsOfBeliefToSend = 4;
            _emailTemplate.MinimumBeliefToSendPerBit = 0.4F;
            _emailTemplate.MaximumNumberOfBitsOfBeliefToSend = 4;
            _emailTemplate.MinimumNumberOfBitsOfBeliefToSend = 4;
            _beliefsModel.AddBelief(_belief.Id);
            _beliefsModel.InitializeBeliefs();
            _beliefsModel.GetAgentBelief(_belief.Id).SetBeliefBits(_knowledge1FBits);
            Assert.IsTrue(1F <= _beliefsModel.FilterBeliefToSend(_belief.Id, 0, _emailTemplate).GetSum());
        }

        /// <summary>
        ///     Without stochastic effect
        /// </summary>
        [TestMethod]
        public void AskBeliefToSend1Test1()
        {
            _cognitiveArchitecture.MessageContent.CanSendBeliefs = true;
            _cognitiveArchitecture.MessageContent.MinimumBeliefToSendPerBit = 0.4F;
            _cognitiveArchitecture.MessageContent.MinimumNumberOfBitsOfBeliefToSend = 0;
            _cognitiveArchitecture.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 3;
            _beliefsModel.AddBelief(_belief.Id);
            _beliefsModel.InitializeBeliefs();
            _beliefsModel.GetAgentBelief(_belief.Id).SetBeliefBits(_knowledge1FBits);
            var sum = _beliefsModel.FilterBeliefToSend(_belief.Id, 0, _emailTemplate).GetSum();
            Assert.IsTrue(sum <= 3);
            Assert.IsTrue(sum >= 1);
            _cognitiveArchitecture.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 2;
            sum = _beliefsModel.FilterBeliefToSend(_belief.Id, 0, _emailTemplate).GetSum();
            Assert.IsTrue(sum <= 2);
            Assert.IsTrue(sum >= 1);
            _cognitiveArchitecture.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 1;
            sum = _beliefsModel.FilterBeliefToSend(_belief.Id, 0, _emailTemplate).GetSum();
            Assert.AreEqual(1, sum);
            _cognitiveArchitecture.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 0;
            _emailTemplate.MinimumNumberOfBitsOfBeliefToSend = 0;
            _emailTemplate.MaximumNumberOfBitsOfBeliefToSend = 0;
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(_belief.Id, 0, _emailTemplate));
        }

        #endregion


        [TestMethod]
        public void BelievesEnoughTest()
        {
            Assert.IsFalse(_beliefsModel.BelievesEnough(_belief.Id, 0, 1));
            _agentBelief.SetBeliefBits(_beliefBitsNonNeutral);
            Assert.IsTrue(_beliefsModel.BelievesEnough(_belief.Id, 0, 0));
        }

        /// <summary>
        ///     Neutral initialization
        /// </summary>
        [TestMethod]
        public void GetBeliefsSumTest()
        {
            _agentBelief.SetBeliefBits(_beliefBitsNeutral);
            Assert.AreEqual(0, _beliefsModel.GetBeliefsSum());
        }

        /// <summary>
        ///     non neutral initialization
        /// </summary>
        [TestMethod]
        public void GetBeliefsSumTest1()
        {
            _agentBelief.SetBeliefBits(_beliefBitsNonNeutral);
            Assert.AreNotEqual(0, _beliefsModel.GetBeliefsSum());
        }

        /// <summary>
        ///     neutral initialization
        /// </summary>
        [TestMethod]
        public void InitializeBeliefsTest()
        {
            _beliefsModel.InitializeBeliefs(true);
            var agentBelief = _beliefsModel.GetAgentBelief(_belief.Id);
            Assert.IsNotNull(agentBelief.BeliefBits);
            Assert.AreEqual(0, agentBelief.BeliefBits.GetBit(0));
        }

        /// <summary>
        ///     non neutral initialization
        /// </summary>
        [TestMethod]
        public void InitializeBeliefsTest1()
        {
            _beliefsModel.InitializeBeliefs(false);
            var agentBelief = _beliefsModel.GetAgentBelief(_belief.Id);
            Assert.IsNotNull(agentBelief.BeliefBits);
            var t = Convert.ToInt32(agentBelief.BeliefBits.GetBit(0));
            Assert.IsTrue(t == 0 || t == 1 || t == -1);
        }


        [TestMethod]
        public void NullInitializeAgentBeliefTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _beliefsModel.InitializeAgentBelief(null, false));
        }

        /// <summary>
        /// Non neutral
        /// </summary>
        [TestMethod]
        public void InitializeAgentBeliefTest()
        {
            Assert.IsTrue(_agentBelief.BeliefBits.IsNull);
            _beliefsModel.InitializeAgentBelief(_agentBelief, false);
            Assert.IsFalse(_agentBelief.BeliefBits.IsNull);
            Assert.AreNotEqual(0, _agentBelief.BeliefBits.GetBit(0));
        }

        /// <summary>
        /// Neutral
        /// </summary>
        [TestMethod]
        public void InitializeAgentBeliefTest1()
        {
            Assert.IsTrue(_agentBelief.BeliefBits.IsNull);
            _beliefsModel.InitializeAgentBelief(_agentBelief, true);
            Assert.IsFalse(_agentBelief.BeliefBits.IsNull);
            Assert.AreEqual(0, _agentBelief.BeliefBits.GetBit(0));
        }

        /// <summary>
        ///     Passing test, with an agent which don't have still a belief
        /// </summary>
        [TestMethod]
        public void LearnNewBeliefTest()
        {
            _beliefsModel.LearnNewBelief(_belief.Id, BeliefLevel.NoBelief);
            Assert.IsNotNull(_beliefsModel.GetAgentBelief(_belief.Id));
        }
    }
}