#region Licence

// Description: Symu - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Repository.Networks.Link;

#endregion


namespace SymuTests.Repository.Networks.Link
{
    [TestClass]
    public class NetworkLinksTests
    {
        private readonly AgentId _agentId1 = new AgentId(2, 2);
        private readonly AgentId _agentId2 = new AgentId(3, 2);
        private readonly AgentId _agentId3 = new AgentId(4, 2);
        private readonly NetworkLinks _links = new NetworkLinks();


        [TestMethod]
        public void RemoveAgentTest()
        {
            _links.AddLink(_agentId1, _agentId2);
            _links.AddLink(_agentId3, _agentId1);
            _links.RemoveAgent(_agentId1);
            Assert.IsFalse(_links.Any());
        }

        [TestMethod]
        public void AnyTest()
        {
            Assert.IsFalse(_links.Any());
            _links.AddLink(_agentId1, _agentId2);
            Assert.IsTrue(_links.Any());
        }

        [TestMethod]
        public void ClearTest()
        {
            _links.AddLink(_agentId1, _agentId2);
            _links.AddLink(_agentId3, _agentId1);
            _links.Clear();
            Assert.IsFalse(_links.Any());
        }

        [TestMethod]
        public void DeactivateTeammatesLinkTest()
        {
            _links.AddLink(_agentId1, _agentId2);
            var link = _links[0];
            // Active link
            Assert.IsTrue(link.IsActive);
            // Deactivate
            _links.DeactivateLink(_agentId1, _agentId2);
            Assert.IsFalse(link.IsActive);
            Assert.IsTrue(link.IsPassive);
        }

        [TestMethod]
        public void HasActiveLinkTest()
        {
            _links.AddLink(_agentId1, _agentId2);
            var link = _links[0];
            Assert.IsTrue(link.HasActiveLink(_agentId1, _agentId2));
            Assert.IsFalse(link.HasActiveLink(_agentId1, _agentId3));
        }

        [TestMethod]
        public void HasPassiveLinkTest()
        {
            _links.AddLink(_agentId1, _agentId2);
            var link = _links[0];
            link.Deactivate();
            Assert.IsTrue(link.HasPassiveLink(_agentId1, _agentId2));
            Assert.IsFalse(link.HasPassiveLink(_agentId1, _agentId3));
        }

        [TestMethod]
        public void GetActiveLinksTest()
        {
            Assert.AreEqual(0, new List<AgentId>(_links.GetActiveLinks(_agentId1)).Count);
            _links.AddLink(_agentId1, _agentId2);
            _links.AddLink(_agentId3, _agentId1);
            var teammateId4 = new AgentId(5, 2);
            _links.AddLink(_agentId1, teammateId4);
            Assert.AreEqual(3, new List<AgentId>(_links.GetActiveLinks(_agentId1)).Count);

            // Distinct test
            _links.AddLink(_agentId1, _agentId2);
            Assert.AreEqual(3, new List<AgentId>(_links.GetActiveLinks(_agentId1)).Count);
        }

        [TestMethod]
        public void TeammateExistsTest()
        {
            _links.AddLink(_agentId1, _agentId2);
            Assert.IsTrue(_links.Exists(_agentId1, _agentId2));
            Assert.IsTrue(_links.Exists(_agentId2, _agentId1));
        }

        [TestMethod]
        public void ExistsTest()
        {
            var link = new NetworkLink(_agentId1, _agentId2);
            Assert.IsFalse(_links.Exists(link));
            _links.List.Add(link);
            Assert.IsTrue(_links.Exists(link));
        }

        [TestMethod]
        public void AddLinkTest()
        {
            var link = new NetworkLink(_agentId1, _agentId2);
            _links.AddLink(_agentId1, _agentId2);
            Assert.IsTrue(_links.Exists(link));
            // Deactivate test
            link.Deactivate();
            _links.AddLink(_agentId1, _agentId2);
            Assert.AreEqual(1, _links.List.Count);
            Assert.IsTrue(_links[0].IsActive);
        }

        /// <summary>
        ///     Empty list
        /// </summary>
        [TestMethod]
        public void AddLinksTest()
        {
            var agents = new List<AgentId>();
            _links.AddLinks(agents);
            Assert.AreEqual(0, _links.Count);
        }

        /// <summary>
        ///     Empty list
        /// </summary>
        [TestMethod]
        public void AddLinksTest1()
        {
            var agents = new List<AgentId> {_agentId1, _agentId2, _agentId3};
            _links.AddLinks(agents);
            Assert.AreEqual(3, _links.Count);
            for (var i = 0; i < 3; i++)
            {
                Assert.AreEqual(1, _links[i].Count);
            }
        }

        /// <summary>
        ///     No link
        /// </summary>
        [TestMethod]
        public void CountLinksTest()
        {
            Assert.AreEqual(0, _links.CountLinks(_agentId1, _agentId2));
            _links.AddLink(_agentId1, _agentId2);
            Assert.AreEqual(1, _links.CountLinks(_agentId1, _agentId2));
            _links.AddLink(_agentId2, _agentId1);
            Assert.AreEqual(2, _links.CountLinks(_agentId1, _agentId2));
        }

        [TestMethod]
        public void NormalizedCountLinksTest()
        {
            Assert.AreEqual(0, _links.NormalizedCountLinks(_agentId1, _agentId2));
            _links.AddLink(_agentId1, _agentId2);
            _links.SetMaxLinksCount();
            Assert.AreEqual(1, _links.NormalizedCountLinks(_agentId1, _agentId2));
            _links.AddLink(_agentId2, _agentId1);
            _links.SetMaxLinksCount();
            Assert.AreEqual(1, _links.NormalizedCountLinks(_agentId1, _agentId2));
        }
    }
}