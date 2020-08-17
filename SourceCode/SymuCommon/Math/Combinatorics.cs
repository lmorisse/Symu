#region Licence

// Description: SymuBiz - SymuTools
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

#endregion

#region using directives

using System;

#endregion

namespace Symu.Common.Math
{
    public static class Combinatorics
    {
        /// <summary>
        ///     Count the number of possible combinations without repetition.
        ///     The order does not matter and each object can be chosen only once.
        /// </summary>
        /// <param name="n">Number of elements in the set.</param>
        /// <param name="k">Number of elements to choose from the set. Each element is chosen at most once.</param>
        /// <returns>Maximum number of combinations.</returns>
        public static uint Combinations(int n, int k)
        {
            return Convert.ToUInt32(System.Math.Round(MathNet.Numerics.Combinatorics.Combinations(n, k)));
        }
    }
}