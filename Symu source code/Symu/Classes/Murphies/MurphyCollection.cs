#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Symu.Classes.Murphies
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
        public MurphyCollection()
        {
            Add(IncompleteInformation);
            Add(IncompleteKnowledge);
            Add(IncompleteBelief);
            Add(UnAvailability);
        }

        /// <summary>
        ///     If true, allow multiple blockers at the same time
        ///     If false, will check new blockers only if there is no blocker
        /// </summary>
        public bool MultipleBlockers { get; set; }

        public MurphyUnAvailability UnAvailability { get; } = new MurphyUnAvailability();
        public MurphyIncompleteKnowledge IncompleteKnowledge { get; } = new MurphyIncompleteKnowledge();

        public MurphyIncompleteBelief IncompleteBelief { get; } = new MurphyIncompleteBelief();

        public MurphyIncompleteInformation IncompleteInformation { get; } = new MurphyIncompleteInformation();

        public List<Murphy> Murphies { get; } = new List<Murphy>();

        /// <summary>
        ///     CopyTo all murphies on
        /// </summary>
        /// <param name="rate"></param>
        public void On(float rate)
        {
            foreach (var murphy in Murphies.Where(x => x != null))
            {
                murphy.On = true;
                murphy.RateOfAgentsOn = rate;
            }
        }

        /// <summary>
        ///     CopyTo all murphies off
        /// </summary>
        public void Off()
        {
            foreach (var murphy in Murphies.Where(x => x != null))
            {
                murphy.On = false;
            }
        }
        /// <summary>
        /// Add a murphy to the collection
        /// </summary>
        /// <param name="murphy"></param>
        public void Add(Murphy murphy)
        {
            Murphies.Add(murphy);
        }
        /// <summary>
        /// Get a murphy from the collection by its type
        /// </summary>
        /// <typeparam name="TMurphy"></typeparam>
        /// <returns></returns>
        public TMurphy Get<TMurphy>() where TMurphy : Murphy
        {
            return Murphies.Find(x => x is TMurphy) as TMurphy;
        }
    }
}