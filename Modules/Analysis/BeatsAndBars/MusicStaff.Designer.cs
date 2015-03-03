namespace VixenModules.Analysis.BeatsAndBars
{
	partial class MusicStaff
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.tsLeftButton = new System.Windows.Forms.Button();
			this.tsRightButton = new System.Windows.Forms.Button();
			this.tsLabel = new System.Windows.Forms.Label();
			this.BeatsPerBarLabel = new System.Windows.Forms.Label();
			this.NoteSizeLabel = new System.Windows.Forms.Label();
			this.splitBeatsCB = new System.Windows.Forms.CheckBox();
			this.BPMLabel = new System.Windows.Forms.Label();
			this.BPMLabelVal = new System.Windows.Forms.Label();
			this.BarPeriodLabel = new System.Windows.Forms.Label();
			this.BarPeriodLabelVal = new System.Windows.Forms.Label();
			this.DivTimeLabel = new System.Windows.Forms.Label();
			this.DivTimeLabelVal = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox1.Image = global::VixenModules.Analysis.BeatsAndBars.Properties.Resources.fullstaff;
			this.pictureBox1.Location = new System.Drawing.Point(5, 31);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(390, 76);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.staffBox1_Paint);
			// 
			// tsLeftButton
			// 
			this.tsLeftButton.Location = new System.Drawing.Point(73, 113);
			this.tsLeftButton.Name = "tsLeftButton";
			this.tsLeftButton.Size = new System.Drawing.Size(27, 23);
			this.tsLeftButton.TabIndex = 1;
			this.tsLeftButton.Text = "<-";
			this.tsLeftButton.UseVisualStyleBackColor = true;
			this.tsLeftButton.Click += new System.EventHandler(this.tsLeftButton_Click);
			// 
			// tsRightButton
			// 
			this.tsRightButton.Location = new System.Drawing.Point(193, 113);
			this.tsRightButton.Name = "tsRightButton";
			this.tsRightButton.Size = new System.Drawing.Size(27, 23);
			this.tsRightButton.TabIndex = 2;
			this.tsRightButton.Text = "->";
			this.tsRightButton.UseVisualStyleBackColor = true;
			this.tsRightButton.Click += new System.EventHandler(this.tsRightButton_Click);
			// 
			// tsLabel
			// 
			this.tsLabel.AutoSize = true;
			this.tsLabel.Location = new System.Drawing.Point(106, 118);
			this.tsLabel.Name = "tsLabel";
			this.tsLabel.Size = new System.Drawing.Size(81, 13);
			this.tsLabel.TabIndex = 3;
			this.tsLabel.Text = "Time Signature:";
			// 
			// BeatsPerBarLabel
			// 
			this.BeatsPerBarLabel.AutoSize = true;
			this.BeatsPerBarLabel.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BeatsPerBarLabel.Location = new System.Drawing.Point(56, 48);
			this.BeatsPerBarLabel.Name = "BeatsPerBarLabel";
			this.BeatsPerBarLabel.Size = new System.Drawing.Size(16, 20);
			this.BeatsPerBarLabel.TabIndex = 4;
			this.BeatsPerBarLabel.Text = "0";
			// 
			// NoteSizeLabel
			// 
			this.NoteSizeLabel.AutoSize = true;
			this.NoteSizeLabel.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.NoteSizeLabel.Location = new System.Drawing.Point(56, 68);
			this.NoteSizeLabel.Name = "NoteSizeLabel";
			this.NoteSizeLabel.Size = new System.Drawing.Size(16, 20);
			this.NoteSizeLabel.TabIndex = 5;
			this.NoteSizeLabel.Text = "0";
			// 
			// splitBeatsCB
			// 
			this.splitBeatsCB.AutoSize = true;
			this.splitBeatsCB.Location = new System.Drawing.Point(253, 119);
			this.splitBeatsCB.Name = "splitBeatsCB";
			this.splitBeatsCB.Size = new System.Drawing.Size(76, 17);
			this.splitBeatsCB.TabIndex = 6;
			this.splitBeatsCB.Text = "Split Beats";
			this.splitBeatsCB.UseVisualStyleBackColor = true;
			this.splitBeatsCB.CheckedChanged += new System.EventHandler(this.splitBeatsCB_CheckedChanged);
			// 
			// BPMLabel
			// 
			this.BPMLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.BPMLabel.AutoSize = true;
			this.BPMLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BPMLabel.Location = new System.Drawing.Point(70, 14);
			this.BPMLabel.Name = "BPMLabel";
			this.BPMLabel.Size = new System.Drawing.Size(37, 13);
			this.BPMLabel.TabIndex = 7;
			this.BPMLabel.Text = "BPM:";
			// 
			// BPMLabelVal
			// 
			this.BPMLabelVal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.BPMLabelVal.AutoSize = true;
			this.BPMLabelVal.Location = new System.Drawing.Point(109, 14);
			this.BPMLabelVal.Name = "BPMLabelVal";
			this.BPMLabelVal.Size = new System.Drawing.Size(13, 13);
			this.BPMLabelVal.TabIndex = 8;
			this.BPMLabelVal.Text = "0";
			// 
			// BarPeriodLabel
			// 
			this.BarPeriodLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.BarPeriodLabel.AutoSize = true;
			this.BarPeriodLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BarPeriodLabel.Location = new System.Drawing.Point(226, 15);
			this.BarPeriodLabel.Name = "BarPeriodLabel";
			this.BarPeriodLabel.Size = new System.Drawing.Size(102, 13);
			this.BarPeriodLabel.TabIndex = 9;
			this.BarPeriodLabel.Text = "Bar Period (sec):";
			// 
			// BarPeriodLabelVal
			// 
			this.BarPeriodLabelVal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.BarPeriodLabelVal.AutoSize = true;
			this.BarPeriodLabelVal.Location = new System.Drawing.Point(317, 15);
			this.BarPeriodLabelVal.Name = "BarPeriodLabelVal";
			this.BarPeriodLabelVal.Size = new System.Drawing.Size(13, 13);
			this.BarPeriodLabelVal.TabIndex = 10;
			this.BarPeriodLabelVal.Text = "0";
			// 
			// DivTimeLabel
			// 
			this.DivTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.DivTimeLabel.AutoSize = true;
			this.DivTimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.DivTimeLabel.Location = new System.Drawing.Point(128, 14);
			this.DivTimeLabel.Name = "DivTimeLabel";
			this.DivTimeLabel.Size = new System.Drawing.Size(88, 13);
			this.DivTimeLabel.TabIndex = 11;
			this.DivTimeLabel.Text = "Div Time (ms):";
			// 
			// DivTimeLabelVal
			// 
			this.DivTimeLabelVal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.DivTimeLabelVal.AutoSize = true;
			this.DivTimeLabelVal.Location = new System.Drawing.Point(207, 14);
			this.DivTimeLabelVal.Name = "DivTimeLabelVal";
			this.DivTimeLabelVal.Size = new System.Drawing.Size(13, 13);
			this.DivTimeLabelVal.TabIndex = 12;
			this.DivTimeLabelVal.Text = "0";
			// 
			// MusicStaff
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.DivTimeLabelVal);
			this.Controls.Add(this.DivTimeLabel);
			this.Controls.Add(this.BarPeriodLabelVal);
			this.Controls.Add(this.BarPeriodLabel);
			this.Controls.Add(this.BPMLabelVal);
			this.Controls.Add(this.BPMLabel);
			this.Controls.Add(this.splitBeatsCB);
			this.Controls.Add(this.NoteSizeLabel);
			this.Controls.Add(this.BeatsPerBarLabel);
			this.Controls.Add(this.tsLabel);
			this.Controls.Add(this.tsRightButton);
			this.Controls.Add(this.tsLeftButton);
			this.Controls.Add(this.pictureBox1);
			this.Name = "MusicStaff";
			this.Size = new System.Drawing.Size(398, 143);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.MusicStaff_Paint);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button tsLeftButton;
		private System.Windows.Forms.Button tsRightButton;
		private System.Windows.Forms.Label tsLabel;
		private System.Windows.Forms.Label BeatsPerBarLabel;
		private System.Windows.Forms.Label NoteSizeLabel;
		private System.Windows.Forms.CheckBox splitBeatsCB;
		private System.Windows.Forms.Label BPMLabel;
		private System.Windows.Forms.Label BPMLabelVal;
		private System.Windows.Forms.Label BarPeriodLabel;
		private System.Windows.Forms.Label BarPeriodLabelVal;
		private System.Windows.Forms.Label DivTimeLabel;
		private System.Windows.Forms.Label DivTimeLabelVal;
	}
}
