#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Concurrent;
using System.Linq;
using SymuEngine.Classes.Agents;

#endregion

namespace SymuEngine.Environment
{
    /// <summary>
    ///     Manage the state of the Environment
    ///     The environment is started when All its agents are started
    /// </summary>
    public class EnvironmentState
    {
        private ushort _number;
        public ConcurrentBag<AgentId> AgentsStarting { get; private set; } = new ConcurrentBag<AgentId>();
        public bool Debug { get; set; } = true;
        public bool Started => _number == 0;

        /// <summary>
        ///     Enqueue a starting agent
        /// </summary>
        /// <param name="agentId"></param>
        public void EnqueueStartingAgent(AgentId agentId)
        {
            if (Debug)
            {
                if (!AgentsStarting.Contains(agentId))
                {
                    AgentsStarting.Add(agentId);
                }
                else
                {
                    throw new ArgumentException(nameof(agentId));
                }
            }

            _number++;
        }

        /// <summary>
        ///     Dequeue a started agent
        /// </summary>
        public void DequeueStartedAgent()
        {
            if (Debug)
            {
                AgentsStarting.TryTake(out _);
            }

            if (_number == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(_number), "Dequeue is called too many times");
            }

            _number--;
        }

        /// <summary>
        ///     Initialize the class
        /// </summary>
        public void Clear()
        {
            _number = 0;
            if (Debug)
            {
                AgentsStarting = new ConcurrentBag<AgentId>();
            }
        }
    }
}