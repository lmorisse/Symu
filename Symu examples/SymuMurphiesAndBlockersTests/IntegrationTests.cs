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
using SymuMurphiesAndBlockers.Classes;
using SymuTools;

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
            var scenario = new TimeStepScenario(_environment)
            {
                NumberOfSteps = NumberOfSteps
            };
            _symu.AddScenario(scenario);
        }

        private int TasksRatio()
        {
            var tasksDoneRatio = _environment.Schedule.Step * _environment.WorkersCount < Constants.Tolerance
                ? 0
                : _environment.IterationResult.Tasks.Total * 100 /
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

            _symu.Process();

            Assert.AreEqual(100, CapacityRatio());
            Assert.AreEqual(100, TasksRatio());
            Assert.AreEqual(0, _environment.IterationResult.Blockers.TotalBlockersDone);
        }

        #region Only Unavailability
        /// <summary>
        /// RateOfAgentsOn = 0
        /// </summary>
        [TestMethod]
        public void OnlyUnavailabilityTest()
        {
            _environment.Organization.Murphies.IncompleteBelief.On = false;
            _environment.Organization.Murphies.IncompleteKnowledge.On = false;
            _environment.Organization.Murphies.UnAvailability.On = true;
            _environment.Organization.Murphies.UnAvailability.RateOfAgentsOn = 0;

            _symu.Process();

            Assert.AreEqual(100, CapacityRatio());
            Assert.AreEqual(100, TasksRatio());
        }
        /// <summary>
        /// RateOfUnavailability = 0
        /// </summary>
        [TestMethod]
        public void OnlyUnavailabilityTest1()
        {
            _environment.Organization.Murphies.IncompleteBelief.On = false;
            _environment.Organization.Murphies.IncompleteKnowledge.On = false;
            _environment.Organization.Murphies.UnAvailability.On = true;
            _environment.Organization.Murphies.UnAvailability.RateOfAgentsOn = 1;
            _environment.Organization.Murphies.UnAvailability.RateOfUnavailability = 0;

            _symu.Process();

            Assert.AreEqual(100, CapacityRatio());
            Assert.AreEqual(100, TasksRatio());
        }
        /// <summary>
        /// RateOfUnavailability = 1
        /// </summary>
        [TestMethod]
        public void OnlyUnavailabilityTest2()
        {
            _environment.Organization.Murphies.IncompleteBelief.On = false;
            _environment.Organization.Murphies.IncompleteKnowledge.On = false;
            _environment.Organization.Murphies.UnAvailability.On = true;
            _environment.Organization.Murphies.UnAvailability.RateOfAgentsOn = 1;
            _environment.Organization.Murphies.UnAvailability.RateOfUnavailability = 1;

            _symu.Process();

            Assert.AreEqual(0, CapacityRatio());
            Assert.AreEqual(0, TasksRatio());
        }
        #endregion
    }
}