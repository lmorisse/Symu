#region Licence

// Description: SymuBiz - SymuScenariosAndEvents
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Classes.Organization;
using Symu.Repository.Entities;

#endregion

namespace SymuExamples.ScenariosAndEvents
{
    public class ExampleMainOrganization : MainOrganization
    {
        public ExampleMainOrganization() : base("symu")
        {
            Models.Knowledge.On = true;
        }

        public byte WorkersCount { get; set; } = 5;
        public byte KnowledgeCount { get; private set; } = 2;

        public override MainOrganization Clone()
        {
            var clone = new ExampleMainOrganization();
            CopyTo(clone);
            clone.WorkersCount = WorkersCount;
            clone.KnowledgeCount = KnowledgeCount;
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
                Knowledge.CreateInstance(ArtifactNetwork, Models, i.ToString(), 10);
            }
        }
    }
}