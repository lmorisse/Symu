#region Licence

// Description: Symu - SymuBeliefsAndInfluence
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.Templates;
using Symu.Classes.Organization;
using Symu.Classes.Task;
using Symu.Common;
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace SymuBeliefsAndInfluence.Classes
{
    public class ExampleEnvironment : SymuEnvironment
    {
        public byte WorkersCount { get; set; } = 5;
        public byte InfluencersCount { get; set; } = 2;
        public byte KnowledgeCount { get; set; } = 2;
        public List<Knowledge> Knowledges { get; private set; }
        public List<InfluencerAgent> Influencers { get; } = new List<InfluencerAgent>();
        public SimpleHumanTemplate InfluencerTemplate { get; } = new SimpleHumanTemplate();
        public SimpleHumanTemplate WorkerTemplate { get; } = new SimpleHumanTemplate();
        public MurphyTask Model { get; } = new MurphyTask();

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
            organization.Models.FollowGroupKnowledge = true;
            organization.Models.FollowGroupFlexibility = true;
            organization.Models.FollowTasks = true;
            organization.Models.InteractionSphere.On = true;
            organization.Models.InteractionSphere.SphereUpdateOverTime = true;
            organization.Models.InteractionSphere.FrequencyOfSphereUpdate = TimeStepType.Monthly;
            organization.Models.InteractionSphere.RandomlyGeneratedSphere = false;
            // Interaction sphere setup
            organization.Models.InteractionSphere.RelativeBeliefWeight = 0.5F;
            organization.Models.InteractionSphere.RelativeActivityWeight = 0;
            organization.Models.InteractionSphere.RelativeKnowledgeWeight = 0.25F;
            organization.Models.InteractionSphere.SocialDemographicWeight = 0.25F;

            SetDebug(false);
        }

        public override void SetModelForAgents()
        {
            base.SetModelForAgents();

            #region Common

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

            #endregion

            var agentIds = new List<AgentId>();

            #region Influencer

            InfluencerTemplate.Cognitive.InteractionPatterns.IsolationIsRandom = false;
            InfluencerTemplate.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            InfluencerTemplate.Cognitive.InteractionPatterns.AllowNewInteractions = false;
            InfluencerTemplate.Cognitive.InteractionCharacteristics.PreferredCommunicationMediums =
                CommunicationMediums.Email;
            InfluencerTemplate.Cognitive.KnowledgeAndBeliefs.HasInitialBelief = true;
            InfluencerTemplate.Cognitive.InternalCharacteristics.InfluenceabilityRateMin = 0;
            InfluencerTemplate.Cognitive.InternalCharacteristics.InfluenceabilityRateMax = 0;

            for (var j = 0; j < InfluencersCount; j++)
            {
                var actor = new InfluencerAgent(Organization.NextEntityIndex(), this);
                //Beliefs are added with knowledge based on DefaultBeliefLevel of the influencer cognitive template
                SetKnowledge(actor, Knowledges);
                Influencers.Add(actor);
                agentIds.Add(actor.Id);
            }

            #endregion

            #region worker

            WorkerTemplate.Cognitive.InteractionPatterns.IsolationIsRandom = false;
            WorkerTemplate.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            WorkerTemplate.Cognitive.InteractionPatterns.AllowNewInteractions = false;
            WorkerTemplate.Cognitive.InteractionCharacteristics.PreferredCommunicationMediums =
                CommunicationMediums.Email;
            WorkerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMin = 0;
            WorkerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMax = 0F;
            WorkerTemplate.Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds = true;
            for (var j = 0; j < WorkersCount; j++)
            {
                var actor = new PersonAgent(Organization.NextEntityIndex(), this);
                //Beliefs are added with knowledge based on DefaultBeliefLevel of the worker cognitive template
                SetKnowledge(actor, Knowledges);
                agentIds.Add(actor.Id);
            }

            #endregion

            WhitePages.Network.NetworkLinks.AddLinks(agentIds);
        }

        private void SetKnowledge(Agent actor, IReadOnlyList<Knowledge> knowledges)
        {
            for (var i = 0; i < KnowledgeCount; i++)
            {
                actor.KnowledgeModel.AddKnowledge(knowledges[i].Id,
                    KnowledgeLevel.FullKnowledge,
                    Organization.Templates.Human.Cognitive.InternalCharacteristics);
                actor.BeliefsModel.AddBelief(knowledges[i].Id);
            }
        }
    }
}