#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Task;
using Symu.Common;
using Symu.Repository.Networks.Knowledges;
using static Symu.Common.Constants;

#endregion

namespace SymuTests.Repository.Networks.Knowledges
{
    [TestClass]
    public class KnowledgeTests
    {
        private readonly Knowledge _knowledge =
            new Knowledge(1, "1", 10);

        private readonly MurphyTask _taskModel = new MurphyTask();


        /// <summary>
        ///     Random Binary Generator
        /// </summary>
        [DataRow(KnowledgeLevel.Expert)]
        [DataRow(KnowledgeLevel.Random)]
        [TestMethod]
        public void InitializeBitsTest(KnowledgeLevel level)
        {
            var knowledgeBits = _knowledge.InitializeBits(RandomGenerator.RandomBinary, level);
            for (byte i = 0; i < 10; i++)
            {
                Assert.IsTrue(Math.Abs(knowledgeBits[i]) < Tolerance ||
                              Math.Abs(knowledgeBits[i] - 1) < Tolerance);
            }
        }

        /// <summary>
        ///     Random Binary Generator with full knowledge
        /// </summary>
        [TestMethod]
        public void InitializeBitsTest1()
        {
            var knowledgeBits = _knowledge.InitializeBits(RandomGenerator.RandomBinary, KnowledgeLevel.FullKnowledge);
            byte no1 = 0;
            for (byte i = 0; i < 10; i++)
            {
                Assert.IsTrue(Math.Abs(knowledgeBits[i] - 1) < Tolerance);
                if (Math.Abs(knowledgeBits[i] - 1) < Tolerance)
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
            var knowledgeBits = _knowledge.InitializeBits(RandomGenerator.RandomBinary, KnowledgeLevel.NoKnowledge);
            byte no1 = 0;
            for (byte i = 0; i < 10; i++)
            {
                Assert.IsTrue(Math.Abs(knowledgeBits[i]) < Tolerance);
                if (Math.Abs(knowledgeBits[i]) < Tolerance)
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
            var knowledgeBits = _knowledge.InitializeBits(RandomGenerator.RandomUniform, level);
            for (byte i = 0; i < 10; i++)
            {
                Assert.IsTrue(
                    Knowledge.GetMinFromKnowledgeLevel(level) <= knowledgeBits[i] ||
                    Math.Abs(knowledgeBits[i]) < Tolerance);
                Assert.IsTrue(knowledgeBits[i] <=
                              Knowledge.GetMaxFromKnowledgeLevel(level));
            }
        }

        /// <summary>
        ///     Random Uniform Generator with random knowledge
        /// </summary>
        [TestMethod]
        public void InitializeBitsTest5()
        {
            var knowledgeBits = _knowledge.InitializeBits(RandomGenerator.RandomUniform, KnowledgeLevel.Random);
            for (byte i = 0; i < 10; i++)
            {
                Assert.IsTrue(0 <= knowledgeBits[i] ||
                              Math.Abs(knowledgeBits[i]) < Tolerance);
                Assert.IsTrue(knowledgeBits[i] <= 1);
            }
        }

        [TestMethod]
        public void GetTaskRequiredBitsTest()
        {
            var taskRequiredBits = _knowledge.GetTaskRequiredBits(_taskModel, 0.8F);
            var numberRequiredBits = Convert.ToByte(Math.Round(_taskModel.RequiredBitsRatio(0.8F) * _knowledge.Length));
            Assert.AreEqual(numberRequiredBits, taskRequiredBits.Length);
            for (byte i = 0; i < taskRequiredBits.Length; i++)
                //It's an index of a Array[knowledge.Size]
            {
                Assert.IsTrue(taskRequiredBits[i] < _knowledge.Length);
            }
        }

        [TestMethod]
        public void GetTaskMandatoryBitsTest()
        {
            var taskMandatoryBits = _knowledge.GetTaskMandatoryBits(_taskModel, 0.8F);
            for (byte i = 0; i < taskMandatoryBits.Length; i++)
                //It's an index of a Array[knowledge.Size]
            {
                Assert.IsTrue(taskMandatoryBits[i] < _knowledge.Length);
            }
        }

        /// <summary>
        ///     MandatoryRatio = 0
        /// </summary>
        [TestMethod]
        public void GetTaskMandatoryBitsTest1()
        {
            _taskModel.MandatoryRatio = 0;
            var taskMandatoryBits = _knowledge.GetTaskMandatoryBits(_taskModel, 0.8F);
            Assert.AreEqual(0, taskMandatoryBits.Length);
        }
    }
}