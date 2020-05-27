#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace Symu.Results.Task
{
    public class TaskResult
    {
        /// <summary>
        ///     Number of tasks in To Do
        /// </summary>
        public int ToDo { get; set; }

        /// <summary>
        ///     Number of tasks In Progress
        /// </summary>
        public int InProgress { get; set; }

        /// <summary>
        ///     Number of tasks in Done
        /// </summary>
        public int Done { get; set; }

        /// <summary>
        ///     Number of tasks in Cancelled
        /// </summary>
        public int Cancelled { get; set; }

        /// <summary>
        ///     Number of tasks
        /// </summary>
        public int TotalTasksNumber { get; set; }

        /// <summary>
        ///     Total weight of tasks done
        /// </summary>
        public float WeightDone { get; set; }
        /// <summary>
        ///     Total impact of incorrectness
        /// </summary>
        public int Incorrectness { get; set; }
    }
}