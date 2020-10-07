#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Common;
using Symu.OrgMod.GraphNetworks.TwoModesNetworks.Sphere;

#endregion

namespace SymuTests.Classes.Agents.Models.CognitiveModels
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
            Assert.IsFalse(_interaction.IsIsolated(0));
        }

        /// <summary>
        ///     Random isolation - always
        /// </summary>
        [TestMethod]
        public void PassingNextIsolationTest()
        {
            _interaction.IsolationCyclicity = Cyclicity.Random;
            _interaction.AgentCanBeIsolated = Frequency.Always;
            Assert.IsTrue(_interaction.IsIsolated(0));
        }

        /// <summary>
        ///     Random isolation - never
        /// </summary>
        [TestMethod]
        public void PassingNextIsolationTest1()
        {
            _interaction.IsolationCyclicity = Cyclicity.Random;
            _interaction.AgentCanBeIsolated = Frequency.Never;
            Assert.IsFalse(_interaction.IsIsolated(0));
        }

        /// <summary>
        ///     Random cyclical - always
        /// </summary>
        [TestMethod]
        public void PassingNextIsolationTest2()
        {
            _interaction.IsolationCyclicity = Cyclicity.Cyclical;
            _interaction.AgentCanBeIsolated = Frequency.Always;
            Assert.IsTrue(_interaction.IsIsolated(0));
        }

        /// <summary>
        ///     Random isolation - never
        /// </summary>
        [TestMethod]
        public void PassingNextIsolationTest3()
        {
            _interaction.IsolationCyclicity = Cyclicity.Cyclical;
            _interaction.AgentCanBeIsolated = Frequency.Never;
            Assert.IsFalse(_interaction.IsIsolated(0));
        }

        /// <summary>
        ///     Random isolation - never
        /// </summary>
        [TestMethod]
        public void PassingNextIsolationTest4()
        {
            _interaction.IsolationCyclicity = Cyclicity.Cyclical;
            _interaction.AgentCanBeIsolated = Frequency.VeryRarely;
            Assert.IsTrue(_interaction.IsIsolated(0));
            Assert.IsFalse(_interaction.IsIsolated(1));
            Assert.IsFalse(_interaction.IsIsolated(2));
        }

        [TestMethod]
        public void NextInteractionStrategyTest()
        {
            _interaction.InteractionsBasedOnHomophily = 1;
            _interaction.InteractionsBasedOnKnowledge = 0;
            _interaction.InteractionsBasedOnActivities = 0;
            Assert.AreEqual(InteractionStrategy.Homophily, _interaction.NextInteractionStrategy());
        }
    }
}