#region Licence

// Description: Symu - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Blockers;

#endregion

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