#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Murphies;

#endregion

namespace SymuEngineTests.Classes.Murphy
{
    [TestClass]
    public class MurphyUnAvailabilityTests
    {
        private readonly MurphyUnAvailability _murphy = new MurphyUnAvailability();

        [TestMethod]
        public void NonPassingNextTest()
        {
            _murphy.Threshold = 0.5F;
            Assert.AreEqual(0.4F, _murphy.Next(0.4F));
        }

        [TestMethod]
        public void PassingNextTest()
        {
            _murphy.Threshold = 0.5F;
            _murphy.On = true;
            Assert.AreEqual(0, _murphy.Next(0.4F));
            Assert.AreEqual(0.5F, _murphy.Next(0.5F));
        }
    }
}