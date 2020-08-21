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
using Symu.Repository.Entity;
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
        private TestAgentResource _agentResourceSupportOn;
        private TestAgentResource _agentResourceWorkingOn;
        private TestAgentResource _agentResourceUsing;

        [TestInitialize]
        public void Initialize()
        {
            _agentResourceSupportOn = new TestAgentResource(_resource.Id, new ResourceUsage(IsSupportOn), 100);
            _agentResourceWorkingOn = new TestAgentResource(_resource.Id, new ResourceUsage(IsWorkingOn), 100);
            _agentResourceUsing = new TestAgentResource(_resource.Id, new ResourceUsage(IsUsing), 100);
        }

        [TestMethod]
        public void ClearTest()
        {
            _resources.Add(_groupId, _agentResourceSupportOn);
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
            Assert.IsFalse(_resources.HasResource(_groupId, new ResourceUsage(IsSupportOn)));
            _resources.Add(_groupId, _agentResourceSupportOn);
            Assert.IsTrue(_resources.HasResource(_groupId, new ResourceUsage(IsSupportOn)));
        }


        [TestMethod]
        public void ExistsTest3()
        {
            Assert.IsFalse(_resources.HasResource(_groupId, _resource.Id, new ResourceUsage(IsSupportOn)));
            _resources.Add(_groupId, _agentResourceSupportOn);
            Assert.IsTrue(_resources.HasResource(_groupId, _resource.Id, new ResourceUsage(IsSupportOn)));
        }

        [TestMethod]
        public void AddTest()
        {
            Assert.IsFalse(_resources.Any());
            _resources.Add(_groupId, _resource, _agentResourceSupportOn);
            Assert.IsTrue(_resources.Any());
            Assert.IsTrue(_resources.Exists(_resource));
            Assert.IsTrue(_resources.HasResource(_groupId, new ResourceUsage(IsSupportOn)));
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
            _resources.Add(_groupId, _resource, _agentResourceSupportOn);
            _resources.RemoveAgent(_groupId);
            Assert.IsFalse(_resources.Any());
            Assert.IsTrue(_resources.Exists(_resource));
            Assert.IsFalse(_resources.HasResource(_groupId, new ResourceUsage(IsSupportOn)));
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
            Assert.AreEqual(0, _resources.GetAllocation(_groupId, _resource.Id, new ResourceUsage(IsWorkingOn)));
            // componentId exists
            _resources.Add(_resource);
            Assert.AreEqual(0, _resources.GetAllocation(_groupId, _resource.Id, new ResourceUsage(IsWorkingOn)));
            _resources.Add(_groupId, _agentResourceWorkingOn);
            Assert.AreEqual(100, _resources.GetAllocation(_groupId, _resource.Id, new ResourceUsage(IsWorkingOn)));
        }

        [TestMethod]
        public void AddMemberTest()
        {
            _resources.Add(_groupId, _agentResourceSupportOn);
            _resources.Add(_groupId, _agentResourceWorkingOn);
            _resources.Add(_groupId, _agentResourceUsing);
            _resources.AddMemberToGroup(_agentId, _groupId);
            Assert.IsTrue(_resources.HasResource(_agentId, new ResourceUsage(IsSupportOn)));
            Assert.IsTrue(_resources.HasResource(_agentId, new ResourceUsage(IsWorkingOn)));
            Assert.IsTrue(_resources.HasResource(_agentId, new ResourceUsage(IsUsing)));
        }

        [TestMethod]
        public void RemoveMemberTest()
        {
            _resources.Add(_groupId, _agentResourceSupportOn);
            _resources.Add(_groupId, _agentResourceWorkingOn);
            _resources.Add(_groupId, _agentResourceUsing);
            _resources.AddMemberToGroup(_agentId, _groupId);
            _resources.RemoveMemberFromGroup(_agentId, _groupId);
            Assert.IsFalse(_resources.HasResource(_agentId, new ResourceUsage(IsSupportOn)));
            Assert.IsFalse(_resources.HasResource(_agentId, new ResourceUsage(IsWorkingOn)));
            Assert.IsFalse(_resources.HasResource(_agentId, new ResourceUsage(IsUsing)));
        }

        [TestMethod]
        public void GetResourceTest()
        {
            Assert.IsNull(_resources.GetAgentResource(_groupId, _resource.Id, new ResourceUsage(IsSupportOn)));
            _resources.Add(_groupId, _agentResourceSupportOn);
            Assert.IsNotNull(_resources.GetAgentResource(_groupId, _resource.Id, new ResourceUsage(IsSupportOn)));
        }

        [TestMethod]
        public void CopyToTest()
        {
            var teamId = new AgentId(3, 3);
            _resources.Add(_resource);
            _resources.Add(teamId, _agentResourceSupportOn);
            _resources.Add(teamId, _agentResourceWorkingOn);
            _resources.Add(teamId, _agentResourceUsing);
            var newTeamId = new AgentId(4, 3);
            _resources.CopyTo(teamId, newTeamId);
            Assert.IsTrue(_resources.HasResource(newTeamId, _resource.Id, new ResourceUsage(IsSupportOn)));
            Assert.IsTrue(_resources.HasResource(newTeamId, _resource.Id, new ResourceUsage(IsWorkingOn)));
            Assert.IsTrue(_resources.HasResource(newTeamId, _resource.Id, new ResourceUsage(IsUsing)));
        }

        [TestMethod]
        public void GetResourceIdsTest()
        {
            Assert.AreEqual(0, _resources.GetResourceIds(_agentId).Count());
            _resources.Add(_agentId, _agentResourceUsing);
            Assert.AreEqual(1, _resources.GetResourceIds(_agentId).Count());
        }

        [TestMethod]
        public void GetResourceIdsTest1()
        {
            Assert.AreEqual(0, _resources.GetResourceIds(_agentId, new ResourceUsage(IsUsing)).Count());
            _resources.Add(_agentId, _agentResourceUsing);
            Assert.AreEqual(1, _resources.GetResourceIds(_agentId, new ResourceUsage(IsUsing)).Count());
        }
    }
}