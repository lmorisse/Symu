#region Licence

// Description: Symu - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models;
using Symu.Classes.Organization;
using Symu.Environment;
using Symu.Repository.Networks;
using Symu.Repository.Networks.Beliefs;
using Symu.Repository.Networks.Knowledges;
using Symu.Results.Organization;
using SymuTests.Helpers;

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
        private OrganizationKnowledgeAndBelief _result;
        private readonly TestEnvironment _environment = new TestEnvironment();


        [TestInitialize]
        public void Initialize()
        {
            _result = new OrganizationKnowledgeAndBelief(_environment);
            _environment.WhitePages.Network.AddKnowledge(_knowledge);
            _environment.WhitePages.Network.AddKnowledge(_knowledge2);
        }

        #region Knowledge

        /// <summary>
        ///     0 knowledge
        /// </summary>
        [TestMethod]
        public void HandleKnowledge0Test()
        {
            _result.HandleKnowledge(0);
            Assert.AreEqual(0, _result.Knowledge[0].Mean);
        }

        /// <summary>
        ///     1 knowledge
        /// </summary>
        [TestMethod]
        public void HandleKnowledge1Test()
        {
            _expertise.Add(_knowledge.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            _environment.WhitePages.Network.NetworkKnowledges.Add(_agentId, _expertise);
            _environment.WhitePages.Network.NetworkKnowledges.InitializeExpertise(_agentId, false, 0);

            _result.HandleKnowledge(0);
            Assert.AreEqual(1, _result.Knowledge[0].Mean);
        }

        /// <summary>
        ///     2 knowledges for 2 agent
        /// </summary>
        [TestMethod]
        public void HandleKnowledge2Test()
        {
            _expertise.Add(_knowledge.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            _expertise.Add(_knowledge2.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            _environment.WhitePages.Network.NetworkKnowledges.Add(_agentId, _expertise);
            _environment.WhitePages.Network.NetworkKnowledges.Add(_agentId2, _expertise);
            _environment.WhitePages.Network.NetworkKnowledges.InitializeExpertise(_agentId, false, 0);
            _environment.WhitePages.Network.NetworkKnowledges.InitializeExpertise(_agentId2, false, 0);

            _result.HandleKnowledge(0);
            Assert.AreEqual(2, _result.Knowledge[0].Mean);
            Assert.AreEqual(0, _result.Knowledge[0].StandardDeviation);
            Assert.AreEqual(4, _result.Knowledge[0].Sum);
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
            _environment.WhitePages.Network.NetworkBeliefs.Add(_agentId, _expertise, BeliefLevel.NeitherAgreeNorDisagree);
            _environment.WhitePages.Network.NetworkBeliefs.InitializeBeliefs(_agentId, false);
            _environment.WhitePages.Network.NetworkBeliefs.GetAgentBelief(_agentId, _knowledge.Id).BeliefBits.SetBit(0, 1);
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
            _environment.WhitePages.Network.NetworkBeliefs.Add(_agentId, _expertise, BeliefLevel.NeitherAgreeNorDisagree);
            _environment.WhitePages.Network.NetworkBeliefs.Add(_agentId2, _expertise, BeliefLevel.NeitherAgreeNorDisagree);
            _environment.WhitePages.Network.NetworkBeliefs.InitializeBeliefs(_agentId, false);
            _environment.WhitePages.Network.NetworkBeliefs.InitializeBeliefs(_agentId2, false);
            _environment.WhitePages.Network.NetworkBeliefs.GetAgentBelief(_agentId, _knowledge.Id).BeliefBits.SetBit(0, 1);
            _environment.WhitePages.Network.NetworkBeliefs.GetAgentBelief(_agentId, _knowledge2.Id).BeliefBits.SetBit(0, 1);
            _environment.WhitePages.Network.NetworkBeliefs.GetAgentBelief(_agentId2, _knowledge.Id).BeliefBits.SetBit(0, 1);
            _environment.WhitePages.Network.NetworkBeliefs.GetAgentBelief(_agentId2, _knowledge2.Id).BeliefBits.SetBit(0, 1);
            _result.HandleBelief(0);
            Assert.AreEqual(2, _result.Beliefs[0].Mean);
            Assert.AreEqual(0, _result.Beliefs[0].StandardDeviation);
            Assert.AreEqual(4, _result.Beliefs[0].Sum);
        }

        #endregion
    }
}