namespace VixenModules.Property.Location {
	partial class SetupForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
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
		private void InitializeComponent() {
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.numericUpDownXPosition = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.numericUpDownYPosition = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.numericUpDownZPosition = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownXPosition)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownYPosition)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownZPosition)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(111, 178);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(87, 27);
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(16, 178);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(87, 27);
			this.buttonOK.TabIndex = 5;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOK.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// numericUpDownXPosition
			// 
			this.numericUpDownXPosition.Location = new System.Drawing.Point(106, 53);
			this.numericUpDownXPosition.Maximum = new decimal(new int[] {
            276447231,
            23283,
            0,
            0});
			this.numericUpDownXPosition.Name = "numericUpDownXPosition";
			this.numericUpDownXPosition.Size = new System.Drawing.Size(83, 23);
			this.numericUpDownXPosition.TabIndex = 7;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(15, 55);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 15);
			this.label1.TabIndex = 8;
			this.label1.Text = "X Position";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(15, 90);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(60, 15);
			this.label2.TabIndex = 10;
			this.label2.Text = "Y Position";
			// 
			// numericUpDownYPosition
			// 
			this.numericUpDownYPosition.Location = new System.Drawing.Point(106, 88);
			this.numericUpDownYPosition.Maximum = new decimal(new int[] {
            276447231,
            23283,
            0,
            0});
			this.numericUpDownYPosition.Name = "numericUpDownYPosition";
			this.numericUpDownYPosition.Size = new System.Drawing.Size(83, 23);
			this.numericUpDownYPosition.TabIndex = 9;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(15, 125);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(60, 15);
			this.label3.TabIndex = 12;
			this.label3.Text = "Z Position";
			// 
			// numericUpDownZPosition
			// 
			this.numericUpDownZPosition.Location = new System.Drawing.Point(106, 122);
			this.numericUpDownZPosition.Maximum = new decimal(new int[] {
            276447231,
            23283,
            0,
            0});
			this.numericUpDownZPosition.Name = "numericUpDownZPosition";
			this.numericUpDownZPosition.Size = new System.Drawing.Size(83, 23);
			this.numericUpDownZPosition.TabIndex = 11;
			// 
			// SetupForm
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(212, 218);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.numericUpDownZPosition);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.numericUpDownYPosition);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.numericUpDownXPosition);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SetupForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Location Setup";
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownXPosition)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownYPosition)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownZPosition)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.NumericUpDown numericUpDownXPosition;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericUpDownYPosition;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown numericUpDownZPosition;
	}
}