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
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Classes.Blockers;
using Symu.Classes.Task.Manager;
using Symu.Common;
using Symu.Environment;
using Symu.Messaging.Manager;
using Symu.Messaging.Messages;
using Symu.Repository;
using Symu.Repository.Networks.Databases;

#endregion

namespace Symu.Classes.Agents
{
    /// <summary>
    ///     An abstract base class for agents.
    ///     You must define your own agent derived classes derived
    /// </summary>
    public abstract partial class CognitiveAgent: ReactiveAgent
    {
        private byte _newInteractionCounter;

        /// <summary>
        ///     constructor for generic new()
        ///     Use with CreateAgent method
        /// </summary>
        protected CognitiveAgent()
        {
        }

        /// <summary>
        ///     Constructor with standard agent template
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="environment"></param>
        protected CognitiveAgent(AgentId agentId, SymuEnvironment environment)
        {
            CreateAgent(agentId, environment);
        }

        /// <summary>
        ///     Constructor with specific agentTemplate
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="environment"></param>
        /// <param name="template"></param>
        protected CognitiveAgent(AgentId agentId, SymuEnvironment environment, CognitiveArchitectureTemplate template)
        {
            CreateAgent(agentId, environment, template);
        }

        /// <summary>
        ///     Define the cognitive architecture model of an agent
        ///     Modules, processes and structure intended to emulate structural and functional components of human cognition :
        ///     working memory, long-term memory, attention, multi tasking, perception, situation assessment, decision making,
        ///     planning, learning, goal management, ...
        /// </summary>
        public CognitiveArchitecture Cognitive { get; set; }

        /// <summary>
        ///     If agent has an email, get the email database of the agent
        /// </summary>
        protected Database Email => Environment.WhitePages.MetaNetwork.Databases.GetDatabase(AgentId.Id);

        /// <summary>
        ///     If agent has an email
        /// </summary>
        protected bool HasEmail => Environment.WhitePages.MetaNetwork.Databases.Exists(AgentId, AgentId.Id);

        //TODO => all the models should be included in the cognitive architecture
        /// <summary>
        ///     Define how agent will forget knowledge during the simulation based on its cognitive architecture
        /// </summary>
        public ForgettingModel ForgettingModel { get; set; }

        /// <summary>
        ///     Define how agent will learn knowledge during the simulation based on its cognitive architecture
        /// </summary>
        public LearningModel LearningModel { get; set; }

        /// <summary>
        ///     Define how agent will influence or be influenced during the simulation based on its cognitive architecture
        /// </summary>
        public InfluenceModel InfluenceModel { get; set; }

        /// <summary>
        ///     Define how agent will manage its beliefs during the simulation based on its cognitive architecture
        /// </summary>
        public BeliefsModel BeliefsModel { get; set; }

        /// <summary>
        ///     Define how agent will manage its knowledge during the simulation based on its cognitive architecture
        /// </summary>
        public KnowledgeModel KnowledgeModel { get; set; }

        /// <summary>
        ///     Define how agent will manage its knowledge during the simulation based on its cognitive architecture
        /// </summary>
        public ActivityModel ActivityModel { get; set; }

        /// <summary>
        ///     Manage every blocker of the agent
        /// </summary>
        [Obsolete]
        public BlockerCollection Blockers { get; } = new BlockerCollection();

        #region Initialization

        protected override void CreateAgent(AgentId agentId, SymuEnvironment environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }
            CreateAgent(agentId, environment, environment.Organization.Templates.Standard);
        }

        protected void CreateAgent(AgentId agentId, SymuEnvironment environment,
            CognitiveArchitectureTemplate agentTemplate)
        {
            base.CreateAgent(agentId, environment);
            SetTemplate(agentTemplate);
            SetCognitive();
        }

        /// <summary>
        ///     Clone the cognitive architecture of the agent
        ///     Applying AgentTemplate
        /// </summary>
        /// <param name="agentTemplate"></param>
        protected void SetTemplate(CognitiveArchitectureTemplate agentTemplate)
        {
            Cognitive = new CognitiveArchitecture();
            //Apply Cognitive template
            agentTemplate?.Set(Cognitive);
        }

        /// <summary>
        ///     Customize the cognitive architecture of the agent
        ///     After setting the Agent template
        /// </summary>
        protected virtual void SetCognitive()
        {
        }

        #endregion

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

            var agentIds = Environment.WhitePages.MetaNetwork.InteractionSphere.GetAgentIdsForNewInteractions(AgentId,
                Cognitive.InteractionPatterns.NextInteractionStrategy()).Cast<AgentId>().ToList();
            return FilterAgentIdsToInteract(agentIds);
        }

        /// <summary>
        ///     List of AgentId for interactions : there is Active link (difference with GetAgentIdsForNewInteractions)
        ///     based on the interaction strategy of the interaction patterns :
        ///     Filtered with interactionStrategy and limit with number of new interactions
        /// </summary>
        public IEnumerable<AgentId> GetAgentIdsForInteractions(InteractionStrategy interactionStrategy)
        {
            return Environment.WhitePages.MetaNetwork.InteractionSphere
                .GetAgentIdsForInteractions(AgentId, interactionStrategy, Cognitive.InteractionPatterns).Cast<AgentId>().ToList();
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
        ///     Initialize all the agent's cognitive models
        ///     Should be called after SetTemplate and after having customized the cognitive parameters
        /// </summary>
        protected override void InitializeModels()
        {
            base.InitializeModels();
            // Initialize agent models
            LearningModel = new LearningModel(AgentId, Environment.Organization.Models,
                Environment.WhitePages.MetaNetwork.Knowledge, Cognitive);
            ForgettingModel = new ForgettingModel(AgentId, Environment.Organization.Models,
                Cognitive, Environment.WhitePages.MetaNetwork.Knowledge);
            InfluenceModel = new InfluenceModel(AgentId, Environment.Organization.Models.Influence,
                Cognitive.InternalCharacteristics, Environment.WhitePages.MetaNetwork);
            BeliefsModel = new BeliefsModel(AgentId, Environment.Organization.Models.Beliefs, Cognitive,
                Environment.WhitePages.MetaNetwork);
            KnowledgeModel = new KnowledgeModel(AgentId, Environment.Organization.Models.Knowledge, Cognitive,
                Environment.WhitePages.MetaNetwork);
            ActivityModel = new ActivityModel(AgentId, Cognitive, Environment.WhitePages.MetaNetwork);
        }

        /// <summary>
        ///     Finalize all the agent's cognitive models
        /// </summary>
        protected override void FinalizeModels()
        {
            base.FinalizeModels();
            if (KnowledgeModel.On)
            {
                KnowledgeModel.InitializeExpertise(Schedule.Step);
                foreach (var agentKnowledge in KnowledgeModel.Expertise.List)
                {
                    BeliefsModel.AddBelief(agentKnowledge.KnowledgeId);
                }
            }

            if (BeliefsModel.On)
            {
                BeliefsModel.InitializeBeliefs();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            TaskProcessor?.Dispose();
        }

        /// <summary>
        ///     Starts the agent execution, after it has been created.
        ///     First, the Setup method is called, and then the Act method is automatically called when the agent receives a
        ///     message.
        /// </summary>
        public override void Start()
        {
            base.Start();

            // TaskProcessor initializing
            if (!Cognitive.TasksAndPerformance.CanPerformTask)
            {
                return;
            }

            TaskProcessor = new TaskProcessor(Cognitive.TasksAndPerformance.TasksLimit, Environment.Debug);
            OnAfterTaskProcessorStart();
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
            var message = new Message(AgentId, agentId, MessageAction.Add, SymuYellowPages.Subscribe, subject);
            if (Schedule.Step == 0)
            {
                // Not sure the receiver exists already
                TrySendDelayed(message);
            }
            else
            {
                Send(message);
            }
        }

        /// <summary>
        ///     UnSubscribe to the Message subject
        /// </summary>
        public void Unsubscribe(AgentId agentId, byte subject)
        {
            Send(agentId, MessageAction.Remove, SymuYellowPages.Subscribe, subject);
        }

        /// <summary>
        ///     UnSubscribe to all subjects
        /// </summary>
        public void Unsubscribe(AgentId agentId)
        {
            Send(agentId, MessageAction.Remove, SymuYellowPages.Subscribe);
        }

        #endregion
    }
}