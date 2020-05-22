#region Licence

// Description: Symu - SymuEngineTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Repository.Networks.Group;

#endregion

namespace SymuTests.Repository.Networks.Group
{
    [TestClass]
    public class NetworkGroupsTests
    {
        private readonly NetworkGroups _group = new NetworkGroups();
        private readonly AgentId _teamId = new AgentId(1, 1);
        private readonly AgentId _teamId2 = new AgentId(2, 1);
        private readonly AgentId _teammateId = new AgentId(3, 2);
        private readonly AgentId _teammateId2 = new AgentId(4, 2);
        private readonly AgentId _teammateId3 = new AgentId(5, 2);

        /// <summary>
        ///     With agent 1 one team
        /// </summary>
        [TestMethod]
        public void RemoveAgentTest()
        {
            // Without Agent
            _group.RemoveAgent(_teammateId);
            // With agent 1 one team
            _group.AddMember(_teammateId, 100, _teamId);
            _group.RemoveAgent(_teammateId);
            Assert.IsFalse(_group.IsMemberOfGroup(_teammateId, _teamId));
        }

        /// <summary>
        ///     With agent 1 two teams
        /// </summary>
        [TestMethod]
        public void RemoveAgentTest1()
        {
            _group.AddMember(_teammateId, 100, _teamId);
            _group.AddMember(_teammateId, 100, _teamId2);
            _group.RemoveAgent(_teammateId);
            Assert.IsFalse(_group.IsMemberOfGroup(_teammateId, _teamId));
            Assert.IsFalse(_group.IsMemberOfGroup(_teammateId, _teamId2));
        }

        [TestMethod]
        public void AddTeammateTest()
        {
            Assert.IsFalse(_group.Any());
            _group.AddMember(_teammateId, 100, _teamId);
            Assert.IsTrue(_group.Any());
            Assert.IsTrue(_group.List[_teamId][0].AgentId.Equals(_teammateId));
        }

        [TestMethod]
        public void ClearTest()
        {
            _group.AddMember(_teammateId, 100, _teamId);
            _group.Clear();
            Assert.IsFalse(_group.Any());
        }

        [TestMethod]
        public void AddTeamTest()
        {
            Assert.IsFalse(_group.Any());
            _group.AddGroup(_teamId);
            Assert.IsTrue(_group.Any());
        }

        [TestMethod]
        public void RemoveTeammateTest()
        {
            // Without Agent
            _group.RemoveMember(_teammateId, _teamId);
            // With agent 1 one team
            _group.AddMember(_teammateId, 100, _teamId);
            _group.RemoveMember(_teammateId, _teamId);
            Assert.IsFalse(_group.IsMemberOfGroup(_teammateId, _teamId));
            // With agent 1 two teams
            _group.AddMember(_teammateId, 100, _teamId);
            _group.AddMember(_teammateId, 100, _teamId2);
            _group.RemoveMember(_teammateId, _teamId);
            Assert.IsFalse(_group.IsMemberOfGroup(_teammateId, _teamId));
            Assert.IsTrue(_group.IsMemberOfGroup(_teammateId, _teamId2));
        }

        [TestMethod]
        public void GetTeammatesTest()
        {
            Assert.IsNull(_group.GetMembers(_teamId, _teammateId.ClassKey));
            _group.AddMember(_teammateId, 100, _teamId);
            Assert.IsNotNull(_group.GetMembers(_teamId, _teammateId.ClassKey));
        }

        [TestMethod]
        public void GetTeammatesCountTest()
        {
            Assert.AreEqual(0, _group.GetMembersCount(_teamId));
            _group.AddMember(_teammateId, 100, _teamId);
            Assert.AreEqual(1, _group.GetMembersCount(_teamId));
            _group.AddMember(_teammateId, 100, _teamId); //Check Duplication 
            Assert.AreEqual(1, _group.GetMembersCount(_teamId));
        }

        [TestMethod]
        public void IsTeammateTest()
        {
            Assert.IsFalse(_group.IsMemberOfGroup(_teammateId, _teamId));
            _group.AddMember(_teammateId, 100, _teamId);
            Assert.IsTrue(_group.IsMemberOfGroup(_teammateId, _teamId));
        }

        [TestMethod]
        public void ExistsTest()
        {
            Assert.IsFalse(_group.Exists(_teamId));
            _group.AddGroup(_teamId);
            Assert.IsTrue(_group.Exists(_teamId));
        }

        [TestMethod]
        public void IsTeammateTest1()
        {
            Assert.AreEqual(0, _group.GetGroups(_teammateId, _teamId.ClassKey).Count());
            _group.AddMember(_teammateId, 100, _teamId);
            Assert.AreEqual(1, _group.GetGroups(_teammateId, _teamId.ClassKey).Count());
            _group.AddMember(_teammateId, 100, _teamId2);
            Assert.AreEqual(2, _group.GetGroups(_teammateId, _teamId.ClassKey).Count());
        }

        [TestMethod]
        public void RemoveGroupTest()
        {
            _group.AddGroup(_teamId);
            _group.RemoveGroup(_teamId);
            Assert.IsFalse(_group.Exists(_teamId));
        }

        [TestMethod]
        public void GetGroupsTest()
        {
            Assert.IsNotNull(_group.GetGroups());
            Assert.AreEqual(0, _group.GetGroups().Count());
            _group.AddGroup(_teamId);
            Assert.AreEqual(1, _group.GetGroups().Count());
        }

        [TestMethod]
        public void GetCoMemberIds()
        {
            Assert.AreEqual(0, _group.GetCoMemberIds(_teammateId, _teamId.ClassKey).Count());
            _group.AddMember(_teammateId, 100, _teamId);
            Assert.AreEqual(0, _group.GetCoMemberIds(_teammateId, _teamId.ClassKey).Count());
            _group.AddMember(_teammateId2, 100, _teamId);
            Assert.AreEqual(1, _group.GetCoMemberIds(_teammateId, _teamId.ClassKey).Count());
            _group.AddMember(_teammateId3, 100, _teamId2);
            Assert.AreEqual(1, _group.GetCoMemberIds(_teammateId, _teamId.ClassKey).Count());
            _group.AddMember(_teammateId, 100, _teamId2);
            Assert.AreEqual(2, _group.GetCoMemberIds(_teammateId, _teamId.ClassKey).Count());
        }

        #region Allocation

        [TestMethod]
        public void GetGroupAllocationsTest()
        {
            Assert.AreEqual(0, _group.GetGroupAllocations(_teammateId, _teamId.ClassKey).Count());
            _group.AddMember(_teammateId, 100, _teamId);
            Assert.AreEqual(1, _group.GetGroupAllocations(_teammateId, _teamId.ClassKey).Count());
        }

        [TestMethod]
        public void GetGroupAllocationTest()
        {
            Assert.IsNull(_group.GetGroupAllocation(_teammateId, _teamId));
            _group.AddMember(_teammateId, 100, _teamId);
            Assert.IsNotNull(_group.GetGroupAllocation(_teammateId, _teamId));
        }

        [TestMethod]
        public void GetAllocationTest()
        {
            Assert.AreEqual(0, _group.GetAllocation(_teammateId, _teamId));
            _group.AddMember(_teammateId, 100, _teamId);
            Assert.AreEqual(100, _group.GetAllocation(_teammateId, _teamId));
        }

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void UpdateGroupAllocationTest()
        {
            Assert.ThrowsException<NullReferenceException>(() =>
                _group.UpdateGroupAllocation(_teammateId, _teamId, 0, 0));
        }

        [TestMethod]
        public void UpdateGroupAllocationTest1()
        {
            // Test increment
            _group.AddMember(_teammateId, 50, _teamId);
            _group.UpdateGroupAllocation(_teammateId, _teamId, 20, 0);
            Assert.AreEqual(70, _group.GetAllocation(_teammateId, _teamId));
            // Test decrement with a threshold
            _group.UpdateGroupAllocation(_teammateId, _teamId, -70, 10);
            Assert.AreEqual(10, _group.GetAllocation(_teammateId, _teamId));
        }

        [TestMethod]
        public void FullAllocUpdateGroupAllocationsTest()
        {
            _group.AddMember(_teammateId, 50, _teamId);
            _group.UpdateGroupAllocations(_teammateId, _teamId.ClassKey, true);
            Assert.AreEqual(100, _group.GetAllocation(_teammateId, _teamId));
        }

        [TestMethod]
        public void UpdateGroupAllocationsTest()
        {
            _group.AddMember(_teammateId, 50, _teamId);
            _group.UpdateGroupAllocations(_teammateId, _teamId.ClassKey, false);
            Assert.AreEqual(50, _group.GetAllocation(_teammateId, _teamId));
        }

        [TestMethod]
        public void NullUpdateGroupAllocationsTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                _group.UpdateGroupAllocations(_teammateId, _teamId.ClassKey, true));
        }

        [TestMethod]
        public void CopyToTest()
        {
            _group.AddMember(_teammateId, 100, _teamId);
            _group.CopyTo(_teamId, _teamId2);
            Assert.AreEqual(100, _group.GetAllocation(_teammateId, _teamId2));
        }

        [TestMethod]
        public void GetMemberAllocationsTest()
        {
            Assert.AreEqual(0, _group.GetMemberAllocations(_teamId));
            _group.AddMember(_teammateId, 100, _teamId);
            Assert.AreEqual(100, _group.GetMemberAllocations(_teamId));
            _group.AddMember(_teammateId2, 50, _teamId);
            Assert.AreEqual(150, _group.GetMemberAllocations(_teamId));
        }

        #endregion
    }
}