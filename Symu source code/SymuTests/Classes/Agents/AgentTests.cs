#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Classes.Blockers;
using Symu.Classes.Murphies;
using Symu.Classes.Organization;
using Symu.Classes.Task;
using Symu.Common;
using Symu.Engine;
using Symu.Messaging.Messages;
using Symu.Repository;
using Symu.Repository.Networks.Beliefs;
using Symu.Repository.Networks.Knowledges;
using Symu.Results.Blocker;
using SymuTests.Helpers;

#endregion

namespace SymuTests.Classes.Agents
{
    [TestClass]
    public class AgentTests
    {
        private readonly TestEnvironment _environment = new TestEnvironment();
        private readonly Knowledge _knowledge = new Knowledge(1, "1", 1);
        private readonly Knowledge _knowledge2 = new Knowledge(2, "2", 1);
        private readonly List<Knowledge> _knowledges = new List<Knowledge>();
        private readonly OrganizationEntity _organizationEntity = new OrganizationEntity("1");
        private readonly SymuEngine _symu = new SymuEngine();
        private TestAgent _agent;
        private AgentKnowledge _agentKnowledge;

        [TestInitialize]
        public void Initialize()
        {
            _environment.SetOrganization(_organizationEntity);
            _symu.SetEnvironment(_environment);
            _organizationEntity.Models.On(1);
            _environment.IterationResult.On();

            _agent = new TestAgent(_organizationEntity.NextEntityIndex(), _environment);
            _agent.Cognitive.KnowledgeAndBeliefs.HasBelief = true;
            _agent.Cognitive.KnowledgeAndBeliefs.HasKnowledge = true;
            _agent.Cognitive.MessageContent.CanReceiveBeliefs = true;
            _agent.Cognitive.MessageContent.CanReceiveKnowledge = true;
            _agent.Cognitive.InternalCharacteristics.CanLearn = true;
            _agent.Cognitive.InternalCharacteristics.CanForget = true;
            _agent.Cognitive.InternalCharacteristics.CanInfluenceOrBeInfluence = true;
            _agent.Cognitive.InteractionCharacteristics.PreferredCommunicationMediums = CommunicationMediums.Email;
            _agent.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _agent.Cognitive.InteractionPatterns.ThresholdForNewInteraction = 1;

            var expertise = new AgentExpertise();
            _knowledges.Add(_knowledge);
            _environment.WhitePages.Network.NetworkKnowledges.AddKnowledge(_knowledge);
            _environment.WhitePages.Network.NetworkBeliefs.AddBelief(_knowledge);
            _environment.WhitePages.Network.NetworkKnowledges.AddKnowledge(_knowledge2);
            _environment.WhitePages.Network.NetworkBeliefs.AddBelief(_knowledge2);
            _agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {1}, 0, -1, 0);
            expertise.Add(_agentKnowledge);
            _environment.WhitePages.Network.NetworkKnowledges.Add(_agent.Id, expertise);

            Assert.AreEqual(AgentState.NotStarted, _agent.State);
            _agent.Start();
            _agent.WaitingToStart();
            Assert.AreEqual(AgentState.Started, _agent.State);
            _environment.Schedule.Step = 0;
            _agent.BeliefsModel.AddBelief(_knowledge.Id);
            _agent.BeliefsModel.InitializeBeliefs();
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
            Assert.AreNotEqual(0, _agent.KnowledgeModel.Expertise.Count);
            Assert.AreNotEqual(0, _environment.WhitePages.Network.NetworkInfluences.GetInfluenceability(_agent.Id));
            Assert.AreNotEqual(0, _environment.WhitePages.Network.NetworkInfluences.GetInfluentialness(_agent.Id));
            Assert.AreEqual(AgentState.Started, _agent.State);
        }

        private TestAgent AddTeammate()
        {
            var teammate = new TestAgent(_organizationEntity.NextEntityIndex(), _environment);
            teammate.Cognitive.InteractionPatterns.LimitNumberOfNewInteractions = false;
            teammate.Cognitive.InteractionPatterns.ThresholdForNewInteraction = 1;
            teammate.Start();
            teammate.WaitingToStart();
            return teammate;
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

        #region task management

        [TestMethod]
        public void PostTasksTest()
        {
            var tasks = new List<SymuTask>();
            for (var i = 0; i < 3; i++)
            {
                var task = new SymuTask(0) {Weight = 0.1F};
                tasks.Add(task);
            }

            _agent.PreStep();
            _agent.Post(tasks);
            Assert.IsTrue(_agent.Capacity.Actual < 1);
        }

        #endregion

        #region status

        [TestMethod]
        public void StateAgentTests()
        {
            _agent.State = AgentState.Stopping;
            Assert.AreEqual(AgentState.Stopping, _agent.State);
            _environment.StopAgents();
            Assert.AreEqual(AgentState.Stopped, _agent.State);
        }

        /// <summary>
        ///     Isolated
        /// </summary>
        [TestMethod]
        public void IsActiveTest()
        {
            Assert.IsFalse(_agent.IsPerformingTask(true));
        }

        /// <summary>
        ///     non Isolated, can't perform task
        /// </summary>
        [TestMethod]
        public void IsActiveTest1()
        {
            _agent.Cognitive.TasksAndPerformance.CanPerformTask = false;
            _agent.Environment.Schedule.Step = 1;
            Assert.IsFalse(_agent.IsPerformingTask(false));
        }

        /// <summary>
        ///     non Isolated, can perform task
        /// </summary>
        [TestMethod]
        public void IsActiveTest2()
        {
            _agent.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _agent.Environment.Schedule.Step = 1;
            Assert.IsTrue(_agent.IsPerformingTask(false));
        }

        /// <summary>
        ///     non Isolated, can't perform task on weekend
        /// </summary>
        [TestMethod]
        public void IsActiveTest3()
        {
            _agent.Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds = false;
            _agent.Environment.Schedule.Step = 5;
            Assert.IsFalse(_agent.IsPerformingTask(false));
        }

        /// <summary>
        ///     non Isolated, can't perform task on weekend
        /// </summary>
        [TestMethod]
        public void IsActiveTest4()
        {
            _agent.Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds = true;
            _agent.Environment.Schedule.Step = 5;
            Assert.IsTrue(_agent.IsPerformingTask(false));
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
                Sender = _agent.Id,
                Medium = CommunicationMediums.Irc
            };
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(1, _environment.Messages.Result.ReceivedMessagesCount);
            message.Medium = CommunicationMediums.Email;
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(2, _environment.Messages.Result.ReceivedMessagesCount);
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
            Assert.AreEqual<uint>(0, _environment.Messages.Result.SentMessagesCount);
            message.Medium = CommunicationMediums.Email;
            _agent.Post(message);
            Assert.AreEqual(2, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(0, _environment.Messages.Result.SentMessagesCount);
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
                Sender = _agent.Id,
                Medium = CommunicationMediums.Phone
            };
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(0, _environment.Messages.Result.ReceivedMessagesCount);
            message.Medium = CommunicationMediums.Meeting;
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(0, _environment.Messages.Result.ReceivedMessagesCount);
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
                Sender = _agent.Id,
                Medium = CommunicationMediums.Phone
            };
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(1, _environment.Messages.Result.ReceivedMessagesCount);
            message.Medium = CommunicationMediums.Meeting;
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(2, _environment.Messages.Result.ReceivedMessagesCount);
            //TODO test Missed messages
        }

        [TestMethod]
        public void PostDelayedMessagesTest()
        {
            var message = new Message();
            // Post as a delayed message
            _agent.PostAsADelayedMessage(message, 0);
            Assert.AreEqual(1, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(0, _environment.Messages.Result.ReceivedMessagesCount);
            Assert.AreEqual(0, _environment.Messages.WaitingMessages.Count);
            // Post Delayed messages
            _agent.PostDelayedMessages();
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(1, _environment.Messages.Result.ReceivedMessagesCount);
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
                Sender = _agent.Id,
                Medium = CommunicationMediums.Email
            };
            _agent.PostMessage(message);
            Assert.AreEqual<uint>(1, _environment.Messages.Result.ReceivedMessagesCount);
            Assert.AreEqual(1, _environment.Messages.LastSentMessages.Count);
            Assert.AreEqual(1, _agent.MessageProcessor.NumberMessagesPerStep);
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
            Assert.AreEqual<uint>(0, _environment.Messages.Result.SentMessagesCount);
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
            _agent.LearningModel.On = true;
            _agent.LearningModel.RateOfAgentsOn = 1;
            _agent.Cognitive.InternalCharacteristics.CanLearn = true;
            _agent.Cognitive.TasksAndPerformance.LearningRate = 1;
            var message = new Message(_agent.Id, _agent.Id, MessageAction.Ask, 0, attachments,
                CommunicationMediums.System);

            _agent.LearnKnowledgesFromPostMessage(message);
            Assert.AreEqual(0, _agent.KnowledgeModel.GetKnowledge(1).GetKnowledgeSum());
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
            _agent.LearningModel.On = true;
            _agent.LearningModel.RateOfAgentsOn = 1;
            _agent.LearningModel.TasksAndPerformance.LearningRate = 1;
            var message = new Message(_agent.Id, _agent.Id, MessageAction.Ask, 0, attachments,
                CommunicationMediums.Email);
            _agent.LearnKnowledgesFromPostMessage(message);
            Assert.AreEqual(1, _agent.KnowledgeModel.GetKnowledge(1).GetKnowledgeSum());
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
            Assert.AreEqual(1, _agent.BeliefsModel.GetBelief(belief.Id).GetBeliefSum());
        }

        private Belief SetBeliefs()
        {
            _agent.Cognitive.KnowledgeAndBeliefs.HasBelief = true;
            var belief = new Belief(1, "1", 1, RandomGenerator.RandomBinary, BeliefWeightLevel.RandomWeight);
            _environment.WhitePages.Network.NetworkBeliefs.AddBelief(belief);
            _environment.WhitePages.Network.NetworkBeliefs.Add(_agent.Id, 1, BeliefLevel.NeitherAgreeNorDisagree);
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
            Assert.AreEqual<uint>(0, _environment.Messages.Result.SentMessagesCount);
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
            Assert.AreEqual<uint>(0, _environment.Messages.Result.SentMessagesCount);

            _agent.State = AgentState.Starting;
            _agent.Post(message);
            Assert.AreEqual(2, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(0, _environment.Messages.Result.SentMessagesCount);
        }

        /// <summary>
        ///     OnLine Agent with Started agent
        /// </summary>
        [TestMethod]
        public void PostTest5()
        {
            _agent.State = AgentState.Started;
            var message = new Message
            {
                Sender = _agent.Id,
                Medium = CommunicationMediums.Email
            };
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual((uint) 1, _environment.Messages.Result.ReceivedMessagesCount);
        }

        /// <summary>
        ///     OnLine Agent with stopping agent
        /// </summary>
        [TestMethod]
        public void PostTest6()
        {
            _agent.State = AgentState.Stopping;
            var message = new Message
            {
                Sender = _agent.Id,
                Medium = CommunicationMediums.Email
            };
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual((uint) 1, _environment.Messages.Result.ReceivedMessagesCount);
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
            _agent.MessageProcessor.NumberMessagesPerStep = 1;
            Assert.IsTrue(_agent.IsMessagesPerPeriodBelowLimit(CommunicationMediums.Email));
            Assert.IsTrue(_agent.IsMessagesPerPeriodBelowLimit(CommunicationMediums.System));
            // Can't exceed byte max value
            _agent.MessageProcessor.NumberMessagesPerStep = ushort.MaxValue;
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
            _agent.MessageProcessor.NumberMessagesPerStep = 1;
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
            _agent.MessageProcessor.NumberMessagesPerStep = 1;
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
            _agent.MessageProcessor.NumberMessagesPerStep = 1;
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
            var agent2 = AddTeammate();
            var message = new Message(_agent.Id, agent2.Id, MessageAction.Add, 0)
            {
                Sender = _agent.Id,
                Medium = CommunicationMediums.Email
            };
            _agent.Send(message);
            _environment.Messages.WaitingToClearAllMessages();
            //environment trace the message
            Assert.IsTrue(_environment.Messages.MessagesReceivedByAgent(0, agent2.Id).Any());
            Assert.IsTrue(_environment.Messages.MessagesSentByAgent(0, _agent.Id).Any());
            Assert.IsTrue(_environment.Messages.LastSentMessages.Any);
            // Agent1
            Assert.AreEqual(1, _agent.MessageProcessor.NumberMessagesPerStep);
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
            Assert.AreEqual(0, _agent.MessageProcessor.NumberMessagesPerStep);
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
            _environment.Schedule.Step = 0;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _agent.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _agent.Cognitive.InteractionPatterns.IsolationCyclicity = Cyclicity.None;
            _agent.MessageProcessor.IncrementMessagesPerPeriod(CommunicationMediums.Email, true);
            _agent.ForgettingModel.InternalCharacteristics.ForgettingMean = 1;

            _agent.PreStep();
            // Status test
            Assert.AreEqual(AgentStatus.Available, _agent.Status);
            // Capacity test
            Assert.AreEqual(1, _agent.Capacity.Initial);
            // ClearMessagesPerPeriod test
            Assert.AreEqual(0, _agent.MessageProcessor.NumberMessagesPerStep);
            Assert.AreEqual(0, _agent.MessageProcessor.NumberSentPerPeriod);
        }

        /// <summary>
        ///     Focus ForgettingModel
        ///     Model Off
        /// </summary>
        [TestMethod]
        public void AskPreStepTest1()
        {
            _agent.ForgettingModel.On = false;
            _agent.KnowledgeModel.AddKnowledge(1, KnowledgeLevel.FullKnowledge, 0, -1);
            _agent.KnowledgeModel.InitializeExpertise(0);
            _environment.InitializeInteractionSphere();
            _environment.PreStep();
            Assert.AreEqual(0, _agent.ForgettingModel.ForgettingExpertise.Count);
        }

        /// <summary>
        ///     Focus ForgettingModel
        ///     Model On
        /// </summary>
        [TestMethod]
        public void AskPreStepTest2()
        {
            _agent.ForgettingModel.On = true;
            _agent.ForgettingModel.InternalCharacteristics.ForgettingMean = 1;
            _agent.KnowledgeModel.AddKnowledge(1, KnowledgeLevel.FullKnowledge, 0, -1);
            _agent.KnowledgeModel.InitializeExpertise(0);
            _environment.InitializeInteractionSphere();
            _environment.PreStep();
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
            _agent.MessageProcessor.Subscriptions.Subscribe(_agent.Id, 1);
            var message = new Message(_agent.Id, _agent.Id, MessageAction.Remove, SymuYellowPages.Subscribe);
            _agent.RemoveSubscribe(message);
            Assert.AreEqual(0, _agent.MessageProcessor.Subscriptions.SubscribersCount(1));
        }

        /// <summary>
        ///     Multiple subscriptions
        /// </summary>
        [TestMethod]
        public void RemoveSubscribeTest1()
        {
            _agent.MessageProcessor.Subscriptions.Subscribe(_agent.Id, 1);
            _agent.MessageProcessor.Subscriptions.Subscribe(_agent.Id, 2);
            _agent.MessageProcessor.Subscriptions.Subscribe(_agent.Id, 3);
            var message = new Message(_agent.Id, _agent.Id, MessageAction.Remove, SymuYellowPages.Subscribe);
            _agent.RemoveSubscribe(message);
            Assert.AreEqual(0, _agent.MessageProcessor.Subscriptions.SubscribersCount(1));
            Assert.AreEqual(0, _agent.MessageProcessor.Subscriptions.SubscribersCount(2));
            Assert.AreEqual(0, _agent.MessageProcessor.Subscriptions.SubscribersCount(3));
        }

        [TestMethod]
        public void AddSubscribeTest()
        {
            var attachments = new MessageAttachments();
            attachments.Add((byte) 1);
            attachments.Add((byte) 2);
            var message = new Message(_agent.Id, _agent.Id, MessageAction.Add, SymuYellowPages.Subscribe, attachments);
            _agent.AddSubscribe(message);
            Assert.AreEqual(1, _agent.MessageProcessor.Subscriptions.SubscribersCount(1));
            Assert.AreEqual(1, _agent.MessageProcessor.Subscriptions.SubscribersCount(2));
            Assert.AreEqual(0, _agent.MessageProcessor.Subscriptions.SubscribersCount(0));
        }

        #endregion

        #region Capacity management

        /// <summary>
        ///     Non working day
        /// </summary>
        [TestMethod]
        public void NonPassingHandleCapacityTest()
        {
            _environment.Schedule.Step = 5;
            _agent.HandleCapacity(false, true);
            Assert.AreEqual(0, _agent.Capacity.Initial);
            Assert.AreEqual(0, _agent.Capacity.Actual);
        }

        /// <summary>
        ///     Can't perform task => no influence on the Capacity
        /// </summary>
        [TestMethod]
        public void PassingHandleCapacityTest1()
        {
            _environment.Schedule.Step = 0;
            _agent.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _agent.HandleCapacity(false, true);
            Assert.AreEqual(1, _agent.Capacity.Initial);
            Assert.AreEqual(1, _agent.Capacity.Actual);
        }

        /// <summary>
        ///     Isolated
        /// </summary>
        [TestMethod]
        public void NonPassingHandleCapacityTest2()
        {
            _environment.Schedule.Step = 0;
            _agent.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _agent.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Always;
            _agent.HandleCapacity(true, true);
            Assert.AreEqual(0, _agent.Capacity.Initial);
            Assert.AreEqual(0, _agent.Capacity.Actual);
        }

        /// <summary>
        ///     Passing tests
        /// </summary>
        [TestMethod]
        public void HandleCapacityTest1()
        {
            _environment.Schedule.Step = 0;
            _agent.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _agent.HandleCapacity(false, true);
            Assert.AreEqual(1, _agent.Capacity.Initial);
            Assert.AreEqual(1, _agent.Capacity.Actual);
        }

        /// <summary>
        ///     No Reset Remaining Capacity
        /// </summary>
        [TestMethod]
        public void HandleCapacityTest2()
        {
            _environment.Schedule.Step = 0;
            _agent.Cognitive.TasksAndPerformance.CanPerformTask = true;
            _environment.Organization.Murphies.UnAvailability.On = false;
            _agent.HandleCapacity(false, false);
            Assert.AreEqual(1, _agent.Capacity.Initial);
            Assert.AreEqual(0, _agent.Capacity.Actual);
        }

        #endregion

        #region Status

        /// <summary>
        ///     Initial capacity = 0 - nothing arrived
        /// </summary>
        [TestMethod]
        public void HandleStatusTest()
        {
            _agent.HandleStatus(true);
            Assert.AreEqual(AgentStatus.Offline, _agent.Status);
        }

        /// <summary>
        ///     Initial capacity = 0 - nothing arrived
        /// </summary>
        [TestMethod]
        public void HandleStatusTest1()
        {
            _agent.HandleStatus(false);
            Assert.AreEqual(AgentStatus.Available, _agent.Status);
        }

        /// <summary>
        ///     Initial capacity >0 + delayed Post messages
        /// </summary>
        [TestMethod]
        public void HandleStatusTest2()
        {
            _agent.Capacity.Initial = 1;
            var message = new Message
            {
                Sender = _agent.Id,
                Receiver = _agent.Id,
                Medium = CommunicationMediums.System
            };
            _agent.MessageProcessor.DelayedMessages.Enqueue(message, 0);
            _agent.HandleStatus(false);
            Assert.IsTrue(_environment.Messages.LastSentMessages.Count >= 1);
            //Assert.IsTrue(_environment.Messages.CheckMessages);
        }

        #endregion

        #region Interactions

        /// <summary>
        ///     With empty list
        /// </summary>
        [TestMethod]
        public void FilterAgentIdsToInteractTest()
        {
            var agentIds = new List<AgentId>();
            Assert.AreEqual(0, _agent.FilterAgentIdsToInteract(agentIds).Count());
        }

        /// <summary>
        ///     With filled list - limit false
        /// </summary>
        [TestMethod]
        public void FilterAgentIdsToInteractTest1()
        {
            var agentIds = new List<AgentId>();
            for (ushort i = 2; i < 12; i++)
            {
                agentIds.Add(new AgentId(i, 1));
            }

            _agent.Cognitive.InteractionPatterns.LimitNumberOfNewInteractions = false;
            Assert.AreEqual(10, _agent.FilterAgentIdsToInteract(agentIds).Count());
        }

        /// <summary>
        ///     With filled list - limit on - below limit(default 5)
        /// </summary>
        [TestMethod]
        public void FilterAgentIdsToInteractTest2()
        {
            var agentIds = new List<AgentId>();
            for (ushort i = 2; i < 5; i++)
            {
                agentIds.Add(new AgentId(i, 1));
            }

            _agent.Cognitive.InteractionPatterns.LimitNumberOfNewInteractions = true;
            _agent.Cognitive.InteractionPatterns.MaxNumberOfNewInteractions = 5;
            Assert.AreEqual(3, _agent.FilterAgentIdsToInteract(agentIds).Count());
        }

        /// <summary>
        ///     With filled list - limit on - above limit(default 5)
        /// </summary>
        [TestMethod]
        public void FilterAgentIdsToInteractTest3()
        {
            var agentIds = new List<AgentId>();
            for (ushort i = 2; i < 12; i++)
            {
                agentIds.Add(new AgentId(i, 1));
            }

            _agent.Cognitive.InteractionPatterns.LimitNumberOfNewInteractions = true;
            _agent.Cognitive.InteractionPatterns.MaxNumberOfNewInteractions = 5;
            Assert.AreEqual(5, _agent.FilterAgentIdsToInteract(agentIds).Count());
        }

        /// <summary>
        ///     IsPartOfInteractionSphere false
        /// </summary>
        [TestMethod]
        public void AcceptNewInteractionTest()
        {
            var agent2 = new TestAgent(2, _environment);
            Assert.IsTrue(_agent.AcceptNewInteraction(agent2.Id));
        }

        /// <summary>
        ///     IsPartOfInteractionSphere true true
        ///     ActiveLink
        /// </summary>
        [TestMethod]
        public void AcceptNewInteractionTest1()
        {
            _agent.Cognitive.InteractionPatterns.IsPartOfInteractionSphere = true;
            var agent2 = new TestAgent(2, _environment);
            _environment.WhitePages.Network.NetworkLinks.AddLink(_agent.Id, agent2.Id);
            Assert.IsTrue(_agent.AcceptNewInteraction(agent2.Id));
        }

        /// <summary>
        ///     IsPartOfInteractionSphere true true
        ///     ActiveLink false
        ///     AllowNewInteractions false
        /// </summary>
        [TestMethod]
        public void AcceptNewInteractionTest2()
        {
            _agent.Cognitive.InteractionPatterns.IsPartOfInteractionSphere = true;
            _agent.Cognitive.InteractionPatterns.AllowNewInteractions = false;
            var agent2 = new TestAgent(2, _environment);
            Assert.IsFalse(_agent.AcceptNewInteraction(agent2.Id));
        }

        /// <summary>
        ///     IsPartOfInteractionSphere true true
        ///     Limit interaction to 0
        /// </summary>
        [TestMethod]
        public void AcceptNewInteractionTest3()
        {
            _agent.Cognitive.InteractionPatterns.IsPartOfInteractionSphere = true;
            _agent.Cognitive.InteractionPatterns.AllowNewInteractions = true;
            _agent.Cognitive.InteractionPatterns.LimitNumberOfNewInteractions = true;
            _agent.Cognitive.InteractionPatterns.MaxNumberOfNewInteractions = 0;
            var agent2 = new TestAgent(2, _environment);
            Assert.IsFalse(_agent.AcceptNewInteraction(agent2.Id));
        }

        /// <summary>
        ///     IsPartOfInteractionSphere true true
        ///     ThresholdForNewInteraction 0
        /// </summary>
        [TestMethod]
        public void AcceptNewInteractionTest4()
        {
            _agent.Cognitive.InteractionPatterns.IsPartOfInteractionSphere = true;
            _agent.Cognitive.InteractionPatterns.AllowNewInteractions = true;
            _agent.Cognitive.InteractionPatterns.ThresholdForNewInteraction = 0;
            var agent2 = new TestAgent(2, _environment);
            Assert.IsFalse(_agent.AcceptNewInteraction(agent2.Id));
        }

        /// <summary>
        ///     IsPartOfInteractionSphere true true
        ///     ThresholdForNewInteraction 1
        /// </summary>
        [TestMethod]
        public void AcceptNewInteractionTest5()
        {
            _agent.Cognitive.InteractionPatterns.IsPartOfInteractionSphere = true;
            _agent.Cognitive.InteractionPatterns.AllowNewInteractions = true;
            _agent.Cognitive.InteractionPatterns.ThresholdForNewInteraction = 1;
            var agent2 = new TestAgent(2, _environment);
            Assert.IsTrue(_agent.AcceptNewInteraction(agent2.Id));
        }

        #endregion

        #region Blockers & help

        /// <summary>
        ///     CHeck MultipleBlockers false
        /// </summary>
        [TestMethod]
        public void CheckBlockersTest()
        {
            _environment.IterationResult.Initialize();
            _organizationEntity.Murphies.MultipleBlockers = false;
            var task = new SymuTask(0);
            task.Add(Murphy.IncompleteBelief, 0);
            _agent.CheckNewBlockers(task);
            // We don't add another blocker
            Assert.AreEqual(1, task.Blockers.List.Count);
        }

        /// <summary>
        ///     With rate 0
        /// </summary>
        [TestMethod]
        public void AskHelpTests()
        {
            var task = new SymuTask(0);
            var parameter = new MessageAttachments();
            parameter.Add(new Blocker(Murphy.IncompleteInformation, 1));
            parameter.Add(task);

            var message = new Message
            {
                Sender = _agent.Id,
                Receiver = _agent.Id,
                Action = MessageAction.Ask,
                Subject = SymuYellowPages.Help,
                Attachments = parameter
            };

            // To be sure to send a delayed message
            _organizationEntity.Murphies.IncompleteInformation.ResponseTime = 10;
            _organizationEntity.Murphies.IncompleteInformation.RateOfAnswers = 0;
            _agent.AskHelp(message);
            Assert.AreEqual(0, _environment.Messages.DelayedMessages.Count);
        }

        /// <summary>
        ///     With Rate 1
        /// </summary>
        [TestMethod]
        public void AskHelpTests1()
        {
            var task = new SymuTask(0);
            var parameter = new MessageAttachments();
            parameter.Add(new Blocker(Murphy.IncompleteInformation, 1));
            parameter.Add(task);

            var message = new Message
            {
                Sender = _agent.Id,
                Receiver = _agent.Id,
                Action = MessageAction.Ask,
                Subject = SymuYellowPages.Help,
                Attachments = parameter
            };

            // To be sure to send a delayed message
            _organizationEntity.Murphies.IncompleteInformation.On = true;
            _organizationEntity.Murphies.IncompleteInformation.RateOfAgentsOn = 1;
            _organizationEntity.Murphies.IncompleteInformation.ResponseTime = 10;
            _organizationEntity.Murphies.IncompleteInformation.RateOfAnswers = 1;
            _agent.AskHelp(message);
            Assert.AreEqual(1, _environment.Messages.DelayedMessages.Count);
        }

        /// <summary>
        ///     Murphies.IncompleteBelief.Off
        /// </summary>
        [TestMethod]
        public void CheckBeliefsBitsTest()
        {
            var task = new SymuTask(0);
            task.SetKnowledgesBits(_agent.Cognitive.TasksAndPerformance.TaskModel, _knowledges,
                MurphyTask.FullRequiredBits);
            _agent.CheckBlockerIncompleteBeliefs(task);
            Assert.AreEqual(ImpactLevel.None, task.Incorrectness);
            Assert.IsFalse(task.Blockers.Exists(Murphy.IncompleteBelief, 0));
        }

        /// <summary>
        ///     Murphies.IncompleteBelief.On
        ///     Task without BeliefsBits
        ///     Test taskBits.Mandatory.Length == 0 && taskBits.Required.Length == 0
        /// </summary>
        [TestMethod]
        public void CheckBeliefsBitsTest1()
        {
            var task = new SymuTask(0);
            task.SetKnowledgesBits(_agent.Cognitive.TasksAndPerformance.TaskModel, _knowledges,
                MurphyTask.NoRequiredBits);
            _organizationEntity.Murphies.IncompleteBelief.On = true;
            _agent.CheckBlockerIncompleteBeliefs(task);
            Assert.AreEqual(ImpactLevel.None, task.Incorrectness);
            Assert.IsFalse(task.Blockers.Exists(Murphy.IncompleteBelief, 0));
        }

        /// <summary>
        ///     Murphies.IncompleteBelief.On
        ///     Task with BeliefsBits but worker has no belief
        /// </summary>
        [TestMethod]
        public void CheckBeliefsBitsTest2()
        {
            _organizationEntity.Murphies.IncompleteBelief.On = true;
            _organizationEntity.Murphies.IncompleteBelief.MandatoryRatio = 1;
            var task = new SymuTask(0);
            task.SetKnowledgesBits(_agent.Cognitive.TasksAndPerformance.TaskModel, _knowledges,
                MurphyTask.FullRequiredBits);
            _agent.CheckBlockerIncompleteBeliefs(task);
            Assert.AreEqual(0, task.Priority);
            Assert.IsFalse(task.Blockers.Exists(Murphy.IncompleteBelief, 0));
        }

        private Message SetMessageWithTaskAndKnowledge()
        {
            var task = new SymuTask(0)
            {
                Weight = 1,
                KeyActivity = 1
            };
            var attachments = new MessageAttachments();
            attachments.Add(0);
            attachments.Add(task);
            attachments.KnowledgeId = _knowledge.Id;
            attachments.KnowledgeBit = 0;
            return new Message(_agent.Id, _agent.Id, MessageAction.Ask, SymuYellowPages.Help,
                attachments,
                CommunicationMediums.Email);
        }

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void AskHelpIncompleteTest()
        {
            var message = SetMessageWithTaskAndKnowledge();
            _organizationEntity.Murphies.IncompleteBelief.RateOfAnswers = 0;
            _agent.AskHelpIncomplete(message, _environment.Organization.Murphies.IncompleteBelief.DelayToReplyToHelp());
            Assert.AreEqual(_agent.Capacity.Initial, _agent.Capacity.Actual);
        }

        /// <summary>
        ///     Passing test
        ///     without expertise
        /// </summary>
        [TestMethod]
        public void AskHelpIncompleteTest1()
        {
            var message = SetMessageWithTaskAndKnowledge();
            _organizationEntity.Murphies.IncompleteBelief.RateOfAnswers = 1;
            _organizationEntity.Murphies.IncompleteBelief.ResponseTime = 0;
            _agent.SetInitialCapacity();
            _agent.Capacity.Reset();
            // no belief
            _agent.BeliefsModel.GetBelief(_knowledge.Id).BeliefBits.SetBit(0, 0);
            _agent.AskHelpIncomplete(message, _environment.Organization.Murphies.IncompleteBelief.DelayToReplyToHelp());
            Assert.IsTrue(_agent.Capacity.Initial > _agent.Capacity.Actual);
            Assert.IsTrue(_environment.Messages.LastSentMessages.Exists(MessageAction.Reply, SymuYellowPages.Help,
                _agent.Id.ClassKey, _agent.Id.ClassKey));
            var reply = _environment.Messages.LastSentMessages.SentByAgent(0, _agent.Id).Last();
            Assert.IsNull(reply.Attachments.BeliefBits);
        }


        /// <summary>
        ///     Passing test
        ///     without belief - with delay
        /// </summary>
        [TestMethod]
        public void AskHelpIncompleteTest2()
        {
            var message = SetMessageWithTaskAndKnowledge();
            _organizationEntity.Murphies.IncompleteBelief.RateOfAnswers = 1;
            _organizationEntity.Murphies.IncompleteBelief.ResponseTime = 10;
            _agent.SetInitialCapacity();
            _agent.Capacity.Reset();
            // no belief
            _agent.BeliefsModel.GetBelief(_knowledge.Id).BeliefBits.SetBit(0, 0);
            _agent.AskHelpIncomplete(message, 3);
            Assert.IsTrue(_agent.Capacity.Initial > _agent.Capacity.Actual);
            Assert.IsFalse(_environment.Messages.LastSentMessages.Exists(MessageAction.Reply, SymuYellowPages.Help,
                _agent.Id.ClassKey, _agent.Id.ClassKey));
            var reply = _environment.Messages.DelayedMessages.Last();
            Assert.IsNull(reply.Attachments.BeliefBits);
        }

        /// <summary>
        ///     Passing test
        ///     With expertise
        /// </summary>
        [TestMethod]
        public void AskHelpIncompleteTest3()
        {
            var message = SetMessageWithTaskAndKnowledge();
            _organizationEntity.Murphies.IncompleteBelief.RateOfAnswers = 1;
            _organizationEntity.Murphies.IncompleteBelief.ResponseTime = 0;
            _agent.SetInitialCapacity();
            _agent.Capacity.Reset();
            _agent.BeliefsModel.GetBelief(_knowledge.Id).BeliefBits.SetBit(0, 1);
            _agent.AskHelpIncomplete(message, _environment.Organization.Murphies.IncompleteBelief.DelayToReplyToHelp());
            Assert.IsTrue(_agent.Capacity.Initial > _agent.Capacity.Actual);
            Assert.IsTrue(_environment.Messages.LastSentMessages.Exists(MessageAction.Reply, SymuYellowPages.Help,
                _agent.Id.ClassKey, _agent.Id.ClassKey));
            var reply = _environment.Messages.LastSentMessages.SentByAgent(0, _agent.Id).Last();
            Assert.IsNotNull(reply.Attachments.BeliefBits);
        }

        /// <summary>
        ///     Task weight = 1
        /// </summary>
        [TestMethod]
        public void RecoverBlockerBeliefByGuessingTest()
        {
            _organizationEntity.Murphies.IncompleteBelief.On = true;
            _organizationEntity.Murphies.IncompleteBelief.RateOfIncorrectGuess = 1;
            _environment.WhitePages.Network.NetworkBeliefs.Model = RandomGenerator.RandomUniform;
            var task = new SymuTask(0) {Weight = 1};
            task.SetKnowledgesBits(_agent.Cognitive.TasksAndPerformance.TaskModel, _knowledges,
                MurphyTask.FullRequiredBits);
            _environment.Organization.Murphies.IncompleteKnowledge.CostFactorOfGuessing = 2;

            var agentBelief = _agent.BeliefsModel.GetBelief(_knowledge.Id);
            agentBelief.BeliefBits.SetBit(0, 0);
            var blocker = new Blocker(1, 0) {Parameter = _knowledge.Id, Parameter2 = (byte) 0};

            _agent.RecoverBlockerIncompleteBeliefByGuessing(task, blocker);
            Assert.AreNotEqual(ImpactLevel.None, task.Incorrectness);
            if (task.Incorrectness == ImpactLevel.Blocked)
            {
                return;
            }

            Assert.AreNotEqual(0, agentBelief.BeliefBits.GetBit(0));
            Assert.AreNotEqual(1, task.Weight);
        }

        /// <summary>
        ///     No passing test
        /// </summary>
        [TestMethod]
        public void ReplyHelpTest()
        {
            var task = new SymuTask(0);
            var blocker = task.Add(Murphy.IncompleteBelief, 0);
            var message = new Message
            {
                Medium = CommunicationMediums.Email,
                Attachments = new MessageAttachments()
            };
            message.Attachments.Add(blocker);
            message.Attachments.Add(task);
            _agent.ReplyHelp(message);
            Assert.IsTrue(task.IsBlocked);
        }

        /// <summary>
        ///     Passing test
        ///     BeliefsBits not null
        /// </summary>
        [TestMethod]
        public void ReplyHelpTest1()
        {
            var task = new SymuTask(0);
            var blocker = task.Add(Murphy.IncompleteBelief, 0);
            var message = new Message
            {
                Medium = CommunicationMediums.Email,
                Attachments = new MessageAttachments()
            };
            message.Attachments.Add(blocker);
            message.Attachments.Add(task);
            message.Attachments.BeliefBits = new Bits(new float[] {1}, 0);
            _agent.ReplyHelp(message);
            Assert.IsFalse(task.IsBlocked);
        }


        [TestMethod]
        public void NullCheckIncompleteInformationTests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _agent.CheckBlockerIncompleteInformation(null));
        }

        /// <summary>
        ///     Model IncompleteInformation off
        /// </summary>
        [TestMethod]
        public void CheckIncompleteInformationTests()
        {
            _organizationEntity.Murphies.IncompleteInformation.On = false;
            var task = new SymuTask(0);
            Assert.IsFalse(_agent.CheckBlockerIncompleteInformation(task));
        }

        /// <summary>
        ///     Model IncompleteInformation on
        ///     Rate 0
        /// </summary>
        [TestMethod]
        public void CheckIncompleteInformationTests1()
        {
            _organizationEntity.Murphies.IncompleteInformation.On = true;
            _organizationEntity.Murphies.IncompleteInformation.ThresholdForReacting = 0;
            var task = new SymuTask(0);
            Assert.IsFalse(_agent.CheckBlockerIncompleteInformation(task));
        }

        /// <summary>
        ///     Model IncompleteInformation on
        ///     Rate 1
        ///     with creator of task == worker
        /// </summary>
        [TestMethod]
        public void CheckIncompleteInformationTests2()
        {
            _organizationEntity.Murphies.IncompleteInformation.On = true;
            _organizationEntity.Murphies.IncompleteInformation.ThresholdForReacting = 1;
            var task = new SymuTask(0) {Assigned = _agent.Id};
            Assert.IsFalse(_agent.CheckBlockerIncompleteInformation(task));
            Assert.IsFalse(task.IsBlocked);
        }

        /// <summary>
        ///     Model IncompleteInformation on
        ///     Rate 1
        ///     with creator of task != worker
        ///     With RAF == 0
        /// </summary>
        [TestMethod]
        public void CheckIncompleteInformationTests3()
        {
            _organizationEntity.Murphies.IncompleteInformation.On = true;
            _organizationEntity.Murphies.IncompleteInformation.ThresholdForReacting = 1;
            var task = new SymuTask(0) {Weight = 0};
            Assert.IsFalse(_agent.CheckBlockerIncompleteInformation(task));
            Assert.IsFalse(task.IsBlocked);
        }

        /// <summary>
        ///     Model IncompleteInformation on
        ///     Rate 1
        ///     with creator of task != worker
        ///     With RAF > 0
        /// </summary>
        [TestMethod]
        public void CheckIncompleteInformationTests4()
        {
            _organizationEntity.Murphies.IncompleteInformation.On = true;
            _organizationEntity.Murphies.IncompleteInformation.ThresholdForReacting = 1;
            var task = new SymuTask(0) {Weight = 1};
            Assert.IsTrue(_agent.CheckBlockerIncompleteInformation(task));
            Assert.IsTrue(task.IsBlocked);
        }

        /// <summary>
        ///     With teammates
        /// </summary>
        [TestMethod]
        public void TryRecoverBlockerIncompleteKnowledgeTest()
        {
            // Add teammates with knowledge
            var teammate = AddTeammate();
            var group = new TestAgent(_organizationEntity.NextEntityIndex(), 2, _environment);
            group.Start();
            _environment.WhitePages.Network.AddMemberToGroup(_agent.Id, 100, group.Id);
            _environment.WhitePages.Network.AddMemberToGroup(teammate.Id, 100, group.Id);
            teammate.KnowledgeModel.AddKnowledge(_knowledge.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            teammate.KnowledgeModel.InitializeExpertise(0);
            _agent.KnowledgeModel.GetKnowledge(_knowledge.Id).SetKnowledgeBit(0, 1, 0);
            _environment.WhitePages.Network.InteractionSphere.SetSphere(true,
                _environment.WhitePages.AllAgentIds().ToList(), _environment.WhitePages.Network);

            var task = new SymuTask(0) {KeyActivity = 1};

            var blocker = new Blocker(Murphy.IncompleteKnowledge, 0)
            {
                Parameter = _knowledge.Id,
                Parameter2 = (byte) 0
            };
            _organizationEntity.Murphies.IncompleteKnowledge.CommunicationMediums = CommunicationMediums.Email;
            _agent.SetInitialCapacity();
            _agent.Capacity.Reset();
            _agent.TryRecoverBlockerIncompleteKnowledge(task, blocker);
            Assert.IsTrue(_environment.Messages.LastSentMessages.Exists(MessageAction.Ask, SymuYellowPages.Help,
                _agent.Id.ClassKey, _agent.Id.ClassKey));
            Assert.IsTrue(_agent.Capacity.Actual < 1);
            Assert.IsTrue(_agent.TimeSpent[task.KeyActivity] > 0);
        }

        /// <summary>
        ///     Murphies.IncompleteKnowledge.Off
        /// </summary>
        [TestMethod]
        public void CheckKnowledgesBitsTest()
        {
            _organizationEntity.Murphies.IncompleteKnowledge.On = false;
            var task = new SymuTask(0) {Weight = 1};
            task.SetKnowledgesBits(_agent.Cognitive.TasksAndPerformance.TaskModel, _knowledges,
                MurphyTask.FullRequiredBits);
            _agent.CheckBlockerIncompleteKnowledge(task);
            Assert.AreEqual(ImpactLevel.None, task.Incorrectness);
            Assert.IsFalse(task.Blockers.Exists(Murphy.IncompleteKnowledge, 0));
        }

        /// <summary>
        ///     Murphies.IncompleteKnowledge.On
        ///     Task without KnowledgesBits
        ///     Test taskBits.Mandatory.Length == 0 && taskBits.Required.Length == 0
        /// </summary>
        [TestMethod]
        public void CheckKnowledgesBitsTest1()
        {
            _organizationEntity.Murphies.IncompleteKnowledge.On = true;
            var task = new SymuTask(0) {Weight = 1};
            task.SetKnowledgesBits(_agent.Cognitive.TasksAndPerformance.TaskModel, _knowledges,
                MurphyTask.NoRequiredBits);
            _agent.CheckBlockerIncompleteKnowledge(task);
            Assert.AreEqual(ImpactLevel.None, task.Incorrectness);
            Assert.IsFalse(task.Blockers.Exists(Murphy.IncompleteKnowledge, 0));
        }

        /// <summary>
        ///     Murphies.IncompleteKnowledge.On
        ///     Task with KnowledgesBits but worker has no knowledge
        /// </summary>
        [TestMethod]
        public void CheckKnowledgesBitsTest2()
        {
            _organizationEntity.Murphies.IncompleteKnowledge.On = true;
            var task = new SymuTask(0) {Weight = 1};
            task.SetTasksManager(_agent.TaskProcessor.TasksManager);
            task.SetKnowledgesBits(_agent.Cognitive.TasksAndPerformance.TaskModel, _knowledges,
                MurphyTask.FullRequiredBits);
            _agent.KnowledgeModel.GetKnowledge(_knowledge.Id).SetKnowledgeBit(0, 0, 0);
            _agent.CheckBlockerIncompleteKnowledge(task);

            Assert.IsTrue(_agent.TaskProcessor.TasksManager.BlockerResult.Cancelled == 1 ||
                          _agent.TaskProcessor.TasksManager.BlockerResult.Done == 1);
        }

        /// <summary>
        ///     Murphies.IncompleteKnowledge.On
        ///     Task with KnowledgesBits and worker has basic knowledge
        /// </summary>
        [TestMethod]
        public void CheckKnowledgesBitsTest3()
        {
            _organizationEntity.Murphies.IncompleteKnowledge.On = true;
            _organizationEntity.Murphies.IncompleteKnowledge.MandatoryRatio = 0;
            _organizationEntity.Murphies.IncompleteKnowledge.RequiredRatio = 1;
            _organizationEntity.Murphies.IncompleteKnowledge.RateOfIncorrectGuess = 1;
            var task = new SymuTask(0) {Weight = 1};
            task.SetKnowledgesBits(_organizationEntity.Murphies.IncompleteKnowledge, _knowledges, 1);
            _agent.Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge = false;
            _agent.KnowledgeModel.AddKnowledge(_knowledge.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            _agent.KnowledgeModel.InitializeExpertise(0);
            _agent.CheckBlockerIncompleteKnowledge(task);
            Assert.AreNotEqual(ImpactLevel.None, task.Incorrectness);
        }


        /// <summary>
        ///     Murphies.IncompleteKnowledge.On
        ///     Task with KnowledgesBits and worker has full knowledge
        /// </summary>
        [TestMethod]
        public void CheckKnowledgesBitsTest4()
        {
            _organizationEntity.Murphies.IncompleteKnowledge.On = true;
            _organizationEntity.Murphies.IncompleteKnowledge.MandatoryRatio = 0;
            _organizationEntity.Murphies.IncompleteKnowledge.RateOfIncorrectGuess = 1;
            var task = new SymuTask(0) {Weight = 1};
            task.SetKnowledgesBits(_organizationEntity.Murphies.IncompleteKnowledge, _knowledges,
                MurphyTask.FullRequiredBits);
            _agent.KnowledgeModel.GetKnowledge(_knowledge.Id).SetKnowledgeBit(0, 1, 0);
            _agent.CheckBlockerIncompleteKnowledge(task);
            Assert.AreEqual(ImpactLevel.None, task.Incorrectness);
            Assert.IsFalse(task.Blockers.Exists(Murphy.IncompleteKnowledge, 0));
        }

        [TestMethod]
        public void RecoverBlockerKnowledgeByGuessingTest()
        {
            _agent.Cognitive.InternalCharacteristics.CanLearn = true;
            _agent.LearningModel.On = true;
            _agent.LearningModel.RateOfAgentsOn = 1;
            _organizationEntity.Murphies.IncompleteKnowledge.On = true;
            _organizationEntity.Murphies.IncompleteKnowledge.RateOfIncorrectGuess = 1;
            _organizationEntity.Murphies.IncompleteKnowledge.CostFactorOfGuessing = 2;
            var task = new SymuTask(0) {Weight = 1};
            task.SetKnowledgesBits(_organizationEntity.Murphies.IncompleteKnowledge, _knowledges,
                MurphyTask.FullRequiredBits);

            _agent.KnowledgeModel.GetKnowledge(_knowledge.Id).SetKnowledgeBit(0, 0, 0);
            _agent.RecoverBlockerIncompleteKnowledgeByGuessing(task, null, _knowledge.Id, 0,
                BlockerResolution.Guessing);
            Assert.AreNotEqual(ImpactLevel.None, task.Incorrectness);
            if (task.HasBeenCancelledBy.Any())
            {
                return;
            }

            Assert.IsTrue(1 < task.Weight);
            var agentKnowledge =
                _environment.WhitePages.Network.NetworkKnowledges.GetAgentKnowledge(_agent.Id, _knowledge.Id);
            Assert.AreEqual(_agent.LearningModel.TasksAndPerformance.LearningByDoingRate,
                agentKnowledge.GetKnowledgeBit(0));
        }

        /// <summary>
        ///     With help
        /// </summary>
        [TestMethod]
        public void TryRecoverBlockerIncompleteInformationTest()
        {
            var task = new SymuTask(0)
            {
                Weight = 1,
                Creator = _agent.Id
            };
            var blocker = task.Add(Murphy.IncompleteKnowledge, 0, (ushort) 0);
            _agent.TryRecoverBlockerIncompleteInformation(task, blocker);
            Assert.IsTrue(_environment.Messages.LastSentMessages.Exists(MessageAction.Ask, SymuYellowPages.Help,
                _agent.Id.ClassKey, _agent.Id.ClassKey));
            Assert.IsTrue(task.IsBlocked);
        }

        /// <summary>
        ///     With Guess
        /// </summary>
        [TestMethod]
        public void TryRecoverBlockerIncompleteInformationTest1()
        {
            _organizationEntity.Murphies.IncompleteInformation.On = true;
            _organizationEntity.Murphies.IncompleteInformation.LimitNumberOfTries = 1;
            var task = new SymuTask(0) {Weight = 1};
            var blocker = task.Add(Murphy.IncompleteKnowledge, 0, (ushort) 0);
            blocker.NumberOfTries = 2;
            _agent.TryRecoverBlockerIncompleteInformation(task, blocker);
            Assert.IsFalse(_environment.Messages.LastSentMessages.Exists(MessageAction.Ask, SymuYellowPages.Help,
                _agent.Id.ClassKey, _agent.Id.ClassKey));
            Assert.IsFalse(task.IsBlocked);
        }

        /// <summary>
        ///     With teammates with beliefs
        /// </summary>
        [TestMethod]
        public void TryRecoverBlockerIncompleteBeliefTest()
        {
            // Add teammates with knowledge
            var teammate = AddTeammate();
            teammate.BeliefsModel.AddBelief(_knowledge.Id, BeliefLevel.StronglyAgree);
            teammate.BeliefsModel.InitializeBeliefs();
            _agent.BeliefsModel.GetBelief(_knowledge.Id).BeliefBits.SetBit(0, 0.1F);
            teammate.BeliefsModel.GetBelief(_knowledge.Id).BeliefBits.SetBit(0, 1F);
            _environment.WhitePages.Network.InteractionSphere.SetSphere(true,
                _environment.WhitePages.AllAgentIds().ToList(), _environment.WhitePages.Network);

            var task = new SymuTask(0) {Weight = 1, KeyActivity = 1};

            var blocker = new Blocker(Murphy.IncompleteBelief, 0)
            {
                Parameter = _knowledge.Id,
                Parameter2 = (byte) 0
            };
            _organizationEntity.Murphies.IncompleteBelief.CommunicationMediums = CommunicationMediums.Email;
            _agent.SetInitialCapacity();
            _agent.Capacity.Reset();
            _agent.TryRecoverBlockerIncompleteBelief(task, blocker);
            Assert.IsTrue(_environment.Messages.LastSentMessages.Exists(MessageAction.Ask, SymuYellowPages.Help,
                _agent.Id.ClassKey, _agent.Id.ClassKey));
            Assert.IsTrue(_agent.Capacity.Actual < 1);
            Assert.IsTrue(_agent.TimeSpent[task.KeyActivity] > 0);
        }


        /// <summary>
        ///     With DynamicEnvironment Off and no teammate with knowledge
        ///     WIth number of tries > LimitNumberOfTries
        /// </summary>
        [TestMethod]
        public void TryRecoverBlockerIncompleteBeliefTest1()
        {
            _organizationEntity.Murphies.IncompleteBelief.On = true;
            _organizationEntity.Murphies.IncompleteBelief.RateOfIncorrectGuess = 1;
            _organizationEntity.Murphies.IncompleteBelief.LimitNumberOfTries = 1;
            var task = new SymuTask(0) {Weight = 1, KeyActivity = 1};
            var blocker = new Blocker(Murphy.IncompleteBelief, 0)
            {
                Parameter = _knowledge2.Id,
                Parameter2 = (byte) 0,
                NumberOfTries = 2
            };
            _agent.TryRecoverBlockerIncompleteBelief(task, blocker);

            // Blocker must be unblocked in a way or another
            Assert.AreNotEqual(ImpactLevel.None, task.Incorrectness);
            Assert.IsTrue(task.Weight > 0);
            Assert.IsFalse(task.Blockers.IsBlocked);
        }

        #endregion

        #region Capacity & TimeSpent management

        /// <summary>
        ///     Send impact of the communication medium
        /// </summary>
        [TestMethod]
        public void ImpactOfTheCommunicationMediumOnCapacityTest()
        {
            _agent.Capacity.Set(1);
            _agent.ImpactOfTheCommunicationMediumOnTimeSpent(CommunicationMediums.Email, true, 1);
            Assert.IsTrue(_agent.TimeSpent[1] > 0);
        }

        /// <summary>
        ///     Receive impact of the communication medium
        /// </summary>
        [TestMethod]
        public void ImpactOfTheCommunicationMediumOnCapacityTest1()
        {
            _agent.Capacity.Set(1);
            _agent.ImpactOfTheCommunicationMediumOnTimeSpent(CommunicationMediums.Email, false, 1);
            Assert.IsTrue(_agent.TimeSpent[1] > 0);
        }

        [TestMethod]
        public void AddTimeSpentTest()
        {
            _agent.AddTimeSpent(1, 1);
            Assert.AreEqual(1, _agent.TimeSpent[1]);
            _agent.AddTimeSpent(1, 1);
            Assert.AreEqual(2, _agent.TimeSpent[1]);
            _agent.AddTimeSpent(2, 1);
            Assert.AreEqual(2, _agent.TimeSpent[1]);
            Assert.AreEqual(1, _agent.TimeSpent[2]);
        }

        #endregion
    }
}