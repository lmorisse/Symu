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
using SymuEngine.Classes.Task.Knowledge;
using SymuEngine.Environment;
using SymuEngine.Messaging.Messages;
using SymuEngine.Repository;

#endregion

namespace SymuBeliefsAndInfluence.Classes
{
    public sealed class PersonAgent : Agent
    {
        public const byte ClassKey = SymuYellowPages.Actor;
        private readonly MurphyTask _model = new MurphyTask();

        public PersonAgent(ushort agentKey, SymuEnvironment environment) : base(
            new AgentId(agentKey, ClassKey),
            environment)
        {
            SetCognitive(Environment.Organization.Templates.Human);
            _model.RequiredRatio = 0.2F;
            _model.RequiredMandatoryRatio = 2F;
        }

        public override void GetNewTasks()
        {
            var task = new SymuTask(TimeStep.Step)
            {
                Weight = 1
            };
            foreach (var knowledge in ((ExampleEnvironment) Environment).Knowledges)
            {
                var bit = new TaskKnowledgeBits
                {
                    KnowledgeId = knowledge.Id
                };
                bit.SetRequired(knowledge.GetTaskRequiredBits(_model, 1));
                bit.SetMandatory(knowledge.GetTaskMandatoryBits(_model, 1));
                task.KnowledgesBits.Add(bit);
            }

            var doTask = true;
            foreach (var knowledgeId in task.KnowledgesBits.KnowledgeIds)
            {
                if (!CheckBelief(task, knowledgeId))
                {
                    doTask = false;
                    break;
                }
            }

            if (doTask)
            {
                TaskProcessor.Post(task);
            }
        }
        /// <summary>
        /// True if belief are ok to do the task
        /// </summary>
        /// <param name="task"></param>
        /// <param name="knowledgeId"></param>
        /// <returns></returns>
        public bool CheckBelief(SymuTask task, ushort knowledgeId)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            var taskBits = task.KnowledgesBits.GetBits(knowledgeId);
            float mandatoryScore = 0;
            float requiredScore = 0;
            byte mandatoryIndex = 0;
            byte requiredIndex = 0;
            Cognitive.KnowledgeAndBeliefs.CheckBelief(knowledgeId, taskBits, ref mandatoryScore, ref requiredScore,
                ref mandatoryIndex, ref requiredIndex);
            if (!(mandatoryScore < 0F))
            {
                return true;
            }

            // mandatoryScore is not enough => Worker don't want to do the task
            // Ask another agent advice
            var teammates = GetAgentIdsForInteractions(InteractionStrategy.Homophily).ToList();
            if (teammates.Any())
            {
                var attachments = new MessageAttachments
                {
                    KnowledgeId = knowledgeId,
                    KnowledgeBit = mandatoryIndex
                };
                SendToMany(teammates, MessageAction.Ask, SymuYellowPages.Belief, attachments, CommunicationMediums.Email);
            }

            return false;

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