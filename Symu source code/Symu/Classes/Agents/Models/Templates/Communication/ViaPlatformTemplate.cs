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
    public class ViaPlatformTemplate : CommunicationTemplate
    {
        public ViaPlatformTemplate()
        {
            CostToSendLevel = GenericLevel.Low;
            CostToReceiveLevel = GenericLevel.Medium;
            MinimumNumberOfBitsOfKnowledgeToSend = 1;
            MaximumNumberOfBitsOfKnowledgeToSend = 1;
            MinimumNumberOfBitsOfBeliefToSend = 1;
            MaximumNumberOfBitsOfBeliefToSend = 1;
            // Forever
            TimeToLive = -1;
        }
    }
}