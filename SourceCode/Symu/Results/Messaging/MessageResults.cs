#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Concurrent;
using System.Linq;
using Symu.Common.Classes;
using Symu.Common.Interfaces;
using Symu.Environment;

#endregion

namespace Symu.Results.Messaging
{
    /// <summary>
    ///     Manage the message metrics for the simulation
    /// </summary>
    public sealed class MessageResults : Result
    {
        public MessageResults(SymuEnvironment environment) : base(environment)
        {
            Frequency = TimeStepType.Daily;
        }

        /// <summary>
        ///     Key => step
        ///     Value => MessageResult for the step
        /// </summary>
        public ConcurrentDictionary<ushort, MessageResult> Results { get; private set; } =
            new ConcurrentDictionary<ushort, MessageResult>();

        /// <summary>
        ///     Total lost messages done during the simulation
        /// </summary>
        public int LostMessages => Results.Values.Any() ? Results.Values.Last().LostMessagesCount : 0;

        /// <summary>
        ///     Total Sent Messages By Email during the simulation
        /// </summary>
        public uint SentMessagesByEmail => Results.Values.Any() ? Results.Values.Last().SentMessagesByEmail : 0;

        /// <summary>
        ///     Total Sent Messages Face to face during the simulation
        /// </summary>
        public uint SentMessagesByFaceToFace =>
            Results.Values.Any() ? Results.Values.Last().SentMessagesByFaceToFace : 0;

        /// <summary>
        ///     Total Sent Messages By IRC during the simulation
        /// </summary>
        public uint SentMessagesByIrc => Results.Values.Any() ? Results.Values.Last().SentMessagesByIrc : 0;

        /// <summary>
        ///     Total Sent Messages By Meeting during the simulation
        /// </summary>
        public uint SentMessagesByMeeting => Results.Values.Any() ? Results.Values.Last().SentMessagesByMeeting : 0;

        /// <summary>
        ///     Total Sent Messages By phone during the simulation
        /// </summary>
        public uint SentMessagesByPhone => Results.Values.Any() ? Results.Values.Last().SentMessagesByPhone : 0;

        /// <summary>
        ///     Total Sent Messages By platform during the simulation
        /// </summary>
        public uint SentMessagesByPlatform => Results.Values.Any() ? Results.Values.Last().SentMessagesByPlatform : 0;

        /// <summary>
        ///     Total Sent Messages during the simulation
        /// </summary>
        public uint SentMessages => Results.Values.Any() ? Results.Values.Last().SentMessagesCount : 0;

        /// <summary>
        ///     Total Received Messages during the simulation
        /// </summary>
        public uint ReceivedMessages => Results.Values.Any() ? Results.Values.Last().ReceivedMessagesCount : 0;

        /// <summary>
        ///     Give the total weight of the sent messages
        /// </summary>
        public float SentMessagesCost => Results.Values.Any() ? Results.Values.Last().SentMessagesCost : 0;

        /// <summary>
        ///     Give the total weight of the receive messages
        /// </summary>
        public float ReceivedMessagesCost => Results.Values.Any() ? Results.Values.Last().ReceivedMessagesCost : 0;

        /// <summary>
        ///     Give the total missed messages by offline agents
        /// </summary>
        public uint MissedMessagesCount => Results.Values.Any() ? Results.Values.Last().MissedMessagesCount : 0;

        /// <summary>
        ///     Give the total rejected new interactions messages
        /// </summary>
        public uint NotAcceptedMessagesCount =>
            Results.Values.Any() ? Results.Values.Last().NotAcceptedMessagesCount : 0;

        public override void SetResults()
        {
            Results.TryAdd(Environment.Schedule.Step, Environment.Messages.Result);
        }

        public override void Clear()
        {
            Results.Clear();
        }

        public override void CopyTo(object clone)
        {
            if (!(clone is MessageResults cloneMessages))
            {
                return;
            }

            cloneMessages.Results = new ConcurrentDictionary<ushort, MessageResult>();
            foreach (var result in Results)
            {
                cloneMessages.Results.TryAdd(result.Key, result.Value);
            }
        }

        public override IResult Clone()
        {
            var clone = new MessageResults(Environment);
            CopyTo(clone);
            return clone;
        }
    }
}