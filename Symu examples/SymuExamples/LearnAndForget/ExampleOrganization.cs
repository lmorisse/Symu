#region Licence

// Description: SymuBiz - SymuLearnAndForget
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Linq;
using Symu.Classes.Organization;
using Symu.Common;
using Symu.Common.Classes;
using Symu.Repository.Entities;

#endregion

namespace SymuExamples.LearnAndForget
{
    public class ExampleMainOrganization : MainOrganization
    {
        public ExampleMainOrganization() : base("symu")
        {
            Models.SetOn(1);
            Murphies.IncompleteKnowledge.On = true;
            Communication.Email.CostToSendLevel = GenericLevel.None;
            Communication.Email.CostToReceiveLevel = GenericLevel.None;
        }

        public Knowledge Knowledge
        {
            get
            {
                if (MetaNetwork.Knowledge.Any())
                {
                    return (Knowledge) MetaNetwork.Knowledge.List.First();
                }

                return null;
            }
        }

        public KnowledgeLevel KnowledgeLevel { get; set; }
        public WikiEntity WikiEntity { get; private set; }

        public override MainOrganization Clone()
        {
            var clone = new ExampleMainOrganization();
            CopyTo(clone);
            clone.KnowledgeLevel = KnowledgeLevel;
            clone.WikiEntity = WikiEntity;
            return clone;
        }


        /// <summary>
        ///     Add Organization wiki
        /// </summary>
        public void AddWiki()
        {
            //Wiki
            WikiEntity = WikiEntity.CreateInstance(MetaNetwork, Models);
            WikiEntity.InitializeKnowledge(Knowledge.EntityId, 0);
        }
    }
}