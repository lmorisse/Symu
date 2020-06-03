#region Licence

// Description: Symu - SymuBeliefsAndInfluence
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
using Symu.Classes.Scenario;
using Symu.Environment;
using Symu.Forms;
using Symu.Repository.Networks.Beliefs;
using SymuBeliefsAndInfluence.Classes;
using static Symu.Tools.Constants;

#endregion

namespace SymuBeliefsAndInfluence
{
    public partial class Home : SymuForm
    {
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

            InfluenceModelOn.Checked = OrganizationEntity.Models.Influence.On;
            InfluenceRateOfAgentsOn.Text = OrganizationEntity.Models.Influence.RateOfAgentsOn.ToString(CultureInfo.InvariantCulture);

            BeliefsModelOn.Checked = OrganizationEntity.Models.Beliefs.On;
            BeliefsRateOfAgentsOn.Text = OrganizationEntity.Models.Beliefs.RateOfAgentsOn.ToString(CultureInfo.InvariantCulture);

            tbWorkers.Text = _environment.WorkersCount.ToString(CultureInfo.InvariantCulture);
            tbInfluencers.Text = _environment.InfluencersCount.ToString(CultureInfo.InvariantCulture);
            tbKnowledge.Text = _environment.KnowledgeCount.ToString(CultureInfo.InvariantCulture);

            HasBeliefs.Checked = OrganizationEntity.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasBelief;

            #region Influencer

            InfluencerBeliefLevel.Items.AddRange(BeliefLevelService.GetNames());
            InfluencerBeliefLevel.SelectedItem = BeliefLevelService.GetName(OrganizationEntity.Templates.Human.Cognitive
                .KnowledgeAndBeliefs.DefaultBeliefLevel);
            MinimumBeliefToSendPerBit.Text = OrganizationEntity.Templates.Human.Cognitive.MessageContent
                .MinimumBeliefToSendPerBit.ToString(CultureInfo.InvariantCulture);
            MinimumNumberOfBitsOfBeliefToSend.Text = OrganizationEntity.Templates.Human.Cognitive.MessageContent
                .MinimumNumberOfBitsOfBeliefToSend.ToString(CultureInfo.InvariantCulture);
            MaximumNumberOfBitsOfBeliefToSend.Text = OrganizationEntity.Templates.Human.Cognitive.MessageContent
                .MaximumNumberOfBitsOfBeliefToSend.ToString(CultureInfo.InvariantCulture);
            InfluentialnessMin.Text = OrganizationEntity.Templates.Human.Cognitive.InternalCharacteristics
                .InfluentialnessRateMin.ToString(CultureInfo.InvariantCulture);
            InfluentialnessMax.Text = OrganizationEntity.Templates.Human.Cognitive.InternalCharacteristics
                .InfluentialnessRateMax.ToString(CultureInfo.InvariantCulture);
            CanSendBeliefs.Checked = OrganizationEntity.Templates.Human.Cognitive.MessageContent.CanSendBeliefs;

            #endregion

            #region Worker

            MandatoryRatio.Text = _environment.Model.MandatoryRatio.ToString(CultureInfo.InvariantCulture);
            RiskAversion.Text = _environment.WorkerTemplate.Cognitive.InternalCharacteristics.RiskAversionThreshold
                .ToString(CultureInfo.InvariantCulture);
            BeliefWeight.Items.AddRange(BeliefWeightLevelService.GetNames());
            BeliefWeight.SelectedItem =
                BeliefWeightLevelService.GetName(OrganizationEntity.Models.ImpactOfBeliefOnTask);
            InfluenceabilityMin.Text = OrganizationEntity.Templates.Human.Cognitive.InternalCharacteristics
                .InfluenceabilityRateMin.ToString(CultureInfo.InvariantCulture);
            InfluenceabilityMax.Text = OrganizationEntity.Templates.Human.Cognitive.InternalCharacteristics
                .InfluenceabilityRateMax.ToString(CultureInfo.InvariantCulture);
            CanReceiveBeliefs.Checked = OrganizationEntity.Templates.Human.Cognitive.MessageContent.CanReceiveBeliefs;
            HasInitialBeliefs.Checked =
                OrganizationEntity.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasInitialBelief;

            #endregion
        }

        protected override void UpdateSettings()
        {
            base.UpdateSettings();
            OrganizationEntity.Models.Influence.On = InfluenceModelOn.Checked;
            OrganizationEntity.Models.Beliefs.On = BeliefsModelOn.Checked;

            #region influencer

            _environment.InfluencerTemplate.Cognitive.KnowledgeAndBeliefs.HasBelief = HasBeliefs.Checked;
            _environment.InfluencerTemplate.Cognitive.MessageContent.CanSendBeliefs = CanSendBeliefs.Checked;
            _environment.InfluencerTemplate.Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel =
                BeliefLevelService.GetValue(InfluencerBeliefLevel.SelectedItem.ToString());

            #endregion

            #region Worker

            _environment.WorkerTemplate.Cognitive.KnowledgeAndBeliefs.HasBelief = HasBeliefs.Checked;
            _environment.WorkerTemplate.Cognitive.KnowledgeAndBeliefs.HasInitialBelief = HasInitialBeliefs.Checked;
            _environment.WorkerTemplate.Cognitive.MessageContent.CanReceiveBeliefs = CanReceiveBeliefs.Checked;
            OrganizationEntity.Models.ImpactOfBeliefOnTask =
                BeliefWeightLevelService.GetValue(BeliefWeight.SelectedItem.ToString());

            #endregion

            var scenario = new TimeBasedScenario(_environment)
            {
                NumberOfSteps = ushort.Parse(tbSteps.Text, CultureInfo.InvariantCulture)
            };

            AddScenario(scenario);

            SetTimeStepType(TimeStepType.Daily);
        }

        protected override void OnStopped()
        {
            base.OnStopped();
            DisplayButtons();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Start(_environment);
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
                _environment.IterationResult.OrganizationKnowledgeAndBelief.Beliefs.Last().Sum
                    .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(InitialTotalBeliefs,
                _environment.IterationResult.OrganizationKnowledgeAndBelief.Beliefs.First().Sum
                    .ToString("F1", CultureInfo.InvariantCulture));
            var tasksDoneRatio = _environment.Schedule.Step * _environment.WorkersCount < Tolerance
                ? 0
                : _environment.IterationResult.Tasks.Done * 100 /
                  (_environment.Schedule.Step * _environment.WorkersCount);
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
                _environment.WorkersCount = byte.Parse(tbWorkers.Text, CultureInfo.InvariantCulture);
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
                _environment.KnowledgeCount = byte.Parse(tbKnowledge.Text, CultureInfo.InvariantCulture);
                tbKnowledge.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbKnowledge.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbKnowledge.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void InfluentialnessMin_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _environment.InfluencerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMin =
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
                _environment.InfluencerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMax =
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
                _environment.WorkerTemplate.Cognitive.InternalCharacteristics.InfluenceabilityRateMin =
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
                _environment.WorkerTemplate.Cognitive.InternalCharacteristics.InfluenceabilityRateMax =
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
                _environment.InfluencerTemplate.Cognitive.MessageContent
                    .MinimumBeliefToSendPerBit = float.Parse(MinimumBeliefToSendPerBit.Text, CultureInfo.InvariantCulture);
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
                _environment.InfluencerTemplate.Cognitive.MessageContent
                    .MinimumNumberOfBitsOfBeliefToSend = byte.Parse(MinimumNumberOfBitsOfBeliefToSend.Text, CultureInfo.InvariantCulture);
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
                _environment.InfluencerTemplate.Cognitive.MessageContent
                    .MaximumNumberOfBitsOfBeliefToSend = byte.Parse(MaximumNumberOfBitsOfBeliefToSend.Text, CultureInfo.InvariantCulture);
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
                _environment.InfluencersCount = byte.Parse(tbInfluencers.Text, CultureInfo.InvariantCulture);
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
                _environment.Model.MandatoryRatio = float.Parse(MandatoryRatio.Text, CultureInfo.InvariantCulture);
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
                OrganizationEntity.Models.Influence.RateOfAgentsOn = float.Parse(InfluenceRateOfAgentsOn.Text, CultureInfo.InvariantCulture);
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

        private void RiskAversion_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _environment.WorkerTemplate.Cognitive.InternalCharacteristics.RiskAversionThreshold =
                    float.Parse(RiskAversion.Text, CultureInfo.InvariantCulture);
                RiskAversion.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                RiskAversion.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                RiskAversion.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void BeliefsRateOfAgentsOn_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Models.Beliefs.RateOfAgentsOn = float.Parse(BeliefsRateOfAgentsOn.Text, CultureInfo.InvariantCulture);
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