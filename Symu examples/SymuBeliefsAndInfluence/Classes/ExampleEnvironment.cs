#region Licence

// Description: SymuBiz - SymuBeliefsAndInfluence
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Classes.Organization;
using Symu.Common;
using Symu.Common.Classes;
using Symu.DNA.Edges;
using Symu.DNA.Entities;
using Symu.DNA.GraphNetworks;
using Symu.Environment;
using Symu.Repository.Entities;

#endregion

namespace SymuBeliefsAndInfluence.Classes
{
    public class ExampleEnvironment : SymuEnvironment
    {
        public ExampleOrganization ExampleOrganization => (ExampleOrganization)Organization;

        public ExampleEnvironment()
        {
            IterationResult.Tasks.On = true;
            IterationResult.KnowledgeAndBeliefResults.On = true;
            IterationResult.OrganizationFlexibility.On = true;

            SetDebug(false);

            SetTimeStepType(TimeStepType.Daily);
        }

        public override void SetAgents()
        {
            base.SetAgents();
            
            for (var j = 0; j < ExampleOrganization.InfluencersCount; j++)
            {
                var actor = InfluencerAgent.CreateInstance(this, ExampleOrganization.InfluencerTemplate);
                ExampleOrganization.Influencers.Add(actor);
            }

            for (var j = 0; j < ExampleOrganization.WorkersCount; j++)
            {
                _ = PersonAgent.CreateInstance(this, ExampleOrganization.WorkerTemplate);
            }

            var actorIds =
                Organization.MetaNetwork.Actor.GetEntityIds().ToList();
            // Set the interactions between the actors
            // Those interactions could be managed via an organization agent.
            for (var i = 0; i < actorIds.Count-1; i++)
            {
                for (var j = i+1; j < actorIds.Count; j++)
                {
                    var interaction = new ActorActor(actorIds[i], actorIds[j]);
                    Organization.MetaNetwork.ActorActor.Add(interaction);
                }
            }
        }
    }
}