#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Common.Classes;
using Symu.Common.Interfaces;
using Symu.Repository.Entities;
using SymuTests.Helpers;
using ActorBelief = Symu.Repository.Edges.ActorBelief;

#endregion


namespace SymuTests.Repository.Edges
{
    [TestClass]
    public class ActorBeliefTests :BaseTestClass
    {
        private const RandomGenerator Model = new RandomGenerator();
        private ActorBelief _actorBelief0 ;
        private ActorBelief _actorBelief1 ;
        private ActorBelief _actorBelief2 ;
        private Belief _belief0 ;
        private Belief _belief1 ;
        private Belief _belief2 ;
        private readonly float[] _bits0 = { };
        private readonly float[] _bits1 = { 0 };
        private readonly float[] _bits2 = { 0, 0 };

        [TestInitialize]
        public void Initialize()
        {
            var agentId = new AgentId(1, 1);
            _belief0 = new Belief(Network, 0, Model, BeliefWeightLevel.RandomWeight);
            _belief1 = new Belief(Network, 1, Model, BeliefWeightLevel.RandomWeight);
            _belief2 = new Belief(Network, 2, Model, BeliefWeightLevel.RandomWeight);
            _actorBelief0 = new ActorBelief(agentId, _belief0.EntityId, BeliefLevel.NeitherAgreeNorDisagree);
            _actorBelief1 = new ActorBelief(agentId, _belief1.EntityId, BeliefLevel.NeitherAgreeNorDisagree);
            _actorBelief2 = new ActorBelief(agentId, _belief2.EntityId, BeliefLevel.NeitherAgreeNorDisagree);
        }

        /// <summary>
        ///     Non passing tests
        /// </summary>
        [TestMethod]
        public void NullCheckTest()
        {
            byte[] taskKnowledge = { };
            Assert.ThrowsException<ArgumentNullException>(() => _actorBelief1.Check(null, out _, _belief1, 1, true));
            Assert.ThrowsException<ArgumentNullException>(
                () => _actorBelief1.Check(taskKnowledge, out _, null, 1, true));
        }

        /// <summary>
        ///     actorBelief not initialized
        /// </summary>
        [TestMethod]
        public void NotInitializedCheckTest()
        {
            byte[] taskKnowledge = { };
            Assert.AreEqual(0, _actorBelief1.Check(taskKnowledge, out _, _belief1, 1, true));
        }

        [TestMethod]
        public void ZeroLengthCheckTest()
        {
            byte[] taskKnowledge = { };
            _actorBelief0.SetBeliefBits(_bits0);
            Assert.AreEqual(0, _actorBelief0.Check(taskKnowledge, out _, _belief0, 1, true));
        }

        /// <summary>
        ///     Passing test - zero filled
        /// </summary>
        [TestMethod]
        public void CheckTest()
        {
            byte[] taskKnowledge = {0};
            _actorBelief1.SetBeliefBits(_bits1);
            var t = _actorBelief1.Check(taskKnowledge, out _, _belief1, 1, true);
            Assert.AreEqual(0, t);
        }

        /// <summary>
        ///     Passing test - nonzero filled
        /// </summary>
        [TestMethod]
        public void CheckTest1()
        {
            byte[] taskIndexes = {0};
            _belief1.Weights.SetBit(0, 1);
            _actorBelief1.SetBeliefBits(_bits1);
            _actorBelief1.BeliefBits.SetBit(0, 1);
            var t = _actorBelief1.Check(taskIndexes, out var index, _belief1, 0, true);
            Assert.AreEqual(1, t);
            Assert.AreEqual(0, index);
        }

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void NullLearnTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _actorBelief1.Learn(null, 0));
        }

        /// <summary>
        ///     Passing tests - no learning
        ///     Neutral belief
        /// </summary>
        [TestMethod]
        public void NoLearningLearnTest()
        {
            var bits = new Bits(new float[] {0}, -1);
            _actorBelief1.SetBeliefBits(_bits1);
            _actorBelief1.BeliefBits.SetBit(0, 1);
            _actorBelief1.Learn(bits, 1);
            Assert.AreEqual(1, _actorBelief1.BeliefBits.GetBit(0));
        }

        /// <summary>
        ///     Passing tests - no learning
        /// </summary>
        [TestMethod]
        public void NoLearningLearnTest1()
        {
            var bits = new Bits(new float[] {-1}, -1);
            _actorBelief1.SetBeliefBits(_bits1);
            _actorBelief1.BeliefBits.SetBit(0, 1);
            // Non influence
            _actorBelief1.Learn(bits, 0);
            Assert.AreEqual(1, _actorBelief1.BeliefBits.GetBit(0));
        }

        /// <summary>
        ///     Passing tests - learning
        /// </summary>
        [TestMethod]
        public void LearnTest()
        {
            var bits = new Bits(new float[] {-1}, -1);
            _actorBelief1.SetBeliefBits(_bits1);
            _actorBelief1.BeliefBits.SetBit(0, 1);
            _actorBelief1.Learn(bits, 1);
            Assert.AreEqual(0, _actorBelief1.BeliefBits.GetBit(0));
        }

        /// <summary>
        ///     Passing tests - reinforcement
        /// </summary>
        [TestMethod]
        public void LearnTest1()
        {
            var bits = new Bits(new float[] {1}, -1);
            _actorBelief1.SetBeliefBits(_bits1);
            _actorBelief1.BeliefBits.SetBit(0, 1);
            _actorBelief1.Learn(bits, 1);
            Assert.AreEqual(1, _actorBelief1.BeliefBits.GetBit(0));
        }

        /// <summary>
        ///     Non passing test
        ///     Non initialized actorBelief
        /// </summary>
        [TestMethod]
        public void NullCloneWrittenBeliefBitsTest()
        {
            Assert.ThrowsException<NullReferenceException>(() => _actorBelief1.CloneWrittenBeliefBits(1));
        }

        [TestMethod]
        public void CloneWrittenBeliefBitsTest()
        {
            _actorBelief1.SetBeliefBits(_bits1);
            // 1
            _actorBelief1.BeliefBits.SetBit(0, 1);
            var bits = _actorBelief1.CloneWrittenBeliefBits(0);
            Assert.AreEqual(1, bits.GetBit(0));
            bits = _actorBelief1.CloneWrittenBeliefBits(2);
            Assert.AreEqual(0, bits.GetBit(0));
            // -1
            _actorBelief1.BeliefBits.SetBit(0, -1);
            bits = _actorBelief1.CloneWrittenBeliefBits(0);
            Assert.AreEqual(-1, bits.GetBit(0));
            bits = _actorBelief1.CloneWrittenBeliefBits(2);
            Assert.AreEqual(0, bits.GetBit(0));
        }

        [TestMethod]
        public void BelievesEnoughTest()
        {
            _actorBelief1.SetBeliefBits(_bits1);
            _actorBelief1.BeliefBits.SetBit(0, -1);
            Assert.IsTrue(_actorBelief1.BelievesEnough(0, 0));
            _actorBelief1.BeliefBits.SetBit(0, 1);
            Assert.IsTrue(_actorBelief1.BelievesEnough(0, 0));
            _actorBelief1.BeliefBits.SetBit(0, 0);
            Assert.IsFalse(_actorBelief1.BelievesEnough(0, 1));
        }

        [TestMethod]
        public void NotInitializedGetBeliefSumTest()
        {
            Assert.AreEqual(0, _actorBelief1.GetBeliefSum());
        }

        /// <summary>
        ///     One belief bit
        /// </summary>
        [TestMethod]
        public void GetBeliefSumTest()
        {
            _actorBelief1.SetBeliefBits(_bits1);
            _actorBelief1.BeliefBits.SetBit(0, -1);
            Assert.AreEqual(-1, _actorBelief1.GetBeliefSum());
            _actorBelief1.BeliefBits.SetBit(0, 1);
            Assert.AreEqual(1, _actorBelief1.GetBeliefSum());
            _actorBelief1.BeliefBits.SetBit(0, 0);
            Assert.AreEqual(0, _actorBelief1.GetBeliefSum());
        }

        /// <summary>
        ///     two beliefs bit
        /// </summary>
        [TestMethod]
        public void GetBeliefSumTest1()
        {
            _actorBelief2.SetBeliefBits(_bits2);
            _actorBelief2.BeliefBits.SetBit(0, -1);
            _actorBelief2.BeliefBits.SetBit(1, -1);
            Assert.AreEqual(-2, _actorBelief2.GetBeliefSum());
            _actorBelief2.BeliefBits.SetBit(0, 1);
            _actorBelief2.BeliefBits.SetBit(1, -1);
            Assert.AreEqual(0, _actorBelief2.GetBeliefSum());
        }

        /// <summary>
        ///     Random uniform
        /// </summary>
        [TestMethod]
        public void LearnTest2()
        {
            _actorBelief1.SetBeliefBits(_bits1);
            Assert.AreEqual(0, _actorBelief1.BeliefBits.GetBit(0));
            _actorBelief1.Learn(RandomGenerator.RandomUniform, 0);
            Assert.AreNotEqual(0, _actorBelief1.BeliefBits.GetBit(0));
            Assert.IsTrue(-1 <= _actorBelief1.BeliefBits.GetBit(0));
            Assert.IsTrue(_actorBelief1.BeliefBits.GetBit(0) <= 1);
        }

        [TestMethod]
        public void SetBeliefBitsTest()
        {
            var bits = new float[] {1, 2};
            _actorBelief1.SetBeliefBits(bits);
            for (byte i = 0; i < 2; i++)
            {
                Assert.AreEqual(bits[i], _actorBelief1.BeliefBits.GetBit(i));
            }
        }
    }
}