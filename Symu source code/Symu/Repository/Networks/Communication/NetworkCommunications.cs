#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents.Models;
using Symu.Classes.Agents.Models.Templates.Communication;
using Symu.Messaging.Messages;

#endregion

namespace Symu.Repository.Networks.Communication
{
    /// <summary>
    ///     Communication network
    ///     Who (agentId) communicates with which medium
    /// </summary>
    /// <example></example>
    public class NetworkCommunications
    {
        private readonly AgentTemplates _agentTemplates;

        public NetworkCommunications(AgentTemplates agentTemplates)
        {
            _agentTemplates = agentTemplates ?? throw new ArgumentNullException(nameof(agentTemplates));
        }

        /// <summary>
        ///     Repository of all the communications used during the symu
        /// </summary>
        public EmailTemplate Email => _agentTemplates.Email;

        public IrcTemplate Irc => _agentTemplates.Irc;
        public PhoneTemplate Phone => _agentTemplates.Phone;
        public MeetingTemplate Meeting => _agentTemplates.Meeting;
        public FaceToFaceTemplate FaceToFace => _agentTemplates.FaceToFace;
        public ViaPlatformTemplate Platform => _agentTemplates.Platform;

        #region repository

        public CommunicationTemplate TemplateFromChannel(CommunicationMediums channel)
        {
            switch (channel)
            {
                case CommunicationMediums.Irc:
                    return Irc;
                case CommunicationMediums.Email:
                    return Email;
                case CommunicationMediums.Phone:
                    return Phone;
                case CommunicationMediums.Meeting:
                    return Meeting;
                case CommunicationMediums.FaceToFace:
                    return FaceToFace;
                case CommunicationMediums.ViaAPlatform:
                    return Platform;
                case CommunicationMediums.System:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(channel), channel, null);
            }
        }

        public float TimeSpent(CommunicationMediums messageType, bool send, byte randomLevelValue)
        {
            switch (messageType)
            {
                case CommunicationMediums.Irc:
                    return send ? Irc.CostToSend(randomLevelValue) : Irc.CostToReceive(randomLevelValue);
                case CommunicationMediums.Email:
                    return send ? Email.CostToSend(randomLevelValue) : Email.CostToReceive(randomLevelValue);
                case CommunicationMediums.Phone:
                    return send ? Phone.CostToSend(randomLevelValue) : Phone.CostToReceive(randomLevelValue);
                case CommunicationMediums.Meeting:
                    return send ? Meeting.CostToSend(randomLevelValue) : Meeting.CostToReceive(randomLevelValue);
                case CommunicationMediums.FaceToFace:
                    return send ? FaceToFace.CostToSend(randomLevelValue) : FaceToFace.CostToReceive(randomLevelValue);
                case CommunicationMediums.ViaAPlatform:
                    return send
                        ? Platform.CostToSend(randomLevelValue)
                        : Platform.CostToReceive(randomLevelValue);
                case CommunicationMediums.System:
                    return 0;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}