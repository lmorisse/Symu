#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Symu.Results.Blocker;

#endregion

namespace Symu.Classes.Blockers
{
    /// <summary>
    ///     Manage the list of blockers of a task
    /// </summary>
    public class BlockerCollection
    {
        /// <summary>
        ///     Number of blockers In Progress
        /// </summary>
        public BlockerResult Result { get; set; } = new BlockerResult();
        public List<Blocker> List { get; } = new List<Blocker>();
        public bool IsBlocked => List.Any();

        /// <summary>
        ///     Add a blocker with two parameters
        ///     And follow it in the IterationResult if FollowBlocker is true
        /// </summary>
        /// <param name="type">type of the blocker</param>
        /// <param name="step">step of creation of the blocker</param>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        public Blocker Add(int type, ushort step, object parameter1, object parameter2)
        {
            var blocker = new Blocker(type, step, parameter1, parameter2);
            return Add(step, blocker);
        }

        /// <summary>
        ///     Add a blocker with one parameter
        ///     And follow it in the IterationResult if FollowBlocker is true
        /// </summary>
        /// <param name="type">type of the blocker</param>
        /// <param name="step">step of creation of the blocker</param>
        /// <param name="parameter"></param>
        public Blocker Add(int type, ushort step, object parameter)
        {
            var blocker = new Blocker(type, step, parameter);
            return Add(step, blocker);
        }

        private Blocker Add(ushort step, Blocker blocker)
        {
            SetBlockerInProgress();
            List.Add(blocker);
            return blocker;
        }

        /// <summary>
        ///     Add a blocker without parameter
        ///     And follow it in the IterationResult if FollowBlocker is true
        /// </summary>
        /// <param name="type">type of the blocker</param>
        /// <param name="step">step of creation of the blocker</param>
        public Blocker Add(int type, ushort step)
        {
            var blocker = new Blocker(type, step);
            return Add(step, blocker);
        }

        /// <summary>
        ///     Remove an existing blocker from a task
        ///     And update IterationResult if FollowBlocker is true
        /// </summary>
        /// <param name="blocker"></param>
        /// <param name="resolution"></param>
        /// <param name="step"></param>
        public void Recover(Blocker blocker, BlockerResolution resolution, ushort step)
        {
            if (!Contains(blocker))
            {
                // Blocker may have been already resolved
                return;
            }
            if (blocker != null)
            {
                Remove(blocker);
            }

            SetBlockerDone(resolution);
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
        public void SetBlockerInProgress()
        {
            Result.InProgress++;
        }

        public void SetBlockerDone(BlockerResolution resolution)
        {
            switch (resolution)
            {
                case BlockerResolution.Internal:
                    Result.InternalHelp++;
                    break;
                case BlockerResolution.External:
                    Result.ExternalHelp++;
                    break;
                case BlockerResolution.Guessing:
                    Result.Guess++;
                    break;
                case BlockerResolution.Searching:
                    Result.Search++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
            }

            Result.Done++;
            Result.InProgress--;
        }
    }
}