namespace Common.Controls
{
	partial class NumberDialog
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
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.labelPrompt = new System.Windows.Forms.Label();
			this.numericUpDownChooser = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownChooser)).BeginInit();
			this.SuspendLayout();
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button2.Location = new System.Drawing.Point(222, 71);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(80, 25);
			this.button2.TabIndex = 7;
			this.button2.Text = "Cancel";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(136, 71);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(80, 25);
			this.button1.TabIndex = 6;
			this.button1.Text = "OK";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// labelPrompt
			// 
			this.labelPrompt.AutoSize = true;
			this.labelPrompt.Location = new System.Drawing.Point(12, 27);
			this.labelPrompt.Name = "labelPrompt";
			this.labelPrompt.Size = new System.Drawing.Size(13, 13);
			this.labelPrompt.TabIndex = 4;
			this.labelPrompt.Text = "[]";
			// 
			// numericUpDownChooser
			// 
			this.numericUpDownChooser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.numericUpDownChooser.Location = new System.Drawing.Point(218, 25);
			this.numericUpDownChooser.Name = "numericUpDownChooser";
			this.numericUpDownChooser.Size = new System.Drawing.Size(84, 20);
			this.numericUpDownChooser.TabIndex = 8;
			// 
			// NumberDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(314, 108);
			this.Controls.Add(this.numericUpDownChooser);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.labelPrompt);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NumberDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownChooser)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label labelPrompt;
		private System.Windows.Forms.NumericUpDown numericUpDownChooser;
	}
}