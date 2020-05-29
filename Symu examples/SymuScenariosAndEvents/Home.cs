#region Licence

// Description: Symu - SymuMurphiesAndBlockers
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
using Symu.Environment.Events;
using Symu.Forms;
using Symu.Messaging.Messages;
using SymuScenariosAndEvents.Classes;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Chart;

#endregion

namespace SymuScenariosAndEvents
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

            tbWorkers.Text = _environment.WorkersCount.ToString(CultureInfo.InvariantCulture);
            
        }
        void Form1_PrepareStyle(object sender, ChartPrepareStyleInfoEventArgs args)
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

        protected override void UpdateSettings()
        {
            base.UpdateSettings();
            Iterations.Max = ushort.Parse(NumberOfIterations.Text);
            if (TimeBased.Checked)
            {
                var scenario = new TimeBasedScenario(_environment)
                {
                    NumberOfSteps = ushort.Parse(NumberOfSteps.Text)
                };
                AddScenario(scenario);
            }
            if (TaskBased.Checked)
            {
                var scenario = new TaskBasedScenario(_environment)
                {
                    NumberOfTasks = ushort.Parse(NumberOfTasks.Text)
                };
                AddScenario(scenario);
            }
            if (MessageBased.Checked)
            {
                var scenario = new MessageBasedScenario(_environment)
                {
                    NumberOfMessages = ushort.Parse(NumberOfMessages.Text)
                };
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
                OrganizationEntity.Models.On(1);
                OrganizationEntity.Models.Generator = RandomGenerator.RandomUniform;
            }
            else
            {
                OrganizationEntity.Models.Off();
            }

            #endregion

            #region Murphies

            if (MurphiesOn.Checked)
            {
                OrganizationEntity.Murphies.On(1);
                OrganizationEntity.Murphies.IncompleteKnowledge.CommunicationMediums = CommunicationMediums.Email;
                OrganizationEntity.Murphies.IncompleteBelief.CommunicationMediums = CommunicationMediums.Email;
            }
            else
            {
                OrganizationEntity.Murphies.Off();
            }

            #endregion


            SetRandomLevel(cbRandomLevel.SelectedIndex);
            SetTimeStepType(TimeStepType.Daily);

            cbIterations.Enabled = false;
        }

        private void SetEvents()
        {
            var eventType = SymuEventType.NoEvent;
            if (rbAtStep.Checked)
            {
                eventType = SymuEventType.OneShot;
            }

            if (rbCyclical.Checked)
            {
                eventType = SymuEventType.Cyclical;
            }

            if (rbRandom.Checked)
            {
                eventType = SymuEventType.Random;
            }

            var eventStep = SetEventStep();

            var randomRatio = SetRandomRatio();

            var cyclicalStep = SetCyclicalStep();

            if (AddPerson.Checked)
            {
                SymuEvent symuEvent = null;
                switch (eventType)
                {
                    case SymuEventType.NoEvent:
                        break;
                    case SymuEventType.OneShot:
                        symuEvent = new SymuEvent {Step = eventStep};
                        break;
                    case SymuEventType.Cyclical:
                        symuEvent = new CyclicalEvent {EveryStep = cyclicalStep};
                        break;
                    case SymuEventType.Random:
                        symuEvent = new RandomEvent {Ratio = randomRatio};
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (symuEvent != null)
                {
                    symuEvent.OnExecute += _environment.PersonEvent;
                    AddEvent(symuEvent);
                }
            }

            if (AddKnowledge.Checked)
            {
                SymuEvent symuEvent = null;
                switch (eventType)
                {
                    case SymuEventType.NoEvent:
                        break;
                    case SymuEventType.OneShot:
                        symuEvent = new SymuEvent {Step = eventStep};
                        break;
                    case SymuEventType.Cyclical:
                        symuEvent = new CyclicalEvent {EveryStep = cyclicalStep };
                        break;
                    case SymuEventType.Random:
                        symuEvent = new RandomEvent {Ratio = randomRatio };
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (symuEvent != null)
                {
                    symuEvent.OnExecute += _environment.KnowledgeEvent;
                    AddEvent(symuEvent);
                }
            }
        }

        private ushort SetCyclicalStep()
        {
            ushort cyclicalStep = 0;
            try
            {
                cyclicalStep = ushort.Parse(CyclicalStep.Text);
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
            float randomRatio=0;
            try
            {
                randomRatio = float.Parse(RandomRatio.Text);
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
                eventStep = ushort.Parse(EventStep.Text);
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
            Start(_environment);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        public override void DisplayStep()
        {
            DisplayButtons();
            WriteTextSafe(TimeStep, _environment.Schedule.Step.ToString());
            WriteTextSafe(TasksDone, _environment.IterationResult.Tasks.Done.ToString());
            WriteTextSafe(MessagesSent, _environment.Messages.SentMessagesCount.ToString());
        }

        public override void DisplayIteration()
        {
            WriteTextSafe(Iteration, Iterations.Number.ToString());

            var tasksResults = SimulationResults.List.Select(x => x.Tasks.Done).ToList();
            var capacityResults = SimulationResults.List.Select(x => x.Capacity).ToList();
            var seriesTasks = new ChartSeries("tasks", ChartSeriesType.Histogram) ;
            var count = Math.Max(tasksResults.Count, capacityResults.Count);
            foreach (var tasksResult in tasksResults)
            {
                seriesTasks.Points.Add(tasksResult, count);
            }
            seriesTasks.Text = seriesTasks.Name;
            seriesTasks.ConfigItems.HistogramItem.NumberOfIntervals = 10;
            //seriesTasks.ConfigItems.HistogramItem.ShowNormalDistribution = true;
            //seriesTasks.ConfigItems.HistogramItem.ShowDataPoints = true;

            var seriesCapacity = new ChartSeries("capacity", ChartSeriesType.Histogram);
            foreach (var capacityResult in capacityResults)
            {
                seriesCapacity.Points.Add(capacityResult, count);
            }
            seriesCapacity.Text = seriesCapacity.Name;
            seriesCapacity.ConfigItems.HistogramItem.NumberOfIntervals = 10;
            //seriesCapacity.ConfigItems.HistogramItem.ShowNormalDistribution = true;
            //seriesCapacity.ConfigItems.HistogramItem.ShowDataPoints = true;
            WriteChartSafe(chartControl2, new[] { seriesTasks, seriesCapacity });

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
            DisplayButtons(btnStart,btnStop, btnPause, btnResume);
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

        #region Menu

        private void symuorgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://symu.org");
        }

        private void documentationToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://docs.symu.org/");
        }

        private void sourceCodeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://github.symu.org/");
        }

        private void issuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://github.symu.org/issues");
        }

        #endregion

        #region Nested type: SafeCallButtonDelegate

        protected delegate void SafeCallChartDelegate(ChartControl chartControl, ChartSeries[] chartSeries);

        #endregion

        private void cbIterations_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = (int)cbIterations.SelectedItem;
            var seriesTasks = new ChartSeries { Name = "Tasks" };
            if (SimulationResults.Count == 0)
            {
                return;
            }
            foreach (var tasksResult in SimulationResults[index].Tasks.Results)
            {
                seriesTasks.Points.Add(tasksResult.Key, tasksResult.Value.Done);
            }
            seriesTasks.Type = ChartSeriesType.Column;
            seriesTasks.Text = seriesTasks.Name;
            var seriesBlockers = new ChartSeries { Name = "Blockers" };
            foreach (var blockerResults in SimulationResults[index].Blockers.Results)
            {
                seriesBlockers.Points.Add(blockerResults.Key, blockerResults.Value.Done);
            }
            seriesBlockers.Type = ChartSeriesType.Column;
            seriesBlockers.Text = seriesBlockers.Name;
            WriteChartSafe(chartControl1, new[] { seriesTasks, seriesBlockers });
        }
    }
}