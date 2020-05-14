#region Licence

// Description: Symu - SymuGroupAndInteraction
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
using SymuBeliefsAndInfluence.Classes;
using SymuEngine.Classes.Scenario;
using SymuEngine.Common;
using SymuEngine.Engine.Form;
using SymuEngine.Environment;
using SymuTools;

#endregion

namespace SymuBeliefsAndInfluence
{
    public partial class Home : SymuForm
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

            HasBeliefs.Checked = OrganizationEntity.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasBelief;
            HasInitialBeliefs.Checked = OrganizationEntity.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasInitialBelief;
            CanSendBeliefs.Checked = OrganizationEntity.Templates.Human.Cognitive.MessageContent.CanSendBeliefs;
            CanReceiveBeliefs.Checked = OrganizationEntity.Templates.Human.Cognitive.MessageContent.CanReceiveBeliefs;

            InfluenceabilityMin.Text = OrganizationEntity.Templates.Human.Cognitive.InternalCharacteristics.InfluenceabilityRateMin.ToString();
            InfluenceabilityMax.Text = OrganizationEntity.Templates.Human.Cognitive.InternalCharacteristics.InfluenceabilityRateMax.ToString();
            InfluentialnessMin.Text = OrganizationEntity.Templates.Human.Cognitive.InternalCharacteristics.InfluentialnessRateMin.ToString();
            InfluentialnessMax.Text = OrganizationEntity.Templates.Human.Cognitive.InternalCharacteristics.InfluentialnessRateMax.ToString();
            MinimumBeliefToSendPerBit.Text = OrganizationEntity.Templates.Human.Cognitive.MessageContent
                .MinimumBeliefToSendPerBit.ToString();
            MinimumNumberOfBitsOfBeliefToSend.Text = OrganizationEntity.Templates.Human.Cognitive.MessageContent
                .MinimumNumberOfBitsOfBeliefToSend.ToString();
            MaximumNumberOfBitsOfBeliefToSend.Text = OrganizationEntity.Templates.Human.Cognitive.MessageContent
                .MaximumNumberOfBitsOfBeliefToSend.ToString();

        }

        protected override void SetUpOrganization()
        {
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
            #region influencer
            _environment.InfluencerTemplate.Cognitive.KnowledgeAndBeliefs.HasBelief = HasBeliefs.Checked;
            _environment.InfluencerTemplate.Cognitive.MessageContent.CanSendBeliefs = CanSendBeliefs.Checked;
            #endregion

            #region Worker
            _environment.WorkerTemplate.Cognitive.KnowledgeAndBeliefs.HasBelief = HasBeliefs.Checked;
            _environment.WorkerTemplate.Cognitive.KnowledgeAndBeliefs.HasInitialBelief = HasInitialBeliefs.Checked;
            _environment.WorkerTemplate.Cognitive.MessageContent.CanReceiveBeliefs = CanReceiveBeliefs.Checked;
            #endregion

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
            WriteTextSafe(Triads,
                _environment.IterationResult.OrganizationFlexibility.Triads.Last().Density
                    .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(TotalBeliefs,
                _environment.IterationResult.OrganizationKnowledgeAndBelief.Beliefs.Last().Sum
                    .ToString("F1", CultureInfo.InvariantCulture));
            var tasksDoneRatio = _environment.TimeStep.Step * _environment.WorkersCount < Constants.Tolerance
                ? 0
                : _environment.IterationResult.Tasks.Total * 100 /
                  (_environment.TimeStep.Step * _environment.WorkersCount);

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

        #region Nested type: SafeCallButtonDelegate

        protected delegate void SafeCallButtonDelegate(Button button, bool enabled);


        #endregion

        private void tbKnowledge_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _environment.Knowledge = byte.Parse(tbKnowledge.Text);
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
                _environment.InfluencerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMin = float.Parse(InfluentialnessMin.Text);
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
                _environment.InfluencerTemplate.Cognitive.InternalCharacteristics.InfluentialnessRateMax = float.Parse(InfluentialnessMax.Text);
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
                _environment.WorkerTemplate.Cognitive.InternalCharacteristics.InfluenceabilityRateMin= float.Parse(InfluenceabilityMin.Text);
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
                _environment.WorkerTemplate.Cognitive.InternalCharacteristics.InfluenceabilityRateMax = float.Parse(InfluenceabilityMax.Text);
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
                    .MinimumBeliefToSendPerBit = float.Parse(MinimumBeliefToSendPerBit.Text);
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
                    .MinimumNumberOfBitsOfBeliefToSend = byte.Parse(MinimumNumberOfBitsOfBeliefToSend.Text);
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
                    .MaximumNumberOfBitsOfBeliefToSend = byte.Parse(MaximumNumberOfBitsOfBeliefToSend.Text);
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
                _environment.InfluencersCount = byte.Parse(tbInfluencers.Text);
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
                _environment.Model.RequiredMandatoryRatio = byte.Parse(MandatoryRatio.Text);
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
    }
}