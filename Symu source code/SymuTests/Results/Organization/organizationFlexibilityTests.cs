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
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Classes.Organization;
using Symu.Common;
using Symu.Results.Organization;
using SymuTests.Helpers;

#endregion

namespace SymuTests.Results.Organization
{
    [TestClass]
    public class OrganizationFlexibilityTests
    {
        private readonly TestEnvironment _environment = new TestEnvironment();
        private readonly OrganizationEntity _organizationEntity = new OrganizationEntity("1");
        private OrganizationFlexibility _result;

        [TestInitialize]
        public void Initialize()
        {
            _organizationEntity.Models.InteractionSphere.SetInteractionPatterns(InteractionStrategy.SocialDemographics);
            _organizationEntity.Models.InteractionSphere.On = true;
            _environment.SetOrganization(_organizationEntity);
            _result = new OrganizationFlexibility(_environment);
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
            var agents = new List<AgentId>();
            for (var i = 0; i < count; i++)
            {
                var agent = new TestCognitiveAgent(_organizationEntity.NextEntityIndex(), _environment);
                agent.Cognitive.InteractionPatterns.IsPartOfInteractionSphere = true;
                agent.State = AgentState.Started;
                agents.Add(agent.Id);
            }

            _environment.WhitePages.MetaNetwork.Links.AddLinks(agents);
            _environment.InitializeInteractionSphere();
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