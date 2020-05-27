#region Licence

// Description: Symu - ModelingTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Murphies;
using Symu.Common;

#endregion


namespace SymuTests.Classes.Murphies
{
    [TestClass]
    public class MurphyIncompleteInformationTests
    {
        private readonly MurphyIncompleteInformation _murphy = new MurphyIncompleteInformation();

        /// <summary>
        ///     Rate 0
        /// </summary>
        [TestMethod]
        public void CheckIncompleteInformationTests()
        {
            _murphy.ThresholdForReacting = 0;
            Assert.IsFalse(_murphy.CheckInformation());
        }

        /// <summary>
        ///     Rate 1
        /// </summary>
        [TestMethod]
        public void CheckIncompleteInformationTests1()
        {
            _murphy.ThresholdForReacting = 1;
            Assert.IsTrue(_murphy.CheckInformation());
        }
    }
}