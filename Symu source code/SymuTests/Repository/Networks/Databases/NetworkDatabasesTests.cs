#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Classes.Organization;
using Symu.Repository.Networks;
using Symu.Repository.Networks.Databases;

#endregion

namespace SymuTests.Repository.Networks.Databases
{
    [TestClass]
    public class NetworkDatabasesTests
    {
        private readonly AgentId _agentId = new AgentId(1, 1);
        private readonly NetworkDatabases _databases = new NetworkDatabases();
        private Database _database;

        [TestInitialize]
        public void Initialize()
        {

            var models = new OrganizationModels();
            var network = new MetaNetwork(models.InteractionSphere, models.ImpactOfBeliefOnTask); 
            var cognitive = new CognitiveArchitecture();
            var databaseEntity = new DataBaseEntity(_agentId, cognitive);
            _database = new Database(databaseEntity, models, network.Knowledge);
        }

        [TestMethod]
        public void ClearTest()
        {
            _databases.Add(_agentId, _database.Entity.AgentId.Key);
            Assert.IsTrue(_databases.Any());
            _databases.Clear();
            Assert.IsFalse(_databases.Any());
        }

        [TestMethod]
        public void GetDatabaseTest()
        {
            Assert.IsNull(_databases.GetDatabase(_database.Entity.AgentId.Key));
            _databases.AddDatabase(_database);
            Assert.IsNotNull(_databases.GetDatabase(_database.Entity.AgentId.Key));
            Assert.AreEqual(_database, _databases.GetDatabase(_database.Entity.AgentId.Key));
        }

        [TestMethod]
        public void AddDatabaseTest()
        {
            Assert.IsFalse(_databases.Exists(_database.Entity.AgentId.Key));
            _databases.AddDatabase(_database);
            Assert.IsTrue(_databases.Exists(_database.Entity.AgentId.Key));
        }

        [TestMethod]
        public void RemoveAgentTest()
        {
            _databases.Add(_agentId, _database.Entity.AgentId.Key);
            _databases.RemoveAgent(_agentId);
            Assert.IsFalse(_databases.Exists(_agentId, _database.Entity.AgentId.Key));
            Assert.IsFalse(_databases.Any());
        }

        [TestMethod]
        public void AddTest()
        {
            Assert.IsFalse(_databases.Exists(_agentId, _database.Entity.AgentId.Key));
            _databases.Add(_agentId, _database.Entity.AgentId.Key);
            Assert.IsTrue(_databases.Exists(_agentId, _database.Entity.AgentId.Key));
        }

        [TestMethod]
        public void AddTest1()
        {
            Assert.IsFalse(_databases.Exists(_database.Entity.AgentId.Key));
            Assert.IsFalse(_databases.Exists(_agentId, _database.Entity.AgentId.Key));
            _databases.Add(_agentId, _database);
            Assert.IsTrue(_databases.Exists(_database.Entity.AgentId.Key));
            Assert.IsTrue(_databases.Exists(_agentId, _database.Entity.AgentId.Key));
        }

        [TestMethod]
        public void AddAgentIdTest()
        {
            Assert.IsFalse(_databases.Exists(_agentId));
            _databases.AddAgentId(_agentId);
            Assert.IsTrue(_databases.Exists(_agentId));
        }
    }
}