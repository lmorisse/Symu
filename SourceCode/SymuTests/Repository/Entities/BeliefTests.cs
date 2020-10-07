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
using Symu.Common.Classes;
using Symu.Repository.Entities;
using SymuTests.Helpers;
using static Symu.Common.Constants;

#endregion

namespace SymuTests.Repository.Entities
{
    [TestClass]
    public class BeliefTests : BaseTestClass
    {
        private Belief _belief;
        private Knowledge _knowledge;

        [TestInitialize]
        public void Initialize()
        {
            MainOrganization.Models.Generator = RandomGenerator.RandomUniform;
            MainOrganization.Models.BeliefWeightLevel = BeliefWeightLevel.RandomWeight;
            MainOrganization.Models.Beliefs.On = true;
            _knowledge = new Knowledge(Network, MainOrganization.Models, "1", 10);
            _belief = _knowledge.AssociatedBelief;
        }

        [TestMethod]
        public void BeliefTest()
        {
            Assert.IsTrue(Network.Belief.Contains(_belief));
            Assert.AreEqual(_knowledge.EntityId, _belief.KnowledgeId);
        }

        [TestMethod]
        public void CloneTest()
        {
            _belief.InitializeWeights(RandomGenerator.RandomBinary, 1, BeliefWeightLevel.RandomWeight);
            var clone = (Belief) _belief.Clone();
            Assert.AreEqual(_belief.Length, clone.Length);
            Assert.AreEqual(_belief.Weights, clone.Weights);
            Assert.AreEqual(_belief.KnowledgeId, clone.KnowledgeId);
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
            for (byte i = 0; i < _knowledge.Length; i++)
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
            for (byte i = 0; i < _knowledge.Length; i++)
            {
                Assert.IsTrue(Math.Abs(knowledgeBits[i]) < Tolerance);
                if (Math.Abs(knowledgeBits[i]) < Tolerance)
                {
                    no1++;
                }
            }

            Assert.IsTrue(no1 == _knowledge.Length);
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
            for (byte i = 0; i < _knowledge.Length; i++)
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
            for (byte i = 0; i < _knowledge.Length; i++)
            {
                Assert.IsTrue(-1 <= bits[i] ||
                    Math.Abs(bits[i]) < Tolerance);
                Assert.IsTrue(bits[i] <= 1);
            }
        }
    }
}