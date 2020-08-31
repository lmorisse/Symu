﻿#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Symu.Classes.Agents;
using Symu.Classes.Organization;
using Symu.Common;
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.DNA;
using Symu.DNA.OneModeNetworks.Agent;

#endregion

namespace Symu.Repository
{
    /// <summary>
    ///     white pages services
    ///     maintaining a directory of agent identifiers (AgentId contains the transport address by which an agent can be
    ///     found)
    ///     agent’s life-cycle administration service
    ///     every application agent must register itself with the WhitePagesServices to permit the validation of its identifier
    ///     in the multi-agent environment.
    /// </summary>
    /// <remarks>FIPA Norm : equivalent of the Agent Management System (AMS)</remarks>
    public class WhitePages
    {
        public WhitePages(OrganizationModels models)
        {
            if (models == null)
            {
                throw new ArgumentNullException(nameof(models));
            }

            MetaNetwork = new MetaNetwork(models.InteractionSphere);
        }

        /// <summary>
        ///     Stopped keep a trace of all the agentIds stopped during the iteration
        /// </summary>
        public List<IAgent> StoppedAgents { get; } = new List<IAgent>();

        public MetaNetwork MetaNetwork { get; }

        public AgentState State { get; set; } = AgentState.NotStarted;

        public bool Any()
        {
            return MetaNetwork.Agents.Any();
        }

        #region Initialize / remove agent

        /// <summary>
        ///     CLear agents between two iterations
        /// </summary>
        public void Clear()
        {
            State = AgentState.Starting;
            StoppedAgents.Clear();
            MetaNetwork.Clear();
        }

        /// <summary>
        ///     CLear agents between two iterations
        /// </summary>
        public void SetStarted()
        {
            State = AgentState.Started;
        }

        /// <summary>
        ///     Stops the execution of the agent identified by name and removes it from the environment. Use the Remove method
        ///     instead of Agent.Stop
        ///     when the decision to stop an agent does not belong to the agent itself, but to some other agent or to an external
        ///     factor.
        ///     Don't call it directly, use WhitePages.RemoveAgent
        /// </summary>
        /// <param name="agent">The agent to be removed</param>
        public void RemoveAgent(IAgent agent)
        {
            if (agent == null)
            {
                throw new ArgumentNullException(nameof(agent));
            }

            MetaNetwork.RemoveAgent(agent.AgentId);
            StoppedAgents.Add(agent);
        }

        #endregion

        public void WaitingForStart(IAgentId agentId)
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
        public void WaitingForStop(IAgentId agentId)
        {
            while (ExistsAndStarted(agentId))
            {
            }
        }

        #region Shortcuts to AgentNetwork

        /// <summary>
        ///     Start Agent if start is set
        /// </summary>
        /// <param name="agentToStart"></param>
        public static void StartAgent(ReactiveAgent agentToStart)
        {
            if (agentToStart is null)
            {
                throw new ArgumentNullException(nameof(agentToStart));
            }

            agentToStart.Start();
        }

        public bool ExistsAndStarted(IAgentId agentId)
        {
            if (!ExistsAgent(agentId))
            {
                return false;
            }

            var agent = GetAgent(agentId);
            return agent != null && agent.State == AgentState.Started;
        }

        public bool ExistsAgent(IAgentId agentId)
        {
            return MetaNetwork.Agents.Exists(agentId);
        }

        public void AddAgent(IAgent agent)
        {
            MetaNetwork.Agents.Add(agent);
        }

        public TAgent GetAgent<TAgent>(IAgentId agentId) where TAgent : IAgent
        {
            return MetaNetwork.Agents.Get<TAgent>(agentId);
        }

        public ReactiveAgent GetAgent(IAgentId agentId)
        {
            return (ReactiveAgent)MetaNetwork.Agents.Get(agentId);
        }

        /// <summary>
        ///     Returns a list with the names of all the agents.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IAgentId> AllAgentIds()
        {
            return MetaNetwork.Agents.GetKeys();
        }

        /// <summary>
        ///     Returns a list with the names of all the agents.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ReactiveAgent> AllAgents()
        {
            return MetaNetwork.Agents.GetValues().Cast<ReactiveAgent>();
        }

        /// <summary>
        ///     Returns a list with the names of all the agents.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CognitiveAgent> AllCognitiveAgents()
        {
            return MetaNetwork.Agents.GetValues().OfType<CognitiveAgent>();
        }

        /// <summary>
        ///     The number of agents in the environment
        /// </summary>
        public ushort FilteredAgentsByClassCount(IClassId classKey)
        {
            return MetaNetwork.Agents.CountByClassId(classKey);
        }

        /// <summary>
        ///     Returns a list with the names of all the agents that contain a certain string.
        /// </summary>
        /// <returns>The name fragment that the agent names should contain</returns>
        public IEnumerable<IAgentId> FilteredAgentIdsByClassId(IClassId classId)
        {
            return MetaNetwork.Agents.FilteredKeysByClassId(classId);
        }

        /// <summary>
        ///     Returns a list with the names of all the agents that contain a certain string.
        /// </summary>
        /// <returns>The name fragment that the agent names should contain</returns>
        public IEnumerable<ReactiveAgent> FilteredAgentsByClassId(IClassId classId)
        {
            return MetaNetwork.Agents.FilteredByClassId(classId).Cast<ReactiveAgent>();
        }

        /// <summary>
        ///     Returns a list with the names of all the agents that contain a certain string.
        /// </summary>
        /// <returns>The name fragment that the agent names should contain</returns>
        public IEnumerable<CognitiveAgent> FilteredCognitiveAgentsByClassId(IClassId classId)
        {
            return MetaNetwork.Agents.FilteredByClassId(classId).OfType<CognitiveAgent>();
        }

        /// <summary>
        ///     FilteredAgentIdsByClassKey with an exclusion list
        /// </summary>
        /// <param name="classKey"></param>
        /// <param name="excludeIds"></param>
        /// <returns></returns>
        public List<IAgentId> GetFilteredAgentIdsWithExclusionList(IClassId classKey, ICollection<IAgentId> excludeIds)
        {
            var actors = FilteredAgentIdsByClassId(classKey).ToList();
            if (excludeIds != null)
            {
                actors.RemoveAll(excludeIds.Contains);
            }

            return actors;
        }


        /// <summary>
        ///     FilteredAgentsByClassKey with an exclusion list
        /// </summary>
        /// <param name="classKey"></param>
        /// <param name="excludeIds"></param>
        /// <returns></returns>
        public IEnumerable<IAgent> GetFilteredAgentsWithExclusionList(IClassId classKey, ICollection<IAgentId> excludeIds)
        {
            var actors = FilteredAgentsByClassId(classKey).ToList();
            if (excludeIds != null)
            {
                actors.RemoveAll(x => excludeIds.Contains(x.AgentId));
            }

            return actors;
        }

        #endregion

        #region ToStop & Stopped Agents

        public bool HasAgentsToStop => AllAgents().Count(a => a.State == AgentState.Stopping) > 0;

        /// <summary>
        ///     Get the number of agents which are part of the interaction sphere
        /// </summary>
        public ushort GetInteractionSphereCount =>
            (ushort)AllCognitiveAgents().Count(a => a.Cognitive.InteractionPatterns.IsPartOfInteractionSphere);

        /// <summary>
        ///     Get the agents which are part of the interaction sphere
        /// </summary>
        public IEnumerable<CognitiveAgent> GetInteractionSphereAgents =>
            AllCognitiveAgents().Where(a => a.Cognitive.InteractionPatterns.IsPartOfInteractionSphere);

        /// <summary>
        ///     Stop Agent is managed by the WhitePages services responsible of Agent Lifecycle Management
        /// </summary>
        public void ManageAgentsToStop()
        {
            while (HasAgentsToStop)
            {
                var agent = AllAgents().First(a => a.State == AgentState.Stopping);
                agent.BeforeStop();
                RemoveAgent(agent);
                agent.Dispose();
            }
        }

        /// <summary>
        ///     Returns a list with the names of all the agents that contain a certain string.
        /// </summary>
        /// <returns>The name fragment that the agent names should contain</returns>
        public ushort FilteredStoppedAgentsByClassIdCount(IClassId classId)
        {
            return (ushort) StoppedAgents.Count(a => a.AgentId.Equals(classId));
        }

        #endregion

        public IAgentId GetAgentId(IId componentId)
        {
            return MetaNetwork.Agents.GetAgentId(componentId);
        }
    }
}