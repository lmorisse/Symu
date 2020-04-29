﻿#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Agent;
using SymuEngine.Classes.Agent.Models.CognitiveArchitecture;
using SymuEngine.Common;
using SymuEngine.Repository.Networks;
using SymuEngine.Repository.Networks.Knowledge.Agent;
using SymuEngine.Repository.Networks.Knowledge.Bits;
using SymuEngine.Repository.Networks.Knowledge.Repository;

#endregion

namespace SymuEngineTests.Classes.Agent.Models.CognitiveArchitecture
{
    [TestClass]
    public class TasksAndPerformanceTests
    {
        private readonly AgentId _agentId = new AgentId(1, 1);
        private readonly AgentExpertise _expertise = new AgentExpertise();
        private readonly Knowledge _knowledge = new Knowledge(1, "1", 1);
        private readonly Network _network = new Network();
        private TasksAndPerformance _tasksAndPerformance;

        [TestInitialize]
        public void Initialize()
        {
            _network.NetworkKnowledges.Add(_agentId, _expertise);
            InitializeModel(0, true);
        }

        public void InitializeModel(byte randomLevel, bool modelOn)
        {
            _tasksAndPerformance = new TasksAndPerformance(_network, _agentId, randomLevel)
            {
                LearningModel = {On = modelOn, RateOfAgentsOn = 1}
            };
        }

        [TestMethod]
        public void LearnByDoingTest()
        {
            _tasksAndPerformance.LearningByDoingRate = 1;
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {0}, 0);
            _expertise.Add(agentKnowledge);
            _network.NetworkKnowledges.Add(_agentId, _expertise);
            _tasksAndPerformance.LearnByDoing(_knowledge.Id, 0, 0);
            Assert.AreEqual(1, agentKnowledge.GetKnowledgeSum());
        }

        /// <summary>
        ///     Passing test learn by Bit
        /// </summary>
        [TestMethod]
        public void LearnTest()
        {
            _tasksAndPerformance.LearningRate = 1;
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {0}, 0);
            _expertise.Add(agentKnowledge);
            _network.NetworkKnowledges.Add(_agentId, _expertise);
            _tasksAndPerformance.Learn(_knowledge.Id, 0, 0);
            Assert.AreEqual(1, agentKnowledge.GetKnowledgeSum());
        }

        /// <summary>
        ///     Non Passing test learn by Bit
        /// </summary>
        [TestMethod]
        public void LearnTest1()
        {
            _tasksAndPerformance.LearningRate = 0;
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {0}, 0);
            _expertise.Add(agentKnowledge);
            _network.NetworkKnowledges.Add(_agentId, _expertise);
            _tasksAndPerformance.Learn(_knowledge.Id, 0, 0);
            Assert.AreEqual(0, agentKnowledge.GetKnowledgeSum());
        }

        /// <summary>
        ///     Passing test learn by Bits
        /// </summary>
        [TestMethod]
        public void LearnTest2()
        {
            _tasksAndPerformance.LearningRate = 1;
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {0, 0}, 0);
            _expertise.Add(agentKnowledge);
            _network.NetworkKnowledges.Add(_agentId, _expertise);
            var bits = new float[] {1, 1};
            var knowledgeBits = new Bits(bits, 0);
            _tasksAndPerformance.Learn(_knowledge.Id, knowledgeBits, 1, 0);
            Assert.AreEqual(2, agentKnowledge.GetKnowledgeSum());
        }

        /// <summary>
        ///     Non Passing test learn by Bits
        /// </summary>
        [TestMethod]
        public void LearnTest3()
        {
            _tasksAndPerformance.LearningRate = 0;
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {0, 0}, 0);
            _expertise.Add(agentKnowledge);
            _network.NetworkKnowledges.Add(_agentId, _expertise);
            var bits = new float[] {1, 1};
            var knowledgeBits = new Bits(bits, 0);
            _tasksAndPerformance.Learn(_knowledge.Id, knowledgeBits, 1, 0);
            Assert.AreEqual(0, agentKnowledge.GetKnowledgeSum());
        }

        [TestMethod]
        public void IsMaximumTasksInTotalTest()
        {
            _tasksAndPerformance.TasksLimit.MaximumTasksInTotal = 2;
            _tasksAndPerformance.TasksLimit.LimitTasksInTotal = false;
            Assert.IsFalse(_tasksAndPerformance.TasksLimit.HasReachedTotalMaximumLimit(2));
            _tasksAndPerformance.TasksLimit.LimitTasksInTotal = true;
            Assert.IsTrue(_tasksAndPerformance.TasksLimit.HasReachedTotalMaximumLimit(2));
        }

        /// <summary>
        ///     Model off
        /// </summary>
        [TestMethod]
        public void NextLearningTest()
        {
            InitializeModel(0, false);
            _tasksAndPerformance.LearningRate = 1;
            Assert.AreEqual(0, _tasksAndPerformance.NextLearning());
        }

        /// <summary>
        ///     Model on - no random
        /// </summary>
        [DataRow(0)]
        [DataRow(1)]
        [TestMethod]
        public void NextLearningTest1(float learningRate)
        {
            InitializeModel(0, true);
            _tasksAndPerformance.LearningModel.On = true;
            _tasksAndPerformance.LearningModel.RateOfAgentsOn = 1;
            _tasksAndPerformance.LearningRate = learningRate;
            Assert.AreEqual(learningRate, _tasksAndPerformance.NextLearning());
        }

        /// <summary>
        ///     Model on - random
        /// </summary>
        [TestMethod]
        public void NextLearningTest2()
        {
            InitializeModel(1, true);
            _tasksAndPerformance.LearningRate = 1;
            _tasksAndPerformance.LearningStandardDeviation = GenericLevel.Complete;
            Assert.AreNotEqual(1, _tasksAndPerformance.NextLearning());
        }

        /// <summary>
        ///     Model off
        /// </summary>
        [TestMethod]
        public void NextLearningByDoingTest()
        {
            InitializeModel(0, false);
            _tasksAndPerformance.LearningByDoingRate = 1;
            Assert.AreEqual(0, _tasksAndPerformance.NextLearningByDoing());
        }

        /// <summary>
        ///     Model on - no random
        /// </summary>
        [DataRow(0)]
        [DataRow(1)]
        [TestMethod]
        public void NextLearningByDoingTest1(float learningRate)
        {
            InitializeModel(0, true);
            _tasksAndPerformance.LearningByDoingRate = learningRate;
            Assert.AreEqual(learningRate, _tasksAndPerformance.NextLearningByDoing());
        }

        /// <summary>
        ///     Model on - random
        /// </summary>
        [TestMethod]
        public void NextLearningByDoingTest2()
        {
            InitializeModel(1, true);
            _tasksAndPerformance.LearningByDoingRate = 1;
            _tasksAndPerformance.LearningStandardDeviation = GenericLevel.Complete;
            Assert.AreNotEqual(1, _tasksAndPerformance.NextLearningByDoing());
        }
    }
}