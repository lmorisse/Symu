#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Classes.Organization;
using Symu.Classes.Task;
using Symu.Common.Classes;
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.DNA;
using Symu.DNA.OneModeNetworks.Knowledge;
using Symu.DNA.TwoModesNetworks.AgentKnowledge;
using Symu.Repository.Entity;

#endregion

namespace SymuTests.Classes.Agents.Models.CognitiveModels
{
    [TestClass]
    public class ForgettingModelTests
    {
        private readonly AgentId _agentId = new AgentId(1, 1);
        private readonly AgentExpertise _expertise = new AgentExpertise();
        private readonly ModelEntity _forgetting = new ModelEntity();
        private readonly Knowledge _knowledge = new Knowledge(1, "1", 1);
        private readonly TaskKnowledgeBits _taskBits = new TaskKnowledgeBits();
        private CognitiveArchitecture _cognitiveArchitecture;
        private ForgettingModel _forgettingModel;
        private InternalCharacteristics _internalCharacteristics;
        private AgentKnowledgeNetwork _knowledgeNetwork;

        [TestInitialize]
        public void Initialize()
        {
            var models = new OrganizationModels();
            var network = new MetaNetwork(models.InteractionSphere);
            _knowledgeNetwork = network.AgentKnowledge;
            _knowledgeNetwork.Add(_agentId, _expertise);

            Assert.AreEqual(_expertise, _knowledgeNetwork.GetAgentExpertise(_agentId));
            _taskBits.SetMandatory(new byte[] {0});
            _taskBits.SetRequired(new byte[] {0});
            _forgetting.On = true;
            _forgetting.RateOfAgentsOn = 1;
            _cognitiveArchitecture = new CognitiveArchitecture();
            _cognitiveArchitecture.KnowledgeAndBeliefs.HasKnowledge = true;
            _cognitiveArchitecture.InternalCharacteristics.ForgettingSelectingMode = ForgettingSelectingMode.Random;
            _internalCharacteristics = _cognitiveArchitecture.InternalCharacteristics;
        }

        public void InitializeModel(bool modelOn, byte randomLevel)
        {
            _cognitiveArchitecture.InternalCharacteristics.CanForget = true;
            _forgetting.On = modelOn;
            _forgetting.RateOfAgentsOn = 1;
            _forgettingModel = new ForgettingModel(_expertise, _cognitiveArchitecture, _forgetting, randomLevel);
            _forgettingModel.InitializeForgettingProcess();
        }

        /// <summary>
        ///     Model off
        /// </summary>
        [TestMethod]
        public void NextRateTest()
        {
            InitializeModel(false, 0);
            Assert.AreEqual(0, _forgettingModel.NextRate());
        }

        /// <summary>
        ///     Model on - Complete forgetting
        /// </summary>
        [TestMethod]
        public void NextRateTest1()
        {
            InitializeModel(true, 0);
            _forgettingModel.InternalCharacteristics.PartialForgetting = false;
            Assert.AreEqual(1, _forgettingModel.NextRate());
        }

        /// <summary>
        ///     Model on - partial forgetting
        /// </summary>
        [TestMethod]
        public void NextRateTest2()
        {
            InitializeModel(true, 0);
            _forgettingModel.InternalCharacteristics.PartialForgetting = true;
            Assert.AreEqual(_internalCharacteristics.PartialForgettingRate, _forgettingModel.NextRate());
        }

        /// <summary>
        ///     Model off
        /// </summary>
        [TestMethod]
        public void NextMeanTest()
        {
            InitializeModel(false, 0);
            Assert.AreEqual(0, _forgettingModel.NextMean());
        }

        /// <summary>
        ///     Model on - no random
        /// </summary>
        [TestMethod]
        public void NextMeanTest1()
        {
            InitializeModel(true, 0);

            Assert.AreEqual(_internalCharacteristics.ForgettingMean, _forgettingModel.NextMean());
        }

        #region forgetting process

        /// <summary>
        ///     ForgettingMean = 0
        /// </summary>
        [TestMethod]
        public void InitializeForgettingProcessTest()
        {
            InitializeModel(true, 0);
            _forgettingModel.InternalCharacteristics.ForgettingMean = 0;
            _forgettingModel.InitializeForgettingProcess();
            Assert.AreEqual(0, _forgettingModel.ForgettingExpertise.Count);
        }

        /// <summary>
        ///     ForgettingMean = 1
        /// </summary>
        [TestMethod]
        public void InitializeForgettingProcessTest1()
        {
            InitializeModel(true, 0);
            _forgettingModel.InternalCharacteristics.ForgettingMean = 1;
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {0}, 0, -1, 0);
            _expertise.Add(agentKnowledge);
            _knowledgeNetwork.Add(_agentId, _expertise);
            _forgettingModel.InitializeForgettingProcess();
            Assert.AreEqual(1, _forgettingModel.ForgettingExpertise.Count);
        }

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void FinalizeForgettingProcessTest()
        {
            InitializeModel(true, 0);
            _forgettingModel.InternalCharacteristics.ForgettingMean = 0;
            _forgettingModel.InternalCharacteristics.PartialForgettingRate = 1;
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {1}, 0, -1, 0);
            _expertise.Add(agentKnowledge);
            _knowledgeNetwork.Add(_agentId, _expertise);
            _forgettingModel.InitializeForgettingProcess();
            _forgettingModel.FinalizeForgettingProcess(0);
            Assert.AreEqual(1, agentKnowledge.GetKnowledgeSum());
        }

        /// <summary>
        ///     Passing Test
        /// </summary>
        [TestMethod]
        public void FinalizeForgettingProcessTest1()
        {
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {1}, 0, -1, 0);
            _expertise.Add(agentKnowledge);
            _knowledgeNetwork.Add(_agentId, _expertise);
            InitializeModel(true, 0);
            _forgettingModel.InternalCharacteristics.ForgettingMean = 1;
            // ForgettingRate < minimumRemainingLevel
            _forgettingModel.InternalCharacteristics.PartialForgettingRate = 0.1F;
            _forgettingModel.InternalCharacteristics.PartialForgetting = true;
            _forgettingModel.InitializeForgettingProcess();
            _forgettingModel.FinalizeForgettingProcess(0);
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
            var knowledge = new AgentKnowledge(1, knowledgeBits, 0, -1, 0);
            InitializeModel(true, 0);
            _forgettingModel.InternalCharacteristics.ForgettingMean = 0;
            var forgetting = _forgettingModel.InitializeForgettingKnowledge(knowledge);
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
            var knowledge = new AgentKnowledge(1, knowledgeBits, 0, -1, 0);
            InitializeModel(true, 0);
            _forgettingModel.InternalCharacteristics.ForgettingMean = 1;
            var forgetting = _forgettingModel.InitializeForgettingKnowledge(knowledge);
            Assert.AreEqual(_forgettingModel.InternalCharacteristics.PartialForgettingRate,
                forgetting.GetKnowledgeBit(0));
        }

        /// <summary>
        ///     No probability to forget a bit of knowledge
        ///     ForgettingSelectingMode.Random
        /// </summary>
        [TestMethod]
        public void SetForgettingKnowledgeTest2()
        {
            var knowledgeBits = new float[] {1};
            var knowledge = new AgentKnowledge(1, knowledgeBits, 0, -1, 0);
            InitializeModel(true, 0);
            _forgettingModel.InternalCharacteristics.ForgettingMean = 0;
            _forgettingModel.InternalCharacteristics.ForgettingSelectingMode = ForgettingSelectingMode.Random;
            var forgetting = _forgettingModel.InitializeForgettingKnowledge(knowledge);
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
            var knowledge = new AgentKnowledge(1, knowledgeBits, 0, -1, 0);
            InitializeModel(true, 0);
            _forgettingModel.InternalCharacteristics.ForgettingMean = 1;
            _forgettingModel.InternalCharacteristics.ForgettingSelectingMode = ForgettingSelectingMode.Random;
            _forgettingModel.InternalCharacteristics.PartialForgetting = true;
            var forgetting = _forgettingModel.InitializeForgettingKnowledge(knowledge);
            Assert.AreEqual(_forgettingModel.InternalCharacteristics.PartialForgettingRate,
                forgetting.GetKnowledgeBit(0));
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
            var knowledge = new AgentKnowledge(1, knowledgeBits, 0, -1, 0);
            InitializeModel(true, 0);
            _forgettingModel.InternalCharacteristics.ForgettingMean = 1;
            _forgettingModel.InternalCharacteristics.ForgettingSelectingMode = ForgettingSelectingMode.Random;
            _forgettingModel.InternalCharacteristics.PartialForgetting = false;
            var forgetting = _forgettingModel.InitializeForgettingKnowledge(knowledge);
            Assert.AreEqual(1, forgetting.GetKnowledgeBit(0));
        }

        /// <summary>
        ///     Updating a knowledgeId non identified today
        /// </summary>
        [TestMethod]
        public void UpdateForgettingKnowledgeTest()
        {
            var forgettingBits = new float[] {1};
            var forgetting = new AgentKnowledge(_knowledge.Id, forgettingBits, 0, -1, 0);
            InitializeModel(true, 0);
            _forgettingModel.ForgettingExpertise.Add(forgetting);
            // working on the index 0 today
            var workingBits = new byte[] {0};
            _forgettingModel.UpdateForgettingProcess(new UId(2), workingBits);
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
            var forgetting = new AgentKnowledge(_knowledge.Id, forgettingBits, 0, -1, 0);
            InitializeModel(true, 0);
            _forgettingModel.ForgettingExpertise.Add(forgetting);
            // working on the index 0 today
            var workingBits = new byte[] {0};
            _forgettingModel.UpdateForgettingProcess(_knowledge.Id, workingBits);
            // ForgettingBits should be set to 0
            Assert.AreEqual(0, forgetting.GetKnowledgeBit(0));
        }

        [TestMethod]
        public void FinalizeForgettingKnowledgeTest()
        {
            var knowledgeBits = new float[] {1};
            var knowledge = new AgentKnowledge(1, knowledgeBits, 0, -1, 0);
            InitializeModel(true, 0);
            _expertise.Add(knowledge);
            //_networkKnowledges.GetAgentExpertise(_agentId).Add(knowledge);
            // ForgettingBits value > minimumRemainingLevel 
            var forgettingBits = new[] {0.1F};
            var forgetting = new AgentKnowledge(1, forgettingBits, 0, -1, 0);
            _forgettingModel.FinalizeForgettingKnowledge(forgetting, 0);
            // Knowledge has been forgotten
            Assert.AreEqual(0.9F, knowledge.GetKnowledgeBit(0));
        }

        #endregion


        /// <summary>
        ///     Minimum Knowledge level = 0
        ///     above min range
        /// </summary>
        [TestMethod]
        public void ForgetTest()
        {
            InitializeModel(true, 0);
            var agentKnowledge1 = new AgentKnowledge(1, new float[] { 1, 1 }, 0, -1, 0);
            var realForget = _forgettingModel.AgentKnowledgeForget(agentKnowledge1, 0, 1, 0);
            Assert.AreEqual(0, agentKnowledge1.GetKnowledgeBit(0));
            Assert.AreEqual(-1, realForget);
        }

        /// <summary>
        ///     Minimum Knowledge level = 0
        ///     below min range
        /// </summary>
        [TestMethod]
        public void ForgetTest1()
        {
            InitializeModel(true, 0);
            var agentKnowledge1 = new AgentKnowledge(1, new float[] { 1, 1 }, 0, -1, 0);
            var realForget = _forgettingModel.AgentKnowledgeForget(agentKnowledge1, 0, 2, 0);
            Assert.AreEqual(0, agentKnowledge1.GetKnowledgeBit(0));
            Assert.AreEqual(-1, realForget);
        }

        /// <summary>
        ///     Minimum Knowledge level = 1
        /// </summary>
        [TestMethod]
        public void ForgetTest2()
        {
            InitializeModel(true, 0);
            var agentKnowledge1 = new AgentKnowledge(1, new float[] {1, 1}, 0, -1, 0) {MinimumKnowledge = 1};
            var realForget = _forgettingModel.AgentKnowledgeForget(agentKnowledge1, 0, 1, 0);
            Assert.AreEqual(1, agentKnowledge1.GetKnowledgeBit(0));
            Assert.AreEqual(0, realForget);
        }
    }
}