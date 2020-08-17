#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Messaging.Messages;

#endregion

namespace Symu.Messaging.Templates
{
    /// <summary>
    ///     List of all available communication templates
    /// </summary>
    /// <example>
    ///     Human
    ///     ...
    /// </example>
    public class CommunicationTemplates
    {
        public EmailTemplate Email { get; } = new EmailTemplate();
        public FaceToFaceTemplate FaceToFace { get; } = new FaceToFaceTemplate();
        public IrcTemplate Irc { get; } = new IrcTemplate();
        public MeetingTemplate Meeting { get; } = new MeetingTemplate();
        public PhoneTemplate Phone { get; } = new PhoneTemplate();
        public ViaPlatformTemplate Platform { get; } = new ViaPlatformTemplate();

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
    }
}