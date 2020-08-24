#region Licence

// Description: SymuBiz - SymuMurphiesAndBlockers
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Common.Interfaces.Entity;
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace SymuMurphiesAndBlockers.Classes
{
    /// <summary>
    ///     Provide an access to internet information if DynamicEnvironmentModel is On
    /// </summary>
    public sealed class InternetAccessAgent : CognitiveAgent
    {
        public const byte Class = 1;

        public InternetAccessAgent(UId id, SymuEnvironment environment,
            CognitiveArchitectureTemplate template) : base(
            new AgentId(id, Class), environment, template)
        {
        }

        public IEnumerable<Knowledge> Knowledges => Environment.Organization.Knowledges;

        /// <summary>
        ///     Customize the models of the agent
        ///     After setting the Agent basics models
        /// </summary>
        public override void SetModels()
        {
            base.SetModels();
            foreach (var knowledge in Knowledges)
            {
                KnowledgeModel.AddKnowledge(knowledge.Id, ((ExampleEnvironment) Environment).KnowledgeLevel,
                    Cognitive.InternalCharacteristics);
            }
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