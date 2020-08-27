#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents.Models;
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Common;
using Symu.Common.Interfaces.Agent;
using Symu.Repository.Entity;
using Symu.Repository.Networks.Beliefs;

#endregion


namespace SymuTests.Repository.Networks.Beliefs
{
    [TestClass]
    public class AgentBeliefsTests
    {
        private const RandomGenerator Model = new RandomGenerator();
        private AgentBelief _agentBelief ;
        private readonly Belief _belief = new Belief(1, "1", 1, Model, BeliefWeightLevel.RandomWeight);
        private readonly AgentBeliefs _beliefs = new AgentBeliefs();

        [TestInitialize]
        public void Initialize()
        {
            _agentBelief = new AgentBelief(_belief.Id, BeliefLevel.NoBelief);
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
            _beliefs.Add(_agentBelief);
            Assert.AreEqual(1, _beliefs.Count);
            // Duplicate
            _beliefs.Add(_agentBelief);
            Assert.AreEqual(1, _beliefs.Count);
        }

        [TestMethod]
        public void ContainsTest()
        {
            Assert.IsFalse(_beliefs.Contains(_belief.Id));
            _beliefs.Add(_agentBelief);
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
            Assert.IsNull(_beliefs.GetAgentBelief(_belief.Id));
            _beliefs.Add(_agentBelief);
            Assert.IsNotNull(_beliefs.GetAgentBelief(_belief.Id));
        }

    }
}