#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Common;
using Symu.Common.Interfaces;
using Symu.OrgMod.Edges;
using Symu.OrgMod.GraphNetworks.TwoModesNetworks.Sphere;
using Symu.Results.Organization;
using SymuTests.Helpers;

#endregion

namespace SymuTests.Results.Organization
{
    [TestClass]
    public class OrganizationFlexibilityTests : BaseTestClass
    {
        private OrganizationFlexibility _result;

        [TestInitialize]
        public void Initialize()
        {
            MainOrganization.Models.InteractionSphere.SetInteractionPatterns(InteractionStrategy.SocialDemographics);
            MainOrganization.Models.InteractionSphere.On = true;
            Environment.SetOrganization(MainOrganization);
            Simulation.Initialize(Environment);
            _result = new OrganizationFlexibility(Environment);
        }

        #region triads

        /// <summary>
        ///     No employee
        /// </summary>
        [TestMethod]
        public void HandleTriadsTest()
        {
            SetAgents(0);
            _result.HandleTriads(0);
            Assert.IsTrue(_result.Triads.Any());
            Assert.AreEqual(0, _result.Triads[0].ActualNumber);
            Assert.AreEqual(0, _result.Triads[0].MaxNumber);
        }

        /// <summary>
        ///     With employees
        /// </summary>
        [TestMethod]
        public void HandleTriadsTest1()
        {
            SetAgents(5);
            _result.HandleTriads(5);
            Assert.AreEqual(1, _result.Triads.Count);
            Assert.IsTrue(_result.Triads[0].ActualNumber > 0);
            Assert.AreEqual(10, _result.Triads[0].MaxNumber);
        }

        private void SetAgents(int count)
        {
            var agentIds = new List<IAgentId>();
            for (var i = 0; i < count; i++)
            {
                var agent = TestCognitiveAgent.CreateInstance(Environment);
                agent.Cognitive.InteractionPatterns.IsPartOfInteractionSphere = true;
                agent.State = AgentState.Started;
                agentIds.Add(agent.AgentId);
            }

            for (var i = 0; i < count; i++)
            {
                var agentId1 = agentIds[i];
                // interaction are undirected
                for (var j = i + 1; j < count; j++)
                {
                    var agentId2 = agentIds[j];
                    ActorActor.CreateInstance(Environment.MainOrganization.ArtifactNetwork.ActorActor, agentId1, agentId2);
                }
            }

            Environment.InitializeInteractionSphere();
        }

        #endregion

        #region links

        /// <summary>
        ///     No employee
        /// </summary>
        [TestMethod]
        public void HandleLinksTest()
        {
            SetAgents(0);
            _result.HandleLinks(0);
            Assert.IsTrue(_result.Links.Any());
            Assert.AreEqual(0, _result.Links[0].ActualNumber);
            Assert.AreEqual(0, _result.Links[0].MaxNumber);
        }

        /// <summary>
        ///     With employees
        /// </summary>
        [TestMethod]
        public void HandleLinksTest1()
        {
            SetAgents(5);
            _result.HandleLinks(5);
            Assert.AreEqual(1, _result.Links.Count);
            Assert.IsTrue(_result.Links[0].ActualNumber > 0);
            Assert.AreEqual(10, _result.Links[0].MaxNumber);
        }

        #endregion

        #region Sphere

        /// <summary>
        ///     No employee
        /// </summary>
        [TestMethod]
        public void HandleSphereTest()
        {
            SetAgents(0);
            _result.HandleSphere();
            Assert.IsTrue(_result.Sphere.Any());
            Assert.AreEqual(0, _result.Sphere[0].ActualNumber);
            Assert.AreEqual(0, _result.Sphere[0].MaxNumber);
        }

        /// <summary>
        ///     With employees
        /// </summary>
        [TestMethod]
        public void HandleSphereTest1()
        {
            SetAgents(5);
            _result.HandleSphere();
            Assert.AreEqual(1, _result.Sphere.Count);
            Assert.IsTrue(_result.Sphere[0].ActualNumber > 0);
            Assert.AreEqual(20, _result.Sphere[0].MaxNumber);
        }

        #endregion
    }
}