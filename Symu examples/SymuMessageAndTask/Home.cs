#region Licence

// Description: Symu - SymuMessageAndTask
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
using Symu.Common;
using Symu.Environment;
using Symu.Forms;
using SymuMessageAndTask.Classes;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Chart;

#endregion

namespace SymuMessageAndTask
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

            #region Environment

            InitialCapacity.Text = _environment.InitialCapacity.ToString(CultureInfo.InvariantCulture);
            SwitchingContextCost.Text = _environment.SwitchingContextCost.ToString(CultureInfo.InvariantCulture);
            costOfTask.Text = _environment.CostOfTask.ToString(CultureInfo.InvariantCulture);
            numberTasksSent.Text = _environment.NumberOfTasks.ToString(CultureInfo.InvariantCulture);

            #endregion

            #region Task model

            CanPerformTask.Checked =
                OrganizationEntity.AgentTemplates.Human.Cognitive.TasksAndPerformance.CanPerformTask;
            CanPerformTasksOnWeekends.Checked = OrganizationEntity.AgentTemplates.Human.Cognitive.TasksAndPerformance
                .CanPerformTaskOnWeekEnds;
            LimitNumberTasks.Checked = OrganizationEntity.AgentTemplates.Human.Cognitive.TasksAndPerformance.TasksLimit
                .LimitTasksInTotal;
            maxNumberTasks.Text =
                OrganizationEntity.AgentTemplates.Human.Cognitive.TasksAndPerformance.TasksLimit.MaximumTasksInTotal
                    .ToString(CultureInfo.InvariantCulture);
            LimitSimultaneousTasks.Checked = OrganizationEntity.AgentTemplates.Human.Cognitive.TasksAndPerformance
                .TasksLimit
                .LimitSimultaneousTasks;
            MaxSimultaneousTasks.Text =
                OrganizationEntity.AgentTemplates.Human.Cognitive.TasksAndPerformance.TasksLimit
                    .MaximumSimultaneousTasks
                    .ToString(CultureInfo.InvariantCulture);
            AgentCanBeIsolated.Items.AddRange(FrequencyLevelService.GetNames());
            AgentCanBeIsolated.SelectedItem = FrequencyLevelService.GetName(OrganizationEntity.AgentTemplates.Human
                .Cognitive.InteractionPatterns.AgentCanBeIsolated);

            #endregion

            #region Message model

            LimitMessages.Checked = OrganizationEntity.AgentTemplates.Human.Cognitive.InteractionCharacteristics
                .LimitMessagesPerPeriod;
            MaxMessages.Text =
                OrganizationEntity.AgentTemplates.Human.Cognitive.InteractionCharacteristics.MaximumMessagesPerPeriod
                    .ToString(CultureInfo.InvariantCulture);
            LimitMessagesSent.Checked = OrganizationEntity.AgentTemplates.Human.Cognitive.InteractionCharacteristics
                .LimitMessagesSentPerPeriod;
            MaxMessagesSent.Text =
                OrganizationEntity.AgentTemplates.Human.Cognitive.InteractionCharacteristics
                    .MaximumMessagesSentPerPeriod
                    .ToString(CultureInfo.InvariantCulture);
            LimitMessagesReceived.Checked = OrganizationEntity.AgentTemplates.Human.Cognitive
                .InteractionCharacteristics.LimitReceptionsPerPeriod;
            MaxMessagesReceived.Text =
                OrganizationEntity.AgentTemplates.Human.Cognitive.InteractionCharacteristics.MaximumReceptionsPerPeriod
                    .ToString(CultureInfo.InvariantCulture);

            CostToSend.Items.AddRange(GenericLevelService.GetNames());
            CostToSend.Text =
                GenericLevelService.GetName(OrganizationEntity.Communication.Email.CostToSendLevel);
            CostToReceive.Items.AddRange(GenericLevelService.GetNames());
            CostToReceive.Text =
                GenericLevelService.GetName(OrganizationEntity.Communication.Email.CostToReceiveLevel);

            #endregion
        }

        protected override void UpdateSettings()
        {
            base.UpdateSettings();
            Iterations.Max = ushort.Parse(NumberOfIterations.Text, CultureInfo.InvariantCulture);

            #region Task model

            OrganizationEntity.AgentTemplates.Human.Cognitive.TasksAndPerformance.CanPerformTask =
                CanPerformTask.Checked;
            OrganizationEntity.AgentTemplates.Human.Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds =
                CanPerformTasksOnWeekends.Checked;
            OrganizationEntity.AgentTemplates.Human.Cognitive.TasksAndPerformance.TasksLimit
                .LimitTasksInTotal = LimitNumberTasks.Checked;
            OrganizationEntity.AgentTemplates.Human.Cognitive.TasksAndPerformance.TasksLimit
                .LimitSimultaneousTasks = LimitSimultaneousTasks.Checked;

            OrganizationEntity.AgentTemplates.Human.Cognitive.InteractionPatterns.AgentCanBeIsolated =
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

            OrganizationEntity.AgentTemplates.Human.Cognitive.InteractionCharacteristics.LimitMessagesPerPeriod =
                LimitMessages.Checked;
            OrganizationEntity.AgentTemplates.Human.Cognitive.InteractionCharacteristics.LimitMessagesSentPerPeriod =
                LimitMessagesSent.Checked;
            OrganizationEntity.AgentTemplates.Human.Cognitive.InteractionCharacteristics.LimitReceptionsPerPeriod =
                LimitMessagesReceived.Checked;

            OrganizationEntity.Communication.Email.CostToSendLevel =
                GenericLevelService.GetValue(CostToSend.SelectedItem.ToString());
            OrganizationEntity.Communication.Email.CostToReceiveLevel =
                GenericLevelService.GetValue(CostToReceive.SelectedItem.ToString());

            #endregion

            var scenario = new TimeBasedScenario(_environment)
            {
                NumberOfSteps = ushort.Parse(tbSteps.Text, CultureInfo.InvariantCulture)
            };

            AddScenario(scenario);

            SetRandomLevel(cbRandomLevel.SelectedIndex);
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
            UpDateMessages();
            UpdateAgents();
        }

        private void UpDateMessages()
        {
            WriteTextSafe(lblMessagesSent, _environment.Messages.Result.SentMessagesCount.ToString(CultureInfo.InvariantCulture));
            WriteTextSafe(ReceivedMessages, _environment.Messages.Result.ReceivedMessagesCount.ToString(CultureInfo.InvariantCulture));
            WriteTextSafe(LostMessages, _environment.Messages.Result.LostMessagesCount.ToString(CultureInfo.InvariantCulture));
            WriteTextSafe(MissedMessages, _environment.Messages.Result.MissedMessagesCount.ToString(CultureInfo.InvariantCulture));
            WriteTextSafe(SentCost, _environment.Messages.Result.SentMessagesCost.ToString("F1", CultureInfo.InvariantCulture));
            WriteTextSafe(ReceivedCost, _environment.Messages.Result.ReceivedMessagesCost.ToString("F1", CultureInfo.InvariantCulture));
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
        public override void DisplayIteration()
        {
            WriteTextSafe(Iteration, Iterations.Number.ToString(CultureInfo.InvariantCulture));
            
            var tasksResults = SimulationResults.List.Select(x => x.Tasks.Done).ToList();
            var seriesTasks = new ChartSeries("tasks", ChartSeriesType.Histogram);
            foreach (var tasksResult in tasksResults)
            {
                seriesTasks.Points.Add(tasksResult, tasksResults.Count);
            }
            seriesTasks.Text = seriesTasks.Name;
            seriesTasks.ConfigItems.HistogramItem.NumberOfIntervals = 10;
            WriteChartSafe(chartControl1, new[] { seriesTasks});


        }

        protected void WriteChartSafe(ChartControl chartControl, ChartSeries[] chartSeries)
        {
            if (chartControl is null)
            {
                throw new ArgumentNullException(nameof(chartControl));
            }

            if (chartSeries == null)
            {
                throw new ArgumentNullException(nameof(chartSeries));
            }

            if (chartControl.InvokeRequired)
            {
                var d = new SafeCallChartDelegate(WriteChartSafe);
                chartControl.Invoke(d, chartControl, chartSeries);
            }
            else
            {
                chartControl.Series.Clear();
                foreach (var chartSerie in chartSeries)
                {
                    chartControl.Series.Add(chartSerie);
                }

                ChartAppearance.ApplyChartStyles(chartControl);
            }
        }

        #region Nested type: SafeCallChartDelegate

        protected delegate void SafeCallChartDelegate(ChartControl chartControl, ChartSeries[] chartSeries);

        #endregion

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

        private void maxNumberTasks_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OrganizationEntity.AgentTemplates.Human.Cognitive.TasksAndPerformance.TasksLimit.MaximumTasksInTotal =
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
                OrganizationEntity.AgentTemplates.Human.Cognitive.TasksAndPerformance.TasksLimit
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
                OrganizationEntity.AgentTemplates.Human.Cognitive.InteractionCharacteristics.MaximumMessagesPerPeriod =
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
                OrganizationEntity.AgentTemplates.Human.Cognitive.InteractionCharacteristics
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
                OrganizationEntity.AgentTemplates.Human.Cognitive.InteractionCharacteristics
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
                _environment.NumberOfTasks = int.Parse(numberTasksSent.Text, CultureInfo.InvariantCulture);
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
                _environment.CostOfTask = float.Parse(costOfTask.Text, CultureInfo.InvariantCulture);
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
                _environment.SwitchingContextCost = float.Parse(SwitchingContextCost.Text, CultureInfo.InvariantCulture);
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
                _environment.WorkersCount = int.Parse(tbWorkers.Text, CultureInfo.InvariantCulture);
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