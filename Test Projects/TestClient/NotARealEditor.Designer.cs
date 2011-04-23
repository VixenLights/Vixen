namespace TestClient {
    partial class NotARealEditor {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.labelSequenceName = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.comboBoxFixtures = new System.Windows.Forms.ComboBox();
			this.comboBoxChannels = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.comboBoxCommands = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.textBoxStartTime = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.textBoxTimeSpan = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.buttonAffectSelected = new System.Windows.Forms.Button();
			this.listBoxChannelData = new System.Windows.Forms.ListBox();
			this.label10 = new System.Windows.Forms.Label();
			this.buttonRefreshChannelData = new System.Windows.Forms.Button();
			this.buttonAffectAllFixtureChannels = new System.Windows.Forms.Button();
			this.buttonAffectAllChannels = new System.Windows.Forms.Button();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.textBoxInterval = new System.Windows.Forms.TextBox();
			this.textBoxSeconds = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.buttonAffectAllChannelsOverTime = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.buttonManualPatch = new System.Windows.Forms.Button();
			this.labelTime = new System.Windows.Forms.Label();
			this.textBoxPlayEndTime = new System.Windows.Forms.TextBox();
			this.label16 = new System.Windows.Forms.Label();
			this.textBoxPlayStartTime = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.buttonStop = new System.Windows.Forms.Button();
			this.buttonResume = new System.Windows.Forms.Button();
			this.buttonPause = new System.Windows.Forms.Button();
			this.buttonStart = new System.Windows.Forms.Button();
			this.comboBoxExecutionController = new System.Windows.Forms.ComboBox();
			this.buttonExecutionPatch = new System.Windows.Forms.Button();
			this.label14 = new System.Windows.Forms.Label();
			this.numericUpDownSequenceLength = new System.Windows.Forms.NumericUpDown();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.buttonAffectSelectedOverTime = new System.Windows.Forms.Button();
			this.label17 = new System.Windows.Forms.Label();
			this.comboBoxTimingSource = new System.Windows.Forms.ComboBox();
			this.comboBoxControllerSetup = new System.Windows.Forms.ComboBox();
			this.label18 = new System.Windows.Forms.Label();
			this.buttonControllerSetup = new System.Windows.Forms.Button();
			this.buttonMasking = new System.Windows.Forms.Button();
			this.buttonQueue = new System.Windows.Forms.Button();
			this.label19 = new System.Windows.Forms.Label();
			this.textBoxQueueContextName = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSequenceLength)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(179, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Pretend that I am a sequence editor.";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 47);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(59, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Sequence:";
			// 
			// labelSequenceName
			// 
			this.labelSequenceName.AutoSize = true;
			this.labelSequenceName.Location = new System.Drawing.Point(77, 47);
			this.labelSequenceName.Name = "labelSequenceName";
			this.labelSequenceName.Size = new System.Drawing.Size(74, 13);
			this.labelSequenceName.TabIndex = 2;
			this.labelSequenceName.Text = "(None loaded)";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 75);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(46, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "Fixtures:";
			// 
			// comboBoxFixtures
			// 
			this.comboBoxFixtures.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxFixtures.FormattingEnabled = true;
			this.comboBoxFixtures.Location = new System.Drawing.Point(80, 72);
			this.comboBoxFixtures.Name = "comboBoxFixtures";
			this.comboBoxFixtures.Size = new System.Drawing.Size(176, 21);
			this.comboBoxFixtures.TabIndex = 4;
			this.comboBoxFixtures.SelectedIndexChanged += new System.EventHandler(this.comboBoxFixtures_SelectedIndexChanged);
			// 
			// comboBoxChannels
			// 
			this.comboBoxChannels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxChannels.FormattingEnabled = true;
			this.comboBoxChannels.Location = new System.Drawing.Point(80, 99);
			this.comboBoxChannels.Name = "comboBoxChannels";
			this.comboBoxChannels.Size = new System.Drawing.Size(176, 21);
			this.comboBoxChannels.TabIndex = 6;
			this.comboBoxChannels.SelectedIndexChanged += new System.EventHandler(this.comboBoxChannels_SelectedIndexChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 102);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(54, 13);
			this.label4.TabIndex = 5;
			this.label4.Text = "Channels:";
			// 
			// comboBoxCommands
			// 
			this.comboBoxCommands.DisplayMember = "Name";
			this.comboBoxCommands.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxCommands.FormattingEnabled = true;
			this.comboBoxCommands.Location = new System.Drawing.Point(80, 126);
			this.comboBoxCommands.Name = "comboBoxCommands";
			this.comboBoxCommands.Size = new System.Drawing.Size(176, 21);
			this.comboBoxCommands.TabIndex = 10;
			this.comboBoxCommands.SelectedIndexChanged += new System.EventHandler(this.comboBoxCommands_SelectedIndexChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(12, 129);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(62, 13);
			this.label7.TabIndex = 9;
			this.label7.Text = "Commands:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 156);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(54, 13);
			this.label5.TabIndex = 11;
			this.label5.Text = "Start time:";
			// 
			// textBoxStartTime
			// 
			this.textBoxStartTime.Location = new System.Drawing.Point(80, 153);
			this.textBoxStartTime.Name = "textBoxStartTime";
			this.textBoxStartTime.Size = new System.Drawing.Size(58, 20);
			this.textBoxStartTime.TabIndex = 12;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(144, 156);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(20, 13);
			this.label6.TabIndex = 13;
			this.label6.Text = "ms";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Enabled = false;
			this.label8.Location = new System.Drawing.Point(144, 182);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(20, 13);
			this.label8.TabIndex = 16;
			this.label8.Text = "ms";
			// 
			// textBoxTimeSpan
			// 
			this.textBoxTimeSpan.Enabled = false;
			this.textBoxTimeSpan.Location = new System.Drawing.Point(80, 179);
			this.textBoxTimeSpan.Name = "textBoxTimeSpan";
			this.textBoxTimeSpan.Size = new System.Drawing.Size(58, 20);
			this.textBoxTimeSpan.TabIndex = 15;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Enabled = false;
			this.label9.Location = new System.Drawing.Point(12, 182);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(43, 13);
			this.label9.TabIndex = 14;
			this.label9.Text = "Length:";
			// 
			// buttonAffectSelected
			// 
			this.buttonAffectSelected.Location = new System.Drawing.Point(80, 205);
			this.buttonAffectSelected.Name = "buttonAffectSelected";
			this.buttonAffectSelected.Size = new System.Drawing.Size(160, 23);
			this.buttonAffectSelected.TabIndex = 17;
			this.buttonAffectSelected.Text = "Affect Selected";
			this.buttonAffectSelected.UseVisualStyleBackColor = true;
			this.buttonAffectSelected.Click += new System.EventHandler(this.buttonAffectSelected_Click);
			// 
			// listBoxChannelData
			// 
			this.listBoxChannelData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxChannelData.FormattingEnabled = true;
			this.listBoxChannelData.Location = new System.Drawing.Point(12, 375);
			this.listBoxChannelData.Name = "listBoxChannelData";
			this.listBoxChannelData.Size = new System.Drawing.Size(582, 147);
			this.listBoxChannelData.TabIndex = 18;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(9, 359);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(70, 13);
			this.label10.TabIndex = 19;
			this.label10.Text = "Channel data";
			// 
			// buttonRefreshChannelData
			// 
			this.buttonRefreshChannelData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRefreshChannelData.Location = new System.Drawing.Point(519, 355);
			this.buttonRefreshChannelData.Name = "buttonRefreshChannelData";
			this.buttonRefreshChannelData.Size = new System.Drawing.Size(75, 20);
			this.buttonRefreshChannelData.TabIndex = 20;
			this.buttonRefreshChannelData.Text = "Refresh";
			this.buttonRefreshChannelData.UseVisualStyleBackColor = true;
			this.buttonRefreshChannelData.Click += new System.EventHandler(this.buttonRefreshChannelData_Click);
			// 
			// buttonAffectAllFixtureChannels
			// 
			this.buttonAffectAllFixtureChannels.Location = new System.Drawing.Point(80, 234);
			this.buttonAffectAllFixtureChannels.Name = "buttonAffectAllFixtureChannels";
			this.buttonAffectAllFixtureChannels.Size = new System.Drawing.Size(160, 23);
			this.buttonAffectAllFixtureChannels.TabIndex = 21;
			this.buttonAffectAllFixtureChannels.Text = "Affect All Channels In Fixture";
			this.buttonAffectAllFixtureChannels.UseVisualStyleBackColor = true;
			this.buttonAffectAllFixtureChannels.Click += new System.EventHandler(this.buttonAffectAllFixtureChannels_Click);
			// 
			// buttonAffectAllChannels
			// 
			this.buttonAffectAllChannels.Location = new System.Drawing.Point(80, 263);
			this.buttonAffectAllChannels.Name = "buttonAffectAllChannels";
			this.buttonAffectAllChannels.Size = new System.Drawing.Size(160, 23);
			this.buttonAffectAllChannels.TabIndex = 22;
			this.buttonAffectAllChannels.Text = "Affect All Channels";
			this.buttonAffectAllChannels.UseVisualStyleBackColor = true;
			this.buttonAffectAllChannels.Click += new System.EventHandler(this.buttonAffectAllChannels_Click);
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(323, 207);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(34, 13);
			this.label11.TabIndex = 23;
			this.label11.Text = "Every";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(427, 207);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(35, 13);
			this.label12.TabIndex = 25;
			this.label12.Text = "ms for";
			// 
			// textBoxInterval
			// 
			this.textBoxInterval.Location = new System.Drawing.Point(363, 204);
			this.textBoxInterval.Name = "textBoxInterval";
			this.textBoxInterval.Size = new System.Drawing.Size(58, 20);
			this.textBoxInterval.TabIndex = 24;
			// 
			// textBoxSeconds
			// 
			this.textBoxSeconds.Location = new System.Drawing.Point(468, 204);
			this.textBoxSeconds.Name = "textBoxSeconds";
			this.textBoxSeconds.Size = new System.Drawing.Size(58, 20);
			this.textBoxSeconds.TabIndex = 26;
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(532, 207);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(47, 13);
			this.label13.TabIndex = 27;
			this.label13.Text = "seconds";
			// 
			// buttonAffectAllChannelsOverTime
			// 
			this.buttonAffectAllChannelsOverTime.Location = new System.Drawing.Point(363, 256);
			this.buttonAffectAllChannelsOverTime.Name = "buttonAffectAllChannelsOverTime";
			this.buttonAffectAllChannelsOverTime.Size = new System.Drawing.Size(160, 23);
			this.buttonAffectAllChannelsOverTime.TabIndex = 28;
			this.buttonAffectAllChannelsOverTime.Text = "Affect All Channels";
			this.buttonAffectAllChannelsOverTime.UseVisualStyleBackColor = true;
			this.buttonAffectAllChannelsOverTime.Click += new System.EventHandler(this.buttonAffectAllChannelsOverTime_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textBoxQueueContextName);
			this.groupBox1.Controls.Add(this.label19);
			this.groupBox1.Controls.Add(this.buttonQueue);
			this.groupBox1.Controls.Add(this.buttonManualPatch);
			this.groupBox1.Controls.Add(this.labelTime);
			this.groupBox1.Controls.Add(this.textBoxPlayEndTime);
			this.groupBox1.Controls.Add(this.label16);
			this.groupBox1.Controls.Add(this.textBoxPlayStartTime);
			this.groupBox1.Controls.Add(this.label15);
			this.groupBox1.Controls.Add(this.buttonStop);
			this.groupBox1.Controls.Add(this.buttonResume);
			this.groupBox1.Controls.Add(this.buttonPause);
			this.groupBox1.Controls.Add(this.buttonStart);
			this.groupBox1.Controls.Add(this.comboBoxExecutionController);
			this.groupBox1.Controls.Add(this.buttonExecutionPatch);
			this.groupBox1.Location = new System.Drawing.Point(300, 21);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(287, 174);
			this.groupBox1.TabIndex = 29;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Execution";
			// 
			// buttonManualPatch
			// 
			this.buttonManualPatch.Location = new System.Drawing.Point(198, 41);
			this.buttonManualPatch.Name = "buttonManualPatch";
			this.buttonManualPatch.Size = new System.Drawing.Size(75, 23);
			this.buttonManualPatch.TabIndex = 11;
			this.buttonManualPatch.Text = "Manual";
			this.buttonManualPatch.UseVisualStyleBackColor = true;
			this.buttonManualPatch.Click += new System.EventHandler(this.buttonManualPatch_Click);
			// 
			// labelTime
			// 
			this.labelTime.AutoSize = true;
			this.labelTime.Location = new System.Drawing.Point(13, 52);
			this.labelTime.Name = "labelTime";
			this.labelTime.Size = new System.Drawing.Size(34, 13);
			this.labelTime.TabIndex = 10;
			this.labelTime.Text = "00:00";
			// 
			// textBoxPlayEndTime
			// 
			this.textBoxPlayEndTime.Location = new System.Drawing.Point(168, 102);
			this.textBoxPlayEndTime.Name = "textBoxPlayEndTime";
			this.textBoxPlayEndTime.Size = new System.Drawing.Size(50, 20);
			this.textBoxPlayEndTime.TabIndex = 9;
			this.textBoxPlayEndTime.Text = "0";
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(119, 105);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(48, 13);
			this.label16.TabIndex = 8;
			this.label16.Text = "End time";
			// 
			// textBoxPlayStartTime
			// 
			this.textBoxPlayStartTime.Location = new System.Drawing.Point(63, 102);
			this.textBoxPlayStartTime.Name = "textBoxPlayStartTime";
			this.textBoxPlayStartTime.Size = new System.Drawing.Size(50, 20);
			this.textBoxPlayStartTime.TabIndex = 7;
			this.textBoxPlayStartTime.Text = "0";
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(6, 105);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(51, 13);
			this.label15.TabIndex = 6;
			this.label15.Text = "Start time";
			// 
			// buttonStop
			// 
			this.buttonStop.Location = new System.Drawing.Point(210, 70);
			this.buttonStop.Name = "buttonStop";
			this.buttonStop.Size = new System.Drawing.Size(62, 23);
			this.buttonStop.TabIndex = 5;
			this.buttonStop.Text = "Stop";
			this.buttonStop.UseVisualStyleBackColor = true;
			this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
			// 
			// buttonResume
			// 
			this.buttonResume.Location = new System.Drawing.Point(142, 70);
			this.buttonResume.Name = "buttonResume";
			this.buttonResume.Size = new System.Drawing.Size(62, 23);
			this.buttonResume.TabIndex = 4;
			this.buttonResume.Text = "Resume";
			this.buttonResume.UseVisualStyleBackColor = true;
			this.buttonResume.Click += new System.EventHandler(this.buttonResume_Click);
			// 
			// buttonPause
			// 
			this.buttonPause.Location = new System.Drawing.Point(74, 70);
			this.buttonPause.Name = "buttonPause";
			this.buttonPause.Size = new System.Drawing.Size(62, 23);
			this.buttonPause.TabIndex = 3;
			this.buttonPause.Text = "Pause";
			this.buttonPause.UseVisualStyleBackColor = true;
			this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
			// 
			// buttonStart
			// 
			this.buttonStart.Location = new System.Drawing.Point(6, 70);
			this.buttonStart.Name = "buttonStart";
			this.buttonStart.Size = new System.Drawing.Size(62, 23);
			this.buttonStart.TabIndex = 2;
			this.buttonStart.Text = "Start";
			this.buttonStart.UseVisualStyleBackColor = true;
			this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
			// 
			// comboBoxExecutionController
			// 
			this.comboBoxExecutionController.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxExecutionController.FormattingEnabled = true;
			this.comboBoxExecutionController.Location = new System.Drawing.Point(13, 19);
			this.comboBoxExecutionController.Name = "comboBoxExecutionController";
			this.comboBoxExecutionController.Size = new System.Drawing.Size(179, 21);
			this.comboBoxExecutionController.TabIndex = 1;
			// 
			// buttonExecutionPatch
			// 
			this.buttonExecutionPatch.Location = new System.Drawing.Point(198, 17);
			this.buttonExecutionPatch.Name = "buttonExecutionPatch";
			this.buttonExecutionPatch.Size = new System.Drawing.Size(75, 23);
			this.buttonExecutionPatch.TabIndex = 0;
			this.buttonExecutionPatch.Text = "Patch All";
			this.buttonExecutionPatch.UseVisualStyleBackColor = true;
			this.buttonExecutionPatch.Click += new System.EventHandler(this.buttonExecutionPatch_Click);
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(303, 290);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(140, 13);
			this.label14.TabIndex = 30;
			this.label14.Text = "Sequence length (seconds):";
			// 
			// numericUpDownSequenceLength
			// 
			this.numericUpDownSequenceLength.Location = new System.Drawing.Point(449, 288);
			this.numericUpDownSequenceLength.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownSequenceLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownSequenceLength.Name = "numericUpDownSequenceLength";
			this.numericUpDownSequenceLength.Size = new System.Drawing.Size(67, 20);
			this.numericUpDownSequenceLength.TabIndex = 31;
			this.numericUpDownSequenceLength.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownSequenceLength.ValueChanged += new System.EventHandler(this.numericUpDownSequenceLength_ValueChanged);
			// 
			// timer
			// 
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// buttonAffectSelectedOverTime
			// 
			this.buttonAffectSelectedOverTime.Location = new System.Drawing.Point(363, 230);
			this.buttonAffectSelectedOverTime.Name = "buttonAffectSelectedOverTime";
			this.buttonAffectSelectedOverTime.Size = new System.Drawing.Size(160, 23);
			this.buttonAffectSelectedOverTime.TabIndex = 32;
			this.buttonAffectSelectedOverTime.Text = "Affect Selected";
			this.buttonAffectSelectedOverTime.UseVisualStyleBackColor = true;
			this.buttonAffectSelectedOverTime.Click += new System.EventHandler(this.buttonAffectSelectedOverTime_Click);
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point(303, 319);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(73, 13);
			this.label17.TabIndex = 33;
			this.label17.Text = "Timing source";
			// 
			// comboBoxTimingSource
			// 
			this.comboBoxTimingSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxTimingSource.FormattingEnabled = true;
			this.comboBoxTimingSource.Items.AddRange(new object[] {
            "(none)"});
			this.comboBoxTimingSource.Location = new System.Drawing.Point(395, 316);
			this.comboBoxTimingSource.Name = "comboBoxTimingSource";
			this.comboBoxTimingSource.Size = new System.Drawing.Size(121, 21);
			this.comboBoxTimingSource.TabIndex = 34;
			this.comboBoxTimingSource.SelectedIndexChanged += new System.EventHandler(this.comboBoxTimingSource_SelectedIndexChanged);
			// 
			// comboBoxControllerSetup
			// 
			this.comboBoxControllerSetup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxControllerSetup.FormattingEnabled = true;
			this.comboBoxControllerSetup.Location = new System.Drawing.Point(98, 533);
			this.comboBoxControllerSetup.Name = "comboBoxControllerSetup";
			this.comboBoxControllerSetup.Size = new System.Drawing.Size(181, 21);
			this.comboBoxControllerSetup.TabIndex = 35;
			// 
			// label18
			// 
			this.label18.AutoSize = true;
			this.label18.Location = new System.Drawing.Point(9, 536);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(83, 13);
			this.label18.TabIndex = 36;
			this.label18.Text = "Controller setup:";
			// 
			// buttonControllerSetup
			// 
			this.buttonControllerSetup.Location = new System.Drawing.Point(285, 531);
			this.buttonControllerSetup.Name = "buttonControllerSetup";
			this.buttonControllerSetup.Size = new System.Drawing.Size(75, 23);
			this.buttonControllerSetup.TabIndex = 37;
			this.buttonControllerSetup.Text = "Setup";
			this.buttonControllerSetup.UseVisualStyleBackColor = true;
			this.buttonControllerSetup.Click += new System.EventHandler(this.buttonControllerSetup_Click);
			// 
			// buttonMasking
			// 
			this.buttonMasking.Location = new System.Drawing.Point(12, 565);
			this.buttonMasking.Name = "buttonMasking";
			this.buttonMasking.Size = new System.Drawing.Size(75, 23);
			this.buttonMasking.TabIndex = 38;
			this.buttonMasking.Text = "Masking";
			this.buttonMasking.UseVisualStyleBackColor = true;
			this.buttonMasking.Click += new System.EventHandler(this.buttonMasking_Click);
			// 
			// buttonQueue
			// 
			this.buttonQueue.Location = new System.Drawing.Point(6, 145);
			this.buttonQueue.Name = "buttonQueue";
			this.buttonQueue.Size = new System.Drawing.Size(75, 23);
			this.buttonQueue.TabIndex = 12;
			this.buttonQueue.Text = "Queue it";
			this.buttonQueue.UseVisualStyleBackColor = true;
			this.buttonQueue.Click += new System.EventHandler(this.buttonQueue_Click);
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(87, 150);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(75, 13);
			this.label19.TabIndex = 13;
			this.label19.Text = "Context name:";
			// 
			// textBoxQueueContextName
			// 
			this.textBoxQueueContextName.Location = new System.Drawing.Point(165, 147);
			this.textBoxQueueContextName.Name = "textBoxQueueContextName";
			this.textBoxQueueContextName.Size = new System.Drawing.Size(114, 20);
			this.textBoxQueueContextName.TabIndex = 14;
			// 
			// NotARealEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(619, 600);
			this.Controls.Add(this.buttonMasking);
			this.Controls.Add(this.buttonControllerSetup);
			this.Controls.Add(this.label18);
			this.Controls.Add(this.comboBoxControllerSetup);
			this.Controls.Add(this.comboBoxTimingSource);
			this.Controls.Add(this.label17);
			this.Controls.Add(this.buttonAffectSelectedOverTime);
			this.Controls.Add(this.numericUpDownSequenceLength);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonAffectAllChannelsOverTime);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.textBoxSeconds);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.textBoxInterval);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.buttonAffectAllChannels);
			this.Controls.Add(this.buttonAffectAllFixtureChannels);
			this.Controls.Add(this.buttonRefreshChannelData);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.listBoxChannelData);
			this.Controls.Add(this.buttonAffectSelected);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textBoxTimeSpan);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textBoxStartTime);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.comboBoxCommands);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.comboBoxChannels);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.comboBoxFixtures);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.labelSequenceName);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Name = "NotARealEditor";
			this.Text = "NotARealEditor";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSequenceLength)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelSequenceName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxFixtures;
        private System.Windows.Forms.ComboBox comboBoxChannels;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxCommands;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxStartTime;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxTimeSpan;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button buttonAffectSelected;
        private System.Windows.Forms.ListBox listBoxChannelData;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button buttonRefreshChannelData;
        private System.Windows.Forms.Button buttonAffectAllFixtureChannels;
        private System.Windows.Forms.Button buttonAffectAllChannels;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxInterval;
        private System.Windows.Forms.TextBox textBoxSeconds;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button buttonAffectAllChannelsOverTime;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonExecutionPatch;
        private System.Windows.Forms.ComboBox comboBoxExecutionController;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonResume;
        private System.Windows.Forms.Button buttonPause;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown numericUpDownSequenceLength;
        private System.Windows.Forms.TextBox textBoxPlayEndTime;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox textBoxPlayStartTime;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button buttonManualPatch;
        private System.Windows.Forms.Button buttonAffectSelectedOverTime;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ComboBox comboBoxTimingSource;
        private System.Windows.Forms.ComboBox comboBoxControllerSetup;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button buttonControllerSetup;
        private System.Windows.Forms.Button buttonMasking;
		private System.Windows.Forms.Button buttonQueue;
		private System.Windows.Forms.TextBox textBoxQueueContextName;
		private System.Windows.Forms.Label label19;
    }
}