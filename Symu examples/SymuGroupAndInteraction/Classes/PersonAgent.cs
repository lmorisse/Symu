#region Licence

// Description: Symu - SymuGroupAndInteraction
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Classes.Agents;
using SymuEngine.Environment;
using SymuEngine.Messaging.Messages;
using SymuEngine.Repository;

#endregion

namespace SymuGroupAndInteraction.Classes
{
    public sealed class PersonAgent : Agent
    {
        public const byte ClassKey = SymuYellowPages.Actor;

        public PersonAgent(ushort agentKey, SymuEnvironment environment) : base(
            new AgentId(agentKey, ClassKey),
            environment)
        {
            SetCognitive(Environment.Organization.Templates.Human);
            // Communication medium
            Cognitive.InteractionCharacteristics.PreferredCommunicationMediums =
                CommunicationMediums.FaceToFace;
        }

        /// <summary>
        ///     Agent is member of a group
        /// </summary>
        public AgentId GroupId { get; set; }


        protected override void ActClassKey(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            base.ActClassKey(message);
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