#region Licence

// Description: Symu - SymuLearnAndForget
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using SymuEngine.Classes.Agent.Models.CognitiveArchitecture;
using SymuEngine.Classes.Scenario;
using SymuEngine.Common;
using SymuEngine.Engine.Form;
using SymuEngine.Environment.TimeStep;
using SymuEngine.Repository.Networks.Databases;
using SymuEngine.Repository.Networks.Knowledges;
using SymuLearnAndForget.Classes;
using SymuTools.Classes.Algorithm;

#endregion

namespace SymuLearnAndForget
{
    public partial class Home : SymuForm
    {
        private readonly ExampleEnvironment _environment = new ExampleEnvironment();
        private ushort _fullKnowledge;
        private Database _wiki;

        public Home()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            DisplayButtons();
            // Models
            OrganizationEntity.OrganizationModels.Learning.On = true;
            OrganizationEntity.OrganizationModels.Learning.RateOfAgentsOn = 1;
            OrganizationEntity.OrganizationModels.Forgetting.On = true;
            OrganizationEntity.OrganizationModels.Forgetting.RateOfAgentsOn = 1;

            #region Knowledge

            tbKnowledgeLength.Text = "50";
            cbBinaryKnowledge.Checked =
                OrganizationEntity.OrganizationModels.KnowledgeGenerator == RandomGenerator.RandomBinary;

            #endregion

            #region Learning

            cbLearningOn.Checked = OrganizationEntity.OrganizationModels.Learning.On;
            tbMicroLearningAgentRate.Text =
                OrganizationEntity.OrganizationModels.Learning.RateOfAgentsOn.ToString(CultureInfo.InvariantCulture);
            cbHasInitialKnowledge.Checked = OrganizationEntity.Templates.SimpleHuman.Cognitive.KnowledgeAndBeliefs
                .HasInitialKnowledge;
            cbInitialKnowledgeLevel.Items.AddRange(KnowledgeLevelService.GetNames());
            cbInitialKnowledgeLevel.SelectedItem = KnowledgeLevelService.GetName(_environment.KnowledgeLevel);
            cbHasKnowledge.Checked =
                OrganizationEntity.Templates.SimpleHuman.Cognitive.KnowledgeAndBeliefs.HasKnowledge;
            tbKnowledgeThreshold.Text =
                OrganizationEntity.Templates.SimpleHuman.Cognitive.KnowledgeAndBeliefs.KnowledgeThreshHoldForDoing
                    .ToString(CultureInfo.InvariantCulture);
            tbTimeToLive.Text =
                OrganizationEntity.Templates.SimpleHuman.Cognitive.InternalCharacteristics.TimeToLive.ToString(
                    CultureInfo.InvariantCulture);
            tbLearnRate.Text =
                OrganizationEntity.Templates.SimpleHuman.Cognitive.TasksAndPerformance.LearningRate.ToString(CultureInfo
                    .InvariantCulture);
            tbLearnByDoingRate.Text =
                OrganizationEntity.Templates.SimpleHuman.Cognitive.TasksAndPerformance.LearningByDoingRate.ToString(
                    CultureInfo.InvariantCulture);
            tbTaskCostFactor.Text =
                OrganizationEntity.Templates.SimpleHuman.Cognitive.TasksAndPerformance.CostFactorOfLearningByDoing
                    .ToString(CultureInfo.InvariantCulture);
            cbCanSendKnowledge.Checked =
                OrganizationEntity.Templates.SimpleHuman.Cognitive.MessageContent.CanSendKnowledge;
            cbCanReceiveKnowledge.Checked =
                OrganizationEntity.Templates.SimpleHuman.Cognitive.MessageContent.CanReceiveKnowledge;
            // Email
            tbMinKnowledge.Text =
                OrganizationEntity.Templates.SimpleHuman.Cognitive.MessageContent.MinimumKnowledgeToSendPerBit.ToString(
                    CultureInfo.InvariantCulture);
            tbMinBitsKnowledge.Text =
                OrganizationEntity.Templates.SimpleHuman.Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend
                    .ToString(CultureInfo.InvariantCulture);
            tbMaxBitsKnowledge.Text =
                OrganizationEntity.Templates.SimpleHuman.Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend
                    .ToString(CultureInfo.InvariantCulture);
            tbMaxRateLearnable.Text =
                OrganizationEntity.Templates.Email.MaxRateLearnable.ToString(CultureInfo.InvariantCulture);

            #endregion

            #region Forgetting

            cbForgettingOn.Checked = OrganizationEntity.OrganizationModels.Forgetting.On;
            tbForgettingAgentRate.Text =
                OrganizationEntity.OrganizationModels.Forgetting.RateOfAgentsOn.ToString(CultureInfo.InvariantCulture);
            tbForgettingMean.Text =
                OrganizationEntity.Templates.SimpleHuman.Cognitive.InternalCharacteristics.ForgettingMean.ToString(
                    CultureInfo.InvariantCulture);
            cgForgettingStdDev.Items.AddRange(GenericLevelService.GetNames());
            cgForgettingStdDev.SelectedItem =
                GenericLevelService.GetName(OrganizationEntity.Templates.SimpleHuman.Cognitive.InternalCharacteristics
                    .ForgettingStandardDeviation);
            cbPartialForgetting.Checked = OrganizationEntity.Templates.SimpleHuman.Cognitive.InternalCharacteristics
                .PartialForgetting;
            tbPartialForgettingRate.Text =
                OrganizationEntity.Templates.SimpleHuman.Cognitive.InternalCharacteristics.PartialForgettingRate
                    .ToString(CultureInfo.InvariantCulture);
            tbMinimRemainningLevel.Text =
                OrganizationEntity.Templates.SimpleHuman.Cognitive.InternalCharacteristics.MinimumRemainingKnowledge
                    .ToString(CultureInfo.InvariantCulture);
            switch (OrganizationEntity.Templates.SimpleHuman.Cognitive.InternalCharacteristics.ForgettingSelectingMode)
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

        protected override void SetUpOrganization()
        {
            SetRandomLevel(cbRandomLevel.SelectedIndex);
            TimeStepType = TimeStepType.Daily;
            _wiki = new Database(OrganizationEntity.Id.Key,
                OrganizationEntity.Templates.Email.Cognitive.TasksAndPerformance, -1);
            _wiki.InitializeKnowledge(_environment.Knowledge, 0);
            OrganizationEntity.AddDatabase(_wiki);
            OrganizationEntity.OrganizationModels.FollowGroupKnowledge = true;
        }

        protected override void SetScenarii()
        {
            _ = new TimeStepScenario(OrganizationEntity.NextEntityIndex(), _environment)
            {
                NumberOfSteps = ushort.Parse(tbSteps.Text)
            };
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            #region Knowledge

            _environment.Knowledge = new Knowledge(1, "1", byte.Parse(tbKnowledgeLength.Text));
            OrganizationEntity.OrganizationModels.KnowledgeGenerator = cbBinaryKnowledge.Checked
                ? RandomGenerator.RandomBinary
                : RandomGenerator.RandomUniform;

            #endregion

            #region Learning

            OrganizationEntity.OrganizationModels.Learning.On = cbLearningOn.Checked;
            OrganizationEntity.OrganizationModels.Learning.RateOfAgentsOn =
                float.Parse(tbMicroLearningAgentRate.Text, CultureInfo.InvariantCulture);
            OrganizationEntity.Templates.SimpleHuman.Cognitive.KnowledgeAndBeliefs.HasKnowledge =
                cbHasKnowledge.Checked;
            OrganizationEntity.Templates.SimpleHuman.Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge =
                cbHasInitialKnowledge.Checked;
            _environment.KnowledgeLevel =
                KnowledgeLevelService.GetValue(cbInitialKnowledgeLevel.SelectedItem.ToString());
            OrganizationEntity.Templates.SimpleHuman.Cognitive.KnowledgeAndBeliefs.KnowledgeThreshHoldForDoing =
                float.Parse(tbKnowledgeThreshold.Text, CultureInfo.InvariantCulture);
            OrganizationEntity.Templates.SimpleHuman.Cognitive.InternalCharacteristics.TimeToLive =
                short.Parse(tbTimeToLive.Text, CultureInfo.InvariantCulture);

            OrganizationEntity.Templates.SimpleHuman.Cognitive.TasksAndPerformance.LearningRate =
                float.Parse(tbLearnRate.Text, CultureInfo.InvariantCulture);
            OrganizationEntity.Templates.SimpleHuman.Cognitive.TasksAndPerformance.LearningByDoingRate =
                float.Parse(tbLearnByDoingRate.Text, CultureInfo.InvariantCulture);
            OrganizationEntity.Templates.SimpleHuman.Cognitive.TasksAndPerformance.CostFactorOfLearningByDoing =
                float.Parse(tbTaskCostFactor.Text, CultureInfo.InvariantCulture);
            OrganizationEntity.Templates.SimpleHuman.Cognitive.MessageContent.CanSendKnowledge =
                cbCanSendKnowledge.Checked;
            OrganizationEntity.Templates.SimpleHuman.Cognitive.MessageContent.CanReceiveKnowledge =
                cbCanReceiveKnowledge.Checked;
            OrganizationEntity.Templates.SimpleHuman.Cognitive.MessageContent.MinimumKnowledgeToSendPerBit =
                float.Parse(tbMinKnowledge.Text, CultureInfo.InvariantCulture);
            OrganizationEntity.Templates.SimpleHuman.Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend =
                byte.Parse(tbMinBitsKnowledge.Text, CultureInfo.InvariantCulture);
            OrganizationEntity.Templates.SimpleHuman.Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend =
                byte.Parse(tbMaxBitsKnowledge.Text, CultureInfo.InvariantCulture);
            // In this example:
            // we use email, but we can use other communications medium
            // we set the email template with the same values of SimpleHuman Template, but it could be different
            OrganizationEntity.Templates.Email.MaxRateLearnable =
                float.Parse(tbMaxRateLearnable.Text, CultureInfo.InvariantCulture);
            OrganizationEntity.Templates.Email.Cognitive.MessageContent.CanSendKnowledge = cbCanSendKnowledge.Checked;
            OrganizationEntity.Templates.Email.Cognitive.MessageContent.CanReceiveKnowledge =
                cbCanReceiveKnowledge.Checked;
            OrganizationEntity.Templates.Email.Cognitive.MessageContent.MinimumKnowledgeToSendPerBit =
                float.Parse(tbMinKnowledge.Text, CultureInfo.InvariantCulture);
            OrganizationEntity.Templates.Email.Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend =
                byte.Parse(tbMinBitsKnowledge.Text, CultureInfo.InvariantCulture);
            OrganizationEntity.Templates.Email.Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend =
                byte.Parse(tbMaxBitsKnowledge.Text, CultureInfo.InvariantCulture);

            #endregion

            #region Forgetting

            OrganizationEntity.OrganizationModels.Forgetting.On = cbForgettingOn.Checked;
            OrganizationEntity.OrganizationModels.Forgetting.RateOfAgentsOn =
                float.Parse(tbForgettingAgentRate.Text, CultureInfo.InvariantCulture);
            OrganizationEntity.Templates.SimpleHuman.Cognitive.InternalCharacteristics.ForgettingMean =
                float.Parse(tbForgettingMean.Text, CultureInfo.InvariantCulture);
            OrganizationEntity.Templates.SimpleHuman.Cognitive.InternalCharacteristics.ForgettingStandardDeviation =
                GenericLevelService.GetValue(cgForgettingStdDev.SelectedItem.ToString());
            OrganizationEntity.Templates.SimpleHuman.Cognitive.InternalCharacteristics.PartialForgetting =
                cbPartialForgetting.Checked;
            OrganizationEntity.Templates.SimpleHuman.Cognitive.InternalCharacteristics.PartialForgettingRate =
                float.Parse(tbPartialForgettingRate.Text, CultureInfo.InvariantCulture);
            OrganizationEntity.Templates.SimpleHuman.Cognitive.InternalCharacteristics.MinimumRemainingKnowledge =
                float.Parse(tbMinimRemainningLevel.Text, CultureInfo.InvariantCulture);
            OrganizationEntity.Templates.SimpleHuman.Cognitive.InternalCharacteristics.ForgettingSelectingMode =
                rbICForgettingSelectingRandomMode.Checked
                    ? ForgettingSelectingMode.Random
                    : ForgettingSelectingMode.Oldest;

            #endregion

            _fullKnowledge = 0;
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

        public override void Display()
        {
            DisplayButtons();
            WriteTextSafe(TimeStep, _environment.TimeStep.Step.ToString());
            // 1st Agent
            WriteTextSafe(lblKnowledge1,
                _environment.LearnFromSourceAgent.Cognitive.KnowledgeAndBeliefs.Expertise.GetKnowledgesSum()
                    .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(lblLearning1,
                _environment.LearnFromSourceAgent.Cognitive.KnowledgeAndBeliefs.Expertise.Learning.ToString("F1",
                    CultureInfo.InvariantCulture));
            WriteTextSafe(lblForgetting1,
                _environment.LearnFromSourceAgent.Cognitive.KnowledgeAndBeliefs.Expertise.Forgetting.ToString("F1",
                    CultureInfo.InvariantCulture));
            // 2nd Agent
            WriteTextSafe(lblKnowledge2,
                _environment.LearnByDoingAgent.Cognitive.KnowledgeAndBeliefs.Expertise.GetKnowledgesSum()
                    .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(lblLearning2,
                _environment.LearnByDoingAgent.Cognitive.KnowledgeAndBeliefs.Expertise.Learning.ToString("F1",
                    CultureInfo.InvariantCulture));
            WriteTextSafe(lblForgetting2,
                _environment.LearnByDoingAgent.Cognitive.KnowledgeAndBeliefs.Expertise.Forgetting.ToString("F1",
                    CultureInfo.InvariantCulture));
            // 3rd Agent
            WriteTextSafe(lblKnowledge3,
                _environment.LearnByAskingAgent.Cognitive.KnowledgeAndBeliefs.Expertise.GetKnowledgesSum()
                    .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(lblLearning3,
                _environment.LearnByAskingAgent.Cognitive.KnowledgeAndBeliefs.Expertise.Learning.ToString("F1",
                    CultureInfo.InvariantCulture));
            WriteTextSafe(lblForgetting3,
                _environment.LearnByAskingAgent.Cognitive.KnowledgeAndBeliefs.Expertise.Forgetting.ToString("F1",
                    CultureInfo.InvariantCulture));
            // 4th Agent
            WriteTextSafe(lblKnowledge4,
                _environment.DoesNotLearnAgent.Cognitive.KnowledgeAndBeliefs.Expertise.GetKnowledgesSum()
                    .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(lblLearning4,
                _environment.DoesNotLearnAgent.Cognitive.KnowledgeAndBeliefs.Expertise.Learning.ToString("F1",
                    CultureInfo.InvariantCulture));
            WriteTextSafe(lblForgetting4,
                _environment.DoesNotLearnAgent.Cognitive.KnowledgeAndBeliefs.Expertise.Forgetting.ToString("F1",
                    CultureInfo.InvariantCulture));
            // Expert Agent
            WriteTextSafe(lblExpertKnowledge,
                _environment.ExpertAgent.Cognitive.KnowledgeAndBeliefs.Expertise.GetKnowledgesSum()
                    .ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(lblExpertLearning,
                _environment.ExpertAgent.Cognitive.KnowledgeAndBeliefs.Expertise.Learning.ToString("F1",
                    CultureInfo.InvariantCulture));
            WriteTextSafe(lblExpertForgetting,
                _environment.ExpertAgent.Cognitive.KnowledgeAndBeliefs.Expertise.Forgetting.ToString("F1",
                    CultureInfo.InvariantCulture));
            // Wiki
            WriteTextSafe(lblWiki, _wiki.GetKnowledgesSum().ToString("F1", CultureInfo.InvariantCulture));
            if (_fullKnowledge == 0 &&
                Math.Abs(_wiki.GetKnowledgesSum() - _environment.Knowledge.Length) < Constants.Tolerance)
            {
                _fullKnowledge = _environment.TimeStep.Step;
            }

            WriteTextSafe(lblFullKnowledge, _fullKnowledge.ToString(CultureInfo.InvariantCulture));

            // Global Knowledge - using iteration result
            if (!_environment.IterationResult.OrganizationKnowledgeAndBelief.Knowledges.Any())
            {
                return;
            }

            var global = _environment.IterationResult.OrganizationKnowledgeAndBelief.Knowledges.Last();
            WriteTextSafe(lblGlobalObsolescence, global.Obsolescence.ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(lblGlobalLearning, global.Learning.ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(lblGlobalForgetting, global.Forgetting.ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(lblGlobalKnowledge, global.Sum.ToString("F1", CultureInfo.InvariantCulture));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Pause();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Resume();
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

        private void tbMicroLearningAgentRate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.OrganizationModels.Learning.RateOfAgentsOn =
                    float.Parse(tbMicroLearningAgentRate.Text, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
            }
            catch (ArgumentOutOfRangeException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        #region Nested type: SafeCallButtonDelegate

        protected delegate void SafeCallButtonDelegate(Button button, bool enabled);

        #endregion
    }
}