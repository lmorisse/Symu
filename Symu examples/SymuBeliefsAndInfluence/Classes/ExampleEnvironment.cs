#region Licence

// Description: SymuBiz - SymuBeliefsAndInfluence
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Classes.Organization;
using Symu.Common;
using Symu.Environment;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace SymuBeliefsAndInfluence.Classes
{
    public class ExampleEnvironment : SymuEnvironment
    {
        public byte WorkersCount { get; set; } = 5;
        public byte InfluencersCount { get; set; } = 2;
        public byte KnowledgeCount { get; set; } = 2;
        public List<InfluencerAgent> Influencers { get; } = new List<InfluencerAgent>();
        public PromoterTemplate InfluencerTemplate { get; } = new PromoterTemplate();

        public SimpleHumanTemplate WorkerTemplate { get; } = new SimpleHumanTemplate();

        public override void SetOrganization(OrganizationEntity organization)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }

            base.SetOrganization(organization);

            organization.Models.Influence.On = true;
            organization.Models.Influence.RateOfAgentsOn = 1;
            organization.Models.Beliefs.On = true;
            organization.Models.Beliefs.RateOfAgentsOn = 1;
            organization.Models.Generator = RandomGenerator.RandomUniform;
            organization.Murphies.Off();
            organization.Murphies.IncompleteBelief.On = true;
            organization.Murphies.IncompleteBelief.RateOfAgentsOn = 1;
            IterationResult.Tasks.On = true;
            IterationResult.KnowledgeAndBeliefResults.On = true;
            IterationResult.OrganizationFlexibility.On = true;

            // Interaction sphere setup
            organization.Models.InteractionSphere.On = true;
            organization.Models.InteractionSphere.SphereUpdateOverTime = true;
            organization.Models.InteractionSphere.FrequencyOfSphereUpdate = TimeStepType.Monthly;
            organization.Models.InteractionSphere.RandomlyGeneratedSphere = false;
            organization.Models.InteractionSphere.RelativeBeliefWeight = 0.5F;
            organization.Models.InteractionSphere.RelativeActivityWeight = 0;
            organization.Models.InteractionSphere.RelativeKnowledgeWeight = 0.25F;
            organization.Models.InteractionSphere.SocialDemographicWeight = 0.25F;

            Organization.Communication.Email.CostToReceiveLevel = GenericLevel.None;
            Organization.Communication.Email.CostToSendLevel = GenericLevel.None;

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
                var knowledge = new Knowledge((ushort)i, i.ToString(), 10);
                Organization.AddKnowledge(knowledge);
            }
        }

        public override void SetAgents()
        {
            base.SetAgents();

            var agentIds = new List<AgentId>();

            for (var j = 0; j < InfluencersCount; j++)
            {
                var actor = new InfluencerAgent(Organization.NextEntityIndex(), this, InfluencerTemplate);
                Influencers.Add(actor);
                agentIds.Add(actor.Id);
            }

            for (var j = 0; j < WorkersCount; j++)
            {
                var actor = new PersonAgent(Organization.NextEntityIndex(), this, WorkerTemplate);
                agentIds.Add(actor.Id);
            }

            WhitePages.Network.NetworkLinks.AddLinks(agentIds);
        }
    }
}