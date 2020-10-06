using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Common.Classes;
using Symu.Repository.Entities;
using SymuTests.Helpers;

namespace SymuTests.Classes.Organization
{
    [TestClass()]
    public class OrganizationModelsTests : BaseTestClass
    {
        [TestMethod()]
        public void CloneTest()
        {
            Organization.Models.SetOn(0.5F);
            Organization.Models.Generator = RandomGenerator.RandomUniform;
            Organization.Models.BeliefWeightLevel = BeliefWeightLevel.RandomWeight;
            var clone = Organization.Models.Clone();
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

            Assert.AreEqual(Organization.Models.Generator, clone.Generator);
            Assert.AreEqual(Organization.Models.BeliefWeightLevel, clone.BeliefWeightLevel );
        }
    }
}