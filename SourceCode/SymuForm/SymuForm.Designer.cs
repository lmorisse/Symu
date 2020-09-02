using Symu.Common.Classes;

namespace Symu.Forms
{
    partial class SymuForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorkerDoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorkerRunWorkerCompleted);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorkerProgressChanged);
            // 
            // SymuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(508, 389);
            this.Name = "SymuForm";
            this.Text = "Home";
            this.ResumeLayout(false);

        }

        #endregion
        private System.ComponentModel.BackgroundWorker backgroundWorker1;

        /// <summary>
        /// Set timeStepType and adjust step based ontimeStepType
        /// </summary>
        /// <param name="timeStepType"></param>
        /// <param name="step"></param>
        /// <returns>Label of the timestep type</returns>
        protected string SetTimeStep(int timeStepType, ref ushort step)
        {
            var s = string.Empty;
            switch (timeStepType)
            {
                case 0:
                    SetTimeStepType(TimeStepType.Daily);
                    step *= 365;
                    s = "days";
                    break;
                case 1:
                    SetTimeStepType(TimeStepType.Weekly);
                    step *= 52;
                    s = "weeks";
                    break;
                case 2:
                    SetTimeStepType(TimeStepType.Monthly);
                    step *= 12;
                    s = "months";
                    break;
            }

            return s;
        }
    }
}

