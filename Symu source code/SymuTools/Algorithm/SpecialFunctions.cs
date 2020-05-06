#region Licence

// Description: Symu - SymuTools
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

#endregion

namespace SymuTools.Algorithm
{
    public static class SpecialFunctions
    {
        /// <summary>
        ///     Return the factorial of a number
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Factorial(int x)
            // Don't change double for float
        {
            return MathNet.Numerics.SpecialFunctions.Factorial(x);
        }
    }
}