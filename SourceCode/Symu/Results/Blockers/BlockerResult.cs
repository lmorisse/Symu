#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace Symu.Results.Blockers
{
    public class BlockerResult
    {
        /// <summary>
        ///     Number of blockers In Progress
        /// </summary>
        public int InProgress { get; set; }

        /// <summary>
        ///     Number of blockers AverageDone
        /// </summary>
        public int Done { get; set; }

        /// <summary>
        ///     Blocker is resolved by asking help to internal agents
        /// </summary>
        public int InternalHelp { get; set; }

        /// <summary>
        ///     Blocker is resolved by searching or asking help to external agents
        /// </summary>
        public int ExternalHelp { get; set; }

        /// <summary>
        ///     Blocker is resolved by guessing the answer
        /// </summary>
        public int Guess { get; set; }

        /// <summary>
        ///     Blocker is resolved by searching in agent's databases
        /// </summary>
        public int Search { get; set; }

        /// <summary>
        ///     Blocker is resolved by cancelling the task
        /// </summary>
        public int Cancelled { get; set; }
    }
}