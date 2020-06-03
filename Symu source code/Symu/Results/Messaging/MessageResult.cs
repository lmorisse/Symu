#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace Symu.Results.Blocker
{
    public class MessageResult
    {
        /// <summary>
        ///     Give the count of the messages sent including lost messages
        /// </summary>
        public uint SentMessagesCount { get; set; }

        /// <summary>
        ///     Give the count of the messages sent by email
        /// </summary>
        public uint SentMessagesByEmail { get; set; }

        /// <summary>
        ///     Give the count of the messages sent by platform
        /// </summary>
        public uint SentMessagesByPlatform { get; set; }

        /// <summary>
        ///     Give the count of the messages sent by IRC
        /// </summary>
        public uint SentMessagesByIrc { get; set; }

        /// <summary>
        ///     Give the count of the messages sent by Meeting
        /// </summary>
        public uint SentMessagesByMeeting { get; set; }

        /// <summary>
        ///     Give the count of the messages sent by face2Face
        /// </summary>
        public uint SentMessagesByFaceToFace { get; set; }

        /// <summary>
        ///     Give the count of the messages sent by phone
        /// </summary>
        public uint SentMessagesByPhone { get; set; }

        /// <summary>
        ///     Give the count of the messages with the state Lost
        /// </summary>
        public ushort LostMessagesCount { get; set; }

        /// <summary>
        ///     Initialize properties
        /// </summary>
        public void Clear()
        {

            SentMessagesCount = 0;
            SentMessagesByEmail = 0;
            SentMessagesByPlatform = 0;
            SentMessagesByIrc = 0;
            SentMessagesByMeeting = 0;
            SentMessagesByFaceToFace = 0;
            SentMessagesByPhone = 0;
            LostMessagesCount = 0;
        }
    }
}