#region Licence

// Description: SymuBiz - SymuScenariosAndEventsTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Organization;
using Symu.Classes.Scenario;
using Symu.Common;
using Symu.Common.Classes;
using Symu.Engine;
using Symu.Repository.Entity;
using SymuScenariosAndEvents.Classes;

#endregion


namespace SymuScenariosAndEventsTests
{
    /// <summary>
    ///     Integration tests using SymuEngine
    /// </summary>
    [TestClass]
    public class IntegrationTests
    {
        private readonly ExampleEnvironment _environment = new ExampleEnvironment();
        private readonly OrganizationEntity _organization = new OrganizationEntity("1");
        private readonly SymuEngine _simulation = new SymuEngine();

        [TestInitialize]
        public void Initialize()
        {
            _environment.SetOrganization(_organization);
            _simulation.SetEnvironment(_environment);
            _environment.SetDebug(true);
            var scenario = TimeBasedScenario.CreateInstance(_environment);
            scenario.NumberOfSteps = 10;
            _simulation.AddScenario(scenario);
            _simulation.Iterations.Max = 3;
        }

        private void SuccessTest()
        {
            _simulation.Process();
            for (var i = 0; i < _simulation.SimulationResults.List.Count; i++)
            {
                var result = _simulation.SimulationResults[i];
                Assert.IsTrue(result.Success);
                Assert.IsTrue(result.Tasks.Done > 0);
                Assert.AreEqual(i + 1, result.Iteration);
            }
        }

        [TestMethod]
        public void SuccessTest0()
        {
            SuccessTest();
        }

        /// <summary>
        ///     Iteration = 0
        /// </summary>
        [TestMethod]
        public void SuccessTest1()
        {
            _simulation.Iterations.Max = 0;
            SuccessTest();
        }

        /// <summary>
        ///     Workers = 0
        /// </summary>
        [TestMethod]
        public void SuccessTest2()
        {
            _environment.WorkersCount = 0;
            {
                _simulation.Process();
                for (var i = 0; i < _simulation.SimulationResults.List.Count; i++)
                {
                    var result = _simulation.SimulationResults[i];
                    Assert.IsTrue(result.Success);
                    Assert.AreEqual(0, result.Tasks.Done);
                    Assert.AreEqual(i + 1, result.Iteration);
                }
            }
        }

        /// <summary>
        ///     Models On
        /// </summary>
        [TestMethod]
        public void SuccessTest3()
        {
            _organization.Models.On(1);
            _organization.Models.Generator = RandomGenerator.RandomUniform;
            SuccessTest();
        }

        /// <summary>
        ///     Murphies On
        /// </summary>
        [TestMethod]
        public void SuccessTest4()
        {
            _organization.Murphies.On(1);
            SuccessTest();
        }

        /// <summary>
        ///     Murphies/Models On
        /// </summary>
        [TestMethod]
        public void SuccessTest5()
        {
            _organization.Murphies.On(1);
            _organization.Models.On(1);
            _organization.Models.Generator = RandomGenerator.RandomUniform;
            SuccessTest();
        }

        /// <summary>
        ///     No scenarios
        /// </summary>
        [TestMethod]
        public void SuccessTest6()
        {
            _simulation.Scenarii.Clear();
            _simulation.Process();
            for (var i = 0; i < _simulation.SimulationResults.List.Count; i++)
            {
                var result = _simulation.SimulationResults[i];
                Assert.IsTrue(result.Success);
                Assert.AreEqual(0, result.Tasks.Done);
                Assert.AreEqual(i + 1, result.Iteration);
            }
        }

        /// <summary>
        ///     All scenarios
        /// </summary>
        [TestMethod]
        public void SuccessTest7()
        {
            var scenario = TimeBasedScenario.CreateInstance(_environment);
            scenario.NumberOfSteps = 10;
            _simulation.AddScenario(scenario);

            var scenario1 = TaskBasedScenario.CreateInstance(_environment);
            scenario1.NumberOfTasks = 10;
            _simulation.AddScenario(scenario1);

            var scenario2 = MessageBasedScenario.CreateInstance(_environment);
            scenario2.NumberOfMessages = 10;
            _simulation.AddScenario(scenario2);
            SuccessTest();
        }

        #region Events

        /// <summary>
        ///     Event worker one shot
        /// </summary>
        [TestMethod]
        public void EventWorkerTest()
        {
            var symuEvent = new SymuEvent(1) { Step = 10};
            symuEvent.OnExecute += _environment.PersonEvent;
            _environment.AddEvent(symuEvent);
            SuccessTest();
        }

        /// <summary>
        ///     Event worker cyclical
        /// </summary>
        [TestMethod]
        public void EventWorkerTest1()
        {
            var symuEvent = new CyclicalEvent(1) {EveryStep = 5};
            symuEvent.OnExecute += _environment.PersonEvent;
            _environment.AddEvent(symuEvent);
            SuccessTest();
        }

        /// <summary>
        ///     Event worker random
        /// </summary>
        [TestMethod]
        public void EventWorkerTest2()
        {
            var symuEvent = new RandomEvent(1) { Ratio = 0.1F};
            symuEvent.OnExecute += _environment.PersonEvent;
            _environment.AddEvent(symuEvent);
            SuccessTest();
        }

        /// <summary>
        ///     Event knowledge one shot
        /// </summary>
        [TestMethod]
        public void EventKnowledgeTest()
        {
            var symuEvent = new SymuEvent(1) { Step = 10};
            symuEvent.OnExecute += _environment.KnowledgeEvent;
            _environment.AddEvent(symuEvent);
            SuccessTest();
        }

        /// <summary>
        ///     Event knowledge cyclical
        /// </summary>
        [TestMethod]
        public void EventKnowledgeTest1()
        {
            var symuEvent = new CyclicalEvent(1) { EveryStep = 5};
            symuEvent.OnExecute += _environment.KnowledgeEvent;
            _environment.AddEvent(symuEvent);
            SuccessTest();
        }

        /// <summary>
        ///     Event knowledge random
        /// </summary>
        [TestMethod]
        public void EventKnowledgeTest2()
        {
            var symuEvent = new RandomEvent(1) { Ratio = 0.1F};
            symuEvent.OnExecute += _environment.KnowledgeEvent;
            _environment.AddEvent(symuEvent);
            SuccessTest();
        }

        #endregion
    }
}