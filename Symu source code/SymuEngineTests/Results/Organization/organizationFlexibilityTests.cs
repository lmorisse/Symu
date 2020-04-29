#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Organization;
using SymuEngine.Results.Organization;
using SymuEngineTests.Helpers;

#endregion

namespace SymuEngineTests.Results.Organization
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
            _environment.SetOrganization(_organizationEntity);
            _result = new OrganizationFlexibility(_environment);
        }

        /// <summary>
        ///     No employee
        /// </summary>
        [TestMethod]
        public void HandleTriadsTest()
        {
            _result.HandleTriads(0);
            Assert.IsTrue(_result.Triads.Any());
            Assert.AreEqual((uint) 0, _result.Triads[0].NumberOfTriads);
            Assert.AreEqual((uint) 0, _result.Triads[0].MaxTriads);
        }

        /// <summary>
        ///     With employees
        /// </summary>
        [TestMethod]
        public void HandleTriadsTest1()
        {
            for (var i = 0; i < 5; i++)
            {
                _ = new TestAgent(_organizationEntity.NextEntityIndex(), _environment);
            }

            _result.HandleTriads(0);
            Assert.AreEqual(1, _result.Triads.Count);
            Assert.IsTrue(_result.Triads[0].NumberOfTriads > 0);
            Assert.IsTrue(_result.Triads[0].MaxTriads > 0);
        }
    }
}