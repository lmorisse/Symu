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
using Symu.Repository.Networks.Roles;

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
        private RoleEntity _roleEntity;
        private RoleEntity _roleEntity2;

        [TestInitialize]
        public void Initialize()
        {
            _roleEntity = new RoleEntity(_teammateId, _teamId, 1);
            _roleEntity2 = new RoleEntity(_teammateId, _teamId2, 1);
        }

        [TestMethod]
        public void ClearTest()
        {
            _roles.Add(_roleEntity);
            _roles.Clear();
            Assert.IsFalse(_roles.Any());
        }

        /// <summary>
        ///     Remove agent
        /// </summary>
        [TestMethod]
        public void RemoveAgentTest()
        {
            _roles.Add(_roleEntity);
            _roles.RemoveAgent(_teammateId);
            Assert.IsFalse(_roles.Any());
        }

        /// <summary>
        ///     Remove group
        /// </summary>
        [TestMethod]
        public void RemoveAgentTest1()
        {
            _roles.Add(_roleEntity);
            _roles.RemoveAgent(_teamId);
            Assert.IsFalse(_roles.Any());
        }

        [TestMethod]
        public void AddTest()
        {
            Assert.IsFalse(_roles.Exists(_roleEntity));
            Assert.IsFalse(_roles.Any());
            _roles.Add(_roleEntity);
            Assert.IsTrue(_roles.Any());
            Assert.IsTrue(_roles.Exists(_roleEntity));
        }

        [TestMethod]
        public void IsTeammateOfTest()
        {
            Assert.AreEqual(0, _roles.IsMemberOfGroups(_teammateId, _classId0).Count());
            _roles.Add(_roleEntity);
            Assert.AreEqual(0, _roles.IsMemberOfGroups(_teammateId, _classId0).Count());
            Assert.AreEqual(1, _roles.IsMemberOfGroups(_teammateId, _teamId.ClassId).Count());
            _roles.Add(_roleEntity2);
            Assert.AreEqual(0, _roles.IsMemberOfGroups(_teammateId, _classId0).Count());
            Assert.AreEqual(2, _roles.IsMemberOfGroups(_teammateId, _teamId.ClassId).Count());
        }

        [TestMethod]
        public void RemoveMemberTest()
        {
            _roles.Add(_roleEntity);
            _roles.Add(_roleEntity2);
            _roles.RemoveMember(_teammateId, _teamId);
            Assert.IsFalse(_roles.HasARoleIn(_teammateId, _teamId));
            Assert.IsTrue(_roles.HasARoleIn(_teammateId, _teamId2));
        }

        [TestMethod]
        public void GetRolesTest()
        {
            var getRoles = _roles.GetRoles(_teamId);
            Assert.IsFalse(getRoles.Any());
            _roles.Add(_roleEntity);
            getRoles = _roles.GetRoles(_teamId);
            Assert.IsTrue(getRoles.Any());
        }

        [TestMethod]
        public void GetRolesTest1()
        {
            var getRoles = _roles.GetRoles(_teammateId, _teamId);
            Assert.IsFalse(getRoles.Any());
            _roles.Add(_roleEntity);
            getRoles = _roles.GetRoles(_teammateId, _teamId);
            Assert.IsTrue(getRoles.Any());
        }

        [TestMethod]
        public void GetAgentsTest()
        {
            Assert.AreEqual(0, _roles.GetAgents(1).Count());
            _roles.Add(_roleEntity);
            Assert.AreEqual(1, _roles.GetAgents(1).Count());
            var RoleEntity3 = new RoleEntity(_teammateId, _teamId, 2);
            _roles.Add(RoleEntity3);
            Assert.AreEqual(1, _roles.GetAgents(1).Count());
        }

        [TestMethod]
        public void HasRoleTest()
        {
            Assert.IsFalse(_roles.HasRole(_teammateId, 1));
            _roles.Add(_roleEntity);
            Assert.IsTrue(_roles.HasRole(_teammateId, 1));
        }


        [TestMethod]
        public void GetGroupsTest()
        {
            Assert.AreEqual(0, _roles.GetGroups(_teammateId, 1).Count());
            _roles.Add(_roleEntity);
            Assert.AreEqual(1, _roles.GetGroups(_teammateId, 1).Count());
            Assert.AreEqual(_teamId, _roles.GetGroups(_teammateId, 1).ElementAt(0));
            _roles.Add(_roleEntity2);
            Assert.AreEqual(2, _roles.GetGroups(_teammateId, 1).Count());
        }

        [TestMethod]
        public void RemoveMembersByRoleTypeFromGroupTest()
        {
            _roles.Add(_roleEntity);
            _roles.RemoveMembersByRoleTypeFromGroup(1, _teamId);
            Assert.AreEqual(0, _roles.GetGroups(_teammateId, 1).Count());
        }
    }
}