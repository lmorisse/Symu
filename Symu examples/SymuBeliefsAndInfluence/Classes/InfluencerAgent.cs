#region Licence

// Description: SymuBiz - SymuBeliefsAndInfluence
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Common;
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.Repository;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace SymuBeliefsAndInfluence.Classes
{
    public sealed class InfluencerAgent : Agent
    {
        public const byte ClassKey = SymuYellowPages.Actor;

        public InfluencerAgent(ushort agentKey, SymuEnvironment environment,
            CognitiveArchitectureTemplate template) : base(
            new AgentId(agentKey, ClassKey), environment, template)
        {
        }

        public IEnumerable<Knowledge> Knowledges => ((ExampleEnvironment) Environment).Knowledges;

        /// <summary>
        ///     Customize the cognitive architecture of the agent
        ///     After setting the Agent template
        /// </summary>
        protected override void SetCognitive()
        {
            base.SetCognitive();
            Cognitive.InteractionPatterns.IsolationCyclicity = Cyclicity.None;
            Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            Cognitive.InteractionPatterns.AllowNewInteractions = false;
            Cognitive.InteractionCharacteristics.PreferredCommunicationMediums =
                CommunicationMediums.Email;
        }

        /// <summary>
        ///     Customize the models of the agent
        ///     After setting the Agent basics models
        /// </summary>
        protected override void SetModels()
        {
            base.SetModels();
            foreach (var knowledge in Knowledges)
            {
                KnowledgeModel.AddKnowledge(knowledge.Id, KnowledgeLevel.FullKnowledge,
                    Cognitive.InternalCharacteristics);
                BeliefsModel.AddBelief(knowledge.Id, Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel);
            }
        }

        /// <summary>
        ///     This is where the main logic of the agent should be placed.
        /// </summary>
        /// <param name="message"></param>
        public override void ActMessage(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            base.ActMessage(message);
            switch (message.Subject)
            {
                case SymuYellowPages.Belief:
                    AskBelief(message);
                    break;
            }
        }

        /// <summary>
        ///     Influencer send back its own belief if he can send beliefs
        ///     which has an impact on the beliefs of the worker if he can receive them
        /// </summary>
        /// <param name="message"></param>
        private void AskBelief(Message message)
        {
            var replyMessage = Message.ReplyMessage(message);
            Reply(replyMessage);
        }
    }
}