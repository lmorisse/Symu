#region Licence

// Description: Symu - SymuLearnAndForgetTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Classes.Organization;
using Symu.Classes.Scenario;
using Symu.Common;
using Symu.Engine;
using Symu.Repository.Networks.Knowledges;
using SymuLearnAndForget.Classes;

#endregion


namespace SymuLearnAndForgetTests
{
    /// <summary>
    ///     Integration tests using SymuEngine
    /// </summary>
    [TestClass]
    public class IntegrationTests
    {
        private readonly ExampleEnvironment _environment = new ExampleEnvironment();
        private readonly OrganizationEntity _organization = new OrganizationEntity("1");
        private readonly SymuEngine _simulation = new SymuEngine();

        [TestInitialize]
        public void Initialize()
        {
            _environment.SetOrganization(_organization);
            _simulation.SetEnvironment(_environment);
            _environment.SetDebug(true);
            var scenario = new TimeBasedScenario(_environment)
            {
                NumberOfSteps = 10
            };
            _simulation.AddScenario(scenario);
            _environment.Knowledge = new Knowledge(1, "1", 50);
        }

        #region ReinforcementByDoing

        /// <summary>
        ///     Learning Model on
        ///     BeInfluenced by doing without initialKnowledge
        /// </summary>
        [TestMethod]
        public void LearnByDoingTest()
        {
            _organization.Models.Learning.On = true;
            _organization.Models.Learning.RateOfAgentsOn = 1;
            _organization.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge = false;
            _simulation.Process();
            Assert.AreEqual(0, _environment.LearnByDoingAgent.KnowledgeModel.Expertise.Learning);
        }

        /// <summary>
        ///     Learning Model on
        ///     BeInfluenced by doing with initialKnowledge - random uniform
        /// </summary>
        [TestMethod]
        public void LearnByDoingTest1()
        {
            _organization.Models.Learning.On = true;
            _organization.Models.Learning.RateOfAgentsOn = 1;
            _organization.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge = true;
            _environment.KnowledgeLevel = KnowledgeLevel.Expert;
            _simulation.Process();
            Assert.IsTrue(0 < _environment.LearnByDoingAgent.KnowledgeModel.Expertise.Learning);
        }

        /// <summary>
        ///     Learning Model on
        ///     BeInfluenced by doing with initialKnowledge - random binary
        /// </summary>
        [TestMethod]
        public void LearnByDoingTest2()
        {
            _organization.Models.Learning.On = true;
            _organization.Models.Learning.RateOfAgentsOn = 1;
            _organization.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge = true;
            _organization.Models.Generator = RandomGenerator.RandomBinary;
            _environment.KnowledgeLevel = KnowledgeLevel.Expert;
            _simulation.Process();
            // Should be 0 because Knowledge threshold for doing is > 0, agent has the knowledge or not but he can't learn
            Assert.AreEqual(0, _environment.LearnByDoingAgent.KnowledgeModel.Expertise.Learning);
        }

        /// <summary>
        ///     Learning Model on
        ///     BeInfluenced by doing with initialKnowledge - random binary
        ///     KnowledgeThreshHoldForReacting == 0
        /// </summary>
        [TestMethod]
        public void LearnByDoingTest3()
        {
            _organization.Models.Learning.On = true;
            _organization.Models.Learning.RateOfAgentsOn = 1;
            _organization.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge = true;
            _organization.Models.Generator = RandomGenerator.RandomBinary;
            _organization.Murphies.IncompleteKnowledge.ThresholdForReacting = 0;
            _environment.KnowledgeLevel = KnowledgeLevel.BasicKnowledge;
            _simulation.Process();
            // Should be > 0 because Knowledge threshold for doing is == 0, agent has the knowledge or not but he can't learn
            Assert.IsTrue(0 < _environment.LearnByDoingAgent.KnowledgeModel.Expertise.Learning);
        }

        /// <summary>
        ///     Learning Model on
        ///     LearningByDoingRate = 0
        /// </summary>
        [TestMethod]
        public void LearnByDoingTest4()
        {
            _organization.Models.Learning.On = true;
            _organization.Models.Learning.RateOfAgentsOn = 1;
            _organization.Templates.Human.Cognitive.TasksAndPerformance.LearningByDoingRate = 0;
            _environment.KnowledgeLevel = KnowledgeLevel.Expert;
            _simulation.Process();
            Assert.AreEqual(0, _environment.LearnByDoingAgent.KnowledgeModel.Expertise.Learning);
        }

        #endregion

        #region LearnByAsking

        /// <summary>
        ///     Learning Model on
        ///     Default setting - Passing test
        /// </summary>
        [TestMethod]
        public void LearnByAskingTest()
        {
            _organization.Models.Learning.On = true;
            _organization.Models.Learning.RateOfAgentsOn = 1;
            _simulation.Process();
            Assert.IsTrue(0 < _environment.LearnByAskingAgent.KnowledgeModel.Expertise.Learning);
        }

        /// <summary>
        ///     Non passing test
        ///     MinimumLengthToSendPerBit =1
        /// </summary>
        [TestMethod]
        public void LearnByAskingTest1()
        {
            _organization.Models.Learning.On = true;
            _organization.Models.Learning.RateOfAgentsOn = 1;
            _organization.Templates.Human.Cognitive.MessageContent.MinimumKnowledgeToSendPerBit = 1;
            _simulation.Process();
            Assert.AreEqual(0, _environment.LearnByAskingAgent.KnowledgeModel.Expertise.Learning);
        }

        /// <summary>
        ///     Non passing test
        ///     CanReceiveKnowledge false
        /// </summary>
        [TestMethod]
        public void LearnByAskingTest2()
        {
            _organization.Models.Learning.On = true;
            _organization.Models.Learning.RateOfAgentsOn = 1;
            _organization.Templates.Human.Cognitive.MessageContent.CanReceiveKnowledge = false;
            _simulation.Process();
            Assert.AreEqual(0, _environment.LearnByAskingAgent.KnowledgeModel.Expertise.Learning);
        }

        /// <summary>
        ///     Non passing test
        ///     CanSendKnowledge false
        /// </summary>
        [TestMethod]
        public void LearnByAskingTest3()
        {
            _organization.Models.Learning.On = true;
            _organization.Models.Learning.RateOfAgentsOn = 1;
            _organization.Templates.Human.Cognitive.MessageContent.CanSendKnowledge = false;
            _simulation.Process();
            Assert.AreEqual(0, _environment.LearnByAskingAgent.KnowledgeModel.Expertise.Learning);
        }

        /// <summary>
        ///     Non passing test
        ///     MaxRateLearnable 0
        /// </summary>
        [TestMethod]
        public void LearnByAskingTest4()
        {
            _organization.Models.Learning.On = true;
            _organization.Models.Learning.RateOfAgentsOn = 1;
            _organization.Communication.Email.MaxRateLearnable = 0;
            _simulation.Process();
            Assert.AreEqual(0, _environment.LearnByAskingAgent.KnowledgeModel.Expertise.Learning);
        }

        /// <summary>
        ///     Non passing test
        ///     NumberOfBitsOfBeliefToSend 0
        /// </summary>
        [TestMethod]
        public void LearnByAskingTest5()
        {
            _organization.Models.Learning.On = true;
            _organization.Models.Learning.RateOfAgentsOn = 1;
            _organization.Templates.Human.Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend = 0;
            _organization.Templates.Human.Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 0;
            _organization.Communication.Email.MinimumNumberOfBitsOfKnowledgeToSend = 0;
            _organization.Communication.Email.MaximumNumberOfBitsOfKnowledgeToSend = 0;
            _simulation.Process();
            Assert.AreEqual(0, _environment.LearnByAskingAgent.KnowledgeModel.Expertise.Learning);
        }

        /// <summary>
        ///     Non passing test
        ///     LearningRate = 0
        /// </summary>
        [TestMethod]
        public void LearnByAskingTest6()
        {
            _organization.Models.Learning.On = true;
            _organization.Models.Learning.RateOfAgentsOn = 1;
            _organization.Templates.Human.Cognitive.TasksAndPerformance.LearningRate = 0;
            _simulation.Process();
            Assert.AreEqual(0, _environment.LearnByAskingAgent.KnowledgeModel.Expertise.Learning);
        }

        #endregion

        #region LearnFromSource

        /// <summary>
        ///     Passing test
        /// </summary>
        [TestMethod]
        public void LearnFromSourceTest()
        {
            _organization.Models.Learning.On = true;
            _organization.Models.Learning.RateOfAgentsOn = 1;
            _simulation.Process();
            Assert.IsTrue(0 < _environment.LearnFromSourceAgent.KnowledgeModel.Expertise.Learning);
        }

        /// <summary>
        ///     Non Passing test
        ///     LearningRate = 0
        /// </summary>
        [TestMethod]
        public void LearnFromSourceTest1()
        {
            _organization.Models.Learning.On = true;
            _organization.Models.Learning.RateOfAgentsOn = 1;
            _organization.Templates.Human.Cognitive.TasksAndPerformance.LearningRate = 0;
            _simulation.Process();
            Assert.AreEqual(0, _environment.LearnFromSourceAgent.KnowledgeModel.Expertise.Learning);
        }

        #endregion

        #region Common

        /// <summary>
        ///     All models off
        /// </summary>
        [TestMethod]
        public void ModelsOffTest()
        {
            _organization.Models.Learning.On = false;
            _simulation.Process();
            var global = _environment.IterationResult.OrganizationKnowledgeAndBelief.Learning.Last();
            Assert.AreEqual(0, global.Sum);
            global = _environment.IterationResult.OrganizationKnowledgeAndBelief.Forgetting.Last();
            Assert.AreEqual(0, global.Sum);
        }

        /// <summary>
        ///     Learning Model on - RateOfAgentsOn = 0
        /// </summary>
        [TestMethod]
        public void ModelOnTest()
        {
            _organization.Models.Learning.On = true;
            _organization.Models.Learning.RateOfAgentsOn = 0;
            _simulation.Process();
            // Should be = 0 because fullKnowledge => nothing to learn
            Assert.AreEqual(0, _environment.LearnFromSourceAgent.KnowledgeModel.Expertise.Learning);
            Assert.AreEqual(0, _environment.LearnByDoingAgent.KnowledgeModel.Expertise.Learning);
            Assert.AreEqual(0, _environment.LearnByAskingAgent.KnowledgeModel.Expertise.Learning);
        }

        /// <summary>
        ///     Learning Model on
        ///     BeInfluenced by doing with initialKnowledge : Full Knowledge- random binary
        /// </summary>
        [DataRow(RandomGenerator.RandomBinary)]
        [DataRow(RandomGenerator.RandomUniform)]
        [TestMethod]
        public void LearnWithInitialFullKnowledgeTest(RandomGenerator model)
        {
            _organization.Models.Learning.On = true;
            _organization.Models.Learning.RateOfAgentsOn = 1;
            _organization.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge = true;
            _organization.Models.Generator = model;
            _environment.KnowledgeLevel = KnowledgeLevel.FullKnowledge;
            _simulation.Process();
            // Should be = 0 because fullKnowledge => nothing to learn
            Assert.AreEqual(0, _environment.LearnFromSourceAgent.KnowledgeModel.Expertise.Learning);
            Assert.AreEqual(0, _environment.LearnByDoingAgent.KnowledgeModel.Expertise.Learning);
            Assert.AreEqual(0, _environment.LearnByAskingAgent.KnowledgeModel.Expertise.Learning);
            Assert.AreEqual(_environment.Knowledge.Length,
                _environment.LearnFromSourceAgent.KnowledgeModel.Expertise.GetKnowledgesSum());
            Assert.AreEqual(_environment.Knowledge.Length,
                _environment.LearnByDoingAgent.KnowledgeModel.Expertise.GetKnowledgesSum());
            Assert.AreEqual(_environment.Knowledge.Length,
                _environment.LearnByAskingAgent.KnowledgeModel.Expertise.GetKnowledgesSum());
        }

        /// <summary>
        ///     Learning Model on - agent has no knowledge
        /// </summary>
        [TestMethod]
        public void HasNoKnowledgeTest()
        {
            _organization.Models.Learning.On = true;
            _organization.Models.Learning.RateOfAgentsOn = 1;
            _organization.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasKnowledge = false;
            _simulation.Process();
            // Should be = 0 because fullKnowledge => nothing to learn
            Assert.AreEqual(0, _environment.LearnFromSourceAgent.KnowledgeModel.Expertise.Learning);
            Assert.AreEqual(0, _environment.LearnByDoingAgent.KnowledgeModel.Expertise.Learning);
            Assert.AreEqual(0, _environment.LearnByAskingAgent.KnowledgeModel.Expertise.Learning);
            Assert.AreEqual(0,
                _environment.LearnFromSourceAgent.KnowledgeModel.Expertise.GetKnowledgesSum());
            Assert.AreEqual(0,
                _environment.LearnByDoingAgent.KnowledgeModel.Expertise.GetKnowledgesSum());
            Assert.AreEqual(0,
                _environment.LearnByAskingAgent.KnowledgeModel.Expertise.GetKnowledgesSum());
            Assert.AreEqual(0, _environment.LearnFromSourceAgent.KnowledgeModel.Expertise.Forgetting);
            Assert.AreEqual(0, _environment.LearnByDoingAgent.KnowledgeModel.Expertise.Forgetting);
            Assert.AreEqual(0, _environment.LearnByAskingAgent.KnowledgeModel.Expertise.Forgetting);
        }

        /// <summary>
        ///     Forgetting PassingTest
        ///     Oldest mode
        /// </summary>
        [TestMethod]
        public void ForgettingTest()
        {
            _organization.Models.Forgetting.On = true;
            _organization.Models.Forgetting.RateOfAgentsOn = 1;
            _organization.Templates.Human.Cognitive.InternalCharacteristics.TimeToLive = 0;
            _organization.Templates.Human.Cognitive.InternalCharacteristics.ForgettingSelectingMode =
                ForgettingSelectingMode.Oldest;
            // must have some knowledge to forget
            _organization.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge = true;
            _environment.KnowledgeLevel = KnowledgeLevel.Expert;
            _simulation.Process();
            Assert.IsTrue(0 > _environment.LearnFromSourceAgent.KnowledgeModel.Expertise.Forgetting);
            Assert.IsTrue(0 > _environment.LearnByDoingAgent.KnowledgeModel.Expertise.Forgetting);
            Assert.IsTrue(0 > _environment.LearnByAskingAgent.KnowledgeModel.Expertise.Forgetting);
        }

        /// <summary>
        ///     Forgetting PassingTest
        ///     Random mode
        /// </summary>
        [TestMethod]
        public void ForgettingTest2()
        {
            _organization.Models.Forgetting.On = true;
            _organization.Models.Forgetting.RateOfAgentsOn = 1;
            _organization.Templates.Human.Cognitive.InternalCharacteristics.ForgettingSelectingMode =
                ForgettingSelectingMode.Random;
            // must have some knowledge to forget
            _organization.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge = true;
            _environment.KnowledgeLevel = KnowledgeLevel.Expert;
            _simulation.Process();
            Assert.IsTrue(0 > _environment.LearnFromSourceAgent.KnowledgeModel.Expertise.Forgetting);
            Assert.IsTrue(0 > _environment.LearnByDoingAgent.KnowledgeModel.Expertise.Forgetting);
            Assert.IsTrue(0 > _environment.LearnByAskingAgent.KnowledgeModel.Expertise.Forgetting);
        }

        /// <summary>
        ///     Forgetting Model on
        ///     Oldest - TimeToLive = -1
        /// </summary>
        [TestMethod]
        public void TimeToLiveTest()
        {
            _organization.Models.Forgetting.On = true;
            _organization.Models.Forgetting.RateOfAgentsOn = 1;
            _organization.Templates.Human.Cognitive.InternalCharacteristics.TimeToLive = -1;
            _organization.Templates.Human.Cognitive.InternalCharacteristics.ForgettingSelectingMode =
                ForgettingSelectingMode.Oldest;
            _organization.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge = true;
            _environment.KnowledgeLevel = KnowledgeLevel.Expert;
            _simulation.Process();
            Assert.AreEqual(0, _environment.LearnFromSourceAgent.KnowledgeModel.Expertise.Forgetting);
            Assert.AreEqual(0, _environment.LearnByDoingAgent.KnowledgeModel.Expertise.Forgetting);
            Assert.AreEqual(0, _environment.LearnByAskingAgent.KnowledgeModel.Expertise.Forgetting);
        }

        /// <summary>
        ///     Forgetting Model on
        ///     ForgettingMean = 0
        /// </summary>
        [TestMethod]
        public void ForgettingMeanTest()
        {
            _organization.Models.Forgetting.On = true;
            _organization.Models.Forgetting.RateOfAgentsOn = 1;
            _organization.Templates.Human.Cognitive.InternalCharacteristics.ForgettingSelectingMode =
                ForgettingSelectingMode.Random;
            _organization.Templates.Human.Cognitive.InternalCharacteristics.ForgettingMean = 0;
            _organization.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge = true;
            _environment.KnowledgeLevel = KnowledgeLevel.Expert;
            _simulation.Process();
            Assert.AreEqual(0, _environment.LearnFromSourceAgent.KnowledgeModel.Expertise.Forgetting);
            Assert.AreEqual(0, _environment.LearnByDoingAgent.KnowledgeModel.Expertise.Forgetting);
            Assert.AreEqual(0, _environment.LearnByAskingAgent.KnowledgeModel.Expertise.Forgetting);
        }

        /// <summary>
        ///     Forgetting Model on
        ///     Random, mean = 1, no partial forgetting, no minimum remaining knowledge
        /// </summary>
        [TestMethod]
        public void NoRemainingKnowledgeTest()
        {
            _organization.Models.Forgetting.On = true;
            _organization.Models.Forgetting.RateOfAgentsOn = 1;
            _organization.Templates.Human.Cognitive.InternalCharacteristics.ForgettingSelectingMode =
                ForgettingSelectingMode.Random;
            _organization.Templates.Human.Cognitive.InternalCharacteristics.ForgettingMean = 1;
            _organization.Templates.Human.Cognitive.InternalCharacteristics.PartialForgetting = false;
            _organization.Templates.Human.Cognitive.InternalCharacteristics.MinimumRemainingKnowledge = 0;
            _organization.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge = true;
            _environment.KnowledgeLevel = KnowledgeLevel.Expert;
            _simulation.Process();
            Assert.AreNotEqual(0, _environment.LearnFromSourceAgent.KnowledgeModel.Expertise.Forgetting);
            Assert.AreNotEqual(0, _environment.LearnByDoingAgent.KnowledgeModel.Expertise.Forgetting);
            Assert.AreNotEqual(0, _environment.LearnByAskingAgent.KnowledgeModel.Expertise.Forgetting);
            Assert.AreEqual(0,
                _environment.LearnFromSourceAgent.KnowledgeModel.Expertise.GetKnowledgesSum());
            Assert.AreEqual(0,
                _environment.LearnByDoingAgent.KnowledgeModel.Expertise.GetKnowledgesSum());
            Assert.AreEqual(0,
                _environment.LearnByAskingAgent.KnowledgeModel.Expertise.GetKnowledgesSum());
        }

        #endregion
    }
}