#region Licence

// Description: SymuBiz - SymuLearnAndForget
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Common.Classes;
using Symu.Environment;
using Symu.OrgMod.Edges;

#endregion

namespace SymuLearnAndForget.Classes
{
    public class ExampleEnvironment : SymuEnvironment
    {
        public ExampleEnvironment()
        {
            IterationResult.KnowledgeAndBeliefResults.On = true;

            SetDebug(false);
            SetTimeStepType(TimeStepType.Daily);
        }

        public ExampleMainOrganization ExampleMainOrganization => (ExampleMainOrganization) MainOrganization;
        public LearnFromSourceAgent LearnFromSourceAgent { get; private set; }
        public LearnByDoingAgent LearnByDoingAgent { get; private set; }
        public LearnByAskingAgent LearnByAskingAgent { get; private set; }
        public LearnAgent DoesNotLearnAgent { get; private set; }
        public ExpertAgent ExpertAgent { get; private set; }

        public override void SetAgents()
        {
            base.SetAgents();

            LearnFromSourceAgent =
                LearnFromSourceAgent.CreateInstance(this, MainOrganization.Templates.Human);
            LearnByDoingAgent =
                LearnByDoingAgent.CreateInstance(this, MainOrganization.Templates.Human);
            LearnByAskingAgent =
                LearnByAskingAgent.CreateInstance(this, MainOrganization.Templates.Human);
            DoesNotLearnAgent = LearnAgent.CreateInstance(this, MainOrganization.Templates.Human);
            ExpertAgent = ExpertAgent.CreateInstance(this, MainOrganization.Templates.Human);
            // Active link between expert and LearnByAskingAgent to be able to exchange information
            _ = new ActorActor(MainOrganization.MetaNetwork.ActorActor, LearnByAskingAgent.AgentId, ExpertAgent.AgentId);
        }
    }
}