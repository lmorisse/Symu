#region Licence

// Description: SymuBiz - SymuLearnAndForget
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.Repository;

#endregion

namespace SymuLearnAndForget.Classes
{
    public sealed class LearnByAskingAgent : LearnAgent
    {
        /// <summary>
        ///     Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        private LearnByAskingAgent(SymuEnvironment environment, CognitiveArchitectureTemplate template)
            : base(environment, template)
        {
        }

        /// <summary>
        ///     Factory method to create an agent
        ///     Call the Initialize method
        /// </summary>
        /// <returns></returns>
        public new static LearnByAskingAgent CreateInstance(SymuEnvironment environment,
            CognitiveArchitectureTemplate template)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var agent = new LearnByAskingAgent(environment, template);
            agent.Initialize();
            return agent;
        }

        /// <summary>
        ///     Agent ask an expert for the information
        /// </summary>
        public override void GetNewTasks()
        {
            var attachments = new MessageAttachments
            {
                KnowledgeId = Knowledge.EntityId,
                KnowledgeBit = Knowledge.GetRandomBitIndex()
            };
            Send(((ExampleEnvironment) Environment).ExpertAgent.AgentId, MessageAction.Ask, SymuYellowPages.Knowledge,
                attachments, CommunicationMediums.Email);
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
                case MessageAction.Reply:
                    // Agent has already learned from the reply
                    // We need to store this information in the wiki
                    Wiki.StoreKnowledge(Knowledge.EntityId, message.Attachments.KnowledgeBits, 1, Schedule.Step);
                    break;
            }
        }
    }
}