#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Diagnostics.CodeAnalysis;

#endregion

namespace SymuEngine.Repository
{
    /// <summary>
    ///     List of all the subjects that are managed in MAS
    /// </summary>
    [SuppressMessage("Design", "CA1052:Les types conteneurs statiques doivent être Static ou NotInheritable",
        Justification = "To be able to inherit from the class")]
    public class SymuYellowPages
    {
        /// <summary>
        ///     From 0 to 19
        /// </summary>
        public const byte stop = 0;

        public const byte preStep = 1;
        public const byte workingDay = 2;
        public const byte cadence = 3;
        public const byte endOfYear = 4;
        public const byte endOfMonth = 5;
        public const byte endOfWeek = 6;

        /// <summary>
        ///     ClassKey
        /// </summary>
        public const byte knowledge = 7;

        public const byte subscribe = 8;
        public const byte timer = 10;
        public const byte scenario = 11;

        public const byte organization = 12;
        public const byte actor = 13;
        public const byte tasks = 14;
    }
}