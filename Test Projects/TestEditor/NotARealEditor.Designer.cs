namespace TestEditor {
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
			this.comboBoxChannels = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.comboBoxEffects = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.textBoxStartTime = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.textBoxTimeSpan = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.buttonAffectSelected = new System.Windows.Forms.Button();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.textBoxInterval = new System.Windows.Forms.TextBox();
			this.textBoxSeconds = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textBoxQueueContextName = new System.Windows.Forms.TextBox();
			this.label19 = new System.Windows.Forms.Label();
			this.buttonQueue = new System.Windows.Forms.Button();
			this.labelTime = new System.Windows.Forms.Label();
			this.textBoxPlayEndTime = new System.Windows.Forms.TextBox();
			this.label16 = new System.Windows.Forms.Label();
			this.textBoxPlayStartTime = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.buttonStop = new System.Windows.Forms.Button();
			this.buttonResume = new System.Windows.Forms.Button();
			this.buttonPause = new System.Windows.Forms.Button();
			this.buttonStart = new System.Windows.Forms.Button();
			this.label14 = new System.Windows.Forms.Label();
			this.numericUpDownSequenceLength = new System.Windows.Forms.NumericUpDown();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.buttonAffectSelectedOverTime = new System.Windows.Forms.Button();
			this.label17 = new System.Windows.Forms.Label();
			this.buttonMedia = new System.Windows.Forms.Button();
			this.listViewTimingSources = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
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
			// comboBoxChannels
			// 
			this.comboBoxChannels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxChannels.FormattingEnabled = true;
			this.comboBoxChannels.Location = new System.Drawing.Point(80, 99);
			this.comboBoxChannels.Name = "comboBoxChannels";
			this.comboBoxChannels.Size = new System.Drawing.Size(176, 21);
			this.comboBoxChannels.TabIndex = 6;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 102);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(41, 13);
			this.label4.TabIndex = 5;
			this.label4.Text = "Nodes:";
			// 
			// comboBoxEffects
			// 
			this.comboBoxEffects.DisplayMember = "Name";
			this.comboBoxEffects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxEffects.FormattingEnabled = true;
			this.comboBoxEffects.Location = new System.Drawing.Point(80, 126);
			this.comboBoxEffects.Name = "comboBoxEffects";
			this.comboBoxEffects.Size = new System.Drawing.Size(176, 21);
			this.comboBoxEffects.TabIndex = 10;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(12, 129);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(43, 13);
			this.label7.TabIndex = 9;
			this.label7.Text = "Effects:";
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
			this.label8.Location = new System.Drawing.Point(144, 182);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(20, 13);
			this.label8.TabIndex = 16;
			this.label8.Text = "ms";
			// 
			// textBoxTimeSpan
			// 
			this.textBoxTimeSpan.Location = new System.Drawing.Point(80, 179);
			this.textBoxTimeSpan.Name = "textBoxTimeSpan";
			this.textBoxTimeSpan.Size = new System.Drawing.Size(58, 20);
			this.textBoxTimeSpan.TabIndex = 15;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
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
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textBoxQueueContextName);
			this.groupBox1.Controls.Add(this.label19);
			this.groupBox1.Controls.Add(this.buttonQueue);
			this.groupBox1.Controls.Add(this.labelTime);
			this.groupBox1.Controls.Add(this.textBoxPlayEndTime);
			this.groupBox1.Controls.Add(this.label16);
			this.groupBox1.Controls.Add(this.textBoxPlayStartTime);
			this.groupBox1.Controls.Add(this.label15);
			this.groupBox1.Controls.Add(this.buttonStop);
			this.groupBox1.Controls.Add(this.buttonResume);
			this.groupBox1.Controls.Add(this.buttonPause);
			this.groupBox1.Controls.Add(this.buttonStart);
			this.groupBox1.Location = new System.Drawing.Point(300, 21);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(287, 126);
			this.groupBox1.TabIndex = 29;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Execution";
			// 
			// textBoxQueueContextName
			// 
			this.textBoxQueueContextName.Location = new System.Drawing.Point(165, 96);
			this.textBoxQueueContextName.Name = "textBoxQueueContextName";
			this.textBoxQueueContextName.Size = new System.Drawing.Size(114, 20);
			this.textBoxQueueContextName.TabIndex = 14;
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(87, 99);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(75, 13);
			this.label19.TabIndex = 13;
			this.label19.Text = "Context name:";
			// 
			// buttonQueue
			// 
			this.buttonQueue.Location = new System.Drawing.Point(6, 94);
			this.buttonQueue.Name = "buttonQueue";
			this.buttonQueue.Size = new System.Drawing.Size(75, 23);
			this.buttonQueue.TabIndex = 12;
			this.buttonQueue.Text = "Queue it";
			this.buttonQueue.UseVisualStyleBackColor = true;
			this.buttonQueue.Click += new System.EventHandler(this.buttonQueue_Click);
			// 
			// labelTime
			// 
			this.labelTime.AutoSize = true;
			this.labelTime.Location = new System.Drawing.Point(232, 51);
			this.labelTime.Name = "labelTime";
			this.labelTime.Size = new System.Drawing.Size(34, 13);
			this.labelTime.TabIndex = 10;
			this.labelTime.Text = "00:00";
			// 
			// textBoxPlayEndTime
			// 
			this.textBoxPlayEndTime.Location = new System.Drawing.Point(168, 51);
			this.textBoxPlayEndTime.Name = "textBoxPlayEndTime";
			this.textBoxPlayEndTime.Size = new System.Drawing.Size(50, 20);
			this.textBoxPlayEndTime.TabIndex = 9;
			this.textBoxPlayEndTime.Text = "0";
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(119, 54);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(48, 13);
			this.label16.TabIndex = 8;
			this.label16.Text = "End time";
			// 
			// textBoxPlayStartTime
			// 
			this.textBoxPlayStartTime.Location = new System.Drawing.Point(63, 51);
			this.textBoxPlayStartTime.Name = "textBoxPlayStartTime";
			this.textBoxPlayStartTime.Size = new System.Drawing.Size(50, 20);
			this.textBoxPlayStartTime.TabIndex = 7;
			this.textBoxPlayStartTime.Text = "0";
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(6, 54);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(51, 13);
			this.label15.TabIndex = 6;
			this.label15.Text = "Start time";
			// 
			// buttonStop
			// 
			this.buttonStop.Location = new System.Drawing.Point(210, 19);
			this.buttonStop.Name = "buttonStop";
			this.buttonStop.Size = new System.Drawing.Size(62, 23);
			this.buttonStop.TabIndex = 5;
			this.buttonStop.Text = "Stop";
			this.buttonStop.UseVisualStyleBackColor = true;
			this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
			// 
			// buttonResume
			// 
			this.buttonResume.Location = new System.Drawing.Point(142, 19);
			this.buttonResume.Name = "buttonResume";
			this.buttonResume.Size = new System.Drawing.Size(62, 23);
			this.buttonResume.TabIndex = 4;
			this.buttonResume.Text = "Resume";
			this.buttonResume.UseVisualStyleBackColor = true;
			this.buttonResume.Click += new System.EventHandler(this.buttonResume_Click);
			// 
			// buttonPause
			// 
			this.buttonPause.Location = new System.Drawing.Point(74, 19);
			this.buttonPause.Name = "buttonPause";
			this.buttonPause.Size = new System.Drawing.Size(62, 23);
			this.buttonPause.TabIndex = 3;
			this.buttonPause.Text = "Pause";
			this.buttonPause.UseVisualStyleBackColor = true;
			this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
			// 
			// buttonStart
			// 
			this.buttonStart.Location = new System.Drawing.Point(6, 19);
			this.buttonStart.Name = "buttonStart";
			this.buttonStart.Size = new System.Drawing.Size(62, 23);
			this.buttonStart.TabIndex = 2;
			this.buttonStart.Text = "Start";
			this.buttonStart.UseVisualStyleBackColor = true;
			this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
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
			this.buttonAffectSelectedOverTime.Enabled = false;
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
			// buttonMedia
			// 
			this.buttonMedia.Location = new System.Drawing.Point(408, 164);
			this.buttonMedia.Name = "buttonMedia";
			this.buttonMedia.Size = new System.Drawing.Size(75, 23);
			this.buttonMedia.TabIndex = 43;
			this.buttonMedia.Text = "Media";
			this.buttonMedia.UseVisualStyleBackColor = true;
			this.buttonMedia.Click += new System.EventHandler(this.buttonMedia_Click);
			// 
			// listViewTimingSources
			// 
			this.listViewTimingSources.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.listViewTimingSources.FullRowSelect = true;
			this.listViewTimingSources.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewTimingSources.HideSelection = false;
			this.listViewTimingSources.Location = new System.Drawing.Point(382, 319);
			this.listViewTimingSources.MultiSelect = false;
			this.listViewTimingSources.Name = "listViewTimingSources";
			this.listViewTimingSources.Size = new System.Drawing.Size(215, 96);
			this.listViewTimingSources.TabIndex = 44;
			this.listViewTimingSources.UseCompatibleStateImageBehavior = false;
			this.listViewTimingSources.View = System.Windows.Forms.View.Details;
			this.listViewTimingSources.SelectedIndexChanged += new System.EventHandler(this.listViewTimingSources_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Width = 188;
			// 
			// NotARealEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(619, 600);
			this.Controls.Add(this.listViewTimingSources);
			this.Controls.Add(this.buttonMedia);
			this.Controls.Add(this.label17);
			this.Controls.Add(this.buttonAffectSelectedOverTime);
			this.Controls.Add(this.numericUpDownSequenceLength);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.textBoxSeconds);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.textBoxInterval);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.buttonAffectSelected);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textBoxTimeSpan);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textBoxStartTime);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.comboBoxEffects);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.comboBoxChannels);
			this.Controls.Add(this.label4);
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
        private System.Windows.Forms.ComboBox comboBoxChannels;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxEffects;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxStartTime;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxTimeSpan;
        private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Button buttonAffectSelected;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxInterval;
        private System.Windows.Forms.TextBox textBoxSeconds;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.GroupBox groupBox1;
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
        private System.Windows.Forms.Button buttonAffectSelectedOverTime;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Button buttonQueue;
		private System.Windows.Forms.TextBox textBoxQueueContextName;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Button buttonMedia;
		private System.Windows.Forms.ListView listViewTimingSources;
		private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}