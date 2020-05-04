#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using SymuEngine.Common;
using SymuEngine.Messaging.Message;

#endregion

namespace SymuEngine.Classes.Agent.Models.Templates.Communication
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
            Cognitive.TasksAndPerformance.LearningModel.On = true;
            Cognitive.TasksAndPerformance.LearningModel.RateOfAgentsOn = 1;
        }
    }
}