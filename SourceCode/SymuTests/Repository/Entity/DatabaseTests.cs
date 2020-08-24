#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Classes.Organization;
using Symu.Common.Interfaces.Entity;
using Symu.Messaging.Templates;
using Symu.Repository.Entity;
using Symu.Repository.Networks;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace SymuTests.Repository.Entity
{
    [TestClass]
    public class DatabaseTests
    {
        private readonly UId _knowledgeId = new UId(1);
        private readonly float[] _floats1 = {1, 1};
        private Bits _bits1;
        private Database _database;

        [TestInitialize]
        public void Initialize()
        {
            var models = new OrganizationModels();
            var network = new MetaNetwork(models.InteractionSphere, models.ImpactOfBeliefOnTask);

            CommunicationTemplate communication = new EmailTemplate();
            var entity = new DatabaseEntity(new UId(1), communication);
            _database = new Database(entity, models, network.Knowledge);
            _bits1 = new Bits(_floats1, 0);
        }

        [TestMethod]
        public void InitializeKnowledgeTest()
        {
            _database.InitializeKnowledge(_knowledgeId, 1, 0);
            var agentKnowledge = _database.GetKnowledge(_knowledgeId);
            Assert.IsNotNull(agentKnowledge);
        }

        /// <summary>
        ///     With max Rate Learnable = 1
        /// </summary>
        [TestMethod]
        public void StoreKnowledgeTest()
        {
            _database.StoreKnowledge(_knowledgeId, _bits1, 1, 0);
            var agentKnowledge = _database.GetKnowledge(_knowledgeId);
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
            _database.StoreKnowledge(_knowledgeId, _bits1, 0, 0);
            var agentKnowledge = _database.GetKnowledge(_knowledgeId);
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
            Assert.IsFalse(_database.SearchKnowledge(_knowledgeId, 0, 0));
            _database.StoreKnowledge(_knowledgeId, _bits1, 1, 0);
            Assert.IsTrue(_database.SearchKnowledge(_knowledgeId, 0, 0));
        }

        /// <summary>
        ///     With minKnowledgeBit = 1
        /// </summary>
        [TestMethod]
        public void SearchKnowledgeTest1()
        {
            Assert.IsFalse(_database.SearchKnowledge(_knowledgeId, 0, 0));
            _database.StoreKnowledge(_knowledgeId, _bits1, 0.5F, 0);
            Assert.IsFalse(_database.SearchKnowledge(_knowledgeId, 0, 0.5F));
            Assert.IsTrue(_database.SearchKnowledge(_knowledgeId, 0, 0.4F));
        }

        /// <summary>
        ///     With TimeToLive = -1 / non forgetting
        /// </summary>
        [TestMethod]
        public void ForgettingProcessTest()
        {
            _database.StoreKnowledge(_knowledgeId, _bits1, 1, 0);
            _database.ForgettingProcess(100);
            Assert.IsTrue(_database.SearchKnowledge(_knowledgeId, 0, 0));
        }

        /// <summary>
        ///     With TimeToLive = 1 / forgetting
        /// </summary>
        [TestMethod]
        public void ForgettingProcessTest1()
        {
            _database.Entity.CognitiveArchitecture.InternalCharacteristics.TimeToLive = 1;
            _database.StoreKnowledge(_knowledgeId, _bits1, 1, 0);
            _database.ForgettingProcess(2);
            Assert.IsFalse(_database.SearchKnowledge(_knowledgeId, 0, 0));
        }
    }
}