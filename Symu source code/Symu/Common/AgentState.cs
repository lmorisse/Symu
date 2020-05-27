#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace Symu.Common
{
    /// <summary>
    ///     State of the agent
    ///     It is the global life cycle of the agent during a symu
    /// </summary>
    public enum AgentState
    {
        /// <summary>
        ///     Agent is created but not started
        /// </summary>
        NotStarted,

        /// <summary>
        ///     Start is launched
        ///     BeforeStart is not yet passed
        /// </summary>
        Starting,
        Started,

        /// <summary>
        ///     Started but in pause
        /// </summary>
        Paused,

        /// <summary>
        ///     During a step, an agent can be marked Stopping
        ///     The agent will be stopped cleanly at the end of the step
        ///     The step is not finished, so that agent is still alive
        /// </summary>
        Stopping,

        /// <summary>
        ///     Agent is stopped
        /// </summary>
        Stopped
    }
}