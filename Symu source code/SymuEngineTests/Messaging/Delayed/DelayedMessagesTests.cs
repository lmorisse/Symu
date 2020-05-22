#region Licence

// Description: Symu - SymuEngineTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Messaging.Delayed;
using Symu.Messaging.Messages;

#endregion

namespace SymuTests.Messaging.Delayed
{
    [TestClass]
    public class DelayedMessagesTests
    {
        private readonly DelayedMessages _delayedMessages = new DelayedMessages();

        [TestMethod]
        public void EnqueueSameStepsTest()
        {
            var message = new Symu.Messaging.Messages.Message();
            Assert.AreEqual(0, _delayedMessages.Count);
            _delayedMessages.Enqueue(message, 0);
            Assert.AreEqual(1, _delayedMessages.Count);
            message = new Symu.Messaging.Messages.Message();
            _delayedMessages.Enqueue(message, 0);
            Assert.AreEqual(2, _delayedMessages.Count);
        }

        [TestMethod]
        public void EnqueueDifferentStepsTest()
        {
            var message = new Symu.Messaging.Messages.Message();
            Assert.AreEqual(0, _delayedMessages.Count);
            _delayedMessages.Enqueue(message, 0);
            Assert.AreEqual(1, _delayedMessages.Count);
            message = new Symu.Messaging.Messages.Message();
            _delayedMessages.Enqueue(message, 1);
            Assert.AreEqual(2, _delayedMessages.Count);
        }

        [TestMethod]
        public void EnqueueNullMessageTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _delayedMessages.Enqueue(null, 0));
        }

        /// <summary>
        ///     Message without attachment
        /// </summary>
        [TestMethod]
        public void DequeueTest()
        {
            var message = new Symu.Messaging.Messages.Message();
            _delayedMessages.Enqueue(message, 1);
            // Non passing test
            Assert.IsNull(_delayedMessages.Dequeue(0));
            Assert.AreEqual(1, _delayedMessages.Count);
            // Passing test Dequeue the exact step
            _delayedMessages.Dequeue(1);
            Assert.AreEqual(0, _delayedMessages.Count);
            // Passing test Dequeue passed step
            _delayedMessages.Enqueue(message, 1);
            _delayedMessages.Dequeue(2);
            Assert.AreEqual(0, _delayedMessages.Count);
        }

        /// <summary>
        ///     Message with attachment
        /// </summary>
        [TestMethod]
        public void DequeueTest1()
        {
            var message = new Symu.Messaging.Messages.Message
            {
                Attachments = new MessageAttachments()
            };
            message.Attachments.Add(1);
            _delayedMessages.Enqueue(message, 1);
            var messageReceived = _delayedMessages.Dequeue(1);
            Assert.IsNotNull(messageReceived);
            Assert.AreEqual(1, messageReceived.Attachments.First);
        }

        [TestMethod]
        public void LastTest()
        {
            var message = new Symu.Messaging.Messages.Message();
            _delayedMessages.Enqueue(message, 1);
            // Non passing test
            Assert.IsNull(_delayedMessages.Last(0));
            // Passing test Last the exact step
            Assert.IsNotNull(_delayedMessages.Last(1));
            Assert.AreEqual(1, _delayedMessages.Count);
        }
    }
}