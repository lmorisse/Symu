#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Messaging.Messages;
using SymuTests.Helpers;

#endregion

namespace SymuTests.Environment
{
    [TestClass]
    public class SymuEnvironmentTests : BaseTestClass
    {
        [TestInitialize]
        public void Initialize()
        {
            Environment.SetOrganization(MainOrganization);
        }

        [TestMethod]
        public void EnqueueMessageLostTest()
        {
            var agent1 = TestReactiveAgent.CreateInstance(Environment);
            var agent2 = TestReactiveAgent.CreateInstance(Environment);
            Simulation.Initialize(Environment);
            Environment.AgentNetwork.RemoveAgent(agent2);
            var message = new Message(agent1.AgentId, agent2.AgentId, MessageAction.Handle, 1);
            Assert.AreEqual(MessageState.Created, message.State);
            Environment.SendAgent(message);
            Assert.AreEqual(MessageState.Lost, message.State);
            Assert.AreEqual(1, Environment.Messages.Result.LostMessagesCount);
        }
    }
}