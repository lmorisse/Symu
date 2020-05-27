#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using SymuTools.Math.ProbabilityDistributions;

#endregion

namespace Symu.Classes.Murphies
{
    /// <summary>
    ///     Tasks on which worker is working may be incomplete
    ///     If so, task is blocked and worker is asking help to the team first, and then to his personal network
    /// </summary>
    public class MurphyIncompleteInformation : MurphyIncomplete
    {
        /// <summary>
        ///     Return the new Type of missing information of the task
        /// </summary>
        /// <returns>
        ///     The type of missing information
        ///     None if there is no missing information
        /// </returns>
        public bool CheckInformation()
        {
            return Bernoulli.Sample(ThresholdForReacting);
        }
    }
}