#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Common.Interfaces.Agent;
using Symu.Repository.Networks.Influences;

#endregion

namespace SymuTests.Repository.Networks.Influences
{
    [TestClass]
    public class NetworkInfluencesTests
    {
        private readonly AgentId _agentId = new AgentId();
        private readonly InfluenceNetwork _network = new InfluenceNetwork();


        [TestMethod]
        public void AddTest()
        {
            Assert.IsFalse(_network.Exists(_agentId));
            _network.Add(_agentId, 1, 1);
            Assert.IsTrue(_network.Exists(_agentId));
            // Duplicate
            _network.Add(_agentId, 1, 1);
            Assert.AreEqual(1, _network.List.Count);
        }

        [TestMethod]
        public void GetInfluentialnessTest()
        {
            Assert.AreEqual(0, _network.GetInfluentialness(_agentId));
            _network.Add(_agentId, 0, 1);
            Assert.AreEqual(1, _network.GetInfluentialness(_agentId));
        }

        [TestMethod]
        public void GetInfluenceTest()
        {
            Assert.IsNull(_network.GetInfluence(_agentId));
            _network.Add(_agentId, 1, 1);
            Assert.IsNotNull(_network.GetInfluence(_agentId));
        }

        [TestMethod]
        public void GetInfluenceabilityTest()
        {
            Assert.AreEqual(0, _network.GetInfluenceability(_agentId));
            _network.Add(_agentId, 1, 0);
            Assert.AreEqual(1, _network.GetInfluenceability(_agentId));
        }

        [TestMethod]
        public void UpdateTest()
        {
            _network.Add(_agentId, 0, 0);
            _network.Update(_agentId, 1, 2);
            var influence = _network.GetInfluence(_agentId);
            Assert.AreEqual(1, influence.Influentialness);
            Assert.AreEqual(2, influence.Influenceability);
        }
    }
}