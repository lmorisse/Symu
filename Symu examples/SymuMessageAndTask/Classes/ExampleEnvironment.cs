#region Licence

// Description: SymuBiz - SymuMessageAndTask
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Environment;
using Symu.OrgMod.Edges;
using Symu.OrgMod.Entities;
using Symu.Repository.Entities;

#endregion

namespace SymuMessageAndTask.Classes
{
    public class ExampleEnvironment : SymuEnvironment
    {
        public ExampleEnvironment()
        {
            IterationResult.Off();
            IterationResult.Tasks.On = true;
            IterationResult.Messages.On = true;

            SetDebug(false);
        }

        public ExampleMainOrganization ExampleMainOrganization => (ExampleMainOrganization) MainOrganization;

        public override void SetAgents()
        {
            base.SetAgents();
            var group = GroupAgent.CreateInstance(this);
            for (var i = 0; i < ExampleMainOrganization.WorkersCount; i++)
            {
                var actor = PersonAgent.CreateInstance(this, MainOrganization.Templates.Human);
                actor.GroupId = group.AgentId;
                var email = EmailEntity.CreateInstance(MainOrganization.MetaNetwork, MainOrganization.Models);
                ActorResource.CreateInstance(MainOrganization.MetaNetwork.ActorResource, actor.AgentId, email.EntityId, new ResourceUsage(0));
                ActorOrganization.CreateInstance(MainOrganization.MetaNetwork.ActorOrganization, actor.AgentId, group.AgentId);
            }
        }
    }
}