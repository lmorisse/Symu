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
using Symu.Environment;
using Symu.Repository.Networks.Knowledges;
using Symu.Tools.Math.ProbabilityDistributions;

#endregion

namespace SymuGroupAndInteraction.Classes
{
    public class ExampleEnvironment : SymuEnvironment
    {
        private readonly List<string> _activities = new List<string>();
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
        public override void AddOrganizationKnowledges()
        {
            base.AddOrganizationKnowledges(); 
            for (var i = 0; i < GroupsCount; i++)
            {
                // knowledge length of 10 is arbitrary in this example
                var knowledge = new Knowledge((ushort)i, i.ToString(), 10);
                Organization.AddKnowledge(knowledge);
                _activities.Add(i.ToString());
                //Beliefs are created based on knowledge
            }
        }

        public override void SetAgents()
        {
            base.SetAgents();

            for (var i = 0; i < GroupsCount; i++)
            {
                var group = new GroupAgent(Organization.NextEntityIndex(), this);

                for (var j = 0; j < WorkersCount; j++)
                {
                    var actor = new PersonAgent(Organization.NextEntityIndex(), this, Organization.Templates.Human)
                    {
                        GroupId = group.Id
                    };
                    WhitePages.Network.AddMemberToGroup(actor.Id, 100, group.Id);
                    //Beliefs are added with knowledge
                    SetKnowledge(actor, Organization.Knowledges, i);
                    SetActivity(actor.Id, _activities, i, group.Id);
                }
            }
        }

        private void SetKnowledge(Agent actor, IReadOnlyList<Knowledge> knowledges, int i)
        {
            switch (Knowledge)
            {
                case 0:
                    // same Knowledge for all
                    WhitePages.Network.NetworkKnowledges.Add(actor.Id, knowledges[0].Id, KnowledgeLevel,
                        actor.Cognitive.InternalCharacteristics);
                    break;
                case 1:
                    // Knowledge is by group
                    WhitePages.Network.NetworkKnowledges.Add(actor.Id, knowledges[i].Id, KnowledgeLevel,
                        actor.Cognitive.InternalCharacteristics);
                    break;
                case 2:
                    // Knowledge is randomly defined for agentId
                    var index = DiscreteUniform.Sample(0, GroupsCount - 1);
                    WhitePages.Network.NetworkKnowledges.Add(actor.Id, knowledges[index].Id, KnowledgeLevel,
                        actor.Cognitive.InternalCharacteristics);
                    break;
            }
        }

        private void SetActivity(AgentId agentId, IReadOnlyList<string> activities, int i, AgentId groupId)
        {
            switch (Activities)
            {
                case 0:
                    // same activity for all
                    WhitePages.Network.NetworkActivities.AddActivity(agentId, activities[0], groupId);
                    break;
                case 1:
                    // Activity is by group
                    WhitePages.Network.NetworkActivities.AddActivity(agentId, activities[i], groupId);
                    break;
                case 2:
                    // Activity is randomly defined for agentId
                    var index = DiscreteUniform.Sample(0, GroupsCount - 1);
                    WhitePages.Network.NetworkActivities.AddActivity(agentId, activities[index], groupId);
                    break;
            }
        }
    }
}