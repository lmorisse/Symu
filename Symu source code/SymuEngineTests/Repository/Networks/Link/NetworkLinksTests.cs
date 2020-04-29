#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Agent;
using SymuEngine.Repository.Networks.Link;

#endregion

namespace SymuEngineTests.Repository.Networks.Link
{
    [TestClass]
    public class NetworkLinksTests
    {
        private readonly NetworkLinks _links = new NetworkLinks();
        private readonly AgentId _teamId = new AgentId(1, 1);
        private readonly AgentId _teammateId1 = new AgentId(2, 2);
        private readonly AgentId _teammateId2 = new AgentId(3, 2);
        private readonly AgentId _teammateId3 = new AgentId(4, 2);


        [TestMethod]
        public void RemoveAgentTest()
        {
            _links.AddMembers(_teammateId1, _teammateId2, _teamId);
            _links.AddMembers(_teammateId3, _teammateId1, _teamId);
            _links.RemoveAgent(_teammateId1);
            Assert.IsFalse(_links.Any());
        }

        [TestMethod]
        public void AnyTest()
        {
            Assert.IsFalse(_links.Any());
            _links.AddMembers(_teammateId1, _teammateId2, _teamId);
            Assert.IsTrue(_links.Any());
        }

        [TestMethod]
        public void AddSubordinateTest()
        {
            _links.AddSubordinate(_teammateId1, _teammateId2, _teamId);
            var link = _links[0] as CommunicationLink;
            Assert.IsNotNull(link);
            Assert.AreEqual(CommunicationType.ReportTo, link.Communication);
        }

        [TestMethod]
        public void RemoveSubordinateTest()
        {
            _links.AddSubordinate(_teammateId1, _teammateId2, _teamId);
            _links.DeactivateSubordinate(_teammateId1, _teammateId2, _teamId);
            Assert.IsTrue(_links.Any());
            Assert.IsTrue(_links[0].IsPassive);
        }

        [TestMethod]
        public void AddTeammatesTest()
        {
            _links.AddMembers(_teammateId1, _teammateId2, _teamId);
            Assert.AreEqual(2, _links.List.Count);
            var link = _links[0] as CommunicationLink;
            Assert.IsNotNull(link);
            Assert.AreEqual(CommunicationType.CommunicateTo, link.Communication);
            link = _links[1] as CommunicationLink;
            Assert.IsNotNull(link);
            Assert.AreEqual(CommunicationType.CommunicateTo, link.Communication);
        }

        [TestMethod]
        public void ClearTest()
        {
            _links.AddMembers(_teammateId1, _teammateId2, _teamId);
            _links.AddMembers(_teammateId3, _teammateId1, _teamId);
            _links.Clear();
            Assert.IsFalse(_links.Any());
        }

        [TestMethod]
        public void DeactivateTeammatesLinkTest()
        {
            _links.AddMembers(_teammateId1, _teammateId2, _teamId);
            var link = _links[0];
            // Active link
            Assert.AreEqual(NetworkLinkState.Active, link.State);
            Assert.IsTrue(link.IsActive);
            // Deactivate
            _links.DeactivateTeammates(_teammateId1, _teammateId2, _teamId);
            Assert.AreEqual(NetworkLinkState.Passive, link.State);
            Assert.IsTrue(link.IsPassive);
        }

        [TestMethod]
        public void HasActiveLinkTest()
        {
            _links.AddMembers(_teammateId1, _teammateId2, _teamId);
            var link = _links[0];
            Assert.IsTrue(link.HasActiveLink(_teammateId1, _teammateId2));
            Assert.IsFalse(link.HasActiveLink(_teammateId1, _teammateId3));
        }

        [TestMethod]
        public void HasPassiveLinkTest()
        {
            _links.AddMembers(_teammateId1, _teammateId2, _teamId);
            var link = _links[0];
            link.Deactivate();
            Assert.IsTrue(link.HasPassiveLink(_teammateId1, _teammateId2));
            Assert.IsFalse(link.HasPassiveLink(_teammateId1, _teammateId3));
        }

        [TestMethod]
        public void GetActiveLinksTest()
        {
            Assert.AreEqual(0, new List<AgentId>(_links.GetActiveLinks(_teammateId1)).Count);
            _links.AddMembers(_teammateId1, _teammateId2, _teamId);
            _links.AddMembers(_teammateId3, _teammateId1, _teamId);
            var teamId2 = new AgentId(2, 1);
            var teammateId4 = new AgentId(5, 2);
            _links.AddMembers(_teammateId1, teammateId4, teamId2);
            Assert.AreEqual(3, new List<AgentId>(_links.GetActiveLinks(_teammateId1)).Count);

            // Distinct test
            _links.AddMembers(_teammateId1, _teammateId2, teamId2);
            Assert.AreEqual(3, new List<AgentId>(_links.GetActiveLinks(_teammateId1)).Count);
        }

        [TestMethod]
        public void SubordinateExistsTest()
        {
            _links.AddSubordinate(_teammateId1, _teammateId2, _teamId);
            Assert.IsFalse(_links.Exists(_teammateId1, CommunicationType.CommunicateTo, _teammateId2, _teamId));
            Assert.IsFalse(_links.Exists(_teammateId2, CommunicationType.CommunicateTo, _teammateId1, _teamId));
            Assert.IsFalse(_links.Exists(_teammateId1, CommunicationType.ReportTo, _teammateId1, _teamId));
            Assert.IsFalse(_links.Exists(_teammateId2, CommunicationType.ReportTo, _teammateId2, _teamId));
            Assert.IsTrue(_links.Exists(_teammateId1, CommunicationType.ReportTo, _teammateId2, _teamId));
        }

        [TestMethod]
        public void TeammateExistsTest()
        {
            _links.AddMembers(_teammateId1, _teammateId2, _teamId);
            Assert.IsTrue(_links.Exists(_teammateId1, CommunicationType.CommunicateTo, _teammateId2, _teamId));
            Assert.IsFalse(_links.Exists(_teammateId1, CommunicationType.ReportTo, _teammateId2, _teamId));
        }

        [TestMethod]
        public void GetActiveLinksByGroupClassKeyTest()
        {
            _links.AddMembers(_teammateId1, _teammateId2, _teamId);
            Assert.AreEqual(1, new List<AgentId>(_links.GetActiveLinks(_teammateId1, _teammateId2.ClassKey)).Count);
            var teammateId4 = new AgentId(5, 3);
            _links.AddMembers(_teammateId1, teammateId4, _teamId);
            Assert.AreEqual(1, new List<AgentId>(_links.GetActiveLinks(_teammateId1, _teammateId2.ClassKey)).Count);
            Assert.AreEqual(1, new List<AgentId>(_links.GetActiveLinks(_teammateId1, teammateId4.ClassKey)).Count);
            // Distinct test
            var teamId2 = new AgentId(2, 1);
            _links.AddMembers(_teammateId1, _teammateId2, teamId2);
            Assert.AreEqual(1, new List<AgentId>(_links.GetActiveLinks(_teammateId1, _teammateId2.ClassKey)).Count);
        }

        [TestMethod]
        public void ExistsTest()
        {
            var link = new CommunicationLink(_teammateId1, CommunicationType.CommunicateTo, _teammateId2, _teamId);
            var linkFalse = new CommunicationLink(_teammateId1, CommunicationType.ReportTo, _teammateId2, _teamId);
            Assert.IsFalse(_links.Exists(link));
            _links.List.Add(link);
            Assert.IsTrue(_links.Exists(link));
            Assert.IsFalse(_links.Exists(linkFalse));
        }

        [TestMethod]
        public void AddLinkTest()
        {
            var link = new CommunicationLink(_teammateId1, CommunicationType.CommunicateTo, _teammateId2, _teamId);
            _links.AddLink(link);
            Assert.IsTrue(_links.Exists(link));
            // Deactivate test
            link.Deactivate();
            _links.AddLink(link);
            Assert.AreEqual(1, _links.List.Count);
            Assert.IsTrue(_links[0].IsActive);
            // Duplicate test
            link = new CommunicationLink(_teammateId1, CommunicationType.CommunicateTo, _teammateId2, _teamId);
            _links.AddLink(link);
            Assert.AreEqual(1, _links.List.Count);
        }
    }
}