#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Common.Interfaces;

#endregion

namespace Symu.Repository.Networks.Link
{
    /// <summary>
    ///     Sphere of interaction - link are bidirectional.
    ///     AgentId1 has the smallest key
    ///     AgentId2 has the highest key
    /// </summary>
    public class NetworkLink
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="agentId1"></param>
        /// <param name="agentId2"></param>
        public NetworkLink(IAgentId agentId1, IAgentId agentId2)
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

            Activate();
        }

        /// <summary>
        ///     Number of links between the two agents
        /// </summary>
        public byte Count { get; private set; }

        /// <summary>
        ///     Unique key of the agent with the smallest key
        /// </summary>
        public IAgentId AgentId1 { get; }

        /// <summary>
        ///     Unique key of the agent with the highest key
        /// </summary>
        public IAgentId AgentId2 { get; }

        public bool IsActive => Count > 0;
        public bool IsPassive => Count == 0;

        public void Activate()
        {
            Count++;
        }

        public void Deactivate()
        {
            if (Count > 0)
            {
                Count--;
            }
        }

        public bool HasActiveLinks(IAgentId agentId)
        {
            return IsActive && (AgentId1.Equals(agentId) || AgentId2.Equals(agentId));
        }

        public bool HasActiveLink(IAgentId agentId1, IAgentId agentId2)
        {
            return IsActive && HasLink(agentId1, agentId2);
        }

        public bool HasPassiveLink(IAgentId agentId1, IAgentId agentId2)
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
            return obj is NetworkLink link &&
                   link.HasLink(AgentId1, AgentId2);
        }
    }
}