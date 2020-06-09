#region Licence

// Description: Symu - SymuGroupAndInteraction
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
using Symu.Tools.Math.ProbabilityDistributions;

#endregion

namespace SymuGroupAndInteraction.Classes
{
    public sealed class PersonAgent : Agent
    {
        public const byte ClassKey = SymuYellowPages.Actor;

        public PersonAgent(ushort agentKey, SymuEnvironment environment, CognitiveArchitectureTemplate template) : base(
            new AgentId(agentKey, ClassKey), environment, template)
        {
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

        /// <summary>
        ///     Agent is member of a group
        /// </summary>
        public AgentId GroupId { get; set; }


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