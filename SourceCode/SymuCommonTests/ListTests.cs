#region Licence

// Description: SymuBiz - SymuToolsTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Common;

#endregion

namespace SymuToolsTests
{
    [TestClass]
    public class ListTests
    {
        [TestMethod]
        public void AverageTest()
        {
            List<byte> bytes = null;
            Assert.ThrowsException<ArgumentNullException>(() => bytes.Average());

            bytes = new List<byte>();
            Assert.AreEqual(0, bytes.Average());
            bytes.Add(1);
            Assert.AreEqual(1, bytes.Average());
            bytes.Add(2);
            Assert.AreEqual(2, bytes.Average());
            bytes.Add(3);
            Assert.AreEqual(2, bytes.Average());
        }

        [TestMethod]
        public void ShuffleTest()
        {
            var items = new List<TestItem>();
            items.Shuffle();
            Assert.AreEqual(0, items.Count);
            items.Add(new TestItem("0"));
            items.Shuffle();
            Assert.AreEqual(1, items.Count);
            for (var i = 1; i < 100; i++)
            {
                items.Add(new TestItem(i.ToString()));
            }

            items.Shuffle();
            Assert.AreEqual(100, items.Count);
            Assert.AreNotEqual("0", items[0].Type);
            Assert.AreNotEqual("99", items[99].Type);
        }

        #region Nested type: TestItem

        private class TestItem
        {
            public readonly string Type;

            public TestItem(string type)
            {
                Type = type;
            }
        }

        #endregion
    }
}