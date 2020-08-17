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
    ///     A utility to easily switch from FrequencyLevel to values
    /// </summary>
    public static class FrequencyLevelService
    {
        /// <summary>
        ///     Get all names of the KnowledgeLevel enum
        /// </summary>
        /// <returns></returns>
        public static string[] GetNames()
        {
            return Enum.GetNames(typeof(Frequency)).ToArray();
        }

        /// <summary>
        ///     Get the value based on the GenericLevel name
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static Frequency GetValue(string level)
        {
            switch (level)
            {
                case "Never":
                    return Frequency.Never;
                case "VeryRarely":
                    return Frequency.VeryRarely;
                case "Rarely":
                    return Frequency.Rarely;
                case "Medium":
                    return Frequency.Medium;
                case "Often":
                    return Frequency.Often;
                case "VeryOften":
                    return Frequency.VeryOften;
                case "Always":
                    return Frequency.Always;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Get the name of a Frequency
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static string GetName(Frequency level)
        {
            return level.ToString();
        }
    }
}