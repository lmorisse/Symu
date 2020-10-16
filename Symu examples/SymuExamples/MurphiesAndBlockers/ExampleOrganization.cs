#region Licence

// Description: SymuBiz - SymuMurphiesAndBlockers
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Classes.Organization;
using Symu.Common.Classes;
using Symu.Messaging.Messages;
using Symu.Repository.Entities;

#endregion

namespace SymuExamples.MurphiesAndBlockers
{
    public class ExampleMainOrganization : MainOrganization
    {
        public ExampleMainOrganization() : base("symu")
        {
            Models.Beliefs.On = true;
            Models.Knowledge.On = true;
            // For email knowledge storing
            Models.Learning.On = true;
            Models.Forgetting.On = false;
            Models.Generator = RandomGenerator.RandomUniform;

            Murphies.IncompleteKnowledge.CommunicationMediums = CommunicationMediums.Email;
            Murphies.IncompleteBelief.CommunicationMediums = CommunicationMediums.Email;
        }

        public byte WorkersCount { get; set; } = 5;
        public byte KnowledgeCount { get; set; } = 2;

        public KnowledgeLevel KnowledgeLevel { get; set; } = KnowledgeLevel.Intermediate;

        public override MainOrganization Clone()
        {
            var clone = new ExampleMainOrganization();
            CopyTo(clone);
            clone.WorkersCount = WorkersCount;
            clone.KnowledgeCount = KnowledgeCount;
            clone.KnowledgeLevel = KnowledgeLevel;
            return clone;
        }

        /// <summary>
        ///     Add Organization knowledge
        /// </summary>
        public void AddKnowledge()
        {
            // KnowledgeCount are added for tasks initialization
            // Adn Beliefs are created based on knowledge
            for (var i = 0; i < KnowledgeCount; i++)
            {
                // knowledge length of 10 is arbitrary in this example
                Knowledge.CreateInstance(MetaNetwork, Models, i.ToString(), 10);
            }
        }
    }
}