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

namespace Symu.Repository.Networks.Beliefs
{
    /// <summary>
    ///     A utility to easily switch from BeliefWeightLevel to values
    /// </summary>
    public static class BeliefWeightLevelService
    {
        /// <summary>
        ///     Get all names of the KnowledgeLevel enum
        /// </summary>
        /// <returns></returns>
        public static string[] GetNames()
        {
            return Enum.GetNames(typeof(BeliefWeightLevel)).ToArray();
        }

        /// <summary>
        ///     Get the value based on the GenericLevel name
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static BeliefWeightLevel GetValue(string level)
        {
            switch (level)
            {
                case "NoWeight":
                    return BeliefWeightLevel.NoWeight;
                case "RandomWeight":
                    return BeliefWeightLevel.RandomWeight;
                case "FullWeight":
                    return BeliefWeightLevel.FullWeight;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Get the name of a BeliefWeightLevel
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static string GetName(BeliefWeightLevel level)
        {
            return level.ToString();
        }
    }
}