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
using Symu.Common.Interfaces;
using Symu.Repository.Edges;
using Symu.Repository.Entities;
using SymuTests.Helpers;

#endregion

namespace SymuTests.Classes.Agents.Models.CognitiveModels
{
    [TestClass]
    public class ForgettingModelTests : BaseTestClass
    {
        private readonly AgentId _agentId = new AgentId(1, 1);
        private readonly ForgettingModelEntity _forgetting = new ForgettingModelEntity();
        private readonly TaskKnowledgeBits _taskBits = new TaskKnowledgeBits();
        private CognitiveArchitecture _cognitiveArchitecture;
        private ForgettingModel _forgettingModel;
        private InternalCharacteristics _internalCharacteristics;
        private Knowledge _knowledge;

        [TestInitialize]
        public void Initialize()
        {
            _knowledge = new Knowledge(Network, MainOrganization.Models, "1", 1);
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
            _forgettingModel = new ForgettingModel(_agentId, Network.ActorKnowledge, _cognitiveArchitecture,
                _forgetting, true, randomLevel);
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


        /// <summary>
        ///     Minimum Knowledge level = 0
        ///     above min range
        /// </summary>
        [TestMethod]
        public void ForgetTest()
        {
            InitializeModel(true, 0);
            var actorKnowledge = new ActorKnowledge(_agentId, _knowledge.EntityId, new float[] {1, 1}, 0, -1);
            var realForget = _forgettingModel.AgentKnowledgeForget(actorKnowledge, 0, 1, 0);
            Assert.AreEqual(0, actorKnowledge.GetKnowledgeBit(0));
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
            var actorKnowledge = new ActorKnowledge(_agentId, _knowledge.EntityId, new float[] {1, 1}, 0, -1);
            var realForget = _forgettingModel.AgentKnowledgeForget(actorKnowledge, 0, 2, 0);
            Assert.AreEqual(0, actorKnowledge.GetKnowledgeBit(0));
            Assert.AreEqual(-1, realForget);
        }

        /// <summary>
        ///     Minimum Knowledge level = 1
        /// </summary>
        [TestMethod]
        public void ForgetTest2()
        {
            InitializeModel(true, 0);
            var actorKnowledge = new ActorKnowledge(_agentId, _knowledge.EntityId, new float[] {1, 1}, 0, -1)
                {MinimumKnowledge = 1};
            var realForget = _forgettingModel.AgentKnowledgeForget(actorKnowledge, 0, 1, 0);
            Assert.AreEqual(1, actorKnowledge.GetKnowledgeBit(0));
            Assert.AreEqual(0, realForget);
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
            ActorKnowledge.CreateInstance(Network.ActorKnowledge, _agentId, _knowledge.EntityId, new float[] {0}, 0, -1);
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
            var actorKnowledge = new ActorKnowledge(Network.ActorKnowledge, _agentId, _knowledge.EntityId, new float[] {1}, 0, -1);
            _forgettingModel.InitializeForgettingProcess();
            _forgettingModel.FinalizeForgettingProcess(0);
            Assert.AreEqual(1, actorKnowledge.GetKnowledgeSum());
        }

        /// <summary>
        ///     Passing Test
        /// </summary>
        [TestMethod]
        public void FinalizeForgettingProcessTest1()
        {
            var actorKnowledge = new ActorKnowledge(Network.ActorKnowledge, _agentId, _knowledge.EntityId, new float[] {1}, 0, -1);
            InitializeModel(true, 0);
            _forgettingModel.InternalCharacteristics.ForgettingMean = 1;
            // ForgettingRate < minimumRemainingLevel
            _forgettingModel.InternalCharacteristics.PartialForgettingRate = 0.1F;
            _forgettingModel.InternalCharacteristics.PartialForgetting = true;
            _forgettingModel.InitializeForgettingProcess();
            _forgettingModel.FinalizeForgettingProcess(0);
            Assert.AreEqual(0.9F, actorKnowledge.GetKnowledgeSum());
        }

        /// <summary>
        ///     No probability to forget a bit of knowledge
        ///     ForgettingSelectingMode.Oldest
        /// </summary>
        [TestMethod]
        public void SetForgettingKnowledgeTest()
        {
            var knowledgeBits = new float[] {1};
            var actorKnowledge = new ActorKnowledge(_agentId, _knowledge.EntityId, knowledgeBits, 0, -1);
            InitializeModel(true, 0);
            _forgettingModel.InternalCharacteristics.ForgettingMean = 0;
            var forgetting = _forgettingModel.InitializeForgettingKnowledge(actorKnowledge);
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
            var actorKnowledge = new ActorKnowledge(_agentId, _knowledge.EntityId, knowledgeBits, 0, -1);
            InitializeModel(true, 0);
            _forgettingModel.InternalCharacteristics.ForgettingMean = 1;
            var forgetting = _forgettingModel.InitializeForgettingKnowledge(actorKnowledge);
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
            var actorKnowledge = new ActorKnowledge(_agentId, _knowledge.EntityId, knowledgeBits, 0, -1);
            InitializeModel(true, 0);
            _forgettingModel.InternalCharacteristics.ForgettingMean = 0;
            _forgettingModel.InternalCharacteristics.ForgettingSelectingMode = ForgettingSelectingMode.Random;
            var forgetting = _forgettingModel.InitializeForgettingKnowledge(actorKnowledge);
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
            var actorKnowledge = new ActorKnowledge(_agentId, _knowledge.EntityId, knowledgeBits, 0, -1);
            InitializeModel(true, 0);
            _forgettingModel.InternalCharacteristics.ForgettingMean = 1;
            _forgettingModel.InternalCharacteristics.ForgettingSelectingMode = ForgettingSelectingMode.Random;
            _forgettingModel.InternalCharacteristics.PartialForgetting = true;
            var forgetting = _forgettingModel.InitializeForgettingKnowledge(actorKnowledge);
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
            var actorKnowledge = new ActorKnowledge(_agentId, _knowledge.EntityId, knowledgeBits, 0, -1);
            InitializeModel(true, 0);
            _forgettingModel.InternalCharacteristics.ForgettingMean = 1;
            _forgettingModel.InternalCharacteristics.ForgettingSelectingMode = ForgettingSelectingMode.Random;
            _forgettingModel.InternalCharacteristics.PartialForgetting = false;
            var forgetting = _forgettingModel.InitializeForgettingKnowledge(actorKnowledge);
            Assert.AreEqual(1, forgetting.GetKnowledgeBit(0));
        }

        /// <summary>
        ///     Updating a knowledgeId non identified today
        /// </summary>
        [TestMethod]
        public void UpdateForgettingKnowledgeTest()
        {
            var forgettingBits = new float[] {1};
            var forgetting = new ActorKnowledge(_agentId, _knowledge.EntityId, forgettingBits, 0, -1);
            InitializeModel(true, 0);
            _forgettingModel.ForgettingExpertise.Add(forgetting);
            // working on the index 0 today
            var workingBits = new byte[] {0};
            _forgettingModel.UpdateForgettingProcess(new AgentId(2, 1), workingBits);
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
            var forgetting = new ActorKnowledge(_agentId, _knowledge.EntityId, forgettingBits, 0, -1);
            InitializeModel(true, 0);
            _forgettingModel.ForgettingExpertise.Add(forgetting);
            // working on the index 0 today
            var workingBits = new byte[] {0};
            _forgettingModel.UpdateForgettingProcess(_knowledge.EntityId, workingBits);
            // ForgettingBits should be set to 0
            Assert.AreEqual(0, forgetting.GetKnowledgeBit(0));
        }

        [TestMethod]
        public void FinalizeForgettingKnowledgeTest()
        {
            var actorKnowledge = new ActorKnowledge(Network.ActorKnowledge, _agentId, _knowledge.EntityId, new float[] {1}, 0, -1);
            InitializeModel(true, 0);
            // ForgettingBits value > minimumRemainingLevel 
            var forgettingBits = new[] {0.1F};
            var forgetting = new ActorKnowledge(_agentId, _knowledge.EntityId, forgettingBits, 0, -1);
            _forgettingModel.FinalizeForgettingKnowledge(forgetting, 0);
            // Knowledge has been forgotten
            Assert.AreEqual(0.9F, actorKnowledge.GetKnowledgeBit(0));
        }

        #endregion
    }
}