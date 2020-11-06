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
        private const string VariableName = "Employees";
        private TestSysDynAgent _agent;

        [TestInitialize]
        public void Initialize()
        {
            _agent = new TestSysDynAgent(Environment.WhitePages.NextAgentId(TestReactiveAgent.ClassId), Environment);
            Environment.SysDynEngine = new SysDynEngine(XmlFile);
            Environment.SysDynEngine.AddVariableAgent(VariableName, _agent.AgentId, "Property1");
        }

        [TestMethod()]
        public void SysDynModelTest()
        {
            Assert.AreEqual(10, Environment.SysDynEngine.Models.RootModel.Variables.GetValue(VariableName));
        }

        [TestMethod()]
        public void ProcessTest()
        {
            Environment.SysDynEngine.Process(Environment.WhitePages.AllAgents().ToList());
            Assert.AreEqual(Environment.SysDynEngine.Models.RootModel.Variables.GetValue(VariableName), _agent.Property1);
        }

        [TestMethod()]
        public void UpdateVariablesTest()
        {
            _agent.Property1 = 2;
            Environment.SysDynEngine.UpdateVariables(Environment.WhitePages.AllAgents().ToList());
            Assert.AreEqual(Environment.SysDynEngine.Models.RootModel.Variables.GetValue(VariableName), _agent.Property1);
        }
    }
}