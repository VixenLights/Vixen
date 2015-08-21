namespace VixenApplication.Setup.ElementTemplates
{
	partial class PixelGrid
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
			this.numericUpDownRows = new System.Windows.Forms.NumericUpDown();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			this.numericUpDownColumns = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.radioButtonRowsFirst = new System.Windows.Forms.RadioButton();
			this.radioButtonColumnsFirst = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownRows)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownColumns)).BeginInit();
			this.SuspendLayout();
			// 
			// numericUpDownRows
			// 
			this.numericUpDownRows.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.numericUpDownRows.Location = new System.Drawing.Point(98, 48);
			this.numericUpDownRows.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownRows.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownRows.Name = "numericUpDownRows";
			this.numericUpDownRows.Size = new System.Drawing.Size(78, 20);
			this.numericUpDownRows.TabIndex = 1;
			this.numericUpDownRows.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(194, 171);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(90, 25);
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(98, 171);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(90, 25);
			this.buttonOk.TabIndex = 5;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonOk.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOk.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// numericUpDownColumns
			// 
			this.numericUpDownColumns.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.numericUpDownColumns.Location = new System.Drawing.Point(98, 78);
			this.numericUpDownColumns.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownColumns.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownColumns.Name = "numericUpDownColumns";
			this.numericUpDownColumns.Size = new System.Drawing.Size(78, 20);
			this.numericUpDownColumns.TabIndex = 2;
			this.numericUpDownColumns.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 80);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(50, 13);
			this.label4.TabIndex = 27;
			this.label4.Text = "Columns:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 50);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(37, 13);
			this.label5.TabIndex = 26;
			this.label5.Text = "Rows:";
			// 
			// textBoxName
			// 
			this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxName.Location = new System.Drawing.Point(98, 17);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(186, 20);
			this.textBoxName.TabIndex = 0;
			this.textBoxName.Text = "Grid";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(12, 20);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(60, 13);
			this.label6.TabIndex = 24;
			this.label6.Text = "Grid Name:";
			// 
			// radioButtonRowsFirst
			// 
			this.radioButtonRowsFirst.AutoSize = true;
			this.radioButtonRowsFirst.Location = new System.Drawing.Point(15, 111);
			this.radioButtonRowsFirst.Name = "radioButtonRowsFirst";
			this.radioButtonRowsFirst.Size = new System.Drawing.Size(163, 17);
			this.radioButtonRowsFirst.TabIndex = 3;
			this.radioButtonRowsFirst.TabStop = true;
			this.radioButtonRowsFirst.Text = "Generate rows, then columns";
			this.radioButtonRowsFirst.UseVisualStyleBackColor = true;
			// 
			// radioButtonColumnsFirst
			// 
			this.radioButtonColumnsFirst.AutoSize = true;
			this.radioButtonColumnsFirst.Location = new System.Drawing.Point(15, 134);
			this.radioButtonColumnsFirst.Name = "radioButtonColumnsFirst";
			this.radioButtonColumnsFirst.Size = new System.Drawing.Size(163, 17);
			this.radioButtonColumnsFirst.TabIndex = 4;
			this.radioButtonColumnsFirst.TabStop = true;
			this.radioButtonColumnsFirst.Text = "Generate columns, then rows";
			this.radioButtonColumnsFirst.UseVisualStyleBackColor = true;
			// 
			// PixelGrid
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(296, 208);
			this.Controls.Add(this.radioButtonColumnsFirst);
			this.Controls.Add(this.radioButtonRowsFirst);
			this.Controls.Add(this.numericUpDownRows);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.numericUpDownColumns);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textBoxName);
			this.Controls.Add(this.label6);
			this.DoubleBuffered = true;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(312, 247);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(312, 247);
			this.Name = "PixelGrid";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Pixel Grid Setup";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PixelGrid_FormClosed);
			this.Load += new System.EventHandler(this.PixelGrid_Load);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownRows)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownColumns)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NumericUpDown numericUpDownRows;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.NumericUpDown numericUpDownColumns;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBoxName;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.RadioButton radioButtonRowsFirst;
		private System.Windows.Forms.RadioButton radioButtonColumnsFirst;
	}
}