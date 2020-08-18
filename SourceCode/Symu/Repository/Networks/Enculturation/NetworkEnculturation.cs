#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using System.Linq;
using Symu.Common.Interfaces;

#endregion

namespace Symu.Repository.Networks.Enculturation
{
    /// <summary>
    ///     Dictionary of all enculturation information of the network
    ///     for every AgentId, the value of the enculturation level
    ///     Key => AgentId
    ///     Value => Enculturation value
    /// </summary>
    public class NetworkEnculturation
    {
        /// <summary>
        ///     List of all agentId and their enculturation information
        /// </summary>
        public Dictionary<IAgentId, float> EnculturationCollection { get; } = new Dictionary<IAgentId, float>();

        public bool Any()
        {
            return EnculturationCollection.Any();
        }

        public void Clear()
        {
            EnculturationCollection.Clear();
        }

        /// <summary>
        ///     Remove agent from network,
        ///     either it is a kanban or an agent
        /// </summary>
        /// <param name="agentId"></param>
        public void RemoveAgent(IAgentId agentId)
        {
            EnculturationCollection.Remove(agentId);
        }

        /// <summary>
        ///     Check that agentId exist
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bool Exists(IAgentId agentId)
        {
            return EnculturationCollection.ContainsKey(agentId);
        }

        /// <summary>
        ///     Add AgentId and initialize the dictionaries
        ///     To update the enculturation information of the agentId, use UpdateEnculturation
        ///     which call AddAgentId
        /// </summary>
        /// <param name="agentId"></param>
        public void AddAgentId(IAgentId agentId)
        {
            if (!Exists(agentId))
            {
                EnculturationCollection.Add(agentId, 0);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="enculturationLevel"></param>
        /// <param name="agentId"></param>
        public void UpdateEnculturation(IAgentId agentId, float enculturationLevel)
        {
            AddAgentId(agentId);
            EnculturationCollection[agentId] = enculturationLevel;
        }

        public float GetEnculturation(IAgentId agentId)
        {
            if (Exists(agentId))
            {
                return EnculturationCollection[agentId];
            }

            return 0;
        }
    }
}