namespace VixenModules.LayerMixingFilter.LumaKey
{
	partial class LumaKeySetup
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
            ((System.ComponentModel.ISupportInitialize)(this.trkLowerLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkUpperLimit)).BeginInit();
            this.SuspendLayout();
            // 
            // chkExcludeZero
            // 
            this.chkExcludeZero.AutoSize = true;
            this.chkExcludeZero.Location = new System.Drawing.Point(47, 12);
            this.chkExcludeZero.Name = "chkExcludeZero";
            this.chkExcludeZero.Size = new System.Drawing.Size(127, 19);
            this.chkExcludeZero.TabIndex = 0;
            this.chkExcludeZero.Text = "Exclude zero values";
            this.chkExcludeZero.UseVisualStyleBackColor = true;
            this.chkExcludeZero.CheckedChanged += new System.EventHandler(this.chkExcludeZero_CheckedChanged);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(116, 142);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
            this.btnOk.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(197, 142);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
            this.btnCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
            // 
            // numLowerLimit
            // 
            this.numLowerLimit.AllowSpace = false;
            this.numLowerLimit.Location = new System.Drawing.Point(235, 50);
            this.numLowerLimit.Name = "numLowerLimit";
            this.numLowerLimit.Size = new System.Drawing.Size(37, 23);
            this.numLowerLimit.TabIndex = 3;
            this.numLowerLimit.Text = "0";
            this.numLowerLimit.TextChanged += new System.EventHandler(this.numLowerLimit_TextChanged);
            // 
            // numUpperLimit
            // 
            this.numUpperLimit.AllowSpace = false;
            this.numUpperLimit.Location = new System.Drawing.Point(235, 91);
            this.numUpperLimit.Name = "numUpperLimit";
            this.numUpperLimit.Size = new System.Drawing.Size(37, 23);
            this.numUpperLimit.TabIndex = 3;
            this.numUpperLimit.Text = "100";
            this.numUpperLimit.TextChanged += new System.EventHandler(this.numUpperLimit_TextChanged);
            // 
            // trkLowerLimit
            // 
            this.trkLowerLimit.Location = new System.Drawing.Point(4, 50);
            this.trkLowerLimit.Maximum = 99;
            this.trkLowerLimit.Name = "trkLowerLimit";
            this.trkLowerLimit.Size = new System.Drawing.Size(225, 45);
            this.trkLowerLimit.TabIndex = 4;
            this.trkLowerLimit.TickFrequency = 5;
            this.trkLowerLimit.Scroll += new System.EventHandler(this.trkLowerLimit_Scroll);
            // 
            // trkUpperLimit
            // 
            this.trkUpperLimit.Location = new System.Drawing.Point(4, 91);
            this.trkUpperLimit.Maximum = 100;
            this.trkUpperLimit.Minimum = 1;
            this.trkUpperLimit.Name = "trkUpperLimit";
            this.trkUpperLimit.Size = new System.Drawing.Size(225, 45);
            this.trkUpperLimit.TabIndex = 5;
            this.trkUpperLimit.TickFrequency = 5;
            this.trkUpperLimit.Value = 100;
            this.trkUpperLimit.Scroll += new System.EventHandler(this.trkUpperLimit_Scroll);
            // 
            // LumaKeySetup
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(284, 177);
            this.Controls.Add(this.trkUpperLimit);
            this.Controls.Add(this.trkLowerLimit);
            this.Controls.Add(this.numUpperLimit);
            this.Controls.Add(this.numLowerLimit);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.chkExcludeZero);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Name = "LumaKeySetup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Luma Key Configuration";
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
    }
}