#region Licence

// Description: SymuBiz - SymuGroupAndInteraction
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using System.Linq;
using Symu.Classes.Agents;
using Symu.Common.Classes;
using Symu.Common.Interfaces;
using Symu.Common.Math.ProbabilityDistributions;
using Symu.Environment;

#endregion

namespace SymuExamples.GroupAndInteraction
{
    public class ExampleEnvironment : SymuEnvironment
    {
        public ExampleEnvironment()
        {
            IterationResult.OrganizationFlexibility.On = true;
            SetDebug(false);
            SetTimeStepType(TimeStepType.Daily);
        }

        public ExampleMainOrganization ExampleMainOrganization => (ExampleMainOrganization)MainOrganization;

        public override void SetAgents()
        {
            base.SetAgents();

            for (var i = 0; i < ExampleMainOrganization.GroupsCount; i++)
            {
                var group = GroupAgent.CreateInstance(this);
                for (var j = 0; j < ExampleMainOrganization.WorkersCount; j++)
                {
                    var actor = PersonAgent.CreateInstance(this,
                        MainOrganization.Templates.Human);
                    group.AddPerson(actor);
                    SetAgentKnowledge(actor, ExampleMainOrganization.MetaNetwork.Knowledge.GetEntityIds().ToList(), i);
                    SetAgentTasks(actor, ExampleMainOrganization.MetaNetwork.Task.GetEntityIds().ToList(), i);
                }
            }
        }

        private void SetAgentKnowledge(CognitiveAgent actor, IReadOnlyList<IAgentId> knowledgeIds, int i)
        {
            var index = 0;
            switch (ExampleMainOrganization.Knowledge)
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
                    index = DiscreteUniform.Sample(0, ExampleMainOrganization.GroupsCount - 1);
                    break;
            }

            actor.KnowledgeModel.AddKnowledge(knowledgeIds[index], ExampleMainOrganization.KnowledgeLevel,
                actor.Cognitive.InternalCharacteristics.MinimumRemainingKnowledge,
                actor.Cognitive.InternalCharacteristics.TimeToLive);
        }

        private void SetAgentTasks(CognitiveAgent actor, IReadOnlyList<IAgentId> taskIds, int i)
        {
            var index = 0;
            switch (ExampleMainOrganization.Activities)
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
                    index = DiscreteUniform.Sample(0, ExampleMainOrganization.GroupsCount - 1);
                    break;
            }

            actor.TaskModel.AddActorTask(taskIds[index]);
        }
    }
}