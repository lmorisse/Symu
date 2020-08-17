#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Classes.Murphies;
using Symu.Classes.Organization;
using Symu.Classes.Task;
using Symu.Repository.Networks;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace SymuTests.Classes.Murphies
{
    [TestClass]
    public class MurphyIncompleteKnowledgeTests
    {
        private readonly AgentId _agentId = new AgentId(1, 1);
        private readonly AgentExpertise _expertise = new AgentExpertise();
        private readonly MurphyIncompleteKnowledge _murphy = new MurphyIncompleteKnowledge();
        private readonly TaskKnowledgeBits _taskBits = new TaskKnowledgeBits();
        private Knowledge _knowledge;
        private MetaNetwork _network;

        [TestInitialize]
        public void Initialize()
        {
            _network = new MetaNetwork(new OrganizationModels());
            _knowledge = new Knowledge(1, "1", 1);
            _network.NetworkKnowledges.AddKnowledge(_knowledge);
            _network.NetworkKnowledges.Add(_agentId, _expertise);
            _taskBits.SetMandatory(new byte[] {0});
            _taskBits.SetRequired(new byte[] {0});
        }

        /// <summary>
        ///     Model off
        /// </summary>
        [TestMethod]
        public void CheckKnowledgeTest0()
        {
            _murphy.On = false;
            var mandatoryCheck = false;
            var requiredCheck = false;
            byte mandatoryIndex = 0;
            byte requiredIndex = 0;
            _murphy.CheckKnowledge(1, _taskBits, _expertise, ref mandatoryCheck, ref requiredCheck, ref mandatoryIndex,
                ref requiredIndex, 0);
            Assert.IsTrue(mandatoryCheck && requiredCheck);
        }

        /// <summary>
        ///     Model on, RateOfAgentsOn = 0
        /// </summary>
        [TestMethod]
        public void CheckKnowledgeTest01()
        {
            _murphy.On = true;
            _murphy.RateOfAgentsOn = 0;
            var mandatoryCheck = false;
            var requiredCheck = false;
            byte mandatoryIndex = 0;
            byte requiredIndex = 0;
            _murphy.CheckKnowledge(1, _taskBits, _expertise, ref mandatoryCheck, ref requiredCheck, ref mandatoryIndex,
                ref requiredIndex, 0);
            Assert.IsTrue(mandatoryCheck && requiredCheck);
        }

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void CheckKnowledgeTest()
        {
            var mandatoryCheck = false;
            var requiredCheck = false;
            byte mandatoryIndex = 0;
            byte requiredIndex = 0;
            _murphy.CheckKnowledge(1, _taskBits, _expertise, ref mandatoryCheck, ref requiredCheck, ref mandatoryIndex,
                ref requiredIndex, 0);
            Assert.IsFalse(mandatoryCheck && requiredCheck);
        }

        /// <summary>
        ///     Without the good knowledge
        /// </summary>
        [TestMethod]
        public void CheckKnowledgeTest1()
        {
            var mandatoryCheck = false;
            var requiredCheck = false;
            byte mandatoryIndex = 0;
            byte requiredIndex = 0;
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {0}, 0, -1, 0);
            _expertise.Add(agentKnowledge);
            _network.NetworkKnowledges.Add(_agentId, _expertise);
            _murphy.CheckKnowledge(1, _taskBits, _expertise, ref mandatoryCheck, ref requiredCheck, ref mandatoryIndex,
                ref requiredIndex, 0);
            Assert.IsFalse(mandatoryCheck && requiredCheck);
        }

        /// <summary>
        ///     With the good knowledge
        /// </summary>
        [TestMethod]
        public void CheckKnowledgeTest2()
        {
            var mandatoryCheck = false;
            var requiredCheck = false;
            byte mandatoryIndex = 0;
            byte requiredIndex = 0;
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] {1}, 0, -1, 0);
            _expertise.Add(agentKnowledge);
            _network.NetworkKnowledges.Add(_agentId, _expertise);
            _murphy.CheckKnowledge(1, _taskBits, _expertise, ref mandatoryCheck, ref requiredCheck, ref mandatoryIndex,
                ref requiredIndex, 0);
            Assert.IsTrue(mandatoryCheck && requiredCheck);
            Assert.AreEqual(0, mandatoryIndex);
            Assert.AreEqual(0, requiredIndex);
        }
    }
}