#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#endregion

namespace Symu.Classes.Task.Manager
{
    /// <summary>
    ///     TaskProcessor handle the tasksManager.
    ///     It propose events trigger by the tasksManager to customize the behaviour of the manager
    /// </summary>
    public class TaskProcessor : IDisposable
    {
        public TaskProcessor(TasksLimit tasksLimit)
        {
            TasksManager = new TasksManager(tasksLimit);
            TasksManager.OnAfterSetTaskInProgress += AfterSetTaskInProgress;
            TasksManager.OnPrioritizeTasks += PrioritizeTasks;
        }

        public TasksManager TasksManager { get; }

        #region IDisposable Members

        public void Dispose()
        {
            TasksManager.Dispose();
        }

        #endregion

        /// <summary>
        ///     EventHandler triggered after the event SetTaskDone
        /// </summary>
        public event EventHandler<TaskEventArgs> OnAfterSetTaskDone;

        /// <summary>
        ///     EventHandler triggered after the event SetTaskInProgress
        /// </summary>
        public event EventHandler<TaskEventArgs> OnAfterSetTaskInProgress;

        /// <summary>
        ///     EventHandler triggered after the event OnPrioritizeTasks
        /// </summary>
        public event EventHandler<TasksEventArgs> OnPrioritizeTasks;

        public void Post(SymuTask task)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            TasksManager.Post(task);
        }

        public void Post(IEnumerable<SymuTask> tasks)
        {
            if (tasks is null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            foreach (var task in tasks)
            {
                TasksManager.Post(task);
            }
        }

        public Task<SymuTask> Receive(ushort step)
        {
            return TasksManager.Receive(step);
        }

        public void PushDone(SymuTask task)
        {
            TasksManager.PushDone(task);
            OnAfterSetTaskDone?.Invoke(this, new TaskEventArgs(task));
        }

        private void AfterSetTaskInProgress(object sender, TaskEventArgs e)
        {
            OnAfterSetTaskInProgress?.Invoke(this, new TaskEventArgs(e.Task));
        }

        private void PrioritizeTasks(object sender, TasksEventArgs e)
        {
            OnPrioritizeTasks?.Invoke(this, new TasksEventArgs(e.Tasks));
        }
    }
}