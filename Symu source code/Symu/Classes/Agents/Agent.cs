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
using Symu.Classes.Agents.Models.CognitiveModel;
using Symu.Classes.Agents.Models.Templates;
using Symu.Classes.Blockers;
using Symu.Classes.Task.Manager;
using Symu.Common;
using Symu.Environment;
using Symu.Messaging.Manager;
using Symu.Messaging.Messages;
using Symu.Repository;
using Symu.Repository.Networks.Databases;
using Symu.Tools;
using Symu.Tools.Math.ProbabilityDistributions;

#endregion

namespace Symu.Classes.Agents
{
    /// <summary>
    ///     An abstract base class for agents.
    ///     You must define your own agent derived classes derived
    /// </summary>
    public abstract partial class Agent
    {
        private byte _newInteractionCounter;

        /// <summary>
        ///     constructor for generic new()
        ///     Use with CreateAgent method
        /// </summary>
        protected Agent()
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="environment"></param>
        protected Agent(AgentId agentId, SymuEnvironment environment)
        {
            CreateAgent(agentId, environment);
        }

        /// <summary>
        ///     The name of the agent. Each agent must have a unique name in its environment.
        ///     Most operations are performed using agent names rather than agent objects.
        /// </summary>
        public AgentId Id { get; set; }

        /// <summary>
        ///     State of the agent
        /// </summary>
        public AgentState State { get; set; } = AgentState.NotStarted;

        /// <summary>
        ///     Interaction Status of the agent
        ///     Agent.State must be started
        /// </summary>
        public AgentStatus Status { get; set; } = AgentStatus.Available;

        /// <summary>
        ///     The environment in which the agent runs. A concurrent agent can only run in a concurrent environment.
        /// </summary>
        public SymuEnvironment Environment { get; private set; }

        /// <summary>
        ///     Define the cognitive architecture model of an agent
        ///     Modules, processes and structure intended to emulate structural and functional components of human cognition :
        ///     working memory, long-term memory, attention, multi tasking, perception, situation assessment, decision making,
        ///     planning, learning, goal management, ...
        /// </summary>
        public CognitiveArchitecture Cognitive { get; set; }

        protected Schedule Schedule => Environment.Schedule;

        /// <summary>
        ///     If agent has an email, get the email database of the agent
        /// </summary>
        protected Database Email => Environment.WhitePages.Network.NetworkDatabases.GetDatabase(Id.Key);

        /// <summary>
        ///     If agent has an email
        /// </summary>
        protected bool HasEmail => Environment.WhitePages.Network.NetworkDatabases.Exists(Id, Id.Key);

        /// <summary>
        ///     Define how agent will forget knowledge during the symu based on its cognitive architecture
        /// </summary>
        public ForgettingModel ForgettingModel { get; set; }

        /// <summary>
        ///     Define how agent will learn knowledge during the symu based on its cognitive architecture
        /// </summary>
        public LearningModel LearningModel { get; set; }

        /// <summary>
        ///     Define how agent will influence or be influenced during the symu based on its cognitive architecture
        /// </summary>
        public InfluenceModel InfluenceModel { get; set; }

        /// <summary>
        ///     Define how agent will manage its beliefs during the symu based on its cognitive architecture
        /// </summary>
        public BeliefsModel BeliefsModel { get; set; }

        /// <summary>
        ///     Define how agent will manage its knowledge during the symu based on its cognitive architecture
        /// </summary>
        public KnowledgeModel KnowledgeModel { get; set; }

        /// <summary>
        ///     Define how agent will manage its knowledge during the symu based on its cognitive architecture
        /// </summary>
        public ActivityModel ActivityModel { get; set; }

        /// <summary>
        ///     Manage every blocker of the agent
        /// </summary>
        [Obsolete]
        public BlockerCollection Blockers { get; } = new BlockerCollection();

        protected void CreateAgent(AgentId agentId, SymuEnvironment environment)
        {
            Id = agentId;
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            Environment.AddAgent(this);
            State = AgentState.NotStarted;
            foreach (var database in environment.Organization.Databases)
            {
                environment.WhitePages.Network.AddDatabase(Id, database.AgentId.Key);
            }
        }

        /// <summary>
        ///     CopyTo the cognitive architecture of the agent
        ///     Applying AgentTemplate
        ///     Initializing parameters
        /// </summary>
        /// <param name="agentTemplate"></param>
        protected virtual void SetCognitive(CognitiveArchitectureTemplate agentTemplate)
        {
            Cognitive = new CognitiveArchitecture();
            //Apply Cognitive template
            agentTemplate?.Set(Cognitive);
            // Initialize agent models
            LearningModel = new LearningModel(Id, Environment.Organization.Models,
                Environment.WhitePages.Network.NetworkKnowledges, Cognitive);
            ForgettingModel = new ForgettingModel(Id, Environment.Organization.Models,
                Cognitive, Environment.WhitePages.Network.NetworkKnowledges);
            InfluenceModel = new InfluenceModel(Id, Environment.Organization.Models.Influence,
                Cognitive.InternalCharacteristics, Environment.WhitePages.Network);
            BeliefsModel = new BeliefsModel(Id, Environment.Organization.Models.Beliefs, Cognitive,
                Environment.WhitePages.Network);
            KnowledgeModel = new KnowledgeModel(Id, Environment.Organization.Models.Knowledge, Cognitive,
                Environment.WhitePages.Network);
            ActivityModel = new ActivityModel(Id, Cognitive, Environment.WhitePages.Network);
        }

        #region Interaction strategy

        /// <summary>
        ///     List of AgentId for new interactions : there is no Active link (difference with GetAgentIdsForInteractions)
        ///     based on the interaction strategy of the interaction patterns :
        ///     Filtered with interactionStrategy and limit with number of new interactions
        /// </summary>
        public IEnumerable<AgentId> GetAgentIdsForNewInteractions()
        {
            if (!Environment.Organization.Models.InteractionSphere.IsAgentOn())
            {
                // Agent don't want to have new interactions today
                return new List<AgentId>();
            }

            var agentIds = Environment.WhitePages.Network.InteractionSphere.GetAgentIdsForNewInteractions(Id,
                Cognitive.InteractionPatterns.NextInteractionStrategy()).ToList();
            return FilterAgentIdsToInteract(agentIds);
        }

        /// <summary>
        ///     List of AgentId for interactions : there is Active link (difference with GetAgentIdsForNewInteractions)
        ///     based on the interaction strategy of the interaction patterns :
        ///     Filtered with interactionStrategy and limit with number of new interactions
        /// </summary>
        public IEnumerable<AgentId> GetAgentIdsForInteractions(InteractionStrategy interactionStrategy)
        {
            return Environment.WhitePages.Network.InteractionSphere
                .GetAgentIdsForInteractions(Id, interactionStrategy, Cognitive.InteractionPatterns).ToList();
        }

        /// <summary>
        ///     Filter the good number of agents based on Cognitive.InteractionPatterns
        /// </summary>
        /// <param name="agentIds"></param>
        /// <returns>List of AgentIds the agent can interact with via message</returns>
        public IEnumerable<AgentId> FilterAgentIdsToInteract(List<AgentId> agentIds)
        {
            if (agentIds == null)
            {
                throw new ArgumentNullException(nameof(agentIds));
            }

            var numberOfNewInteractions =
                Cognitive.InteractionPatterns.MaxNumberOfNewInteractions - _newInteractionCounter;
            if (Cognitive.InteractionPatterns.LimitNumberOfNewInteractions && numberOfNewInteractions > 0 &&
                agentIds.Count > numberOfNewInteractions)
            {
                agentIds.RemoveRange(Cognitive.InteractionPatterns.MaxNumberOfNewInteractions,
                    agentIds.Count - Cognitive.InteractionPatterns.MaxNumberOfNewInteractions);
            }

            return agentIds;
        }

        #endregion

        #region Start/stop

        /// <summary>
        ///     This method is called right after Start, before any messages have been received. It is similar to the constructor
        ///     of the class, but it should be used for agent-related logic, e.g. for sending initial message(s).
        ///     Send Delayed message to the Schedule.STep to be sure the receiver exists and its started
        /// </summary>
        public virtual void BeforeStart()
        {
            KnowledgeModel.InitializeExpertise(Schedule.Step);
            BeliefsModel.InitializeBeliefs();
            // Messaging initializing
            while (MessageProcessor is null)
            {
                //Sometimes Messaging is still null 
                //Just wait for Messaging to be initialized
            }

            MessageProcessor.OnBeforePostEvent += MessageOnBeforePost;
        }

        /// <summary>
        ///     Use to trigger an event before the agent is stopped
        /// </summary>
        public virtual void BeforeStop()
        {
            State = AgentState.Stopped;
        }

        public void Dispose()
        {
            MessageProcessor?.Dispose();
            TaskProcessor?.Dispose();
        }

        /// <summary>
        ///     Starts the agent execution, after it has been created.
        ///     First, the Setup method is called, and then the Act method is automatically called when the agent receives a
        ///     message.
        /// </summary>
        public void Start()
        {
            State = AgentState.Starting;
            if (Environment == null)
            {
                throw new Exception("Environment is null in agent " + Id.Key + " (ConcurrentAgent.Start)");
            }

            // MessageProcessor initializing
            MessageProcessor = AsyncMessageProcessor.Start(async mp =>
            {
                BeforeStart();
                State = AgentState.Started;
                while (true)
                {
                    var message = await mp.Receive().ConfigureAwait(false);
                    Act(message);
                    Environment.Messages.DeQueueWaitingMessage(message);
                }
            });
            // TaskProcessor initializing
            if (!Cognitive.TasksAndPerformance.CanPerformTask)
            {
                return;
            }

            TaskProcessor = new TaskProcessor(Cognitive.TasksAndPerformance.TasksLimit, Environment.Debug);
            OnAfterTaskProcessorStart();
        }

        /// <summary>
        ///     Waiting that the status agent == State.Started
        /// </summary>
        public void WaitingToStart()
        {
            while (State != AgentState.Started)
            {
            }
        }

        #endregion

        #region Subscribe

        private void ActSubscribe(Message message)
        {
            switch (message.Action)
            {
                case MessageAction.Remove:
                    RemoveSubscribe(message);
                    break;
                case MessageAction.Add:
                    AddSubscribe(message);
                    break;
            }
        }

        /// <summary>
        ///     Remove a subscription from the list of subscriptions
        /// </summary>
        /// <param name="message"></param>
        public void RemoveSubscribe(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message.HasAttachments)
            {
                MessageProcessor.Subscriptions.Unsubscribe(message.Sender, (byte) message.Attachments.First);
            }
            else
            {
                MessageProcessor.Subscriptions.Unsubscribe(message.Sender);
            }
        }

        /// <summary>
        ///     Add a subscription from the list of subscriptions
        /// </summary>
        /// <param name="message"></param>
        public void AddSubscribe(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            foreach (var subject in message.Attachments.Objects)
            {
                MessageProcessor.Subscriptions.Subscribe(message.Sender, (byte) subject);
            }
        }

        /// <summary>
        ///     Send a message to subscribe to the AgentId to the subject
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="subject"></param>
        public void Subscribe(AgentId agentId, byte subject)
        {
            var message = new Message(Id, agentId, MessageAction.Add, SymuYellowPages.Subscribe, subject);
            SendDelayed(message, Schedule.Step);
        }

        /// <summary>
        ///     UnSubscribe to the Message subject
        /// </summary>
        public void Unsubscribe(AgentId agentId, byte subject)
        {
            Send(agentId, MessageAction.Remove, SymuYellowPages.Subscribe, subject);
        }

        #endregion
    }
}