#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Diagnostics.CodeAnalysis;
using Symu.OrgMod.Entities;

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
        public const byte Role = ClassIdCollection.Role;

        public const byte Knowledge = ClassIdCollection.Knowledge;
        public const byte Belief = ClassIdCollection.Belief;
        public const byte Organization = ClassIdCollection.Organization;
        public const byte Actor = ClassIdCollection.Actor;
        public const byte Event = ClassIdCollection.Event;
        public const byte Resource = ClassIdCollection.Resource;
        public const byte Task = ClassIdCollection.Task;
        public const byte Stop = 8;
        public const byte PreStep = 9;
        public const byte WorkingDay = 10;
        public const byte EndOfWeek = 11;
        public const byte Subscribe = 12;
        public const byte Scenario = 13;
        public const byte Help = 14;
        public const byte SplitStep = 15;
        public const byte Wiki = 16;
        public const byte Email = 17;

        public const byte Database = 18;
        //public const byte Cadence = 11;
        //public const byte EndOfYear = 12;
        //public const byte EndOfMonth = 13;
    }
}