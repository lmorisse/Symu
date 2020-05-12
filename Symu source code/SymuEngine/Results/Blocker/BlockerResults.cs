#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Concurrent;
using System.Linq;

#endregion

namespace SymuEngine.Results.Blocker
{
    /// <summary>
    ///     Manage the task blockers metrics for the simulation
    /// </summary>
    public class BlockerResults
    {
        /// <summary>
        ///     Key => step
        ///     Value => BlockerResult for the step
        /// </summary>
        private readonly ConcurrentDictionary<ushort, BlockerResult> _results =
            new ConcurrentDictionary<ushort, BlockerResult>();

        /// <summary>
        ///     Total blockers done during the simulation
        /// </summary>
        public int TotalBlockersDone => _results.Values.Any() ? _results.Values.Sum(x => x.Done) : 0;

        /// <summary>
        ///     Total blockers done during the simulation
        /// </summary>
        public int BlockersStillInProgress => _results.Values.Any() ? _results.Values.Last().InProgress : 0;

        /// <summary>
        ///     Total blockers resolved by Internal Help during the simulation
        /// </summary>
        public int TotalInternalHelp => _results.Values.Any() ? _results.Values.Sum(x => x.InternalHelp) : 0;

        /// <summary>
        ///     Total blockers resolved by External Help during the simulation
        /// </summary>
        public int TotalExternalHelp => _results.Values.Any() ? _results.Values.Sum(x => x.ExternalHelp) : 0;

        /// <summary>
        ///     Total blockers resolved by guessing during the simulation
        /// </summary>
        public int TotalGuesses => _results.Values.Any() ? _results.Values.Sum(x => x.Guess) : 0;

        /// <summary>
        ///     Total blockers resolved by searching during the simulation
        /// </summary>
        public int TotalSearches => _results.Values.Any() ? _results.Values.Sum(x => x.Search) : 0;

        public void AddBlockerInProgress(ushort step)
        {
            SetStep(step);
            _results[step].InProgress++;
        }

        public void BlockerDone(BlockerResolution resolution, ushort step)
        {
            SetStep(step);
            switch (resolution)
            {
                case BlockerResolution.Internal:
                    _results[step].InternalHelp++;
                    break;
                case BlockerResolution.External:
                    _results[step].ExternalHelp++;
                    break;
                case BlockerResolution.Guessing:
                    _results[step].Guess++;
                    break;
                case BlockerResolution.Searching:
                    _results[step].Search++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
            }

            _results[step].Done++;
            _results[step].InProgress--;
        }

        private void SetStep(ushort step)
        {
            if (!_results.ContainsKey(step))
            {
                _results.TryAdd(step, new BlockerResult());
            }
        }
    }
}