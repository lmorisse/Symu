#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.ComponentModel;
using System.Windows.Forms;
using SymuEngine.Classes.Murphies;
using SymuEngine.Classes.Organization;
using SymuEngine.Common;
using SymuEngine.Environment;
using SymuEngine.Environment.TimeStep;

#endregion

namespace SymuEngine.Engine.Form
{
    public partial class SymuForm : System.Windows.Forms.Form
    {
        private SymuEnvironment _environment;

        public SymuForm()
        {
            InitializeComponent();
        }

        protected OrganizationEntity OrganizationEntity { get; set; }
        protected MurphyUnAvailability UnAvailability { get; } = new MurphyUnAvailability();
        protected TimeStepType TimeStepType { get; set; } = TimeStepType.Daily;
        protected AgentState State { get; private set; } = AgentState.Stopped;

        /// <summary>
        ///     Used when Event OnNextDay is triggered by this class
        /// </summary>
        public virtual void OnNextStep()
        {
            _environment.OnNextStep();
            // Post Process to avoid problem
            _environment.ManageAgentsToStop();
            // For Form Update
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

            SetUp(environment);
            State = AgentState.Starting;
            PreIteration();
            if (backgroundWorker1.IsBusy != true)
                // Start the asynchronous operation.
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

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

        private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            var i = 0;
            if (!(sender is BackgroundWorker worker))
            {
                throw new ArgumentNullException(nameof(worker));
            }

            while (!StopIteration())
            {
                i++;
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                OnNextStep();
                worker.ReportProgress(i);
            }
        }

        private void BackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            PostIteration();
        }

        private void BackgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Display();
        }

        protected void Cancel()
        {
            if (backgroundWorker1.WorkerSupportsCancellation)
                // Cancel the asynchronous operation.
            {
                backgroundWorker1.CancelAsync();
            }
        }

        #region Nested type: SafeCallTextDelegate

        protected delegate void SafeCallTextDelegate(Label label, string text);

        #endregion


        #region Initialize / set

        private void SetUp(SymuEnvironment environment)
        {
            OrganizationEntity = new OrganizationEntity("symu");
            _environment = environment;
            _environment.SetOrganization(OrganizationEntity);
            SetUpOrganization();
            _environment.TimeStep.Step = 0;
            _environment.InitializeIteration();
            SetScenarii();
        }

        protected virtual void SetUpOrganization()
        {
        }

        protected virtual void SetScenarii()
        {
        }

        protected void SetDebug(bool value)
        {
            _environment.Messages.Debug = value;
            _environment.State.Debug = value;
        }

        protected void SetDelay(int value)
        {
            _environment.Delay = value;
        }

        protected void SetRandomLevel(int value)
        {
            _environment.SetRandomLevel(value);
        }

        #endregion

        #region Iteration level

        /// <summary>
        ///     When to stop Timer event
        ///     Call each Environment StopIteration
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
            _environment.TimeStep.Type = TimeStepType;
            _environment.Start();
            _environment.WaitingForStart();
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