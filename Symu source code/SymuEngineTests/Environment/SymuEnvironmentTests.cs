#region Licence

// Description: Symu - SymuEngineTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Organization;
using SymuEngine.Engine;
using SymuEngine.Messaging.Messages;
using SymuEngineTests.Helpers;

#endregion

namespace SymuEngineTests.Environment
{
    [TestClass]
    public class SymuEnvironmentTests
    {
        private readonly TestEnvironment _environment = new TestEnvironment();
        private readonly OrganizationEntity _organizationEntity = new OrganizationEntity("1");
        private readonly SimulationEngine _simulation = new SimulationEngine();

        [TestInitialize]
        public void Initialize()
        {
            _environment.SetOrganization(_organizationEntity);
            _simulation.SetEnvironment(_environment);
        }

        [TestMethod]
        public void EnvironmentStateTest()
        {
            var agent = new TestAgent(1, _environment);
            Assert.IsFalse(_environment.State.Started);
            agent.Start();
            _environment.WaitingForStart();
            Assert.IsTrue(_environment.State.Started);
        }

        [TestMethod]
        public void EnqueueMessageLostTest()
        {
            var agent1 = new TestAgent(1, _environment);
            var agent2 = new TestAgent(2, _environment);
            _environment.Start();
            _environment.WaitingForStart();
            _environment.RemoveAgent(agent2.Id);
            var message = new Message(agent1.Id, agent2.Id, MessageAction.Handle, 1);
            Assert.AreEqual(MessageState.Created, message.State);
            _environment.SendAgent(message);
            Assert.AreEqual(MessageState.Lost, message.State);
            Assert.AreEqual(1, _environment.Messages.LostMessagesCount);
        }
    }
}