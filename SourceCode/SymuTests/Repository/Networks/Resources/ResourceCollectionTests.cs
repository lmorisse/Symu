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
using Symu.Classes.Organization;
using Symu.Common.Interfaces.Entity;
using Symu.Repository.Entity;
using Symu.Repository.Networks;
using Symu.Repository.Networks.Resources;

#endregion

namespace SymuTests.Repository.Networks.Resources
{
    [TestClass]
    public class ResourceCollectionTests
    {
        private readonly ResourceCollection _collection =
            new ResourceCollection();

        private Database _database;

        [TestInitialize]
        public void Initialize()
        {
            var models = new OrganizationModels();
            var network = new MetaNetwork(models.InteractionSphere, models.ImpactOfBeliefOnTask);
            var cognitive = new CognitiveArchitecture();
            var databaseEntity = new DatabaseEntity(new UId(1), cognitive);
            _database = new Database(databaseEntity, models, network.Knowledge);
        }

        [TestMethod]
        public void AddTest()
        {
            Assert.IsFalse(_collection.Exists(_database.Entity.Id));
            _collection.Add(_database);
            Assert.IsTrue(_collection.Exists(_database.Entity.Id));
            // Duplicate
            _collection.Add(_database);
            Assert.AreEqual(1, _collection.List.Count);
        }

        [TestMethod]
        public void ContainsTest()
        {
            Assert.IsFalse(_collection.Contains(_database));
            _collection.Add(_database);
            Assert.IsTrue(_collection.Contains(_database));
        }

        [TestMethod]
        public void GetDatabaseTest()
        {
            Assert.IsNull(_collection.Get(_database.Entity.Id));
            _collection.Add(_database);
            Assert.IsNotNull(_collection.Get(_database.Entity.Id));
            Assert.AreEqual(_database, _collection.Get(_database.Entity.Id));
        }

        [TestMethod]
        public void ClearTest()
        {
            _collection.Add(_database);
            _collection.Clear();
            Assert.IsFalse(_collection.Contains(_database));
        }
    }
}