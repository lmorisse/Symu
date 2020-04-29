#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using SymuEngine.Classes.Agent;

#endregion

namespace SymuEngine.Repository.Networks.Group
{
    public class GroupAllocation
    {
        public GroupAllocation(AgentId agentId, float allocation)
        {
            AgentId = agentId;
            Allocation = allocation;
        }

        public AgentId AgentId { get; }
        public float Allocation { get; set; }
    }
}