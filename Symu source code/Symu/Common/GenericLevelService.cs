#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;

#endregion

namespace Symu.Common
{
    /// <summary>
    ///     A utility to easily switch from GenericLevel to values
    /// </summary>
    public static class GenericLevelService
    {
        /// <summary>
        ///     Get all names of the GenericLevel enum
        /// </summary>
        /// <returns></returns>
        public static string[] GetNames()
        {
            return Enum.GetNames(typeof(GenericLevel)).ToArray();
        }

        /// <summary>
        ///     Get the value based on the GenericLevel name
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static GenericLevel GetValue(string level)
        {
            switch (level)
            {
                case "None":
                    return GenericLevel.None;
                case "VeryLow":
                    return GenericLevel.VeryLow;
                case "Low":
                    return GenericLevel.Low;
                case "Medium":
                    return GenericLevel.Medium;
                case "High":
                    return GenericLevel.High;
                case "VeryHigh":
                    return GenericLevel.VeryHigh;
                case "Complete":
                    return GenericLevel.Complete;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Get the name of a GenericLevel
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static string GetName(GenericLevel level)
        {
            return level.ToString();
        }
    }
}