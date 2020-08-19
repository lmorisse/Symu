#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Classes.Organization;
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
        private readonly TestEnvironment _environment = new TestEnvironment();
        private readonly AgentExpertise _expertise = new AgentExpertise();
        private readonly Knowledge _knowledge = new Knowledge(1, "1", 1);
        private readonly Knowledge _knowledge2 = new Knowledge(2, "2", 1);
        private KnowledgeAndBeliefResults _result;
        private TestCognitiveAgent _agent;
        private TestCognitiveAgent _agent2;


        [TestInitialize]
        public void Initialize()
        {
            var organization = new OrganizationEntity("1");
            _environment.SetOrganization(organization);
            _result = new KnowledgeAndBeliefResults(_environment);
            _environment.WhitePages.MetaNetwork.AddKnowledge(_knowledge);
            _environment.WhitePages.MetaNetwork.AddKnowledge(_knowledge2);
            _agent = new TestCognitiveAgent(1, _environment);
            _agent2 = new TestCognitiveAgent(2, _environment);
            _environment.Start();
        }

        #region Knowledge

        /// <summary>
        ///     0 knowledge
        /// </summary>
        [TestMethod]
        public void HandleKnowledge0Test()
        {
            _result.HandleKnowledge();
            Assert.AreEqual(0, _result.Knowledge[0].Mean);
        }

        /// <summary>
        ///     1 knowledge
        /// </summary>
        [TestMethod]
        public void HandleKnowledge1Test()
        {
            _agent.KnowledgeModel.AddKnowledge(_knowledge.Id, KnowledgeLevel.FullKnowledge,0, -1);
            _agent.KnowledgeModel.InitializeExpertise(0);
            _agent2.KnowledgeModel.AddKnowledge(_knowledge.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            _agent2.KnowledgeModel.InitializeExpertise(0);
            _result.HandleKnowledge();
            Assert.AreEqual(1, _result.Knowledge[0].Mean);
        }

        /// <summary>
        ///     2 knowledges for 2 agent
        /// </summary>
        [TestMethod]
        public void HandleKnowledge2Test()
        {
            _agent.KnowledgeModel.AddKnowledge(_knowledge.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            _agent.KnowledgeModel.AddKnowledge(_knowledge2.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            _agent.KnowledgeModel.InitializeExpertise(0);
            _agent2.KnowledgeModel.AddKnowledge(_knowledge.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            _agent2.KnowledgeModel.AddKnowledge(_knowledge2.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            _agent2.KnowledgeModel.InitializeExpertise(0);

            _result.HandleKnowledge();
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
            _result.HandleBelief();
            Assert.AreEqual(0, _result.Beliefs[0].Mean);
        }

        /// <summary>
        ///     1 knowledge
        /// </summary>
        [TestMethod]
        public void HandleBelief1Test()
        {
            _expertise.Add(_knowledge.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            _environment.WhitePages.MetaNetwork.Beliefs.Add(_agent.AgentId, _expertise,
                BeliefLevel.NeitherAgreeNorDisagree);
            _environment.WhitePages.MetaNetwork.Beliefs.Add(_agent2.AgentId, _expertise,
                BeliefLevel.NeitherAgreeNorDisagree);
            _environment.WhitePages.MetaNetwork.Beliefs.InitializeBeliefs(_agent.AgentId, false);
            _environment.WhitePages.MetaNetwork.Beliefs.GetAgentBelief(_agent.AgentId, _knowledge.Id).BeliefBits
                .SetBit(0, 1);
            _environment.WhitePages.MetaNetwork.Beliefs.InitializeBeliefs(_agent2.AgentId, false);
            _environment.WhitePages.MetaNetwork.Beliefs.GetAgentBelief(_agent2.AgentId, _knowledge.Id).BeliefBits
                .SetBit(0, 1);
            _result.HandleBelief();
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
            _environment.WhitePages.MetaNetwork.Beliefs.Add(_agent.AgentId, _expertise,
                BeliefLevel.NeitherAgreeNorDisagree);
            _environment.WhitePages.MetaNetwork.Beliefs.Add(_agent2.AgentId, _expertise,
                BeliefLevel.NeitherAgreeNorDisagree);
            _environment.WhitePages.MetaNetwork.Beliefs.InitializeBeliefs(_agent.AgentId, false);
            _environment.WhitePages.MetaNetwork.Beliefs.InitializeBeliefs(_agent2.AgentId, false);
            _environment.WhitePages.MetaNetwork.Beliefs.GetAgentBelief(_agent.AgentId, _knowledge.Id).BeliefBits
                .SetBit(0, 1);
            _environment.WhitePages.MetaNetwork.Beliefs.GetAgentBelief(_agent.AgentId, _knowledge2.Id).BeliefBits
                .SetBit(0, 1);
            _environment.WhitePages.MetaNetwork.Beliefs.GetAgentBelief(_agent2.AgentId, _knowledge.Id).BeliefBits
                .SetBit(0, 1);
            _environment.WhitePages.MetaNetwork.Beliefs.GetAgentBelief(_agent2.AgentId, _knowledge2.Id).BeliefBits
                .SetBit(0, 1);
            _result.HandleBelief();
            Assert.AreEqual(2, _result.Beliefs[0].Mean);
            Assert.AreEqual(0, _result.Beliefs[0].StandardDeviation);
            Assert.AreEqual(4, _result.Beliefs[0].Sum);
        }

        #endregion
    }
}