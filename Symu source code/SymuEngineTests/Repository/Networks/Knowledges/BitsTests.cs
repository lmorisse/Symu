#region Licence

// Description: Symu - SymuEngineTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace SymuTests.Repository.Networks.Knowledges
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

        /// <summary>
        ///     above min range
        /// </summary>
        [TestMethod]
        public void UpdateBitTest()
        {
            Assert.AreEqual(-0.5F, _bits1.UpdateBit(0, -0.5F));
        }

        /// <summary>
        ///     above min range
        /// </summary>
        [TestMethod]
        public void UpdateBitTest1()
        {
            Assert.AreEqual(-1, _bits1.UpdateBit(0, -3));
        }

        /// <summary>
        ///     below max range
        /// </summary>
        [TestMethod]
        public void UpdateBitTest2()
        {
            Assert.AreEqual(0.5F, _bits0.UpdateBit(0, 0.5F));
        }

        /// <summary>
        ///     above max range
        /// </summary>
        [TestMethod]
        public void UpdateBitTest3()
        {
            Assert.AreEqual(1, _bits0.UpdateBit(0, 3));
        }

        /// <summary>
        ///     issue : Case Forget a Bit[]=0 with a RangeMin >0
        /// </summary>
        [TestMethod]
        public void UpdateBitTest4()
        {
            _bits0 = new Bits(_floats0, 0.1F);
            Assert.AreEqual(0, _bits0.UpdateBit(0, -1));
            Assert.AreEqual(0, _bits0.GetBit(0));
        }


        [TestMethod]
        public void GetRelativeKnowledgeBitsTest()
        {
            _bits0 = new Bits(_floats0, 0);
            Assert.AreEqual(0, Bits.GetRelativeBits(_bits0, _bits0));
            _bits1 = new Bits(_floats1, 0);
            Assert.AreEqual(1, Bits.GetRelativeBits(_bits1, _bits1));
            Assert.AreEqual(0, Bits.GetRelativeBits(_bits0, _bits1));
        }
    }
}