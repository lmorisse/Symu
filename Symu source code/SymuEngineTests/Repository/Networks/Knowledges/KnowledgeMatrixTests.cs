#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Agent;
using SymuEngine.Repository.Networks.Knowledges;

#endregion

namespace SymuEngineTests.Repository.Networks.Knowledges
{
    [TestClass]
    public class KnowledgeMatrixTests
    {
        private readonly List<AgentId> _actors = new List<AgentId>();
        private readonly List<bool> _actorStyles = new List<bool>();
        private readonly AgentId _agentId1 = new AgentId(1, 1);
        private readonly AgentId _agentId2 = new AgentId(2, 1);
        private readonly AgentId _agentId3 = new AgentId(3, 1);

        private readonly Knowledge _info1 =
            new Knowledge(1, "1", 1);

        private readonly Knowledge _info2 =
            new Knowledge(2, "2", 1);

        private readonly Knowledge _info3 =
            new Knowledge(3, "3", 1);

        private readonly KnowledgeCollection _knowledges = new KnowledgeCollection();
        private readonly NetworkKnowledges _network = new NetworkKnowledges();

        private void Interaction1X1()
        {
            _actors.Add(_agentId1);
            _knowledges.Add(_info1);
            _network.Add(_agentId1, _info1, KnowledgeLevel.Expert, 0, -1);
        }

        private void NoInteraction2X2()
        {
            Interaction1X1();
            _actors.Add(_agentId2);
            _knowledges.Add(_info2);
            _network.Add(_agentId2, _info2, KnowledgeLevel.Expert, 0, -1);
        }

        private void NoInteraction3X3()
        {
            NoInteraction2X2();
            _actors.Add(_agentId3);
            _knowledges.Add(_info3);
            _network.Add(_agentId3, _info3, KnowledgeLevel.Expert, 0, -1);
        }

        [TestMethod]
        public void MaxTriadsTest()
        {
            Assert.AreEqual((uint) 0, KnowledgeMatrix.MaxTriads(1));
            Assert.AreEqual((uint) 0, KnowledgeMatrix.MaxTriads(2));
            Assert.AreEqual((uint) 1, KnowledgeMatrix.MaxTriads(3));
            Assert.AreEqual((uint) 4, KnowledgeMatrix.MaxTriads(4));
        }

        #region Knowledge matrix

        [TestMethod]
        public void GetMatrixKnowledge1X1Test()
        {
            Interaction1X1();

            var matrix = KnowledgeMatrix.GetMatrixKnowledge(_knowledges, _actors, _network);
            Assert.AreEqual(1, matrix[0, 0]);
        }

        [TestMethod]
        public void GetMatrixKnowledge2X2Test()
        {
            NoInteraction2X2();

            var matrix = KnowledgeMatrix.GetMatrixKnowledge(_knowledges, _actors, _network);
            Assert.AreEqual(1, matrix[0, 0]);
            Assert.AreEqual(1, matrix[1, 1]);
            Assert.AreEqual(0, matrix[1, 0]);
            Assert.AreEqual(0, matrix[0, 1]);
        }

        #endregion

        #region PassiveInteractionMatrix

        [TestMethod]
        public void PassiveInteractionMatrix1X1Test()
        {
            Interaction1X1();

            var matrix = KnowledgeMatrix.GetPassiveInteractionMatrix(_knowledges, _actors, _network);
            Assert.AreEqual(1, matrix[0, 0]);
        }

        [TestMethod]
        public void PassiveInteractionMatrix2X2Test()
        {
            NoInteraction2X2();

            var matrix = KnowledgeMatrix.GetPassiveInteractionMatrix(_knowledges, _actors, _network);
            Assert.AreEqual(1, matrix[0, 0]);
            Assert.AreEqual(1, matrix[1, 1]);
            Assert.AreEqual(0, matrix[1, 0]);
            Assert.AreEqual(0, matrix[0, 1]);
        }

        #endregion

        #region ActiveInteractionMatrix

        [TestMethod]
        public void ActiveInteractionMatrix1X1Test()
        {
            Interaction1X1();
            _actorStyles.Add(true);

            var matrix = KnowledgeMatrix.GetActiveInteractionMatrix(_knowledges, _actors, _actorStyles, _network);
            Assert.AreEqual(0, matrix[0, 0]);
        }

        /// <summary>
        ///     Given active actors
        ///     Active interaction matrix 2x2
        /// </summary>
        [TestMethod]
        public void GivenActiveActorsActiveInteractionMatrix2X2Test()
        {
            NoInteraction2X2();
            _actorStyles.Add(true);
            _actorStyles.Add(true);

            var matrix = KnowledgeMatrix.GetActiveInteractionMatrix(_knowledges, _actors, _actorStyles, _network);
            Assert.AreEqual(0, matrix[0, 0]);
            Assert.AreEqual(0, matrix[1, 1]);
            Assert.AreEqual(1, matrix[1, 0]);
            Assert.AreEqual(1, matrix[0, 1]);
        }

        /// <summary>
        ///     Given passive Actors
        ///     Active Interaction Matrix 2x2
        /// </summary>
        [TestMethod]
        public void GivenPassiveActorsActiveInteractionMatrix2X2Test()
        {
            NoInteraction2X2();
            _actorStyles.Add(false);
            _actorStyles.Add(false);

            var matrix = KnowledgeMatrix.GetActiveInteractionMatrix(_knowledges, _actors, _actorStyles, _network);
            Assert.AreEqual(0, matrix[0, 0]);
            Assert.AreEqual(0, matrix[1, 1]);
            Assert.AreEqual(0, matrix[1, 0]);
            Assert.AreEqual(0, matrix[0, 1]);
        }

        #endregion

        #region Average interaction

        [TestMethod]
        public void AverageInteraction1X1Test()
        {
            Interaction1X1();

            Assert.AreEqual(0, KnowledgeMatrix.GetAverageInteractionMatrix(_knowledges, _actors, _network));
        }

        [TestMethod]
        public void Average0Interaction2X2Test()
        {
            NoInteraction2X2();

            Assert.AreEqual(0, KnowledgeMatrix.GetAverageInteractionMatrix(_knowledges, _actors, _network));
        }

        [TestMethod]
        public void Average1Interaction2X2Test()
        {
            NoInteraction2X2();
            _network.Add(_agentId1, _info2, KnowledgeLevel.Expert, 0, -1);
            _network.Add(_agentId2, _info1, KnowledgeLevel.Expert, 0, -1);

            Assert.AreEqual(0.5F, KnowledgeMatrix.GetAverageInteractionMatrix(_knowledges, _actors, _network));
        }

        [TestMethod]
        public void Average0Interaction3X3Test()
        {
            NoInteraction3X3();

            Assert.AreEqual(0, KnowledgeMatrix.GetAverageInteractionMatrix(_knowledges, _actors, _network));
        }

        #endregion

        #region triad

        [TestMethod]
        public void NumberOfTriads1X1Test()
        {
            Interaction1X1();

            Assert.AreEqual((uint) 0, KnowledgeMatrix.NumberOfTriads(_knowledges, _actors, _network));
        }

        [TestMethod]
        public void NoInteractionNumberOfTriads2X2Test()
        {
            NoInteraction2X2();

            Assert.AreEqual((uint) 0, KnowledgeMatrix.NumberOfTriads(_knowledges, _actors, _network));
        }

        [TestMethod]
        public void NoInteractionNumberOfTriads3X3Test()
        {
            NoInteraction3X3();

            Assert.AreEqual((uint) 1, KnowledgeMatrix.NumberOfTriads(_knowledges, _actors, _network));
        }

        [TestMethod]
        public void InteractionNumberOfTriads3X3Test()
        {
            NoInteraction3X3();
            _network.Add(_agentId1, _info2, KnowledgeLevel.Expert, 0, -1);
            _network.Add(_agentId1, _info3, KnowledgeLevel.Expert, 0, -1);
            _network.Add(_agentId2, _info1, KnowledgeLevel.Expert, 0, -1);
            _network.Add(_agentId2, _info3, KnowledgeLevel.Expert, 0, -1);
            _network.Add(_agentId3, _info1, KnowledgeLevel.Expert, 0, -1);
            _network.Add(_agentId3, _info2, KnowledgeLevel.Expert, 0, -1);
            Assert.AreEqual((uint) 1, KnowledgeMatrix.NumberOfTriads(_knowledges, _actors, _network));
        }

        #endregion
    }
}