#region Licence

// Description: SymuBiz - SymuBeliefsAndInfluence
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Classes.Organization;
using Symu.Common;
using Symu.Common.Classes;
using Symu.Repository.Entities;

#endregion

namespace SymuBeliefsAndInfluence.Classes
{
    public class ExampleMainOrganization : MainOrganization
    {
        public ExampleMainOrganization() : base("Symu")
        {
            Models.Influence.On = true;
            Models.Beliefs.On = true;

            Models.Generator = RandomGenerator.RandomUniform;
            Murphies.SetOff();
            Murphies.IncompleteBelief.On = true;

            // Interaction sphere setup
            Models.InteractionSphere.On = true;
            Models.InteractionSphere.SphereUpdateOverTime = true;
            Models.InteractionSphere.RandomlyGeneratedSphere = false;
            Models.InteractionSphere.RelativeBeliefWeight = 0.5F;
            Models.InteractionSphere.RelativeActivityWeight = 0;
            Models.InteractionSphere.RelativeKnowledgeWeight = 0.25F;
            Models.InteractionSphere.SocialDemographicWeight = 0.25F;

            Communication.Email.CostToReceiveLevel = GenericLevel.None;
            Communication.Email.CostToSendLevel = GenericLevel.None;
        }

        public byte WorkersCount { get; set; } = 5;
        public byte InfluencersCount { get; set; } = 2;
        public byte BeliefCount { get; set; } = 2;
        public List<InfluencerAgent> Influencers { get; private set; } = new List<InfluencerAgent>();
        public PromoterTemplate InfluencerTemplate { get; private set; } = new PromoterTemplate();

        public SimpleHumanTemplate WorkerTemplate { get; private set; } = new SimpleHumanTemplate();

        public override MainOrganization Clone()
        {
            var clone = new ExampleMainOrganization();
            CopyTo(clone);
            clone.WorkersCount = WorkersCount;
            clone.InfluencersCount = InfluencersCount;
            clone.BeliefCount = BeliefCount;
            clone.Influencers = Influencers;
            clone.InfluencerTemplate = InfluencerTemplate;
            clone.WorkerTemplate = WorkerTemplate;
            return clone;
        }

        /// <summary>
        ///     Add Organization knowledge
        /// </summary>
        public void AddBeliefs()
        {
            // KnowledgeCount are added for tasks initialization
            // Adn Beliefs are created based on knowledge
            for (var i = 0; i < BeliefCount; i++)
            {
                // knowledge length of 10 is arbitrary in this example
                _ = new Knowledge(MetaNetwork, Models, i.ToString(), 10);
            }
        }
    }
}