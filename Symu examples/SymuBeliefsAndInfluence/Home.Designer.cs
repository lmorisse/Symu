namespace SymuBeliefsAndInfluence
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
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.TimeStep = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.TasksDone = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Triads = new System.Windows.Forms.Label();
            this.TotalBeliefs = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbWorkers = new System.Windows.Forms.TextBox();
            this.btnResume = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.InfluenceabilityMax = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.InfluenceabilityMin = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.InfluentialnessMax = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.InfluentialnessMin = new System.Windows.Forms.TextBox();
            this.CanReceiveBeliefs = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.CanSendBeliefs = new System.Windows.Forms.CheckBox();
            this.HasInitialBeliefs = new System.Windows.Forms.CheckBox();
            this.HasBeliefs = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.tbKnowledge = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tbSteps = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.MinimumBeliefToSendPerBit = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.MinimumNumberOfBitsOfBeliefToSend = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.MaximumNumberOfBitsOfBeliefToSend = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox1.SuspendLayout();
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
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Controls.Add(this.TasksDone);
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.Triads);
            this.groupBox3.Controls.Add(this.TotalBeliefs);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Location = new System.Drawing.Point(32, 128);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(305, 161);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Beliefs impacts ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(256, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 17);
            this.label3.TabIndex = 23;
            this.label3.Text = "%";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(26, 98);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(82, 17);
            this.label18.TabIndex = 21;
            this.label18.Text = "Tasks done";
            // 
            // TasksDone
            // 
            this.TasksDone.AutoSize = true;
            this.TasksDone.Location = new System.Drawing.Point(224, 98);
            this.TasksDone.Name = "TasksDone";
            this.TasksDone.Size = new System.Drawing.Size(16, 17);
            this.TasksDone.TabIndex = 22;
            this.TasksDone.Text = "0";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(256, 37);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(20, 17);
            this.label19.TabIndex = 20;
            this.label19.Text = "%";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(256, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 17);
            this.label5.TabIndex = 19;
            this.label5.Text = "%";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 17);
            this.label4.TabIndex = 17;
            this.label4.Text = "Triads density";
            // 
            // Triads
            // 
            this.Triads.AutoSize = true;
            this.Triads.Location = new System.Drawing.Point(224, 67);
            this.Triads.Name = "Triads";
            this.Triads.Size = new System.Drawing.Size(16, 17);
            this.Triads.TabIndex = 18;
            this.Triads.Text = "0";
            // 
            // TotalBeliefs
            // 
            this.TotalBeliefs.AutoSize = true;
            this.TotalBeliefs.Location = new System.Drawing.Point(224, 37);
            this.TotalBeliefs.Name = "TotalBeliefs";
            this.TotalBeliefs.Size = new System.Drawing.Size(16, 17);
            this.TotalBeliefs.TabIndex = 6;
            this.TotalBeliefs.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(26, 37);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(85, 17);
            this.label8.TabIndex = 5;
            this.label8.Text = "Total beliefs";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "Number of people";
            // 
            // tbWorkers
            // 
            this.tbWorkers.Location = new System.Drawing.Point(242, 55);
            this.tbWorkers.Name = "tbWorkers";
            this.tbWorkers.Size = new System.Drawing.Size(87, 22);
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
            this.richTextBox2.Location = new System.Drawing.Point(457, 12);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(823, 174);
            this.richTextBox2.TabIndex = 18;
            this.richTextBox2.Text = "Scenario:\n* People with beliefs who try to influence others\n\nBeliefs have an impa" +
    "ct on different parameters : perform tasks, modify the sphere of interaction, ag" +
    "ents beliefs.";
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.AliceBlue;
            this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.richTextBox1.Location = new System.Drawing.Point(4, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(428, 174);
            this.richTextBox1.TabIndex = 17;
            this.richTextBox1.Text = "Goal:\nBeliefs and influence models are implemented in Symu.\nThe objective of this" +
    " example is to show how to use, configure and see the impacts of those models on" +
    " agents.";
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
            this.groupBox5.Location = new System.Drawing.Point(920, 203);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(360, 575);
            this.groupBox5.TabIndex = 20;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Run";
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.SystemColors.ControlLight;
            this.groupBox4.Controls.Add(this.groupBox1);
            this.groupBox4.Controls.Add(this.groupBox9);
            this.groupBox4.Controls.Add(this.groupBox6);
            this.groupBox4.Location = new System.Drawing.Point(4, 203);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(900, 575);
            this.groupBox4.TabIndex = 21;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Settings";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.MaximumNumberOfBitsOfBeliefToSend);
            this.groupBox9.Controls.Add(this.label15);
            this.groupBox9.Controls.Add(this.MinimumNumberOfBitsOfBeliefToSend);
            this.groupBox9.Controls.Add(this.label14);
            this.groupBox9.Controls.Add(this.MinimumBeliefToSendPerBit);
            this.groupBox9.Controls.Add(this.CanReceiveBeliefs);
            this.groupBox9.Controls.Add(this.label13);
            this.groupBox9.Controls.Add(this.CanSendBeliefs);
            this.groupBox9.Controls.Add(this.HasInitialBeliefs);
            this.groupBox9.Controls.Add(this.HasBeliefs);
            this.groupBox9.Location = new System.Drawing.Point(29, 21);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(374, 289);
            this.groupBox9.TabIndex = 133;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Agent beliefs";
            // 
            // InfluenceabilityMax
            // 
            this.InfluenceabilityMax.Location = new System.Drawing.Point(269, 56);
            this.InfluenceabilityMax.Name = "InfluenceabilityMax";
            this.InfluenceabilityMax.Size = new System.Drawing.Size(53, 22);
            this.InfluenceabilityMax.TabIndex = 139;
            this.InfluenceabilityMax.Text = "2";
            this.InfluenceabilityMax.TextChanged += new System.EventHandler(this.InfluenceabilityMax_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(229, 59);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(37, 17);
            this.label10.TabIndex = 138;
            this.label10.Text = "max.";
            // 
            // InfluenceabilityMin
            // 
            this.InfluenceabilityMin.Location = new System.Drawing.Point(170, 56);
            this.InfluenceabilityMin.Name = "InfluenceabilityMin";
            this.InfluenceabilityMin.Size = new System.Drawing.Size(53, 22);
            this.InfluenceabilityMin.TabIndex = 137;
            this.InfluenceabilityMin.Text = "2";
            this.InfluenceabilityMin.TextChanged += new System.EventHandler(this.InfluenceabilityMin_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(31, 59);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(135, 17);
            this.label11.TabIndex = 136;
            this.label11.Text = "Influenceability: min.";
            // 
            // InfluentialnessMax
            // 
            this.InfluentialnessMax.Location = new System.Drawing.Point(269, 27);
            this.InfluentialnessMax.Name = "InfluentialnessMax";
            this.InfluentialnessMax.Size = new System.Drawing.Size(53, 22);
            this.InfluentialnessMax.TabIndex = 135;
            this.InfluentialnessMax.Text = "2";
            this.InfluentialnessMax.TextChanged += new System.EventHandler(this.InfluentialnessMax_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(229, 30);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 17);
            this.label9.TabIndex = 134;
            this.label9.Text = "max.";
            // 
            // InfluentialnessMin
            // 
            this.InfluentialnessMin.Location = new System.Drawing.Point(170, 27);
            this.InfluentialnessMin.Name = "InfluentialnessMin";
            this.InfluentialnessMin.Size = new System.Drawing.Size(53, 22);
            this.InfluentialnessMin.TabIndex = 133;
            this.InfluentialnessMin.Text = "2";
            this.InfluentialnessMin.TextChanged += new System.EventHandler(this.InfluentialnessMin_TextChanged);
            // 
            // CanReceiveBeliefs
            // 
            this.CanReceiveBeliefs.AutoSize = true;
            this.CanReceiveBeliefs.Location = new System.Drawing.Point(34, 111);
            this.CanReceiveBeliefs.Name = "CanReceiveBeliefs";
            this.CanReceiveBeliefs.Size = new System.Drawing.Size(150, 21);
            this.CanReceiveBeliefs.TabIndex = 3;
            this.CanReceiveBeliefs.Text = "Can receive beliefs";
            this.CanReceiveBeliefs.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(31, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(132, 17);
            this.label6.TabIndex = 132;
            this.label6.Text = "Influentialness: min.";
            // 
            // CanSendBeliefs
            // 
            this.CanSendBeliefs.AutoSize = true;
            this.CanSendBeliefs.Location = new System.Drawing.Point(34, 84);
            this.CanSendBeliefs.Name = "CanSendBeliefs";
            this.CanSendBeliefs.Size = new System.Drawing.Size(135, 21);
            this.CanSendBeliefs.TabIndex = 2;
            this.CanSendBeliefs.Text = "Can send beliefs";
            this.CanSendBeliefs.UseVisualStyleBackColor = true;
            // 
            // HasInitialBeliefs
            // 
            this.HasInitialBeliefs.AutoSize = true;
            this.HasInitialBeliefs.Location = new System.Drawing.Point(83, 57);
            this.HasInitialBeliefs.Name = "HasInitialBeliefs";
            this.HasInitialBeliefs.Size = new System.Drawing.Size(136, 21);
            this.HasInitialBeliefs.TabIndex = 1;
            this.HasInitialBeliefs.Text = "Has initial beliefs";
            this.HasInitialBeliefs.UseVisualStyleBackColor = true;
            // 
            // HasBeliefs
            // 
            this.HasBeliefs.AutoSize = true;
            this.HasBeliefs.Location = new System.Drawing.Point(34, 30);
            this.HasBeliefs.Name = "HasBeliefs";
            this.HasBeliefs.Size = new System.Drawing.Size(100, 21);
            this.HasBeliefs.TabIndex = 0;
            this.HasBeliefs.Text = "Has beliefs";
            this.HasBeliefs.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.tbKnowledge);
            this.groupBox6.Controls.Add(this.label12);
            this.groupBox6.Controls.Add(this.tbWorkers);
            this.groupBox6.Controls.Add(this.label2);
            this.groupBox6.Controls.Add(this.tbSteps);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Location = new System.Drawing.Point(453, 306);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(374, 152);
            this.groupBox6.TabIndex = 18;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Simulation";
            // 
            // tbKnowledge
            // 
            this.tbKnowledge.Location = new System.Drawing.Point(242, 22);
            this.tbKnowledge.Name = "tbKnowledge";
            this.tbKnowledge.Size = new System.Drawing.Size(87, 22);
            this.tbKnowledge.TabIndex = 44;
            this.tbKnowledge.Text = "5";
            this.tbKnowledge.TextChanged += new System.EventHandler(this.tbKnowledge_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(25, 27);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(119, 17);
            this.label12.TabIndex = 43;
            this.label12.Text = "Number of beliefs";
            // 
            // tbSteps
            // 
            this.tbSteps.Location = new System.Drawing.Point(242, 87);
            this.tbSteps.Name = "tbSteps";
            this.tbSteps.Size = new System.Drawing.Size(87, 22);
            this.tbSteps.TabIndex = 39;
            this.tbSteps.Text = "200";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(25, 92);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(112, 17);
            this.label7.TabIndex = 42;
            this.label7.Text = "Number of steps";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.InfluenceabilityMax);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.InfluentialnessMin);
            this.groupBox1.Controls.Add(this.InfluenceabilityMin);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.InfluentialnessMax);
            this.groupBox1.Location = new System.Drawing.Point(29, 345);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(374, 100);
            this.groupBox1.TabIndex = 134;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Agent influence";
            // 
            // MinimumBeliefToSendPerBit
            // 
            this.MinimumBeliefToSendPerBit.Location = new System.Drawing.Point(248, 157);
            this.MinimumBeliefToSendPerBit.Name = "MinimumBeliefToSendPerBit";
            this.MinimumBeliefToSendPerBit.Size = new System.Drawing.Size(87, 22);
            this.MinimumBeliefToSendPerBit.TabIndex = 46;
            this.MinimumBeliefToSendPerBit.Text = "5";
            this.MinimumBeliefToSendPerBit.TextChanged += new System.EventHandler(this.MinimumBeliefToSendPerBit_TextChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(31, 159);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(188, 17);
            this.label13.TabIndex = 45;
            this.label13.Text = "Minimum belief to send  [0;1]";
            // 
            // MinimumNumberOfBitsOfBeliefToSend
            // 
            this.MinimumNumberOfBitsOfBeliefToSend.Location = new System.Drawing.Point(248, 187);
            this.MinimumNumberOfBitsOfBeliefToSend.Name = "MinimumNumberOfBitsOfBeliefToSend";
            this.MinimumNumberOfBitsOfBeliefToSend.Size = new System.Drawing.Size(87, 22);
            this.MinimumNumberOfBitsOfBeliefToSend.TabIndex = 48;
            this.MinimumNumberOfBitsOfBeliefToSend.Text = "5";
            this.MinimumNumberOfBitsOfBeliefToSend.TextChanged += new System.EventHandler(this.MinimumNumberOfBitsOfBeliefToSend_TextChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(31, 189);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(251, 21);
            this.label14.TabIndex = 47;
            this.label14.Text = "Minimum number of bits [0; 10]";
            // 
            // MaximumNumberOfBitsOfBeliefToSend
            // 
            this.MaximumNumberOfBitsOfBeliefToSend.Location = new System.Drawing.Point(248, 216);
            this.MaximumNumberOfBitsOfBeliefToSend.Name = "MaximumNumberOfBitsOfBeliefToSend";
            this.MaximumNumberOfBitsOfBeliefToSend.Size = new System.Drawing.Size(87, 22);
            this.MaximumNumberOfBitsOfBeliefToSend.TabIndex = 50;
            this.MaximumNumberOfBitsOfBeliefToSend.Text = "5";
            this.MaximumNumberOfBitsOfBeliefToSend.TextChanged += new System.EventHandler(this.MaximumNumberOfBitsOfBeliefToSend_TextChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(31, 218);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(255, 21);
            this.label15.TabIndex = 49;
            this.label15.Text = "Maximum number of bits [0; 10]";
            // 
            // Home
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1324, 825);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.richTextBox1);
            this.Name = "Home";
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label Triads;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label TotalBeliefs;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label TasksDone;
        private System.Windows.Forms.CheckBox CanReceiveBeliefs;
        private System.Windows.Forms.CheckBox CanSendBeliefs;
        private System.Windows.Forms.CheckBox HasInitialBeliefs;
        private System.Windows.Forms.CheckBox HasBeliefs;
        private System.Windows.Forms.TextBox InfluentialnessMax;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox InfluentialnessMin;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox InfluenceabilityMax;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox InfluenceabilityMin;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbKnowledge;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox MinimumBeliefToSendPerBit;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox MaximumNumberOfBitsOfBeliefToSend;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox MinimumNumberOfBitsOfBeliefToSend;
        private System.Windows.Forms.Label label14;
    }
}