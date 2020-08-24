#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Common.Interfaces.Agent;
using Symu.Repository.Networks.Enculturation;

#endregion

namespace SymuTests.Repository.Networks.Enculturation
{
    [TestClass]
    public class NetworkEnculturationTests
    {
        private readonly AgentId _agentId = new AgentId(1, 1);

        private readonly EnculturationNetwork _enculturation =
            new EnculturationNetwork();

        [TestMethod]
        public void RemoveAgentTest()
        {
            _enculturation.RemoveAgent(_agentId);
            _enculturation.AddAgentId(_agentId);
            Assert.IsTrue(_enculturation.Exists(_agentId));
            _enculturation.RemoveAgent(_agentId);
            Assert.IsFalse(_enculturation.Exists(_agentId));
        }

        [TestMethod]
        public void AddAgentIdTest()
        {
            Assert.IsFalse(_enculturation.Exists(_agentId));
            _enculturation.AddAgentId(_agentId);
            Assert.IsTrue(_enculturation.Exists(_agentId));
            Assert.AreEqual(0, _enculturation.GetEnculturation(_agentId));
        }

        [TestMethod]
        public void UpdateEnculturationTest()
        {
            Assert.AreEqual(0, _enculturation.GetEnculturation(_agentId));
            _enculturation.UpdateEnculturation(_agentId, 1);
            Assert.AreEqual(1, _enculturation.GetEnculturation(_agentId));
        }
    }
}