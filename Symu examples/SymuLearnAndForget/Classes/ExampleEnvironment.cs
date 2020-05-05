#region Licence

// Description: Symu - SymuLearnAndForget
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using SymuEngine.Common;
using SymuEngine.Environment;
using SymuEngine.Repository.Networks.Knowledges;

#endregion

namespace SymuLearnAndForget.Classes
{
    public class ExampleEnvironment : SymuEnvironment
    {
        public LearnFromSourceAgent LearnFromSourceAgent { get; private set; }
        public LearnByDoingAgent LearnByDoingAgent { get; private set; }
        public LearnByAskingAgent LearnByAskingAgent { get; private set; }
        public LearnAgent DoesNotLearnAgent { get; private set; }
        public ExpertAgent ExpertAgent { get; private set; }
        public Knowledge Knowledge { get; set; }
        public KnowledgeLevel KnowledgeLevel { get; set; }

        public override void SetModelForAgents()
        {
            base.SetModelForAgents();
            WhitePages.Network.NetworkCommunications.Email.CostToSendLevel = GenericLevel.None;
            WhitePages.Network.NetworkCommunications.Email.CostToReceiveLevel = GenericLevel.None;
            WhitePages.Network.AddKnowledge(Knowledge);
            LearnFromSourceAgent = new LearnFromSourceAgent(Organization.NextEntityIndex(), this);
            LearnFromSourceAgent.Cognitive.KnowledgeAndBeliefs.AddKnowledge(Knowledge, KnowledgeLevel,
                Organization.Templates.SimpleHuman.Cognitive.InternalCharacteristics);
            LearnByDoingAgent = new LearnByDoingAgent(Organization.NextEntityIndex(), this);
            LearnByDoingAgent.Cognitive.KnowledgeAndBeliefs.AddKnowledge(Knowledge, KnowledgeLevel,
                Organization.Templates.SimpleHuman.Cognitive.InternalCharacteristics);
            LearnByAskingAgent = new LearnByAskingAgent(Organization.NextEntityIndex(), this);
            LearnByAskingAgent.Cognitive.KnowledgeAndBeliefs.AddKnowledge(Knowledge, KnowledgeLevel,
                Organization.Templates.SimpleHuman.Cognitive.InternalCharacteristics);
            DoesNotLearnAgent = new LearnAgent(Organization.NextEntityIndex(), this);
            DoesNotLearnAgent.Cognitive.KnowledgeAndBeliefs.AddKnowledge(Knowledge, KnowledgeLevel,
                Organization.Templates.SimpleHuman.Cognitive.InternalCharacteristics);
            ExpertAgent = new ExpertAgent(Organization.NextEntityIndex(), this);
            ExpertAgent.Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge = true;
            ExpertAgent.Cognitive.KnowledgeAndBeliefs.AddKnowledge(Knowledge, KnowledgeLevel.Expert,
                Organization.Templates.SimpleHuman.Cognitive.InternalCharacteristics);
        }
    }
}