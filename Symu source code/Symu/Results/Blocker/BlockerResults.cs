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
        ///     If set to true, blockerResults will be filled with value and stored during the simulation
        /// </summary>
        public bool On { get; set; }

        /// <summary>
        ///     Key => step
        ///     Value => BlockerResult for the step
        /// </summary>
        private readonly ConcurrentDictionary<ushort, BlockerResult> _results =
            new ConcurrentDictionary<ushort, BlockerResult>();

        /// <summary>
        ///     Total blockers done during the simulation
        /// </summary>
        public int TotalBlockersDone => _results.Values.Any() ? _results.Values.Last().Done : 0;

        /// <summary>
        ///     Total blockers done during the simulation
        /// </summary>
        public int BlockersStillInProgress => _results.Values.Any() ? _results.Values.Last().InProgress : 0;

        /// <summary>
        ///     Total blockers cancelled the task during the simulation
        /// </summary>
        public int TotalCancelled => _results.Values.Any() ? _results.Values.Last().Cancelled: 0;

        /// <summary>
        ///     Total blockers resolved by Internal Help during the simulation
        /// </summary>
        public int TotalInternalHelp => _results.Values.Any() ? _results.Values.Last().InternalHelp : 0;

        /// <summary>
        ///     Total blockers resolved by External Help during the simulation
        /// </summary>
        public int TotalExternalHelp => _results.Values.Any() ? _results.Values.Last().ExternalHelp : 0;

        /// <summary>
        ///     Total blockers resolved by guessing during the simulation
        /// </summary>
        public int TotalGuesses => _results.Values.Any() ? _results.Values.Last().Guess : 0;

        /// <summary>
        ///     Total blockers cancelled by the agent during the simulation
        /// </summary>
        public int TotalSearches => _results.Values.Any() ? _results.Values.Last().Search : 0;

        public void SetResults(SymuEnvironment environment)
        {
            if (!On)
            {
                return;
            }

            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var result = new BlockerResult();
            // Blockers from agents
            foreach (var agentResult in environment.WhitePages.AllAgents().Where(agent => agent.Blockers != null).Select(x => x.Blockers.Result))
            {
                result.InProgress += agentResult.InProgress;
                result.Done += agentResult.Done;
                result.ExternalHelp += agentResult.ExternalHelp;
                result.Guess += agentResult.Guess;
                result.InternalHelp += agentResult.InternalHelp;
                result.Search += agentResult.Search;
            }
            //Blockers from tasks
            foreach (var tasks in environment.WhitePages.AllAgents().Where(agent => agent.TaskProcessor != null).Select(x => x.TaskProcessor.TasksManager.AllTasks))
            {
                result.InProgress += tasks.Sum(x => x.Blockers.Result.InProgress);
                result.Done += tasks.Sum(x => x.Blockers.Result.Done);
                result.ExternalHelp += tasks.Sum(x => x.Blockers.Result.ExternalHelp);
                result.Guess += tasks.Sum(x => x.Blockers.Result.Guess);
                result.InternalHelp += tasks.Sum(x => x.Blockers.Result.InternalHelp);
                result.Search += tasks.Sum(x => x.Blockers.Result.Search);
                result.Cancelled += tasks.Sum(x => x.Blockers.Result.Cancelled);
            }
            _results.TryAdd(environment.Schedule.Step, result);
        }

        public void Clear()
        {
            _results.Clear();
        }
    }
}