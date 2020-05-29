#region Licence

// Description: Symu - SymuForm
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Symu.Classes.Organization;
using Symu.Classes.Scenario;
using Symu.Common;
using Symu.Engine;
using Symu.Environment;
using Symu.Environment.Events;
using Symu.Results;

#endregion

namespace Symu.Forms
{
    /// <summary>
    ///     Symu Engine to use in GUI mode
    ///     Use Symu in batch mode
    /// </summary>
    public partial class MultipleIterationsForm : System.Windows.Forms.Form
    {
        private readonly List<SimulationScenario> _scenarii = new List<SimulationScenario>();
        private SymuEnvironment _environment;
        private bool _pauseWorker;

        public MultipleIterationsForm()
        {
            InitializeComponent();
        }

        protected OrganizationEntity OrganizationEntity { get; set; } = new OrganizationEntity("symu");
        protected AgentState State { get; private set; } = AgentState.NotStarted;
        /// <summary>
        ///     Manage the multiple iterations of the simulation
        ///     A interaction is a number of interaction steps
        ///     Multiple iterations are used to replay a simulation for MonteCarlo process or to vary parameters
        /// </summary>
        public Iterations Iterations { get; set; } = new Iterations();
        /// <summary>
        ///     Store the results of each iteration
        /// </summary>
        public SimulationResults SimulationResults { get; set; } = new SimulationResults();

        #region Display
        public virtual void DisplayStep()
        {
        }
        public virtual void DisplayIteration()
        {
        }

        protected void WriteTextSafe(Label label, string text)
        {
            if (label is null)
            {
                throw new ArgumentNullException(nameof(label));
            }

            if (label.InvokeRequired)
            {
                var d = new SafeCallTextDelegate(WriteTextSafe);
                label.Invoke(d, label, text);
            }
            else
            {
                label.Text = text;
            }
        }
        protected void DisplayButtons(Button btnStart, Button btnStop, Button btnPause, Button btnResume)
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
        #endregion

        #region Engine

        /// <summary>
        ///     Used when Event OnNextDay is triggered by this class
        /// </summary>
        public virtual void OnNextStep()
        {
            _environment.OnNextStep();
            _environment.ManageAgentsToStop();
            DisplayStep();
        }

        /// <summary>
        /// </summary>
        /// <param name="environment"></param>
        protected void Start(SymuEnvironment environment)
        {
            if (environment is null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            State = AgentState.Starting;
            SetUp(environment);
            PreProcess();
            if (backgroundWorker1.IsBusy != true)
                // Start the asynchronous operation.
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            if (!(sender is BackgroundWorker worker))
            {
                throw new ArgumentNullException(nameof(worker));
            }

            var i = 0;
            while (!StopProcess())
            {
                if (_pauseWorker)
                {
                    while (_pauseWorker)
                    {
                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;
                            break;
                        }

                        if (_pauseWorker == false)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    PreIteration();
                    while (!StopIteration())
                    {
                        if (_pauseWorker)
                        {
                            while (_pauseWorker)
                            {
                                if (worker.CancellationPending)
                                {
                                    e.Cancel = true;
                                    break;
                                }

                                if (_pauseWorker == false)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (worker.CancellationPending)
                            {
                                e.Cancel = true;
                                break;
                            }

                            i++;
                            OnNextStep();
                            worker.ReportProgress(i);
                        }
                    }
                    PostIteration();
                }
            }

            OnStopped();
        }
        /// <summary>
        /// Trigger after the event Stopped
        /// </summary>
        protected virtual void OnStopped()
        {
            State = AgentState.Stopped;
        }

        private void BackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            PostProcess();
        }

        private void BackgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DisplayStep();
        }

        protected void Cancel()
        {
            State = AgentState.Stopping;
            if (backgroundWorker1.WorkerSupportsCancellation)
                // Cancel the asynchronous operation.
            {
                backgroundWorker1.CancelAsync();
            }
        }

        protected void Pause()
        {
            _pauseWorker = true;
            State = AgentState.Paused;
        }

        protected void Resume()
        {
            _pauseWorker = false;
            State = AgentState.Started;
        }
        #endregion

        #region Nested type: SafeCallDelegate

        protected delegate void SafeCallTextDelegate(Label label, string text);
        protected delegate void SafeCallButtonDelegate(Button button, bool enabled);

        #endregion

        #region Initialize / set

        private void SetUp(SymuEnvironment environment)
        {
            OrganizationEntity.Clear();
            _environment = environment;
            _environment.SetOrganization(OrganizationEntity);
            UpdateSettings();
        }

        public void AddScenario(SimulationScenario scenario)
        {
            if (!_scenarii.Exists(s => s.Id.Equals(scenario.Id)))
            {
                _scenarii.Add(scenario);
            }
        }

        protected void AddEvent(SymuEvent symuEvent)
        {
            _environment.AddEvent(symuEvent);
        }

        /// <summary>
        ///     Update settings of the environment and the organization via the form
        ///     Use SetOrganization to initialize organization
        /// </summary>
        protected virtual void UpdateSettings()
        {
            _scenarii.Clear();
        }

        protected void SetDebug(bool value)
        {
            _environment.SetDebug(value);
        }

        protected void SetDelay(int value)
        {
            _environment.SetDelay(value);
        }

        protected void SetRandomLevel(int value)
        {
            _environment.SetRandomLevel(value);
        }

        protected void SetTimeStepType(TimeStepType type)
        {
            _environment.SetTimeStepType(type);
        }

        #endregion

        #region Iteration level

        public void PreIteration()
        {
            State = AgentState.Starting;
            _environment.InitializeIteration();
            // AddScenario should stay after initialize
            SetScenariiAndTimeStep();
            // AddScenario should stay after initialize
            _environment.Schedule.Step = 0;
            Iterations.UpdateIteration(_scenarii);
            _environment.Start();
            _environment.WaitingForStart();
            _environment.SetInteractionSphere(true);
            State = AgentState.Started;
        }
        protected void SetScenariiAndTimeStep()
        {
            ushort step0 = 0;
            foreach (var scenario in _scenarii.Where(sc => sc.IsActive))
            {
                var clone = scenario.Clone();
                clone.SetUp();
                // scenarii could have different Day0 (>0)
                step0 = step0 == 0 ? clone.Day0 : Math.Min(step0, clone.Day0);
            }

            _environment.Schedule.Step = step0;
        }

        public void Iteration()
        {
            PreIteration();
            while (!StopIteration())
            {
                OnNextStep();
            }
            PostIteration();
        }

        /// <summary>
        ///     When to stop the iteration based on the scenario agents.
        ///     You can add custom control 
        /// </summary>
        /// <returns></returns>
        public virtual bool StopIteration()
        {
            return _environment.StopIteration();
        }

        private void PostIteration()
        {
            SimulationResults.List.Add(_environment.SetIterationResult(Iterations.Number));
            if (_environment.IterationResult.Success)
            {
                AnalyzeIteration();
            }
            State = AgentState.Stopped;
            DisplayIteration();
        }

        protected virtual void AnalyzeIteration()
        {
            if (!_environment.IterationResult.Success)
            {
            }
        }

        #endregion

        #region Process level

        public void PreProcess()
        {
            SimulationResults.Clear();
            Iterations.SetUp();
        }

        public virtual void Process()
        {
            State = AgentState.Starting;
            PreProcess();
            while (!StopProcess())
            {
                Iteration();
            }
            PostProcess();
        }

        protected virtual void PostProcess()
        {
        }

        protected bool StopProcess()
        {
            return Iterations.Stop();
        }

        #endregion
    }
}