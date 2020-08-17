#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Symu.Classes.Blockers
{
    /// <summary>
    ///     Manage the list of blockers of a task
    /// </summary>
    public class BlockerCollection
    {
        public List<Blocker> List { get; } = new List<Blocker>();

        public bool IsBlocked => List.Any();

        public void Add(Blocker blocker)
        {
            List.Add(blocker);
        }

        /// <summary>
        ///     Remove an existing blocker from a task
        ///     And update IterationResult if FollowBlocker is true
        /// </summary>
        /// <param name="blocker"></param>
        public bool Remove(Blocker blocker)
        {
            if (blocker == null || !Contains(blocker))
            {
                // Blocker may have been already resolved
                return false;
            }

            List.Remove(blocker);
            return true;
        }

        /// <summary>
        ///     Remove all blockers of a step
        /// </summary>
        /// <param name="step"></param>
        public void Remove(ushort step)
        {
            List.RemoveAll(m => m.Equals(step));
        }

        /// <summary>
        ///     Get all the blockers except the blockers of the day
        /// </summary>
        /// <param name="step"></param>
        /// <returns>list of blockers</returns>
        public List<Blocker> FilterBlockers(ushort step)
        {
            return List.FindAll(m => !m.Equals(step));
        }

        /// <summary>
        ///     Get the blocker except the blockers of the day
        /// </summary>
        /// <param name="type"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public Blocker GetBlocker(int type, ushort step)
        {
            return List.Find(m => m.Equals(type, step));
        }

        /// <summary>
        ///     Has no blockers for the day
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public bool NotBlockedToday(ushort step)
        {
            return !List.Exists(m => m.Equals(step));
        }

        /// <summary>
        ///     Has no blockers for the day for the specific Blocker
        /// </summary>
        /// <param name="type"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public bool Exists(int type, ushort step)
        {
            return List.Exists(m => m.Equals(type, step));
        }

        /// <summary>
        ///     Initialize the list of blockers
        /// </summary>
        public void Clear()
        {
            List.Clear();
        }

        public bool Contains(Blocker blocker)
        {
            return List.Contains(blocker);
        }
    }
}