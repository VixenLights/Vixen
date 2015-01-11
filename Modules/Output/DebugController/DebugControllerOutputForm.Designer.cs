namespace VixenModules.Output.DebugController
{
	partial class DebugControllerOutputForm
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
			if (disposing && (components != null)) {
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
			this.textBoxOutput = new System.Windows.Forms.TextBox();
			this.chkVerbose = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// textBoxOutput
			// 
			this.textBoxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxOutput.Location = new System.Drawing.Point(12, 12);
			this.textBoxOutput.Multiline = true;
			this.textBoxOutput.Name = "textBoxOutput";
			this.textBoxOutput.Size = new System.Drawing.Size(514, 331);
			this.textBoxOutput.TabIndex = 0;
			// 
			// chkVerbose
			// 
			this.chkVerbose.AutoSize = true;
			this.chkVerbose.Location = new System.Drawing.Point(395, 366);
			this.chkVerbose.Name = "chkVerbose";
			this.chkVerbose.Size = new System.Drawing.Size(65, 17);
			this.chkVerbose.TabIndex = 1;
			this.chkVerbose.Text = "Verbose";
			this.chkVerbose.UseVisualStyleBackColor = true;
			this.chkVerbose.CheckedChanged += new System.EventHandler(this.chkVerbose_CheckedChanged);
			// 
			// DebugControllerOutputForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(538, 405);
			this.Controls.Add(this.chkVerbose);
			this.Controls.Add(this.textBoxOutput);
			this.DoubleBuffered = true;
			this.Name = "DebugControllerOutputForm";
			this.Text = "Debug Controller Output";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxOutput;
		private System.Windows.Forms.CheckBox chkVerbose;
	}
}