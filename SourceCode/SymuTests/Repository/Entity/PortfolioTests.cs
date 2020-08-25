#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Common.Interfaces.Entity;
using Symu.Repository.Entity;

#endregion

namespace SymuTests.Repository.Entity
{
    [TestClass]
    public class PortfolioTests
    {
        private readonly ResourceUsage _isWorkingOn = new ResourceUsage(1);
        private readonly ResourceUsage _isSupportOn = new ResourceUsage(2);
        private readonly ResourceUsage _isUsing = new ResourceUsage(3);
        private readonly UId _resourceId = new UId(1);

        [TestMethod]
        public void IsTypeTest()
        {
            var portfolio = new AgentPortfolio(_resourceId, _isSupportOn, 100);
            Assert.IsTrue(portfolio.Equals(_isSupportOn));
            Assert.IsFalse(portfolio.Equals(_isUsing));
        }

        [TestMethod]
        public void EqualsTest()
        {
            var portfolio = new AgentPortfolio(_resourceId, _isSupportOn, 100);
            Assert.IsTrue(portfolio.Equals(_resourceId, _isSupportOn));
            Assert.IsFalse(portfolio.Equals(_resourceId, _isWorkingOn));
            var resource2 = new UId(2);
            Assert.IsFalse(portfolio.Equals(resource2, _isSupportOn));
        }
    }
}