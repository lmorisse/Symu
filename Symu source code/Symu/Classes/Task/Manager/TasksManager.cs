#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Symu.Messaging.Messages;
using Symu.Results.Blocker;
using Symu.Results.Task;
using static Symu.Tools.Constants;

#endregion

namespace Symu.Classes.Task.Manager
{
    /// <summary>
    ///     Async tasks manager for agent
    ///     Tasks have 4 states : To Do, In progress, Done, Cancelled
    ///     Tasks limits are managed
    /// </summary>
    public class TasksManager
    {
        private readonly bool _debug;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="tasksLimit">Agent.Cognitive.TasksAndPerformance.TasksLimit</param>
        /// <param name="debug">Environment.Debug</param>
        public TasksManager(TasksLimit tasksLimit, bool debug)
        {
            TasksLimit = tasksLimit;
            _debug = debug;
        }

        /// <summary>
        ///     BlockerResult of the agent the agent during the simulation
        /// </summary>
        public BlockerResult BlockerResult { get; } = new BlockerResult();

        /// <summary>
        ///     TaskResult of the agent the agent during the simulation
        /// </summary>
        public TaskResult TaskResult{ get; } = new TaskResult();

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
        ///     Tasks cancelled
        /// </summary>
        public List<SymuTask> Cancelled { get; } = new List<SymuTask>();

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
            // We don't want to track message as Task
            if (task.Parent is Message)
            {
                return;
            }
            TaskResult.TotalTasksNumber++;
            TaskResult.ToDo++;
            task.SetTasksManager(this);
        }

        /// <summary>
        ///     Add a task directly in AverageInProgress
        /// </summary>
        /// <param name="task"></param>
        public void AddInProgress(SymuTask task)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            InProgress.Add(task);
            // We don't want to track message as Task
            if (task.Parent is Message)
            {
                return;
            }
            TaskResult.TotalTasksNumber++;
            TaskResult.InProgress++;
            task.SetTasksManager(this);
        }

        /// <summary>
        ///     Push the task in progress
        /// </summary>
        /// <param name="task"></param>
        public void SetInProgress(SymuTask task)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            ToDo.Remove(task);
            InProgress.Add(task);
            OnAfterSetTaskInProgress?.Invoke(this, new TaskEventArgs(task));
            // We don't want to track message as Task
            if (task.Parent is Message)
            {
                return;
            }
            TaskResult.ToDo--;
            TaskResult.InProgress++;

        }

        /// <summary>
        ///     Worker has finished a task, he push it to done in the TaskManager
        /// </summary>
        /// <param name="task"></param>
        public void SetDone(SymuTask task)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            var todo = ToDo.Contains(task);
            if (todo)
            {
                ToDo.Remove(task);
            }
            else
            {
                InProgress.Remove(task);
            }
            task.SetDone();
            if (_debug)
            {
                Done.Add(task);
            }

            // We don't want to track message as Task
            if (task.Parent is Message)
            {
                return;
            }
            if (todo)
            {
                TaskResult.ToDo--;
            }
            else
            {
                TaskResult.InProgress--;
            }
            TaskResult.Done++;
            TaskResult.WeightDone += task.Weight;
            TaskResult.Incorrectness += (int)task.Incorrectness;

        }

        /// <summary>
        ///     CancelBlocker a task :
        ///     remove a task from either To do or in progress
        /// </summary>
        /// <param name="task"></param>
        public void Cancel(SymuTask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            var todo = ToDo.Contains(task);
            if (todo)
            {
                ToDo.Remove(task);
            }
            else
            {
                InProgress.Remove(task);
            }

            task.Cancel();
            if (_debug)
            {
                Cancelled.Add(task);
            }
            // We don't want to track message as Task
            if (task.Parent is Message)
            {
                return;
            }
            if (todo)
            {
                TaskResult.ToDo--;
            }
            else
            {
                TaskResult.InProgress--;
            }
            TaskResult.Cancelled++;
        }

        /// <summary>
        ///     Check that Task Manager is ok
        /// </summary>
        public void TasksCheck(ushort step)
        {
            bool Match(SymuTask t)
            {
                return t.Weight > 0 && Math.Abs(t.WorkToDo) < Tolerance;
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

            CancelExpiredTasks(step);
        }

        /// <summary>
        ///     Cancel tasks that have expired based on TimeToLive
        /// </summary>
        /// <param name="step"></param>
        public void CancelExpiredTasks(ushort step)
        {
            bool Match(SymuTask t)
            {
                return t.TimeToLive != -1 && step >= t.Created + t.TimeToLive;
            }

            foreach (var task in ToDo.FindAll(Match))
            {
                Cancel(task);
            }
            foreach (var task in InProgress.FindAll(Match))
            {
                Cancel(task);
            }
        }
        /// <summary>
        /// Checks that Done contains the task
        /// Debug should be set to true
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
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
        ///     All tasks in the task manager are set done,
        ///     Blockers are cleared
        /// </summary>
        public void SetAllTasksDone()
        {
            while (ToDo.Any())
            {
                SetDone(ToDo.First());
            }
            while (InProgress.Any())
            {
                var task = InProgress.First();
                task.ClearBlockers();
                SetDone(task);
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

            // First push new tasks to do in progress depending of the multi tasking model of the agent
            while (ToDo.Any() && !HasReachedSimultaneousMaximumLimit)
            {
                var task = PrioritizeNextTask(ToDo);
                SetInProgress(task);
            }

            // Then check tasks already in progress but non blocked today
            return PrioritizeNextTask(GetTasksInProgress(step));
        }

        private bool SelectNextMessage(CommunicationMediums medium, out SymuTask symuTask)
        {
            if (ToDo.Exists(t => t.Type == medium.ToString()))
            {
                var task = ToDo.Find(t => t.Type == medium.ToString());
                SetInProgress(task);
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
        ///     Check if Worker has reached the maximum number of tasks he can do during the symu
        /// </summary>
        /// <returns>true if worker has not reached yet the maximum number of tasks</returns>
        public bool HasReachedTotalMaximumLimit => TasksLimit.HasReachedTotalMaximumLimit(TaskResult.TotalTasksNumber);

        /// <summary>
        ///     Check if Worker has reached the maximum number of tasks he can do during the symu
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
                if (ToDo.Count == 0 || HasReachedSimultaneousMaximumLimit || HasReachedTotalMaximumLimit)
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