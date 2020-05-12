#region Licence

// Description: Symu - SymuEngineTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Agents;
using SymuEngine.Classes.Agents.Models;
using SymuEngine.Classes.Agents.Models.CognitiveArchitecture;
using SymuEngine.Classes.Organization;
using SymuEngine.Common;
using SymuEngine.Repository.Networks;
using SymuEngine.Repository.Networks.Beliefs;

#endregion

namespace SymuEngineTests.Classes.Agents.Models.CognitiveArchitecture
{
    [TestClass]
    public class InternalCharacteristicsTests
    {
        private readonly AgentId _agentId = new AgentId(1, 1);
        private InternalCharacteristics _model;
        private Network _network;

        [TestInitialize]
        public void Initialize()
        {
            _network = new Network(new AgentTemplates(), new InteractionSphereModel());
            _model = new InternalCharacteristics(_network, _agentId);
        }

        [TestMethod]
        public void NextInfluentialnessTest()
        {
            _model.InfluentialnessRateMin = 0;
            _model.InfluentialnessRateMax = 0;
            Assert.AreEqual(0, _model.NextInfluentialness());
        }

        [TestMethod]
        public void NextInfluentialnessTest1()
        {
            _model.InfluentialnessRateMin = 0;
            _model.InfluentialnessRateMax = 1;
            var influence = _model.NextInfluentialness();
            Assert.IsTrue(0 <= influence && influence <= 1);
        }

        [TestMethod]
        public void NextInfluenceabilityTest()
        {
            _model.InfluenceabilityRateMin = 0;
            _model.InfluenceabilityRateMax = 0;
            Assert.AreEqual(0, _model.NextInfluenceability());
        }

        [TestMethod]
        public void NextInfluenceabilityTest1()
        {
            _model.InfluenceabilityRateMin = 0;
            _model.InfluenceabilityRateMax = 1;
            var influence = _model.NextInfluenceability();
            Assert.IsTrue(0 <= influence && influence <= 1);
        }

        [TestMethod]
        public void LearnByDoingTest()
        {
            _network.NetworkBeliefs.Model = RandomGenerator.RandomUniform;
            var belief = new Belief(1, 1, _network.NetworkBeliefs.Model);
            _network.NetworkBeliefs.AddBelief(belief);
            Assert.IsFalse(_network.NetworkBeliefs.Exists(_agentId, belief.Id));
            _model.LearnByDoing(belief.Id, 0);
            //Learn new belief
            Assert.IsTrue(_network.NetworkBeliefs.Exists(_agentId, belief.Id));
            var agentBelief = _network.NetworkBeliefs.GetAgentBelief(_agentId, belief.Id);
            Assert.AreNotEqual(0, agentBelief.BeliefBits.GetBit(0));
        }
    }
}