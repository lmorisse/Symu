#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Repository.Entity;
using Symu.Repository.Networks.Resources;

#endregion

namespace SymuTests.Repository.Entity
{
    [TestClass]
    public class PortfolioTests
    {
        private ResourceUsage IsWorkingOn = new ResourceUsage(1);
        private ResourceUsage IsSupportOn = new ResourceUsage(2);
        private ResourceUsage IsUsing = new ResourceUsage(3);
        private readonly AgentId _agentId = new AgentId(1, 1);
        private readonly ClassId _classId2 = new ClassId(2);

        [TestMethod]
        public void IsTypeTest()
        {
            var portfolio = new AgentPortfolio(_agentId, IsSupportOn, 100);
            Assert.IsTrue(portfolio.IsResourceUsage(IsSupportOn));
            Assert.IsFalse(portfolio.IsResourceUsage(IsUsing));
        }

        [TestMethod]
        public void IsTypeAndClassIdTest()
        {
            var portfolio = new AgentPortfolio(_agentId, IsSupportOn, 100);
            Assert.IsTrue(portfolio.IsResourceUsageAndClassId(IsSupportOn, _agentId.ClassId));
            Assert.IsFalse(portfolio.IsResourceUsageAndClassId(IsSupportOn, _classId2));
            Assert.IsFalse(portfolio.IsResourceUsageAndClassId(IsUsing, _agentId.ClassId));
        }

        [TestMethod]
        public void EqualsTest()
        {
            var portfolio = new AgentPortfolio(_agentId, IsSupportOn, 100);
            Assert.IsTrue(portfolio.Equals(_agentId, IsSupportOn));
            Assert.IsFalse(portfolio.Equals(_agentId, IsWorkingOn));
            var agentId2 = new AgentId(2, 1);
            Assert.IsFalse(portfolio.Equals(agentId2, IsSupportOn));
        }
    }
}