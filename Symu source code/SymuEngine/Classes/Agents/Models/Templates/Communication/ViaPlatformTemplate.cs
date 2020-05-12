#region Licence

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
    public class ViaPlatformTemplate : CommunicationTemplate
    {
        public ViaPlatformTemplate()
        {
            CostToSendLevel = GenericLevel.Low;
            CostToReceiveLevel = GenericLevel.Medium;
            Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend = 1;
            Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 1;
            Cognitive.MessageContent.MinimumNumberOfBitsOfBeliefToSend = 1;
            Cognitive.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 1;
            Cognitive.InteractionCharacteristics.PreferredCommunicationMediums = CommunicationMediums.ViaAPlatform;
            // Forever
            Cognitive.InternalCharacteristics.TimeToLive = -1;
            Cognitive.TasksAndPerformance.LearningModel.On = true;
            Cognitive.TasksAndPerformance.LearningModel.RateOfAgentsOn = 1;
        }
    }
}