#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Common.Interfaces.Agent;
using Symu.Repository.Entity;

#endregion

namespace SymuTests.Repository.Entity
{
    [TestClass]
    public class InteractionTests
    {
        private readonly AgentId _agentId1 = new AgentId(1, 1);
        private readonly AgentId _agentId2 = new AgentId(2, 2);

        [TestMethod]
        public void HasActiveLinkTest()
        {
            var link = new Interaction(_agentId1, _agentId2);
            Assert.IsTrue(link.HasActiveInteraction(_agentId1, _agentId2));
            Assert.IsFalse(link.HasPassiveInteraction(_agentId1, _agentId2));
        }

        [TestMethod]
        public void HasPassiveLinkTest()
        {
            var link = new Interaction(_agentId1, _agentId2);
            link.DecreaseWeight();
            Assert.IsFalse(link.HasActiveInteraction(_agentId1, _agentId2));
            Assert.IsTrue(link.HasPassiveInteraction(_agentId1, _agentId2));
        }

        [TestMethod]
        public void HasLinkTest()
        {
            var link = new Interaction(_agentId1, _agentId2);
            Assert.IsTrue(link.HasLink(_agentId1, _agentId2));
        }

        [TestMethod]
        public void EqualsTest()
        {
            var link = new Interaction(_agentId1, _agentId2);
            var link2 = new Interaction(_agentId1, _agentId2);
            var link3 = new Interaction(_agentId2, _agentId1);
            var link4 = new Interaction(_agentId1, _agentId1);
            Assert.IsTrue(link.Equals(link2));
            Assert.IsTrue(link.Equals(link3));
            Assert.IsFalse(link.Equals(link4));
        }

        [TestMethod]
        public void ActivateTest()
        {
            var link = new Interaction(_agentId1, _agentId2);
            link.DecreaseWeight();
            link.IncreaseWeight();
            Assert.IsTrue(link.HasActiveInteraction(_agentId1, _agentId2));
        }

        [TestMethod]
        public void DesActivateTest()
        {
            var link = new Interaction(_agentId1, _agentId2);
            link.DecreaseWeight();
            Assert.IsTrue(link.HasPassiveInteraction(_agentId1, _agentId2));
        }

        [TestMethod]
        public void HasActiveLinksTest()
        {
            var link = new Interaction(_agentId1, _agentId2);
            Assert.IsTrue(link.HasActiveInteractions(_agentId1));
            // links are bidirectional
            Assert.IsTrue(link.HasActiveInteractions(_agentId2));
            link.DecreaseWeight();
            Assert.IsFalse(link.HasActiveInteractions(_agentId1));
            Assert.IsFalse(link.HasActiveInteractions(_agentId2));
        }
    }
}