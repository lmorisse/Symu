#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Common.Interfaces;

#endregion

namespace Symu.Repository.Networks.Activities
{
    public class AgentActivity
    {
        public AgentActivity(IAgentId agentId, string activity)
        {
            AgentId = agentId;
            Activity = activity;
        }

        public IAgentId AgentId { get; }
        public string Activity { get; set; }
    }
}