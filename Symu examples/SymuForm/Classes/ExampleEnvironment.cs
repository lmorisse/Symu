#region Licence

// Description: Symu - SymuForm
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using SymuEngine.Classes.Agent.Models.Templates.Communication;
using SymuEngine.Common;
using SymuEngine.Environment;
using SymuEngine.Environment.TimeStep;
using SymuEngine.Repository.Networks.Knowledges;

#endregion

namespace Symu.Classes
{
    public class ExampleEnvironment : SymuEnvironment
    {
        private readonly Knowledge _knowledge = new Knowledge(1, "1", 50);
        public int WorkersCount { get; set; }

        public override void SetModelForAgents()
        {
            base.SetModelForAgents();
            TimeStep.Type = TimeStepType.Intraday;
            WhitePages.Network.NetworkCommunications.Email.CostToSendLevel = GenericLevel.None;
            WhitePages.Network.NetworkCommunications.Email.CostToReceiveLevel = GenericLevel.None;
            WhitePages.Network.AddKnowledge(_knowledge);
            var group = new GroupAgent(Organization.NextEntityIndex(), this);
            for (var i = 0; i < WorkersCount; i++)
            {
                var actor = new PersonAgent(Organization.NextEntityIndex(), this)
                {
                    GroupId = group.Id
                };
                CommunicationTemplate communication = new EmailTemplate();
                WhitePages.Network.AddEmail(actor.Id, communication);
                WhitePages.Network.AddMemberToGroup(actor.Id, 100, group.Id);
                actor.LearnNewKnowledge(_knowledge.Id, 0);
            }
        }
    }
}