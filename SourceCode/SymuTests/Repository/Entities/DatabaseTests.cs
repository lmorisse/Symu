#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Messaging.Templates;
using Symu.OrgMod.Entities;
using Symu.Repository.Entities;
using SymuTests.Helpers;

#endregion

namespace SymuTests.Repository.Entities
{
    [TestClass]
    public class DatabaseTests : BaseTestClass
    {
        private readonly CommunicationTemplate _communication = new EmailTemplate();
        private readonly float[] _floats1 = {1, 1};
        private Bits _bits1;
        private Database _database;
        private IKnowledge _knowledge;

        [TestInitialize]
        public void Initialize()
        {
            MainOrganization.Models.SetOn(1);
            _knowledge = new Knowledge(MainOrganization.MetaNetwork, MainOrganization.Models, "1",
                (byte) _floats1.Length);
            _database = new Database(MainOrganization.MetaNetwork, MainOrganization.Models, _communication, 0);
            _database.CognitiveArchitecture.InternalCharacteristics.TimeToLive = 10;
            _bits1 = new Bits(_floats1, 0);
        }

        [TestMethod]
        public void CloneTest()
        {
            var clone = (Database) _database.Clone();
            Assert.AreEqual(_database.TimeToLive, clone.TimeToLive);
        }

        [TestMethod]
        public void InitializeKnowledgeTest()
        {
            _database.InitializeKnowledge(_knowledge.EntityId, 0);
            var actorKnowledge = _database.GetKnowledge(_knowledge.EntityId);
            Assert.IsNotNull(actorKnowledge);
        }

        /// <summary>
        ///     With max Rate Learnable = 1
        /// </summary>
        [TestMethod]
        public void StoreKnowledgeTest()
        {
            _database.StoreKnowledge(_knowledge.EntityId, _bits1, 1, 0);
            var actorKnowledge = _database.GetKnowledge(_knowledge.EntityId);
            Assert.IsNotNull(actorKnowledge);
            Assert.AreEqual(1, actorKnowledge.GetKnowledgeBit(0));
            Assert.AreEqual(1, actorKnowledge.GetKnowledgeBit(1));
        }

        /// <summary>
        ///     With max Rate Learnable = 0
        /// </summary>
        [TestMethod]
        public void StoreKnowledge1Test()
        {
            _database.StoreKnowledge(_knowledge.EntityId, _bits1, 0, 0);
            var actorKnowledge = _database.GetKnowledge(_knowledge.EntityId);
            Assert.IsNotNull(actorKnowledge);
            Assert.AreEqual(0, actorKnowledge.GetKnowledgeBit(0));
            Assert.AreEqual(0, actorKnowledge.GetKnowledgeBit(1));
        }

        /// <summary>
        ///     With minKnowledgeBit = 0
        /// </summary>
        [TestMethod]
        public void SearchKnowledgeTest()
        {
            Assert.IsFalse(_database.SearchKnowledge(_knowledge.EntityId, 0, 0));
            _database.StoreKnowledge(_knowledge.EntityId, _bits1, 1, 0);
            Assert.IsTrue(_database.SearchKnowledge(_knowledge.EntityId, 0, 0));
        }

        /// <summary>
        ///     With minKnowledgeBit = 1
        /// </summary>
        [TestMethod]
        public void SearchKnowledgeTest1()
        {
            Assert.IsFalse(_database.SearchKnowledge(_knowledge.EntityId, 0, 0));
            _database.StoreKnowledge(_knowledge.EntityId, _bits1, 0.5F, 0);
            Assert.IsFalse(_database.SearchKnowledge(_knowledge.EntityId, 0, 0.55F));
            Assert.IsTrue(_database.SearchKnowledge(_knowledge.EntityId, 0, 0.5F));
        }

        /// <summary>
        ///     With TimeToLive = -1 / non forgetting
        /// </summary>
        [TestMethod]
        public void ForgettingProcessTest()
        {
            _database.StoreKnowledge(_knowledge.EntityId, _bits1, 1, 0);
            _database.ForgettingProcess(100);
            Assert.IsTrue(_database.SearchKnowledge(_knowledge.EntityId, 0, 0));
        }

        /// <summary>
        ///     With TimeToLive = -1 / non forgetting
        /// </summary>
        [TestMethod]
        public void ForgettingProcessTest1()
        {
            _database.StoreKnowledge(_knowledge.EntityId, _bits1, 1, 0);
            _database.ForgettingProcess(100);
            Assert.IsTrue(_database.SearchKnowledge(_knowledge.EntityId, 0, 0));
        }

        /// <summary>
        ///     With TimeToLive = 1 / forgetting off
        /// </summary>
        [TestMethod]
        public void ForgettingProcessTest2()
        {
            MainOrganization.Models.Forgetting.On = true;
            _database = new Database(MainOrganization.MetaNetwork, MainOrganization.Models, _communication,
                Environment.RandomLevelValue);
            _database.CognitiveArchitecture.InternalCharacteristics.TimeToLive = 1;
            _database.StoreKnowledge(_knowledge.EntityId, _bits1, 1, 0);
            _database.ForgettingProcess(2);
            Assert.IsTrue(_database.SearchKnowledge(_knowledge.EntityId, 0, 0));
        }
    }
}