#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Common.Interfaces;
using Symu.OrgMod.Entities;
using Symu.Repository.Edges;

#endregion

namespace SymuTests.Repository.Entities
{
    [TestClass]
    public class PortfolioTests
    {
        private readonly IAgentId _agentId = new AgentId(1, 1);
        private readonly IResourceUsage _isSupportOn = new ResourceUsage(2);
        private readonly IResourceUsage _isUsing = new ResourceUsage(3);
        private readonly IAgentId _resourceId = new AgentId(1, 0);

        [TestMethod]
        public void EqualsTest()
        {
            var portfolio = new ActorPortfolio(_agentId, _resourceId, _isSupportOn, 100);
            Assert.IsTrue(portfolio.Equals(_isSupportOn));
            Assert.IsFalse(portfolio.Equals(_isUsing));
        }
    }
}