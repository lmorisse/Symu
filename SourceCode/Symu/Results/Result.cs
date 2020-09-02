#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Common.Classes;
using Symu.Common.Interfaces;
using Symu.Environment;

#endregion

namespace Symu.Results
{
    /// <summary>
    ///     Default implementation of IResult
    ///     Base class for iteration results
    ///     You may implement your own specifics results class:
    ///     Add a list or a dictionary with the tuple (step, result)
    /// </summary>
    public abstract class Result : IResult
    {
        protected Result(SymuEnvironment environment)
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

        /// <summary>
        ///     Put the logic to compute the result and store it in the list
        /// </summary>
        public abstract void SetResults();

        /// <summary>
        ///     Clear the list or all information
        /// </summary>
        public abstract void Clear();

        /// <summary>
        ///     Copy each parameter of the instance in the object
        /// </summary>
        /// <returns></returns>
        public abstract void CopyTo(object clone);

        /// <summary>
        ///     Clone the instance.
        ///     IterationResult is the actual iterationResult.
        ///     With a multiple iterations simulation, SimulationResults store a clone of each IterationResult
        ///     It is a factory that create a SymuResults then CopyTo the existing instance and return the clone
        /// </summary>
        /// <returns></returns>
        public abstract IResult Clone();
    }
}