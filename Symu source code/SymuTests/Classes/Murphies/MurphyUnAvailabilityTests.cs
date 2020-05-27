#region Licence

// Description: Symu - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Murphies;

#endregion

namespace SymuTests.Classes.Murphies
{
    [TestClass]
    public class MurphyUnAvailabilityTests
    {
        private readonly MurphyUnAvailability _murphy = new MurphyUnAvailability();

        [TestMethod]
        public void NonPassingNextTest()
        {
            _murphy.On = false;
            _murphy.RateOfUnavailability = 1;
            Assert.IsFalse(_murphy.Next());
        }

        /// <summary>
        ///     RateOfUnavailability = 0
        /// </summary>
        [TestMethod]
        public void PassingNextTest()
        {
            _murphy.On = true;
            _murphy.RateOfAgentsOn = 1;
            _murphy.RateOfUnavailability = 0;
            Assert.IsFalse(_murphy.Next());
        }

        /// <summary>
        ///     RateOfUnavailability = 1
        /// </summary>
        [TestMethod]
        public void PassingNextTest1()
        {
            _murphy.On = true;
            _murphy.RateOfAgentsOn = 1;
            _murphy.RateOfUnavailability = 1;
            Assert.IsTrue(_murphy.Next());
        }
    }
}