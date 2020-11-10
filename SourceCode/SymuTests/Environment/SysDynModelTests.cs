using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Environment;
using SymuTests.Helpers;

namespace SymuTests.Environment
{
    [TestClass()]
    public class SysDynModelTests : BaseTestClass
    {
        private const string XmlFile = "../../../Resources/sysdyn.xmile";
        private const string VariableOutput = "_Employees";
        private const string VariableInput = "_Attrition";
        private TestSysDynAgent _agentOutput;
        private TestSysDynAgent _agentInput;

        [TestInitialize]
        public void Initialize()
        {
            _agentOutput = new TestSysDynAgent(Environment.AgentNetwork.NextAgentId(TestReactiveAgent.ClassId), Environment);
            _agentInput = new TestSysDynAgent(Environment.AgentNetwork.NextAgentId(TestReactiveAgent.ClassId), Environment);
            Environment.SysDynEngine = new SysDynEngine(XmlFile);
            Environment.SysDynEngine.AddVariableAgent(VariableOutput, _agentOutput.AgentId, "Property1");
            Environment.SysDynEngine.AddVariableAgent(VariableInput, _agentInput.AgentId, "Property1");
        }

        [TestMethod()]
        public void SysDynModelTest()
        {
            Assert.AreEqual(10, Environment.SysDynEngine.Models.RootModel.Variables.GetValue(VariableOutput));
            Assert.AreEqual(0.1F, Environment.SysDynEngine.Models.RootModel.Variables.GetValue(VariableInput));
        }

        [TestMethod()]
        public void ProcessTest()
        {
            Environment.SysDynEngine.Process(Environment.AgentNetwork.AllAgents().ToList(), null);
            Assert.AreEqual(Environment.SysDynEngine.Models.RootModel.Variables.GetValue(VariableOutput), _agentOutput.Property1);
        }

        [TestMethod()]
        public void SynchronizeSysDynTest()
        {
            _agentInput.Property1 = 2;
            Environment.SysDynEngine.SynchronizeSysDyn(Environment.AgentNetwork.AllAgents().ToList(), null);
            Assert.AreEqual(_agentInput.Property1, Environment.SysDynEngine.Models.RootModel.Variables.GetValue(VariableInput));
        }

        [TestMethod()]
        public void SynchronizeSymuTest()
        {
            _agentOutput.Property1 = 2;
            Environment.SysDynEngine.SynchronizeSymu(Environment.AgentNetwork.AllAgents().ToList(), null);
            Assert.AreEqual(Environment.SysDynEngine.Models.RootModel.Variables.GetValue(VariableOutput), _agentOutput.Property1);
        }
    }
}