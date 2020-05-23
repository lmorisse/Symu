#region Licence

// Description: Symu - SymuEngineTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models;
using Symu.Classes.Agents.Models.CognitiveModel;
using Symu.Classes.Agents.Models.Templates.Communication;
using Symu.Classes.Murphies;
using Symu.Classes.Organization;
using Symu.Classes.Task;
using Symu.Common;
using Symu.Messaging.Messages;
using Symu.Repository.Networks;
using Symu.Repository.Networks.Beliefs;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace SymuTests.Classes.Murphies
{
    [TestClass]
    public class MurphyIncompleteBeliefTests
    {
        private readonly MurphyIncompleteBelief _murphy = new MurphyIncompleteBelief();

        private readonly AgentId _agentId = new AgentId(1, 1);
        private readonly TaskKnowledgeBits _taskBits = new TaskKnowledgeBits();
        private Belief _belief;
        private BeliefsModel _beliefsModel;
        private CognitiveArchitecture _cognitiveArchitecture;
        private Network _network;
        private AgentBeliefs _agentBeliefs;

        [TestInitialize]
        public void Initialize()
        {
            _network = new Network(new AgentTemplates(), new OrganizationModels());
            _cognitiveArchitecture = new CognitiveArchitecture
            {
                KnowledgeAndBeliefs = { HasBelief = true, HasKnowledge = true },
                MessageContent = { CanReceiveBeliefs = true, CanReceiveKnowledge = true },
                InternalCharacteristics = { CanLearn = true, CanForget = true, CanInfluenceOrBeInfluence = true }
            };
            var modelEntity = new ModelEntity();
            _beliefsModel = new BeliefsModel(_agentId, modelEntity, _cognitiveArchitecture, _network) { On = true };
            _belief = new Belief(1, "1", 1, RandomGenerator.RandomUniform, BeliefWeightLevel.RandomWeight);

            _network.NetworkBeliefs.AddBelief(_belief);
            _network.NetworkBeliefs.Add(_agentId, _belief, BeliefLevel.NeitherAgreeNorDisagree);
            _agentBeliefs = _network.NetworkBeliefs.GetAgentBeliefs(_agentId);

            _taskBits.SetMandatory(new byte[] { 0 });
            _taskBits.SetRequired(new byte[] { 0 });
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
                _murphy.CheckBelief(_belief, null, _agentBeliefs, ref mandatoryCheck, ref requiredCheck, ref mandatoryIndex,
                    ref requiredIndex));
            // no belief
            Assert.ThrowsException<NullReferenceException>(() => _murphy.CheckBelief(null, _taskBits, _agentBeliefs,
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
            _murphy.CheckBelief(_belief, _taskBits, _agentBeliefs, ref mandatoryCheck, ref requiredCheck, ref mandatoryIndex,
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
            _beliefsModel.On = true;
            _beliefsModel.AddBelief(_belief.Id, BeliefLevel.NeitherAgreeNorDisagree);
            _beliefsModel.InitializeBeliefs();
            // Force beliefBits
            _beliefsModel.GetBelief(_belief.Id).BeliefBits.SetBit(0, 1);
            _belief.Weights.SetBit(0, 1);
            _murphy.CheckBelief(_belief, _taskBits, _agentBeliefs, ref mandatoryCheck, ref requiredCheck, ref mandatoryIndex,
                ref requiredIndex);
            Assert.AreEqual(1, mandatoryCheck);
            Assert.AreEqual(1, requiredCheck);
        }

        /// <summary>
        ///     Model Off
        /// </summary>
        [TestMethod]
        public void NextGuessTest()
        {
            _murphy.On = false;
            Assert.AreEqual(ImpactLevel.None, _murphy.NextGuess());
        }

        /// <summary>
        ///     Model On - RateOfIncorrectGuess = 0
        /// </summary>
        [TestMethod]
        public void NextGuessTest1()
        {
            _murphy.On = true;
            _murphy.RateOfAgentsOn = 1;
            _murphy.RateOfIncorrectGuess = 0;
            Assert.AreEqual(ImpactLevel.None, _murphy.NextGuess());
        }

        /// <summary>
        ///     Model On - RateOfIncorrectGuess = 1
        /// </summary>
        [TestMethod]
        public void NextGuessTest2()
        {
            _murphy.On = true;
            _murphy.RateOfAgentsOn = 1;
            _murphy.RateOfIncorrectGuess = 1;
            Assert.AreNotEqual(ImpactLevel.None, _murphy.NextGuess());
        }

        [TestMethod]
        public void AskOnWhichChannelTest()
        {
            Assert.AreEqual(CommunicationMediums.System, _murphy.AskOnWhichChannel(CommunicationMediums.System));
            Assert.AreEqual(CommunicationMediums.System, _murphy.AskOnWhichChannel(CommunicationMediums.Email));
        }

        /// <summary>
        ///     No limit
        /// </summary>
        [TestMethod]
        public void ShouldGuessTest()
        {
            _murphy.LimitNumberOfTries = -1;
            Assert.IsFalse(_murphy.ShouldGuess(10));
        }

        /// <summary>
        ///     With limit
        /// </summary>
        [TestMethod]
        public void ShouldGuessTest1()
        {
            _murphy.LimitNumberOfTries = 1;
            Assert.IsFalse(_murphy.ShouldGuess(0));
            Assert.IsFalse(_murphy.ShouldGuess(1));
            Assert.IsTrue(_murphy.ShouldGuess(2));
        }

        /// <summary>
        ///     Model off
        /// </summary>
        [TestMethod]
        public void NextImpactOnTimeSpentTest()
        {
            _murphy.On = false;
            Assert.AreEqual(0, _murphy.NextImpactOnTimeSpent());
        }

        /// <summary>
        ///     Model on - ImpactOnTimeSpentRatio = 0
        /// </summary>
        [TestMethod]
        public void NextImpactOnTimeSpentTest1()
        {
            _murphy.On = true;
            _murphy.RateOfAgentsOn = 1;
            _murphy.ImpactOnTimeSpentRatio = 1;
            Assert.AreNotEqual(0, _murphy.NextImpactOnTimeSpent());
        }

        /// <summary>
        ///     RateOfAnswers = 0
        /// </summary>
        [TestMethod]
        public void DelayToReplyToHelpTest()
        {
            _murphy.RateOfAnswers = 0;
            Assert.AreEqual(-1, _murphy.DelayToReplyToHelp());
        }

        /// <summary>
        ///     RateOfAnswers = 1
        /// </summary>
        [TestMethod]
        public void DelayToReplyToHelpTes1T()
        {
            _murphy.RateOfAnswers = 1;
            _murphy.ResponseTime = 2;
            Assert.AreNotEqual(-1, _murphy.DelayToReplyToHelp());
            Assert.IsTrue(_murphy.DelayToReplyToHelp() <= _murphy.ResponseTime);
        }
    }
}