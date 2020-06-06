#region Licence

// Description: Symu - SymuMurphiesAndBlockersTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Organization;
using Symu.Classes.Scenario;
using Symu.Engine;
using Symu.Messaging.Messages;
using Symu.Repository.Networks.Beliefs;
using Symu.Repository.Networks.Knowledges;
using Symu.Tools;
using SymuMurphiesAndBlockers.Classes;

#endregion


namespace SymuMurphiesAndBlockersTests
{
    /// <summary>
    ///     Integration tests using SymuEngine
    /// </summary>
    [TestClass]
    public class IntegrationTests
    {
        private const int NumberOfSteps = 61; // 3 IterationResult computations
        private readonly ExampleEnvironment _environment = new ExampleEnvironment();
        private readonly OrganizationEntity _organization = new OrganizationEntity("1");
        private readonly SymuEngine _symu = new SymuEngine();

        [TestInitialize]
        public void Initialize()
        {
            _environment.SetOrganization(_organization);
            _symu.SetEnvironment(_environment);
            _environment.SetDebug(true);
            var scenario = new TimeBasedScenario(_environment)
            {
                NumberOfSteps = NumberOfSteps
            };
            _symu.AddScenario(scenario);
        }

        private int TasksRatio()
        {
            var tasksDoneRatio = _environment.Schedule.Step * _environment.WorkersCount < Constants.Tolerance
                ? 0
                : _environment.IterationResult.Tasks.Done * 100 /
                  (_environment.Schedule.Step * _environment.WorkersCount);
            return tasksDoneRatio;
        }

        private float CapacityRatio()
        {
            return _environment.Schedule.Step * _environment.WorkersCount < Constants.Tolerance
                ? 0
                : _environment.IterationResult.Capacity * 100 /
                  (_environment.Schedule.Step * _environment.WorkersCount);
        }

        [DataRow(0)]
        [DataRow(10)]
        [TestMethod]
        public void NoMurphiesTest(int knowledgeCount)
        {
            _environment.KnowledgeCount = (byte) knowledgeCount;
            _environment.Organization.Murphies.IncompleteBelief.On = false;
            _environment.Organization.Murphies.IncompleteKnowledge.On = false;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _environment.Organization.Murphies.IncompleteInformation.On = false;
            _environment.Organization.AgentTemplates.Human.Cognitive.InternalCharacteristics.RiskAversionThreshold = 1;

            _symu.Process();

            Assert.AreEqual(100, CapacityRatio());
            Assert.AreEqual(100, TasksRatio());
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalBlockersDone);
        }

        #region Only Unavailability

        /// <summary>
        ///     RateOfAgentsOn = 0
        /// </summary>
        [TestMethod]
        public void OnlyUnavailabilityTest()
        {
            _environment.Organization.Murphies.IncompleteBelief.On = false;
            _environment.Organization.Murphies.IncompleteKnowledge.On = false;
            _environment.Organization.Murphies.IncompleteInformation.On = false;
            _environment.Organization.Murphies.UnAvailability.On = true;
            _environment.Organization.Murphies.UnAvailability.RateOfAgentsOn = 0;
            _environment.Organization.AgentTemplates.Human.Cognitive.InternalCharacteristics.RiskAversionThreshold = 1;

            _symu.Process();

            Assert.AreEqual(100, CapacityRatio());
            Assert.AreEqual(100, TasksRatio());
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalBlockersDone);
        }

        /// <summary>
        ///     RateOfUnavailability = 0
        /// </summary>
        [TestMethod]
        public void OnlyUnavailabilityTest1()
        {
            _environment.Organization.Murphies.IncompleteBelief.On = false;
            _environment.Organization.Murphies.IncompleteKnowledge.On = false;
            _environment.Organization.Murphies.IncompleteInformation.On = false;
            _environment.Organization.Murphies.UnAvailability.On = true;
            _environment.Organization.Murphies.UnAvailability.RateOfAgentsOn = 1;
            _environment.Organization.Murphies.UnAvailability.RateOfUnavailability = 0;
            _environment.Organization.AgentTemplates.Human.Cognitive.InternalCharacteristics.RiskAversionThreshold = 1;

            _symu.Process();

            Assert.AreEqual(100, CapacityRatio());
            Assert.AreEqual(100, TasksRatio());
        }

        /// <summary>
        ///     RateOfUnavailability = 1
        /// </summary>
        [TestMethod]
        public void OnlyUnavailabilityTest2()
        {
            _environment.Organization.Murphies.IncompleteBelief.On = false;
            _environment.Organization.Murphies.IncompleteKnowledge.On = false;
            _environment.Organization.Murphies.IncompleteInformation.On = false;
            _environment.Organization.Murphies.UnAvailability.On = true;
            _environment.Organization.Murphies.UnAvailability.RateOfAgentsOn = 1;
            _environment.Organization.Murphies.UnAvailability.RateOfUnavailability = 1;

            _symu.Process();

            Assert.AreEqual(0, CapacityRatio());
            Assert.AreEqual(0, TasksRatio());
        }

        #endregion

        #region Only Knowledge

        /// <summary>
        ///     RateOfAgentsOn = 0
        /// </summary>
        [TestMethod]
        public void OnlyKnowledgeTest()
        {
            _environment.Organization.Murphies.IncompleteBelief.On = false;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _environment.Organization.Murphies.IncompleteInformation.On = false;
            _environment.Organization.Murphies.IncompleteKnowledge.On = true;
            _environment.Organization.Murphies.IncompleteKnowledge.RateOfAgentsOn = 0;
            _environment.Organization.AgentTemplates.Human.Cognitive.InternalCharacteristics.RiskAversionThreshold = 1;

            _symu.Process();

            Assert.AreEqual(100, CapacityRatio());
            Assert.AreEqual(100, TasksRatio());
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalBlockersDone);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.BlockersStillInProgress);
        }

        /// <summary>
        ///     RateOfAgentsOn = 1
        ///     MandatoryRatio = 0
        /// </summary>
        [TestMethod]
        public void OnlyKnowledgeTest1()
        {
            _environment.Organization.Murphies.IncompleteBelief.On = false;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _environment.Organization.Murphies.IncompleteInformation.On = false;
            _environment.Organization.Murphies.IncompleteKnowledge.On = true;
            _environment.Organization.Murphies.IncompleteKnowledge.RateOfAgentsOn = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.MandatoryRatio = 0;

            _symu.Process();

            Assert.AreEqual(100, CapacityRatio());
            // they may have some tasks cancelled
            Assert.IsTrue(95 < TasksRatio());

            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalBlockersDone);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.BlockersStillInProgress);
        }

        /// <summary>
        ///     RateOfAgentsOn = 1
        ///     MandatoryRatio = 1
        ///     ThresholdForReacting = 0
        /// </summary>
        [TestMethod]
        public void OnlyKnowledgeTest2()
        {
            _environment.Organization.Murphies.IncompleteBelief.On = false;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _environment.Organization.Murphies.IncompleteInformation.On = false;
            _environment.Organization.Murphies.IncompleteKnowledge.On = true;
            _environment.Organization.Murphies.IncompleteKnowledge.RateOfAgentsOn = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.MandatoryRatio = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.ThresholdForReacting = 0;
            _environment.Organization.AgentTemplates.Human.Cognitive.InternalCharacteristics.RiskAversionThreshold = 1;

            _symu.Process();

            Assert.AreEqual(100, CapacityRatio());
            Assert.AreEqual(100, TasksRatio());
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalBlockersDone);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.BlockersStillInProgress);
        }

        /// <summary>
        ///     Passing test
        /// </summary>
        [TestMethod]
        public void OnlyKnowledgeTest3()
        {
            _environment.Organization.Murphies.IncompleteBelief.On = false;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _environment.Organization.Murphies.IncompleteInformation.On = false;
            _environment.Organization.Murphies.IncompleteKnowledge.On = true;
            _environment.Organization.Murphies.IncompleteKnowledge.RateOfAgentsOn = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.MandatoryRatio = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.ThresholdForReacting = 1;

            _symu.Process();

            Assert.AreEqual(100, CapacityRatio());
            Assert.AreNotEqual(100, TasksRatio());
            Assert.IsTrue(_environment.IterationResult.Blockers.TotalBlockersDone > 0);
            Assert.IsTrue(_environment.IterationResult.Blockers.BlockersStillInProgress > 0);
        }

        /// <summary>
        ///     Full knowledge
        /// </summary>
        [TestMethod]
        public void OnlyKnowledgeTest4()
        {
            _environment.Organization.Murphies.IncompleteBelief.On = false;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _environment.Organization.Murphies.IncompleteInformation.On = false;
            _environment.Organization.Murphies.IncompleteKnowledge.On = true;
            _environment.Organization.Murphies.IncompleteKnowledge.RateOfAgentsOn = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.MandatoryRatio = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.ThresholdForReacting = 1;
            _environment.Organization.AgentTemplates.Human.Cognitive.InternalCharacteristics.RiskAversionThreshold = 1;
            _environment.KnowledgeLevel = KnowledgeLevel.FullKnowledge;

            _symu.Process();

            Assert.AreEqual(100, CapacityRatio());
            Assert.AreEqual(100, TasksRatio());
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalBlockersDone);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.BlockersStillInProgress);
        }

        /// <summary>
        ///     Limit number of tries to 0
        /// </summary>
        [TestMethod]
        public void OnlyKnowledgeTest5()
        {
            _environment.Organization.Murphies.IncompleteBelief.On = false;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _environment.Organization.Murphies.IncompleteInformation.On = false;
            _environment.Organization.Murphies.IncompleteKnowledge.On = true;
            _environment.Organization.Murphies.IncompleteKnowledge.RateOfAgentsOn = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.MandatoryRatio = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.ThresholdForReacting = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.LimitNumberOfTries = 0;

            _symu.Process();

            Assert.AreEqual(100, CapacityRatio());
            Assert.IsTrue(TasksRatio() < 100);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalInternalHelp);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalSearches);
        }

        /// <summary>
        ///     Ony Guessing
        /// </summary>
        [TestMethod]
        public void OnlyKnowledgeTest6()
        {
            _environment.Organization.Murphies.IncompleteBelief.On = false;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _environment.Organization.Murphies.IncompleteInformation.On = false;
            _environment.Organization.Murphies.IncompleteKnowledge.On = true;
            _environment.Organization.Murphies.IncompleteKnowledge.RateOfAgentsOn = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.MandatoryRatio = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.ThresholdForReacting = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.LimitNumberOfTries = 0;
            _environment.Organization.Murphies.IncompleteKnowledge.DelayBeforeSearchingExternally = 200;
            _environment.Organization.AgentTemplates.Human.Cognitive.InteractionCharacteristics
                    .PreferredCommunicationMediums =
                CommunicationMediums.FaceToFace;

            _symu.Process();

            Assert.AreEqual(100, CapacityRatio());
            Assert.IsTrue(TasksRatio() < 100);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalInternalHelp);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalSearches);
            Assert.AreNotEqual(0, _environment.IterationResult.Blockers.TotalGuesses);
            Assert.AreNotEqual(0, _environment.IterationResult.Blockers.TotalCancelled);
        }

        /// <summary>
        ///     Only external
        /// </summary>
        [TestMethod]
        public void OnlyKnowledgeTest7()
        {
            _environment.Organization.Murphies.IncompleteBelief.On = false;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _environment.Organization.Murphies.IncompleteInformation.On = false;
            _environment.Organization.Murphies.IncompleteKnowledge.On = true;
            _environment.Organization.Murphies.IncompleteKnowledge.RateOfAgentsOn = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.MandatoryRatio = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.ThresholdForReacting = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.LimitNumberOfTries = -1;
            _environment.Organization.Murphies.IncompleteKnowledge.DelayBeforeSearchingExternally = 200;
            _environment.Organization.Murphies.IncompleteKnowledge.RateOfAnswers = 0;
            _environment.Organization.AgentTemplates.Human.Cognitive.InteractionCharacteristics
                    .PreferredCommunicationMediums =
                CommunicationMediums.FaceToFace;

            _symu.Process();

            Assert.AreEqual(100, CapacityRatio());
            Assert.IsTrue(TasksRatio() < 100);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalInternalHelp);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalSearches);
            Assert.IsTrue(0 <= _environment.IterationResult.Blockers.TotalExternalHelp);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalGuesses);
        }

        /// <summary>
        ///     Only External
        /// </summary>
        [TestMethod]
        public void OnlyKnowledgeTest8()
        {
            _environment.Organization.Murphies.IncompleteBelief.On = false;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _environment.Organization.Murphies.IncompleteInformation.On = false;
            _environment.Organization.Murphies.IncompleteKnowledge.On = true;
            _environment.Organization.Murphies.IncompleteKnowledge.RateOfAgentsOn = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.MandatoryRatio = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.ThresholdForReacting = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.DelayBeforeSearchingExternally = 0;

            _symu.Process();

            Assert.AreEqual(100, CapacityRatio());
            Assert.IsTrue(TasksRatio() < 100);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalInternalHelp);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalSearches);
            Assert.AreNotEqual(0, _environment.IterationResult.Blockers.TotalExternalHelp);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalGuesses);
        }

        /// <summary>
        ///     Incorrectness RateOfIncorrectGuess = 0
        /// </summary>
        [TestMethod]
        public void OnlyKnowledgeTest9()
        {
            _environment.Organization.Murphies.IncompleteBelief.On = false;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _environment.Organization.Murphies.IncompleteInformation.On = false;
            _environment.Organization.Murphies.IncompleteKnowledge.On = true;
            _environment.Organization.Murphies.IncompleteKnowledge.RateOfAgentsOn = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.MandatoryRatio = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.ThresholdForReacting = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.LimitNumberOfTries = 0;
            _environment.Organization.Murphies.IncompleteKnowledge.DelayBeforeSearchingExternally = 200;
            _environment.Organization.Murphies.IncompleteKnowledge.RateOfIncorrectGuess = 0;

            _symu.Process();

            Assert.AreEqual(0, _environment.IterationResult.Tasks.Incorrectness);
        }

        /// <summary>
        ///     Incorrectness RateOfIncorrectGuess = 1
        /// </summary>
        [TestMethod]
        public void OnlyKnowledgeTest10()
        {
            _environment.Organization.Murphies.IncompleteBelief.On = false;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _environment.Organization.Murphies.IncompleteInformation.On = false;
            _environment.Organization.Murphies.IncompleteKnowledge.On = true;
            _environment.Organization.Murphies.IncompleteKnowledge.RateOfAgentsOn = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.MandatoryRatio = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.ThresholdForReacting = 1;
            _environment.Organization.Murphies.IncompleteKnowledge.LimitNumberOfTries = 0;
            _environment.Organization.Murphies.IncompleteKnowledge.DelayBeforeSearchingExternally = 200;
            _environment.Organization.Murphies.IncompleteKnowledge.RateOfIncorrectGuess = 1;

            _symu.Process();

            Assert.AreNotEqual(0, _environment.IterationResult.Tasks.Incorrectness);
        }

        #endregion

        #region Only Beliefs

        /// <summary>
        ///     RateOfAgentsOn = 0
        /// </summary>
        [TestMethod]
        public void OnlyBeliefsTest()
        {
            _environment.Organization.Murphies.IncompleteKnowledge.On = false;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _environment.Organization.Murphies.IncompleteInformation.On = false;
            _environment.Organization.Murphies.IncompleteBelief.On = true;
            _environment.Organization.Murphies.IncompleteBelief.RateOfAgentsOn = 0;
            _environment.Organization.AgentTemplates.Human.Cognitive.InternalCharacteristics.RiskAversionThreshold = 1;

            _symu.Process();

            Assert.AreEqual(100, CapacityRatio());
            Assert.AreEqual(100, TasksRatio());
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalBlockersDone);
        }

        /// <summary>
        ///     RateOfAgentsOn = 1
        ///     Strongly agree
        /// </summary>
        [TestMethod]
        public void OnlyBeliefsTest1()
        {
            _environment.Organization.Murphies.IncompleteKnowledge.On = false;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _environment.Organization.Murphies.IncompleteInformation.On = false;
            _environment.Organization.Murphies.IncompleteBelief.On = true;
            _environment.Organization.Murphies.IncompleteBelief.RateOfAgentsOn = 1;
            _environment.Organization.AgentTemplates.Human.Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel =
                BeliefLevel.StronglyAgree;

            _symu.Process();

            Assert.AreEqual(100, CapacityRatio());
            Assert.AreEqual(100, TasksRatio());
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalBlockersDone);
        }

        /// <summary>
        ///     RateOfAgentsOn = 1
        ///     Risk aversion = 1
        /// </summary>
        [TestMethod]
        public void OnlyBeliefsTest2()
        {
            _environment.Organization.Murphies.IncompleteKnowledge.On = false;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _environment.Organization.Murphies.IncompleteInformation.On = false;
            _environment.Organization.Murphies.IncompleteBelief.On = true;
            _environment.Organization.Murphies.IncompleteBelief.RateOfAgentsOn = 1;
            _environment.Organization.Murphies.IncompleteBelief.ThresholdForReacting = 1;
            _environment.Organization.AgentTemplates.Human.Cognitive.InternalCharacteristics.RiskAversionThreshold = 1;

            _symu.Process();

            Assert.AreEqual(100, CapacityRatio());
            Assert.AreEqual(100, TasksRatio());
            Assert.IsTrue(0 <= _environment.IterationResult.Blockers.TotalBlockersDone);
        }

        /// <summary>
        ///     RateOfAgentsOn = 1
        ///     Strongly disagree
        /// </summary>
        [TestMethod]
        public void OnlyBeliefsTest3()
        {
            _environment.Organization.Murphies.IncompleteKnowledge.On = false;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _environment.Organization.Murphies.IncompleteInformation.On = false;
            _environment.Organization.Murphies.IncompleteBelief.On = true;
            _environment.Organization.Murphies.IncompleteBelief.RateOfAgentsOn = 1;
            _environment.Organization.Murphies.IncompleteBelief.ThresholdForReacting = 0;
            _environment.Organization.AgentTemplates.Human.Cognitive.InternalCharacteristics.RiskAversionThreshold = 0;
            _environment.Organization.AgentTemplates.Human.Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel =
                BeliefLevel.StronglyDisagree;

            _symu.Process();

            Assert.AreEqual(100, CapacityRatio());
            Assert.IsTrue(0 <= TasksRatio());
            Assert.IsTrue(0 <= _environment.IterationResult.Blockers.TotalBlockersDone);
            Assert.IsTrue(0 <= _environment.IterationResult.Blockers.TotalInternalHelp);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalExternalHelp);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalSearches);
            Assert.IsTrue(0 <= _environment.IterationResult.Blockers.TotalGuesses);
        }

        /// <summary>
        ///     RateOfAgentsOn = 1
        ///     Only guessing
        /// </summary>
        [TestMethod]
        public void OnlyBeliefsTest4()
        {
            _environment.Organization.Murphies.IncompleteKnowledge.On = false;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _environment.Organization.Murphies.IncompleteInformation.On = false;
            _environment.Organization.Murphies.IncompleteBelief.On = true;
            _environment.Organization.Murphies.IncompleteBelief.RateOfAgentsOn = 1;
            //_environment.Organization.AgentTemplates.Human.Cognitive.InternalCharacteristics.RiskAversionThreshold = 0;
            _environment.Organization.Murphies.IncompleteBelief.ThresholdForReacting = 0;
            _environment.Organization.Murphies.IncompleteBelief.RateOfAnswers = 0;
            _environment.Organization.Murphies.IncompleteBelief.RateOfIncorrectGuess = 0;

            _symu.Process();

            Assert.AreEqual(100, CapacityRatio());
            Assert.IsTrue(0 <= TasksRatio());
            Assert.AreEqual(0, _environment.IterationResult.Tasks.Incorrectness);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalInternalHelp);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalExternalHelp);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalSearches);
            Assert.IsTrue(0 <= _environment.IterationResult.Blockers.TotalGuesses);
            Assert.IsTrue(0 <= _environment.IterationResult.Blockers.TotalCancelled);
        }

        /// <summary>
        ///     RateOfAgentsOn = 1
        ///     Only guessing + cancelled
        /// </summary>
        [TestMethod]
        public void OnlyBeliefsTest5()
        {
            _environment.Organization.Murphies.IncompleteKnowledge.On = false;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _environment.Organization.Murphies.IncompleteInformation.On = false;
            _environment.Organization.Murphies.IncompleteBelief.On = true;
            _environment.Organization.Murphies.IncompleteBelief.RateOfAgentsOn = 1;
            _environment.Organization.Murphies.IncompleteBelief.ThresholdForReacting = 0;
            _environment.Organization.Murphies.IncompleteBelief.RateOfAnswers = 0;
            _environment.Organization.Murphies.IncompleteBelief.RateOfIncorrectGuess = 1;

            _symu.Process();

            Assert.AreEqual(100, CapacityRatio());
            Assert.IsTrue(0 <= TasksRatio());
            Assert.IsTrue(0 <= _environment.IterationResult.Tasks.Incorrectness);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalInternalHelp);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalExternalHelp);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalSearches);
            Assert.IsTrue(0 <= _environment.IterationResult.Blockers.TotalGuesses);
            Assert.IsTrue(0 <= _environment.IterationResult.Blockers.TotalCancelled);
        }

        /// <summary>
        ///     RateOfAgentsOn = 1
        ///     Only internal
        /// </summary>
        [TestMethod]
        public void OnlyBeliefsTest6()
        {
            _environment.Organization.Murphies.IncompleteKnowledge.On = false;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _environment.Organization.Murphies.IncompleteInformation.On = false;
            _environment.Organization.Murphies.IncompleteBelief.On = true;
            _environment.Organization.Murphies.IncompleteBelief.RateOfAgentsOn = 1;
            _environment.Organization.Murphies.IncompleteBelief.ThresholdForReacting = 0;
            _environment.Organization.Murphies.IncompleteBelief.RateOfAnswers = 1;
            _environment.Organization.AgentTemplates.Human.Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel =
                BeliefLevel.StronglyDisagree;

            _symu.Process();

            Assert.AreEqual(100, CapacityRatio());
            Assert.IsTrue(TasksRatio() < 100);
            Assert.IsTrue(0 <= _environment.IterationResult.Blockers.TotalInternalHelp);
            Assert.IsTrue(0 <= _environment.IterationResult.Blockers.TotalGuesses);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalExternalHelp);
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalSearches);
        }

        #endregion
    }
}