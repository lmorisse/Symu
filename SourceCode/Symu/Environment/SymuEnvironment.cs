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
using System.Threading;
using Symu.Classes.Agents;
using Symu.Classes.Organization;
using Symu.Classes.Scenario;
using Symu.Common;
using Symu.Common.Classes;
using Symu.Common.Interfaces;
using Symu.Common.Interfaces.Agent;
using Symu.Messaging.Messages;
using Symu.Messaging.Tracker;
using Symu.Repository;
using Symu.Repository.Entity;
using Symu.Repository.Networks.Events;
using Symu.Results;

#endregion

namespace Symu.Environment
{
    /// <summary>
    ///     A environment, where the agents run in parallel
    /// </summary>
    public abstract class SymuEnvironment
    {
        protected SymuEnvironment()
        {
            IterationResult = new IterationResult(this);
        }

        public OrganizationEntity Organization { get; protected set; }

        /// <summary>
        ///     The white pages service of the simulation
        ///     To have access to all agents
        /// </summary>
        public WhitePages WhitePages { get; private set; }

        public IterationResult IterationResult { get; set; }

        /// <summary>
        ///     Use to slow down or speed up the simulation
        ///     Delay is in milliseconds
        /// </summary>
        /// <example>Delay = 1000</example>
        public int Delay { get; set; }

        /// <summary>
        ///     Clone the debug mode for additional information
        /// </summary>
        public bool Debug { get; set; } = true;

        /// <summary>
        ///     Manage interaction steps
        /// </summary>
        public Schedule Schedule { get; set; } = new Schedule();

        /// <summary>
        ///     Use to log messages in the simulation
        ///     to debug and manage TimeBased Step
        /// </summary>
        public MessagesTracker Messages { get; set; } = new MessagesTracker();

        #region Add / remove agent

        /// <summary>
        ///     Adds an agent to the environment. The agent should already have a name and its name should be unique.
        /// </summary>
        /// <param name="agent">The concurrent agent that will be added</param>
        public void AddAgent(ReactiveAgent agent)
        {
            if (agent is null)
            {
                throw new ArgumentNullException(nameof(agent));
            }

            if (WhitePages.ExistsAgent(agent.AgentId))
            {
                throw new ArgumentException("Trying to add an agent " + agent.AgentId.ClassId + " with an existing key: " +
                                            agent.AgentId.Id);
            }

            WhitePages.AddAgent(agent);
        }

        #endregion

        #region Start and Stop

        public virtual void SetOrganization(OrganizationEntity organization)
        {
            Organization = organization ?? throw new ArgumentNullException(nameof(organization));
            WhitePages = new WhitePages(Organization.Models);
        }

        /// <summary>
        ///     Waiting for all the agents to start
        /// </summary>
        public void WaitingForStart()
        {
            while (WhitePages.AllAgents().ToList().Exists(a => a.State != AgentState.Started))
            {
            }
        }

        public void SetRandomLevel(int value)
        {
            Organization.Models.SetRandomLevel(value);
        }

        public void SetDebug(bool value)
        {
            Messages.Debug = value;
            Debug = value;
        }

        public void SetDelay(int value)
        {
            Delay = value;
        }

        public void SetTimeStepType(TimeStepType type)
        {
            Schedule.Type = type;
            foreach (var result in IterationResult.Results.Where(result => result.Frequency < type))
            {
                result.Frequency = type;
            }
        }

        /// <summary>
        ///     override : use to set specific PostProcessResult parameter
        /// </summary>
        public virtual IterationResult SetIterationResult(ushort iterationNumber)
        {
            IterationResult.Iteration = iterationNumber;
            IterationResult.Step = Schedule.Step;
            var scenarii = GetAllStoppedScenarii();
            IterationResult.Success = scenarii.All(s => s.Success);
            //IterationResult.HasItemsNotDone = scenarii.Exists(s => s.IterationResult.HasItemsNotDone);
            //IterationResult.NotFinishedInTime = scenarii.Exists(s => s.IterationResult.NotFinishedInTime);
            //IterationResult.SeemsToBeBlocked = scenarii.Exists(s => s.IterationResult.SeemsToBeBlocked);
            return IterationResult.Clone();
        }

        /// <summary>
        ///     Symu will stop when all scenario are stopped
        /// </summary>
        /// <returns>
        ///     true if iteration must stop, false otherwise
        /// </returns>
        public bool StopIteration()
        {
            return WhitePages.FilteredAgentsByClassCount(new ClassId(SymuYellowPages.Scenario)) == 0;
        }

        /// <summary>
        ///     Initialize model :
        ///     clear Agents and create/Add all agents of the model
        ///     called by Engine.InitializeIteration
        /// </summary>
        public virtual void InitializeIteration()
        {
            Messages.Clear();
            WhitePages.Clear();
            WhitePages.MetaNetwork.Knowledge.Model =
                Organization.Models.Generator;
            //WhitePages.MetaNetwork.Beliefs.Model =
            //    Organization.Models.Generator;
            IterationResult.Initialize();
            // Intentionally before AddOrganizationKnowledges
            AddOrganizationDatabase();
            SetDatabases();
            // Intentionally before SetAgents
            AddOrganizationKnowledges();
            SetKnowledges();
            SetAgents();
            // Intentionally after SetAgents
            InitializeInteractionNetwork();
            WhitePages.SetStarted();
        }

        /// <summary>
        ///     Initialize the network interactions.
        ///     For performance it is not done in AddMemberToGroup at initialization
        /// </summary>
        public void InitializeInteractionNetwork()
        {
            foreach (var groupId in WhitePages.MetaNetwork.Groups.GetGroups().ToList())
            {
                var agentIds = WhitePages.MetaNetwork.Groups.GetAgents(groupId, new ClassId(SymuYellowPages.Actor))
                    .ToList();

                var count = agentIds.Count;
                for (var i = 0; i < count; i++)
                {
                    var agentId1 = agentIds[i];
                    // interaction are undirected
                    for (var j = i + 1; j < count; j++)
                    {
                        var agentId2 = agentIds[j];
                        var interaction = new Interaction(agentId1, agentId2);
                        WhitePages.MetaNetwork.Interactions.AddInteraction(interaction);
                    }
                }
            }
        }

        /// <summary>
        ///     Stop Agent is managed by the WhitePages services responsible of Agent Lifecycle Management
        /// </summary>
        public void StopAgents()
        {
            WhitePages.ManageAgentsToStop();
            Messages.WaitingToClearAllMessages();
        }

        public List<SimulationScenario> GetAllStoppedScenarii()
        {
            var scenarioIds = WhitePages.StoppedAgents.FindAll(a => a.AgentId.Equals(SymuYellowPages.Scenario))
                .Select(x => x.AgentId);

            return scenarioIds.Select(scenarioId => WhitePages.GetAgent<SimulationScenario>(scenarioId))
                .Where(scenario => scenario != null).ToList();
        }

        #endregion

        #region Send & delayed messages

        /// <summary>
        ///     Send message in this environment
        /// </summary>
        /// <param name="message"></param>
        public void SendAgent(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (WhitePages.ExistsAgent(message.Receiver))
                // Log is done within Agent.Post
            {
                WhitePages.GetAgent(message.Receiver).Post(message);
            }
            else
            {
                Messages.EnqueueLostMessage(message);
            }
        }

        /// <summary>
        ///     Send a message that will be received another step
        /// </summary>
        /// <param name="message">the message to delay</param>
        /// <param name="step">the step to which the message will be sent</param>
        public void SendDelayedMessage(Message message, ushort step)
        {
            Messages.DelayedMessages.Enqueue(message, step);
        }

        public void SendDelayedMessages()
        {
            while (Messages.DelayedMessages.Dequeue(Schedule.Step) is Message message)
            {
                SendAgent(message);
            }
        }

        #endregion

        #region Manage interaction Step

        /// <summary>
        ///     Prevents the program from closing by waiting as long as some agents still run in the environment. This method
        ///     should be used at the end of the
        ///     main program, after all the agents have been added to the environment and started.
        /// </summary>
        public void OnNextStep()
        {
            PreStep();
            NextStep();
            while (WaitForStepEnd())
            {
            }

            PostStep();
            StopAgents();
        }

        public void NextStep()
        {
            // Here, Sphere is updated, it is initialized in InitializeIteration
            if (Schedule.Step > 0)
            {
                UpdateInteractionSphere();
            }

            ScheduleEvents();

            SendDelayedMessages();

            var agents = WhitePages.AllAgents().Shuffle();
            if (Schedule.Type <= TimeStepType.Daily)
            {
                if (Schedule.IsWorkingDay)
                {
                    agents.ForEach(a => a.ActCadence());
                    agents.ForEach(a => a.ActWorkingDay());
                }
                else
                {
                    agents.ForEach(a => a.ActWeekEnd());
                }
            }

            if (Schedule.Type <= TimeStepType.Weekly && Schedule.IsEndOfWeek)
            {
                agents.ForEach(a => a.ActEndOfWeek());
            }

            if (Schedule.Type <= TimeStepType.Monthly && Schedule.IsEndOfMonth)
            {
                agents.ForEach(a => a.ActEndOfMonth());
            }

            if (Schedule.IsEndOfYear)
            {
                agents.ForEach(a => a.ActEndOfYear());
            }
        }

        /// <summary>
        ///     Initialize the InteractionSphere
        ///     for all the started agents with Cognitive.InteractionPatterns.IsPartOfInteractionSphere :
        ///     Knowledge model, beliefs models must be initialized for the agents
        /// </summary>
        public void InitializeInteractionSphere()
        {
            SetInteractionSphere(true);
        }

        /// <summary>
        ///     Update the InteractionSphere
        ///     for all the started agents with Cognitive.InteractionPatterns.IsPartOfInteractionSphere
        /// </summary>
        public void UpdateInteractionSphere()
        {
            SetInteractionSphere(false);
        }

        /// <summary>
        ///     SetSphere for the InteractionSphere
        ///     for all the started agents with Cognitive.InteractionPatterns.IsPartOfInteractionSphere
        /// </summary>
        private void SetInteractionSphere(bool initialization)
        {
            var agentIds = WhitePages.AllCognitiveAgents().Where(x =>
                x.Cognitive.InteractionPatterns.IsPartOfInteractionSphere &&
                x.State == AgentState.Started).Select(x => x.AgentId).ToList();
            WhitePages.MetaNetwork.InteractionSphere.SetSphere(initialization, agentIds, WhitePages.MetaNetwork);
        }

        /// <summary>
        ///     Trigger every event before the new step
        ///     Do not send messages, use NextStep for that
        ///     Trigger Agent.PreStep()
        /// </summary>
        public void PreStep()
        {
            WhitePages.AllAgents().ToList().ForEach(a => a.PreStep());
        }

        /// <summary>
        ///     Trigger every event after the actual step
        ///     Trigger Agent.SetResults()
        /// </summary>
        public void PostStep()
        {
            WhitePages.AllAgents().ToList().ForEach(a => a.PostStep());
            Messages.ClearMessagesSent(Schedule.Step);
            IterationResult.SetResults();
            Schedule.Step++;
        }

        private bool WaitForStepEnd()
        {
            // For unit tests
            Thread.Sleep(Delay);
            return Messages.IsThereAnyWaitingMessages();
        }

        public void Start()
        {
            WhitePages.AllAgents().ToList().ForEach(a => a.Start());
        }

        #endregion

        #region Clone model

        /// <summary>
        ///     Create every agent of the simulation in this method
        /// </summary>
        /// <remarks>Call Initialize method after having created an agent</remarks>
        public virtual void SetAgents()
        {
        }

        /// <summary>
        ///     Add Organization knowledge
        /// </summary>
        public virtual void AddOrganizationKnowledges()
        {
        }

        /// <summary>
        ///     Clone repository of Knowledge network
        /// </summary>
        public void SetKnowledges()
        {
            WhitePages.MetaNetwork.Knowledge.AddKnowledges(Organization.Knowledges);
            foreach (var knowledge in Organization.Knowledges)
            {
                var belief = new Belief(knowledge, knowledge.Length, Organization.Models.Generator, Organization.Models.BeliefWeightLevel);
                WhitePages.MetaNetwork.Beliefs.AddBelief(belief);
            }
        }


        /// <summary>
        ///     Add Organization database
        /// </summary>
        public virtual void AddOrganizationDatabase()
        {
        }

        /// <summary>
        ///     Clone repository of Databases network
        /// </summary>
        public void SetDatabases()
        {
            foreach (var database in Organization.Databases.Select(databaseEntity =>
                new Database(databaseEntity, Organization.Models, WhitePages.MetaNetwork.Knowledge)))
            {
                WhitePages.MetaNetwork.Resources.Add(database);
            }
        }

        #endregion

        #region Events

        /// <summary>
        ///     Schedule Events from the list Events
        /// </summary>
        public void ScheduleEvents()
        {
            foreach (var symuEvent in WhitePages.MetaNetwork.Events.List.Cast<SymuEvent>())
            {
                symuEvent.Schedule(Schedule.Step);
            }
        }

        public void AddEvent(SymuEvent symuEvent)
        {
            if (symuEvent == null)
            {
                throw new ArgumentNullException(nameof(symuEvent));
            }

            WhitePages.MetaNetwork.Events.Add(symuEvent);
        }

        #endregion
    }
}