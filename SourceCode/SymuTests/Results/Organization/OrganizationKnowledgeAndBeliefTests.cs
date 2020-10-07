#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Environment;
using Symu.Repository.Entities;
using Symu.Results.Organization;
using SymuTests.Helpers;
using ActorKnowledge = Symu.Repository.Edges.ActorKnowledge;

#endregion

namespace SymuTests.Results.Organization
{
    [TestClass]
    public class OrganizationKnowledgeAndBeliefTests: BaseTestClass
    {
        private Knowledge _knowledge;
        private Knowledge _knowledge1;
        private Belief _belief;
        private Belief _belief1;
        private KnowledgeAndBeliefResults _result;
        private TestCognitiveAgent _agent;
        private TestCognitiveAgent _agent1;


        [TestInitialize]
        public void Initialize()
        {
            // Entities
            Organization.Models.SetOn(1);
            _result = new KnowledgeAndBeliefResults(Environment);
            _knowledge = new Knowledge(Organization.MetaNetwork, Organization.Models, "1", 1);
            _knowledge1 = new Knowledge(Organization.MetaNetwork, Organization.Models, "2", 1);
            _belief = _knowledge.AssociatedBelief;
            _belief1 = _knowledge1.AssociatedBelief;

            Environment.SetOrganization(Organization);
            Simulation.Initialize(Environment);


            // Agents
            _agent = TestCognitiveAgent.CreateInstance(Environment);
            _agent.Start();
            _agent1 = TestCognitiveAgent.CreateInstance(Environment);
            _agent1.Start();
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
            _agent.KnowledgeModel.AddKnowledge(_knowledge.EntityId, KnowledgeLevel.FullKnowledge,0, -1);
            _agent.KnowledgeModel.InitializeExpertise(0);
            _agent1.KnowledgeModel.AddKnowledge(_knowledge.EntityId, KnowledgeLevel.FullKnowledge, 0, -1);
            _agent1.KnowledgeModel.InitializeExpertise(0);
            _result.HandleKnowledge();
            Assert.AreEqual(1, _result.Knowledge[0].Mean);
        }

        /// <summary>
        ///     2 knowledges for 2 agent
        /// </summary>
        [TestMethod]
        public void HandleKnowledge2Test()
        {
            _agent.KnowledgeModel.AddKnowledge(_knowledge.EntityId, KnowledgeLevel.FullKnowledge, 0, -1);
            _agent.KnowledgeModel.AddKnowledge(_knowledge1.EntityId, KnowledgeLevel.FullKnowledge, 0, -1);
            _agent.KnowledgeModel.InitializeExpertise(0);
            _agent1.KnowledgeModel.AddKnowledge(_knowledge.EntityId, KnowledgeLevel.FullKnowledge, 0, -1);
            _agent1.KnowledgeModel.AddKnowledge(_knowledge1.EntityId, KnowledgeLevel.FullKnowledge, 0, -1);
            _agent1.KnowledgeModel.InitializeExpertise(0);

            _result.HandleKnowledge();
            Assert.AreEqual(2, _result.Knowledge[0].Mean);
            Assert.AreEqual(0, _result.Knowledge[0].StandardDeviation);
            Assert.AreEqual(4, _result.Knowledge[0].Sum);
        }

        #endregion

        #region Beliefs

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
            _agent.BeliefsModel.AddBelief(_belief.EntityId, BeliefLevel.NeitherAgreeNorDisagree);
            _agent.BeliefsModel.InitializeBeliefs(false);
            _agent.BeliefsModel.GetActorBelief(_belief.EntityId).BeliefBits
                .SetBit(0, 1);

            _agent1.BeliefsModel.AddBelief(_belief.EntityId, BeliefLevel.NeitherAgreeNorDisagree);
            _agent1.BeliefsModel.InitializeBeliefs(false);
            _agent1.BeliefsModel.SetBelief(_belief.EntityId,0, 1);

            _result.HandleBelief();
            Assert.AreEqual(1, _result.Beliefs[0].Mean);
        }

        /// <summary>
        ///     2 knowledges for 2 agent
        /// </summary>
        [TestMethod]
        public void HandleBelief2Test()
        {
            _agent.BeliefsModel.AddBelief(_belief.EntityId, BeliefLevel.NeitherAgreeNorDisagree);
            _agent.BeliefsModel.AddBelief(_belief1.EntityId, BeliefLevel.NeitherAgreeNorDisagree);
            _agent.BeliefsModel.InitializeBeliefs(false);
            _agent.BeliefsModel.SetBelief(_belief.EntityId,0, 1);
            _agent.BeliefsModel.SetBelief(_belief1.EntityId,0, 1);

            _agent1.BeliefsModel.AddBelief(_belief.EntityId, BeliefLevel.NeitherAgreeNorDisagree);
            _agent1.BeliefsModel.AddBelief(_belief1.EntityId, BeliefLevel.NeitherAgreeNorDisagree);
            _agent1.BeliefsModel.InitializeBeliefs(false);
            _agent1.BeliefsModel.SetBelief(_belief.EntityId,0, 1);
            _agent1.BeliefsModel.SetBelief(_belief1.EntityId,0, 1);

            _result.HandleBelief();
            Assert.AreEqual(2, _result.Beliefs[0].Mean);
            Assert.AreEqual(0, _result.Beliefs[0].StandardDeviation);
            Assert.AreEqual(4, _result.Beliefs[0].Sum);
        }

        #endregion
    }
}