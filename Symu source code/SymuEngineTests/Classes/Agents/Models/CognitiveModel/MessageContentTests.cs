#region Licence

// Description: Symu - SymuEngineTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Agents.Models.CognitiveModel;
using SymuEngine.Classes.Agents.Models.Templates.Communication;
using SymuEngine.Repository.Networks.Beliefs;
using SymuEngine.Repository.Networks.Knowledges;

#endregion

namespace SymuEngineTests.Classes.Agents.Models.CognitiveModel
{
    [TestClass]
    public class MessageContentTests
    {
        private readonly EmailTemplate _emailTemplate = new EmailTemplate();
        private AgentBelief _agentBelief1;
        private AgentBelief _agentBeliefF;
        private AgentKnowledge _agentKnowledge1;
        private AgentKnowledge _agentKnowledgeF;
        private MessageContent _messageContent;

        [TestInitialize]
        public void Initialize()
        {
            _messageContent = new MessageContent();
            var knowledge1FBits = new[] {1, 0.5F, 0.3F, 0};
            _agentKnowledgeF = new AgentKnowledge(0, knowledge1FBits, 0, -1, 0);
            var knowledge1Bits = new float[] {1, 1, 1, 1};
            _agentKnowledge1 = new AgentKnowledge(0, knowledge1Bits, 0, -1, 0);
            _agentBeliefF = new AgentBelief(0, BeliefLevel.NeitherAgreeNorDisagree)
            {
                BeliefBits = new Bits(knowledge1FBits, -1)
            };
            _agentBelief1 = new AgentBelief(0, BeliefLevel.NeitherAgreeNorDisagree)
            {
                BeliefBits = new Bits(knowledge1FBits, -1)
            };
        }

        #region Knowledge

        /// <summary>
        ///     WIth stochastic effect - non passing
        /// </summary>
        [TestMethod]
        public void AskKnowledgeToSendTest()
        {
            Assert.IsNull(_messageContent.GetFilteredKnowledgeToSend(_agentKnowledgeF, 0, _emailTemplate, out _));
        }

        /// <summary>
        ///     WIth stochastic effect
        ///     Passing test
        ///     length to send == 0
        /// </summary>
        [TestMethod]
        public void AskKnowledgeToSendTest1()
        {
            _messageContent.CanSendKnowledge = true;
            _messageContent.MinimumNumberOfBitsOfKnowledgeToSend = 0;
            _messageContent.MaximumNumberOfBitsOfKnowledgeToSend = 0;
            _emailTemplate.Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend = 0;
            _emailTemplate.Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 0;
            Assert.IsNull(_messageContent.GetFilteredKnowledgeToSend(_agentKnowledgeF, 0, _emailTemplate, out _));
        }

        /// <summary>
        ///     WIth stochastic effect
        ///     Passing test
        ///     MinimumLengthToSendPerBit = 1
        /// </summary>
        [TestMethod]
        public void AskKnowledgeToSendTest3()
        {
            _messageContent.CanSendKnowledge = true;
            _messageContent.MinimumKnowledgeToSendPerBit= 1;
            _messageContent.MaximumNumberOfBitsOfKnowledgeToSend = 2;
            _messageContent.MinimumNumberOfBitsOfKnowledgeToSend = 2;
            _emailTemplate.Cognitive.MessageContent.MinimumKnowledgeToSendPerBit= 1;
            _emailTemplate.Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 2;
            _emailTemplate.Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend = 2;
            Assert.AreEqual(1F,
                _messageContent.GetFilteredKnowledgeToSend(_agentKnowledgeF, 0, _emailTemplate, out _).GetSum());
        }

        /// <summary>
        ///     WIth stochastic effect
        ///     Passing test
        ///     MinimumLengthToSendPerBit = 0.5F
        /// </summary>
        [TestMethod]
        public void AskKnowledgeToSendTest4()
        {
            _messageContent.CanSendKnowledge = true;
            _messageContent.MinimumKnowledgeToSendPerBit= 0.4F;
            _messageContent.MaximumNumberOfBitsOfKnowledgeToSend = 4;
            _messageContent.MinimumNumberOfBitsOfKnowledgeToSend = 4;
            _emailTemplate.Cognitive.MessageContent.MinimumKnowledgeToSendPerBit= 0.4F;
            _emailTemplate.Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 4;
            _emailTemplate.Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend = 4;
            Assert.IsTrue(
                1F <= _messageContent.GetFilteredKnowledgeToSend(_agentKnowledgeF, 0, _emailTemplate, out _).GetSum());
        }

        /// <summary>
        ///     Without stochastic effect
        /// </summary>
        [TestMethod]
        public void AskKnowledgeToSend1Test1()
        {
            _messageContent.CanSendKnowledge = true;
            _messageContent.MinimumKnowledgeToSendPerBit= 0.4F;
            _messageContent.MinimumNumberOfBitsOfKnowledgeToSend = 0;
            _messageContent.MaximumNumberOfBitsOfKnowledgeToSend = 3;
            Assert.IsTrue(_messageContent.GetFilteredKnowledgeToSend(_agentKnowledge1, 0, _emailTemplate, out _)
                              .GetSum() <=
                          3);
            Assert.IsTrue(_messageContent.GetFilteredKnowledgeToSend(_agentKnowledge1, 0, _emailTemplate, out _)
                              .GetSum() >=
                          1);
            _messageContent.MaximumNumberOfBitsOfKnowledgeToSend = 2;
            Assert.IsTrue(_messageContent.GetFilteredKnowledgeToSend(_agentKnowledge1, 0, _emailTemplate, out _)
                              .GetSum() <=
                          2);
            Assert.IsTrue(_messageContent.GetFilteredKnowledgeToSend(_agentKnowledge1, 0, _emailTemplate, out _)
                              .GetSum() >=
                          1);
            _messageContent.MaximumNumberOfBitsOfKnowledgeToSend = 1;
            Assert.AreEqual(1,
                _messageContent.GetFilteredKnowledgeToSend(_agentKnowledge1, 0, _emailTemplate, out _).GetSum());
            _messageContent.MaximumNumberOfBitsOfKnowledgeToSend = 0;
            _emailTemplate.Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend = 0;
            _emailTemplate.Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 0;
            Assert.IsNull(_messageContent.GetFilteredKnowledgeToSend(_agentKnowledge1, 0, _emailTemplate, out _));
        }

        #endregion


        #region Belief

        /// <summary>
        ///     WIth stochastic effect - non passing
        /// </summary>
        [TestMethod]
        public void AskBeliefToSendTest()
        {
            Assert.IsNull(_messageContent.GetFilteredBeliefToSend(_agentBeliefF, 0, _emailTemplate));
        }

        /// <summary>
        ///     WIth stochastic effect
        ///     Passing test
        ///     length to send == 0
        /// </summary>
        [TestMethod]
        public void AskBeliefToSendTest1()
        {
            _messageContent.CanSendBeliefs = true;
            _messageContent.MinimumNumberOfBitsOfBeliefToSend = 0;
            _messageContent.MaximumNumberOfBitsOfBeliefToSend = 0;
            _emailTemplate.Cognitive.MessageContent.MinimumNumberOfBitsOfBeliefToSend = 0;
            _emailTemplate.Cognitive.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 0;
            Assert.IsNull(_messageContent.GetFilteredBeliefToSend(_agentBeliefF, 0, _emailTemplate));
        }

        /// <summary>
        ///     WIth stochastic effect
        ///     Passing test
        ///     MinimumBeliefToSendPerBit > _agentBeliefF
        /// </summary>
        [TestMethod]
        public void AskBeliefToSendTest2()
        {
            _messageContent.CanSendBeliefs = true;
            _messageContent.MinimumBeliefToSendPerBit = 1;
            _messageContent.MaximumNumberOfBitsOfBeliefToSend = 1;
            _messageContent.MinimumNumberOfBitsOfBeliefToSend = 1;
            _emailTemplate.Cognitive.MessageContent.MinimumBeliefToSendPerBit = 1;
            _emailTemplate.Cognitive.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 1;
            _emailTemplate.Cognitive.MessageContent.MinimumNumberOfBitsOfBeliefToSend = 1;
            _agentBeliefF.BeliefBits.SetBit(0, 0.5F);
            Assert.IsNull(_messageContent.GetFilteredBeliefToSend(_agentBeliefF, 0, _emailTemplate));
        }

        /// <summary>
        ///     WIth stochastic effect
        ///     Passing test
        ///     MinimumBeliefToSendPerBit = 1
        /// </summary>
        [TestMethod]
        public void AskBeliefToSendTest3()
        {
            _messageContent.CanSendBeliefs = true;
            _messageContent.MinimumBeliefToSendPerBit = 1;
            _messageContent.MaximumNumberOfBitsOfBeliefToSend = 2;
            _messageContent.MinimumNumberOfBitsOfBeliefToSend = 2;
            _emailTemplate.Cognitive.MessageContent.MinimumBeliefToSendPerBit = 1;
            _emailTemplate.Cognitive.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 2;
            _emailTemplate.Cognitive.MessageContent.MinimumNumberOfBitsOfBeliefToSend = 2;
            Assert.AreEqual(1F, _messageContent.GetFilteredBeliefToSend(_agentBeliefF, 0, _emailTemplate).GetSum());
        }

        /// <summary>
        ///     WIth stochastic effect
        ///     Passing test
        ///     MinimumBeliefToSendPerBit = 0.5F
        /// </summary>
        [TestMethod]
        public void AskBeliefToSendTest4()
        {
            _messageContent.CanSendBeliefs = true;
            _messageContent.MinimumBeliefToSendPerBit = 0.4F;
            _messageContent.MaximumNumberOfBitsOfBeliefToSend = 4;
            _messageContent.MinimumNumberOfBitsOfBeliefToSend = 4;
            _emailTemplate.Cognitive.MessageContent.MinimumBeliefToSendPerBit = 0.4F;
            _emailTemplate.Cognitive.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 4;
            _emailTemplate.Cognitive.MessageContent.MinimumNumberOfBitsOfBeliefToSend = 4;
            Assert.IsTrue(1F <= _messageContent.GetFilteredBeliefToSend(_agentBeliefF, 0, _emailTemplate).GetSum());
        }

        /// <summary>
        ///     Without stochastic effect
        /// </summary>
        [TestMethod]
        public void AskBeliefToSend1Test1()
        {
            _messageContent.CanSendBeliefs = true;
            _messageContent.MinimumBeliefToSendPerBit = 0.4F;
            _messageContent.MinimumNumberOfBitsOfBeliefToSend = 0;
            _messageContent.MaximumNumberOfBitsOfBeliefToSend = 3;
            Assert.IsTrue(_messageContent.GetFilteredBeliefToSend(_agentBelief1, 0, _emailTemplate).GetSum() <= 3);
            Assert.IsTrue(_messageContent.GetFilteredBeliefToSend(_agentBelief1, 0, _emailTemplate).GetSum() >= 1);
            _messageContent.MaximumNumberOfBitsOfBeliefToSend = 2;
            Assert.IsTrue(_messageContent.GetFilteredBeliefToSend(_agentBelief1, 0, _emailTemplate).GetSum() <= 2);
            Assert.IsTrue(_messageContent.GetFilteredBeliefToSend(_agentBelief1, 0, _emailTemplate).GetSum() >= 1);
            _messageContent.MaximumNumberOfBitsOfBeliefToSend = 1;
            Assert.AreEqual(1, _messageContent.GetFilteredBeliefToSend(_agentBelief1, 0, _emailTemplate).GetSum());
            _messageContent.MaximumNumberOfBitsOfBeliefToSend = 0;
            _emailTemplate.Cognitive.MessageContent.MinimumNumberOfBitsOfBeliefToSend = 0;
            _emailTemplate.Cognitive.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 0;
            Assert.IsNull(_messageContent.GetFilteredBeliefToSend(_agentBelief1, 0, _emailTemplate));
        }

        #endregion
    }
}