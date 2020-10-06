#region Licence

// Description: SymuBiz - SymuMessageAndTask
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Organization;
using Symu.DNA.Edges;
using Symu.DNA.Entities;
using Symu.Environment;
using Symu.Repository.Entities;

#endregion

namespace SymuMessageAndTask.Classes
{
    public class ExampleEnvironment : SymuEnvironment
    {
        public ExampleOrganization ExampleOrganization => (ExampleOrganization)Organization;

        public ExampleEnvironment()
        {
            IterationResult.Off();
            IterationResult.Tasks.On = true;
            IterationResult.Messages.On = true;

            SetDebug(false);
        }

        public override void SetAgents()
        {
            base.SetAgents();
            var group = GroupAgent.CreateInstance(this);
            for (var i = 0; i < ExampleOrganization.WorkersCount; i++)
            {
                var actor = PersonAgent.CreateInstance(this, Organization.Templates.Human);
                actor.GroupId = group.AgentId;
                var email = EmailEntity.CreateInstance(Organization.MetaNetwork, Organization.Models);
                var actorResource = new ActorResource(actor.AgentId, email.EntityId, new ResourceUsage(0));
                Organization.MetaNetwork.ActorResource.Add(actorResource);
                var actorGroup = new ActorOrganization(actor.AgentId, group.AgentId);
                Organization.MetaNetwork.ActorOrganization.Add(actorGroup);
            }
        }
    }
}