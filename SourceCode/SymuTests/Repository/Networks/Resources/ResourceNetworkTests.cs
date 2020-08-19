﻿#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Common.Interfaces;
using Symu.Repository.Networks.Portfolio;
using Symu.Repository.Networks.Resources;
using SymuTests.Helpers;

#endregion

namespace SymuTests.Repository.Networks.Resources
{
    [TestClass]
    public class ResourceNetworkTests
    {
        private const byte IsSupportOn = 1;
        private const byte IsWorkingOn = 2;
        private const byte IsUsing = 3;
        private readonly TestAgentId _agentId = new TestAgentId(3, 3);
        private readonly TestAgentId _groupId = new TestAgentId(1, 1);
        private readonly TestResource _resource = new TestResource(2, 2);
        private readonly ResourceNetwork _resources = new ResourceNetwork();

        [TestMethod]
        public void ClearTest()
        {
            _resources.Add(_groupId, _resource, IsSupportOn, 100);
            _resources.Clear();
            Assert.IsFalse(_resources.Any());
        }

        [TestMethod]
        public void ExistsTest1()
        {
            Assert.IsFalse(_resources.Exists(_resource));
            _resources.Add(_resource);
            Assert.IsTrue(_resources.Exists(_resource));
        }

        [TestMethod]
        public void ExistsTest2()
        {
            Assert.IsFalse(_resources.HasResource(_groupId, IsSupportOn));
            _resources.Add(_groupId, _resource, IsSupportOn, 100);
            Assert.IsTrue(_resources.HasResource(_groupId, IsSupportOn));
        }


        [TestMethod]
        public void ExistsTest3()
        {
            Assert.IsFalse(_resources.HasResource(_groupId, _resource.Id, IsSupportOn));
            _resources.Add(_groupId, _resource, IsSupportOn, 100);
            Assert.IsTrue(_resources.HasResource(_groupId, _resource.Id, IsSupportOn));
        }

        [TestMethod]
        public void AddTest()
        {
            Assert.IsFalse(_resources.Any());
            _resources.Add(_groupId, _resource, IsSupportOn, 100);
            Assert.IsTrue(_resources.Any());
            Assert.IsTrue(_resources.Exists(_resource));
            Assert.IsTrue(_resources.HasResource(_groupId, IsSupportOn));
        }

        [TestMethod]
        public void AddTest1()
        {
            Assert.IsFalse(_resources.Any());
            _resources.Add(_resource);
            Assert.IsFalse(_resources.Any());
            Assert.IsTrue(_resources.Exists(_resource));
        }

        [TestMethod]
        public void AddAgentIdTest()
        {
            _resources.AddAgentId(_agentId);
            Assert.IsTrue(_resources.Any());
            Assert.IsFalse(_resources.Exists(_agentId));
        }

        [TestMethod]
        public void RemoveAgentTest()
        {
            _resources.RemoveAgent(_agentId);
            _resources.AddAgentId(_agentId);
            _resources.RemoveAgent(_agentId);
            Assert.IsFalse(_resources.Any());
            Assert.IsFalse(_resources.Exists(_resource));
        }

        /// <summary>
        ///     With a resource
        /// </summary>
        [TestMethod]
        public void RemoveAgentTest1()
        {
            _resources.RemoveAgent(_groupId);
            _resources.Add(_groupId, _resource, IsSupportOn, 100);
            _resources.RemoveAgent(_groupId);
            Assert.IsFalse(_resources.Any());
            Assert.IsTrue(_resources.Exists(_resource));
            Assert.IsFalse(_resources.HasResource(_groupId, IsSupportOn));
        }

        [TestMethod]
        public void RemoveTest()
        {
            _resources.Remove(_resource);
            _resources.Add(_resource);
            _resources.Remove(_resource);
            Assert.IsFalse(_resources.Any());
            Assert.IsFalse(_resources.Exists(_resource));
        }

        [TestMethod]
        public void GetAllocationTest()
        {
            // componentId don't exists
            Assert.AreEqual(0, _resources.GetAllocation(_groupId, _resource.Id, IsWorkingOn));
            // componentId exists
            _resources.Add(_resource);
            Assert.AreEqual(0, _resources.GetAllocation(_groupId, _resource.Id, IsWorkingOn));
            _resources.Add(_groupId, _resource, IsWorkingOn, 100);
            Assert.AreEqual(100, _resources.GetAllocation(_groupId, _resource.Id, IsWorkingOn));
        }

        [TestMethod]
        public void AddMemberTest()
        {
            _resources.Add(_groupId, _resource, IsSupportOn, 100);
            _resources.Add(_groupId, _resource, IsWorkingOn, 100);
            _resources.Add(_groupId, _resource, IsUsing, 100);
            _resources.AddMemberToGroup(_agentId, _groupId);
            Assert.IsTrue(_resources.HasResource(_agentId, IsSupportOn));
            Assert.IsTrue(_resources.HasResource(_agentId, IsWorkingOn));
            Assert.IsTrue(_resources.HasResource(_agentId, IsUsing));
        }

        [TestMethod]
        public void RemoveMemberTest()
        {
            _resources.Add(_groupId, _resource, IsSupportOn, 100);
            _resources.Add(_groupId, _resource, IsWorkingOn, 100);
            _resources.Add(_groupId, _resource, IsUsing, 100);
            _resources.AddMemberToGroup(_agentId, _groupId);
            _resources.RemoveMemberFromGroup(_agentId, _groupId);
            Assert.IsFalse(_resources.HasResource(_agentId, IsSupportOn));
            Assert.IsFalse(_resources.HasResource(_agentId, IsWorkingOn));
            Assert.IsFalse(_resources.HasResource(_agentId, IsUsing));
        }

        [TestMethod]
        public void GetResourceTest()
        {
            Assert.IsNull(_resources.GetResource(_groupId, _resource.Id, IsSupportOn));
            _resources.Add(_groupId, _resource, IsSupportOn, 100);
            Assert.IsNotNull(_resources.GetResource(_groupId, _resource.Id, IsSupportOn));
        }

        [TestMethod]
        public void CopyToTest()
        {
            var teamId = new AgentId(3, 3);
            _resources.Add(teamId, _resource, IsSupportOn, 10);
            _resources.Add(teamId, _resource, IsWorkingOn, 50);
            _resources.Add(teamId, _resource, IsUsing, 100);
            var newTeamId = new AgentId(4, 3);
            _resources.CopyTo(teamId, newTeamId);
            Assert.IsTrue(_resources.HasResource(newTeamId, _resource.Id, IsSupportOn));
            Assert.IsTrue(_resources.HasResource(newTeamId, _resource.Id, IsWorkingOn));
            Assert.IsTrue(_resources.HasResource(newTeamId, _resource.Id, IsUsing));
        }

        [TestMethod]
        public void GetResourceIdsTest()
        {
            Assert.IsNull(_resources.GetResourceIds(_agentId));
            _resources.Add(_agentId, _resource, IsUsing, 100);
            Assert.AreEqual(1, _resources.GetResourceIds(_agentId).Count());
        }

        [TestMethod]
        public void GetResourceIdsTest1()
        {
            Assert.IsNull(_resources.GetResourceIds(_agentId, IsUsing));
            _resources.Add(_agentId, _resource, IsUsing, 100);
            Assert.AreEqual(1, _resources.GetResourceIds(_agentId, IsUsing).Count());
        }
    }
}