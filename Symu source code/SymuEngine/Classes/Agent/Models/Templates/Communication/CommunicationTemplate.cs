#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Common;
using SymuTools.Classes.ProbabilityDistributions;

#endregion

namespace SymuEngine.Classes.Agent.Models.Templates.Communication
{
    /// <summary>
    ///     Set all the CognitiveArchitecture parameters for the CommunicationChannels
    ///     base class for all the communication channels
    /// </summary>
    public class CommunicationTemplate : CognitiveArchitectureTemplate
    {
        public CommunicationTemplate()
        {
            // Knowledge & Beliefs
            Cognitive.KnowledgeAndBeliefs.HasKnowledge = true;
            Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge = false;
            Cognitive.KnowledgeAndBeliefs.HasBelief = false;
            Cognitive.KnowledgeAndBeliefs.HasInitialBelief = false;
            // Message content
            Cognitive.MessageContent.CanSendKnowledge = true;
            Cognitive.MessageContent.CanReceiveKnowledge = false;
            Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend = 1;
            Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend = 1;
            Cognitive.MessageContent.CanSendBeliefs = true;
            Cognitive.MessageContent.CanReceiveBeliefs = false;
            Cognitive.MessageContent.MinimumNumberOfBitsOfBeliefToSend = 1;
            Cognitive.MessageContent.MaximumNumberOfBitsOfBeliefToSend = 1;
            // Internal Characteristics
            // Interaction Characteristics
            Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod = false;
            Cognitive.InteractionCharacteristics.LimitMessagesSentPerPeriod = true;
            Cognitive.InteractionCharacteristics.MaximumMessagesSentPerPeriod = 0;
            Cognitive.InteractionCharacteristics.LimitReceptionsPerPeriod = false;
            // Tasks and performance
            Cognitive.TasksAndPerformance.CanPerformTask = false; // For network.NetworkActivities.AddActivities
            Cognitive.TasksAndPerformance.TasksLimit.LimitSimultaneousTasks = false;
            Cognitive.TasksAndPerformance.TasksLimit.LimitTasksInTotal = false;
            Cognitive.TasksAndPerformance.LearningModel.On = false;
            Cognitive.TasksAndPerformance.LearningRate = 0;
            Cognitive.TasksAndPerformance.LearningByDoingRate = 0;
            // Cognitive.InteractionPatterns
            Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.VeryRarely;
        }

        /// <summary>
        ///     Cost : time spent to send an message
        ///     Range [0;1]
        /// </summary>
        /// <example>time spent to write an email</example>
        public GenericLevel CostToSendLevel { get; set; } = GenericLevel.Medium;

        /// <summary>
        ///     Cost : time spent to read an message
        ///     Range [0;1]
        /// </summary>
        /// <example>time spent to read an email</example>
        public GenericLevel CostToReceiveLevel { get; set; } = GenericLevel.Medium;

        /// <summary>
        ///     Maximum rate learnable the message can be
        ///     Range [0;1]
        /// </summary>
        /// <example>a phone call can be less learnable than an email because you can forget easier</example>
        /// <example>If 0, nothing is learnable from the message</example>
        /// <example>If 1, everything is learnable from the message</example>
        public float MaxRateLearnable { get; set; } = 1;

        public float CostToSend(byte random)
        {
            return Cost(CostToSendLevel, random);
        }

        public float CostToReceive(byte random)
        {
            return Cost(CostToReceiveLevel, random);
        }

        public static float Cost(GenericLevel level, byte random)
        {
            float cost;
            switch (level)
            {
                case GenericLevel.None:
                    cost = 0;
                    break;
                case GenericLevel.VeryLow:
                    cost = 0.05F;
                    break;
                case GenericLevel.Low:
                    cost = 0.1F;
                    break;
                case GenericLevel.Medium:
                    cost = 0.15F;
                    break;
                case GenericLevel.High:
                    cost = 0.20F;
                    break;
                case GenericLevel.VeryHigh:
                    cost = 0.25F;
                    break;
                case GenericLevel.Complete:
                    cost = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }

            cost = Normal.Sample(cost, 0.05F * random);
            return cost < 0 ? 0 : cost;
        }
    }
}