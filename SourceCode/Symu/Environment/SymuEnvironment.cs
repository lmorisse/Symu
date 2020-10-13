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
using Symu.Classes.Organization;
using Symu.Classes.Scenario;
using Symu.Common;
using Symu.Common.Classes;
using Symu.Engine;
using Symu.Messaging.Messages;
using Symu.Messaging.Tracker;
using Symu.OrgMod.Edges;
using Symu.OrgMod.Entities;
using Symu.Repository;
using Symu.Results;
using Symu.SysDyn;
using EventEntity = Symu.Repository.Entities.EventEntity;

#endregion

namespace Symu.Environment
{
    /// <summary>
    ///     The environment manage the agents, the messages, and the entities (the metaNetwork)
    /// </summary>
    public class SymuEnvironment
    {
        //TODO refactor MainOrganizationReference, StateMachineReference, ... in a readonly struct
        protected MainOrganization MainOrganizationReference {get; set; }

        public SymuEnvironment()
        {
            IterationResult = new IterationResult(this);
        }

        /// <summary>
        /// The MainOrganization that encapsulates the metaNetwork, the organizational models and so on.
        /// </summary>
        public MainOrganization MainOrganization { get; protected set; }

        /// <summary>
        ///     The white pages service of the simulation
        ///     To have access to all agents
        /// </summary>
        public WhitePages WhitePages { get; } = new WhitePages();

        /// <summary>
        /// The iteration result manage and store all results of an iteration
        /// </summary>
        public IterationResult IterationResult { get; set; }

        public SysDynModel SysDynModel { get; set; }

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

        /// <summary>
        ///     Define level of random for the simulation.
        /// </summary>
        public RandomLevel RandomLevel { get; set; } = RandomLevel.NoRandom;

        public byte RandomLevelValue => (byte) RandomLevel;

        public void SetRandomLevel(int level)
        {
            switch (level)
            {
                case 1:
                    RandomLevel = RandomLevel.Simple;
                    break;
                case 2:
                    RandomLevel = RandomLevel.Double;
                    break;
                case 3:
                    RandomLevel = RandomLevel.Triple;
                    break;
                default:
                    RandomLevel = RandomLevel.NoRandom;
                    break;
            }
        }

        #region Events

        /// <summary>
        ///     Schedule Events from the list Events
        /// </summary>
        public void ScheduleEvents()
        {
            foreach (var symuEvent in MainOrganization.MetaNetwork.Event.List.Cast<EventEntity>())
            {
                symuEvent.Schedule(Schedule.Step);
            }
        }

        //public void AddEvent(EventEntity eventEntity)
        //{
        //    if (eventEntity == null)
        //    {
        //        throw new ArgumentNullException(nameof(eventEntity));
        //    }

        //    Organization.MetaNetwork.Event.Add(eventEntity);
        //}

        #endregion

        #region Start and Stop

        /// <summary>
        ///     Once this method is called followed by InitializeIteration (via Initialize or Process methods)
        ///     You have to use Environment.Organization instead of your organization
        /// </summary>
        /// <param name="organization"></param>
        public virtual void SetOrganization(MainOrganization organization)
        {
            MainOrganizationReference = organization ?? throw new ArgumentNullException(nameof(organization));
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
            return WhitePages.FilteredAgentsByClassCount(ScenarioAgent.ClassId) == 0;
        }

        /// <summary>
        ///     Initialize model :
        ///     clear Agents and create/Add all agents of the model
        ///     called by Engine.InitializeIteration
        /// </summary>
        public virtual void InitializeIteration()
        {
            WhitePages.AgentIndex = 1;
            Messages.Clear();
            //At this point, we must use Environment.Organization.MetaNetwork and not Organization.MetaNetwork
            MainOrganization = MainOrganizationReference.Clone();
            WhitePages.Clear();
            IterationResult.Initialize();
            SetAgents();
            // Intentionally after SetAgents
            //InitializeInteractionNetwork();
            WhitePages.SetStarted();
        }

        /// <summary>
        ///     Create every agent of the environment in this method
        /// </summary>
        /// <remarks>Call Initialize method after having created an agent</remarks>
        public virtual void SetAgents()
        {
        }

        /// <summary>
        ///     Stop Agent is managed by the WhitePages services responsible of Agent Lifecycle Management
        /// </summary>
        public void StopAgents()
        {
            WhitePages.ManageAgentsToStop();
            Messages.WaitingToClearAllMessages();
        }

        public List<ScenarioAgent> GetAllStoppedScenarii()
        {
            var scenarioIds = WhitePages.StoppedAgents.FindAll(a => a.AgentId.ClassId.Equals(ScenarioAgent.ClassId))
                .Select(x => x.AgentId);

            return scenarioIds.Select(scenarioId => WhitePages.GetAgent<ScenarioAgent>(scenarioId))
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
            if (message?.Receiver == null)
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

            Messages.WaitingToClearAllMessages();
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
        //todo should be done in MetaNetwork
        private void SetInteractionSphere(bool initialization)
        {
            var agentIds = WhitePages.AllCognitiveAgents().Where(x =>
                x.Cognitive.InteractionPatterns.IsPartOfInteractionSphere &&
                x.State == AgentState.Started).Select(x => x.AgentId).ToList();
            MainOrganization.MetaNetwork.InteractionSphere.SetSphere(initialization, agentIds,
                MainOrganization.MetaNetwork);
        }

        /// <summary>
        ///     Trigger every event before the new step
        ///     Do not send messages, use NextStep for that
        ///     Trigger Agent.PreStep()
        /// </summary>
        public void PreStep()
        {
            var agents = WhitePages.AllAgents().ToList();
            // First update variables with the agents' properties' values
            SysDynModel.UpdateVariables(agents);
            // Then Process 
            SysDynModel.Process(agents);
            agents.ForEach(a => a.PreStep());
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
    }
}