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
    public class CommunicationMediumsModelTests
    {
        /// <summary>
        ///     0 medium
        /// </summary>
        [TestMethod]
        public void AskOnWhichChannelTest0()
        {
            Assert.AreEqual(CommunicationMediums.System,
                CommunicationMediumsModel.AskOnWhichChannel(new CommunicationMediums()));
        }

        /// <summary>
        ///     One medium
        /// </summary>
        [TestMethod]
        public void AskOnWhichChannelTest()
        {
            Assert.AreEqual(CommunicationMediums.Email,
                CommunicationMediumsModel.AskOnWhichChannel(CommunicationMediums.Email));
        }

        /// <summary>
        ///     two medium
        /// </summary>
        [TestMethod]
        public void AskOnWhichChannelTest1()
        {
            const CommunicationMediums mediums = CommunicationMediums.Email | CommunicationMediums.FaceToFace;
            var result = CommunicationMediumsModel.AskOnWhichChannel(mediums);
            Assert.IsTrue(result == CommunicationMediums.Email || result == CommunicationMediums.FaceToFace);
        }

        /// <summary>
        ///     three medium
        /// </summary>
        [TestMethod]
        public void AskOnWhichChannelTest2()
        {
            const CommunicationMediums mediums =
                CommunicationMediums.Email | CommunicationMediums.FaceToFace | CommunicationMediums.Irc;
            var result = CommunicationMediumsModel.AskOnWhichChannel(mediums);
            Assert.IsTrue(result == CommunicationMediums.Email || result == CommunicationMediums.FaceToFace ||
                          result == CommunicationMediums.Irc);
        }
    }
}