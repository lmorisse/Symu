#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Common.Interfaces;
using Symu.Repository.Networks.Resources;

#endregion

namespace Symu.Repository.Entity
{
    /// <summary>
    ///     Define who is using a component or a product and how
    /// </summary>
    public class AgentPortfolio : AgentResource
    {
        public AgentPortfolio(IAgentId agentId, ResourceUsage resourceUsage, float resourceAllocation): base(agentId, resourceUsage, resourceAllocation)
        {
        }
    }
}