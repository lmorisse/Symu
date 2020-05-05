#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using SymuEngine.Classes.Agent.Models.CognitiveArchitecture;
using SymuEngine.Common;

#endregion

namespace SymuEngine.Classes.Agent.Models.Templates
{
    /// <summary>
    ///     Set all the CognitiveArchitecture parameters for the Simple human template
    /// </summary>
    public class SimpleHumanTemplate : CognitiveArchitectureTemplate
    {
        public SimpleHumanTemplate()
        {
            // Knowledge & Beliefs
            Cognitive.KnowledgeAndBeliefs.HasKnowledge = true;
            Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge = true;
            Cognitive.KnowledgeAndBeliefs.HasBelief = true;
            Cognitive.KnowledgeAndBeliefs.HasInitialBelief = true;
            // Message content
            Cognitive.MessageContent.CanSendKnowledge = true;
            Cognitive.MessageContent.CanReceiveKnowledge = true;
            Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend = 1;
            Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 2;
            Cognitive.MessageContent.CanSendBeliefs = true;
            Cognitive.MessageContent.CanReceiveBeliefs = true;
            Cognitive.MessageContent.MinimumNumberOfBitsOfBeliefToSend = 1;
            Cognitive.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 2;
            // Internal Characteristics
            Cognitive.InternalCharacteristics.ForgettingMean = 0.05F;
            Cognitive.InternalCharacteristics.PartialForgettingRate = 0.01F;
            Cognitive.InternalCharacteristics.ForgettingSelectingMode = ForgettingSelectingMode.Oldest;
            Cognitive.InternalCharacteristics.InfluentialnessRateMin = 0;
            Cognitive.InternalCharacteristics.InfluenceabilityRateMax = 1;
            Cognitive.InternalCharacteristics.InfluenceabilityRateMin = 0;
            Cognitive.InternalCharacteristics.InfluentialnessRateMax = 1;
            Cognitive.InternalCharacteristics.InfluentialnessRateMin = 0;
            // Interaction Characteristics
            Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod = false;
            Cognitive.InteractionCharacteristics.LimitMessagesSentPerPeriod = false;
            Cognitive.InteractionCharacteristics.MaximumMessagesSentPerPeriod = 1;
            Cognitive.InteractionCharacteristics.LimitReceptionsPerPeriod = false;
            Cognitive.InteractionCharacteristics.MaximumReceptionsPerPeriod = 1;
            // Done in the simulator
            // Cognitive.InteractionCharacteristics.PreferredCommunicationMediums ;
            // Tasks and performance
            Cognitive.TasksAndPerformance.CanPerformTask = true;
            Cognitive.TasksAndPerformance.TasksLimit.LimitSimultaneousTasks = false;
            Cognitive.TasksAndPerformance.TasksLimit.MaximumSimultaneousTasks = 10;
            Cognitive.TasksAndPerformance.TasksLimit.LimitTasksInTotal = false;
            Cognitive.TasksAndPerformance.LearningRate = 0.05F;
            Cognitive.TasksAndPerformance.LearningByDoingRate = 0.1F;
            Cognitive.TasksAndPerformance.CostFactorOfLearningByDoing = 1.2F;
            // Cognitive.InteractionPatterns
            Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Rarely;
            Cognitive.InteractionPatterns.IsolationIsRandom = true;
            Cognitive.InteractionPatterns.InteractionsUsingHomophily = 100;
            Cognitive.InteractionPatterns.InteractionsDeliberateSearch = 0;
            Cognitive.InteractionPatterns.InteractionsWithCoWorkers = 0;
            Cognitive.InternalCharacteristics.TimeToLive = 365;
        }
    }
}