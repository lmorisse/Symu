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
    public class NetworkPortfolioTests
    {
        private const byte IsWorkingOn = 1;
        private const byte IsSupportOn = 2;
        private const byte IsUsing = 3;
        private readonly AgentId _agentId = new AgentId(1, 1);

        [TestMethod]
        public void IsTypeTest()
        {
            var portfolio = new NetworkPortfolio(_agentId, IsSupportOn, 100);
            Assert.IsTrue(portfolio.IsType(IsSupportOn));
            Assert.IsFalse(portfolio.IsType(IsUsing));
        }

        [TestMethod]
        public void IsTypeAndClassKeyTest()
        {
            var portfolio = new NetworkPortfolio(_agentId, IsSupportOn, 100);
            Assert.IsTrue(portfolio.IsTypeAndClassKey(IsSupportOn, _agentId.ClassKey));
            Assert.IsFalse(portfolio.IsTypeAndClassKey(IsSupportOn, 2));
            Assert.IsFalse(portfolio.IsTypeAndClassKey(IsUsing, _agentId.ClassKey));
        }

        [TestMethod]
        public void EqualsTest()
        {
            var portfolio = new NetworkPortfolio(_agentId, IsSupportOn, 100);
            Assert.IsTrue(portfolio.Equals(_agentId, IsSupportOn));
            Assert.IsFalse(portfolio.Equals(_agentId, IsWorkingOn));
            var agentId2 = new AgentId(2, 1);
            Assert.IsFalse(portfolio.Equals(agentId2, IsSupportOn));
        }
    }
}