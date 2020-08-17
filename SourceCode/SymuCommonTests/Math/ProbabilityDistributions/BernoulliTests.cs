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
    public class BernoulliTests
    {
        [TestMethod]
        public void FailBernoulliTest()
        {
            Assert.ThrowsException<ArgumentException>(() => Bernoulli.Sample(-0.1F));
            Assert.ThrowsException<ArgumentException>(() => Bernoulli.Sample(1.1F));
        }

        [TestMethod]
        public void SampleTest()
        {
            Assert.IsTrue(Bernoulli.Sample(1));
            Assert.IsFalse(Bernoulli.Sample(0));
        }
    }
}