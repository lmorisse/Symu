#region Licence

// Description: Symu - SymuEngineTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Murphies;
using SymuEngine.Common;
using SymuEngine.Messaging.Messages;

#endregion

namespace SymuEngineTests.Classes.Murphy
{
    [TestClass]
    public class MurphyIncompleteBeliefTests
    {
        private readonly MurphyIncompleteBelief _murphy = new MurphyIncompleteBelief();

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

        [TestMethod]
        public void AskOnWhichChannelTest()
        {
            Assert.AreEqual(CommunicationMediums.System, _murphy.AskOnWhichChannel(CommunicationMediums.System));
            Assert.AreEqual(CommunicationMediums.System, _murphy.AskOnWhichChannel(CommunicationMediums.Email));
        }

        /// <summary>
        ///     No limit
        /// </summary>
        [TestMethod]
        public void ShouldGuessTest()
        {
            _murphy.LimitNumberOfTries = -1;
            Assert.IsFalse(_murphy.ShouldGuess(10));
        }

        /// <summary>
        ///     With limit
        /// </summary>
        [TestMethod]
        public void ShouldGuessTest1()
        {
            _murphy.LimitNumberOfTries = 1;
            Assert.IsFalse(_murphy.ShouldGuess(0));
            Assert.IsFalse(_murphy.ShouldGuess(1));
            Assert.IsTrue(_murphy.ShouldGuess(2));
        }

        /// <summary>
        ///     Model off
        /// </summary>
        [TestMethod]
        public void NextImpactOnTimeSpentTest()
        {
            Assert.AreEqual(0, _murphy.NextImpactOnTimeSpent());
        }

        /// <summary>
        ///     Model on - ImpactOnTimeSpentRatio = 0
        /// </summary>
        [TestMethod]
        public void NextImpactOnTimeSpentTest1()
        {
            _murphy.On = true;
            _murphy.ImpactOnTimeSpentRatio = 1;
            Assert.AreNotEqual(0, _murphy.NextImpactOnTimeSpent());
        }

        /// <summary>
        ///     RateOfAnswers = 0
        /// </summary>
        [TestMethod]
        public void DelayToReplyToHelpTest()
        {
            _murphy.RateOfAnswers = 0;
            Assert.AreEqual(-1, _murphy.DelayToReplyToHelp());
        }

        /// <summary>
        ///     RateOfAnswers = 1
        /// </summary>
        [TestMethod]
        public void DelayToReplyToHelpTes1T()
        {
            _murphy.RateOfAnswers = 1;
            _murphy.ResponseTime = 2;
            Assert.AreNotEqual(-1, _murphy.DelayToReplyToHelp());
            Assert.IsTrue(_murphy.DelayToReplyToHelp() <= _murphy.ResponseTime);
        }
    }
}