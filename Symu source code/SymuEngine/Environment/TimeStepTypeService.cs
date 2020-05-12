#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;

#endregion

namespace SymuEngine.Environment
{
    public static class TimeStepTypeService
    {
        /// <summary>
        ///     Get all names of the KnowledgeLevel enum
        /// </summary>
        /// <returns></returns>
        public static string[] GetNames()
        {
            return Enum.GetNames(typeof(TimeStepType)).ToArray();
        }

        /// <summary>
        ///     Get the value based on the GenericLevel name
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static TimeStepType GetValue(string level)
        {
            switch (level)
            {
                case "Intraday":
                    return TimeStepType.Intraday;
                case "Daily":
                    return TimeStepType.Daily;
                case "Weekly":
                    return TimeStepType.Weekly;
                case "Monthly":
                    return TimeStepType.Monthly;
                case "Yearly":
                    return TimeStepType.Yearly;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Get the name of a TimeStepType
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static string GetName(TimeStepType level)
        {
            return level.ToString();
        }
    }
}