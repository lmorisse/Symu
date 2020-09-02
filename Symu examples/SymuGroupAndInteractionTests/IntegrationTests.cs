#region Licence

// Description: SymuBiz - SymuGroupAndInteractionTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Classes.Organization;
using Symu.Classes.Scenario;
using Symu.DNA.Networks.TwoModesNetworks.Sphere;
using Symu.Engine;
using Symu.Repository.Entity;
using SymuGroupAndInteraction.Classes;

#endregion


namespace SymuGroupAndInteractionTests
{
    /// <summary>
    ///     Integration tests using SymuEngine
    /// </summary>
    [TestClass]
    public class IntegrationTests
    {
        private const int NumberOfSteps = 31; // 2 organizationFlexibility computations
        private readonly ExampleEnvironment _environment = new ExampleEnvironment();
        private readonly OrganizationEntity _organization = new OrganizationEntity("1");
        private readonly SymuEngine _simulation = new SymuEngine();

        [TestInitialize]
        public void Initialize()
        {
            _organization.Templates.Human.Cognitive.InteractionPatterns.AllowNewInteractions = false;
            _environment.SetOrganization(_organization);
            _simulation.SetEnvironment(_environment);
            _environment.SetDebug(true);
            var scenario = TimeBasedScenario.CreateInstance(_environment);
            scenario.NumberOfSteps = NumberOfSteps;
            _simulation.AddScenario(scenario);
        }

        public void SetInteractionPatterns(InteractionStrategy strategy)
        {
            _organization.Models.InteractionSphere.SetInteractionPatterns(strategy);
            _organization.Templates.Human.Cognitive.InteractionPatterns.SetInteractionPatterns(strategy);
        }

        private int GetNotAcceptedMessages()
        {
            return _environment.WhitePages.AllAgents().Sum(agent => agent.MessageProcessor.NotAcceptedMessages.Count);
        }

        #region n groups- 10 workers

        /// <summary>
        ///     Interaction sphere == Coworkers because no opportunities to grow the sphere of interaction which is initialized
        ///     with coworkers
        /// </summary>
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [TestMethod]
        public void KnowledgeSphereCount(int groupsCount)
        {
            SetInteractionPatterns(InteractionStrategy.Knowledge);
            _environment.GroupsCount = (byte) groupsCount;
            _environment.WorkersCount = 10;
            _organization.Templates.Human.Cognitive.InteractionPatterns.LimitNumberOfNewInteractions = true;
            _organization.Templates.Human.Cognitive.InteractionPatterns.MaxNumberOfNewInteractions = 1;
            _environment.Knowledge = 1;
            _simulation.Process();
            var links = _environment.WhitePages.MetaNetwork.Interaction.Count;
            var triads = _environment.IterationResult.OrganizationFlexibility.Triads.Last().ActualNumber;
            _simulation.Process();
            Assert.AreEqual(links, _environment.WhitePages.MetaNetwork.Interaction.Count);
            Assert.AreEqual(triads, _environment.IterationResult.OrganizationFlexibility.Triads.Last().ActualNumber);
        }

        #endregion

        #region 0 group

        [DataRow(0)]
        [DataRow(1)]
        [TestMethod]
        public void NoGroupsOneMemberTest(int agentCount)
        {
            _environment.GroupsCount = 0;
            _environment.WorkersCount = (byte) agentCount;
            _simulation.Process();
            Assert.AreEqual(0F, GetNotAcceptedMessages());
            Assert.AreEqual(0F, _environment.Messages.Result.SentMessagesCount);
            Assert.AreEqual(0F, _environment.IterationResult.OrganizationFlexibility.Links.Last().Density);
            Assert.AreEqual(0F, _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density);
            Assert.AreEqual(0F, _environment.IterationResult.OrganizationFlexibility.Sphere.Last().Density);
        }

        #endregion

        #region 1 group

        [DataRow(0)]
        [DataRow(1)]
        [TestMethod]
        public void OneGroupsOneMemberTest(int agentCount)
        {
            _environment.GroupsCount = 0;
            _environment.WorkersCount = (byte) agentCount;
            _organization.Templates.Human.Cognitive.InteractionPatterns.AllowNewInteractions = true;
            _simulation.Process();
            Assert.AreEqual(0F, GetNotAcceptedMessages());
            Assert.AreEqual(0F, _environment.IterationResult.OrganizationFlexibility.Links.Last().Density);
            Assert.AreEqual(0F, _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density);
            Assert.AreEqual(0F, _environment.IterationResult.OrganizationFlexibility.Sphere.Last().Density);
        }

        [DataRow(2, 100, 0, 100)]
        [DataRow(3, 100, 100, 100)]
        [DataRow(4, 100, 100, 100)]
        [TestMethod]
        public void LinksCountTest(int workers, float links, float triads, float sphere)
        {
            SetInteractionPatterns(InteractionStrategy.Knowledge);
            _environment.GroupsCount = 1;
            _environment.WorkersCount = (byte) workers;
            _simulation.Process();
            Assert.AreEqual(links, _environment.IterationResult.OrganizationFlexibility.Links.Last().Density);
            Assert.AreEqual(triads, _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density);
            Assert.AreEqual(sphere, _environment.IterationResult.OrganizationFlexibility.Sphere.Last().Density);
        }

        /// <summary>
        ///     Randomly generated
        /// </summary>
        [TestMethod]
        public void TriadsTest5()
        {
            SetInteractionPatterns(InteractionStrategy.Knowledge);
            _environment.GroupsCount = 1;
            _environment.WorkersCount = 3;
            _organization.Models.InteractionSphere.RandomlyGeneratedSphere = true;
            _organization.Models.InteractionSphere.MinSphereDensity = 1;
            _organization.Models.InteractionSphere.MaxSphereDensity = 1;
            _simulation.Process();
            Assert.AreEqual(100F, _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density);
        }

        /// <summary>
        ///     knowledge by group
        /// </summary>
        [TestMethod]
        public void CoworkersCountTest()
        {
            _environment.GroupsCount = 1;
            _environment.WorkersCount = 10;
            _environment.Knowledge = 1;
            SetInteractionPatterns(InteractionStrategy.Knowledge);
            _simulation.Process();
            var links = _environment.WhitePages.MetaNetwork.Interaction.Count;
            var triads = _environment.IterationResult.OrganizationFlexibility.Triads.Last().ActualNumber;
            // results should be a multiple of groups count, because interaction sphere can't change
            _environment.GroupsCount = 2;
            _simulation.Process();
            Assert.AreEqual(links * _environment.GroupsCount, _environment.WhitePages.MetaNetwork.Interaction.Count);
            //Assert.AreEqual(triads * _environment.GroupsCount,
            //    _environment.IterationResult.OrganizationFlexibility.Triads.Last().ActualNumber);
            _environment.GroupsCount = 3;
            _simulation.Process();
            Assert.AreEqual(links * _environment.GroupsCount, _environment.WhitePages.MetaNetwork.Interaction.Count);
            //Assert.AreEqual(triads * _environment.GroupsCount,
            //    _environment.IterationResult.OrganizationFlexibility.Triads.Last().ActualNumber);
        }

        /// <summary>
        ///     With new interactions
        /// </summary>
        [TestMethod]
        public void NewInteractionTest()
        {
            _environment.GroupsCount = 1;
            _environment.WorkersCount = 3;
            _simulation.Process();
            Assert.AreEqual(0F, GetNotAcceptedMessages());
        }

        #endregion

        #region 2 groups - 1 member

        /// <summary>
        ///     Same Knowledge
        /// </summary>
        [TestMethod]
        public void TwoGroupsOneMemberTest()
        {
            SetInteractionPatterns(InteractionStrategy.Knowledge);
            _environment.GroupsCount = 2;
            _environment.WorkersCount = 1;
            _environment.Knowledge = 0;
            _simulation.Process();
            Assert.AreEqual(0F, _environment.IterationResult.OrganizationFlexibility.Links.Last().Density);
            Assert.AreEqual(0F, _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density);
            Assert.AreEqual(100F, _environment.IterationResult.OrganizationFlexibility.Sphere.Last().Density);
        }

        /// <summary>
        ///     By group Knowledge
        /// </summary>
        [TestMethod]
        public void TwoGroupsOneMemberTest1()
        {
            SetInteractionPatterns(InteractionStrategy.Knowledge);
            _environment.GroupsCount = 2;
            _environment.WorkersCount = 1;
            _environment.Knowledge = 1;
            _simulation.Process();
            Assert.AreEqual(0F, _environment.IterationResult.OrganizationFlexibility.Links.Last().Density);
            Assert.AreEqual(0F, _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density);
            Assert.AreEqual(0F, _environment.IterationResult.OrganizationFlexibility.Sphere.Last().Density);
        }

        /// <summary>
        ///     Allow interaction - threshold = 0
        /// </summary>
        [TestMethod]
        public void TwoGroupsOneMemberTest2()
        {
            SetInteractionPatterns(InteractionStrategy.Knowledge);
            _environment.GroupsCount = 2;
            _environment.WorkersCount = 1;
            _environment.Knowledge = 1;
            _organization.Templates.Human.Cognitive.InteractionPatterns.AllowNewInteractions = true;
            _organization.Templates.Human.Cognitive.InteractionPatterns.ThresholdForNewInteraction = 0;
            _simulation.Process();
            Assert.AreEqual(0F, _environment.IterationResult.OrganizationFlexibility.Links.Last().Density);
            Assert.AreEqual(0F, _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density);
            Assert.AreEqual(0F, _environment.IterationResult.OrganizationFlexibility.Sphere.Last().Density);
            Assert.IsTrue(GetNotAcceptedMessages() > 0);
        }

        /// <summary>
        ///     Allow interaction - threshold = 1
        /// </summary>
        [TestMethod]
        public void TwoGroupsOneMemberTest3()
        {
            SetInteractionPatterns(InteractionStrategy.Knowledge);
            _environment.GroupsCount = 2;
            _environment.WorkersCount = 1;
            _environment.Knowledge = 1;
            _organization.Templates.Human.Cognitive.InteractionPatterns.AllowNewInteractions = true;
            _organization.Templates.Human.Cognitive.InteractionPatterns.ThresholdForNewInteraction = 1;
            _simulation.Process();
            Assert.AreEqual(100F, _environment.IterationResult.OrganizationFlexibility.Links.Last().Density);
            Assert.AreEqual(0F, _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density);
            Assert.AreEqual(0F, _environment.IterationResult.OrganizationFlexibility.Sphere.Last().Density);
            Assert.AreEqual(0, GetNotAcceptedMessages());
        }

        #endregion

        #region 2 groups > 1 member

        /// <summary>
        ///     With same knowledge
        /// </summary>
        [TestMethod]
        public void TriadsTest()
        {
            SetInteractionPatterns(InteractionStrategy.Knowledge);
            _environment.GroupsCount = 2;
            _environment.WorkersCount = 3;
            _environment.Knowledge = 0;
            _simulation.Process();
            Assert.AreEqual(40.0F, _environment.IterationResult.OrganizationFlexibility.Links.Last().Density);
            Assert.AreEqual(100.0F, _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density);
            Assert.AreEqual(100.0F, _environment.IterationResult.OrganizationFlexibility.Sphere.Last().Density);
        }

        /// <summary>
        ///     With knowledge by group
        /// </summary>
        [TestMethod]
        public void TriadsTest1()
        {
            SetInteractionPatterns(InteractionStrategy.Knowledge);
            _environment.GroupsCount = 2;
            _environment.WorkersCount = 3;
            _environment.Knowledge = 1;
            _simulation.Process();
            Assert.AreEqual(40.0F, _environment.IterationResult.OrganizationFlexibility.Links.Last().Density);
            Assert.AreEqual(10.0F, _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density);
            Assert.AreEqual(40.0F, _environment.IterationResult.OrganizationFlexibility.Sphere.Last().Density);
        }

        /// <summary>
        ///     With same activity
        /// </summary>
        [TestMethod]
        public void TriadsTest2()
        {
            SetInteractionPatterns(InteractionStrategy.Activities);
            _environment.GroupsCount = 2;
            _environment.WorkersCount = 3;
            _environment.Activities = 0;
            _simulation.Process();
            Assert.AreEqual(40.0F, _environment.IterationResult.OrganizationFlexibility.Links.Last().Density);
            Assert.AreEqual(100.0F, _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density);
            Assert.AreEqual(100.0F, _environment.IterationResult.OrganizationFlexibility.Sphere.Last().Density);
        }

        /// <summary>
        ///     With activity by group
        /// </summary>
        [TestMethod]
        public void TriadsTest3()
        {
            SetInteractionPatterns(InteractionStrategy.Activities);
            _environment.GroupsCount = 2;
            _environment.WorkersCount = 3;
            _environment.Activities = 1;
            _simulation.Process();
            Assert.AreEqual(40.0F, _environment.IterationResult.OrganizationFlexibility.Links.Last().Density);
            Assert.AreEqual(10.0F, _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density);
            Assert.AreEqual(40.0F, _environment.IterationResult.OrganizationFlexibility.Sphere.Last().Density);
        }

        /// <summary>
        ///     With random no knowledge
        /// </summary>
        [TestMethod]
        public void TriadsTest4()
        {
            SetInteractionPatterns(InteractionStrategy.Knowledge);
            _environment.GroupsCount = 2;
            _environment.WorkersCount = 3;
            _environment.Knowledge = 2;
            _environment.KnowledgeLevel = KnowledgeLevel.NoKnowledge;
            _simulation.Process();
            Assert.AreEqual(0F, _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density);
        }

        /// <summary>
        ///     Allow new interaction with homophily - threshold = 0
        /// </summary>
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [TestMethod]
        public void NewInteractionWithHomophilyTest(int interactions)
        {
            _environment.GroupsCount = 2;
            _environment.WorkersCount = 3;
            _organization.Templates.Human.Cognitive.InteractionPatterns.AllowNewInteractions = true;
            _organization.Templates.Human.Cognitive.InteractionPatterns.ThresholdForNewInteraction = 0;
            _organization.Templates.Human.Cognitive.InteractionPatterns.MaxNumberOfNewInteractions =
                (byte) interactions;
            _simulation.Process();
            Assert.AreEqual(40F, _environment.IterationResult.OrganizationFlexibility.Links.Last().Density);
            Assert.AreEqual(10F, _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density);
            Assert.IsTrue(GetNotAcceptedMessages() > 0);
        }

        /// <summary>
        ///     Allow new interaction with homophily - threshold = 1
        /// </summary>
        [TestMethod]
        public void NewInteractionWithHomophily1Test()
        {
            _environment.GroupsCount = 2;
            _environment.WorkersCount = 3;
            _organization.Templates.Human.Cognitive.InteractionPatterns.AllowNewInteractions = true;
            _organization.Templates.Human.Cognitive.InteractionPatterns.ThresholdForNewInteraction = 1;
            _organization.Templates.Human.Cognitive.InteractionPatterns.MaxNumberOfNewInteractions = 5;
            _simulation.Process();
            Assert.AreEqual(100F, _environment.IterationResult.OrganizationFlexibility.Links.Last().Density);
            Assert.AreEqual(0, GetNotAcceptedMessages());
        }

        #endregion
    }
}