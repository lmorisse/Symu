#region Licence

// Description: SymuBiz - SymuMurphiesAndBlockers
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Classes.Task;
using Symu.Common.Classes;
using Symu.Environment;
using Symu.OrgMod.Edges;
using Symu.OrgMod.Entities;
using Symu.Repository.Entities;

#endregion

namespace SymuMurphiesAndBlockers.Classes
{
    public class ExampleEnvironment : SymuEnvironment
    {
        public ExampleOrganization ExampleOrganization => (ExampleOrganization)Organization;
        public MurphyTask Model => Organization.Murphies.IncompleteKnowledge;

        public InternetAccessAgent Internet { get; private set; }

        public ExampleEnvironment()
        {
            IterationResult.Blockers.On = true;
            IterationResult.Tasks.On = true;

            SetDebug(false);
            SetTimeStepType(TimeStepType.Daily);
        }

        public override void SetAgents()
        {
            base.SetAgents();

            var group = GroupAgent.CreateInstance(this);
            Internet = InternetAccessAgent.CreateInstance(this, ExampleOrganization.Templates.Internet);
            for (var j = 0; j < ExampleOrganization.WorkersCount; j++)
            {
                var actor = PersonAgent.CreateInstance(this, ExampleOrganization.Templates.Human);
                actor.GroupId = group.AgentId;
                var email = EmailEntity.CreateInstance(ExampleOrganization.MetaNetwork, Organization.Models);
                var actorResource = new ActorResource(actor.AgentId, email.EntityId, new ResourceUsage(0));
                ExampleOrganization.MetaNetwork.ActorResource.Add(actorResource);
                var actorGroup = new ActorOrganization(actor.AgentId, group.AgentId);
                ExampleOrganization.MetaNetwork.ActorOrganization.Add(actorGroup);
            }
        }
    }
}