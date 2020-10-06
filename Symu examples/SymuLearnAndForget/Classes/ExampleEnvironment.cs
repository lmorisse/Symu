#region Licence

// Description: SymuBiz - SymuLearnAndForget
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;
using Symu.Classes.Organization;
using Symu.Common;
using Symu.Common.Classes;
using Symu.DNA.Edges;
using Symu.Environment;
using Symu.Repository.Edges;
using Symu.Repository.Entities;

#endregion

namespace SymuLearnAndForget.Classes
{
    public class ExampleEnvironment : SymuEnvironment
    {
        public ExampleOrganization ExampleOrganization => (ExampleOrganization)Organization;
        public LearnFromSourceAgent LearnFromSourceAgent { get; private set; }
        public LearnByDoingAgent LearnByDoingAgent { get; private set; }
        public LearnByAskingAgent LearnByAskingAgent { get; private set; }
        public LearnAgent DoesNotLearnAgent { get; private set; }
        public ExpertAgent ExpertAgent { get; private set; }


        public ExampleEnvironment()
        {
            IterationResult.KnowledgeAndBeliefResults.On = true;

            SetDebug(false);
            SetTimeStepType(TimeStepType.Daily);
        }

        public override void SetAgents()
        {
            base.SetAgents();

            LearnFromSourceAgent =
                LearnFromSourceAgent.CreateInstance(this, Organization.Templates.Human);
            LearnByDoingAgent =
                LearnByDoingAgent.CreateInstance(this, Organization.Templates.Human);
            LearnByAskingAgent =
                LearnByAskingAgent.CreateInstance(this, Organization.Templates.Human);
            DoesNotLearnAgent = LearnAgent.CreateInstance(this, Organization.Templates.Human);
            ExpertAgent = ExpertAgent.CreateInstance(this, Organization.Templates.Human);
            // Active link between expert and LearnByAskingAgent to be able to exchange information
            var interaction = new ActorActor(LearnByAskingAgent.AgentId, ExpertAgent.AgentId);
            Organization.MetaNetwork.ActorActor.Add(interaction);
        }
    }
}