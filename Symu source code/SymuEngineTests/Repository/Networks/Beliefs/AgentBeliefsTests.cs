#region Licence

// Description: Symu - SymuEngineTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Common;
using SymuEngine.Repository.Networks.Beliefs;

#endregion


namespace SymuEngineTests.Repository.Networks.Beliefs
{
    [TestClass]
    public class AgentBeliefsTests
    {
        private const RandomGenerator Model = new RandomGenerator();
        private readonly AgentBelief _agentBelief = new AgentBelief(1);
        private readonly AgentBeliefs _beliefs = new AgentBeliefs();

        [TestMethod]
        public void AddTest()
        {
            Assert.AreEqual(0, _beliefs.Count);
            _beliefs.Add(_agentBelief);
            Assert.AreEqual(1, _beliefs.Count);
            // Duplicate
            _beliefs.Add(_agentBelief);
            Assert.AreEqual(1, _beliefs.Count);
        }

        [TestMethod]
        public void AddTest1()
        {
            Assert.AreEqual(0, _beliefs.Count);
            _beliefs.Add(1);
            Assert.AreEqual(1, _beliefs.Count);
            // Duplicate
            _beliefs.Add(1);
            Assert.AreEqual(1, _beliefs.Count);
        }

        [TestMethod]
        public void ContainsTest()
        {
            Assert.IsFalse(_beliefs.Contains(1));
            _beliefs.Add(1);
            Assert.IsTrue(_beliefs.Contains(1));
        }

        [TestMethod]
        public void ContainsTest1()
        {
            Assert.IsFalse(_beliefs.Contains(_agentBelief));
            _beliefs.Add(_agentBelief);
            Assert.IsTrue(_beliefs.Contains(_agentBelief));
        }

        [TestMethod]
        public void GetBeliefTest()
        {
            Assert.IsNull(_beliefs.GetBelief(1));
            _beliefs.Add(1);
            Assert.IsNotNull(_beliefs.GetBelief(1));
        }

        [TestMethod]
        public void BelievesEnoughTest()
        {
            Assert.IsFalse(_beliefs.BelievesEnough(1, 0, 1));
            _agentBelief.InitializeBeliefBits(Model, 1, false);
            _beliefs.Add(_agentBelief);
            Assert.IsTrue(_beliefs.BelievesEnough(1, 0, 0));
        }

        /// <summary>
        ///     Neutral initialization
        /// </summary>
        [TestMethod]
        public void GetBeliefsSumTest()
        {
            _agentBelief.InitializeBeliefBits(Model, 1, true);
            _beliefs.Add(_agentBelief);
            Assert.AreEqual(0, _beliefs.GetBeliefsSum());
        }

        /// <summary>
        ///     non neutral initialization
        /// </summary>
        [TestMethod]
        public void GetBeliefsSumTest1()
        {
            _agentBelief.InitializeBeliefBits(RandomGenerator.RandomUniform, 1, false);
            _beliefs.Add(_agentBelief);
            Assert.AreNotEqual(0, _beliefs.GetBeliefsSum());
        }
    }
}