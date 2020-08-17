#region Licence

// Description: SymuBiz - SymuToolsTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Common;
using Symu.Common.Math.ProbabilityDistributions;

#endregion

namespace SymuToolsTests.Math.ProbabilityDistributions
{
    [TestClass]
    public class DiscreteUniformTests
    {
        /// <summary>
        ///     Upper and lower bounds
        /// </summary>
        [TestMethod]
        public void SampleTest()
        {
            Assert.AreEqual(0, DiscreteUniform.Sample(0, 0));
            Assert.AreEqual(1, DiscreteUniform.Sample(1, 1));

            var t = DiscreteUniform.Sample(0, 1);
            Assert.IsTrue(t == 0 || t == 1);
            t = DiscreteUniform.Sample(-1, 1);
            Assert.IsTrue(t == 0 || t == 1 || t == -1);
        }

        /// <summary>
        ///     upper bound
        /// </summary>
        [TestMethod]
        public void SampleTest1()
        {
            Assert.AreEqual(0, DiscreteUniform.Sample(0));
            var t = DiscreteUniform.Sample(1);
            Assert.IsTrue(t == 0 || t == 1);
        }

        [TestMethod]
        public void SamplesTest()
        {
            var t = DiscreteUniform.Samples(10, -1, 1);
            Assert.AreEqual(10, t.Length);
            for (var i = 0; i < 10; i++)
            {
                Assert.IsTrue(System.Math.Abs(t[i]) < Constants.Tolerance ||
                              System.Math.Abs(t[i] - 1) < Constants.Tolerance ||
                              System.Math.Abs(t[i] + 1) < Constants.Tolerance);
            }
        }

        [TestMethod]
        public void SampleByMeanToByteTest()
        {
            // test that there is no exception
            Assert.IsTrue(DiscreteUniform.SampleByMeanToByte(1, 2) >= 0);
        }
    }
}