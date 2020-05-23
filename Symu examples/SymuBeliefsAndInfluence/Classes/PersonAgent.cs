#region Licence

// Description: Symu - SymuBeliefsAndInfluence
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.Templates;
using Symu.Classes.Blockers;
using Symu.Classes.Task;
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.Repository;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace SymuBeliefsAndInfluence.Classes
{
    public sealed class PersonAgent : Agent
    {
        public const byte ClassKey = SymuYellowPages.Actor;

        public PersonAgent(ushort agentKey, SymuEnvironment environment) : base(
            new AgentId(agentKey, ClassKey),
            environment)
        {
            SetCognitive(Template);
        }

        private MurphyTask Model => ((ExampleEnvironment) Environment).Model;
        private SimpleHumanTemplate Template => ((ExampleEnvironment) Environment).WorkerTemplate;
        public List<Knowledge> Knowledges => ((ExampleEnvironment) Environment).Knowledges;
        public IEnumerable<AgentId> Influencers => ((ExampleEnvironment) Environment).Influencers.Select(x => x.Id);

        public override void GetNewTasks()
        {
            var task = new SymuTask(Schedule.Step)
            {
                Weight = 1
            };
            task.SetKnowledgesBits(Model, Knowledges, 1);
            Post(task);
        }

        /// <summary>
        /// </summary>
        /// <param name="task"></param>
        /// <param name="blocker"></param>
        /// <returns></returns>
        public override void TryRecoverBlockerIncompleteBelief(SymuTask task, Blocker blocker)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (blocker is null)
            {
                throw new ArgumentNullException(nameof(blocker));
            }

            // RiskAversionThreshold has been exceeded =>
            // Worker don't want to do the task, the task is blocked in base method
            // Ask advice from influencers
            var attachments = new MessageAttachments
            {
                KnowledgeId = (ushort) blocker.Parameter,
                KnowledgeBit = (byte) blocker.Parameter2
            };
            SendToMany(Influencers, MessageAction.Ask, SymuYellowPages.Belief, attachments, CommunicationMediums.Email);
        }
    }
}