﻿#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using SymuEngine.Common;
using SymuEngine.Messaging.Messages;

#endregion

namespace SymuEngine.Classes.Agents.Models.Templates.Communication
{
    /// <summary>
    ///     Set all the CognitiveArchitecture parameters for the Email
    /// </summary>
    public class MeetingTemplate : CommunicationTemplate
    {
        public MeetingTemplate()
        {
            CostToSendLevel = GenericLevel.Low;
            CostToReceiveLevel = GenericLevel.VeryHigh;
            Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend = 1;
            Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 2;
            Cognitive.MessageContent.MinimumNumberOfBitsOfBeliefToSend = 1;
            Cognitive.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 2;
            Cognitive.InteractionCharacteristics.PreferredCommunicationMediums = CommunicationMediums.Meeting;
            Cognitive.InternalCharacteristics.TimeToLive = 365;
        }
    }
}