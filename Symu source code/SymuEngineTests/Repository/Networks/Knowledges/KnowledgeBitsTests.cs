#region Licence

// Description: Symu - SymuEngineTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Repository.Networks.Knowledges;

#endregion


namespace SymuEngineTests.Repository.Networks.Knowledges
{
    [TestClass]
    public class KnowledgeBitsTests
    {
        private readonly float[] _floats0 = {0, 0};
        private readonly float[] _floats1 = {1, 1};
        private readonly KnowledgeBits _knowledgeBits = new KnowledgeBits(0, -1);


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

        /// <summary>
        ///     LastTouched reset
        /// </summary>
        [TestMethod]
        public void SetBitTest2()
        {
            _knowledgeBits.InitializeWith0(1, 1);
            _knowledgeBits.SetBit(0, 0, 2);
            Assert.AreEqual(0, _knowledgeBits.GetBit(0));
            Assert.AreEqual(0, _knowledgeBits.GetLastTouched()[0]);
        }

        [TestMethod]
        public void GetBitTest1()
        {
            _knowledgeBits.InitializeWith0(1, 1);
            _knowledgeBits.GetBit(0, 2);
            Assert.AreEqual(2, _knowledgeBits.GetLastTouched()[0]);
        }

        /// <summary>
        ///     TimeToLive == -1
        /// </summary>
        [TestMethod]
        public void ForgetTest()
        {
            _knowledgeBits.SetBits(_floats1, 0);
            Assert.AreEqual(0, _knowledgeBits.ForgetOldest(0, 1));
            Assert.AreEqual(1, _knowledgeBits.GetBit(0));
            Assert.AreEqual(1, _knowledgeBits.GetBit(1));
        }

        /// <summary>
        ///     TimeToLive == 0
        /// </summary>
        [TestMethod]
        public void ForgetTest1()
        {
            _knowledgeBits.TimeToLive = 0;
            _knowledgeBits.SetBits(_floats1, 0);
            Assert.AreEqual(-2, _knowledgeBits.ForgetOldest(1, 1));
            Assert.AreEqual(0, _knowledgeBits.GetBit(0));
            Assert.AreEqual(0, _knowledgeBits.GetBit(1));
        }

        /// <summary>
        ///     Passing test
        /// </summary>
        [TestMethod]
        public void UpdateBitTest()
        {
            _knowledgeBits.SetBits(_floats1, 0);
            Assert.AreEqual(-0.5F, _knowledgeBits.UpdateBit(0, -0.5F, 1));
            Assert.AreEqual(0.5F, _knowledgeBits.GetBit(0));
            Assert.AreEqual(1, _knowledgeBits.GetLastTouched()[0]);
            Assert.AreEqual(1, _knowledgeBits.GetBit(1));
            Assert.AreEqual(0, _knowledgeBits.GetLastTouched()[1]);
        }

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void UpdateBitTest1()
        {
            _knowledgeBits.SetBits(_floats0, 0);
            Assert.AreEqual(0, _knowledgeBits.UpdateBit(0, -1, 1));
            Assert.AreEqual(0, _knowledgeBits.GetBit(0));
            Assert.AreEqual(0, _knowledgeBits.GetLastTouched()[0]);
        }

        /// <summary>
        ///     Non passing test - value already at minimum range
        /// </summary>
        [TestMethod]
        public void UpdateLastTouchedTest()
        {
            _knowledgeBits.SetBits(_floats0, 0);
            _knowledgeBits.UpdateLastTouched(0, 1);
            Assert.AreEqual(0, _knowledgeBits.GetLastTouched()[0]);
        }

        /// <summary>
        ///     passing test
        /// </summary>
        [TestMethod]
        public void UpdateLastTouchedTest1()
        {
            _knowledgeBits.SetBits(_floats1, 0);
            _knowledgeBits.UpdateLastTouched(0, 1);
            Assert.AreEqual(1, _knowledgeBits.GetLastTouched()[0]);
        }
    }
}