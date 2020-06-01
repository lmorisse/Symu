#region Licence

// Description: Symu - SymuLearnAndForget
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
using Symu.Classes.Agents.Models.CognitiveModel;
using Symu.Classes.Scenario;
using Symu.Common;
using Symu.Environment;
using Symu.Forms;
using Symu.Repository.Networks.Knowledges;
using SymuLearnAndForget.Classes;
using static Symu.Tools.Constants;

#endregion

namespace SymuLearnAndForget
{
    public partial class Home : SymuForm
    {
        private readonly ExampleEnvironment _environment = new ExampleEnvironment();
        private ushort _fullKnowledge;

        public Home()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            DisplayButtons();

            #region Knowledge

            tbKnowledgeLength.Text = "50";
            cbBinaryKnowledge.Checked =
                OrganizationEntity.Models.Generator == RandomGenerator.RandomBinary;

            #endregion

            #region Learning

            cbLearningOn.Checked = true;
            tbMicroLearningAgentRate.Text = "1";
            cbHasInitialKnowledge.Checked = OrganizationEntity.Templates.Human.Cognitive.KnowledgeAndBeliefs
                .HasInitialKnowledge;
            cbInitialKnowledgeLevel.Items.AddRange(KnowledgeLevelService.GetNames());
            cbInitialKnowledgeLevel.SelectedItem = KnowledgeLevelService.GetName(_environment.KnowledgeLevel);
            cbHasKnowledge.Checked =
                OrganizationEntity.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasKnowledge;
            tbKnowledgeThreshold.Text =
                OrganizationEntity.Murphies.IncompleteKnowledge.ThresholdForReacting
                    .ToString(CultureInfo.InvariantCulture);
            tbTimeToLive.Text =
                OrganizationEntity.Templates.Human.Cognitive.InternalCharacteristics.TimeToLive.ToString(
                    CultureInfo.InvariantCulture);
            tbLearnRate.Text =
                OrganizationEntity.Templates.Human.Cognitive.TasksAndPerformance.LearningRate.ToString(CultureInfo
                    .InvariantCulture);
            tbLearnByDoingRate.Text =
                OrganizationEntity.Templates.Human.Cognitive.TasksAndPerformance.LearningByDoingRate.ToString(
                    CultureInfo.InvariantCulture);
            tbTaskCostFactor.Text =
                OrganizationEntity.Murphies.IncompleteKnowledge.CostFactorOfGuessing
                    .ToString(CultureInfo.InvariantCulture);
            cbCanSendKnowledge.Checked =
                OrganizationEntity.Templates.Human.Cognitive.MessageContent.CanSendKnowledge;
            cbCanReceiveKnowledge.Checked =
                OrganizationEntity.Templates.Human.Cognitive.MessageContent.CanReceiveKnowledge;
            // Email
            tbMinKnowledge.Text =
                OrganizationEntity.Templates.Human.Cognitive.MessageContent.MinimumKnowledgeToSendPerBit.ToString(
                    CultureInfo.InvariantCulture);
            tbMinBitsKnowledge.Text =
                OrganizationEntity.Templates.Human.Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend
                    .ToString(CultureInfo.InvariantCulture);
            tbMaxBitsKnowledge.Text =
                OrganizationEntity.Templates.Human.Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend
                    .ToString(CultureInfo.InvariantCulture);
            tbMaxRateLearnable.Text =
                OrganizationEntity.Templates.Email.MaxRateLearnable.ToString(CultureInfo.InvariantCulture);

            #endregion

            #region Forgetting

            cbForgettingOn.Checked = true;
            tbForgettingAgentRate.Text = "1";
            tbForgettingMean.Text =
                OrganizationEntity.Templates.Human.Cognitive.InternalCharacteristics.ForgettingMean.ToString(
                    CultureInfo.InvariantCulture);
            cgForgettingStdDev.Items.AddRange(GenericLevelService.GetNames());
            cgForgettingStdDev.SelectedItem =
                GenericLevelService.GetName(OrganizationEntity.Templates.Human.Cognitive.InternalCharacteristics
                    .ForgettingStandardDeviation);
            cbPartialForgetting.Checked = OrganizationEntity.Templates.Human.Cognitive.InternalCharacteristics
                .PartialForgetting;
            tbPartialForgettingRate.Text =
                OrganizationEntity.Templates.Human.Cognitive.InternalCharacteristics.PartialForgettingRate
                    .ToString(CultureInfo.InvariantCulture);
            tbMinimRemainningLevel.Text =
                OrganizationEntity.Templates.Human.Cognitive.InternalCharacteristics.MinimumRemainingKnowledge
                    .ToString(CultureInfo.InvariantCulture);
            switch (OrganizationEntity.Templates.Human.Cognitive.InternalCharacteristics.ForgettingSelectingMode)
            {
                case ForgettingSelectingMode.Random:
                    rbICForgettingSelectingRandomMode.Checked = true;
                    break;
                case ForgettingSelectingMode.Oldest:
                    rbICForgettingSelectingOldestMode.Checked = true;
                    break;
            }

            #endregion
        }

        protected override void UpdateSettings()
        {
            #region Knowledge

            OrganizationEntity.Models.Generator = cbBinaryKnowledge.Checked
                ? RandomGenerator.RandomBinary
                : RandomGenerator.RandomUniform;

            #endregion

            #region Learning

            OrganizationEntity.Models.Learning.On = cbLearningOn.Checked;
            OrganizationEntity.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasKnowledge =
                cbHasKnowledge.Checked;
            OrganizationEntity.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge =
                cbHasInitialKnowledge.Checked;
            _environment.KnowledgeLevel =
                KnowledgeLevelService.GetValue(cbInitialKnowledgeLevel.SelectedItem.ToString());

            OrganizationEntity.Templates.Human.Cognitive.MessageContent.CanSendKnowledge =
                cbCanSendKnowledge.Checked;
            OrganizationEntity.Templates.Human.Cognitive.MessageContent.CanReceiveKnowledge =
                cbCanReceiveKnowledge.Checked;
            // In this example:
            // we use email, but we can use other communications medium
            // we set the email template with the same values of Human Template, but it could be different
            OrganizationEntity.Templates.Email.Cognitive.MessageContent.CanSendKnowledge = cbCanSendKnowledge.Checked;
            OrganizationEntity.Templates.Email.Cognitive.MessageContent.CanReceiveKnowledge =
                cbCanReceiveKnowledge.Checked;

            #endregion

            #region Forgetting

            OrganizationEntity.Models.Forgetting.On = cbForgettingOn.Checked;
            OrganizationEntity.Templates.Human.Cognitive.InternalCharacteristics.ForgettingStandardDeviation =
                GenericLevelService.GetValue(cgForgettingStdDev.SelectedItem.ToString());
            OrganizationEntity.Templates.Human.Cognitive.InternalCharacteristics.PartialForgetting =
                cbPartialForgetting.Checked;
            OrganizationEntity.Templates.Human.Cognitive.InternalCharacteristics.ForgettingSelectingMode =
                rbICForgettingSelectingRandomMode.Checked
                    ? ForgettingSelectingMode.Random
                    : ForgettingSelectingMode.Oldest;

            #endregion

            _fullKnowledge = 0;
            var scenario = new TimeBasedScenario(_environment)
            {
                NumberOfSteps = ushort.Parse(tbSteps.Text)
            };

            AddScenario(scenario);

            SetRandomLevel(cbRandomLevel.SelectedIndex);
            SetTimeStepType(TimeStepType.Daily);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Start(_environment);
        }

        protected override void OnStopped()
        {
            base.OnStopped();
            DisplayButtons();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        public override void DisplayStep()
        {
            DisplayButtons();
            WriteTextSafe(TimeStep, _environment.Schedule.Step.ToString());
            // 1st Agent
            WriteTextSafe(lblKnowledge1,
                _environment.LearnFromSourceAgent.KnowledgeModel.Expertise.GetKnowledgesSum()
                    .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(lblLearning1,
                _environment.LearnFromSourceAgent.KnowledgeModel.Expertise.Learning.ToString("F1",
                    CultureInfo.InvariantCulture));
            WriteTextSafe(lblForgetting1,
                _environment.LearnFromSourceAgent.KnowledgeModel.Expertise.Forgetting.ToString("F1",
                    CultureInfo.InvariantCulture));
            // 2nd Agent
            WriteTextSafe(lblKnowledge2,
                _environment.LearnByDoingAgent.KnowledgeModel.Expertise.GetKnowledgesSum()
                    .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(lblLearning2,
                _environment.LearnByDoingAgent.KnowledgeModel.Expertise.Learning.ToString("F1",
                    CultureInfo.InvariantCulture));
            WriteTextSafe(lblForgetting2,
                _environment.LearnByDoingAgent.KnowledgeModel.Expertise.Forgetting.ToString("F1",
                    CultureInfo.InvariantCulture));
            // 3rd Agent
            WriteTextSafe(lblKnowledge3,
                _environment.LearnByAskingAgent.KnowledgeModel.Expertise.GetKnowledgesSum()
                    .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(lblLearning3,
                _environment.LearnByAskingAgent.KnowledgeModel.Expertise.Learning.ToString("F1",
                    CultureInfo.InvariantCulture));
            WriteTextSafe(lblForgetting3,
                _environment.LearnByAskingAgent.KnowledgeModel.Expertise.Forgetting.ToString("F1",
                    CultureInfo.InvariantCulture));
            // 4th Agent
            WriteTextSafe(lblKnowledge4,
                _environment.DoesNotLearnAgent.KnowledgeModel.Expertise.GetKnowledgesSum()
                    .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(lblLearning4,
                _environment.DoesNotLearnAgent.KnowledgeModel.Expertise.Learning.ToString("F1",
                    CultureInfo.InvariantCulture));
            WriteTextSafe(lblForgetting4,
                _environment.DoesNotLearnAgent.KnowledgeModel.Expertise.Forgetting.ToString("F1",
                    CultureInfo.InvariantCulture));
            // Expert Agent
            WriteTextSafe(lblExpertKnowledge,
                _environment.ExpertAgent.KnowledgeModel.Expertise.GetKnowledgesSum()
                    .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(lblExpertLearning,
                _environment.ExpertAgent.KnowledgeModel.Expertise.Learning.ToString("F1",
                    CultureInfo.InvariantCulture));
            WriteTextSafe(lblExpertForgetting,
                _environment.ExpertAgent.KnowledgeModel.Expertise.Forgetting.ToString("F1",
                    CultureInfo.InvariantCulture));
            // Wiki
            WriteTextSafe(lblWiki, _environment.Wiki.GetKnowledgesSum().ToString("F1", CultureInfo.InvariantCulture));
            if (_fullKnowledge == 0 &&
                Math.Abs(_environment.Wiki.GetKnowledgesSum() - _environment.Knowledge.Length) < Tolerance)
            {
                _fullKnowledge = _environment.Schedule.Step;
            }

            WriteTextSafe(lblFullKnowledge, _fullKnowledge.ToString(CultureInfo.InvariantCulture));

            // Global Knowledge - using iteration result

            lock (_environment.IterationResult.OrganizationKnowledgeAndBelief.Knowledge)
            {
                if (!_environment.IterationResult.OrganizationKnowledgeAndBelief.Knowledge.Any())
                {
                    return;
                }
            }


            var knowledge = _environment.IterationResult.OrganizationKnowledgeAndBelief.Knowledge.Last();
            WriteTextSafe(lblGlobalKnowledge, knowledge.Sum.ToString("F1", CultureInfo.InvariantCulture));
            var obsolescence = _environment.IterationResult.OrganizationKnowledgeAndBelief.KnowledgeObsolescence.Last();
            WriteTextSafe(lblGlobalObsolescence, obsolescence.Sum.ToString("F1", CultureInfo.InvariantCulture));
            var learning = _environment.IterationResult.OrganizationKnowledgeAndBelief.Learning.Last();
            WriteTextSafe(lblGlobalLearning, learning.Sum.ToString("F1", CultureInfo.InvariantCulture));
            var forgetting = _environment.IterationResult.OrganizationKnowledgeAndBelief.Forgetting.Last();
            WriteTextSafe(lblGlobalForgetting, forgetting.Sum.ToString("F1", CultureInfo.InvariantCulture));
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

        private void tbMicroLearningAgentRate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Models.Learning.RateOfAgentsOn =
                    float.Parse(tbMicroLearningAgentRate.Text, CultureInfo.InvariantCulture);
                tbMicroLearningAgentRate.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbMicroLearningAgentRate.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbMicroLearningAgentRate.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbKnowledgeThreshold_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Murphies.IncompleteKnowledge.ThresholdForReacting =
                    float.Parse(tbKnowledgeThreshold.Text, CultureInfo.InvariantCulture);
                tbKnowledgeThreshold.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbKnowledgeThreshold.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbKnowledgeThreshold.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbLearnRate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Templates.Human.Cognitive.TasksAndPerformance.LearningRate =
                    float.Parse(tbLearnRate.Text, CultureInfo.InvariantCulture);
                tbLearnRate.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbLearnRate.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbLearnRate.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbLearnByDoingRate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Templates.Human.Cognitive.TasksAndPerformance.LearningByDoingRate =
                    float.Parse(tbLearnByDoingRate.Text, CultureInfo.InvariantCulture);
                tbLearnByDoingRate.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbLearnByDoingRate.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbLearnByDoingRate.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbTaskCostFactor_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Murphies.IncompleteKnowledge.CostFactorOfGuessing =
                    float.Parse(tbTaskCostFactor.Text, CultureInfo.InvariantCulture);
                tbTaskCostFactor.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbTaskCostFactor.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbTaskCostFactor.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbMinKnowledge_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Templates.Human.Cognitive.MessageContent.MinimumKnowledgeToSendPerBit =
                    float.Parse(tbMinKnowledge.Text, CultureInfo.InvariantCulture);
                OrganizationEntity.Templates.Email.Cognitive.MessageContent.MinimumKnowledgeToSendPerBit =
                    float.Parse(tbMinKnowledge.Text, CultureInfo.InvariantCulture);
                tbMinKnowledge.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbMinKnowledge.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbMinKnowledge.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbMinBitsKnowledge_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Templates.Human.Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend =
                    byte.Parse(tbMinBitsKnowledge.Text, CultureInfo.InvariantCulture);
                OrganizationEntity.Templates.Email.Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend =
                    byte.Parse(tbMinBitsKnowledge.Text, CultureInfo.InvariantCulture);
                tbMinBitsKnowledge.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbMinBitsKnowledge.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbMinBitsKnowledge.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbMaxBitsKnowledge_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Templates.Human.Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend =
                    byte.Parse(tbMaxBitsKnowledge.Text, CultureInfo.InvariantCulture);
                OrganizationEntity.Templates.Email.Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend =
                    byte.Parse(tbMaxBitsKnowledge.Text, CultureInfo.InvariantCulture);
                tbMaxBitsKnowledge.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbMaxBitsKnowledge.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbMaxBitsKnowledge.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbMaxRateLearnable_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Templates.Email.MaxRateLearnable =
                    float.Parse(tbMaxRateLearnable.Text, CultureInfo.InvariantCulture);
                tbMaxRateLearnable.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbMaxRateLearnable.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbMaxRateLearnable.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbForgettingAgentRate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Models.Forgetting.RateOfAgentsOn =
                    float.Parse(tbForgettingAgentRate.Text, CultureInfo.InvariantCulture);
                tbForgettingAgentRate.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbForgettingAgentRate.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbForgettingAgentRate.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbForgettingMean_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Templates.Human.Cognitive.InternalCharacteristics.ForgettingMean =
                    float.Parse(tbForgettingMean.Text, CultureInfo.InvariantCulture);
                tbForgettingMean.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbForgettingMean.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbForgettingMean.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbTimeToLive_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Templates.Human.Cognitive.InternalCharacteristics.TimeToLive =
                    short.Parse(tbTimeToLive.Text, CultureInfo.InvariantCulture);
                tbTimeToLive.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbTimeToLive.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbTimeToLive.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbPartialForgettingRate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Templates.Human.Cognitive.InternalCharacteristics.PartialForgettingRate =
                    float.Parse(tbPartialForgettingRate.Text, CultureInfo.InvariantCulture);
                tbPartialForgettingRate.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbPartialForgettingRate.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbPartialForgettingRate.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbMinimRemainingLevel_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Templates.Human.Cognitive.InternalCharacteristics.MinimumRemainingKnowledge =
                    float.Parse(tbMinimRemainningLevel.Text, CultureInfo.InvariantCulture);
                tbMinimRemainningLevel.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbMinimRemainningLevel.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                tbMinimRemainningLevel.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbKnowledgeLength_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var value = byte.Parse(tbKnowledgeLength.Text);
                if (value > Bits.MaxBits)
                {
                    throw new ArgumentOutOfRangeException("Knowledge should be < " + Bits.MaxBits);
                }

                _environment.Knowledge = new Knowledge(1, "1", value);
                tbKnowledgeLength.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbKnowledgeLength.BackColor = Color.Red;
            }
            catch (OverflowException exception)
            {
                tbKnowledgeLength.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbSteps_TextChanged(object sender, EventArgs e)
        {
            try
            {
                byte.Parse(tbSteps.Text);
                tbSteps.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                tbSteps.BackColor = Color.Red;
            }
            catch (OverflowException exception)
            {
                tbSteps.BackColor = Color.Red;
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