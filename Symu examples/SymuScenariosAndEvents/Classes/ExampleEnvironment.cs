#region Licence

// Description: SymuBiz - SymuScenariosAndEvents
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents;
using Symu.Classes.Organization;
using Symu.Classes.Task;
using Symu.Environment;
using Symu.Messaging.Templates;
using Symu.Repository.Networks.Databases;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace SymuScenariosAndEvents.Classes
{
    public class ExampleEnvironment : SymuEnvironment
    {
        private AgentId _groupId;
        public byte WorkersCount { get; set; } = 5;
        private byte KnowledgeCount { get; } = 2;

        public MurphyTask Model => Organization.Murphies.IncompleteKnowledge;

        public override void SetOrganization(OrganizationEntity organization)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }

            base.SetOrganization(organization);

            IterationResult.Blockers.On = true;
            IterationResult.Tasks.On = true;

            SetDebug(false);
        }

        /// <summary>
        ///     Add Organization knowledge
        /// </summary>
        public override void AddOrganizationKnowledges()
        {
            base.AddOrganizationKnowledges();
            // KnowledgeCount are added for tasks initialization
            // Adn Beliefs are created based on knowledge
            for (var i = 0; i < KnowledgeCount; i++)
            {
                // knowledge length of 10 is arbitrary in this example
                var knowledge = new Knowledge((ushort) i, i.ToString(), 10);
                Organization.AddKnowledge(knowledge);
            }
        }

        public override void SetAgents()
        {
            base.SetAgents();

            var group = new GroupAgent(Organization.NextEntityIndex(), this);
            _groupId = group.Id;
            for (var j = 0; j < WorkersCount; j++)
            {
                AddPersonAgent();
            }
        }

        private PersonAgent AddPersonAgent()
        {
            var actor = new PersonAgent(Organization.NextEntityIndex(), this, Organization.Templates.Human)
            {
                GroupId = _groupId
            };
            CommunicationTemplate communication = new EmailTemplate();
            var entity = new DataBaseEntity(actor.Id, communication);
            var email = new Database(entity, Organization.Models, WhitePages.MetaNetwork.NetworkKnowledges);
            WhitePages.MetaNetwork.AddDatabase(actor.Id, email);
            WhitePages.MetaNetwork.AddMemberToGroup(actor.Id, 100, _groupId, false);
            return actor;
        }

        #region events

        public void PersonEvent(object sender, EventArgs e)
        {
            var actor = AddPersonAgent();
            actor.Start();
        }

        public void KnowledgeEvent(object sender, EventArgs e)
        {
            // knowledge length of 10 is arbitrary in this example
            var knowledge = new Knowledge(KnowledgeCount, KnowledgeCount.ToString(), 10);
            WhitePages.MetaNetwork.AddKnowledge(knowledge);

            foreach (var person in WhitePages.FilteredCognitiveAgentsByClassKey(PersonAgent.ClassKey))
            {
                person.KnowledgeModel.AddKnowledge(knowledge.Id, KnowledgeLevel.BasicKnowledge, 0.15F, -1);
                person.KnowledgeModel.InitializeKnowledge(knowledge.Id, Schedule.Step);
            }
        }

        #endregion
    }
}