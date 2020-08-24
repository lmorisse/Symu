#region Licence

// Description: SymuBiz - SymuLearnAndForget
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Common;
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.Repository;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace SymuLearnAndForget.Classes
{
    public sealed class ExpertAgent : CognitiveAgent
    {
        public const byte Class = 2;

        public ExpertAgent(UId id, SymuEnvironment environment, CognitiveArchitectureTemplate template) : base(
            new AgentId(id, Class), environment, template)
        {
        }

        /// <summary>
        ///     Customize the cognitive architecture of the agent
        ///     After setting the Agent template
        /// </summary>
        protected override void SetCognitive()
        {
            base.SetCognitive();
            Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge = true;
        }

        /// <summary>
        ///     Customize the models of the agent
        ///     After setting the Agent basics models
        /// </summary>
        public override void SetModels()
        {
            base.SetModels();
            KnowledgeModel.AddKnowledge(((ExampleEnvironment) Environment).Knowledge.Id, KnowledgeLevel.Expert,
                Cognitive.InternalCharacteristics);
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
                case SymuYellowPages.Knowledge:
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