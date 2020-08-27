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
using Symu.Common.Interfaces.Entity;
using Symu.DNA.Beliefs;
using Symu.Repository.Entity;

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
        public List<IBelief> List { get; } = new List<IBelief>();

        public int Count => List.Count;

        public bool Any()
        {
            return List.Any();
        }

        public void Clear()
        {
            List.Clear();
        }

        public void Add(IBelief belief)
        {
            if (!Contains(belief))
            {
                List.Add(belief);
            }
        }

        public bool Contains(IBelief belief)
        {
            if (belief is null)
            {
                throw new ArgumentNullException(nameof(belief));
            }

            return Exists(belief.Id);
        }

        public IBelief GetBelief(IId beliefId)
        {
            return List.Find(k => k.Id.Equals(beliefId));
        }

        public TBelief GetBelief<TBelief>(IId beliefId) where TBelief : IBelief
        {
            return (TBelief)GetBelief(beliefId);
        }

        public bool Exists(IId beliefId)
        {
            return List.Exists(k => k.Id.Equals(beliefId));
        }
    }
}