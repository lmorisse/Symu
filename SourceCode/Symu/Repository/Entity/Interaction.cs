#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Common.Interfaces.Agent;
using Symu.Repository.Networks.Interactions;
using static Symu.Common.Constants;

#endregion

namespace Symu.Repository.Entity
{
    /// <summary>
    ///Default implementation of IInteraction
    /// Defines the interaction between two agents used by InteractionNetwork
    ///     link are bidirectional.
    ///     AgentId1 has the smallest key
    ///     AgentId2 has the highest key
    /// </summary>
    public class Interaction : IInteraction
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="agentId1"></param>
        /// <param name="agentId2"></param>
        public Interaction(IAgentId agentId1, IAgentId agentId2)
        {
            if (agentId1 == null)
            {
                throw new ArgumentNullException(nameof(agentId1));
            }

            if (agentId1.CompareTo(agentId2))
            {
                AgentId1 = agentId1;
                AgentId2 = agentId2;
            }
            else
            {
                AgentId1 = agentId2;
                AgentId2 = agentId1;
            }

            IncreaseWeight();
        }

        public Interaction(IAgentId agentId1, IAgentId agentId2, float weight) : this(agentId1, agentId2)
        {
            Weight = weight;
        }

        /// <summary>
        ///     Number of interactions between the two agents
        /// </summary>
        public float Weight { get; private set; }

        /// <summary>
        ///     Unique key of the agent with the smallest key
        /// </summary>
        public IAgentId AgentId1 { get; }

        /// <summary>
        ///     Unique key of the agent with the highest key
        /// </summary>
        public IAgentId AgentId2 { get; }

        public bool IsActive => Weight > 0;
        public bool IsPassive => Weight < Tolerance;

        /// <summary>
        /// Increase the weight of the interaction
        /// </summary>
        public void IncreaseWeight()
        {
            Weight++;
        }

        /// <summary>
        /// Decrease the weight of the interaction
        /// </summary>
        public void DecreaseWeight()
        {
            if (Weight > 0)
            {
                Weight--;
            }
        }
        /// <summary>
        /// Agent has active interaction based on the weight of the interaction
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bool HasActiveInteractions(IAgentId agentId)
        {
            return IsActive && (AgentId1.Equals(agentId) || AgentId2.Equals(agentId));
        }

        /// <summary>
        /// Agent has active interaction based on the weight of the interaction
        /// </summary>
        /// <param name="agentId1"></param>
        /// <param name="agentId2"></param>
        /// <returns></returns>
        public bool HasActiveInteraction(IAgentId agentId1, IAgentId agentId2)
        {
            return IsActive && HasLink(agentId1, agentId2);
        }
        /// <summary>
        /// Agent has passive interaction based on the weight of the interaction
        /// </summary>
        /// <param name="agentId1"></param>
        /// <param name="agentId2"></param>
        /// <returns></returns>
        public bool HasPassiveInteraction(IAgentId agentId1, IAgentId agentId2)
        {
            return IsPassive && HasLink(agentId1, agentId2);
        }

        public bool HasLink(IAgentId agentId1, IAgentId agentId2)
        {
            if (agentId1 == null)
            {
                throw new ArgumentNullException(nameof(agentId1));
            }

            if (agentId1.CompareTo(agentId2))
            {
                return AgentId1.Equals(agentId1) && AgentId2.Equals(agentId2);
            }

            return AgentId1.Equals(agentId2) && AgentId2.Equals(agentId1);
        }

        public override bool Equals(object obj)
        {
            return obj is Interaction link &&
                   link.HasLink(AgentId1, AgentId2);
        }

        public bool Equals(IInteraction obj)
        {
            return obj is Interaction link &&
                   link.HasLink(AgentId1, AgentId2);
        }
    }
}