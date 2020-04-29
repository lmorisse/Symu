﻿#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SymuEngine.Messaging.Message;
using static SymuTools.Classes.Algorithm.Constants;

#endregion

namespace SymuEngine.Classes.Task.Manager
{
    /// <summary>
    ///     Async tasks manager for agent
    ///     Tasks have 3 states : To Do, In progress, Done
    ///     Tasks limits are managed
    /// </summary>
    public class TasksManager
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="tasksLimit">Agent.Cognitive.TasksAndPerformance.TasksLimit</param>
        public TasksManager(TasksLimit tasksLimit)
        {
            TasksLimit = tasksLimit;
        }

        /// <summary>
        ///     Total tasks done by the agent during the simulation
        /// </summary>
        public ushort TotalTasksNumber { get; private set; }

        /// <summary>
        ///     Tasks to do
        /// </summary>
        public List<SymuTask> ToDo { get; } = new List<SymuTask>();

        /// <summary>
        ///     Tasks in progress
        /// </summary>
        public List<SymuTask> InProgress { get; } = new List<SymuTask>();

        /// <summary>
        ///     Tasks done
        /// </summary>
        public List<SymuTask> Done { get; } = new List<SymuTask>();

        /// <summary>
        ///     Manage the limits on the tasks
        /// </summary>
        public TasksLimit TasksLimit { get; }

        /// <summary>
        ///     EventHandler triggered after the event SetTaskInProgress
        /// </summary>
        public event EventHandler<TaskEventArgs> OnAfterSetTaskInProgress;

        /// <summary>
        ///     EventHandler triggered during the Method PrioritizeNextTask
        /// </summary>
        public event EventHandler<TasksEventArgs> OnPrioritizeTasks;

        /// <summary>
        ///     Add new task to the To Do column
        ///     Should not be called directly
        ///     Use TaskProcessor.Post()
        /// </summary>
        /// <param name="task"></param>
        public void AddToDo(SymuTask task)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            ToDo.Add(task);
            TotalTasksNumber += 1;
        }

        /// <summary>
        ///     Add a task directly in InProgress
        /// </summary>
        /// <param name="task"></param>
        public void AddInProgress(SymuTask task)
        {
            InProgress.Add(task);
            TotalTasksNumber += 1;
        }

        /// <summary>
        ///     Push the task in progress
        /// </summary>
        /// <param name="task"></param>
        public void PushInProgress(SymuTask task)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            ToDo.Remove(task);
            InProgress.Add(task);
            OnAfterSetTaskInProgress?.Invoke(this, new TaskEventArgs(task));
        }

        /// <summary>
        ///     Worker has finished a task, he push it to done in the TaskManager
        /// </summary>
        /// <param name="task"></param>
        public void PushDone(SymuTask task)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            task.SetDone();
            InProgress.Remove(task);
            Done.Add(task);
        }

        /// <summary>
        ///     Check that Task Manager is ok
        /// </summary>
        public void TasksCheck(ushort step)
        {
            bool Match(SymuTask t)
            {
                return t.Weight > 0 && Math.Abs(t.WorkToDo) < tolerance;
            }

            // Check tasks stuck in a list
            if (ToDo.Exists(Match))
            {
                throw new ArgumentException("Task is blocked in TODO column");
            }

            if (InProgress.Exists(Match))
            {
                throw new ArgumentException("Task is blocked in INPROGRESS column");
            }

            RemoveExpiredTasks(step);
        }

        /// <summary>
        ///     Remove tasks that have expired based on TimeToLive
        /// </summary>
        /// <param name="step"></param>
        public void RemoveExpiredTasks(ushort step)
        {
            bool Match(SymuTask t)
            {
                return t.TimeToLive != -1 && step >= t.Created + t.TimeToLive;
            }

            ToDo.RemoveAll(Match);
            InProgress.RemoveAll(Match);
        }

        public bool IsDone(SymuTask task)
        {
            return Done.Contains(task);
        }

        public bool IsInProgress(SymuTask task)
        {
            return InProgress.Contains(task);
        }

        /// <summary>
        ///     agent stop working and must finished properly the tasks
        ///     All tasks in the task manager are set done
        /// </summary>
        public void SetAllTasksDone()
        {
            ToDo.ForEach(PushDone);
            ToDo.Clear();
            while (InProgress.Any())
            {
                var task = InProgress.First();
                task.Blockers.Clear();
                PushDone(task);
            }
        }

        /// <summary>
        ///     Get all the tasks that are not blocked today
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public List<SymuTask> GetTasksInProgress(ushort step)
        {
            return InProgress.FindAll(t => t.Blockers.NotBlockedToday(step));
        }

        /// <summary>
        ///     Get the total remaining weight of tasks in Progress
        ///     Blocked tasks are taken into account or not depending on
        ///     the Micro.Models.Backlog.TakeBlockerIntoAccount
        /// </summary>
        /// <returns></returns>
        public float GetRaf(bool takeBlockerIntoAccount)
        {
            return takeBlockerIntoAccount
                ? InProgress.Sum(i => i.WorkToDo)
                : InProgress.FindAll(i => !i.IsBlocked).Sum(i => i.WorkToDo);
        }

        /// <summary>
        ///     Trigger weekly events
        /// </summary>
        public void ClearDone()
        {
            Done.Clear();
        }

        /// <summary>
        ///     Select next task
        ///     first from tasks in progress
        ///     then from tasks to do
        /// </summary>
        /// <returns>the next task to work on</returns>
        public SymuTask SelectNextTask(ushort step)
        {
            // First check new messages
            if (SelectNextMessage(CommunicationMediums.FaceToFace, out var symuTask))
            {
                return symuTask;
            }

            if (SelectNextMessage(CommunicationMediums.Irc, out symuTask))
            {
                return symuTask;
            }

            if (SelectNextMessage(CommunicationMediums.Phone, out symuTask))
            {
                return symuTask;
            }

            if (SelectNextMessage(CommunicationMediums.Meeting, out symuTask))
            {
                return symuTask;
            }

            if (SelectNextMessage(CommunicationMediums.Email, out symuTask))
            {
                return symuTask;
            }

            if (SelectNextMessage(CommunicationMediums.ViaAPlatform, out symuTask))
            {
                return symuTask;
            }

            // Then check tasks already in progress but non blocked today
            var taskInProgress = PrioritizeNextTask(GetTasksInProgress(step));
            if (taskInProgress != null)
            {
                return taskInProgress;
            }

            // Check if Worker has reached the maximum number of simultaneous tasks he can do during a step
            if (HasReachedSimultaneousMaximumLimit)
            {
                return null;
            }

            // Finally, the agent start a new task by pulling it from the to do list
            taskInProgress = PrioritizeNextTask(ToDo);
            if (taskInProgress != null)
            {
                PushInProgress(taskInProgress);
            }

            return taskInProgress;
        }

        private bool SelectNextMessage(CommunicationMediums medium, out SymuTask symuTask)
        {
            if (ToDo.Exists(t => t.Type == medium.ToString()))
            {
                var task = ToDo.Find(t => t.Type == medium.ToString());
                PushInProgress(task);
                {
                    symuTask = task;
                    return true;
                }
            }

            symuTask = null;
            return false;
        }

        /// <summary>
        ///     Prioritize the next task from a list of tasks
        ///     By default, get the first task of the list
        ///     Subscribe to OnPrioritizeTasks event to filter and prioritize the list of tasks
        /// </summary>
        /// <param name="tasks">list of tasks to prioritize</param>
        /// <returns>The prioritize task</returns>
        public SymuTask PrioritizeNextTask(IEnumerable<SymuTask> tasks)
        {
            if (tasks is null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            var symuTasks = tasks.ToList();
            if (!symuTasks.Any())
            {
                return null;
            }

            symuTasks = symuTasks.OrderByDescending(t => t.LastTouched).ToList();
            OnPrioritizeTasks?.Invoke(this, new TasksEventArgs(symuTasks));
            return symuTasks.First();
        }

        #region Tasks Limit management

        /// <summary>
        ///     Check if Worker has reached the maximum number of tasks he can do during the simulation
        /// </summary>
        /// <returns>true if worker has not reached yet the maximum number of tasks</returns>
        public bool HasReachedTotalMaximumLimit => TasksLimit.HasReachedTotalMaximumLimit(TotalTasksNumber);

        /// <summary>
        ///     Check if Worker has reached the maximum number of tasks he can do during the simulation
        /// </summary>
        /// <returns>true if worker has not reached yet the maximum number of tasks</returns>
        public bool HasReachedSimultaneousMaximumLimit =>
            TasksLimit.HasReachedSimultaneousMaximumLimit((ushort) InProgress.Count);

        #endregion

        #region Async management

        private TaskCompletionSource<bool> _savedCont;

        private async Task<bool> WaitOneNoTimeout()
        {
            if (_savedCont != null)
            {
                //throw new Exception("multiple waiting reader continuations");
                var sc = _savedCont;
                _savedCont = null;
                sc.SetResult(true);
            }

            bool descheduled;
            // An arrival may have happened while we're preparing to deschedule
            lock (ToDo)
            {
                if (ToDo.Count == 0)
                {
                    _savedCont = new TaskCompletionSource<bool>();
                    descheduled = true;
                }
                else
                {
                    descheduled = false;
                }
            }

            if (descheduled)
            {
                return await _savedCont.Task.ConfigureAwait(false);
            }

            // If we didn't deschedule then run the continuation immediately
            return true;
        }

        /// <summary>
        ///     Post the task into the todoColumn
        /// </summary>
        /// <param name="task"></param>
        public void Post(SymuTask task)
        {
            lock (ToDo)
            {
                AddToDo(task);
                // This is called when we enqueue a message, within a lock
                // We cooperatively unblock any waiting reader. If there is no waiting
                // reader we just leave the message in the incoming queue

                if (_savedCont == null)
                {
                    return;
                }

                var sc = _savedCont;
                _savedCont = null;
                sc.SetResult(true);
            }
        }

        /// <summary>
        ///     Select the next task to work on
        /// </summary>
        /// <returns></returns>
        public async Task<SymuTask> Receive(ushort step)
        {
            async Task<SymuTask> ProcessNextTask()
            {
                while (true)
                {
                    var res = SelectNextTask(step);
                    if (res != null)
                    {
                        return res;
                    }

                    var ok = await WaitOneNoTimeout().ConfigureAwait(false);

                    if (ok)
                    {
                        continue;
                    }

                    throw new TimeoutException("Receive Timed Out");
                }
            }

            return await ProcessNextTask().ConfigureAwait(false);
        }

        public void Dispose()
        {
        }

        #endregion
    }
}