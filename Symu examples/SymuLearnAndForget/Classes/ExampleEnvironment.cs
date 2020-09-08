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
using Symu.Repository.Entity;

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
        public Database WikiEntity => (Database)WhitePages.MetaNetwork.Resource.List.First();

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
        public override void AddOrganizationKnowledge()
        {
            base.AddOrganizationKnowledge();
            Organization.AddKnowledge(Knowledge);
            WikiEntity.InitializeKnowledge(Knowledge, 0);
        }

        /// <summary>
        ///     Add Organization database
        /// </summary>
        public override void AddOrganizationDatabase()
        {
            base.AddOrganizationDatabase();

            var wikiEntity = Wiki.CreateInstance(Organization.AgentId.Id, Organization.Models, Organization.MetaNetwork);
            Organization.AddResource(wikiEntity);
        }

        public override void SetAgents()
        {
            base.SetAgents();

            LearnFromSourceAgent =
                LearnFromSourceAgent.CreateInstance(Organization.NextEntityId(), this, Organization.Templates.Human);
            LearnByDoingAgent =
                LearnByDoingAgent.CreateInstance(Organization.NextEntityId(), this, Organization.Templates.Human);
            LearnByAskingAgent =
                LearnByAskingAgent.CreateInstance(Organization.NextEntityId(), this, Organization.Templates.Human);
            DoesNotLearnAgent = LearnAgent.CreateInstance(Organization.NextEntityId(), this, Organization.Templates.Human);
            ExpertAgent = ExpertAgent.CreateInstance(Organization.NextEntityId(), this, Organization.Templates.Human);
            // Active link between expert and LearnByAskingAgent to be able to exchange information
            var interaction = new AgentAgent(LearnByAskingAgent.AgentId, ExpertAgent.AgentId);
            WhitePages.MetaNetwork.AgentAgent.AddInteraction(interaction);
        }
    }
}