#region Licence

// Description: Symu - SymuEngineTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Blockers;

#endregion

namespace SymuEngineTests.Classes.Blockers
{
    [TestClass]
    public class BlockerTests
    {
        [TestMethod]
        public void BlockerTest()
        {
            var blocker = new Blocker(1, 2);
            Assert.AreEqual(1, blocker.Type);
            Assert.AreEqual(2, blocker.InitialStep);
            Assert.AreEqual(2, blocker.LastRecoverStep);
        }

        [TestMethod]
        public void BlockerTest1()
        {
            var blocker = new Blocker(1, 2, 3);
            Assert.AreEqual(3, (int) blocker.Parameter);
        }

        [TestMethod]
        public void BlockerTest2()
        {
            var blocker = new Blocker(1, 2, 3, 4);
            Assert.AreEqual(4, (int) blocker.Parameter2);
        }

        [TestMethod]
        public void EqualsTest()
        {
            var blocker = new Blocker(1, 2);
            Assert.IsTrue(blocker.Equals(1));
            Assert.IsFalse(blocker.Equals(2));
        }

        [TestMethod]
        public void EqualsTest1()
        {
            var blocker = new Blocker(1, 2);
            Assert.IsTrue(blocker.Equals((ushort) 2));
            Assert.IsFalse(blocker.Equals((ushort) 1));
        }

        [TestMethod]
        public void EqualsTest2()
        {
            var blocker = new Blocker(1, 2);
            Assert.IsTrue(blocker.Equals(1, 2));
            Assert.IsFalse(blocker.Equals(1, 1));
            Assert.IsFalse(blocker.Equals(2, 2));
        }

        [TestMethod]
        public void UpdateTest()
        {
            var blocker = new Blocker(0, 0);
            blocker.Update(1);
            Assert.AreEqual(1, blocker.LastRecoverStep);
            Assert.AreEqual(0, blocker.InitialStep);
        }
    }
}