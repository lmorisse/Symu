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
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Classes.Blockers;
using Symu.Classes.Task.Manager;
using Symu.Common.Interfaces.Agent;
using Symu.DNA.TwoModesNetworks.Sphere;
using Symu.Environment;
using Symu.Repository.Entity;

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
        private readonly CognitiveArchitectureTemplate _cognitiveTemplate;

        private CognitiveAgent()
        {
        }

        /// <summary>
        ///     Constructor with standard agent template
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="environment"></param>
        /// <remarks> Make constructor private and create a factory method to create an agent that call the Initialize method</remarks>
        protected CognitiveAgent(IAgentId agentId, SymuEnvironment environment) : base(agentId, environment)
        {
            _cognitiveTemplate = environment.Organization.Templates.Standard;
        }

        /// <summary>
        ///     Constructor with specific agentTemplate
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="environment"></param>
        /// <param name="template"></param>
        /// <remarks> Make constructor private and create a factory method to create an agent that call the Initialize method</remarks>
        protected CognitiveAgent(IAgentId agentId, SymuEnvironment environment, CognitiveArchitectureTemplate template) : base(agentId, environment)
        {
            _cognitiveTemplate = template;
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
        protected Database Email => HasEmail ? Environment.WhitePages.MetaNetwork.Resource.Get<Database>(AgentId.Id) : null;

        /// <summary>
        ///     If agent has an email
        /// </summary>
        public bool HasEmail => Environment.WhitePages.MetaNetwork.AgentResource.Exists(AgentId, AgentId.Id);

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

        /// <summary>
        /// Should be called after the constructor 
        /// </summary>
        protected override void Initialize()
        {
            SetTemplate(_cognitiveTemplate);
            SetCognitive();
            // Databases are added to CognitiveAgent only, as it is a source of knowledge
            foreach (var database in Environment.Organization.Databases)
            {
                // Organization databases are used by every one
                var agentResource = new AgentResource(database.Id, new ResourceUsage(0), 100);
                Environment.WhitePages.MetaNetwork.AgentResource.Add(AgentId, agentResource);
            }
            // intentionally before base.Initialize()
            base.Initialize();
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
        public IEnumerable<IAgentId> GetAgentIdsForNewInteractions()
        {
            if (!Environment.Organization.Models.InteractionSphere.IsAgentOn())
            {
                // Agent don't want to have new interactions today
                return new List<IAgentId>();
            }

            var agentIds = Environment.WhitePages.MetaNetwork.InteractionSphere.GetAgentIdsForNewInteractions(AgentId,
                Cognitive.InteractionPatterns.NextInteractionStrategy());
            return FilterAgentIdsToInteract(agentIds.ToList());
        }

        /// <summary>
        ///     List of AgentId for interactions : there is Active link (difference with GetAgentIdsForNewInteractions)
        ///     based on the interaction strategy of the interaction patterns :
        ///     Filtered with interactionStrategy and limit with number of new interactions
        /// </summary>
        public IEnumerable<IAgentId> GetAgentIdsForInteractions(InteractionStrategy interactionStrategy)
        {
            return Environment.WhitePages.MetaNetwork.InteractionSphere
                .GetAgentIdsForInteractions(AgentId, interactionStrategy);
        }

        /// <summary>
        ///     Filter the good number of agents based on Cognitive.InteractionPatterns
        /// </summary>
        /// <param name="agentIds"></param>
        /// <returns>List of AgentIds the agent can interact with via message</returns>
        public IEnumerable<IAgentId> FilterAgentIdsToInteract(List<IAgentId> agentIds)
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
            KnowledgeModel = new KnowledgeModel(AgentId, Environment.Organization.Models.Knowledge, Cognitive,
                Environment.WhitePages.MetaNetwork, Environment.Organization.Models.Generator);
            BeliefsModel = new BeliefsModel(AgentId, Environment.Organization.Models.Beliefs, Cognitive,
                Environment.WhitePages.MetaNetwork, Environment.Organization.Models.Generator);
            LearningModel = new LearningModel(AgentId, Environment.Organization.Models,
                Environment.WhitePages.MetaNetwork, Cognitive, Environment.Organization.Models.Generator);
            ForgettingModel = new ForgettingModel(KnowledgeModel.Expertise, Cognitive, Environment.Organization.Models);
            InfluenceModel = new InfluenceModel(Environment.Organization.Models.Influence,
                Cognitive, Environment.WhitePages.MetaNetwork, BeliefsModel, Environment.Organization.Models.Generator);
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
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
    }
}