#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Messaging.Messages;

#endregion

namespace SymuTests.Messaging.Message
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
                CommunicationMediumsModel.NextMedium(new CommunicationMediums()));
        }

        /// <summary>
        ///     One medium
        /// </summary>
        [TestMethod]
        public void AskOnWhichChannelTest()
        {
            Assert.AreEqual(CommunicationMediums.Email,
                CommunicationMediumsModel.NextMedium(CommunicationMediums.Email));
        }

        /// <summary>
        ///     two medium
        /// </summary>
        [TestMethod]
        public void AskOnWhichChannelTest1()
        {
            const CommunicationMediums mediums = CommunicationMediums.Email | CommunicationMediums.FaceToFace;
            var result = CommunicationMediumsModel.NextMedium(mediums);
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
            var result = CommunicationMediumsModel.NextMedium(mediums);
            Assert.IsTrue(result == CommunicationMediums.Email || result == CommunicationMediums.FaceToFace ||
                          result == CommunicationMediums.Irc);
        }
    }
}