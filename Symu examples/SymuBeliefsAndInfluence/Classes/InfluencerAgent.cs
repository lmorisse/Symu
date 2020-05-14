#region Licence

// Description: Symu - SymuGroupAndInteraction
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;
using SymuEngine.Classes.Agents;
using SymuEngine.Classes.Agents.Models.CognitiveArchitecture;
using SymuEngine.Classes.Task;
using SymuEngine.Environment;
using SymuEngine.Messaging.Messages;
using SymuEngine.Repository;

#endregion

namespace SymuBeliefsAndInfluence.Classes
{
    public sealed class InfluenceurAgent : Agent
    {
        public const byte ClassKey = SymuYellowPages.Actor;
        public InfluenceurAgent(ushort agentKey, SymuEnvironment environment) : base(
            new AgentId(agentKey, ClassKey),
            environment)
        {
            SetCognitive(((ExampleEnvironment) Environment).InfluencerTemplate);
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

        private void AskBelief(Message message)
        {
            var replyMessage = Message.ReplyMessage(message);
            // In this reply message, agent will send back its own belief if he can send beliefs
            // that will have an impact on the beliefs of the sender if he can receive beliefs
            Reply(replyMessage);
        }
    }
}