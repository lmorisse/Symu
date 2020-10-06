#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Classes.Murphies;
using Symu.Classes.Organization;
using Symu.Classes.Task;
using Symu.Common.Classes;
using Symu.Common.Interfaces;
using Symu.DNA.Edges;
using Symu.DNA.GraphNetworks;
using Symu.Repository.Entities;
using SymuTests.Helpers;
using ActorBelief = Symu.Repository.Edges.ActorBelief;

#endregion

namespace SymuTests.Classes.Murphies
{
    [TestClass]
    public class MurphyIncompleteBeliefTests : BaseTestClass
    {
        private readonly AgentId _agentId = new AgentId(1, 1);
        private readonly MurphyIncompleteBelief _murphy = new MurphyIncompleteBelief();
        private readonly TaskKnowledgeBits _taskBits = new TaskKnowledgeBits();
        private Belief _belief;
        private BeliefsModel _beliefsModel;
        private CognitiveArchitecture _cognitiveArchitecture;
        private ActorBelief _actorBelief;

        [TestInitialize]
        public void Initialize()
        {
            Organization.Models.Generator = RandomGenerator.RandomUniform;
            _cognitiveArchitecture = new CognitiveArchitecture
            {
                KnowledgeAndBeliefs = {HasBelief = true, HasKnowledge = true},
                MessageContent = {CanReceiveBeliefs = true, CanReceiveKnowledge = true},
                InternalCharacteristics = {CanLearn = true, CanForget = true, CanInfluenceOrBeInfluence = true}
            };
            var modelEntity = new BeliefModelEntity {On = true};
            _beliefsModel = new BeliefsModel(_agentId, modelEntity, _cognitiveArchitecture, Network, Organization.Models.Generator) ;
            _belief = new Belief(Network, 1, Organization.Models.Generator, BeliefWeightLevel.RandomWeight);
            _actorBelief = new ActorBelief(_agentId, _belief.EntityId, BeliefLevel.NeitherAgreeNorDisagree);
            Network.ActorBelief.Add(_actorBelief);
            
            _taskBits.SetMandatory(new byte[] {0});
            _taskBits.SetRequired(new byte[] {0});
        }

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void NullCheckBeliefTest()
        {
            float mandatoryCheck = 0;
            float requiredCheck = 0;
            byte mandatoryIndex = 0;
            byte requiredIndex = 0;
            Assert.ThrowsException<ArgumentNullException>(() =>
                _murphy.CheckBelief(_belief, null, _actorBelief, ref mandatoryCheck, ref requiredCheck,
                    ref mandatoryIndex,
                    ref requiredIndex));
            // no belief
            Assert.ThrowsException<ArgumentNullException>(() => _murphy.CheckBelief(null, _taskBits, _actorBelief,
                ref mandatoryCheck, ref requiredCheck, ref mandatoryIndex, ref requiredIndex));
        }

        /// <summary>
        ///     Model off
        /// </summary>
        [TestMethod]
        public void CheckBeliefTest()
        {
            float mandatoryCheck = 0;
            float requiredCheck = 0;
            byte mandatoryIndex = 0;
            byte requiredIndex = 0;
            _murphy.CheckBelief(_belief, _taskBits, _actorBelief, ref mandatoryCheck, ref requiredCheck,
                ref mandatoryIndex,
                ref requiredIndex);
            Assert.AreEqual(0, mandatoryCheck);
            Assert.AreEqual(0, requiredCheck);
        }

        /// <summary>
        ///     Model on
        /// </summary>
        [TestMethod]
        public void CheckBeliefTest1()
        {
            float mandatoryCheck = 0;
            float requiredCheck = 0;
            byte mandatoryIndex = 0;
            byte requiredIndex = 0;
            _murphy.On = true;
            _beliefsModel.Entity.On = true;
            _beliefsModel.AddBelief(_belief.EntityId, BeliefLevel.NeitherAgreeNorDisagree);
            _beliefsModel.InitializeBeliefs();
            // Force beliefBits
            _beliefsModel.SetBelief(_belief.EntityId, 0, 1);
            _belief.Weights.SetBit(0, 1);
            _murphy.CheckBelief(_belief, _taskBits, _actorBelief, ref mandatoryCheck, ref requiredCheck,
                ref mandatoryIndex,
                ref requiredIndex);
            Assert.AreEqual(1, mandatoryCheck);
            Assert.AreEqual(1, requiredCheck);
        }
    }
}