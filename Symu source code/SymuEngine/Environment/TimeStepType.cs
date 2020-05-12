#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace SymuEngine.Environment
{
    public enum TimeStepType
    {
        /// <summary>
        ///     WorkOnTask is splitted so that an agent has the opportunity to select another task with customized
        ///     PrioritizeNextTask
        /// </summary>
        Intraday = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Yearly = 4
    }
}