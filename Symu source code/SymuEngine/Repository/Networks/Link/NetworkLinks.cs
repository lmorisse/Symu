#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using System.Linq;
using SymuEngine.Classes.Agent;

#endregion

namespace SymuEngine.Repository.Networks.Link
{
    /// <summary>
    ///     List of Links of a NetWork
    /// </summary>
    public class NetworkLinks
    {
        public List<NetworkLink> List { get; } = new List<NetworkLink>();

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

        public void RemoveAgent(AgentId agentId)
        {
            List.RemoveAll(l => l.AgentId1.Equals(agentId) || l.AgentId2.Equals(agentId));
        }

        public bool Any()
        {
            return List.Any();
        }

        /// <summary>
        ///     teammateId is managerId subordinate in the context of teamId
        /// </summary>
        /// <param name="teammateId"></param>
        /// <param name="managerId"></param>
        /// <param name="teamId"></param>
        public void AddSubordinate(AgentId teammateId, AgentId managerId, AgentId teamId)
        {
            var link = new CommunicationLink(teammateId, CommunicationType.ReportTo, managerId, teamId);
            AddLink(link);
        }

        public void DeactivateSubordinate(AgentId teammateId, AgentId managerId, AgentId teamId)
        {
            var link = new CommunicationLink(teammateId, CommunicationType.ReportTo, managerId, teamId);
            DeactivateLink(link);
        }

        /// <summary>
        ///     teammate1 and teammate2 are now teammates in the context of the teamId
        ///     Link are bidirectional
        /// </summary>
        public void AddMembers(AgentId teammateId1, AgentId teammateId2, AgentId teamId)
        {
            var link1 = new CommunicationLink(teammateId1, CommunicationType.CommunicateTo, teammateId2, teamId);
            AddLink(link1);
            var link2 = new CommunicationLink(teammateId2, CommunicationType.CommunicateTo, teammateId1, teamId);
            AddLink(link2);
        }

        public void Clear()
        {
            List.Clear();
        }

        /// <summary>
        ///     Use AddMembers
        /// </summary>
        /// <param name="link"></param>
        public void AddLink(CommunicationLink link)
        {
            if (Exists(link))
            {
                Activate(link);
            }
            else
            {
                List.Add(link);
            }
        }

        public bool Exists(AgentId teammateId1, CommunicationType type, AgentId teammateId2, AgentId teamId)
        {
            var link = new CommunicationLink(teammateId1, type, teammateId2, teamId);
            return Exists(link);
        }

        public bool Exists(NetworkLink link)
        {
            return List.Contains(link);
        }

        private NetworkLink Get(NetworkLink link)
        {
            return List.Find(l => l.Equals(link));
        }

        private void Activate(NetworkLink link)
        {
            Get(link).Activate();
        }

        public void DeactivateTeammates(AgentId teammateId1, AgentId teammateId2, AgentId teamId)
        {
            var link1 = new CommunicationLink(teammateId1, CommunicationType.CommunicateTo, teammateId2, teamId);
            DeactivateLink(link1);
            var link2 = new CommunicationLink(teammateId2, CommunicationType.CommunicateTo, teammateId1, teamId);
            DeactivateLink(link2);
        }

        private void DeactivateLink(NetworkLink link)
        {
            if (Exists(link))
            {
                Get(link).Deactivate();
            }
        }

        public bool HasActiveLink(AgentId agentId1, AgentId agentId2)
        {
            return List.Exists(l => l.HasActiveLink(agentId1, agentId2));
        }

        public bool HasPassiveLink(AgentId agentId1, AgentId agentId2)
        {
            return List.Exists(l => l.HasPassiveLink(agentId1, agentId2));
        }

        public IEnumerable<AgentId> GetActiveLinks(AgentId agentId)
        {
            return List.FindAll(l => l.HasActiveLinks(agentId)).Select(l => l.AgentId2).Distinct();
        }

        public IEnumerable<AgentId> GetActiveLinks(AgentId agentId, byte groupClassKey)
        {
            return List.FindAll(l => l.HasActiveLinks(agentId, groupClassKey)).Select(l => l.AgentId2).Distinct();
        }
    }
}