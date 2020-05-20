#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using SymuEngine.Classes.Agents.Models.CognitiveModel;
using SymuEngine.Classes.Agents.Models.Templates;
using SymuEngine.Classes.Task.Manager;
using SymuEngine.Common;
using SymuEngine.Environment;
using SymuEngine.Messaging.Manager;
using SymuEngine.Messaging.Messages;
using SymuEngine.Repository;
using SymuEngine.Repository.Networks.Databases;
using SymuTools;
using SymuTools.Math.ProbabilityDistributions;

#endregion

namespace SymuEngine.Classes.Agents
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
        ///     Messaging of the agent
        /// </summary>
        public MessageProcessor MessageProcessor { get; set; }

        /// <summary>
        ///     Tasks manager for the agent
        ///     Null if !Cognitive.TasksAndPerformance.CanPerformTask
        /// </summary>
        public TaskProcessor TaskProcessor { get; private set; }

        /// <summary>
        ///     Define the cognitive architecture model of an agent
        ///     Modules, processes and structure intended to emulate structural and functional components of human cognition :
        ///     working memory, long-term memory, attention, multi tasking, perception, situation assessment, decision making,
        ///     planning, learning, goal management, ...
        /// </summary>
        public CognitiveArchitecture Cognitive { get; set; }

        protected TimeStep TimeStep => Environment.TimeStep;

        /// <summary>
        ///     If agent has an email, get the email database of the agent
        /// </summary>
        protected Database Email => Environment.WhitePages.Network.NetworkDatabases.GetDatabase(Id.Key);

        /// <summary>
        ///     If agent has an email
        /// </summary>
        protected bool HasEmail => Environment.WhitePages.Network.NetworkDatabases.Exists(Id, Id.Key);

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
        ///     Set the cognitive architecture of the agent
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
                Cognitive.InteractionPatterns.NextInteractionStrategy(), Cognitive.InteractionPatterns).ToList();
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
        ///     Send Delayed message to the TimeStep.STep to be sure the receiver exists and its started
        /// </summary>
        public virtual void BeforeStart()
        {
            KnowledgeModel.InitializeExpertise(TimeStep.Step);
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
                Environment.State.DequeueStartedAgent();
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

            {
                TaskProcessor = new TaskProcessor(Cognitive.TasksAndPerformance.TasksLimit);
                OnAfterTaskProcessorStart();
            }
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

        #region Post message

        public void Post(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (MessageProcessor is null)
            {
                throw new ArgumentNullException(nameof(MessageProcessor));
            }

            switch (message.Medium)
            {
                case CommunicationMediums.Irc:
                case CommunicationMediums.Email:
                case CommunicationMediums.ViaAPlatform:
                    if (Status == AgentStatus.Offline)
                    {
                        // If receiver is offline, the message is postponed until the next interaction
                        PostAsADelayedMessage(message, (ushort) (TimeStep.Step + 1));
                    }
                    else
                    {
                        // The message is posted
                        PostMessage(message);
                    }

                    break;
                case CommunicationMediums.Phone:
                case CommunicationMediums.Meeting:
                case CommunicationMediums.FaceToFace:
                    if (Status == AgentStatus.Offline)
                    {
                        // message is Missed
                        MessageProcessor.AddMissedMessage(message, Environment.State.Debug);
                    }
                    else
                    {
                        // The message is posted
                        PostMessage(message);
                    }

                    break;
                //case MessageType.System:
                default:
                    switch (State)
                    {
                        case AgentState.NotStarted:
                        case AgentState.Starting:
                            // If receiver is offline, the message is postponed until the next interaction
                            PostAsADelayedMessage(message, (ushort) (TimeStep.Step + 1));
                            break;
                        case AgentState.Stopped:
                            break;
                        default:
                            PostMessage(message);
                            break;
                    }

                    break;
            }
        }

        /// <summary>
        ///     Message post is postponed until another step, because agent were offline
        /// </summary>
        /// <param name="message">the message to delay</param>
        /// <param name="step">the step to which the message will be post</param>
        public void PostAsADelayedMessage(Message message, ushort step)
        {
            MessageProcessor.PostAsADelayed(message, step);
        }

        /// <summary>
        ///     Message is post this step
        /// </summary>
        /// <param name="message"></param>
        public void PostMessage(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            // The agent may have received too much messages for the step
            if (IsMessagesPerPeriodBelowLimit(message.Medium) && IsMessagesReceivedPerPeriodBelowLimit(message.Medium))
            {
                if (AcceptNewInteraction(message.Sender))
                {
                    OnBeforePostMessage(message);
                    MessageProcessor.Post(message);
                    OnAfterPostMessage(message);
                }
                else
                {
                    MessageProcessor.AddNotAcceptedMessages(message, Environment.State.Debug);
                }
            }
            else
            {
                MessageProcessor.AddMissedMessage(message, Environment.State.Debug);
            }
        }

        /// <summary>
        ///     The message may be accepted or not depending if it's in its interaction sphere :
        ///     Or does he accept a new interaction :
        /// </summary>
        /// <param name="senderId"></param>
        /// <returns>True if the new interaction has been accepted</returns>
        public bool AcceptNewInteraction(AgentId senderId)
        {
            if (Id.Equals(senderId))
            {
                // for unit test
                return true;
            }

            var sender = Environment.WhitePages.GetAgent(senderId);
            if (!Cognitive.InteractionPatterns.IsPartOfInteractionSphere ||
                !sender.Cognitive.InteractionPatterns.IsPartOfInteractionSphere)
            {
                return true;
            }

            if (Environment.WhitePages.Network.NetworkLinks.HasActiveLink(Id, senderId))
            {
                return true;
            }

            if (!Cognitive.InteractionPatterns.AllowNewInteractions)
            {
                return false;
            }

            if (Cognitive.InteractionPatterns.LimitNumberOfNewInteractions && _newInteractionCounter >=
                Cognitive.InteractionPatterns.MaxNumberOfNewInteractions)
            {
                return false;
            }

            // Not in its sphere of interaction (with an active link)
            if (!Bernoulli.Sample(Cognitive.InteractionPatterns.ThresholdForNewInteraction))
            {
                return false;
            }

            _newInteractionCounter++;

            // Decide to positively answer to this new interaction
            if (Environment.Organization.Models.InteractionSphere.SphereUpdateOverTime)
            {
                // Message.Sender is now part of agent interaction sphere
                Environment.WhitePages.Network.NetworkLinks.AddLink(Id, senderId);
            }

            return true;
        }

        /// <summary>
        ///     Triggered before post message in the MessageManager
        /// </summary>
        /// <param name="message"></param>
        public virtual void OnBeforePostMessage(Message message)
        {
        }

        /// <summary>
        ///     Triggered before message send in the mailbox
        /// </summary>
        /// <param name="message"></param>
        public virtual void OnBeforeSendMessage(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            // Impact of the Communication channels on the remaining capacity
            var impact =
                Environment.WhitePages.Network.NetworkCommunications.TimeSpent(message.Medium, true,
                    Environment.Organization.Models.RandomLevelValue);
            Capacity.Decrement(impact);
        }

        /// <summary>
        ///     Triggered after message post in the mailbox
        /// </summary>
        public virtual void OnAfterPostMessage(Message message)
        {
            if (message is null || message.Medium == CommunicationMediums.System || !message.HasAttachments)
            {
                return;
            }

            LearnKnowledgesFromPostMessage(message);
            LearnBeliefsFromPostMessage(message);
        }

        /// <summary>
        ///     Learning Knowledges from posted message
        ///     Agent learn about knowledgeId from other agent with KnowledgeBits
        ///     Number of KnowledgeBits is defined by Cognitive.MessageContent.MaximumBitsOfKnowledge
        ///     If Agent must learn only the blocked KnowledgeBit set MaximumBitsOfKnowledge to 1
        /// </summary>
        /// <param name="message"></param>
        public void LearnKnowledgesFromPostMessage(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message.Medium == CommunicationMediums.System || !Cognitive.MessageContent.CanReceiveKnowledge ||
                message.Attachments.KnowledgeBits is null)
            {
                return;
            }

            var communication =
                Environment.WhitePages.Network.NetworkCommunications.TemplateFromChannel(message.Medium);
            LearningModel.Learn(message.Attachments.KnowledgeId,
                message.Attachments.KnowledgeBits, communication.MaxRateLearnable, TimeStep.Step);
            if (message.Medium == CommunicationMediums.Email && HasEmail)
            {
                Email.StoreKnowledge(message.Attachments.KnowledgeId, message.Attachments.KnowledgeBits,
                    communication.MaxRateLearnable, TimeStep.Step);
            }
        }

        /// <summary>
        ///     Learning Beliefs from posted message
        ///     Agent get other agent's beliefs with beliefBits
        ///     Number of beliefBits is defined by Cognitive.MessageContent.MaximumBitsOfBelief
        ///     Depending on influentialness and influenceability, agent can change its belief or not
        /// </summary>
        /// <param name="message"></param>
        public void LearnBeliefsFromPostMessage(Message message)
        {
            if (message is null || !message.HasAttachments)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message.Medium == CommunicationMediums.System || !Cognitive.MessageContent.CanReceiveBeliefs)
            {
                return;
            }

            InfluenceModel.BeInfluenced(message.Attachments.KnowledgeId, message.Attachments.BeliefBits,
                message.Sender, Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel);
        }

        public void MessageOnBeforePost(object sender, MessageEventArgs e)
        {
            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            Environment.Messages.EnQueueWaitingMessage(e.Message, TimeStep.Step);
        }

        /// <summary>
        ///     Post all delayed messages of this current interaction step
        /// </summary>
        public void PostDelayedMessages()
        {
            while (MessageProcessor.NextDelayedMessages(TimeStep.Step) is Message message)
            {
                PostMessage(message);
            }
        }

        #endregion

        #region Send messages

        /// <summary>
        ///     Sends a message to a specific agent, identified by name.
        /// </summary>
        /// <param name="receiverId">The agent that will receive the message</param>
        /// <param name="content">The content of the message</param>
        /// <param name="action">
        ///     A conversation identifier, for the cases when a conversation involves multiple messages
        ///     that refer to the same topic
        /// </param>
        public void Send(AgentId receiverId, MessageAction action, byte content)
        {
            var message = new Message(Id, receiverId, action, content);
            Send(message);
        }

        public void Send(AgentId receiverId, MessageAction action, byte content, object parameter)
        {
            var message = new Message(Id, receiverId, action, content, parameter);
            Send(message);
        }

        public void Send(AgentId receiverId, MessageAction action, byte content, CommunicationMediums mediums)
        {
            var message = new Message(Id, receiverId, action, content, mediums);
            Send(message);
        }

        public void Send(AgentId receiverId, MessageAction action, byte content, MessageAttachments parameter)
        {
            var message = new Message(Id, receiverId, action, content, parameter);
            Send(message);
        }

        public void Send(AgentId receiverId, MessageAction action, byte content, MessageAttachments parameter,
            CommunicationMediums communicationMedium)
        {
            var message = new Message(Id, receiverId, action, content, parameter, communicationMedium);
            Send(message);
        }

        /// <summary>
        ///     Send a message to another agent define by the message.Receiver
        ///     It count in the Mailbox.NumberMessagesPerPeriod
        ///     It will be effectively sent only if IsMessages is above Limits
        /// </summary>
        /// <param name="message"></param>
        public void Send(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!IsMessagesPerPeriodBelowLimit(message.Medium) ||
                !IsMessagesSendPerPeriodBelowLimit(message.Medium))
            {
                return;
            }

            MessageProcessor.IncrementMessagesPerPeriod(message.Medium, true);
            OnBeforeSendMessage(message);
            Environment.SendAgent(message);
            OnAfterSendMessage(message);
        }

        /// <summary>
        ///     Triggered after the message is send
        /// </summary>
        /// <param name="message"></param>
        public virtual void OnAfterSendMessage(Message message)
        {
        }

        public void SendDelayed(Message message, ushort step)
        {
            Environment.SendDelayedMessage(message, step);
        }

        public ushort SendToClass(byte classKey, MessageAction action, byte content)
        {
            var receivers = Environment.WhitePages.FilteredAgentIdsByClassKey(classKey).ToList();

            foreach (var a in receivers.Shuffle())
            {
                Send(a, action, content);
            }

            return (ushort) receivers.Count;
        }

        public void SendToMany(IEnumerable<AgentId> receivers, MessageAction action, byte content)
        {
            if (receivers is null)
            {
                return;
            }

            foreach (var a in receivers.Shuffle())
            {
                Send(a, action, content);
            }
        }

        public void SendToMany(IEnumerable<AgentId> receivers, MessageAction action, byte content, object parameter)
        {
            if (receivers is null)
            {
                return;
            }

            foreach (var a in receivers.Shuffle())
            {
                Send(a, action, content, parameter);
            }
        }

        public void SendToMany(IEnumerable<AgentId> receivers, MessageAction action, byte content,
            MessageAttachments parameter)
        {
            SendToMany(receivers, action, content, parameter, CommunicationMediums.System);
        }

        public void SendToMany(IEnumerable<AgentId> receivers, MessageAction action, byte content,
            MessageAttachments parameter, CommunicationMediums communicationMedium)
        {
            if (receivers is null)
            {
                return;
            }

            foreach (var a in receivers.Shuffle())
            {
                Send(a, action, content, parameter, communicationMedium);
            }
        }

        #endregion

        #region Reply message

        /// <summary>
        ///     Reply to a message from another agent
        ///     It does count in the Mailbox.NumberMessagesPerPeriod
        ///     It doesn't count in the Mailbox.NumberSentPerPeriod
        ///     It will be effectively sent only if IsMessages is above Limits
        /// </summary>
        /// <param name="message"></param>
        /// <param name="delayed"></param>
        /// <param name="delay"></param>
        public void Reply(Message message, bool delayed, ushort delay)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!IsMessagesPerPeriodBelowLimit(message.Medium))
            {
                return;
            }

            MessageProcessor.IncrementMessagesPerPeriod(message.Medium, true);

            if (message.HasAttachments && message.Medium != CommunicationMediums.System)
            {
                var ma = message.Attachments;
                var communication =
                    Environment.WhitePages.Network.NetworkCommunications.TemplateFromChannel(message.Medium);
                ma.KnowledgeBits = KnowledgeModel.FilterKnowledgeToSend(ma.KnowledgeId, ma.KnowledgeBit, communication,
                    TimeStep.Step, out var knowledgeIndexToSend);
                ma.BeliefBits = BeliefsModel.FilterBeliefToSend(ma.KnowledgeId, ma.KnowledgeBit, communication);
                // The agent is asked for his knowledge, so he can't forget it
                if (ma.KnowledgeBits != null)
                {
                    ForgettingModel.UpdateForgettingProcess(ma.KnowledgeId, knowledgeIndexToSend);
                }
            }

            OnBeforeSendMessage(message);
            if (delayed)
            {
                SendDelayed(message, delay);
            }
            else
            {
                Environment.SendAgent(message);
            }

            OnAfterSendMessage(message);
        }

        /// <summary>
        ///     Reply to a message from another agent
        ///     It does count in the Mailbox.NumberMessagesPerPeriod
        ///     It doesn't count in the Mailbox.NumberSentPerPeriod
        ///     It will be effectively sent only if IsMessages is above Limits
        /// </summary>
        /// <param name="message"></param>
        public void Reply(Message message)
        {
            Reply(message, false, 0);
        }

        /// <summary>
        ///     Reply to a message from another agent
        ///     It does count in the Mailbox.NumberMessagesPerPeriod
        ///     It doesn't count in the Mailbox.NumberSentPerPeriod
        ///     It will be effectively sent only if IsMessages is above Limits
        /// </summary>
        /// <param name="message"></param>
        /// <param name="delay"></param>
        public void ReplyDelayed(Message message, ushort delay)
        {
            Reply(message, true, delay);
        }

        /// <summary>
        ///     Sends a message to a specific agent, identified by name.
        /// </summary>
        /// <param name="receiverId">The agent that will receive the message</param>
        /// <param name="content">The content of the message</param>
        /// <param name="action">
        ///     A conversation identifier, for the cases when a conversation involves multiple messages
        ///     that refer to the same topic
        /// </param>
        public void Reply(AgentId receiverId, MessageAction action, byte content)
        {
            var message = new Message(Id, receiverId, action, content);
            Reply(message);
        }

        public void Reply(AgentId receiverId, MessageAction action, byte content, object parameter)
        {
            var message = new Message(Id, receiverId, action, content, parameter);
            Reply(message);
        }

        public void Reply(AgentId receiverId, MessageAction action, byte content, MessageAttachments parameter)
        {
            var message = new Message(Id, receiverId, action, content, parameter);
            Reply(message);
        }

        public void Reply(AgentId receiverId, MessageAction action, byte content, MessageAttachments parameter,
            CommunicationMediums communicationMedium)
        {
            var message = new Message(Id, receiverId, action, content, parameter, communicationMedium);
            Reply(message);
        }

        #endregion

        #region Limit MessagesManager par period

        /// <summary>
        ///     If Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod is set to true,
        ///     check that numberMessagesPerPeriod is below MaximumMessagesPerPeriod
        /// </summary>
        /// <returns>true if numberMessagesPerPeriod is below the maximum, a new message can be send or receive</returns>
        public bool IsMessagesPerPeriodBelowLimit(CommunicationMediums medium)
        {
            // System messages are not concerned by the limits
            if (medium == CommunicationMediums.System)
            {
                return true;
            }

            var noLimit = !Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod &&
                          MessageProcessor.NumberMessagesPerPeriod < byte.MaxValue;
            var limit = Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod &&
                        MessageProcessor.NumberMessagesPerPeriod <
                        Cognitive.InteractionCharacteristics.MaximumMessagesPerPeriod;
            return limit | noLimit;
        }

        /// <summary>
        ///     If Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod is set to true,
        ///     check that NumberPostPerPeriod is below MaximumInitiationsPerPeriod
        /// </summary>
        /// <returns>true if numberMessagesPerPeriod is below the maximum, a new message can be send or receive</returns>
        public bool IsMessagesSendPerPeriodBelowLimit(CommunicationMediums medium)
        {
            // System messages are not concerned by the limits
            if (medium == CommunicationMediums.System)
            {
                return true;
            }

            var noLimit = !Cognitive.InteractionCharacteristics.LimitMessagesSentPerPeriod &&
                          MessageProcessor.NumberSentPerPeriod < byte.MaxValue;
            var limit = Cognitive.InteractionCharacteristics.LimitMessagesSentPerPeriod &&
                        MessageProcessor.NumberSentPerPeriod <
                        Cognitive.InteractionCharacteristics.MaximumMessagesSentPerPeriod;
            return limit | noLimit;
        }

        /// <summary>
        ///     If Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod is set to true,
        ///     check that NumberPostPerPeriod is below MaximumInitiationsPerPeriod
        /// </summary>
        /// <returns>true if numberMessagesPerPeriod is below the maximum, a new message can be send or receive</returns>
        public bool IsMessagesReceivedPerPeriodBelowLimit(CommunicationMediums medium)
        {
            // System messages are not concerned by the limits
            if (medium == CommunicationMediums.System)
            {
                return true;
            }

            var noLimit = !Cognitive.InteractionCharacteristics.LimitReceptionsPerPeriod &&
                          MessageProcessor.NumberReceivedPerPeriod < byte.MaxValue;
            var limit = Cognitive.InteractionCharacteristics.LimitReceptionsPerPeriod &&
                        MessageProcessor.NumberReceivedPerPeriod <
                        Cognitive.InteractionCharacteristics.MaximumReceptionsPerPeriod;
            return limit | noLimit;
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
            SendDelayed(message, TimeStep.Step);
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