#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Common;
using Symu.Repository.Networks.Beliefs;

#endregion


namespace SymuTests.Repository.Networks.Beliefs
{
    [TestClass]
    public class AgentBeliefsTests
    {
        private const RandomGenerator Model = new RandomGenerator();
        private readonly AgentBelief _agentBelief = new AgentBelief(1, BeliefLevel.NoBelief);
        private readonly Belief _belief = new Belief(1, "1", 1, Model, BeliefWeightLevel.RandomWeight);
        private readonly AgentBeliefs _beliefs = new AgentBeliefs();
        private float[] _beliefBitsNeutral;
        private float[] _beliefBitsNonNeutral;

        [TestInitialize]
        public void Initialize()
        {
            _beliefBitsNonNeutral = _belief.InitializeBits(Model, BeliefLevel.NeitherAgreeNorDisagree);
            _beliefBitsNeutral = _belief.InitializeBits(Model, BeliefLevel.NoBelief);
        }

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
            _beliefs.Add(_belief.Id, BeliefLevel.NoBelief);
            Assert.AreEqual(1, _beliefs.Count);
            // Duplicate
            _beliefs.Add(_belief.Id, BeliefLevel.NoBelief);
            Assert.AreEqual(1, _beliefs.Count);
        }

        [TestMethod]
        public void ContainsTest()
        {
            Assert.IsFalse(_beliefs.Contains(_belief.Id));
            _beliefs.Add(_belief.Id, BeliefLevel.NoBelief);
            Assert.IsTrue(_beliefs.Contains(_belief.Id));
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
            Assert.IsNull(_beliefs.GetBelief(_belief.Id));
            _beliefs.Add(_belief.Id, BeliefLevel.NoBelief);
            Assert.IsNotNull(_beliefs.GetBelief(_belief.Id));
        }

        [TestMethod]
        public void BelievesEnoughTest()
        {
            Assert.IsFalse(_beliefs.BelievesEnough(_belief.Id, 0, 1));
            _agentBelief.SetBeliefBits(_beliefBitsNonNeutral);
            _beliefs.Add(_agentBelief);
            Assert.IsTrue(_beliefs.BelievesEnough(_belief.Id, 0, 0));
        }

        /// <summary>
        ///     Neutral initialization
        /// </summary>
        [TestMethod]
        public void GetBeliefsSumTest()
        {
            _agentBelief.SetBeliefBits(_beliefBitsNeutral);
            _beliefs.Add(_agentBelief);
            Assert.AreEqual(0, _beliefs.GetBeliefsSum());
        }

        /// <summary>
        ///     non neutral initialization
        /// </summary>
        [TestMethod]
        public void GetBeliefsSumTest1()
        {
            _agentBelief.SetBeliefBits(_beliefBitsNonNeutral);
            _beliefs.Add(_agentBelief);
            Assert.AreNotEqual(0, _beliefs.GetBeliefsSum());
        }
    }
}