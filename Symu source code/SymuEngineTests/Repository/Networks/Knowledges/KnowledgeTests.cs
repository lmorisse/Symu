#region Licence

// Description: Symu - SymuEngineTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Task;
using SymuEngine.Common;
using SymuEngine.Repository.Networks.Knowledges;
using static SymuTools.Constants;

#endregion

namespace SymuEngineTests.Repository.Networks.Knowledges
{
    [TestClass]
    public class KnowledgeTests
    {
        private readonly Knowledge _knowledge =
            new Knowledge(1, "1", 10);

        private readonly MurphyTask _taskModel = new MurphyTask();

        private RandomGenerator _knowledgeModel;

        /// <summary>
        ///     Random Binary Generator
        /// </summary>
        [TestMethod]
        public void GetBitsTest()
        {
            _knowledgeModel = RandomGenerator.RandomBinary;
            var knowledgeBits = _knowledge.InitializeBits(_knowledgeModel, KnowledgeLevel.Expert);
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
        public void GetBitsTest1()
        {
            _knowledgeModel = RandomGenerator.RandomBinary;
            var knowledgeBits = _knowledge.InitializeBits(_knowledgeModel, KnowledgeLevel.FullKnowledge);
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
        public void GetBitsTest2()
        {
            _knowledgeModel = RandomGenerator.RandomBinary;
            var knowledgeBits = _knowledge.InitializeBits(_knowledgeModel, KnowledgeLevel.NoKnowledge);
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
        [TestMethod]
        public void GetBitsTest3()
        {
            _knowledgeModel = RandomGenerator.RandomUniform;
            var knowledgeBits = _knowledge.InitializeBits(_knowledgeModel, KnowledgeLevel.Expert);
            for (byte i = 0; i < 10; i++)
            {
                Assert.IsTrue(
                    Knowledge.GetMinFromKnowledgeLevel(
                        KnowledgeLevel.Expert) <= knowledgeBits[i] ||
                    Math.Abs(knowledgeBits[i]) < Tolerance);
                Assert.IsTrue(knowledgeBits[i] <=
                              Knowledge.GetMaxFromKnowledgeLevel(
                                  KnowledgeLevel.Expert));
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