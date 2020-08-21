#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

using Symu.Common.Interfaces;

namespace Symu.Repository.Networks.Groups
{
    /// <summary>
    /// Interface to define who is member of a group and how
    /// By default how is characterized by an allocation of capacity to define part-time membership
    /// 
    /// </summary>
    public interface IAgentGroup
    {
        IAgentId AgentId { get; }

        /// <summary>
        ///     Range 0 - 100
        /// </summary>
        float Allocation { get; set; }
    }
}