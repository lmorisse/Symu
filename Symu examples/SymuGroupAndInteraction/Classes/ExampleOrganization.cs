#region Licence

// Description: SymuBiz - SymuGroupAndInteraction
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Classes.Organization;
using Symu.Common.Classes;
using Symu.OrgMod.Entities;
using Symu.Repository.Entities;

#endregion

namespace SymuGroupAndInteraction.Classes
{
    public class ExampleMainOrganization : MainOrganization
    {
        public ExampleMainOrganization() : base("symu")
        {
            Models.SetOn(1);
            Models.InteractionSphere.SphereUpdateOverTime = true;
            Models.Generator = RandomGenerator.RandomUniform;
        }

        public byte GroupsCount { get; set; } = 2;
        public byte WorkersCount { get; set; } = 5;
        public byte Knowledge { get; set; }
        public byte Activities { get; set; }
        public KnowledgeLevel KnowledgeLevel { get; set; } = KnowledgeLevel.FullKnowledge;

        public override MainOrganization Clone()
        {
            var clone = new ExampleMainOrganization();
            CopyTo(clone);
            clone.GroupsCount = GroupsCount;
            clone.WorkersCount = WorkersCount;
            clone.Knowledge = Knowledge;
            clone.Activities = Activities;
            clone.KnowledgeLevel = KnowledgeLevel;
            return clone;
        }

        /// <summary>
        ///     Add Organization knowledge
        /// </summary>
        public void AddKnowledge()
        {
            for (ushort i = 0; i < GroupsCount; i++)
            {
                // knowledge length of 10 is arbitrary in this example
                Symu.Repository.Entities.Knowledge.CreateInstance(MetaNetwork, Models, i.ToString(), 10);
                //Beliefs are created based on knowledge
            }
        }

        /// <summary>
        ///     Add Organization tasks
        /// </summary>
        public void AddTasks()
        {
            for (ushort i = 0; i < GroupsCount; i++)
            {
                TaskEntity.CreateInstance(MetaNetwork);
            }
        }
    }
}