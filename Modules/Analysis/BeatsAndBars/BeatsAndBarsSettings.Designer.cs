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
			this.m_paramsGroupBox = new System.Windows.Forms.GroupBox();
			this.m_vampParamCtrl = new QMLibrary.VampParamCtrl();
			this.m_outputGroupBox = new System.Windows.Forms.GroupBox();
			this.m_barsSubmarks = new System.Windows.Forms.NumericUpDown();
			this.m_beatsSubMarks = new System.Windows.Forms.NumericUpDown();
			this.m_subMarksLabel = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.m_barsNameTB = new System.Windows.Forms.TextBox();
			this.m_beatsNameTB = new System.Windows.Forms.TextBox();
			this.m_barsCB = new System.Windows.Forms.CheckBox();
			this.m_beatsCB = new System.Windows.Forms.CheckBox();
			this.m_goButton = new System.Windows.Forms.Button();
			this.m_beatColorPanel = new System.Windows.Forms.Panel();
			this.m_barColorPanel = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.m_paramsGroupBox.SuspendLayout();
			this.m_outputGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_barsSubmarks)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_beatsSubMarks)).BeginInit();
			this.SuspendLayout();
			// 
			// m_paramsGroupBox
			// 
			this.m_paramsGroupBox.Controls.Add(this.m_vampParamCtrl);
			this.m_paramsGroupBox.Location = new System.Drawing.Point(12, 12);
			this.m_paramsGroupBox.Name = "m_paramsGroupBox";
			this.m_paramsGroupBox.Size = new System.Drawing.Size(350, 24);
			this.m_paramsGroupBox.TabIndex = 2;
			this.m_paramsGroupBox.TabStop = false;
			this.m_paramsGroupBox.Text = "Parameters";
			// 
			// m_vampParamCtrl
			// 
			this.m_vampParamCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_vampParamCtrl.AutoSize = true;
			this.m_vampParamCtrl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.m_vampParamCtrl.Location = new System.Drawing.Point(6, 19);
			this.m_vampParamCtrl.Name = "m_vampParamCtrl";
			this.m_vampParamCtrl.Size = new System.Drawing.Size(344, 4);
			this.m_vampParamCtrl.TabIndex = 0;
			// 
			// m_outputGroupBox
			// 
			this.m_outputGroupBox.Controls.Add(this.label2);
			this.m_outputGroupBox.Controls.Add(this.m_barColorPanel);
			this.m_outputGroupBox.Controls.Add(this.m_beatColorPanel);
			this.m_outputGroupBox.Controls.Add(this.m_barsSubmarks);
			this.m_outputGroupBox.Controls.Add(this.m_beatsSubMarks);
			this.m_outputGroupBox.Controls.Add(this.m_subMarksLabel);
			this.m_outputGroupBox.Controls.Add(this.label1);
			this.m_outputGroupBox.Controls.Add(this.m_barsNameTB);
			this.m_outputGroupBox.Controls.Add(this.m_beatsNameTB);
			this.m_outputGroupBox.Controls.Add(this.m_barsCB);
			this.m_outputGroupBox.Controls.Add(this.m_beatsCB);
			this.m_outputGroupBox.Location = new System.Drawing.Point(13, 64);
			this.m_outputGroupBox.Name = "m_outputGroupBox";
			this.m_outputGroupBox.Size = new System.Drawing.Size(494, 130);
			this.m_outputGroupBox.TabIndex = 3;
			this.m_outputGroupBox.TabStop = false;
			this.m_outputGroupBox.Text = "Collections to Generate";
			// 
			// m_barsSubmarks
			// 
			this.m_barsSubmarks.Location = new System.Drawing.Point(270, 87);
			this.m_barsSubmarks.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.m_barsSubmarks.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.m_barsSubmarks.Name = "m_barsSubmarks";
			this.m_barsSubmarks.Size = new System.Drawing.Size(46, 20);
			this.m_barsSubmarks.TabIndex = 9;
			this.m_barsSubmarks.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.m_barsSubmarks.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// m_beatsSubMarks
			// 
			this.m_beatsSubMarks.Location = new System.Drawing.Point(270, 55);
			this.m_beatsSubMarks.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.m_beatsSubMarks.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.m_beatsSubMarks.Name = "m_beatsSubMarks";
			this.m_beatsSubMarks.Size = new System.Drawing.Size(46, 20);
			this.m_beatsSubMarks.TabIndex = 8;
			this.m_beatsSubMarks.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.m_beatsSubMarks.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// m_subMarksLabel
			// 
			this.m_subMarksLabel.AutoSize = true;
			this.m_subMarksLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_subMarksLabel.Location = new System.Drawing.Point(267, 26);
			this.m_subMarksLabel.Name = "m_subMarksLabel";
			this.m_subMarksLabel.Size = new System.Drawing.Size(97, 13);
			this.m_subMarksLabel.TabIndex = 7;
			this.m_subMarksLabel.Text = "Marks Per Feature:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(76, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 6;
			this.label1.Text = "Name:";
			// 
			// m_barsNameTB
			// 
			this.m_barsNameTB.Location = new System.Drawing.Point(79, 86);
			this.m_barsNameTB.Name = "m_barsNameTB";
			this.m_barsNameTB.Size = new System.Drawing.Size(168, 20);
			this.m_barsNameTB.TabIndex = 4;
			this.m_barsNameTB.Text = "Bars";
			this.m_barsNameTB.TextChanged += new System.EventHandler(this.m_barsNameTB_TextChanged);
			// 
			// m_beatsNameTB
			// 
			this.m_beatsNameTB.Location = new System.Drawing.Point(79, 54);
			this.m_beatsNameTB.Name = "m_beatsNameTB";
			this.m_beatsNameTB.Size = new System.Drawing.Size(168, 20);
			this.m_beatsNameTB.TabIndex = 2;
			this.m_beatsNameTB.Text = "Beats";
			this.m_beatsNameTB.TextChanged += new System.EventHandler(this.m_beatsNameTB_TextChanged);
			// 
			// m_barsCB
			// 
			this.m_barsCB.AutoSize = true;
			this.m_barsCB.Location = new System.Drawing.Point(6, 88);
			this.m_barsCB.Name = "m_barsCB";
			this.m_barsCB.Size = new System.Drawing.Size(47, 17);
			this.m_barsCB.TabIndex = 1;
			this.m_barsCB.Text = "Bars";
			this.m_barsCB.UseVisualStyleBackColor = true;
			this.m_barsCB.CheckedChanged += new System.EventHandler(this.m_barsCB_CheckedChanged);
			// 
			// m_beatsCB
			// 
			this.m_beatsCB.AutoSize = true;
			this.m_beatsCB.Location = new System.Drawing.Point(6, 56);
			this.m_beatsCB.Name = "m_beatsCB";
			this.m_beatsCB.Size = new System.Drawing.Size(53, 17);
			this.m_beatsCB.TabIndex = 0;
			this.m_beatsCB.Text = "Beats";
			this.m_beatsCB.UseVisualStyleBackColor = true;
			this.m_beatsCB.CheckedChanged += new System.EventHandler(this.m_beatsCB_CheckedChanged);
			// 
			// m_goButton
			// 
			this.m_goButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_goButton.Location = new System.Drawing.Point(117, 221);
			this.m_goButton.Name = "m_goButton";
			this.m_goButton.Size = new System.Drawing.Size(75, 23);
			this.m_goButton.TabIndex = 4;
			this.m_goButton.Text = "Go";
			this.m_goButton.UseVisualStyleBackColor = true;
			// 
			// m_beatColorPanel
			// 
			this.m_beatColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_beatColorPanel.Location = new System.Drawing.Point(392, 52);
			this.m_beatColorPanel.Name = "m_beatColorPanel";
			this.m_beatColorPanel.Size = new System.Drawing.Size(57, 21);
			this.m_beatColorPanel.TabIndex = 10;
			this.m_beatColorPanel.Click += new System.EventHandler(this.m_beatColorPanel_Click);
			// 
			// m_barColorPanel
			// 
			this.m_barColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_barColorPanel.Location = new System.Drawing.Point(392, 85);
			this.m_barColorPanel.Name = "m_barColorPanel";
			this.m_barColorPanel.Size = new System.Drawing.Size(57, 21);
			this.m_barColorPanel.TabIndex = 11;
			this.m_barColorPanel.Click += new System.EventHandler(this.m_barColorPanel_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(389, 26);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(34, 13);
			this.label2.TabIndex = 12;
			this.label2.Text = "Color:";
			// 
			// BeatsAndBarsDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(519, 269);
			this.Controls.Add(this.m_goButton);
			this.Controls.Add(this.m_outputGroupBox);
			this.Controls.Add(this.m_paramsGroupBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "BeatsAndBarsDialog";
			this.ShowIcon = false;
			this.Text = "Beats and Bars";
			this.m_paramsGroupBox.ResumeLayout(false);
			this.m_paramsGroupBox.PerformLayout();
			this.m_outputGroupBox.ResumeLayout(false);
			this.m_outputGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_barsSubmarks)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_beatsSubMarks)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private QMLibrary.VampParamCtrl m_vampParamCtrl;
		private System.Windows.Forms.GroupBox m_paramsGroupBox;
		private System.Windows.Forms.GroupBox m_outputGroupBox;
		private System.Windows.Forms.Button m_goButton;
		private System.Windows.Forms.Label m_subMarksLabel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox m_barsNameTB;
		private System.Windows.Forms.TextBox m_beatsNameTB;
		private System.Windows.Forms.CheckBox m_barsCB;
		private System.Windows.Forms.CheckBox m_beatsCB;
		private System.Windows.Forms.NumericUpDown m_beatsSubMarks;
		private System.Windows.Forms.NumericUpDown m_barsSubmarks;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel m_barColorPanel;
		private System.Windows.Forms.Panel m_beatColorPanel;

	}
}