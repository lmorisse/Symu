#region Licence

// Description: Symu - Simulation
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;

#endregion

namespace Symu.Classes.Task.Manager
{
    /// <summary>
    /// </summary>
    public class TasksManagerMetrics
    {
        public Dictionary<ushort, float> ToDo { get; } = new Dictionary<ushort, float>();
        public Dictionary<ushort, float> InProgress { get; } = new Dictionary<ushort, float>();
        public Dictionary<ushort, float> Done { get; } = new Dictionary<ushort, float>();
        public Dictionary<ushort, float> Cancelled { get; } = new Dictionary<ushort, float>();

        public void Track(ushort step, TasksManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            ToDo.Add(step, manager.ToDo.Count);
            InProgress.Add(step, manager.InProgress.Count);
            Done.Add(step, manager.Done.Count);
            Cancelled.Add(step, manager.Cancelled.Count);
        }

        public void Clear()
        {
            ToDo.Clear();
            InProgress.Clear();
            Done.Clear();
            Cancelled.Clear();
        }

        public void CopyTo(TasksManagerMetrics clone)
        {
            if (clone == null)
            {
                throw new ArgumentNullException(nameof(clone));
            }

            foreach (var todo in ToDo)
            {
                clone.ToDo.Add(todo.Key, todo.Value);
            }

            foreach (var inProgress in InProgress)
            {
                clone.InProgress.Add(inProgress.Key, inProgress.Value);
            }
            foreach (var done in Done)
            {
                clone.Done.Add(done.Key, done.Value);
            }
        }
    }
}