using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.DNA.GraphNetworks;
using Symu.Repository.Entities;
using SymuTests.Helpers;

namespace SymuTests.Repository.Entities
{
    [TestClass()]
    public class EventEntityTests : BaseTestClass
    {
        private EventEntity _event;

        [TestInitialize]
        public void Initialize()
        {
            _event = new EventEntity(Network) {Step = 10};
        }

        [TestMethod()]
        public void CloneTest()
        {
            var clone = (EventEntity)_event.Clone();
            Assert.AreEqual(_event.Step, clone.Step);
        }
    }
}