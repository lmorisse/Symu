#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Common;

#endregion

namespace Symu.Messaging.Templates
{
    /// <summary>
    ///     CopyTo all the CognitiveArchitecture parameters for the Email
    /// </summary>
    public class EmailTemplate : CommunicationTemplate
    {
        public EmailTemplate()
        {
            CostToSendLevel = GenericLevel.Medium;
            CostToReceiveLevel = GenericLevel.Low;
            MinimumNumberOfBitsOfKnowledgeToSend = 1;
            MaximumNumberOfBitsOfKnowledgeToSend = 1;
            MinimumNumberOfBitsOfBeliefToSend = 1;
            MaximumNumberOfBitsOfBeliefToSend = 1;
            // One year
            TimeToLive = 365;
        }
    }
}