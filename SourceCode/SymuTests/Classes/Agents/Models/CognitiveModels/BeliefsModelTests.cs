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
using Symu.Repository.Edges;
using Symu.Repository.Entities;
using SymuTests.Helpers;
using ActorBelief = Symu.Repository.Edges.ActorBelief;

#endregion


namespace SymuTests.Classes.Agents.Models.CognitiveModels
{
    [TestClass]
    public class BeliefsModelTests : BaseTestClass
    {
        private readonly AgentId _agentId = new AgentId(1, 1);
        private readonly EmailTemplate _emailTemplate = new EmailTemplate();
        private readonly float[] _knowledge1FBits = {1, 0.5F, 0.3F, 0};
        private readonly TaskKnowledgeBits _taskBits = new TaskKnowledgeBits();
        private ActorBelief _actorBelief;
        private Belief _belief;
        private float[] _beliefBitsNeutral;
        private float[] _beliefBitsNonNeutral;
        private BeliefsModel _beliefsModel;
        private CognitiveArchitecture _cognitiveArchitecture;
        private Knowledge _knowledge;

        [TestInitialize]
        public void Initialize()
        {
            _cognitiveArchitecture = new CognitiveArchitecture
            {
                KnowledgeAndBeliefs = {HasBelief = true, HasKnowledge = true},
                MessageContent = {CanReceiveBeliefs = true, CanReceiveKnowledge = true},
                InternalCharacteristics = {CanLearn = true, CanForget = true, CanInfluenceOrBeInfluence = true}
            };
            _knowledge = new Knowledge(Network, MainOrganization.Models, "1", 1);
            var modelEntity = new BeliefModelEntity {On = true};
            _beliefsModel = new BeliefsModel(_agentId, modelEntity, _cognitiveArchitecture, Network,
                MainOrganization.Models.Generator);
            _belief = new Belief(Network, 1, RandomGenerator.RandomUniform, BeliefWeightLevel.RandomWeight);
            _beliefBitsNonNeutral =
                _belief.InitializeBits(MainOrganization.Models.Generator, BeliefLevel.NeitherAgreeNorDisagree);
            _beliefBitsNeutral = _belief.InitializeBits(MainOrganization.Models.Generator, BeliefLevel.NoBelief);

            Network.Belief.Add(_belief);
            _actorBelief = new ActorBelief(_agentId, _belief.EntityId, BeliefLevel.NeitherAgreeNorDisagree);
            Network.ActorBelief.Add(_actorBelief);

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
            var actorKnowledge = new ActorKnowledge(_agentId, _knowledge.EntityId, new float[] {0}, 0, -1);
            var expertise = new List<IEntityKnowledge> {actorKnowledge};
            _beliefsModel.AddBeliefsFromKnowledge(expertise);
            Assert.IsFalse(Network.ActorBelief.Exists(_agentId, actorKnowledge.Target));
        }

        /// <summary>
        ///     Non Passing test
        ///     Belief from knowledge doesn't exists
        /// </summary>
        [TestMethod]
        public void AddBeliefsTest1()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasBelief = true;
            var actorKnowledge = new ActorKnowledge(_agentId, _knowledge.EntityId, new float[] {0}, 0, -1);
            var expertise = new List<IEntityKnowledge> {actorKnowledge};
            Assert.ThrowsException<NullReferenceException>(() => _beliefsModel.AddBeliefsFromKnowledge(expertise));
        }

        /// <summary>
        ///     Passing test
        /// </summary>
        [TestMethod]
        public void AddBeliefsTest2()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasBelief = true;
            var actorKnowledge = new ActorKnowledge(_agentId, _knowledge.EntityId, new float[] {0}, 0, -1);
            _ = new Belief(Network, _knowledge, _knowledge.Length, RandomGenerator.RandomUniform,
                BeliefWeightLevel.RandomWeight);
            var expertise = new List<IEntityKnowledge> {actorKnowledge};
            _beliefsModel.AddBeliefsFromKnowledge(expertise);
            Assert.IsTrue(Network.ActorBelief.ExistsSource(_agentId));
            Assert.AreEqual(2, Network.ActorBelief.EdgesFilteredBySourceCount(_agentId));
        }

        /// <summary>
        ///     Don't have initial belief
        /// </summary>
        [TestMethod]
        public void InitializeBeliefTest()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasInitialBelief = false;
            _beliefsModel.InitializeBeliefs();
            Assert.IsTrue(Network.ActorBelief.ExistsSource(_agentId));
        }

        /// <summary>
        ///     Have initial Knowledge
        /// </summary>
        [TestMethod]
        public void InitializeBeliefTest1()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasInitialBelief = true;

            _beliefsModel.AddBelief(_belief.EntityId, BeliefLevel.NeitherAgreeNorDisagree);
            _beliefsModel.InitializeBeliefs();
            var actorBelief = Network.ActorBelief.Edge<ActorBelief>(_agentId, _belief.EntityId);
            Assert.IsNotNull(actorBelief);
            Assert.IsNotNull(actorBelief.BeliefBits);
        }

        /// <summary>
        ///     Model off
        /// </summary>
        [TestMethod]
        public void AddBeliefTest()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasBelief = false;
            _beliefsModel.AddBelief(_belief.EntityId);
            Assert.IsFalse(_beliefsModel.Beliefs.Any());
        }

        /// <summary>
        ///     model on
        /// </summary>
        [TestMethod]
        public void AddBeliefTest1()
        {
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasBelief = true;
            _beliefsModel.AddBelief(_belief.EntityId);
            var actorBelief = _beliefsModel.GetActorBelief(_belief.EntityId);
            Assert.IsNotNull(actorBelief);
        }

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void NullGetFilteredBeliefToSendTest()
        {
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(_belief.EntityId, 0, _emailTemplate));
            _cognitiveArchitecture.MessageContent.CanSendBeliefs = true;
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(_belief.EntityId, 0, _emailTemplate));
        }

        /// <summary>
        ///     Non passing test
        ///     Model Off
        /// </summary>
        [TestMethod]
        public void GetFilteredBeliefToSendTest0()
        {
            _beliefsModel.Entity.On = false;
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(_belief.EntityId, 0, _emailTemplate));
        }

        /// <summary>
        ///     Non passing test
        ///     Can't send belief
        /// </summary>
        [TestMethod]
        public void GetFilteredBeliefToSendTest()
        {
            _cognitiveArchitecture.MessageContent.CanSendBeliefs = false;
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(_belief.EntityId, 0, _emailTemplate));
        }

        /// <summary>
        ///     Non passing test
        ///     no belief asked
        /// </summary>
        [TestMethod]
        public void GetFilteredBeliefToSendTest1()
        {
            _cognitiveArchitecture.MessageContent.CanSendBeliefs = true;
            Assert.IsNull(_beliefsModel.FilterBeliefToSendFromKnowledgeId(new AgentId(0, 0), 0, _emailTemplate));
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
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(_belief.EntityId, 0, _emailTemplate));
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
            var bits = _beliefsModel.FilterBeliefToSend(_belief.EntityId, 0, _emailTemplate);
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
            Network.ActorBelief.Edge<ActorBelief>(_agentId, _belief.EntityId).BeliefBits
                .SetBit(0, 1);
            _cognitiveArchitecture.MessageContent.CanSendBeliefs = true;
            _cognitiveArchitecture.MessageContent.MinimumBeliefToSendPerBit = 0;
            var bits = _beliefsModel.FilterBeliefToSend(_belief.EntityId, 0, _emailTemplate);
            Assert.IsNotNull(bits);
            Assert.AreEqual(1, bits.GetSum());
        }


        [TestMethod]
        public void BelievesEnoughTest()
        {
            Assert.IsFalse(_beliefsModel.BelievesEnough(_belief.EntityId, 0, 1));
            _actorBelief.SetBeliefBits(_beliefBitsNonNeutral);
            Assert.IsTrue(_beliefsModel.BelievesEnough(_belief.EntityId, 0, 0));
        }

        /// <summary>
        ///     Neutral initialization
        /// </summary>
        [TestMethod]
        public void GetBeliefsSumTest()
        {
            _actorBelief.SetBeliefBits(_beliefBitsNeutral);
            Assert.AreEqual(0, _beliefsModel.GetBeliefsSum());
        }

        /// <summary>
        ///     non neutral initialization
        /// </summary>
        [TestMethod]
        public void GetBeliefsSumTest1()
        {
            _actorBelief.SetBeliefBits(_beliefBitsNonNeutral);
            Assert.AreNotEqual(0, _beliefsModel.GetBeliefsSum());
        }

        /// <summary>
        ///     neutral initialization
        /// </summary>
        [TestMethod]
        public void InitializeBeliefsTest()
        {
            _beliefsModel.InitializeBeliefs(true);
            var actorBelief = _beliefsModel.GetActorBelief(_belief.EntityId);
            Assert.IsNotNull(actorBelief.BeliefBits);
            Assert.AreEqual(0, actorBelief.BeliefBits.GetBit(0));
        }

        /// <summary>
        ///     non neutral initialization
        /// </summary>
        [TestMethod]
        public void InitializeBeliefsTest1()
        {
            _beliefsModel.InitializeBeliefs(false);
            var actorBelief = _beliefsModel.GetActorBelief(_belief.EntityId);
            Assert.IsNotNull(actorBelief.BeliefBits);
            var t = Convert.ToInt32(actorBelief.BeliefBits.GetBit(0));
            Assert.IsTrue(t == 0 || t == 1 || t == -1);
        }


        [TestMethod]
        public void NullInitializeAgentBeliefTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _beliefsModel.InitializeActorBelief(null, false));
        }

        /// <summary>
        ///     Non neutral
        /// </summary>
        [TestMethod]
        public void InitializeAgentBeliefTest()
        {
            Assert.IsTrue(_actorBelief.BeliefBits.IsNull);
            _beliefsModel.InitializeActorBelief(_actorBelief, false);
            Assert.IsFalse(_actorBelief.BeliefBits.IsNull);
            Assert.AreNotEqual(0, _actorBelief.BeliefBits.GetBit(0));
        }

        /// <summary>
        ///     Neutral
        /// </summary>
        [TestMethod]
        public void InitializeAgentBeliefTest1()
        {
            Assert.IsTrue(_actorBelief.BeliefBits.IsNull);
            _beliefsModel.InitializeActorBelief(_actorBelief, true);
            Assert.IsFalse(_actorBelief.BeliefBits.IsNull);
            Assert.AreEqual(0, _actorBelief.BeliefBits.GetBit(0));
        }

        /// <summary>
        ///     Passing test, with an agent which don't have still a belief
        /// </summary>
        [TestMethod]
        public void LearnNewBeliefTest()
        {
            _beliefsModel.LearnNewBelief(_belief.EntityId, BeliefLevel.NoBelief);
            Assert.IsNotNull(_beliefsModel.GetActorBelief(_belief.EntityId));
        }

        [TestMethod]
        public void GetBeliefFromKnowledgeIdTest()
        {
            Assert.IsNull(_beliefsModel.GetBeliefFromKnowledgeId(_knowledge.EntityId));
            var belief = new Belief(Network, _knowledge, _knowledge.Length, RandomGenerator.RandomUniform,
                BeliefWeightLevel.RandomWeight);
            _beliefsModel.AddBeliefFromKnowledgeId(_knowledge.EntityId);
            Assert.AreEqual(belief, _beliefsModel.GetBeliefFromKnowledgeId(_knowledge.EntityId));
        }

        [TestMethod]
        public void GetBeliefIdFromKnowledgeIdTest()
        {
            Assert.IsNull(_beliefsModel.GetBeliefIdFromKnowledgeId(_knowledge.EntityId));
            var belief = new Belief(Network, _knowledge, _knowledge.Length, RandomGenerator.RandomUniform,
                BeliefWeightLevel.RandomWeight);
            _beliefsModel.AddBeliefFromKnowledgeId(_knowledge.EntityId);
            Assert.AreEqual(belief.EntityId, _beliefsModel.GetBeliefIdFromKnowledgeId(_knowledge.EntityId));
        }

        [TestMethod]
        public void AddBeliefFromKnowledgeIdTest()
        {
            Assert.IsNull(_beliefsModel.GetBeliefFromKnowledgeId(_knowledge.EntityId));
            var belief = new Belief(Network, _knowledge, _knowledge.Length, RandomGenerator.RandomUniform,
                BeliefWeightLevel.RandomWeight);
            _beliefsModel.AddBeliefFromKnowledgeId(_knowledge.EntityId);
            Assert.IsNotNull(_beliefsModel.GetBelief(belief.EntityId));
        }

        #region Belief

        /// <summary>
        ///     WIth stochastic effect - non passing
        /// </summary>
        [TestMethod]
        public void AskBeliefToSendTest()
        {
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(_belief.EntityId, 0, _emailTemplate));
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
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(_belief.EntityId, 0, _emailTemplate));
        }

        /// <summary>
        ///     WIth stochastic effect
        ///     Passing test
        ///     MinimumBeliefToSendPerBit > _actorBeliefF
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
            _beliefsModel.AddBelief(_belief.EntityId);
            _beliefsModel.InitializeBeliefs();
            _beliefsModel.SetBelief(_belief.EntityId, 0, 0.5F);
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(_belief.EntityId, 0, _emailTemplate));
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
            _beliefsModel.AddBelief(_belief.EntityId);
            _beliefsModel.InitializeBeliefs();
            _beliefsModel.SetBelief(_belief.EntityId, _knowledge1FBits);
            Assert.AreEqual(1F, _beliefsModel.FilterBeliefToSend(_belief.EntityId, 0, _emailTemplate).GetSum());
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
            _beliefsModel.AddBelief(_belief.EntityId);
            _beliefsModel.InitializeBeliefs();
            _beliefsModel.SetBelief(_belief.EntityId, _knowledge1FBits);
            Assert.IsTrue(1F <= _beliefsModel.FilterBeliefToSend(_belief.EntityId, 0, _emailTemplate).GetSum());
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
            _beliefsModel.AddBelief(_belief.EntityId);
            _beliefsModel.InitializeBeliefs();
            _beliefsModel.SetBelief(_belief.EntityId, _knowledge1FBits);
            var sum = _beliefsModel.FilterBeliefToSend(_belief.EntityId, 0, _emailTemplate).GetSum();
            Assert.IsTrue(sum <= 3);
            Assert.IsTrue(sum >= 1);
            _cognitiveArchitecture.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 2;
            sum = _beliefsModel.FilterBeliefToSend(_belief.EntityId, 0, _emailTemplate).GetSum();
            Assert.IsTrue(sum <= 2);
            Assert.IsTrue(sum >= 1);
            _cognitiveArchitecture.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 1;
            sum = _beliefsModel.FilterBeliefToSend(_belief.EntityId, 0, _emailTemplate).GetSum();
            Assert.AreEqual(1, sum);
            _cognitiveArchitecture.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 0;
            _emailTemplate.MinimumNumberOfBitsOfBeliefToSend = 0;
            _emailTemplate.MaximumNumberOfBitsOfBeliefToSend = 0;
            Assert.IsNull(_beliefsModel.FilterBeliefToSend(_belief.EntityId, 0, _emailTemplate));
        }

        #endregion
    }
}