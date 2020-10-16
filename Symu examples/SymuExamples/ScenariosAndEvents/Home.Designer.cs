namespace SymuExamples.ScenariosAndEvents
{
    partial class Home
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Home));
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.TimeStep = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbWorkers = new System.Windows.Forms.TextBox();
            this.btnResume = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.chartControl2 = new Syncfusion.Windows.Forms.Chart.ChartControl();
            this.chartControl1 = new Syncfusion.Windows.Forms.Chart.ChartControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbIterations = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.MessagesSent = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.TasksDone = new System.Windows.Forms.Label();
            this.Iteration = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.RandomRatio = new System.Windows.Forms.TextBox();
            this.rbRandom = new System.Windows.Forms.RadioButton();
            this.label10 = new System.Windows.Forms.Label();
            this.CyclicalStep = new System.Windows.Forms.TextBox();
            this.rbCyclical = new System.Windows.Forms.RadioButton();
            this.rbAtStep = new System.Windows.Forms.RadioButton();
            this.AddPerson = new System.Windows.Forms.CheckBox();
            this.AddKnowledge = new System.Windows.Forms.CheckBox();
            this.EventStep = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.TaskBased = new System.Windows.Forms.CheckBox();
            this.MessageBased = new System.Windows.Forms.CheckBox();
            this.TimeBased = new System.Windows.Forms.CheckBox();
            this.NumberOfMessages = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.NumberOfTasks = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.NumberOfSteps = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.MurphiesOn = new System.Windows.Forms.CheckBox();
            this.ModelsOn = new System.Windows.Forms.CheckBox();
            this.label22 = new System.Windows.Forms.Label();
            this.cbRandomLevel = new System.Windows.Forms.ComboBox();
            this.NumberOfIterations = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.symuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.symuorgToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.documentationToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.sourceCodeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.issuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox5.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(32, 34);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.Button1_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(32, 63);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.Button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(253, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Steps";
            // 
            // TimeStep
            // 
            this.TimeStep.AutoSize = true;
            this.TimeStep.Location = new System.Drawing.Point(318, 59);
            this.TimeStep.Name = "TimeStep";
            this.TimeStep.Size = new System.Drawing.Size(16, 17);
            this.TimeStep.TabIndex = 3;
            this.TimeStep.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "Number of workers";
            // 
            // tbWorkers
            // 
            this.tbWorkers.Location = new System.Drawing.Point(161, 25);
            this.tbWorkers.Name = "tbWorkers";
            this.tbWorkers.Size = new System.Drawing.Size(63, 22);
            this.tbWorkers.TabIndex = 10;
            this.tbWorkers.Text = "5";
            this.tbWorkers.TextChanged += new System.EventHandler(this.tbWorkers_TextChanged);
            // 
            // btnResume
            // 
            this.btnResume.Location = new System.Drawing.Point(119, 63);
            this.btnResume.Name = "btnResume";
            this.btnResume.Size = new System.Drawing.Size(75, 23);
            this.btnResume.TabIndex = 12;
            this.btnResume.Text = "Resume";
            this.btnResume.UseVisualStyleBackColor = true;
            this.btnResume.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(119, 34);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(75, 23);
            this.btnPause.TabIndex = 11;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.button4_Click);
            // 
            // richTextBox2
            // 
            this.richTextBox2.BackColor = System.Drawing.Color.AliceBlue;
            this.richTextBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox2.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.richTextBox2.Location = new System.Drawing.Point(457, 32);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(1071, 153);
            this.richTextBox2.TabIndex = 18;
            this.richTextBox2.Text = resources.GetString("richTextBox2.Text");
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.AliceBlue;
            this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.richTextBox1.Location = new System.Drawing.Point(4, 32);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(428, 153);
            this.richTextBox1.TabIndex = 17;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            // 
            // groupBox5
            // 
            this.groupBox5.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.groupBox5.Controls.Add(this.chartControl2);
            this.groupBox5.Controls.Add(this.chartControl1);
            this.groupBox5.Controls.Add(this.groupBox1);
            this.groupBox5.Controls.Add(this.Iteration);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.btnResume);
            this.groupBox5.Controls.Add(this.btnPause);
            this.groupBox5.Controls.Add(this.TimeStep);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.btnStop);
            this.groupBox5.Controls.Add(this.btnStart);
            this.groupBox5.Location = new System.Drawing.Point(524, 203);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(1004, 646);
            this.groupBox5.TabIndex = 20;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Run";
            // 
            // chartControl2
            // 
            this.chartControl2.BackInterior = new Syncfusion.Drawing.BrushInfo(System.Drawing.SystemColors.ActiveCaption);
            this.chartControl2.ChartArea.CursorLocation = new System.Drawing.Point(0, 0);
            this.chartControl2.ChartArea.CursorReDraw = false;
            this.chartControl2.IsWindowLess = false;
            // 
            // 
            // 
            this.chartControl2.Legend.Location = new System.Drawing.Point(359, 81);
            this.chartControl2.Localize = null;
            this.chartControl2.Location = new System.Drawing.Point(0, 147);
            this.chartControl2.Name = "chartControl2";
            this.chartControl2.PrimaryXAxis.LogLabelsDisplayMode = Syncfusion.Windows.Forms.Chart.LogLabelsDisplayMode.Default;
            this.chartControl2.PrimaryXAxis.Margin = true;
            this.chartControl2.PrimaryYAxis.LogLabelsDisplayMode = Syncfusion.Windows.Forms.Chart.LogLabelsDisplayMode.Default;
            this.chartControl2.PrimaryYAxis.Margin = true;
            this.chartControl2.Size = new System.Drawing.Size(480, 478);
            this.chartControl2.TabIndex = 56;
            this.chartControl2.Text = "chartControl2";
            // 
            // 
            // 
            this.chartControl2.Title.Name = "Default";
            this.chartControl2.Titles.Add(this.chartControl2.Title);
            this.chartControl2.VisualTheme = "";
            // 
            // chartControl1
            // 
            this.chartControl1.BackInterior = new Syncfusion.Drawing.BrushInfo(System.Drawing.SystemColors.ActiveCaption);
            this.chartControl1.ChartArea.CursorLocation = new System.Drawing.Point(0, 0);
            this.chartControl1.ChartArea.CursorReDraw = false;
            this.chartControl1.IsWindowLess = false;
            // 
            // 
            // 
            this.chartControl1.Legend.Location = new System.Drawing.Point(361, 81);
            this.chartControl1.Localize = null;
            this.chartControl1.Location = new System.Drawing.Point(504, 147);
            this.chartControl1.Name = "chartControl1";
            this.chartControl1.PrimaryXAxis.LogLabelsDisplayMode = Syncfusion.Windows.Forms.Chart.LogLabelsDisplayMode.Default;
            this.chartControl1.PrimaryXAxis.Margin = true;
            this.chartControl1.PrimaryYAxis.LogLabelsDisplayMode = Syncfusion.Windows.Forms.Chart.LogLabelsDisplayMode.Default;
            this.chartControl1.PrimaryYAxis.Margin = true;
            this.chartControl1.Size = new System.Drawing.Size(482, 487);
            this.chartControl1.TabIndex = 55;
            this.chartControl1.Text = "chartControl1";
            // 
            // 
            // 
            this.chartControl1.Title.Name = "Default";
            this.chartControl1.Titles.Add(this.chartControl1.Title);
            this.chartControl1.VisualTheme = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.cbIterations);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.MessagesSent);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.TasksDone);
            this.groupBox1.Location = new System.Drawing.Point(498, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(500, 626);
            this.groupBox1.TabIndex = 189;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Iteration results";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(33, 89);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(102, 17);
            this.label8.TabIndex = 192;
            this.label8.Text = "Iteration charts";
            // 
            // cbIterations
            // 
            this.cbIterations.FormattingEnabled = true;
            this.cbIterations.Location = new System.Drawing.Point(189, 86);
            this.cbIterations.Name = "cbIterations";
            this.cbIterations.Size = new System.Drawing.Size(63, 24);
            this.cbIterations.TabIndex = 191;
            this.cbIterations.SelectedIndexChanged += new System.EventHandler(this.cbIterations_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 17);
            this.label4.TabIndex = 25;
            this.label4.Text = "Messages sent";
            // 
            // MessagesSent
            // 
            this.MessagesSent.AutoSize = true;
            this.MessagesSent.Location = new System.Drawing.Point(186, 59);
            this.MessagesSent.Name = "MessagesSent";
            this.MessagesSent.Size = new System.Drawing.Size(16, 17);
            this.MessagesSent.TabIndex = 26;
            this.MessagesSent.Text = "0";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(33, 37);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(82, 17);
            this.label18.TabIndex = 23;
            this.label18.Text = "Tasks done";
            // 
            // TasksDone
            // 
            this.TasksDone.AutoSize = true;
            this.TasksDone.Location = new System.Drawing.Point(186, 37);
            this.TasksDone.Name = "TasksDone";
            this.TasksDone.Size = new System.Drawing.Size(16, 17);
            this.TasksDone.TabIndex = 24;
            this.TasksDone.Text = "0";
            // 
            // Iteration
            // 
            this.Iteration.AutoSize = true;
            this.Iteration.Location = new System.Drawing.Point(317, 34);
            this.Iteration.Name = "Iteration";
            this.Iteration.Size = new System.Drawing.Size(16, 17);
            this.Iteration.TabIndex = 188;
            this.Iteration.Text = "0";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(254, 34);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(59, 17);
            this.label9.TabIndex = 187;
            this.label9.Text = "Iteration";
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.SystemColors.ControlLight;
            this.groupBox4.Controls.Add(this.groupBox7);
            this.groupBox4.Controls.Add(this.groupBox3);
            this.groupBox4.Controls.Add(this.groupBox6);
            this.groupBox4.Location = new System.Drawing.Point(4, 203);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(481, 646);
            this.groupBox4.TabIndex = 21;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Settings";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.RandomRatio);
            this.groupBox7.Controls.Add(this.rbRandom);
            this.groupBox7.Controls.Add(this.label10);
            this.groupBox7.Controls.Add(this.CyclicalStep);
            this.groupBox7.Controls.Add(this.rbCyclical);
            this.groupBox7.Controls.Add(this.rbAtStep);
            this.groupBox7.Controls.Add(this.AddPerson);
            this.groupBox7.Controls.Add(this.AddKnowledge);
            this.groupBox7.Controls.Add(this.EventStep);
            this.groupBox7.Location = new System.Drawing.Point(17, 466);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(439, 159);
            this.groupBox7.TabIndex = 54;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Events";
            // 
            // RandomRatio
            // 
            this.RandomRatio.Location = new System.Drawing.Point(309, 100);
            this.RandomRatio.Name = "RandomRatio";
            this.RandomRatio.Size = new System.Drawing.Size(63, 22);
            this.RandomRatio.TabIndex = 58;
            this.RandomRatio.Text = "0,1";
            // 
            // rbRandom
            // 
            this.rbRandom.AutoSize = true;
            this.rbRandom.Location = new System.Drawing.Point(178, 99);
            this.rbRandom.Name = "rbRandom";
            this.rbRandom.Size = new System.Drawing.Size(122, 21);
            this.rbRandom.TabIndex = 57;
            this.rbRandom.TabStop = true;
            this.rbRandom.Text = "Random : ratio";
            this.rbRandom.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(387, 75);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(44, 17);
            this.label10.TabIndex = 54;
            this.label10.Text = "Steps";
            // 
            // CyclicalStep
            // 
            this.CyclicalStep.Location = new System.Drawing.Point(309, 72);
            this.CyclicalStep.Name = "CyclicalStep";
            this.CyclicalStep.Size = new System.Drawing.Size(63, 22);
            this.CyclicalStep.TabIndex = 56;
            this.CyclicalStep.Text = "10";
            // 
            // rbCyclical
            // 
            this.rbCyclical.AutoSize = true;
            this.rbCyclical.Location = new System.Drawing.Point(178, 72);
            this.rbCyclical.Name = "rbCyclical";
            this.rbCyclical.Size = new System.Drawing.Size(123, 21);
            this.rbCyclical.TabIndex = 55;
            this.rbCyclical.TabStop = true;
            this.rbCyclical.Text = "Cyclical : every";
            this.rbCyclical.UseVisualStyleBackColor = true;
            // 
            // rbAtStep
            // 
            this.rbAtStep.AutoSize = true;
            this.rbAtStep.Location = new System.Drawing.Point(178, 45);
            this.rbAtStep.Name = "rbAtStep";
            this.rbAtStep.Size = new System.Drawing.Size(73, 21);
            this.rbAtStep.TabIndex = 54;
            this.rbAtStep.TabStop = true;
            this.rbAtStep.Text = "At step";
            this.rbAtStep.UseVisualStyleBackColor = true;
            // 
            // AddPerson
            // 
            this.AddPerson.AutoSize = true;
            this.AddPerson.Location = new System.Drawing.Point(6, 88);
            this.AddPerson.Name = "AddPerson";
            this.AddPerson.Size = new System.Drawing.Size(142, 21);
            this.AddPerson.TabIndex = 53;
            this.AddPerson.Text = "Add a new worker";
            this.AddPerson.UseVisualStyleBackColor = true;
            // 
            // AddKnowledge
            // 
            this.AddKnowledge.AutoSize = true;
            this.AddKnowledge.Location = new System.Drawing.Point(6, 58);
            this.AddKnowledge.Name = "AddKnowledge";
            this.AddKnowledge.Size = new System.Drawing.Size(148, 21);
            this.AddKnowledge.TabIndex = 51;
            this.AddKnowledge.Text = "Add a new Techno";
            this.AddKnowledge.UseVisualStyleBackColor = true;
            // 
            // EventStep
            // 
            this.EventStep.Location = new System.Drawing.Point(309, 44);
            this.EventStep.Name = "EventStep";
            this.EventStep.Size = new System.Drawing.Size(63, 22);
            this.EventStep.TabIndex = 39;
            this.EventStep.Text = "50";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.TaskBased);
            this.groupBox3.Controls.Add(this.MessageBased);
            this.groupBox3.Controls.Add(this.TimeBased);
            this.groupBox3.Controls.Add(this.NumberOfMessages);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.NumberOfTasks);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.NumberOfSteps);
            this.groupBox3.Location = new System.Drawing.Point(17, 274);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(439, 151);
            this.groupBox3.TabIndex = 45;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Scenarios";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(175, 103);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(142, 17);
            this.label6.TabIndex = 50;
            this.label6.Text = "Number of messages";
            // 
            // TaskBased
            // 
            this.TaskBased.AutoSize = true;
            this.TaskBased.Location = new System.Drawing.Point(28, 72);
            this.TaskBased.Name = "TaskBased";
            this.TaskBased.Size = new System.Drawing.Size(104, 21);
            this.TaskBased.TabIndex = 53;
            this.TaskBased.Text = "Task based";
            this.TaskBased.UseVisualStyleBackColor = true;
            // 
            // MessageBased
            // 
            this.MessageBased.AutoSize = true;
            this.MessageBased.Location = new System.Drawing.Point(28, 102);
            this.MessageBased.Name = "MessageBased";
            this.MessageBased.Size = new System.Drawing.Size(130, 21);
            this.MessageBased.TabIndex = 52;
            this.MessageBased.Text = "Message based";
            this.MessageBased.UseVisualStyleBackColor = true;
            // 
            // TimeBased
            // 
            this.TimeBased.AutoSize = true;
            this.TimeBased.Checked = true;
            this.TimeBased.CheckState = System.Windows.Forms.CheckState.Checked;
            this.TimeBased.Location = new System.Drawing.Point(28, 42);
            this.TimeBased.Name = "TimeBased";
            this.TimeBased.Size = new System.Drawing.Size(104, 21);
            this.TimeBased.TabIndex = 51;
            this.TimeBased.Text = "Time based";
            this.TimeBased.UseVisualStyleBackColor = true;
            // 
            // NumberOfMessages
            // 
            this.NumberOfMessages.Location = new System.Drawing.Point(356, 100);
            this.NumberOfMessages.Name = "NumberOfMessages";
            this.NumberOfMessages.Size = new System.Drawing.Size(63, 22);
            this.NumberOfMessages.TabIndex = 49;
            this.NumberOfMessages.Text = "200";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(175, 73);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(111, 17);
            this.label5.TabIndex = 47;
            this.label5.Text = "Number of tasks";
            // 
            // NumberOfTasks
            // 
            this.NumberOfTasks.Location = new System.Drawing.Point(356, 70);
            this.NumberOfTasks.Name = "NumberOfTasks";
            this.NumberOfTasks.Size = new System.Drawing.Size(63, 22);
            this.NumberOfTasks.TabIndex = 46;
            this.NumberOfTasks.Text = "150";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(175, 43);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(112, 17);
            this.label7.TabIndex = 42;
            this.label7.Text = "Number of steps";
            // 
            // NumberOfSteps
            // 
            this.NumberOfSteps.Location = new System.Drawing.Point(356, 40);
            this.NumberOfSteps.Name = "NumberOfSteps";
            this.NumberOfSteps.Size = new System.Drawing.Size(63, 22);
            this.NumberOfSteps.TabIndex = 39;
            this.NumberOfSteps.Text = "100";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.MurphiesOn);
            this.groupBox6.Controls.Add(this.ModelsOn);
            this.groupBox6.Controls.Add(this.label22);
            this.groupBox6.Controls.Add(this.cbRandomLevel);
            this.groupBox6.Controls.Add(this.NumberOfIterations);
            this.groupBox6.Controls.Add(this.label3);
            this.groupBox6.Controls.Add(this.tbWorkers);
            this.groupBox6.Controls.Add(this.label2);
            this.groupBox6.Location = new System.Drawing.Point(17, 26);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(439, 187);
            this.groupBox6.TabIndex = 18;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Simulation";
            // 
            // MurphiesOn
            // 
            this.MurphiesOn.AutoSize = true;
            this.MurphiesOn.Checked = true;
            this.MurphiesOn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MurphiesOn.Location = new System.Drawing.Point(28, 148);
            this.MurphiesOn.Name = "MurphiesOn";
            this.MurphiesOn.Size = new System.Drawing.Size(128, 21);
            this.MurphiesOn.TabIndex = 132;
            this.MurphiesOn.Text = "Murphies on/off";
            this.MurphiesOn.UseVisualStyleBackColor = true;
            // 
            // ModelsOn
            // 
            this.ModelsOn.AutoSize = true;
            this.ModelsOn.Checked = true;
            this.ModelsOn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ModelsOn.Location = new System.Drawing.Point(28, 121);
            this.ModelsOn.Name = "ModelsOn";
            this.ModelsOn.Size = new System.Drawing.Size(115, 21);
            this.ModelsOn.TabIndex = 54;
            this.ModelsOn.Text = "Models on/off";
            this.ModelsOn.UseVisualStyleBackColor = true;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(25, 85);
            this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(183, 17);
            this.label22.TabIndex = 131;
            this.label22.Text = "Task\'s weight random Level";
            // 
            // cbRandomLevel
            // 
            this.cbRandomLevel.FormattingEnabled = true;
            this.cbRandomLevel.Items.AddRange(new object[] {
            "Not random",
            "Simple",
            "Double",
            "Triple"});
            this.cbRandomLevel.Location = new System.Drawing.Point(264, 82);
            this.cbRandomLevel.Margin = new System.Windows.Forms.Padding(2);
            this.cbRandomLevel.Name = "cbRandomLevel";
            this.cbRandomLevel.Size = new System.Drawing.Size(134, 24);
            this.cbRandomLevel.TabIndex = 130;
            this.cbRandomLevel.Text = "Not random";
            // 
            // NumberOfIterations
            // 
            this.NumberOfIterations.Location = new System.Drawing.Point(161, 53);
            this.NumberOfIterations.Name = "NumberOfIterations";
            this.NumberOfIterations.Size = new System.Drawing.Size(63, 22);
            this.NumberOfIterations.TabIndex = 43;
            this.NumberOfIterations.Text = "50";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 17);
            this.label3.TabIndex = 44;
            this.label3.Text = "Number of iterations";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.symuToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1549, 28);
            this.menuStrip1.TabIndex = 22;
            this.menuStrip1.Text = "Symu";
            // 
            // symuToolStripMenuItem
            // 
            this.symuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.symuorgToolStripMenuItem,
            this.documentationToolStripMenuItem1,
            this.sourceCodeToolStripMenuItem1,
            this.issuesToolStripMenuItem});
            this.symuToolStripMenuItem.Name = "symuToolStripMenuItem";
            this.symuToolStripMenuItem.Size = new System.Drawing.Size(59, 24);
            this.symuToolStripMenuItem.Text = "Symu";
            // 
            // symuorgToolStripMenuItem
            // 
            this.symuorgToolStripMenuItem.Name = "symuorgToolStripMenuItem";
            this.symuorgToolStripMenuItem.Size = new System.Drawing.Size(195, 26);
            this.symuorgToolStripMenuItem.Text = "Symu.org";
            this.symuorgToolStripMenuItem.Click += new System.EventHandler(this.symuorgToolStripMenuItem_Click);
            // 
            // documentationToolStripMenuItem1
            // 
            this.documentationToolStripMenuItem1.Name = "documentationToolStripMenuItem1";
            this.documentationToolStripMenuItem1.Size = new System.Drawing.Size(195, 26);
            this.documentationToolStripMenuItem1.Text = "Documentation";
            this.documentationToolStripMenuItem1.Click += new System.EventHandler(this.documentationToolStripMenuItem1_Click);
            // 
            // sourceCodeToolStripMenuItem1
            // 
            this.sourceCodeToolStripMenuItem1.Name = "sourceCodeToolStripMenuItem1";
            this.sourceCodeToolStripMenuItem1.Size = new System.Drawing.Size(195, 26);
            this.sourceCodeToolStripMenuItem1.Text = "Source code";
            this.sourceCodeToolStripMenuItem1.Click += new System.EventHandler(this.sourceCodeToolStripMenuItem1_Click);
            // 
            // issuesToolStripMenuItem
            // 
            this.issuesToolStripMenuItem.Name = "issuesToolStripMenuItem";
            this.issuesToolStripMenuItem.Size = new System.Drawing.Size(195, 26);
            this.issuesToolStripMenuItem.Text = "Issues";
            this.issuesToolStripMenuItem.Click += new System.EventHandler(this.issuesToolStripMenuItem_Click);
            // 
            // Home
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1549, 860);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Home";
            this.Text = "Scenarios and events models";
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnResume;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.TextBox NumberOfSteps;
        private System.Windows.Forms.TextBox tbWorkers;
        private System.Windows.Forms.Label TimeStep;

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem symuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem symuorgToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem documentationToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem sourceCodeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem issuesToolStripMenuItem;
        private System.Windows.Forms.Label Iteration;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox NumberOfIterations;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label MessagesSent;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label TasksDone;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox NumberOfMessages;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox NumberOfTasks;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.CheckBox AddPerson;
        private System.Windows.Forms.CheckBox AddKnowledge;
        private System.Windows.Forms.TextBox EventStep;
        private System.Windows.Forms.CheckBox TaskBased;
        private System.Windows.Forms.CheckBox MessageBased;
        private System.Windows.Forms.CheckBox TimeBased;
        private Syncfusion.Windows.Forms.Chart.ChartControl chartControl1;
        private Syncfusion.Windows.Forms.Chart.ChartControl chartControl2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbIterations;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.ComboBox cbRandomLevel;
        private System.Windows.Forms.TextBox RandomRatio;
        private System.Windows.Forms.RadioButton rbRandom;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox CyclicalStep;
        private System.Windows.Forms.RadioButton rbCyclical;
        private System.Windows.Forms.RadioButton rbAtStep;
        private System.Windows.Forms.CheckBox MurphiesOn;
        private System.Windows.Forms.CheckBox ModelsOn;
    }
}