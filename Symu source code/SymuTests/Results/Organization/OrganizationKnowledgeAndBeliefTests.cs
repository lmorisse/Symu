#region Licence

// Description: Symu - SymuEngineTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models;
using Symu.Classes.Organization;
using Symu.Repository.Networks;
using Symu.Repository.Networks.Beliefs;
using Symu.Repository.Networks.Knowledges;
using Symu.Results.Organization;

#endregion

namespace SymuTests.Results.Organization
{
    [TestClass]
    public class OrganizationKnowledgeAndBeliefTests
    {
        private readonly AgentId _agentId = new AgentId(1, 1);
        private readonly AgentId _agentId2 = new AgentId(2, 1);
        private readonly AgentExpertise _expertise = new AgentExpertise();
        private readonly Knowledge _knowledge = new Knowledge(1, "1", 1);
        private readonly Knowledge _knowledge2 = new Knowledge(2, "2", 1);
        private readonly Network _network = new Network(new AgentTemplates(), new OrganizationModels());
        private readonly OrganizationModels _organizationModels = new OrganizationModels();
        private OrganizationKnowledgeAndBelief _result;


        [TestInitialize]
        public void Initialize()
        {
            _result = new OrganizationKnowledgeAndBelief(_network, _organizationModels);
            _network.AddKnowledge(_knowledge);
            _network.AddKnowledge(_knowledge2);
        }

        #region Knowledge

        /// <summary>
        ///     0 knowledge
        /// </summary>
        [TestMethod]
        public void HandleKnowledge0Test()
        {
            _result.HandleKnowledge(0);
            Assert.AreEqual(0, _result.Knowledges[0].Mean);
        }

        /// <summary>
        ///     1 knowledge
        /// </summary>
        [TestMethod]
        public void HandleKnowledge1Test()
        {
            _expertise.Add(_knowledge.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            _network.NetworkKnowledges.Add(_agentId, _expertise);
            _network.NetworkKnowledges.InitializeExpertise(_agentId, false, 0);

            _result.HandleKnowledge(0);
            Assert.AreEqual(1, _result.Knowledges[0].Mean);
        }

        /// <summary>
        ///     2 knowledges for 2 agent
        /// </summary>
        [TestMethod]
        public void HandleKnowledge2Test()
        {
            _expertise.Add(_knowledge.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            _expertise.Add(_knowledge2.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            _network.NetworkKnowledges.Add(_agentId, _expertise);
            _network.NetworkKnowledges.Add(_agentId2, _expertise);
            _network.NetworkKnowledges.InitializeExpertise(_agentId, false, 0);
            _network.NetworkKnowledges.InitializeExpertise(_agentId2, false, 0);

            _result.HandleKnowledge(0);
            Assert.AreEqual(2, _result.Knowledges[0].Mean);
            Assert.AreEqual(0, _result.Knowledges[0].StandardDeviation);
            Assert.AreEqual(4, _result.Knowledges[0].Sum);
        }

        #endregion

        #region Belief

        /// <summary>
        ///     0 knowledge
        /// </summary>
        [TestMethod]
        public void HandleBelief0Test()
        {
            _result.HandleBelief(0);
            Assert.AreEqual(0, _result.Beliefs[0].Mean);
        }

        /// <summary>
        ///     1 knowledge
        /// </summary>
        [TestMethod]
        public void HandleBelief1Test()
        {
            _expertise.Add(_knowledge.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            _network.NetworkBeliefs.Add(_agentId, _expertise, BeliefLevel.NeitherAgreeNorDisagree);
            _network.NetworkBeliefs.InitializeBeliefs(_agentId, false);
            _network.NetworkBeliefs.GetAgentBelief(_agentId, _knowledge.Id).BeliefBits.SetBit(0, 1);
            _result.HandleBelief(0);
            Assert.AreEqual(1, _result.Beliefs[0].Mean);
        }

        /// <summary>
        ///     2 knowledges for 2 agent
        /// </summary>
        [TestMethod]
        public void HandleBelief2Test()
        {
            _expertise.Add(_knowledge.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            _expertise.Add(_knowledge2.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            _network.NetworkBeliefs.Add(_agentId, _expertise, BeliefLevel.NeitherAgreeNorDisagree);
            _network.NetworkBeliefs.Add(_agentId2, _expertise, BeliefLevel.NeitherAgreeNorDisagree);
            _network.NetworkBeliefs.InitializeBeliefs(_agentId, false);
            _network.NetworkBeliefs.InitializeBeliefs(_agentId2, false);
            _network.NetworkBeliefs.GetAgentBelief(_agentId, _knowledge.Id).BeliefBits.SetBit(0, 1);
            _network.NetworkBeliefs.GetAgentBelief(_agentId, _knowledge2.Id).BeliefBits.SetBit(0, 1);
            _network.NetworkBeliefs.GetAgentBelief(_agentId2, _knowledge.Id).BeliefBits.SetBit(0, 1);
            _network.NetworkBeliefs.GetAgentBelief(_agentId2, _knowledge2.Id).BeliefBits.SetBit(0, 1);
            _result.HandleBelief(0);
            Assert.AreEqual(2, _result.Beliefs[0].Mean);
            Assert.AreEqual(0, _result.Beliefs[0].StandardDeviation);
            Assert.AreEqual(4, _result.Beliefs[0].Sum);
        }

        #endregion
    }
}