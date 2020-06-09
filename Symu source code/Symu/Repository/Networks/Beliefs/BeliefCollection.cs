#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Symu.Repository.Networks.Beliefs
{
    /// <summary>
    ///     List of all the Beliefs
    ///     Use by NetworkBeliefs
    /// </summary>
    public class BeliefCollection
    {
        /// <summary>
        ///     Key => ComponentId
        ///     Values => List of Belief
        /// </summary>
        public List<Belief> List { get; } = new List<Belief>();

        public int Count => List.Count;

        public bool Any()
        {
            return List.Any();
        }

        public void Clear()
        {
            List.Clear();
        }

        public void Add(Belief belief)
        {
            if (!Contains(belief))
            {
                List.Add(belief);
            }
        }

        public bool Contains(Belief belief)
        {
            if (belief is null)
            {
                throw new ArgumentNullException(nameof(belief));
            }

            return Exists(belief.Id);
        }

        public Belief GetBelief(ushort beliefId)
        {
            return List.Find(k => k.Id == beliefId);
        }

        public bool Exists(ushort beliefId)
        {
            return List.Exists(k => k.Id == beliefId);
        }
    }
}