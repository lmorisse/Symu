#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;
using SymuEngine.Repository.Networks.Knowledges;

#endregion

namespace SymuEngine.Repository.Networks.Beliefs
{
    /// <summary>
    ///     A utility to easily switch from BeliefLevel to values
    /// </summary>
    public static class BeliefLevelService
    {
        /// <summary>
        ///     Get all names of the KnowledgeLevel enum
        /// </summary>
        /// <returns></returns>
        public static string[] GetNames()
        {
            return Enum.GetNames(typeof(BeliefLevel)).ToArray();
        }

        /// <summary>
        ///     Get the value based on the GenericLevel name
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static BeliefLevel GetValue(string level)
        {
            switch (level)
            {
                case "NoBelief":
                    return BeliefLevel.NoBelief;
                case "StronglyDisagree":
                    return BeliefLevel.StronglyDisagree;
                case "Disagree":
                    return BeliefLevel.Disagree;
                case "NeitherAgreeNorDisagree":
                    return BeliefLevel.NeitherAgreeNorDisagree;
                case "Agree":
                    return BeliefLevel.Agree;
                case "StronglyAgree":
                    return BeliefLevel.StronglyAgree;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Get the name of a KnowledgeLevel
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static string GetName(BeliefLevel level)
        {
            return level.ToString();
        }
    }
}