#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Concurrent;
using System.Linq;
using Symu.Environment;
using Symu.Messaging.Messages;

#endregion

namespace Symu.Results.Task
{
    /// <summary>
    ///     Manage the task metrics for the symu
    /// </summary>
    public class TaskResults
    {
        /// <summary>
        ///     If set to true, TaskResults will be filled with value and stored during the simulation
        /// </summary>
        public bool On { get; set; }

        /// <summary>
        ///     Key => step
        ///     Value => TaskResult for the step
        /// </summary>
        private readonly ConcurrentDictionary<ushort, TaskResult> _results =
            new ConcurrentDictionary<ushort, TaskResult>();

        /// <summary>
        ///     Total tasks still in to do
        /// </summary>
        public float AverageToDo => _results.Values.Any() ? (float) _results.Values.Average(x => x.ToDo) : 0F;

        /// <summary>
        ///     Total tasks still in progress
        /// </summary>
        public float AverageInProgress =>
            _results.Values.Any() ? (float) _results.Values.Average(x => x.InProgress) : 0F;

        /// <summary>
        ///     Total tasks still in done
        /// </summary>
        public float AverageDone => _results.Values.Any() ? (float) _results.Values.Average(x => x.Done) : 0F;

        /// <summary>
        ///     Total tasks done during the simulation
        /// </summary>
        public int Total => _results.Values.Any() ? _results.Values.Last().TotalTasksNumber : 0;

        /// <summary>
        ///     Total tasks done during the simulation
        /// </summary>
        public int Done => _results.Values.Any() ? _results.Values.Last().Done : 0;
        /// <summary>
        ///     Total tasks cancelled during the simulation
        /// </summary>
        public int Cancelled => _results.Values.Any() ? _results.Values.Last().Cancelled: 0;
        /// <summary>
        ///     Total impact of incorrectness 
        /// </summary>
        public int Incorrectness => _results.Values.Any() ? _results.Values.Last().Incorrectness : 0;

        /// <summary>
        ///     Total weight of tasks done during the simulation
        /// </summary>
        public float Weight => _results.Values.Any() ? _results.Values.Last().WeightDone : 0;

        public void SetResults(SymuEnvironment environment)
        {
            if (!On)
            {
                return;
            }

            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var result = new TaskResult();
            foreach (var tasksManager in environment.WhitePages.AllAgents().Where(agent => agent.TaskProcessor != null).Select(x => x.TaskProcessor.TasksManager))
            {
                result.ToDo += tasksManager.ToDo.Count(x => !(x.Parent is Message));
                result.InProgress += tasksManager.InProgress.Count(x => !(x.Parent is Message));
                result.Done += tasksManager.Done.Count(x => !(x.Parent is Message));
                result.Cancelled += tasksManager.Cancelled.Count(x => !(x.Parent is Message));
                result.TotalTasksNumber += tasksManager.TotalTasksNumber;
                result.WeightDone += tasksManager.TotalWeightDone;
                result.Incorrectness += tasksManager.AllTasks.Sum(x => (int)x.Incorrect);
            }

            _results.TryAdd(environment.Schedule.Step, result);
        }

        public void Clear()
        {
            _results.Clear();
        }
    }
}