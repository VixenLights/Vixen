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
			this.label2 = new System.Windows.Forms.Label();
			this.BaseColorPanel = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.BeatsNameTB = new System.Windows.Forms.TextBox();
			this.GenerateButton = new System.Windows.Forms.Button();
			this.CancelButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.BeatSplitsCB = new System.Windows.Forms.CheckBox();
			this.AllFeaturesCB = new System.Windows.Forms.CheckBox();
			this.BeatCountsCB = new System.Windows.Forms.CheckBox();
			this.BarsCB = new System.Windows.Forms.CheckBox();
			this.PreviewButton = new System.Windows.Forms.Button();
			this.musicStaff1 = new VixenModules.Analysis.BeatsAndBars.MusicStaff();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(267, 31);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(61, 13);
			this.label2.TabIndex = 12;
			this.label2.Text = "Base Color:";
			// 
			// BaseColorPanel
			// 
			this.BaseColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.BaseColorPanel.Location = new System.Drawing.Point(270, 47);
			this.BaseColorPanel.Name = "BaseColorPanel";
			this.BaseColorPanel.Size = new System.Drawing.Size(57, 21);
			this.BaseColorPanel.TabIndex = 10;
			this.BaseColorPanel.Click += new System.EventHandler(this.BeatColorPanel_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(32, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(114, 13);
			this.label1.TabIndex = 6;
			this.label1.Text = "Collection Base Name:";
			// 
			// BeatsNameTB
			// 
			this.BeatsNameTB.Location = new System.Drawing.Point(35, 48);
			this.BeatsNameTB.Name = "BeatsNameTB";
			this.BeatsNameTB.Size = new System.Drawing.Size(168, 20);
			this.BeatsNameTB.TabIndex = 2;
			this.BeatsNameTB.Text = "Beats";
			// 
			// GenerateButton
			// 
			this.GenerateButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.GenerateButton.Location = new System.Drawing.Point(455, 364);
			this.GenerateButton.Name = "GenerateButton";
			this.GenerateButton.Size = new System.Drawing.Size(75, 23);
			this.GenerateButton.TabIndex = 4;
			this.GenerateButton.Text = "Generate";
			this.GenerateButton.UseVisualStyleBackColor = true;
			this.GenerateButton.Click += new System.EventHandler(this.GoButton_Click);
			// 
			// CancelButton
			// 
			this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton.Location = new System.Drawing.Point(540, 364);
			this.CancelButton.Name = "CancelButton";
			this.CancelButton.Size = new System.Drawing.Size(75, 23);
			this.CancelButton.TabIndex = 5;
			this.CancelButton.Text = "Cancel";
			this.CancelButton.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.musicStaff1);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(612, 164);
			this.groupBox1.TabIndex = 13;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Divisons";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.BeatSplitsCB);
			this.groupBox2.Controls.Add(this.AllFeaturesCB);
			this.groupBox2.Controls.Add(this.BeatCountsCB);
			this.groupBox2.Controls.Add(this.BarsCB);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.BeatsNameTB);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.BaseColorPanel);
			this.groupBox2.Location = new System.Drawing.Point(12, 199);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(608, 146);
			this.groupBox2.TabIndex = 14;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Output";
			// 
			// BeatSplitsCB
			// 
			this.BeatSplitsCB.AutoSize = true;
			this.BeatSplitsCB.Location = new System.Drawing.Point(170, 84);
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
			this.AllFeaturesCB.Location = new System.Drawing.Point(170, 107);
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
			this.BeatCountsCB.Location = new System.Drawing.Point(35, 107);
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
			this.BarsCB.Location = new System.Drawing.Point(35, 84);
			this.BarsCB.Name = "BarsCB";
			this.BarsCB.Size = new System.Drawing.Size(47, 17);
			this.BarsCB.TabIndex = 13;
			this.BarsCB.Text = "Bars";
			this.BarsCB.UseVisualStyleBackColor = true;
			this.BarsCB.CheckedChanged += new System.EventHandler(this.BarsCB_CheckedChanged);
			// 
			// PreviewButton
			// 
			this.PreviewButton.Location = new System.Drawing.Point(12, 364);
			this.PreviewButton.Name = "PreviewButton";
			this.PreviewButton.Size = new System.Drawing.Size(75, 23);
			this.PreviewButton.TabIndex = 15;
			this.PreviewButton.Text = "Preview";
			this.PreviewButton.UseVisualStyleBackColor = true;
			// 
			// musicStaff1
			// 
			this.musicStaff1.BeatsPerBar = 4;
			this.musicStaff1.Location = new System.Drawing.Point(6, 29);
			this.musicStaff1.Name = "musicStaff1";
			this.musicStaff1.Size = new System.Drawing.Size(597, 112);
			this.musicStaff1.TabIndex = 6;
			this.musicStaff1.Paint += new System.Windows.Forms.PaintEventHandler(this.musicStaff1_Paint);
			// 
			// BeatsAndBarsDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(634, 418);
			this.Controls.Add(this.PreviewButton);
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
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel BaseColorPanel;
		private System.Windows.Forms.Button CancelButton;
		private MusicStaff musicStaff1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox AllFeaturesCB;
		private System.Windows.Forms.CheckBox BeatCountsCB;
		private System.Windows.Forms.CheckBox BarsCB;
		private System.Windows.Forms.CheckBox BeatSplitsCB;
		private System.Windows.Forms.Button PreviewButton;

	}
}