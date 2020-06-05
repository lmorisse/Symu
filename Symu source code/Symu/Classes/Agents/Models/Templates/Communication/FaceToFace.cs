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
    ///     CopyTo all the CognitiveArchitecture parameters for the Email
    /// </summary>
    public class FaceToFaceTemplate : CommunicationTemplate
    {
        public FaceToFaceTemplate()
        {
            CostToSendLevel = GenericLevel.Low;
            CostToReceiveLevel = GenericLevel.Low;
            MinimumNumberOfBitsOfKnowledgeToSend = 1;
            MaximumNumberOfBitsOfKnowledgeToSend = 2;
            MinimumNumberOfBitsOfBeliefToSend = 1;
            MaximumNumberOfBitsOfBeliefToSend = 2;
            TimeToLive = 0;
        }
    }
}