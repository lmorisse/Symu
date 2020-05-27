#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Common;
using Symu.Messaging.Messages;

#endregion

namespace Symu.Classes.Agents.Models.Templates.Communication
{
    /// <summary>
    ///     Set all the CognitiveArchitecture parameters for the Email
    /// </summary>
    public class IrcTemplate : CommunicationTemplate
    {
        public IrcTemplate()
        {
            CostToSendLevel = GenericLevel.VeryLow;
            CostToReceiveLevel = GenericLevel.VeryLow;
            Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend = 1;
            Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 1;
            Cognitive.MessageContent.MinimumNumberOfBitsOfBeliefToSend = 1;
            Cognitive.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 1;
            Cognitive.InteractionCharacteristics.PreferredCommunicationMediums = CommunicationMediums.Irc;
            // One week
            Cognitive.InternalCharacteristics.TimeToLive = 7;
            Cognitive.InternalCharacteristics.CanLearn = true;
            Cognitive.InternalCharacteristics.CanForget = false;
            Cognitive.InternalCharacteristics.CanInfluenceOrBeInfluence = false;
        }
    }
}