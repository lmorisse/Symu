#region Licence

// Description: Symu - SymuMessageAndTaskTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Organization;
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
        private readonly OrganizationEntity _organization = new OrganizationEntity("1");
        private readonly SymuEngine _simulation = new SymuEngine();

        [TestInitialize]
        public void Initialize()
        {
            _environment.SetOrganization(_organization);
            _simulation.SetEnvironment(_environment);
            _environment.SetDebug(true);
            var scenario = new TimeBasedScenario(_environment)
            {
                NumberOfSteps = NumberOfSteps
            };
            _simulation.AddScenario(scenario);
        }

        #region Task model

        /// <summary>
        ///     model off
        /// </summary>
        [TestMethod]
        public void TaskModelOffTest()
        {
            _organization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = false;
            _simulation.Process();
            Assert.AreEqual(0, _environment.IterationResult.Tasks.Total);
        }

        /// <summary>
        ///     Passing test
        /// </summary>
        [TestMethod]
        public void TaskModelOnTest()
        {
            _organization.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            _organization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _simulation.Process();
            var total = _environment.IterationResult.Tasks.Total;
            Assert.IsTrue(total > 0);
            _organization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds = true;
            _simulation.Process();
            var total2 = _environment.IterationResult.Tasks.Total;
            Assert.IsTrue(total2 > total);
        }

        [TestMethod]
        public void TotalWeight()
        {
            _organization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _organization.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            _organization.Communication.Email.CostToReceiveLevel = GenericLevel.None;
            _organization.Communication.Email.CostToSendLevel = GenericLevel.None;
            _simulation.Process();
            Assert.AreEqual(_environment.IterationResult.Tasks.Total, _environment.IterationResult.Tasks.Weight);
            Assert.AreEqual(_environment.IterationResult.Capacity, _environment.IterationResult.Tasks.Weight);
        }

        /// <summary>
        ///     Task.Weight = 0
        /// </summary>
        [TestMethod]
        public void TotalWeight1()
        {
            _organization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _organization.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            _organization.Communication.Email.CostToReceiveLevel = GenericLevel.None;
            _organization.Communication.Email.CostToSendLevel = GenericLevel.None;
            _environment.CostOfTask = 0;
            _simulation.Process();
            Assert.AreEqual(_environment.IterationResult.Tasks.Total, _environment.IterationResult.Capacity);
            Assert.AreEqual(0, _environment.IterationResult.Tasks.Weight);
        }

        /// <summary>
        ///     With multi tasking
        /// </summary>
        [TestMethod]
        public void ToDoTest()
        {
            _organization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _environment.NumberOfTasks = 10;
            _simulation.Process();
            Assert.AreEqual(0, _environment.IterationResult.Tasks.AverageToDo);
        }

        /// <summary>
        ///     Without multitasking
        /// </summary>
        [TestMethod]
        public void ToDoTest1()
        {
            _organization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _organization.Templates.Human.Cognitive.TasksAndPerformance.TasksLimit.LimitSimultaneousTasks = true;
            _organization.Templates.Human.Cognitive.TasksAndPerformance.TasksLimit.MaximumSimultaneousTasks = 1;
            _environment.NumberOfTasks = 10;
            _simulation.Process();
            Assert.IsTrue(0 < _environment.IterationResult.Tasks.AverageToDo);
        }

        [TestMethod]
        public void InProgressTest()
        {
            _organization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _environment.InitialCapacity = 0.1F;
            _simulation.Process();
            Assert.IsTrue(0 < _environment.IterationResult.Tasks.AverageInProgress);
        }

        [TestMethod]
        public void LimitNumberOfTasksTest()
        {
            const ushort number = 4;
            _organization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _organization.Templates.Human.Cognitive.TasksAndPerformance.TasksLimit.LimitTasksInTotal = true;
            _organization.Templates.Human.Cognitive.TasksAndPerformance.TasksLimit.MaximumTasksInTotal = number;
            _organization.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            _organization.Communication.Email.CostToReceiveLevel = GenericLevel.None;
            _organization.Communication.Email.CostToSendLevel = GenericLevel.None;
            _simulation.Process();
            Assert.AreEqual(number * _environment.WorkersCount, _environment.IterationResult.Tasks.Total);
        }

        [TestMethod]
        public void AgentCanBeIsolatedTest()
        {
            _organization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _organization.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Always;
            _simulation.Process();
            Assert.AreEqual(0, _environment.IterationResult.Tasks.Total);
            Assert.AreEqual(0, _environment.IterationResult.Capacity);
        }

        [TestMethod]
        public void RandomLevelTest()
        {
            _organization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _organization.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            _organization.Communication.Email.CostToReceiveLevel = GenericLevel.None;
            _organization.Communication.Email.CostToSendLevel = GenericLevel.None;
            _environment.SetRandomLevel(3);
            _simulation.Process();
            Assert.AreEqual(_environment.IterationResult.Capacity, _environment.IterationResult.Tasks.Total);
            Assert.AreEqual(_environment.IterationResult.Tasks.Weight, _environment.IterationResult.Tasks.Total);
        }

        [TestMethod]
        public void SwitchingContextTest()
        {
            _organization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _organization.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            _organization.Communication.Email.CostToReceiveLevel = GenericLevel.None;
            _organization.Communication.Email.CostToSendLevel = GenericLevel.None;
            _environment.SwitchingContextCost = 2;
            _environment.CostOfTask = 0.5F;
            _environment.NumberOfTasks = 2;
            _simulation.Process();
            Assert.AreEqual(_environment.IterationResult.Capacity, 2 * _environment.IterationResult.Tasks.Weight);
        }

        #endregion

        #region Message

        /// <summary>
        ///     Agent can't ask for new tasks
        /// </summary>
        [TestMethod]
        public void LimitMessageTest()
        {
            _organization.Templates.Human.Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod = true;
            _organization.Templates.Human.Cognitive.InteractionCharacteristics.MaximumMessagesPerPeriod = 0;
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
            _organization.Templates.Human.Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod = true;
            _organization.Templates.Human.Cognitive.InteractionCharacteristics.MaximumMessagesPerPeriod = 1;
            _organization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _organization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds = true;
            _organization.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            _simulation.Process();
            var count = _environment.WorkersCount * NumberOfSteps;
            Assert.AreEqual(2*count, (int) _environment.Messages.Result.SentMessagesCount);
            Assert.AreEqual(count , _environment.IterationResult.Messages.SentMessages, _environment.IterationResult.Messages.MissedMessagesCount);
            Assert.AreEqual((uint)count, _environment.IterationResult.Messages.ReceivedMessages);
            Assert.AreEqual(0, _environment.IterationResult.Tasks.Total);
        }

        /// <summary>
        ///     Agent can ask for new tasks and can receive the answer
        /// </summary>
        [TestMethod]
        public void LimitMessageTest2()
        {
            _organization.Templates.Human.Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod = true;
            _organization.Templates.Human.Cognitive.InteractionCharacteristics.MaximumMessagesPerPeriod = 2;
            _organization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _organization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds = true;
            _organization.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            _organization.Communication.Email.CostToReceiveLevel = GenericLevel.None;
            _organization.Communication.Email.CostToSendLevel = GenericLevel.None;
            _simulation.Process();
            Assert.AreEqual(2 * _environment.WorkersCount * NumberOfSteps,
                (int) _environment.Messages.Result.SentMessagesCount);
            Assert.AreEqual(_environment.WorkersCount * NumberOfSteps, _environment.IterationResult.Tasks.Total);
            Assert.AreEqual(0, _environment.IterationResult.Messages.ReceivedMessagesCost);
            Assert.AreEqual(0, _environment.IterationResult.Messages.SentMessagesCost);
        }

        /// <summary>
        ///     Agent can ask for new tasks and can receive the answer
        /// </summary>
        [TestMethod]
        public void LimitMessageTest3()
        {
            _organization.Templates.Human.Cognitive.InteractionCharacteristics.LimitMessagesSentPerPeriod = true;
            _organization.Templates.Human.Cognitive.InteractionCharacteristics.MaximumMessagesSentPerPeriod = 1;
            _organization.Templates.Human.Cognitive.InteractionCharacteristics.LimitReceptionsPerPeriod = true;
            _organization.Templates.Human.Cognitive.InteractionCharacteristics.MaximumReceptionsPerPeriod = 1;
            _organization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _organization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds = true;
            _organization.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            _organization.Communication.Email.CostToReceiveLevel = GenericLevel.None;
            _organization.Communication.Email.CostToSendLevel = GenericLevel.None;
            _simulation.Process();
            Assert.AreEqual(2 * _environment.WorkersCount * NumberOfSteps,
                (int) _environment.Messages.Result.SentMessagesCount);
            Assert.AreEqual(_environment.WorkersCount * NumberOfSteps, _environment.IterationResult.Tasks.Total);
            Assert.AreEqual(0, _environment.IterationResult.Messages.ReceivedMessagesCost);
            Assert.AreEqual(0, _environment.IterationResult.Messages.SentMessagesCost);
        }

        [TestMethod]
        public void CostOfMessageTest()
        {
            _organization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _organization.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            _organization.Communication.Email.CostToReceiveLevel = GenericLevel.Complete;
            _organization.Communication.Email.CostToSendLevel = GenericLevel.Complete;
            _simulation.Process();
            Assert.IsTrue(_environment.IterationResult.Capacity > _environment.IterationResult.Tasks.Total);
            Assert.AreEqual(0, _environment.IterationResult.Tasks.AverageDone);
            Assert.AreEqual(_environment.IterationResult.Capacity, _environment.IterationResult.Messages.ReceivedMessagesCost);
        }

        #endregion


        #region Common

        #endregion
    }
}