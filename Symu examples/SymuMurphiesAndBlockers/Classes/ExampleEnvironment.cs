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
        public ExampleEnvironment()
        {
            IterationResult.Blockers.On = true;
            IterationResult.Tasks.On = true;

            SetDebug(false);
            SetTimeStepType(TimeStepType.Daily);
        }

        public ExampleMainOrganization ExampleMainOrganization => (ExampleMainOrganization) MainOrganization;
        public MurphyTask Model => MainOrganization.Murphies.IncompleteKnowledge;

        public InternetAccessAgent Internet { get; private set; }

        public override void SetAgents()
        {
            base.SetAgents();

            var group = GroupAgent.CreateInstance(this);
            Internet = InternetAccessAgent.CreateInstance(this, ExampleMainOrganization.Templates.Internet);
            for (var j = 0; j < ExampleMainOrganization.WorkersCount; j++)
            {
                var actor = PersonAgent.CreateInstance(this, ExampleMainOrganization.Templates.Human);
                actor.GroupId = group.AgentId;
                var email = EmailEntity.CreateInstance(ExampleMainOrganization.MetaNetwork, MainOrganization.Models);
                _ = new ActorResource(ExampleMainOrganization.MetaNetwork.ActorResource, actor.AgentId, email.EntityId, new ResourceUsage(0));
                _ = new ActorOrganization(ExampleMainOrganization.MetaNetwork.ActorOrganization, actor.AgentId, group.AgentId);
            }
        }
    }
}