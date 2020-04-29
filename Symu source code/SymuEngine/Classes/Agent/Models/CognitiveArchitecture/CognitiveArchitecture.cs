﻿#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Repository.Networks;

#endregion

namespace SymuEngine.Classes.Agent.Models.CognitiveArchitecture
{
    /// <summary>
    ///     Define the cognitive architecture model of an agent
    ///     Modules, processes and structure intended to emulate structural and functional components of human cognition :
    ///     working memory, long-term memory, attention, multi tasking, perception, situation assessment, decision making,
    ///     planning, learning, goal management, ...
    /// </summary>
    public class CognitiveArchitecture
    {
        public CognitiveArchitecture(Network network, AgentId agentId, byte randomLevel)
        {
            KnowledgeAndBeliefs = new KnowledgeAndBeliefs(network, agentId);
            InternalCharacteristics = new InternalCharacteristics(network, agentId);
            TasksAndPerformance = new TasksAndPerformance(network, agentId, randomLevel);
            MessageContent = new MessageContent();
            InteractionCharacteristics = new InteractionCharacteristics();
            InteractionPatterns = new InteractionPatterns();
        }

        /// <summary>
        ///     Knowledge & Beliefs from Construct Software
        ///     Knowledge and knowledge transactive memory
        ///     Beliefs and beliefs transactive memory
        ///     Referral
        /// </summary>
        public KnowledgeAndBeliefs KnowledgeAndBeliefs { get; }

        /// <summary>
        ///     InternalCharacteristics from Construct Software
        ///     Influentialness, influenceability
        ///     attention
        ///     forgetting
        ///     risk aversion
        ///     socio demographics
        /// </summary>
        public InternalCharacteristics InternalCharacteristics { get; }

        /// <summary>
        ///     Tasks & Performance from Construct Software
        ///     MultiTasking
        ///     Performs tasks
        ///     learning by doing
        /// </summary>
        public TasksAndPerformance TasksAndPerformance { get; }

        /// <summary>
        ///     Message content from Construct Software
        ///     Send & receive :
        ///     Knowledges
        ///     Beliefs
        ///     with transactive memory
        ///     Referral
        /// </summary>
        public MessageContent MessageContent { get; }

        /// <summary>
        ///     Interaction Characteristics from Construct Software
        ///     Interactions
        ///     Initiations (of interactions)
        ///     Receptions
        ///     Length of message
        ///     Distinct message
        /// </summary>
        public InteractionCharacteristics InteractionCharacteristics { get; }

        /// <summary>
        ///     InteractionPatterns from Construct Software
        ///     Sphere of interaction
        ///     Isolation
        ///     Interactions patterns
        /// </summary>
        public InteractionPatterns InteractionPatterns { get; }

        public void Initialize(ushort step)
        {
            KnowledgeAndBeliefs.InitializeExpertise(step);
            KnowledgeAndBeliefs.InitializeBeliefs();
        }

        public void CopyTo(CognitiveArchitecture cognitive)
        {
            if (cognitive is null)
            {
                throw new ArgumentNullException(nameof(cognitive));
            }

            KnowledgeAndBeliefs.CopyTo(cognitive.KnowledgeAndBeliefs);
            InternalCharacteristics.CopyTo(cognitive.InternalCharacteristics);
            TasksAndPerformance.CopyTo(cognitive.TasksAndPerformance);
            MessageContent.CopyTo(cognitive.MessageContent);
            InteractionCharacteristics.CopyTo(cognitive.InteractionCharacteristics);
            InteractionPatterns.CopyTo(cognitive.InteractionPatterns);
        }
    }
}