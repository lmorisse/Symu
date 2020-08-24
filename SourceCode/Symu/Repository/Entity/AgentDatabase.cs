#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Common.Interfaces;
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.Repository.Networks.Resources;

#endregion

namespace Symu.Repository.Entity
{
    /// <summary>
    ///     Define who is using a database and how
    /// </summary>
    public class AgentDatabase : AgentResource
    {
        public AgentDatabase(IId resourceId, IResourceUsage resourceUsage, float resourceAllocation): base(resourceId, resourceUsage, resourceAllocation)
        {
        }
    }
}