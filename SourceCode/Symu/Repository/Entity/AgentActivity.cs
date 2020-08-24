#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Common.Interfaces;
using Symu.Common.Interfaces.Agent;

#endregion

namespace Symu.Repository.Networks.Activities
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