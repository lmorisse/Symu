#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Agent.Models.CognitiveArchitecture;
using SymuEngine.Repository.Networks.Databases;
using SymuEngine.Repository.Networks.Knowledges;

#endregion

namespace SymuEngineTests.Repository.Networks.Databases
{
    [TestClass]
    public class DatabaseTests
    {
        private const ushort KnowledgeId = 1;
        private readonly float[] _floats1 = {1, 1};
        private readonly TasksAndPerformance _tasks = new TasksAndPerformance();
        private Bits _bits1;
        private Database _database;

        [TestInitialize]
        public void Initialize()
        {
            _tasks.LearningRate = 1;
            _tasks.LearningModel.On = true;
            _tasks.LearningModel.RateOfAgentsOn = 1;
            _bits1 = new Bits(_floats1, 0);
            _database = new Database(1, _tasks, -1);
        }

        [TestMethod]
        public void InitializeKnowledgeTest()
        {
            _database.InitializeKnowledge(KnowledgeId, 1, 0); 
            var agentKnowledge = _database.GetKnowledge(KnowledgeId);
            Assert.IsNotNull(agentKnowledge);

        }

        /// <summary>
        ///     With max Rate Learnable = 1
        /// </summary>
        [TestMethod]
        public void StoreKnowledgeTest()
        {
            _database.StoreKnowledge(KnowledgeId, _bits1, 1, 0);
            var agentKnowledge = _database.GetKnowledge(KnowledgeId);
            Assert.IsNotNull(agentKnowledge);
            Assert.AreEqual(1, agentKnowledge.GetKnowledgeBit(0));
            Assert.AreEqual(1, agentKnowledge.GetKnowledgeBit(1));
        }

        /// <summary>
        ///     With max Rate Learnable = 0
        /// </summary>
        [TestMethod]
        public void StoreKnowledge1Test()
        {
            _database.StoreKnowledge(KnowledgeId, _bits1, 0, 0);
            var agentKnowledge = _database.GetKnowledge(KnowledgeId);
            Assert.IsNotNull(agentKnowledge);
            Assert.AreEqual(0, agentKnowledge.GetKnowledgeBit(0));
            Assert.AreEqual(0, agentKnowledge.GetKnowledgeBit(1));
        }

        /// <summary>
        ///     With minKnowledgeBit = 0
        /// </summary>
        [TestMethod]
        public void SearchKnowledgeTest()
        {
            Assert.IsFalse(_database.SearchKnowledge(KnowledgeId, 0, 0));
            _database.StoreKnowledge(KnowledgeId, _bits1, 1, 0);
            Assert.IsTrue(_database.SearchKnowledge(KnowledgeId, 0, 0));
        }

        /// <summary>
        ///     With minKnowledgeBit = 1
        /// </summary>
        [TestMethod]
        public void SearchKnowledgeTest1()
        {
            Assert.IsFalse(_database.SearchKnowledge(KnowledgeId, 0, 0));
            _database.StoreKnowledge(KnowledgeId, _bits1, 0.5F, 0);
            Assert.IsFalse(_database.SearchKnowledge(KnowledgeId, 0, 0.5F));
            Assert.IsTrue(_database.SearchKnowledge(KnowledgeId, 0, 0.4F));
        }

        /// <summary>
        ///     With TimeToLive = -1 / non forgetting
        /// </summary>
        [TestMethod]
        public void ForgettingProcessTest()
        {
            _database.StoreKnowledge(KnowledgeId, _bits1, 1, 0);
            _database.ForgettingProcess(100);
            Assert.IsTrue(_database.SearchKnowledge(KnowledgeId, 0, 0));
        }

        /// <summary>
        ///     With TimeToLive = 1 / forgetting
        /// </summary>
        [TestMethod]
        public void ForgettingProcessTest1()
        {
            _database = new Database(1, _tasks, 1);
            _database.StoreKnowledge(KnowledgeId, _bits1, 1, 0);
            _database.ForgettingProcess(2);
            Assert.IsFalse(_database.SearchKnowledge(KnowledgeId, 0, 0));
        }
    }
}