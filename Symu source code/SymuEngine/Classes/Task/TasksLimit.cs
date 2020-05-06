#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;

#endregion

namespace SymuEngine.Classes.Task
{
    /// <summary>
    ///     Manage all limits related to Tasks
    /// </summary>
    /// <remarks>Tasks and Performance from Construct Software</remarks>
    public class TasksLimit
    {
        private byte _maximumSimultaneousTasks = 10;

        /// <summary>
        ///     This parameter specify that the maximum number of tasks that an agent of this class can perform during one
        ///     interaction period is not fixed if false
        ///     Default is set to false
        /// </summary>
        public bool LimitSimultaneousTasks { get; set; }

        /// <summary>
        ///     Maximum number of tasks performed simultaneously:
        ///     This parameter specify the maximum number of tasks that an agent of this class can perform during one interaction
        ///     period
        ///     If the maximum number of tasks is unlimited, SimultaneousTasksLimit should be set to true
        /// </summary>
        public byte MaximumSimultaneousTasks
        {
            get => _maximumSimultaneousTasks;
            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("MaximumSimultaneousTasks should be between [1; 255]");
                }

                _maximumSimultaneousTasks = value;
            }
        }

        /// <summary>
        ///     This parameter specify that the total maximum number of tasks that an agent of this class can perform during the
        ///     simulation is not fixed if false
        ///     Default is set to false
        /// </summary>
        public bool LimitTasksInTotal { get; set; }

        /// <summary>
        ///     This parameter specify the maximum number of tasks that an agent can perform over the course of the virtual
        ///     experiment.
        ///     This value is a maximum value,so agents may not be able to complete this number of tasks due to time or other
        ///     constraints.
        ///     If the total number of tasks is unlimited, the value should be set to ++.
        /// </summary>
        public ushort MaximumTasksInTotal { get; set; }

        /// <summary>
        ///     To be able to multitask, agent must be overcommitted to have enough available tasks to work on
        ///     OverCommitmentRatio is applied on agent capacity
        /// </summary>
        /// <example>OverCommitmentRatio= 0, agent is not overcommitted, if he is doing multitasking, he may be idle some times</example>
        /// <example>OverCommitmentRatio= 0.5, agent is 50% overcommitted , he has enough tasks to multi task</example>
        public float OverCommitmentRatio { get; set; } = 0.5F;

        public void CopyTo(TasksLimit tasksLimit)
        {
            if (tasksLimit is null)
            {
                throw new ArgumentNullException(nameof(tasksLimit));
            }

            tasksLimit.LimitSimultaneousTasks = LimitSimultaneousTasks;
            tasksLimit.MaximumSimultaneousTasks = MaximumSimultaneousTasks;
            tasksLimit.LimitTasksInTotal = LimitTasksInTotal;
            tasksLimit.MaximumTasksInTotal = MaximumTasksInTotal;
            tasksLimit.OverCommitmentRatio = OverCommitmentRatio;
        }

        /// <summary>
        ///     Set agent taskLimits from this global model TasksLimit
        /// </summary>
        /// <param name="agentTasksLimit">taskLimit to set</param>
        public void SetAgentTasksLimit(TasksLimit agentTasksLimit)
        {
            if (agentTasksLimit is null)
            {
                throw new ArgumentNullException(nameof(agentTasksLimit));
            }

            if (!LimitSimultaneousTasks)
            {
                agentTasksLimit.LimitSimultaneousTasks = false;
            }
            else
            {
                agentTasksLimit.MaximumSimultaneousTasks =
                    Math.Min(agentTasksLimit.MaximumSimultaneousTasks, MaximumSimultaneousTasks);
            }

            if (!LimitTasksInTotal)
            {
                agentTasksLimit.LimitTasksInTotal = false;
            }
            else
            {
                agentTasksLimit.MaximumTasksInTotal =
                    Math.Min(agentTasksLimit.MaximumTasksInTotal, MaximumTasksInTotal);
            }
        }

        /// <summary>
        ///     Check if the number of tasks has reached the maximum number of tasks allowed
        /// </summary>
        /// <param name="tasksNumber"></param>
        /// <returns>true if tasks number > Maximum tasks</returns>
        public bool HasReachedTotalMaximumLimit(ushort tasksNumber)
        {
            if (!LimitTasksInTotal)
            {
                return false;
            }

            return tasksNumber >= MaximumTasksInTotal;
        }

        /// <summary>
        ///     Check if the number of tasks has reached the maximum number of simultaneous tasks allowed
        /// </summary>
        /// <param name="tasksNumber"></param>
        /// <returns>true if the maximum is reached</returns>
        public bool HasReachedSimultaneousMaximumLimit(ushort tasksNumber)
        {
            if (!LimitSimultaneousTasks)
            {
                return false;
            }

            return tasksNumber >= MaximumSimultaneousTasks;
        }
    }
}