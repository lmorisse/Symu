#region Licence

// Description: Symu - SymuEngineTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Agents;
using SymuEngine.Classes.Agents.Models;
using SymuEngine.Classes.Agents.Models.CognitiveModel;
using SymuEngine.Classes.Organization;
using SymuEngine.Repository.Networks;
using SymuEngine.Repository.Networks.Databases;
using SymuEngine.Repository.Networks.Knowledges;

#endregion

namespace SymuEngineTests.Repository.Networks.Databases
{
    [TestClass]
    public class DatabaseCollectionTests
    {
        private readonly DatabaseCollection _databases =
            new DatabaseCollection();

        private Database _database;

        [TestInitialize]
        public void Initialize()
        {
            var agentId = new AgentId(1, 1);
            var agentTemplates = new AgentTemplates();
            var models = new OrganizationModels();
            var network = new Network(agentTemplates, models);
            var cognitive = new CognitiveArchitecture(network, agentId);
            var databaseEntity = new DataBaseEntity(agentId, cognitive);
            _database = new Database(databaseEntity, models, network.NetworkKnowledges);
        }

        [TestMethod]
        public void AddTest()
        {
            Assert.IsFalse(_databases.Exists(_database.Entity.AgentId.Key));
            _databases.Add(_database);
            Assert.IsTrue(_databases.Exists(_database.Entity.AgentId.Key));
            // Duplicate
            _databases.Add(_database);
            Assert.AreEqual(1, _databases.List.Count);
        }

        [TestMethod]
        public void ContainsTest()
        {
            Assert.IsFalse(_databases.Contains(_database));
            _databases.Add(_database);
            Assert.IsTrue(_databases.Contains(_database));
        }

        [TestMethod]
        public void GetDatabaseTest()
        {
            Assert.IsNull(_databases.GetDatabase(_database.Entity.AgentId.Key));
            _databases.Add(_database);
            Assert.IsNotNull(_databases.GetDatabase(_database.Entity.AgentId.Key));
            Assert.AreEqual(_database, _databases.GetDatabase(_database.Entity.AgentId.Key));
        }

        [TestMethod]
        public void ClearTest()
        {
            _databases.Add(_database);
            _databases.Clear();
            Assert.IsFalse(_databases.Contains(_database));
        }
    }
}