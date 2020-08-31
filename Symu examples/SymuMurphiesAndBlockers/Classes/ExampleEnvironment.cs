﻿#region Licence

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
using Symu.Common.Classes;
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.Repository.Entity;

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

            var group = GroupAgent.CreateInstance(Organization.NextEntityId(), this);
            Internet = InternetAccessAgent.CreateInstance(Organization.NextEntityId(), this, Organization.Templates.Internet);
            for (var j = 0; j < WorkersCount; j++)
            {
                var actor = PersonAgent.CreateInstance(Organization.NextEntityId(), this, Organization.Templates.Human);
                actor.GroupId = group.AgentId;
                var email = Email.CreateInstance(actor.AgentId.Id, Organization.Models, WhitePages.MetaNetwork.Knowledge);
                var agentResource = new AgentResource(email.Id, new ResourceUsage(0));
                WhitePages.MetaNetwork.Resources.Add(actor.AgentId, email, agentResource);
                var agentGroup = new AgentGroup(actor.AgentId, 100);
                WhitePages.MetaNetwork.AddAgentToGroup(agentGroup, group.AgentId);
            }
        }
    }
}