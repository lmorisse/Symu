#region Licence

// Description: Symu - SymuGroupAndInteraction
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using SymuEngine.Classes.Agents;
using SymuEngine.Classes.Agents.Models.CognitiveArchitecture;
using SymuEngine.Classes.Agents.Models.Templates;
using SymuEngine.Classes.Task;
using SymuEngine.Common;
using SymuEngine.Environment;
using SymuEngine.Messaging.Messages;
using SymuEngine.Repository.Networks.Knowledges;
using SymuTools.Math.ProbabilityDistributions;

#endregion

namespace SymuBeliefsAndInfluence.Classes
{
    public class ExampleEnvironment : SymuEnvironment
    {
        public byte KnowledgeCount { get; set; } = 2;
        public byte WorkersCount { get; set; } = 5;
        public byte InfluencersCount { get; set; } = 5;
        public byte Knowledge { get; set; } = 0;
        public List<Knowledge> Knowledges { get; private set; }
        public List<InfluenceurAgent> Influencers { get; } = new List<InfluenceurAgent>();
        public SimpleHumanTemplate InfluencerTemplate { get; } = new SimpleHumanTemplate();
        public SimpleHumanTemplate WorkerTemplate { get; } = new SimpleHumanTemplate();
        public MurphyTask Model { get; } = new MurphyTask();

        public override void SetModelForAgents()
        {
            base.SetModelForAgents();
            Organization.Models.Generator = RandomGenerator.RandomUniform;

            #region Influencer
            InfluencerTemplate.Cognitive.InteractionPatterns.IsolationIsRandom = false;
            InfluencerTemplate.Cognitive.InteractionPatterns.IsolationIsRandom = false;
            InfluencerTemplate.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            InfluencerTemplate.Cognitive.InteractionPatterns.AllowNewInteractions = false;
            InfluencerTemplate.Cognitive.InteractionCharacteristics.PreferredCommunicationMediums =
                CommunicationMediums.Email;
            InfluencerTemplate.Cognitive.KnowledgeAndBeliefs.HasInitialBelief = true;
            InfluencerTemplate.Cognitive.InternalCharacteristics.InfluenceabilityRateMin = 0;
            InfluencerTemplate.Cognitive.InternalCharacteristics.InfluenceabilityRateMax = 0.1F;
            #endregion

            #region worker
            WorkerTemplate.Cognitive.InteractionPatterns.IsolationIsRandom = false;
            WorkerTemplate.Cognitive.InteractionPatterns.IsolationIsRandom = false;
            WorkerTemplate.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            WorkerTemplate.Cognitive.InteractionPatterns.AllowNewInteractions = false;
            WorkerTemplate.Cognitive.InteractionCharacteristics.PreferredCommunicationMediums =
                CommunicationMediums.Email;
            WorkerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMin = 0;
            WorkerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMax = 0.1F;
            #endregion

            Organization.Models.FollowGroupKnowledge = true;
            Organization.Models.FollowGroupFlexibility= true;
            Organization.Models.InteractionSphere.On = true;
            Organization.Models.InteractionSphere.SphereUpdateOverTime = true;
            Organization.Models.InteractionSphere.FrequencyOfSphereUpdate = TimeStepType.Monthly;
            Organization.Models.InteractionSphere.RandomlyGeneratedSphere = false;
            
            Knowledges = new List<Knowledge>();
            for (var i = 0; i < KnowledgeCount; i++)
            {
                // knowledge length of 10 is arbitrary in this example
                var knowledge = new Knowledge((ushort) i, i.ToString(), 10);
                WhitePages.Network.AddKnowledge(knowledge);
                Knowledges.Add(knowledge);
                //Beliefs are created based on knowledge
            }

            var agentIds = new List<AgentId>();
            for (var j = 0; j < WorkersCount; j++)
            {
                var actor = new PersonAgent(Organization.NextEntityIndex(), this);
                //Beliefs are added with knowledge
                SetKnowledge(actor, Knowledges);
                agentIds.Add(actor.Id);
            }
            // All PersonAgent are linked, so that they can interact between each others
            Organization.Models.InteractionSphere.RelativeActivityWeight = 0F;
            Organization.Models.InteractionSphere.RelativeBeliefWeight = 0.5F;
            Organization.Models.InteractionSphere.RelativeKnowledgeWeight = 0F;
            Organization.Models.InteractionSphere.SocialDemographicWeight = 0.5F;
            WhitePages.Network.NetworkLinks.AddLinks(agentIds);

            for (var j = 0; j < WorkersCount; j++)
            {
                var actor = new InfluenceurAgent(Organization.NextEntityIndex(), this);
                //Beliefs are added with knowledge
                SetKnowledge(actor, Knowledges);
                Influencers.Add(actor);
            }
        }

        private void SetKnowledge(Agent actor, IReadOnlyList<Knowledge> knowledges)
        {
            for (var i = 0; i < KnowledgeCount; i++)
            {
                actor.Cognitive.KnowledgeAndBeliefs.AddKnowledge(knowledges[i],
                    KnowledgeLevel.FullKnowledge,
                        Organization.Templates.Human.Cognitive.InternalCharacteristics);

            }
        }
    }
}