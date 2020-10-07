#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;

#endregion

namespace Symu.Results
{
    /// <summary>
    ///     The list of all IterationResults of a symu
    /// </summary>
    public class SimulationResults
    {
        /// <summary>
        ///     List of specific (except generic DNA result) IterationResult
        /// </summary>
        public List<IterationResult> List { get; } = new List<IterationResult>();

        public int Count => List.Count;

        /// <summary>
        ///     Gets or sets the element at the specified index
        /// </summary>
        /// <param name="index">0 based</param>
        /// <returns></returns>
        public IterationResult this[int index]
        {
            get => List[index];
            set => List[index] = value;
        }

        internal void AddRange(SimulationResults simulationResults)
        {
            List.AddRange(simulationResults.List);
        }

        public void Clear()
        {
            List.Clear();
        }
    }
}