#region Licence

// Description: SymuBiz - SymuLearnAndForgetTests
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
using Symu.Common.Classes;
using Symu.Engine;
using Symu.Repository.Entities;
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
        private readonly ExampleOrganization _organization = new ExampleOrganization();
        private readonly SymuEngine _simulation = new SymuEngine();

        [TestInitialize]
        public void Initialize()
        {
            _ = new Knowledge(_organization.MetaNetwork,_organization.Models, "1", 50);
            _environment.SetOrganization(_organization);
            _simulation.SetEnvironment(_environment);
            _environment.SetDebug(true);
            var scenario = TimeBasedScenario.CreateInstance(_environment);
            scenario.NumberOfSteps = 10;
            _simulation.AddScenario(scenario);
        }

        private void Process()
        {
            _organization.AddWiki();
            _simulation.Process();
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
            Process();
            Assert.AreEqual(0, _environment.LearnByDoingAgent.LearningModel.CumulativeLearning);
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
            _organization.KnowledgeLevel = KnowledgeLevel.Expert;
            Process();
            Assert.IsTrue(0 < _environment.LearnByDoingAgent.LearningModel.CumulativeLearning);
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
            _organization.KnowledgeLevel = KnowledgeLevel.Expert;
            Process();
            // Should be 0 because Knowledge threshold for doing is > 0, agent has the knowledge or not but he can't learn
            Assert.AreEqual(0, _environment.LearnByDoingAgent.LearningModel.CumulativeLearning);
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
            _organization.KnowledgeLevel = KnowledgeLevel.BasicKnowledge;
            Process();
            // Should be > 0 because Knowledge threshold for doing is == 0, agent has the knowledge or not but he can't learn
            Assert.IsTrue(0 < _environment.LearnByDoingAgent.LearningModel.CumulativeLearning);
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
            _organization.KnowledgeLevel = KnowledgeLevel.Expert;
            Process();
            Assert.AreEqual(0, _environment.LearnByDoingAgent.LearningModel.CumulativeLearning);
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
            Process();
            Assert.IsTrue(0 < _environment.LearnByAskingAgent.LearningModel.CumulativeLearning);
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
            Process();
            Assert.AreEqual(0, _environment.LearnByAskingAgent.LearningModel.CumulativeLearning);
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
            Process();
            Assert.AreEqual(0, _environment.LearnByAskingAgent.LearningModel.CumulativeLearning);
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
            Process();
            Assert.AreEqual(0, _environment.LearnByAskingAgent.LearningModel.CumulativeLearning);
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
            Process();
            Assert.AreEqual(0, _environment.LearnByAskingAgent.LearningModel.CumulativeLearning);
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
            Process();
            Assert.AreEqual(0, _environment.LearnByAskingAgent.LearningModel.CumulativeLearning);
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
            Process();
            Assert.AreEqual(0, _environment.LearnByAskingAgent.LearningModel.CumulativeLearning);
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
            Process();
            Assert.IsTrue(0 < _environment.LearnFromSourceAgent.LearningModel.CumulativeLearning);
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
            Process();
            Assert.AreEqual(0, _environment.LearnFromSourceAgent.LearningModel.CumulativeLearning);
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
            Process();
            var global = _environment.IterationResult.KnowledgeAndBeliefResults.Learning.Last();
            Assert.AreEqual(0, global.Sum);
            global = _environment.IterationResult.KnowledgeAndBeliefResults.Forgetting.Last();
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
            Process();
            // Should be = 0 because fullKnowledge => nothing to learn
            Assert.AreEqual(0, _environment.LearnFromSourceAgent.LearningModel.CumulativeLearning);
            Assert.AreEqual(0, _environment.LearnByDoingAgent.LearningModel.CumulativeLearning);
            Assert.AreEqual(0, _environment.LearnByAskingAgent.LearningModel.CumulativeLearning);
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
            _organization.KnowledgeLevel = KnowledgeLevel.FullKnowledge;
            Process();
            // Should be = 0 because fullKnowledge => nothing to learn
            Assert.AreEqual(0, _environment.LearnFromSourceAgent.LearningModel.CumulativeLearning);
            Assert.AreEqual(0, _environment.LearnByDoingAgent.LearningModel.CumulativeLearning);
            Assert.AreEqual(0, _environment.LearnByAskingAgent.LearningModel.CumulativeLearning);
            Assert.AreEqual(_environment.ExampleOrganization.Knowledge.Length,
                _environment.LearnFromSourceAgent.KnowledgeModel.GetKnowledgeSum());
            Assert.AreEqual(_environment.ExampleOrganization.Knowledge.Length,
                _environment.LearnByDoingAgent.KnowledgeModel.GetKnowledgeSum());
            Assert.AreEqual(_environment.ExampleOrganization.Knowledge.Length,
                _environment.LearnByAskingAgent.KnowledgeModel.GetKnowledgeSum());
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
            Process();
            // Should be = 0 because fullKnowledge => nothing to learn
            Assert.AreEqual(0, _environment.LearnFromSourceAgent.LearningModel.CumulativeLearning);
            Assert.AreEqual(0, _environment.LearnByDoingAgent.LearningModel.CumulativeLearning);
            Assert.AreEqual(0, _environment.LearnByAskingAgent.LearningModel.CumulativeLearning);
            Assert.AreEqual(0,
                _environment.LearnFromSourceAgent.KnowledgeModel.GetKnowledgeSum());
            Assert.AreEqual(0,
                _environment.LearnByDoingAgent.KnowledgeModel.GetKnowledgeSum());
            Assert.AreEqual(0,
                _environment.LearnByAskingAgent.KnowledgeModel.GetKnowledgeSum());
            Assert.AreEqual(0, _environment.LearnFromSourceAgent.ForgettingModel.CumulativeForgetting);
            Assert.AreEqual(0, _environment.LearnByDoingAgent.ForgettingModel.CumulativeForgetting);
            Assert.AreEqual(0, _environment.LearnByAskingAgent.ForgettingModel.CumulativeForgetting);
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
            _organization.KnowledgeLevel = KnowledgeLevel.Expert;
            Process();
            Assert.IsTrue(0 > _environment.LearnFromSourceAgent.ForgettingModel.CumulativeForgetting);
            Assert.IsTrue(0 > _environment.LearnByDoingAgent.ForgettingModel.CumulativeForgetting);
            Assert.IsTrue(0 > _environment.LearnByAskingAgent.ForgettingModel.CumulativeForgetting);
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
            _organization.KnowledgeLevel = KnowledgeLevel.Expert;
            Process();
            Assert.IsTrue(0 > _environment.LearnFromSourceAgent.ForgettingModel.CumulativeForgetting);
            Assert.IsTrue(0 > _environment.LearnByDoingAgent.ForgettingModel.CumulativeForgetting);
            Assert.IsTrue(0 > _environment.LearnByAskingAgent.ForgettingModel.CumulativeForgetting);
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
            _organization.KnowledgeLevel = KnowledgeLevel.Expert;
            Process();
            Assert.AreEqual(0, _environment.LearnFromSourceAgent.ForgettingModel.CumulativeForgetting);
            Assert.AreEqual(0, _environment.LearnByDoingAgent.ForgettingModel.CumulativeForgetting);
            Assert.AreEqual(0, _environment.LearnByAskingAgent.ForgettingModel.CumulativeForgetting);
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
            _organization.KnowledgeLevel = KnowledgeLevel.Expert;
            Process();
            Assert.AreEqual(0, _environment.LearnFromSourceAgent.ForgettingModel.CumulativeForgetting);
            Assert.AreEqual(0, _environment.LearnByDoingAgent.ForgettingModel.CumulativeForgetting);
            Assert.AreEqual(0, _environment.LearnByAskingAgent.ForgettingModel.CumulativeForgetting);
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
            _organization.KnowledgeLevel = KnowledgeLevel.Expert;
            Process();
            Assert.AreNotEqual(0, _environment.LearnFromSourceAgent.ForgettingModel.CumulativeForgetting);
            Assert.AreNotEqual(0, _environment.LearnByDoingAgent.ForgettingModel.CumulativeForgetting);
            Assert.AreNotEqual(0, _environment.LearnByAskingAgent.ForgettingModel.CumulativeForgetting);
            Assert.AreEqual(0,
                _environment.LearnFromSourceAgent.KnowledgeModel.GetKnowledgeSum());
            Assert.AreEqual(0,
                _environment.LearnByDoingAgent.KnowledgeModel.GetKnowledgeSum());
            Assert.AreEqual(0,
                _environment.LearnByAskingAgent.KnowledgeModel.GetKnowledgeSum());
        }

        #endregion
    }
}