#region Licence

// Description: SymuBiz - SymuBeliefsAndInfluenceTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Organization;
using Symu.Classes.Scenario;
using Symu.Common;
using Symu.Common.Classes;
using Symu.Engine;
using Symu.Repository.Entities;
using SymuBeliefsAndInfluence.Classes;

#endregion


namespace SymuBeliefsAndInfluenceTests
{
    /// <summary>
    ///     Integration tests using SymuEngine
    /// </summary>
    [TestClass]
    public class IntegrationTests
    {
        private const int NumberOfSteps = 15; // 3 IterationResult computations
        private readonly ExampleEnvironment _environment = new ExampleEnvironment();
        private readonly ExampleOrganization _organization = new ExampleOrganization();
        private readonly SymuEngine _simulation = new SymuEngine();

        [TestInitialize]
        public void Initialize()
        {
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
            _organization.AddBeliefs();
            _simulation.Process();
        }

        /// <summary>
        ///     No knowledge
        /// </summary>
        [TestMethod]
        public void NoBeliefTest()
        {
            _organization.BeliefCount = 0;
            Process();
            Assert.AreEqual(0, _environment.IterationResult.KnowledgeAndBeliefResults.Beliefs.First().Sum);
            Assert.AreEqual(100, _environment.IterationResult.OrganizationFlexibility.Triads.First().Density);
        }

        /// <summary>
        ///     Agents don't have beliefs
        /// </summary>
        [TestMethod]
        public void HasBeliefsTest()
        {
            _organization.InfluencerTemplate.Cognitive.KnowledgeAndBeliefs.HasBelief = false;
            _organization.WorkerTemplate.Cognitive.KnowledgeAndBeliefs.HasBelief = false;
            Process();
            Assert.AreEqual(0, _environment.IterationResult.KnowledgeAndBeliefResults.Beliefs.First().Sum);
            Assert.AreEqual(100, _environment.IterationResult.OrganizationFlexibility.Triads.First().Density);
        }

        /// <summary>
        ///     Influence model off
        /// </summary>
        [TestMethod]
        public void InfluenceModelTest()
        {
            _organization.Models.Influence.On = false;
            Process();
            CheckNoChange();
        }

        /// <summary>
        ///     Influence model on && rate of agents = 0
        /// </summary>
        [TestMethod]
        public void InfluenceModelTest1()
        {
            _organization.Models.Influence.RateOfAgentsOn = 0;
            Process();
            CheckNoChange();
        }

        /// <summary>
        ///     No Influencers
        /// </summary>
        [TestMethod]
        public void NoInfluencerTest()
        {
            _organization.InfluencersCount = 0;
            Process();
            CheckNoChange();
        }

        /// <summary>
        ///     Influencers can't send beliefs
        ///     Belief should not change
        /// </summary>
        [TestMethod]
        public void CantSendBeliefsTest()
        {
            _organization.InfluencerTemplate.Cognitive.MessageContent.CanSendBeliefs = false;
            Process();
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
            _organization.InfluencerTemplate.Cognitive.MessageContent.MinimumBeliefToSendPerBit = 1;
            _organization.InfluencerTemplate.Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel =
                BeliefLevel.StronglyDisagree;
            Process();
            CheckNoChange();
        }

        /// <summary>
        ///     Influencers have the minimum belief to send, but can't send any belief
        ///     Belief should not change
        /// </summary>
        [TestMethod]
        public void BeliefBitsToSendTest()
        {
            _organization.InfluencerTemplate.Cognitive.MessageContent.MinimumNumberOfBitsOfBeliefToSend = 0;
            _organization.InfluencerTemplate.Cognitive.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 0;
            _organization.InfluencerTemplate.Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel =
                BeliefLevel.StronglyDisagree;
            Process();
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
            _organization.InfluencerTemplate.Cognitive.MessageContent.MinimumBeliefToSendPerBit = 1;
            _organization.InfluencerTemplate.Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel =
                BeliefLevel.StronglyAgree;
            Process();
            CheckNoChange();
        }

        /// <summary>
        ///     Worker can't receive beliefs
        ///     Belief should not change
        /// </summary>
        [TestMethod]
        public void CantReceiveBeliefsTest()
        {
            _organization.WorkerTemplate.Cognitive.MessageContent.CanReceiveBeliefs = false;
            Process();
            CheckNoChange();
        }

        /// <summary>
        ///     Influencers have no Influentialness
        ///     Belief should not change
        /// </summary>
        [TestMethod]
        public void NoInfluentialnessTest()
        {
            _organization.InfluencerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMax = 0;
            Process();
            CheckNoChange();
        }

        /// <summary>
        ///     Workers have no influenceability
        ///     Belief should not change
        /// </summary>
        [TestMethod]
        public void NoInfluenceabilityTest()
        {
            _organization.WorkerTemplate.Cognitive.InternalCharacteristics.InfluenceabilityRateMax = 0;
            Process();
            CheckNoChange();
        }

        private void CheckNoChange()
        {
            Assert.AreEqual(_environment.IterationResult.KnowledgeAndBeliefResults.Beliefs.First().Sum,
                _environment.IterationResult.KnowledgeAndBeliefResults.Beliefs.Last().Sum);
            Assert.AreEqual(_environment.IterationResult.OrganizationFlexibility.Triads.First().Density,
                _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density);
        }

        [TestMethod]
        public void NoTaskBlockedTest()
        {
            _organization.Murphies.IncompleteBelief.MandatoryRatio = 0;
            Process();
            CheckNoChange();
            var tasksDoneRatio = _environment.Schedule.Step * _organization.WorkersCount < Constants.Tolerance
                ? 0
                : _environment.IterationResult.Tasks.Total * 100 /
                  (_environment.Schedule.Step * _organization.WorkersCount);
            Assert.AreEqual(100, tasksDoneRatio);
        }

        [TestMethod]
        public void NoRiskAversionTest()
        {
            _organization.Murphies.IncompleteBelief.MandatoryRatio = 1;
            _organization.WorkerTemplate.Cognitive.InternalCharacteristics.RiskAversionLevel = GenericLevel.None;
            _organization.Murphies.IncompleteBelief.ThresholdForReacting = 1;
            Process();
            CheckNoChange();
            var tasksDoneRatio = _environment.Schedule.Step * _organization.WorkersCount < Constants.Tolerance
                ? 0
                : _environment.IterationResult.Tasks.Total * 100 /
                  (_environment.Schedule.Step * _organization.WorkersCount);
            Assert.AreEqual(100, tasksDoneRatio);
        }

        [TestMethod]
        public void NoWeightTest()
        {
            _organization.Murphies.IncompleteBelief.MandatoryRatio = 1;
            _organization.Models.BeliefWeightLevel = BeliefWeightLevel.NoWeight;
            Process();
            CheckNoChange();
            var tasksDoneRatio = _environment.Schedule.Step * _organization.WorkersCount < Constants.Tolerance
                ? 0
                : _environment.IterationResult.Tasks.Total * 100 /
                  (_environment.Schedule.Step * _organization.WorkersCount);
            Assert.AreEqual(100, tasksDoneRatio);
        }

        /// <summary>
        ///     Full risk aversion
        /// </summary>
        [TestMethod]
        public void FullWeightTest()
        {
            _organization.Murphies.IncompleteBelief.MandatoryRatio = 1;
            _organization.Murphies.IncompleteBelief.ThresholdForReacting = 1;
            _organization.WorkerTemplate.Cognitive.InternalCharacteristics.RiskAversionLevel =
                GenericLevel.Complete;
            _organization.Models.BeliefWeightLevel = BeliefWeightLevel.FullWeight;
            Process();
            CheckNoChange();
            var tasksDoneRatio = _environment.Schedule.Step * _organization.WorkersCount < Constants.Tolerance
                ? 0
                : _environment.IterationResult.Tasks.Done * 100 /
                  (_environment.Schedule.Step * _organization.WorkersCount);
            Assert.AreEqual(0, tasksDoneRatio);
            Assert.AreEqual(_environment.Schedule.Step * _organization.WorkersCount,
                _environment.IterationResult.Tasks.Cancelled);
        }

        /// <summary>
        ///     No risk aversion
        /// </summary>
        [TestMethod]
        public void FullWeightTest1()
        {
            _organization.Murphies.IncompleteBelief.MandatoryRatio = 1;
            _organization.Murphies.IncompleteBelief.ThresholdForReacting = 1;
            _organization.WorkerTemplate.Cognitive.InternalCharacteristics.RiskAversionLevel =
                GenericLevel.None;
            _organization.Models.BeliefWeightLevel = BeliefWeightLevel.FullWeight;
            Process();
            CheckNoChange();
            var tasksDoneRatio = _environment.Schedule.Step * _organization.WorkersCount < Constants.Tolerance
                ? 0
                : _environment.IterationResult.Tasks.Done * 100 /
                  (_environment.Schedule.Step * _organization.WorkersCount);
            Assert.AreEqual(100, tasksDoneRatio);
        }

        /// <summary>
        ///     Influencers strongly disagree
        ///     Belief should decrease, but not too quickly (if mean Rate > 0.5, beliefs has reached is maximum within one month) 
        ///     Triads should increase (but sometimes it takes more than just 60 steps
        /// </summary>
        [TestMethod]
        public void StronglyDisagreeTest()
        {
            _organization.WorkerTemplate.Cognitive.InternalCharacteristics.RiskAversionLevel =
                GenericLevel.None;
            _organization.InfluencerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMax = 0.8F;
            _organization.InfluencerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMin = 0F;
            _organization.InfluencerTemplate.Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel =
                BeliefLevel.StronglyDisagree;
            Process();
            Assert.IsTrue(_environment.IterationResult.KnowledgeAndBeliefResults.Beliefs.First().Sum >
                          _environment.IterationResult.KnowledgeAndBeliefResults.Beliefs.Last().Sum);
            Assert.IsTrue(_environment.IterationResult.KnowledgeAndBeliefResults.Beliefs.First().Mean >
                          _environment.IterationResult.KnowledgeAndBeliefResults.Beliefs.Last().Mean);
            //Assert.IsTrue(_environment.IterationResult.OrganizationFlexibility.Triads.First().Density <=
            //              _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density);
        }

        /// <summary>
        ///     Influencers strongly agree
        ///     Belief should increase, but not too quickly (if mean Rate > 0.5, beliefs has reached is maximum within one month) 
        ///     Triads should increase
        /// </summary>
        [TestMethod]
        public void StronglyAgreeTest()
        {
            _organization.WorkerTemplate.Cognitive.InternalCharacteristics.RiskAversionLevel =
                GenericLevel.None;
            _organization.InfluencerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMax = 0.8F;
            _organization.InfluencerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMin = 0F;
            _organization.InfluencerTemplate.Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel =
                BeliefLevel.StronglyAgree;
            Process();
            Assert.IsTrue(_environment.IterationResult.KnowledgeAndBeliefResults.Beliefs.First().Sum <
                          _environment.IterationResult.KnowledgeAndBeliefResults.Beliefs.Last().Sum);
            Assert.IsTrue(_environment.IterationResult.KnowledgeAndBeliefResults.Beliefs.First().Mean <
                          _environment.IterationResult.KnowledgeAndBeliefResults.Beliefs.Last().Mean);
            //Assert.IsTrue(_environment.IterationResult.OrganizationFlexibility.Triads.First().Density <
            //              _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density);
        }

        /// <summary>
        ///     Influencers strongly agree
        ///     Belief should increase
        ///     Triads should increase
        /// </summary>
        [TestMethod]
        public void NeitherAgreeNorDisagreeTest()
        {
            _organization.InfluencerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMax = 1;
            _organization.InfluencerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMin = 1;
            _organization.InfluencerTemplate.Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel =
                BeliefLevel.NeitherAgreeNorDisagree;
            Process();
            CheckNoChange();
        }
    }
}