namespace VixenApplication.Setup.ElementTemplates
{
	partial class Icicles
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
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.textBoxTreeName = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			this.numericUpDownStrings = new System.Windows.Forms.NumericUpDown();
			this.textBoxStringPattern = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxStringPrefix = new System.Windows.Forms.TextBox();
			this.textBoxPixelPrefix = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStrings)).BeginInit();
			this.SuspendLayout();
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(14, 97);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(123, 15);
			this.label4.TabIndex = 17;
			this.label4.Text = "Pixels / String Pattern:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(14, 61);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(77, 15);
			this.label5.TabIndex = 15;
			this.label5.Text = "String Count:";
			// 
			// textBoxTreeName
			// 
			this.textBoxTreeName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxTreeName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxTreeName.Location = new System.Drawing.Point(152, 22);
			this.textBoxTreeName.Name = "textBoxTreeName";
			this.textBoxTreeName.Size = new System.Drawing.Size(390, 23);
			this.textBoxTreeName.TabIndex = 0;
			this.textBoxTreeName.Text = "Icicles";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(14, 27);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(72, 15);
			this.label6.TabIndex = 13;
			this.label6.Text = "Icicle Name:";
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(437, 169);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(105, 29);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(325, 169);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(105, 29);
			this.buttonOk.TabIndex = 4;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonOk.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOk.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// numericUpDownStrings
			// 
			this.numericUpDownStrings.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.numericUpDownStrings.Location = new System.Drawing.Point(152, 56);
			this.numericUpDownStrings.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownStrings.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownStrings.Name = "numericUpDownStrings";
			this.numericUpDownStrings.Size = new System.Drawing.Size(91, 23);
			this.numericUpDownStrings.TabIndex = 1;
			this.numericUpDownStrings.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
			// 
			// textBoxStringPattern
			// 
			this.textBoxStringPattern.Location = new System.Drawing.Point(152, 94);
			this.textBoxStringPattern.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.textBoxStringPattern.Name = "textBoxStringPattern";
			this.textBoxStringPattern.Size = new System.Drawing.Size(176, 23);
			this.textBoxStringPattern.TabIndex = 18;
			this.textBoxStringPattern.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxStringPattern_KeyPress);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(149, 124);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(82, 15);
			this.label1.TabIndex = 19;
			this.label1.Text = "Example: 7,9,5";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(334, 57);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(74, 15);
			this.label2.TabIndex = 20;
			this.label2.Text = "String Prefix:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(334, 97);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(68, 15);
			this.label3.TabIndex = 21;
			this.label3.Text = "Pixel Prefix:";
			// 
			// textBoxStringPrefix
			// 
			this.textBoxStringPrefix.Location = new System.Drawing.Point(422, 55);
			this.textBoxStringPrefix.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.textBoxStringPrefix.Name = "textBoxStringPrefix";
			this.textBoxStringPrefix.Size = new System.Drawing.Size(120, 23);
			this.textBoxStringPrefix.TabIndex = 22;
			this.textBoxStringPrefix.Text = "S";
			// 
			// textBoxPixelPrefix
			// 
			this.textBoxPixelPrefix.Location = new System.Drawing.Point(422, 94);
			this.textBoxPixelPrefix.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.textBoxPixelPrefix.Name = "textBoxPixelPrefix";
			this.textBoxPixelPrefix.Size = new System.Drawing.Size(120, 23);
			this.textBoxPixelPrefix.TabIndex = 23;
			this.textBoxPixelPrefix.Text = "Px";
			// 
			// Icicles
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(560, 214);
			this.Controls.Add(this.textBoxPixelPrefix);
			this.Controls.Add(this.textBoxStringPrefix);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxStringPattern);
			this.Controls.Add(this.numericUpDownStrings);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textBoxTreeName);
			this.Controls.Add(this.label6);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(576, 253);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(576, 253);
			this.Name = "Icicles";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Icicles Setup";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Icicles_FormClosed);
			this.Load += new System.EventHandler(this.Icicles_Load);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStrings)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBoxTreeName;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.NumericUpDown numericUpDownStrings;
		private System.Windows.Forms.TextBox textBoxStringPattern;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBoxStringPrefix;
		private System.Windows.Forms.TextBox textBoxPixelPrefix;
	}
}