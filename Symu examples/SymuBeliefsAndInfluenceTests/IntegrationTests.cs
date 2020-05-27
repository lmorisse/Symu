#region Licence

// Description: Symu - SymuBeliefsAndInfluenceTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Organization;
using Symu.Classes.Scenario;
using Symu.Engine;
using Symu.Repository.Networks.Beliefs;
using SymuBeliefsAndInfluence.Classes;
using SymuTools;

#endregion


namespace SymuBeliefsAndInfluenceTests
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
        private readonly SymuEngine _simulation = new SymuEngine();

        [TestInitialize]
        public void Initialize()
        {
            _environment.SetOrganization(_organization);
            _simulation.SetEnvironment(_environment);
            _environment.SetDebug(true);
            var scenario = new TimeStepScenario(_environment)
            {
                NumberOfSteps = NumberOfSteps
            };
            _simulation.AddScenario(scenario);
        }

        /// <summary>
        ///     No belief
        /// </summary>
        [TestMethod]
        public void NoBeliefTest()
        {
            _environment.KnowledgeCount = 0;
            _simulation.Process();
            Assert.AreEqual(0, _environment.IterationResult.OrganizationKnowledgeAndBelief.Beliefs.First().Sum);
            Assert.AreEqual(100, _environment.IterationResult.OrganizationFlexibility.Triads.First().Density);
        }

        /// <summary>
        ///     Agents don't have beliefs
        /// </summary>
        [TestMethod]
        public void HasBeliefsTest()
        {
            _environment.InfluencerTemplate.Cognitive.KnowledgeAndBeliefs.HasBelief = false;
            _environment.WorkerTemplate.Cognitive.KnowledgeAndBeliefs.HasBelief = false;
            _simulation.Process();
            Assert.AreEqual(0, _environment.IterationResult.OrganizationKnowledgeAndBelief.Beliefs.First().Sum);
            Assert.AreEqual(100, _environment.IterationResult.OrganizationFlexibility.Triads.First().Density);
        }

        /// <summary>
        ///     Influence model off
        /// </summary>
        [TestMethod]
        public void InfluenceModelTest()
        {
            _environment.Organization.Models.Influence.On = false;
            _simulation.Process();
            CheckNoChange();
        }

        /// <summary>
        ///     Influence model on && rate of agents = 0
        /// </summary>
        [TestMethod]
        public void InfluenceModelTest1()
        {
            _environment.Organization.Models.Influence.RateOfAgentsOn = 0;
            _simulation.Process();
            CheckNoChange();
        }

        /// <summary>
        ///     No Influencers
        /// </summary>
        [TestMethod]
        public void NoInfluencerTest()
        {
            _environment.InfluencersCount = 0;
            _simulation.Process();
            CheckNoChange();
        }

        /// <summary>
        ///     Influencers can't send beliefs
        ///     Belief should not change
        /// </summary>
        [TestMethod]
        public void CantSendBeliefsTest()
        {
            _environment.InfluencerTemplate.Cognitive.MessageContent.CanSendBeliefs = false;
            _simulation.Process();
            CheckNoChange();
        }

        /// <summary>
        ///     Influencers don't have the minimum belief to send
        ///     Belief should not change
        ///     Strongly Disagree
        /// </summary>
        [TestMethod]
        public void MinimumBeliefsToSendTest()
        {
            _environment.InfluencerTemplate.Cognitive.MessageContent.MinimumBeliefToSendPerBit = 1;
            _environment.InfluencerTemplate.Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel =
                BeliefLevel.StronglyDisagree;
            _simulation.Process();
            CheckNoChange();
        }

        /// <summary>
        ///     Influencers have the minimum belief to send, but can't send any belief
        ///     Belief should not change
        /// </summary>
        [TestMethod]
        public void BeliefBitsToSendTest()
        {
            _environment.InfluencerTemplate.Cognitive.MessageContent.MinimumNumberOfBitsOfBeliefToSend = 0;
            _environment.InfluencerTemplate.Cognitive.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 0;
            _environment.InfluencerTemplate.Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel =
                BeliefLevel.StronglyDisagree;
            _simulation.Process();
            CheckNoChange();
        }

        /// <summary>
        ///     Influencers don't have the minimum belief to send
        ///     Belief should not change
        ///     Strongly agree
        /// </summary>
        [TestMethod]
        public void MinimumBeliefsToSendTest2()
        {
            _environment.InfluencerTemplate.Cognitive.MessageContent.MinimumBeliefToSendPerBit = 1;
            _environment.InfluencerTemplate.Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel =
                BeliefLevel.StronglyAgree;
            _simulation.Process();
            CheckNoChange();
        }

        /// <summary>
        ///     Worker can't receive beliefs
        ///     Belief should not change
        /// </summary>
        [TestMethod]
        public void CantReceiveBeliefsTest()
        {
            _environment.WorkerTemplate.Cognitive.MessageContent.CanReceiveBeliefs = false;
            _simulation.Process();
            CheckNoChange();
        }

        /// <summary>
        ///     Influencers have no Influentialness
        ///     Belief should not change
        /// </summary>
        [TestMethod]
        public void NoInfluentialnessTest()
        {
            _environment.InfluencerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMax = 0;
            _simulation.Process();
            CheckNoChange();
        }

        /// <summary>
        ///     Workers have no influenceability
        ///     Belief should not change
        /// </summary>
        [TestMethod]
        public void NoInfluenceabilityTest()
        {
            _environment.WorkerTemplate.Cognitive.InternalCharacteristics.InfluenceabilityRateMax = 0;
            _simulation.Process();
            CheckNoChange();
        }

        private void CheckNoChange()
        {
            Assert.AreEqual(_environment.IterationResult.OrganizationKnowledgeAndBelief.Beliefs.First().Sum,
                _environment.IterationResult.OrganizationKnowledgeAndBelief.Beliefs.Last().Sum);
            Assert.AreEqual(_environment.IterationResult.OrganizationFlexibility.Triads.First().Density,
                _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density);
        }

        [TestMethod]
        public void NoTaskBlockedTest()
        {
            _environment.Model.MandatoryRatio = 0;
            _simulation.Process();
            CheckNoChange();
            var tasksDoneRatio = _environment.Schedule.Step * _environment.WorkersCount < Constants.Tolerance
                ? 0
                : _environment.IterationResult.Tasks.Total * 100 /
                  (_environment.Schedule.Step * _environment.WorkersCount);
            Assert.AreEqual(100, tasksDoneRatio);
        }

        [TestMethod]
        public void NoRiskAversionTest()
        {
            _environment.Model.MandatoryRatio = 1;
            _environment.WorkerTemplate.Cognitive.InternalCharacteristics.RiskAversionThreshold = 1;
            _simulation.Process();
            CheckNoChange();
            var tasksDoneRatio = _environment.Schedule.Step * _environment.WorkersCount < Constants.Tolerance
                ? 0
                : _environment.IterationResult.Tasks.Total * 100 /
                  (_environment.Schedule.Step * _environment.WorkersCount);
            Assert.AreEqual(100, tasksDoneRatio);
        }

        [TestMethod]
        public void NoWeightTest()
        {
            _environment.Model.MandatoryRatio = 1;
            _environment.WhitePages.Network.NetworkBeliefs.BeliefWeightLevel = BeliefWeightLevel.NoWeight;
            _simulation.Process();
            CheckNoChange();
            var tasksDoneRatio = _environment.Schedule.Step * _environment.WorkersCount < Constants.Tolerance
                ? 0
                : _environment.IterationResult.Tasks.Total * 100 /
                  (_environment.Schedule.Step * _environment.WorkersCount);
            Assert.AreEqual(100, tasksDoneRatio);
        }

        [TestMethod]
        public void FullWeightTest()
        {
            _environment.Model.MandatoryRatio = 1;
            _environment.WorkerTemplate.Cognitive.InternalCharacteristics.RiskAversionThreshold =
                0; // Full risk aversion
            _environment.WhitePages.Network.NetworkBeliefs.BeliefWeightLevel = BeliefWeightLevel.FullWeight;
            _simulation.Process();
            CheckNoChange();
            var tasksDoneRatio = _environment.Schedule.Step * _environment.WorkersCount < Constants.Tolerance
                ? 0
                : _environment.IterationResult.Tasks.Done * 100 /
                  (_environment.Schedule.Step * _environment.WorkersCount);
            Assert.AreNotEqual(100, tasksDoneRatio);
        }

        /// <summary>
        ///     Influencers strongly disagree
        ///     Belief should decrease
        ///     Triads should increase
        /// </summary>
        [TestMethod]
        public void StronglyDisagreeTest()
        {
            _environment.WorkerTemplate.Cognitive.InternalCharacteristics.RiskAversionThreshold =
                1; // no risk aversion
            _environment.InfluencerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMax = 1;
            _environment.InfluencerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMin = 1;
            _environment.InfluencerTemplate.Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel =
                BeliefLevel.StronglyDisagree;
            _simulation.Process();
            Assert.IsTrue(_environment.IterationResult.OrganizationKnowledgeAndBelief.Beliefs.First().Sum >
                          _environment.IterationResult.OrganizationKnowledgeAndBelief.Beliefs.Last().Sum);
            Assert.IsTrue(_environment.IterationResult.OrganizationKnowledgeAndBelief.Beliefs.First().Mean >
                          _environment.IterationResult.OrganizationKnowledgeAndBelief.Beliefs.Last().Mean);
            Assert.IsTrue(_environment.IterationResult.OrganizationFlexibility.Triads.First().Density <
                          _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density);
        }

        /// <summary>
        ///     Influencers strongly agree
        ///     Belief should increase
        ///     Triads should increase
        /// </summary>
        [TestMethod]
        public void StronglyAgreeTest()
        {
            _environment.WorkerTemplate.Cognitive.InternalCharacteristics.RiskAversionThreshold =
                1; // no risk aversion
            _environment.InfluencerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMax = 1;
            _environment.InfluencerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMin = 1;
            _environment.InfluencerTemplate.Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel =
                BeliefLevel.StronglyAgree;
            _simulation.Process();
            Assert.IsTrue(_environment.IterationResult.OrganizationKnowledgeAndBelief.Beliefs.First().Sum <
                          _environment.IterationResult.OrganizationKnowledgeAndBelief.Beliefs.Last().Sum);
            Assert.IsTrue(_environment.IterationResult.OrganizationKnowledgeAndBelief.Beliefs.First().Mean <
                          _environment.IterationResult.OrganizationKnowledgeAndBelief.Beliefs.Last().Mean);
            Assert.IsTrue(_environment.IterationResult.OrganizationFlexibility.Triads.First().Density <
                          _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density);
        }

        /// <summary>
        ///     Influencers strongly agree
        ///     Belief should increase
        ///     Triads should increase
        /// </summary>
        [TestMethod]
        public void NeitherAgreeNorDisagreeTest()
        {
            _environment.InfluencerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMax = 1;
            _environment.InfluencerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMin = 1;
            _environment.InfluencerTemplate.Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel =
                BeliefLevel.NeitherAgreeNorDisagree;
            _simulation.Process();
            CheckNoChange();
        }
    }
}