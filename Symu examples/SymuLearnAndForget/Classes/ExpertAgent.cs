#region Licence

// Description: Symu - SymuLearnAndForget
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Classes.Agent;
using SymuEngine.Common;
using SymuEngine.Environment;
using SymuEngine.Messaging.Message;
using SymuEngine.Repository;

#endregion

namespace SymuLearnAndForget.Classes
{
    public sealed class ExpertAgent : Agent
    {
        public const byte ClassKey = 2;

        public ExpertAgent(ushort agentKey, SymuEnvironment environment) : base(
            new AgentId(agentKey, ClassKey),
            environment)
        {
            SetCognitive(Environment.Organization.Templates.SimpleHuman);
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
                case SymuYellowPages.knowledge:
                    ActKnowledge(message);
                    break;
            }
        }

        private void ActKnowledge(Message message)
        {
            switch (message.Action)
            {
                case MessageAction.Ask:
                    var reply = Message.ReplyMessage(message);
                    // Reply manage the knowledge send back to the agent
                    Reply(reply);
                    break;
            }
        }
    }
}