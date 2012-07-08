namespace VixenApplication.Controls {
	partial class FlatNumericTemplateEditor {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxStartNumber = new System.Windows.Forms.TextBox();
			this.textBoxIncrement = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxFormat = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(24, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(67, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Start number";
			// 
			// textBoxStartNumber
			// 
			this.textBoxStartNumber.Location = new System.Drawing.Point(107, 19);
			this.textBoxStartNumber.Name = "textBoxStartNumber";
			this.textBoxStartNumber.Size = new System.Drawing.Size(56, 20);
			this.textBoxStartNumber.TabIndex = 1;
			this.textBoxStartNumber.Text = "1";
			this.textBoxStartNumber.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxStartNumber_Validating);
			// 
			// textBoxIncrement
			// 
			this.textBoxIncrement.Location = new System.Drawing.Point(107, 56);
			this.textBoxIncrement.Name = "textBoxIncrement";
			this.textBoxIncrement.Size = new System.Drawing.Size(56, 20);
			this.textBoxIncrement.TabIndex = 3;
			this.textBoxIncrement.Text = "1";
			this.textBoxIncrement.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxIncrement_Validating);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(24, 59);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(54, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Increment";
			// 
			// textBoxFormat
			// 
			this.textBoxFormat.Location = new System.Drawing.Point(107, 107);
			this.textBoxFormat.Name = "textBoxFormat";
			this.textBoxFormat.Size = new System.Drawing.Size(118, 20);
			this.textBoxFormat.TabIndex = 5;
			this.textBoxFormat.Text = "Channel-{#}";
			this.textBoxFormat.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxFormat_Validating);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(24, 110);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(67, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Name format";
			// 
			// FlatNumericTemplateEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.textBoxFormat);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textBoxIncrement);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBoxStartNumber);
			this.Controls.Add(this.label1);
			this.Name = "FlatNumericTemplateEditor";
			this.Size = new System.Drawing.Size(295, 172);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxStartNumber;
		private System.Windows.Forms.TextBox textBoxIncrement;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxFormat;
		private System.Windows.Forms.Label label3;
	}
}
