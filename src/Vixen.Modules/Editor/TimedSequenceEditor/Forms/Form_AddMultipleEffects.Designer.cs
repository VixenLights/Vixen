namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class Form_AddMultipleEffects
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
			components = new System.ComponentModel.Container();
			label1 = new Label();
			label2 = new Label();
			label3 = new Label();
			label4 = new Label();
			btnOK = new Button();
			btnCancel = new Button();
			txtStartTime = new TimeControl();
			txtDuration = new TimeControl();
			txtDurationBetween = new TimeControl();
			toolTip = new ToolTip(components);
			txtEffectCount = new NumericUpDown();
			lblPossibleEffects = new Label();
			listBoxMarkCollections = new ListView();
			checkBoxAlignToBeatMarks = new CheckBox();
			checkBoxFillDuration = new CheckBox();
			checkBoxSelectEffects = new CheckBox();
			btnShowBeatMarkOptions = new Button();
			lblShowBeatMarkOptions = new Label();
			btnHideBeatMarkOptions = new Button();
			flowLayoutPanel1 = new FlowLayoutPanel();
			panelMain = new Panel();
			lblEndingTime = new Label();
			txtEndTime = new TimeControl();
			panelBeatAlignment = new Panel();
			chkUseMarkStartEnd = new CheckBox();
			checkBoxSkipEOBeat = new CheckBox();
			panelOKCancel = new Panel();
			((System.ComponentModel.ISupportInitialize)txtEffectCount).BeginInit();
			flowLayoutPanel1.SuspendLayout();
			panelMain.SuspendLayout();
			panelBeatAlignment.SuspendLayout();
			panelOKCancel.SuspendLayout();
			SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(17, 45);
			label1.Name = "label1";
			label1.Size = new Size(140, 15);
			label1.TabIndex = 0;
			label1.Text = "Number of effects to add";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(17, 75);
			label2.Name = "label2";
			label2.Size = new Size(75, 15);
			label2.TabIndex = 2;
			label2.Text = "Starting time";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new Point(17, 135);
			label3.Name = "label3";
			label3.Size = new Size(53, 15);
			label3.TabIndex = 4;
			label3.Text = "Duration";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new Point(17, 165);
			label4.Name = "label4";
			label4.Size = new Size(101, 15);
			label4.TabIndex = 6;
			label4.Text = "Duration between";
			// 
			// btnOK
			// 
			btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			btnOK.DialogResult = DialogResult.OK;
			btnOK.Location = new Point(17, 30);
			btnOK.Name = "btnOK";
			btnOK.Size = new Size(87, 27);
			btnOK.TabIndex = 13;
			btnOK.Text = "OK";
			btnOK.UseVisualStyleBackColor = true;
			btnOK.Click += btnOK_Click;
			// 
			// btnCancel
			// 
			btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			btnCancel.DialogResult = DialogResult.Cancel;
			btnCancel.Location = new Point(198, 30);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new Size(87, 27);
			btnCancel.TabIndex = 14;
			btnCancel.Text = "Cancel";
			btnCancel.UseVisualStyleBackColor = true;
			// 
			// txtStartTime
			// 
			txtStartTime.Location = new Point(169, 72);
			txtStartTime.Name = "txtStartTime";
			txtStartTime.Size = new Size(116, 23);
			txtStartTime.TabIndex = 2;
			// 
			// txtDuration
			// 
			txtDuration.Location = new Point(169, 132);
			txtDuration.Name = "txtDuration";
			txtDuration.Size = new Size(116, 23);
			txtDuration.TabIndex = 4;
			// 
			// txtDurationBetween
			// 
			txtDurationBetween.Location = new Point(169, 162);
			txtDurationBetween.Name = "txtDurationBetween";
			txtDurationBetween.Size = new Size(116, 23);
			txtDurationBetween.TabIndex = 5;
			// 
			// txtEffectCount
			// 
			txtEffectCount.Location = new Point(169, 42);
			txtEffectCount.Name = "txtEffectCount";
			txtEffectCount.Size = new Size(117, 23);
			txtEffectCount.TabIndex = 1;
			// 
			// lblPossibleEffects
			// 
			lblPossibleEffects.AutoSize = true;
			lblPossibleEffects.Location = new Point(89, 10);
			lblPossibleEffects.Name = "lblPossibleEffects";
			lblPossibleEffects.Size = new Size(101, 15);
			lblPossibleEffects.TabIndex = 15;
			lblPossibleEffects.Text = "n effects possible.";
			lblPossibleEffects.DoubleClick += lblPossibleEffects_DoubleClick;
			// 
			// listBoxMarkCollections
			// 
			listBoxMarkCollections.CheckBoxes = true;
			listBoxMarkCollections.Enabled = false;
			listBoxMarkCollections.Location = new Point(17, 118);
			listBoxMarkCollections.Name = "listBoxMarkCollections";
			listBoxMarkCollections.Size = new Size(268, 111);
			listBoxMarkCollections.TabIndex = 10;
			listBoxMarkCollections.UseCompatibleStateImageBehavior = false;
			listBoxMarkCollections.View = View.List;
			listBoxMarkCollections.Visible = false;
			listBoxMarkCollections.ItemChecked += listBoxMarkCollections_ItemChecked;
			// 
			// checkBoxAlignToBeatMarks
			// 
			checkBoxAlignToBeatMarks.AutoSize = true;
			checkBoxAlignToBeatMarks.Location = new Point(21, 3);
			checkBoxAlignToBeatMarks.Name = "checkBoxAlignToBeatMarks";
			checkBoxAlignToBeatMarks.Size = new Size(129, 19);
			checkBoxAlignToBeatMarks.TabIndex = 7;
			checkBoxAlignToBeatMarks.Text = "Align to beat marks";
			checkBoxAlignToBeatMarks.UseVisualStyleBackColor = true;
			checkBoxAlignToBeatMarks.CheckedChanged += checkBoxAlignToBeatMarks_CheckStateChanged;
			// 
			// checkBoxFillDuration
			// 
			checkBoxFillDuration.AutoCheck = false;
			checkBoxFillDuration.AutoSize = true;
			checkBoxFillDuration.Location = new Point(21, 57);
			checkBoxFillDuration.Name = "checkBoxFillDuration";
			checkBoxFillDuration.Size = new Size(172, 19);
			checkBoxFillDuration.TabIndex = 9;
			checkBoxFillDuration.Text = "Fill duration between marks";
			checkBoxFillDuration.UseVisualStyleBackColor = true;
			checkBoxFillDuration.CheckedChanged += checkBoxFillDuration_CheckStateChanged;
			// 
			// checkBoxSelectEffects
			// 
			checkBoxSelectEffects.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			checkBoxSelectEffects.AutoSize = true;
			checkBoxSelectEffects.Location = new Point(21, 4);
			checkBoxSelectEffects.Name = "checkBoxSelectEffects";
			checkBoxSelectEffects.Size = new Size(126, 19);
			checkBoxSelectEffects.TabIndex = 11;
			checkBoxSelectEffects.Text = "Select / Edit effects";
			checkBoxSelectEffects.UseVisualStyleBackColor = true;
			// 
			// btnShowBeatMarkOptions
			// 
			btnShowBeatMarkOptions.Location = new Point(17, 192);
			btnShowBeatMarkOptions.Name = "btnShowBeatMarkOptions";
			btnShowBeatMarkOptions.Size = new Size(28, 27);
			btnShowBeatMarkOptions.TabIndex = 21;
			btnShowBeatMarkOptions.UseVisualStyleBackColor = true;
			btnShowBeatMarkOptions.Click += btnShowBeatMarkOptions_Click;
			// 
			// lblShowBeatMarkOptions
			// 
			lblShowBeatMarkOptions.AutoSize = true;
			lblShowBeatMarkOptions.Location = new Point(52, 197);
			lblShowBeatMarkOptions.Name = "lblShowBeatMarkOptions";
			lblShowBeatMarkOptions.Size = new Size(192, 15);
			lblShowBeatMarkOptions.TabIndex = 22;
			lblShowBeatMarkOptions.Text = "Show beat mark alignment options";
			// 
			// btnHideBeatMarkOptions
			// 
			btnHideBeatMarkOptions.Location = new Point(17, 192);
			btnHideBeatMarkOptions.Name = "btnHideBeatMarkOptions";
			btnHideBeatMarkOptions.Size = new Size(28, 27);
			btnHideBeatMarkOptions.TabIndex = 6;
			btnHideBeatMarkOptions.Text = "+";
			btnHideBeatMarkOptions.UseVisualStyleBackColor = true;
			btnHideBeatMarkOptions.Visible = false;
			btnHideBeatMarkOptions.Click += btnHideBeatMarkOptions_Click;
			// 
			// flowLayoutPanel1
			// 
			flowLayoutPanel1.AutoSize = true;
			flowLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			flowLayoutPanel1.BorderStyle = BorderStyle.Fixed3D;
			flowLayoutPanel1.Controls.Add(panelMain);
			flowLayoutPanel1.Controls.Add(panelBeatAlignment);
			flowLayoutPanel1.Controls.Add(panelOKCancel);
			flowLayoutPanel1.Dock = DockStyle.Fill;
			flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
			flowLayoutPanel1.Location = new Point(0, 0);
			flowLayoutPanel1.Name = "flowLayoutPanel1";
			flowLayoutPanel1.Size = new Size(321, 557);
			flowLayoutPanel1.TabIndex = 25;
			// 
			// panelMain
			// 
			panelMain.Controls.Add(lblEndingTime);
			panelMain.Controls.Add(txtEndTime);
			panelMain.Controls.Add(label1);
			panelMain.Controls.Add(label2);
			panelMain.Controls.Add(btnHideBeatMarkOptions);
			panelMain.Controls.Add(label3);
			panelMain.Controls.Add(lblShowBeatMarkOptions);
			panelMain.Controls.Add(label4);
			panelMain.Controls.Add(btnShowBeatMarkOptions);
			panelMain.Controls.Add(txtStartTime);
			panelMain.Controls.Add(txtDuration);
			panelMain.Controls.Add(txtDurationBetween);
			panelMain.Controls.Add(lblPossibleEffects);
			panelMain.Controls.Add(txtEffectCount);
			panelMain.Location = new Point(3, 3);
			panelMain.Name = "panelMain";
			panelMain.Size = new Size(303, 222);
			panelMain.TabIndex = 0;
			// 
			// lblEndingTime
			// 
			lblEndingTime.AutoSize = true;
			lblEndingTime.Location = new Point(17, 105);
			lblEndingTime.Name = "lblEndingTime";
			lblEndingTime.Size = new Size(71, 15);
			lblEndingTime.TabIndex = 24;
			lblEndingTime.Text = "Ending time";
			// 
			// txtEndTime
			// 
			txtEndTime.Location = new Point(169, 102);
			txtEndTime.Name = "txtEndTime";
			txtEndTime.Size = new Size(116, 23);
			txtEndTime.TabIndex = 3;
			// 
			// panelBeatAlignment
			// 
			panelBeatAlignment.Controls.Add(chkUseMarkStartEnd);
			panelBeatAlignment.Controls.Add(checkBoxSkipEOBeat);
			panelBeatAlignment.Controls.Add(checkBoxAlignToBeatMarks);
			panelBeatAlignment.Controls.Add(checkBoxFillDuration);
			panelBeatAlignment.Controls.Add(listBoxMarkCollections);
			panelBeatAlignment.Location = new Point(3, 231);
			panelBeatAlignment.Name = "panelBeatAlignment";
			panelBeatAlignment.Size = new Size(303, 238);
			panelBeatAlignment.TabIndex = 1;
			// 
			// chkUseMarkStartEnd
			// 
			chkUseMarkStartEnd.AutoCheck = false;
			chkUseMarkStartEnd.AutoSize = true;
			chkUseMarkStartEnd.Location = new Point(21, 82);
			chkUseMarkStartEnd.Name = "chkUseMarkStartEnd";
			chkUseMarkStartEnd.Size = new Size(155, 19);
			chkUseMarkStartEnd.TabIndex = 11;
			chkUseMarkStartEnd.Text = "Align to mark start / end";
			chkUseMarkStartEnd.UseVisualStyleBackColor = true;
			chkUseMarkStartEnd.CheckStateChanged += chkUseMarkStartEnd_CheckStateChanged;
			// 
			// checkBoxSkipEOBeat
			// 
			checkBoxSkipEOBeat.AutoCheck = false;
			checkBoxSkipEOBeat.AutoSize = true;
			checkBoxSkipEOBeat.Location = new Point(21, 30);
			checkBoxSkipEOBeat.Name = "checkBoxSkipEOBeat";
			checkBoxSkipEOBeat.Size = new Size(136, 19);
			checkBoxSkipEOBeat.TabIndex = 8;
			checkBoxSkipEOBeat.Text = "Skip every other beat";
			checkBoxSkipEOBeat.UseVisualStyleBackColor = true;
			checkBoxSkipEOBeat.CheckedChanged += checkBoxSkipEOBeat_CheckedChanged;
			// 
			// panelOKCancel
			// 
			panelOKCancel.Controls.Add(checkBoxSelectEffects);
			panelOKCancel.Controls.Add(btnOK);
			panelOKCancel.Controls.Add(btnCancel);
			panelOKCancel.Location = new Point(3, 475);
			panelOKCancel.Name = "panelOKCancel";
			panelOKCancel.Size = new Size(303, 66);
			panelOKCancel.TabIndex = 2;
			// 
			// Form_AddMultipleEffects
			// 
			AcceptButton = btnOK;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = true;
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			BackColor = Color.FromArgb(68, 68, 68);
			CancelButton = btnCancel;
			ClientSize = new Size(321, 557);
			ControlBox = false;
			Controls.Add(flowLayoutPanel1);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			Name = "Form_AddMultipleEffects";
			StartPosition = FormStartPosition.CenterParent;
			Text = "Add Multiple Effects";
			Load += Form_AddMultipleEffects_Load;
			((System.ComponentModel.ISupportInitialize)txtEffectCount).EndInit();
			flowLayoutPanel1.ResumeLayout(false);
			panelMain.ResumeLayout(false);
			panelMain.PerformLayout();
			panelBeatAlignment.ResumeLayout(false);
			panelBeatAlignment.PerformLayout();
			panelOKCancel.ResumeLayout(false);
			panelOKCancel.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private TimeControl txtStartTime;
		private TimeControl txtDuration;
		private TimeControl txtDurationBetween;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.NumericUpDown txtEffectCount;
		private System.Windows.Forms.Label lblPossibleEffects;
		private System.Windows.Forms.ListView listBoxMarkCollections;
		private System.Windows.Forms.CheckBox checkBoxAlignToBeatMarks;
		private System.Windows.Forms.CheckBox checkBoxFillDuration;
		private System.Windows.Forms.CheckBox checkBoxSelectEffects;
		private System.Windows.Forms.Button btnShowBeatMarkOptions;
		private System.Windows.Forms.Label lblShowBeatMarkOptions;
		private System.Windows.Forms.Button btnHideBeatMarkOptions;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Panel panelMain;
		private System.Windows.Forms.Panel panelBeatAlignment;
		private System.Windows.Forms.Panel panelOKCancel;
		private System.Windows.Forms.Label lblEndingTime;
		private TimeControl txtEndTime;
		private System.Windows.Forms.CheckBox checkBoxSkipEOBeat;
		private CheckBox chkUseMarkStartEnd;
	}
}