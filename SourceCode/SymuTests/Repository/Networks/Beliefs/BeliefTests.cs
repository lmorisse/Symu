#region Licence
// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License
#endregion

#region using directives

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Common;
using Symu.Repository.Entity;
using Symu.Repository.Networks.Beliefs;
using static Symu.Common.Constants;

#endregion

namespace SymuTests.Repository.Networks.Beliefs
{
    [TestClass]
    public class BeliefTests
    {
        private Belief _belief;

        [TestInitialize]
        public void Initialize()
        {
            _belief = new Belief(1, "1", 10, RandomGenerator.RandomUniform, BeliefWeightLevel.RandomWeight);
        }

        /// <summary>
        ///     RandomBinary
        /// </summary>
        [TestMethod]
        public void InitializeWeightsTest()
        {
            _belief.InitializeWeights(RandomGenerator.RandomBinary, 1, BeliefWeightLevel.RandomWeight);
            float[] results = {-1, 0, 1};
            Assert.IsTrue(results.Contains(_belief.Weights.GetBit(0)));
        }

        /// <summary>
        ///     RandomUniform
        /// </summary>
        [TestMethod]
        public void InitializeWeightsTest1()
        {
            _belief.InitializeWeights(RandomGenerator.RandomUniform, 1, BeliefWeightLevel.RandomWeight);
            Assert.IsTrue(-1 <= _belief.Weights.GetBit(0) && _belief.Weights.GetBit(0) <= 1);
        }

        /// <summary>
        ///     Random Binary Generator
        /// </summary>
        [DataRow(BeliefLevel.StronglyAgree)]
        [DataRow(BeliefLevel.Random)]
        [TestMethod]
        public void InitializeBitsTest(BeliefLevel level)
        {
            var knowledgeBits = _belief.InitializeBits(RandomGenerator.RandomBinary, level);
            for (byte i = 0; i < 10; i++)
            {
                Assert.IsTrue(Math.Abs(knowledgeBits[i]) < Tolerance ||
                              Math.Abs(knowledgeBits[i] - 1) < Tolerance);
            }
        }

        /// <summary>
        ///     Random Binary Generator with no belief
        /// </summary>
        [TestMethod]
        public void InitializeBitsTest2()
        {
            var knowledgeBits = _belief.InitializeBits(RandomGenerator.RandomBinary, BeliefLevel.NoBelief);
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
        /// 

        [DataRow(BeliefLevel.StronglyAgree)]
        [DataRow(BeliefLevel.StronglyDisagree)]
        [DataRow(BeliefLevel.Agree)]
        [DataRow(BeliefLevel.Disagree)]
        [DataRow(BeliefLevel.NeitherAgreeNorDisagree)]
        [TestMethod]
        public void InitializeBitsTest3(BeliefLevel level)
        {
            var knowledgeBits = _belief.InitializeBits(RandomGenerator.RandomUniform, level);
            for (byte i = 0; i < 10; i++)
            {
                Assert.IsTrue(
                    Belief.GetMinFromBeliefLevel(level) <= knowledgeBits[i] ||
                    Math.Abs(knowledgeBits[i]) < Tolerance);
                Assert.IsTrue(knowledgeBits[i] <=
                              Belief.GetMaxFromBeliefLevel(level));
            }
        }

        /// <summary>
        ///     Random Uniform Generator with random knowledge
        /// </summary>
        [TestMethod]
        public void InitializeBitsTest5()
        {
            var bits = _belief.InitializeBits(RandomGenerator.RandomUniform, BeliefLevel.Random);
            for (byte i = 0; i < 10; i++)
            {
                Assert.IsTrue(-1 <= bits[i] ||
                    Math.Abs(bits[i]) < Tolerance);
                Assert.IsTrue(bits[i] <= 1);
            }
        }
    }
}