#region Licence

// Description: SymuBiz - SymuBeliefsAndInfluence
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Symu.Classes.Organization;
using Symu.Classes.Scenario;
using Symu.Common;
using Symu.Common.Classes;
using Symu.Forms;
using Symu.Repository.Entities;
using SymuBeliefsAndInfluence.Classes;
using static Symu.Common.Constants;

#endregion

namespace SymuBeliefsAndInfluence
{
    public partial class Home : SymuForm
    {
        private readonly ExampleOrganization _organization = new ExampleOrganization();
        private readonly ExampleEnvironment _environment = new ExampleEnvironment();
        private int _initialTasksDone;

        public Home()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            DisplayButtons();

            InfluenceModelOn.Checked = _organization.Models.Influence.On;
            InfluenceRateOfAgentsOn.Text =
                _organization.Models.Influence.RateOfAgentsOn.ToString(CultureInfo.InvariantCulture);

            BeliefsModelOn.Checked = _organization.Models.Beliefs.On;
            BeliefsRateOfAgentsOn.Text =
                _organization.Models.Beliefs.RateOfAgentsOn.ToString(CultureInfo.InvariantCulture);

            tbWorkers.Text = _organization.WorkersCount.ToString(CultureInfo.InvariantCulture);
            tbInfluencers.Text = _organization.InfluencersCount.ToString(CultureInfo.InvariantCulture);
            tbBeliefs.Text = _organization.BeliefCount.ToString(CultureInfo.InvariantCulture);

            HasBeliefs.Checked = _organization.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasBelief;
            ThresholdForReacting.Text =
                _organization.Murphies.IncompleteBelief.ThresholdForReacting
                    .ToString(CultureInfo.InvariantCulture);

            #region Influencer

            InfluencerBeliefLevel.Items.AddRange(BeliefLevelService.GetNames());
            InfluencerBeliefLevel.SelectedItem = BeliefLevelService.GetName(_organization.InfluencerTemplate.Cognitive
                .KnowledgeAndBeliefs.DefaultBeliefLevel);
            MinimumBeliefToSendPerBit.Text = _organization.InfluencerTemplate.Cognitive.MessageContent
                .MinimumBeliefToSendPerBit.ToString(CultureInfo.InvariantCulture);
            MinimumNumberOfBitsOfBeliefToSend.Text = _organization.InfluencerTemplate.Cognitive.MessageContent
                .MinimumNumberOfBitsOfBeliefToSend.ToString(CultureInfo.InvariantCulture);
            MaximumNumberOfBitsOfBeliefToSend.Text = _organization.InfluencerTemplate.Cognitive.MessageContent
                .MaximumNumberOfBitsOfBeliefToSend.ToString(CultureInfo.InvariantCulture);
            InfluentialnessMin.Text = _organization.InfluencerTemplate.Cognitive.InternalCharacteristics
                .InfluentialnessRateMin.ToString(CultureInfo.InvariantCulture);
            InfluentialnessMax.Text = _organization.InfluencerTemplate.Cognitive.InternalCharacteristics
                .InfluentialnessRateMax.ToString(CultureInfo.InvariantCulture);
            CanSendBeliefs.Checked = _organization.InfluencerTemplate.Cognitive.MessageContent.CanSendBeliefs;

            #endregion

            #region Worker

            MandatoryRatio.Text =
                _organization.Murphies.IncompleteBelief.MandatoryRatio.ToString(CultureInfo.InvariantCulture);


            RiskAversion.Items.AddRange(GenericLevelService.GetNames());
            RiskAversion.SelectedItem =
                GenericLevelService.GetName(_organization.WorkerTemplate.Cognitive.InternalCharacteristics
                    .RiskAversionLevel);

            BeliefWeight.Items.AddRange(BeliefWeightLevelService.GetNames());
            BeliefWeight.SelectedItem =
                BeliefWeightLevelService.GetName(_organization.Models.BeliefWeightLevel);
            InfluenceabilityMin.Text = _organization.WorkerTemplate.Cognitive.InternalCharacteristics
                .InfluenceabilityRateMin.ToString(CultureInfo.InvariantCulture);
            InfluenceabilityMax.Text = _organization.WorkerTemplate.Cognitive.InternalCharacteristics
                .InfluenceabilityRateMax.ToString(CultureInfo.InvariantCulture);
            CanReceiveBeliefs.Checked = _organization.WorkerTemplate.Cognitive.MessageContent.CanReceiveBeliefs;
            HasInitialBeliefs.Checked =
                _organization.WorkerTemplate.Cognitive.KnowledgeAndBeliefs.HasInitialBelief;

            #endregion
        }

        protected override void SetUpOrganization()
        {
            base.SetUpOrganization();

            _organization.Models.Influence.On = InfluenceModelOn.Checked;
            _organization.Models.Beliefs.On = BeliefsModelOn.Checked;

            _organization.WorkerTemplate.Cognitive.InternalCharacteristics.RiskAversionLevel =
                GenericLevelService.GetValue(RiskAversion.SelectedItem.ToString());

            #region influencer

            _organization.InfluencerTemplate.Cognitive.KnowledgeAndBeliefs.HasBelief = HasBeliefs.Checked;
            _organization.InfluencerTemplate.Cognitive.MessageContent.CanSendBeliefs = CanSendBeliefs.Checked;
            _organization.InfluencerTemplate.Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel =
                BeliefLevelService.GetValue(InfluencerBeliefLevel.SelectedItem.ToString());

            #endregion

            #region Worker

            _organization.WorkerTemplate.Cognitive.KnowledgeAndBeliefs.HasBelief = HasBeliefs.Checked;
            _organization.WorkerTemplate.Cognitive.KnowledgeAndBeliefs.HasInitialBelief = HasInitialBeliefs.Checked;
            _organization.WorkerTemplate.Cognitive.MessageContent.CanReceiveBeliefs = CanReceiveBeliefs.Checked;
            _organization.Models.BeliefWeightLevel =
                BeliefWeightLevelService.GetValue(BeliefWeight.SelectedItem.ToString());

            #endregion

            _organization.AddBeliefs();
        }

        /// <summary>
        ///     Update scenarii settings via the form
        ///     Add scenarios after calling base.UpdateSettings
        /// </summary>
        protected override void SetUpScenarii()
        {
            base.SetUpScenarii();

            var scenario = TimeBasedScenario.CreateInstance(_environment);
            scenario.NumberOfSteps = ushort.Parse(tbSteps.Text, CultureInfo.InvariantCulture);
            AddScenario(scenario);
        }

        protected override void OnStopped()
        {
            base.OnStopped();
            DisplayButtons();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Start(_environment, _organization);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        public override void DisplayStep()
        {
            DisplayButtons();
            WriteTextSafe(TimeStep, _environment.Schedule.Step.ToString(CultureInfo.InvariantCulture));
            UpdateAgents();
        }

        private void UpdateAgents()
        {
            WriteTextSafe(Triads,
                _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density
                    .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(InitialTriads,
                _environment.IterationResult.OrganizationFlexibility.Triads.First().Density
                    .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(TotalBeliefs,
                _environment.IterationResult.KnowledgeAndBeliefResults.Beliefs.Last().Percentage
                    .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(InitialTotalBeliefs,
                _environment.IterationResult.KnowledgeAndBeliefResults.Beliefs.First().Percentage
                    .ToString("F1", CultureInfo.InvariantCulture));
            var tasksDoneRatio = _environment.Schedule.Step * _environment.ExampleOrganization.WorkersCount < Tolerance
                ? 0
                : _environment.IterationResult.Tasks.Done * 100 /
                  (_environment.Schedule.Step * _environment.ExampleOrganization.WorkersCount);
            if (_environment.Schedule.Step == 1)
            {
                _initialTasksDone = tasksDoneRatio;
            }

            WriteTextSafe(InitialTasksDone, _initialTasksDone
                .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(TasksDone, tasksDoneRatio
                .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(TasksCancelled, _environment.IterationResult.Tasks.Cancelled
                .ToString("F0", CultureInfo.InvariantCulture));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Pause();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Resume();
        }

        private void DisplayButtons()
        {
            DisplayButtons(btnStart, btnStop, btnPause, btnResume);
        }

        private void tbWorkers_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _organization.WorkersCount = byte.Parse(tbWorkers.Text, CultureInfo.InvariantCulture);
                tbWorkers.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbWorkers.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbWorkers.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbKnowledge_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _organization.BeliefCount = byte.Parse(tbBeliefs.Text, CultureInfo.InvariantCulture);
                tbBeliefs.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbBeliefs.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbBeliefs.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void InfluentialnessMin_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _organization.InfluencerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMin =
                    float.Parse(InfluentialnessMin.Text, CultureInfo.InvariantCulture);
                InfluentialnessMin.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                InfluentialnessMin.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                InfluentialnessMin.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void InfluentialnessMax_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _organization.InfluencerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMax =
                    float.Parse(InfluentialnessMax.Text, CultureInfo.InvariantCulture);
                InfluentialnessMax.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                InfluentialnessMax.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                InfluentialnessMax.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void InfluenceabilityMin_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _organization.WorkerTemplate.Cognitive.InternalCharacteristics.InfluenceabilityRateMin =
                    float.Parse(InfluenceabilityMin.Text, CultureInfo.InvariantCulture);
                InfluenceabilityMin.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                InfluenceabilityMin.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                InfluenceabilityMin.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void InfluenceabilityMax_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _organization.WorkerTemplate.Cognitive.InternalCharacteristics.InfluenceabilityRateMax =
                    float.Parse(InfluenceabilityMax.Text, CultureInfo.InvariantCulture);
                InfluenceabilityMax.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                InfluenceabilityMax.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                InfluenceabilityMax.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void MinimumBeliefToSendPerBit_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _organization.InfluencerTemplate.Cognitive.MessageContent
                        .MinimumBeliefToSendPerBit =
                    float.Parse(MinimumBeliefToSendPerBit.Text, CultureInfo.InvariantCulture);
                MinimumBeliefToSendPerBit.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                MinimumBeliefToSendPerBit.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                MinimumBeliefToSendPerBit.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void MinimumNumberOfBitsOfBeliefToSend_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _organization.InfluencerTemplate.Cognitive.MessageContent
                    .MinimumNumberOfBitsOfBeliefToSend = byte.Parse(MinimumNumberOfBitsOfBeliefToSend.Text,
                    CultureInfo.InvariantCulture);
                MinimumNumberOfBitsOfBeliefToSend.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                MinimumNumberOfBitsOfBeliefToSend.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                MinimumNumberOfBitsOfBeliefToSend.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void MaximumNumberOfBitsOfBeliefToSend_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _organization.InfluencerTemplate.Cognitive.MessageContent
                    .MaximumNumberOfBitsOfBeliefToSend = byte.Parse(MaximumNumberOfBitsOfBeliefToSend.Text,
                    CultureInfo.InvariantCulture);
                MaximumNumberOfBitsOfBeliefToSend.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                MaximumNumberOfBitsOfBeliefToSend.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                MaximumNumberOfBitsOfBeliefToSend.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbInfluencers_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _organization.InfluencersCount = byte.Parse(tbInfluencers.Text, CultureInfo.InvariantCulture);
                tbInfluencers.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbInfluencers.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbInfluencers.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void MandatoryRatio_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _organization.Murphies.IncompleteBelief.MandatoryRatio =
                    float.Parse(MandatoryRatio.Text, CultureInfo.InvariantCulture);
                MandatoryRatio.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                MandatoryRatio.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                MandatoryRatio.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void RateOfAgentsOn_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _organization.Models.Influence.RateOfAgentsOn =
                    float.Parse(InfluenceRateOfAgentsOn.Text, CultureInfo.InvariantCulture);
                InfluenceRateOfAgentsOn.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                InfluenceRateOfAgentsOn.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                InfluenceRateOfAgentsOn.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void BeliefsRateOfAgentsOn_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _organization.Models.Beliefs.RateOfAgentsOn =
                    float.Parse(BeliefsRateOfAgentsOn.Text, CultureInfo.InvariantCulture);
                BeliefsRateOfAgentsOn.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                BeliefsRateOfAgentsOn.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                BeliefsRateOfAgentsOn.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _organization.Murphies.IncompleteBelief.ThresholdForReacting =
                    float.Parse(ThresholdForReacting.Text, CultureInfo.InvariantCulture);
                ThresholdForReacting.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                ThresholdForReacting.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                ThresholdForReacting.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        #region Menu

        private void symuorgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://symu.org");
        }

        private void documentationToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Process.Start("http://docs.symu.org/");
        }

        private void sourceCodeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Process.Start("http://github.symu.org/");
        }

        private void issuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://github.symu.org/issues");
        }

        #endregion
    }
}