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

namespace SymuEngine.Classes.Agents.Models.Templates
{
    /// <summary>
    ///     Set all the CognitiveArchitecture parameters for the InternetAccessTemplate
    /// </summary>
    public class InternetAccessTemplate : CognitiveArchitectureTemplate
    {
        public InternetAccessTemplate()
        {
            // Knowledge & Beliefs
            Cognitive.KnowledgeAndBeliefs.HasKnowledge = true;
            Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge = true;
            Cognitive.KnowledgeAndBeliefs.HasBelief = true;
            Cognitive.KnowledgeAndBeliefs.HasInitialBelief = true;
            // Message content
            Cognitive.MessageContent.CanSendKnowledge = true;
            Cognitive.MessageContent.CanReceiveKnowledge = false;
            Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 5;
            Cognitive.MessageContent.CanSendBeliefs = true;
            Cognitive.MessageContent.CanReceiveBeliefs = false;
            Cognitive.MessageContent.MinimumNumberOfBitsOfBeliefToSend = 0;
            Cognitive.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 3;
            // Internal Characteristics
            Cognitive.InternalCharacteristics.CanLearn = true;
            Cognitive.InternalCharacteristics.CanForget = false;
            Cognitive.InternalCharacteristics.CanInfluenceOrBeInfluence = false;
            Cognitive.InternalCharacteristics.InfluenceabilityRateMax = 0;
            Cognitive.InternalCharacteristics.InfluenceabilityRateMin = 0;
            Cognitive.InternalCharacteristics.InfluentialnessRateMax = 1;
            Cognitive.InternalCharacteristics.InfluentialnessRateMin = 0;
            // Interaction Characteristics
            Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod = false;
            Cognitive.InteractionCharacteristics.LimitMessagesSentPerPeriod = true;
            Cognitive.InteractionCharacteristics.MaximumMessagesSentPerPeriod = 0;
            Cognitive.InteractionCharacteristics.LimitReceptionsPerPeriod = false;
            Cognitive.InteractionCharacteristics.MaximumReceptionsPerPeriod = 1;
            Cognitive.InteractionCharacteristics.PreferredCommunicationMediums = CommunicationMediums.ViaAPlatform;
            // Tasks and performance
            Cognitive.TasksAndPerformance.CanPerformTask = false;
            Cognitive.TasksAndPerformance.TasksLimit.LimitSimultaneousTasks = false;
            Cognitive.TasksAndPerformance.TasksLimit.LimitTasksInTotal = false;
            Cognitive.TasksAndPerformance.LearningByDoingRate = 0;
            // Cognitive.InteractionPatterns
            Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.VeryRarely;
        }
    }
}