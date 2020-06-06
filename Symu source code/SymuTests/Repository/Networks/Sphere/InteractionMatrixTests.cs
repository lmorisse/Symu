#region Licence

// Description: Symu - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models;
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Classes.Organization;
using Symu.Repository.Networks;
using Symu.Repository.Networks.Knowledges;
using Symu.Repository.Networks.Sphere;

#endregion

namespace SymuTests.Repository.Networks.Sphere
{
    [TestClass]
    public class InteractionMatrixTests
    {
        private readonly List<AgentId> _actors = new List<AgentId>();
        private readonly AgentId _agentId1 = new AgentId(1, 1);
        private readonly AgentId _agentId2 = new AgentId(2, 1);
        private readonly AgentId _agentId3 = new AgentId(3, 1);

        private readonly Knowledge _info1 =
            new Knowledge(1, "1", 1);

        private readonly Knowledge _info2 =
            new Knowledge(2, "2", 1);

        private readonly Knowledge _info3 =
            new Knowledge(3, "3", 1);

        private readonly AgentTemplates _templates = new AgentTemplates();
        private Network _network;

        private NetworkKnowledges _networkKnowledge;

        [TestInitialize]
        public void Initialize()
        {
            _templates.Human.Cognitive.InteractionPatterns.SetInteractionPatterns(InteractionStrategy.Knowledge);
            var organizationModels = new OrganizationModels {InteractionSphere = {On = true}};
            _network = new Network(organizationModels);
            _networkKnowledge = _network.NetworkKnowledges;
        }

        private void Interaction1X1()
        {
            _actors.Add(_agentId1);
            _networkKnowledge.AddKnowledge(_info1);
            _networkKnowledge.Add(_agentId1, _info1.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            _networkKnowledge.InitializeExpertise(_agentId1, false, 0);
            _network.InteractionSphere.SetSphere(true, _actors, _network);
        }

        private void NoInteraction2X2()
        {
            Interaction1X1();
            _actors.Add(_agentId2);
            _networkKnowledge.AddKnowledge(_info2);
            _networkKnowledge.Add(_agentId2, _info2.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            _networkKnowledge.InitializeExpertise(_agentId2, false, 0);
            _network.InteractionSphere.SetSphere(true, _actors, _network);
        }

        private void NoInteraction3X3()
        {
            NoInteraction2X2();
            _actors.Add(_agentId3);
            _networkKnowledge.AddKnowledge(_info3);
            _networkKnowledge.Add(_agentId3, _info3.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            _networkKnowledge.InitializeExpertise(_agentId3, false, 0);
            _network.InteractionSphere.SetSphere(true, _actors, _network);
        }

        [TestMethod]
        public void MaxTriadsTest()
        {
            Assert.AreEqual((uint) 0, InteractionMatrix.MaxTriads(1));
            Assert.AreEqual((uint) 0, InteractionMatrix.MaxTriads(2));
            Assert.AreEqual((uint) 1, InteractionMatrix.MaxTriads(3));
            Assert.AreEqual((uint) 4, InteractionMatrix.MaxTriads(4));
            Assert.AreEqual((uint) 10, InteractionMatrix.MaxTriads(5));
            Assert.AreEqual((uint) 20, InteractionMatrix.MaxTriads(6));
        }

        #region Average interaction

        [TestMethod]
        public void AverageInteraction1X1Test()
        {
            Interaction1X1();
            Assert.AreEqual(0,
                InteractionMatrix.GetAverageInteractionMatrix(
                    InteractionMatrix.GetInteractionMatrix(_network.InteractionSphere.Sphere)));
        }

        [TestMethod]
        public void Average0Interaction2X2Test()
        {
            NoInteraction2X2();

            Assert.AreEqual(0,
                InteractionMatrix.GetAverageInteractionMatrix(
                    InteractionMatrix.GetInteractionMatrix(_network.InteractionSphere.Sphere)));
        }

        [TestMethod]
        public void Average1Interaction2X2Test()
        {
            NoInteraction2X2();
            _networkKnowledge.Add(_agentId1, _info2.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            _networkKnowledge.Add(_agentId2, _info1.Id, KnowledgeLevel.FullKnowledge, 0, -1);
            _networkKnowledge.InitializeExpertise(_agentId1, false, 0);
            _networkKnowledge.InitializeExpertise(_agentId2, false, 0);
            _network.InteractionSphere.SetSphere(true, _actors, _network);

            Assert.AreEqual(1F,
                InteractionMatrix.GetAverageInteractionMatrix(
                    InteractionMatrix.GetInteractionMatrix(_network.InteractionSphere.Sphere)));
        }

        [TestMethod]
        public void Average0Interaction3X3Test()
        {
            NoInteraction3X3();

            Assert.AreEqual(0,
                InteractionMatrix.GetAverageInteractionMatrix(
                    InteractionMatrix.GetInteractionMatrix(_network.InteractionSphere.Sphere)));
        }

        #endregion

        #region triad

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        [TestMethod]
        public void SameKnowledgeNumberEqualMaxTriads(int count)
        {
            _networkKnowledge.AddKnowledge(_info1);
            for (ushort i = 0; i < count; i++)
            {
                var agentId = new AgentId(i, 1);
                _actors.Add(agentId);
                _networkKnowledge.Add(agentId, _info1.Id, KnowledgeLevel.FullKnowledge, 0, -1);
                _networkKnowledge.InitializeExpertise(agentId, false, 0);
            }

            _network.InteractionSphere.SetSphere(true, _actors, _network);
            Assert.AreEqual(InteractionMatrix.MaxTriads(count),
                InteractionMatrix.NumberOfTriads(_network.InteractionSphere.Sphere));
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        [TestMethod]
        public void DifferentKnowledgeNumberEqualMaxTriads(int count)
        {
            for (ushort i = 0; i < count; i++)
            {
                var info =
                    new Knowledge(i, i.ToString(), 1);
                _networkKnowledge.AddKnowledge(info);
                var agentId = new AgentId(i, 1);
                _actors.Add(agentId);
                _networkKnowledge.Add(agentId, info.Id, KnowledgeLevel.FullKnowledge, 0, -1);
                _networkKnowledge.InitializeExpertise(agentId, false, 0);
            }

            _network.InteractionSphere.SetSphere(true, _actors, _network);
            Assert.AreEqual((uint) 0, InteractionMatrix.NumberOfTriads(_network.InteractionSphere.Sphere));
        }

        #endregion
    }
}