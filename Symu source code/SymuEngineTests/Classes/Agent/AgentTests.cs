#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Agent;
using SymuEngine.Classes.Agent.Models;
using SymuEngine.Classes.Agent.Models.CognitiveArchitecture;
using SymuEngine.Classes.Agent.Models.Templates.Communication;
using SymuEngine.Classes.Organization;
using SymuEngine.Common;
using SymuEngine.Engine;
using SymuEngine.Messaging.Message;
using SymuEngine.Repository;
using SymuEngine.Repository.Networks.Beliefs;
using SymuEngine.Repository.Networks.Knowledges;
using SymuEngineTests.Helpers;

#endregion

namespace SymuEngineTests.Classes.Agent
{
    [TestClass]
    public class AgentTests
    {
        private const RandomGenerator Model = new RandomGenerator();
        private readonly EmailTemplate _emailTemplate = new EmailTemplate();
        private readonly TestEnvironment _environment = new TestEnvironment();
        private readonly OrganizationEntity _organizationEntity = new OrganizationEntity("1");
        private readonly SimulationEngine _simulation = new SimulationEngine();
        private TestAgent _agent;
        private AgentKnowledge _agentKnowledge;
        private Belief _belief;

        [TestInitialize]
        public void Initialize()
        {
            _environment.SetOrganization(_organizationEntity);
            _simulation.SetEnvironment(_environment);

            _agent = new TestAgent(1, _environment);
            _agent.Cognitive = new CognitiveArchitecture(_environment.WhitePages.Network, _agent.Id, 0)
            {
                KnowledgeAndBeliefs = {HasBelief = true, HasKnowledge = true},
                MessageContent = {CanReceiveBeliefs = true, CanReceiveKnowledge = true}
            };
            _belief = new Belief(1, 1, Model);

            var expertise = new AgentExpertise();
            var knowledge = new Knowledge(1, "1", 1);
            _environment.WhitePages.Network.NetworkKnowledges.AddKnowledge(knowledge);
            _agentKnowledge = new AgentKnowledge(knowledge.Id, new float[] {1}, 0, -1, 0);
            expertise.Add(_agentKnowledge);
            _environment.WhitePages.Network.NetworkKnowledges.Add(_agent.Id, expertise);

            Assert.AreEqual(AgentState.NotStarted, _agent.State);
            _agent.Start();
            Assert.AreEqual(AgentState.Starting, _agent.State);
            _agent.WaitingToStart();
            Assert.AreEqual(AgentState.Started, _agent.State);
        }

        [TestMethod]
        public void StateAgentTests()
        {
            _agent.State = AgentState.Stopping;
            Assert.AreEqual(AgentState.Stopping, _agent.State);
            _environment.ManageAgentsToStop();
            Assert.AreEqual(AgentState.Stopped, _agent.State);
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        [TestMethod]
        public void AgentTest()
        {
            Assert.AreEqual(1, _agent.Id.Key);
            Assert.IsNotNull(_agent.Environment);
            Assert.IsNotNull(_agent.Cognitive);
            Assert.AreEqual(0, _environment.WhitePages.Network.NetworkInfluences.GetInfluenceability(_agent.Id));
            Assert.AreEqual(0, _environment.WhitePages.Network.NetworkInfluences.GetInfluentialness(_agent.Id));
            Assert.AreEqual(AgentState.Started, _agent.State);
        }

        #region message

        [TestMethod]
        public void OnBeforeSendMessageTest()
        {
            var message = new Message
            {
                Medium = CommunicationMediums.Email
            };
            _agent.Capacity.Set(1);
            _agent.OnBeforeSendMessage(message);
            Assert.IsTrue(_agent.Capacity.Actual < 1);
        }

        #endregion

        #region Post message

        /// <summary>
        ///     Online Agent with a IRC/Email message
        /// </summary>
        [TestMethod]
        public void PostTest1()
        {
            _agent.Status = AgentStatus.Available;
            var message = new Message
            {
                Medium = CommunicationMediums.Irc
            };
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(1, _environment.Messages.SentMessagesCount);
            message.Medium = CommunicationMediums.Email;
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(2, _environment.Messages.SentMessagesCount);
        }

        /// <summary>
        ///     Offline Agent with a IRC/Email message
        /// </summary>
        [TestMethod]
        public void PostTest11()
        {
            _agent.Status = AgentStatus.Offline;
            var message = new Message
            {
                Medium = CommunicationMediums.Irc
            };
            _agent.Post(message);
            Assert.AreEqual(1, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(0, _environment.Messages.SentMessagesCount);
            message.Medium = CommunicationMediums.Email;
            _agent.Post(message);
            Assert.AreEqual(2, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(0, _environment.Messages.SentMessagesCount);
        }

        /// <summary>
        ///     Offline Agent with a phone/meeting message
        /// </summary>
        [TestMethod]
        public void PostTest2()
        {
            _agent.Status = AgentStatus.Offline;
            var message = new Message
            {
                Medium = CommunicationMediums.Phone
            };
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(0, _environment.Messages.SentMessagesCount);
            message.Medium = CommunicationMediums.Meeting;
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(0, _environment.Messages.SentMessagesCount);
            //TODO test Missed messages
        }

        /// <summary>
        ///     Online Agent with a phone/meeting message
        /// </summary>
        [TestMethod]
        public void PostTest22()
        {
            _agent.Status = AgentStatus.Available;
            var message = new Message
            {
                Medium = CommunicationMediums.Phone
            };
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(1, _environment.Messages.SentMessagesCount);
            message.Medium = CommunicationMediums.Meeting;
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(2, _environment.Messages.SentMessagesCount);
            //TODO test Missed messages
        }

        [TestMethod]
        public void PostDelayedMessagesTest()
        {
            var message = new Message();
            // Post as a delayed message
            _agent.PostAsADelayedMessage(message, 0);
            Assert.AreEqual(1, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(0, _environment.Messages.SentMessagesCount);
            Assert.AreEqual(0, _environment.Messages.WaitingMessages.Count);
            // Post Delayed messages
            _agent.PostDelayedMessages();
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(1, _environment.Messages.SentMessagesCount);
            Assert.AreEqual(0, _environment.Messages.WaitingMessages.Count);
        }

        /// <summary>
        ///     Without limit messages per period
        /// </summary>
        [TestMethod]
        public void PostMessageTest()
        {
            _agent.Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod = false;
            var message = new Message
            {
                Medium = CommunicationMediums.Email
            };
            _agent.PostMessage(message);
            Assert.AreEqual<uint>(1, _environment.Messages.SentMessagesCount);
            Assert.AreEqual(1, _environment.Messages.LastSentMessages.Count);
            Assert.AreEqual(1, _agent.MessageProcessor.NumberMessagesPerPeriod);
        }

        /// <summary>
        ///     With limit messages per period
        /// </summary>
        [TestMethod]
        public void PostMessageTest1()
        {
            _agent.Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod = true;
            _agent.Cognitive.InteractionCharacteristics.MaximumMessagesPerPeriod = 0;
            var message = new Message
            {
                Medium = CommunicationMediums.Email
            };
            _agent.PostMessage(message);
            Assert.AreEqual<uint>(0, _environment.Messages.SentMessagesCount);
            Assert.AreEqual(0, _environment.Messages.LastSentMessages.Count);
            Assert.AreEqual(1, _agent.MessageProcessor.MissedMessages.Count);
            Assert.AreEqual(0, _agent.MessageProcessor.NumberReceivedPerPeriod);
        }

        /// <summary>
        ///     Message.Medium == system
        /// </summary>
        [TestMethod]
        public void LearnKnowledgesFromPostMessageTest()
        {
            var bit1S = new KnowledgeBits(new float[] {1}, 0, -1);
            var attachments = new MessageAttachments
            {
                KnowledgeId = 1,
                KnowledgeBits = bit1S
            };
            _agent.Cognitive.TasksAndPerformance.LearningModel.On = true;
            _agent.Cognitive.TasksAndPerformance.LearningModel.RateOfAgentsOn = 1;
            _agent.Cognitive.TasksAndPerformance.LearningRate = 1;
            var message = new Message(_agent.Id, _agent.Id, MessageAction.Ask, 0, attachments,
                CommunicationMediums.System);

            _agent.LearnKnowledgesFromPostMessage(message);
            Assert.AreEqual(0, _agent.Cognitive.KnowledgeAndBeliefs.Expertise.GetKnowledge(1).GetKnowledgeSum());
        }

        /// <summary>
        ///     Message.Medium = email
        /// </summary>
        [TestMethod]
        public void LearnKnowledgesFromPostMessageTest1()
        {
            var bit1S = new KnowledgeBits(new float[] {1}, 0, -1);
            var attachments = new MessageAttachments
            {
                KnowledgeId = 1,
                KnowledgeBits = bit1S
            };
            _agent.Cognitive.TasksAndPerformance.LearningModel.On = true;
            _agent.Cognitive.TasksAndPerformance.LearningModel.RateOfAgentsOn = 1;
            _agent.Cognitive.TasksAndPerformance.LearningRate = 1;
            var message = new Message(_agent.Id, _agent.Id, MessageAction.Ask, 0, attachments,
                CommunicationMediums.Email);
            _agent.LearnKnowledgesFromPostMessage(message);
            Assert.AreEqual(1, _agent.Cognitive.KnowledgeAndBeliefs.Expertise.GetKnowledge(1).GetKnowledgeSum());
        }

        [TestMethod]
        public void LearnBeliefsFromPostMessageTest()
        {
            var bit1S = new Bits(new float[] {1}, 0);
            var attachments = new MessageAttachments
            {
                KnowledgeId = 1,
                KnowledgeBits = bit1S,
                BeliefBits = bit1S
            };
            var belief = SetBeliefs();
            var message = new Message(_agent.Id, _agent.Id, MessageAction.Ask, 0, attachments,
                CommunicationMediums.Email);
            _agent.LearnBeliefsFromPostMessage(message);
            Assert.AreEqual(1, _agent.Cognitive.KnowledgeAndBeliefs.Beliefs.GetBelief(belief.Id).GetBeliefSum());
        }

        private Belief SetBeliefs()
        {
            _agent.Cognitive.KnowledgeAndBeliefs.HasBelief = true;
            var belief = new Belief(1, 1, RandomGenerator.RandomBinary);
            _environment.WhitePages.Network.NetworkBeliefs.AddBelief(belief);
            _environment.WhitePages.Network.NetworkBeliefs.Add(_agent.Id, 1);
            _environment.WhitePages.Network.NetworkBeliefs.InitializeBeliefs(_agent.Id, true);
            _environment.WhitePages.Network.NetworkInfluences.Update(_agent.Id, 1, 1);
            return belief;
        }

        private void SetExpertise(KnowledgeBits bit0S)
        {
            _agent.Cognitive.KnowledgeAndBeliefs.HasKnowledge = true;
            // Knowledge
            var knowledge = new Knowledge(1, "1", 1);
            _environment.WhitePages.Network.NetworkKnowledges.AddKnowledge(knowledge);
            var agentExpertise = new AgentExpertise();
            var agentKnowledge = new AgentKnowledge(knowledge.Id, bit0S);
            agentExpertise.Add(agentKnowledge);
            _environment.WhitePages.Network.NetworkKnowledges.Add(_agent.Id, agentExpertise);
        }

        #endregion

        #region System message

        /// <summary>
        ///     Stopped Agent with a system message
        /// </summary>
        [TestMethod]
        public void PostTest()
        {
            _agent.State = AgentState.Stopped;
            var message = new Message();
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(0, _environment.Messages.SentMessagesCount);
        }

        /// <summary>
        ///     OnLine Agent with stopped agent
        /// </summary>
        [TestMethod]
        public void PostTest4()
        {
            _agent.State = AgentState.NotStarted;
            var message = new Message();
            _agent.Post(message);
            Assert.AreEqual(1, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(0, _environment.Messages.SentMessagesCount);

            _agent.State = AgentState.Starting;
            _agent.Post(message);
            Assert.AreEqual(2, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(0, _environment.Messages.SentMessagesCount);
        }

        /// <summary>
        ///     OnLine Agent with Started/stopping agent
        /// </summary>
        [TestMethod]
        public void PostTest5()
        {
            _agent.State = AgentState.Started;
            var message = new Message();
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual((uint) 1, _environment.Messages.SentMessagesCount);

            _agent.State = AgentState.Stopping;
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual((uint) 2, _environment.Messages.SentMessagesCount);
        }

        #endregion

        #region Send message

        /// <summary>
        ///     Limit Off
        /// </summary>
        [TestMethod]
        public void IsMessagesPerPeriodAboveLimitTest()
        {
            _agent.Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod = false;
            _agent.Cognitive.InteractionCharacteristics.MaximumMessagesPerPeriod = 0;
            _agent.MessageProcessor.NumberMessagesPerPeriod = 1;
            Assert.IsTrue(_agent.IsMessagesPerPeriodBelowLimit(CommunicationMediums.Email));
            Assert.IsTrue(_agent.IsMessagesPerPeriodBelowLimit(CommunicationMediums.System));
            // Can't exceed byte max value
            _agent.MessageProcessor.NumberMessagesPerPeriod = byte.MaxValue;
            Assert.IsFalse(_agent.IsMessagesPerPeriodBelowLimit(CommunicationMediums.Email));
        }

        /// <summary>
        ///     Limit on
        ///     With maximum of 0
        /// </summary>
        [TestMethod]
        public void IsMessagesPerPeriodAboveLimitTest1()
        {
            _agent.Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod = true;
            _agent.MessageProcessor.NumberMessagesPerPeriod = 1;
            Assert.IsFalse(_agent.IsMessagesPerPeriodBelowLimit(CommunicationMediums.Email));
        }

        /// <summary>
        ///     Limit on
        ///     With maximum of 1
        /// </summary>
        [TestMethod]
        public void IsMessagesPerPeriodAboveLimitTest2()
        {
            _agent.Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod = true;
            _agent.MessageProcessor.NumberMessagesPerPeriod = 1;
            _agent.Cognitive.InteractionCharacteristics.MaximumMessagesPerPeriod = 1;
            Assert.IsFalse(_agent.IsMessagesPerPeriodBelowLimit(CommunicationMediums.Email));
        }

        /// <summary>
        ///     Limit on
        ///     With maximum > 1
        /// </summary>
        [TestMethod]
        public void IsMessagesPerPeriodAboveLimitTest3()
        {
            _agent.Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod = true;
            _agent.MessageProcessor.NumberMessagesPerPeriod = 1;
            _agent.Cognitive.InteractionCharacteristics.MaximumMessagesPerPeriod = 2;
            Assert.IsTrue(_agent.IsMessagesPerPeriodBelowLimit(CommunicationMediums.Email));
            Assert.IsTrue(_agent.IsMessagesPerPeriodBelowLimit(CommunicationMediums.System));
        }

        /// <summary>
        ///     Limit Off
        /// </summary>
        [TestMethod]
        public void IsMessagesSendPerPeriodAboveLimitTest()
        {
            _agent.Cognitive.InteractionCharacteristics.LimitMessagesSentPerPeriod = false;
            _agent.Cognitive.InteractionCharacteristics.MaximumMessagesSentPerPeriod = 0;
            _agent.MessageProcessor.NumberSentPerPeriod = 1;
            Assert.IsTrue(_agent.IsMessagesSendPerPeriodBelowLimit(CommunicationMediums.Email));
            Assert.IsTrue(_agent.IsMessagesSendPerPeriodBelowLimit(CommunicationMediums.System));
            // Can't exceed byte max value
            _agent.MessageProcessor.NumberSentPerPeriod = byte.MaxValue;
            Assert.IsFalse(_agent.IsMessagesSendPerPeriodBelowLimit(CommunicationMediums.Email));
        }

        /// <summary>
        ///     Limit ON
        ///     With maximum of 0
        /// </summary>
        [TestMethod]
        public void IsMessagesSendPerPeriodAboveLimitTest1()
        {
            _agent.Cognitive.InteractionCharacteristics.LimitMessagesSentPerPeriod = true;
            _agent.MessageProcessor.NumberSentPerPeriod = 1;
            Assert.IsFalse(_agent.IsMessagesSendPerPeriodBelowLimit(CommunicationMediums.Email));
        }

        /// <summary>
        ///     Limit ON
        ///     With maximum of 1
        /// </summary>
        [TestMethod]
        public void IsMessagesSendPerPeriodAboveLimitTest2()
        {
            _agent.Cognitive.InteractionCharacteristics.LimitMessagesSentPerPeriod = true;
            _agent.MessageProcessor.NumberSentPerPeriod = 1;
            _agent.Cognitive.InteractionCharacteristics.MaximumMessagesSentPerPeriod = 1;
            Assert.IsFalse(_agent.IsMessagesSendPerPeriodBelowLimit(CommunicationMediums.Email));
        }

        /// <summary>
        ///     Limit ON
        ///     With maximum > 1
        /// </summary>
        [TestMethod]
        public void IsMessagesSendPerPeriodAboveLimitTest3()
        {
            _agent.Cognitive.InteractionCharacteristics.LimitMessagesSentPerPeriod = true;
            _agent.MessageProcessor.NumberSentPerPeriod = 1;
            _agent.Cognitive.InteractionCharacteristics.MaximumMessagesSentPerPeriod = 1;
            _agent.Cognitive.InteractionCharacteristics.MaximumMessagesSentPerPeriod = 2;
            Assert.IsTrue(_agent.IsMessagesSendPerPeriodBelowLimit(CommunicationMediums.Email));
            Assert.IsTrue(_agent.IsMessagesSendPerPeriodBelowLimit(CommunicationMediums.System));
        }

        /// <summary>
        ///     Limit OFF
        /// </summary>
        [TestMethod]
        public void IsMessagesReceivedPerPeriodAboveLimitTest()
        {
            _agent.Cognitive.InteractionCharacteristics.LimitReceptionsPerPeriod = false;
            _agent.Cognitive.InteractionCharacteristics.MaximumReceptionsPerPeriod = 0;
            _agent.MessageProcessor.NumberReceivedPerPeriod = 1;
            Assert.IsTrue(_agent.IsMessagesReceivedPerPeriodBelowLimit(CommunicationMediums.Email));
            Assert.IsTrue(_agent.IsMessagesReceivedPerPeriodBelowLimit(CommunicationMediums.System));
            // Can't exceed byte max value
            _agent.MessageProcessor.NumberReceivedPerPeriod = byte.MaxValue;
            Assert.IsFalse(_agent.IsMessagesReceivedPerPeriodBelowLimit(CommunicationMediums.Email));
        }

        /// <summary>
        ///     Limit ON
        ///     With maximum of 0
        /// </summary>
        [TestMethod]
        public void IsMessagesReceivedPerPeriodAboveLimitTest1()
        {
            _agent.Cognitive.InteractionCharacteristics.LimitReceptionsPerPeriod = true;
            _agent.MessageProcessor.NumberReceivedPerPeriod = 1;
            Assert.IsFalse(_agent.IsMessagesReceivedPerPeriodBelowLimit(CommunicationMediums.Email));
        }

        /// <summary>
        ///     Limit ON
        ///     With maximum of 1
        /// </summary>
        [TestMethod]
        public void IsMessagesReceivedPerPeriodAboveLimitTest2()
        {
            _agent.Cognitive.InteractionCharacteristics.LimitReceptionsPerPeriod = true;
            _agent.MessageProcessor.NumberReceivedPerPeriod = 1;
            _agent.Cognitive.InteractionCharacteristics.MaximumReceptionsPerPeriod = 1;
            Assert.IsFalse(_agent.IsMessagesReceivedPerPeriodBelowLimit(CommunicationMediums.Email));
        }

        /// <summary>
        ///     Limit ON
        ///     With maximum > 1
        /// </summary>
        [TestMethod]
        public void IsMessagesReceivedPerPeriodAboveLimitTest3()
        {
            _agent.Cognitive.InteractionCharacteristics.LimitReceptionsPerPeriod = true;
            _agent.MessageProcessor.NumberReceivedPerPeriod = 1;
            _agent.Cognitive.InteractionCharacteristics.MaximumReceptionsPerPeriod = 2;
            Assert.IsTrue(_agent.IsMessagesReceivedPerPeriodBelowLimit(CommunicationMediums.Email));
            Assert.IsTrue(_agent.IsMessagesReceivedPerPeriodBelowLimit(CommunicationMediums.System));
        }

        /// <summary>
        ///     Without limit messages per period
        /// </summary>
        [TestMethod]
        public void SendAMessageTests()
        {
            _agent.Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod = false;
            var agent2 = new TestAgent(2, _environment);
            agent2.Start();
            agent2.WaitingToStart();
            var message = new Message(_agent.Id, agent2.Id, MessageAction.Add, 0)
            {
                Medium = CommunicationMediums.Email
            };
            _agent.Send(message);
            _environment.Messages.WaitingToClearAllMessages();
            //environment trace the message
            Assert.IsTrue(_environment.Messages.MessagesReceivedByAgent(0, agent2.Id).Any());
            Assert.IsTrue(_environment.Messages.MessagesSentByAgent(0, _agent.Id).Any());
            Assert.IsTrue(_environment.Messages.LastSentMessages.Any);
            // Agent1
            Assert.AreEqual(1, _agent.MessageProcessor.NumberMessagesPerPeriod);
        }

        /// <summary>
        ///     With limit messages per period
        /// </summary>
        [TestMethod]
        public void SendAMessageTests1()
        {
            _agent.Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod = true;
            _agent.Cognitive.InteractionCharacteristics.MaximumMessagesPerPeriod = 0;
            var agent2 = new TestAgent(2, _environment);
            agent2.Start();
            agent2.WaitingToStart();
            var message = new Message(_agent.Id, agent2.Id, MessageAction.Add, 0)
            {
                Medium = CommunicationMediums.Email
            };
            _agent.Send(message);
            _environment.Messages.WaitingToClearAllMessages();
            //environment trace the message
            Assert.IsNull(_environment.Messages.MessagesReceivedByAgent(0, agent2.Id));
            Assert.IsNull(_environment.Messages.MessagesSentByAgent(0, _agent.Id));
            Assert.IsFalse(_environment.Messages.LastSentMessages.Any);
            // Agent1
            Assert.AreEqual(1, _agent.MessageProcessor.NumberMessagesPerPeriod);
        }

        #endregion

        #region reply

        [TestMethod]
        public void ReplyTest()
        {
            var knowledge = new Knowledge(1, "1", 1);
            var attachments = new MessageAttachments
            {
                KnowledgeId = knowledge.Id,
                KnowledgeBit = 0
            };
            var message = new Message(_agent.Id, _agent.Id, MessageAction.Ask, 0, attachments)
            {
                Medium = CommunicationMediums.Email
            };
            // Knowledge
            _agent.Cognitive.MessageContent.CanSendKnowledge = true;
            var bits = new KnowledgeBits(new float[] {1}, 0, -1);
            SetExpertise(bits);
            _environment.WhitePages.Network.NetworkKnowledges.GetAgentKnowledge(_agent.Id, knowledge.Id)
                .SetKnowledgeBit(0, 1, 0);
            // Belief
            _agent.Cognitive.MessageContent.CanSendBeliefs = true;
            var belief = SetBeliefs();
            _environment.WhitePages.Network.NetworkBeliefs.GetAgentBelief(_agent.Id, belief.Id).BeliefBits.SetBit(0, 1);
            _agent.Reply(message);

            Assert.AreEqual(1, _agent.MessageProcessor.NumberSentPerPeriod);
            var messageSent = _environment.Messages.LastSentMessages.SentByAgent(0, _agent.Id).Last();
            Assert.IsNotNull(messageSent);
            Assert.IsNotNull(messageSent.Attachments.KnowledgeBits);
            Assert.IsNotNull(messageSent.Attachments.BeliefBits);
        }

        [TestMethod]
        public void ReplyDelayedTest()
        {
            var knowledge = new Knowledge(1, "1", 1);
            var attachments = new MessageAttachments
            {
                KnowledgeId = knowledge.Id,
                KnowledgeBit = 0
            };
            var message = new Message(_agent.Id, _agent.Id, MessageAction.Ask, 0, attachments)
            {
                Medium = CommunicationMediums.Email
            };
            // Knowledge
            _agent.Cognitive.MessageContent.CanSendKnowledge = true;
            var bits = new KnowledgeBits(new float[] {1}, 0, -1);
            SetExpertise(bits);
            _environment.WhitePages.Network.NetworkKnowledges.GetAgentKnowledge(_agent.Id, knowledge.Id)
                .SetKnowledgeBit(0, 1, 0);
            // Belief
            _agent.Cognitive.MessageContent.CanSendBeliefs = true;
            var belief = SetBeliefs();
            _environment.WhitePages.Network.NetworkBeliefs.GetAgentBelief(_agent.Id, belief.Id).BeliefBits.SetBit(0, 1);
            _agent.ReplyDelayed(message, 0);

            Assert.AreEqual(1, _agent.MessageProcessor.NumberSentPerPeriod);
            var messageSent = _environment.Messages.DelayedMessages.Last(0);
            Assert.IsNotNull(messageSent);
            Assert.IsNotNull(messageSent.Attachments.KnowledgeBits);
            Assert.IsNotNull(messageSent.Attachments.BeliefBits);
        }

        #endregion

        #region Act

        [TestMethod]
        public void AskPreStepTest()
        {
            _environment.TimeStep.Step = 0;
            _agent.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _agent.Cognitive.InteractionPatterns.IsolationIsRandom = false;
            _agent.MessageProcessor.IncrementMessagesPerPeriod(CommunicationMediums.Email, true);
            _agent.Cognitive.InternalCharacteristics.ForgettingMean = 1;
            _agent.Start();
            _agent.PreStep();
            // Status test
            Assert.AreEqual(AgentStatus.Available, _agent.Status);
            // Capacity test
            Assert.AreEqual(1, _agent.Capacity.Initial);
            // ClearMessagesPerPeriod test
            Assert.AreEqual(0, _agent.MessageProcessor.NumberMessagesPerPeriod);
            Assert.AreEqual(0, _agent.MessageProcessor.NumberSentPerPeriod);
        }

        /// <summary>
        ///     Focus ForgettingModel
        ///     Model Off
        /// </summary>
        [TestMethod]
        public void AskPreStepTest1()
        {
            _agent.Start();
            _agent.ForgettingModel = new ForgettingModel(new ModelEntity(), _agent.Cognitive.InternalCharacteristics, 0,
                _environment.WhitePages.Network.NetworkKnowledges, _agent.Id)
            {
                On = false
            };
            var expertise = new AgentExpertise();
            var agentKnowledge = new AgentKnowledge(1, new float[] {1}, 0, -1, 0);
            expertise.Add(agentKnowledge);
            _environment.WhitePages.Network.NetworkKnowledges.Add(_agent.Id, expertise);
            _agent.PreStep();
            Assert.AreEqual(0, _agent.ForgettingModel.ForgettingExpertise.Count);
        }

        /// <summary>
        ///     Focus ForgettingModel
        ///     Model On
        /// </summary>
        [TestMethod]
        public void AskPreStepTest2()
        {
            _agent.Start();
            _agent.Cognitive.InternalCharacteristics.ForgettingMean = 1;
            _agent.ForgettingModel = new ForgettingModel(new ModelEntity(), _agent.Cognitive.InternalCharacteristics, 0,
                _environment.WhitePages.Network.NetworkKnowledges, _agent.Id)
            {
                On = true
            };
            var expertise = new AgentExpertise();
            var agentKnowledge = new AgentKnowledge(1, new float[] {1}, 0, -1, 0);
            expertise.Add(agentKnowledge);
            _environment.WhitePages.Network.NetworkKnowledges.Add(_agent.Id, expertise);
            _agent.PreStep();
            Assert.AreEqual(1, _agent.ForgettingModel.ForgettingExpertise.Count);
        }

        #endregion

        #region subscription

        /// <summary>
        ///     One subscription
        /// </summary>
        [TestMethod]
        public void RemoveSubscribeTest()
        {
            _agent.Start();
            _agent.MessageProcessor.Subscriptions.Subscribe(_agent.Id, 1);
            var message = new Message(_agent.Id, _agent.Id, MessageAction.Remove, SymuYellowPages.subscribe);
            _agent.RemoveSubscribe(message);
            Assert.AreEqual(0, _agent.MessageProcessor.Subscriptions.SubscribersCount(1));
        }

        /// <summary>
        ///     Multiple subscriptions
        /// </summary>
        [TestMethod]
        public void RemoveSubscribeTest1()
        {
            _agent.Start();
            _agent.MessageProcessor.Subscriptions.Subscribe(_agent.Id, 1);
            _agent.MessageProcessor.Subscriptions.Subscribe(_agent.Id, 2);
            _agent.MessageProcessor.Subscriptions.Subscribe(_agent.Id, 3);
            var message = new Message(_agent.Id, _agent.Id, MessageAction.Remove, SymuYellowPages.subscribe);
            _agent.RemoveSubscribe(message);
            Assert.AreEqual(0, _agent.MessageProcessor.Subscriptions.SubscribersCount(1));
            Assert.AreEqual(0, _agent.MessageProcessor.Subscriptions.SubscribersCount(2));
            Assert.AreEqual(0, _agent.MessageProcessor.Subscriptions.SubscribersCount(3));
        }

        [TestMethod]
        public void AddSubscribeTest()
        {
            _agent.Start();
            var attachments = new MessageAttachments();
            attachments.Add((byte) 1);
            attachments.Add((byte) 2);
            var message = new Message(_agent.Id, _agent.Id, MessageAction.Add, SymuYellowPages.subscribe, attachments);
            _agent.AddSubscribe(message);
            Assert.AreEqual(1, _agent.MessageProcessor.Subscriptions.SubscribersCount(1));
            Assert.AreEqual(1, _agent.MessageProcessor.Subscriptions.SubscribersCount(2));
            Assert.AreEqual(0, _agent.MessageProcessor.Subscriptions.SubscribersCount(0));
        }

        #endregion

        #region Knowledge

        /// <summary>
        ///     Passing test
        ///     Can't send belief
        /// </summary>
        [TestMethod]
        public void GetFilteredKnowledgeToSendTest()
        {
            _agent.Cognitive.MessageContent.CanSendKnowledge = false;
            Assert.IsNull(_agent.FilterKnowledgeToSend(1, 0, _emailTemplate));
        }

        /// <summary>
        ///     Passing test
        ///     no belief asked
        /// </summary>
        [TestMethod]
        public void GetFilteredKnowledgeToSendTest1()
        {
            _agent.Cognitive.MessageContent.CanSendKnowledge = true;
            Assert.IsNull(_agent.FilterKnowledgeToSend(0, 0, _emailTemplate));
        }

        /// <summary>
        ///     Passing test
        ///     don't BelievesEnough
        /// </summary>
        [TestMethod]
        public void GetFilteredKnowledgeToSendTest2()
        {
            _agent.Cognitive.MessageContent.CanSendKnowledge = true;
            _agent.Cognitive.MessageContent.MinimumKnowledgeToSendPerBit = 2;
            Assert.IsNull(_agent.FilterKnowledgeToSend(_agentKnowledge.KnowledgeId, 0, _emailTemplate));
        }

        /// <summary>
        ///     Passing test
        ///     enough belief
        /// </summary>
        [TestMethod]
        public void GetFilteredKnowledgeToSendTest3()
        {
            _agent.Cognitive.MessageContent.CanSendKnowledge = true;
            _agent.Cognitive.MessageContent.MinimumKnowledgeToSendPerBit = 0;
            _agentKnowledge.SetKnowledgeBit(0, 1, 0);
            var bits = _agent.FilterKnowledgeToSend(_agentKnowledge.KnowledgeId, 0, _emailTemplate);
            Assert.IsNotNull(bits);
            Assert.AreEqual(1, bits.GetSum());
        }

        #endregion

        #region Capacity management

        /// <summary>
        ///     Non working day
        /// </summary>
        [TestMethod]
        public void NonPassingHandleCapacityTest()
        {
            _environment.TimeStep.Step = 5;
            _agent.HandleCapacity(true);
            Assert.AreEqual(0, _agent.Capacity.Initial);
            Assert.AreEqual(0, _agent.Capacity.Actual);
        }

        /// <summary>
        ///     Can't perform task => no influence on the Capacity
        /// </summary>
        [TestMethod]
        public void PassingHandleCapacityTest1()
        {
            _environment.TimeStep.Step = 0;
            _agent.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _agent.HandleCapacity(true);
            Assert.AreEqual(1, _agent.Capacity.Initial);
            Assert.AreEqual(1, _agent.Capacity.Actual);
        }

        /// <summary>
        ///     Isolated
        /// </summary>
        [TestMethod]
        public void NonPassingHandleCapacityTest2()
        {
            _environment.TimeStep.Step = 0;
            _agent.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _agent.Cognitive.InteractionPatterns.IsolationIsRandom = true;
            _agent.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Always;
            _agent.HandleCapacity(true);
            Assert.AreEqual(0, _agent.Capacity.Initial);
            Assert.AreEqual(0, _agent.Capacity.Actual);
        }

        /// <summary>
        ///     Passing tests
        /// </summary>
        [TestMethod]
        public void HandleCapacityTest1()
        {
            _environment.TimeStep.Step = 0;
            _agent.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _agent.Cognitive.InteractionPatterns.IsolationIsRandom = false;
            _agent.HandleCapacity(true);
            Assert.AreEqual(1, _agent.Capacity.Initial);
            Assert.AreEqual(1, _agent.Capacity.Actual);
        }

        /// <summary>
        ///     No Reset Remaining Capacity
        /// </summary>
        [TestMethod]
        public void HandleCapacityTest2()
        {
            _environment.TimeStep.Step = 0;
            _agent.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _agent.Cognitive.InteractionPatterns.IsolationIsRandom = false;
            _agent.HandleCapacity(false);
            Assert.AreEqual(1, _agent.Capacity.Initial);
            Assert.AreEqual(0, _agent.Capacity.Actual);
        }

        #endregion

        #region Belief

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void NullGetFilteredBeliefToSendTest()
        {
            Assert.IsNull(_agent.FilterBeliefToSend(1, 0, _emailTemplate));
            _agent.Cognitive.MessageContent.CanSendBeliefs = true;
            Assert.IsNull(_agent.FilterBeliefToSend(1, 0, _emailTemplate));
        }

        /// <summary>
        ///     Passing test
        ///     Can't send belief
        /// </summary>
        [TestMethod]
        public void GetFilteredBeliefToSendTest()
        {
            _environment.WhitePages.Network.NetworkBeliefs.Add(_agent.Id, 1);
            _agent.Cognitive.MessageContent.CanSendBeliefs = false;
            Assert.IsNull(_agent.FilterBeliefToSend(1, 0, _emailTemplate));
        }

        /// <summary>
        ///     Passing test
        ///     no belief asked
        /// </summary>
        [TestMethod]
        public void GetFilteredBeliefToSendTest1()
        {
            _environment.WhitePages.Network.NetworkBeliefs.Add(_agent.Id, 1);
            _agent.Cognitive.MessageContent.CanSendBeliefs = true;
            Assert.IsNull(_agent.FilterBeliefToSend(0, 0, _emailTemplate));
        }

        /// <summary>
        ///     Passing test
        ///     don't BelievesEnough
        /// </summary>
        [TestMethod]
        public void GetFilteredBeliefToSendTest2()
        {
            _environment.WhitePages.Network.NetworkBeliefs.AddBelief(_belief);
            _environment.WhitePages.Network.NetworkBeliefs.Add(_agent.Id, _belief.Id);
            _environment.WhitePages.Network.NetworkBeliefs.GetAgentBelief(_agent.Id, _belief.Id)
                .InitializeBeliefBits(Model, 1, true);
            _agent.Cognitive.MessageContent.CanSendBeliefs = true;
            Assert.IsNull(_agent.FilterBeliefToSend(_belief.Id, 0, _emailTemplate));
        }

        /// <summary>
        ///     Passing test
        ///     MinimumBeliefToSendPerBit too high
        /// </summary>
        [TestMethod]
        public void GetFilteredBeliefToSendTest3()
        {
            _environment.WhitePages.Network.NetworkBeliefs.AddBelief(_belief);
            _environment.WhitePages.Network.NetworkBeliefs.Add(_agent.Id, _belief.Id);
            _environment.WhitePages.Network.NetworkBeliefs.GetAgentBelief(_agent.Id, _belief.Id)
                .InitializeBeliefBits(Model, 1, false);
            _agent.Cognitive.MessageContent.CanSendBeliefs = true;
            _agent.Cognitive.MessageContent.MinimumBeliefToSendPerBit = 2;
            var bits = _agent.FilterBeliefToSend(1, 0, _emailTemplate);
            Assert.IsNull(bits);
        }

        /// <summary>
        ///     Passing test
        ///     enough belief
        /// </summary>
        [TestMethod]
        public void GetFilteredBeliefToSendTest4()
        {
            _environment.WhitePages.Network.NetworkBeliefs.AddBelief(_belief);
            _environment.WhitePages.Network.NetworkBeliefs.Add(_agent.Id, _belief.Id);
            _environment.WhitePages.Network.NetworkBeliefs.GetAgentBelief(_agent.Id, _belief.Id)
                .InitializeBeliefBits(Model, 1, false);
            _environment.WhitePages.Network.NetworkBeliefs.GetAgentBelief(_agent.Id, _belief.Id).BeliefBits
                .SetBit(0, 1);
            _agent.Cognitive.MessageContent.CanSendBeliefs = true;
            _agent.Cognitive.MessageContent.MinimumBeliefToSendPerBit = 0;
            var bits = _agent.FilterBeliefToSend(1, 0, _emailTemplate);
            Assert.IsNotNull(bits);
            Assert.AreEqual(1, bits.GetSum());
        }

        #endregion

        #region Status

        /// <summary>
        ///     Initial capacity = 0 - nothing arrived
        /// </summary>
        [TestMethod]
        public void HandleStatusTest()
        {
            _agent.Capacity.Initial = 0;
            _agent.HandleStatus();
            Assert.AreEqual(AgentStatus.Offline, _agent.Status);
        }

        /// <summary>
        ///     Initial capacity = 0 - nothing arrived
        /// </summary>
        [TestMethod]
        public void HandleStatusTest1()
        {
            _agent.Start();
            _agent.Capacity.Initial = 1;
            _agent.HandleStatus();
            Assert.AreEqual(AgentStatus.Available, _agent.Status);
        }

        /// <summary>
        ///     Initial capacity >0 + delayed Post messages
        /// </summary>
        [TestMethod]
        public void HandleStatusTest2()
        {
            _agent.Start();
            _agent.WaitingToStart();
            _agent.Capacity.Initial = 1;
            var message = new Message
            {
                Sender = _agent.Id,
                Receiver = _agent.Id,
                Medium = CommunicationMediums.System
            };
            _agent.MessageProcessor.DelayedMessages.Enqueue(message, 0);
            _agent.HandleStatus();
            Assert.IsTrue(_environment.Messages.LastSentMessages.Count >= 1);
            //Assert.IsTrue(_environment.Messages.CheckMessages);
        }

        #endregion
    }
}