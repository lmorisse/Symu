#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Classes.Organization;
using Symu.Common;
using Symu.Common.Interfaces;
using Symu.Common.Interfaces.Agent;
using Symu.Repository.Entity;
using Symu.Repository.Networks;
using Symu.Repository.Networks.Beliefs;
using Symu.Repository.Networks.Knowledges;
using Symu.Repository.Networks.Link;
using Symu.Repository.Networks.Sphere;

#endregion

namespace SymuTests.Repository.Networks.Sphere
{
    [TestClass]
    public class InteractionSphereTests
    {
        private readonly Activity _activity = new Activity("1");
        private readonly AgentId _agentId1 = new AgentId(1, 1);
        private readonly AgentId _agentId2 = new AgentId(2, 1);
        private readonly List<AgentId> _agents = new List<AgentId>();
        private List<IAgentId> _iAgents => _agents.Cast<IAgentId>().ToList();

        private readonly Belief _belief =
            new Belief(1, "1", 1, RandomGenerator.RandomBinary, BeliefWeightLevel.RandomWeight);

        private readonly AgentId _groupId = new AgentId(3, 2);
        private readonly Knowledge _knowledge = new Knowledge(1, "1", 1);
        private readonly OrganizationModels _model = new OrganizationModels();
        private MetaNetwork _network;
        private InteractionSphere InteractionSphere => _network.InteractionSphere;

        [TestInitialize]
        public void Initialize()
        {
            _model.InteractionSphere.On = true;
            _network = new MetaNetwork(_model.InteractionSphere, _model.ImpactOfBeliefOnTask);
            _agents.Add(_agentId1);
            _agents.Add(_agentId2);
            _network.Knowledge.AddKnowledge(_knowledge);
            _network.Beliefs.AddBelief(_belief);
            _network.Activities.AddActivity(_activity, _groupId);
        }

        /// <summary>
        ///     No interaction
        /// </summary>
        [TestMethod]
        public void GeAgentIdsForInteractionsTest()
        {
            InteractionSphere.SetSphere(true, _iAgents, _network);
            Assert.AreEqual(0, InteractionSphere.GetAgentIdsForInteractions(_agentId1, InteractionStrategy.Homophily).Count());
        }

        /// <summary>
        ///     Without link & !_model.SphereUpdateOverTime
        /// </summary>
        [TestMethod]
        public void GeAgentIdsForNewInteractionsTest()
        {
            InteractionSphere.SetSphere(true, _iAgents, _network);
            Assert.AreEqual(0,
                InteractionSphere.GetAgentIdsForNewInteractions(_agentId1, InteractionStrategy.Homophily).Count());
        }

        /// <summary>
        ///     Without link & _model.SphereUpdateOverTime
        /// </summary>
        [TestMethod]
        public void GeAgentIdsForNewInteractionsTest1()
        {
            _model.InteractionSphere.SphereUpdateOverTime = true;
            InteractionSphere.SetSphere(true, _iAgents, _network);
            Assert.AreEqual(1,
                InteractionSphere.GetAgentIdsForNewInteractions(_agentId1, InteractionStrategy.Homophily).Count());
        }

        /// <summary>
        ///     With link
        /// </summary>
        [TestMethod]
        public void GeAgentIdsForNewInteractionsTest2()
        {
            _model.InteractionSphere.SphereUpdateOverTime = true;
            AddLink();
            InteractionSphere.SetSphere(true, _iAgents, _network);
            Assert.AreEqual(0,
                InteractionSphere.GetAgentIdsForNewInteractions(_agentId1, InteractionStrategy.Homophily).Count());
        }

        /// <summary>
        ///     With no interaction
        /// </summary>
        [TestMethod]
        public void GetSphereWeightTest()
        {
            InteractionSphere.SetSphere(true, _iAgents, _network);
            Assert.AreEqual(0, InteractionSphere.GetSphereWeight());
        }

        /// <summary>
        ///     With full interaction
        /// </summary>
        [TestMethod]
        public void GetSphereWeightTest1()
        {
            AddBelief(_agentId1, 1);
            AddBelief(_agentId2, 1);
            AddKnowledge(_agentId1, KnowledgeLevel.FullKnowledge);
            AddKnowledge(_agentId2, KnowledgeLevel.FullKnowledge);
            AddLink();
            AddActivity(_agentId1);
            AddActivity(_agentId2);
            InteractionSphere.SetSphere(true, _iAgents, _network);
            Assert.AreEqual(InteractionSphere.GetMaxSphereWeight(), InteractionSphere.GetSphereWeight());
        }

        [TestMethod]
        public void GetMaxSphereWeightTest()
        {
            InteractionSphere.SetSphere(true, _iAgents, _network);
            Assert.AreEqual(8, InteractionSphere.GetMaxSphereWeight());
        }

        #region common

        private void AddLink()
        {
            _network.Links.AddLink(_agentId1, _agentId2);
        }

        private void AddKnowledge(AgentId agentId, KnowledgeLevel level)
        {
            _network.Knowledge.Add(agentId, _knowledge.Id, level, 0, -1);
            _network.Knowledge.InitializeExpertise(agentId, false, 0);
        }

        private void AddActivity(AgentId agentId)
        {
            var agentActivity = new AgentActivity(agentId, _activity);
            _network.Activities.AddAgentActivity(agentId, _groupId, agentActivity);
        }

        private void AddBelief(AgentId agentId, float belief)
        {
            _network.Beliefs.Add(agentId, _belief, BeliefLevel.NoBelief);
            _network.Beliefs.InitializeBeliefs(agentId, false);
            _network.Beliefs.GetAgentBelief(agentId, _belief.Id).BeliefBits.SetBit(0, belief);
        }

        #endregion

        #region Homophily

        /// <summary>
        ///     Empty network
        /// </summary>
        [TestMethod]
        public void GetHomophilyTest()
        {
            InteractionSphere.SetSphere(true, _iAgents, _network);
            Assert.AreEqual(0, InteractionSphere.GetHomophily(_agentId1, _agentId2));
        }

        /// <summary>
        ///     Linked agents
        /// </summary>
        [TestMethod]
        public void GetHomophilyTest1()
        {
            AddLink();
            _model.InteractionSphere.SetInteractionPatterns(InteractionStrategy.SocialDemographics);
            InteractionSphere.SetSphere(true, _iAgents, _network);
            Assert.AreEqual(1, InteractionSphere.GetHomophily(_agentId1, _agentId2));
        }

        /// <summary>
        ///     Knowledge
        /// </summary>
        [TestMethod]
        public void GetHomophilyTest2()
        {
            AddKnowledge(_agentId1, KnowledgeLevel.FullKnowledge);
            AddKnowledge(_agentId2, KnowledgeLevel.FullKnowledge);
            InteractionSphere.SetSphere(true, _iAgents, _network);
            Assert.AreEqual(1, InteractionSphere.GetHomophily(_agentId1, _agentId2));
        }

        /// <summary>
        ///     Belief
        /// </summary>
        [TestMethod]
        public void GetHomophilyTest3()
        {
            AddBelief(_agentId1, 1);
            AddBelief(_agentId2, 1);
            InteractionSphere.SetSphere(true, _iAgents, _network);
            Assert.AreEqual(1, InteractionSphere.GetHomophily(_agentId1, _agentId2));
        }

        #endregion

        #region Belief

        /// <summary>
        ///     Without belief
        /// </summary>
        [TestMethod]
        public void SetRelativeBeliefTest()
        {
            Assert.AreEqual(0, InteractionSphere.SetRelativeBelief(_agentId1, _agentId2, _network.Beliefs));
        }

        /// <summary>
        ///     With same belief 1
        /// </summary>
        [TestMethod]
        public void SetRelativeBeliefTest1()
        {
            AddBelief(_agentId1, 1);
            AddBelief(_agentId2, 1);
            Assert.AreEqual(1, InteractionSphere.SetRelativeBelief(_agentId1, _agentId2, _network.Beliefs));
        }

        /// <summary>
        ///     With same belief -1
        /// </summary>
        [TestMethod]
        public void SetRelativeBeliefTest2()
        {
            AddBelief(_agentId1, -1);
            AddBelief(_agentId2, -1);
            Assert.AreEqual(1, InteractionSphere.SetRelativeBelief(_agentId1, _agentId2, _network.Beliefs));
        }

        /// <summary>
        ///     With same belief 0
        /// </summary>
        [TestMethod]
        public void SetRelativeBeliefTest3()
        {
            AddBelief(_agentId1, 0);
            AddBelief(_agentId2, 0);
            Assert.AreEqual(0, InteractionSphere.SetRelativeBelief(_agentId1, _agentId2, _network.Beliefs));
        }

        /// <summary>
        ///     With different belief
        /// </summary>
        [TestMethod]
        public void SetRelativeBeliefTest4()
        {
            AddBelief(_agentId1, -1);
            AddBelief(_agentId2, 1);
            Assert.AreEqual(-1, InteractionSphere.SetRelativeBelief(_agentId1, _agentId2, _network.Beliefs));
        }

        #endregion

        #region Knowledge

        /// <summary>
        ///     Without knowledge
        /// </summary>
        [TestMethod]
        public void SetRelativeExpertiseTest()
        {
            Assert.AreEqual(0,
                InteractionSphere.SetRelativeKnowledge(_agentId1, _agentId2, _network.Knowledge));
        }

        /// <summary>
        ///     With different knowledge level
        /// </summary>
        [TestMethod]
        public void SetRelativeExpertiseTest1()
        {
            AddKnowledge(_agentId1, KnowledgeLevel.FullKnowledge);
            AddKnowledge(_agentId2, KnowledgeLevel.NoKnowledge);
            Assert.AreEqual(0,
                InteractionSphere.SetRelativeKnowledge(_agentId1, _agentId2, _network.Knowledge));
        }

        /// <summary>
        ///     With same knowledge level
        /// </summary>
        [TestMethod]
        public void SetRelativeExpertiseTest2()
        {
            AddKnowledge(_agentId1, KnowledgeLevel.FullKnowledge);
            AddKnowledge(_agentId2, KnowledgeLevel.FullKnowledge);
            Assert.AreEqual(1,
                InteractionSphere.SetRelativeKnowledge(_agentId1, _agentId2, _network.Knowledge));
        }

        #endregion

        #region SocialProximity

        /// <summary>
        ///     Without link
        /// </summary>
        [TestMethod]
        public void SetSocialProximityTest()
        {
            _network.Links.SetMaxLinksCount();
            Assert.AreEqual(0, InteractionSphere.SetSocialProximity(_agentId1, _agentId2, _network.Links));
        }

        /// <summary>
        ///     With active link
        /// </summary>
        [TestMethod]
        public void SetSocialProximityTest1()
        {
            AddLink();
            _network.Links.SetMaxLinksCount();
            Assert.AreEqual(1, InteractionSphere.SetSocialProximity(_agentId1, _agentId2, _network.Links));
        }

        /// <summary>
        ///     With passive link
        /// </summary>
        [TestMethod]
        public void SetSocialProximityTest2()
        {
            var networkLink = new LinkEntity(_agentId1, _agentId2);
            networkLink.Deactivate();
            _network.Links.AddLink(_agentId1, _agentId2);
            Assert.AreEqual(0F, InteractionSphere.SetSocialProximity(_agentId1, _agentId2, _network.Links));
        }

        #endregion

        #region Activities

        /// <summary>
        ///     Without Activity
        /// </summary>
        [TestMethod]
        public void SetRelativeActivityTest()
        {
            Assert.AreEqual(0, InteractionSphere.SetRelativeActivity(_agentId1, _agentId2, _network.Activities));
        }

        /// <summary>
        ///     With different activities level
        /// </summary>
        [TestMethod]
        public void SetRelativeActivityTest1()
        {
            AddActivity(_agentId1);
            AddActivity(_agentId2);
            Assert.AreEqual(1, InteractionSphere.SetRelativeActivity(_agentId1, _agentId2, _network.Activities));
        }

        #endregion
    }
}