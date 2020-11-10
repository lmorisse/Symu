#region Licence

// Description: SymuBiz - SymuScenariosAndEvents
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Task;
using Symu.Common.Classes;
using Symu.Common.Interfaces;
using Symu.Environment;
using Symu.OrgMod.Edges;
using Symu.OrgMod.Entities;
using Symu.Repository.Entities;

#endregion

namespace SymuExamples.ScenariosAndEvents
{
    public class ExampleEnvironment : SymuEnvironment
    {
        private IAgentId _groupId;

        public ExampleEnvironment()
        {
            IterationResult.Blockers.On = true;
            IterationResult.Tasks.On = true;

            SetDebug(false);
            SetTimeStepType(TimeStepType.Daily);
        }

        public MurphyTask Model => MainOrganization.Murphies.IncompleteKnowledge;
        public ExampleMainOrganization ExampleMainOrganization => (ExampleMainOrganization) MainOrganization;

        public override void SetAgents()
        {
            base.SetAgents();

            var group = GroupAgent.CreateInstance(this);
            _groupId = group.AgentId;
            for (var j = 0; j < ExampleMainOrganization.WorkersCount; j++)
            {
                AddPersonAgent();
            }
        }

        private PersonAgent AddPersonAgent()
        {
            var actor = PersonAgent.CreateInstance(this, ExampleMainOrganization.Templates.Human);
            actor.GroupId = _groupId;
            var email = EmailEntity.CreateInstance(ExampleMainOrganization.ArtifactNetwork, MainOrganization.Models);
            ActorResource.CreateInstance(ExampleMainOrganization.ArtifactNetwork.ActorResource, actor.AgentId, email.EntityId, new ResourceUsage(0));
            ActorOrganization.CreateInstance(ExampleMainOrganization.ArtifactNetwork.ActorOrganization, actor.AgentId, _groupId);
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
            var knowledge = new Knowledge(ExampleMainOrganization.ArtifactNetwork, ExampleMainOrganization.Models,
                ExampleMainOrganization.KnowledgeCount.ToString(), 10);

            foreach (var person in AgentNetwork.FilteredCognitiveAgentsByClassId(PersonAgent.ClassId))
            {
                person.KnowledgeModel.AddKnowledge(knowledge.EntityId, KnowledgeLevel.BasicKnowledge, 0.15F, -1);
                person.KnowledgeModel.InitializeKnowledge(knowledge.EntityId, Schedule.Step);
            }
        }

        #endregion
    }
}