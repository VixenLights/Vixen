namespace VixenModules.App.Scheduler {
	partial class ScheduleItemEditForm {
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
			this.comboBoxDateUnitCount = new System.Windows.Forms.ComboBox();
			this.comboBoxDateUnit = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.dateTimePickerStartDate = new System.Windows.Forms.DateTimePicker();
			this.dateTimePickerEndDate = new System.Windows.Forms.DateTimePicker();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.panelRecurrence = new System.Windows.Forms.Panel();
			this.recurrenceControls = new Common.Controls.TablessTabControl(this.components);
			this.noneTab = new System.Windows.Forms.TabPage();
			this.dayTab = new System.Windows.Forms.TabPage();
			this.weekTab = new System.Windows.Forms.TabPage();
			this.radioButtonSaturday = new System.Windows.Forms.RadioButton();
			this.radioButtonFriday = new System.Windows.Forms.RadioButton();
			this.radioButtonThursday = new System.Windows.Forms.RadioButton();
			this.radioButtonWednesday = new System.Windows.Forms.RadioButton();
			this.radioButtonTuesday = new System.Windows.Forms.RadioButton();
			this.radioButtonMonday = new System.Windows.Forms.RadioButton();
			this.radioButtonSunday = new System.Windows.Forms.RadioButton();
			this.label6 = new System.Windows.Forms.Label();
			this.monthTab = new System.Windows.Forms.TabPage();
			this.comboBoxDow = new System.Windows.Forms.ComboBox();
			this.comboBoxDayCount = new System.Windows.Forms.ComboBox();
			this.radioButtonWeekDayCount = new System.Windows.Forms.RadioButton();
			this.radioButtonLastDay = new System.Windows.Forms.RadioButton();
			this.textBoxSpecificDate = new System.Windows.Forms.TextBox();
			this.radioButtonSpecificDate = new System.Windows.Forms.RadioButton();
			this.label5 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.checkBoxNoRecurrence = new System.Windows.Forms.CheckBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.comboBoxStartTime = new System.Windows.Forms.ComboBox();
			this.panel3 = new System.Windows.Forms.Panel();
			this.panelInterval = new System.Windows.Forms.Panel();
			this.label9 = new System.Windows.Forms.Label();
			this.textBoxInterval = new System.Windows.Forms.TextBox();
			this.checkBoxInterval = new System.Windows.Forms.CheckBox();
			this.comboBoxEndTime = new System.Windows.Forms.ComboBox();
			this.checkBoxRepeat = new System.Windows.Forms.CheckBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.panel4 = new System.Windows.Forms.Panel();
			this.panel5 = new System.Windows.Forms.Panel();
			this.panel6 = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label10 = new System.Windows.Forms.Label();
			this.panel7 = new System.Windows.Forms.Panel();
			this.buttonEditProgram = new System.Windows.Forms.Button();
			this.labelWhat = new System.Windows.Forms.Label();
			this.buttonNewProgram = new System.Windows.Forms.Button();
			this.buttonSelectProgram = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.panelRecurrence.SuspendLayout();
			this.recurrenceControls.SuspendLayout();
			this.weekTab.SuspendLayout();
			this.monthTab.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panelInterval.SuspendLayout();
			this.panel4.SuspendLayout();
			this.panel5.SuspendLayout();
			this.panel6.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel7.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(5, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(34, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Every";
			// 
			// comboBoxDateUnitCount
			// 
			this.comboBoxDateUnitCount.FormattingEnabled = true;
			this.comboBoxDateUnitCount.Items.AddRange(new object[] {
            "single",
            "2",
            "3",
            "4"});
			this.comboBoxDateUnitCount.Location = new System.Drawing.Point(54, 3);
			this.comboBoxDateUnitCount.Name = "comboBoxDateUnitCount";
			this.comboBoxDateUnitCount.Size = new System.Drawing.Size(93, 21);
			this.comboBoxDateUnitCount.TabIndex = 1;
			// 
			// comboBoxDateUnit
			// 
			this.comboBoxDateUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxDateUnit.FormattingEnabled = true;
			this.comboBoxDateUnit.Items.AddRange(new object[] {
            "day",
            "week",
            "month"});
			this.comboBoxDateUnit.Location = new System.Drawing.Point(157, 3);
			this.comboBoxDateUnit.Name = "comboBoxDateUnit";
			this.comboBoxDateUnit.Size = new System.Drawing.Size(121, 21);
			this.comboBoxDateUnit.TabIndex = 2;
			this.comboBoxDateUnit.SelectedIndexChanged += new System.EventHandler(this.comboBoxDateUnit_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(5, 2);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(43, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Starting";
			// 
			// dateTimePickerStartDate
			// 
			this.dateTimePickerStartDate.Location = new System.Drawing.Point(75, 2);
			this.dateTimePickerStartDate.Name = "dateTimePickerStartDate";
			this.dateTimePickerStartDate.Size = new System.Drawing.Size(200, 20);
			this.dateTimePickerStartDate.TabIndex = 4;
			// 
			// dateTimePickerEndDate
			// 
			this.dateTimePickerEndDate.Location = new System.Drawing.Point(75, 32);
			this.dateTimePickerEndDate.Name = "dateTimePickerEndDate";
			this.dateTimePickerEndDate.Size = new System.Drawing.Size(200, 20);
			this.dateTimePickerEndDate.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(5, 32);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(28, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Until";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(12, 8);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(190, 19);
			this.label4.TabIndex = 8;
			this.label4.Text = "What days will this run?";
			// 
			// panelRecurrence
			// 
			this.panelRecurrence.Controls.Add(this.recurrenceControls);
			this.panelRecurrence.Controls.Add(this.comboBoxDateUnitCount);
			this.panelRecurrence.Controls.Add(this.label1);
			this.panelRecurrence.Controls.Add(this.comboBoxDateUnit);
			this.panelRecurrence.Location = new System.Drawing.Point(49, 282);
			this.panelRecurrence.Name = "panelRecurrence";
			this.panelRecurrence.Size = new System.Drawing.Size(288, 121);
			this.panelRecurrence.TabIndex = 5;
			// 
			// recurrenceControls
			// 
			this.recurrenceControls.Controls.Add(this.noneTab);
			this.recurrenceControls.Controls.Add(this.dayTab);
			this.recurrenceControls.Controls.Add(this.weekTab);
			this.recurrenceControls.Controls.Add(this.monthTab);
			this.recurrenceControls.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.recurrenceControls.Location = new System.Drawing.Point(0, 27);
			this.recurrenceControls.Multiline = true;
			this.recurrenceControls.Name = "recurrenceControls";
			this.recurrenceControls.SelectedIndex = 0;
			this.recurrenceControls.Size = new System.Drawing.Size(288, 94);
			this.recurrenceControls.TabIndex = 3;
			// 
			// noneTab
			// 
			this.noneTab.BackColor = System.Drawing.SystemColors.Control;
			this.noneTab.Location = new System.Drawing.Point(0, 0);
			this.noneTab.Name = "noneTab";
			this.noneTab.Size = new System.Drawing.Size(288, 94);
			this.noneTab.TabIndex = 3;
			this.noneTab.Text = "None";
			// 
			// dayTab
			// 
			this.dayTab.BackColor = System.Drawing.SystemColors.Control;
			this.dayTab.Location = new System.Drawing.Point(0, 0);
			this.dayTab.Name = "dayTab";
			this.dayTab.Size = new System.Drawing.Size(288, 94);
			this.dayTab.TabIndex = 2;
			this.dayTab.Text = "Day";
			// 
			// weekTab
			// 
			this.weekTab.BackColor = System.Drawing.SystemColors.Control;
			this.weekTab.Controls.Add(this.radioButtonSaturday);
			this.weekTab.Controls.Add(this.radioButtonFriday);
			this.weekTab.Controls.Add(this.radioButtonThursday);
			this.weekTab.Controls.Add(this.radioButtonWednesday);
			this.weekTab.Controls.Add(this.radioButtonTuesday);
			this.weekTab.Controls.Add(this.radioButtonMonday);
			this.weekTab.Controls.Add(this.radioButtonSunday);
			this.weekTab.Controls.Add(this.label6);
			this.weekTab.Location = new System.Drawing.Point(0, 0);
			this.weekTab.Name = "weekTab";
			this.weekTab.Padding = new System.Windows.Forms.Padding(3);
			this.weekTab.Size = new System.Drawing.Size(288, 94);
			this.weekTab.TabIndex = 0;
			this.weekTab.Text = "Week";
			// 
			// radioButtonSaturday
			// 
			this.radioButtonSaturday.AutoSize = true;
			this.radioButtonSaturday.Location = new System.Drawing.Point(168, 54);
			this.radioButtonSaturday.Name = "radioButtonSaturday";
			this.radioButtonSaturday.Size = new System.Drawing.Size(67, 17);
			this.radioButtonSaturday.TabIndex = 7;
			this.radioButtonSaturday.Text = "Saturday";
			this.radioButtonSaturday.UseVisualStyleBackColor = true;
			// 
			// radioButtonFriday
			// 
			this.radioButtonFriday.AutoSize = true;
			this.radioButtonFriday.Location = new System.Drawing.Point(168, 32);
			this.radioButtonFriday.Name = "radioButtonFriday";
			this.radioButtonFriday.Size = new System.Drawing.Size(53, 17);
			this.radioButtonFriday.TabIndex = 6;
			this.radioButtonFriday.Text = "Friday";
			this.radioButtonFriday.UseVisualStyleBackColor = true;
			// 
			// radioButtonThursday
			// 
			this.radioButtonThursday.AutoSize = true;
			this.radioButtonThursday.Location = new System.Drawing.Point(168, 10);
			this.radioButtonThursday.Name = "radioButtonThursday";
			this.radioButtonThursday.Size = new System.Drawing.Size(69, 17);
			this.radioButtonThursday.TabIndex = 5;
			this.radioButtonThursday.Text = "Thursday";
			this.radioButtonThursday.UseVisualStyleBackColor = true;
			// 
			// radioButtonWednesday
			// 
			this.radioButtonWednesday.AutoSize = true;
			this.radioButtonWednesday.Location = new System.Drawing.Point(62, 69);
			this.radioButtonWednesday.Name = "radioButtonWednesday";
			this.radioButtonWednesday.Size = new System.Drawing.Size(82, 17);
			this.radioButtonWednesday.TabIndex = 4;
			this.radioButtonWednesday.Text = "Wednesday";
			this.radioButtonWednesday.UseVisualStyleBackColor = true;
			// 
			// radioButtonTuesday
			// 
			this.radioButtonTuesday.AutoSize = true;
			this.radioButtonTuesday.Location = new System.Drawing.Point(62, 47);
			this.radioButtonTuesday.Name = "radioButtonTuesday";
			this.radioButtonTuesday.Size = new System.Drawing.Size(66, 17);
			this.radioButtonTuesday.TabIndex = 3;
			this.radioButtonTuesday.Text = "Tuesday";
			this.radioButtonTuesday.UseVisualStyleBackColor = true;
			// 
			// radioButtonMonday
			// 
			this.radioButtonMonday.AutoSize = true;
			this.radioButtonMonday.Location = new System.Drawing.Point(62, 25);
			this.radioButtonMonday.Name = "radioButtonMonday";
			this.radioButtonMonday.Size = new System.Drawing.Size(63, 17);
			this.radioButtonMonday.TabIndex = 2;
			this.radioButtonMonday.Text = "Monday";
			this.radioButtonMonday.UseVisualStyleBackColor = true;
			// 
			// radioButtonSunday
			// 
			this.radioButtonSunday.AutoSize = true;
			this.radioButtonSunday.Checked = true;
			this.radioButtonSunday.Location = new System.Drawing.Point(62, 3);
			this.radioButtonSunday.Name = "radioButtonSunday";
			this.radioButtonSunday.Size = new System.Drawing.Size(61, 17);
			this.radioButtonSunday.TabIndex = 1;
			this.radioButtonSunday.TabStop = true;
			this.radioButtonSunday.Text = "Sunday";
			this.radioButtonSunday.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(6, 3);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(19, 13);
			this.label6.TabIndex = 0;
			this.label6.Text = "on";
			// 
			// monthTab
			// 
			this.monthTab.BackColor = System.Drawing.SystemColors.Control;
			this.monthTab.Controls.Add(this.comboBoxDow);
			this.monthTab.Controls.Add(this.comboBoxDayCount);
			this.monthTab.Controls.Add(this.radioButtonWeekDayCount);
			this.monthTab.Controls.Add(this.radioButtonLastDay);
			this.monthTab.Controls.Add(this.textBoxSpecificDate);
			this.monthTab.Controls.Add(this.radioButtonSpecificDate);
			this.monthTab.Location = new System.Drawing.Point(0, 0);
			this.monthTab.Name = "monthTab";
			this.monthTab.Padding = new System.Windows.Forms.Padding(3);
			this.monthTab.Size = new System.Drawing.Size(288, 94);
			this.monthTab.TabIndex = 1;
			this.monthTab.Text = "Month";
			// 
			// comboBoxDow
			// 
			this.comboBoxDow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxDow.FormattingEnabled = true;
			this.comboBoxDow.Items.AddRange(new object[] {
            "Sunday",
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday"});
			this.comboBoxDow.Location = new System.Drawing.Point(164, 46);
			this.comboBoxDow.Name = "comboBoxDow";
			this.comboBoxDow.Size = new System.Drawing.Size(111, 21);
			this.comboBoxDow.TabIndex = 5;
			// 
			// comboBoxDayCount
			// 
			this.comboBoxDayCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxDayCount.FormattingEnabled = true;
			this.comboBoxDayCount.Items.AddRange(new object[] {
            "first",
            "second",
            "third",
            "fourth",
            "fifth"});
			this.comboBoxDayCount.Location = new System.Drawing.Point(81, 46);
			this.comboBoxDayCount.Name = "comboBoxDayCount";
			this.comboBoxDayCount.Size = new System.Drawing.Size(73, 21);
			this.comboBoxDayCount.TabIndex = 4;
			// 
			// radioButtonWeekDayCount
			// 
			this.radioButtonWeekDayCount.AutoSize = true;
			this.radioButtonWeekDayCount.Location = new System.Drawing.Point(20, 46);
			this.radioButtonWeekDayCount.Name = "radioButtonWeekDayCount";
			this.radioButtonWeekDayCount.Size = new System.Drawing.Size(55, 17);
			this.radioButtonWeekDayCount.TabIndex = 3;
			this.radioButtonWeekDayCount.Text = "on the";
			this.radioButtonWeekDayCount.UseVisualStyleBackColor = true;
			// 
			// radioButtonLastDay
			// 
			this.radioButtonLastDay.AutoSize = true;
			this.radioButtonLastDay.Location = new System.Drawing.Point(20, 25);
			this.radioButtonLastDay.Name = "radioButtonLastDay";
			this.radioButtonLastDay.Size = new System.Drawing.Size(156, 17);
			this.radioButtonLastDay.TabIndex = 2;
			this.radioButtonLastDay.Text = "on the last day of the month";
			this.radioButtonLastDay.UseVisualStyleBackColor = true;
			// 
			// textBoxSpecificDate
			// 
			this.textBoxSpecificDate.Location = new System.Drawing.Point(209, 4);
			this.textBoxSpecificDate.Name = "textBoxSpecificDate";
			this.textBoxSpecificDate.Size = new System.Drawing.Size(39, 20);
			this.textBoxSpecificDate.TabIndex = 1;
			// 
			// radioButtonSpecificDate
			// 
			this.radioButtonSpecificDate.AutoSize = true;
			this.radioButtonSpecificDate.Checked = true;
			this.radioButtonSpecificDate.Location = new System.Drawing.Point(20, 4);
			this.radioButtonSpecificDate.Name = "radioButtonSpecificDate";
			this.radioButtonSpecificDate.Size = new System.Drawing.Size(174, 17);
			this.radioButtonSpecificDate.TabIndex = 0;
			this.radioButtonSpecificDate.TabStop = true;
			this.radioButtonSpecificDate.Text = "on a specific date of the month:";
			this.radioButtonSpecificDate.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(12, 8);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(268, 19);
			this.label5.TabIndex = 10;
			this.label5.Text = "Within what dates will this be run?";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.checkBoxNoRecurrence);
			this.panel2.Controls.Add(this.dateTimePickerEndDate);
			this.panel2.Controls.Add(this.label2);
			this.panel2.Controls.Add(this.label3);
			this.panel2.Controls.Add(this.dateTimePickerStartDate);
			this.panel2.Location = new System.Drawing.Point(49, 159);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(288, 78);
			this.panel2.TabIndex = 3;
			// 
			// checkBoxNoRecurrence
			// 
			this.checkBoxNoRecurrence.AutoSize = true;
			this.checkBoxNoRecurrence.Location = new System.Drawing.Point(75, 54);
			this.checkBoxNoRecurrence.Name = "checkBoxNoRecurrence";
			this.checkBoxNoRecurrence.Size = new System.Drawing.Size(72, 17);
			this.checkBoxNoRecurrence.TabIndex = 7;
			this.checkBoxNoRecurrence.Text = "Just once";
			this.checkBoxNoRecurrence.UseVisualStyleBackColor = true;
			this.checkBoxNoRecurrence.CheckedChanged += new System.EventHandler(this.checkBoxNoRecurrence_CheckedChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.Location = new System.Drawing.Point(12, 8);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(236, 19);
			this.label7.TabIndex = 12;
			this.label7.Text = "What time during those days?";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(5, 7);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(41, 13);
			this.label8.TabIndex = 13;
			this.label8.Text = "Start at";
			// 
			// comboBoxStartTime
			// 
			this.comboBoxStartTime.FormattingEnabled = true;
			this.comboBoxStartTime.Location = new System.Drawing.Point(92, 4);
			this.comboBoxStartTime.Name = "comboBoxStartTime";
			this.comboBoxStartTime.Size = new System.Drawing.Size(121, 21);
			this.comboBoxStartTime.TabIndex = 14;
			this.comboBoxStartTime.Leave += new System.EventHandler(this.comboBoxStartTime_Leave);
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.panelInterval);
			this.panel3.Controls.Add(this.comboBoxEndTime);
			this.panel3.Controls.Add(this.comboBoxStartTime);
			this.panel3.Controls.Add(this.checkBoxRepeat);
			this.panel3.Controls.Add(this.label8);
			this.panel3.Location = new System.Drawing.Point(49, 447);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(288, 81);
			this.panel3.TabIndex = 7;
			// 
			// panelInterval
			// 
			this.panelInterval.Controls.Add(this.label9);
			this.panelInterval.Controls.Add(this.textBoxInterval);
			this.panelInterval.Controls.Add(this.checkBoxInterval);
			this.panelInterval.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelInterval.Enabled = false;
			this.panelInterval.Location = new System.Drawing.Point(0, 53);
			this.panelInterval.Name = "panelInterval";
			this.panelInterval.Size = new System.Drawing.Size(288, 28);
			this.panelInterval.TabIndex = 16;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(149, 7);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(43, 13);
			this.label9.TabIndex = 2;
			this.label9.Text = "minutes";
			// 
			// textBoxInterval
			// 
			this.textBoxInterval.Location = new System.Drawing.Point(92, 4);
			this.textBoxInterval.Name = "textBoxInterval";
			this.textBoxInterval.Size = new System.Drawing.Size(51, 20);
			this.textBoxInterval.TabIndex = 1;
			// 
			// checkBoxInterval
			// 
			this.checkBoxInterval.AutoSize = true;
			this.checkBoxInterval.Location = new System.Drawing.Point(8, 6);
			this.checkBoxInterval.Name = "checkBoxInterval";
			this.checkBoxInterval.Size = new System.Drawing.Size(52, 17);
			this.checkBoxInterval.TabIndex = 0;
			this.checkBoxInterval.Text = "every";
			this.checkBoxInterval.UseVisualStyleBackColor = true;
			// 
			// comboBoxEndTime
			// 
			this.comboBoxEndTime.FormattingEnabled = true;
			this.comboBoxEndTime.Location = new System.Drawing.Point(92, 31);
			this.comboBoxEndTime.Name = "comboBoxEndTime";
			this.comboBoxEndTime.Size = new System.Drawing.Size(121, 21);
			this.comboBoxEndTime.TabIndex = 15;
			// 
			// checkBoxRepeat
			// 
			this.checkBoxRepeat.AutoSize = true;
			this.checkBoxRepeat.Location = new System.Drawing.Point(8, 33);
			this.checkBoxRepeat.Name = "checkBoxRepeat";
			this.checkBoxRepeat.Size = new System.Drawing.Size(78, 17);
			this.checkBoxRepeat.TabIndex = 0;
			this.checkBoxRepeat.Text = "repeat until";
			this.checkBoxRepeat.UseVisualStyleBackColor = true;
			this.checkBoxRepeat.CheckedChanged += new System.EventHandler(this.checkBoxRepeat_CheckedChanged);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(203, 537);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 8;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(284, 537);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 9;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// panel4
			// 
			this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel4.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.panel4.Controls.Add(this.label5);
			this.panel4.Location = new System.Drawing.Point(0, 120);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(373, 35);
			this.panel4.TabIndex = 2;
			// 
			// panel5
			// 
			this.panel5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel5.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.panel5.Controls.Add(this.label4);
			this.panel5.Location = new System.Drawing.Point(0, 243);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(373, 35);
			this.panel5.TabIndex = 4;
			// 
			// panel6
			// 
			this.panel6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel6.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.panel6.Controls.Add(this.label7);
			this.panel6.Location = new System.Drawing.Point(0, 409);
			this.panel6.Name = "panel6";
			this.panel6.Size = new System.Drawing.Size(373, 35);
			this.panel6.TabIndex = 6;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.panel1.Controls.Add(this.label10);
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(373, 35);
			this.panel1.TabIndex = 0;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label10.Location = new System.Drawing.Point(12, 8);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(117, 19);
			this.label10.TabIndex = 10;
			this.label10.Text = "What will run?";
			// 
			// panel7
			// 
			this.panel7.Controls.Add(this.buttonEditProgram);
			this.panel7.Controls.Add(this.labelWhat);
			this.panel7.Controls.Add(this.buttonNewProgram);
			this.panel7.Controls.Add(this.buttonSelectProgram);
			this.panel7.Location = new System.Drawing.Point(0, 39);
			this.panel7.Name = "panel7";
			this.panel7.Size = new System.Drawing.Size(336, 75);
			this.panel7.TabIndex = 1;
			// 
			// buttonEditProgram
			// 
			this.buttonEditProgram.Location = new System.Drawing.Point(10, 37);
			this.buttonEditProgram.Name = "buttonEditProgram";
			this.buttonEditProgram.Size = new System.Drawing.Size(38, 20);
			this.buttonEditProgram.TabIndex = 4;
			this.buttonEditProgram.Text = "Edit";
			this.buttonEditProgram.UseVisualStyleBackColor = true;
			this.buttonEditProgram.Visible = false;
			this.buttonEditProgram.Click += new System.EventHandler(this.buttonEditProgram_Click);
			// 
			// labelWhat
			// 
			this.labelWhat.AutoSize = true;
			this.labelWhat.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
			this.labelWhat.ForeColor = System.Drawing.Color.Red;
			this.labelWhat.Location = new System.Drawing.Point(54, 39);
			this.labelWhat.Name = "labelWhat";
			this.labelWhat.Size = new System.Drawing.Size(98, 16);
			this.labelWhat.TabIndex = 3;
			this.labelWhat.Text = "Not Selected";
			// 
			// buttonNewProgram
			// 
			this.buttonNewProgram.Location = new System.Drawing.Point(164, 3);
			this.buttonNewProgram.Name = "buttonNewProgram";
			this.buttonNewProgram.Size = new System.Drawing.Size(96, 23);
			this.buttonNewProgram.TabIndex = 2;
			this.buttonNewProgram.Text = "New Program";
			this.buttonNewProgram.UseVisualStyleBackColor = true;
			this.buttonNewProgram.Click += new System.EventHandler(this.buttonNewProgram_Click);
			// 
			// buttonSelectProgram
			// 
			this.buttonSelectProgram.Location = new System.Drawing.Point(57, 3);
			this.buttonSelectProgram.Name = "buttonSelectProgram";
			this.buttonSelectProgram.Size = new System.Drawing.Size(101, 23);
			this.buttonSelectProgram.TabIndex = 1;
			this.buttonSelectProgram.Text = "Select Program";
			this.buttonSelectProgram.UseVisualStyleBackColor = true;
			this.buttonSelectProgram.Click += new System.EventHandler(this.buttonSelectProgram_Click);
			// 
			// ScheduleItemEditForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(371, 572);
			this.Controls.Add(this.panel7);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panel6);
			this.Controls.Add(this.panel5);
			this.Controls.Add(this.panel4);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panelRecurrence);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ScheduleItemEditForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Scheduled Item";
			this.panelRecurrence.ResumeLayout(false);
			this.panelRecurrence.PerformLayout();
			this.recurrenceControls.ResumeLayout(false);
			this.weekTab.ResumeLayout(false);
			this.weekTab.PerformLayout();
			this.monthTab.ResumeLayout(false);
			this.monthTab.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.panelInterval.ResumeLayout(false);
			this.panelInterval.PerformLayout();
			this.panel4.ResumeLayout(false);
			this.panel4.PerformLayout();
			this.panel5.ResumeLayout(false);
			this.panel5.PerformLayout();
			this.panel6.ResumeLayout(false);
			this.panel6.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel7.ResumeLayout(false);
			this.panel7.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxDateUnitCount;
		private System.Windows.Forms.ComboBox comboBoxDateUnit;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.DateTimePicker dateTimePickerStartDate;
		private System.Windows.Forms.DateTimePicker dateTimePickerEndDate;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Panel panelRecurrence;
		private Common.Controls.TablessTabControl recurrenceControls;
		private System.Windows.Forms.TabPage weekTab;
		private System.Windows.Forms.RadioButton radioButtonSaturday;
		private System.Windows.Forms.RadioButton radioButtonFriday;
		private System.Windows.Forms.RadioButton radioButtonThursday;
		private System.Windows.Forms.RadioButton radioButtonWednesday;
		private System.Windows.Forms.RadioButton radioButtonTuesday;
		private System.Windows.Forms.RadioButton radioButtonMonday;
		private System.Windows.Forms.RadioButton radioButtonSunday;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TabPage monthTab;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.ComboBox comboBoxDow;
		private System.Windows.Forms.ComboBox comboBoxDayCount;
		private System.Windows.Forms.RadioButton radioButtonWeekDayCount;
		private System.Windows.Forms.RadioButton radioButtonLastDay;
		private System.Windows.Forms.TextBox textBoxSpecificDate;
		private System.Windows.Forms.RadioButton radioButtonSpecificDate;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox comboBoxStartTime;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Panel panelInterval;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textBoxInterval;
		private System.Windows.Forms.CheckBox checkBoxInterval;
		private System.Windows.Forms.ComboBox comboBoxEndTime;
		private System.Windows.Forms.CheckBox checkBoxRepeat;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.TabPage dayTab;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.Panel panel6;
		private System.Windows.Forms.CheckBox checkBoxNoRecurrence;
		private System.Windows.Forms.TabPage noneTab;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Panel panel7;
		private System.Windows.Forms.Label labelWhat;
		private System.Windows.Forms.Button buttonNewProgram;
		private System.Windows.Forms.Button buttonSelectProgram;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.Button buttonEditProgram;
	}
}