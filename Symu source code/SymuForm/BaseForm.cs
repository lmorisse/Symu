#region Licence

// Description: Symu - SymuForm
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.ComponentModel;
using System.Windows.Forms;
using Symu.Classes.Organization;
using Symu.Common;
using Symu.Environment;
using Symu.Environment.Events;

#endregion

namespace SymuForm
{
    /// <summary>
    ///     Symu Engine to use in GUI mode
    ///     Use Symu in batch mode
    /// </summary>
    public partial class BaseForm : Form
    {
        private SymuEnvironment _environment;
        private bool _pauseWorker;

        public BaseForm()
        {
            InitializeComponent();
        }

        protected OrganizationEntity OrganizationEntity { get; set; } = new OrganizationEntity("symu");
        //private TimeStepType TimeStepType { get; set; } = TimeStepType.Daily;
        protected AgentState State { get; private set; } = AgentState.NotStarted;

        #region DisplayStep
        /// <summary>
        /// Called at the end of each new step.
        /// Used this method to display the state of the new step
        /// </summary>
        public virtual void Display()
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
        #endregion

        /// <summary>
        ///     Used when Event OnNextDay is triggered by this class
        /// </summary>
        public virtual void OnNextStep()
        {
            _environment.OnNextStep();
            _environment.ManageAgentsToStop();
            Display();
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
            PreIteration();
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

                    OnNextStep();
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
            PostIteration();
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

        #region Nested type: SafeCallTextDelegate

        protected delegate void SafeCallTextDelegate(Label label, string text);

        #endregion

        #region Initialize / set

        private void SetUp(SymuEnvironment environment)
        {
            OrganizationEntity.Clear();
            _environment = environment;
            _environment.SetOrganization(OrganizationEntity);
            UpdateSettings();
            _environment.Schedule.Step = 0;
            _environment.InitializeIteration();
            SetScenarii();
        }

        /// <summary>
        ///     Update settings of the environment and the organization via the form
        ///     Use SetOrganization to initialize organization
        /// </summary>
        protected virtual void UpdateSettings()
        {
        }

        protected virtual void SetScenarii()
        {
        }
        protected void AddEvent(SymuEvent symuEvent)
        {
            _environment.AddEvent(symuEvent);
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
            _environment.SetIterationResult(1);
            AnalyzeIteration();
            State = AgentState.Stopped;
        }

        public void PreIteration()
        {
            //_environment.Schedule.Type = TimeStepType;
            _environment.Start();
            _environment.WaitingForStart();
            _environment.SetInteractionSphere(true);
            State = AgentState.Started;
        }

        protected virtual void AnalyzeIteration()
        {
            if (!_environment.IterationResult.Success)
            {
            }
        }

        #endregion
    }
}