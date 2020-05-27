#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Classes.Agents;

#endregion

namespace Symu.Repository.Networks.Activities
{
    public class AgentActivity
    {
        public AgentActivity(AgentId agentId, string activity)
        {
            AgentId = agentId;
            Activity = activity;
        }

        public AgentId AgentId { get; }
        public string Activity { get; set; }
    }
}