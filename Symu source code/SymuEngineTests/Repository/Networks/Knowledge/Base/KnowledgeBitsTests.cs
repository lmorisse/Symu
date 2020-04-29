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
    public class KnowledgeBitsTests
    {
        private readonly float[] _floats0 = {0, 0};
        private readonly float[] _floats1 = {1, 1};
        private readonly KnowledgeBits _knowledgeBits = new KnowledgeBits(0);


        [TestMethod]
        public void SetBitsTest1()
        {
            _knowledgeBits.SetBits(_floats0, 1);
            Assert.AreEqual(2, _knowledgeBits.GetLastTouched().Length);
            Assert.AreEqual(1, _knowledgeBits.GetLastTouched()[0]);
            Assert.AreEqual(1, _knowledgeBits.GetLastTouched()[1]);
        }

        [TestMethod]
        public void InitializeWith0Test()
        {
            _knowledgeBits.InitializeWith0(1, 1);
            Assert.AreEqual(1, _knowledgeBits.GetLastTouched().Length);
            Assert.AreEqual(0, _knowledgeBits.GetBit(0));
            Assert.AreEqual(1, _knowledgeBits.GetLastTouched()[0]);
        }

        [TestMethod]
        public void SetBitTest1()
        {
            _knowledgeBits.InitializeWith0(1, 1);
            _knowledgeBits.SetBit(0, 1, 2);
            Assert.AreEqual(1, _knowledgeBits.GetBit(0));
            Assert.AreEqual(2, _knowledgeBits.GetLastTouched()[0]);
        }

        [TestMethod]
        public void GetBitTest1()
        {
            _knowledgeBits.InitializeWith0(1, 1);
            _knowledgeBits.GetBit(0, 2);
            Assert.AreEqual(2, _knowledgeBits.GetLastTouched()[0]);
        }

        [TestMethod]
        public void ForgetTest()
        {
            _knowledgeBits.SetBits(_floats1, 1);
            _knowledgeBits.Forget(0, 1, 0);
            Assert.AreEqual(0, _knowledgeBits.GetBit(0));
            Assert.AreEqual(1, _knowledgeBits.GetBit(1));
        }
    }
}