#region Licence

// Description: SymuBiz - SymuScenariosAndEventsTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Scenario;
using Symu.Common.Classes;
using Symu.Engine;
using Symu.Repository.Entities;
using SymuExamples.ScenariosAndEvents;

#endregion


namespace SymuExamplesTests
{
    /// <summary>
    ///     Integration tests for SymuScenariosAndEvents
    /// </summary>
    [TestClass]
    public class SymuScenariosAndEventsTests
    {
        private readonly ExampleEnvironment _environment = new ExampleEnvironment();
        private readonly ExampleMainOrganization _mainOrganization = new ExampleMainOrganization();
        private readonly SymuEngine _simulation = new SymuEngine();

        [TestInitialize]
        public void Initialize()
        {
            _environment.SetOrganization(_mainOrganization);
            _simulation.SetEnvironment(_environment);
            _environment.SetDebug(true);
        }

        private void AddScenario(ushort max)
        {
            var scenario = TimeBasedScenario.CreateInstance(_environment);
            scenario.NumberOfSteps = 10;
            _simulation.AddScenario(scenario);
            _simulation.Iterations.Max = max;
        }

        private void Process()
        {
            _mainOrganization.AddKnowledge();
            _simulation.Process();
        }

        private void SuccessTest(ushort max)
        {
            AddScenario(max);
            Process();
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
            SuccessTest(3);
        }

        /// <summary>
        ///     Iteration = 0
        /// </summary>
        [TestMethod]
        public void SuccessTest1()
        {
            SuccessTest(0);
        }

        /// <summary>
        ///     Workers = 0
        /// </summary>
        [TestMethod]
        public void SuccessTest2()
        {
            _mainOrganization.WorkersCount = 0;

            AddScenario(3);
            Process();
            for (var i = 0; i < _simulation.SimulationResults.List.Count; i++)
            {
                var result = _simulation.SimulationResults[i];
                Assert.IsTrue(result.Success);
                Assert.AreEqual(0, result.Tasks.Done);
                Assert.AreEqual(i + 1, result.Iteration);
            }
        }

        /// <summary>
        ///     Models On
        /// </summary>
        [TestMethod]
        public void SuccessTest3()
        {
            _mainOrganization.Models.SetOn(1);
            _mainOrganization.Models.Generator = RandomGenerator.RandomUniform;
            SuccessTest(3);
        }

        /// <summary>
        ///     Murphies On
        /// </summary>
        [TestMethod]
        public void SuccessTest4()
        {
            _mainOrganization.Murphies.SetOn(1);
            SuccessTest(3);
        }

        /// <summary>
        ///     Murphies/Models On
        /// </summary>
        [TestMethod]
        public void SuccessTest5()
        {
            _mainOrganization.Murphies.SetOn(1);
            _mainOrganization.Models.SetOn(1);
            _mainOrganization.Models.Generator = RandomGenerator.RandomUniform;
            SuccessTest(3);
        }

        /// <summary>
        ///     No scenarios
        /// </summary>
        [TestMethod]
        public void SuccessTest6()
        {
            Process();
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
            //var scenario = TimeBasedScenario.CreateInstance(_environment);
            //scenario.NumberOfSteps = 10;
            //_simulation.AddScenario(scenario);

            var scenario1 = TaskBasedScenario.CreateInstance(_environment);
            scenario1.NumberOfTasks = 10;
            _simulation.AddScenario(scenario1);

            var scenario2 = MessageBasedScenario.CreateInstance(_environment);
            scenario2.NumberOfMessages = 10;
            _simulation.AddScenario(scenario2);
            SuccessTest(10);
        }

        #region Events

        /// <summary>
        ///     Event worker one shot
        /// </summary>
        [TestMethod]
        public void EventWorkerTest()
        {
            var symuEvent = new EventEntity(_mainOrganization.MetaNetwork) {Step = 10};
            symuEvent.OnExecute += _environment.PersonEvent;
            SuccessTest(3);
        }

        /// <summary>
        ///     Event worker cyclical
        /// </summary>
        [TestMethod]
        public void EventWorkerTest1()
        {
            var symuEvent = new CyclicalEvent(_mainOrganization.MetaNetwork) {EveryStep = 5};
            symuEvent.OnExecute += _environment.PersonEvent;
            SuccessTest(3);
        }

        /// <summary>
        ///     Event worker random
        /// </summary>
        [TestMethod]
        public void EventWorkerTest2()
        {
            var symuEvent = new RandomEvent(_mainOrganization.MetaNetwork) {Ratio = 0.1F};
            symuEvent.OnExecute += _environment.PersonEvent;
            SuccessTest(3);
        }

        /// <summary>
        ///     Event knowledge one shot
        /// </summary>
        [TestMethod]
        public void EventKnowledgeTest()
        {
            var symuEvent = new EventEntity(_mainOrganization.MetaNetwork) {Step = 10};
            symuEvent.OnExecute += _environment.KnowledgeEvent;
            SuccessTest(3);
        }

        /// <summary>
        ///     Event knowledge cyclical
        /// </summary>
        [TestMethod]
        public void EventKnowledgeTest1()
        {
            var symuEvent = new CyclicalEvent(_mainOrganization.MetaNetwork) {EveryStep = 5};
            symuEvent.OnExecute += _environment.KnowledgeEvent;
            SuccessTest(3);
        }

        /// <summary>
        ///     Event knowledge random
        /// </summary>
        [TestMethod]
        public void EventKnowledgeTest2()
        {
            var symuEvent = new RandomEvent(_mainOrganization.MetaNetwork) {Ratio = 0.1F};
            symuEvent.OnExecute += _environment.KnowledgeEvent;
            SuccessTest(3);
        }

        #endregion
    }
}