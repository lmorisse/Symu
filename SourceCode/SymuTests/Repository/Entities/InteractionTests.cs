﻿#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Common.Interfaces;
using Symu.OrgMod.Edges;
using Symu.OrgMod.GraphNetworks;

#endregion

namespace SymuTests.Repository.Entities
{
    [TestClass]
    public class InteractionTests
    {
        private readonly GraphMetaNetwork _network = new GraphMetaNetwork();
        private readonly AgentId _agentId1 = new AgentId(1, 1);
        private readonly AgentId _agentId2 = new AgentId(2, 2);

        [TestMethod]
        public void HasActiveLinkTest()
        {
            var link = new ActorActor(_network.ActorActor,_agentId1, _agentId2);
            Assert.IsTrue(link.HasActiveInteraction(_agentId1, _agentId2));
            Assert.IsFalse(link.HasPassiveInteraction(_agentId1, _agentId2));
        }

        [TestMethod]
        public void HasPassiveLinkTest()
        {
            var link = new ActorActor(_network.ActorActor,_agentId1, _agentId2);
            link.DecreaseWeight();
            Assert.IsFalse(link.HasActiveInteraction(_agentId1, _agentId2));
            Assert.IsTrue(link.HasPassiveInteraction(_agentId1, _agentId2));
        }

        [TestMethod]
        public void HasLinkTest()
        {
            var link = new ActorActor(_network.ActorActor,_agentId1, _agentId2);
            Assert.IsTrue(link.HasLink(_agentId1, _agentId2));
        }

        [TestMethod]
        public void EqualsTest()
        {
            var link = new ActorActor(_network.ActorActor,_agentId1, _agentId2);
            var link2 = new ActorActor(_network.ActorActor,_agentId1, _agentId2);
            var link3 = new ActorActor(_network.ActorActor,_agentId2, _agentId1);
            var link4 = new ActorActor(_network.ActorActor,_agentId1, _agentId1);
            Assert.IsTrue(link.Equals(link2));
            Assert.IsTrue(link.Equals(link3));
            Assert.IsFalse(link.Equals(link4));
        }

        [TestMethod]
        public void ActivateTest()
        {
            var link = new ActorActor(_network.ActorActor,_agentId1, _agentId2);
            link.DecreaseWeight();
            link.IncreaseWeight();
            Assert.IsTrue(link.HasActiveInteraction(_agentId1, _agentId2));
        }

        [TestMethod]
        public void DesActivateTest()
        {
            var link = new ActorActor(_network.ActorActor,_agentId1, _agentId2);
            link.DecreaseWeight();
            Assert.IsTrue(link.HasPassiveInteraction(_agentId1, _agentId2));
        }

        [TestMethod]
        public void HasActiveLinksTest()
        {
            var link = new ActorActor(_network.ActorActor,_agentId1, _agentId2);
            Assert.IsTrue(link.HasActiveInteraction(_agentId1));
            // links are bidirectional
            Assert.IsTrue(link.HasActiveInteraction(_agentId2));
            link.DecreaseWeight();
            Assert.IsFalse(link.HasActiveInteraction(_agentId1));
            Assert.IsFalse(link.HasActiveInteraction(_agentId2));
        }
    }
}