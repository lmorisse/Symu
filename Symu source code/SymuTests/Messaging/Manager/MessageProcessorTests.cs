#region Licence

// Description: Symu - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Messaging.Manager;
using Symu.Messaging.Messages;

#endregion

namespace SymuTests.Messaging.Manager
{
    [TestClass]
    public class MessageProcessorTests
    {
        private MessageProcessor _mailbox;

        [TestInitialize]
        public void Initialize()
        {
            _mailbox = new MessageProcessor(async mb =>
            {
                while (true)
                {
                    await mb.Receive().ConfigureAwait(false);
                    Act();
                }
            });
        }

        /// <summary>
        ///     Goal catch exception raised by MailboxProcessor
        /// </summary>
        [TestMethod]
        public void ExceptionTest()
        {
            var message = new Symu.Messaging.Messages.Message();
            _mailbox.Post(message);
        }

        [SuppressMessage("Design", "CA1031:Ne pas intercepter les types d'exception générale",
            Justification = "<En attente>")]
        private static void Act()
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
            }
        }

        [TestMethod]
        public void PostDelayedTest()
        {
            Assert.AreEqual(0, _mailbox.DelayedMessages.Count);
            var message = new Symu.Messaging.Messages.Message();
            _mailbox.PostAsADelayed(message, 0);
            Assert.AreEqual(1, _mailbox.DelayedMessages.Count);
        }

        [TestMethod]
        public void NextDelayedMessagesTest()
        {
            var message = new Symu.Messaging.Messages.Message();
            _mailbox.PostAsADelayed(message, 0);
            Assert.AreEqual(message, _mailbox.NextDelayedMessages(0));
            Assert.AreEqual(0, _mailbox.DelayedMessages.Count);
            Assert.IsNull(_mailbox.NextDelayedMessages(0));
        }

        [TestMethod]
        public void IncrementMessagesPerPeriodTest()
        {
            Assert.AreEqual(0, _mailbox.NumberMessagesPerPeriod);
            // System message
            _mailbox.IncrementMessagesPerPeriod(CommunicationMediums.System, true);
            Assert.AreEqual(0, _mailbox.NumberMessagesPerPeriod);
            // Not system message
            _mailbox.IncrementMessagesPerPeriod(CommunicationMediums.Email, true);
            Assert.AreEqual(1, _mailbox.NumberMessagesPerPeriod);
            // Exceed byte.MaxValue
            _mailbox.NumberMessagesPerPeriod = byte.MaxValue;
            _mailbox.IncrementMessagesPerPeriod(CommunicationMediums.Email, true);
            Assert.AreEqual(byte.MaxValue, _mailbox.NumberMessagesPerPeriod);
        }

        [TestMethod]
        public void PostTest()
        {
            var message = new Symu.Messaging.Messages.Message
            {
                Medium = CommunicationMediums.Email
            };
            _mailbox.Post(message);

            Assert.AreEqual(1, _mailbox.NumberMessagesPerPeriod);
        }
    }
}