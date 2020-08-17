#region Licence

// Description: SymuBiz - SymuToolsTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Common.Math.ProbabilityDistributions;

#endregion

namespace SymuToolsTests.Math.ProbabilityDistributions
{
    [TestClass]
    public class ContinuousUniformTests
    {
        [TestMethod]
        public void SampleTest()
        {
            Assert.AreEqual(0, ContinuousUniform.Sample(0, 0));
            var t = DiscreteUniform.Sample(0, 1);
            Assert.IsTrue(t >= 0 && t <= 1);
        }

        [TestMethod]
        public void SamplesTest()
        {
            var t = ContinuousUniform.Samples(10, 0, 1);
            Assert.AreEqual(10, t.Length);
            for (var i = 0; i < 10; i++)
            {
                Assert.IsTrue(t[i] >= 0 && t[i] <= 1);
            }
        }

        /// <summary>
        ///     Value below 1 are converted in 0
        /// </summary>
        [TestMethod]
        public void BinaryArrayTest()
        {
            var binaryArray = ContinuousUniform.FilteredSamples(1, 1);
            Assert.AreEqual(0, binaryArray[0]);
        }

        /// <summary>
        ///     Value above 0 are converted in 1
        /// </summary>
        [TestMethod]
        public void BinaryArrayTest1()
        {
            var binaryArray = ContinuousUniform.FilteredSamples(1, 0);
            Assert.AreEqual(1, binaryArray[0]);
        }

        /// <summary>
        ///     Value below 1 are converted in 0
        /// </summary>
        [TestMethod]
        public void ValueBinaryArrayTest()
        {
            var binaryArray = ContinuousUniform.FilteredSamples(1, 1, 5);
            Assert.AreEqual(0, binaryArray[0]);
        }

        /// <summary>
        ///     Value above 0 are converted in 1
        /// </summary>
        [TestMethod]
        public void ValueBinaryArrayTest1()
        {
            var binaryArray = ContinuousUniform.FilteredSamples(1, 0, 5);
            Assert.AreEqual(5, binaryArray[0]);
        }
    }
}