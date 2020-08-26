#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Repository.Entity;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace SymuTests.Repository.Networks.Knowledges
{
    [TestClass]
    public class AgentExpertiseTests
    {
        private readonly AgentExpertise _expertise = new AgentExpertise();

        private readonly Knowledge _knowledge =
            new Knowledge(1, "1", 10);

        private AgentKnowledge _agentKnowledge;

        [TestInitialize]
        public void Initialize()
        {
            _agentKnowledge = new AgentKnowledge(_knowledge.Id, KnowledgeLevel.Expert, 0, -1);
        }

        [TestMethod]
        public void ContainsTest()
        {
            Assert.IsFalse(_expertise.Contains(_knowledge.Id));
            _expertise.Add(_agentKnowledge);
            Assert.IsTrue(_expertise.Contains(_knowledge.Id));
        }

        [TestMethod]
        public void ContainsIdTest()
        {
            Assert.IsFalse(_expertise.Contains(_knowledge.Id));
            _expertise.Add(_agentKnowledge);
            Assert.IsTrue(_expertise.Contains(_knowledge.Id));
        }

        [TestMethod]
        public void GetKnowledgesTest()
        {
            Assert.AreEqual(0, _expertise.GetKnowledgeIds().Count());
            _expertise.Add(_agentKnowledge);
            Assert.AreEqual(1, _expertise.GetKnowledgeIds().Count());
        }

        /// <summary>
        ///     Non passing test
        /// </summary>
        [TestMethod]
        public void GetKnowledgeTest()
        {
            Assert.IsNull(_expertise.GetAgentKnowledge(_knowledge.Id));
        }
    }
}