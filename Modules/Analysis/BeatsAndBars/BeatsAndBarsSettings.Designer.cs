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
			this.CancelButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.musicStaff1 = new VixenModules.Analysis.BeatsAndBars.MusicStaff();
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
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.label1.Location = new System.Drawing.Point(30, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(114, 13);
			this.label1.TabIndex = 6;
			this.label1.Text = "Collection Base Name:";
			// 
			// BeatsNameTB
			// 
			this.BeatsNameTB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
			this.BeatsNameTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.BeatsNameTB.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.BeatsNameTB.Location = new System.Drawing.Point(33, 41);
			this.BeatsNameTB.Name = "BeatsNameTB";
			this.BeatsNameTB.Size = new System.Drawing.Size(167, 20);
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
			this.GenerateButton.Location = new System.Drawing.Point(455, 485);
			this.GenerateButton.Name = "GenerateButton";
			this.GenerateButton.Size = new System.Drawing.Size(75, 23);
			this.GenerateButton.TabIndex = 4;
			this.GenerateButton.Text = "Generate";
			this.GenerateButton.UseVisualStyleBackColor = true;
			this.GenerateButton.Click += new System.EventHandler(this.GoButton_Click);
			this.GenerateButton.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
			this.GenerateButton.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.GenerateButton.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// CancelButton
			// 
			this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.CancelButton.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.CancelButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.CancelButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.CancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CancelButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.CancelButton.Location = new System.Drawing.Point(540, 485);
			this.CancelButton.Name = "CancelButton";
			this.CancelButton.Size = new System.Drawing.Size(75, 23);
			this.CancelButton.TabIndex = 5;
			this.CancelButton.Text = "Cancel";
			this.CancelButton.UseVisualStyleBackColor = true;
			this.CancelButton.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
			this.CancelButton.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.CancelButton.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.musicStaff1);
			this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(612, 178);
			this.groupBox1.TabIndex = 13;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Divisons";
			this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// musicStaff1
			// 
			this.musicStaff1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.musicStaff1.BeatPeriod = 0D;
			this.musicStaff1.BeatsPerBar = 4;
			this.musicStaff1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.musicStaff1.Location = new System.Drawing.Point(6, 19);
			this.musicStaff1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.musicStaff1.Name = "musicStaff1";
			this.musicStaff1.Size = new System.Drawing.Size(597, 151);
			this.musicStaff1.TabIndex = 6;
			this.musicStaff1.Paint += new System.Windows.Forms.PaintEventHandler(this.musicStaff1_Paint);
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
			this.groupBox2.Location = new System.Drawing.Point(12, 353);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(608, 114);
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
			this.AllColorPanel.Location = new System.Drawing.Point(505, 73);
			this.AllColorPanel.Name = "AllColorPanel";
			this.AllColorPanel.Size = new System.Drawing.Size(24, 21);
			this.AllColorPanel.TabIndex = 18;
			this.AllColorPanel.Click += new System.EventHandler(this.ColorPanel_Click);
			// 
			// BeatSplitsColorPanel
			// 
			this.BeatSplitsColorPanel.BackColor = System.Drawing.Color.Lime;
			this.BeatSplitsColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.BeatSplitsColorPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.BeatSplitsColorPanel.Location = new System.Drawing.Point(372, 73);
			this.BeatSplitsColorPanel.Name = "BeatSplitsColorPanel";
			this.BeatSplitsColorPanel.Size = new System.Drawing.Size(24, 21);
			this.BeatSplitsColorPanel.TabIndex = 17;
			this.BeatSplitsColorPanel.Click += new System.EventHandler(this.ColorPanel_Click);
			// 
			// BeatCountsColorPanel
			// 
			this.BeatCountsColorPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
			this.BeatCountsColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.BeatCountsColorPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.BeatCountsColorPanel.Location = new System.Drawing.Point(236, 73);
			this.BeatCountsColorPanel.Name = "BeatCountsColorPanel";
			this.BeatCountsColorPanel.Size = new System.Drawing.Size(24, 21);
			this.BeatCountsColorPanel.TabIndex = 17;
			this.BeatCountsColorPanel.Click += new System.EventHandler(this.ColorPanel_Click);
			// 
			// BarsColorPanel
			// 
			this.BarsColorPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
			this.BarsColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.BarsColorPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.BarsColorPanel.Location = new System.Drawing.Point(86, 73);
			this.BarsColorPanel.Name = "BarsColorPanel";
			this.BarsColorPanel.Size = new System.Drawing.Size(24, 21);
			this.BarsColorPanel.TabIndex = 16;
			this.BarsColorPanel.Click += new System.EventHandler(this.ColorPanel_Click);
			// 
			// BeatSplitsCB
			// 
			this.BeatSplitsCB.AutoSize = true;
			this.BeatSplitsCB.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.BeatSplitsCB.Location = new System.Drawing.Point(290, 77);
			this.BeatSplitsCB.Name = "BeatSplitsCB";
			this.BeatSplitsCB.Size = new System.Drawing.Size(76, 17);
			this.BeatSplitsCB.TabIndex = 16;
			this.BeatSplitsCB.Text = "Beat Splits";
			this.BeatSplitsCB.UseVisualStyleBackColor = true;
			this.BeatSplitsCB.CheckedChanged += new System.EventHandler(this.BeatSplitsCB_CheckedChanged);
			// 
			// AllFeaturesCB
			// 
			this.AllFeaturesCB.AutoSize = true;
			this.AllFeaturesCB.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.AllFeaturesCB.Location = new System.Drawing.Point(418, 77);
			this.AllFeaturesCB.Name = "AllFeaturesCB";
			this.AllFeaturesCB.Size = new System.Drawing.Size(81, 17);
			this.AllFeaturesCB.TabIndex = 15;
			this.AllFeaturesCB.Text = "All Features";
			this.AllFeaturesCB.UseVisualStyleBackColor = true;
			this.AllFeaturesCB.CheckedChanged += new System.EventHandler(this.AllFeaturesCB_CheckedChanged);
			// 
			// BeatCountsCB
			// 
			this.BeatCountsCB.AutoSize = true;
			this.BeatCountsCB.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.BeatCountsCB.Location = new System.Drawing.Point(146, 77);
			this.BeatCountsCB.Name = "BeatCountsCB";
			this.BeatCountsCB.Size = new System.Drawing.Size(84, 17);
			this.BeatCountsCB.TabIndex = 14;
			this.BeatCountsCB.Text = "Beat Counts";
			this.BeatCountsCB.UseVisualStyleBackColor = true;
			this.BeatCountsCB.CheckedChanged += new System.EventHandler(this.BeatCountsCB_CheckedChanged);
			// 
			// BarsCB
			// 
			this.BarsCB.AutoSize = true;
			this.BarsCB.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.BarsCB.Location = new System.Drawing.Point(33, 77);
			this.BarsCB.Name = "BarsCB";
			this.BarsCB.Size = new System.Drawing.Size(47, 17);
			this.BarsCB.TabIndex = 13;
			this.BarsCB.Text = "Bars";
			this.BarsCB.UseVisualStyleBackColor = true;
			this.BarsCB.CheckedChanged += new System.EventHandler(this.BarsCB_CheckedChanged);
			// 
			// PreviewGroupBox
			// 
			this.PreviewGroupBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.PreviewGroupBox.Location = new System.Drawing.Point(12, 196);
			this.PreviewGroupBox.Name = "PreviewGroupBox";
			this.PreviewGroupBox.Size = new System.Drawing.Size(612, 126);
			this.PreviewGroupBox.TabIndex = 15;
			this.PreviewGroupBox.TabStop = false;
			this.PreviewGroupBox.Text = "Preview";
			this.PreviewGroupBox.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// BeatsAndBarsDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.ClientSize = new System.Drawing.Size(634, 520);
			this.Controls.Add(this.PreviewGroupBox);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.CancelButton);
			this.Controls.Add(this.GenerateButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "BeatsAndBarsDialog";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Beats and Bars";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button GenerateButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox BeatsNameTB;
		private System.Windows.Forms.Button CancelButton;
		private MusicStaff musicStaff1;
		private System.Windows.Forms.GroupBox groupBox1;
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