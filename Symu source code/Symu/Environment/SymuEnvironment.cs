#region Licence

// Description: Symu - Symu
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
using Symu.Environment.Events;
using Symu.Messaging.Messages;
using Symu.Messaging.Tracker;
using Symu.Repository;
using Symu.Repository.Networks.Databases;
using Symu.Results;
using SymuTools;

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
        ///     The white pages service of the symu
        ///     To have access to all agents
        /// </summary>
        public WhitePages WhitePages { get; private set; }

        public IterationResult IterationResult { get; set; }

        /// <summary>
        ///     Use to slow down or speed up the symu
        ///     Delay is in milliseconds
        /// </summary>
        /// <example>Delay = 1000</example>
        public int Delay { get; set; }

        /// <summary>
        ///     Set the debug mode for additional information
        /// </summary>
        public bool Debug { get; set; } = true;

        /// <summary>
        ///     Manage interaction steps
        /// </summary>
        public Schedule Schedule { get; set; } = new Schedule();

        /// <summary>
        ///     Use to log messages in the symu
        ///     to debug and manage TimeBased Step
        /// </summary>
        public MessagesTracker Messages { get; set; } = new MessagesTracker();

        public List<SymuEvent> Events { get; set; } = new List<SymuEvent>();

        #region Start and Stop

        public virtual void SetOrganization(OrganizationEntity organization)
        {
            Organization = organization ?? throw new ArgumentNullException(nameof(organization));
            WhitePages = new WhitePages(Organization.Templates, Organization.Models);
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
            return WhitePages.FilteredAgentsByClassCount(SymuYellowPages.Scenario) == 0;
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
            WhitePages.Network.NetworkKnowledges.Model =
                Organization.Models.Generator;
            WhitePages.Network.NetworkBeliefs.Model =
                Organization.Models.Generator;
            IterationResult.Initialize();
            // Intentionally before SetModelForAgents
            SetModelForAgents();
            // Intentionally after SetModelForAgents
            WhitePages.Network.InitializeNetworkLinks();
            WhitePages.SetStarted();
        }

        /// <summary>
        ///     Stop Agent is managed by the WhitePages services responsible of Agent Lifecycle Management
        /// </summary>
        public void ManageAgentsToStop()
        {
            WhitePages.ManageAgentsToStop();
            Messages.WaitingToClearAllMessages();
        }

        public List<SimulationScenario> GetAllStoppedScenarii()
        {
            var scenarioIds = WhitePages.StoppedAgents.FindAll(a => a.ClassKey == SymuYellowPages.Scenario);

            return scenarioIds.Select(scenarioId => WhitePages.GetAgent<SimulationScenario>(scenarioId))
                .Where(scenario => scenario != null).ToList();
        }

        #endregion

        #region Add / remove agent

        /// <summary>
        ///     Adds an agent to the environment. The agent should already have a name and its name should be unique.
        /// </summary>
        /// <param name="agent">The concurrent agent that will be added</param>
        public void AddAgent(Agent agent)
        {
            if (agent is null)
            {
                throw new ArgumentNullException(nameof(agent));
            }

            if (WhitePages.Agents.Exists(agent.Id))
            {
                throw new ArgumentException("Trying to add an agent " + agent.Id.ClassKey + " with an existing key: " +
                                            agent.Id.Key);
            }

            WhitePages.Agents.Add(agent);
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
            WhitePages.RemoveAgent(agentId);
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

            if (WhitePages.Agents.Exists(message.Receiver))
                // Log is done within Agent.Post
            {
                WhitePages.Agents.Get(message.Receiver).Post(message);
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
        }

        public void NextStep()
        {
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

                if (Organization.Models.InteractionSphere.FrequencyOfSphereUpdate <=
                    TimeStepType.Daily && Schedule.Step > 0)
                {
                    SetInteractionSphere(false);
                }
            }

            if (Schedule.Type <= TimeStepType.Weekly && Schedule.IsEndOfWeek)
            {
                agents.ForEach(a => a.ActEndOfWeek());
                if (Organization.Models.InteractionSphere.FrequencyOfSphereUpdate ==
                    TimeStepType.Weekly && Schedule.Step > 0)
                {
                    SetInteractionSphere(false);
                }
            }

            if (Schedule.Type <= TimeStepType.Monthly && Schedule.IsEndOfMonth)
            {
                SetMonthlyIterationResult();
                agents.ForEach(a => a.ActEndOfMonth());
                if (Organization.Models.InteractionSphere.FrequencyOfSphereUpdate ==
                    TimeStepType.Monthly && Schedule.Step > 0)
                {
                    SetInteractionSphere(false);
                }
            }

            if (Schedule.IsEndOfYear)
            {
                agents.ForEach(a => a.ActEndOfYear());
                if (Organization.Models.InteractionSphere.FrequencyOfSphereUpdate ==
                    TimeStepType.Yearly && Schedule.Step > 0)
                {
                    SetInteractionSphere(false);
                }
            }
        }

        /// <summary>
        ///     Set Sphere for the InteractionSphere
        /// </summary>
        public void SetInteractionSphere(bool initialization)
        {
            var agentIds = WhitePages.AllAgents().Where(x => x.Cognitive.InteractionPatterns.IsPartOfInteractionSphere)
                .Select(x => x.Id).ToList();
            WhitePages.Network.InteractionSphere.SetSphere(initialization, agentIds, WhitePages.Network);
        }

        /// <summary>
        ///     Set monthly iterationResult
        /// </summary>
        public virtual void SetMonthlyIterationResult()
        {
            // Flexibility
            IterationResult.OrganizationFlexibility.HandlePerformance(Schedule.Step);
            // Knowledge
            IterationResult.OrganizationKnowledgeAndBelief.HandlePerformance(Schedule.Step);
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
        ///     Trigger Agent.PostStep()
        /// </summary>
        public void PostStep()
        {
            WhitePages.AllAgents().ToList().ForEach(a => a.PostStep());
            Messages.ClearMessagesSent(Schedule.Step);
            IterationResult.PostStep();
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

        #region Set model

        /// <summary>
        ///     Transform organization into agents
        /// </summary>
        public virtual void SetModelForAgents()
        {
            SetKnowledges();
            SetDatabases();
        }

        /// <summary>
        ///     Set repository of Knowledge network
        /// </summary>
        public virtual void SetKnowledges()
        {
            foreach (var knowledge in Organization.Knowledges)
            {
                WhitePages.Network.AddKnowledge(knowledge);
            }
        }

        /// <summary>
        ///     Set repository of Databases network
        /// </summary>
        public virtual void SetDatabases()
        {
            foreach (var database in Organization.Databases.Select(databaseEntity =>
                new Database(databaseEntity, Organization.Models, WhitePages.Network.NetworkKnowledges)))
            {
                WhitePages.Network.NetworkDatabases.AddDatabase(database);
            }
        }

        #endregion

        #region Events
        /// <summary>
        /// Schedule Events from the list Events
        /// </summary>
        public void ScheduleEvents()
        {
            foreach (var symuEvent in Events)
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

            Events.Add(symuEvent);
        }
        #endregion
    }
}