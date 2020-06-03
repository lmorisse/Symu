#region Licence

// Description: Symu - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Task;
using Symu.Classes.Task.Manager;
using Symu.Messaging.Messages;

#endregion

namespace SymuTests.Classes.Task.Manager
{
    [TestClass]
    public class TasksManagerTests
    {
        private readonly TasksLimit _tasksLimit = new TasksLimit();
        private SymuTask _task;
        private SymuTask _message;
        private List<SymuTask> _tasks;
        private TasksManager _tasksManager;

        [TestInitialize]
        public void Initialize()
        {
            _task = new SymuTask(0);
            _message = new SymuTask(0) {Parent = new Message()};
            var tasksLimit = new TasksLimit();
            _tasksManager = new TasksManager(tasksLimit, true);
            _tasks = new List<SymuTask> {_task};
        }
        /// <summary>
        /// with a Task 
        /// </summary>
        [TestMethod]
        public void AddToDoTest()
        {
            _tasksManager.AddToDo(_task);
            Assert.AreEqual(1, _tasksManager.TaskResult.TotalTasksNumber);
            Assert.AreEqual(1, _tasksManager.TaskResult.ToDo);
            Assert.AreEqual(1, _tasksManager.ToDo.Count);
            Assert.AreEqual(0, _tasksManager.InProgress.Count);
            Assert.AreEqual(0, _tasksManager.Done.Count);
        }
        /// <summary>
        /// with a message 
        /// </summary>
        [TestMethod]
        public void AddToDoTest1()
        {
            _tasksManager.AddToDo(_message);
            Assert.AreEqual(0, _tasksManager.TaskResult.TotalTasksNumber);
            Assert.AreEqual(0, _tasksManager.TaskResult.ToDo);
            Assert.AreEqual(1, _tasksManager.ToDo.Count);
            Assert.AreEqual(0, _tasksManager.InProgress.Count);
            Assert.AreEqual(0, _tasksManager.Done.Count);
        }
        /// <summary>
        /// with a task
        /// </summary>
        [TestMethod]
        public void AddInProgressTest()
        {
            _tasksManager.AddInProgress(_task);
            Assert.AreEqual(1, _tasksManager.TaskResult.TotalTasksNumber);
            Assert.AreEqual(1, _tasksManager.TaskResult.InProgress);
            Assert.AreEqual(0, _tasksManager.ToDo.Count);
            Assert.AreEqual(1, _tasksManager.InProgress.Count);
            Assert.AreEqual(0, _tasksManager.Done.Count);
        }
        /// <summary>
        /// with a message
        /// </summary>
        [TestMethod]
        public void AddInProgressTest1()
        {
            _tasksManager.AddInProgress(_message);
            Assert.AreEqual(0, _tasksManager.TaskResult.TotalTasksNumber);
            Assert.AreEqual(0, _tasksManager.TaskResult.InProgress);
            Assert.AreEqual(0, _tasksManager.ToDo.Count);
            Assert.AreEqual(1, _tasksManager.InProgress.Count);
            Assert.AreEqual(0, _tasksManager.Done.Count);
        }
        /// <summary>
        /// With a task
        /// </summary>
        [TestMethod]
        public void SetInProgressTest()
        {
            _tasksManager.AddToDo(_task);
            _tasksManager.SetInProgress(_task);
            Assert.AreEqual(0, _tasksManager.TaskResult.ToDo);
            Assert.AreEqual(1, _tasksManager.TaskResult.InProgress);
            Assert.AreEqual(0, _tasksManager.ToDo.Count);
            Assert.AreEqual(1, _tasksManager.InProgress.Count);
            Assert.AreEqual(0, _tasksManager.Done.Count);
        }
        /// <summary>
        /// With a message
        /// </summary>
        [TestMethod]
        public void SetInProgressTest1()
        {
            _tasksManager.AddToDo(_message);
            _tasksManager.SetInProgress(_message);
            Assert.AreEqual(0, _tasksManager.TaskResult.ToDo);
            Assert.AreEqual(0, _tasksManager.TaskResult.InProgress);
            Assert.AreEqual(0, _tasksManager.ToDo.Count);
            Assert.AreEqual(1, _tasksManager.InProgress.Count);
            Assert.AreEqual(0, _tasksManager.Done.Count);
        }
        /// <summary>
        /// With a task, from inprogress
        /// </summary>
        [TestMethod]
        public void SetTaskDoneTest()
        {
            _task.Weight = 1;
            _task.WorkToDo = 1;
            _tasksManager.AddInProgress(_task);
            _tasksManager.SetDone(_task);
            Assert.AreEqual(0, _tasksManager.TaskResult.InProgress);
            Assert.AreEqual(1, _tasksManager.TaskResult.Done);
            Assert.AreEqual(1, _tasksManager.TaskResult.WeightDone);
            Assert.AreEqual(0, _tasksManager.ToDo.Count);
            Assert.AreEqual(0, _tasksManager.InProgress.Count);
            Assert.AreEqual(1, _tasksManager.Done.Count);
            Assert.AreEqual(0, _task.WorkToDo);
        }        
        /// <summary>
        /// With a task, from to do
        /// </summary>
        [TestMethod]
        public void SetTaskDoneTest1()
        {
            _task.Weight = 1;
            _task.WorkToDo = 1;
            _tasksManager.AddToDo(_task);
            _tasksManager.SetDone(_task);
            Assert.AreEqual(0, _tasksManager.TaskResult.ToDo);
            Assert.AreEqual(1, _tasksManager.TaskResult.Done);
            Assert.AreEqual(1, _tasksManager.TaskResult.WeightDone);
            Assert.AreEqual(0, _tasksManager.ToDo.Count);
            Assert.AreEqual(0, _tasksManager.InProgress.Count);
            Assert.AreEqual(1, _tasksManager.Done.Count);
            Assert.AreEqual(0, _task.WorkToDo);
        }
        /// <summary>
        /// With a message, from inprogress
        /// </summary>
        [TestMethod]
        public void SetTaskDoneTest2()
        {
            _message.Weight = 1;
            _message.WorkToDo = 1;
            _tasksManager.AddInProgress(_message);
            _tasksManager.SetDone(_message);
            Assert.AreEqual(0, _tasksManager.TaskResult.InProgress);
            Assert.AreEqual(0, _tasksManager.TaskResult.Done);
            Assert.AreEqual(0, _tasksManager.TaskResult.WeightDone);
            Assert.AreEqual(0, _tasksManager.ToDo.Count);
            Assert.AreEqual(0, _tasksManager.InProgress.Count);
            Assert.AreEqual(1, _tasksManager.Done.Count);
            Assert.AreEqual(0, _task.WorkToDo);
        }
        /// <summary>
        /// With a task, from inprogress
        /// </summary>
        [TestMethod]
        public void SetTaskCancelTest()
        {
            _task.Weight = 1;
            _task.WorkToDo = 1;
            _tasksManager.AddInProgress(_task);
            _tasksManager.Cancel(_task);
            Assert.AreEqual(0, _tasksManager.TaskResult.InProgress);
            Assert.AreEqual(1, _tasksManager.TaskResult.Cancelled);
            Assert.AreEqual(0, _tasksManager.ToDo.Count);
            Assert.AreEqual(0, _tasksManager.InProgress.Count);
            Assert.AreEqual(1, _tasksManager.Cancelled.Count);
            Assert.AreEqual(1, _task.WorkToDo);
        }
        /// <summary>
        /// With a task, from to do
        /// </summary>
        [TestMethod]
        public void SetTaskCancelTest1()
        {
            _task.Weight = 1;
            _task.WorkToDo = 1;
            _tasksManager.AddToDo(_task);
            _tasksManager.Cancel(_task);
            Assert.AreEqual(0, _tasksManager.TaskResult.ToDo);
            Assert.AreEqual(1, _tasksManager.TaskResult.Cancelled);
            Assert.AreEqual(0, _tasksManager.ToDo.Count);
            Assert.AreEqual(0, _tasksManager.InProgress.Count);
            Assert.AreEqual(1, _tasksManager.Cancelled.Count);
            Assert.AreEqual(1, _task.WorkToDo);
        }
        /// <summary>
        /// With a message, from inprogress
        /// </summary>
        [TestMethod]
        public void SetTaskCancelTest2()
        {
            _tasksManager.AddInProgress(_message);
            _tasksManager.Cancel(_message);
            Assert.AreEqual(0, _tasksManager.TaskResult.InProgress);
            Assert.AreEqual(0, _tasksManager.TaskResult.Cancelled);
            Assert.AreEqual(0, _tasksManager.ToDo.Count);
            Assert.AreEqual(0, _tasksManager.InProgress.Count);
            Assert.AreEqual(1, _tasksManager.Cancelled.Count);
        }
        /// <summary>
        ///     TasksCHeck AverageToDo
        /// </summary>
        [TestMethod]
        public void TasksCheckTest()
        {
            _task.Weight = 1;
            _task.WorkToDo = 0;
            _tasksManager.AddToDo(_task);
            Assert.ThrowsException<ArgumentException>(() => _tasksManager.TasksCheck(0));
        }

        /// <summary>
        ///     TasksCHeck in progress
        /// </summary>
        [TestMethod]
        public void TasksCheckTest1()
        {
            _task.Weight = 1;
            _task.WorkToDo = 0;
            _tasksManager.AddInProgress(_task);
            Assert.ThrowsException<ArgumentException>(() => _tasksManager.TasksCheck(0));
        }

        [TestMethod]
        public void IsTaskDoneTest()
        {
            _tasksManager.AddToDo(_task);
            Assert.IsFalse(_tasksManager.IsDone(_task));
            _tasksManager.SetInProgress(_task);
            Assert.IsFalse(_tasksManager.IsDone(_task));
            _tasksManager.SetDone(_task);
            Assert.IsTrue(_tasksManager.IsDone(_task));
        }

        [TestMethod]
        public void IsTaskInProgressTest()
        {
            _tasksManager.AddToDo(_task);
            Assert.IsFalse(_tasksManager.IsInProgress(_task));
            _tasksManager.SetInProgress(_task);
            Assert.IsTrue(_tasksManager.IsInProgress(_task));
            _tasksManager.SetDone(_task);
            Assert.IsFalse(_tasksManager.IsInProgress(_task));
        }

        /// <summary>
        ///     First column
        /// </summary>
        [TestMethod]
        public void SetAllTasksDoneTest()
        {
            _tasksManager.AddToDo(_task);
            _tasksManager.SetAllTasksDone();
            Assert.IsTrue(_tasksManager.IsDone(_task));
        }

        /// <summary>
        ///     IN progress column
        /// </summary>
        [TestMethod]
        public void SetAllTasksDoneTest1()
        {
            _tasksManager.AddInProgress(_task);
            _tasksManager.SetAllTasksDone();
            Assert.IsTrue(_tasksManager.IsDone(_task));
        }

        /// <summary>
        ///     No blocked Task
        /// </summary>
        [TestMethod]
        public void GetTasksInProgressTest()
        {
            Assert.AreEqual(0, _tasksManager.GetTasksInProgress(1).Count);
            _tasksManager.AddInProgress(_task);
            Assert.AreEqual(1, _tasksManager.GetTasksInProgress(1).Count);
        }

        /// <summary>
        ///     blocked Task
        /// </summary>
        [TestMethod]
        public void GetTasksInProgressTest2()
        {
            Assert.AreEqual(0, _tasksManager.GetTasksInProgress(1).Count);
            _tasksManager.AddInProgress(_task);
            _task.Add(1, 1);
            Assert.AreEqual(0, _tasksManager.GetTasksInProgress(1).Count);
        }

        /// <summary>
        ///     With no blocker
        /// </summary>
        [TestMethod]
        public void GetRafTest()
        {
            _task.Weight = 1;
            _tasksManager.AddToDo(_task);
            Assert.AreEqual(0, _tasksManager.GetRaf(false));
            _tasksManager.SetInProgress(_task);
            Assert.AreEqual(1, _tasksManager.GetRaf(false));
            _tasksManager.SetDone(_task);
            Assert.AreEqual(0, _tasksManager.GetRaf(false));
        }

        /// <summary>
        ///     With blockers
        /// </summary>
        [TestMethod]
        public void GetRafTest1()
        {
            _task.Weight = 1;
            _task.Add(1, 0);
            _tasksManager.AddInProgress(_task);
            Assert.AreEqual(1, _tasksManager.GetRaf(true));
            Assert.AreEqual(0, _tasksManager.GetRaf(false));
        }

        /// <summary>
        ///     FaceToFace first
        /// </summary>
        [TestMethod]
        public void SelectNextTaskTest()
        {
            var task = new SymuTask(0) {Type = CommunicationMediums.FaceToFace.ToString()};
            _tasksManager.AddToDo(task);
            task = new SymuTask(0) {Type = CommunicationMediums.Email.ToString()};
            _tasksManager.AddToDo(task);
            var selectedTask = _tasksManager.SelectNextTask(0);
            Assert.AreEqual(selectedTask.Type, CommunicationMediums.FaceToFace.ToString());
        }

        /// <summary>
        ///     Intraday test
        ///     Communication first
        /// </summary>
        [TestMethod]
        public void SelectNextTaskTest1()
        {
            var task = new SymuTask(0) {Type = CommunicationMediums.FaceToFace.ToString()};
            _tasksManager.AddToDo(task);
            task = new SymuTask(0);
            _tasksManager.AddInProgress(task);
            var selectedTask = _tasksManager.SelectNextTask(0);
            Assert.AreEqual(selectedTask.Type, CommunicationMediums.FaceToFace.ToString());
        }

        /// <summary>
        ///     Inprogress first
        /// </summary>
        [TestMethod]
        public void SelectNextTaskTest2()
        {
            var task = new SymuTask(0) {Type = "todo"};
            _tasksManager.AddToDo(task);
            task = new SymuTask(0) {Type = "ip"};
            _tasksManager.AddInProgress(task);
            var selectedTask = _tasksManager.SelectNextTask(0);
            Assert.AreEqual(selectedTask.Type, "ip");
        }

        /// <summary>
        ///     HasReachedSimultaneousMaximumLimit true
        /// </summary>
        [TestMethod]
        public void SelectNextTaskTest3()
        {
            _tasksLimit.LimitSimultaneousTasks = true;
            _tasksLimit.MaximumSimultaneousTasks = 1;
            _tasksManager = new TasksManager(_tasksLimit, true);
            _tasksManager.AddInProgress(new SymuTask(0) {Type = "ip"});
            _tasksManager.AddToDo(new SymuTask(0) {Type = "todo"});
            // without the limit, the task to do should be pushed in progress
            _tasksManager.SelectNextTask(0);
            Assert.AreEqual(1, _tasksManager.ToDo.Count);
        }

        /// <summary>
        ///     HasReachedSimultaneousMaximumLimit false
        ///     Finally in To do
        /// </summary>
        [TestMethod]
        public void SelectNextTaskTest4()
        {
            _tasksLimit.LimitSimultaneousTasks = false;
            _tasksManager = new TasksManager(_tasksLimit, true);
            var task = new SymuTask(0) {Type = "todo"};
            _tasksManager.AddToDo(task);
            Assert.IsNotNull(_tasksManager.SelectNextTask(0));
            Assert.AreEqual(1, _tasksManager.InProgress.Count);
        }

        /// <summary>
        ///     Null
        /// </summary>
        [TestMethod]
        public void PrioritizeNextTaskTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _tasksManager.PrioritizeNextTask(null));
        }

        /// <summary>
        ///     Not null but empty list
        /// </summary>
        [TestMethod]
        public void PrioritizeNextTaskTest1()
        {
            Assert.IsNull(_tasksManager.PrioritizeNextTask(new List<SymuTask>(0)));
        }

        /// <summary>
        ///     Not null but empty list
        /// </summary>
        [TestMethod]
        public void PrioritizeNextTaskTest2()
        {
            Assert.IsNotNull(_tasksManager.PrioritizeNextTask(_tasks));
        }

        /// <summary>
        ///     Null
        /// </summary>
        [TestMethod]
        public void PostTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _tasksManager.Post(null));
        }

        /// <summary>
        ///     Not Null
        /// </summary>
        [TestMethod]
        public void PostTest1()
        {
            _tasksManager.Post(_task);
            Assert.AreEqual(1, _tasksManager.TaskResult.TotalTasksNumber);
            Assert.AreEqual(1, _tasksManager.ToDo.Count);
            Assert.AreEqual(0, _tasksManager.InProgress.Count);
            Assert.AreEqual(0, _tasksManager.Done.Count);
        }

        /// <summary>
        ///     selected task
        /// </summary>
        [TestMethod]
        public void ReceiveTest1()
        {
            _tasksManager.Post(_task);
            var task = _tasksManager.Receive(0);
            Assert.AreEqual(_task, task.Result);
        }

        /// <summary>
        ///     TimeToLive = -1
        /// </summary>
        [TestMethod]
        public void CancelExpiredTasksTest()
        {
            _task.TimeToLive = -1;
            _tasksManager.AddToDo(_task);
            _tasksManager.CancelExpiredTasks(1);
            Assert.AreEqual(1, _tasksManager.ToDo.Count);
            Assert.AreEqual(1, _tasksManager.TaskResult.ToDo);
            Assert.AreEqual(0, _tasksManager.TaskResult.Cancelled);
        }

        /// <summary>
        ///     TimeToLive != -1
        /// </summary>
        [TestMethod]
        public void CancelExpiredTasksTest1()
        {
            _task.TimeToLive = 1;
            _tasksManager.AddToDo(_task);
            _tasksManager.CancelExpiredTasks(0);
            Assert.AreEqual(1, _tasksManager.ToDo.Count);
            Assert.AreEqual(1, _tasksManager.TaskResult.ToDo);
            Assert.AreEqual(0, _tasksManager.TaskResult.Cancelled);
            _tasksManager.CancelExpiredTasks(1);
            Assert.AreEqual(0, _tasksManager.ToDo.Count);
            Assert.AreEqual(1, _tasksManager.Cancelled.Count);
            Assert.AreEqual(0, _tasksManager.TaskResult.ToDo);
            Assert.AreEqual(1, _tasksManager.TaskResult.Cancelled);
        }

        #region Blockers management
        [TestMethod]
        public void AddTest()
        {
            Assert.IsFalse(_task.Blockers.Exists(1, 1));
            _task.Add(1, 1);
            Assert.IsTrue(_task.Blockers.Exists(1, 1));
        }

        [TestMethod]
        public void AddTest1()
        {
            Assert.IsFalse(_task.Blockers.Exists(1, 1));
            _task.Add(1, 1, 1);
            Assert.IsTrue(_task.Blockers.Exists(1, 1));
        }
        #endregion
    }
}