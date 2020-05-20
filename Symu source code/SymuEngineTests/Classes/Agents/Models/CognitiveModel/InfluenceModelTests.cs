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
using SymuEngine.Classes.Agents.Models.CognitiveModel;
using SymuEngine.Classes.Organization;
using SymuEngine.Common;
using SymuEngine.Repository.Networks;
using SymuEngine.Repository.Networks.Beliefs;

#endregion

namespace SymuEngineTests.Classes.Agents.Models.CognitiveModel
{
    [TestClass]
    public class InfluenceModelTests
    {
        private readonly AgentId _agentId = new AgentId(1, 1);
        private readonly InternalCharacteristics _internalCharacteristics = new InternalCharacteristics();
        private InfluenceModel _influenceModel;
        private Network _network;


        [TestInitialize]
        public void Initialize()
        {
            _network = new Network(new AgentTemplates(), new OrganizationModels());
            var entity = new ModelEntity();
            _influenceModel = new InfluenceModel(_agentId, entity, _internalCharacteristics, _network);
        }

        /// <summary>
        ///     Model off
        /// </summary>
        [TestMethod]
        public void LearnByDoingTest()
        {
            _influenceModel.On = false;
            _network.NetworkBeliefs.Model = RandomGenerator.RandomUniform;
            var belief = new Belief(1, "1", 1, _network.NetworkBeliefs.Model, BeliefWeightLevel.RandomWeight);
            _network.NetworkBeliefs.AddBelief(belief);
            _influenceModel.ReinforcementByDoing(belief.Id, 0, BeliefLevel.NoBelief);
            Assert.IsFalse(_network.NetworkBeliefs.Exists(_agentId, belief.Id));
        }

        /// <summary>
        ///     Model on
        /// </summary>
        [TestMethod]
        public void LearnByDoingTest1()
        {
            _influenceModel.On = true;
            _network.NetworkBeliefs.Model = RandomGenerator.RandomUniform;
            var belief = new Belief(1, "1", 1, _network.NetworkBeliefs.Model, BeliefWeightLevel.RandomWeight);
            _network.NetworkBeliefs.AddBelief(belief);
            Assert.IsFalse(_network.NetworkBeliefs.Exists(_agentId, belief.Id));
            _influenceModel.ReinforcementByDoing(belief.Id, 0, BeliefLevel.NoBelief);
            //BeInfluenced new belief
            Assert.IsTrue(_network.NetworkBeliefs.Exists(_agentId, belief.Id));
            var agentBelief = _network.NetworkBeliefs.GetAgentBelief(_agentId, belief.Id);
            Assert.AreNotEqual(0, agentBelief.BeliefBits.GetBit(0));
        }

        [TestMethod]
        public void NextInfluentialnessTest()
        {
            _internalCharacteristics.InfluentialnessRateMin = 0;
            _internalCharacteristics.InfluentialnessRateMax = 0;
            Assert.AreEqual(0, InfluenceModel.NextInfluentialness(_internalCharacteristics));
        }

        [TestMethod]
        public void NextInfluentialnessTest1()
        {
            _internalCharacteristics.InfluentialnessRateMin = 0;
            _internalCharacteristics.InfluentialnessRateMax = 1;
            var influence = InfluenceModel.NextInfluentialness(_internalCharacteristics);
            Assert.IsTrue(0 <= influence && influence <= 1);
        }

        [TestMethod]
        public void NextInfluenceabilityTest()
        {
            _internalCharacteristics.InfluenceabilityRateMin = 0;
            _internalCharacteristics.InfluenceabilityRateMax = 0;
            Assert.AreEqual(0, InfluenceModel.NextInfluenceability(_internalCharacteristics));
        }

        [TestMethod]
        public void NextInfluenceabilityTest1()
        {
            _internalCharacteristics.InfluenceabilityRateMin = 0;
            _internalCharacteristics.InfluenceabilityRateMax = 1;
            var influence = InfluenceModel.NextInfluenceability(_internalCharacteristics);
            Assert.IsTrue(0 <= influence && influence <= 1);
        }
    }
}