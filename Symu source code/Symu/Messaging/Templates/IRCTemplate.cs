#region Licence

// Description: SymuBiz - Symu
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
    ///     Clone all the CognitiveArchitecture parameters for the Email
    /// </summary>
    public class IrcTemplate : CommunicationTemplate
    {
        public IrcTemplate()
        {
            CostToSendLevel = GenericLevel.VeryLow;
            CostToReceiveLevel = GenericLevel.VeryLow;
            MinimumNumberOfBitsOfKnowledgeToSend = 1;
            MaximumNumberOfBitsOfKnowledgeToSend = 1;
            MinimumNumberOfBitsOfBeliefToSend = 1;
            MaximumNumberOfBitsOfBeliefToSend = 1;
            // One week
            TimeToLive = 7;
        }
    }
}