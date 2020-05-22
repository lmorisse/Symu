#region Licence

// Description: Symu - SymuBeliefsAndInfluence
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Symu.Classes.Scenario;
using Symu.Common;
using Symu.Environment;
using SymuForm;
using SymuMurphiesAndBlockers.Classes;
using SymuTools;

#endregion

namespace SymuMurphiesAndBlockers
{
    public partial class Home : BaseForm
    {
        private readonly ExampleEnvironment _environment = new ExampleEnvironment();

        public Home()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            DisplayButtons();

            tbWorkers.Text = _environment.WorkersCount.ToString(CultureInfo.InvariantCulture);
            tbKnowledge.Text = _environment.KnowledgeCount.ToString(CultureInfo.InvariantCulture);
            cbMultipleBlockers.Checked = OrganizationEntity.Murphies.MultipleBlockers;

            #region unavaibility
            tbUnavailabilityThreshhold.Text = OrganizationEntity.Murphies.UnAvailability.Threshold.ToString();
            BeliefsOn.Checked = OrganizationEntity.Murphies.IncompleteBelief.On;
            BeliefsRate.Text = OrganizationEntity.Murphies.IncompleteBelief.RateOfAgentsOn.ToString();
            #endregion

            #region incomplete knowledge murphy

            tbKnowledgeThreshHoldForDoing.Text =
                OrganizationEntity.Murphies.IncompleteKnowledge.KnowledgeThresholdForDoing.ToString(CultureInfo.InvariantCulture);
            tbLackRateOfIncorrectGuess.Text = OrganizationEntity.Murphies.IncompleteKnowledge.RateOfIncorrectGuess.ToString();
            tbLackRateOfAnswers.Text = OrganizationEntity.Murphies.IncompleteKnowledge.RateOfAnswers.ToString();
            tbLackResponseTime.Text = OrganizationEntity.Murphies.IncompleteKnowledge.ResponseTime.ToString();
            cbLimitNumberOfTriesKnowledge.Checked = OrganizationEntity.Murphies.IncompleteKnowledge.LimitNumberOfTries != -1;
            tbMaxNumberOfTriesKnowledge.Text = OrganizationEntity.Murphies.IncompleteKnowledge.LimitNumberOfTries.ToString();
            tbLackDelayBeforeSearchingExternally.Text = OrganizationEntity.Murphies.IncompleteKnowledge.DelayBeforeSearchingExternally.ToString();
            tbRequiredMandatoryRatio.Text = OrganizationEntity.Murphies.IncompleteKnowledge.MandatoryRatio.ToString();

            KnowledgeOn.Checked = OrganizationEntity.Murphies.IncompleteKnowledge.On;

            KnowledgeRate.Text =  OrganizationEntity.Murphies.IncompleteKnowledge.RateOfAgentsOn.ToString();
            #endregion

            #region incomplete belief murphy

            BeliefsRate.Text = OrganizationEntity.Murphies.IncompleteBelief.RateOfAgentsOn.ToString();
            BeliefsOn.Checked = OrganizationEntity.Murphies.IncompleteBelief.On;
            tbBeliefRateIncorrectGuess.Text = OrganizationEntity.Murphies.IncompleteBelief.RateOfIncorrectGuess.ToString();
            tbBeliefRateAnswers.Text = OrganizationEntity.Murphies.IncompleteBelief.RateOfAnswers.ToString();
            tbBeliefResponseTime.Text = OrganizationEntity.Murphies.IncompleteBelief.ResponseTime.ToString();
            cbLimitNumberOfTriesBelief.Checked = OrganizationEntity.Murphies.IncompleteBelief.LimitNumberOfTries != -1;
            tbMaxNumberOfTriesBelief.Text = OrganizationEntity.Murphies.IncompleteBelief.LimitNumberOfTries.ToString();
            #endregion


        }

        protected override void UpdateSettings()
        {
            if (!cbLimitNumberOfTriesKnowledge.Checked)
            {
                OrganizationEntity.Murphies.IncompleteKnowledge.LimitNumberOfTries = -1;
            }
            else
            {
                OrganizationEntity.Murphies.IncompleteKnowledge.LimitNumberOfTries = Convert.ToSByte(tbMaxNumberOfTriesKnowledge.Text);
            }
            OrganizationEntity.Murphies.MultipleBlockers = cbMultipleBlockers.Checked;
            OrganizationEntity.Murphies.IncompleteBelief.On = BeliefsOn.Checked;
            OrganizationEntity.Murphies.UnAvailability.On = UnavailabilityOn.Checked;
            OrganizationEntity.Murphies.IncompleteKnowledge.On = KnowledgeOn.Checked;

            SetDebug(false);
            TimeStepType = TimeStepType.Daily;
        }

        protected override void SetScenarii()
        {
            _ = new TimeStepScenario(_environment)
            {
                NumberOfSteps = ushort.Parse(tbSteps.Text)
            };
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

        public override void Display()
        {
            DisplayButtons();
            WriteTextSafe(TimeStep, _environment.TimeStep.Step.ToString());
            UpdateAgents();
        }

        private void UpdateAgents()
        {
            WriteTextSafe(Capacity,
                _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density
                    .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(Blockers,
                _environment.IterationResult.OrganizationKnowledgeAndBelief.Beliefs.Last().Sum
                    .ToString("F1", CultureInfo.InvariantCulture));
            var tasksDoneRatio = _environment.TimeStep.Step * _environment.WorkersCount < Constants.Tolerance
                ? 0
                : _environment.IterationResult.Tasks.Total * 100 /
                  (_environment.TimeStep.Step * _environment.WorkersCount);
            if (_environment.TimeStep.Step == 1)
            {
            }

            WriteTextSafe(TasksDone, tasksDoneRatio
                .ToString("F1", CultureInfo.InvariantCulture));
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
            switch (State)
            {
                case AgentState.Stopped:
                case AgentState.NotStarted:
                    WriteButtonSafe(btnStart, true);
                    WriteButtonSafe(btnStop, false);
                    WriteButtonSafe(btnPause, false);
                    WriteButtonSafe(btnResume, false);
                    break;
                case AgentState.Stopping:
                case AgentState.Starting:
                    WriteButtonSafe(btnStart, false);
                    WriteButtonSafe(btnStop, false);
                    WriteButtonSafe(btnPause, false);
                    WriteButtonSafe(btnResume, false);
                    break;
                case AgentState.Started:
                    WriteButtonSafe(btnStart, false);
                    WriteButtonSafe(btnStop, true);
                    WriteButtonSafe(btnPause, true);
                    WriteButtonSafe(btnResume, false);
                    break;
                case AgentState.Paused:
                    WriteButtonSafe(btnStart, false);
                    WriteButtonSafe(btnStop, true);
                    WriteButtonSafe(btnPause, false);
                    WriteButtonSafe(btnResume, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected void WriteButtonSafe(Button button, bool enabled)
        {
            if (button is null)
            {
                throw new ArgumentNullException(nameof(button));
            }

            if (button.InvokeRequired)
            {
                var d = new SafeCallButtonDelegate(WriteButtonSafe);
                button.Invoke(d, button, enabled);
            }
            else
            {
                button.Enabled = enabled;
            }
        }

        private void tbWorkers_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _environment.WorkersCount = byte.Parse(tbWorkers.Text);
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
                _environment.KnowledgeCount = byte.Parse(tbKnowledge.Text);
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

       

        #region Nested type: SafeCallButtonDelegate

        protected delegate void SafeCallButtonDelegate(Button button, bool enabled);

        #endregion

        private void tbKnowledgeThreshHoldForDoing_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Murphies.IncompleteKnowledge.KnowledgeThresholdForDoing = float.Parse(tbKnowledgeThreshHoldForDoing.Text);
                tbKnowledgeThreshHoldForDoing.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbKnowledgeThreshHoldForDoing.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbKnowledgeThreshHoldForDoing.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbLackRateOfIncorrectGuess_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Murphies.IncompleteKnowledge.RateOfIncorrectGuess = float.Parse(tbLackRateOfIncorrectGuess.Text);
                tbLackRateOfIncorrectGuess.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbLackRateOfIncorrectGuess.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbLackRateOfIncorrectGuess.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbLackRateOfAnswers_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Murphies.IncompleteKnowledge.RateOfAnswers = float.Parse(tbLackRateOfAnswers.Text);
                tbLackRateOfAnswers.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbLackRateOfAnswers.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbLackRateOfAnswers.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbLackResponseTime_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Murphies.IncompleteKnowledge.ResponseTime = byte.Parse(tbLackResponseTime.Text);
                tbLackResponseTime.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbLackResponseTime.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbLackResponseTime.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbMaxNumberOfTriesKnowledge_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Murphies.IncompleteKnowledge.LimitNumberOfTries = sbyte.Parse(tbMaxNumberOfTriesKnowledge.Text);
                tbMaxNumberOfTriesKnowledge.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbMaxNumberOfTriesKnowledge.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbMaxNumberOfTriesKnowledge.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbLackDelayBeforeSearchingExternally_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Murphies.IncompleteKnowledge.DelayBeforeSearchingExternally = byte.Parse(tbLackDelayBeforeSearchingExternally.Text);
                tbLackDelayBeforeSearchingExternally.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbLackDelayBeforeSearchingExternally.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbLackDelayBeforeSearchingExternally.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbRequiredMandatoryRatio_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Murphies.IncompleteKnowledge.MandatoryRatio = float.Parse(tbRequiredMandatoryRatio.Text);
                tbRequiredMandatoryRatio.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbRequiredMandatoryRatio.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbRequiredMandatoryRatio.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbBeliefRateIncorrectGuess_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Murphies.IncompleteBelief.RateOfIncorrectGuess= float.Parse(tbBeliefRateIncorrectGuess.Text);
                tbBeliefRateIncorrectGuess.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbBeliefRateIncorrectGuess.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbBeliefRateIncorrectGuess.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbBeliefRateAnswers_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Murphies.IncompleteBelief.RateOfAnswers= float.Parse(tbBeliefRateAnswers.Text);
                tbBeliefRateAnswers.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbBeliefRateAnswers.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbBeliefRateAnswers.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbBeliefResponseTime_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Murphies.IncompleteBelief.ResponseTime = byte.Parse(tbBeliefResponseTime.Text);
                tbBeliefResponseTime.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbBeliefResponseTime.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbBeliefResponseTime.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbMaxNumberOfTriesBelief_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Murphies.IncompleteBelief.LimitNumberOfTries = sbyte.Parse(tbMaxNumberOfTriesBelief.Text);
                tbMaxNumberOfTriesBelief.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbMaxNumberOfTriesBelief.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbMaxNumberOfTriesBelief.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbUnavailabilityThreshhold_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Murphies.UnAvailability.Threshold = float.Parse(tbUnavailabilityThreshhold.Text);
                tbUnavailabilityThreshhold.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbUnavailabilityThreshhold.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbUnavailabilityThreshhold.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void BeliefsRate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Murphies.IncompleteBelief.RateOfAgentsOn = float.Parse(BeliefsRate.Text);
                BeliefsRate.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                BeliefsRate.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                BeliefsRate.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void UnavailabilityRate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Murphies.UnAvailability.RateOfAgentsOn = float.Parse(UnavailabilityRate.Text);
                UnavailabilityRate.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                UnavailabilityRate.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                UnavailabilityRate.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void KnowledgeRate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Murphies.IncompleteKnowledge.RateOfAgentsOn = float.Parse(KnowledgeRate.Text);
                KnowledgeRate.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                KnowledgeRate.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                KnowledgeRate.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }
    }
}