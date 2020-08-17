#region Licence

// Description: SymuBiz - SymuLearnAndForget
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;
using Symu.Classes.Organization;
using Symu.Common;
using Symu.Environment;
using Symu.Repository.Networks.Databases;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace SymuLearnAndForget.Classes
{
    public class ExampleEnvironment : SymuEnvironment
    {
        public LearnFromSourceAgent LearnFromSourceAgent { get; private set; }
        public LearnByDoingAgent LearnByDoingAgent { get; private set; }
        public LearnByAskingAgent LearnByAskingAgent { get; private set; }
        public LearnAgent DoesNotLearnAgent { get; private set; }
        public ExpertAgent ExpertAgent { get; private set; }
        public Knowledge Knowledge { get; set; }
        public KnowledgeLevel KnowledgeLevel { get; set; }
        public Database Wiki => WhitePages.MetaNetwork.NetworkDatabases.Repository.List.First();

        public override void SetOrganization(OrganizationEntity organization)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }

            base.SetOrganization(organization);

            IterationResult.KnowledgeAndBeliefResults.On = true;
            Organization.Communication.Email.CostToSendLevel = GenericLevel.None;
            Organization.Communication.Email.CostToReceiveLevel = GenericLevel.None;

            SetDebug(false);
        }

        /// <summary>
        ///     Add Organization knowledge
        /// </summary>
        public override void AddOrganizationKnowledges()
        {
            base.AddOrganizationKnowledges();
            Organization.AddKnowledge(Knowledge);
            Wiki.InitializeKnowledge(Knowledge, 0);
        }

        /// <summary>
        ///     Add Organization database
        /// </summary>
        public override void AddOrganizationDatabase()
        {
            base.AddOrganizationDatabase();

            var wikiEntity = new DataBaseEntity(Organization.Id, Organization.Communication.Email);
            Organization.AddDatabase(wikiEntity);
        }

        public override void SetAgents()
        {
            base.SetAgents();

            LearnFromSourceAgent =
                new LearnFromSourceAgent(Organization.NextEntityIndex(), this, Organization.Templates.Human);
            LearnByDoingAgent =
                new LearnByDoingAgent(Organization.NextEntityIndex(), this, Organization.Templates.Human);
            LearnByAskingAgent =
                new LearnByAskingAgent(Organization.NextEntityIndex(), this, Organization.Templates.Human);
            DoesNotLearnAgent = new LearnAgent(Organization.NextEntityIndex(), this, Organization.Templates.Human);
            ExpertAgent = new ExpertAgent(Organization.NextEntityIndex(), this, Organization.Templates.Human);
            // Active link between expert and LearnByAskingAgent to be able to exchange information
            WhitePages.MetaNetwork.NetworkLinks.AddLink(LearnByAskingAgent.Id, ExpertAgent.Id);
        }
    }
}