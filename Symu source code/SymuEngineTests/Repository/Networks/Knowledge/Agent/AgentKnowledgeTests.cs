#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Repository.Networks.Knowledge.Agent;
using SymuEngine.Repository.Networks.Knowledge.Bits;

#endregion

namespace SymuEngineTests.Repository.Networks.Knowledge.Agent
{
    [TestClass]
    public class AgentKnowledgeTests
    {
        private const float Threshold = 0.1F;
        private readonly AgentKnowledge _agentKnowledge = new AgentKnowledge(4, KnowledgeLevel.BasicKnowledge);
        private readonly float[] _knowledge01Bits = {0, 1};
        private readonly float[] _knowledge0Bits = {0, 0};
        private readonly float[] _knowledge1Bits = {1, 1};
        private readonly float[] _knowledgeFloatBits = {0.1F, 0.1F};
        private readonly byte[] _taskKnowledge0 = {0};
        private readonly byte[] _taskKnowledge1 = {1};
        private AgentKnowledge _agentKnowledge0;
        private AgentKnowledge _agentKnowledge01;
        private AgentKnowledge _agentKnowledge1;
        private AgentKnowledge _agentKnowledgeFloat;

        [TestInitialize]
        public void Initialize()
        {
            _agentKnowledge0 = new AgentKnowledge(0, _knowledge0Bits, 0);
            _agentKnowledge1 = new AgentKnowledge(1, _knowledge1Bits, 0);
            _agentKnowledge01 = new AgentKnowledge(2, _knowledge01Bits, 0);
            _agentKnowledgeFloat = new AgentKnowledge(3, _knowledgeFloatBits, 0);
        }

        [TestMethod]
        public void CheckTest()
        {
            Assert.IsFalse(_agentKnowledge0.Check(_taskKnowledge0, out var index, Threshold, 0));
            Assert.AreEqual(0, index);
            Assert.IsFalse(_agentKnowledge0.Check(_taskKnowledge1, out index, Threshold, 0));
            Assert.AreEqual(1, index);
            Assert.IsTrue(_agentKnowledge1.Check(_taskKnowledge0, out _, Threshold, 0));
            Assert.IsTrue(_agentKnowledge1.Check(_taskKnowledge1, out _, Threshold, 0));
            Assert.IsFalse(_agentKnowledge01.Check(_taskKnowledge0, out index, Threshold, 0));
            Assert.AreEqual(0, index);
            Assert.IsTrue(_agentKnowledge01.Check(_taskKnowledge1, out _, Threshold, 0));
            Assert.IsTrue(_agentKnowledgeFloat.Check(_taskKnowledge0, out _, Threshold, 0));
            Assert.IsTrue(_agentKnowledgeFloat.Check(_taskKnowledge1, out _, Threshold, 0));
        }

        [TestMethod]
        public void KnowsEnoughTest()
        {
            // Non passing test
            Assert.IsFalse(_agentKnowledge.KnowsEnough(0, Threshold, 0));
            // Passing tests
            Assert.IsTrue(_agentKnowledge0.KnowsEnough(0, 0, 0));
            Assert.IsFalse(_agentKnowledge0.KnowsEnough(0, Threshold, 0));
            Assert.IsFalse(_agentKnowledge0.KnowsEnough(1, Threshold, 0));
            Assert.IsTrue(_agentKnowledge1.KnowsEnough(0, Threshold, 0));
            Assert.IsTrue(_agentKnowledge1.KnowsEnough(1, Threshold, 0));
            Assert.IsFalse(_agentKnowledge01.KnowsEnough(0, Threshold, 0));
            Assert.IsTrue(_agentKnowledge01.KnowsEnough(1, Threshold, 0));
            Assert.IsTrue(_agentKnowledgeFloat.KnowsEnough(0, Threshold, 0));
            Assert.IsTrue(_agentKnowledgeFloat.KnowsEnough(1, Threshold, 0));
        }

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void LearnTest()
        {
            Assert.ThrowsException<IndexOutOfRangeException>(() => _agentKnowledge.Learn(0, 1, 0));
        }

        /// <summary>
        ///     Passing test
        /// </summary>
        [TestMethod]
        public void LearnTest1()
        {
            _agentKnowledge0.Learn(0, 1, 0);
            Assert.AreEqual(1, _agentKnowledge0.GetKnowledgeBit(0));
            _agentKnowledge0.Learn(0, 2, 0);
            Assert.AreEqual(1, _agentKnowledge0.GetKnowledgeBit(0));
        }

        /// <summary>
        ///     Minimum Knowledge level = 0
        /// </summary>
        [TestMethod]
        public void ForgetTest()
        {
            _agentKnowledge1.Forget(0, 1, 0);
            Assert.AreEqual(0, _agentKnowledge1.GetKnowledgeBit(0));
            _agentKnowledge1.Forget(0, 2, 0);
            Assert.AreEqual(0, _agentKnowledge1.GetKnowledgeBit(0));
        }

        /// <summary>
        ///     Minimum Knowledge level = 1
        /// </summary>
        [TestMethod]
        public void ForgetTest1()
        {
            _agentKnowledge1.Forget(0, 1, 1);
            Assert.AreEqual(1, _agentKnowledge1.GetKnowledgeBit(0));
        }

        [TestMethod]
        public void GetKnowledgeSumTest()
        {
            // Non passing test knowledgeBits == null
            Assert.AreEqual(0, _agentKnowledge.GetKnowledgeSum());
            // Passing tests
            Assert.AreEqual(0, _agentKnowledge0.GetKnowledgeSum());
            Assert.AreEqual(1, _agentKnowledge01.GetKnowledgeSum());
            Assert.AreEqual(2, _agentKnowledge1.GetKnowledgeSum());
        }

        [TestMethod]
        public void SizeTest()
        {
            // Non passing test knowledgeBits == null
            Assert.AreEqual(0, _agentKnowledge.Length);
            // Passing tests
            Assert.AreEqual(2, _agentKnowledge0.Length);
        }

        [TestMethod]
        public void GetKnowledgeBitsTest()
        {
            Assert.AreEqual(_knowledge0Bits[0], _agentKnowledge0.GetKnowledgeBit(0));
            Assert.AreEqual(_knowledge1Bits[0], _agentKnowledge1.GetKnowledgeBit(0));
            Assert.AreEqual(_knowledge01Bits[0], _agentKnowledge01.GetKnowledgeBit(0));
        }

        [TestMethod]
        public void GetKnowledgeBitTest()
        {
            // Non passing test knowledgeBits == null
            Assert.AreEqual(-1, _agentKnowledge.GetKnowledgeBit(0));
            // Passing tests
            for (byte i = 0; i < 2; i++)
            {
                Assert.AreEqual(_knowledge0Bits[i], _agentKnowledge0.GetKnowledgeBit(i));
                Assert.AreEqual(_knowledge1Bits[i], _agentKnowledge1.GetKnowledgeBit(i));
                Assert.AreEqual(_knowledge01Bits[i], _agentKnowledge01.GetKnowledgeBit(i));
            }
        }

        [TestMethod]
        public void SetKnowledgeBitsTest()
        {
            _agentKnowledge.SetKnowledgeBits(_knowledge1Bits, 0);
            for (byte i = 0; i < 2; i++)
            {
                Assert.AreEqual(_knowledge1Bits[i], _agentKnowledge.GetKnowledgeBit(i));
            }
        }

        [TestMethod]
        public void SetKnowledgeBitTest()
        {
            for (byte i = 0; i < 2; i++)
            {
                _agentKnowledge0.SetKnowledgeBit(i, _knowledge1Bits[i], 0);
                Assert.AreEqual(_knowledge1Bits[i], _agentKnowledge0.GetKnowledgeBit(i));
            }
        }

        [TestMethod]
        public void GetKnowledgeBitsTest1()
        {
            var knowledgeBits = _agentKnowledge1.CloneWrittenKnowledgeBits(1.1F);
            Assert.AreEqual(0, knowledgeBits.GetBit(0));
            Assert.AreEqual(0, knowledgeBits.GetBit(1));
            knowledgeBits = _agentKnowledge1.CloneWrittenKnowledgeBits(0);
            Assert.AreEqual(1, knowledgeBits.GetBit(0));
            Assert.AreEqual(1, knowledgeBits.GetBit(1));
        }

        [TestMethod]
        public void CloneTest()
        {
            var clone = _agentKnowledge1.CloneBits();
            Assert.IsNotNull(clone);
            Assert.AreNotEqual(_agentKnowledge1.KnowledgeBits, clone);
            Assert.AreEqual(_agentKnowledge1.KnowledgeBits.GetBit(0), clone.GetBit(0));
            Assert.AreEqual(_agentKnowledge1.KnowledgeBits.GetBit(1), clone.GetBit(1));
        }
    }
}