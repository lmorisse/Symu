#region Licence

// Description: Symu - SymuBeliefsAndInfluence
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents;
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.Repository;

#endregion

namespace SymuBeliefsAndInfluence.Classes
{
    public sealed class InfluencerAgent : Agent
    {
        public const byte ClassKey = SymuYellowPages.Actor;

        public InfluencerAgent(ushort agentKey, SymuEnvironment environment) : base(
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