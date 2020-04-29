#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using SymuEngine.Classes.Agent;
using SymuEngine.Common;
using SymuEngine.Repository.Networks;

#endregion

namespace SymuEngine.Repository
{
    /// <summary>
    ///     white pages services
    ///     maintaining a directory of agent identifiers (AgentId contains the transport address by which an agent can be
    ///     found)
    ///     agent’s life-cycle administration service
    ///     every application agent must register itself with the WhitePagesServices to permit the validation of its identifier
    ///     in the multi-agent environment.
    /// </summary>
    /// <remarks>FIPA Norm : Agent Management System (AMS)</remarks>
    public class WhitePages
    {
        /// <summary>
        ///     Local agents of this environment
        /// </summary>
        public ConcurrentAgents<Agent> Agents { get; } = new ConcurrentAgents<Agent>();

        /// <summary>
        ///     Stopped keep a trace of all the agents stopped during the iteration
        ///     It is active when Debug is On
        ///     Key => Environment.Id
        ///     Value => list of agent Id of the stopped Agents>
        /// </summary>
        public List<AgentId> StoppedAgents { get; } = new List<AgentId>();

        public Network Network { get; } = new Network();

        #region Clear / remove agent

        /// <summary>
        ///     CLear agents between two iterations
        /// </summary>
        public void Clear()
        {
            StoppedAgents.Clear();
            Network.Clear();
            Agents.Clear();
        }

        /// <summary>
        ///     Stops the execution of the agent identified by name and removes it from the environment. Use the Remove method
        ///     instead of Agent.Stop
        ///     when the decision to stop an agent does not belong to the agent itself, but to some other agent or to an external
        ///     factor.
        ///     Don't call it directly, use WhitePages.RemoveAgent
        /// </summary>
        /// <param name="agentId">The name of the agent to be removed</param>
        public void RemoveAgent(AgentId agentId)
        {
            Network.RemoveAgent(agentId);
            Agents.Remove(agentId);
            StoppedAgents.Add(agentId);
        }

        #endregion

        #region Exists and Get Agent

        public void WaitingForStart(AgentId agentId)
        {
            while (!ExistsAndStarted(agentId))
            {
            }
        }

        /// <summary>
        ///     Waiting for all the Agents to stop
        ///     Useful for unit tests
        /// </summary>
        /// <param name="agentId"></param>
        public void WaitingForStop(AgentId agentId)
        {
            while (ExistsAndStarted(agentId))
            {
            }
        }

        /// <summary>
        ///     Start Agent if start is set
        /// </summary>
        /// <param name="agentToStart"></param>
        public static void StartAgent(Agent agentToStart)
        {
            if (agentToStart is null)
            {
                throw new ArgumentNullException(nameof(agentToStart));
            }

            agentToStart.Start();
        }

        public bool ExistsAndStarted(AgentId agentId)
        {
            if (!ExistsAgent(agentId))
            {
                return false;
            }

            var agent = GetAgent(agentId);
            return agent != null && agent.State == AgentState.Started;
        }

        public bool ExistsAgent(AgentId agentId)
        {
            return Agents.Exists(agentId);
        }

        public TAgent GetAgent<TAgent>(AgentId agentId) where TAgent : Agent
        {
            return Agents.Get<TAgent>(agentId);
        }

        public Agent GetAgent(AgentId agentId)
        {
            return Agents.Get(agentId);
        }

        /// <summary>
        ///     Returns a list with the names of all the agents.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AgentId> AllAgentIds()
        {
            return Agents.GetKeys();
        }

        /// <summary>
        ///     Returns a list with the names of all the agents.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Agent> AllAgents()
        {
            return Agents.GetValues();
        }

        /// <summary>
        ///     The number of agents in the environment
        /// </summary>
        public ushort FilteredAgentsByClassCount(byte classKey)
        {
            return Agents.CountByClassKey(classKey);
        }

        /// <summary>
        ///     Returns a list with the names of all the agents that contain a certain string.
        /// </summary>
        /// <returns>The name fragment that the agent names should contain</returns>
        public IEnumerable<AgentId> FilteredAgentIdsByClassKey(byte classKey)
        {
            return Agents.FilteredKeysByClassKey(classKey);
        }

        /// <summary>
        ///     Returns a list with the names of all the agents that contain a certain string.
        /// </summary>
        /// <returns>The name fragment that the agent names should contain</returns>
        public IEnumerable<Agent> FilteredAgentsByClassKey(byte classKey)
        {
            return Agents.FilteredByClassKey(classKey);
        }

        #endregion

        #region ToStop & Stopped Agents

        public bool HasAgentsToStop => AllAgents().Count(a => a.State == AgentState.Stopping) > 0;

        /// <summary>
        ///     Stop Agent is managed by the WhitePages services responsible of Agent Lifecycle Management
        /// </summary>
        public void ManageAgentsToStop()
        {
            while (HasAgentsToStop)
            {
                var agent = AllAgents().First(a => a.State == AgentState.Stopping);
                agent.BeforeStop();
                RemoveAgent(agent.Id);
                agent.Dispose();
            }
        }

        /// <summary>
        ///     Returns a list with the names of all the agents that contain a certain string.
        /// </summary>
        /// <returns>The name fragment that the agent names should contain</returns>
        public ushort FilteredStoppedAgentsByClassKeyCount(byte classKey)
        {
            return (ushort) StoppedAgents.Count(a => a.ClassKey == classKey);
        }

        #endregion
    }
}