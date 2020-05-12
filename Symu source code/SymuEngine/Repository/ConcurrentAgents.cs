#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SymuEngine.Classes.Agents;

#endregion

namespace SymuEngine.Repository
{
    /// <summary>
    ///     thread-safe list of agents that can be accessed by multiple threads concurrently
    /// </summary>
    public class ConcurrentAgents<T> where T : Agent
    {
        private readonly ConcurrentDictionary<AgentId, T> _list = new ConcurrentDictionary<AgentId, T>();
        public int Count => _list.Count;

        internal ushort CountByClassKey(byte classKey)
        {
            var count = _list.Values.Count(a => a.Id.ClassKey == classKey);
            return Convert.ToUInt16(count);
        }

        internal void Add(T agent)
        {
            _list[agent.Id] = agent;
        }

        public bool Exists(ushort key, byte classKey)
        {
            var agentId = new AgentId(key, classKey);
            return Exists(agentId);
        }

        public bool Exists(AgentId agentId)
        {
            return _list.ContainsKey(agentId);
        }

        public void Clear()
        {
            _list.Clear();
        }

        /// <summary>
        ///     Get a typed agent by its agentId
        /// </summary>
        /// <typeparam name="TAgent"></typeparam>
        /// <param name="agentId"></param>
        /// <returns>The typed agent</returns>
        public TAgent Get<TAgent>(AgentId agentId) where TAgent : T
        {
            if (Exists(agentId))
            {
                return _list[agentId] as TAgent;
            }

            return null;
        }

        public T Get(AgentId agentId)
        {
            return Exists(agentId) ? _list[agentId] : null;
        }

        /// <summary>
        ///     Returns a list with the names of all the agents.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AgentId> GetKeys()
        {
            return _list.Keys;
        }

        /// <summary>
        ///     Returns a list with the names of all the agents.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetValues()
        {
            return _list.Values;
        }

        /// <summary>
        ///     Returns a list with the names of all the agents that contain a certain string.
        /// </summary>
        /// <returns>The name fragment that the agent names should contain</returns>
        public IEnumerable<T> FilteredByClassKey(byte classKey)
        {
            return GetValues().Where(a => a.Id.ClassKey == classKey);
        }

        /// <summary>
        ///     Returns a list with the names of all the agents that contain a certain string.
        /// </summary>
        /// <returns>The name fragment that the agent names should contain</returns>
        public IEnumerable<AgentId> FilteredKeysByClassKey(byte classKey)
        {
            return _list.Keys.Where(a => a.ClassKey == classKey);
        }

        /// <summary>
        ///     Stops the execution of the agent identified by name and removes it from the environment. Use the Remove method
        ///     instead of Agent.Stop
        ///     when the decision to stop an agent does not belong to the agent itself, but to some other agent or to an external
        ///     factor.
        ///     Don't call it directly, use WhitePages.RemoveAgent
        /// </summary>
        /// <param name="agentId">The name of the agent to be removed</param>
        public void Remove(AgentId agentId)
        {
            if (Exists(agentId))
            {
                var remove = _list.TryRemove(agentId, out _);
                if (!remove)
                {
                    throw new Exception("Concurrent access");
                }
            }
            else
            {
                throw new Exception("Agent " + agentId + " does not exist (ConcurrentEnvironment.Remove)");
            }
        }
    }
}