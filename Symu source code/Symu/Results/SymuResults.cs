#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Environment;

#endregion

namespace Symu.Results
{
    /// <summary>
    ///     Base class for iteration results
    ///     You may implement your own specifics results class:
    ///     Add a list or a dictionary with the tuple (step, result)
    /// </summary>
    public abstract class SymuResults
    {
        protected SymuResults(SymuEnvironment environment)
        {
            Environment = environment;
        }

        protected SymuEnvironment Environment { get; }

        /// <summary>
        ///     If set to true, Tasks will be filled with value and stored during the simulation
        /// </summary>
        public bool On { get; set; }

        /// <summary>
        ///     Frequency of the results
        /// </summary>
        public TimeStepType Frequency { get; set; } = TimeStepType.Monthly;

        public void SetResults()
        {
            if (!On)
            {
                return;
            }

            bool handle;
            switch (Frequency)
            {
                case TimeStepType.Intraday:
                case TimeStepType.Daily:
                    handle = true;
                    break;
                case TimeStepType.Weekly:
                    handle = Environment.Schedule.IsEndOfWeek;
                    break;
                case TimeStepType.Monthly:
                    handle = Environment.Schedule.IsEndOfMonth;
                    break;
                case TimeStepType.Yearly:
                    handle = Environment.Schedule.IsEndOfYear;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!handle)
            {
                return;
            }

            HandleResults();
        }

        /// <summary>
        ///     Put the logic to compute the result and store it in the list
        /// </summary>
        protected abstract void HandleResults();

        /// <summary>
        ///     Clear the list or all information
        /// </summary>
        public abstract void Clear();

        /// <summary>
        ///     Clone the instance.
        ///     IterationResult is the actual iterationResult.
        ///     With a multiple iterations simulation, SimulationResults store a clone of each IterationResult
        /// </summary>
        /// <returns></returns>
        public abstract void CopyTo(object clone);

        /// <summary>
        ///     Clone the instance.
        ///     IterationResult is the actual iterationResult.
        ///     With a multiple iterations simulation, SimulationResults store a clone of each IterationResult
        /// </summary>
        /// <returns></returns>
        public abstract SymuResults Clone();
    }
}