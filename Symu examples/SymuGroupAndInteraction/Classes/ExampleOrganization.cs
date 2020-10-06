#region Licence

// Description: SymuBiz - SymuGroupAndInteraction
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Symu.Classes.Agents;
using Symu.Classes.Organization;
using Symu.Common.Classes;
using Symu.Common.Interfaces;
using Symu.Common.Math.ProbabilityDistributions;
using Symu.DNA.Edges;
using Symu.DNA.Entities;
using Symu.Environment;
using Symu.Repository.Edges;
using Symu.Repository.Entities;
using Symu.Results;

#endregion

namespace SymuGroupAndInteraction.Classes
{
    public class ExampleOrganization : Organization
    {
        public byte GroupsCount { get; set; } = 2;
        public byte WorkersCount { get; set; } = 5;
        public byte Knowledge { get; set; }
        public byte Activities { get; set; } 
        public KnowledgeLevel KnowledgeLevel { get; set; } = KnowledgeLevel.FullKnowledge;

        public ExampleOrganization(): base("symu")
        {
            Models.SetOn(1);
            Models.InteractionSphere.SphereUpdateOverTime = true;
            Models.Generator = RandomGenerator.RandomUniform;
        }
        public override Organization Clone()
        {
            var clone = new ExampleOrganization();
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
                _ = new Knowledge(MetaNetwork, Models, i.ToString(), 10);
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
                _ = new TaskEntity(MetaNetwork);
            }
        }
    }
}