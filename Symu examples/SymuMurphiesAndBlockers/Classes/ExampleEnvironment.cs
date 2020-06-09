#region Licence

// Description: SymuBiz - SymuMurphiesAndBlockers
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using Symu.Classes.Organization;
using Symu.Classes.Task;
using Symu.Common;
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.Messaging.Templates;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace SymuMurphiesAndBlockers.Classes
{
    public class ExampleEnvironment : SymuEnvironment
    {
        public byte WorkersCount { get; set; } = 5;
        public byte KnowledgeCount { get; set; } = 2;

        public KnowledgeLevel KnowledgeLevel { get; set; } = KnowledgeLevel.Intermediate;
        public List<Knowledge> Knowledges { get; private set; }
        public MurphyTask Model => Organization.Murphies.IncompleteKnowledge;

        public InternetAccessAgent Internet { get; private set; }

        public override void SetOrganization(OrganizationEntity organization)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }

            base.SetOrganization(organization);

            IterationResult.Blockers.On = true;
            IterationResult.Tasks.On = true;
            // For email knowledge storing
            organization.Models.Learning.On = true;
            organization.Models.Learning.RateOfAgentsOn = 1;
            organization.Models.Forgetting.On = false;
            organization.Models.Generator = RandomGenerator.RandomUniform;

            organization.Murphies.IncompleteKnowledge.CommunicationMediums = CommunicationMediums.Email;
            organization.Murphies.IncompleteBelief.CommunicationMediums = CommunicationMediums.Email;

            SetDebug(false);
        }

        public override void SetAgents()
        {
            base.SetAgents();

            // KnowledgeCount are added for tasks initialization
            // Adn Beliefs are created based on knowledge
            Knowledges = new List<Knowledge>();
            for (var i = 0; i < KnowledgeCount; i++)
            {
                // knowledge length of 10 is arbitrary in this example
                var knowledge = new Knowledge((ushort) i, i.ToString(), 10);
                WhitePages.Network.AddKnowledge(knowledge);
                Knowledges.Add(knowledge);
            }

            var group = new GroupAgent(Organization.NextEntityIndex(), this);
            Internet = new InternetAccessAgent(Organization.NextEntityIndex(), this, Organization.Templates.Internet);
            for (var j = 0; j < WorkersCount; j++)
            {
                var actor = new PersonAgent(Organization.NextEntityIndex(), this, Organization.Templates.Human)
                {
                    GroupId = group.Id
                };
                CommunicationTemplate communication = new EmailTemplate();
                WhitePages.Network.AddEmail(actor.Id, communication);
                WhitePages.Network.AddMemberToGroup(actor.Id, 100, group.Id);
            }
        }
    }
}