namespace VixenApplication.Setup.ElementTemplates
{
	partial class Megatree
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
			this.numericUpDownPixelsPerString = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.textBoxTreeName = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			this.numericUpDownStrings = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.checkBoxPixelTree = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPixelsPerString)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStrings)).BeginInit();
			this.SuspendLayout();
			// 
			// numericUpDownPixelsPerString
			// 
			this.numericUpDownPixelsPerString.Location = new System.Drawing.Point(114, 126);
			this.numericUpDownPixelsPerString.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownPixelsPerString.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownPixelsPerString.Name = "numericUpDownPixelsPerString";
			this.numericUpDownPixelsPerString.Size = new System.Drawing.Size(91, 23);
			this.numericUpDownPixelsPerString.TabIndex = 3;
			this.numericUpDownPixelsPerString.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(14, 128);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(81, 15);
			this.label4.TabIndex = 17;
			this.label4.Text = "Pixels / String:";
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
			this.textBoxTreeName.Location = new System.Drawing.Point(114, 23);
			this.textBoxTreeName.Name = "textBoxTreeName";
			this.textBoxTreeName.Size = new System.Drawing.Size(215, 23);
			this.textBoxTreeName.TabIndex = 0;
			this.textBoxTreeName.Text = "Megatree";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(14, 27);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(68, 15);
			this.label6.TabIndex = 13;
			this.label6.Text = "Tree Name:";
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(225, 173);
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
			this.buttonOk.Location = new System.Drawing.Point(113, 173);
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
			this.numericUpDownStrings.Location = new System.Drawing.Point(114, 59);
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
            16,
            0,
            0,
            0});
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(14, 95);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 15);
			this.label1.TabIndex = 22;
			this.label1.Text = "Pixel Tree:";
			// 
			// checkBoxPixelTree
			// 
			this.checkBoxPixelTree.AutoSize = true;
			this.checkBoxPixelTree.Checked = true;
			this.checkBoxPixelTree.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxPixelTree.Location = new System.Drawing.Point(114, 95);
			this.checkBoxPixelTree.Name = "checkBoxPixelTree";
			this.checkBoxPixelTree.Size = new System.Drawing.Size(15, 14);
			this.checkBoxPixelTree.TabIndex = 2;
			this.checkBoxPixelTree.UseVisualStyleBackColor = true;
			this.checkBoxPixelTree.CheckedChanged += new System.EventHandler(this.checkBoxPixelTree_CheckedChanged);
			// 
			// Megatree
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(344, 217);
			this.Controls.Add(this.checkBoxPixelTree);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.numericUpDownStrings);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.numericUpDownPixelsPerString);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textBoxTreeName);
			this.Controls.Add(this.label6);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(360, 255);
			this.Name = "Megatree";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Megatree Setup";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Megatree_FormClosed);
			this.Load += new System.EventHandler(this.Megatree_Load);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPixelsPerString)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStrings)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NumericUpDown numericUpDownPixelsPerString;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBoxTreeName;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.NumericUpDown numericUpDownStrings;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkBoxPixelTree;
	}
}