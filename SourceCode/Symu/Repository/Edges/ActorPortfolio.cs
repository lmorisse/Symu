#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Common.Interfaces;
using Symu.OrgMod.Edges;
using Symu.OrgMod.Entities;

#endregion

namespace Symu.Repository.Edges
{
    /// <summary>
    ///     Define who is using a component or a product and how
    /// </summary>
    public class ActorPortfolio : ActorResource
    {
        public ActorPortfolio(IAgentId actorId, IAgentId resourceId, IResourceUsage resourceUsage, float weight): base(actorId, resourceId, resourceUsage, weight)
        {
        }
    }
}