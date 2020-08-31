#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Common;
using Symu.Common.Interfaces.Agent;
using Symu.Environment;
using Symu.Messaging.Manager;
using Symu.Messaging.Messages;
using Symu.Repository;

#endregion

namespace Symu.Classes.Agents
{
    public interface IAgent
    {
        IAgentId AgentId {get;
            set;
        }
    }
    /// <summary>
    ///     An abstract base class for agents.
    ///     You must define your own agent derived classes derived
    /// </summary>
    public abstract partial class ReactiveAgent: IAgent
    {
        /// <summary>
        ///     constructor for generic new()
        ///     Use with CreateAgent method
        /// </summary>
        protected ReactiveAgent()
        {
        }

        /// <summary>
        ///     Constructor with standard agent template
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="environment"></param>
        protected ReactiveAgent(IAgentId agentId, SymuEnvironment environment)
        {
            AgentId = agentId;
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            Environment.AddAgent(this);
            State = AgentState.NotStarted;
            Created = Schedule.Step;
        }

        /// <summary>
        ///     Day of creation of the  agent
        /// </summary>
        public ushort Created { get; protected set; }

        /// <summary>
        ///     Day of stopped of the agent
        /// </summary>
        public ushort Stopped { get; private set; }

        /// <summary>
        ///     The name of the agent. Each agent must have a unique name in its environment.
        ///     Most operations are performed using agent names rather than agent objects.
        /// </summary>
        public IAgentId AgentId { get; set; }

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
        public SymuEnvironment Environment { get; protected set; }

        protected Schedule Schedule => Environment.Schedule;

        /// <summary>
        /// Initialize the agent models
        /// </summary>
        /// <remarks>Should be called after the constructor
        /// Use the factory method CreateInstance instead of the constructor to call Initialize implicitly</remarks>
        protected virtual void Initialize()
        {
            InitializeModels();
            SetModels();
        }

        #region Start/stop

        /// <summary>
        ///     This method is called right after Start, before any messages have been received. It is similar to the constructor
        ///     of the class, but it should be used for agent-related logic, e.g. for sending initial message(s).
        ///     Send Delayed message to the Schedule.STep to be sure the receiver exists and its started
        /// </summary>
        public virtual void BeforeStart()
        {
            // Messaging initializing
            while (MessageProcessor is null)
            {
                //Sometimes Messaging is still null 
                //Just wait for Messaging to be initialized
            }

            MessageProcessor.OnBeforePostEvent += MessageOnBeforePost;
        }

        /// <summary>
        ///     Set the state of the agent to Stopping so that the agent will be stopped at the end of this step
        /// </summary>
        public void Stop()
        {
            State = AgentState.Stopping;
        }
        public virtual void Dispose()
        {
            MessageProcessor?.Dispose();
        }

        /// <summary>
        ///     Use to trigger an event before the agent is stopped
        ///     Be sure to call base.BeforeStop() at the end of the override method
        ///     Don't remove items from WhitePages.Network, it is automatically done by WhitePages.RemoveAgent
        /// </summary>
        public virtual void BeforeStop()
        {
            State = AgentState.Stopped;
            Stopped = Schedule.Step;
        }


        /// <summary>
        ///     Customize the models of the agent
        ///     After setting the Agent basics models
        /// </summary>
        public virtual void SetModels()
        {
        }
        protected virtual void InitializeModels()
        {
        }

        protected virtual void FinalizeModels()
        {
        }

        /// <summary>
        ///     Starts the agent execution, after it has been created.
        ///     First, the Setup method is called, and then the Act method is automatically called when the agent receives a
        ///     message.
        /// </summary>
        /// <remarks>Synchronize the code with CognitiveAgent.Start</remarks>
        public virtual void Start()
        {
            State = AgentState.Starting;
            if (Environment == null)
            {
                throw new Exception("Environment is null in agent " + AgentId.Id + " (ConcurrentAgent.Start)");
            }

            //now in CreateAgent
            //InitializeModels();
            //SetModels();
            FinalizeModels();

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