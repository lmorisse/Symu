#region Licence

// Description: Symu - SymuEngineTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Common;
using SymuEngine.Repository.Networks.Beliefs;

#endregion

namespace SymuEngineTests.Repository.Networks.Beliefs
{
    [TestClass]
    public class BeliefTests
    {
        private Belief _belief;

        [TestInitialize]
        public void Initialize()
        {
            _belief = new Belief(1, 1, RandomGenerator.RandomUniform);
        }

        /// <summary>
        ///     RandomBinary
        /// </summary>
        [TestMethod]
        public void InitializeWeightsTest()
        {
            _belief.InitializeWeights(RandomGenerator.RandomBinary, 1);
            float[] results = {-1, 0, 1};
            Assert.IsTrue(results.Contains(_belief.Weights.GetBit(0)));
        }

        /// <summary>
        ///     RandomUniform
        /// </summary>
        [TestMethod]
        public void InitializeWeightsTest1()
        {
            _belief.InitializeWeights(RandomGenerator.RandomUniform, 1);
            Assert.IsTrue(-1 <= _belief.Weights.GetBit(0) && _belief.Weights.GetBit(0) <= 1);
        }
    }
}