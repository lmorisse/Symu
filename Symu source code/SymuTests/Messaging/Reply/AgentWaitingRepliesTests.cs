#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Messaging.Reply;

#endregion

namespace SymuTests.Messaging.Reply
{
    [TestClass]
    public class AgentWaitingRepliesTests
    {
        private const byte ClassId = 1;
        private readonly WaitingReplies _agentWaitingReplies = new WaitingReplies();

        [TestMethod]
        public void EnqueueTest()
        {
            _agentWaitingReplies.Enqueue(ClassId, 1);
            Assert.AreEqual(1, _agentWaitingReplies.WaitingMessages.Count);
            var waitingReply = _agentWaitingReplies.WaitingMessages[0];
            Assert.AreEqual(1, _agentWaitingReplies.WaitingMessages.Count);
            Assert.AreEqual(1, waitingReply.MessagesSent);
            _agentWaitingReplies.Enqueue(ClassId, 2);
            Assert.AreEqual(1, _agentWaitingReplies.WaitingMessages.Count);
            Assert.AreEqual(2, waitingReply.MessagesSent);
        }

        [TestMethod]
        public void DequeueTest()
        {
            _agentWaitingReplies.Enqueue(ClassId, 2);
            var waitingReply = _agentWaitingReplies.WaitingMessages[0];
            Assert.AreEqual(2, waitingReply.MessagesSent);
            _agentWaitingReplies.Dequeue(ClassId);
            Assert.AreEqual(1, waitingReply.RepliesReceived);
            _agentWaitingReplies.Dequeue(ClassId);
            Assert.AreEqual(2, waitingReply.RepliesReceived);
            //Replies received > messagesSent
            Assert.ThrowsException<IndexOutOfRangeException>(() => _agentWaitingReplies.Dequeue(ClassId));
        }

        [TestMethod]
        public void NoWaitingMessageTest()
        {
            Assert.IsTrue(_agentWaitingReplies.NoWaitingReply(ClassId));
            _agentWaitingReplies.Enqueue(ClassId, 1);
            Assert.IsFalse(_agentWaitingReplies.NoWaitingReply(ClassId));
            _agentWaitingReplies.Dequeue(ClassId);
            Assert.IsTrue(_agentWaitingReplies.NoWaitingReply(ClassId));
        }
    }
}