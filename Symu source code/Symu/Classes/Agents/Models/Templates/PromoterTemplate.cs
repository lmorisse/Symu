#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Classes.Agents.Models.CognitiveModel;
using Symu.Common;
using Symu.Messaging.Messages;
using Symu.Repository.Networks.Beliefs;

#endregion

namespace Symu.Classes.Agents.Models.Templates
{
    /// <summary>
    ///     CopyTo all the CognitiveArchitecture parameters for the Simple human template
    /// </summary>
    public class PromoterTemplate : CognitiveArchitectureTemplate
    {
        /// <summary>
        /// A promoter agent is designed to sway beliefs and encourage or discourage participation, and as such will have a 100% chance of knowing each fact.
        /// </summary>
        public PromoterTemplate()
        {
            // Knowledge & Beliefs
            Cognitive.KnowledgeAndBeliefs.HasKnowledge = true;
            Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge = true;
            Cognitive.KnowledgeAndBeliefs.HasBelief = true;
            Cognitive.KnowledgeAndBeliefs.HasInitialBelief = true;
            Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel = BeliefLevel.StronglyAgree;
            // Message content
            Cognitive.MessageContent.CanSendKnowledge = true;
            Cognitive.MessageContent.CanReceiveKnowledge = true;
            Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend = 1;
            Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 2;
            Cognitive.MessageContent.CanSendBeliefs = true;
            Cognitive.MessageContent.CanReceiveBeliefs = false;
            Cognitive.MessageContent.MinimumNumberOfBitsOfBeliefToSend = 2;
            Cognitive.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 4;
            // Internal Characteristics
            Cognitive.InternalCharacteristics.CanLearn = true;
            Cognitive.InternalCharacteristics.CanForget = false;
            Cognitive.InternalCharacteristics.CanInfluenceOrBeInfluence = true;
            Cognitive.InternalCharacteristics.InfluenceabilityRateMax = 0;
            Cognitive.InternalCharacteristics.InfluenceabilityRateMin = 0;
            Cognitive.InternalCharacteristics.InfluentialnessRateMax = 1;
            Cognitive.InternalCharacteristics.InfluentialnessRateMin = 0;
            Cognitive.InternalCharacteristics.TimeToLive = 365;
            // Interaction Characteristics
            Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod = false;
            Cognitive.InteractionCharacteristics.LimitMessagesSentPerPeriod = false;
            Cognitive.InteractionCharacteristics.MaximumMessagesSentPerPeriod = 1;
            Cognitive.InteractionCharacteristics.LimitReceptionsPerPeriod = false;
            Cognitive.InteractionCharacteristics.MaximumReceptionsPerPeriod = 1;
            // AverageDone in the simulator
            Cognitive.InteractionCharacteristics.PreferredCommunicationMediums =
                CommunicationMediums.FaceToFace | CommunicationMediums.Phone;
            // Tasks and performance
            Cognitive.TasksAndPerformance.CanPerformTask = false;
            Cognitive.TasksAndPerformance.LearningRate = 0.05F;
            Cognitive.TasksAndPerformance.LearningByDoingRate = 0.1F;
            // Cognitive.InteractionPatterns
            Cognitive.InteractionPatterns.IsPartOfInteractionSphere = true;
            Cognitive.InteractionPatterns.AllowNewInteractions = true;
            Cognitive.InteractionPatterns.LimitNumberOfNewInteractions = false;
            Cognitive.InteractionPatterns.ThresholdForNewInteraction = 0.2F;
            Cognitive.InteractionPatterns.MaxNumberOfNewInteractions = 1;
            Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Rarely;
            Cognitive.InteractionPatterns.IsolationCyclicity = Cyclicity.Cyclical;
            Cognitive.InteractionPatterns.InteractionsBasedOnHomophily = 1;
            Cognitive.InteractionPatterns.InteractionsBasedOnKnowledge = 0;
            Cognitive.InteractionPatterns.InteractionsBasedOnActivities = 0;
            Cognitive.InteractionPatterns.InteractionsBasedOnBeliefs = 0;
            Cognitive.InteractionPatterns.InteractionsBasedOnSocialDemographics = 0;
        }
    }
}