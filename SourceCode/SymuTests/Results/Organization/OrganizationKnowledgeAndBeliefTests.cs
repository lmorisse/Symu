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
using Symu.Common.Interfaces.Entity;
using Symu.DNA.Knowledges;
using Symu.Repository.Entity;
using Symu.Repository.Networks.Beliefs;
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
        private AgentKnowledge _agentKnowledge;
        private AgentKnowledge _agentKnowledge2;


        [TestInitialize]
        public void Initialize()
        {
            var organization = new OrganizationEntity("1");
            _environment.SetOrganization(organization);
            _result = new KnowledgeAndBeliefResults(_environment);
            _environment.WhitePages.MetaNetwork.AddKnowledge(_knowledge, _environment.Organization.Models.BeliefWeightLevel);
            _environment.WhitePages.MetaNetwork.AddKnowledge(_knowledge2, _environment.Organization.Models.BeliefWeightLevel);
            _agent = TestCognitiveAgent.CreateInstance(new UId(1), _environment);
            _agent2 = TestCognitiveAgent.CreateInstance(new UId(2), _environment);
            _environment.Start();
            _agentKnowledge = new AgentKnowledge(_knowledge.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            _agentKnowledge2 = new AgentKnowledge(_knowledge2.Id, KnowledgeLevel.FullKnowledge, 0, -1);
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
            _expertise.Add(_agentKnowledge);
            _agent.BeliefsModel.AddBeliefs(_expertise,  BeliefLevel.NeitherAgreeNorDisagree);
            _agent.BeliefsModel.InitializeBeliefs(false);
            _agent.BeliefsModel.GetAgentBelief(_knowledge.Id).BeliefBits
                .SetBit(0, 1);

            _agent2.BeliefsModel.AddBeliefs(_expertise,  BeliefLevel.NeitherAgreeNorDisagree);
            _agent2.BeliefsModel.InitializeBeliefs(false);
            _agent2.BeliefsModel.GetAgentBelief(_knowledge.Id).BeliefBits
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
            _expertise.Add(_agentKnowledge);
            _expertise.Add(_agentKnowledge2);

            _agent.BeliefsModel.AddBeliefs(_expertise, BeliefLevel.NeitherAgreeNorDisagree);
            _agent.BeliefsModel.InitializeBeliefs(false);
            _agent.BeliefsModel.GetAgentBelief(_knowledge.Id).BeliefBits
                .SetBit(0, 1);
            _agent.BeliefsModel.GetAgentBelief(_knowledge2.Id).BeliefBits
                .SetBit(0, 1);

            _agent2.BeliefsModel.AddBeliefs(_expertise, BeliefLevel.NeitherAgreeNorDisagree);
            _agent2.BeliefsModel.InitializeBeliefs(false);
            _agent2.BeliefsModel.GetAgentBelief(_knowledge.Id).BeliefBits
                .SetBit(0, 1);
            _agent2.BeliefsModel.GetAgentBelief(_knowledge2.Id).BeliefBits
                .SetBit(0, 1);

            _result.HandleBelief();
            Assert.AreEqual(2, _result.Beliefs[0].Mean);
            Assert.AreEqual(0, _result.Beliefs[0].StandardDeviation);
            Assert.AreEqual(4, _result.Beliefs[0].Sum);
        }

        #endregion
    }
}