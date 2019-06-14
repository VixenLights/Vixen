namespace VixenModules.Analysis.BeatsAndBars
{
	partial class BeatsAndBarsDialog
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
			this.BeatsNameTB = new System.Windows.Forms.TextBox();
			this.GenerateButton = new System.Windows.Forms.Button();
			this.CancelBtn = new System.Windows.Forms.Button();
			this.grpDivisions = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.AllColorPanel = new System.Windows.Forms.Panel();
			this.BeatSplitsColorPanel = new System.Windows.Forms.Panel();
			this.BeatCountsColorPanel = new System.Windows.Forms.Panel();
			this.BarsColorPanel = new System.Windows.Forms.Panel();
			this.BeatSplitsCB = new System.Windows.Forms.CheckBox();
			this.AllFeaturesCB = new System.Windows.Forms.CheckBox();
			this.BeatCountsCB = new System.Windows.Forms.CheckBox();
			this.BarsCB = new System.Windows.Forms.CheckBox();
			this.PreviewGroupBox = new System.Windows.Forms.GroupBox();
			this.musicStaff1 = new VixenModules.Analysis.BeatsAndBars.MusicStaff();
			this.grpDivisions.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.label1.Location = new System.Drawing.Point(35, 29);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(126, 15);
			this.label1.TabIndex = 6;
			this.label1.Text = "Collection Base Name:";
			// 
			// BeatsNameTB
			// 
			this.BeatsNameTB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
			this.BeatsNameTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.BeatsNameTB.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.BeatsNameTB.Location = new System.Drawing.Point(38, 47);
			this.BeatsNameTB.Name = "BeatsNameTB";
			this.BeatsNameTB.Size = new System.Drawing.Size(194, 23);
			this.BeatsNameTB.TabIndex = 2;
			this.BeatsNameTB.Text = "Beats";
			// 
			// GenerateButton
			// 
			this.GenerateButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.GenerateButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.GenerateButton.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.GenerateButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.GenerateButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.GenerateButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.GenerateButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.GenerateButton.Location = new System.Drawing.Point(531, 560);
			this.GenerateButton.Name = "GenerateButton";
			this.GenerateButton.Size = new System.Drawing.Size(87, 27);
			this.GenerateButton.TabIndex = 4;
			this.GenerateButton.Text = "Generate";
			this.GenerateButton.UseVisualStyleBackColor = true;
			this.GenerateButton.Click += new System.EventHandler(this.GoButton_Click);
			this.GenerateButton.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.GenerateButton.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// CancelButton
			// 
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.CancelBtn.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.CancelBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.CancelBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.CancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CancelBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.CancelBtn.Location = new System.Drawing.Point(630, 560);
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Size = new System.Drawing.Size(87, 27);
			this.CancelBtn.TabIndex = 5;
			this.CancelBtn.Text = "Cancel";
			this.CancelBtn.UseVisualStyleBackColor = true;
			this.CancelBtn.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.CancelBtn.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// grpDivisions
			// 
			this.grpDivisions.Controls.Add(this.musicStaff1);
			this.grpDivisions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.grpDivisions.Location = new System.Drawing.Point(14, 14);
			this.grpDivisions.Name = "grpDivisions";
			this.grpDivisions.Size = new System.Drawing.Size(714, 205);
			this.grpDivisions.TabIndex = 13;
			this.grpDivisions.TabStop = false;
			this.grpDivisions.Text = "Divisons";
			this.grpDivisions.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.AllColorPanel);
			this.groupBox2.Controls.Add(this.BeatSplitsColorPanel);
			this.groupBox2.Controls.Add(this.BeatCountsColorPanel);
			this.groupBox2.Controls.Add(this.BarsColorPanel);
			this.groupBox2.Controls.Add(this.BeatSplitsCB);
			this.groupBox2.Controls.Add(this.AllFeaturesCB);
			this.groupBox2.Controls.Add(this.BeatCountsCB);
			this.groupBox2.Controls.Add(this.BarsCB);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.BeatsNameTB);
			this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.groupBox2.Location = new System.Drawing.Point(14, 407);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(709, 132);
			this.groupBox2.TabIndex = 14;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Output";
			this.groupBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// AllColorPanel
			// 
			this.AllColorPanel.BackColor = System.Drawing.Color.White;
			this.AllColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.AllColorPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.AllColorPanel.Location = new System.Drawing.Point(589, 84);
			this.AllColorPanel.Name = "AllColorPanel";
			this.AllColorPanel.Size = new System.Drawing.Size(28, 24);
			this.AllColorPanel.TabIndex = 18;
			this.AllColorPanel.Click += new System.EventHandler(this.ColorPanel_Click);
			// 
			// BeatSplitsColorPanel
			// 
			this.BeatSplitsColorPanel.BackColor = System.Drawing.Color.Lime;
			this.BeatSplitsColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.BeatSplitsColorPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.BeatSplitsColorPanel.Location = new System.Drawing.Point(434, 84);
			this.BeatSplitsColorPanel.Name = "BeatSplitsColorPanel";
			this.BeatSplitsColorPanel.Size = new System.Drawing.Size(28, 24);
			this.BeatSplitsColorPanel.TabIndex = 17;
			this.BeatSplitsColorPanel.Click += new System.EventHandler(this.ColorPanel_Click);
			// 
			// BeatCountsColorPanel
			// 
			this.BeatCountsColorPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
			this.BeatCountsColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.BeatCountsColorPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.BeatCountsColorPanel.Location = new System.Drawing.Point(275, 84);
			this.BeatCountsColorPanel.Name = "BeatCountsColorPanel";
			this.BeatCountsColorPanel.Size = new System.Drawing.Size(28, 24);
			this.BeatCountsColorPanel.TabIndex = 17;
			this.BeatCountsColorPanel.Click += new System.EventHandler(this.ColorPanel_Click);
			// 
			// BarsColorPanel
			// 
			this.BarsColorPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
			this.BarsColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.BarsColorPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.BarsColorPanel.Location = new System.Drawing.Point(100, 84);
			this.BarsColorPanel.Name = "BarsColorPanel";
			this.BarsColorPanel.Size = new System.Drawing.Size(28, 24);
			this.BarsColorPanel.TabIndex = 16;
			this.BarsColorPanel.Click += new System.EventHandler(this.ColorPanel_Click);
			// 
			// BeatSplitsCB
			// 
			this.BeatSplitsCB.AutoSize = true;
			this.BeatSplitsCB.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.BeatSplitsCB.Location = new System.Drawing.Point(338, 89);
			this.BeatSplitsCB.Name = "BeatSplitsCB";
			this.BeatSplitsCB.Size = new System.Drawing.Size(80, 19);
			this.BeatSplitsCB.TabIndex = 16;
			this.BeatSplitsCB.Text = "Beat Splits";
			this.BeatSplitsCB.UseVisualStyleBackColor = true;
			this.BeatSplitsCB.CheckedChanged += new System.EventHandler(this.BeatSplitsCB_CheckedChanged);
			// 
			// AllFeaturesCB
			// 
			this.AllFeaturesCB.AutoSize = true;
			this.AllFeaturesCB.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.AllFeaturesCB.Location = new System.Drawing.Point(488, 89);
			this.AllFeaturesCB.Name = "AllFeaturesCB";
			this.AllFeaturesCB.Size = new System.Drawing.Size(87, 19);
			this.AllFeaturesCB.TabIndex = 15;
			this.AllFeaturesCB.Text = "All Features";
			this.AllFeaturesCB.UseVisualStyleBackColor = true;
			this.AllFeaturesCB.CheckedChanged += new System.EventHandler(this.AllFeaturesCB_CheckedChanged);
			// 
			// BeatCountsCB
			// 
			this.BeatCountsCB.AutoSize = true;
			this.BeatCountsCB.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.BeatCountsCB.Location = new System.Drawing.Point(170, 89);
			this.BeatCountsCB.Name = "BeatCountsCB";
			this.BeatCountsCB.Size = new System.Drawing.Size(90, 19);
			this.BeatCountsCB.TabIndex = 14;
			this.BeatCountsCB.Text = "Beat Counts";
			this.BeatCountsCB.UseVisualStyleBackColor = true;
			this.BeatCountsCB.CheckedChanged += new System.EventHandler(this.BeatCountsCB_CheckedChanged);
			// 
			// BarsCB
			// 
			this.BarsCB.AutoSize = true;
			this.BarsCB.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.BarsCB.Location = new System.Drawing.Point(38, 89);
			this.BarsCB.Name = "BarsCB";
			this.BarsCB.Size = new System.Drawing.Size(48, 19);
			this.BarsCB.TabIndex = 13;
			this.BarsCB.Text = "Bars";
			this.BarsCB.UseVisualStyleBackColor = true;
			this.BarsCB.CheckedChanged += new System.EventHandler(this.BarsCB_CheckedChanged);
			// 
			// PreviewGroupBox
			// 
			this.PreviewGroupBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.PreviewGroupBox.Location = new System.Drawing.Point(14, 226);
			this.PreviewGroupBox.Name = "PreviewGroupBox";
			this.PreviewGroupBox.Size = new System.Drawing.Size(714, 145);
			this.PreviewGroupBox.TabIndex = 15;
			this.PreviewGroupBox.TabStop = false;
			this.PreviewGroupBox.Text = "Preview";
			this.PreviewGroupBox.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// musicStaff1
			// 
			this.musicStaff1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.musicStaff1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.musicStaff1.BeatPeriod = 0D;
			this.musicStaff1.BeatsPerBar = 4;
			this.musicStaff1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.musicStaff1.Location = new System.Drawing.Point(7, 22);
			this.musicStaff1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.musicStaff1.Name = "musicStaff1";
			this.musicStaff1.Size = new System.Drawing.Size(696, 174);
			this.musicStaff1.TabIndex = 6;
			this.musicStaff1.Paint += new System.Windows.Forms.PaintEventHandler(this.musicStaff1_Paint);
			// 
			// BeatsAndBarsDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.ClientSize = new System.Drawing.Size(740, 600);
			this.Controls.Add(this.PreviewGroupBox);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.grpDivisions);
			this.Controls.Add(this.CancelBtn);
			this.Controls.Add(this.GenerateButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "BeatsAndBarsDialog";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Beats and Bars";
			this.grpDivisions.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button GenerateButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox BeatsNameTB;
		private System.Windows.Forms.Button CancelBtn;
		private MusicStaff musicStaff1;
		private System.Windows.Forms.GroupBox grpDivisions;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox AllFeaturesCB;
		private System.Windows.Forms.CheckBox BeatCountsCB;
		private System.Windows.Forms.CheckBox BarsCB;
		private System.Windows.Forms.CheckBox BeatSplitsCB;
		private System.Windows.Forms.Panel AllColorPanel;
		private System.Windows.Forms.Panel BeatSplitsColorPanel;
		private System.Windows.Forms.Panel BeatCountsColorPanel;
		private System.Windows.Forms.Panel BarsColorPanel;
		private System.Windows.Forms.GroupBox PreviewGroupBox;

	}
}