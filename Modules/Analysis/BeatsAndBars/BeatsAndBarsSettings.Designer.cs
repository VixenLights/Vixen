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
			this.BeatColorPanel = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.BeatsNameTB = new System.Windows.Forms.TextBox();
			this.m_goButton = new System.Windows.Forms.Button();
			this.m_cancelButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.BeatSplitsCB = new System.Windows.Forms.CheckBox();
			this.EverythingCB = new System.Windows.Forms.CheckBox();
			this.BeatCountsDB = new System.Windows.Forms.CheckBox();
			this.BarsCB = new System.Windows.Forms.CheckBox();
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
			// BeatColorPanel
			// 
			this.BeatColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.BeatColorPanel.Location = new System.Drawing.Point(270, 47);
			this.BeatColorPanel.Name = "BeatColorPanel";
			this.BeatColorPanel.Size = new System.Drawing.Size(57, 21);
			this.BeatColorPanel.TabIndex = 10;
			this.BeatColorPanel.Click += new System.EventHandler(this.BeatColorPanel_Click);
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
			// m_goButton
			// 
			this.m_goButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_goButton.Location = new System.Drawing.Point(455, 364);
			this.m_goButton.Name = "m_goButton";
			this.m_goButton.Size = new System.Drawing.Size(75, 23);
			this.m_goButton.TabIndex = 4;
			this.m_goButton.Text = "Generate";
			this.m_goButton.UseVisualStyleBackColor = true;
			this.m_goButton.Click += new System.EventHandler(this.GoButton_Click);
			// 
			// m_cancelButton
			// 
			this.m_cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_cancelButton.Location = new System.Drawing.Point(540, 364);
			this.m_cancelButton.Name = "m_cancelButton";
			this.m_cancelButton.Size = new System.Drawing.Size(75, 23);
			this.m_cancelButton.TabIndex = 5;
			this.m_cancelButton.Text = "Cancel";
			this.m_cancelButton.UseVisualStyleBackColor = true;
			this.m_cancelButton.Click += new System.EventHandler(this.m_cancelButton_Click);
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
			this.groupBox2.Controls.Add(this.EverythingCB);
			this.groupBox2.Controls.Add(this.BeatCountsDB);
			this.groupBox2.Controls.Add(this.BarsCB);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.BeatsNameTB);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.BeatColorPanel);
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
			// 
			// EverythingCB
			// 
			this.EverythingCB.AutoSize = true;
			this.EverythingCB.Location = new System.Drawing.Point(170, 107);
			this.EverythingCB.Name = "EverythingCB";
			this.EverythingCB.Size = new System.Drawing.Size(76, 17);
			this.EverythingCB.TabIndex = 15;
			this.EverythingCB.Text = "Everything";
			this.EverythingCB.UseVisualStyleBackColor = true;
			// 
			// BeatCountsDB
			// 
			this.BeatCountsDB.AutoSize = true;
			this.BeatCountsDB.Location = new System.Drawing.Point(35, 107);
			this.BeatCountsDB.Name = "BeatCountsDB";
			this.BeatCountsDB.Size = new System.Drawing.Size(84, 17);
			this.BeatCountsDB.TabIndex = 14;
			this.BeatCountsDB.Text = "Beat Counts";
			this.BeatCountsDB.UseVisualStyleBackColor = true;
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
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.m_cancelButton);
			this.Controls.Add(this.m_goButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "BeatsAndBarsDialog";
			this.ShowIcon = false;
			this.Text = "Beats and Bars";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BeatsAndBarsDialog_FormClosing);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button m_goButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox BeatsNameTB;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel BeatColorPanel;
		private System.Windows.Forms.Button m_cancelButton;
		private MusicStaff musicStaff1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox EverythingCB;
		private System.Windows.Forms.CheckBox BeatCountsDB;
		private System.Windows.Forms.CheckBox BarsCB;
		private System.Windows.Forms.CheckBox BeatSplitsCB;

	}
}