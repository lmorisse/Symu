﻿#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Classes.Agent.Models.Templates.Communication;
using SymuEngine.Messaging.Message;

#endregion

namespace SymuEngine.Repository.Networks.Communication
{
    /// <summary>
    ///     Communication network
    ///     Who (agentId) communicates with which medium
    /// </summary>
    /// <example></example>
    public class NetworkCommunications
    {
        /// <summary>
        ///     Repository of all the communications used during the simulation
        /// </summary>
        public EmailTemplate Email { get; } = new EmailTemplate();

        public IrcTemplate Irc { get; } = new IrcTemplate();
        public PhoneTemplate Phone { get; } = new PhoneTemplate();
        public MeetingTemplate Meeting { get; } = new MeetingTemplate();
        public FaceToFaceTemplate FaceToFace { get; } = new FaceToFaceTemplate();
        public ViaPlatformTemplate ViaPlatform { get; } = new ViaPlatformTemplate();

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
                    return ViaPlatform;
                default:
                    return null;
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
                        ? ViaPlatform.CostToSend(randomLevelValue)
                        : ViaPlatform.CostToReceive(randomLevelValue);
                case CommunicationMediums.System:
                    return 0;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}