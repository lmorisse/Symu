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
using Symu.Common.Interfaces;

#endregion

namespace Symu.Repository.Networks.Link
{
    /// <summary>
    ///     List of Links of a NetWork
    /// </summary>
    public class NetworkLinks
    {
        private uint _maxLinksCount;
        public List<NetworkLink> List { get; } = new List<NetworkLink>();
        public int Count => List.Count;

        /// <summary>
        ///     Gets or sets the element at the specified index
        /// </summary>
        /// <param name="index">0 based</param>
        /// <returns></returns>
        public NetworkLink this[int index]
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
        public void AddLinks(List<IAgentId> members)
        {
            if (members == null)
            {
                throw new ArgumentNullException(nameof(members));
            }

            var count = members.Count;
            for (var i = 0; i < count; i++)
            {
                var agentId1 = members[i];
                for (var j = i + 1; j < count; j++)
                {
                    var agentId2 = members[j];
                    AddLink(agentId1, agentId2);
                }
            }
        }

        public void Clear()
        {
            List.Clear();
        }

        /// <summary>
        ///     Use AddLink
        ///     Link is bidirectional
        /// </summary>
        /// <param name="agentId1"></param>
        /// <param name="agentId2"></param>
        public void AddLink(IAgentId agentId1, IAgentId agentId2)
        {
            if (agentId1.Equals(agentId2))
            {
                return;
            }

            if (Exists(agentId1, agentId2))
            {
                Activate(agentId1, agentId2);
            }
            else
            {
                List.Add(new NetworkLink(agentId1, agentId2));
            }
        }

        public bool Exists(NetworkLink link)
        {
            return List.Contains(link);
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

        private NetworkLink Get(IAgentId agentId1, IAgentId agentId2)
        {
            return List.Find(x => x.HasLink(agentId1, agentId2));
        }

        private void Activate(IAgentId agentId1, IAgentId agentId2)
        {
            Get(agentId1, agentId2).Activate();
        }

        public void DeactivateLink(IAgentId agentId1, IAgentId agentId2)
        {
            if (Exists(agentId1, agentId2))
            {
                Get(agentId1, agentId2).Deactivate();
            }
        }

        public bool HasActiveLink(IAgentId agentId1, IAgentId agentId2)
        {
            return List.Exists(l => l.HasActiveLink(agentId1, agentId2));
        }

        public float CountLinks(IAgentId agentId1, IAgentId agentId2)
        {
            return Exists(agentId1, agentId2) ? Get(agentId1, agentId2).Count : 0;
        }

        public float NormalizedCountLinks(IAgentId agentId1, IAgentId agentId2)
        {
            return _maxLinksCount == 0 ? 0 : CountLinks(agentId1, agentId2) / _maxLinksCount;
        }

        public void SetMaxLinksCount()
        {
            _maxLinksCount = List.Any() ? List.Max(x => x.Count) : (uint) 0;
        }

        #region unit tests

        public bool HasPassiveLink(IAgentId agentId1, IAgentId agentId2)
        {
            return List.Exists(l => l.HasPassiveLink(agentId1, agentId2));
        }

        /// <summary>
        ///     Get all the active links of an agent
        /// </summary>
        public IEnumerable<IAgentId> GetActiveLinks(IAgentId agentId)
        {
            return List.FindAll(l => l.HasActiveLinks(agentId)).Select(l => l.AgentId2).Distinct();
        }

        #endregion
    }
}