#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections;
using System.Linq;
using SymuTools.Math.ProbabilityDistributions;

#endregion

namespace Symu.Messaging.Messages
{
    public static class CommunicationMediumsModel
    {
        public static int Count(CommunicationMediums messageTypes)
        {
            return new BitArray(new[] {(int) messageTypes}).OfType<bool>().Count(x => x);
        }

        public static int[] ToArray(CommunicationMediums messageTypes)
        {
            var array = new int[Count(messageTypes)];
            var index = 0;
            if (messageTypes.HasFlag(CommunicationMediums.Irc))
            {
                array[index] = (int) CommunicationMediums.Irc;
                index++;
            }

            if (messageTypes.HasFlag(CommunicationMediums.Email))
            {
                array[index] = (int) CommunicationMediums.Email;
                index++;
            }

            if (messageTypes.HasFlag(CommunicationMediums.Phone))
            {
                array[index] = (int) CommunicationMediums.Phone;
                index++;
            }

            if (messageTypes.HasFlag(CommunicationMediums.Meeting))
            {
                array[index] = (int) CommunicationMediums.Meeting;
                index++;
            }

            if (messageTypes.HasFlag(CommunicationMediums.FaceToFace))
            {
                array[index] = (int) CommunicationMediums.FaceToFace;
                index++;
            }

            if (messageTypes.HasFlag(CommunicationMediums.ViaAPlatform))
            {
                array[index] = (int) CommunicationMediums.ViaAPlatform;
            }

            return array;
        }

        /// <summary>
        ///     an agent ask for help, but he can choose different mediums like email, phone, ...
        /// </summary>
        /// <returns></returns>
        public static CommunicationMediums AskOnWhichChannel(CommunicationMediums mediums)
        {
            var count = Count(mediums);
            if (count == 0)
            {
                return CommunicationMediums.System;
            }

            var index = DiscreteUniform.SampleToByte(count - 1);
            var channels = ToArray(mediums);
            return (CommunicationMediums) channels[index];
        }
    }
}