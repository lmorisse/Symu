#region Licence

// Description: Symu - Symu
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
        ///     Key => step
        ///     Value => TaskResult for the step
        /// </summary>
        public ConcurrentDictionary<ushort, TaskResult> Results { get; private set; } =
            new ConcurrentDictionary<ushort, TaskResult>();

        /// <summary>
        ///     If set to true, TaskResults will be filled with value and stored during the simulation
        /// </summary>
        public bool On { get; set; }

        /// <summary>
        ///     Total tasks still in to do
        /// </summary>
        public float AverageToDo => Results.Values.Any() ? (float) Results.Values.Average(x => x.ToDo) : 0F;

        /// <summary>
        ///     Total tasks still in progress
        /// </summary>
        public float AverageInProgress =>
            Results.Values.Any() ? (float) Results.Values.Average(x => x.InProgress) : 0F;

        /// <summary>
        ///     Total tasks still in done
        /// </summary>
        public float AverageDone => Results.Values.Any() ? (float) Results.Values.Average(x => x.Done) : 0F;

        /// <summary>
        ///     Total tasks done during the simulation
        /// </summary>
        public int Total => Results.Values.Any() ? Results.Values.Last().TotalTasksNumber : 0;

        /// <summary>
        ///     Total tasks done during the simulation
        /// </summary>
        public int Done => Results.Values.Any() ? Results.Values.Last().Done : 0;

        /// <summary>
        ///     Total tasks cancelled during the simulation
        /// </summary>
        public int Cancelled => Results.Values.Any() ? Results.Values.Last().Cancelled : 0;

        /// <summary>
        ///     Total impact of incorrectness
        /// </summary>
        public int Incorrectness => Results.Values.Any() ? Results.Values.Last().Incorrectness : 0;

        /// <summary>
        ///     Total weight of tasks done during the simulation
        /// </summary>
        public float Weight => Results.Values.Any() ? Results.Values.Last().WeightDone : 0;

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
            foreach (var taskResult in environment.WhitePages.AllAgents().Where(agent => agent.TaskProcessor != null)
                .Select(x => x.TaskProcessor.TasksManager.TaskResult))
            {
                result.ToDo += taskResult.ToDo;
                result.InProgress += taskResult.InProgress;
                result.Done += taskResult.Done;
                result.Cancelled += taskResult.Cancelled;
                result.TotalTasksNumber += taskResult.TotalTasksNumber;
                result.WeightDone += taskResult.WeightDone;
                result.Incorrectness += taskResult.Incorrectness;
            }

            Results.TryAdd(environment.Schedule.Step, result);
        }

        public void Clear()
        {
            Results.Clear();
        }

        public void CopyTo(TaskResults cloneTasks)
        {
            if (cloneTasks == null)
            {
                throw new ArgumentNullException(nameof(cloneTasks));
            }

            cloneTasks.Results = new ConcurrentDictionary<ushort, TaskResult>();
            foreach (var result in Results)
            {
                cloneTasks.Results.TryAdd(result.Key, result.Value);
            }
        }
    }
}