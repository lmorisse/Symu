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
    ///     Define who is using an object and how
    /// </summary>
    public class Portfolio : IAgentResource
    {
        public Portfolio(IAgentId agentId, byte typeOfUse, float allocation): base(agentId, typeOfUse, allocation)
        {
        }
    }
}