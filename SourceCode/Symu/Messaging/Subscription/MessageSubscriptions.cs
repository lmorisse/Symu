#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using System.Linq;
using Symu.Classes.Agents;
using Symu.Common.Interfaces.Agent;

#endregion

namespace Symu.Messaging.Subscription
{
    /// <summary>
    ///     Manage subscriptions/unsubscriptions to an agent's messages
    ///     The subscription is based on the subject of the message
    ///     If an agent subscribe, the agent will receive all messages sent on the subject
    /// </summary>
    public class MessageSubscriptions
    {
        /// <summary>
        ///     Don't use ConcurrentBag to manage Subscriptions
        /// </summary>
        private readonly List<MessageSubscription> _subscriptions = new List<MessageSubscription>();

        /// <summary>
        ///     Subscribe to the Message content
        /// </summary>
        public void Subscribe(AgentId agentId, byte content)
        {
            if (!HasSubscribed(agentId, content))
            {
                _subscriptions.Add(new MessageSubscription(agentId, content));
            }
        }

        /// <summary>
        ///     UnSubscribe to the Message content
        /// </summary>
        public void Unsubscribe(AgentId agentId, byte content)
        {
            _subscriptions.RemoveAll(s => s.AgentId.Equals(agentId) && s.Content == content);
        }

        /// <summary>
        ///     UnSubscribe to the all Message contents
        /// </summary>
        public void Unsubscribe(AgentId agentId)
        {
            _subscriptions.RemoveAll(s => s.AgentId.Equals(agentId));
        }

        /// <summary>
        ///     Subscribe to the Message content
        /// </summary>
        public bool HasSubscribed(AgentId agentId, byte content)
        {
            return _subscriptions.Exists(s => s.AgentId.Equals(agentId) && s.Content == content);
        }

        public IEnumerable<AgentId> Subscribers(byte content)
        {
            return _subscriptions.Where(s => s.Content == content).Select(s => s.AgentId).ToList();
        }

        /// <summary>
        ///     Even if _subscriptions should be up to date if agent unsubscribe before stop
        ///     It happens that _subscriptions is not well synchronized
        /// </summary>
        /// <param name="content"></param>
        /// <param name="stoppedAgentIds"></param>
        /// <returns></returns>
        public IEnumerable<AgentId> Subscribers(byte content, List<AgentId> stoppedAgentIds)
        {
            _subscriptions.RemoveAll(x => stoppedAgentIds.Exists(stopped => stopped.Equals(x.AgentId)));
            return _subscriptions.Where(s => s.Content == content).Select(s => s.AgentId).ToList();
        }

        public ushort SubscribersCount(byte content)
        {
            return (ushort) _subscriptions.Count(s => s.Content == content);
        }
    }
}