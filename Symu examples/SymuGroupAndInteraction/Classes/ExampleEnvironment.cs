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
using Symu.Common.Classes;
using Symu.Common.Interfaces;
using Symu.Common.Math.ProbabilityDistributions;
using Symu.DNA.Edges;
using Symu.DNA.Entities;
using Symu.Environment;
using Symu.Repository.Edges;
using Symu.Repository.Entities;

#endregion

namespace SymuGroupAndInteraction.Classes
{
    public class ExampleEnvironment : SymuEnvironment
    {
        public ExampleOrganization ExampleOrganization => (ExampleOrganization)Organization;

        public ExampleEnvironment()
        {
            IterationResult.OrganizationFlexibility.On = true;
            SetDebug(false);
            SetTimeStepType(TimeStepType.Daily);
        }

        public override void SetAgents()
        {
            base.SetAgents();

            for (var i = 0; i < ExampleOrganization.GroupsCount; i++)
            {
                var group = GroupAgent.CreateInstance(this);
                for (var j = 0; j < ExampleOrganization.WorkersCount; j++)
                {
                    var actor = PersonAgent.CreateInstance(this,
                        Organization.Templates.Human);
                    group.AddPerson(actor);
                    SetAgentKnowledge(actor, ExampleOrganization.MetaNetwork.Knowledge.GetEntityIds().ToList(), i);
                    SetAgentTasks(actor, ExampleOrganization.MetaNetwork.Task.GetEntityIds().ToList(), i);
                }
            }
        }

        private void SetAgentKnowledge(CognitiveAgent actor, IReadOnlyList<IAgentId> knowledgeIds, int i)
        {
            var index = 0;
            switch (ExampleOrganization.Knowledge)
            {
                case 0:
                    // same Knowledge for all
                    index = 0;
                    break;
                case 1:
                    // Knowledge is by group
                    index = i;
                    break;
                case 2:
                    // Knowledge is randomly defined for agentId
                    index = DiscreteUniform.Sample(0, ExampleOrganization.GroupsCount - 1);
                    break;
            }
            actor.KnowledgeModel.AddKnowledge(knowledgeIds[index], ExampleOrganization.KnowledgeLevel,
                actor.Cognitive.InternalCharacteristics.MinimumRemainingKnowledge,
                actor.Cognitive.InternalCharacteristics.TimeToLive);
        }

        private void SetAgentTasks(CognitiveAgent actor, IReadOnlyList<IAgentId> taskIds, int i)
        {
            var index = 0;
            switch (ExampleOrganization.Activities)
            {
                case 0:
                    // same activity for all
                    index = 0;
                    break;
                case 1:
                    // Activity is by group
                    index = i;
                    break;
                case 2:
                    // Activity is randomly defined for agentId
                    index = DiscreteUniform.Sample(0, ExampleOrganization.GroupsCount - 1);
                    break;
            }
            actor.TaskModel.AddActorTask(taskIds[index]);
        }
    }
}