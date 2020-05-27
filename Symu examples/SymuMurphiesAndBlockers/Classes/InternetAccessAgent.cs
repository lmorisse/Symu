#region Licence

// Description: Symu - SymuMurphiesAndBlockers
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents;
using Symu.Environment;
using Symu.Messaging.Messages;

#endregion

namespace SymuMurphiesAndBlockers.Classes
{
    /// <summary>
    ///     Provide an access to internet information if DynamicEnvironmentModel is On
    /// </summary>
    public sealed class InternetAccessAgent : Agent
    {
        public const byte ClassKey = 1;

        public InternetAccessAgent(ushort agentKey, SymuEnvironment environment) : base(
            new AgentId(agentKey, ClassKey),
            environment)
        {
            SetCognitive(Environment.Organization.Templates.Internet);
        }

        /// <summary>
        ///     Ask Help from PersonAgent when blocked
        /// </summary>
        /// <return>a reply help message</return>
        public override void AskHelp(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var reply = Message.ReplyMessage(message);
            Reply(reply);
        }
    }
}