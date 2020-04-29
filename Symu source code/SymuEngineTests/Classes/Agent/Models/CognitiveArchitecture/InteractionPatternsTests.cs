#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Agent.Models.CognitiveArchitecture;
using SymuEngine.Common;

#endregion

namespace SymuEngineTests.Classes.Agent.Models.CognitiveArchitecture
{
    [TestClass]
    public class InteractionPatternsTests
    {
        private InteractionPatterns _interaction;

        [TestInitialize]
        public void Initialize()
        {
            _interaction = new InteractionPatterns();
        }

        [TestMethod]
        public void NonPassingNextIsolationTest()
        {
            _interaction.AgentCanBeIsolated = Frequency.Never;
            Assert.IsFalse(_interaction.IsIsolated());
        }

        [TestMethod]
        public void PassingNextIsolationTest()
        {
            _interaction.IsolationIsRandom = true;
            _interaction.AgentCanBeIsolated = Frequency.Always;
            Assert.IsTrue(_interaction.IsIsolated());
            _interaction.AgentCanBeIsolated = Frequency.Never;
            Assert.IsFalse(_interaction.IsIsolated());
        }

        [TestMethod]
        public void NextInteractionStrategyTest()
        {
            _interaction.InteractionsUsingHomophily = 1;
            _interaction.InteractionsDeliberateSearch = 0;
            _interaction.InteractionsWithCoWorkers = 0;
            Assert.AreEqual(InteractionStrategy.Homophily, _interaction.NextInteractionStrategy());
        }
    }
}