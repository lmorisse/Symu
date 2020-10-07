#region Licence

// Description: SymuBiz - SymuMessageAndTaskTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Scenario;
using Symu.Common;
using Symu.Engine;
using SymuMessageAndTask.Classes;

#endregion


namespace SymuMessageAndTaskTests
{
    /// <summary>
    ///     Integration tests using SymuEngine
    /// </summary>
    [TestClass]
    public class IntegrationTests
    {
        private const int NumberOfSteps = 10;
        private readonly ExampleEnvironment _environment = new ExampleEnvironment();
        private readonly ExampleMainOrganization _mainOrganization = new ExampleMainOrganization();
        private readonly SymuEngine _simulation = new SymuEngine();

        [TestInitialize]
        public void Initialize()
        {
            _environment.SetOrganization(_mainOrganization);
            _simulation.SetEnvironment(_environment);
            _environment.SetDebug(true);
            var scenario = TimeBasedScenario.CreateInstance(_environment);
            scenario.NumberOfSteps = NumberOfSteps;
            _simulation.AddScenario(scenario);
        }

        #region Task model

        /// <summary>
        ///     model off
        /// </summary>
        [TestMethod]
        public void TaskModelOffTest()
        {
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = false;
            _simulation.Process();
            Assert.AreEqual(0, _environment.IterationResult.Tasks.Total);
        }

        /// <summary>
        ///     Passing test
        /// </summary>
        [TestMethod]
        public void TaskModelOnTest()
        {
            _mainOrganization.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _simulation.Process();
            var total = _environment.IterationResult.Tasks.Total;
            Assert.IsTrue(total > 0);
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds = true;
            _simulation.Process();
            var total2 = _environment.IterationResult.Tasks.Total;
            Assert.IsTrue(total2 > total);
        }

        [TestMethod]
        public void TotalWeight()
        {
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _mainOrganization.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            _mainOrganization.Communication.Email.CostToReceiveLevel = GenericLevel.None;
            _mainOrganization.Communication.Email.CostToSendLevel = GenericLevel.None;
            _simulation.Process();
            Assert.AreEqual(_environment.IterationResult.Tasks.Total, _environment.IterationResult.Tasks.Weight);
            Assert.AreEqual(_environment.IterationResult.Tasks.SumCapacity.Last(),
                _environment.IterationResult.Tasks.Weight);
        }

        /// <summary>
        ///     Task.Weight = 0
        /// </summary>
        [TestMethod]
        public void TotalWeight1()
        {
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _mainOrganization.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            _mainOrganization.Communication.Email.CostToReceiveLevel = GenericLevel.None;
            _mainOrganization.Communication.Email.CostToSendLevel = GenericLevel.None;
            _mainOrganization.CostOfTask = 0;
            _simulation.Process();
            Assert.AreEqual(_environment.IterationResult.Tasks.Total,
                _environment.IterationResult.Tasks.SumCapacity.Last());
            Assert.AreEqual(0, _environment.IterationResult.Tasks.Weight);
        }

        /// <summary>
        ///     With multi tasking
        /// </summary>
        [TestMethod]
        public void ToDoTest()
        {
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.TasksLimit.LimitSimultaneousTasks = false;
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _mainOrganization.NumberOfTasks = 10;
            _simulation.Process();
            Assert.AreEqual(0, _environment.IterationResult.Tasks.AverageToDo);
        }

        /// <summary>
        ///     Without multitasking
        /// </summary>
        [TestMethod]
        public void ToDoTest1()
        {
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.TasksLimit.LimitSimultaneousTasks = true;
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.TasksLimit.MaximumSimultaneousTasks = 1;
            _mainOrganization.NumberOfTasks = 10;
            _simulation.Process();
            Assert.IsTrue(0 < _environment.IterationResult.Tasks.AverageToDo);
        }

        [TestMethod]
        public void InProgressTest()
        {
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _mainOrganization.InitialCapacity = 0.1F;
            _simulation.Process();
            Assert.IsTrue(0 < _environment.IterationResult.Tasks.AverageInProgress);
        }

        [TestMethod]
        public void LimitNumberOfTasksTest()
        {
            const ushort number = 4;
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.TasksLimit.LimitTasksInTotal = true;
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.TasksLimit.MaximumTasksInTotal = number;
            _mainOrganization.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            _mainOrganization.Communication.Email.CostToReceiveLevel = GenericLevel.None;
            _mainOrganization.Communication.Email.CostToSendLevel = GenericLevel.None;
            _simulation.Process();
            Assert.AreEqual(number * _environment.ExampleMainOrganization.WorkersCount,
                _environment.IterationResult.Tasks.Total);
        }

        [TestMethod]
        public void AgentCanBeIsolatedTest()
        {
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _mainOrganization.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Always;
            _simulation.Process();
            Assert.AreEqual(0, _environment.IterationResult.Tasks.Total);
            Assert.AreEqual(0, _environment.IterationResult.Tasks.SumCapacity.Last());
        }

        [TestMethod]
        public void RandomLevelTest()
        {
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _mainOrganization.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            _mainOrganization.Communication.Email.CostToReceiveLevel = GenericLevel.None;
            _mainOrganization.Communication.Email.CostToSendLevel = GenericLevel.None;
            _environment.SetRandomLevel(3);
            _simulation.Process();
            Assert.AreEqual(_environment.IterationResult.Tasks.SumCapacity.Last(),
                _environment.IterationResult.Tasks.Total);
            Assert.AreEqual(_environment.IterationResult.Tasks.Weight, _environment.IterationResult.Tasks.Total);
        }

        [TestMethod]
        public void SwitchingContextTest()
        {
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _mainOrganization.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            _mainOrganization.Communication.Email.CostToReceiveLevel = GenericLevel.None;
            _mainOrganization.Communication.Email.CostToSendLevel = GenericLevel.None;
            _mainOrganization.SwitchingContextCost = 2;
            _mainOrganization.CostOfTask = 0.5F;
            _mainOrganization.NumberOfTasks = 2;
            _simulation.Process();
            Assert.AreEqual(_environment.IterationResult.Tasks.SumCapacity.Last(),
                2 * _environment.IterationResult.Tasks.Weight);
        }

        #endregion

        #region Message

        /// <summary>
        ///     Agent can't ask for new tasks
        /// </summary>
        [TestMethod]
        public void LimitMessageTest()
        {
            _mainOrganization.Templates.Human.Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod = true;
            _mainOrganization.Templates.Human.Cognitive.InteractionCharacteristics.MaximumMessagesPerPeriod = 0;
            _simulation.Process();
            Assert.AreEqual(0, (int) _environment.Messages.Result.SentMessagesCount);
            Assert.AreEqual(0, _environment.IterationResult.Tasks.Total);
        }

        /// <summary>
        ///     Agent can ask for new tasks but can't receive the answer
        /// </summary>
        [TestMethod]
        public void LimitMessageTest1()
        {
            _mainOrganization.Templates.Human.Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod = true;
            _mainOrganization.Templates.Human.Cognitive.InteractionCharacteristics.MaximumMessagesPerPeriod = 1;
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds = true;
            _mainOrganization.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            _simulation.Process();
            var count = _environment.ExampleMainOrganization.WorkersCount * NumberOfSteps;
            Assert.AreEqual(2 * count, (int) _environment.Messages.Result.SentMessagesCount);
            Assert.AreEqual(count, _environment.IterationResult.Messages.SentMessages,
                _environment.IterationResult.Messages.MissedMessagesCount);
            Assert.AreEqual((uint) count, _environment.IterationResult.Messages.ReceivedMessages);
            Assert.AreEqual(0, _environment.IterationResult.Tasks.Total);
        }

        /// <summary>
        ///     Agent can ask for new tasks and can receive the answer
        /// </summary>
        [TestMethod]
        public void LimitMessageTest2()
        {
            _mainOrganization.Templates.Human.Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod = true;
            _mainOrganization.Templates.Human.Cognitive.InteractionCharacteristics.MaximumMessagesPerPeriod = 2;
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds = true;
            _mainOrganization.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            _mainOrganization.Communication.Email.CostToReceiveLevel = GenericLevel.None;
            _mainOrganization.Communication.Email.CostToSendLevel = GenericLevel.None;
            _simulation.Process();
            Assert.AreEqual(2 * _environment.ExampleMainOrganization.WorkersCount * NumberOfSteps,
                (int) _environment.Messages.Result.SentMessagesCount);
            Assert.AreEqual(_environment.ExampleMainOrganization.WorkersCount * NumberOfSteps,
                _environment.IterationResult.Tasks.Total);
            Assert.AreEqual(0, _environment.IterationResult.Messages.ReceivedMessagesCost);
            Assert.AreEqual(0, _environment.IterationResult.Messages.SentMessagesCost);
        }

        /// <summary>
        ///     Agent can ask for new tasks and can receive the answer
        /// </summary>
        [TestMethod]
        public void LimitMessageTest3()
        {
            _mainOrganization.Templates.Human.Cognitive.InteractionCharacteristics.LimitMessagesSentPerPeriod = true;
            _mainOrganization.Templates.Human.Cognitive.InteractionCharacteristics.MaximumMessagesSentPerPeriod = 1;
            _mainOrganization.Templates.Human.Cognitive.InteractionCharacteristics.LimitReceptionsPerPeriod = true;
            _mainOrganization.Templates.Human.Cognitive.InteractionCharacteristics.MaximumReceptionsPerPeriod = 1;
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds = true;
            _mainOrganization.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            _mainOrganization.Communication.Email.CostToReceiveLevel = GenericLevel.None;
            _mainOrganization.Communication.Email.CostToSendLevel = GenericLevel.None;
            _simulation.Process();
            Assert.AreEqual(2 * _environment.ExampleMainOrganization.WorkersCount * NumberOfSteps,
                (int) _environment.Messages.Result.SentMessagesCount);
            Assert.AreEqual(_environment.ExampleMainOrganization.WorkersCount * NumberOfSteps,
                _environment.IterationResult.Tasks.Total);
            Assert.AreEqual(0, _environment.IterationResult.Messages.ReceivedMessagesCost);
            Assert.AreEqual(0, _environment.IterationResult.Messages.SentMessagesCost);
        }

        [TestMethod]
        public void CostOfMessageTest()
        {
            _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _mainOrganization.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            _mainOrganization.Communication.Email.CostToReceiveLevel = GenericLevel.Complete;
            _mainOrganization.Communication.Email.CostToSendLevel = GenericLevel.Complete;
            _simulation.Process();
            Assert.IsTrue(_environment.IterationResult.Tasks.SumCapacity.Last() >
                          _environment.IterationResult.Tasks.Total);
            Assert.AreEqual(0, _environment.IterationResult.Tasks.AverageDone);
            Assert.AreEqual(_environment.IterationResult.Tasks.SumCapacity.Last(),
                _environment.IterationResult.Messages.ReceivedMessagesCost);
        }

        #endregion


        #region Common

        #endregion
    }
}