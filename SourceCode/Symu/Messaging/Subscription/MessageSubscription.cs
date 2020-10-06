#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Classes.Agents;
using Symu.Common.Interfaces;

#endregion

namespace Symu.Messaging.Subscription
{
    /// <summary>
    ///     Manage the subscription by agent to a message content
    ///     ///
    /// </summary>
    public struct MessageSubscription
    {
        public MessageSubscription(IAgentId agentId, byte content)
        {
            Content = content;
            AgentId = agentId;
        }

        /// <summary>
        ///     The content the agent is subscribing for
        /// </summary>
        public byte Content { get; }

        /// <summary>
        ///     The name of the agent
        /// </summary>
        public IAgentId AgentId { get; set; }
    }
}