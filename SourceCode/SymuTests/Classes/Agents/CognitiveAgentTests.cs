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
using Symu.Classes.Task;
using Symu.Common;
using Symu.Common.Classes;
using Symu.Common.Interfaces;
using Symu.Messaging.Messages;
using Symu.OrgMod.Edges;
using Symu.OrgMod.Entities;
using Symu.OrgMod.GraphNetworks.TwoModesNetworks.Sphere;
using Symu.Repository;
using Symu.Repository.Edges;
using Symu.Repository.Entities;
using Symu.Results.Blockers;
using SymuTests.Helpers;

#endregion

namespace SymuTests.Classes.Agents
{
    [TestClass]
    public class CognitiveAgentTests : BaseTestClass
    {
        private TestCognitiveAgent _agent;
        private TestCognitiveAgent _agent2;
        private Belief _belief;
        private Knowledge _knowledge;

        [TestInitialize]
        public void Initialize()
        {
            //Organization
            MainOrganization.Models.SetOn(1);
            _knowledge = new Knowledge(MainOrganization.MetaNetwork, MainOrganization.Models, "1", 1);
            _belief = _knowledge.AssociatedBelief;

            Environment.SetOrganization(MainOrganization);
            Environment.IterationResult.On();

            // At this point use _environment.Organization.MetaNetwork
            Simulation.Initialize(Environment);

            _agent = TestCognitiveAgent.CreateInstance(Environment);

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
            _agent.InfluenceModel.Influentialness = 1;
            _agent.InfluenceModel.Influenceability = 1;
            _agent.KnowledgeModel.AddKnowledge(_knowledge.EntityId, new float[] {1}, 0, -1);
            _agent.BeliefsModel.AddBelief(_belief.EntityId);
            _agent.Start();

            _agent2 = TestCognitiveAgent.CreateInstance(Environment);
            _agent2.Start();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        [TestMethod]
        public void AgentTest()
        {
            Assert.AreEqual(AgentState.Started, _agent.State);
            Assert.AreEqual(1, UId.Cast(_agent.AgentId.Id));
            Assert.IsNotNull(_agent.Environment);
            Assert.IsNotNull(_agent.Cognitive);
            Assert.AreNotEqual(0, _agent.KnowledgeModel.Expertise.Count);
            Assert.AreNotEqual(0, _agent.InfluenceModel.Influenceability);
            Assert.AreNotEqual(0, _agent.InfluenceModel.Influentialness);
            Assert.AreEqual(AgentState.Started, _agent.State);
        }

        private TestCognitiveAgent AddAgent()
        {
            var agent = TestCognitiveAgent.CreateInstance(Environment);
            agent.Cognitive.InteractionPatterns.LimitNumberOfNewInteractions = false;
            agent.Cognitive.InteractionPatterns.ThresholdForNewInteraction = 1;
            agent.Start();
            agent.WaitingToStart();
            return agent;
        }

        [TestMethod]
        public void StartTest()
        {
            var agent = AddAgent();
            Assert.AreEqual(AgentState.Started, agent.State);
            Assert.IsNotNull(agent.TaskProcessor);
        }


        [TestMethod]
        public void HasEmailTest()
        {
            Assert.IsNull(_agent.Email);
            Assert.IsFalse(_agent.HasEmail);
            var email = EmailEntity.CreateInstance(Environment.MainOrganization.MetaNetwork, MainOrganization.Models);
            var usage = new ResourceUsage(0);
            _ = new ActorResource(Environment.MainOrganization.MetaNetwork.ActorResource, _agent.AgentId, email.EntityId, usage);
            _agent.EmailId = email.EntityId;
            Assert.IsTrue(_agent.HasEmail);
            Assert.IsNotNull(_agent.Email);
        }

        #region message

        [TestMethod]
        public void OnBeforeSendMessageTest()
        {
            var message = new Message
            {
                Sender = _agent.AgentId,
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

        #region Post message

        /// <summary>
        ///     Without limit messages per period
        /// </summary>
        [TestMethod]
        public void PostMessageTest()
        {
            _agent.Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod = false;
            var message = new Message
            {
                Sender = _agent.AgentId,
                Receiver = _agent.AgentId,
                Medium = CommunicationMediums.Email
            };
            _agent.PostMessage(message);
            Assert.AreEqual<uint>(1, Environment.Messages.Result.ReceivedMessagesCount);
            Assert.AreEqual(1, Environment.Messages.LastSentMessages.Count);
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
                Sender = _agent.AgentId,
                Medium = CommunicationMediums.Email
            };
            _agent.PostMessage(message);
            Assert.AreEqual<uint>(0, Environment.Messages.Result.SentMessagesCount);
            Assert.AreEqual(0, Environment.Messages.LastSentMessages.Count);
            Assert.AreEqual(1, _agent.MessageProcessor.MissedMessages.Count);
            Assert.AreEqual(0, _agent.MessageProcessor.NumberReceivedPerPeriod);
        }

        /// <summary>
        ///     Message.Medium == system
        /// </summary>
        [TestMethod]
        public void LearnKnowledgesFromPostMessageTest()
        {
            _agent.KnowledgeModel.SetKnowledge(_knowledge.EntityId, 0, 0, 0);
            var bit1S = new KnowledgeBits(new float[] {1}, 0, -1);
            var attachments = new MessageAttachments
            {
                KnowledgeId = _knowledge.EntityId,
                KnowledgeBits = bit1S
            };
            _agent.LearningModel.On = true;
            _agent.LearningModel.RateOfAgentsOn = 1;
            _agent.Cognitive.InternalCharacteristics.CanLearn = true;
            _agent.Cognitive.TasksAndPerformance.LearningRate = 1;
            var message = new Message(_agent.AgentId, _agent.AgentId, MessageAction.Ask, 0, attachments,
                CommunicationMediums.System);

            _agent.LearnKnowledgesFromPostMessage(message);
            Assert.AreEqual(0, _agent.KnowledgeModel.GetActorKnowledge(_knowledge.EntityId).GetKnowledgeSum());
        }

        /// <summary>
        ///     Message.Medium = email
        /// </summary>
        [TestMethod]
        public void LearnKnowledgesFromPostMessageTest1()
        {
            _agent.KnowledgeModel.SetKnowledge(_knowledge.EntityId, 0, 0, 0);
            var bit1S = new KnowledgeBits(new float[] {1}, 0, -1);
            var attachments = new MessageAttachments
            {
                KnowledgeId = _knowledge.EntityId,
                KnowledgeBits = bit1S
            };
            _agent.LearningModel.On = true;
            _agent.LearningModel.RateOfAgentsOn = 1;
            _agent.LearningModel.TasksAndPerformance.LearningRate = 1;
            var message = new Message(_agent.AgentId, _agent.AgentId, MessageAction.Ask, 0, attachments,
                CommunicationMediums.Email);
            _agent.LearnKnowledgesFromPostMessage(message);
            Assert.AreEqual(1, _agent.KnowledgeModel.GetActorKnowledge(_knowledge.EntityId).GetKnowledgeSum());
        }

        [TestMethod]
        public void LearnBeliefsFromPostMessageTest()
        {
            var bit1S = new Bits(new float[] {1}, 0);
            var attachments = new MessageAttachments
            {
                KnowledgeId = _knowledge.EntityId,
                KnowledgeBits = bit1S,
                BeliefBits = bit1S
            };
            var message = new Message(_agent.AgentId, _agent.AgentId, MessageAction.Ask, 0, attachments,
                CommunicationMediums.Email);
            _agent.LearnBeliefsFromPostMessage(message);
            Assert.AreEqual(1, _agent.BeliefsModel.GetActorBelief(_belief.EntityId).GetBeliefSum());
        }


        private void SetExpertise(KnowledgeBits bit0S)
        {
            _agent.Cognitive.KnowledgeAndBeliefs.HasKnowledge = true;
            var knowledge = new Knowledge(Environment.MainOrganization.MetaNetwork, MainOrganization.Models, "1", 1);
            _ = new ActorKnowledge(Environment.MainOrganization.MetaNetwork.ActorKnowledge, _agent.AgentId, knowledge.EntityId, bit0S);
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
            var agent2 = AddAgent();
            var message = new Message(_agent.AgentId, agent2.AgentId, MessageAction.Add, 0)
            {
                Sender = _agent.AgentId,
                Medium = CommunicationMediums.Email
            };
            _agent.Send(message);
            Environment.Messages.WaitingToClearAllMessages();
            //environment trace the message
            Assert.IsTrue(Environment.Messages.MessagesReceivedByAgent(0, agent2.AgentId).Any());
            Assert.IsTrue(Environment.Messages.MessagesSentByAgent(0, _agent.AgentId).Any());
            Assert.IsTrue(Environment.Messages.LastSentMessages.Any);
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
            var message = new Message(_agent.AgentId, _agent2.AgentId, MessageAction.Add, 0)
            {
                Medium = CommunicationMediums.Email
            };
            _agent.Send(message);
            Environment.Messages.WaitingToClearAllMessages();
            //environment trace the message
            Assert.IsNull(Environment.Messages.MessagesReceivedByAgent(0, _agent2.AgentId));
            Assert.IsNull(Environment.Messages.MessagesSentByAgent(0, _agent.AgentId));
            Assert.IsFalse(Environment.Messages.LastSentMessages.Any);
            // Agent1
            Assert.AreEqual(0, _agent.MessageProcessor.NumberMessagesPerStep);
        }

        #endregion

        #region reply

        [TestMethod]
        public void ReplyTest()
        {
            var attachments = new MessageAttachments
            {
                KnowledgeId = _knowledge.EntityId,
                KnowledgeBit = 0
            };
            var message = new Message(_agent.AgentId, _agent.AgentId, MessageAction.Ask, 0, attachments)
            {
                Medium = CommunicationMediums.Email
            };
            // Knowledge
            _agent.Cognitive.MessageContent.CanSendKnowledge = true;
            var bits = new KnowledgeBits(new float[] {1}, 0, -1);
            SetExpertise(bits);
            _agent.KnowledgeModel.SetKnowledge(_knowledge.EntityId, 0, 1, 0);
            // Belief
            _agent.Cognitive.MessageContent.CanSendBeliefs = true;
            _agent.BeliefsModel.SetBelief(_belief.EntityId, 0, 1);

            _agent.Reply(message);

            Assert.AreEqual(1, _agent.MessageProcessor.NumberSentPerPeriod);
            var messageSent = Environment.Messages.LastSentMessages.SentByAgent(0, _agent.AgentId).Last();
            Assert.IsNotNull(messageSent);
            Assert.IsNotNull(messageSent.Attachments.KnowledgeBits);
            Assert.IsNotNull(messageSent.Attachments.BeliefBits);
        }

        [TestMethod]
        public void ReplyDelayedTest()
        {
            var attachments = new MessageAttachments
            {
                KnowledgeId = _knowledge.EntityId,
                KnowledgeBit = 0
            };
            var message = new Message(_agent.AgentId, _agent.AgentId, MessageAction.Ask, 0, attachments)
            {
                Medium = CommunicationMediums.Email
            };
            // Knowledge
            _agent.Cognitive.MessageContent.CanSendKnowledge = true;
            var bits = new KnowledgeBits(new float[] {1}, 0, -1);
            SetExpertise(bits);
            _agent.KnowledgeModel.SetKnowledge(_knowledge.EntityId, 0, 1, 0);
            // Belief
            _agent.Cognitive.MessageContent.CanSendBeliefs = true;
            _agent.BeliefsModel.SetBelief(_belief.EntityId, 0, 1);
            _agent.ReplyDelayed(message, 0);

            Assert.AreEqual(1, _agent.MessageProcessor.NumberSentPerPeriod);
            var messageSent = Environment.Messages.DelayedMessages.Last(0);
            Assert.IsNotNull(messageSent);
            Assert.IsNotNull(messageSent.Attachments.KnowledgeBits);
            Assert.IsNotNull(messageSent.Attachments.BeliefBits);
        }

        #endregion

        #region Act

        [TestMethod]
        public void AskPreStepTest()
        {
            Environment.Schedule.Step = 0;
            Environment.MainOrganization.Murphies.UnAvailability.On = false;
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
            Environment.InitializeInteractionSphere();
            Environment.PreStep();
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
            Environment.InitializeInteractionSphere();
            Environment.PreStep();
            Assert.AreEqual(1, _agent.ForgettingModel.ForgettingExpertise.Count);
        }

        #endregion

        #region Capacity management

        /// <summary>
        ///     Non working day
        /// </summary>
        [TestMethod]
        public void NonPassingHandleCapacityTest()
        {
            Environment.Schedule.Step = 5;
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
            Environment.Schedule.Step = 0;
            _agent.Cognitive.TasksAndPerformance.CanPerformTask = true;
            Environment.MainOrganization.Murphies.UnAvailability.On = false;
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
            Environment.Schedule.Step = 0;
            _agent.Cognitive.TasksAndPerformance.CanPerformTask = true;
            Environment.MainOrganization.Murphies.UnAvailability.On = false;
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
            Environment.Schedule.Step = 0;
            _agent.Cognitive.TasksAndPerformance.CanPerformTask = true;
            Environment.MainOrganization.Murphies.UnAvailability.On = false;
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
            Environment.Schedule.Step = 0;
            _agent.Cognitive.TasksAndPerformance.CanPerformTask = true;
            Environment.MainOrganization.Murphies.UnAvailability.On = false;
            _agent.HandleCapacity(false, false);
            Assert.AreEqual(1, _agent.Capacity.Initial);
            Assert.AreEqual(0, _agent.Capacity.Actual);
        }

        #endregion

        #region status

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

        /// <summary>
        ///     Initial capacity >0 + delayed Post messages
        /// </summary>
        [TestMethod]
        public void HandleStatusTest2()
        {
            _agent.Capacity.Initial = 1;
            var message = new Message
            {
                Sender = _agent.AgentId,
                Receiver = _agent.AgentId,
                Medium = CommunicationMediums.System
            };
            _agent.MessageProcessor.DelayedMessages.Enqueue(message, 0);
            _agent.HandleStatus(false);
            Assert.IsTrue(Environment.Messages.LastSentMessages.Count >= 1);
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
            var agentIds = new List<IAgentId>();
            Assert.AreEqual(0, _agent.FilterAgentIdsToInteract(agentIds).Count());
        }

        /// <summary>
        ///     With filled list - limit false
        /// </summary>
        [TestMethod]
        public void FilterAgentIdsToInteractTest1()
        {
            var agentIds = new List<IAgentId>();
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
            var agentIds = new List<IAgentId>();
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
            var agentIds = new List<IAgentId>();
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
            Assert.IsTrue(_agent.AcceptNewInteraction(_agent2.AgentId));
        }

        /// <summary>
        ///     IsPartOfInteractionSphere true true
        ///     ActiveLink
        /// </summary>
        [TestMethod]
        public void AcceptNewInteractionTest1()
        {
            _agent.Cognitive.InteractionPatterns.IsPartOfInteractionSphere = true;
            _ = new ActorActor(Environment.MainOrganization.MetaNetwork.ActorActor, _agent.AgentId, _agent2.AgentId);
            Assert.IsTrue(_agent.AcceptNewInteraction(_agent2.AgentId));
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
            Assert.IsFalse(_agent.AcceptNewInteraction(_agent2.AgentId));
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
            Assert.IsFalse(_agent.AcceptNewInteraction(_agent2.AgentId));
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
            Assert.IsFalse(_agent.AcceptNewInteraction(_agent2.AgentId));
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
            Assert.IsTrue(_agent.AcceptNewInteraction(_agent2.AgentId));
        }

        /// <summary>
        ///     model off
        /// </summary>
        [TestMethod]
        public void GetAgentIdsForNewInteractionsTest()
        {
            Environment.MainOrganization.MetaNetwork.InteractionSphere.Model.On = false;
            Assert.IsFalse(_agent.GetAgentIdsForNewInteractions().Any());
        }

        /// <summary>
        ///     model on - sphereUpdate false
        /// </summary>
        [TestMethod]
        public void GetAgentIdsForNewInteractionsTest1()
        {
            Environment.MainOrganization.MetaNetwork.InteractionSphere.Model.SphereUpdateOverTime = false;
            for (var i = 0; i < 2; i++)
            {
                _ = TestCognitiveAgent.CreateInstance(Environment);
            }

            Environment.MainOrganization.MetaNetwork.InteractionSphere.SetSphere(true,
                Environment.WhitePages.AllAgentIds().ToList(), Environment.MainOrganization.MetaNetwork);
            Assert.IsFalse(_agent.GetAgentIdsForNewInteractions().Any());
        }

        /// <summary>
        ///     model on
        /// </summary>
        [TestMethod]
        public void GetAgentIdsForNewInteractionsTest2()
        {
            Environment.MainOrganization.MetaNetwork.InteractionSphere.Model.SphereUpdateOverTime = true;
            for (var i = 0; i < 2; i++)
            {
                _ = TestCognitiveAgent.CreateInstance(Environment);
            }

            Environment.MainOrganization.MetaNetwork.InteractionSphere.SetSphere(true,
                Environment.WhitePages.AllAgentIds().ToList(), Environment.MainOrganization.MetaNetwork);
            Assert.IsTrue(_agent.GetAgentIdsForNewInteractions().Any());
        }

        /// <summary>
        ///     Model off
        /// </summary>
        [TestMethod]
        public void GetAgentIdsForInteractionsTest()
        {
            Environment.MainOrganization.MetaNetwork.InteractionSphere.Model.SphereUpdateOverTime = false;
            Assert.IsFalse(_agent.GetAgentIdsForInteractions(InteractionStrategy.Homophily).Any());
        }

        /// <summary>
        ///     Model on - list empty
        /// </summary>
        [TestMethod]
        public void GetAgentIdsForInteractionsTest1()
        {
            Assert.IsFalse(_agent.GetAgentIdsForInteractions(InteractionStrategy.Homophily).Any());
        }

        /// <summary>
        ///     Model on - list empty
        /// </summary>
        [TestMethod]
        public void GetAgentIdsForInteractionsTest2()
        {
            _ = new ActorActor(Environment.MainOrganization.MetaNetwork.ActorActor, _agent.AgentId, _agent2.AgentId);
            Environment.MainOrganization.MetaNetwork.InteractionSphere.SetSphere(true,
                Environment.WhitePages.AllAgentIds().ToList(), Environment.MainOrganization.MetaNetwork);
            Assert.IsTrue(_agent.GetAgentIdsForInteractions(InteractionStrategy.Homophily).Any());
        }

        #endregion

        #region Blockers & help

        /// <summary>
        ///     CHeck MultipleBlockers false
        /// </summary>
        [TestMethod]
        public void CheckBlockersTest()
        {
            Environment.IterationResult.Initialize();
            MainOrganization.Murphies.MultipleBlockers = false;
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
                Sender = _agent.AgentId,
                Receiver = _agent.AgentId,
                Action = MessageAction.Ask,
                Subject = SymuYellowPages.Help,
                Attachments = parameter
            };

            // To be sure to send a delayed message
            MainOrganization.Murphies.IncompleteInformation.ResponseTime = 10;
            MainOrganization.Murphies.IncompleteInformation.RateOfAnswers = 0;
            _agent.AskHelp(message);
            Assert.AreEqual(0, Environment.Messages.DelayedMessages.Count);
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
                Sender = _agent.AgentId,
                Receiver = _agent.AgentId,
                Action = MessageAction.Ask,
                Subject = SymuYellowPages.Help,
                Attachments = parameter
            };

            // To be sure to send a delayed message
            MainOrganization.Murphies.IncompleteInformation.On = true;
            MainOrganization.Murphies.IncompleteInformation.RateOfAgentsOn = 1;
            MainOrganization.Murphies.IncompleteInformation.ResponseTime = 10;
            MainOrganization.Murphies.IncompleteInformation.RateOfAnswers = 1;
            _agent.AskHelp(message);
            Assert.AreEqual(1, Environment.Messages.DelayedMessages.Count);
        }

        /// <summary>
        ///     Murphies.IncompleteBelief.Off
        /// </summary>
        [TestMethod]
        public void CheckBeliefsBitsTest()
        {
            var task = new SymuTask(0);
            task.SetKnowledgesBits(_agent.Cognitive.TasksAndPerformance.TaskModel, Knowledges,
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
            task.SetKnowledgesBits(_agent.Cognitive.TasksAndPerformance.TaskModel, Knowledges,
                MurphyTask.NoRequiredBits);
            MainOrganization.Murphies.IncompleteBelief.On = true;
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
            MainOrganization.Murphies.IncompleteBelief.On = true;
            MainOrganization.Murphies.IncompleteBelief.MandatoryRatio = 1;
            var task = new SymuTask(0);
            task.SetKnowledgesBits(_agent.Cognitive.TasksAndPerformance.TaskModel, Knowledges,
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
                KeyActivity = Uid1
            };
            var attachments = new MessageAttachments();
            attachments.Add(0);
            attachments.Add(task);
            attachments.KnowledgeId = _knowledge.EntityId;
            attachments.KnowledgeBit = 0;
            return new Message(_agent.AgentId, _agent.AgentId, MessageAction.Ask, SymuYellowPages.Help,
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
            MainOrganization.Murphies.IncompleteBelief.RateOfAnswers = 0;
            _agent.AskHelpIncomplete(message,
                Environment.MainOrganization.Murphies.IncompleteBelief.DelayToReplyToHelp());
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
            MainOrganization.Murphies.IncompleteBelief.RateOfAnswers = 1;
            MainOrganization.Murphies.IncompleteBelief.ResponseTime = 0;
            _agent.SetInitialCapacity();
            _agent.Capacity.Reset();
            // no belief
            _agent.BeliefsModel.SetBelief(_belief.EntityId, 0, 0);
            _agent.AskHelpIncomplete(message,
                Environment.MainOrganization.Murphies.IncompleteBelief.DelayToReplyToHelp());
            Assert.IsTrue(_agent.Capacity.Initial > _agent.Capacity.Actual);
            Assert.IsTrue(Environment.Messages.LastSentMessages.Exists(MessageAction.Reply, SymuYellowPages.Help,
                _agent.AgentId.ClassId, _agent.AgentId.ClassId));
            var reply = Environment.Messages.LastSentMessages.SentByAgent(0, _agent.AgentId).Last();
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
            MainOrganization.Murphies.IncompleteBelief.RateOfAnswers = 1;
            MainOrganization.Murphies.IncompleteBelief.ResponseTime = 10;
            _agent.SetInitialCapacity();
            _agent.Capacity.Reset();
            // no belief
            _agent.BeliefsModel.SetBelief(_belief.EntityId, 0, 0);
            _agent.AskHelpIncomplete(message, 3);
            Assert.IsTrue(_agent.Capacity.Initial > _agent.Capacity.Actual);
            Assert.IsFalse(Environment.Messages.LastSentMessages.Exists(MessageAction.Reply, SymuYellowPages.Help,
                _agent.AgentId.ClassId, _agent.AgentId.ClassId));
            var reply = Environment.Messages.DelayedMessages.Last();
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
            MainOrganization.Murphies.IncompleteBelief.RateOfAnswers = 1;
            MainOrganization.Murphies.IncompleteBelief.ResponseTime = 0;
            _agent.SetInitialCapacity();
            _agent.Capacity.Reset();
            _agent.BeliefsModel.SetBelief(_belief.EntityId, 0, 1);
            _agent.AskHelpIncomplete(message,
                Environment.MainOrganization.Murphies.IncompleteBelief.DelayToReplyToHelp());
            Assert.IsTrue(_agent.Capacity.Initial > _agent.Capacity.Actual);
            Assert.IsTrue(Environment.Messages.LastSentMessages.Exists(MessageAction.Reply, SymuYellowPages.Help,
                _agent.AgentId.ClassId, _agent.AgentId.ClassId));
            var reply = Environment.Messages.LastSentMessages.SentByAgent(0, _agent.AgentId).Last();
            Assert.IsNotNull(reply.Attachments.BeliefBits);
        }

        /// <summary>
        ///     Task weight = 1
        /// </summary>
        [TestMethod]
        public void RecoverBlockerBeliefByGuessingTest()
        {
            MainOrganization.Murphies.IncompleteBelief.On = true;
            MainOrganization.Murphies.IncompleteBelief.RateOfIncorrectGuess = 1;
            Environment.MainOrganization.Models.Generator = RandomGenerator.RandomUniform;
            var task = new SymuTask(0) {Weight = 1};
            task.SetKnowledgesBits(_agent.Cognitive.TasksAndPerformance.TaskModel, Knowledges,
                MurphyTask.FullRequiredBits);
            Environment.MainOrganization.Murphies.IncompleteKnowledge.CostFactorOfGuessing = 2;

            var actorBelief = _agent.BeliefsModel.GetActorBelief(_belief.EntityId);
            actorBelief.BeliefBits.SetBit(0, 0);
            var blocker = new Blocker(1, 0) {Parameter = _knowledge.EntityId, Parameter2 = (byte) 0};

            _agent.RecoverBlockerIncompleteBeliefByGuessing(task, blocker);
            Assert.AreNotEqual(ImpactLevel.None, task.Incorrectness);
            if (task.Incorrectness == ImpactLevel.Blocked)
            {
                return;
            }

            Assert.AreNotEqual(0, actorBelief.BeliefBits.GetBit(0));
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
                Sender = _agent.AgentId,
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
            var task = new SymuTask(0) {Assigned = _agent.AgentId};
            var blocker = task.Add(Murphy.IncompleteBelief, 0);
            var message = new Message
            {
                Sender = _agent.AgentId,
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
            MainOrganization.Murphies.IncompleteInformation.On = false;
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
            MainOrganization.Murphies.IncompleteInformation.On = true;
            MainOrganization.Murphies.IncompleteInformation.ThresholdForReacting = 0;
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
            MainOrganization.Murphies.IncompleteInformation.On = true;
            MainOrganization.Murphies.IncompleteInformation.ThresholdForReacting = 1;
            var task = new SymuTask(0) {Assigned = _agent.AgentId};
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
            MainOrganization.Murphies.IncompleteInformation.On = true;
            MainOrganization.Murphies.IncompleteInformation.ThresholdForReacting = 1;
            var task = new SymuTask(0) {Weight = 0, Assigned = _agent.AgentId};
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
            MainOrganization.Murphies.IncompleteInformation.On = true;
            MainOrganization.Murphies.IncompleteInformation.ThresholdForReacting = 1;
            var task = new SymuTask(0) {Weight = 1, Assigned = _agent.AgentId, Creator = _agent2.AgentId};
            Assert.IsTrue(_agent.CheckBlockerIncompleteInformation(task));
        }

        /// <summary>
        ///     Model IncompleteInformation on
        ///     Rate 1
        ///     with creator of task == null
        ///     With RAF > 0
        /// </summary>
        [TestMethod]
        public void CheckIncompleteInformationTests5()
        {
            MainOrganization.Murphies.IncompleteInformation.On = true;
            MainOrganization.Murphies.IncompleteInformation.ThresholdForReacting = 1;
            var task = new SymuTask(0) {Weight = 1, Assigned = _agent.AgentId};
            Assert.IsFalse(_agent.CheckBlockerIncompleteInformation(task));
        }

        /// <summary>
        ///     With teammates
        /// </summary>
        [TestMethod]
        public void TryRecoverBlockerIncompleteKnowledgeTest()
        {
            // Add teammates with knowledge
            var teammate = AddAgent();
            var group = TestCognitiveAgent.CreateInstance(2, Environment);
            group.Start();
            _ = new ActorOrganization(Environment.MainOrganization.MetaNetwork.ActorOrganization, _agent.AgentId, group.AgentId);
            _ = new ActorOrganization(Environment.MainOrganization.MetaNetwork.ActorOrganization, teammate.AgentId, group.AgentId);
            _ = new ActorActor(Environment.MainOrganization.MetaNetwork.ActorActor, _agent.AgentId, teammate.AgentId);
            teammate.KnowledgeModel.AddKnowledge(_knowledge.EntityId, KnowledgeLevel.FullKnowledge, 0, -1);
            teammate.KnowledgeModel.InitializeExpertise(0);
            _agent.KnowledgeModel.SetKnowledge(_knowledge.EntityId, 0, 1, 0);
            Environment.MainOrganization.MetaNetwork.InteractionSphere.SetSphere(true,
                Environment.WhitePages.AllAgentIds().ToList(), Environment.MainOrganization.MetaNetwork);

            var task = new SymuTask(0) {KeyActivity = Uid1};

            var blocker = new Blocker(Murphy.IncompleteKnowledge, 0)
            {
                Parameter = _knowledge.EntityId,
                Parameter2 = (byte) 0
            };
            MainOrganization.Murphies.IncompleteKnowledge.CommunicationMediums = CommunicationMediums.Email;
            _agent.SetInitialCapacity();
            _agent.Capacity.Reset();
            _agent.TryRecoverBlockerIncompleteKnowledge(task, blocker);
            Assert.IsTrue(Environment.Messages.LastSentMessages.Exists(MessageAction.Ask, SymuYellowPages.Help,
                _agent.AgentId.ClassId, _agent.AgentId.ClassId));
            Assert.IsTrue(_agent.Capacity.Actual < 1);
            Assert.IsTrue(_agent.TimeSpent[task.KeyActivity] > 0);
        }

        /// <summary>
        ///     Murphies.IncompleteKnowledge.Off
        /// </summary>
        [TestMethod]
        public void CheckKnowledgesBitsTest()
        {
            MainOrganization.Murphies.IncompleteKnowledge.On = false;
            var task = new SymuTask(0) {Weight = 1};
            task.SetKnowledgesBits(_agent.Cognitive.TasksAndPerformance.TaskModel, Knowledges,
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
            MainOrganization.Murphies.IncompleteKnowledge.On = true;
            var task = new SymuTask(0) {Weight = 1};
            task.SetKnowledgesBits(_agent.Cognitive.TasksAndPerformance.TaskModel, Knowledges,
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
            MainOrganization.Murphies.IncompleteKnowledge.On = true;
            var task = new SymuTask(0) {Weight = 1, Assigned = _agent.AgentId};
            task.SetTasksManager(_agent.TaskProcessor.TasksManager);
            task.SetKnowledgesBits(_agent.Cognitive.TasksAndPerformance.TaskModel, Knowledges,
                MurphyTask.FullRequiredBits);
            _agent.KnowledgeModel.SetKnowledge(_knowledge.EntityId, 0, 0, 0);
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
            MainOrganization.Murphies.IncompleteKnowledge.On = true;
            MainOrganization.Murphies.IncompleteKnowledge.MandatoryRatio = 0;
            MainOrganization.Murphies.IncompleteKnowledge.RequiredRatio = 1;
            MainOrganization.Murphies.IncompleteKnowledge.RateOfIncorrectGuess = 1;
            var task = new SymuTask(0) {Weight = 1, Assigned = _agent.AgentId};
            task.SetKnowledgesBits(MainOrganization.Murphies.IncompleteKnowledge, Knowledges, 1);
            _agent.Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge = false;
            _agent.KnowledgeModel.AddKnowledge(_knowledge.EntityId, KnowledgeLevel.FullKnowledge, 0, -1);
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
            MainOrganization.Murphies.IncompleteKnowledge.On = true;
            MainOrganization.Murphies.IncompleteKnowledge.MandatoryRatio = 0;
            MainOrganization.Murphies.IncompleteKnowledge.RateOfIncorrectGuess = 1;
            var task = new SymuTask(0) {Weight = 1, Assigned = _agent.AgentId};
            task.SetKnowledgesBits(MainOrganization.Murphies.IncompleteKnowledge, Knowledges,
                MurphyTask.FullRequiredBits);
            _agent.KnowledgeModel.SetKnowledge(_knowledge.EntityId, 0, 1, 0);
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
            MainOrganization.Murphies.IncompleteKnowledge.On = true;
            MainOrganization.Murphies.IncompleteKnowledge.RateOfIncorrectGuess = 1;
            MainOrganization.Murphies.IncompleteKnowledge.CostFactorOfGuessing = 2;
            var task = new SymuTask(0) {Weight = 1};
            task.SetKnowledgesBits(MainOrganization.Murphies.IncompleteKnowledge, Knowledges,
                MurphyTask.FullRequiredBits);

            _agent.KnowledgeModel.SetKnowledge(_knowledge.EntityId, 0, 0, 0);
            _agent.RecoverBlockerIncompleteKnowledgeByGuessing(task, null, _knowledge.EntityId, 0,
                BlockerResolution.Guessing);
            Assert.AreNotEqual(ImpactLevel.None, task.Incorrectness);
            if (task.HasBeenCancelledBy.Any())
            {
                return;
            }

            Assert.IsTrue(1 < task.Weight);
            var actorKnowledge =
                _agent.KnowledgeModel.GetActorKnowledge(_knowledge.EntityId);
            Assert.AreEqual(_agent.LearningModel.TasksAndPerformance.LearningByDoingRate,
                actorKnowledge.GetKnowledgeBit(0));
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
                Creator = _agent.AgentId
            };
            var blocker = task.Add(Murphy.IncompleteKnowledge, 0, (ushort) 0);
            _agent.TryRecoverBlockerIncompleteInformation(task, blocker);
            Assert.IsTrue(Environment.Messages.LastSentMessages.Exists(MessageAction.Ask, SymuYellowPages.Help,
                _agent.AgentId.ClassId, _agent.AgentId.ClassId));
            Assert.IsTrue(task.IsBlocked);
        }

        /// <summary>
        ///     With Guess
        /// </summary>
        [TestMethod]
        public void TryRecoverBlockerIncompleteInformationTest1()
        {
            MainOrganization.Murphies.IncompleteInformation.On = true;
            MainOrganization.Murphies.IncompleteInformation.LimitNumberOfTries = 1;
            var task = new SymuTask(0) {Weight = 1};
            var blocker = task.Add(Murphy.IncompleteKnowledge, 0, (ushort) 0);
            blocker.NumberOfTries = 2;
            _agent.TryRecoverBlockerIncompleteInformation(task, blocker);
            Assert.IsFalse(Environment.Messages.LastSentMessages.Exists(MessageAction.Ask, SymuYellowPages.Help,
                _agent.AgentId.ClassId, _agent.AgentId.ClassId));
            Assert.IsFalse(task.IsBlocked);
        }

        /// <summary>
        ///     With teammates with beliefs
        /// </summary>
        [TestMethod]
        public void TryRecoverBlockerIncompleteBeliefTest()
        {
            // Add teammates with knowledge
            var teammate = AddAgent();
            teammate.BeliefsModel.AddBeliefFromKnowledgeId(_knowledge.EntityId, BeliefLevel.StronglyAgree);
            teammate.BeliefsModel.InitializeBeliefs();
            _agent.BeliefsModel.SetBelief(_belief.EntityId, 0, 0.1F);
            teammate.BeliefsModel.SetBelief(_belief.EntityId, 0, 1F);
            Environment.MainOrganization.MetaNetwork.InteractionSphere.SetSphere(true,
                Environment.WhitePages.AllAgentIds().ToList(), Environment.MainOrganization.MetaNetwork);

            var task = new SymuTask(0) {Weight = 1, KeyActivity = Uid1};

            var blocker = new Blocker(Murphy.IncompleteBelief, 0)
            {
                Parameter = _knowledge.EntityId,
                Parameter2 = (byte) 0
            };
            MainOrganization.Murphies.IncompleteBelief.CommunicationMediums = CommunicationMediums.Email;
            _agent.SetInitialCapacity();
            _agent.Capacity.Reset();
            _agent.TryRecoverBlockerIncompleteBelief(task, blocker);
            Assert.IsTrue(Environment.Messages.LastSentMessages.Exists(MessageAction.Ask, SymuYellowPages.Help,
                _agent.AgentId.ClassId, _agent.AgentId.ClassId));
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
            var knowledge2 = new Knowledge(Environment.MainOrganization.MetaNetwork,
                Environment.MainOrganization.Models, "2", 1);
            _ = new Belief(Environment.MainOrganization.MetaNetwork, knowledge2, knowledge2.Length,
                MainOrganization.Models.Generator, MainOrganization.Models.BeliefWeightLevel);

            MainOrganization.Murphies.IncompleteBelief.On = true;
            MainOrganization.Murphies.IncompleteBelief.RateOfIncorrectGuess = 1;
            MainOrganization.Murphies.IncompleteBelief.LimitNumberOfTries = 1;
            var task = new SymuTask(0) {Weight = 1, KeyActivity = Uid1};
            var blocker = new Blocker(Murphy.IncompleteBelief, 0)
            {
                Parameter = knowledge2.EntityId,
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
            _agent.ImpactOfTheCommunicationMediumOnTimeSpent(CommunicationMediums.Email, true, Uid1);
            Assert.IsTrue(_agent.TimeSpent[Uid1] > 0);
        }

        /// <summary>
        ///     Receive impact of the communication medium
        /// </summary>
        [TestMethod]
        public void ImpactOfTheCommunicationMediumOnCapacityTest1()
        {
            _agent.Capacity.Set(1);
            _agent.ImpactOfTheCommunicationMediumOnTimeSpent(CommunicationMediums.Email, false, Uid1);
            Assert.IsTrue(_agent.TimeSpent[Uid1] > 0);
        }

        [TestMethod]
        public void AddTimeSpentTest()
        {
            _agent.AddTimeSpent(Uid1, 1);
            Assert.AreEqual(1, _agent.TimeSpent[Uid1]);
            _agent.AddTimeSpent(Uid1, 1);
            Assert.AreEqual(2, _agent.TimeSpent[Uid1]);
            var uid2 = new AgentId(2, 2);
            _agent.AddTimeSpent(uid2, 1);
            Assert.AreEqual(2, _agent.TimeSpent[Uid1]);
            Assert.AreEqual(1, _agent.TimeSpent[uid2]);
        }

        #endregion
    }
}