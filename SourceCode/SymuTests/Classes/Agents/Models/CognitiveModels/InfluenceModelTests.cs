#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Common.Classes;
using Symu.Common.Interfaces;
using Symu.Repository.Edges;
using Symu.Repository.Entities;
using SymuTests.Helpers;

#endregion

namespace SymuTests.Classes.Agents.Models.CognitiveModels
{
    [TestClass]
    public class InfluenceModelTests : BaseTestClass
    {
        private readonly AgentId _agentId = new AgentId(1, 1);
        private readonly CognitiveArchitecture _cognitiveArchitecture = new CognitiveArchitecture();
        private Belief _belief;
        private BeliefsModel _beliefsModel;
        private InfluenceModel _influenceModel;
        private InternalCharacteristics InternalCharacteristics => _cognitiveArchitecture.InternalCharacteristics;


        [TestInitialize]
        public void Initialize()
        {
            MainOrganization.Models.SetOn(1);
            _belief = new Belief(MainOrganization.MetaNetwork, 1, RandomGenerator.RandomUniform,
                BeliefWeightLevel.RandomWeight);
            Environment.SetOrganization(MainOrganization);
            _beliefsModel = new BeliefsModel(_agentId, MainOrganization.Models.Beliefs, _cognitiveArchitecture,
                MainOrganization.MetaNetwork, MainOrganization.Models.Generator);
            _influenceModel = new InfluenceModel(MainOrganization.Models.Influence, _cognitiveArchitecture, WhitePages,
                _beliefsModel, MainOrganization.Models.Generator);
        }

        /// <summary>
        ///     Model off
        /// </summary>
        [TestMethod]
        public void ReinforcementByDoingTest()
        {
            _influenceModel.On = false;
            _influenceModel.ReinforcementByDoing(_belief.EntityId, 0, BeliefLevel.NoBelief);
            Assert.IsFalse(MainOrganization.MetaNetwork.ActorBelief.Exists(_agentId, _belief.EntityId));
        }

        /// <summary>
        ///     Model on / have belief
        /// </summary>
        [TestMethod]
        public void ReinforcementByDoingTest1()
        {
            _influenceModel.On = true;
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasBelief = true;
            Assert.IsFalse(MainOrganization.MetaNetwork.ActorBelief.Exists(_agentId, _belief.EntityId));
            _influenceModel.ReinforcementByDoing(_belief.EntityId, 0, BeliefLevel.NoBelief);
            //BeInfluenced new belief
            Assert.IsTrue(MainOrganization.MetaNetwork.ActorBelief.Exists(_agentId, _belief.EntityId));
            var actorBelief = MainOrganization.MetaNetwork.ActorBelief.Edge<ActorBelief>(_agentId, _belief.EntityId);
            Assert.AreNotEqual(0, actorBelief.BeliefBits.GetBit(0));
        }

        /// <summary>
        ///     Model on / Don't have belief (by default)
        /// </summary>
        [TestMethod]
        public void LearnByDoingTest2()
        {
            _influenceModel.On = true;
            _influenceModel.ReinforcementByDoing(_belief.EntityId, 0, BeliefLevel.NoBelief);
            Assert.IsFalse(MainOrganization.MetaNetwork.ActorBelief.Exists(_agentId, _belief.EntityId));
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