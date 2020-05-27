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
using SymuTools;

#endregion

namespace SymuMurphiesAndBlockers.Classes
{
    public sealed class PersonAgent : Agent
    {
        public const byte ClassKey = SymuYellowPages.Actor;
        public AgentId GroupId { get; set; }

        public PersonAgent(ushort agentKey, SymuEnvironment environment) : base(
            new AgentId(agentKey, ClassKey),
            environment)
        {
            SetCognitive(Environment.Organization.Templates.Human);
        }

        private MurphyTask Model => ((ExampleEnvironment) Environment).Model;
        public List<Knowledge> Knowledges => ((ExampleEnvironment)Environment).Knowledges;
        public InternetAccessAgent Internet => ((ExampleEnvironment)Environment).Internet;

        public override void GetNewTasks()
        {
            var task = new SymuTask(Schedule.Step)
            {
                Weight = 1,
                // Creator is randomly  a person of the group - for the incomplete information murphy
                Creator = Environment.WhitePages.FilteredAgentIdsByClassKey(ClassKey).Shuffle().First()
            };
            task.SetKnowledgesBits(Model, Knowledges, 1);
            Post(task);
        }
        public override void TryRecoverBlockerIncompleteKnowledgeExternally(SymuTask task, Blocker blocker, ushort knowledgeId,
            byte knowledgeBit)
        {
            if (blocker == null)
            {
                throw new ArgumentNullException(nameof(blocker));
            }

            var attachments = new MessageAttachments();
            attachments.Add(blocker);
            attachments.Add(task);
            attachments.KnowledgeId = knowledgeId;
            attachments.KnowledgeBit = knowledgeBit;
            Send(Internet.Id, MessageAction.Ask, SymuYellowPages.Help, attachments,
                CommunicationMediums.ViaAPlatform);

        }
    }
}