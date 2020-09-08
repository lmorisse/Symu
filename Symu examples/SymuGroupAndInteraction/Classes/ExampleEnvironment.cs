#region Licence

// Description: SymuBiz - SymuGroupAndInteraction
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using Symu.Classes.Agents;
using Symu.Classes.Organization;
using Symu.Common;
using Symu.Common.Classes;
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.Common.Math.ProbabilityDistributions;
using Symu.DNA.Networks.OneModeNetworks;
using Symu.Environment;
using Symu.Repository.Entity;

#endregion

namespace SymuGroupAndInteraction.Classes
{
    public class ExampleEnvironment : SymuEnvironment
    {
        public byte GroupsCount { get; set; } = 2;
        public byte WorkersCount { get; set; } = 5;
        public byte Knowledge { get; set; } = 0;
        public byte Activities { get; set; } = 0;
        public KnowledgeLevel KnowledgeLevel { get; set; } = KnowledgeLevel.FullKnowledge;

        public override void SetOrganization(OrganizationEntity organization)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }

            base.SetOrganization(organization);

            Organization.Models.InteractionSphere.SphereUpdateOverTime = true;
            Organization.Models.InteractionSphere.On = true;
            Organization.Models.Generator = RandomGenerator.RandomUniform;
            IterationResult.OrganizationFlexibility.On = true;
            SetDebug(false);
        }

        /// <summary>
        ///     Add Organization knowledge
        /// </summary>
        public override void AddOrganizationKnowledge()
        {
            base.AddOrganizationKnowledge();
            for (ushort i = 0; i < GroupsCount; i++)
            {
                // knowledge length of 10 is arbitrary in this example
                var knowledge = new Knowledge(Organization.MetaNetwork.Knowledge.NextIdentity(), i.ToString(), 10);
                Organization.AddKnowledge(knowledge);
                //Beliefs are created based on knowledge
            }
        }

        /// <summary>
        ///     Add Organization tasks
        /// </summary>
        public override void AddOrganizationTasks()
        {
            base.AddOrganizationTasks();
            for (ushort i = 0; i < GroupsCount; i++)
            {
                Organization.AddTask(new Task(Organization.MetaNetwork.Task.NextIdentity(), i.ToString(), WhitePages.MetaNetwork.TaskKnowledge));
            }
        }

        public override void SetAgents()
        {
            base.SetAgents();

            for (var i = 0; i < GroupsCount; i++)
            {
                var group = GroupAgent.CreateInstance(Organization.NextEntityId(), this);
                for (var j = 0; j < WorkersCount; j++)
                {
                    var actor = PersonAgent.CreateInstance(Organization.NextEntityId(), this,
                        Organization.Templates.Human);
                    actor.GroupId = group.AgentId;
                    var agentGroup = new AgentOrganization(actor.AgentId, 100);
                    WhitePages.MetaNetwork.AddAgentToGroup(agentGroup, group.AgentId);
                    SetAgentKnowledge(actor, Organization.Knowledge, i);
                    SetAgentTasks(actor, Organization.Tasks, i);
                }
            }
        }

        private void SetAgentKnowledge(CognitiveAgent actor, IReadOnlyList<IKnowledge> knowledges, int i)
        {
            var index = 0;
            switch (Knowledge)
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
                    index = DiscreteUniform.Sample(0, GroupsCount - 1);
                    break;
            }
            actor.KnowledgeModel.AddKnowledge(knowledges[index].Id, KnowledgeLevel,
                actor.Cognitive.InternalCharacteristics.MinimumRemainingKnowledge,
                actor.Cognitive.InternalCharacteristics.TimeToLive);
        }

        private void SetAgentTasks(CognitiveAgent actor, IReadOnlyList<ITask> activities, int i)
        {
            var index = 0;
            switch (Activities)
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
                    index = DiscreteUniform.Sample(0, GroupsCount - 1);
                    break;
            }
            actor.TaskModel.AddAgentTask(activities[index]);
            //WhitePages.MetaNetwork.AgentTask.Add(agentId, new AgentTask(agentId, activities[0]));
        }
    }
}