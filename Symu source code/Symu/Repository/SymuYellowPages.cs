#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Diagnostics.CodeAnalysis;

#endregion

namespace Symu.Repository
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
        public const byte Stop = 0;

        public const byte PreStep = 1;
        public const byte WorkingDay = 2;
        public const byte Cadence = 3;
        public const byte EndOfYear = 4;
        public const byte EndOfMonth = 5;
        public const byte EndOfWeek = 6;

        public const byte Knowledge = 7;
        public const byte Belief = 8;
        public const byte Subscribe = 9;

        public const byte Scenario = 10;

        public const byte Organization = 11;
        public const byte Actor = 12;
        public const byte Tasks = 13;
        public const byte Help = 14;
        public const byte SplitStep = 15;
    }
}