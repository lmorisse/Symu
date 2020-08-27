#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using Symu.Classes.Agents.Models;
using Symu.Classes.Task;
using Symu.Common.Interfaces.Entity;
using Symu.Environment;
using Symu.Messaging.Messages;
using static Symu.Common.Constants;

#endregion

namespace Symu.Classes.Agents
{
    /// <summary>
    ///     An abstract base class for agents.
    ///     You must define your own agent derived classes derived
    ///     This partial class focus on tasks management methods
    /// </summary>
    public abstract partial class CognitiveAgent
    {
        #region Work on task

        /// <summary>
        ///     Override this method to specify how an agent will get new tasks to complete
        ///     Define a task then Post(task)
        ///     By default, if worker can't perform task or has reached the maximum number of tasks,
        ///     he can't ask for more tasks, just finished the tasks in the taskManager
        /// </summary>
        public virtual void GetNewTasks()
        {
        }

        /// <summary>
        ///     Work on the next task
        /// </summary>
        public void WorkInProgress(SymuTask task)
        {
            if (task == null)
            {
                Status = AgentStatus.Available;
                return;
            }

            // The task may be blocked, try to unlock it
            TryRecoverBlockedTask(task);
            // Agent may discover new blockers
            CheckBlockers(task);
            // Task may have been blocked or cancelled
            // Capacity may have been used for blockers
            if (!task.IsBlocked && Capacity.HasCapacity && task.IsAssigned) // && !task.IsCancelledBy(Id)
            {
                WorkOnTask(task);
            }

            if (Capacity.HasCapacity)
                // We start a new loop on the current tasks of the agent
            {
                SwitchingContextModel();
            }
        }

        /// <summary>
        ///     Simulate the work on a specific task
        /// </summary>
        /// <param name="task"></param>
        public virtual float WorkOnTask(SymuTask task)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            float timeSpent;
            if (Schedule.Type == TimeStepType.Intraday)
            {
                timeSpent = Math.Min(Environment.Organization.Models.Intraday, Capacity.Actual);
            }
            else
            {
                timeSpent = Cognitive.TasksAndPerformance.TasksLimit.LimitSimultaneousTasks
                    // Mono tasking
                    ? Math.Min(task.Weight, Capacity.Actual)
                    // Multi tasking
                    : Math.Min(task.Weight / 2, Capacity.Actual);
            }

            timeSpent = Math.Min(task.WorkToDo, timeSpent);
            task.WorkToDo -= timeSpent;
            if (task.WorkToDo < Tolerance)
            {
                SetTaskDone(task);
            }
            else
            {
                UpdateTask(task);
            }

            // As the agent work on task that requires knowledge, the agent can't forget the associate knowledge today
            ForgettingModel.UpdateForgettingProcess(task.KnowledgesBits);

            Capacity.Decrement(timeSpent);
            return timeSpent;
        }

        /// <summary>
        ///     Clone the task done in task manager
        /// </summary>
        /// <param name="task"></param>
        public void SetTaskDone(SymuTask task)
        {
            TaskProcessor.SetTaskDone(task);
        }

        /// <summary>
        ///     Update the task as the agent has worked on it, but not complete it
        /// </summary>
        /// <param name="task"></param>
        public void UpdateTask(SymuTask task)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            task.Update(Schedule.Step);
        }

        #endregion

        #region Capacity

        /// <summary>
        ///     Describe the agent capacity
        /// </summary>
        public AgentCapacity Capacity { get; } = new AgentCapacity();

        /// <summary>
        ///     Clone the initial capacity for the new step based on SetInitialCapacity, working day,
        ///     By default = Initial capacity if it's a working day, 0 otherwise
        ///     If resetRemainingCapacity set to true, Remaining capacity is reset to Initial Capacity value
        /// </summary>
        public void HandleCapacity(bool isolated, bool resetRemainingCapacity)
        {
            // Intentionally no test on Agent that must be able to perform tasks
            // && Cognitive.TasksAndPerformance.CanPerformTask
            // Example : internet access don't perform task, but is online
            if (IsPerformingTask(isolated))
            {
                SetInitialCapacity();
                // Intentionally after SetInitialCapacity
                MurphiesImpactsOnCapacity();
            }
            else
            {
                Capacity.Initial = 0;
            }

            if (resetRemainingCapacity)
            {
                Capacity.Reset();
            }
        }

        /// <summary>
        ///     Use to set the baseline value of the initial capacity
        /// </summary>
        /// <returns></returns>
        public virtual void SetInitialCapacity()
        {
            Capacity.Initial = 1;
        }

        /// <summary>
        ///     Murphies impacts on the capacity
        /// </summary>
        /// <returns></returns>
        public virtual void MurphiesImpactsOnCapacity()
        {
            // Unavailability
            if (Environment.Organization.Murphies.UnAvailability.Next())
            {
                Capacity.Initial = 0;
                Status = AgentStatus.Offline;
            }
        }

        /// <summary>
        ///     Switching context may have an impact on the agent capacity
        /// </summary>
        public virtual void SwitchingContextModel()
        {
        }

        #endregion

        #region TimeSpent

        /// <summary>
        ///     Daily Track of the TimeSPent for each keyActivity
        ///     Key => step
        ///     Value => time spent
        /// </summary>
        public Dictionary<UId, float> TimeSpent { get; } = new Dictionary<UId, float>();

        /// <summary>
        ///     Impact of the Communication channels on the time spent
        ///     Allocate this time on the keyActivity
        /// </summary>
        /// <param name="medium"></param>
        /// <param name="keyActivity">the keyActivity activity of the task, to track TimeSpent</param>
        /// <param name="send">If set, it is an ask help task, otherwise it is a reply help task</param>
        /// <remarks>Impact on capacity is done in OnBeforeSendMessage and OnAfterPostMessage</remarks>
        public void ImpactOfTheCommunicationMediumOnTimeSpent(CommunicationMediums medium, bool send,
            UId keyActivity)
        {
            if (keyActivity == null || keyActivity.IsNull)
            {
                return;
            }

            var impact =
                Environment.Organization.Communication.TimeSpent(medium, send,
                    Environment.Organization.Models.RandomLevelValue);
            AddTimeSpent(keyActivity, impact);
        }

        /// <summary>
        ///     Manage TimeSpent of the agent, by keyActivity
        /// </summary>
        /// <param name="keyActivity"></param>
        /// <param name="timeSpent"></param>
        public void AddTimeSpent(UId keyActivity, float timeSpent)
        {
            if (keyActivity == null)
            {
                // Task may not have a KeyActivity, a message transform into a task for example.
                // This can be improve inCognitiveAgent.Act.ConvertMessageIntoTask
                return;
            }
            if (TimeSpent == null)
            {
                throw new ArgumentNullException(nameof(TimeSpent));
            }

            if (!TimeSpent.ContainsKey(keyActivity))
            {
                TimeSpent.Add(keyActivity, 0);
            }

            TimeSpent[keyActivity] += timeSpent;
        }

        #endregion

        #region Post task

        /// <summary>
        ///     Post a task in the TasksProcessor
        /// </summary>
        /// <param name="task"></param>
        /// <remarks>Don't use TaskProcessor.Post directly to handle the OnBeforeTaskPost event</remarks>
        public void Post(SymuTask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            // if filtering tasks here, we must unassigned the task otherwise the task would not be taken by anyone
            // for the moment it is not filtered
            if (!Cognitive.TasksAndPerformance.CanPerformTask) // || task.IsCancelledBy(Id)) //|| task.IsBlocked)
            {
                return;
            }

            OnBeforePostTask(task);
            TaskProcessor.Post(task);
            OnAfterPostTask(task);
        }

        /// <summary>
        ///     Post a task in the TasksProcessor
        /// </summary>
        /// <param name="tasks"></param>
        /// <remarks>Don't use TaskProcessor.Post directly to handle the OnBeforeTaskPost event</remarks>
        public void Post(IEnumerable<SymuTask> tasks)
        {
            if (tasks is null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            if (!Cognitive.TasksAndPerformance.CanPerformTask)
            {
                return;
            }

            // if filtering tasks here, we must unassigned the task otherwise the task would not be taken by anyone
            // for the moment it is not filtered
            foreach (var task in tasks) //.Where(x => !x.IsCancelledBy(Id))) //.Where(x => !x.IsBlocked))
            {
                OnBeforePostTask(task);
                TaskProcessor.Post(task);
                OnAfterPostTask(task);
            }
        }

        /// <summary>
        ///     EventHandler triggered before the event TaskProcessor.Post(task)
        /// </summary>
        /// <param name="task"></param>
        protected virtual void OnBeforePostTask(SymuTask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            task.Assigned = AgentId;
        }

        /// <summary>
        ///     EventHandler triggered after the event TaskProcessor.Post(task)
        /// </summary>
        /// <param name="task"></param>
        protected virtual void OnAfterPostTask(SymuTask task)
        {
        }

        #endregion
    }
}