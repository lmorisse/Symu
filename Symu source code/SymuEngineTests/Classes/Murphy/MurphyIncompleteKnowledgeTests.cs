#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Murphies;
using SymuEngine.Common;

#endregion

namespace SymuEngineTests.Classes.Murphy
{
    [TestClass]
    public class MurphyIncompleteKnowledgeTests
    {
        private readonly MurphyIncompleteKnowledge _murphy = new MurphyIncompleteKnowledge();

        [TestMethod]
        public void AskInternallyTest()
        {
            _murphy.DelayBeforeSearchingExternally = 3;
            Assert.IsTrue(_murphy.AskInternally(2, 0));
            Assert.IsFalse(_murphy.AskInternally(3, 0));
        }

        /// <summary>
        ///     Model Off
        /// </summary>
        [TestMethod]
        public void NextGuessTest()
        {
            Assert.AreEqual(ImpactLevel.None, _murphy.NextGuess());
        }

        /// <summary>
        ///     Model On - RateOfIncorrectGuess = 0
        /// </summary>
        [TestMethod]
        public void NextGuessTest1()
        {
            _murphy.On = true;
            _murphy.RateOfIncorrectGuess = 0;
            Assert.AreEqual(ImpactLevel.None, _murphy.NextGuess());
        }

        /// <summary>
        ///     Model On - RateOfIncorrectGuess = 1
        /// </summary>
        [TestMethod]
        public void NextGuessTest2()
        {
            _murphy.On = true;
            _murphy.RateOfIncorrectGuess = 1;
            Assert.AreNotEqual(ImpactLevel.None, _murphy.NextGuess());
        }
    }
}