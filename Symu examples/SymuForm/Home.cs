#region Licence

// Description: Symu - SymuForm
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;
using Symu.Classes;
using SymuEngine.Classes.Agent.Models.Templates.Communication;
using SymuEngine.Classes.Scenario;
using SymuEngine.Engine.Form;
using SymuEngine.Environment.TimeStep;
using SymuEngine.Repository.Networks.Databases.Repository;

#endregion

namespace Symu
{
    public partial class Home : SymuForm
    {
        private readonly SymuEnvironment _environment = new SymuEnvironment();
        private Database _wiki;

        public Home()
        {
            InitializeComponent();
        }

        protected override void SetUpOrganization()
        {
            // Murphy
            UnAvailability.On = true;
            // Common Wiki 
            CommunicationTemplate communication = new ViaPlatformTemplate();
            _wiki = new Database(OrganizationEntity.Id.Key, communication.Cognitive.TasksAndPerformance, -1);
            OrganizationEntity.AddDatabase(_wiki);
            // Models
            OrganizationEntity.OrganizationModels.Learning.On = true;
        }

        protected override void SetScenarii()
        {
            _ = new TimeStepScenario(OrganizationEntity.NextEntityIndex(), _environment)
            {
                NumberOfSteps = 500,
                TimeStepType = TimeStepType.Daily
            };
            TimeStepType = TimeStepType.Daily;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            _environment.WorkersCount = Convert.ToInt32(tbWorkers.Text);
            _environment.TimeStep.Type = TimeStepType.Intraday;
            Start(_environment);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        public override void Display()
        {
            WriteTextSafe(TimeStep, _environment.TimeStep.Step.ToString());
            UpdateKnowledge();
            UpDateMessages();
            UpdateAgents();
        }

        private void UpDateMessages()
        {
            if (_environment.Messages.SentMessagesCount == 0)
            {
                return;
            }

            WriteTextSafe(lblMessagesSent, _environment.Messages.SentMessagesCount.ToString());
        }

        private void UpdateKnowledge()
        {
            var sum = _environment.WhitePages.Network.NetworkKnowledges.AgentsRepository.Values.Sum(expertise =>
                expertise.GetKnowledgesSum());

            WriteTextSafe(lblKnowledge, sum.ToString());
            // Wiki
            sum = _wiki.GetKnowledgesSum();
            WriteTextSafe(lblWiki, sum.ToString());
        }

        private void UpdateAgents()
        {
            WriteTextSafe(lblWorked, _environment.IterationResult.Capacity.ToString());
            var done = _environment.WhitePages.FilteredAgentsByClassKey(GroupAgent.ClassKey)
                .Aggregate(0, (current, agent) => current + ((GroupAgent) agent).TotalTasksDone);

            WriteTextSafe(lblTasksDone, done.ToString());
        }
    }
}