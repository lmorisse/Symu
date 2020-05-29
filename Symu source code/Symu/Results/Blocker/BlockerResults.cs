#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Concurrent;
using System.Linq;
using Symu.Environment;

#endregion

namespace Symu.Results.Blocker
{
    /// <summary>
    ///     Manage the task blockers metrics for the symu
    /// </summary>
    public class BlockerResults
    {
        /// <summary>
        ///     Key => step
        ///     Value => BlockerResult for the step
        /// </summary>
        public ConcurrentDictionary<ushort, BlockerResult> Results { get; private set; } =
            new ConcurrentDictionary<ushort, BlockerResult>();

        /// <summary>
        ///     If set to true, blockerResults will be filled with value and stored during the simulation
        /// </summary>
        public bool On { get; set; }

        /// <summary>
        ///     Total blockers done during the simulation
        /// </summary>
        public int TotalBlockersDone => Results.Values.Any() ? Results.Values.Last().Done : 0;

        /// <summary>
        ///     Total blockers done during the simulation
        /// </summary>
        public int BlockersStillInProgress => Results.Values.Any() ? Results.Values.Last().InProgress : 0;

        /// <summary>
        ///     Total blockers cancelled the task during the simulation
        /// </summary>
        public int TotalCancelled => Results.Values.Any() ? Results.Values.Last().Cancelled : 0;

        /// <summary>
        ///     Total blockers resolved by Internal Help during the simulation
        /// </summary>
        public int TotalInternalHelp => Results.Values.Any() ? Results.Values.Last().InternalHelp : 0;

        /// <summary>
        ///     Total blockers resolved by External Help during the simulation
        /// </summary>
        public int TotalExternalHelp => Results.Values.Any() ? Results.Values.Last().ExternalHelp : 0;

        /// <summary>
        ///     Total blockers resolved by guessing during the simulation
        /// </summary>
        public int TotalGuesses => Results.Values.Any() ? Results.Values.Last().Guess : 0;

        /// <summary>
        ///     Total blockers cancelled by the agent during the simulation
        /// </summary>
        public int TotalSearches => Results.Values.Any() ? Results.Values.Last().Search : 0;

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
            foreach (var agentResult in environment.WhitePages.AllAgents().Where(agent => agent.Blockers != null)
                .Select(x => x.Blockers.Result))
            {
                result.InProgress += agentResult.InProgress;
                result.Done += agentResult.Done;
                result.ExternalHelp += agentResult.ExternalHelp;
                result.Guess += agentResult.Guess;
                result.InternalHelp += agentResult.InternalHelp;
                result.Search += agentResult.Search;
            }

            //Blockers from tasks
            foreach (var tasks in environment.WhitePages.AllAgents().Where(agent => agent.TaskProcessor != null)
                .Select(x => x.TaskProcessor.TasksManager.AllTasks))
            {
                result.InProgress += tasks.Sum(x => x.Blockers.Result.InProgress);
                result.Done += tasks.Sum(x => x.Blockers.Result.Done);
                result.ExternalHelp += tasks.Sum(x => x.Blockers.Result.ExternalHelp);
                result.Guess += tasks.Sum(x => x.Blockers.Result.Guess);
                result.InternalHelp += tasks.Sum(x => x.Blockers.Result.InternalHelp);
                result.Search += tasks.Sum(x => x.Blockers.Result.Search);
                result.Cancelled += tasks.Sum(x => x.Blockers.Result.Cancelled);
            }

            Results.TryAdd(environment.Schedule.Step, result);
        }

        public void Clear()
        {
            Results.Clear();
        }

        public void CopyTo(BlockerResults cloneBlockers)
        {
            if (cloneBlockers == null)
            {
                throw new ArgumentNullException(nameof(cloneBlockers));
            }

            cloneBlockers.Results = new ConcurrentDictionary<ushort, BlockerResult>();
            foreach (var result in Results)
            {
                cloneBlockers.Results.TryAdd(result.Key, result.Value);
            }
        }
    }
}