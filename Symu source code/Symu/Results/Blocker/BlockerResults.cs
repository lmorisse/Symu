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
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.Results.Task;

#endregion

namespace Symu.Results.Blocker
{
    /// <summary>
    ///     Manage the task blockers metrics for the symu
    /// </summary>
    public class BlockerResults
    {
        /// <summary>
        ///     If set to true, blockerResults will be filled with value
        /// </summary>
        private readonly bool _followBlockers;

        /// <summary>
        ///     Key => step
        ///     Value => BlockerResult for the step
        /// </summary>
        private readonly ConcurrentDictionary<ushort, BlockerResult> _results =
            new ConcurrentDictionary<ushort, BlockerResult>();

        public BlockerResults(bool followBlockers)
        {
            _followBlockers = followBlockers;
        }

        /// <summary>
        ///     Total blockers done during the symu
        /// </summary>
        public int TotalBlockersDone => _results.Values.Any() ? _results.Values.Sum(x => x.Done) : 0;

        /// <summary>
        ///     Total blockers done during the symu
        /// </summary>
        public int BlockersStillInProgress => _results.Values.Any() ? _results.Values.Last().InProgress : 0;

        /// <summary>
        ///     Total blockers resolved by Internal Help during the symu
        /// </summary>
        public int TotalInternalHelp => _results.Values.Any() ? _results.Values.Sum(x => x.InternalHelp) : 0;

        /// <summary>
        ///     Total blockers resolved by External Help during the symu
        /// </summary>
        public int TotalExternalHelp => _results.Values.Any() ? _results.Values.Sum(x => x.ExternalHelp) : 0;

        /// <summary>
        ///     Total blockers resolved by guessing during the symu
        /// </summary>
        public int TotalGuesses => _results.Values.Any() ? _results.Values.Sum(x => x.Guess) : 0;

        /// <summary>
        ///     Total blockers resolved by searching during the symu
        /// </summary>
        public int TotalSearches => _results.Values.Any() ? _results.Values.Sum(x => x.Search) : 0;

        public void SetResults(SymuEnvironment environment)
        {
            if (!_followBlockers)
            {
                return;
            }

            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var result = new BlockerResult();
            foreach (var blocker in environment.WhitePages.AllAgents().Where(agent => agent.Blockers != null).Select(x => x.Blockers))
            {
                result.InProgress += blocker.Result.InProgress;
                result.Done += blocker.Result.Done;
                result.ExternalHelp += blocker.Result.ExternalHelp;
                result.Guess += blocker.Result.Guess;
                result.InternalHelp += blocker.Result.InternalHelp;
                result.Search += blocker.Result.Search;
            }

            _results.TryAdd(environment.Schedule.Step, result);
        }
    }
}