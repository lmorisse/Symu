#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Common;
using Symu.Common.Interfaces;
using Symu.Messaging.Messages;
using Symu.Repository;
using SymuTests.Helpers;

#endregion

namespace SymuTests.Classes.Agents
{
    [TestClass]
    public class ReactiveAgentTests : BaseTestClass
    {
        private TestReactiveAgent _agent;

        [TestInitialize]
        public void Initialize()
        {
            MainOrganization.Models.SetOn(1);
            Environment.SetOrganization(MainOrganization);
            Environment.IterationResult.On();

            _agent = TestReactiveAgent.CreateInstance(Environment);
            Assert.AreEqual(AgentState.NotStarted, _agent.State);

            Simulation.Initialize(Environment);

            Assert.AreEqual(AgentState.Started, _agent.State);
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        [TestMethod]
        public void AgentTest()
        {
            Assert.AreEqual(1, UId.Cast(_agent.AgentId.Id));
            Assert.IsNotNull(_agent.Environment);
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
            _agent.OnBeforeSendMessage(message);
            Assert.AreEqual<uint>(1, Environment.Messages.Result.SentMessagesCount);
            Assert.AreEqual<uint>(1, Environment.Messages.Result.SentMessagesByEmail);
            Assert.IsTrue(Environment.Messages.Result.SentMessagesCost > 0);
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
                Receiver = _agent.AgentId,
                Sender = _agent.AgentId,
                Medium = CommunicationMediums.Irc
            };
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(1, Environment.Messages.Result.ReceivedMessagesCount);
            message.Medium = CommunicationMediums.Email;
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(2, Environment.Messages.Result.ReceivedMessagesCount);
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
            Assert.AreEqual<uint>(0, Environment.Messages.Result.SentMessagesCount);
            message.Medium = CommunicationMediums.Email;
            _agent.Post(message);
            Assert.AreEqual(2, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(0, Environment.Messages.Result.SentMessagesCount);
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
                Receiver = _agent.AgentId,
                Sender = _agent.AgentId,
                Medium = CommunicationMediums.Phone
            };
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(0, Environment.Messages.Result.ReceivedMessagesCount);
            message.Medium = CommunicationMediums.Meeting;
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(0, Environment.Messages.Result.ReceivedMessagesCount);
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
                Receiver = _agent.AgentId,
                Sender = _agent.AgentId,
                Medium = CommunicationMediums.Phone
            };
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(1, Environment.Messages.Result.ReceivedMessagesCount);
            message.Medium = CommunicationMediums.Meeting;
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(2, Environment.Messages.Result.ReceivedMessagesCount);
            //TODO test Missed messages
        }

        [TestMethod]
        public void PostDelayedMessagesTest()
        {
            var message = new Message
            {
                Receiver = _agent.AgentId,
                Sender = _agent.AgentId
            };
            // Post as a delayed message
            _agent.PostAsADelayedMessage(message, 0);
            Assert.AreEqual(1, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(0, Environment.Messages.Result.ReceivedMessagesCount);
            Assert.AreEqual(0, Environment.Messages.WaitingMessages.Count);
            // Post Delayed messages
            _agent.PostDelayedMessages();
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(1, Environment.Messages.Result.ReceivedMessagesCount);
            Assert.AreEqual(0, Environment.Messages.WaitingMessages.Count);
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
            Assert.AreEqual<uint>(0, Environment.Messages.Result.SentMessagesCount);
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
            Assert.AreEqual<uint>(0, Environment.Messages.Result.SentMessagesCount);

            _agent.State = AgentState.Starting;
            _agent.Post(message);
            Assert.AreEqual(2, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual<uint>(0, Environment.Messages.Result.SentMessagesCount);
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
                Receiver = _agent.AgentId,
                Sender = _agent.AgentId,
                Medium = CommunicationMediums.Email
            };
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual((uint) 1, Environment.Messages.Result.ReceivedMessagesCount);
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
                Receiver = _agent.AgentId,
                Sender = _agent.AgentId,
                Medium = CommunicationMediums.Email
            };
            _agent.Post(message);
            Assert.AreEqual(0, _agent.MessageProcessor.DelayedMessages.Count);
            Assert.AreEqual((uint) 1, Environment.Messages.Result.ReceivedMessagesCount);
        }

        #endregion

        #region subscription

        /// <summary>
        ///     One subscription
        /// </summary>
        [TestMethod]
        public void RemoveSubscribeTest()
        {
            _agent.MessageProcessor.Subscriptions.Subscribe(_agent.AgentId, 1);
            var message = new Message(_agent.AgentId, _agent.AgentId, MessageAction.Remove, SymuYellowPages.Subscribe);
            _agent.RemoveSubscribe(message);
            Assert.AreEqual(0, _agent.MessageProcessor.Subscriptions.SubscribersCount(1));
        }

        /// <summary>
        ///     Multiple subscriptions
        /// </summary>
        [TestMethod]
        public void RemoveSubscribeTest1()
        {
            _agent.MessageProcessor.Subscriptions.Subscribe(_agent.AgentId, 1);
            _agent.MessageProcessor.Subscriptions.Subscribe(_agent.AgentId, 2);
            _agent.MessageProcessor.Subscriptions.Subscribe(_agent.AgentId, 3);
            var message = new Message(_agent.AgentId, _agent.AgentId, MessageAction.Remove, SymuYellowPages.Subscribe);
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
            var message = new Message(_agent.AgentId, _agent.AgentId, MessageAction.Add, SymuYellowPages.Subscribe,
                attachments);
            _agent.AddSubscribe(message);
            Assert.AreEqual(1, _agent.MessageProcessor.Subscriptions.SubscribersCount(1));
            Assert.AreEqual(1, _agent.MessageProcessor.Subscriptions.SubscribersCount(2));
            Assert.AreEqual(0, _agent.MessageProcessor.Subscriptions.SubscribersCount(0));
        }

        #endregion

        #region status

        [TestMethod]
        public void StateAgentTests()
        {
            _agent.State = AgentState.Stopping;
            Assert.AreEqual(AgentState.Stopping, _agent.State);
            Environment.StopAgents();
            Assert.AreEqual(AgentState.Stopped, _agent.State);
        }

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

        #endregion
    }
}