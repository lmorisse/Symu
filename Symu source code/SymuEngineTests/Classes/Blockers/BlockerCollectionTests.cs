using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Blockers;
using Symu.Classes.Organization;
using SymuTests.Helpers;

namespace SymuTests.Classes.Blockers
{
    [TestClass]
    public class BlockerCollectionTests
    {
        private readonly TestEnvironment _environment = new TestEnvironment();
        private BlockerCollection _blockers;

        [TestInitialize]
        public void Initialize()
        {
            var organization = new OrganizationEntity("1") {Models = {FollowBlockers = true}};
            _environment.SetOrganization(organization);
            _environment.IterationResult.Initialize();
            _blockers= new BlockerCollection(_environment.IterationResult.Blockers);
        }

        [TestMethod]
        public void AddBlockerTest()
        {
            Assert.IsNotNull(_blockers.Add(0, 1, 0));
            Assert.IsTrue(_blockers.IsBlocked);
            Assert.AreEqual(1, _environment.IterationResult.Blockers.BlockersStillInProgress);
        }
    }
}