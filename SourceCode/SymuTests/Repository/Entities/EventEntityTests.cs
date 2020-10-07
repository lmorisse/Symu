#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Repository.Entities;
using SymuTests.Helpers;

#endregion

namespace SymuTests.Repository.Entities
{
    [TestClass]
    public class EventEntityTests : BaseTestClass
    {
        private EventEntity _event;

        [TestInitialize]
        public void Initialize()
        {
            _event = new EventEntity(Network) {Step = 10};
        }

        [TestMethod]
        public void CloneTest()
        {
            var clone = (EventEntity) _event.Clone();
            Assert.AreEqual(_event.Step, clone.Step);
        }
    }
}