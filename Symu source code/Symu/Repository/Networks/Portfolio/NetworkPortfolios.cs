#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Symu.Classes.Agents;

#endregion

namespace Symu.Repository.Networks.Portfolio
{
    /// <summary>
    ///     Directory of objects used by the agentIds
    ///     Objects are defined as agent
    /// </summary>
    /// <example>An agent is using a printer</example>
    public class NetworkPortfolios
    {
        /// <summary>
        ///     Key => ObjectIds
        ///     Values => List of NetworkPortfolio which define who is using which object and how
        /// </summary>
        public ConcurrentDictionary<AgentId, List<NetworkPortfolio>> List { get; } =
            new ConcurrentDictionary<AgentId, List<NetworkPortfolio>>();

        public int Count => List.Count;

        public bool Any()
        {
            return List.Any();
        }

        public void Clear()
        {
            List.Clear();
        }

        /// <summary>
        ///     Add a new portfolio to the objectId.
        ///     ObjectId is already existing
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="networkPortfolio"></param>
        private void Add(AgentId objectId, NetworkPortfolio networkPortfolio)
        {
            List[objectId].Add(networkPortfolio);
            // Reallocation
            var objectIds = GetObjectIds(networkPortfolio.AgentId, networkPortfolio.TypeOfUse);
            var totalAllocation =
                objectIds.Sum(oId => GetAllocation(networkPortfolio.AgentId, oId, networkPortfolio.TypeOfUse));

            // There is non main object used at 100%
            // Objects are added as things progress, with the good allocation
            if (!(totalAllocation >= 100))
            {
                return;
            }

            foreach (var portfolio in objectIds.Select(oId =>
                GetNetworkPortfolio(networkPortfolio.AgentId, oId, networkPortfolio.TypeOfUse)))
            {
                //Don't use ComponentAllocation[ca] *= 100/ => return 0
                portfolio.Allocation = Convert.ToSingle(portfolio.Allocation * 100 / totalAllocation);
            }
        }

        public bool Exists(AgentId agentId, AgentId objectId, byte type)
        {
            return ContainsObject(objectId) && List[objectId].Exists(n => n.Equals(agentId, type));
        }

        private void AddPortfolio(AgentId objectId, NetworkPortfolio networkPortfolio)
        {
            AddObject(objectId);
            if (!List[objectId].Contains(networkPortfolio))
            {
                Add(objectId, networkPortfolio);
            }
        }

        public void AddPortfolio(AgentId agentId, AgentId objectId, byte type, float allocation)
        {
            var portfolio = new NetworkPortfolio(agentId, type, allocation);
            AddPortfolio(objectId, portfolio);
        }

        public void AddObject(AgentId objectId)
        {
            if (!ContainsObject(objectId))
            {
                List.TryAdd(objectId, new List<NetworkPortfolio>());
            }
        }

        public void RemoveAgent(AgentId agentId)
        {
            // AgentId can be a component  
            if (ContainsObject(agentId))
            {
                RemoveObject(agentId);
            }
            // Or a kanban/worker

            foreach (var key in List.Keys)
            {
                List[key].RemoveAll(n => n.AgentId.Equals(agentId));
            }

        }

        public void RemoveObject(AgentId objectId)
        {
            List.TryRemove(objectId, out _);
        }

        public float GetAllocation(AgentId agentId, AgentId objectId, byte type)
        {
            if (Exists(agentId, objectId, type))
            {
                return GetNetworkPortfolio(agentId, objectId, type).Allocation;
            }

            return 0;
        }

        public IEnumerable<AgentId> GetByType(AgentId objectId, byte type, byte groupClassKey)
        {
            return ContainsObject(objectId)
                ? List[objectId].FindAll(n => n.IsTypeAndClassKey(type, groupClassKey)).Select(x => x.AgentId)
                : null;
        }

        public bool ContainsObject(AgentId objectId)
        {
            return List.ContainsKey(objectId);
        }

        public bool HasObject(AgentId agentId, byte type)
        {
            return List.Keys.Any(objectId => List[objectId].Exists(n => n.Equals(agentId, type)));
        }

        /// <summary>
        ///     Get the list of all the objects the agentId is using filtered by type of use
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<AgentId> GetObjectIds(AgentId agentId, byte type)
        {
            //var objectIds = new HashSet<AgentId>();
            //foreach (var objectId in List.Keys.Where(iId => List[iId].Exists(n => n.Equals(agentId, type))))
            //{
            //    objectIds.Add(objectId);
            //}

            //return objectIds;
            return List.Keys.Where(iId => List[iId].Exists(n => n != null && n.Equals(agentId, type)));
        }

        /// <summary>
        ///     Get the list of all the objects the agentId is using
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public IEnumerable<AgentId> GetObjectIds(AgentId agentId)
        {
            return List.Keys.Where(oId => List[oId].Exists(n => n != null && n.Equals(agentId)));
        }

        /// <summary>
        ///     Get the list of all the objects the agentId is using
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public IEnumerable<NetworkPortfolio> GetNetworkPortfolios(AgentId agentId, AgentId objectId)
        {
            return List[objectId].FindAll(n => n.Equals(agentId));
        }

        /// <summary>
        ///     Get the NetworkPortfolio used by an agent with a specific type of use
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="objectId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public NetworkPortfolio GetNetworkPortfolio(AgentId agentId, AgentId objectId, byte type)
        {
            return Exists(agentId, objectId, type) ? List[objectId].Find(n => n.Equals(agentId, type)) : null;
        }

        /// <summary>
        ///     agentId is added to groupId (team, kanban, ....)
        ///     agentId inherits all the objects of the groupId
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="groupId"></param>
        public void AddMemberToGroup(AgentId agentId, AgentId groupId)
        {
            foreach (var objectId in GetObjectIds(groupId))
            foreach (var groupPortfolio in GetNetworkPortfolios(groupId, objectId))
            {
                var portfolio = new NetworkPortfolio(agentId, groupPortfolio.TypeOfUse, groupPortfolio.Allocation);
                AddPortfolio(objectId, portfolio);
            }
        }

        public void RemoveMemberFromGroup(AgentId agentId, AgentId groupId)
        {
            foreach (var objectId in GetObjectIds(groupId))
            foreach (var groupPortfolio in GetNetworkPortfolios(groupId, objectId))
            {
                List[objectId].RemoveAll(l => l.Equals(agentId, groupPortfolio.TypeOfUse));
            }
        }

        /// <summary>
        ///     Make a clone of Portfolios from modeling to Symu
        /// </summary>
        /// <param name="portfolios"></param>
        public void CopyTo(NetworkPortfolios portfolios)
        {
            if (portfolios is null)
            {
                throw new ArgumentNullException(nameof(portfolios));
            }

            foreach (var networkPortfolio in List)
            foreach (var portfolio in networkPortfolio.Value)
            {
                portfolios.AddPortfolio(networkPortfolio.Key, portfolio);
            }
        }

        /// <summary>
        ///     Copy the same networks from a group to another
        /// </summary>
        /// <param name="fromGroupId"></param>
        /// <param name="toGroupId"></param>
        public void CopyTo(AgentId fromGroupId, AgentId toGroupId)
        {
            foreach (var networkPortfolio in List)
            foreach (var portfolio in networkPortfolio.Value.FindAll(n => n.AgentId.Equals(fromGroupId)))
            {
                var newPortfolio = new NetworkPortfolio(toGroupId, portfolio.TypeOfUse, portfolio.Allocation);
                AddPortfolio(networkPortfolio.Key, newPortfolio);
            }
        }
    }
}