#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Common.Interfaces;

#endregion

namespace Symu.Repository.Networks.Portfolio
{
    /// <summary>
    ///     Define who is using an object and how
    /// </summary>
    public class NetworkPortfolio
    {
        public NetworkPortfolio(IAgentId agentId, byte type, float allocation)
        {
            AgentId = agentId;
            TypeOfUse = type;
            Allocation = allocation;
        }

        /// <summary>
        ///     The agentId using an object of the portfolio
        /// </summary>
        public IAgentId AgentId { get; }

        /// <summary>
        ///     Define how the AgentId is using the object
        /// </summary>
        public byte TypeOfUse { get; }

        /// <summary>
        ///     Allocation of capacity per component
        ///     capacity allocation ranging from [0; 100]
        /// </summary>
        public float Allocation { get; set; }

        public bool IsType(byte type)
        {
            return TypeOfUse == type;
        }

        public bool IsTypeAndClassId(byte type, IClassId classId)
        {
            return IsType(type) && AgentId.Equals(classId);
        }

        public bool Equals(IAgentId agentId, byte type)
        {
            return IsType(type) && AgentId.Equals(agentId);
        }

        public bool Equals(IAgentId agentId)
        {
            return AgentId.Equals(agentId);
        }

        public override bool Equals(object obj)
        {
            return obj is NetworkPortfolio portfolio &&
                   AgentId.Equals(portfolio.AgentId) &&
                   TypeOfUse == portfolio.TypeOfUse;
        }
    }
}