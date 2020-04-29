#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Repository.Networks.Knowledge.Bits;

#endregion

namespace SymuEngineTests.Repository.Networks.Knowledge.Base
{
    [TestClass]
    public class BitsTests
    {
        private readonly float[] _floats0 = {0, 0};
        private readonly float[] _floats1 = {1, 1};
        private Bits _bits0;
        private Bits _bits1;

        [TestInitialize]
        public void Initialize()
        {
            _bits0 = new Bits(_floats0, 0);
            _bits1 = new Bits(_floats1, 0);
        }

        [TestMethod]
        public void CloneTest()
        {
            var clone = _bits1.Clone();
            Assert.AreEqual(_bits1.GetBit(0), clone.GetBit(0));
            Assert.AreEqual(_bits1.GetBit(1), clone.GetBit(1));
            // It's a real clone
            Assert.AreNotEqual(_bits1, clone);
            _bits1.SetBit(0, 0);
            Assert.AreNotEqual(_bits1.GetBit(0), clone.GetBit(0));
        }

        [TestMethod]
        public void GetBitTest()
        {
            Assert.AreEqual(0, _bits0.GetBit(0));
            Assert.AreEqual(0, _bits0.GetBit(1));
            Assert.AreEqual(1, _bits1.GetBit(0));
            Assert.AreEqual(1, _bits1.GetBit(1));
        }

        [TestMethod]
        public void GetSumTest()
        {
            Assert.AreEqual(0, _bits0.GetSum());
            Assert.AreEqual(2, _bits1.GetSum());
        }

        [TestMethod]
        public void SetBitTest()
        {
            _bits0.SetBits(_floats1);

            Assert.AreEqual(1, _bits0.GetBit(0));
            Assert.AreEqual(1, _bits0.GetBit(1));
        }

        [TestMethod]
        public void ForgetTest()
        {
            _bits1.Forget(0, 1, 0);
            Assert.AreEqual(0, _bits1.GetBit(0));
            _bits1.Forget(1, 0, 0);
            Assert.AreEqual(1, _bits1.GetBit(1));
            _bits1.Forget(1, 0, 1);
            Assert.AreEqual(1, _bits1.GetBit(1));
        }
    }
}