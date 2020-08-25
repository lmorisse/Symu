#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Symu.Common.Interfaces.Agent;
using Symu.Repository.Entity;
using static Symu.Common.Constants;
#endregion

namespace Symu.Repository.Networks.Interactions
{
    /// <summary>
    ///     Network of all the interactions between agent
    ///     Agent x Agent Network
    ///     Who knows who
    /// </summary>
    public class InteractionNetwork
    {
        private float _maxWeight;
        public List<IInteraction> List { get; } = new List<IInteraction>();
        public int Count => List.Count;

        /// <summary>
        ///     Gets or sets the element at the specified index
        /// </summary>
        /// <param name="index">0 based</param>
        /// <returns></returns>
        public IInteraction this[int index]
        {
            get => List[index];
            set => List[index] = value;
        }

        public void RemoveAgent(IAgentId agentId)
        {
            List.RemoveAll(l => l.AgentId1.Equals(agentId) || l.AgentId2.Equals(agentId));
        }

        public bool Any()
        {
            return List.Any();
        }

        /// <summary>
        ///     Reinitialize links between members of a group :
        ///     Add a bi directional link between every member of a group
        /// </summary>
        public void AddInteractions(List<IInteraction> interactions)
        {
            if (interactions == null)
            {
                throw new ArgumentNullException(nameof(interactions));
            }

            foreach (var interaction in interactions)
            {
                AddInteraction(interaction);
            }
        }


        public void Clear()
        {
            List.Clear();
        }

        /// <summary>
        ///     Add interaction.
        /// If interaction already exist, it calls IncreaseInteraction
        /// </summary>
        /// <param name="interaction"></param>
        public void AddInteraction(IInteraction interaction)
        {
            if (interaction == null)
            {
                throw new ArgumentNullException(nameof(interaction));
            }

            if (interaction.AgentId2.Equals(interaction.AgentId1))
            {
                return;
            }

            if (Exists(interaction))
            {
                IncreaseInteraction(interaction);
            }
            else  
            {
                List.Add(interaction);
            }
        }

        public bool Exists(IInteraction interaction)
        {
            return List.Contains(interaction);
        }

        /// <summary>
        ///     Link exists between agentId1 and agentId2 in the context of the groupId
        /// </summary>
        /// <param name="agentId1"></param>
        /// <param name="agentId2"></param>
        public bool Exists(IAgentId agentId1, IAgentId agentId2)
        {
            return List.Exists(x => x.HasLink(agentId1, agentId2));
        }

        private IInteraction Get(IAgentId agentId1, IAgentId agentId2)
        {
            return List.Find(x => x.HasLink(agentId1, agentId2));
        }

        /// <summary>
        /// Increase the weight of the interaction if the interaction is weighted
        /// </summary>
        private void IncreaseInteraction(IInteraction interaction)
        {
            // As interaction can be a new instance of IInteraction, it may be not the one that is stored in the network
            var interactionFromNetwork = Get(interaction.AgentId1, interaction.AgentId2);
            interactionFromNetwork.IncreaseWeight();
        }

        /// <summary>
        /// Decrease the weight of the interaction if the interaction is weighted
        /// </summary>
        public void DecreaseInteraction(IAgentId agentId1, IAgentId agentId2)
        {
            if (Exists(agentId1, agentId2))
            {
                Get(agentId1, agentId2).DecreaseWeight();
            }
        }

        public bool HasActiveInteraction(IAgentId agentId1, IAgentId agentId2)
        {
            return List.Exists(l => l.HasActiveInteraction(agentId1, agentId2));
        }

        public float GetInteractionWeight(IAgentId agentId1, IAgentId agentId2)
        {
            return Exists(agentId1, agentId2) ? Get(agentId1, agentId2).Weight : 0;
        }

        public float NormalizedCountLinks(IAgentId agentId1, IAgentId agentId2)
        {
            return _maxWeight < Tolerance ? 0 : GetInteractionWeight(agentId1, agentId2) / _maxWeight;
        }

        public void SetMaxLinksCount()
        {
            _maxWeight = List.Any() ? List.Max(x => x.Weight) : 0;
        }

        #region unit tests

        public bool HasPassiveInteraction(IAgentId agentId1, IAgentId agentId2)
        {
            return List.Exists(l => l.HasPassiveInteraction(agentId1, agentId2));
        }

        /// <summary>
        ///     Get all the active links of an agent
        /// </summary>
        public IEnumerable<IAgentId> GetActiveInteractions(IAgentId agentId)
        {
            return List.FindAll(l => l.HasActiveInteractions(agentId)).Select(l => l.AgentId2).Distinct();
        }

        #endregion
    }
}