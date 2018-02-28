using VixenModules.Property.Color;

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
            this.chkExcludeZero = new System.Windows.Forms.CheckBox();
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
            ((System.ComponentModel.ISupportInitialize)(this.trkLowerLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkUpperLimit)).BeginInit();
            this.SuspendLayout();
            // 
            // chkExcludeZero
            // 
            this.chkExcludeZero.AutoSize = true;
            this.chkExcludeZero.Location = new System.Drawing.Point(16, 272);
            this.chkExcludeZero.Name = "chkExcludeZero";
            this.chkExcludeZero.Size = new System.Drawing.Size(127, 19);
            this.chkExcludeZero.TabIndex = 0;
            this.chkExcludeZero.TabStop = false;
            this.chkExcludeZero.Text = "Exclude zero values";
            this.chkExcludeZero.UseVisualStyleBackColor = true;
            this.chkExcludeZero.Visible = false;
            this.chkExcludeZero.CheckedChanged += new System.EventHandler(this.chkExcludeZero_CheckedChanged);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(172, 304);
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
            this.btnCancel.Location = new System.Drawing.Point(253, 304);
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
            this.numLowerLimit.AllowSpace = false;
            this.numLowerLimit.Location = new System.Drawing.Point(300, 177);
            this.numLowerLimit.Name = "numLowerLimit";
            this.numLowerLimit.Size = new System.Drawing.Size(26, 23);
            this.numLowerLimit.TabIndex = 2;
            this.numLowerLimit.Text = "0";
            this.numLowerLimit.LostFocus += new System.EventHandler(this.numLowerLimit_LostFocus);
            // 
            // numUpperLimit
            // 
            this.numUpperLimit.AllowSpace = false;
            this.numUpperLimit.Location = new System.Drawing.Point(300, 218);
            this.numUpperLimit.Name = "numUpperLimit";
            this.numUpperLimit.Size = new System.Drawing.Size(26, 23);
            this.numUpperLimit.TabIndex = 4;
            this.numUpperLimit.Text = "100";
            this.numUpperLimit.LostFocus += new System.EventHandler(this.numUpperLimit_LostFocus);
            // 
            // trkLowerLimit
            // 
            this.trkLowerLimit.Location = new System.Drawing.Point(58, 177);
            this.trkLowerLimit.Maximum = 99;
            this.trkLowerLimit.Name = "trkLowerLimit";
            this.trkLowerLimit.Size = new System.Drawing.Size(236, 45);
            this.trkLowerLimit.TabIndex = 1;
            this.trkLowerLimit.TickFrequency = 5;
            this.trkLowerLimit.Scroll += new System.EventHandler(this.trkLowerLimit_Scroll);
            // 
            // trkUpperLimit
            // 
            this.trkUpperLimit.Location = new System.Drawing.Point(58, 218);
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
            this.label1.Location = new System.Drawing.Point(13, 180);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "Lower";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 221);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "Upper";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(120, 147);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "Brightness Range";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(152, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "Color";
            // 
            // colorPanel1
            // 
            this.colorPanel1.BackColor = System.Drawing.Color.Black;
            this.colorPanel1.Color = System.Drawing.Color.Black;
            this.colorPanel1.Location = new System.Drawing.Point(137, 54);
            this.colorPanel1.Name = "colorPanel1";
            this.colorPanel1.Size = new System.Drawing.Size(64, 64);
            this.colorPanel1.TabIndex = 11;
            
            this.colorPanel1.Load += new System.EventHandler(this.colorPanel1_Load);
            this.colorPanel1.ColorChanged += new System.EventHandler<ColorPanelEventArgs>(this.colorPanel1_ColorChanged);      
            // 
            // ChromaKeySetup
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(340, 339);
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
            this.Controls.Add(this.chkExcludeZero);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Name = "ChromaKeySetup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chroma Key Configuration";
            ((System.ComponentModel.ISupportInitialize)(this.trkLowerLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkUpperLimit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox chkExcludeZero;
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
        private System.Windows.Forms.ColorDialog colorDialog1;
        private Property.Color.ColorPanel colorPanel1;
    }
}