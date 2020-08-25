#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

using Symu.Common.Interfaces.Agent;

namespace Symu.Repository.Networks.Interactions
{
    /// <summary>
    ///     Defines the interaction between two agents used by InteractionNetwork
    ///     link are bidirectional.
    ///     AgentId1 has the smallest key
    ///     AgentId2 has the highest key
    /// </summary>
    /// <remarks>You can define your own definition of a passive/active interaction</remarks>
    public interface IInteraction
    {
        /// <summary>
        ///     Unique key of the agent with the smallest key
        /// </summary>
        IAgentId AgentId1 { get; }

        /// <summary>
        ///     Unique key of the agent with the highest key
        /// </summary>
        IAgentId AgentId2 { get; }
        float Weight { get; }

        bool IsActive {get; }
        bool IsPassive { get; }

        bool HasLink(IAgentId agentId1, IAgentId agentId2);
        bool Equals(object obj);
        bool Equals(IInteraction obj);

        /// <summary>
        /// Increase the weight of the interaction - if interaction are weighted
        /// </summary>
        void IncreaseWeight();
        /// <summary>
        /// Decrease the weight of the interaction - if interaction are weighted
        /// </summary>
        void DecreaseWeight();
        /// <summary>
        /// Agent has active interaction based on the weight of the interaction
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        bool HasActiveInteractions(IAgentId agentId);
        /// <summary>
        /// Agent has active interaction based on the weight of the interaction
        /// </summary>
        /// <param name="agentId1"></param>
        /// <param name="agentId2"></param>
        /// <returns></returns>
        bool HasActiveInteraction(IAgentId agentId1, IAgentId agentId2);
        /// <summary>
        /// Agent has passive interaction based on the weight of the interaction
        /// </summary>
        /// <param name="agentId1"></param>
        /// <param name="agentId2"></param>
        /// <returns></returns>
        bool HasPassiveInteraction(IAgentId agentId1, IAgentId agentId2);
    }
}