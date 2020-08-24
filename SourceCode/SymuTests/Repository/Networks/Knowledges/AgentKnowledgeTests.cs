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
using Symu.Repository.Networks.Knowledges;
using static Symu.Common.Constants;
#endregion

namespace SymuTests.Repository.Networks.Knowledges
{
    [TestClass]
    public class AgentKnowledgeTests
    {
        private readonly Knowledge _knowledge = new Knowledge(3, "k3", 10);
        private AgentKnowledge _agentKnowledge ;
        private readonly float[] _knowledge01Bits = {0, 1};
        private readonly float[] _knowledge0Bits = {0, 0};
        private readonly float[] _knowledge1Bits = {1, 1};
        private AgentKnowledge _agentKnowledge0;
        private AgentKnowledge _agentKnowledge01;
        private AgentKnowledge _agentKnowledge1;

        [TestInitialize]
        public void Initialize()
        {
            _agentKnowledge0 = new AgentKnowledge(0, _knowledge0Bits, 0, -1, 0);
            _agentKnowledge1 = new AgentKnowledge(1, _knowledge1Bits, 0, -1, 0);
            _agentKnowledge01 = new AgentKnowledge(2, _knowledge01Bits, 0, -1, 0);
            _agentKnowledge = new AgentKnowledge(_knowledge.Id, KnowledgeLevel.BasicKnowledge, 0, -1);
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

        /// <summary>
        ///     Non passing test knowledgeBits == null
        /// </summary>
        [TestMethod]
        public void GetKnowledgeBitTest()
        {
            Assert.AreEqual(0, _agentKnowledge.GetKnowledgeBit(0));
        }

        /// <summary>
        ///     Passing test
        /// </summary>
        [TestMethod]
        public void GetKnowledgeBitTest1()
        {
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

        #region Initialize AgentKnowledge
        /// <summary>
        ///     Random Binary Generator
        /// </summary>
        [DataRow(KnowledgeLevel.Expert)]
        [DataRow(KnowledgeLevel.Random)]
        [TestMethod]
        public void InitializeBitsTest(KnowledgeLevel level)
        {
            _agentKnowledge.InitializeBits(_knowledge.Length, RandomGenerator.RandomBinary, level, 0);
            var knowledgeBits = _agentKnowledge.KnowledgeBits;
            for (byte i = 0; i < 10; i++)
            {
                Assert.IsTrue(Math.Abs(knowledgeBits.GetBit(i)) < Tolerance ||
                              Math.Abs(knowledgeBits.GetBit(i) - 1) < Tolerance);
            }
        }

        /// <summary>
        ///     Random Binary Generator with full knowledge
        /// </summary>
        [TestMethod]
        public void InitializeBitsTest1()
        {
            _agentKnowledge.InitializeBits(_knowledge.Length, RandomGenerator.RandomBinary, KnowledgeLevel.FullKnowledge, 0);
            var knowledgeBits = _agentKnowledge.KnowledgeBits;
            byte no1 = 0;
            for (byte i = 0; i < 10; i++)
            {
                Assert.IsTrue(Math.Abs(knowledgeBits.GetBit(i) - 1) < Tolerance);
                if (Math.Abs(knowledgeBits.GetBit(i) - 1) < Tolerance)
                {
                    no1++;
                }
            }

            Assert.IsTrue(no1 == 10);
        }

        /// <summary>
        ///     Random Binary Generator with no knowledge
        /// </summary>
        [TestMethod]
        public void InitializeBitsTest2()
        {
            _agentKnowledge.InitializeBits(_knowledge.Length, RandomGenerator.RandomBinary, KnowledgeLevel.NoKnowledge, 0);
            var knowledgeBits = _agentKnowledge.KnowledgeBits;
            byte no1 = 0;
            for (byte i = 0; i < 10; i++)
            {
                Assert.IsTrue(Math.Abs(knowledgeBits.GetBit(i)) < Tolerance);
                if (Math.Abs(knowledgeBits.GetBit(i)) < Tolerance)
                {
                    no1++;
                }
            }

            Assert.IsTrue(no1 == 10);
        }

        /// <summary>
        ///     Random Uniform Generator
        /// </summary>
        [DataRow(KnowledgeLevel.Expert)]
        [DataRow(KnowledgeLevel.BasicKnowledge)]
        [DataRow(KnowledgeLevel.Foundational)]
        [DataRow(KnowledgeLevel.FullProficiency)]
        [DataRow(KnowledgeLevel.Intermediate)]
        [TestMethod]
        public void InitializeBitsTest3(KnowledgeLevel level)
        {
            _agentKnowledge.InitializeBits(_knowledge.Length, RandomGenerator.RandomUniform, level, 0);
            var knowledgeBits = _agentKnowledge.KnowledgeBits;
            for (byte i = 0; i < 10; i++)
            {
                Assert.IsTrue(
                    Knowledge.GetMinFromKnowledgeLevel(level) <= knowledgeBits.GetBit(i) ||
                    Math.Abs(knowledgeBits.GetBit(i)) < Tolerance);
                Assert.IsTrue(knowledgeBits.GetBit(i) <=
                              Knowledge.GetMaxFromKnowledgeLevel(level));
            }
        }

        /// <summary>
        ///     Random Uniform Generator with random knowledge
        /// </summary>
        [TestMethod]
        public void InitializeBitsTest5()
        {
            _agentKnowledge.InitializeBits(_knowledge.Length, RandomGenerator.RandomBinary, KnowledgeLevel.Random, 0);
            var knowledgeBits = _agentKnowledge.KnowledgeBits;
            for (byte i = 0; i < 10; i++)
            {
                Assert.IsTrue(0 <= knowledgeBits.GetBit(i) ||
                              Math.Abs(knowledgeBits.GetBit(i)) < Tolerance);
                Assert.IsTrue(knowledgeBits.GetBit(i) <= 1);
            }
        }
        #endregion
    }
}