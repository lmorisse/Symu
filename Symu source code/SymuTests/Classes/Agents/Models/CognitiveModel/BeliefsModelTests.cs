#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models;
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Classes.Organization;
using Symu.Classes.Task;
using Symu.Common;
using Symu.Messaging.Templates;
using Symu.Repository.Networks;
using Symu.Repository.Networks.Beliefs;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace SymuTests.Classes.Agents.Models.CognitiveModel
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

        [TestInitialize]
        public void Initialize()
        {
            var models = new OrganizationModels();
            _network = new MetaNetwork(models.InteractionSphere, models.ImpactOfBeliefOnTask);
            _cognitiveArchitecture = new CognitiveArchitecture
            {
                KnowledgeAndBeliefs = {HasBelief = true, HasKnowledge = true},
                MessageContent = {CanReceiveBeliefs = true, CanReceiveKnowledge = true},
                InternalCharacteristics = {CanLearn = true, CanForget = true, CanInfluenceOrBeInfluence = true}
            };
            var modelEntity = new ModelEntity();
            _beliefsModel = new BeliefsModel(_agentId, modelEntity, _cognitiveArchitecture, _network) {On = true};
            _belief = new Belief(1, "1", 1, RandomGenerator.RandomUniform, BeliefWeightLevel.RandomWeight);

            _network.Beliefs.AddBelief(_belief);
            _network.Beliefs.Add(_agentId, _belief, BeliefLevel.NeitherAgreeNorDisagree);

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
            Assert.IsFalse(_network.Beliefs.Exists(_agentId, 2));
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
            Assert.IsTrue(_network.Beliefs.Exists(_agentId));
            Assert.AreEqual(2, _network.Beliefs.GetAgentBeliefs(_agentId).Count);
        }

        /// <summary>
        ///     Don't have initial belief
        /// </summary>
        [TestMethod]
        public void InitializeBeliefTest()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasInitialBelief = false;
            _beliefsModel.InitializeBeliefs();
            Assert.IsTrue(_network.Beliefs.Exists(_agentId));
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
            var agentBelief = _network.Beliefs.GetAgentBelief(_agentId, _belief.Id);
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
                _beliefsModel.GetBelief(_belief.Id));
        }

        /// <summary>
        ///     model on
        /// </summary>
        [TestMethod]
        public void AddBeliefTest1()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasBelief = true;
            _beliefsModel.AddBelief(_belief.Id);
            var agentBelief = _beliefsModel.GetBelief(_belief.Id);
            Assert.IsNotNull(agentBelief);
        }

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void NullGetFilteredBeliefToSendTest()
        {
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(1, 0, _emailTemplate));
            _cognitiveArchitecture.MessageContent.CanSendBeliefs = true;
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(1, 0, _emailTemplate));
        }

        /// <summary>
        ///     Non passing test
        ///     Model Off
        /// </summary>
        [TestMethod]
        public void GetFilteredBeliefToSendTest0()
        {
            _beliefsModel.On = false;
            _network.Beliefs.Add(_agentId, 1, BeliefLevel.NeitherAgreeNorDisagree);
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(1, 0, _emailTemplate));
        }

        /// <summary>
        ///     Non passing test
        ///     Can't send belief
        /// </summary>
        [TestMethod]
        public void GetFilteredBeliefToSendTest()
        {
            _network.Beliefs.Add(_agentId, 1, BeliefLevel.NeitherAgreeNorDisagree);
            _cognitiveArchitecture.MessageContent.CanSendBeliefs = false;
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(1, 0, _emailTemplate));
        }

        /// <summary>
        ///     Non passing test
        ///     no belief asked
        /// </summary>
        [TestMethod]
        public void GetFilteredBeliefToSendTest1()
        {
            _network.Beliefs.Add(_agentId, 1, BeliefLevel.NeitherAgreeNorDisagree);
            _cognitiveArchitecture.MessageContent.CanSendBeliefs = true;
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(0, 0, _emailTemplate));
        }

        /// <summary>
        ///     Non passing test
        ///     don't BelievesEnough
        /// </summary>
        [TestMethod]
        public void GetFilteredBeliefToSendTest2()
        {
            _network.Beliefs.AddBelief(_belief);
            _network.Beliefs.Add(_agentId, _belief.Id, BeliefLevel.NeitherAgreeNorDisagree);
            _network.Beliefs.InitializeBeliefs(_agentId, true);
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
            _network.Beliefs.AddBelief(_belief);
            _network.Beliefs.Add(_agentId, _belief.Id, BeliefLevel.NeitherAgreeNorDisagree);
            _network.Beliefs.InitializeBeliefs(_agentId, false);
            _cognitiveArchitecture.MessageContent.CanSendBeliefs = true;
            _cognitiveArchitecture.MessageContent.MinimumBeliefToSendPerBit = 1;
            var bits = _beliefsModel.FilterBeliefToSend(1, 0, _emailTemplate);
            Assert.IsNull(bits);
        }

        /// <summary>
        ///     Passing test
        ///     enough belief
        /// </summary>
        [TestMethod]
        public void GetFilteredBeliefToSendTest4()
        {
            _network.Beliefs.AddBelief(_belief);
            _network.Beliefs.Add(_agentId, _belief.Id, BeliefLevel.NeitherAgreeNorDisagree);
            _network.Beliefs.InitializeBeliefs(_agentId, false);
            _network.Beliefs.GetAgentBelief(_agentId, _belief.Id).BeliefBits
                .SetBit(0, 1);
            _cognitiveArchitecture.MessageContent.CanSendBeliefs = true;
            _cognitiveArchitecture.MessageContent.MinimumBeliefToSendPerBit = 0;
            var bits = _beliefsModel.FilterBeliefToSend(1, 0, _emailTemplate);
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
            _beliefsModel.GetBelief(_belief.Id).BeliefBits.SetBit(0, 0.5F);
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
            _beliefsModel.GetBelief(_belief.Id).SetBeliefBits(_knowledge1FBits);
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
            _beliefsModel.GetBelief(_belief.Id).SetBeliefBits(_knowledge1FBits);
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
            _beliefsModel.GetBelief(_belief.Id).SetBeliefBits(_knowledge1FBits);
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
    }
}