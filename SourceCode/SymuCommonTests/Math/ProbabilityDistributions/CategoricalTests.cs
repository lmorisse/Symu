#region Licence

// Description: SymuBiz - SymuToolsTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Common.Math.ProbabilityDistributions;

#endregion

namespace SymuToolsTests.Math.ProbabilityDistributions
{
    [TestClass]
    public class CategoricalTests
    {
        [TestMethod]
        public void NonValidProbabilityMassTest()
        {
            // negative 
            Assert.ThrowsException<ArgumentException>(() => Categorical.SampleIndex(new[] {0.5, -0.5, 0.2}));
            // 0 sum
            Assert.ThrowsException<ArgumentException>(() => Categorical.SampleIndex(new double[] {0, 0, 0}));
        }

        /// <summary>
        ///     Force Sample to index 0
        /// </summary>
        [TestMethod]
        public void SampleIndexTest()
        {
            Assert.AreEqual(0, Categorical.SampleIndex(new double[] {1, 0, 0}));
        }

        /// <summary>
        ///     Force Sample to index 1
        /// </summary>
        [TestMethod]
        public void SampleIndexTest1()
        {
            Assert.AreEqual(1, Categorical.SampleIndex(0, 1, 0));
        }

        /// <summary>
        ///     Force Sample to not be equal to index 0
        /// </summary>
        [TestMethod]
        public void SampleIndexTest2()
        {
            Assert.AreNotEqual(0, Categorical.SampleIndex(0, 0.3F, 0.3F, 0.4F));
        }

        /// <summary>
        ///     Force Sample to index 0
        /// </summary>
        [TestMethod]
        public void StaticSampleTest()
        {
            var sample = Categorical.SampleValue(new double[] {1, 0, 0});
            Assert.AreEqual(1, sample);
        }

        /// <summary>
        ///     Force Sample to index 0
        ///     Float
        /// </summary>
        [TestMethod]
        public void StaticSampleTest2()
        {
            var sample = Categorical.SampleValue(new float[] {1, 0, 0});
            Assert.AreEqual(1, sample);
        }
    }
}