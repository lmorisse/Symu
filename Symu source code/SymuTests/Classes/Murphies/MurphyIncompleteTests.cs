#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Murphies;
using Symu.Common;
using Symu.Messaging.Messages;

#endregion

namespace SymuTests.Classes.Murphies
{
    [TestClass]
    public class MurphyIncompleteTests
    {
        private readonly MurphyIncomplete _murphy = new MurphyIncomplete();

        /// <summary>
        ///     Rate 0
        /// </summary>
        [TestMethod]
        public void ReplyToHelpTests()
        {
            _murphy.RateOfAnswers = 0;
            Assert.AreEqual(-1, _murphy.DelayToReplyToHelp());
        }

        /// <summary>
        ///     Rate 1
        /// </summary>
        [TestMethod]
        public void ReplyToHelpTests1()
        {
            _murphy.RateOfAnswers = 1;
            _murphy.ResponseTime = 2;
            Assert.IsTrue(_murphy.DelayToReplyToHelp() >= 0);
            Assert.IsTrue(_murphy.DelayToReplyToHelp() <= 2);
        }

        /// <summary>
        ///     Model off
        /// </summary>
        [TestMethod]
        public void NextGuessTest()
        {
            _murphy.On = false;
            Assert.AreEqual(ImpactLevel.None, _murphy.NextGuess());
        }

        /// <summary>
        ///     Model on rate 0
        /// </summary>
        [TestMethod]
        public void NextGuessTest1()
        {
            _murphy.On = true;
            _murphy.RateOfAgentsOn = 1;
            _murphy.RateOfIncorrectGuess = 0;
            Assert.AreEqual(ImpactLevel.None, _murphy.NextGuess());
        }

        /// <summary>
        ///     model 0n - rate 1
        /// </summary>
        [TestMethod]
        public void NextGuessTest2()
        {
            _murphy.On = true;
            _murphy.RateOfAgentsOn = 1;
            _murphy.RateOfIncorrectGuess = 1;
            Assert.AreNotEqual(ImpactLevel.None, _murphy.NextGuess());
        }

        /// <summary>
        ///     No limit
        ///     Limit -1
        /// </summary>
        [TestMethod]
        public void ShouldGuessTest()
        {
            _murphy.LimitNumberOfTries = -1;
            Assert.IsFalse(_murphy.ShouldGuess(0));
        }

        /// <summary>
        ///     Limit of 0
        /// </summary>
        [TestMethod]
        public void ShouldGuessTest1()
        {
            _murphy.LimitNumberOfTries = 0;
            Assert.IsTrue(_murphy.ShouldGuess(0));
        }

        /// <summary>
        ///     Limit of 1
        /// </summary>
        [TestMethod]
        public void ShouldGuessTest2()
        {
            _murphy.LimitNumberOfTries = 1;
            Assert.IsFalse(_murphy.ShouldGuess(0));
            Assert.IsTrue(_murphy.ShouldGuess(1));
        }

        [TestMethod]
        public void AskInternallyTest()
        {
            _murphy.DelayBeforeSearchingExternally = 3;
            Assert.IsTrue(_murphy.AskInternally(2, 0));
            Assert.IsFalse(_murphy.AskInternally(3, 0));
        }


        [TestMethod]
        public void AskOnWhichChannelTest()
        {
            Assert.AreEqual(CommunicationMediums.System, _murphy.AskOnWhichChannel(CommunicationMediums.System));
            Assert.AreEqual(CommunicationMediums.System, _murphy.AskOnWhichChannel(CommunicationMediums.Email));
        }

        /// <summary>
        ///     Model off
        /// </summary>
        [TestMethod]
        public void NextImpactOnTimeSpentTest()
        {
            _murphy.On = false;
            Assert.AreEqual(1, _murphy.NextImpactOnTimeSpent());
        }

        /// <summary>
        ///     Model on - CostFactorOfGuessing = 0
        /// </summary>
        [TestMethod]
        public void NextImpactOnTimeSpentTest1()
        {
            _murphy.On = true;
            _murphy.RateOfAgentsOn = 1;
            _murphy.CostFactorOfGuessing = 1;
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