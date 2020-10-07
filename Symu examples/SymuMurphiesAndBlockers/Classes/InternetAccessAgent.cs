#region Licence

// Description: SymuBiz - SymuMurphiesAndBlockers
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Common.Interfaces;
using Symu.Environment;
using Symu.Messaging.Messages;

#endregion

namespace SymuMurphiesAndBlockers.Classes
{
    /// <summary>
    ///     Provide an access to internet information if DynamicEnvironmentModel is On
    /// </summary>
    public sealed class InternetAccessAgent : CognitiveAgent
    {
        public const byte Class = 1;

        /// <summary>
        ///     Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        private InternetAccessAgent(SymuEnvironment environment,
            CognitiveArchitectureTemplate template) : base(
            ClassId, environment, template)
        {
        }

        public static IClassId ClassId => new ClassId(Class);
        private ExampleMainOrganization MainOrganization => ((ExampleEnvironment) Environment).ExampleMainOrganization;

        /// <summary>
        ///     Factory method to create an agent
        ///     Call the Initialize method
        /// </summary>
        /// <returns></returns>
        public static InternetAccessAgent CreateInstance(SymuEnvironment environment,
            CognitiveArchitectureTemplate template)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var agent = new InternetAccessAgent(environment, template);
            agent.Initialize();
            return agent;
        }

        /// <summary>
        ///     Customize the models of the agent
        ///     After setting the Agent basics models
        /// </summary>
        public override void SetModels()
        {
            base.SetModels();
            foreach (var knowledgeId in Environment.MainOrganization.MetaNetwork.Knowledge.GetEntityIds())
            {
                KnowledgeModel.AddKnowledge(knowledgeId, MainOrganization.KnowledgeLevel,
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