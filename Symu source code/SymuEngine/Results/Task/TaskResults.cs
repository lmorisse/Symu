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
using SymuEngine.Environment;
using SymuEngine.Messaging.Messages;

#endregion

namespace SymuEngine.Results.Task
{
    /// <summary>
    ///     Manage the task metrics for the simulation
    /// </summary>
    public class TaskResults
    {
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
        ///     Total weight of tasks done during the simulation
        /// </summary>
        public float Weight => _results.Values.Any() ? _results.Values.Last().WeightDone : 0;

        public void SetResults(SymuEnvironment environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var result = new TaskResult();
            foreach (var agent in environment.WhitePages.AllAgents().Where(agent => agent.TaskProcessor != null))
            {
                result.ToDo += agent.TaskProcessor.TasksManager.ToDo.Count(x => !(x.Parent is Message));
                result.InProgress += agent.TaskProcessor.TasksManager.InProgress.Count(x => !(x.Parent is Message));
                result.Done += agent.TaskProcessor.TasksManager.Done.Count(x => !(x.Parent is Message));
                result.TotalTasksNumber += agent.TaskProcessor.TasksManager.TotalTasksNumber;
                result.WeightDone += agent.TaskProcessor.TasksManager.TotalWeightDone;
            }

            _results.TryAdd(environment.TimeStep.Step, result);
        }
    }
}