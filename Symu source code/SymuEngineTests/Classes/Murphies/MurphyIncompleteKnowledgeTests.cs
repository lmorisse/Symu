#region Licence

// Description: Symu - SymuEngineTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models;
using Symu.Classes.Murphies;
using Symu.Classes.Organization;
using Symu.Classes.Task;
using Symu.Common;
using Symu.Repository.Networks;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace SymuTests.Classes.Murphies
{
    [TestClass]
    public class MurphyIncompleteKnowledgeTests
    {
        private readonly MurphyIncompleteKnowledge _murphy = new MurphyIncompleteKnowledge();
        private readonly AgentId _agentId = new AgentId(1, 1);
        private readonly AgentExpertise _expertise = new AgentExpertise();
        private readonly TaskKnowledgeBits _taskBits = new TaskKnowledgeBits();
        private Knowledge _knowledge;
        private Network _network;

        [TestInitialize]
        public void Initialize()
        {
            _network = new Network(new AgentTemplates(), new OrganizationModels());
            _knowledge = new Knowledge(1, "1", 1);
            _network.NetworkKnowledges.AddKnowledge(_knowledge);
            _network.NetworkKnowledges.Add(_agentId, _expertise);
            _taskBits.SetMandatory(new byte[] {0});
            _taskBits.SetRequired(new byte[] {0});
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
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] { 0 }, 0, -1, 0);
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
            var agentKnowledge = new AgentKnowledge(_knowledge.Id, new float[] { 1 }, 0, -1, 0);
            _expertise.Add(agentKnowledge);
            _network.NetworkKnowledges.Add(_agentId, _expertise);
            _murphy.CheckKnowledge(1, _taskBits, _expertise, ref mandatoryCheck, ref requiredCheck, ref mandatoryIndex,
                ref requiredIndex, 0);
            Assert.IsTrue(mandatoryCheck && requiredCheck);
            Assert.AreEqual(0, mandatoryIndex);
            Assert.AreEqual(0, requiredIndex);
        }


        [TestMethod]
        public void AskInternallyTest()
        {
            _murphy.DelayBeforeSearchingExternally = 3;
            Assert.IsTrue(_murphy.AskInternally(2, 0));
            Assert.IsFalse(_murphy.AskInternally(3, 0));
        }

        /// <summary>
        ///     Model Off
        /// </summary>
        [TestMethod]
        public void NextGuessTest()
        {
            _murphy.On = false;
            Assert.AreEqual(ImpactLevel.None, _murphy.NextGuess());
        }

        /// <summary>
        ///     Model On - RateOfIncorrectGuess = 0
        /// </summary>
        [TestMethod]
        public void NextGuessTest1()
        {
            _murphy.On = true;
            _murphy.RateOfAgentsOn = 1;
            _murphy.RateOfIncorrectGuess = 0;
            Assert.AreEqual(ImpactLevel.None, _murphy.NextGuess());
        }

        /// <summary>
        ///     Model On - RateOfIncorrectGuess = 1
        /// </summary>
        [TestMethod]
        public void NextGuessTest2()
        {
            _murphy.On = true;
            _murphy.RateOfAgentsOn = 1;
            _murphy.RateOfIncorrectGuess = 1;
            Assert.AreNotEqual(ImpactLevel.None, _murphy.NextGuess());
        }
    }
}