﻿#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Repository.Networks.Roles;

#endregion

namespace SymuTests.Repository.Networks.Roles
{
    [TestClass]
    public class RoleEntityTests
    {
        private readonly AgentId _agentId = new AgentId(1, 1);
        private readonly AgentId _agentId2 = new AgentId(2, 1);
        private readonly AgentId _groupId = new AgentId(3, 2);
        private readonly AgentId _groupId2 = new AgentId(4, 1);

        [TestMethod]
        public void IsMemberOfGroupsTest()
        {
            var role = new RoleEntity(_agentId, _groupId, 1);
            Assert.IsTrue(role.IsMemberOfGroups(_agentId, _groupId.ClassId));
            Assert.IsFalse(role.IsMemberOfGroups(_agentId2, _groupId.ClassId));
            Assert.IsFalse(role.IsMemberOfGroups(_agentId2, _groupId2.ClassId));
            Assert.IsFalse(role.IsMemberOfGroups(_agentId, _groupId2.ClassId));
        }

        [TestMethod]
        public void HasRoleInGroupTest()
        {
            var role = new RoleEntity(_agentId, _groupId, 1);
            Assert.IsTrue(role.HasRoleInGroup(1, _groupId));
            Assert.IsFalse(role.HasRoleInGroup(2, _groupId));
            Assert.IsFalse(role.HasRoleInGroup(1, _groupId2));
        }

        [TestMethod]
        public void HasRoleInGroupTest1()
        {
            var role = new RoleEntity(_agentId, _groupId, 1);
            Assert.IsTrue(role.HasRoleInGroup(_agentId, 1, _groupId));
            Assert.IsFalse(role.HasRoleInGroup(_agentId, 2, _groupId));
            Assert.IsFalse(role.HasRoleInGroup(_agentId, 1, _groupId2));
            Assert.IsFalse(role.HasRoleInGroup(_agentId2, 1, _groupId));
        }

        [TestMethod]
        public void HasRoleInGroupTest2()
        {
            var role = new RoleEntity(_agentId, _groupId, 1);
            Assert.IsTrue(role.HasRoleInGroup(_agentId, _groupId));
            Assert.IsFalse(role.HasRoleInGroup(_agentId, _groupId2));
            Assert.IsFalse(role.HasRoleInGroup(_agentId2, _groupId));
        }

        [TestMethod]
        public void IsGroupTest()
        {
            var role = new RoleEntity(_agentId, _groupId, 1);
            Assert.IsTrue(role.IsGroup(_groupId));
            Assert.IsFalse(role.IsGroup(_groupId2));
        }

        [TestMethod]
        public void IsAgentTest()
        {
            var role = new RoleEntity(_agentId, _groupId, 1);
            Assert.IsTrue(role.IsAgent(_agentId));
            Assert.IsFalse(role.IsAgent(_agentId2));
        }

        [TestMethod]
        public void HasRoleTest()
        {
            var role = new RoleEntity(_agentId, _groupId, 1);
            Assert.IsTrue(role.HasRole(1));
            Assert.IsFalse(role.HasRole(2));
        }
    }
}