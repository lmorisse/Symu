#region Licence

// Description: Symu - SymuEngineTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Agents;
using SymuEngine.Repository.Networks.Portfolio;

#endregion

namespace SymuEngineTests.Repository.Networks.Portfolio
{
    [TestClass]
    public class NetworkPortfoliosTests
    {
        private const byte IsSupportOn = 1;
        private const byte IsWorkingOn = 2;
        private const byte IsUsing = 3;
        private readonly AgentId _agentId = new AgentId(3, 3);
        private readonly AgentId _groupId = new AgentId(1, 1);
        private readonly AgentId _objectId = new AgentId(2, 2);
        private readonly NetworkPortfolios _portfolios = new NetworkPortfolios();

        [TestMethod]
        public void ClearTest()
        {
            _portfolios.AddPortfolio(_groupId, _objectId, IsSupportOn, 100);
            _portfolios.Clear();
            Assert.IsFalse(_portfolios.Any());
        }

        [TestMethod]
        public void AddPortfolioTest()
        {
            Assert.IsFalse(_portfolios.Any());
            _portfolios.AddPortfolio(_groupId, _objectId, IsSupportOn, 100);
            Assert.IsTrue(_portfolios.Any());
            Assert.IsTrue(_portfolios.ContainsObject(_objectId));
            Assert.IsTrue(_portfolios.HasObject(_groupId, IsSupportOn));
        }

        [TestMethod]
        public void AddObjectTest()
        {
            Assert.IsFalse(_portfolios.Any());
            _portfolios.AddObject(_objectId);
            Assert.IsTrue(_portfolios.Any());
            Assert.IsTrue(_portfolios.ContainsObject(_objectId));
        }

        /// <summary>
        ///     With a component
        /// </summary>
        [TestMethod]
        public void RemoveAgentTest()
        {
            _portfolios.RemoveAgent(_objectId);
            _portfolios.AddObject(_objectId);
            _portfolios.RemoveAgent(_objectId);
            Assert.IsFalse(_portfolios.Any());
            Assert.IsFalse(_portfolios.ContainsObject(_objectId));
        }

        /// <summary>
        ///     With a kanban
        /// </summary>
        [TestMethod]
        public void RemoveAgentTest1()
        {
            _portfolios.RemoveAgent(_groupId);
            _portfolios.AddPortfolio(_groupId, _objectId, IsSupportOn, 100);
            _portfolios.RemoveAgent(_groupId);
            Assert.IsTrue(_portfolios.Any());
            Assert.IsTrue(_portfolios.ContainsObject(_objectId));
            Assert.IsFalse(_portfolios.HasObject(_groupId, IsSupportOn));
        }

        [TestMethod]
        public void RemoveObjectTest()
        {
            _portfolios.RemoveObject(_objectId);
            _portfolios.AddObject(_objectId);
            _portfolios.RemoveObject(_objectId);
            Assert.IsFalse(_portfolios.Any());
            Assert.IsFalse(_portfolios.ContainsObject(_objectId));
        }

        [TestMethod]
        public void GetCapacityAllocationTest()
        {
            // componentId don't exists
            Assert.AreEqual(0, _portfolios.GetAllocation(_groupId, _objectId, IsWorkingOn));
            // componentId exists
            _portfolios.AddObject(_objectId);
            Assert.AreEqual(0, _portfolios.GetAllocation(_groupId, _objectId, IsWorkingOn));
            _portfolios.AddPortfolio(_groupId, _objectId, IsWorkingOn, 100);
            Assert.AreEqual(100, _portfolios.GetAllocation(_groupId, _objectId, IsWorkingOn));
        }

        [TestMethod]
        public void ContainsObjectTest()
        {
            Assert.IsFalse(_portfolios.ContainsObject(_objectId));
            _portfolios.AddObject(_objectId);
            Assert.IsTrue(_portfolios.ContainsObject(_objectId));
        }

        [TestMethod]
        public void HasObjectTest()
        {
            Assert.IsFalse(_portfolios.HasObject(_groupId, IsSupportOn));
            _portfolios.AddPortfolio(_groupId, _objectId, IsSupportOn, 100);
            Assert.IsTrue(_portfolios.HasObject(_groupId, IsSupportOn));
        }

        [TestMethod]
        public void AddMemberTest()
        {
            _portfolios.AddPortfolio(_groupId, _objectId, IsSupportOn, 100);
            _portfolios.AddPortfolio(_groupId, _objectId, IsWorkingOn, 100);
            _portfolios.AddPortfolio(_groupId, _objectId, IsUsing, 100);
            _portfolios.AddMemberToGroup(_agentId, _groupId);
            Assert.IsTrue(_portfolios.HasObject(_agentId, IsSupportOn));
            Assert.IsTrue(_portfolios.HasObject(_agentId, IsWorkingOn));
            Assert.IsTrue(_portfolios.HasObject(_agentId, IsUsing));
        }

        [TestMethod]
        public void RemoveMemberTest()
        {
            _portfolios.AddPortfolio(_groupId, _objectId, IsSupportOn, 100);
            _portfolios.AddPortfolio(_groupId, _objectId, IsWorkingOn, 100);
            _portfolios.AddPortfolio(_groupId, _objectId, IsUsing, 100);
            _portfolios.AddMemberToGroup(_agentId, _groupId);
            _portfolios.RemoveMemberFromGroup(_agentId, _groupId);
            Assert.IsFalse(_portfolios.HasObject(_agentId, IsSupportOn));
            Assert.IsFalse(_portfolios.HasObject(_agentId, IsWorkingOn));
            Assert.IsFalse(_portfolios.HasObject(_agentId, IsUsing));
        }

        [TestMethod]
        public void ExistsTest()
        {
            Assert.IsFalse(_portfolios.Exists(_groupId, _objectId, IsSupportOn));
            _portfolios.AddPortfolio(_groupId, _objectId, IsSupportOn, 100);
            Assert.IsTrue(_portfolios.Exists(_groupId, _objectId, IsSupportOn));
        }

        [TestMethod]
        public void GetNetworkPortfolioTest()
        {
            Assert.IsNull(_portfolios.GetNetworkPortfolio(_groupId, _objectId, IsSupportOn));
            _portfolios.AddPortfolio(_groupId, _objectId, IsSupportOn, 100);
            Assert.IsNotNull(_portfolios.GetNetworkPortfolio(_groupId, _objectId, IsSupportOn));
        }

        [TestMethod]
        public void CopyToTest()
        {
            var teamId = new AgentId(3, 3);
            _portfolios.AddPortfolio(teamId, _objectId, IsSupportOn, 10);
            _portfolios.AddPortfolio(teamId, _objectId, IsWorkingOn, 50);
            _portfolios.AddPortfolio(teamId, _objectId, IsUsing, 100);
            var newTeamId = new AgentId(4, 3);
            _portfolios.CopyTo(teamId, newTeamId);
            Assert.IsTrue(_portfolios.Exists(newTeamId, _objectId, IsSupportOn));
            Assert.IsTrue(_portfolios.Exists(newTeamId, _objectId, IsWorkingOn));
            Assert.IsTrue(_portfolios.Exists(newTeamId, _objectId, IsUsing));
        }

        [TestMethod]
        public void GetObjectIdsTest()
        {
            Assert.AreEqual(0, _portfolios.GetObjectIds(_agentId).Count);
            _portfolios.AddPortfolio(_agentId, _objectId, IsUsing, 100);
            Assert.AreEqual(1, _portfolios.GetObjectIds(_agentId).Count);
        }

        [TestMethod]
        public void GetObjectIdsTest1()
        {
            Assert.AreEqual(0, _portfolios.GetObjectIds(_agentId, IsUsing).Count);
            _portfolios.AddPortfolio(_agentId, _objectId, IsUsing, 100);
            Assert.AreEqual(1, _portfolios.GetObjectIds(_agentId, IsUsing).Count);
        }
    }
}