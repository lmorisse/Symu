#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Organization;
using Symu.Common.Interfaces.Entity;
using Symu.Engine;
using Symu.Messaging.Messages;
using SymuTests.Helpers;

#endregion

namespace SymuTests.Environment
{
    [TestClass]
    public class SymuEnvironmentTests
    {
        private readonly TestEnvironment _environment = new TestEnvironment();
        private readonly OrganizationEntity _organizationEntity = new OrganizationEntity("1");
        private readonly SymuEngine _symu = new SymuEngine();

        [TestInitialize]
        public void Initialize()
        {
            _environment.SetOrganization(_organizationEntity);
            _symu.SetEnvironment(_environment);
        }

        [TestMethod]
        public void EnqueueMessageLostTest()
        {
            var agent1 = new TestReactiveAgent(new UId(1), _environment);
            var agent2 = new TestReactiveAgent(new UId(2), _environment);
            _environment.Start();
            _environment.WaitingForStart();
            _environment.WhitePages.RemoveAgent(agent2);
            var message = new Message(agent1.AgentId, agent2.AgentId, MessageAction.Handle, 1);
            Assert.AreEqual(MessageState.Created, message.State);
            _environment.SendAgent(message);
            Assert.AreEqual(MessageState.Lost, message.State);
            Assert.AreEqual(1, _environment.Messages.Result.LostMessagesCount);
        }
    }
}