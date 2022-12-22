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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.txtStartTime = new System.Windows.Forms.MaskedTextBox();
			this.txtDuration = new System.Windows.Forms.MaskedTextBox();
			this.txtDurationBetween = new System.Windows.Forms.MaskedTextBox();
			this.toolTip = new System.Windows.Forms.ToolTip();
			this.txtEffectCount = new System.Windows.Forms.NumericUpDown();
			this.lblPossibleEffects = new System.Windows.Forms.Label();
			this.listBoxMarkCollections = new System.Windows.Forms.ListView();
			this.checkBoxAlignToBeatMarks = new System.Windows.Forms.CheckBox();
			this.checkBoxFillDuration = new System.Windows.Forms.CheckBox();
			this.checkBoxSelectEffects = new System.Windows.Forms.CheckBox();
			this.btnShowBeatMarkOptions = new System.Windows.Forms.Button();
			this.lblShowBeatMarkOptions = new System.Windows.Forms.Label();
			this.btnHideBeatMarkOptions = new System.Windows.Forms.Button();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.panelMain = new System.Windows.Forms.Panel();
			this.lblEndingTime = new System.Windows.Forms.Label();
			this.txtEndTime = new System.Windows.Forms.MaskedTextBox();
			this.panelBeatAlignment = new System.Windows.Forms.Panel();
			this.checkBoxSkipEOBeat = new System.Windows.Forms.CheckBox();
			this.panelOKCancel = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.txtEffectCount)).BeginInit();
			this.flowLayoutPanel1.SuspendLayout();
			this.panelMain.SuspendLayout();
			this.panelBeatAlignment.SuspendLayout();
			this.panelOKCancel.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.label1.Location = new System.Drawing.Point(17, 45);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(140, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "Number of effects to add";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.label2.Location = new System.Drawing.Point(17, 75);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(75, 15);
			this.label2.TabIndex = 2;
			this.label2.Text = "Starting time";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.label3.Location = new System.Drawing.Point(17, 135);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(53, 15);
			this.label3.TabIndex = 4;
			this.label3.Text = "Duration";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.label4.Location = new System.Drawing.Point(17, 165);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(101, 15);
			this.label4.TabIndex = 6;
			this.label4.Text = "Duration between";
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.btnOK.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.btnOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnOK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.btnOK.Location = new System.Drawing.Point(17, 30);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(87, 27);
			this.btnOK.TabIndex = 13;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			this.btnOK.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.btnOK.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.btnCancel.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.btnCancel.Location = new System.Drawing.Point(198, 30);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(87, 27);
			this.btnCancel.TabIndex = 14;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.btnCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// txtStartTime
			// 
			this.txtStartTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.txtStartTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtStartTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.txtStartTime.Location = new System.Drawing.Point(169, 72);
			this.txtStartTime.Name = "txtStartTime";
			this.txtStartTime.Size = new System.Drawing.Size(116, 23);
			this.txtStartTime.TabIndex = 2;
			this.txtStartTime.MaskInputRejected += new System.Windows.Forms.MaskInputRejectedEventHandler(this.txtStartTime_MaskInputRejected);
			this.txtStartTime.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtStartTime_KeyDown);
			this.txtStartTime.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtStartTime_KeyUp);
			// 
			// txtDuration
			// 
			this.txtDuration.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.txtDuration.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtDuration.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.txtDuration.Location = new System.Drawing.Point(169, 132);
			this.txtDuration.Name = "txtDuration";
			this.txtDuration.Size = new System.Drawing.Size(116, 23);
			this.txtDuration.TabIndex = 4;
			this.txtDuration.MaskInputRejected += new System.Windows.Forms.MaskInputRejectedEventHandler(this.txtDuration_MaskInputRejected);
			this.txtDuration.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDuration_KeyDown);
			this.txtDuration.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtDuration_KeyUp);
			// 
			// txtDurationBetween
			// 
			this.txtDurationBetween.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.txtDurationBetween.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtDurationBetween.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.txtDurationBetween.Location = new System.Drawing.Point(169, 162);
			this.txtDurationBetween.Name = "txtDurationBetween";
			this.txtDurationBetween.Size = new System.Drawing.Size(116, 23);
			this.txtDurationBetween.TabIndex = 5;
			this.txtDurationBetween.MaskInputRejected += new System.Windows.Forms.MaskInputRejectedEventHandler(this.txtDurationBetween_MaskInputRejected);
			this.txtDurationBetween.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDurationBetween_KeyDown);
			this.txtDurationBetween.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtDurationBetween_KeyUp);
			// 
			// txtEffectCount
			// 
			this.txtEffectCount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.txtEffectCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtEffectCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.txtEffectCount.Location = new System.Drawing.Point(169, 42);
			this.txtEffectCount.Name = "txtEffectCount";
			this.txtEffectCount.Size = new System.Drawing.Size(117, 23);
			this.txtEffectCount.TabIndex = 1;
			// 
			// lblPossibleEffects
			// 
			this.lblPossibleEffects.AutoSize = true;
			this.lblPossibleEffects.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.lblPossibleEffects.Location = new System.Drawing.Point(89, 10);
			this.lblPossibleEffects.Name = "lblPossibleEffects";
			this.lblPossibleEffects.Size = new System.Drawing.Size(101, 15);
			this.lblPossibleEffects.TabIndex = 15;
			this.lblPossibleEffects.Text = "n effects possible.";
			this.lblPossibleEffects.DoubleClick += new System.EventHandler(this.lblPossibleEffects_DoubleClick);
			// 
			// listBoxMarkCollections
			// 
			this.listBoxMarkCollections.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.listBoxMarkCollections.CheckBoxes = true;
			this.listBoxMarkCollections.Enabled = false;
			this.listBoxMarkCollections.Location = new System.Drawing.Point(21, 83);
			this.listBoxMarkCollections.Name = "listBoxMarkCollections";
			this.listBoxMarkCollections.Size = new System.Drawing.Size(250, 137);
			this.listBoxMarkCollections.TabIndex = 10;
			this.listBoxMarkCollections.UseCompatibleStateImageBehavior = false;
			this.listBoxMarkCollections.View = System.Windows.Forms.View.List;
			this.listBoxMarkCollections.Visible = false;
			this.listBoxMarkCollections.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listBoxMarkCollections_ItemChecked);
			// 
			// checkBoxAlignToBeatMarks
			// 
			this.checkBoxAlignToBeatMarks.AutoSize = true;
			this.checkBoxAlignToBeatMarks.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.checkBoxAlignToBeatMarks.Location = new System.Drawing.Point(21, 3);
			this.checkBoxAlignToBeatMarks.Name = "checkBoxAlignToBeatMarks";
			this.checkBoxAlignToBeatMarks.Size = new System.Drawing.Size(129, 19);
			this.checkBoxAlignToBeatMarks.TabIndex = 7;
			this.checkBoxAlignToBeatMarks.Text = "Align to beat marks";
			this.checkBoxAlignToBeatMarks.UseVisualStyleBackColor = true;
			this.checkBoxAlignToBeatMarks.CheckedChanged += new System.EventHandler(this.checkBoxAlignToBeatMarks_CheckStateChanged);
			// 
			// checkBoxFillDuration
			// 
			this.checkBoxFillDuration.AutoCheck = false;
			this.checkBoxFillDuration.AutoSize = true;
			this.checkBoxFillDuration.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.checkBoxFillDuration.Location = new System.Drawing.Point(21, 57);
			this.checkBoxFillDuration.Name = "checkBoxFillDuration";
			this.checkBoxFillDuration.Size = new System.Drawing.Size(172, 19);
			this.checkBoxFillDuration.TabIndex = 9;
			this.checkBoxFillDuration.Text = "Fill duration between marks";
			this.checkBoxFillDuration.UseVisualStyleBackColor = true;
			this.checkBoxFillDuration.CheckedChanged += new System.EventHandler(this.checkBoxFillDuration_CheckStateChanged);
			// 
			// checkBoxSelectEffects
			// 
			this.checkBoxSelectEffects.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBoxSelectEffects.AutoSize = true;
			this.checkBoxSelectEffects.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.checkBoxSelectEffects.Location = new System.Drawing.Point(21, 4);
			this.checkBoxSelectEffects.Name = "checkBoxSelectEffects";
			this.checkBoxSelectEffects.Size = new System.Drawing.Size(126, 19);
			this.checkBoxSelectEffects.TabIndex = 11;
			this.checkBoxSelectEffects.Text = "Select / Edit effects";
			this.checkBoxSelectEffects.UseVisualStyleBackColor = true;
			// 
			// btnShowBeatMarkOptions
			// 
			this.btnShowBeatMarkOptions.FlatAppearance.BorderSize = 0;
			this.btnShowBeatMarkOptions.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.btnShowBeatMarkOptions.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.btnShowBeatMarkOptions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnShowBeatMarkOptions.Location = new System.Drawing.Point(17, 192);
			this.btnShowBeatMarkOptions.Name = "btnShowBeatMarkOptions";
			this.btnShowBeatMarkOptions.Size = new System.Drawing.Size(28, 27);
			this.btnShowBeatMarkOptions.TabIndex = 21;
			this.btnShowBeatMarkOptions.Text = "+";
			this.btnShowBeatMarkOptions.UseVisualStyleBackColor = true;
			this.btnShowBeatMarkOptions.Click += new System.EventHandler(this.btnShowBeatMarkOptions_Click);
			// 
			// lblShowBeatMarkOptions
			// 
			this.lblShowBeatMarkOptions.AutoSize = true;
			this.lblShowBeatMarkOptions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.lblShowBeatMarkOptions.Location = new System.Drawing.Point(52, 197);
			this.lblShowBeatMarkOptions.Name = "lblShowBeatMarkOptions";
			this.lblShowBeatMarkOptions.Size = new System.Drawing.Size(192, 15);
			this.lblShowBeatMarkOptions.TabIndex = 22;
			this.lblShowBeatMarkOptions.Text = "Show beat mark alignment options";
			// 
			// btnHideBeatMarkOptions
			// 
			this.btnHideBeatMarkOptions.FlatAppearance.BorderSize = 0;
			this.btnHideBeatMarkOptions.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.btnHideBeatMarkOptions.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.btnHideBeatMarkOptions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnHideBeatMarkOptions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.btnHideBeatMarkOptions.Location = new System.Drawing.Point(17, 192);
			this.btnHideBeatMarkOptions.Name = "btnHideBeatMarkOptions";
			this.btnHideBeatMarkOptions.Size = new System.Drawing.Size(28, 27);
			this.btnHideBeatMarkOptions.TabIndex = 6;
			this.btnHideBeatMarkOptions.Text = "+";
			this.btnHideBeatMarkOptions.UseVisualStyleBackColor = true;
			this.btnHideBeatMarkOptions.Visible = false;
			this.btnHideBeatMarkOptions.Click += new System.EventHandler(this.btnHideBeatMarkOptions_Click);
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.flowLayoutPanel1.Controls.Add(this.panelMain);
			this.flowLayoutPanel1.Controls.Add(this.panelBeatAlignment);
			this.flowLayoutPanel1.Controls.Add(this.panelOKCancel);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(321, 542);
			this.flowLayoutPanel1.TabIndex = 25;
			// 
			// panelMain
			// 
			this.panelMain.Controls.Add(this.lblEndingTime);
			this.panelMain.Controls.Add(this.txtEndTime);
			this.panelMain.Controls.Add(this.label1);
			this.panelMain.Controls.Add(this.label2);
			this.panelMain.Controls.Add(this.btnHideBeatMarkOptions);
			this.panelMain.Controls.Add(this.label3);
			this.panelMain.Controls.Add(this.lblShowBeatMarkOptions);
			this.panelMain.Controls.Add(this.label4);
			this.panelMain.Controls.Add(this.btnShowBeatMarkOptions);
			this.panelMain.Controls.Add(this.txtStartTime);
			this.panelMain.Controls.Add(this.txtDuration);
			this.panelMain.Controls.Add(this.txtDurationBetween);
			this.panelMain.Controls.Add(this.lblPossibleEffects);
			this.panelMain.Controls.Add(this.txtEffectCount);
			this.panelMain.Location = new System.Drawing.Point(3, 3);
			this.panelMain.Name = "panelMain";
			this.panelMain.Size = new System.Drawing.Size(303, 222);
			this.panelMain.TabIndex = 0;
			// 
			// lblEndingTime
			// 
			this.lblEndingTime.AutoSize = true;
			this.lblEndingTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.lblEndingTime.Location = new System.Drawing.Point(17, 105);
			this.lblEndingTime.Name = "lblEndingTime";
			this.lblEndingTime.Size = new System.Drawing.Size(71, 15);
			this.lblEndingTime.TabIndex = 24;
			this.lblEndingTime.Text = "Ending time";
			// 
			// txtEndTime
			// 
			this.txtEndTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.txtEndTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtEndTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.txtEndTime.Location = new System.Drawing.Point(169, 102);
			this.txtEndTime.Name = "txtEndTime";
			this.txtEndTime.Size = new System.Drawing.Size(116, 23);
			this.txtEndTime.TabIndex = 3;
			this.txtEndTime.MaskInputRejected += new System.Windows.Forms.MaskInputRejectedEventHandler(this.txtEndTime_MaskInputRejected);
			this.txtEndTime.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtEndTime_KeyDown);
			this.txtEndTime.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtEndTime_KeyUp);
			// 
			// panelBeatAlignment
			// 
			this.panelBeatAlignment.Controls.Add(this.checkBoxSkipEOBeat);
			this.panelBeatAlignment.Controls.Add(this.checkBoxAlignToBeatMarks);
			this.panelBeatAlignment.Controls.Add(this.checkBoxFillDuration);
			this.panelBeatAlignment.Controls.Add(this.listBoxMarkCollections);
			this.panelBeatAlignment.Location = new System.Drawing.Point(3, 231);
			this.panelBeatAlignment.Name = "panelBeatAlignment";
			this.panelBeatAlignment.Size = new System.Drawing.Size(303, 224);
			this.panelBeatAlignment.TabIndex = 1;
			// 
			// checkBoxSkipEOBeat
			// 
			this.checkBoxSkipEOBeat.AutoCheck = false;
			this.checkBoxSkipEOBeat.AutoSize = true;
			this.checkBoxSkipEOBeat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.checkBoxSkipEOBeat.Location = new System.Drawing.Point(21, 30);
			this.checkBoxSkipEOBeat.Name = "checkBoxSkipEOBeat";
			this.checkBoxSkipEOBeat.Size = new System.Drawing.Size(136, 19);
			this.checkBoxSkipEOBeat.TabIndex = 8;
			this.checkBoxSkipEOBeat.Text = "Skip every other beat";
			this.checkBoxSkipEOBeat.UseVisualStyleBackColor = true;
			this.checkBoxSkipEOBeat.CheckedChanged += new System.EventHandler(this.checkBoxSkipEOBeat_CheckedChanged);
			// 
			// panelOKCancel
			// 
			this.panelOKCancel.Controls.Add(this.checkBoxSelectEffects);
			this.panelOKCancel.Controls.Add(this.btnOK);
			this.panelOKCancel.Controls.Add(this.btnCancel);
			this.panelOKCancel.Location = new System.Drawing.Point(3, 461);
			this.panelOKCancel.Name = "panelOKCancel";
			this.panelOKCancel.Size = new System.Drawing.Size(303, 66);
			this.panelOKCancel.TabIndex = 2;
			// 
			// Form_AddMultipleEffects
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(321, 542);
			this.ControlBox = false;
			this.Controls.Add(this.flowLayoutPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "Form_AddMultipleEffects";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add Multiple Effects";
			this.Load += new System.EventHandler(this.Form_AddMultipleEffects_Load);
			((System.ComponentModel.ISupportInitialize)(this.txtEffectCount)).EndInit();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.panelMain.ResumeLayout(false);
			this.panelMain.PerformLayout();
			this.panelBeatAlignment.ResumeLayout(false);
			this.panelBeatAlignment.PerformLayout();
			this.panelOKCancel.ResumeLayout(false);
			this.panelOKCancel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.MaskedTextBox txtStartTime;
		private System.Windows.Forms.MaskedTextBox txtDuration;
		private System.Windows.Forms.MaskedTextBox txtDurationBetween;
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
		private System.Windows.Forms.MaskedTextBox txtEndTime;
		private System.Windows.Forms.CheckBox checkBoxSkipEOBeat;
	}
}