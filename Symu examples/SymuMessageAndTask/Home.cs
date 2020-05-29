#region Licence

// Description: Symu - SymuMessageAndTask
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Symu.Classes.Scenario;
using Symu.Common;
using Symu.Environment;
using SymuForm;
using SymuMessageAndTask.Classes;

#endregion

namespace SymuMessageAndTask
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

            #region Environment

            InitialCapacity.Text = _environment.InitialCapacity.ToString(CultureInfo.InvariantCulture);
            SwitchingContextCost.Text = _environment.SwitchingContextCost.ToString(CultureInfo.InvariantCulture);
            costOfTask.Text = _environment.CostOfTask.ToString(CultureInfo.InvariantCulture);
            numberTasksSent.Text = _environment.NumberOfTasks.ToString(CultureInfo.InvariantCulture);

            #endregion

            #region Task model

            CanPerformTask.Checked =
                OrganizationEntity.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask;
            CanPerformTasksOnWeekends.Checked = OrganizationEntity.Templates.Human.Cognitive.TasksAndPerformance
                .CanPerformTaskOnWeekEnds;
            LimitNumberTasks.Checked = OrganizationEntity.Templates.Human.Cognitive.TasksAndPerformance.TasksLimit
                .LimitTasksInTotal;
            maxNumberTasks.Text =
                OrganizationEntity.Templates.Human.Cognitive.TasksAndPerformance.TasksLimit.MaximumTasksInTotal
                    .ToString(CultureInfo.InvariantCulture);
            LimitSimultaneousTasks.Checked = OrganizationEntity.Templates.Human.Cognitive.TasksAndPerformance
                .TasksLimit
                .LimitSimultaneousTasks;
            MaxSimultaneousTasks.Text =
                OrganizationEntity.Templates.Human.Cognitive.TasksAndPerformance.TasksLimit
                    .MaximumSimultaneousTasks
                    .ToString(CultureInfo.InvariantCulture);
            AgentCanBeIsolated.Items.AddRange(FrequencyLevelService.GetNames());
            AgentCanBeIsolated.SelectedItem = FrequencyLevelService.GetName(OrganizationEntity.Templates.Human
                .Cognitive.InteractionPatterns.AgentCanBeIsolated);

            #endregion

            #region Message model

            LimitMessages.Checked = OrganizationEntity.Templates.Human.Cognitive.InteractionCharacteristics
                .LimitMessagesPerPeriod;
            MaxMessages.Text =
                OrganizationEntity.Templates.Human.Cognitive.InteractionCharacteristics.MaximumMessagesPerPeriod
                    .ToString(CultureInfo.InvariantCulture);
            LimitMessagesSent.Checked = OrganizationEntity.Templates.Human.Cognitive.InteractionCharacteristics
                .LimitMessagesSentPerPeriod;
            MaxMessagesSent.Text =
                OrganizationEntity.Templates.Human.Cognitive.InteractionCharacteristics
                    .MaximumMessagesSentPerPeriod
                    .ToString(CultureInfo.InvariantCulture);
            LimitMessagesReceived.Checked = OrganizationEntity.Templates.Human.Cognitive
                .InteractionCharacteristics.LimitReceptionsPerPeriod;
            MaxMessagesReceived.Text =
                OrganizationEntity.Templates.Human.Cognitive.InteractionCharacteristics.MaximumReceptionsPerPeriod
                    .ToString(CultureInfo.InvariantCulture);

            CostToSend.Items.AddRange(GenericLevelService.GetNames());
            CostToSend.Text =
                GenericLevelService.GetName(OrganizationEntity.Templates.Email.CostToSendLevel);
            CostToReceive.Items.AddRange(GenericLevelService.GetNames());
            CostToReceive.Text =
                GenericLevelService.GetName(OrganizationEntity.Templates.Email.CostToReceiveLevel);

            #endregion
        }

        protected override void UpdateSettings()
        {
            #region Task model

            OrganizationEntity.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTask =
                CanPerformTask.Checked;
            OrganizationEntity.Templates.Human.Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds =
                CanPerformTasksOnWeekends.Checked;
            OrganizationEntity.Templates.Human.Cognitive.TasksAndPerformance.TasksLimit
                .LimitTasksInTotal = LimitNumberTasks.Checked;
            OrganizationEntity.Templates.Human.Cognitive.TasksAndPerformance.TasksLimit
                .LimitSimultaneousTasks = LimitSimultaneousTasks.Checked;

            OrganizationEntity.Templates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated =
                FrequencyLevelService.GetValue(AgentCanBeIsolated.SelectedItem.ToString());
            try
            {
                _environment.InitialCapacity = float.Parse(InitialCapacity.Text);
                InitialCapacity.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                InitialCapacity.BackColor = Color.Red;
            }

            #endregion

            #region message

            OrganizationEntity.Templates.Human.Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod =
                LimitMessages.Checked;
            OrganizationEntity.Templates.Human.Cognitive.InteractionCharacteristics.LimitMessagesSentPerPeriod =
                LimitMessagesSent.Checked;
            OrganizationEntity.Templates.Human.Cognitive.InteractionCharacteristics.LimitReceptionsPerPeriod =
                LimitMessagesReceived.Checked;

            OrganizationEntity.Templates.Email.CostToSendLevel =
                GenericLevelService.GetValue(CostToSend.SelectedItem.ToString());
            OrganizationEntity.Templates.Email.CostToReceiveLevel =
                GenericLevelService.GetValue(CostToReceive.SelectedItem.ToString());

            #endregion

            SetRandomLevel(cbRandomLevel.SelectedIndex);
            SetTimeStepType(TimeStepType.Daily);
        }

        protected override void SetScenarii()
        {
            _ = new TimeBasedScenario(_environment)
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
            WriteTextSafe(TimeStep, _environment.Schedule.Step.ToString());
            UpDateMessages();
            UpdateAgents();
        }

        private void UpDateMessages()
        {
            WriteTextSafe(lblMessagesSent, _environment.Messages.SentMessagesCount.ToString());
        }

        private void UpdateAgents()
        {
            WriteTextSafe(lblWorked,
                _environment.IterationResult.Capacity.ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(TasksTotal, _environment.IterationResult.Tasks.Total.ToString(CultureInfo.InvariantCulture));
            WriteTextSafe(TasksToDo,
                _environment.IterationResult.Tasks.AverageToDo.ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(TasksInProgress,
                _environment.IterationResult.Tasks.AverageInProgress.ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(TasksDone,
                _environment.IterationResult.Tasks.AverageDone.ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(TasksWeight,
                _environment.IterationResult.Tasks.Weight.ToString("F1", CultureInfo.InvariantCulture));
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

        private void maxNumberTasks_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Templates.Human.Cognitive.TasksAndPerformance.TasksLimit.MaximumTasksInTotal =
                    ushort.Parse(maxNumberTasks.Text, CultureInfo.InvariantCulture);
                maxNumberTasks.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                maxNumberTasks.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                maxNumberTasks.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void MaxSimultaneousTasks_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Templates.Human.Cognitive.TasksAndPerformance.TasksLimit
                        .MaximumSimultaneousTasks =
                    byte.Parse(MaxSimultaneousTasks.Text, CultureInfo.InvariantCulture);
                MaxSimultaneousTasks.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                MaxSimultaneousTasks.BackColor = Color.Red;
            }
            catch (OverflowException exception)
            {
                MaxSimultaneousTasks.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                MaxSimultaneousTasks.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void MaxMessages_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Templates.Human.Cognitive.InteractionCharacteristics.MaximumMessagesPerPeriod =
                    byte.Parse(MaxMessages.Text, CultureInfo.InvariantCulture);
                MaxMessages.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                MaxMessages.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                MaxMessages.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void MaxMessagesSent_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Templates.Human.Cognitive.InteractionCharacteristics
                        .MaximumMessagesSentPerPeriod =
                    byte.Parse(MaxMessagesSent.Text, CultureInfo.InvariantCulture);
                MaxMessagesSent.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                MaxMessagesSent.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                MaxMessagesSent.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void MaxMessagesReceived_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.Templates.Human.Cognitive.InteractionCharacteristics
                        .MaximumReceptionsPerPeriod =
                    byte.Parse(MaxMessagesReceived.Text, CultureInfo.InvariantCulture);
                MaxMessagesReceived.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                MaxMessagesReceived.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                MaxMessagesReceived.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void numberTasksSent_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _environment.NumberOfTasks = int.Parse(numberTasksSent.Text);
                numberTasksSent.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                numberTasksSent.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                numberTasksSent.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void costOfTask_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _environment.CostOfTask = float.Parse(costOfTask.Text);
                costOfTask.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                costOfTask.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                costOfTask.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void SwitchingContextCost_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _environment.SwitchingContextCost = float.Parse(SwitchingContextCost.Text);
                SwitchingContextCost.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                SwitchingContextCost.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                SwitchingContextCost.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }
        }

        private void tbWorkers_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _environment.WorkersCount = int.Parse(tbWorkers.Text);
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
    }
}