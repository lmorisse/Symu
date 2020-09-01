#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.DNA.OneModeNetworks.Activity;
using Symu.DNA.TwoModesNetworks.Assignment;

#endregion

namespace Symu.Repository.Entity
{
    public class AgentActivity : IAgentActivity
    {
        public AgentActivity(IAgentId id, IActivity activity)
        {
            Id = id;
            Activity = activity;
        }

        public IAgentId Id { get; }
        public IActivity Activity { get; set; }
    }
}