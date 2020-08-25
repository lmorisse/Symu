#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Common.Interfaces.Agent;
using Symu.DNA.TwoModesNetworks.Interactions;
using Symu.Repository.Entity;

#endregion


namespace SymuTests.Repository.Networks.Interactions
{
    [TestClass]
    public class InteractionNetworkTests
    {
        private readonly AgentId _agentId1 = new AgentId(2, 2);
        private readonly AgentId _agentId2 = new AgentId(3, 2);
        private readonly AgentId _agentId3 = new AgentId(4, 2);
        private readonly InteractionNetwork _interactions = new InteractionNetwork();

        private Interaction _interaction12;
        private Interaction _interaction21;
        private Interaction _interaction31;

        [TestInitialize]
        public void Initialize()
        {
            _interaction12 = new Interaction(_agentId1, _agentId2);
            _interaction21 = new Interaction(_agentId2, _agentId1);
            _interaction31 = new Interaction(_agentId3, _agentId1);
        }

        [TestMethod]
        public void RemoveAgentTest()
        {
            _interactions.AddInteraction(_interaction12);
            _interactions.AddInteraction(_interaction31);
            _interactions.RemoveAgent(_agentId1);
            Assert.IsFalse(_interactions.Any());
        }

        [TestMethod]
        public void AnyTest()
        {
            Assert.IsFalse(_interactions.Any());
            _interactions.AddInteraction(_interaction12);
            Assert.IsTrue(_interactions.Any());
        }

        [TestMethod]
        public void ClearTest()
        {
            _interactions.AddInteraction(_interaction12);
            _interactions.AddInteraction(_interaction31);
            _interactions.Clear();
            Assert.IsFalse(_interactions.Any());
        }

        [TestMethod]
        public void DeactivateTeammatesLinkTest()
        {
            _interactions.AddInteraction(_interaction12);
            var link = _interactions[0];
            // Active link
            Assert.IsTrue(link.IsActive);
            // Deactivate
            _interactions.DecreaseInteraction(_agentId1, _agentId2);
            Assert.IsFalse(link.IsActive);
            Assert.IsTrue(link.IsPassive);
        }

        [TestMethod]
        public void HasActiveLinkTest()
        {
            _interactions.AddInteraction(_interaction12);
            var link = _interactions[0];
            Assert.IsTrue(link.HasActiveInteraction(_agentId1, _agentId2));
            Assert.IsFalse(link.HasActiveInteraction(_agentId1, _agentId3));
        }

        [TestMethod]
        public void HasPassiveLinkTest()
        {
            _interactions.AddInteraction(_interaction12);
            var link = _interactions[0];
            link.DecreaseWeight();
            Assert.IsTrue(link.HasPassiveInteraction(_agentId1, _agentId2));
            Assert.IsFalse(link.HasPassiveInteraction(_agentId1, _agentId3));
        }

        [TestMethod]
        public void GetActiveLinksTest()
        {
            Assert.AreEqual(0, new List<IAgentId>(_interactions.GetActiveInteractions(_agentId1)).Count);
            _interactions.AddInteraction(_interaction12);
            _interactions.AddInteraction(_interaction31);
            var teammateId4 = new AgentId(5, 2);
            var interaction = new Interaction(_agentId1, teammateId4);
            _interactions.AddInteraction(interaction);
            Assert.AreEqual(3, new List<IAgentId>(_interactions.GetActiveInteractions(_agentId1)).Count);

            // Distinct test
            _interactions.AddInteraction(_interaction12);
            Assert.AreEqual(3, new List<IAgentId>(_interactions.GetActiveInteractions(_agentId1)).Count);
        }

        [TestMethod]
        public void TeammateExistsTest()
        {
            _interactions.AddInteraction(_interaction12);
            Assert.IsTrue(_interactions.Exists(_agentId1, _agentId2));
            Assert.IsTrue(_interactions.Exists(_agentId2, _agentId1));
        }

        [TestMethod]
        public void ExistsTest()
        {
            var link = new Interaction(_agentId1, _agentId2);
            Assert.IsFalse(_interactions.Exists(link));
            _interactions.List.Add(link);
            Assert.IsTrue(_interactions.Exists(link));
        }

        [TestMethod]
        public void AddLinkTest()
        {
            var link = new Interaction(_agentId1, _agentId2);
            _interactions.AddInteraction(_interaction12);
            Assert.IsTrue(_interactions.Exists(link));
            // Deactivate test
            link.DecreaseWeight();
            _interactions.AddInteraction(_interaction12);
            Assert.AreEqual(1, _interactions.List.Count);
            Assert.IsTrue(_interactions[0].IsActive);
        }

        /// <summary>
        ///     Empty list
        /// </summary>
        [TestMethod]
        public void AddLinksTest()
        {
            var agents = new List<IInteraction>();
            _interactions.AddInteractions(agents);
            Assert.AreEqual(0, _interactions.Count);
        }

        /// <summary>
        ///     Empty list
        /// </summary>
        [TestMethod]
        public void AddLinksTest1()
        {
            var agents = new List<IInteraction> {_interaction12, _interaction31};
            _interactions.AddInteractions(agents);
            Assert.AreEqual(2, _interactions.Count);
            for (var i = 0; i < 2; i++)
            {
                Assert.AreEqual(1, _interactions[i].Weight);
            }
        }

        [TestMethod]
        public void GetInteractionWeightTest()
        {
            Assert.AreEqual(0, _interactions.GetInteractionWeight(_agentId1, _agentId2));
            _interactions.AddInteraction(_interaction12);
            Assert.AreEqual(1, _interactions.GetInteractionWeight(_agentId1, _agentId2));
            _interactions.AddInteraction(_interaction21);
            Assert.AreEqual(2, _interactions.GetInteractionWeight(_agentId1, _agentId2));
        }

        [TestMethod]
        public void NormalizedCountLinksTest()
        {
            Assert.AreEqual(0, _interactions.NormalizedCountLinks(_agentId1, _agentId2));
            _interactions.AddInteraction(_interaction12);
            _interactions.SetMaxLinksCount();
            Assert.AreEqual(1, _interactions.NormalizedCountLinks(_agentId1, _agentId2));
            _interactions.AddInteraction(_interaction21);
            _interactions.SetMaxLinksCount();
            Assert.AreEqual(1, _interactions.NormalizedCountLinks(_agentId1, _agentId2));
        }
    }
}