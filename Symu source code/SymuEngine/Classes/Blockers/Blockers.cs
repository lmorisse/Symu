#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using System.Linq;

#endregion

namespace SymuEngine.Classes.Blockers
{
    /// <summary>
    ///     Manage the list of blockers of a task
    /// </summary>
    public class Blockers
    {
        public List<Blocker> List { get; } = new List<Blocker>();
        public bool IsBlocked => List.Any();

        /// <summary>
        ///     Add a blocker with two parameters
        /// </summary>
        /// <param name="type">type of the blocker</param>
        /// <param name="step">step of creation of the blocker</param>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        public Blocker Add(int type, ushort step, object parameter1, object parameter2)
        {
            var blocker = new Blocker(type, step, parameter1, parameter2);
            List.Add(blocker);
            return blocker;
        }

        /// <summary>
        ///     Add a blocker with one parameter
        /// </summary>
        /// <param name="type">type of the blocker</param>
        /// <param name="step">step of creation of the blocker</param>
        /// <param name="parameter"></param>
        public Blocker Add(int type, ushort step, object parameter)
        {
            var blocker = new Blocker(type, step, parameter);
            List.Add(blocker);
            return blocker;
        }

        /// <summary>
        ///     Add a blocker without parameter
        /// </summary>
        /// <param name="type">type of the blocker</param>
        /// <param name="step">step of creation of the blocker</param>
        public Blocker Add(int type, ushort step)
        {
            var blocker = new Blocker(type, step);
            List.Add(blocker);
            return blocker;
        }

        /// <summary>
        ///     Remove a blocker
        /// </summary>
        /// <param name="blocker"></param>
        public void Remove(Blocker blocker)
        {
            List.Remove(blocker);
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
        ///     Clear the list of blockers
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