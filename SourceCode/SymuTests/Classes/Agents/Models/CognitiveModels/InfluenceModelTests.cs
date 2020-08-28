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
using Symu.Common.Classes;
using Symu.Common.Interfaces.Agent;
using Symu.Repository.Entity;
using Symu.Repository.Networks;

#endregion

namespace SymuTests.Classes.Agents.Models.CognitiveModels
{
    [TestClass]
    public class InfluenceModelTests
    {
        private readonly AgentId _agentId = new AgentId(1, 1);
        private readonly CognitiveArchitecture _cognitiveArchitecture = new CognitiveArchitecture();
        private readonly OrganizationModels _models = new OrganizationModels();
        private InfluenceModel _influenceModel;
        private SymuMetaNetwork _network;
        private BeliefsModel _beliefsModel;
        private InternalCharacteristics InternalCharacteristics => _cognitiveArchitecture.InternalCharacteristics;


        [TestInitialize]
        public void Initialize()
        {
            _network = new SymuMetaNetwork(_models.InteractionSphere);
            _beliefsModel = new BeliefsModel(_agentId, _models.Beliefs, _cognitiveArchitecture, _network, _models.Generator);
            _influenceModel = new InfluenceModel(_agentId, _models.Influence, _cognitiveArchitecture, _network, _beliefsModel, _models.Generator);
        }

        /// <summary>
        ///     Model off
        /// </summary>
        [TestMethod]
        public void LearnByDoingTest()
        {
            _influenceModel.On = false;
            var belief = new Belief(1, "1", 1, RandomGenerator.RandomUniform, BeliefWeightLevel.RandomWeight);
            _network.Beliefs.AddBelief(belief);
            _influenceModel.ReinforcementByDoing(belief.Id, 0, BeliefLevel.NoBelief);
            Assert.IsFalse(_network.Beliefs.Exists(_agentId, belief.Id));
        }

        /// <summary>
        ///     Model on / have belief
        /// </summary>
        [TestMethod]
        public void LearnByDoingTest1()
        {
            _influenceModel.On = true;
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasBelief = true;
            _influenceModel = new InfluenceModel(_agentId, _models.Influence, _cognitiveArchitecture, _network, _beliefsModel, _models.Generator);

            var belief = new Belief(1, "1", 1, RandomGenerator.RandomUniform, BeliefWeightLevel.RandomWeight);
            _network.Beliefs.AddBelief(belief);
            Assert.IsFalse(_network.Beliefs.Exists(_agentId, belief.Id));
            _influenceModel.ReinforcementByDoing(belief.Id, 0, BeliefLevel.NoBelief);
            //BeInfluenced new belief
            Assert.IsTrue(_network.Beliefs.Exists(_agentId, belief.Id));
            var agentBelief = _network.Beliefs.GetAgentBelief<AgentBelief>(_agentId, belief.Id);
            Assert.AreNotEqual(0, agentBelief.BeliefBits.GetBit(0));
        }

        /// <summary>
        ///     Model on / Don't have belief (by default)
        /// </summary>
        [TestMethod]
        public void LearnByDoingTest2()
        {
            _influenceModel.On = true;
            var belief = new Belief(1, "1", 1, RandomGenerator.RandomUniform, BeliefWeightLevel.RandomWeight);
            _network.Beliefs.AddBelief(belief);
            _influenceModel.ReinforcementByDoing(belief.Id, 0, BeliefLevel.NoBelief);
            Assert.IsFalse(_network.Beliefs.Exists(_agentId, belief.Id));
        }

        [TestMethod]
        public void NextInfluentialnessTest()
        {
            InternalCharacteristics.InfluentialnessRateMin = 0;
            InternalCharacteristics.InfluentialnessRateMax = 0;
            Assert.AreEqual(0, InfluenceModel.NextInfluentialness(InternalCharacteristics));
        }

        [TestMethod]
        public void NextInfluentialnessTest1()
        {
            InternalCharacteristics.InfluentialnessRateMin = 0;
            InternalCharacteristics.InfluentialnessRateMax = 1;
            var influence = InfluenceModel.NextInfluentialness(InternalCharacteristics);
            Assert.IsTrue(0 <= influence && influence <= 1);
        }

        [TestMethod]
        public void NextInfluenceabilityTest()
        {
            InternalCharacteristics.InfluenceabilityRateMin = 0;
            InternalCharacteristics.InfluenceabilityRateMax = 0;
            Assert.AreEqual(0, InfluenceModel.NextInfluenceability(InternalCharacteristics));
        }

        [TestMethod]
        public void NextInfluenceabilityTest1()
        {
            InternalCharacteristics.InfluenceabilityRateMin = 0;
            InternalCharacteristics.InfluenceabilityRateMax = 1;
            var influence = InfluenceModel.NextInfluenceability(InternalCharacteristics);
            Assert.IsTrue(0 <= influence && influence <= 1);
        }
    }
}