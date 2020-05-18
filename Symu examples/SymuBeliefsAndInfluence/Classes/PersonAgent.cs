#region Licence

// Description: Symu - SymuGroupAndInteraction
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using SymuEngine.Classes.Agents;
using SymuEngine.Classes.Agents.Models.Templates;
using SymuEngine.Classes.Task;
using SymuEngine.Environment;
using SymuEngine.Messaging.Messages;
using SymuEngine.Repository;
using SymuEngine.Repository.Networks.Knowledges;

#endregion

namespace SymuBeliefsAndInfluence.Classes
{
    public sealed class PersonAgent : Agent
    {
        public const byte ClassKey = SymuYellowPages.Actor;
        private MurphyTask Model => ((ExampleEnvironment) Environment).Model;
        private SimpleHumanTemplate Template => ((ExampleEnvironment) Environment).WorkerTemplate;
        public List<Knowledge> Knowledges => ((ExampleEnvironment) Environment).Knowledges;
        public IEnumerable<AgentId> Influencers => ((ExampleEnvironment) Environment).Influencers.Select(x => x.Id);
        public PersonAgent(ushort agentKey, SymuEnvironment environment) : base(
            new AgentId(agentKey, ClassKey),
            environment)
        {
            SetCognitive(Template);
        }

        public override void GetNewTasks()
        {
            var task = new SymuTask(TimeStep.Step)
            {
                Weight = 1
            };
            task.SetKnowledgesBits(Model, Knowledges, 1);
            Post(task);
        }

        /// <summary>
        /// True if belief are ok to do the task
        /// </summary>
        /// <param name="task"></param>
        /// <param name="knowledgeId"></param>
        /// <param name="mandatoryScore"></param>
        /// <param name="requiredScore"></param>
        /// <param name="mandatoryIndex"></param>
        /// <param name="requiredIndex"></param>
        /// <returns></returns>
        protected override void CheckBlockerBelief(SymuTask task, ushort knowledgeId, float mandatoryScore, float requiredScore, byte mandatoryIndex, byte requiredIndex)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            base.CheckBlockerBelief(task, knowledgeId, mandatoryScore, requiredScore, mandatoryIndex, requiredIndex);
            if (mandatoryScore > -Cognitive.InternalCharacteristics.RiskAversionThreshold)
            {
                return;
            }

            // RiskAversionThreshold has been exceeded =>
            // Worker don't want to do the task, the task is blocked in base method
            // Ask advice from influencers
            var attachments = new MessageAttachments
            {
                KnowledgeId = knowledgeId,
                KnowledgeBit = mandatoryIndex
            };
            SendToMany(Influencers, MessageAction.Ask, SymuYellowPages.Belief, attachments, CommunicationMediums.Email);
        }
    }
}