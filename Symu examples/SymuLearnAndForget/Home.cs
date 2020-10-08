#region Licence

// Description: SymuBiz - SymuLearnAndForget
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
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Classes.Scenario;
using Symu.Common;
using Symu.Common.Classes;
using Symu.Forms;
using Symu.Repository.Entities;
using SymuLearnAndForget.Classes;
using static Symu.Common.Constants;

#endregion

namespace SymuLearnAndForget
{
    public partial class Home : SymuForm
    {
        private readonly ExampleEnvironment _environment = new ExampleEnvironment();
        private readonly ExampleMainOrganization _mainOrganization = new ExampleMainOrganization();
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
                _mainOrganization.Models.Generator == RandomGenerator.RandomBinary;

            #endregion

            #region Learning

            cbLearningOn.Checked = true;
            tbMicroLearningAgentRate.Text = "1";
            cbHasInitialKnowledge.Checked = _mainOrganization.Templates.Human.Cognitive.KnowledgeAndBeliefs
                .HasInitialKnowledge;
            cbInitialKnowledgeLevel.Items.AddRange(KnowledgeLevelService.GetNames());
            cbInitialKnowledgeLevel.SelectedItem = KnowledgeLevelService.GetName(_mainOrganization.KnowledgeLevel);
            cbHasKnowledge.Checked =
                _mainOrganization.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasKnowledge;
            tbKnowledgeThreshold.Text =
                _mainOrganization.Murphies.IncompleteKnowledge.ThresholdForReacting
                    .ToString(CultureInfo.InvariantCulture);
            tbTimeToLive.Text =
                _mainOrganization.Templates.Human.Cognitive.InternalCharacteristics.TimeToLive.ToString(
                    CultureInfo.InvariantCulture);
            tbLearnRate.Text =
                _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.LearningRate.ToString(CultureInfo
                    .InvariantCulture);
            tbLearnByDoingRate.Text =
                _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.LearningByDoingRate.ToString(
                    CultureInfo.InvariantCulture);
            tbTaskCostFactor.Text =
                _mainOrganization.Murphies.IncompleteKnowledge.CostFactorOfGuessing
                    .ToString(CultureInfo.InvariantCulture);
            cbCanSendKnowledge.Checked =
                _mainOrganization.Templates.Human.Cognitive.MessageContent.CanSendKnowledge;
            cbCanReceiveKnowledge.Checked =
                _mainOrganization.Templates.Human.Cognitive.MessageContent.CanReceiveKnowledge;
            // Email
            tbMinKnowledge.Text =
                _mainOrganization.Templates.Human.Cognitive.MessageContent.MinimumKnowledgeToSendPerBit.ToString(
                    CultureInfo.InvariantCulture);
            tbMinBitsKnowledge.Text =
                _mainOrganization.Templates.Human.Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend
                    .ToString(CultureInfo.InvariantCulture);
            tbMaxBitsKnowledge.Text =
                _mainOrganization.Templates.Human.Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend
                    .ToString(CultureInfo.InvariantCulture);
            tbMaxRateLearnable.Text =
                _mainOrganization.Communication.Email.MaxRateLearnable.ToString(CultureInfo.InvariantCulture);

            #endregion

            #region Forgetting

            cbForgettingOn.Checked = true;
            tbForgettingAgentRate.Text = "1";
            tbForgettingMean.Text =
                _mainOrganization.Templates.Human.Cognitive.InternalCharacteristics.ForgettingMean.ToString(
                    CultureInfo.InvariantCulture);
            cgForgettingStdDev.Items.AddRange(GenericLevelService.GetNames());
            cgForgettingStdDev.SelectedItem =
                GenericLevelService.GetName(_mainOrganization.Templates.Human.Cognitive.InternalCharacteristics
                    .ForgettingStandardDeviation);
            cbPartialForgetting.Checked = _mainOrganization.Templates.Human.Cognitive.InternalCharacteristics
                .PartialForgetting;
            tbPartialForgettingRate.Text =
                _mainOrganization.Templates.Human.Cognitive.InternalCharacteristics.PartialForgettingRate
                    .ToString(CultureInfo.InvariantCulture);
            tbMinimRemainningLevel.Text =
                _mainOrganization.Templates.Human.Cognitive.InternalCharacteristics.MinimumRemainingKnowledge
                    .ToString(CultureInfo.InvariantCulture);
            switch (_mainOrganization.Templates.Human.Cognitive.InternalCharacteristics.ForgettingSelectingMode)
            {
                case ForgettingSelectingMode.Random:
                    rbICForgettingSelectingRandomMode.Checked = true;
                    break;
                case ForgettingSelectingMode.Oldest:
                    rbICForgettingSelectingOldestMode.Checked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            #endregion
        }

        protected override void SetUpOrganization()
        {
            base.SetUpOrganization();

            #region Knowledge

            _mainOrganization.Models.Generator = cbBinaryKnowledge.Checked
                ? RandomGenerator.RandomBinary
                : RandomGenerator.RandomUniform;

            #endregion

            #region Learning

            _mainOrganization.Models.Learning.On = cbLearningOn.Checked;
            _mainOrganization.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasKnowledge =
                cbHasKnowledge.Checked;
            _mainOrganization.Templates.Human.Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge =
                cbHasInitialKnowledge.Checked;
            _mainOrganization.KnowledgeLevel =
                KnowledgeLevelService.GetValue(cbInitialKnowledgeLevel.SelectedItem.ToString());

            _mainOrganization.Templates.Human.Cognitive.MessageContent.CanSendKnowledge =
                cbCanSendKnowledge.Checked;
            _mainOrganization.Templates.Human.Cognitive.MessageContent.CanReceiveKnowledge =
                cbCanReceiveKnowledge.Checked;

            #endregion

            #region Forgetting

            _mainOrganization.Models.Forgetting.On = cbForgettingOn.Checked;
            _mainOrganization.Templates.Human.Cognitive.InternalCharacteristics.ForgettingStandardDeviation =
                GenericLevelService.GetValue(cgForgettingStdDev.SelectedItem.ToString());
            _mainOrganization.Templates.Human.Cognitive.InternalCharacteristics.PartialForgetting =
                cbPartialForgetting.Checked;
            _mainOrganization.Templates.Human.Cognitive.InternalCharacteristics.ForgettingSelectingMode =
                rbICForgettingSelectingRandomMode.Checked
                    ? ForgettingSelectingMode.Random
                    : ForgettingSelectingMode.Oldest;

            #endregion

            _fullKnowledge = 0;
            var scenario = TimeBasedScenario.CreateInstance(_environment);
            scenario.NumberOfSteps = ushort.Parse(tbSteps.Text, CultureInfo.InvariantCulture);
            AddScenario(scenario);

            SetRandomLevel(cbRandomLevel.SelectedIndex);
            _mainOrganization.AddWiki();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Start(_environment, _mainOrganization);
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
            WriteTextSafe(TimeStep, _environment.Schedule.Step.ToString(CultureInfo.InvariantCulture));
            // 1st Agent

            WriteTextSafe(lblKnowledge1,
                _environment.LearnFromSourceAgent.KnowledgeModel.PercentageKnowledge
                    .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(lblLearning1,
                _environment.LearnFromSourceAgent.LearningModel.PercentageLearning.ToString("F1",
                    CultureInfo.InvariantCulture));
            WriteTextSafe(lblForgetting1,
                _environment.LearnFromSourceAgent.ForgettingModel.PercentageForgetting.ToString("F1",
                    CultureInfo.InvariantCulture));
            // 2nd Agent
            WriteTextSafe(lblKnowledge2,
                _environment.LearnByDoingAgent.KnowledgeModel.PercentageKnowledge
                    .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(lblLearning2,
                _environment.LearnByDoingAgent.LearningModel.PercentageLearning.ToString("F1",
                    CultureInfo.InvariantCulture));
            WriteTextSafe(lblForgetting2,
                _environment.LearnByDoingAgent.ForgettingModel.PercentageForgetting.ToString("F1",
                    CultureInfo.InvariantCulture));
            // 3rd Agent
            WriteTextSafe(lblKnowledge3,
                _environment.LearnByAskingAgent.KnowledgeModel.PercentageKnowledge
                    .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(lblLearning3,
                _environment.LearnByAskingAgent.LearningModel.PercentageLearning.ToString("F1",
                    CultureInfo.InvariantCulture));
            WriteTextSafe(lblForgetting3,
                _environment.LearnByAskingAgent.ForgettingModel.PercentageForgetting.ToString("F1",
                    CultureInfo.InvariantCulture));
            // 4th Agent
            WriteTextSafe(lblKnowledge4,
                _environment.DoesNotLearnAgent.KnowledgeModel.PercentageKnowledge
                    .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(lblLearning4,
                _environment.DoesNotLearnAgent.LearningModel.PercentageLearning.ToString("F1",
                    CultureInfo.InvariantCulture));
            WriteTextSafe(lblForgetting4,
                _environment.DoesNotLearnAgent.ForgettingModel.PercentageForgetting.ToString("F1",
                    CultureInfo.InvariantCulture));
            // Expert Agent
            WriteTextSafe(lblExpertKnowledge,
                _environment.ExpertAgent.KnowledgeModel.PercentageKnowledge
                    .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(lblExpertLearning,
                _environment.ExpertAgent.LearningModel.PercentageLearning.ToString("F1",
                    CultureInfo.InvariantCulture));
            WriteTextSafe(lblExpertForgetting,
                _environment.ExpertAgent.ForgettingModel.PercentageForgetting.ToString("F1",
                    CultureInfo.InvariantCulture));
            // Wiki
            WriteTextSafe(lblWiki,
                _environment.ExampleMainOrganization.WikiEntity.GetKnowledgesSum()
                    .ToString("F1", CultureInfo.InvariantCulture));
            if (_fullKnowledge == 0 &&
                Math.Abs(_environment.ExampleMainOrganization.WikiEntity.GetKnowledgesSum() -
                         _environment.ExampleMainOrganization.Knowledge.Length) < Tolerance)
            {
                _fullKnowledge = _environment.Schedule.Step;
            }

            WriteTextSafe(lblFullKnowledge, _fullKnowledge.ToString(CultureInfo.InvariantCulture));

            // Global Knowledge - using iteration result

            lock (_environment.IterationResult.KnowledgeAndBeliefResults.Knowledge)
            {
                if (!_environment.IterationResult.KnowledgeAndBeliefResults.Knowledge.Any())
                {
                    return;
                }
            }


            var knowledge = _environment.IterationResult.KnowledgeAndBeliefResults.Knowledge.Last();
            WriteTextSafe(lblGlobalKnowledge, knowledge.Percentage.ToString("F1", CultureInfo.InvariantCulture));
            var obsolescence = _environment.IterationResult.KnowledgeAndBeliefResults.KnowledgeObsolescence.Last();
            WriteTextSafe(lblGlobalObsolescence, obsolescence.Percentage.ToString("F1", CultureInfo.InvariantCulture));
            var learning = _environment.IterationResult.KnowledgeAndBeliefResults.Learning.Last();
            WriteTextSafe(lblGlobalLearning, learning.Percentage.ToString("F1", CultureInfo.InvariantCulture));
            var forgetting = _environment.IterationResult.KnowledgeAndBeliefResults.Forgetting.Last();
            WriteTextSafe(lblGlobalForgetting, forgetting.Percentage.ToString("F1", CultureInfo.InvariantCulture));
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
                _mainOrganization.Models.Learning.RateOfAgentsOn =
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
                _mainOrganization.Murphies.IncompleteKnowledge.ThresholdForReacting =
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
                _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.LearningRate =
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
                _mainOrganization.Templates.Human.Cognitive.TasksAndPerformance.LearningByDoingRate =
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
                _mainOrganization.Murphies.IncompleteKnowledge.CostFactorOfGuessing =
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
                _mainOrganization.Templates.Human.Cognitive.MessageContent.MinimumKnowledgeToSendPerBit =
                    float.Parse(tbMinKnowledge.Text, CultureInfo.InvariantCulture);
                _mainOrganization.Communication.Email.MinimumKnowledgeToSendPerBit =
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
                _mainOrganization.Templates.Human.Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend =
                    byte.Parse(tbMinBitsKnowledge.Text, CultureInfo.InvariantCulture);
                _mainOrganization.Communication.Email.MinimumNumberOfBitsOfKnowledgeToSend =
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
                _mainOrganization.Templates.Human.Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend =
                    byte.Parse(tbMaxBitsKnowledge.Text, CultureInfo.InvariantCulture);
                _mainOrganization.Communication.Email.MaximumNumberOfBitsOfKnowledgeToSend =
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
                _mainOrganization.Communication.Email.MaxRateLearnable =
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
                _mainOrganization.Models.Forgetting.RateOfAgentsOn =
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
                _mainOrganization.Templates.Human.Cognitive.InternalCharacteristics.ForgettingMean =
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
                _mainOrganization.Templates.Human.Cognitive.InternalCharacteristics.TimeToLive =
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
                _mainOrganization.Templates.Human.Cognitive.InternalCharacteristics.PartialForgettingRate =
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
                _mainOrganization.Templates.Human.Cognitive.InternalCharacteristics.MinimumRemainingKnowledge =
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
                var value = byte.Parse(tbKnowledgeLength.Text, CultureInfo.InvariantCulture);
                if (value > Bits.MaxBits)
                {
                    throw new ArgumentOutOfRangeException("Knowledge should be < " + Bits.MaxBits);
                }

                Knowledge.CreateInstance(_mainOrganization.MetaNetwork, _mainOrganization.Models, "1", value);
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
                byte.Parse(tbSteps.Text, CultureInfo.InvariantCulture);
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