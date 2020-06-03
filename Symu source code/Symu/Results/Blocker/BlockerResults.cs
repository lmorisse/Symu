﻿#region Licence

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
    ///     Manage the task blockers results for the simulation
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
            //foreach (var blockerResult in environment.WhitePages.AllAgents().Where(agent => agent.Blockers != null)
            //    .Select(x => x.Blockers.Result))
            //{
            //    result.InProgress += blockerResult.InProgress;
            //    result.Done += blockerResult.Done;
            //    result.ExternalHelp += blockerResult.ExternalHelp;
            //    result.Guess += blockerResult.Guess;
            //    result.InternalHelp += blockerResult.InternalHelp;
            //    result.Search += blockerResult.Search;
            //    result.Cancelled += blockerResult.Cancelled;
            //}

            //Blockers from tasks
            foreach (var blockerResult in environment.WhitePages.AllAgents().Where(agent => agent.TaskProcessor != null)
                .Select(x => x.TaskProcessor.TasksManager.BlockerResult))
            {
                result.InProgress += blockerResult.InProgress;
                result.Done += blockerResult.Done;
                result.ExternalHelp += blockerResult.ExternalHelp;
                result.Guess += blockerResult.Guess;
                result.InternalHelp += blockerResult.InternalHelp;
                result.Search += blockerResult.Search;
                result.Cancelled += blockerResult.Cancelled;
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