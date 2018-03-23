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
			this.components = new System.ComponentModel.Container();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.numLowerLimit = new Common.Controls.NumericTextBox();
			this.numUpperLimit = new Common.Controls.NumericTextBox();
			this.trkLowerLimit = new System.Windows.Forms.TrackBar();
			this.trkUpperLimit = new System.Windows.Forms.TrackBar();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.colorPanel1 = new VixenModules.Property.Color.ColorPanel();
			this.label5 = new System.Windows.Forms.Label();
			this.trkHueTolerance = new System.Windows.Forms.TrackBar();
			this.trkSaturationTolerance = new System.Windows.Forms.TrackBar();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			((System.ComponentModel.ISupportInitialize)(this.trkLowerLimit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trkUpperLimit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trkHueTolerance)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trkSaturationTolerance)).BeginInit();
			this.SuspendLayout();
			// 
			// btnOk
			// 
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(172, 228);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 5;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.btnOk.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(253, 228);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.btnCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// numLowerLimit
			// 
			this.numLowerLimit.DecimalNumber = 0;
			this.numLowerLimit.Groupsep = ',';
			this.numLowerLimit.Location = new System.Drawing.Point(300, 142);
			this.numLowerLimit.MaxCheck = true;
			this.numLowerLimit.MaxValue = 100D;
			this.numLowerLimit.MinCheck = true;
			this.numLowerLimit.MinValue = 0D;
			this.numLowerLimit.Name = "numLowerLimit";
			this.numLowerLimit.NumberFormat = NumberFormat.UnsignedInteger;
			this.numLowerLimit.Size = new System.Drawing.Size(26, 23);
			this.numLowerLimit.TabIndex = 2;
			this.numLowerLimit.Text = "0";
			this.numLowerLimit.Usegroupseparator = false;
			this.numLowerLimit.TextChanged += new System.EventHandler(this.numLowerLimit_TextChanged);
			this.numLowerLimit.LostFocus += new System.EventHandler(this.numLowerLimit_LostFocus);
			// 
			// numUpperLimit
			// 
			this.numUpperLimit.DecimalNumber = 0;
			this.numUpperLimit.Groupsep = ',';
			this.numUpperLimit.Location = new System.Drawing.Point(300, 184);
			this.numUpperLimit.MaxCheck = true;
			this.numUpperLimit.MaxValue = 100D;
			this.numUpperLimit.MinCheck = true;
			this.numUpperLimit.MinValue = 0D;
			this.numUpperLimit.Name = "numUpperLimit";
			this.numUpperLimit.NumberFormat = NumberFormat.UnsignedInteger;
			this.numUpperLimit.Size = new System.Drawing.Size(26, 23);
			this.numUpperLimit.TabIndex = 4;
			this.numUpperLimit.Text = "0";
			this.numUpperLimit.Usegroupseparator = false;
			this.numUpperLimit.TextChanged += new System.EventHandler(this.numUpperLimit_TextChanged);
			this.numUpperLimit.LostFocus += new System.EventHandler(this.numUpperLimit_LostFocus);
			// 
			// trkLowerLimit
			// 
			this.trkLowerLimit.Location = new System.Drawing.Point(58, 142);
			this.trkLowerLimit.Maximum = 99;
			this.trkLowerLimit.Name = "trkLowerLimit";
			this.trkLowerLimit.Size = new System.Drawing.Size(236, 45);
			this.trkLowerLimit.TabIndex = 1;
			this.trkLowerLimit.TickFrequency = 5;
			this.trkLowerLimit.Scroll += new System.EventHandler(this.trkLowerLimit_Scroll);
			// 
			// trkUpperLimit
			// 
			this.trkUpperLimit.Location = new System.Drawing.Point(58, 184);
			this.trkUpperLimit.Maximum = 100;
			this.trkUpperLimit.Minimum = 1;
			this.trkUpperLimit.Name = "trkUpperLimit";
			this.trkUpperLimit.Size = new System.Drawing.Size(236, 45);
			this.trkUpperLimit.TabIndex = 3;
			this.trkUpperLimit.TickFrequency = 5;
			this.trkUpperLimit.Value = 100;
			this.trkUpperLimit.Scroll += new System.EventHandler(this.trkUpperLimit_Scroll);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 146);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(39, 15);
			this.label1.TabIndex = 6;
			this.label1.Text = "Lower";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 187);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(39, 15);
			this.label2.TabIndex = 7;
			this.label2.Text = "Upper";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(120, 118);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(98, 15);
			this.label3.TabIndex = 8;
			this.label3.Text = "Brightness Range";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(46, 13);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(36, 15);
			this.label4.TabIndex = 9;
			this.label4.Text = "Color";
			// 
			// colorPanel1
			// 
			this.colorPanel1.BackColor = System.Drawing.Color.Black;
			this.colorPanel1.Color = System.Drawing.Color.Black;
			this.colorPanel1.Location = new System.Drawing.Point(31, 36);
			this.colorPanel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.colorPanel1.Name = "colorPanel1";
			this.colorPanel1.Size = new System.Drawing.Size(64, 64);
			this.colorPanel1.TabIndex = 11;
			this.colorPanel1.ColorChanged += new System.EventHandler<VixenModules.Property.Color.ColorPanelEventArgs>(this.colorPanel1_ColorChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(192, 13);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(58, 15);
			this.label5.TabIndex = 12;
			this.label5.Text = "Tolerance";
			// 
			// trkHueTolerance
			// 
			this.trkHueTolerance.Location = new System.Drawing.Point(193, 36);
			this.trkHueTolerance.Maximum = 180;
			this.trkHueTolerance.Name = "trkHueTolerance";
			this.trkHueTolerance.Size = new System.Drawing.Size(132, 45);
			this.trkHueTolerance.TabIndex = 13;
			this.trkHueTolerance.TickFrequency = 20;
			this.trkHueTolerance.Value = 5;
			this.trkHueTolerance.Scroll += new System.EventHandler(this.trkHueTolerance_Scroll);
			// 
			// trkSaturationTolerance
			// 
			this.trkSaturationTolerance.Location = new System.Drawing.Point(193, 76);
			this.trkSaturationTolerance.Maximum = 99;
			this.trkSaturationTolerance.Name = "trkSaturationTolerance";
			this.trkSaturationTolerance.Size = new System.Drawing.Size(132, 45);
			this.trkSaturationTolerance.TabIndex = 14;
			this.trkSaturationTolerance.TickFrequency = 10;
			this.trkSaturationTolerance.Value = 5;
			this.trkSaturationTolerance.Scroll += new System.EventHandler(this.trkSaturationTolerance_Scroll);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(157, 36);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(29, 15);
			this.label6.TabIndex = 15;
			this.label6.Text = "Hue";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(121, 76);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(61, 15);
			this.label7.TabIndex = 16;
			this.label7.Text = "Saturation";
			// 
			// toolTip
			// 
			this.toolTip.AutomaticDelay = 250;
			// 
			// ChromaKeySetup
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(340, 265);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.trkSaturationTolerance);
			this.Controls.Add(this.trkHueTolerance);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.colorPanel1);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.trkUpperLimit);
			this.Controls.Add(this.trkLowerLimit);
			this.Controls.Add(this.numUpperLimit);
			this.Controls.Add(this.numLowerLimit);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.HelpButton = true;
			this.Name = "ChromaKeySetup";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Chroma Key Configuration";
			((System.ComponentModel.ISupportInitialize)(this.trkLowerLimit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trkUpperLimit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trkHueTolerance)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trkSaturationTolerance)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

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
    }
}