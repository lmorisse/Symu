namespace SymuMessageAndTask
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblMessagesSent = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.TasksWeight = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.TasksDone = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.TasksInProgress = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.TasksToDo = new System.Windows.Forms.Label();
            this.lblWorked = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.TasksTotal = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbWorkers = new System.Windows.Forms.TextBox();
            this.btnResume = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.CostToReceive = new System.Windows.Forms.ComboBox();
            this.CostToSend = new System.Windows.Forms.ComboBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.LimitMessagesReceived = new System.Windows.Forms.CheckBox();
            this.MaxMessagesReceived = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.LimitMessagesSent = new System.Windows.Forms.CheckBox();
            this.MaxMessagesSent = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.LimitMessages = new System.Windows.Forms.CheckBox();
            this.MaxMessages = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label22 = new System.Windows.Forms.Label();
            this.tbSteps = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cbRandomLevel = new System.Windows.Forms.ComboBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.SwitchingContextCost = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.costOfTask = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.numberTasksSent = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.CanPerformTasksOnWeekends = new System.Windows.Forms.CheckBox();
            this.AgentCanBeIsolated = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.InitialCapacity = new System.Windows.Forms.TextBox();
            this.LimitSimultaneousTasks = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.MaxSimultaneousTasks = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.LimitNumberTasks = new System.Windows.Forms.CheckBox();
            this.maxNumberTasks = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.CanPerformTask = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox12.SuspendLayout();
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblMessagesSent);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(32, 141);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(305, 70);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Messages";
            // 
            // lblMessagesSent
            // 
            this.lblMessagesSent.AutoSize = true;
            this.lblMessagesSent.Location = new System.Drawing.Point(138, 30);
            this.lblMessagesSent.Name = "lblMessagesSent";
            this.lblMessagesSent.Size = new System.Drawing.Size(16, 17);
            this.lblMessagesSent.TabIndex = 6;
            this.lblMessagesSent.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Messages sent";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.TasksWeight);
            this.groupBox3.Controls.Add(this.label23);
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Controls.Add(this.TasksDone);
            this.groupBox3.Controls.Add(this.label17);
            this.groupBox3.Controls.Add(this.TasksInProgress);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.TasksToDo);
            this.groupBox3.Controls.Add(this.lblWorked);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.TasksTotal);
            this.groupBox3.Location = new System.Drawing.Point(32, 274);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(305, 248);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Tasks";
            // 
            // TasksWeight
            // 
            this.TasksWeight.AutoSize = true;
            this.TasksWeight.Location = new System.Drawing.Point(222, 184);
            this.TasksWeight.Name = "TasksWeight";
            this.TasksWeight.Size = new System.Drawing.Size(16, 17);
            this.TasksWeight.TabIndex = 24;
            this.TasksWeight.Text = "0";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(24, 184);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(157, 17);
            this.label23.TabIndex = 23;
            this.label23.Text = "Total weight tasks done";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(62, 122);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(112, 17);
            this.label19.TabIndex = 21;
            this.label19.Text = "Done  (average)";
            // 
            // TasksDone
            // 
            this.TasksDone.AutoSize = true;
            this.TasksDone.Location = new System.Drawing.Point(224, 122);
            this.TasksDone.Name = "TasksDone";
            this.TasksDone.Size = new System.Drawing.Size(16, 17);
            this.TasksDone.TabIndex = 22;
            this.TasksDone.Text = "0";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(62, 96);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(149, 17);
            this.label17.TabIndex = 19;
            this.label17.Text = "In progress  (average)";
            // 
            // TasksInProgress
            // 
            this.TasksInProgress.AutoSize = true;
            this.TasksInProgress.Location = new System.Drawing.Point(224, 96);
            this.TasksInProgress.Name = "TasksInProgress";
            this.TasksInProgress.Size = new System.Drawing.Size(16, 17);
            this.TasksInProgress.TabIndex = 20;
            this.TasksInProgress.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(62, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 17);
            this.label4.TabIndex = 17;
            this.label4.Text = "Todo (average)";
            // 
            // TasksToDo
            // 
            this.TasksToDo.AutoSize = true;
            this.TasksToDo.Location = new System.Drawing.Point(224, 67);
            this.TasksToDo.Name = "TasksToDo";
            this.TasksToDo.Size = new System.Drawing.Size(16, 17);
            this.TasksToDo.TabIndex = 18;
            this.TasksToDo.Text = "0";
            // 
            // lblWorked
            // 
            this.lblWorked.AutoSize = true;
            this.lblWorked.Location = new System.Drawing.Point(222, 158);
            this.lblWorked.Name = "lblWorked";
            this.lblWorked.Size = new System.Drawing.Size(16, 17);
            this.lblWorked.TabIndex = 6;
            this.lblWorked.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(24, 158);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(96, 17);
            this.label8.TabIndex = 5;
            this.label8.Text = "Total capacity";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 38);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 17);
            this.label5.TabIndex = 15;
            this.label5.Text = "Total tasks";
            // 
            // TasksTotal
            // 
            this.TasksTotal.AutoSize = true;
            this.TasksTotal.Location = new System.Drawing.Point(222, 38);
            this.TasksTotal.Name = "TasksTotal";
            this.TasksTotal.Size = new System.Drawing.Size(16, 17);
            this.TasksTotal.TabIndex = 16;
            this.TasksTotal.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "Number of workers";
            // 
            // tbWorkers
            // 
            this.tbWorkers.Location = new System.Drawing.Point(242, 39);
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
            this.richTextBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox2.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.richTextBox2.Location = new System.Drawing.Point(457, 12);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(823, 151);
            this.richTextBox2.TabIndex = 18;
            this.richTextBox2.Text = resources.GetString("richTextBox2.Text");
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.AliceBlue;
            this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.richTextBox1.Location = new System.Drawing.Point(4, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(428, 151);
            this.richTextBox1.TabIndex = 17;
            this.richTextBox1.Text = "Goal:\nMessaging and tasking models are implemented in Symu.\nThe objective of this" +
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
            this.groupBox5.Controls.Add(this.groupBox1);
            this.groupBox5.Location = new System.Drawing.Point(920, 169);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(360, 540);
            this.groupBox5.TabIndex = 20;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Run";
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.SystemColors.ControlLight;
            this.groupBox4.Controls.Add(this.groupBox11);
            this.groupBox4.Controls.Add(this.groupBox6);
            this.groupBox4.Controls.Add(this.groupBox10);
            this.groupBox4.Location = new System.Drawing.Point(4, 169);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(900, 540);
            this.groupBox4.TabIndex = 21;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Settings";
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.CostToReceive);
            this.groupBox11.Controls.Add(this.CostToSend);
            this.groupBox11.Controls.Add(this.label20);
            this.groupBox11.Controls.Add(this.label18);
            this.groupBox11.Controls.Add(this.LimitMessagesReceived);
            this.groupBox11.Controls.Add(this.MaxMessagesReceived);
            this.groupBox11.Controls.Add(this.label15);
            this.groupBox11.Controls.Add(this.LimitMessagesSent);
            this.groupBox11.Controls.Add(this.MaxMessagesSent);
            this.groupBox11.Controls.Add(this.label14);
            this.groupBox11.Controls.Add(this.LimitMessages);
            this.groupBox11.Controls.Add(this.MaxMessages);
            this.groupBox11.Controls.Add(this.label13);
            this.groupBox11.Location = new System.Drawing.Point(453, 34);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(374, 346);
            this.groupBox11.TabIndex = 16;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Messaging model";
            // 
            // CostToReceive
            // 
            this.CostToReceive.FormattingEnabled = true;
            this.CostToReceive.Location = new System.Drawing.Point(268, 301);
            this.CostToReceive.Margin = new System.Windows.Forms.Padding(2);
            this.CostToReceive.Name = "CostToReceive";
            this.CostToReceive.Size = new System.Drawing.Size(87, 24);
            this.CostToReceive.TabIndex = 132;
            // 
            // CostToSend
            // 
            this.CostToSend.FormattingEnabled = true;
            this.CostToSend.Location = new System.Drawing.Point(268, 273);
            this.CostToSend.Margin = new System.Windows.Forms.Padding(2);
            this.CostToSend.Name = "CostToSend";
            this.CostToSend.Size = new System.Drawing.Size(87, 24);
            this.CostToSend.TabIndex = 131;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(51, 304);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(163, 17);
            this.label20.TabIndex = 44;
            this.label20.Text = "Cost to receive message";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(51, 276);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(148, 17);
            this.label18.TabIndex = 42;
            this.label18.Text = "Cost to send message";
            // 
            // LimitMessagesReceived
            // 
            this.LimitMessagesReceived.AutoSize = true;
            this.LimitMessagesReceived.Location = new System.Drawing.Point(15, 183);
            this.LimitMessagesReceived.Name = "LimitMessagesReceived";
            this.LimitMessagesReceived.Size = new System.Drawing.Size(241, 21);
            this.LimitMessagesReceived.TabIndex = 41;
            this.LimitMessagesReceived.Text = "Limit messages received per step";
            this.LimitMessagesReceived.UseVisualStyleBackColor = true;
            // 
            // MaxMessagesReceived
            // 
            this.MaxMessagesReceived.Location = new System.Drawing.Point(268, 218);
            this.MaxMessagesReceived.Name = "MaxMessagesReceived";
            this.MaxMessagesReceived.Size = new System.Drawing.Size(87, 22);
            this.MaxMessagesReceived.TabIndex = 40;
            this.MaxMessagesReceived.TextChanged += new System.EventHandler(this.MaxMessagesReceived_TextChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(51, 219);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(163, 17);
            this.label15.TabIndex = 39;
            this.label15.Text = "Max. messages received";
            // 
            // LimitMessagesSent
            // 
            this.LimitMessagesSent.AutoSize = true;
            this.LimitMessagesSent.Location = new System.Drawing.Point(15, 107);
            this.LimitMessagesSent.Name = "LimitMessagesSent";
            this.LimitMessagesSent.Size = new System.Drawing.Size(214, 21);
            this.LimitMessagesSent.TabIndex = 38;
            this.LimitMessagesSent.Text = "Limit messages sent per step";
            this.LimitMessagesSent.UseVisualStyleBackColor = true;
            // 
            // MaxMessagesSent
            // 
            this.MaxMessagesSent.Location = new System.Drawing.Point(268, 142);
            this.MaxMessagesSent.Name = "MaxMessagesSent";
            this.MaxMessagesSent.Size = new System.Drawing.Size(87, 22);
            this.MaxMessagesSent.TabIndex = 37;
            this.MaxMessagesSent.TextChanged += new System.EventHandler(this.MaxMessagesSent_TextChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(51, 143);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(136, 17);
            this.label14.TabIndex = 36;
            this.label14.Text = "Max. messages sent";
            // 
            // LimitMessages
            // 
            this.LimitMessages.AutoSize = true;
            this.LimitMessages.Location = new System.Drawing.Point(15, 42);
            this.LimitMessages.Name = "LimitMessages";
            this.LimitMessages.Size = new System.Drawing.Size(183, 21);
            this.LimitMessages.TabIndex = 35;
            this.LimitMessages.Text = "Limit messages per step";
            this.LimitMessages.UseVisualStyleBackColor = true;
            // 
            // MaxMessages
            // 
            this.MaxMessages.Location = new System.Drawing.Point(268, 77);
            this.MaxMessages.Name = "MaxMessages";
            this.MaxMessages.Size = new System.Drawing.Size(87, 22);
            this.MaxMessages.TabIndex = 34;
            this.MaxMessages.TextChanged += new System.EventHandler(this.MaxMessages_TextChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(51, 78);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(105, 17);
            this.label13.TabIndex = 33;
            this.label13.Text = "Max. messages";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.tbWorkers);
            this.groupBox6.Controls.Add(this.label2);
            this.groupBox6.Controls.Add(this.label22);
            this.groupBox6.Controls.Add(this.tbSteps);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Controls.Add(this.cbRandomLevel);
            this.groupBox6.Location = new System.Drawing.Point(453, 386);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(374, 136);
            this.groupBox6.TabIndex = 18;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Simulation";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(51, 109);
            this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(99, 17);
            this.label22.TabIndex = 129;
            this.label22.Text = "Random Level";
            // 
            // tbSteps
            // 
            this.tbSteps.Location = new System.Drawing.Point(242, 71);
            this.tbSteps.Name = "tbSteps";
            this.tbSteps.Size = new System.Drawing.Size(87, 22);
            this.tbSteps.TabIndex = 39;
            this.tbSteps.Text = "500";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(51, 76);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(112, 17);
            this.label7.TabIndex = 42;
            this.label7.Text = "Number of steps";
            // 
            // cbRandomLevel
            // 
            this.cbRandomLevel.FormattingEnabled = true;
            this.cbRandomLevel.Items.AddRange(new object[] {
            "Not random",
            "Simple",
            "Double",
            "Triple"});
            this.cbRandomLevel.Location = new System.Drawing.Point(242, 106);
            this.cbRandomLevel.Margin = new System.Windows.Forms.Padding(2);
            this.cbRandomLevel.Name = "cbRandomLevel";
            this.cbRandomLevel.Size = new System.Drawing.Size(87, 24);
            this.cbRandomLevel.TabIndex = 128;
            this.cbRandomLevel.Text = "Not random";
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.groupBox7);
            this.groupBox10.Controls.Add(this.groupBox12);
            this.groupBox10.Location = new System.Drawing.Point(25, 31);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(403, 491);
            this.groupBox10.TabIndex = 13;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Tasking model";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.SwitchingContextCost);
            this.groupBox7.Controls.Add(this.label21);
            this.groupBox7.Controls.Add(this.costOfTask);
            this.groupBox7.Controls.Add(this.label9);
            this.groupBox7.Controls.Add(this.numberTasksSent);
            this.groupBox7.Controls.Add(this.label10);
            this.groupBox7.Location = new System.Drawing.Point(20, 355);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(374, 136);
            this.groupBox7.TabIndex = 32;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Task";
            // 
            // SwitchingContextCost
            // 
            this.SwitchingContextCost.Location = new System.Drawing.Point(281, 106);
            this.SwitchingContextCost.Name = "SwitchingContextCost";
            this.SwitchingContextCost.Size = new System.Drawing.Size(87, 22);
            this.SwitchingContextCost.TabIndex = 32;
            this.SwitchingContextCost.TextChanged += new System.EventHandler(this.SwitchingContextCost_TextChanged);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(25, 106);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(146, 17);
            this.label21.TabIndex = 31;
            this.label21.Text = "Switching context cost";
            // 
            // costOfTask
            // 
            this.costOfTask.Location = new System.Drawing.Point(281, 76);
            this.costOfTask.Name = "costOfTask";
            this.costOfTask.Size = new System.Drawing.Size(87, 22);
            this.costOfTask.TabIndex = 30;
            this.costOfTask.TextChanged += new System.EventHandler(this.costOfTask_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(25, 76);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(82, 17);
            this.label9.TabIndex = 29;
            this.label9.Text = "Cost of task";
            // 
            // numberTasksSent
            // 
            this.numberTasksSent.Location = new System.Drawing.Point(281, 42);
            this.numberTasksSent.Name = "numberTasksSent";
            this.numberTasksSent.Size = new System.Drawing.Size(87, 22);
            this.numberTasksSent.TabIndex = 5;
            this.numberTasksSent.TextChanged += new System.EventHandler(this.numberTasksSent_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(25, 42);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(262, 17);
            this.label10.TabIndex = 4;
            this.label10.Text = "Number of tasks send by group per step";
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.CanPerformTasksOnWeekends);
            this.groupBox12.Controls.Add(this.AgentCanBeIsolated);
            this.groupBox12.Controls.Add(this.label6);
            this.groupBox12.Controls.Add(this.InitialCapacity);
            this.groupBox12.Controls.Add(this.LimitSimultaneousTasks);
            this.groupBox12.Controls.Add(this.label11);
            this.groupBox12.Controls.Add(this.MaxSimultaneousTasks);
            this.groupBox12.Controls.Add(this.label12);
            this.groupBox12.Controls.Add(this.LimitNumberTasks);
            this.groupBox12.Controls.Add(this.maxNumberTasks);
            this.groupBox12.Controls.Add(this.label16);
            this.groupBox12.Controls.Add(this.CanPerformTask);
            this.groupBox12.Location = new System.Drawing.Point(20, 32);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(374, 290);
            this.groupBox12.TabIndex = 16;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Agent";
            // 
            // CanPerformTasksOnWeekends
            // 
            this.CanPerformTasksOnWeekends.AutoSize = true;
            this.CanPerformTasksOnWeekends.Location = new System.Drawing.Point(79, 53);
            this.CanPerformTasksOnWeekends.Name = "CanPerformTasksOnWeekends";
            this.CanPerformTasksOnWeekends.Size = new System.Drawing.Size(153, 21);
            this.CanPerformTasksOnWeekends.TabIndex = 131;
            this.CanPerformTasksOnWeekends.Text = "including weekends";
            this.CanPerformTasksOnWeekends.UseVisualStyleBackColor = true;
            // 
            // AgentCanBeIsolated
            // 
            this.AgentCanBeIsolated.FormattingEnabled = true;
            this.AgentCanBeIsolated.Location = new System.Drawing.Point(281, 247);
            this.AgentCanBeIsolated.Margin = new System.Windows.Forms.Padding(2);
            this.AgentCanBeIsolated.Name = "AgentCanBeIsolated";
            this.AgentCanBeIsolated.Size = new System.Drawing.Size(87, 24);
            this.AgentCanBeIsolated.TabIndex = 130;
            this.AgentCanBeIsolated.Text = "Never";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(25, 250);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(145, 17);
            this.label6.TabIndex = 33;
            this.label6.Text = "Agent can be isolated";
            // 
            // InitialCapacity
            // 
            this.InitialCapacity.Location = new System.Drawing.Point(281, 215);
            this.InitialCapacity.Name = "InitialCapacity";
            this.InitialCapacity.Size = new System.Drawing.Size(87, 22);
            this.InitialCapacity.TabIndex = 32;
            // 
            // LimitSimultaneousTasks
            // 
            this.LimitSimultaneousTasks.AutoSize = true;
            this.LimitSimultaneousTasks.Location = new System.Drawing.Point(28, 152);
            this.LimitSimultaneousTasks.Name = "LimitSimultaneousTasks";
            this.LimitSimultaneousTasks.Size = new System.Drawing.Size(183, 21);
            this.LimitSimultaneousTasks.TabIndex = 31;
            this.LimitSimultaneousTasks.Text = "Limit simultaneous tasks";
            this.LimitSimultaneousTasks.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(25, 215);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(96, 17);
            this.label11.TabIndex = 31;
            this.label11.Text = "Initial capacity";
            // 
            // MaxSimultaneousTasks
            // 
            this.MaxSimultaneousTasks.Location = new System.Drawing.Point(281, 184);
            this.MaxSimultaneousTasks.Name = "MaxSimultaneousTasks";
            this.MaxSimultaneousTasks.Size = new System.Drawing.Size(87, 22);
            this.MaxSimultaneousTasks.TabIndex = 30;
            this.MaxSimultaneousTasks.TextChanged += new System.EventHandler(this.MaxSimultaneousTasks_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(64, 185);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(142, 17);
            this.label12.TabIndex = 29;
            this.label12.Text = "Max. number of tasks";
            // 
            // LimitNumberTasks
            // 
            this.LimitNumberTasks.AutoSize = true;
            this.LimitNumberTasks.Location = new System.Drawing.Point(28, 84);
            this.LimitNumberTasks.Name = "LimitNumberTasks";
            this.LimitNumberTasks.Size = new System.Drawing.Size(195, 21);
            this.LimitNumberTasks.TabIndex = 28;
            this.LimitNumberTasks.Text = "Limit total number of tasks";
            this.LimitNumberTasks.UseVisualStyleBackColor = true;
            // 
            // maxNumberTasks
            // 
            this.maxNumberTasks.Location = new System.Drawing.Point(281, 116);
            this.maxNumberTasks.Name = "maxNumberTasks";
            this.maxNumberTasks.Size = new System.Drawing.Size(87, 22);
            this.maxNumberTasks.TabIndex = 5;
            this.maxNumberTasks.TextChanged += new System.EventHandler(this.maxNumberTasks_TextChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(64, 117);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(142, 17);
            this.label16.TabIndex = 4;
            this.label16.Text = "Max. number of tasks";
            // 
            // CanPerformTask
            // 
            this.CanPerformTask.AutoSize = true;
            this.CanPerformTask.Location = new System.Drawing.Point(28, 26);
            this.CanPerformTask.Name = "CanPerformTask";
            this.CanPerformTask.Size = new System.Drawing.Size(138, 21);
            this.CanPerformTask.TabIndex = 3;
            this.CanPerformTask.Text = "Can perform task";
            this.CanPerformTask.UseVisualStyleBackColor = true;
            // 
            // Home
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1298, 724);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.richTextBox1);
            this.Name = "Home";
            this.Text = "Symu : messageing and tasking models";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label TimeStep;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblMessagesSent;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblWorked;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label TasksTotal;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbWorkers;
        private System.Windows.Forms.Button btnResume;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox tbSteps;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cbRandomLevel;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.TextBox maxNumberTasks;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.CheckBox CanPerformTask;
        private System.Windows.Forms.CheckBox LimitNumberTasks;
        private System.Windows.Forms.CheckBox LimitSimultaneousTasks;
        private System.Windows.Forms.TextBox MaxSimultaneousTasks;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.TextBox costOfTask;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox numberTasksSent;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox InitialCapacity;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox LimitMessagesReceived;
        private System.Windows.Forms.TextBox MaxMessagesReceived;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.CheckBox LimitMessagesSent;
        private System.Windows.Forms.TextBox MaxMessagesSent;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox LimitMessages;
        private System.Windows.Forms.TextBox MaxMessages;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label TasksWeight;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label TasksDone;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label TasksInProgress;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label TasksToDo;
        private System.Windows.Forms.ComboBox AgentCanBeIsolated;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.ComboBox CostToReceive;
        private System.Windows.Forms.ComboBox CostToSend;
        private System.Windows.Forms.CheckBox CanPerformTasksOnWeekends;
        private System.Windows.Forms.TextBox SwitchingContextCost;
        private System.Windows.Forms.Label label21;
    }
}