#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Agent.Models.CognitiveArchitecture.Knowledge;
using SymuEngine.Common;
using SymuEngine.Repository.Networks.Belief.Agent;
using SymuEngine.Repository.Networks.Knowledge.Bits;

#endregion

namespace SymuEngineTests.Repository.Networks.Belief.Agent
{
    [TestClass]
    public class AgentBeliefTests
    {
        private readonly AgentBelief _agentBelief = new AgentBelief(1);
        private readonly KnowledgeModel _model = new KnowledgeModel();

        /// <summary>
        ///     Initialize
        /// </summary>
        [TestMethod]
        public void InitializeBeliefBitsTest()
        {
            Assert.IsNull(_agentBelief.BeliefBits);
            _agentBelief.InitializeBeliefBits(_model, 1, true);
            Assert.IsNotNull(_agentBelief.BeliefBits);
            Assert.AreEqual(1, _agentBelief.BeliefBits.Length);
        }

        /// <summary>
        ///     Neutral Initialize
        /// </summary>
        [TestMethod]
        public void InitializeBeliefBitsTest1()
        {
            _agentBelief.InitializeBeliefBits(_model, 1, true);
            Assert.AreEqual(0, _agentBelief.BeliefBits.GetBit(0));
        }

        /// <summary>
        ///     Initialize RandomUniform
        /// </summary>
        [TestMethod]
        public void InitializeBeliefBitsTest2()
        {
            _model.RandomGenerator = RandomGenerator.RandomUniform;
            _agentBelief.InitializeBeliefBits(_model, 1, false);
            var t = _agentBelief.BeliefBits.GetBit(0);
            Assert.IsTrue(-1 <= t && t <= 1);
        }

        /// <summary>
        ///     Initialize RandomBinary
        /// </summary>
        [TestMethod]
        public void InitializeBeliefBitsTest3()
        {
            _model.RandomGenerator = RandomGenerator.RandomBinary;
            _agentBelief.InitializeBeliefBits(_model, 1, false);
            var t = Convert.ToInt32(_agentBelief.BeliefBits.GetBit(0));
            Assert.IsTrue(-1 == t || t == 1 || t == 0);
        }

        /// <summary>
        ///     Non passing tests
        /// </summary>
        [TestMethod]
        public void NullCheckTest()
        {
            byte[] taskKnowledge = { };
            var belief = new SymuEngine.Repository.Networks.Belief.Repository.Belief(1, 1, _model);
            Assert.ThrowsException<ArgumentNullException>(() => _agentBelief.Check(null, out _, belief, 1));
            Assert.ThrowsException<ArgumentNullException>(() => _agentBelief.Check(taskKnowledge, out _, null, 1));
            // agentBelief not initialized
            Assert.ThrowsException<ArgumentNullException>(() => _agentBelief.Check(taskKnowledge, out _, belief, 1));
        }

        [TestMethod]
        public void ZeroLengthCheckTest()
        {
            byte[] taskKnowledge = { };
            var belief = new SymuEngine.Repository.Networks.Belief.Repository.Belief(1, 1, _model);
            _agentBelief.InitializeBeliefBits(_model, 0, true);
            Assert.AreEqual(0, _agentBelief.Check(taskKnowledge, out _, belief, 1));
        }

        /// <summary>
        ///     Passing test - zero filled
        /// </summary>
        [TestMethod]
        public void CheckTest()
        {
            byte[] taskKnowledge = {0};
            var belief = new SymuEngine.Repository.Networks.Belief.Repository.Belief(1, 1, _model);
            _agentBelief.InitializeBeliefBits(_model, 1, true);
            var t = _agentBelief.Check(taskKnowledge, out _, belief, 1);
            Assert.AreEqual(0, t);
        }

        /// <summary>
        ///     Passing test - nonzero filled
        /// </summary>
        [TestMethod]
        public void CheckTest1()
        {
            byte[] taskIndexes = {0};
            var belief = new SymuEngine.Repository.Networks.Belief.Repository.Belief(1, 1, _model);
            belief.Weights.SetBit(0, 1);
            _agentBelief.InitializeBeliefBits(_model, 1, true);
            _agentBelief.BeliefBits.SetBit(0, 1);
            var t = _agentBelief.Check(taskIndexes, out var index, belief, 0);
            Assert.AreEqual(1, t);
            Assert.AreEqual(0, index);
        }

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void NullLearnTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _agentBelief.Learn(null, 0));
        }

        /// <summary>
        ///     Passing tests - no learning
        ///     Neutral belief
        /// </summary>
        [TestMethod]
        public void NoLearningLearnTest()
        {
            var bits = new Bits(new float[] {0}, -1);
            _agentBelief.InitializeBeliefBits(_model, 1, true);
            _agentBelief.BeliefBits.SetBit(0, 1);
            _agentBelief.Learn(bits, 1);
            Assert.AreEqual(1, _agentBelief.BeliefBits.GetBit(0));
        }

        /// <summary>
        ///     Passing tests - no learning
        /// </summary>
        [TestMethod]
        public void NoLearningLearnTest1()
        {
            var bits = new Bits(new float[] {-1}, -1);
            _agentBelief.InitializeBeliefBits(_model, 1, true);
            _agentBelief.BeliefBits.SetBit(0, 1);
            // Non influence
            _agentBelief.Learn(bits, 0);
            Assert.AreEqual(1, _agentBelief.BeliefBits.GetBit(0));
        }

        /// <summary>
        ///     Passing tests - learning
        /// </summary>
        [TestMethod]
        public void LearnTest()
        {
            var bits = new Bits(new float[] {-1}, -1);
            _agentBelief.InitializeBeliefBits(_model, 1, true);
            _agentBelief.BeliefBits.SetBit(0, 1);
            _agentBelief.Learn(bits, 1);
            Assert.AreEqual(0, _agentBelief.BeliefBits.GetBit(0));
        }

        /// <summary>
        ///     Passing tests - reinforcement
        /// </summary>
        [TestMethod]
        public void LearnTest1()
        {
            var bits = new Bits(new float[] {1}, -1);
            _agentBelief.InitializeBeliefBits(_model, 1, true);
            _agentBelief.BeliefBits.SetBit(0, 1);
            _agentBelief.Learn(bits, 1);
            Assert.AreEqual(1, _agentBelief.BeliefBits.GetBit(0));
        }

        /// <summary>
        ///     Non passing test
        ///     Non initialized agentBelief
        /// </summary>
        [TestMethod]
        public void NullCloneWrittenBeliefBitsTest()
        {
            Assert.ThrowsException<NullReferenceException>(() => _agentBelief.CloneWrittenBeliefBits(1));
        }

        [TestMethod]
        public void CloneWrittenBeliefBitsTest()
        {
            _agentBelief.InitializeBeliefBits(_model, 1, true);
            // 1
            _agentBelief.BeliefBits.SetBit(0, 1);
            var bits = _agentBelief.CloneWrittenBeliefBits(0);
            Assert.AreEqual(1, bits.GetBit(0));
            bits = _agentBelief.CloneWrittenBeliefBits(2);
            Assert.AreEqual(0, bits.GetBit(0));
            // -1
            _agentBelief.BeliefBits.SetBit(0, -1);
            bits = _agentBelief.CloneWrittenBeliefBits(0);
            Assert.AreEqual(-1, bits.GetBit(0));
            bits = _agentBelief.CloneWrittenBeliefBits(2);
            Assert.AreEqual(0, bits.GetBit(0));
        }

        [TestMethod]
        public void BelievesEnoughTest()
        {
            _agentBelief.InitializeBeliefBits(_model, 1, true);
            _agentBelief.BeliefBits.SetBit(0, -1);
            Assert.IsTrue(_agentBelief.BelievesEnough(0, 0));
            _agentBelief.BeliefBits.SetBit(0, 1);
            Assert.IsTrue(_agentBelief.BelievesEnough(0, 0));
            _agentBelief.BeliefBits.SetBit(0, 0);
            Assert.IsFalse(_agentBelief.BelievesEnough(0, 1));
        }

        [TestMethod]
        public void NullGetBeliefSumTest()
        {
            Assert.ThrowsException<NullReferenceException>(() => _agentBelief.GetBeliefSum());
        }

        /// <summary>
        ///     One belief bit
        /// </summary>
        [TestMethod]
        public void GetBeliefSumTest()
        {
            _agentBelief.InitializeBeliefBits(_model, 1, true);
            _agentBelief.BeliefBits.SetBit(0, -1);
            Assert.AreEqual(-1, _agentBelief.GetBeliefSum());
            _agentBelief.BeliefBits.SetBit(0, 1);
            Assert.AreEqual(1, _agentBelief.GetBeliefSum());
            _agentBelief.BeliefBits.SetBit(0, 0);
            Assert.AreEqual(0, _agentBelief.GetBeliefSum());
        }

        /// <summary>
        ///     two beliefs bit
        /// </summary>
        [TestMethod]
        public void GetBeliefSumTest1()
        {
            _agentBelief.InitializeBeliefBits(_model, 2, true);
            _agentBelief.BeliefBits.SetBit(0, -1);
            _agentBelief.BeliefBits.SetBit(1, -1);
            Assert.AreEqual(-2, _agentBelief.GetBeliefSum());
            _agentBelief.BeliefBits.SetBit(0, 1);
            _agentBelief.BeliefBits.SetBit(1, -1);
            Assert.AreEqual(0, _agentBelief.GetBeliefSum());
        }

        [TestMethod]
        public void LearnTest2()
        {
            _model.RandomGenerator = RandomGenerator.RandomUniform;
            _agentBelief.InitializeBeliefBits(_model, 1, true);
            Assert.AreEqual(0, _agentBelief.BeliefBits.GetBit(0));
            _agentBelief.Learn(_model, 0);
            Assert.AreNotEqual(0, _agentBelief.BeliefBits.GetBit(0));
            Assert.IsTrue(-1 <= _agentBelief.BeliefBits.GetBit(0));
            Assert.IsTrue(_agentBelief.BeliefBits.GetBit(0) <= 1);
        }
    }
}