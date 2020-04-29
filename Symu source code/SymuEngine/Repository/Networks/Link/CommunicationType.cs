#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace SymuEngine.Repository.Networks.Link
{
    public enum CommunicationType
    {
        /// <summary>
        ///     Teammate report to its manager
        /// </summary>
        ReportTo,

        /// <summary>
        ///     Teammate communicate to its teammates, its PO, its users, ...
        /// </summary>
        CommunicateTo
    }
}