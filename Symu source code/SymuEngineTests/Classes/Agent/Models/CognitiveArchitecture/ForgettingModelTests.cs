#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Agent;
using SymuEngine.Classes.Agent.Models;
using SymuEngine.Classes.Agent.Models.CognitiveArchitecture;
using SymuEngine.Classes.Agent.Models.CognitiveArchitecture.Forgetting;
using SymuEngine.Classes.Task.Knowledge;
using SymuEngine.Repository.Networks;
using SymuEngine.Repository.Networks.Knowledge;
using SymuEngine.Repository.Networks.Knowledge.Agent;
using SymuEngine.Repository.Networks.Knowledge.Repository;

#endregion

namespace SymuEngineTests.Classes.Agent.Models.CognitiveArchitecture
{
    [TestClass]
    public class ForgettingModelTests
    {
        private readonly AgentId _agentId = new AgentId(1, 1);
        private readonly AgentExpertise _expertise = new AgentExpertise();
        private readonly ModelEntity _forgetting = new ModelEntity();
        private readonly Knowledge _knowledge = new Knowledge(1, "1", 1);
        private readonly TaskKnowledgeBits _taskBits = new TaskKnowledgeBits();
        private InternalCharacteristics _internalCharacteristics;
        private ForgettingModel _model;
        private NetworkKnowledges _networkKnowledges;

        [TestInitialize]
        public void Initialize()
        {
            var network = new Network();
            _networkKnowledges = network.NetworkKnowledges;
            _networkKnowledges.Add(_agentId, _expertise);
            _taskBits.SetMandatory(new byte[] {0});
            _taskBits.SetRequired(new byte[] {0});
            _forgetting.On = true;
            _forgetting.RateOfAgentsOn = 1;
            _internalCharacteristics = new InternalCharacteristics(network, _agentId);
        }

        public void InitializeModel(bool modelOn, byte randomLevel)
        {
            _forgetting.On = modelOn;
            _model = new ForgettingModel(_forgetting, _internalCharacteristics, randomLevel, _networkKnowledges,
                _agentId);
        }

        /// <summary>
        ///     Model off
        /// </summary>
        [TestMethod]
        public void NextRateTest()
        {
            InitializeModel(false, 0);
            Assert.AreEqual(0, _model.NextRate());
        }

        /// <summary>
        ///     Model on - Complete forgetting
        /// </summary>
        [TestMethod]
        public void NextRateTest1()
        {
            _internalCharacteristics.PartialForgetting = false;
            InitializeModel(true, 0);
            Assert.AreEqual(1, _model.NextRate());
        }

        /// <summary>
        ///     Model on - partial forgetting
        /// </summary>
        [TestMethod]
        public void NextRateTest2()
        {
            _internalCharacteristics.PartialForgetting = true;
            InitializeModel(true, 0);
            Assert.AreEqual(_internalCharacteristics.PartialForgettingRate, _model.NextRate());
        }

        /// <summary>
        ///     Model off
        /// </summary>
        [TestMethod]
        public void NextMeanTest()
        {
            InitializeModel(false, 0);
            Assert.AreEqual(0, _model.NextMean());
        }

        /// <summary>
        ///     Model on - no random
        /// </summary>
        [TestMethod]
        public void NextMeanTest1()
        {
            InitializeModel(true, 0);

            Assert.AreEqual(_internalCharacteristics.ForgettingMean, _model.NextMean());
        }

        /// <summary>
        ///     With NextForgettingRate == 0 - ForgettingMean = 1
        /// </summary>
        [TestMethod]
        public void InitializeForgettingKnowledgeOldestTest()
        {
            var knowledgeBits = new float[] {0};
            var knowledge = new AgentKnowledge(1, knowledgeBits, 0);
            _internalCharacteristics.ForgettingMean = 1;
            InitializeModel(true, 0);
            var forgetting = _model.InitializeForgettingKnowledgeOldest(knowledge, 0);
            Assert.AreEqual(0, forgetting[0]);
        }

        /// <summary>
        ///     With ForgettingMean = 0
        /// </summary>
        [TestMethod]
        public void InitializeForgettingKnowledgeOldestTest1()
        {
            var knowledgeBits = new float[] {0};
            var knowledge = new AgentKnowledge(1, knowledgeBits, 0);
            _internalCharacteristics.ForgettingMean = 0;
            InitializeModel(true, 0);
            var forgetting = _model.InitializeForgettingKnowledgeOldest(knowledge, 1);
            Assert.AreEqual(0, forgetting[0]);
        }

        /// <summary>
        ///     With ForgettingMean = 1
        /// </summary>
        [TestMethod]
        public void InitializeForgettingKnowledgeOldestTest2()
        {
            var knowledgeBits = new float[] {0};
            var knowledge = new AgentKnowledge(1, knowledgeBits, 0);
            _internalCharacteristics.ForgettingMean = 1;
            InitializeModel(true, 0);
            var forgetting = _model.InitializeForgettingKnowledgeOldest(knowledge, 1);
            Assert.AreEqual(1, forgetting[0]);
        }

        /// <summary>
        ///     With NextForgettingRate == 1 - ForgettingMean = 0.25
        ///     3 bits
        /// </summary>
        [TestMethod]
        public void InitializeForgettingKnowledgeOldestTest3()
        {
            var knowledgeBits = new float[] {0, 0, 0};
            var knowledge = new AgentKnowledge(1, knowledgeBits, 0);
            _internalCharacteristics.ForgettingMean = 0.25F;
            InitializeModel(true, 0);
            knowledge.KnowledgeBits.GetLastTouched()[0] = 0;
            knowledge.KnowledgeBits.GetLastTouched()[1] = 1;
            knowledge.KnowledgeBits.GetLastTouched()[2] = 1;
            var forgetting = _model.InitializeForgettingKnowledgeOldest(knowledge, 1);
            Assert.AreEqual(1, forgetting[0]);
            Assert.AreEqual(0, forgetting[1]);
            Assert.AreEqual(0, forgetting[2]);
        }

        #region forgetting process

        /// <summary>
        ///     ForgettingMean = 0
        /// </summary>
        [TestMethod]
        public void InitializeForgettingProcessTest()
        {
            _internalCharacteristics.ForgettingMean = 0;
            InitializeModel(true, 0);
            _model.InitializeForgettingProcess();
            Assert.AreEqual(0, _model.ForgettingExpertise.Count);
        }

        /// <summary>
        ///     ForgettingMean = 1
        /// </summary>
        [TestMethod]
        public void InitializeForgettingProcessTest1()
        {
            _internalCharacteristics.ForgettingMean = 1;
            InitializeModel(true, 0);
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {0}, 0);
            _expertise.Add(agentKnowledge);
            _networkKnowledges.Add(_agentId, _expertise);
            _model.InitializeForgettingProcess();
            Assert.AreEqual(1, _model.ForgettingExpertise.Count);
        }

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void FinalizeForgettingProcessTest()
        {
            _internalCharacteristics.ForgettingMean = 0;
            _internalCharacteristics.PartialForgettingRate = 1;
            InitializeModel(true, 0);
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {1}, 0);
            _expertise.Add(agentKnowledge);
            _networkKnowledges.Add(_agentId, _expertise);
            _model.InitializeForgettingProcess();
            _model.FinalizeForgettingProcess();
            Assert.AreEqual(1, agentKnowledge.GetKnowledgeSum());
        }

        /// <summary>
        ///     Passing Test
        /// </summary>
        [TestMethod]
        public void FinalizeForgettingProcessTest1()
        {
            _internalCharacteristics.ForgettingMean = 1;
            // ForgettingRate < minimumRemainingLevel
            _internalCharacteristics.PartialForgettingRate = 0.1F;
            _internalCharacteristics.PartialForgetting = true;
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {1}, 0);
            _expertise.Add(agentKnowledge);
            _networkKnowledges.Add(_agentId, _expertise);
            InitializeModel(true, 0);
            _model.InitializeForgettingProcess();
            _model.FinalizeForgettingProcess();
            Assert.AreEqual(0.9F, agentKnowledge.GetKnowledgeSum());
        }

        /// <summary>
        ///     No probability to forget a bit of knowledge
        ///     ForgettingSelectingMode.Oldest
        /// </summary>
        [TestMethod]
        public void SetForgettingKnowledgeTest()
        {
            var knowledgeBits = new float[] {1};
            var knowledge = new AgentKnowledge(1, knowledgeBits, 0);
            _internalCharacteristics.ForgettingMean = 0;
            InitializeModel(true, 0);
            var forgetting = _model.InitializeForgettingKnowledge(knowledge);
            Assert.AreEqual(0, forgetting.GetKnowledgeBit(0));
        }

        /// <summary>
        ///     Full probability to forget a bit of knowledge
        ///     ForgettingSelectingMode.Oldest
        /// </summary>
        [TestMethod]
        public void SetForgettingKnowledgeTest1()
        {
            var knowledgeBits = new float[] {1};
            var knowledge = new AgentKnowledge(1, knowledgeBits, 0);
            _internalCharacteristics.ForgettingMean = 1;
            InitializeModel(true, 0);
            var forgetting = _model.InitializeForgettingKnowledge(knowledge);
            Assert.AreEqual(_internalCharacteristics.PartialForgettingRate, forgetting.GetKnowledgeBit(0));
        }

        /// <summary>
        ///     No probability to forget a bit of knowledge
        ///     ForgettingSelectingMode.Random
        /// </summary>
        [TestMethod]
        public void SetForgettingKnowledgeTest2()
        {
            var knowledgeBits = new float[] {1};
            var knowledge = new AgentKnowledge(1, knowledgeBits, 0);
            _internalCharacteristics.ForgettingMean = 0;
            _internalCharacteristics.ForgettingSelectingMode = ForgettingSelectingMode.Random;
            InitializeModel(true, 0);
            var forgetting = _model.InitializeForgettingKnowledge(knowledge);
            Assert.AreEqual(0, forgetting.GetKnowledgeBit(0));
        }

        /// <summary>
        ///     Full probability to forget a bit of knowledge
        ///     ForgettingSelectingMode.Random
        ///     PartialForgetting
        /// </summary>
        [TestMethod]
        public void SetForgettingKnowledgeTest3()
        {
            var knowledgeBits = new float[] {1};
            var knowledge = new AgentKnowledge(1, knowledgeBits, 0);
            _internalCharacteristics.ForgettingMean = 1;
            _internalCharacteristics.ForgettingSelectingMode = ForgettingSelectingMode.Random;
            _internalCharacteristics.PartialForgetting = true;
            InitializeModel(true, 0);
            var forgetting = _model.InitializeForgettingKnowledge(knowledge);
            Assert.AreEqual(_internalCharacteristics.PartialForgettingRate, forgetting.GetKnowledgeBit(0));
        }

        /// <summary>
        ///     Full probability to forget a bit of knowledge
        ///     ForgettingSelectingMode.Random
        ///     No PartialForgetting
        /// </summary>
        [TestMethod]
        public void SetForgettingKnowledgeTest4()
        {
            var knowledgeBits = new float[] {1};
            var knowledge = new AgentKnowledge(1, knowledgeBits, 0);
            _internalCharacteristics.ForgettingMean = 1;
            _internalCharacteristics.ForgettingSelectingMode = ForgettingSelectingMode.Random;
            _internalCharacteristics.PartialForgetting = false;
            InitializeModel(true, 0);
            var forgetting = _model.InitializeForgettingKnowledge(knowledge);
            Assert.AreEqual(1, forgetting.GetKnowledgeBit(0));
        }

        /// <summary>
        ///     Updating a knowledgeId non identified today
        /// </summary>
        [TestMethod]
        public void UpdateForgettingKnowledgeTest()
        {
            var forgettingBits = new float[] {1};
            var forgetting = new AgentKnowledge(1, forgettingBits, 0);
            InitializeModel(true, 0);
            _model.ForgettingExpertise.Add(forgetting);
            // working on the index 0 today
            var workingBits = new byte[] {0};
            _model.UpdateForgettingProcess(2, workingBits);
            // ForgettingBits should not be updated because the KnowledgeId is not the same
            Assert.AreEqual(1, forgetting.GetKnowledgeBit(0));
        }

        /// <summary>
        ///     updating a knowledgeId identified today
        /// </summary>
        [TestMethod]
        public void UpdateForgettingKnowledgeTest1()
        {
            var forgettingBits = new float[] {1};
            var forgetting = new AgentKnowledge(1, forgettingBits, 0);
            InitializeModel(true, 0);
            _model.ForgettingExpertise.Add(forgetting);
            // working on the index 0 today
            var workingBits = new byte[] {0};
            _model.UpdateForgettingProcess(1, workingBits);
            // ForgettingBits should be set to 0
            Assert.AreEqual(0, forgetting.GetKnowledgeBit(0));
        }

        [TestMethod]
        public void FinalizeForgettingKnowledgeTest()
        {
            var knowledgeBits = new float[] {1};
            var knowledge = new AgentKnowledge(1, knowledgeBits, 0);
            InitializeModel(true, 0);
            _networkKnowledges.GetAgentExpertise(_agentId).Add(knowledge);
            // ForgettingBits value > minimumRemainingLevel 
            var forgettingBits = new[] {0.1F};
            var forgetting = new AgentKnowledge(1, forgettingBits, 0);
            _model.FinalizeForgettingKnowledge(forgetting);
            //Knowledge has been forgotten
            Assert.AreEqual(0.9F, knowledge.GetKnowledgeBit(0));
        }

        #endregion
    }
}