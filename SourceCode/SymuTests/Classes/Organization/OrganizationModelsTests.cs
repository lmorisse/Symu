#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Common.Classes;
using Symu.Repository.Entities;
using SymuTests.Helpers;

#endregion

namespace SymuTests.Classes.Organization
{
    [TestClass]
    public class OrganizationModelsTests : BaseTestClass
    {
        [TestMethod]
        public void CloneTest()
        {
            MainOrganization.Models.SetOn(0.5F);
            MainOrganization.Models.Generator = RandomGenerator.RandomUniform;
            MainOrganization.Models.BeliefWeightLevel = BeliefWeightLevel.RandomWeight;
            var clone = MainOrganization.Models.Clone();
            foreach (var model in clone.List)
            {
                Assert.IsTrue(model.On);
                Assert.AreEqual(0.5F, model.RateOfAgentsOn);
            }

            Assert.IsTrue(clone.Knowledge.On);
            Assert.IsTrue(clone.Beliefs.On);
            Assert.IsTrue(clone.Influence.On);
            Assert.IsTrue(clone.Forgetting.On);
            Assert.IsTrue(clone.Learning.On);
            Assert.IsTrue(clone.InteractionSphere.On);

            Assert.AreEqual(MainOrganization.Models.Generator, clone.Generator);
            Assert.AreEqual(MainOrganization.Models.BeliefWeightLevel, clone.BeliefWeightLevel);
        }
    }
}