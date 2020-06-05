#region Licence

// Description: Symu - SymuLearnAndForget
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
        public Database Wiki => WhitePages.Network.NetworkDatabases.Repository.List.First();

        public override void SetOrganization(OrganizationEntity organization)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }

            base.SetOrganization(organization);

            var wiki = new DataBaseEntity(organization.Id, organization.Templates.Email);
            organization.AddDatabase(wiki);
            IterationResult.OrganizationKnowledgeAndBelief.On = true;

            SetDebug(false);
        }

        public override void SetModelForAgents()
        {
            base.SetModelForAgents();
            WhitePages.Network.NetworkCommunications.Email.CostToSendLevel = GenericLevel.None;
            WhitePages.Network.NetworkCommunications.Email.CostToReceiveLevel = GenericLevel.None;
            WhitePages.Network.AddKnowledge(Knowledge);
            Wiki.InitializeKnowledge(Knowledge, 0);

            LearnFromSourceAgent = new LearnFromSourceAgent(Organization.NextEntityIndex(), this);
            LearnFromSourceAgent.KnowledgeModel.AddKnowledge(Knowledge.Id, KnowledgeLevel,
                Organization.Templates.Human.Cognitive.InternalCharacteristics);
            LearnByDoingAgent = new LearnByDoingAgent(Organization.NextEntityIndex(), this);
            LearnByDoingAgent.KnowledgeModel.AddKnowledge(Knowledge.Id, KnowledgeLevel,
                Organization.Templates.Human.Cognitive.InternalCharacteristics);
            LearnByAskingAgent = new LearnByAskingAgent(Organization.NextEntityIndex(), this);
            LearnByAskingAgent.KnowledgeModel.AddKnowledge(Knowledge.Id, KnowledgeLevel,
                Organization.Templates.Human.Cognitive.InternalCharacteristics);
            DoesNotLearnAgent = new LearnAgent(Organization.NextEntityIndex(), this);
            DoesNotLearnAgent.KnowledgeModel.AddKnowledge(Knowledge.Id, KnowledgeLevel,
                Organization.Templates.Human.Cognitive.InternalCharacteristics);
            ExpertAgent = new ExpertAgent(Organization.NextEntityIndex(), this);
            ExpertAgent.Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge = true;
            ExpertAgent.KnowledgeModel.AddKnowledge(Knowledge.Id, KnowledgeLevel.Expert,
                Organization.Templates.Human.Cognitive.InternalCharacteristics);
            // CopyTo active link between expert and LearnByAskingAgent to be able to exchange information
            WhitePages.Network.NetworkLinks.AddLink(LearnByAskingAgent.Id, ExpertAgent.Id);
        }
    }
}