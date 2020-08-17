#region Licence

// Description: SymuBiz - SymuMurphiesAndBlockers
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Organization;
using Symu.Classes.Task;
using Symu.Common;
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.Messaging.Templates;
using Symu.Repository.Networks.Databases;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace SymuMurphiesAndBlockers.Classes
{
    public class ExampleEnvironment : SymuEnvironment
    {
        public byte WorkersCount { get; set; } = 5;
        public byte KnowledgeCount { get; set; } = 2;

        public KnowledgeLevel KnowledgeLevel { get; set; } = KnowledgeLevel.Intermediate;
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

        /// <summary>
        ///     Add Organization knowledge
        /// </summary>
        public override void AddOrganizationKnowledges()
        {
            base.AddOrganizationKnowledges();
            // KnowledgeCount are added for tasks initialization
            // Adn Beliefs are created based on knowledge
            for (var i = 0; i < KnowledgeCount; i++)
            {
                // knowledge length of 10 is arbitrary in this example
                var knowledge = new Knowledge((ushort) i, i.ToString(), 10);
                Organization.AddKnowledge(knowledge);
            }
        }

        public override void SetAgents()
        {
            base.SetAgents();

            var group = new GroupAgent(Organization.NextEntityIndex(), this);
            Internet = new InternetAccessAgent(Organization.NextEntityIndex(), this, Organization.Templates.Internet);
            for (var j = 0; j < WorkersCount; j++)
            {
                var actor = new PersonAgent(Organization.NextEntityIndex(), this, Organization.Templates.Human)
                {
                    GroupId = group.AgentId
                };
                CommunicationTemplate communication = new EmailTemplate();
                var entity = new DataBaseEntity(actor.AgentId, communication);
                var email = new Database(entity, Organization.Models, WhitePages.MetaNetwork.Knowledge);
                WhitePages.MetaNetwork.Databases.Add(actor.AgentId, email);
                WhitePages.MetaNetwork.AddAgentToGroup(actor.AgentId, 100, group.AgentId, false);
            }
        }
    }
}