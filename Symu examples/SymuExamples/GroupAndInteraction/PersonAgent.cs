#region Licence

// Description: SymuBiz - SymuGroupAndInteraction
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
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.OrgMod.Entities;
using Symu.Repository;

#endregion

namespace SymuExamples.GroupAndInteraction
{
    public sealed class PersonAgent : CognitiveAgent
    {
        public const byte Class = SymuYellowPages.Actor;

        /// <summary>
        ///     Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        private PersonAgent(IAgentId entityId, SymuEnvironment environment,
            CognitiveArchitectureTemplate template) : base(
            entityId, environment, template)
        {
        }

        public static IClassId ClassId => new ClassId(Class);

        /// <summary>
        ///     Agent is member of a group
        /// </summary>
        public IAgentId GroupId { get; set; }

        /// <summary>
        ///     Factory method to create an agent
        ///     Call the Initialize method
        /// </summary>
        /// <returns></returns>
        public static PersonAgent CreateInstance(SymuEnvironment environment, CognitiveArchitectureTemplate template)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var entity = new ActorEntity(environment.MainOrganization.MetaNetwork);
            var agent = new PersonAgent(entity.EntityId, environment, template);
            agent.Initialize();
            return agent;
        }

        /// <summary>
        ///     Customize the cognitive architecture of the agent
        ///     After setting the Agent template
        /// </summary>
        protected override void SetCognitive()
        {
            base.SetCognitive();
            // Communication medium
            Cognitive.InteractionCharacteristics.PreferredCommunicationMediums =
                CommunicationMediums.FaceToFace;
            Cognitive.InteractionPatterns.IsolationCyclicity = Cyclicity.None;
            Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
        }


        public override void ActMessage(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            base.ActMessage(message);
            switch (message.Subject)
            {
                case SymuYellowPages.Actor:
                    ActActor(message);
                    break;
            }
        }

        private void ActActor(Message message)
        {
            switch (message.Action)
            {
                case MessageAction.Ask:
                    AskActor(message);
                    break;
            }
        }

        private void AskActor(Message message)
        {
            // New interaction has already been accepted
            // Let's reply positively 
            var reply = Message.ReplyMessage(message);
            Send(reply);
        }

        public override void ActEndOfDay()
        {
            base.ActEndOfDay();
            // Time for a coffee break and have interaction with other agents
            Send(GroupId, MessageAction.Stop, SymuYellowPages.WorkingDay, CommunicationMediums.FaceToFace);
        }
    }
}