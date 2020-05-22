namespace SymuMurphiesAndBlockers
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label28 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.TasksDone = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Capacity = new System.Windows.Forms.Label();
            this.Blockers = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbWorkers = new System.Windows.Forms.TextBox();
            this.btnResume = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.gbUncompleteKnowledge = new System.Windows.Forms.GroupBox();
            this.label21 = new System.Windows.Forms.Label();
            this.tbMaxNumberOfTriesKnowledge = new System.Windows.Forms.TextBox();
            this.KnowledgeOn = new System.Windows.Forms.CheckBox();
            this.KnowledgeRate = new System.Windows.Forms.TextBox();
            this.cbLimitNumberOfTriesKnowledge = new System.Windows.Forms.CheckBox();
            this.label22 = new System.Windows.Forms.Label();
            this.tbRequiredMandatoryRatio = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.tbLackDelayBeforeSearchingExternally = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.tbLackResponseTime = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.tbLackRateOfAnswers = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbLackRateOfIncorrectGuess = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tbKnowledgeThreshHoldForDoing = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.gbBelief = new System.Windows.Forms.GroupBox();
            this.label19 = new System.Windows.Forms.Label();
            this.BeliefsOn = new System.Windows.Forms.CheckBox();
            this.BeliefsRate = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cbLimitNumberOfTriesBelief = new System.Windows.Forms.CheckBox();
            this.tbMaxNumberOfTriesBelief = new System.Windows.Forms.TextBox();
            this.tbBeliefResponseTime = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.tbBeliefRateAnswers = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.tbBeliefRateIncorrectGuess = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label20 = new System.Windows.Forms.Label();
            this.UnavailabilityOn = new System.Windows.Forms.CheckBox();
            this.UnavailabilityRate = new System.Windows.Forms.TextBox();
            this.tbUnavailabilityThreshhold = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.cbMultipleBlockers = new System.Windows.Forms.CheckBox();
            this.tbKnowledge = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tbSteps = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.gbUncompleteKnowledge.SuspendLayout();
            this.gbBelief.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
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
            this.label1.Location = new System.Drawing.Point(253, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Steps";
            // 
            // TimeStep
            // 
            this.TimeStep.AutoSize = true;
            this.TimeStep.Location = new System.Drawing.Point(316, 40);
            this.TimeStep.Name = "TimeStep";
            this.TimeStep.Size = new System.Drawing.Size(16, 17);
            this.TimeStep.TabIndex = 3;
            this.TimeStep.Text = "0";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label28);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Controls.Add(this.TasksDone);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.Capacity);
            this.groupBox3.Controls.Add(this.Blockers);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Location = new System.Drawing.Point(32, 128);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(316, 172);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Beliefs impacts ";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(164, 24);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(47, 17);
            this.label28.TabIndex = 30;
            this.label28.Text = "Actual";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(222, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 17);
            this.label3.TabIndex = 23;
            this.label3.Text = "%";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(26, 116);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(82, 17);
            this.label18.TabIndex = 21;
            this.label18.Text = "Tasks done";
            // 
            // TasksDone
            // 
            this.TasksDone.AutoSize = true;
            this.TasksDone.Location = new System.Drawing.Point(179, 116);
            this.TasksDone.Name = "TasksDone";
            this.TasksDone.Size = new System.Drawing.Size(16, 17);
            this.TasksDone.TabIndex = 22;
            this.TasksDone.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(222, 85);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 17);
            this.label5.TabIndex = 19;
            this.label5.Text = "%";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 17);
            this.label4.TabIndex = 17;
            this.label4.Text = "Total blockers";
            // 
            // Capacity
            // 
            this.Capacity.AutoSize = true;
            this.Capacity.Location = new System.Drawing.Point(179, 85);
            this.Capacity.Name = "Capacity";
            this.Capacity.Size = new System.Drawing.Size(16, 17);
            this.Capacity.TabIndex = 18;
            this.Capacity.Text = "0";
            // 
            // Blockers
            // 
            this.Blockers.AutoSize = true;
            this.Blockers.Location = new System.Drawing.Point(179, 55);
            this.Blockers.Name = "Blockers";
            this.Blockers.Size = new System.Drawing.Size(16, 17);
            this.Blockers.TabIndex = 6;
            this.Blockers.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(26, 55);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(94, 17);
            this.label8.TabIndex = 5;
            this.label8.Text = "Capacity ratio";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "Number of workers";
            // 
            // tbWorkers
            // 
            this.tbWorkers.Location = new System.Drawing.Point(281, 55);
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
            this.richTextBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox2.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.richTextBox2.Location = new System.Drawing.Point(457, 12);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(823, 174);
            this.richTextBox2.TabIndex = 18;
            this.richTextBox2.Text = resources.GetString("richTextBox2.Text");
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.AliceBlue;
            this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.richTextBox1.Location = new System.Drawing.Point(4, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(428, 174);
            this.richTextBox1.TabIndex = 17;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            // 
            // groupBox5
            // 
            this.groupBox5.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.groupBox5.Controls.Add(this.btnResume);
            this.groupBox5.Controls.Add(this.btnPause);
            this.groupBox5.Controls.Add(this.TimeStep);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.btnStop);
            this.groupBox5.Controls.Add(this.btnStart);
            this.groupBox5.Controls.Add(this.groupBox3);
            this.groupBox5.Location = new System.Drawing.Point(857, 203);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(423, 441);
            this.groupBox5.TabIndex = 20;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Run";
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.SystemColors.ControlLight;
            this.groupBox4.Controls.Add(this.gbUncompleteKnowledge);
            this.groupBox4.Controls.Add(this.gbBelief);
            this.groupBox4.Controls.Add(this.groupBox7);
            this.groupBox4.Controls.Add(this.groupBox6);
            this.groupBox4.Location = new System.Drawing.Point(4, 203);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(847, 574);
            this.groupBox4.TabIndex = 21;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Settings";
            // 
            // gbUncompleteKnowledge
            // 
            this.gbUncompleteKnowledge.Controls.Add(this.label21);
            this.gbUncompleteKnowledge.Controls.Add(this.tbMaxNumberOfTriesKnowledge);
            this.gbUncompleteKnowledge.Controls.Add(this.KnowledgeOn);
            this.gbUncompleteKnowledge.Controls.Add(this.KnowledgeRate);
            this.gbUncompleteKnowledge.Controls.Add(this.cbLimitNumberOfTriesKnowledge);
            this.gbUncompleteKnowledge.Controls.Add(this.label22);
            this.gbUncompleteKnowledge.Controls.Add(this.tbRequiredMandatoryRatio);
            this.gbUncompleteKnowledge.Controls.Add(this.label15);
            this.gbUncompleteKnowledge.Controls.Add(this.tbLackDelayBeforeSearchingExternally);
            this.gbUncompleteKnowledge.Controls.Add(this.label14);
            this.gbUncompleteKnowledge.Controls.Add(this.tbLackResponseTime);
            this.gbUncompleteKnowledge.Controls.Add(this.label13);
            this.gbUncompleteKnowledge.Controls.Add(this.tbLackRateOfAnswers);
            this.gbUncompleteKnowledge.Controls.Add(this.label10);
            this.gbUncompleteKnowledge.Controls.Add(this.tbLackRateOfIncorrectGuess);
            this.gbUncompleteKnowledge.Controls.Add(this.label11);
            this.gbUncompleteKnowledge.Controls.Add(this.tbKnowledgeThreshHoldForDoing);
            this.gbUncompleteKnowledge.Controls.Add(this.label9);
            this.gbUncompleteKnowledge.Location = new System.Drawing.Point(21, 169);
            this.gbUncompleteKnowledge.Name = "gbUncompleteKnowledge";
            this.gbUncompleteKnowledge.Size = new System.Drawing.Size(407, 342);
            this.gbUncompleteKnowledge.TabIndex = 158;
            this.gbUncompleteKnowledge.TabStop = false;
            this.gbUncompleteKnowledge.Text = "Incomplete knowledge murphy";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(82, 55);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(121, 17);
            this.label21.TabIndex = 178;
            this.label21.Text = "Rate of agents on";
            // 
            // tbMaxNumberOfTriesKnowledge
            // 
            this.tbMaxNumberOfTriesKnowledge.Location = new System.Drawing.Point(306, 229);
            this.tbMaxNumberOfTriesKnowledge.Name = "tbMaxNumberOfTriesKnowledge";
            this.tbMaxNumberOfTriesKnowledge.Size = new System.Drawing.Size(59, 22);
            this.tbMaxNumberOfTriesKnowledge.TabIndex = 167;
            this.tbMaxNumberOfTriesKnowledge.TextChanged += new System.EventHandler(this.tbMaxNumberOfTriesKnowledge_TextChanged);
            // 
            // KnowledgeOn
            // 
            this.KnowledgeOn.AutoSize = true;
            this.KnowledgeOn.Location = new System.Drawing.Point(11, 31);
            this.KnowledgeOn.Name = "KnowledgeOn";
            this.KnowledgeOn.Size = new System.Drawing.Size(97, 21);
            this.KnowledgeOn.TabIndex = 177;
            this.KnowledgeOn.Text = "Murphy on";
            this.KnowledgeOn.UseVisualStyleBackColor = true;
            // 
            // KnowledgeRate
            // 
            this.KnowledgeRate.Location = new System.Drawing.Point(306, 50);
            this.KnowledgeRate.Name = "KnowledgeRate";
            this.KnowledgeRate.Size = new System.Drawing.Size(59, 22);
            this.KnowledgeRate.TabIndex = 176;
            this.KnowledgeRate.TextChanged += new System.EventHandler(this.KnowledgeRate_TextChanged);
            // 
            // cbLimitNumberOfTriesKnowledge
            // 
            this.cbLimitNumberOfTriesKnowledge.AutoSize = true;
            this.cbLimitNumberOfTriesKnowledge.Location = new System.Drawing.Point(10, 202);
            this.cbLimitNumberOfTriesKnowledge.Name = "cbLimitNumberOfTriesKnowledge";
            this.cbLimitNumberOfTriesKnowledge.Size = new System.Drawing.Size(394, 21);
            this.cbLimitNumberOfTriesKnowledge.TabIndex = 168;
            this.cbLimitNumberOfTriesKnowledge.Text = "Limit number of tries (to have an answer before guessing)";
            this.cbLimitNumberOfTriesKnowledge.UseVisualStyleBackColor = true;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(75, 233);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(136, 17);
            this.label22.TabIndex = 167;
            this.label22.Text = "Max. number of tries";
            // 
            // tbRequiredMandatoryRatio
            // 
            this.tbRequiredMandatoryRatio.Location = new System.Drawing.Point(306, 299);
            this.tbRequiredMandatoryRatio.Name = "tbRequiredMandatoryRatio";
            this.tbRequiredMandatoryRatio.Size = new System.Drawing.Size(59, 22);
            this.tbRequiredMandatoryRatio.TabIndex = 162;
            this.tbRequiredMandatoryRatio.TextChanged += new System.EventHandler(this.tbRequiredMandatoryRatio_TextChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(5, 300);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(169, 17);
            this.label15.TabIndex = 161;
            this.label15.Text = "Required mandatory ratio";
            // 
            // tbLackDelayBeforeSearchingExternally
            // 
            this.tbLackDelayBeforeSearchingExternally.Location = new System.Drawing.Point(306, 271);
            this.tbLackDelayBeforeSearchingExternally.Name = "tbLackDelayBeforeSearchingExternally";
            this.tbLackDelayBeforeSearchingExternally.Size = new System.Drawing.Size(59, 22);
            this.tbLackDelayBeforeSearchingExternally.TabIndex = 160;
            this.tbLackDelayBeforeSearchingExternally.TextChanged += new System.EventHandler(this.tbLackDelayBeforeSearchingExternally_TextChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(5, 272);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(263, 17);
            this.label14.TabIndex = 159;
            this.label14.Text = "Delay before searching externally (days)";
            // 
            // tbLackResponseTime
            // 
            this.tbLackResponseTime.Location = new System.Drawing.Point(306, 174);
            this.tbLackResponseTime.Name = "tbLackResponseTime";
            this.tbLackResponseTime.Size = new System.Drawing.Size(59, 22);
            this.tbLackResponseTime.TabIndex = 158;
            this.tbLackResponseTime.TextChanged += new System.EventHandler(this.tbLackResponseTime_TextChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(5, 175);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(146, 17);
            this.label13.TabIndex = 157;
            this.label13.Text = "Response time (days)";
            // 
            // tbLackRateOfAnswers
            // 
            this.tbLackRateOfAnswers.Location = new System.Drawing.Point(306, 147);
            this.tbLackRateOfAnswers.Name = "tbLackRateOfAnswers";
            this.tbLackRateOfAnswers.Size = new System.Drawing.Size(59, 22);
            this.tbLackRateOfAnswers.TabIndex = 156;
            this.tbLackRateOfAnswers.TextChanged += new System.EventHandler(this.tbLackRateOfAnswers_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(5, 148);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(110, 17);
            this.label10.TabIndex = 155;
            this.label10.Text = "Rate of answers";
            // 
            // tbLackRateOfIncorrectGuess
            // 
            this.tbLackRateOfIncorrectGuess.Location = new System.Drawing.Point(306, 118);
            this.tbLackRateOfIncorrectGuess.Name = "tbLackRateOfIncorrectGuess";
            this.tbLackRateOfIncorrectGuess.Size = new System.Drawing.Size(59, 22);
            this.tbLackRateOfIncorrectGuess.TabIndex = 154;
            this.tbLackRateOfIncorrectGuess.TextChanged += new System.EventHandler(this.tbLackRateOfIncorrectGuess_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(5, 119);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(187, 17);
            this.label11.TabIndex = 153;
            this.label11.Text = "Rate of incorrect guess [0;1]";
            // 
            // tbKnowledgeThreshHoldForDoing
            // 
            this.tbKnowledgeThreshHoldForDoing.Location = new System.Drawing.Point(306, 87);
            this.tbKnowledgeThreshHoldForDoing.Name = "tbKnowledgeThreshHoldForDoing";
            this.tbKnowledgeThreshHoldForDoing.Size = new System.Drawing.Size(59, 22);
            this.tbKnowledgeThreshHoldForDoing.TabIndex = 3;
            this.tbKnowledgeThreshHoldForDoing.TextChanged += new System.EventHandler(this.tbKnowledgeThreshHoldForDoing_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(5, 88);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(242, 17);
            this.label9.TabIndex = 2;
            this.label9.Text = "Knowledge threshHold for doing [0;1]";
            // 
            // gbBelief
            // 
            this.gbBelief.Controls.Add(this.label19);
            this.gbBelief.Controls.Add(this.BeliefsOn);
            this.gbBelief.Controls.Add(this.BeliefsRate);
            this.gbBelief.Controls.Add(this.label6);
            this.gbBelief.Controls.Add(this.cbLimitNumberOfTriesBelief);
            this.gbBelief.Controls.Add(this.tbMaxNumberOfTriesBelief);
            this.gbBelief.Controls.Add(this.tbBeliefResponseTime);
            this.gbBelief.Controls.Add(this.label23);
            this.gbBelief.Controls.Add(this.tbBeliefRateAnswers);
            this.gbBelief.Controls.Add(this.label24);
            this.gbBelief.Controls.Add(this.tbBeliefRateIncorrectGuess);
            this.gbBelief.Controls.Add(this.label25);
            this.gbBelief.Location = new System.Drawing.Point(434, 191);
            this.gbBelief.Name = "gbBelief";
            this.gbBelief.Size = new System.Drawing.Size(407, 239);
            this.gbBelief.TabIndex = 166;
            this.gbBelief.TabStop = false;
            this.gbBelief.Text = "Incomplete belief murphy";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(83, 55);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(121, 17);
            this.label19.TabIndex = 172;
            this.label19.Text = "Rate of agents on";
            // 
            // BeliefsOn
            // 
            this.BeliefsOn.AutoSize = true;
            this.BeliefsOn.Location = new System.Drawing.Point(12, 22);
            this.BeliefsOn.Name = "BeliefsOn";
            this.BeliefsOn.Size = new System.Drawing.Size(97, 21);
            this.BeliefsOn.TabIndex = 171;
            this.BeliefsOn.Text = "Murphy on";
            this.BeliefsOn.UseVisualStyleBackColor = true;
            // 
            // BeliefsRate
            // 
            this.BeliefsRate.Location = new System.Drawing.Point(285, 50);
            this.BeliefsRate.Name = "BeliefsRate";
            this.BeliefsRate.Size = new System.Drawing.Size(63, 22);
            this.BeliefsRate.TabIndex = 170;
            this.BeliefsRate.TextChanged += new System.EventHandler(this.BeliefsRate_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(85, 195);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(136, 17);
            this.label6.TabIndex = 169;
            this.label6.Text = "Max. number of tries";
            // 
            // cbLimitNumberOfTriesBelief
            // 
            this.cbLimitNumberOfTriesBelief.AutoSize = true;
            this.cbLimitNumberOfTriesBelief.Location = new System.Drawing.Point(14, 162);
            this.cbLimitNumberOfTriesBelief.Name = "cbLimitNumberOfTriesBelief";
            this.cbLimitNumberOfTriesBelief.Size = new System.Drawing.Size(394, 21);
            this.cbLimitNumberOfTriesBelief.TabIndex = 166;
            this.cbLimitNumberOfTriesBelief.Text = "Limit number of tries (to have an answer before guessing)";
            this.cbLimitNumberOfTriesBelief.UseVisualStyleBackColor = true;
            // 
            // tbMaxNumberOfTriesBelief
            // 
            this.tbMaxNumberOfTriesBelief.Location = new System.Drawing.Point(287, 190);
            this.tbMaxNumberOfTriesBelief.Name = "tbMaxNumberOfTriesBelief";
            this.tbMaxNumberOfTriesBelief.Size = new System.Drawing.Size(63, 22);
            this.tbMaxNumberOfTriesBelief.TabIndex = 165;
            this.tbMaxNumberOfTriesBelief.TextChanged += new System.EventHandler(this.tbMaxNumberOfTriesBelief_TextChanged);
            // 
            // tbBeliefResponseTime
            // 
            this.tbBeliefResponseTime.Location = new System.Drawing.Point(285, 135);
            this.tbBeliefResponseTime.Name = "tbBeliefResponseTime";
            this.tbBeliefResponseTime.Size = new System.Drawing.Size(63, 22);
            this.tbBeliefResponseTime.TabIndex = 158;
            this.tbBeliefResponseTime.TextChanged += new System.EventHandler(this.tbBeliefResponseTime_TextChanged);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(9, 136);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(146, 17);
            this.label23.TabIndex = 157;
            this.label23.Text = "Response time (days)";
            // 
            // tbBeliefRateAnswers
            // 
            this.tbBeliefRateAnswers.Location = new System.Drawing.Point(285, 108);
            this.tbBeliefRateAnswers.Name = "tbBeliefRateAnswers";
            this.tbBeliefRateAnswers.Size = new System.Drawing.Size(63, 22);
            this.tbBeliefRateAnswers.TabIndex = 156;
            this.tbBeliefRateAnswers.TextChanged += new System.EventHandler(this.tbBeliefRateAnswers_TextChanged);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(9, 109);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(110, 17);
            this.label24.TabIndex = 155;
            this.label24.Text = "Rate of answers";
            // 
            // tbBeliefRateIncorrectGuess
            // 
            this.tbBeliefRateIncorrectGuess.Location = new System.Drawing.Point(285, 79);
            this.tbBeliefRateIncorrectGuess.Name = "tbBeliefRateIncorrectGuess";
            this.tbBeliefRateIncorrectGuess.Size = new System.Drawing.Size(63, 22);
            this.tbBeliefRateIncorrectGuess.TabIndex = 154;
            this.tbBeliefRateIncorrectGuess.TextChanged += new System.EventHandler(this.tbBeliefRateIncorrectGuess_TextChanged);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(9, 80);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(187, 17);
            this.label25.TabIndex = 153;
            this.label25.Text = "Rate of incorrect guess [0;1]";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label20);
            this.groupBox7.Controls.Add(this.UnavailabilityOn);
            this.groupBox7.Controls.Add(this.UnavailabilityRate);
            this.groupBox7.Controls.Add(this.tbUnavailabilityThreshhold);
            this.groupBox7.Controls.Add(this.label17);
            this.groupBox7.Location = new System.Drawing.Point(21, 29);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(404, 134);
            this.groupBox7.TabIndex = 51;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Unavailability murphy";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(89, 57);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(121, 17);
            this.label20.TabIndex = 175;
            this.label20.Text = "Rate of agents on";
            // 
            // UnavailabilityOn
            // 
            this.UnavailabilityOn.AutoSize = true;
            this.UnavailabilityOn.Location = new System.Drawing.Point(18, 33);
            this.UnavailabilityOn.Name = "UnavailabilityOn";
            this.UnavailabilityOn.Size = new System.Drawing.Size(97, 21);
            this.UnavailabilityOn.TabIndex = 174;
            this.UnavailabilityOn.Text = "Murphy on";
            this.UnavailabilityOn.UseVisualStyleBackColor = true;
            // 
            // UnavailabilityRate
            // 
            this.UnavailabilityRate.Location = new System.Drawing.Point(306, 52);
            this.UnavailabilityRate.Name = "UnavailabilityRate";
            this.UnavailabilityRate.Size = new System.Drawing.Size(59, 22);
            this.UnavailabilityRate.TabIndex = 173;
            this.UnavailabilityRate.TextChanged += new System.EventHandler(this.UnavailabilityRate_TextChanged);
            // 
            // tbUnavailabilityThreshhold
            // 
            this.tbUnavailabilityThreshhold.Location = new System.Drawing.Point(306, 85);
            this.tbUnavailabilityThreshhold.Name = "tbUnavailabilityThreshhold";
            this.tbUnavailabilityThreshhold.Size = new System.Drawing.Size(59, 22);
            this.tbUnavailabilityThreshhold.TabIndex = 134;
            this.tbUnavailabilityThreshhold.TextChanged += new System.EventHandler(this.tbUnavailabilityThreshhold_TextChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(15, 86);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(329, 21);
            this.label17.TabIndex = 133;
            this.label17.Text = "Capacity threshold for unavailability [0;1]";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.cbMultipleBlockers);
            this.groupBox6.Controls.Add(this.tbKnowledge);
            this.groupBox6.Controls.Add(this.label12);
            this.groupBox6.Controls.Add(this.tbWorkers);
            this.groupBox6.Controls.Add(this.label2);
            this.groupBox6.Controls.Add(this.tbSteps);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Location = new System.Drawing.Point(438, 29);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(374, 156);
            this.groupBox6.TabIndex = 18;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Symu";
            // 
            // cbMultipleBlockers
            // 
            this.cbMultipleBlockers.AutoSize = true;
            this.cbMultipleBlockers.Location = new System.Drawing.Point(28, 82);
            this.cbMultipleBlockers.Name = "cbMultipleBlockers";
            this.cbMultipleBlockers.Size = new System.Drawing.Size(171, 21);
            this.cbMultipleBlockers.TabIndex = 151;
            this.cbMultipleBlockers.Text = "Allow multiple blockers";
            this.cbMultipleBlockers.UseVisualStyleBackColor = true;
            // 
            // tbKnowledge
            // 
            this.tbKnowledge.Location = new System.Drawing.Point(281, 22);
            this.tbKnowledge.Name = "tbKnowledge";
            this.tbKnowledge.Size = new System.Drawing.Size(63, 22);
            this.tbKnowledge.TabIndex = 44;
            this.tbKnowledge.Text = "5";
            this.tbKnowledge.TextChanged += new System.EventHandler(this.tbKnowledge_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(25, 27);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(145, 17);
            this.label12.TabIndex = 43;
            this.label12.Text = "Number of knowledge";
            // 
            // tbSteps
            // 
            this.tbSteps.Location = new System.Drawing.Point(281, 115);
            this.tbSteps.Name = "tbSteps";
            this.tbSteps.Size = new System.Drawing.Size(63, 22);
            this.tbSteps.TabIndex = 39;
            this.tbSteps.Text = "200";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(25, 117);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(112, 17);
            this.label7.TabIndex = 42;
            this.label7.Text = "Number of steps";
            // 
            // Home
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1296, 789);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.richTextBox1);
            this.Name = "Home";
            this.Text = "Murphies and blockers models";
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.gbUncompleteKnowledge.ResumeLayout(false);
            this.gbUncompleteKnowledge.PerformLayout();
            this.gbBelief.ResumeLayout(false);
            this.gbBelief.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label TimeStep;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbWorkers;
        private System.Windows.Forms.Button btnResume;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox tbSteps;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label Capacity;
        private System.Windows.Forms.Label Blockers;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label TasksDone;
        private System.Windows.Forms.TextBox tbKnowledge;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.GroupBox gbBelief;
        private System.Windows.Forms.CheckBox cbLimitNumberOfTriesBelief;
        private System.Windows.Forms.TextBox tbMaxNumberOfTriesBelief;
        private System.Windows.Forms.TextBox tbBeliefResponseTime;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox tbBeliefRateAnswers;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox tbBeliefRateIncorrectGuess;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.GroupBox gbUncompleteKnowledge;
        private System.Windows.Forms.TextBox tbMaxNumberOfTriesKnowledge;
        private System.Windows.Forms.CheckBox cbLimitNumberOfTriesKnowledge;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox tbRequiredMandatoryRatio;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox tbLackDelayBeforeSearchingExternally;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox tbLackResponseTime;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tbLackRateOfAnswers;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbLackRateOfIncorrectGuess;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbKnowledgeThreshHoldForDoing;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbUnavailabilityThreshhold;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.CheckBox cbMultipleBlockers;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.CheckBox BeliefsOn;
        private System.Windows.Forms.TextBox BeliefsRate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.CheckBox UnavailabilityOn;
        private System.Windows.Forms.TextBox UnavailabilityRate;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.CheckBox KnowledgeOn;
        private System.Windows.Forms.TextBox KnowledgeRate;
    }
}