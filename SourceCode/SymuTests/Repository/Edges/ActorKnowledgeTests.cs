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
using Symu.DNA.Entities;
using Symu.DNA.GraphNetworks;
using Symu.Repository.Edges;
using Symu.Repository.Entities;
using SymuTests.Helpers;
using static Symu.Common.Constants;
#endregion

namespace SymuTests.Repository.Edges
{
    [TestClass]
    public class ActorKnowledgeTests : BaseTestClass
    {
        private Knowledge _knowledge;
        private Knowledge _knowledge1;
        private Knowledge _knowledge2;
        private ActorKnowledge _actorKnowledge ;
        private readonly float[] _knowledge01Bits = {0, 1};
        private readonly float[] _knowledge0Bits = {0, 0};
        private readonly float[] _knowledge1Bits = {1, 1};
        private ActorKnowledge _actorKnowledge0;
        private ActorKnowledge _actorKnowledge01;
        private ActorKnowledge _actorKnowledge1;

        [TestInitialize]
        public void Initialize()
        {
            var agentId = new AgentId(1, 1);
            _knowledge = new Knowledge(Network, Organization.Models, "0", 10);
            _knowledge1 = new Knowledge(Network, Organization.Models, "1", 10);
            _knowledge2 = new Knowledge(Network, Organization.Models, "2", 10);
            _actorKnowledge0 = new ActorKnowledge(agentId, _knowledge.EntityId, _knowledge0Bits, 0, -1, 0);
            _actorKnowledge1 = new ActorKnowledge(agentId, _knowledge1.EntityId, _knowledge1Bits, 0, -1, 0);
            _actorKnowledge01 = new ActorKnowledge(agentId, _knowledge2.EntityId, _knowledge01Bits, 0, -1, 0);
            _actorKnowledge = new ActorKnowledge(agentId, _knowledge.EntityId, KnowledgeLevel.BasicKnowledge, 0, -1);
        }

        [TestMethod]
        public void GetKnowledgeSumTest()
        {
            // Non passing test knowledgeBits == null
            Assert.AreEqual(0, _actorKnowledge.GetKnowledgeSum());
            // Passing tests
            Assert.AreEqual(0, _actorKnowledge0.GetKnowledgeSum());
            Assert.AreEqual(1, _actorKnowledge01.GetKnowledgeSum());
            Assert.AreEqual(2, _actorKnowledge1.GetKnowledgeSum());
        }

        [TestMethod]
        public void SizeTest()
        {
            // Non passing test knowledgeBits == null
            Assert.AreEqual(0, _actorKnowledge.Length);
            // Passing tests
            Assert.AreEqual(2, _actorKnowledge0.Length);
        }

        [TestMethod]
        public void GetKnowledgeBitsTest()
        {
            Assert.AreEqual(_knowledge0Bits[0], _actorKnowledge0.GetKnowledgeBit(0));
            Assert.AreEqual(_knowledge1Bits[0], _actorKnowledge1.GetKnowledgeBit(0));
            Assert.AreEqual(_knowledge01Bits[0], _actorKnowledge01.GetKnowledgeBit(0));
        }

        /// <summary>
        ///     Non passing test knowledgeBits == null
        /// </summary>
        [TestMethod]
        public void GetKnowledgeBitTest()
        {
            Assert.AreEqual(0, _actorKnowledge.GetKnowledgeBit(0));
        }

        /// <summary>
        ///     Passing test
        /// </summary>
        [TestMethod]
        public void GetKnowledgeBitTest1()
        {
            for (byte i = 0; i < 2; i++)
            {
                Assert.AreEqual(_knowledge0Bits[i], _actorKnowledge0.GetKnowledgeBit(i));
                Assert.AreEqual(_knowledge1Bits[i], _actorKnowledge1.GetKnowledgeBit(i));
                Assert.AreEqual(_knowledge01Bits[i], _actorKnowledge01.GetKnowledgeBit(i));
            }
        }

        [TestMethod]
        public void SetKnowledgeBitsTest()
        {
            _actorKnowledge.SetKnowledgeBits(_knowledge1Bits, 0);
            for (byte i = 0; i < 2; i++)
            {
                Assert.AreEqual(_knowledge1Bits[i], _actorKnowledge.GetKnowledgeBit(i));
            }
        }

        [TestMethod]
        public void SetKnowledgeBitTest()
        {
            for (byte i = 0; i < 2; i++)
            {
                _actorKnowledge0.SetKnowledgeBit(i, _knowledge1Bits[i], 0);
                Assert.AreEqual(_knowledge1Bits[i], _actorKnowledge0.GetKnowledgeBit(i));
            }
        }

        [TestMethod]
        public void GetKnowledgeBitsTest1()
        {
            var knowledgeBits = _actorKnowledge1.CloneWrittenKnowledgeBits(1.1F);
            Assert.AreEqual(0, knowledgeBits.GetBit(0));
            Assert.AreEqual(0, knowledgeBits.GetBit(1));
            knowledgeBits = _actorKnowledge1.CloneWrittenKnowledgeBits(0);
            Assert.AreEqual(1, knowledgeBits.GetBit(0));
            Assert.AreEqual(1, knowledgeBits.GetBit(1));
        }

        [TestMethod]
        public void CloneBitsTest()
        {
            var clone = _actorKnowledge1.CloneBits();
            Assert.IsNotNull(clone);
            Assert.AreEqual(_actorKnowledge1.KnowledgeBits, clone);
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
            _actorKnowledge.InitializeKnowledge(_knowledge.Length, RandomGenerator.RandomBinary, level, 0);
            var knowledgeBits = _actorKnowledge.KnowledgeBits;
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
            _actorKnowledge.InitializeKnowledge(_knowledge.Length, RandomGenerator.RandomBinary, KnowledgeLevel.FullKnowledge, 0);
            var knowledgeBits = _actorKnowledge.KnowledgeBits;
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
            _actorKnowledge.InitializeKnowledge(_knowledge.Length, RandomGenerator.RandomBinary, KnowledgeLevel.NoKnowledge, 0);
            var knowledgeBits = _actorKnowledge.KnowledgeBits;
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
            _actorKnowledge.InitializeKnowledge(_knowledge.Length, RandomGenerator.RandomUniform, level, 0);
            var knowledgeBits = _actorKnowledge.KnowledgeBits;
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
            _actorKnowledge.InitializeKnowledge(_knowledge.Length, RandomGenerator.RandomBinary, KnowledgeLevel.Random, 0);
            var knowledgeBits = _actorKnowledge.KnowledgeBits;
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