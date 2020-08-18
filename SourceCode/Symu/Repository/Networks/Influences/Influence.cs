#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Common.Interfaces;

#endregion

namespace Symu.Repository.Networks.Influences
{
    /// <summary>
    ///     Influence specify influenceability(how susceptible an agent will be to the influentialness of another agent)
    ///     and  influentialness (how influential an agent will be)
    /// </summary>
    public class Influence
    {
        public Influence(IAgentId agentId, float influenceability, float influentialness)
        {
            AgentId = agentId;
            Influenceability = influenceability;
            Influentialness = influentialness;
        }

        /// <summary>
        ///     Unique key of the agent
        /// </summary>
        public IAgentId AgentId { get; }

        /// <summary>
        ///     how susceptible an agent will be to the influentialness of another agent
        /// </summary>
        public float Influenceability { get; set; }

        /// <summary>
        ///     how influential an agent will be
        /// </summary>
        public float Influentialness { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Influence influence &&
                   AgentId.Equals(influence.AgentId);
        }

        public bool Equals(IAgentId agentId)
        {
            return AgentId.Equals(agentId);
        }
    }
}