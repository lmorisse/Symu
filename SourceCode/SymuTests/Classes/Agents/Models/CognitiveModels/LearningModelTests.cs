#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Classes.Organization;
using Symu.Common;
using Symu.Common.Interfaces.Agent;
using Symu.Engine;
using Symu.Repository.Networks;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace SymuTests.Classes.Agents.Models.CognitiveModels
{
    [TestClass]
    public class LearningModelTests
    {
        private readonly AgentId _agentId = new AgentId(1, 1);
        private readonly AgentExpertise _expertise = new AgentExpertise();
        private readonly Knowledge _knowledge = new Knowledge(1, "1", 1);
        private MetaNetwork _network;
        private readonly OrganizationModels _organizationModels = new OrganizationModels();
        private CognitiveArchitecture _cognitiveArchitecture;
        private LearningModel _learningModel;
        private AgentKnowledge _agentKnowledge;

        [TestInitialize]
        public void Initialize()
        {
            var models = new OrganizationModels();
            _network = new MetaNetwork(models.InteractionSphere, models.ImpactOfBeliefOnTask);
            _network.Knowledge.Add(_agentId, _expertise);
            InitializeModel(0, true);
            _agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] { 0, 0 }, 0, -1, 0);
            _expertise.Add(_agentKnowledge);
            _network.Knowledge.Add(_agentId, _expertise);
        }

        public void InitializeModel(RandomLevel randomLevelLevel, bool modelOn)
        {
            _organizationModels.RandomLevel = randomLevelLevel;
            _cognitiveArchitecture = new CognitiveArchitecture();
            _cognitiveArchitecture.InternalCharacteristics.CanLearn = true;
            _learningModel = new LearningModel(_agentId, _organizationModels, _network.Knowledge,
                _cognitiveArchitecture) {On = modelOn, RateOfAgentsOn = 1};
        }

        /// <summary>
        ///     Model Off
        /// </summary>
        [TestMethod]
        public void LearnByDoingTest0()
        {
            _learningModel.On = false;
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {0}, 0, -1, 0);
            _expertise.Add(agentKnowledge);
            _network.Knowledge.Add(_agentId, _expertise);
            var realLearning = _learningModel.LearnByDoing(_knowledge.Id, 0, 0, -1, 0);
            Assert.AreEqual(0, agentKnowledge.GetKnowledgeSum());
            Assert.AreEqual(0, realLearning);
        }

        [TestMethod]
        public void LearnByDoingTest()
        {
            _cognitiveArchitecture.TasksAndPerformance.LearningByDoingRate = 1;
            var realLearning = _learningModel.LearnByDoing(_knowledge.Id, 0, 0, -1, 0);
            Assert.AreEqual(1, _agentKnowledge.GetKnowledgeSum());
            Assert.AreEqual(1, realLearning);
        }

        /// <summary>
        ///     Model Off
        /// </summary>
        [TestMethod]
        public void LearnTest0()
        {
            _learningModel.On = false;
            var realLearning = _learningModel.Learn(_knowledge.Id, 0, -1, 0, 0);
            Assert.AreEqual(0, _agentKnowledge.GetKnowledgeSum());
            Assert.AreEqual(0, realLearning);
        }

        /// <summary>
        ///     Passing test learn by Bit
        /// </summary>
        [TestMethod]
        public void LearnTest()
        {
            _cognitiveArchitecture.TasksAndPerformance.LearningRate = 1;
            var realLearning = _learningModel.Learn(_knowledge.Id, 0, -1, 0, 0);
            Assert.AreEqual(1, _agentKnowledge.GetKnowledgeSum());
            Assert.AreEqual(1, realLearning);
        }

        /// <summary>
        ///     Non Passing test learn by Bit
        /// </summary>
        [TestMethod]
        public void LearnTest1()
        {
            _cognitiveArchitecture.TasksAndPerformance.LearningRate = 0;
            _learningModel.Learn(_knowledge.Id, 0, 0, -1, 0);
            Assert.AreEqual(0, _agentKnowledge.GetKnowledgeSum());
        }

        /// <summary>
        ///     Passing test learn by Bits
        /// </summary>
        [TestMethod]
        public void LearnTest2()
        {
            _cognitiveArchitecture.TasksAndPerformance.LearningRate = 1;
            var bits = new float[] {1, 1};
            var knowledgeBits = new Bits(bits, 0);
            _learningModel.Learn(_knowledge.Id, knowledgeBits, 1, 0, -1, 0);
            Assert.AreEqual(2, _agentKnowledge.GetKnowledgeSum());
        }

        /// <summary>
        ///     Non Passing test learn by Bits
        /// </summary>
        [TestMethod]
        public void LearnTest3()
        {
            _cognitiveArchitecture.TasksAndPerformance.LearningRate = 0;
            var bits = new float[] {1, 1};
            var knowledgeBits = new Bits(bits, 0);
            _learningModel.Learn(_knowledge.Id, knowledgeBits, 1, 0, -1, 0);
            Assert.AreEqual(0, _agentKnowledge.GetKnowledgeSum());
        }

        /// <summary>
        ///     Model off
        /// </summary>
        [TestMethod]
        public void NextLearningTest()
        {
            InitializeModel(RandomLevel.NoRandom, false);
            _cognitiveArchitecture.TasksAndPerformance.LearningRate = 1;
            Assert.AreEqual(0, _learningModel.NextLearning());
        }

        /// <summary>
        ///     Model on - no random
        /// </summary>
        [DataRow(0)]
        [DataRow(1)]
        [TestMethod]
        public void NextLearningTest1(float learningRate)
        {
            InitializeModel(RandomLevel.NoRandom, true);
            _learningModel.On = true;
            _learningModel.RateOfAgentsOn = 1;
            _cognitiveArchitecture.TasksAndPerformance.LearningRate = learningRate;
            Assert.AreEqual(learningRate, _learningModel.NextLearning());
        }

        /// <summary>
        ///     Model on - random
        /// </summary>
        [TestMethod]
        public void NextLearningTest2()
        {
            InitializeModel(RandomLevel.Simple, true);
            _learningModel.TasksAndPerformance.LearningRate = 1;
            _learningModel.TasksAndPerformance.LearningStandardDeviation = GenericLevel.Complete;
            Assert.AreNotEqual(1, _learningModel.NextLearning());
        }

        /// <summary>
        ///     Model off
        /// </summary>
        [TestMethod]
        public void NextLearningByDoingTest()
        {
            InitializeModel(RandomLevel.NoRandom, false);
            _cognitiveArchitecture.TasksAndPerformance.LearningByDoingRate = 1;
            Assert.AreEqual(0, _learningModel.NextLearningByDoing());
        }

        /// <summary>
        ///     Model on - no random
        /// </summary>
        [DataRow(0)]
        [DataRow(1)]
        [TestMethod]
        public void NextLearningByDoingTest1(float learningRate)
        {
            InitializeModel(RandomLevel.NoRandom, true);
            _cognitiveArchitecture.TasksAndPerformance.LearningByDoingRate = learningRate;
            Assert.AreEqual(learningRate, _learningModel.NextLearningByDoing());
        }

        /// <summary>
        ///     Model on - random
        /// </summary>
        [TestMethod]
        public void NextLearningByDoingTest2()
        {
            InitializeModel(RandomLevel.Simple, true);
            _cognitiveArchitecture.TasksAndPerformance.LearningByDoingRate = 1;
            _cognitiveArchitecture.TasksAndPerformance.LearningStandardDeviation = GenericLevel.Complete;
            Assert.AreNotEqual(1, _learningModel.NextLearningByDoing());
        }
        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void AgentKnowledgeLearnTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _learningModel.AgentKnowledgeLearn(null, 0, 1, 0));
        }

        /// <summary>
        ///     Passing test - below range
        /// </summary>
        [TestMethod]
        public void AgentKnowledgeLearnTest1()
        {
            var realLearning = _learningModel.AgentKnowledgeLearn(_agentKnowledge, 0, 1, 0);
            Assert.AreEqual(1, _agentKnowledge.GetKnowledgeBit(0));
            Assert.AreEqual(1, realLearning);
        }

        /// <summary>
        ///     Passing test - above range
        /// </summary>
        [TestMethod]
        public void AgentKnowledgeLearnTest2()
        {
            var realLearning = _learningModel.AgentKnowledgeLearn(_agentKnowledge, 0, 2, 0);
            Assert.AreEqual(1, _agentKnowledge.GetKnowledgeBit(0));
            Assert.AreEqual(1, realLearning);
        }
    }
}