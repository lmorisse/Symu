#region Licence

// Description: Symu - SymuScenariosAndEvents
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.Templates.Communication;
using Symu.Classes.Organization;
using Symu.Classes.Task;
using Symu.Common;
using Symu.Environment;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace SymuScenariosAndEvents.Classes
{
    public class ExampleEnvironment : SymuEnvironment
    {
        private AgentId _groupId;
        public byte WorkersCount { get; set; } = 5;
        private byte KnowledgeCount { get; } = 2;

        public List<Knowledge> Knowledges { get; private set; }
        public MurphyTask Model => Organization.Murphies.IncompleteKnowledge;

        public override void SetOrganization(OrganizationEntity organization)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }

            base.SetOrganization(organization);

            #region Template

            organization.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasKnowledge = true;
            organization.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge = true;
            organization.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasBelief = true;
            organization.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasInitialBelief = true;
            organization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask = true;
            organization.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds = false;
            organization.Templates.Human.Cognitive.TasksAndPerformance.TasksLimit.LimitSimultaneousTasks = true;
            organization.Templates.Human.Cognitive.TasksAndPerformance.TasksLimit.MaximumSimultaneousTasks = 1;
            organization.Templates.Human.Cognitive.InteractionPatterns.IsolationCyclicity = Cyclicity.None;
            organization.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            organization.Templates.Human.Cognitive.InteractionPatterns.AllowNewInteractions = true;

            #endregion

            #region Results

            IterationResult.Blockers.On = true;
            IterationResult.Tasks.On = true;

            #endregion

            SetDebug(false);
        }

        public override void SetModelForAgents()
        {
            base.SetModelForAgents();

            // KnowledgeCount are added for tasks initialization
            // Adn Beliefs are created based on knowledge
            Knowledges = new List<Knowledge>();
            for (var i = 0; i < KnowledgeCount; i++)
            {
                // knowledge length of 10 is arbitrary in this example
                var knowledge = new Knowledge((ushort) i, i.ToString(), 10);
                WhitePages.Network.AddKnowledge(knowledge);
                Knowledges.Add(knowledge);
            }

            var group = new GroupAgent(Organization.NextEntityIndex(), this);
            _groupId = group.Id;
            for (var j = 0; j < WorkersCount; j++)
            {
                AddPersonAgent();
            }
        }

        private void SetKnowledge(Agent actor, IReadOnlyList<Knowledge> knowledges)
        {
            for (var i = 0; i < KnowledgeCount; i++)
            {
                actor.KnowledgeModel.AddKnowledge(knowledges[i].Id,
                    KnowledgeLevel.Intermediate,
                    Organization.Templates.Human.Cognitive.InternalCharacteristics);
                actor.BeliefsModel.AddBelief(knowledges[i].Id);
            }
        }

        private PersonAgent AddPersonAgent()
        {
            var actor = new PersonAgent(Organization.NextEntityIndex(), this)
            {
                GroupId = _groupId
            };
            CommunicationTemplate communication = new EmailTemplate();
            WhitePages.Network.AddEmail(actor.Id, communication);
            WhitePages.Network.AddMemberToGroup(actor.Id, 100, _groupId);
            SetKnowledge(actor, Knowledges);
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
            WhitePages.Network.AddKnowledge(knowledge);
            Knowledges.Add(knowledge);

            foreach (var person in WhitePages.FilteredAgentsByClassKey(PersonAgent.ClassKey))
            {
                person.KnowledgeModel.AddKnowledge(knowledge.Id, KnowledgeLevel.BasicKnowledge, 0.15F, -1);
                person.KnowledgeModel.InitializeKnowledge(knowledge.Id, Schedule.Step);
            }
        }

        #endregion
    }
}