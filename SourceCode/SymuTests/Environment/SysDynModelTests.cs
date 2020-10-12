﻿using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Environment;
using SymuTests.Helpers;

namespace SymuTests.Environment
{
    [TestClass()]
    public class SysDynModelTests : BaseTestClass
    {
        private const string XmlFile = "../../../Resources/sysdyn.xmile";
        private const string NodeId = "Employees";
        private TestSysDynAgent _agent;

        [TestInitialize]
        public void Initialize()
        {
            _agent = new TestSysDynAgent(Environment.WhitePages.NextAgentId(TestReactiveAgent.ClassId), Environment);
            Environment.SysDynModel = new SysDynModel(XmlFile);
            Environment.SysDynModel.AddNodeAgent(NodeId, _agent.AgentId, "Property1");
        }

        [TestMethod()]
        public void SysDynModelTest()
        {
            Assert.AreEqual(10, Environment.SysDynModel.GetVariable(NodeId));
        }

        [TestMethod()]
        public void ProcessTest()
        {
            Environment.SysDynModel.Process(Environment.WhitePages.AllAgents().ToList());
            Assert.AreEqual(Environment.SysDynModel.GetVariable(NodeId), _agent.Property1);
        }
    }
}