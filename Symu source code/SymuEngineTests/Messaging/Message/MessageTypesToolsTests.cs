#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Messaging.Message;

#endregion

namespace SymuEngineTests.Messaging.Message
{
    [TestClass]
    public class MessageTypesToolsTests
    {
        private CommunicationMediums _messages;

        [TestMethod]
        public void CountTest()
        {
            Assert.AreEqual(0, CommunicationMediumsModel.Count(_messages));
            _messages |= CommunicationMediums.Email;
            Assert.AreEqual(1, CommunicationMediumsModel.Count(_messages));
            _messages |= CommunicationMediums.ViaAPlatform;
            Assert.AreEqual(2, CommunicationMediumsModel.Count(_messages));
        }

        [TestMethod]
        public void ToArrayTest()
        {
            Assert.AreEqual(0, CommunicationMediumsModel.ToArray(_messages).Length);
            _messages |= CommunicationMediums.Email;
            Assert.AreEqual(1, CommunicationMediumsModel.ToArray(_messages).Length);
            Assert.AreEqual(CommunicationMediums.Email,
                (CommunicationMediums) CommunicationMediumsModel.ToArray(_messages)[0]);
            _messages |= CommunicationMediums.ViaAPlatform;
            Assert.AreEqual(2, CommunicationMediumsModel.ToArray(_messages).Length);
            Assert.AreEqual(CommunicationMediums.ViaAPlatform,
                (CommunicationMediums) CommunicationMediumsModel.ToArray(_messages)[1]);
        }

        /// <summary>
        ///     Empty murphy
        /// </summary>
        [TestMethod]
        public void AskOnWhichChannelTest()
        {
            const CommunicationMediums mediums = new CommunicationMediums();
            Assert.AreEqual(CommunicationMediums.System, CommunicationMediumsModel.AskOnWhichChannel(mediums));
        }

        [TestMethod]
        public void AskOnWhichChannelTest1()
        {
            var mediums = new CommunicationMediums();
            mediums |= CommunicationMediums.ViaAPlatform;
            var medium = CommunicationMediumsModel.AskOnWhichChannel(mediums);
            Assert.IsTrue(medium == CommunicationMediums.ViaAPlatform);
            mediums |= CommunicationMediums.Email;
            medium = CommunicationMediumsModel.AskOnWhichChannel(mediums);
            Assert.IsTrue(medium == CommunicationMediums.Email || medium == CommunicationMediums.ViaAPlatform);
        }

        [TestMethod]
        public void IntersectTest()
        {
            // No intersection
            var mediums1 = CommunicationMediums.ViaAPlatform;
            var mediums2 = CommunicationMediums.Email;
            var intersect = mediums1 & mediums2;
            Assert.AreEqual(CommunicationMediums.System, intersect);
            // One intersection
            mediums2 |= CommunicationMediums.ViaAPlatform;
            intersect = mediums1 & mediums2;
            Assert.AreEqual(CommunicationMediums.ViaAPlatform, intersect);
            // two intersections
            mediums1 |= CommunicationMediums.Email;
            intersect = mediums1 & mediums2;
            Assert.AreEqual(CommunicationMediums.ViaAPlatform | CommunicationMediums.Email, intersect);
        }
    }
}