#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Common;
using Symu.Repository.Entity;

#endregion


namespace SymuTests.Repository.Entity
{
    [TestClass]
    public class AgentBeliefTests
    {
        private const RandomGenerator Model = new RandomGenerator();
        private AgentBelief _agentBelief0 ;
        private AgentBelief _agentBelief1 ;
        private AgentBelief _agentBelief2 ;
        private readonly Belief _belief0 = new Belief(0, "0", 0, Model, BeliefWeightLevel.RandomWeight);
        private readonly Belief _belief1 = new Belief(1, "1", 1, Model, BeliefWeightLevel.RandomWeight);
        private readonly Belief _belief2 = new Belief(2, "2", 2, Model, BeliefWeightLevel.RandomWeight);
        private readonly float[] _bits0 = { };
        private readonly float[] _bits1 = { 0 };
        private readonly float[] _bits2 = { 0, 0 };

        [TestInitialize]
        public void Initialize()
        {
            _agentBelief0 = new AgentBelief(_belief0.Id, BeliefLevel.NeitherAgreeNorDisagree);
            _agentBelief1 = new AgentBelief(_belief1.Id, BeliefLevel.NeitherAgreeNorDisagree);
            _agentBelief2 = new AgentBelief(_belief2.Id, BeliefLevel.NeitherAgreeNorDisagree);
        }

        /// <summary>
        ///     Non passing tests
        /// </summary>
        [TestMethod]
        public void NullCheckTest()
        {
            byte[] taskKnowledge = { };
            Assert.ThrowsException<ArgumentNullException>(() => _agentBelief1.Check(null, out _, _belief1, 1, true));
            Assert.ThrowsException<ArgumentNullException>(
                () => _agentBelief1.Check(taskKnowledge, out _, null, 1, true));
        }

        /// <summary>
        ///     agentBelief not initialized
        /// </summary>
        [TestMethod]
        public void NotInitializedCheckTest()
        {
            byte[] taskKnowledge = { };
            Assert.AreEqual(0, _agentBelief1.Check(taskKnowledge, out _, _belief1, 1, true));
        }

        [TestMethod]
        public void ZeroLengthCheckTest()
        {
            byte[] taskKnowledge = { };
            _agentBelief0.SetBeliefBits(_bits0);
            Assert.AreEqual(0, _agentBelief0.Check(taskKnowledge, out _, _belief0, 1, true));
        }

        /// <summary>
        ///     Passing test - zero filled
        /// </summary>
        [TestMethod]
        public void CheckTest()
        {
            byte[] taskKnowledge = {0};
            _agentBelief1.SetBeliefBits(_bits1);
            var t = _agentBelief1.Check(taskKnowledge, out _, _belief1, 1, true);
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
            _agentBelief1.SetBeliefBits(_bits1);
            _agentBelief1.BeliefBits.SetBit(0, 1);
            var t = _agentBelief1.Check(taskIndexes, out var index, _belief1, 0, true);
            Assert.AreEqual(1, t);
            Assert.AreEqual(0, index);
        }

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void NullLearnTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _agentBelief1.Learn(null, 0));
        }

        /// <summary>
        ///     Passing tests - no learning
        ///     Neutral belief
        /// </summary>
        [TestMethod]
        public void NoLearningLearnTest()
        {
            var bits = new Bits(new float[] {0}, -1);
            _agentBelief1.SetBeliefBits(_bits1);
            _agentBelief1.BeliefBits.SetBit(0, 1);
            _agentBelief1.Learn(bits, 1);
            Assert.AreEqual(1, _agentBelief1.BeliefBits.GetBit(0));
        }

        /// <summary>
        ///     Passing tests - no learning
        /// </summary>
        [TestMethod]
        public void NoLearningLearnTest1()
        {
            var bits = new Bits(new float[] {-1}, -1);
            _agentBelief1.SetBeliefBits(_bits1);
            _agentBelief1.BeliefBits.SetBit(0, 1);
            // Non influence
            _agentBelief1.Learn(bits, 0);
            Assert.AreEqual(1, _agentBelief1.BeliefBits.GetBit(0));
        }

        /// <summary>
        ///     Passing tests - learning
        /// </summary>
        [TestMethod]
        public void LearnTest()
        {
            var bits = new Bits(new float[] {-1}, -1);
            _agentBelief1.SetBeliefBits(_bits1);
            _agentBelief1.BeliefBits.SetBit(0, 1);
            _agentBelief1.Learn(bits, 1);
            Assert.AreEqual(0, _agentBelief1.BeliefBits.GetBit(0));
        }

        /// <summary>
        ///     Passing tests - reinforcement
        /// </summary>
        [TestMethod]
        public void LearnTest1()
        {
            var bits = new Bits(new float[] {1}, -1);
            _agentBelief1.SetBeliefBits(_bits1);
            _agentBelief1.BeliefBits.SetBit(0, 1);
            _agentBelief1.Learn(bits, 1);
            Assert.AreEqual(1, _agentBelief1.BeliefBits.GetBit(0));
        }

        /// <summary>
        ///     Non passing test
        ///     Non initialized agentBelief
        /// </summary>
        [TestMethod]
        public void NullCloneWrittenBeliefBitsTest()
        {
            Assert.ThrowsException<NullReferenceException>(() => _agentBelief1.CloneWrittenBeliefBits(1));
        }

        [TestMethod]
        public void CloneWrittenBeliefBitsTest()
        {
            _agentBelief1.SetBeliefBits(_bits1);
            // 1
            _agentBelief1.BeliefBits.SetBit(0, 1);
            var bits = _agentBelief1.CloneWrittenBeliefBits(0);
            Assert.AreEqual(1, bits.GetBit(0));
            bits = _agentBelief1.CloneWrittenBeliefBits(2);
            Assert.AreEqual(0, bits.GetBit(0));
            // -1
            _agentBelief1.BeliefBits.SetBit(0, -1);
            bits = _agentBelief1.CloneWrittenBeliefBits(0);
            Assert.AreEqual(-1, bits.GetBit(0));
            bits = _agentBelief1.CloneWrittenBeliefBits(2);
            Assert.AreEqual(0, bits.GetBit(0));
        }

        [TestMethod]
        public void BelievesEnoughTest()
        {
            _agentBelief1.SetBeliefBits(_bits1);
            _agentBelief1.BeliefBits.SetBit(0, -1);
            Assert.IsTrue(_agentBelief1.BelievesEnough(0, 0));
            _agentBelief1.BeliefBits.SetBit(0, 1);
            Assert.IsTrue(_agentBelief1.BelievesEnough(0, 0));
            _agentBelief1.BeliefBits.SetBit(0, 0);
            Assert.IsFalse(_agentBelief1.BelievesEnough(0, 1));
        }

        [TestMethod]
        public void NotInitializedGetBeliefSumTest()
        {
            Assert.AreEqual(0, _agentBelief1.GetBeliefSum());
        }

        /// <summary>
        ///     One belief bit
        /// </summary>
        [TestMethod]
        public void GetBeliefSumTest()
        {
            _agentBelief1.SetBeliefBits(_bits1);
            _agentBelief1.BeliefBits.SetBit(0, -1);
            Assert.AreEqual(-1, _agentBelief1.GetBeliefSum());
            _agentBelief1.BeliefBits.SetBit(0, 1);
            Assert.AreEqual(1, _agentBelief1.GetBeliefSum());
            _agentBelief1.BeliefBits.SetBit(0, 0);
            Assert.AreEqual(0, _agentBelief1.GetBeliefSum());
        }

        /// <summary>
        ///     two beliefs bit
        /// </summary>
        [TestMethod]
        public void GetBeliefSumTest1()
        {
            _agentBelief2.SetBeliefBits(_bits2);
            _agentBelief2.BeliefBits.SetBit(0, -1);
            _agentBelief2.BeliefBits.SetBit(1, -1);
            Assert.AreEqual(-2, _agentBelief2.GetBeliefSum());
            _agentBelief2.BeliefBits.SetBit(0, 1);
            _agentBelief2.BeliefBits.SetBit(1, -1);
            Assert.AreEqual(0, _agentBelief2.GetBeliefSum());
        }

        /// <summary>
        ///     Random uniform
        /// </summary>
        [TestMethod]
        public void LearnTest2()
        {
            _agentBelief1.SetBeliefBits(_bits1);
            Assert.AreEqual(0, _agentBelief1.BeliefBits.GetBit(0));
            _agentBelief1.Learn(RandomGenerator.RandomUniform, 0);
            Assert.AreNotEqual(0, _agentBelief1.BeliefBits.GetBit(0));
            Assert.IsTrue(-1 <= _agentBelief1.BeliefBits.GetBit(0));
            Assert.IsTrue(_agentBelief1.BeliefBits.GetBit(0) <= 1);
        }

        [TestMethod]
        public void SetBeliefBitsTest()
        {
            var bits = new float[] {1, 2};
            _agentBelief1.SetBeliefBits(bits);
            for (byte i = 0; i < 2; i++)
            {
                Assert.AreEqual(bits[i], _agentBelief1.BeliefBits.GetBit(i));
            }
        }
    }
}