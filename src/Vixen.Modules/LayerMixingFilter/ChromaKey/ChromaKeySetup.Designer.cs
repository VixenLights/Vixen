namespace VixenModules.LayerMixingFilter.ChromaKey
{
	partial class ChromaKeySetup
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
			btnOk = new Button();
			btnCancel = new Button();
			numLowerLimit = new Common.Controls.NumericTextBox();
			numUpperLimit = new Common.Controls.NumericTextBox();
			trkLowerLimit = new TrackBar();
			trkUpperLimit = new TrackBar();
			label1 = new Label();
			label2 = new Label();
			label3 = new Label();
			label4 = new Label();
			colorPanel1 = new Property.Color.ColorPanel();
			label5 = new Label();
			trkHueTolerance = new TrackBar();
			trkSaturationTolerance = new TrackBar();
			label6 = new Label();
			label7 = new Label();
			toolTip = new ToolTip(components);
			chkTransparentOnZeroBrightness = new CheckBox();
			((System.ComponentModel.ISupportInitialize)trkLowerLimit).BeginInit();
			((System.ComponentModel.ISupportInitialize)trkUpperLimit).BeginInit();
			((System.ComponentModel.ISupportInitialize)trkHueTolerance).BeginInit();
			((System.ComponentModel.ISupportInitialize)trkSaturationTolerance).BeginInit();
			SuspendLayout();
			// 
			// btnOk
			// 
			btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			btnOk.DialogResult = DialogResult.OK;
			btnOk.Location = new Point(172, 274);
			btnOk.Name = "btnOk";
			btnOk.Size = new Size(75, 23);
			btnOk.TabIndex = 5;
			btnOk.Text = "OK";
			btnOk.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			btnCancel.DialogResult = DialogResult.Cancel;
			btnCancel.Location = new Point(253, 274);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new Size(75, 23);
			btnCancel.TabIndex = 6;
			btnCancel.Text = "Cancel";
			btnCancel.UseVisualStyleBackColor = true;
			// 
			// numLowerLimit
			// 
			numLowerLimit.DecimalNumber = 0;
			numLowerLimit.Groupsep = ',';
			numLowerLimit.Location = new Point(300, 142);
			numLowerLimit.MaxCheck = true;
			numLowerLimit.MaxValue = 100D;
			numLowerLimit.MinCheck = true;
			numLowerLimit.MinValue = 0D;
			numLowerLimit.Name = "numLowerLimit";
			numLowerLimit.NumberFormat = NumberFormat.UnsignedInteger;
			numLowerLimit.Size = new Size(26, 23);
			numLowerLimit.TabIndex = 2;
			numLowerLimit.Text = "0";
			numLowerLimit.Usegroupseparator = false;
			numLowerLimit.TextChanged += numLowerLimit_TextChanged;
			numLowerLimit.LostFocus += numLowerLimit_LostFocus;
			// 
			// numUpperLimit
			// 
			numUpperLimit.DecimalNumber = 0;
			numUpperLimit.Groupsep = ',';
			numUpperLimit.Location = new Point(300, 184);
			numUpperLimit.MaxCheck = true;
			numUpperLimit.MaxValue = 100D;
			numUpperLimit.MinCheck = true;
			numUpperLimit.MinValue = 0D;
			numUpperLimit.Name = "numUpperLimit";
			numUpperLimit.NumberFormat = NumberFormat.UnsignedInteger;
			numUpperLimit.Size = new Size(26, 23);
			numUpperLimit.TabIndex = 4;
			numUpperLimit.Text = "0";
			numUpperLimit.Usegroupseparator = false;
			numUpperLimit.TextChanged += numUpperLimit_TextChanged;
			numUpperLimit.LostFocus += numUpperLimit_LostFocus;
			// 
			// trkLowerLimit
			// 
			trkLowerLimit.Location = new Point(58, 142);
			trkLowerLimit.Maximum = 99;
			trkLowerLimit.Name = "trkLowerLimit";
			trkLowerLimit.Size = new Size(236, 45);
			trkLowerLimit.TabIndex = 1;
			trkLowerLimit.TickFrequency = 5;
			trkLowerLimit.Scroll += trkLowerLimit_Scroll;
			// 
			// trkUpperLimit
			// 
			trkUpperLimit.Location = new Point(58, 184);
			trkUpperLimit.Maximum = 100;
			trkUpperLimit.Minimum = 1;
			trkUpperLimit.Name = "trkUpperLimit";
			trkUpperLimit.Size = new Size(236, 45);
			trkUpperLimit.TabIndex = 3;
			trkUpperLimit.TickFrequency = 5;
			trkUpperLimit.Value = 100;
			trkUpperLimit.Scroll += trkUpperLimit_Scroll;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(13, 146);
			label1.Name = "label1";
			label1.Size = new Size(39, 15);
			label1.TabIndex = 6;
			label1.Text = "Lower";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(13, 187);
			label2.Name = "label2";
			label2.Size = new Size(39, 15);
			label2.TabIndex = 7;
			label2.Text = "Upper";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new Point(120, 118);
			label3.Name = "label3";
			label3.Size = new Size(98, 15);
			label3.TabIndex = 8;
			label3.Text = "Brightness Range";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new Point(46, 13);
			label4.Name = "label4";
			label4.Size = new Size(36, 15);
			label4.TabIndex = 9;
			label4.Text = "Color";
			// 
			// colorPanel1
			// 
			colorPanel1.BackColor = Color.Black;
			colorPanel1.Color = Color.Black;
			colorPanel1.Location = new Point(31, 36);
			colorPanel1.Margin = new Padding(4);
			colorPanel1.Name = "colorPanel1";
			colorPanel1.Size = new Size(64, 64);
			colorPanel1.TabIndex = 11;
			colorPanel1.ColorChanged += colorPanel1_ColorChanged;
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new Point(192, 13);
			label5.Name = "label5";
			label5.Size = new Size(57, 15);
			label5.TabIndex = 12;
			label5.Text = "Tolerance";
			// 
			// trkHueTolerance
			// 
			trkHueTolerance.Location = new Point(193, 36);
			trkHueTolerance.Maximum = 180;
			trkHueTolerance.Name = "trkHueTolerance";
			trkHueTolerance.Size = new Size(132, 45);
			trkHueTolerance.TabIndex = 13;
			trkHueTolerance.TickFrequency = 20;
			trkHueTolerance.Value = 5;
			trkHueTolerance.Scroll += trkHueTolerance_Scroll;
			// 
			// trkSaturationTolerance
			// 
			trkSaturationTolerance.Location = new Point(193, 76);
			trkSaturationTolerance.Maximum = 99;
			trkSaturationTolerance.Name = "trkSaturationTolerance";
			trkSaturationTolerance.Size = new Size(132, 45);
			trkSaturationTolerance.TabIndex = 14;
			trkSaturationTolerance.TickFrequency = 10;
			trkSaturationTolerance.Value = 5;
			trkSaturationTolerance.Scroll += trkSaturationTolerance_Scroll;
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new Point(157, 36);
			label6.Name = "label6";
			label6.Size = new Size(29, 15);
			label6.TabIndex = 15;
			label6.Text = "Hue";
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new Point(121, 76);
			label7.Name = "label7";
			label7.Size = new Size(61, 15);
			label7.TabIndex = 16;
			label7.Text = "Saturation";
			// 
			// toolTip
			// 
			toolTip.AutomaticDelay = 250;
			// 
			// chkTransparentOnZeroBrightness
			// 
			chkTransparentOnZeroBrightness.AutoSize = true;
			chkTransparentOnZeroBrightness.Location = new Point(16, 232);
			chkTransparentOnZeroBrightness.Name = "chkTransparentOnZeroBrightness";
			chkTransparentOnZeroBrightness.Size = new Size(217, 19);
			chkTransparentOnZeroBrightness.TabIndex = 17;
			chkTransparentOnZeroBrightness.Text = "Treat Zero Brightness as Transparent.";
			chkTransparentOnZeroBrightness.UseVisualStyleBackColor = true;
			chkTransparentOnZeroBrightness.CheckedChanged += chkTransparentOnZeroBrightness_CheckedChanged;
			// 
			// ChromaKeySetup
			// 
			AcceptButton = btnOk;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = true;
			CancelButton = btnCancel;
			ClientSize = new Size(340, 311);
			Controls.Add(chkTransparentOnZeroBrightness);
			Controls.Add(label7);
			Controls.Add(label6);
			Controls.Add(trkSaturationTolerance);
			Controls.Add(trkHueTolerance);
			Controls.Add(label5);
			Controls.Add(colorPanel1);
			Controls.Add(label4);
			Controls.Add(label3);
			Controls.Add(label2);
			Controls.Add(label1);
			Controls.Add(trkUpperLimit);
			Controls.Add(trkLowerLimit);
			Controls.Add(numUpperLimit);
			Controls.Add(numLowerLimit);
			Controls.Add(btnCancel);
			Controls.Add(btnOk);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "ChromaKeySetup";
			StartPosition = FormStartPosition.CenterParent;
			Text = "Chroma Key Configuration";
			((System.ComponentModel.ISupportInitialize)trkLowerLimit).EndInit();
			((System.ComponentModel.ISupportInitialize)trkUpperLimit).EndInit();
			((System.ComponentModel.ISupportInitialize)trkHueTolerance).EndInit();
			((System.ComponentModel.ISupportInitialize)trkSaturationTolerance).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
        private Common.Controls.NumericTextBox numLowerLimit;
        private Common.Controls.NumericTextBox numUpperLimit;
        private System.Windows.Forms.TrackBar trkLowerLimit;
        private System.Windows.Forms.TrackBar trkUpperLimit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private Property.Color.ColorPanel colorPanel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar trkHueTolerance;
        private System.Windows.Forms.TrackBar trkSaturationTolerance;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.CheckBox chkTransparentOnZeroBrightness;
	}
}