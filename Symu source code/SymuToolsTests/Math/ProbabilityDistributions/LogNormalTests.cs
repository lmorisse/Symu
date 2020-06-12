using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Tools.Math.ProbabilityDistributions;

namespace SymuToolsTests.Math.ProbabilityDistributions
{
    [TestClass]
    public class LogNormalTests
    {
        [TestMethod]
        public void SampleTest()
        {
            Assert.AreEqual(0, LogNormal.Sample(0, 0));
            Assert.AreEqual(1, LogNormal.Sample(1, 0));
        }
    }
}