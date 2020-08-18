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

namespace Symu.Repository.Networks.Influences
{
    /// <summary>
    ///     NetWork of agents influences
    /// </summary>
    public class NetworkInfluences
    {
        public List<Influence> List { get; } = new List<Influence>();

        public void RemoveAgent(IAgentId agentId)
        {
            List.RemoveAll(l => l.Equals(agentId));
        }

        public bool Any()
        {
            return List.Any();
        }

        public void Clear()
        {
            List.Clear();
        }

        /// <summary>
        ///     Initialize the agent influence and add it to the network
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="influenceability"></param>
        /// <param name="influentialness"></param>
        public void Add(IAgentId agentId, float influenceability, float influentialness)
        {
            if (Exists(agentId))
            {
                return;
            }

            var influence = new Influence(agentId, influenceability, influentialness);
            lock (List)
            {
                List.Add(influence);
            }
        }

        /// <summary>
        ///     Update the agent influence if exists in the network
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="influentialness"></param>
        /// <param name="influenceability"></param>
        public void Update(IAgentId agentId, float influentialness, float influenceability)
        {
            if (!Exists(agentId))
            {
                return;
            }

            var influence = GetInfluence(agentId);
            influence.Influenceability = influenceability;
            influence.Influentialness = influentialness;
        }

        /// <summary>
        ///     Verify that the influence of an agent exists in the network
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bool Exists(IAgentId agentId)
        {
            return List.Exists(l => l.Equals(agentId));
        }

        /// <summary>
        ///     Return Influentialness of an agentId if exists
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns>Influentialness of an agentId if exists, 0 if not</returns>
        public float GetInfluentialness(IAgentId agentId)
        {
            if (Exists(agentId))
            {
                return GetInfluence(agentId).Influentialness;
            }

            return 0;
        }

        /// <summary>
        ///     Return Influenceability of an agentId if exists
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns>Influenceability of an agentId if exists, 0 if not</returns>
        public float GetInfluenceability(IAgentId agentId)
        {
            if (Exists(agentId))
            {
                return GetInfluence(agentId).Influenceability;
            }

            return 0;
        }

        /// <summary>
        ///     Return Influence of an agentId if exists
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns>influence of an agentId if exists, null if not</returns>
        public Influence GetInfluence(IAgentId agentId)
        {
            return Exists(agentId) ? List.Find(l => l.Equals(agentId)) : null;
        }
    }
}