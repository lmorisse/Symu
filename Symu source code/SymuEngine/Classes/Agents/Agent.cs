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
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using SymuEngine.Classes.Agents.Models;
using SymuEngine.Classes.Agents.Models.CognitiveArchitecture;
using SymuEngine.Classes.Agents.Models.Templates;
using SymuEngine.Classes.Agents.Models.Templates.Communication;
using SymuEngine.Classes.Blockers;
using SymuEngine.Classes.Task;
using SymuEngine.Classes.Task.Manager;
using SymuEngine.Common;
using SymuEngine.Environment;
using SymuEngine.Messaging.Manager;
using SymuEngine.Messaging.Messages;
using SymuEngine.Repository;
using SymuEngine.Repository.Networks.Databases;
using SymuEngine.Repository.Networks.Knowledges;
using SymuTools;
using SymuTools.Math.ProbabilityDistributions;
using static SymuTools.Constants;

#endregion

namespace SymuEngine.Classes.Agents
{
    /// <summary>
    ///     An abstract base class for agents.
    ///     You must define your own agent derived classes derived
    /// </summary>
    public abstract class Agent
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

        public ForgettingModel ForgettingModel { get; set; }

        protected void CreateAgent(AgentId agentId, SymuEnvironment environment)
        {
            Id = agentId;
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            Environment.AddAgent(this);
            State = AgentState.NotStarted;
            foreach (var database in environment.Organization.Databases.List)
            {
                environment.WhitePages.Network.AddDatabase(Id, database.Id);
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
            Cognitive = new CognitiveArchitecture(Environment.WhitePages.Network, Id,
                Environment.Organization.Models.RandomLevelValue);
            //Apply Cognitive template
            agentTemplate?.Set(Cognitive);
            // Initialize parameters
            Environment.WhitePages.Network.NetworkInfluences.Add(Id,
                Cognitive.InternalCharacteristics.NextInfluenceability(),
                Cognitive.InternalCharacteristics.NextInfluentialness());
            // Learning model
            Environment.Organization.Models.Learning.CopyTo(Cognitive.TasksAndPerformance
                .LearningModel);
            // Forgetting model
            ForgettingModel = new ForgettingModel(Environment.Organization.Models.Forgetting,
                Cognitive.InternalCharacteristics, Environment.Organization.Models.RandomLevelValue,
                Environment.WhitePages.Network.NetworkKnowledges, Id);
        }

        #region Interaction strategy

        /// <summary>
        ///     List of AgentId for new interactions : there is no Active link (difference with GetAgentIdsForInteractions)
        ///     based on the interaction strategy of the interaction patterns :
        ///     Filtered with interactionStrategy and limit with number of new interactions
        /// </summary>
        public IEnumerable<AgentId> GetAgentIdsForNewInteractions()
        {
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
        /// Filter the good number of agents based on Cognitive.InteractionPatterns
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
            Cognitive.Initialize(TimeStep.Step);
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
            Cognitive.TasksAndPerformance.Learn(message.Attachments.KnowledgeId,
                message.Attachments.KnowledgeBits, communication.MaxRateLearnable, Cognitive.InternalCharacteristics,
                TimeStep.Step);
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

            Cognitive.InternalCharacteristics.Learn(message.Attachments.KnowledgeId, message.Attachments.BeliefBits,
                message.Sender);
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

            if (message.HasAttachments)
            {
                var ma = message.Attachments;
                var communication =
                    Environment.WhitePages.Network.NetworkCommunications.TemplateFromChannel(message.Medium);
                ma.KnowledgeBits = FilterKnowledgeToSend(ma.KnowledgeId, ma.KnowledgeBit, communication);
                ma.BeliefBits = FilterBeliefToSend(ma.KnowledgeId, ma.KnowledgeBit, communication);
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

        #region Knowledge and Beliefs

        /// <summary>
        ///     The agent have received a message that ask for knowledge in return
        /// </summary>
        /// <returns>null if he don't have the knowledge or the right</returns>
        /// <returns>a knowledgeBits if he has the knowledge or the right</returns>
        public Bits FilterKnowledgeToSend(ushort knowledgeId, byte knowledgeBit, CommunicationTemplate medium)
        {
            // If can't send knowledge or no knowledge asked
            if (!Cognitive.MessageContent.CanSendKnowledge || knowledgeId == 0 ||
                Cognitive.KnowledgeAndBeliefs.Expertise == null)
            {
                return null;
            }

            if (!Cognitive.KnowledgeAndBeliefs.Expertise.KnowsEnough(knowledgeId, knowledgeBit,
                Cognitive.MessageContent.MinimumKnowledgeToSendPerBit, TimeStep.Step))
            {
                return null;
            }

            var agentKnowledge = Cognitive.KnowledgeAndBeliefs.Expertise.GetKnowledge(knowledgeId);
            // Filter the Knowledge to send, via the good communication medium
            var bitsToSend = Cognitive.MessageContent.GetFilteredKnowledgeToSend(agentKnowledge, knowledgeBit, medium,
                out var knowledgeIndexToSend);

            // The agent is asked for his knowledge, so he can't forget it
            if (knowledgeIndexToSend != null)
            {
                ForgettingModel.UpdateForgettingProcess(knowledgeId, knowledgeIndexToSend);
            }

            return bitsToSend;
        }

        /// <summary>
        ///     The agent have received a message that ask for belief in return
        /// </summary>
        /// <returns>null if he don't have the belief or the right</returns>
        /// <returns>a beliefBits if he has the belief or the right</returns>
        public Bits FilterBeliefToSend(ushort beliefId, byte beliefBit, CommunicationTemplate channel)
        {
            // If don't have belief, can't send belief or no belief asked
            if (!Cognitive.KnowledgeAndBeliefs.HasBelief || !Cognitive.MessageContent.CanSendBeliefs || beliefId == 0)
            {
                return null;
            }

            // intentionally after Cognitive.MessageContent.CanSendKnowledge test
            if (Cognitive.KnowledgeAndBeliefs.Beliefs == null)
            {
                throw new NullReferenceException(nameof(Cognitive.KnowledgeAndBeliefs.Expertise));
            }

            // If Agent don't have the belief, he can't reply
            if (!Cognitive.KnowledgeAndBeliefs.Beliefs.BelievesEnough(beliefId, beliefBit,
                Cognitive.MessageContent.MinimumKnowledgeToSendPerBit))
            {
                return null;
            }

            var agentBelief = Cognitive.KnowledgeAndBeliefs.Beliefs.GetBelief(beliefId);
            // Filter the belief to send, via the good communication channel
            return Cognitive.MessageContent.GetFilteredBeliefToSend(agentBelief, beliefBit, channel);
        }

        #endregion

        #region Act

        /// <summary>
        ///     This is the method that is called when the agent receives a message and is activated.
        ///     When TimeStep.Type is Intraday, messages are treated as tasks and stored in task.Parent attribute
        /// </summary>
        /// <param name="message">The message that the agent has received and should respond to</param>
        public void Act(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            //if (TimeStep.Type == TimeStepType.Intraday && message.Medium != CommunicationMediums.System)
            if (Cognitive.TasksAndPerformance.CanPerformTask && message.Medium != CommunicationMediums.System)
            {
                // Switch message into a task in the task manager
                var communication =
                    Environment.WhitePages.Network.NetworkCommunications.TemplateFromChannel(message.Medium);
                var task = new SymuTask(TimeStep.Step)
                {
                    Type = message.Medium.ToString(),
                    TimeToLive = communication.Cognitive.InternalCharacteristics.TimeToLive,
                    Parent = message,
                    Weight = Environment.WhitePages.Network.NetworkCommunications.TimeSpent(message.Medium, false,
                        Environment.Organization.Models.RandomLevelValue)
                };
                TaskProcessor.Post(task);
            }
            else
            {
                ActMessage(message);
            }
        }

        /// <summary>
        ///     This is where the main logic of the agent should be placed.
        /// </summary>
        /// <param name="message"></param>
        public virtual void ActMessage(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            switch (message.Subject)
            {
                case SymuYellowPages.Stop:
                    State = AgentState.Stopping;
                    break;
                case SymuYellowPages.Subscribe:
                    ActSubscribe(message);
                    break;
                default:
                    ActClassKey(message);
                    break;
            }
        }

        /// <summary>
        ///     Trigger every event before the new step
        ///     Do not send messages, use NextStep for that
        /// </summary>
        public virtual async void PreStep()
        {
            MessageProcessor?.ClearMessagesPerPeriod();
            // Forgetting model
            if (ForgettingModel != null && Cognitive.KnowledgeAndBeliefs.HasKnowledge)
            {
                ForgettingModel.InitializeForgettingProcess();
            }

            // Databases
            if (HasEmail)
            {
                Email.ForgettingProcess(TimeStep.Step);
            }

            _newInteractionCounter = 0;
            HandleStatus();
            // intentionally after Status
            HandleCapacity(true);
            // Task manager
            if (!Cognitive.TasksAndPerformance.CanPerformTask)
            {
                return;
            }

            async Task<bool> ProcessWorkInProgress()
            {
                while (Capacity.HasCapacity && Status != AgentStatus.Offline)
                {
                    try
                    {
                        var task = await TaskProcessor.Receive(TimeStep.Step).ConfigureAwait(false);
                        switch (task.Parent)
                        {
                            case Message message:
                                // When TimeStep.Type is Intraday, messages are treated as tasks and stored in task.Parent attribute
                                // Once a message (as a task) is receive it is treated as a message
                                if (task.IsToDo)
                                {
                                    ActMessage(message);
                                }

                                WorkOnTask(task);
                                break;
                            default:
                                WorkInProgress(task);
                                break;
                        }
                    }
                    catch (Exception exception)
                    {
                        var exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                        exceptionDispatchInfo.Throw();
                    }
                }

                // If we didn't deschedule then run the continuation immediately
                return true;
            }

            await ProcessWorkInProgress().ConfigureAwait(false);

            ActEndOfDay();
        }

        /// <summary>
        ///     Trigger event after the taskManager is started.
        ///     Used by the agent to subscribe to AfterSetTaskDone event
        /// </summary>
        /// <example>TaskManager.AfterSetTaskDone += AfterSetTaskDone;</example>
        public virtual void OnAfterTaskProcessorStart()
        {
        }

        /// <summary>
        ///     Trigger every event after the actual step,
        ///     Do not send messages
        /// </summary>
        public virtual void PostStep()
        {
            if (ForgettingModel != null && Cognitive.KnowledgeAndBeliefs.HasKnowledge)
            {
                ForgettingModel.FinalizeForgettingProcess(TimeStep.Step);
            }
        }

        /// <summary>
        ///     Trigger at the end of day,
        ///     agent can still send message
        /// </summary>
        public virtual void ActEndOfDay()
        {
            SendNewInteractions();
            TaskProcessor?.TasksManager.TasksCheck(TimeStep.Step);
        }

        /// <summary>
        ///     Send new interactions to augment its sphere of interaction if possible
        ///     Depends on Cognitive.InteractionPatterns && Cognitive.InteractionCharacteristics
        /// </summary>
        public void SendNewInteractions()
        {
            var agents = GetAgentIdsForNewInteractions().ToList();
            if (!agents.Any())
            {
                return;
            }

            // Send new interactions
            SendToMany(agents, MessageAction.Ask, SymuYellowPages.Actor, CommunicationMediums.FaceToFace);
        }

        /// <summary>
        ///     Start a weekend, by asking new tasks if agent perform tasks on weekends
        /// </summary>
        public virtual void ActWeekEnd()
        {
            if (!Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds ||
                TaskProcessor.TasksManager.HasReachedTotalMaximumLimit)
            {
                return;
            }

            GetNewTasks();
        }

        public virtual void ActCadence()
        {
        }

        /// <summary>
        ///     Start the working day, by asking new tasks
        /// </summary>
        public virtual void ActWorkingDay()
        {
            if (!Cognitive.TasksAndPerformance.CanPerformTask || TaskProcessor.TasksManager.HasReachedTotalMaximumLimit)
            {
                return;
            }

            GetNewTasks();
        }

        /// <summary>
        ///     Event that occur on friday to end the work week
        /// </summary>
        public virtual void ActEndOfWeek()
        {
        }

        public virtual void ActEndOfMonth()
        {
        }

        public virtual void ActEndOfYear()
        {
        }

        /// <summary>
        ///     Check if agent is performing task today depending on its settings or if agent is active
        /// </summary>
        /// <returns>true if agent is performing task, false if agent is not</returns>
        public bool IsPerformingTask()
        {
            // Agent can be temporary isolated
            var isPerformingTask = !Cognitive.InteractionPatterns.IsIsolated();
            return isPerformingTask && (Cognitive.TasksAndPerformance.CanPerformTask && TimeStep.IsWorkingDay ||
                                        Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds &&
                                        !TimeStep.IsWorkingDay);
        }

        /// <summary>
        ///     Set the Status to available if agent as InitialCapacity, Offline otherwise
        /// </summary>
        public virtual void HandleStatus()
        {
            Status = !Cognitive.InteractionPatterns.IsIsolated() ? AgentStatus.Available : AgentStatus.Offline;
            if (Status != AgentStatus.Offline)
                // Open the agent mailbox with all the waiting messages
            {
                PostDelayedMessages();
            }
        }

        protected virtual void ActClassKey(Message message)
        {
        }

        #endregion

        #region Capacity

        /// <summary>
        ///     Describe the agent capacity
        /// </summary>
        public AgentCapacity Capacity { get; } = new AgentCapacity();

        /// <summary>
        ///     Set the initial capacity for the new step based on SetInitialCapacity, working day,
        ///     By default = Initial capacity if it's a working day, 0 otherwise
        ///     If resetRemainingCapacity set to true, Remaining capacity is reset to Initial Capacity value
        /// </summary>
        public void HandleCapacity(bool resetRemainingCapacity)
        {
            // Intentionally no test on Agent that must be able to perform tasks
            // && Cognitive.TasksAndPerformance.CanPerformTask
            // Example : internet access don't perform task, but is online
            if (IsPerformingTask())
            {
                SetInitialCapacity();
                if (Cognitive.TasksAndPerformance.CanPerformTask)
                {
                    Environment.IterationResult.Capacity += Capacity.Initial;
                }
            }
            else
            {
                Capacity.Initial = 0;
            }

            if (resetRemainingCapacity)
            {
                Capacity.Reset();
            }
        }

        /// <summary>
        ///     Use to set the baseline value of the initial capacity
        /// </summary>
        /// <returns></returns>
        public virtual void SetInitialCapacity()
        {
            Capacity.Initial = 1;
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

        #region Tasks management

        /// <summary>
        ///     Override this method to specify how an agent will get new tasks to complete
        ///     By default, if worker can't perform task or has reached the maximum number of tasks,
        ///     he can't ask for more tasks, just finished the tasks in the taskManager
        /// </summary>
        public virtual void GetNewTasks()
        {
        }
        /// <summary>
        /// Post a task in the TasksProcessor
        /// </summary>
        /// <param name="task"></param>
        /// <remarks>Don't use TaskProcessor.Post directly to handle the OnBeforeTaskPost event</remarks>
        public void Post(SymuTask task)
        {
            OnBeforePostTask(task);
            if (!task.IsBlocked)
            {
                TaskProcessor.Post(task);
            }
        }
        /// <summary>
        /// Post a task in the TasksProcessor
        /// </summary>
        /// <param name="tasks"></param>
        /// <remarks>Don't use TaskProcessor.Post directly to handle the OnBeforeTaskPost event</remarks>
        public void Post(IEnumerable<SymuTask> tasks)
        {
            if (tasks is null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            foreach (var task in tasks)
            {
                OnBeforePostTask(task);
                if (!task.IsBlocked)
                {
                    TaskProcessor.Post(task);
                }
            }
        }

        /// <summary>
        ///     EventHandler triggered before the event TaskProcessor.Post(task)
        ///     By default CheckBlockerBeliefs
        ///     If task must be posted, use task.Blockers
        /// </summary>
        /// <param name="task"></param>
        protected virtual void OnBeforePostTask(SymuTask task)
        {
            CheckBlockerBeliefs(task);
        }

        /// <summary>
        ///     Work on the next task
        /// </summary>
        public void WorkInProgress(SymuTask task)
        {
            if (task == null)
            {
                Status = AgentStatus.Available;
                return;
            }

            // The task may be blocked, try to unlock it
            TryRecoverBlockedTask(task);
            // Agent may discover new blockers
            CheckBlockers(task);
            // Task may have been blocked
            // Capacity may have been used for blockers
            if (!task.IsBlocked && Capacity.HasCapacity)
            {
                WorkOnTask(task);
            }

            if (Capacity.HasCapacity)
                // We start a new loop on the current tasks of the agent
            {
                SwitchingContextModel();
            }
        }

        /// <summary>
        ///     Simulate the work on a specific task
        /// </summary>
        /// <param name="task"></param>
        public virtual float WorkOnTask(SymuTask task)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            float timeSpent;
            if (TimeStep.Type == TimeStepType.Intraday)
            {
                timeSpent = Math.Min(Environment.Organization.Models.Intraday, Capacity.Actual);
            }
            else
            {
                timeSpent = Cognitive.TasksAndPerformance.TasksLimit.LimitSimultaneousTasks
                    // Mono tasking
                    ? Math.Min(task.Weight, Capacity.Actual)
                    // Multi tasking
                    : Math.Min(task.Weight / 2, Capacity.Actual);
            }

            timeSpent = Math.Min(task.WorkToDo, timeSpent);
            task.WorkToDo -= timeSpent;
            if (task.WorkToDo < Tolerance)
            {
                SetTaskDone(task);
            }
            else
            {
                UpdateTask(task);
            }

            // As the agent work on task that requires knowledge, the agent can't forget the associate knowledge today
            foreach (var knowledgeId in task.KnowledgesBits.KnowledgeIds)
            {
                ForgettingModel.UpdateForgettingProcess(knowledgeId, task.KnowledgesBits.GetBits(knowledgeId));
            }

            Capacity.Decrement(timeSpent);
            return timeSpent;
        }

        /// <summary>
        ///     Set the task done in task manager
        /// </summary>
        /// <param name="task"></param>
        public void SetTaskDone(SymuTask task)
        {
            TaskProcessor.PushDone(task);
        }

        /// <summary>
        ///     Update the task as the agent has worked on it, but not complete it
        /// </summary>
        /// <param name="task"></param>
        public void UpdateTask(SymuTask task)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            task.Update(TimeStep.Step);
        }

        /// <summary>
        ///     Launch a recovery for all the blockers
        /// </summary>
        /// <param name="task"></param>
        private void TryRecoverBlockedTask(SymuTask task)
        {
            foreach (var blocker in task.Blockers.FilterBlockers(TimeStep.Step))
            {
                blocker.Update(TimeStep.Step);
                TryRecoverBlocker(task, blocker);
            }
        }

        /// <summary>
        ///     Launch a recovery for the blocker
        /// </summary>
        /// <param name="task"></param>
        /// <param name="blocker"></param>
        protected virtual void TryRecoverBlocker(SymuTask task, Blocker blocker)
        {
        }

        /// <summary>
        ///     Check if there are  blockers today on the task
        /// </summary>
        /// <param name="task"></param>
        private void CheckBlockers(SymuTask task)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (!Environment.Organization.Models.MultipleBlockers && task.IsBlocked)
                // One blocker at a time
            {
                return;
            }

            CheckNewBlockers(task);
        }

        /// <summary>
        ///     Launch a recovery for the blocker
        /// </summary>
        /// <param name="task"></param>
        public virtual void CheckNewBlockers(SymuTask task)
        {
        }
        /// <summary>
        ///     Check Task.KnowledgesBits against Agent.expertise
        /// </summary>
        public void CheckBlockerBeliefs(SymuTask task)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (task.Parent is Message)
            {
                return;
            }

            foreach (var knowledgeId in task.KnowledgesBits.KnowledgeIds)
            {
                CheckBlockerBelief(task, knowledgeId);
            }
        }
        public void CheckBlockerBelief(SymuTask task, ushort knowledgeId)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            var taskBits = task.KnowledgesBits.GetBits(knowledgeId);
            float mandatoryScore = 0;
            float requiredScore = 0;
            byte mandatoryIndex = 0;
            byte requiredIndex = 0;
            Cognitive.KnowledgeAndBeliefs.CheckBelief(knowledgeId, taskBits, ref mandatoryScore, ref requiredScore,
                ref mandatoryIndex, ref requiredIndex);
            CheckBlockerBelief(task, knowledgeId, mandatoryScore, requiredScore, mandatoryIndex, requiredIndex);
        }

        protected virtual void CheckBlockerBelief(SymuTask task, ushort knowledgeId, float mandatoryScore, float requiredScore, byte mandatoryIndex, byte requiredIndex)
        {
        }

        /// <summary>
        ///     Switching context may have an impact on the agent capacity
        /// </summary>
        public virtual void SwitchingContextModel()
        {
        }

        #endregion
    }
}