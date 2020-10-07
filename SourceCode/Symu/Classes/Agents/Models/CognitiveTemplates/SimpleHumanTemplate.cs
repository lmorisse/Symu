#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Common;
using Symu.Repository.Entities;

#endregion

namespace Symu.Classes.Agents.Models.CognitiveTemplates
{
    /// <summary>
    ///     Clone all the CognitiveArchitecture parameters for the Simple human template
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
            Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel = BeliefLevel.Agree;
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
            Cognitive.InternalCharacteristics.CanLearn = true;
            Cognitive.InternalCharacteristics.CanForget = true;
            Cognitive.InternalCharacteristics.CanInfluenceOrBeInfluence = true;
            Cognitive.InternalCharacteristics.ForgettingMean = 0.05F;
            Cognitive.InternalCharacteristics.PartialForgettingRate = 0.01F;
            Cognitive.InternalCharacteristics.ForgettingSelectingMode = ForgettingSelectingMode.Oldest;
            Cognitive.InternalCharacteristics.InfluentialnessRateMin = 0;
            Cognitive.InternalCharacteristics.InfluenceabilityRateMax = 1;
            Cognitive.InternalCharacteristics.InfluenceabilityRateMin = 0;
            Cognitive.InternalCharacteristics.InfluentialnessRateMax = 1;
            Cognitive.InternalCharacteristics.InfluentialnessRateMin = 0;
            Cognitive.InternalCharacteristics.RiskAversionLevel = GenericLevel.VeryLow;
            // Interaction Characteristics
            Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod = false;
            Cognitive.InteractionCharacteristics.LimitMessagesSentPerPeriod = false;
            Cognitive.InteractionCharacteristics.MaximumMessagesSentPerPeriod = 1;
            Cognitive.InteractionCharacteristics.LimitReceptionsPerPeriod = false;
            Cognitive.InteractionCharacteristics.MaximumReceptionsPerPeriod = 1;
            // AverageDone in the simulator
            // Cognitive.InteractionCharacteristics.PreferredCommunicationMediums ;
            // Tasks and performance
            Cognitive.TasksAndPerformance.CanPerformTask = true;
            Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds = false;
            Cognitive.TasksAndPerformance.TasksLimit.LimitSimultaneousTasks = true;
            Cognitive.TasksAndPerformance.TasksLimit.MaximumSimultaneousTasks = 10;
            Cognitive.TasksAndPerformance.TasksLimit.LimitTasksInTotal = false;
            Cognitive.TasksAndPerformance.LearningRate = 0.05F;
            Cognitive.TasksAndPerformance.LearningByDoingRate = 0.1F;
            // Cognitive.InteractionPatterns
            Cognitive.InteractionPatterns.IsPartOfInteractionSphere = true;
            Cognitive.InteractionPatterns.AllowNewInteractions = true;
            Cognitive.InteractionPatterns.LimitNumberOfNewInteractions = true;
            Cognitive.InteractionPatterns.ThresholdForNewInteraction = 0.2F;
            Cognitive.InteractionPatterns.MaxNumberOfNewInteractions = 1;
            Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Rarely;
            Cognitive.InteractionPatterns.IsolationCyclicity = Cyclicity.Random;
            Cognitive.InteractionPatterns.InteractionsBasedOnHomophily = 1;
            Cognitive.InteractionPatterns.InteractionsBasedOnKnowledge = 0;
            Cognitive.InteractionPatterns.InteractionsBasedOnActivities = 0;
            Cognitive.InteractionPatterns.InteractionsBasedOnBeliefs = 0;
            Cognitive.InteractionPatterns.InteractionsBasedOnSocialDemographics = 0;
            Cognitive.InternalCharacteristics.TimeToLive = 365;
        }
    }
}