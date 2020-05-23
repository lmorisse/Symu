using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Blockers;
using Symu.Classes.Organization;
using SymuTests.Helpers;

namespace SymuTests.Classes.Blockers
{
    [TestClass]
    public class BlockerCollectionTests
    {
        private readonly BlockerCollection _blockers = new BlockerCollection();
        
        [TestMethod]
        public void AddBlockerTest()
        {
            Assert.IsNotNull(_blockers.Add(0, 1, 0));
            Assert.IsTrue(_blockers.IsBlocked);
            Assert.AreEqual(1, _blockers.Result.InProgress);
        }
    }
}