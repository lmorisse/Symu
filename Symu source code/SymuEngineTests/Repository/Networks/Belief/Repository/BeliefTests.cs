#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Agent.Models.CognitiveArchitecture.Knowledge;
using SymuEngine.Common;

#endregion

namespace SymuEngineTests.Repository.Networks.Belief.Repository
{
    [TestClass]
    public class BeliefTests
    {
        private readonly KnowledgeModel _model = new KnowledgeModel();
        private SymuEngine.Repository.Networks.Belief.Repository.Belief _belief;

        [TestInitialize]
        public void Initialize()
        {
            _belief = new SymuEngine.Repository.Networks.Belief.Repository.Belief(1, 1, _model);
        }

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void NullInitializeWeightsTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _belief.InitializeWeights(null, 1));
        }

        /// <summary>
        ///     RandomBinary
        /// </summary>
        [TestMethod]
        public void InitializeWeightsTest()
        {
            _model.RandomGenerator = RandomGenerator.RandomBinary;
            _belief.InitializeWeights(_model, 1);
            float[] results = {-1, 0, 1};
            Assert.IsTrue(results.Contains(_belief.Weights.GetBit(0)));
        }

        /// <summary>
        ///     RandomUniform
        /// </summary>
        [TestMethod]
        public void InitializeWeightsTest1()
        {
            _model.RandomGenerator = RandomGenerator.RandomUniform;
            _belief.InitializeWeights(_model, 1);
            Assert.IsTrue(-1 <= _belief.Weights.GetBit(0) && _belief.Weights.GetBit(0) <= 1);
        }
    }
}