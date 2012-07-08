namespace Color {
	partial class ColorSetupForm {
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.radioButtonRed = new System.Windows.Forms.RadioButton();
			this.radioButtonGreen = new System.Windows.Forms.RadioButton();
			this.radioButtonBlue = new System.Windows.Forms.RadioButton();
			this.radioButtonYellow = new System.Windows.Forms.RadioButton();
			this.radioButtonWhite = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.radioButtonWhite);
			this.groupBox1.Controls.Add(this.radioButtonYellow);
			this.groupBox1.Controls.Add(this.radioButtonBlue);
			this.groupBox1.Controls.Add(this.radioButtonGreen);
			this.groupBox1.Controls.Add(this.radioButtonRed);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(180, 209);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Color";
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(36, 227);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(117, 227);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(17, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(148, 32);
			this.label1.TabIndex = 0;
			this.label1.Text = "Which color will this output be restricted to?";
			// 
			// radioButtonRed
			// 
			this.radioButtonRed.AutoSize = true;
			this.radioButtonRed.Location = new System.Drawing.Point(54, 72);
			this.radioButtonRed.Name = "radioButtonRed";
			this.radioButtonRed.Size = new System.Drawing.Size(45, 17);
			this.radioButtonRed.TabIndex = 1;
			this.radioButtonRed.TabStop = true;
			this.radioButtonRed.Text = "Red";
			this.radioButtonRed.UseVisualStyleBackColor = true;
			// 
			// radioButtonGreen
			// 
			this.radioButtonGreen.AutoSize = true;
			this.radioButtonGreen.Location = new System.Drawing.Point(54, 95);
			this.radioButtonGreen.Name = "radioButtonGreen";
			this.radioButtonGreen.Size = new System.Drawing.Size(54, 17);
			this.radioButtonGreen.TabIndex = 2;
			this.radioButtonGreen.TabStop = true;
			this.radioButtonGreen.Text = "Green";
			this.radioButtonGreen.UseVisualStyleBackColor = true;
			// 
			// radioButtonBlue
			// 
			this.radioButtonBlue.AutoSize = true;
			this.radioButtonBlue.Location = new System.Drawing.Point(54, 118);
			this.radioButtonBlue.Name = "radioButtonBlue";
			this.radioButtonBlue.Size = new System.Drawing.Size(46, 17);
			this.radioButtonBlue.TabIndex = 3;
			this.radioButtonBlue.TabStop = true;
			this.radioButtonBlue.Text = "Blue";
			this.radioButtonBlue.UseVisualStyleBackColor = true;
			// 
			// radioButtonYellow
			// 
			this.radioButtonYellow.AutoSize = true;
			this.radioButtonYellow.Location = new System.Drawing.Point(54, 141);
			this.radioButtonYellow.Name = "radioButtonYellow";
			this.radioButtonYellow.Size = new System.Drawing.Size(56, 17);
			this.radioButtonYellow.TabIndex = 4;
			this.radioButtonYellow.TabStop = true;
			this.radioButtonYellow.Text = "Yellow";
			this.radioButtonYellow.UseVisualStyleBackColor = true;
			// 
			// radioButtonWhite
			// 
			this.radioButtonWhite.AutoSize = true;
			this.radioButtonWhite.Location = new System.Drawing.Point(54, 164);
			this.radioButtonWhite.Name = "radioButtonWhite";
			this.radioButtonWhite.Size = new System.Drawing.Size(53, 17);
			this.radioButtonWhite.TabIndex = 5;
			this.radioButtonWhite.TabStop = true;
			this.radioButtonWhite.Text = "White";
			this.radioButtonWhite.UseVisualStyleBackColor = true;
			// 
			// ColorSetupForm
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(204, 262);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ColorSetupForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "RGBYW Setup";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonWhite;
		private System.Windows.Forms.RadioButton radioButtonYellow;
		private System.Windows.Forms.RadioButton radioButtonBlue;
		private System.Windows.Forms.RadioButton radioButtonGreen;
		private System.Windows.Forms.RadioButton radioButtonRed;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
	}
}