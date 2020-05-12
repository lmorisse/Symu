#region Licence

// Description: Symu - SymuEngineTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Agents.Models.CognitiveArchitecture;
using SymuEngine.Repository.Networks.Databases;

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
            var tasks = new TasksAndPerformance();
            _database = new Database(1, tasks, -1);
        }

        [TestMethod]
        public void AddTest()
        {
            Assert.IsFalse(_databases.Exists(_database.Id));
            _databases.Add(_database);
            Assert.IsTrue(_databases.Exists(_database.Id));
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
            Assert.IsNull(_databases.GetDatabase(_database.Id));
            _databases.Add(_database);
            Assert.IsNotNull(_databases.GetDatabase(_database.Id));
            Assert.AreEqual(_database, _databases.GetDatabase(_database.Id));
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