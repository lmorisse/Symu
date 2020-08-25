#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Common.Interfaces.Agent;
using Symu.Repository.Entity;
using Symu.Repository.Networks.Roles;
using SymuTests.Helpers;

#endregion

namespace SymuTests.Repository.Networks.Roles
{
    [TestClass]
    public class RoleNetworkTests
    {
        private readonly RoleNetwork _roles = new RoleNetwork();
        private readonly AgentId _teamId = new AgentId(1, 1);
        private readonly AgentId _teamId2 = new AgentId(2, 1);
        private readonly AgentId _teammateId = new AgentId(2, 2);
        private readonly ClassId _classId0 = new ClassId(0);
        private TestAgentRole _agentRole;
        private TestAgentRole _agentRole2;

        [TestInitialize]
        public void Initialize()
        {
            _agentRole = new TestAgentRole(_teammateId, _teamId, 1);
            _agentRole2 = new TestAgentRole(_teammateId, _teamId2, 1);
        }

        [TestMethod]
        public void ClearTest()
        {
            _roles.Add(_agentRole);
            _roles.Clear();
            Assert.IsFalse(_roles.Any());
        }

        /// <summary>
        ///     Remove agent
        /// </summary>
        [TestMethod]
        public void RemoveAgentTest()
        {
            _roles.Add(_agentRole);
            _roles.RemoveAgent(_teammateId);
            Assert.IsFalse(_roles.Any());
        }

        /// <summary>
        ///     Remove group
        /// </summary>
        [TestMethod]
        public void RemoveAgentTest1()
        {
            _roles.Add(_agentRole);
            _roles.RemoveAgent(_teamId);
            Assert.IsFalse(_roles.Any());
        }

        [TestMethod]
        public void AddTest()
        {
            Assert.IsFalse(_roles.Exists(_agentRole));
            Assert.IsFalse(_roles.Any());
            _roles.Add(_agentRole);
            Assert.IsTrue(_roles.Any());
            Assert.IsTrue(_roles.Exists(_agentRole));
        }

        [TestMethod]
        public void IsTeammateOfTest()
        {
            Assert.AreEqual(0, _roles.IsMemberOfGroups(_teammateId, _classId0).Count());
            _roles.Add(_agentRole);
            Assert.AreEqual(0, _roles.IsMemberOfGroups(_teammateId, _classId0).Count());
            Assert.AreEqual(1, _roles.IsMemberOfGroups(_teammateId, _teamId.ClassId).Count());
            _roles.Add(_agentRole2);
            Assert.AreEqual(0, _roles.IsMemberOfGroups(_teammateId, _classId0).Count());
            Assert.AreEqual(2, _roles.IsMemberOfGroups(_teammateId, _teamId.ClassId).Count());
        }

        [TestMethod]
        public void RemoveMemberTest()
        {
            _roles.Add(_agentRole);
            _roles.Add(_agentRole2);
            _roles.RemoveMember(_teammateId, _teamId);
            Assert.IsFalse(_roles.HasARoleIn(_teammateId, _teamId));
            Assert.IsTrue(_roles.HasARoleIn(_teammateId, _teamId2));
        }

        [TestMethod]
        public void GetRolesTest()
        {
            var getRoles = _roles.GetRoles(_teamId);
            Assert.IsFalse(getRoles.Any());
            _roles.Add(_agentRole);
            getRoles = _roles.GetRoles(_teamId);
            Assert.IsTrue(getRoles.Any());
        }

        [TestMethod]
        public void GetRolesTest1()
        {
            var getRoles = _roles.GetRoles(_teammateId, _teamId);
            Assert.IsFalse(getRoles.Any());
            _roles.Add(_agentRole);
            getRoles = _roles.GetRoles(_teammateId, _teamId);
            Assert.IsTrue(getRoles.Any());
        }

        [TestMethod]
        public void GetAgentsTest()
        {
            Assert.AreEqual(0, _roles.GetAgents(_agentRole.Role).Count());
            _roles.Add(_agentRole);
            Assert.AreEqual(1, _roles.GetAgents(_agentRole.Role).Count());
            var roleEntity3 = new TestAgentRole(_teammateId, _teamId, 2);
            _roles.Add(roleEntity3);
            Assert.AreEqual(1, _roles.GetAgents(_agentRole.Role).Count());
        }

        [TestMethod]
        public void HasRoleTest()
        {
            Assert.IsFalse(_roles.HasRole(_teammateId, _agentRole.Role));
            _roles.Add(_agentRole);
            Assert.IsTrue(_roles.HasRole(_teammateId, _agentRole.Role));
        }


        [TestMethod]
        public void GetGroupsTest()
        {
            Assert.AreEqual(0, _roles.GetGroups(_teammateId, _agentRole.Role).Count());
            _roles.Add(_agentRole);
            Assert.AreEqual(1, _roles.GetGroups(_teammateId, _agentRole.Role).Count());
            Assert.AreEqual(_teamId, _roles.GetGroups(_teammateId, _agentRole.Role).ElementAt(0));
            _roles.Add(_agentRole2);
            Assert.AreEqual(2, _roles.GetGroups(_teammateId, _agentRole2.Role).Count());
        }

        [TestMethod]
        public void RemoveMembersByRoleTypeFromGroupTest()
        {
            _roles.Add(_agentRole);
            _roles.RemoveMembersByRoleTypeFromGroup(_agentRole.Role, _teamId);
            Assert.AreEqual(0, _roles.GetGroups(_teammateId, _agentRole.Role).Count());
        }
    }
}