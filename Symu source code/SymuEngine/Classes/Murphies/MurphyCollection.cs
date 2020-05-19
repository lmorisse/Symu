#region Licence

// Description: Symu - Modeling
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;

#endregion

namespace SymuEngine.Classes.Murphies
{
    /// <summary>
    ///     Base Class to model a murphy
    ///     We design an optimal operating system
    ///     But under internal stress conditions, we have a suboptimal operating system
    ///     A murphy is an event that create an internal stress condition
    /// </summary>
    /// <example>
    ///     Time pressure
    ///     incomplete/changing/incorrect information
    ///     Communication breakdown
    ///     agent unavailable (bottleneck, illness, holidays, ...)
    /// </example>
    public class MurphyCollection
    {
        public MurphyUnAvailability UnAvailability { get; } = new MurphyUnAvailability();
        public MurphyIncompleteKnowledge IncompleteKnowledge { get; } = new MurphyIncompleteKnowledge();
        
        public MurphyIncompleteBelief IncompleteBelief { get; } = new MurphyIncompleteBelief();

        public List<Murphy> Murphies { get; } = new List<Murphy>();

        public MurphyCollection()
        {
            Add(IncompleteKnowledge);
            Add(IncompleteBelief);
            Add(UnAvailability);
        }

        public void Add(Murphy murphy)
        {
            Murphies.Add(murphy);
        }

        public TMurphy Get<TMurphy>() where TMurphy : Murphy
        {
            return Murphies.Find(x => x is TMurphy) as TMurphy;
        }
    }
}