namespace VixenModules.App.SuperScheduler
{
	partial class SetupScheduleForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupScheduleForm));
			this.buttonHelp = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.dateStop = new System.Windows.Forms.DateTimePicker();
			this.label2 = new System.Windows.Forms.Label();
			this.dateStart = new System.Windows.Forms.DateTimePicker();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.checkSaturday = new System.Windows.Forms.CheckBox();
			this.checkSunday = new System.Windows.Forms.CheckBox();
			this.checkMonday = new System.Windows.Forms.CheckBox();
			this.checkFriday = new System.Windows.Forms.CheckBox();
			this.checkTuesday = new System.Windows.Forms.CheckBox();
			this.checkWednesday = new System.Windows.Forms.CheckBox();
			this.checkThursday = new System.Windows.Forms.CheckBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.labelDuration = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.dateEndTime = new System.Windows.Forms.DateTimePicker();
			this.dateStartTime = new System.Windows.Forms.DateTimePicker();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.comboBoxShow = new System.Windows.Forms.ComboBox();
			this.checkEnabled = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonHelp
			// 
			this.buttonHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonHelp.Image = ((System.Drawing.Image)(resources.GetObject("buttonHelp.Image")));
			this.buttonHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonHelp.Location = new System.Drawing.Point(12, 328);
			this.buttonHelp.Name = "buttonHelp";
			this.buttonHelp.Size = new System.Drawing.Size(60, 23);
			this.buttonHelp.TabIndex = 60;
			this.buttonHelp.Tag = "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/nutcracker-eff" +
    "ects/";
			this.buttonHelp.Text = "Help";
			this.buttonHelp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.buttonHelp.UseVisualStyleBackColor = true;
			this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(245, 328);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 62;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonOK.Location = new System.Drawing.Point(167, 328);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 61;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.dateStop);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.dateStart);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(12, 88);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(308, 77);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Dates:";
			// 
			// dateStop
			// 
			this.dateStop.Location = new System.Drawing.Point(44, 47);
			this.dateStop.Name = "dateStop";
			this.dateStop.Size = new System.Drawing.Size(200, 20);
			this.dateStop.TabIndex = 8;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 51);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(32, 13);
			this.label2.TabIndex = 7;
			this.label2.Text = "Stop:";
			// 
			// dateStart
			// 
			this.dateStart.Location = new System.Drawing.Point(44, 22);
			this.dateStart.Name = "dateStart";
			this.dateStart.Size = new System.Drawing.Size(200, 20);
			this.dateStart.TabIndex = 6;
			this.dateStart.ValueChanged += new System.EventHandler(this.dateStart_ValueChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(32, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Start:";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.checkSaturday);
			this.groupBox2.Controls.Add(this.checkSunday);
			this.groupBox2.Controls.Add(this.checkMonday);
			this.groupBox2.Controls.Add(this.checkFriday);
			this.groupBox2.Controls.Add(this.checkTuesday);
			this.groupBox2.Controls.Add(this.checkWednesday);
			this.groupBox2.Controls.Add(this.checkThursday);
			this.groupBox2.Location = new System.Drawing.Point(12, 171);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(308, 71);
			this.groupBox2.TabIndex = 63;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Days:";
			// 
			// checkSaturday
			// 
			this.checkSaturday.AutoSize = true;
			this.checkSaturday.Location = new System.Drawing.Point(150, 44);
			this.checkSaturday.Name = "checkSaturday";
			this.checkSaturday.Size = new System.Drawing.Size(68, 17);
			this.checkSaturday.TabIndex = 12;
			this.checkSaturday.Text = "Saturday";
			this.checkSaturday.UseVisualStyleBackColor = true;
			// 
			// checkSunday
			// 
			this.checkSunday.AutoSize = true;
			this.checkSunday.Location = new System.Drawing.Point(9, 21);
			this.checkSunday.Name = "checkSunday";
			this.checkSunday.Size = new System.Drawing.Size(62, 17);
			this.checkSunday.TabIndex = 6;
			this.checkSunday.Text = "Sunday";
			this.checkSunday.UseVisualStyleBackColor = true;
			// 
			// checkMonday
			// 
			this.checkMonday.AutoSize = true;
			this.checkMonday.Location = new System.Drawing.Point(80, 21);
			this.checkMonday.Name = "checkMonday";
			this.checkMonday.Size = new System.Drawing.Size(64, 17);
			this.checkMonday.TabIndex = 7;
			this.checkMonday.Text = "Monday";
			this.checkMonday.UseVisualStyleBackColor = true;
			// 
			// checkFriday
			// 
			this.checkFriday.AutoSize = true;
			this.checkFriday.Location = new System.Drawing.Point(80, 44);
			this.checkFriday.Name = "checkFriday";
			this.checkFriday.Size = new System.Drawing.Size(54, 17);
			this.checkFriday.TabIndex = 11;
			this.checkFriday.Text = "Friday";
			this.checkFriday.UseVisualStyleBackColor = true;
			// 
			// checkTuesday
			// 
			this.checkTuesday.AutoSize = true;
			this.checkTuesday.Location = new System.Drawing.Point(150, 21);
			this.checkTuesday.Name = "checkTuesday";
			this.checkTuesday.Size = new System.Drawing.Size(67, 17);
			this.checkTuesday.TabIndex = 8;
			this.checkTuesday.Text = "Tuesday";
			this.checkTuesday.UseVisualStyleBackColor = true;
			// 
			// checkWednesday
			// 
			this.checkWednesday.AutoSize = true;
			this.checkWednesday.Location = new System.Drawing.Point(221, 21);
			this.checkWednesday.Name = "checkWednesday";
			this.checkWednesday.Size = new System.Drawing.Size(83, 17);
			this.checkWednesday.TabIndex = 9;
			this.checkWednesday.Text = "Wednesday";
			this.checkWednesday.UseVisualStyleBackColor = true;
			// 
			// checkThursday
			// 
			this.checkThursday.AutoSize = true;
			this.checkThursday.Location = new System.Drawing.Point(9, 44);
			this.checkThursday.Name = "checkThursday";
			this.checkThursday.Size = new System.Drawing.Size(70, 17);
			this.checkThursday.TabIndex = 10;
			this.checkThursday.Text = "Thursday";
			this.checkThursday.UseVisualStyleBackColor = true;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.labelDuration);
			this.groupBox3.Controls.Add(this.label9);
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.dateEndTime);
			this.groupBox3.Controls.Add(this.dateStartTime);
			this.groupBox3.Location = new System.Drawing.Point(12, 248);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(308, 69);
			this.groupBox3.TabIndex = 64;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Time:";
			// 
			// labelDuration
			// 
			this.labelDuration.AutoSize = true;
			this.labelDuration.Location = new System.Drawing.Point(62, 47);
			this.labelDuration.Name = "labelDuration";
			this.labelDuration.Size = new System.Drawing.Size(39, 13);
			this.labelDuration.TabIndex = 72;
			this.labelDuration.Text = "(hours)";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(157, 28);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(55, 13);
			this.label9.TabIndex = 71;
			this.label9.Text = "End Time:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 28);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(58, 13);
			this.label3.TabIndex = 70;
			this.label3.Text = "Start Time:";
			// 
			// dateEndTime
			// 
			this.dateEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
			this.dateEndTime.Location = new System.Drawing.Point(214, 24);
			this.dateEndTime.Name = "dateEndTime";
			this.dateEndTime.ShowUpDown = true;
			this.dateEndTime.Size = new System.Drawing.Size(86, 20);
			this.dateEndTime.TabIndex = 69;
			this.dateEndTime.ValueChanged += new System.EventHandler(this.dateEndTime_ValueChanged);
			// 
			// dateStartTime
			// 
			this.dateStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
			this.dateStartTime.Location = new System.Drawing.Point(65, 24);
			this.dateStartTime.Name = "dateStartTime";
			this.dateStartTime.ShowUpDown = true;
			this.dateStartTime.Size = new System.Drawing.Size(86, 20);
			this.dateStartTime.TabIndex = 68;
			this.dateStartTime.ValueChanged += new System.EventHandler(this.dateStartTime_ValueChanged);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.label4);
			this.groupBox4.Controls.Add(this.comboBoxShow);
			this.groupBox4.Controls.Add(this.checkEnabled);
			this.groupBox4.Location = new System.Drawing.Point(12, 12);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(308, 70);
			this.groupBox4.TabIndex = 65;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Show";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 22);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(74, 13);
			this.label4.TabIndex = 19;
			this.label4.Text = "Show to Start:";
			// 
			// comboBoxShow
			// 
			this.comboBoxShow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxShow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxShow.FormattingEnabled = true;
			this.comboBoxShow.Location = new System.Drawing.Point(86, 19);
			this.comboBoxShow.Name = "comboBoxShow";
			this.comboBoxShow.Size = new System.Drawing.Size(211, 21);
			this.comboBoxShow.TabIndex = 18;
			this.comboBoxShow.SelectedIndexChanged += new System.EventHandler(this.comboBoxShow_SelectedIndexChanged);
			// 
			// checkEnabled
			// 
			this.checkEnabled.AutoSize = true;
			this.checkEnabled.Location = new System.Drawing.Point(86, 46);
			this.checkEnabled.Name = "checkEnabled";
			this.checkEnabled.Size = new System.Drawing.Size(107, 17);
			this.checkEnabled.TabIndex = 0;
			this.checkEnabled.Text = "Enable Schedule";
			this.checkEnabled.UseVisualStyleBackColor = true;
			// 
			// SetupScheduleForm
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(330, 361);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonHelp);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SetupScheduleForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Schedule a Show";
			this.Load += new System.EventHandler(this.SetupScheduleForm_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonHelp;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.DateTimePicker dateStop;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.DateTimePicker dateStart;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox checkSaturday;
		private System.Windows.Forms.CheckBox checkSunday;
		private System.Windows.Forms.CheckBox checkMonday;
		private System.Windows.Forms.CheckBox checkFriday;
		private System.Windows.Forms.CheckBox checkTuesday;
		private System.Windows.Forms.CheckBox checkWednesday;
		private System.Windows.Forms.CheckBox checkThursday;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.DateTimePicker dateEndTime;
		private System.Windows.Forms.DateTimePicker dateStartTime;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.CheckBox checkEnabled;
		private System.Windows.Forms.ComboBox comboBoxShow;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label labelDuration;
	}
}