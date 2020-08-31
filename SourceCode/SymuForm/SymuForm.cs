#region Licence

// Description: SymuBiz - SymuForm
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.ComponentModel;
using System.Windows.Forms;
using Symu.Classes.Organization;
using Symu.Classes.Scenario;
using Symu.Common;
using Symu.Engine;
using Symu.Environment;
using Symu.Repository.Entity;
using Symu.Results;

#endregion

namespace Symu.Forms
{
    /// <summary>
    ///     Symu Engine to use in GUI mode
    ///     Use Symu in batch mode
    /// </summary>
    public partial class SymuForm : Form
    {
        private bool _pauseWorker;

        public SymuForm()
        {
            InitializeComponent();
        }

        public SymuEngine Engine { get; } = new SymuEngine();

        protected OrganizationEntity OrganizationEntity { get; set; } = new OrganizationEntity("symu");

        /// <summary>
        ///     Manage the multiple iterations of the simulation
        ///     A interaction is a number of interaction steps
        ///     Multiple iterations are used to replay a simulation for MonteCarlo process or to vary parameters
        /// </summary>
        public Iterations Iterations => Engine.Iterations;

        /// <summary>
        ///     Store the results of each iteration
        /// </summary>
        public SimulationResults SimulationResults => Engine.SimulationResults;

        #region Nested type: SafeCallButtonDelegate

        protected delegate void SafeCallButtonDelegate(Button button, bool enabled);

        #endregion

        #region Nested type: SafeCallCheckBoxDelegate

        protected delegate void SafeCallCheckBoxDelegate(CheckBox checkbox, bool checkedValue);

        #endregion

        #region Nested type: SafeCallTextDelegate

        protected delegate void SafeCallTextDelegate(Label label, string text);

        #endregion

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

        protected void WriteCheckBoxSafe(CheckBox checkBox, bool checkedValue)
        {
            if (checkBox is null)
            {
                throw new ArgumentNullException(nameof(checkBox));
            }

            if (checkBox.InvokeRequired)
            {
                var d = new SafeCallCheckBoxDelegate(WriteCheckBoxSafe);
                checkBox.Invoke(d, checkBox, checkedValue);
            }
            else
            {
                checkBox.Checked = checkedValue;
            }
        }

        protected void DisplayButtons(Button btnStart, Button btnStop, Button btnPause, Button btnResume)
        {
            switch (Engine.State)
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
            Engine.OnNextStep();
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

            Engine.State = AgentState.Starting;
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
        ///     Trigger after the event Stopped
        /// </summary>
        protected virtual void OnStopped()
        {
            Engine.State = AgentState.Stopped;
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
            Engine.State = AgentState.Stopping;
            if (backgroundWorker1.WorkerSupportsCancellation)
                // CancelBlocker the asynchronous operation.
            {
                backgroundWorker1.CancelAsync();
            }
        }

        protected void Pause()
        {
            _pauseWorker = true;
            Engine.State = AgentState.Paused;
        }

        protected void Resume()
        {
            _pauseWorker = false;
            Engine.State = AgentState.Started;
        }

        #endregion

        #region Initialize / set

        private void SetUp(SymuEnvironment environment)
        {
            Engine.SetEnvironment(environment);
            Engine.Environment.SetOrganization(OrganizationEntity);
            UpdateSettings();
        }

        public void AddScenario(SimulationScenario scenario)
        {
            Engine.AddScenario(scenario);
        }

        protected void AddEvent(SymuEvent symuEvent)
        {
            Engine.AddEvent(symuEvent);
        }

        /// <summary>
        ///     Update settings of the environment and the organization via the form
        ///     Use SetOrganization to initialize organization
        ///     Add scenarios after calling base.UpdateSettings
        /// </summary>
        protected virtual void UpdateSettings()
        {
            Engine.Scenarii.Clear();
            OrganizationEntity.Clear();
        }

        protected void SetDebug(bool value)
        {
            Engine.Environment.SetDebug(value);
        }

        protected void SetDelay(int value)
        {
            Engine.Environment.SetDelay(value);
        }

        protected void SetRandomLevel(int value)
        {
            Engine.Environment.SetRandomLevel(value);
        }

        protected void SetTimeStepType(TimeStepType type)
        {
            Engine.Environment.SetTimeStepType(type);
        }

        #endregion

        #region Iteration level

        protected virtual void PreIteration()
        {
            Engine.PreIteration();
        }

        /// <summary>
        ///     When to stop the iteration based on the scenario agents.
        ///     You can add custom control
        /// </summary>
        /// <returns></returns>
        protected virtual bool StopIteration()
        {
            return Engine.StopIteration();
        }

        protected virtual void PostIteration()
        {
            Engine.PostIteration();
            DisplayIteration();
        }

        #endregion

        #region Process level

        protected virtual void PreProcess()
        {
            Engine.PreProcess();
        }

        protected virtual void PostProcess()
        {
            Engine.PostProcess();
        }

        protected virtual bool StopProcess()
        {
            return Engine.StopProcess();
        }

        #endregion
    }
}