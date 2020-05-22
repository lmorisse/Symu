#region Licence

// Description: Symu - SymuEngineTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Environment;

#endregion

namespace SymuTests.Environment
{
    [TestClass]
    public class EnvironmentStateTests
    {
        private readonly AgentId _agentId = new AgentId(1, 1);
        private readonly EnvironmentState _state = new EnvironmentState();

        [TestMethod]
        public void GivenDebugSimpleStateTest()
        {
            _state.Debug = true;
            _state.EnqueueStartingAgent(_agentId);
            Assert.IsFalse(_state.Started);
            _state.DequeueStartedAgent();
            Assert.IsTrue(_state.Started);
        }

        [TestMethod]
        public void GivenNoDebugSimpleStateTest()
        {
            _state.Debug = false;
            _state.EnqueueStartingAgent(_agentId);
            Assert.IsFalse(_state.Started);
            _state.DequeueStartedAgent();
            Assert.IsTrue(_state.Started);
        }
    }
}