#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models;
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Classes.Organization;
using Symu.Common;
using Symu.Repository.Networks;
using Symu.Repository.Networks.Beliefs;

#endregion

namespace SymuTests.Classes.Agents.Models.CognitiveModel
{
    [TestClass]
    public class InfluenceModelTests
    {
        private readonly AgentId _agentId = new AgentId(1, 1);
        private readonly InternalCharacteristics _internalCharacteristics = new InternalCharacteristics();
        private InfluenceModel _influenceModel;
        private MetaNetwork _network;


        [TestInitialize]
        public void Initialize()
        {
            var models = new OrganizationModels();
            _network = new MetaNetwork(models.InteractionSphere, models.ImpactOfBeliefOnTask);
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
            _network.Beliefs.Model = RandomGenerator.RandomUniform;
            var belief = new Belief(1, "1", 1, _network.Beliefs.Model, BeliefWeightLevel.RandomWeight);
            _network.Beliefs.AddBelief(belief);
            _influenceModel.ReinforcementByDoing(belief.Id, 0, BeliefLevel.NoBelief);
            Assert.IsFalse(_network.Beliefs.Exists(_agentId, belief.Id));
        }

        /// <summary>
        ///     Model on
        /// </summary>
        [TestMethod]
        public void LearnByDoingTest1()
        {
            _influenceModel.On = true;
            _network.Beliefs.Model = RandomGenerator.RandomUniform;
            var belief = new Belief(1, "1", 1, _network.Beliefs.Model, BeliefWeightLevel.RandomWeight);
            _network.Beliefs.AddBelief(belief);
            Assert.IsFalse(_network.Beliefs.Exists(_agentId, belief.Id));
            _influenceModel.ReinforcementByDoing(belief.Id, 0, BeliefLevel.NoBelief);
            //BeInfluenced new belief
            Assert.IsTrue(_network.Beliefs.Exists(_agentId, belief.Id));
            var agentBelief = _network.Beliefs.GetAgentBelief(_agentId, belief.Id);
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