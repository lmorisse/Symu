#region Licence

// Description: SymuBiz - SymuBeliefsAndInfluence
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Linq;
using Symu.Common.Classes;
using Symu.Environment;
using Symu.OrgMod.Edges;

#endregion

namespace SymuExamples.BeliefsAndInfluence
{
    public class ExampleEnvironment : SymuEnvironment
    {
        public ExampleEnvironment()
        {
            IterationResult.Tasks.On = true;
            IterationResult.KnowledgeAndBeliefResults.On = true;
            IterationResult.OrganizationFlexibility.On = true;

            SetDebug(false);

            SetTimeStepType(TimeStepType.Daily);
        }

        public ExampleMainOrganization ExampleMainOrganization => (ExampleMainOrganization) MainOrganization;

        public override void SetAgents()
        {
            base.SetAgents();

            for (var j = 0; j < ExampleMainOrganization.InfluencersCount; j++)
            {
                var actor = InfluencerAgent.CreateInstance(this, ExampleMainOrganization.InfluencerTemplate);
                ExampleMainOrganization.Influencers.Add(actor);
            }

            for (var j = 0; j < ExampleMainOrganization.WorkersCount; j++)
            {
                _ = PersonAgent.CreateInstance(this, ExampleMainOrganization.WorkerTemplate);
            }

            var actorIds =
                MainOrganization.MetaNetwork.Actor.GetEntityIds().ToList();
            // Set the interactions between the actors
            // Those interactions could be managed via an organization agent.
            for (var i = 0; i < actorIds.Count - 1; i++)
            {
                for (var j = i + 1; j < actorIds.Count; j++)
                {
                    ActorActor.CreateInstance(MainOrganization.MetaNetwork.ActorActor, actorIds[i], actorIds[j]);
                }
            }
        }
    }
}