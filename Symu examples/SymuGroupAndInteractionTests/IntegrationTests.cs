#region Licence

// Description: SymuBiz - SymuGroupAndInteractionTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Organization;
using Symu.Classes.Scenario;
using Symu.Common.Classes;
using Symu.Engine;
using Symu.OrgMod.GraphNetworks.TwoModesNetworks.Sphere;
using Symu.Repository.Entities;
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
        private const int NumberOfSteps = 15; // 2 organizationFlexibility computations
        private readonly ExampleEnvironment _environment = new ExampleEnvironment();
        private readonly ExampleOrganization _organization = new ExampleOrganization(); 
        private readonly SymuEngine _simulation = new SymuEngine();

        [TestInitialize]
        public void Initialize()
        {
            _organization.Templates.Human.Cognitive.InteractionPatterns.AllowNewInteractions = false;
            _environment.SetOrganization(_organization);
            _simulation.SetEnvironment(_environment);

            _environment.IterationResult.KnowledgeAndBeliefResults.Frequency = TimeStepType.Weekly;
            _environment.IterationResult.OrganizationFlexibility.Frequency = TimeStepType.Weekly;

            _environment.SetDebug(true);
            var scenario = TimeBasedScenario.CreateInstance(_environment);
            scenario.NumberOfSteps = NumberOfSteps;
            _simulation.AddScenario(scenario);
        }
        private void Process()
        {
            _organization.AddKnowledge();
            _organization.AddTasks();
            _simulation.Process();
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
            _organization.GroupsCount = (byte) groupsCount;
            _organization.WorkersCount = 10;
            _organization.Templates.Human.Cognitive.InteractionPatterns.LimitNumberOfNewInteractions = true;
            _organization.Templates.Human.Cognitive.InteractionPatterns.MaxNumberOfNewInteractions = 1;
            _organization.Knowledge = 1;
            Process();
            var links = _environment.Organization.MetaNetwork.ActorActor.Count;
            var triads = _environment.IterationResult.OrganizationFlexibility.Triads.Last().ActualNumber;
            Process();
            Assert.AreEqual(links, _environment.Organization.MetaNetwork.ActorActor.Count);
            Assert.AreEqual(triads, _environment.IterationResult.OrganizationFlexibility.Triads.Last().ActualNumber);
        }

        #endregion

        #region 0 group

        [DataRow(0)]
        [DataRow(1)]
        [TestMethod]
        public void NoGroupsOneMemberTest(int agentCount)
        {
            _organization.GroupsCount = 0;
            _organization.WorkersCount = (byte) agentCount;
            Process();
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
            _organization.GroupsCount = 0;
            _organization.WorkersCount = (byte) agentCount;
            _organization.Templates.Human.Cognitive.InteractionPatterns.AllowNewInteractions = true;
            Process();
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
            _organization.GroupsCount = 1;
            _organization.WorkersCount = (byte) workers;
            Process();
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
            _organization.GroupsCount = 1;
            _organization.WorkersCount = 3;
            _organization.Models.InteractionSphere.RandomlyGeneratedSphere = true;
            _organization.Models.InteractionSphere.MinSphereDensity = 1;
            _organization.Models.InteractionSphere.MaxSphereDensity = 1;
            Process();
            Assert.AreEqual(100F, _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density);
        }

        /// <summary>
        ///     knowledge by group
        /// </summary>
        [TestMethod]
        public void CoworkersCountTest()
        {
            _organization.GroupsCount = 1;
            _organization.WorkersCount = 10;
            _organization.Knowledge = 1;
            SetInteractionPatterns(InteractionStrategy.Knowledge);
            Process();
            var links = _environment.Organization.MetaNetwork.ActorActor.Count;
            var triads = _environment.IterationResult.OrganizationFlexibility.Triads.Last().ActualNumber;
            // results should be a multiple of groups count, because interaction sphere can't change
            _environment.ExampleOrganization.GroupsCount = 2;
            Process();
            Assert.AreEqual(links * _environment.ExampleOrganization.GroupsCount, _environment.Organization.MetaNetwork.ActorActor.Count);
            //Assert.AreEqual(triads * _environment.GroupsCount,
            //    _environment.IterationResult.OrganizationFlexibility.Triads.Last().ActualNumber);
            _environment.ExampleOrganization.GroupsCount = 3;
            Process();
            Assert.AreEqual(links * _environment.ExampleOrganization.GroupsCount, _environment.Organization.MetaNetwork.ActorActor.Count);
            //Assert.AreEqual(triads * _environment.GroupsCount,
            //    _environment.IterationResult.OrganizationFlexibility.Triads.Last().ActualNumber);
        }

        /// <summary>
        ///     With new interactions
        /// </summary>
        [TestMethod]
        public void NewInteractionTest()
        {
            _organization.GroupsCount = 1;
            _organization.WorkersCount = 3;
            Process();
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
            _organization.GroupsCount = 2;
            _organization.WorkersCount = 1;
            _organization.Knowledge = 0;
            Process();
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
            _organization.GroupsCount = 2;
            _organization.WorkersCount = 1;
            _organization.Knowledge = 1;
            Process();
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
            _organization.GroupsCount = 2;
            _organization.WorkersCount = 1;
            _organization.Knowledge = 1;
            _organization.Templates.Human.Cognitive.InteractionPatterns.AllowNewInteractions = true;
            _organization.Templates.Human.Cognitive.InteractionPatterns.ThresholdForNewInteraction = 0;
            Process();
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
            _organization.GroupsCount = 2;
            _organization.WorkersCount = 1;
            _organization.Knowledge = 1;
            _organization.Templates.Human.Cognitive.InteractionPatterns.AllowNewInteractions = true;
            _organization.Templates.Human.Cognitive.InteractionPatterns.ThresholdForNewInteraction = 1;
            Process();
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
            _organization.GroupsCount = 2;
            _organization.WorkersCount = 3;
            _organization.Knowledge = 0;
            Process();
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
            _organization.GroupsCount = 2;
            _organization.WorkersCount = 3;
            _organization.Knowledge = 1;
            Process();
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
            _organization.GroupsCount = 2;
            _organization.WorkersCount = 3;
            _organization.Activities = 0;
            Process();
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
            _organization.GroupsCount = 2;
            _organization.WorkersCount = 3;
            _organization.Activities = 1;
            Process();
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
            _organization.GroupsCount = 2;
            _organization.WorkersCount = 3;
            _organization.Knowledge = 2;
            _organization.KnowledgeLevel = KnowledgeLevel.NoKnowledge;
            Process();
            Assert.AreEqual(0F, _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density);
        }

        /// <summary>
        ///     Allow new interaction with homophily - threshold = 0 (no new interaction)
        /// </summary>
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [TestMethod]
        public void NewInteractionWithHomophilyTest(int interactions)
        {
            _organization.GroupsCount = 2;
            _organization.WorkersCount = 3;
            _organization.Templates.Human.Cognitive.InteractionPatterns.AllowNewInteractions = true;
            _organization.Templates.Human.Cognitive.InteractionPatterns.ThresholdForNewInteraction = 0;
            _organization.Templates.Human.Cognitive.InteractionPatterns.MaxNumberOfNewInteractions =
                (byte) interactions;
            Process();
            Assert.AreEqual(40F, _environment.IterationResult.OrganizationFlexibility.Links.Last().Density);
            Assert.AreEqual(10F, _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density);
            Assert.IsTrue(GetNotAcceptedMessages() > 0);
        }

        /// <summary>
        ///     Allow new interaction with homophily - threshold = 1 (full new interactions)
        /// </summary>
        [TestMethod]
        public void NewInteractionWithHomophily1Test()
        {
            _organization.GroupsCount = 2;
            _organization.WorkersCount = 3;
            _organization.Templates.Human.Cognitive.InteractionPatterns.AllowNewInteractions = true;
            _organization.Templates.Human.Cognitive.InteractionPatterns.ThresholdForNewInteraction = 1;
            _organization.Templates.Human.Cognitive.InteractionPatterns.MaxNumberOfNewInteractions = 5;
            Process();
            Assert.AreEqual(100F, _environment.IterationResult.OrganizationFlexibility.Links.Last().Density);
            Assert.AreEqual(0, GetNotAcceptedMessages());
        }

        #endregion
    }
}