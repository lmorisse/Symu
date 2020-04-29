#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Agent.Models.CognitiveArchitecture.Knowledge;
using SymuEngine.Repository.Networks.Belief.Repository;

#endregion

namespace SymuEngineTests.Repository.Networks.Belief.Repository
{
    [TestClass]
    public class BeliefsTests
    {
        private readonly Beliefs _beliefs = new Beliefs();
        private SymuEngine.Repository.Networks.Belief.Repository.Belief _belief;

        [TestInitialize]
        public void Initialize()
        {
            var model = new KnowledgeModel();
            _belief = new SymuEngine.Repository.Networks.Belief.Repository.Belief(1, 1, model);
        }

        [TestMethod]
        public void ClearTest()
        {
            _beliefs.Add(_belief);
            _beliefs.Clear();
            Assert.IsFalse(_beliefs.Any());
        }

        [TestMethod]
        public void AddTest()
        {
            Assert.IsFalse(_beliefs.Any());
            _beliefs.Add(_belief);
            Assert.IsTrue(_beliefs.Any());
        }

        [TestMethod]
        public void ContainsTest()
        {
            Assert.IsFalse(_beliefs.Contains(_belief));
            _beliefs.Add(_belief);
            Assert.IsTrue(_beliefs.Contains(_belief));
        }

        [TestMethod]
        public void GetBeliefTest()
        {
            Assert.IsNull(_beliefs.GetBelief(_belief.Id));
            _beliefs.Add(_belief);
            Assert.IsNotNull(_beliefs.GetBelief(_belief.Id));
        }

        [TestMethod]
        public void ExistsTest()
        {
            Assert.IsFalse(_beliefs.Exists(_belief.Id));
            _beliefs.Add(_belief);
            Assert.IsTrue(_beliefs.Exists(_belief.Id));
        }
    }
}