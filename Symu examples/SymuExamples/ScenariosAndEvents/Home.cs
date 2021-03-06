﻿#region Licence

// Description: SymuBiz - SymuScenariosAndEvents
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
using Symu.Common.Classes;
using Symu.Forms;
using Symu.Messaging.Messages;
using Symu.Repository.Entities;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Chart;

#endregion

namespace SymuExamples.ScenariosAndEvents
{
    public partial class Home : SymuForm
    {
        private readonly ExampleEnvironment _environment = new ExampleEnvironment();
        private readonly ExampleMainOrganization _mainOrganization = new ExampleMainOrganization();

        public Home()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            DisplayButtons();
            chartControl1.Series[0].PrepareStyle += Form1_PrepareStyle;
            chartControl2.Series[0].PrepareStyle += Form1_PrepareStyle;

            #region Legend Customization

            chartControl1.PrimaryXAxis.Title = "Step";
            //chartControl1.PrimaryYAxis.Title = "Tasks";
            chartControl1.Legend.Visible = true;
            chartControl1.Title.Visible = false;

            chartControl2.PrimaryYAxis.Title = "#";
            chartControl2.Legend.Visible = true;
            chartControl2.Title.Text = "Simulation (Monte Carlo)";
            chartControl2.Title.Visible = true;
            chartControl2.DropSeriesPoints = true;

            #endregion

            tbWorkers.Text = _mainOrganization.WorkersCount.ToString(CultureInfo.InvariantCulture);
        }

        private void Form1_PrepareStyle(object sender, ChartPrepareStyleInfoEventArgs args)
        {
            var series = chartControl1.Series[0];
            if (series == null)
            {
                return;
            }

            switch (args.Index)
            {
                case 0:
                    args.Style.Interior = new BrushInfo(Color.FromArgb(0xFF, 0x1B, 0xA1, 0xE2));
                    break;
                case 1:
                    args.Style.Interior = new BrushInfo(Color.FromArgb(0xFF, 0xA0, 0x50, 0x00));
                    break;
            }
        }

        protected override void SetUpOrganization()
        {
            base.SetUpOrganization();
            Iterations.Max = ushort.Parse(NumberOfIterations.Text, CultureInfo.InvariantCulture);
            if (TimeBased.Checked)
            {
                var scenario = TimeBasedScenario.CreateInstance(_environment);
                scenario.NumberOfSteps = ushort.Parse(NumberOfSteps.Text);
                AddScenario(scenario);
            }

            if (TaskBased.Checked)
            {
                var scenario = TaskBasedScenario.CreateInstance(_environment);
                scenario.NumberOfTasks = ushort.Parse(NumberOfTasks.Text, CultureInfo.InvariantCulture);
                AddScenario(scenario);
            }

            if (MessageBased.Checked)
            {
                var scenario = MessageBasedScenario.CreateInstance(_environment);
                scenario.NumberOfMessages = ushort.Parse(NumberOfMessages.Text, CultureInfo.InvariantCulture);
                AddScenario(scenario);
            }

            cbIterations.Items.Clear();
            for (var i = 0; i < Iterations.Max; i++)
            {
                cbIterations.Items.Add(i);
            }

            SetEvents();

            #region models

            if (ModelsOn.Checked)
            {
                _mainOrganization.Models.SetOn(1);
                _mainOrganization.Models.Generator = RandomGenerator.RandomUniform;
            }
            else
            {
                _mainOrganization.Models.SetOff();
            }

            #endregion

            #region Murphies

            if (MurphiesOn.Checked)
            {
                _mainOrganization.Murphies.SetOn(1);
                _mainOrganization.Murphies.IncompleteKnowledge.CommunicationMediums = CommunicationMediums.Email;
                _mainOrganization.Murphies.IncompleteBelief.CommunicationMediums = CommunicationMediums.Email;
            }
            else
            {
                _mainOrganization.Murphies.SetOff();
            }

            #endregion


            SetRandomLevel(cbRandomLevel.SelectedIndex);
            cbIterations.Enabled = false;
            _mainOrganization.AddKnowledge();
        }

        private void SetEvents()
        {
            var eventType = Cyclicity.None;
            if (rbAtStep.Checked)
            {
                eventType = Cyclicity.OneShot;
            }

            if (rbCyclical.Checked)
            {
                eventType = Cyclicity.Cyclical;
            }

            if (rbRandom.Checked)
            {
                eventType = Cyclicity.Random;
            }

            var eventStep = SetEventStep();

            var randomRatio = SetRandomRatio();

            var cyclicalStep = SetCyclicalStep();

            if (AddPerson.Checked)
            {
                EventEntity eventEntity = null;
                switch (eventType)
                {
                    case Cyclicity.None:
                        break;
                    case Cyclicity.OneShot:
                        eventEntity = new EventEntity(_environment.MainOrganization.ArtifactNetwork) {Step = eventStep};
                        break;
                    case Cyclicity.Cyclical:
                        eventEntity = new CyclicalEvent(_environment.MainOrganization.ArtifactNetwork)
                            {EveryStep = cyclicalStep};
                        break;
                    case Cyclicity.Random:
                        eventEntity = new RandomEvent(_environment.MainOrganization.ArtifactNetwork) {Ratio = randomRatio};
                        break;
                    case Cyclicity.Always:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (eventEntity != null)
                {
                    eventEntity.OnExecute += _environment.PersonEvent;
                }
            }

            if (AddKnowledge.Checked)
            {
                EventEntity eventEntity = null;
                switch (eventType)
                {
                    case Cyclicity.None:
                        break;
                    case Cyclicity.OneShot:
                        eventEntity = new EventEntity(_environment.MainOrganization.ArtifactNetwork) {Step = eventStep};
                        break;
                    case Cyclicity.Cyclical:
                        eventEntity = new CyclicalEvent(_environment.MainOrganization.ArtifactNetwork)
                            {EveryStep = cyclicalStep};
                        break;
                    case Cyclicity.Random:
                        eventEntity = new RandomEvent(_environment.MainOrganization.ArtifactNetwork) {Ratio = randomRatio};
                        break;
                    case Cyclicity.Always:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (eventEntity != null)
                {
                    eventEntity.OnExecute += _environment.KnowledgeEvent;
                }
            }
        }

        private ushort SetCyclicalStep()
        {
            ushort cyclicalStep = 0;
            try
            {
                cyclicalStep = ushort.Parse(CyclicalStep.Text, CultureInfo.InvariantCulture);
                CyclicalStep.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                CyclicalStep.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                CyclicalStep.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }

            return cyclicalStep;
        }

        private float SetRandomRatio()
        {
            float randomRatio = 0;
            try
            {
                randomRatio = float.Parse(RandomRatio.Text, CultureInfo.InvariantCulture);
                RandomRatio.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                RandomRatio.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                RandomRatio.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }

            return randomRatio;
        }

        private ushort SetEventStep()
        {
            ushort eventStep = 0;
            try
            {
                eventStep = ushort.Parse(EventStep.Text, CultureInfo.InvariantCulture);
                EventStep.BackColor = SystemColors.Window;
            }
            catch (FormatException)
            {
                EventStep.BackColor = Color.Red;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                EventStep.BackColor = Color.Red;
                MessageBox.Show(exception.Message);
            }

            return eventStep;
        }

        protected override void OnStopped()
        {
            base.OnStopped();
            DisplayButtons();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Start(_environment, _mainOrganization);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        public override void DisplayStep()
        {
            DisplayButtons();
            WriteTextSafe(TimeStep, _environment.Schedule.Step.ToString(CultureInfo.InvariantCulture));
            WriteTextSafe(TasksDone, _environment.IterationResult.Tasks.Done.ToString(CultureInfo.InvariantCulture));
            WriteTextSafe(MessagesSent,
                _environment.Messages.Result.SentMessagesCount.ToString(CultureInfo.InvariantCulture));
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

            var seriesCapacity = new ChartSeries("Ratio capacity", ChartSeriesType.Histogram);
            var capacityResults = SimulationResults.List.Select(x => x.Tasks.Capacity).ToList();
            foreach (var capacityResult in capacityResults)
            {
                seriesCapacity.Points.Add(capacityResult.Last().Density, tasksResults.Count);
            }

            seriesCapacity.Text = seriesCapacity.Name;
            seriesCapacity.ConfigItems.HistogramItem.NumberOfIntervals = 10;
            WriteChartSafe(chartControl2, new[] {seriesTasks, seriesCapacity});
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

        protected override void PostProcess()
        {
            base.PostProcess();
            if (cbIterations.Items.Count <= 0)
            {
                return;
            }

            cbIterations.Enabled = true;
            cbIterations.SelectedIndex = 0;
        }

        private void tbWorkers_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _mainOrganization.WorkersCount = byte.Parse(tbWorkers.Text, CultureInfo.InvariantCulture);
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

        private void cbIterations_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = (int) cbIterations.SelectedItem;
            var seriesTasks = new ChartSeries {Name = "Tasks"};
            if (SimulationResults.Count == 0)
            {
                return;
            }

            foreach (var tasksResult in SimulationResults[index].Tasks.Tasks)
            {
                seriesTasks.Points.Add(tasksResult.Key, tasksResult.Value.Done);
            }

            seriesTasks.Type = ChartSeriesType.Column;
            seriesTasks.Text = seriesTasks.Name;
            var seriesBlockers = new ChartSeries {Name = "Blockers"};
            foreach (var blockerResults in SimulationResults[index].Blockers.Results)
            {
                seriesBlockers.Points.Add(blockerResults.Key, blockerResults.Value.Done);
            }

            seriesBlockers.Type = ChartSeriesType.Column;
            seriesBlockers.Text = seriesBlockers.Name;
            WriteChartSafe(chartControl1, new[] {seriesTasks, seriesBlockers});
        }

        #region Nested type: SafeCallChartDelegate

        protected delegate void SafeCallChartDelegate(ChartControl chartControl, ChartSeries[] chartSeries);

        #endregion

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