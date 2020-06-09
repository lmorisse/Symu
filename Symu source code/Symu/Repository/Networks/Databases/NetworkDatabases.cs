#region Licence

// Description: Symu - Symu
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

namespace Symu.Repository.Networks.Databases
{
    /// <summary>
    ///     NetworkDatabases
    ///     All databases in the network
    ///     A database is a system where agent store temporary or permanent information
    /// </summary>
    /// <example>messaging database, IRC channel, wiki pages, comments, ...</example>
    public class NetworkDatabases
    {
        /// <summary>
        ///     Repository of all the Databases used during the simulation
        /// </summary>
        public DatabaseCollection Repository { get; } = new DatabaseCollection();

        /// <summary>
        ///     AgentDataBases.Key = AgentId
        ///     AgentDataBases.Value = List of all the databaseId the agentId has subscribed
        /// </summary>
        public ConcurrentDictionary<AgentId, List<ushort>> AgentDataBases { get; } =
            new ConcurrentDictionary<AgentId, List<ushort>>();

        public bool Any()
        {
            return AgentDataBases.Any();
        }

        public void Clear()
        {
            Repository.Clear();
            AgentDataBases.Clear();
        }

        #region Repository

        public Database GetDatabase(ushort databaseId)
        {
            return Repository.GetDatabase(databaseId);
        }

        /// <summary>
        ///     Add a database to the repository
        /// </summary>
        public void AddDatabase(Database database)
        {
            if (Exists(database))
            {
                return;
            }

            Repository.Add(database);
        }

        public bool Exists(Database database)
        {
            return Repository.Contains(database);
        }

        public bool Exists(ushort databaseId)
        {
            return Repository.Exists(databaseId);
        }

        #endregion

        #region Agent Database

        /// <summary>
        ///     Remove agent from network
        /// </summary>
        /// <param name="agentId"></param>
        public void RemoveAgent(AgentId agentId)
        {
            AgentDataBases.TryRemove(agentId, out _);
        }

        public bool Exists(AgentId agentId)
        {
            return AgentDataBases.ContainsKey(agentId);
        }

        public bool Exists(AgentId agentId, ushort databaseId)
        {
            return Exists(agentId) && AgentDataBases[agentId].Contains(databaseId);
        }

        public void Add(AgentId agentId, Database database)
        {
            if (database is null)
            {
                throw new ArgumentNullException(nameof(database));
            }

            AddDatabase(database);
            AddAgentId(agentId);
            AddDatabase(agentId, database.Entity.AgentId.Key);
        }

        public void Add(AgentId agentId, ushort databaseId)
        {
            AddAgentId(agentId);
            AddDatabase(agentId, databaseId);
        }

        /// <summary>
        ///     Add a Belief to an AgentId
        ///     AgentId is supposed to be already present in the collection.
        ///     if not use Add method
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="databaseId"></param>
        public void AddDatabase(AgentId agentId, ushort databaseId)
        {
            if (!AgentDataBases[agentId].Contains(databaseId))
            {
                AgentDataBases[agentId].Add(databaseId);
            }
        }

        public void AddAgentId(AgentId agentId)
        {
            if (!Exists(agentId))
            {
                AgentDataBases.TryAdd(agentId, new List<ushort>());
            }
        }

        #endregion
    }
}