#region Licence

// Description: SymuBiz - SymuBeliefsAndInfluence
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Common;
using Symu.Common.Interfaces;
using Symu.DNA.Entities;
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.Repository;
using Symu.Repository.Entities;

#endregion

namespace SymuBeliefsAndInfluence.Classes
{
    public sealed class InfluencerAgent : CognitiveAgent
    {
        public const byte Class = SymuYellowPages.Actor;
        public static IClassId ClassId => new ClassId(Class);
        /// <summary>
        /// Factory method to create an agent
        /// Call the Initialize method
        /// </summary>
        /// <returns></returns>
        public static InfluencerAgent CreateInstance(SymuEnvironment environment, CognitiveArchitectureTemplate template)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }
            var entity = new ActorEntity(environment.Organization.MetaNetwork);
            var agent = new InfluencerAgent(entity.EntityId, environment, template);
            agent.Initialize();
            return agent;
        }

        /// <summary>
        /// Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        private InfluencerAgent(IAgentId entityId, SymuEnvironment environment,
            CognitiveArchitectureTemplate template) : base(
            entityId, environment, template)
        {
        }

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
        public override void SetModels()
        {
            base.SetModels();
            foreach (var knowledgeId in Environment.Organization.MetaNetwork.Knowledge.GetEntityIds())
            {
                KnowledgeModel.AddKnowledge(knowledgeId, KnowledgeLevel.FullKnowledge,
                    Cognitive.InternalCharacteristics);
                BeliefsModel.AddBeliefFromKnowledgeId(knowledgeId, Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel);
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