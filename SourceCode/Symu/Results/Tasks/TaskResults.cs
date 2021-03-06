﻿#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Symu.Classes.Agents;
using Symu.Common.Classes;
using Symu.Common.Interfaces;
using Symu.Environment;

#endregion

namespace Symu.Results.Tasks
{
    /// <summary>
    ///     Manage the task metrics for the simulation
    /// </summary>
    public sealed class TaskResults : Result
    {
        public TaskResults(SymuEnvironment environment) : base(environment)
        {
            Frequency = TimeStepType.Daily;
        }

        /// <summary>
        ///     Key => step
        ///     Value => TaskResult for the step
        /// </summary>
        public ConcurrentDictionary<ushort, TaskResult> Tasks { get; private set; } =
            new ConcurrentDictionary<ushort, TaskResult>();

        /// <summary>
        ///     The number of connections between agents
        /// </summary>
        public List<DensityStruct> Capacity { get; private set; } = new List<DensityStruct>();

        /// <summary>
        ///     The number of connections between agents
        /// </summary>
        public List<float> SumCapacity { get; private set; } = new List<float>();

        public override void SetResults()
        {
            HandleTasks();
            HandleCapacity();
        }

        private void HandleCapacity()
        {
            float sum;
            float max;
            if (!Environment.AgentNetwork.Any())
            {
                return;
            }

            if (Environment.Schedule.IsWorkingDay)
            {
                max = Environment.AgentNetwork.AllCognitiveAgents()
                    .Count(agent => agent.Cognitive.TasksAndPerformance.CanPerformTask);
                sum = Environment.AgentNetwork.AllCognitiveAgents()
                    .Where(agent => agent.Cognitive.TasksAndPerformance.CanPerformTask)
                    .Select(x => x.Capacity.Initial).Sum();
            }
            else
            {
                max = Environment.AgentNetwork.AllCognitiveAgents()
                    .Count(agent => agent.Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds);
                sum = Environment.AgentNetwork.AllCognitiveAgents()
                    .Where(agent => agent.Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds)
                    .Select(x => x.Capacity.Initial).Sum();
            }

            var density = new DensityStruct(sum, max, Environment.Schedule.Step);
            Capacity.Add(density);

            SumCapacity.Add(Capacity.Sum(x => x.ActualNumber));
        }

        private void HandleTasks()
        {
            var result = new TaskResult();
            if (!Environment.AgentNetwork.Any())
            {
                return;
            }

            // alive agents
            HandleResults(Environment.AgentNetwork.AllCognitiveAgents().Where(agent => agent.TaskProcessor != null)
                .Select(x => x.TaskProcessor.TasksManager.TaskResult), result);
            // stopped agents
            HandleResults(Environment.AgentNetwork.StoppedAgents.OfType<CognitiveAgent>()
                .Where(agent => agent.TaskProcessor != null)
                .Select(x => x.TaskProcessor.TasksManager.TaskResult), result);
            Tasks.TryAdd(Environment.Schedule.Step, result);
        }

        private static void HandleResults(IEnumerable<TaskResult> taskResults, TaskResult result)
        {
            foreach (var taskResult in taskResults)
            {
                result.ToDo += taskResult.ToDo;
                result.InProgress += taskResult.InProgress;
                result.Done += taskResult.Done;
                result.Cancelled += taskResult.Cancelled;
                result.TotalTasksNumber += taskResult.TotalTasksNumber;
                result.WeightDone += taskResult.WeightDone;
                result.Incorrectness += taskResult.Incorrectness;
            }
        }

        public override void Clear()
        {
            Tasks.Clear();
            Capacity.Clear();
            SumCapacity.Clear();
        }

        public override void CopyTo(object clone)
        {
            if (!(clone is TaskResults cloneTasks))
            {
                return;
            }


            cloneTasks.Tasks = new ConcurrentDictionary<ushort, TaskResult>();
            foreach (var result in Tasks)
            {
                cloneTasks.Tasks.TryAdd(result.Key, result.Value);
            }

            cloneTasks.Capacity = new List<DensityStruct>();
            cloneTasks.Capacity.AddRange(Capacity);
            cloneTasks.SumCapacity = new List<float>();
            cloneTasks.SumCapacity.AddRange(SumCapacity);
        }

        public override IResult Clone()
        {
            var clone = new TaskResults(Environment);
            CopyTo(clone);
            return clone;
        }

        #region Shortcuts to tasks result

        /// <summary>
        ///     Total tasks still in to do
        /// </summary>
        public float AverageToDo => Tasks.Values.Any() ? (float) Tasks.Values.Average(x => x.ToDo) : 0F;

        /// <summary>
        ///     Total tasks still in progress
        /// </summary>
        public float AverageInProgress =>
            Tasks.Values.Any() ? (float) Tasks.Values.Average(x => x.InProgress) : 0F;

        /// <summary>
        ///     Total tasks still in done
        /// </summary>
        public float AverageDone => Tasks.Values.Any() ? (float) Tasks.Values.Average(x => x.Done) : 0F;

        /// <summary>
        ///     Total tasks done during the simulation
        /// </summary>
        public int Total => Tasks.Values.Any() ? Tasks.Values.Last().TotalTasksNumber : 0;

        /// <summary>
        ///     Total tasks done during the simulation
        /// </summary>
        public int Done => Tasks.Values.Any() ? Tasks.Values.Last().Done : 0;

        /// <summary>
        ///     Total tasks cancelled during the simulation
        /// </summary>
        public int Cancelled => Tasks.Values.Any() ? Tasks.Values.Last().Cancelled : 0;

        /// <summary>
        ///     Total impact of incorrectness
        /// </summary>
        public int Incorrectness => Tasks.Values.Any() ? Tasks.Values.Last().Incorrectness : 0;

        /// <summary>
        ///     Total weight of tasks done during the simulation
        /// </summary>
        public float Weight => Tasks.Values.Any() ? Tasks.Values.Last().WeightDone : 0;

        #endregion
    }
}