namespace VixenApplication.Controls {
	partial class FlatStringTemplateEditor {
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
			this.textBoxFormat = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxIncrement = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxStartLetter = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textBoxFormat
			// 
			this.textBoxFormat.Location = new System.Drawing.Point(112, 116);
			this.textBoxFormat.Name = "textBoxFormat";
			this.textBoxFormat.Size = new System.Drawing.Size(118, 20);
			this.textBoxFormat.TabIndex = 11;
			this.textBoxFormat.Text = "Channel-{A}";
			this.textBoxFormat.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxFormat_Validating);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(29, 119);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(67, 13);
			this.label3.TabIndex = 10;
			this.label3.Text = "Name format";
			// 
			// textBoxIncrement
			// 
			this.textBoxIncrement.Location = new System.Drawing.Point(112, 65);
			this.textBoxIncrement.Name = "textBoxIncrement";
			this.textBoxIncrement.Size = new System.Drawing.Size(56, 20);
			this.textBoxIncrement.TabIndex = 9;
			this.textBoxIncrement.Text = "1";
			this.textBoxIncrement.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxIncrement_Validating);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(29, 68);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(54, 13);
			this.label2.TabIndex = 8;
			this.label2.Text = "Increment";
			// 
			// textBoxStartLetter
			// 
			this.textBoxStartLetter.Location = new System.Drawing.Point(112, 28);
			this.textBoxStartLetter.Name = "textBoxStartLetter";
			this.textBoxStartLetter.Size = new System.Drawing.Size(56, 20);
			this.textBoxStartLetter.TabIndex = 7;
			this.textBoxStartLetter.Text = "A";
			this.textBoxStartLetter.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxStartLetter_Validating);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(29, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(55, 13);
			this.label1.TabIndex = 6;
			this.label1.Text = "Start letter";
			// 
			// FlatStringTemplateEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.textBoxFormat);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textBoxIncrement);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBoxStartLetter);
			this.Controls.Add(this.label1);
			this.Name = "FlatStringTemplateEditor";
			this.Size = new System.Drawing.Size(270, 204);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxFormat;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBoxIncrement;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxStartLetter;
		private System.Windows.Forms.Label label1;
	}
}
