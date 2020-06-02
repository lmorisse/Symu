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
        private readonly Blocker _blocker1 = new Blocker(1, 1);
        private readonly Blocker _blocker2 = new Blocker(1, 2);

        [TestMethod]
        public void AddBlockerTest()
        {
            Assert.IsNotNull(_blockers.Add(_blocker1));
            Assert.IsTrue(_blockers.IsBlocked);
        }

        [TestMethod]
        public void RemoveTest()
        {
            _blockers.Add(_blocker2);
            _blockers.Remove(2);
            Assert.IsFalse(_blockers.Exists(1, 2));
        }

        [TestMethod]
        public void RemoveTest1()
        {
            var blocker = _blockers.Add(_blocker1);
            _blockers.Remove(blocker);
            Assert.IsFalse(_blockers.Exists(1, 1));
        }

        [TestMethod]
        public void GetBlockersTest()
        {
            Assert.AreEqual(0, _blockers.FilterBlockers(1).Count);
            _blockers.Add(_blocker1);
            Assert.AreEqual(0, _blockers.FilterBlockers(1).Count);
            _blockers.Add(_blocker2);
            Assert.AreEqual(1, _blockers.FilterBlockers(1).Count);
        }

        [TestMethod]
        public void GetBlockerTest()
        {
            Assert.IsNull(_blockers.GetBlocker(1, 1));
            _blockers.Add(_blocker1);
            Assert.IsNotNull(_blockers.GetBlocker(1, 1));
        }

        [TestMethod]
        public void NotBlockedTodayTest()
        {
            Assert.IsTrue(_blockers.NotBlockedToday(1));
            _blockers.Add(_blocker1);
            Assert.IsFalse(_blockers.NotBlockedToday(1));
        }

        [TestMethod]
        public void ClearTest()
        {
            Assert.AreEqual(0, _blockers.List.Count);
            _blockers.Add(_blocker1);
            _blockers.Clear();
            Assert.AreEqual(0, _blockers.List.Count);
        }
    }
}