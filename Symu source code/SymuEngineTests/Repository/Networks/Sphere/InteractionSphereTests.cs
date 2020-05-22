#region Licence

// Description: Symu - SymuEngineTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models;
using Symu.Classes.Agents.Models.CognitiveModel;
using Symu.Classes.Organization;
using Symu.Common;
using Symu.Repository.Networks;
using Symu.Repository.Networks.Activities;
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

        private readonly Belief _belief =
            new Belief(1, "1", 1, RandomGenerator.RandomBinary, BeliefWeightLevel.RandomWeight);

        private readonly AgentId _groupId = new AgentId(3, 2);
        private readonly Knowledge _knowledge = new Knowledge(1, "1", 1);
        private readonly OrganizationModels _model = new OrganizationModels();
        private readonly AgentTemplates _templates = new AgentTemplates();
        private Network _network;
        private InteractionSphere InteractionSphere => _network.InteractionSphere;

        [TestInitialize]
        public void Initialize()
        {
            _model.InteractionSphere.On = true;
            _network = new Network(_templates, _model);
            _agents.Add(_agentId1);
            _agents.Add(_agentId2);
            _network.NetworkKnowledges.AddKnowledge(_knowledge);
            _network.NetworkBeliefs.AddBelief(_belief);
            _network.NetworkActivities.AddActivity(_activity, _groupId);
        }

        /// <summary>
        ///     No interaction
        /// </summary>
        [TestMethod]
        public void GeAgentIdsForInteractionsTest()
        {
            InteractionSphere.SetSphere(true, _agents, _network);
            Assert.AreEqual(0, InteractionSphere.GetAgentIdsForInteractions(_agentId1, InteractionStrategy.Homophily,
                new InteractionPatterns()).Count());
        }

        /// <summary>
        ///     Without link & !_model.SphereUpdateOverTime
        /// </summary>
        [TestMethod]
        public void GeAgentIdsForNewInteractionsTest()
        {
            InteractionSphere.SetSphere(true, _agents, _network);
            Assert.AreEqual(0, InteractionSphere.GetAgentIdsForNewInteractions(_agentId1, InteractionStrategy.Homophily,
                new InteractionPatterns()).Count());
        }

        /// <summary>
        ///     Without link & _model.SphereUpdateOverTime
        /// </summary>
        [TestMethod]
        public void GeAgentIdsForNewInteractionsTest1()
        {
            _model.InteractionSphere.SphereUpdateOverTime = true;
            InteractionSphere.SetSphere(true, _agents, _network);
            Assert.AreEqual(1, InteractionSphere.GetAgentIdsForNewInteractions(_agentId1, InteractionStrategy.Homophily,
                new InteractionPatterns()).Count());
        }

        /// <summary>
        ///     With link
        /// </summary>
        [TestMethod]
        public void GeAgentIdsForNewInteractionsTest2()
        {
            _model.InteractionSphere.SphereUpdateOverTime = true;
            AddLink();
            InteractionSphere.SetSphere(true, _agents, _network);
            Assert.AreEqual(0, InteractionSphere.GetAgentIdsForNewInteractions(_agentId1, InteractionStrategy.Homophily,
                new InteractionPatterns()).Count());
        }

        /// <summary>
        ///     With no interaction
        /// </summary>
        [TestMethod]
        public void GetSphereWeightTest()
        {
            InteractionSphere.SetSphere(true, _agents, _network);
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
            InteractionSphere.SetSphere(true, _agents, _network);
            Assert.AreEqual(InteractionSphere.GetMaxSphereWeight(), InteractionSphere.GetSphereWeight());
        }

        [TestMethod]
        public void GetMaxSphereWeightTest()
        {
            InteractionSphere.SetSphere(true, _agents, _network);
            Assert.AreEqual(8, InteractionSphere.GetMaxSphereWeight());
        }

        #region common

        private void AddLink()
        {
            _network.NetworkLinks.AddLink(_agentId1, _agentId2);
        }

        private void AddKnowledge(AgentId agentId, KnowledgeLevel level)
        {
            _network.NetworkKnowledges.Add(agentId, _knowledge.Id, level, 0, -1);
            _network.NetworkKnowledges.InitializeExpertise(agentId, false, 0);
        }

        private void AddActivity(AgentId agentId)
        {
            _network.NetworkActivities.AddActivity(agentId, _activity.Name, _groupId);
        }

        private void AddBelief(AgentId agentId, float belief)
        {
            _network.NetworkBeliefs.Add(agentId, _belief, BeliefLevel.NoBelief);
            _network.NetworkBeliefs.InitializeBeliefs(agentId, false);
            _network.NetworkBeliefs.GetAgentBelief(agentId, _belief.Id).BeliefBits.SetBit(0, belief);
        }

        #endregion

        #region Homophily

        /// <summary>
        ///     Empty network
        /// </summary>
        [TestMethod]
        public void GetHomophilyTest()
        {
            InteractionSphere.SetSphere(true, _agents, _network);
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
            InteractionSphere.SetSphere(true, _agents, _network);
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
            InteractionSphere.SetSphere(true, _agents, _network);
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
            InteractionSphere.SetSphere(true, _agents, _network);
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
            Assert.AreEqual(0, InteractionSphere.SetRelativeBelief(_agentId1, _agentId2, _network.NetworkBeliefs));
        }

        /// <summary>
        ///     With same belief 1
        /// </summary>
        [TestMethod]
        public void SetRelativeBeliefTest1()
        {
            AddBelief(_agentId1, 1);
            AddBelief(_agentId2, 1);
            Assert.AreEqual(1, InteractionSphere.SetRelativeBelief(_agentId1, _agentId2, _network.NetworkBeliefs));
        }

        /// <summary>
        ///     With same belief -1
        /// </summary>
        [TestMethod]
        public void SetRelativeBeliefTest2()
        {
            AddBelief(_agentId1, -1);
            AddBelief(_agentId2, -1);
            Assert.AreEqual(1, InteractionSphere.SetRelativeBelief(_agentId1, _agentId2, _network.NetworkBeliefs));
        }

        /// <summary>
        ///     With same belief 0
        /// </summary>
        [TestMethod]
        public void SetRelativeBeliefTest3()
        {
            AddBelief(_agentId1, 0);
            AddBelief(_agentId2, 0);
            Assert.AreEqual(0, InteractionSphere.SetRelativeBelief(_agentId1, _agentId2, _network.NetworkBeliefs));
        }

        /// <summary>
        ///     With different belief
        /// </summary>
        [TestMethod]
        public void SetRelativeBeliefTest4()
        {
            AddBelief(_agentId1, -1);
            AddBelief(_agentId2, 1);
            Assert.AreEqual(-1, InteractionSphere.SetRelativeBelief(_agentId1, _agentId2, _network.NetworkBeliefs));
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
                InteractionSphere.SetRelativeKnowledge(_agentId1, _agentId2, _network.NetworkKnowledges));
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
                InteractionSphere.SetRelativeKnowledge(_agentId1, _agentId2, _network.NetworkKnowledges));
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
                InteractionSphere.SetRelativeKnowledge(_agentId1, _agentId2, _network.NetworkKnowledges));
        }

        #endregion

        #region SocialProximity

        /// <summary>
        ///     Without link
        /// </summary>
        [TestMethod]
        public void SetSocialProximityTest()
        {
            _network.NetworkLinks.SetMaxLinksCount();
            Assert.AreEqual(0, InteractionSphere.SetSocialProximity(_agentId1, _agentId2, _network.NetworkLinks));
        }

        /// <summary>
        ///     With active link
        /// </summary>
        [TestMethod]
        public void SetSocialProximityTest1()
        {
            AddLink();
            _network.NetworkLinks.SetMaxLinksCount();
            Assert.AreEqual(1, InteractionSphere.SetSocialProximity(_agentId1, _agentId2, _network.NetworkLinks));
        }

        /// <summary>
        ///     With passive link
        /// </summary>
        [TestMethod]
        public void SetSocialProximityTest2()
        {
            var networkLink = new NetworkLink(_agentId1, _agentId2);
            networkLink.Deactivate();
            _network.NetworkLinks.AddLink(_agentId1, _agentId2);
            Assert.AreEqual(0F, InteractionSphere.SetSocialProximity(_agentId1, _agentId2, _network.NetworkLinks));
        }

        #endregion

        #region Activities

        /// <summary>
        ///     Without Activity
        /// </summary>
        [TestMethod]
        public void SetRelativeActivityTest()
        {
            Assert.AreEqual(0, InteractionSphere.SetRelativeActivity(_agentId1, _agentId2, _network.NetworkActivities));
        }

        /// <summary>
        ///     With different activities level
        /// </summary>
        [TestMethod]
        public void SetRelativeActivityTest1()
        {
            AddActivity(_agentId1);
            AddActivity(_agentId2);
            Assert.AreEqual(1, InteractionSphere.SetRelativeActivity(_agentId1, _agentId2, _network.NetworkActivities));
        }

        #endregion
    }
}