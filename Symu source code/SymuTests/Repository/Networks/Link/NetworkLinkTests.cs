#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Repository.Networks.Link;

#endregion

namespace SymuTests.Repository.Networks.Link
{
    [TestClass]
    public class NetworkLinkTests
    {
        private readonly AgentId _agentId1 = new AgentId(1, 1);
        private readonly AgentId _agentId2 = new AgentId(2, 2);

        [TestMethod]
        public void HasActiveLinkTest()
        {
            var link = new NetworkLink(_agentId1, _agentId2);
            Assert.IsTrue(link.HasActiveLink(_agentId1, _agentId2));
            Assert.IsFalse(link.HasPassiveLink(_agentId1, _agentId2));
        }

        [TestMethod]
        public void HasPassiveLinkTest()
        {
            var link = new NetworkLink(_agentId1, _agentId2);
            link.Deactivate();
            Assert.IsFalse(link.HasActiveLink(_agentId1, _agentId2));
            Assert.IsTrue(link.HasPassiveLink(_agentId1, _agentId2));
        }

        [TestMethod]
        public void HasLinkTest()
        {
            var link = new NetworkLink(_agentId1, _agentId2);
            Assert.IsTrue(link.HasLink(_agentId1, _agentId2));
        }

        [TestMethod]
        public void EqualsTest()
        {
            var link = new NetworkLink(_agentId1, _agentId2);
            var link2 = new NetworkLink(_agentId1, _agentId2);
            var link3 = new NetworkLink(_agentId2, _agentId1);
            var link4 = new NetworkLink(_agentId1, _agentId1);
            Assert.IsTrue(link.Equals(link2));
            Assert.IsTrue(link.Equals(link3));
            Assert.IsFalse(link.Equals(link4));
        }

        [TestMethod]
        public void ActivateTest()
        {
            var link = new NetworkLink(_agentId1, _agentId2);
            link.Deactivate();
            link.Activate();
            Assert.IsTrue(link.HasActiveLink(_agentId1, _agentId2));
        }

        [TestMethod]
        public void DesActivateTest()
        {
            var link = new NetworkLink(_agentId1, _agentId2);
            link.Deactivate();
            Assert.IsTrue(link.HasPassiveLink(_agentId1, _agentId2));
        }

        [TestMethod]
        public void HasActiveLinksTest()
        {
            var link = new NetworkLink(_agentId1, _agentId2);
            Assert.IsTrue(link.HasActiveLinks(_agentId1));
            // links are bidirectional
            Assert.IsTrue(link.HasActiveLinks(_agentId2));
            link.Deactivate();
            Assert.IsFalse(link.HasActiveLinks(_agentId1));
            Assert.IsFalse(link.HasActiveLinks(_agentId2));
        }
    }
}