#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Agent;
using SymuEngine.Classes.Agent.Models.CognitiveArchitecture;
using SymuEngine.Repository.Networks.Databases;

#endregion

namespace SymuEngineTests.Repository.Networks.Databases
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
            var tasks = new TasksAndPerformance();
            _database = new Database(1, tasks, -1);
        }

        [TestMethod]
        public void ClearTest()
        {
            _databases.Add(_agentId, _database.Id);
            Assert.IsTrue(_databases.Any());
            _databases.Clear();
            Assert.IsFalse(_databases.Any());
        }

        [TestMethod]
        public void GetDatabaseTest()
        {
            Assert.IsNull(_databases.GetDatabase(_database.Id));
            _databases.AddDatabase(_database);
            Assert.IsNotNull(_databases.GetDatabase(_database.Id));
            Assert.AreEqual(_database, _databases.GetDatabase(_database.Id));
        }

        [TestMethod]
        public void AddDatabaseTest()
        {
            Assert.IsFalse(_databases.Exists(_database.Id));
            _databases.AddDatabase(_database);
            Assert.IsTrue(_databases.Exists(_database.Id));
        }

        [TestMethod]
        public void RemoveAgentTest()
        {
            _databases.Add(_agentId, _database.Id);
            _databases.RemoveAgent(_agentId);
            Assert.IsFalse(_databases.Exists(_agentId, _database.Id));
            Assert.IsFalse(_databases.Any());
        }

        [TestMethod]
        public void AddTest()
        {
            Assert.IsFalse(_databases.Exists(_agentId, _database.Id));
            _databases.Add(_agentId, _database.Id);
            Assert.IsTrue(_databases.Exists(_agentId, _database.Id));
        }

        [TestMethod]
        public void AddTest1()
        {
            Assert.IsFalse(_databases.Exists(_database.Id));
            Assert.IsFalse(_databases.Exists(_agentId, _database.Id));
            _databases.Add(_agentId, _database);
            Assert.IsTrue(_databases.Exists(_database.Id));
            Assert.IsTrue(_databases.Exists(_agentId, _database.Id));
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