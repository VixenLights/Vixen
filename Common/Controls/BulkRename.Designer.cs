namespace CommonElements
{
	partial class BulkRename
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BulkRename));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioButtonNumbersFirst = new System.Windows.Forms.RadioButton();
			this.label5 = new System.Windows.Forms.Label();
			this.numericUpDownStartNumber = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxLetters = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxPattern = new System.Windows.Forms.TextBox();
			this.radioButtonLettersFirst = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.listViewNames = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartNumber)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.radioButtonNumbersFirst);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.numericUpDownStartNumber);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.textBoxLetters);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.textBoxPattern);
			this.groupBox1.Controls.Add(this.radioButtonLettersFirst);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(12, 158);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(460, 261);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Renaming Options";
			// 
			// radioButtonNumbersFirst
			// 
			this.radioButtonNumbersFirst.AutoSize = true;
			this.radioButtonNumbersFirst.Location = new System.Drawing.Point(25, 227);
			this.radioButtonNumbersFirst.Name = "radioButtonNumbersFirst";
			this.radioButtonNumbersFirst.Size = new System.Drawing.Size(306, 17);
			this.radioButtonNumbersFirst.TabIndex = 9;
			this.radioButtonNumbersFirst.TabStop = true;
			this.radioButtonNumbersFirst.Text = "Iterate through numbers first, then iterate through the letters.";
			this.radioButtonNumbersFirst.UseVisualStyleBackColor = true;
			this.radioButtonNumbersFirst.CheckedChanged += new System.EventHandler(this.radioButtonNumbersFirst_CheckedChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(62, 169);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(55, 13);
			this.label5.TabIndex = 8;
			this.label5.Text = "Start from:";
			// 
			// numericUpDownStartNumber
			// 
			this.numericUpDownStartNumber.Location = new System.Drawing.Point(123, 167);
			this.numericUpDownStartNumber.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownStartNumber.Name = "numericUpDownStartNumber";
			this.numericUpDownStartNumber.Size = new System.Drawing.Size(51, 20);
			this.numericUpDownStartNumber.TabIndex = 7;
			this.numericUpDownStartNumber.ValueChanged += new System.EventHandler(this.numericUpDownStartNumber_ValueChanged);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(14, 77);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(434, 24);
			this.label4.TabIndex = 6;
			this.label4.Text = "An option is provided to determine if the letters will be iterated through first," +
				" or the numbers.";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(14, 26);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(432, 51);
			this.label3.TabIndex = 5;
			this.label3.Text = resources.GetString("label3.Text");
			// 
			// textBoxLetters
			// 
			this.textBoxLetters.Location = new System.Drawing.Point(123, 141);
			this.textBoxLetters.Name = "textBoxLetters";
			this.textBoxLetters.Size = new System.Drawing.Size(74, 20);
			this.textBoxLetters.TabIndex = 4;
			this.textBoxLetters.TextChanged += new System.EventHandler(this.textBoxLetters_TextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(17, 144);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Letter Substitutions:";
			// 
			// textBoxPattern
			// 
			this.textBoxPattern.Location = new System.Drawing.Point(123, 115);
			this.textBoxPattern.Name = "textBoxPattern";
			this.textBoxPattern.Size = new System.Drawing.Size(176, 20);
			this.textBoxPattern.TabIndex = 2;
			this.textBoxPattern.TextChanged += new System.EventHandler(this.textBoxPattern_TextChanged);
			// 
			// radioButtonLettersFirst
			// 
			this.radioButtonLettersFirst.AutoSize = true;
			this.radioButtonLettersFirst.Location = new System.Drawing.Point(25, 204);
			this.radioButtonLettersFirst.Name = "radioButtonLettersFirst";
			this.radioButtonLettersFirst.Size = new System.Drawing.Size(279, 17);
			this.radioButtonLettersFirst.TabIndex = 1;
			this.radioButtonLettersFirst.TabStop = true;
			this.radioButtonLettersFirst.Text = "Iterate through letters first, then increment the number.";
			this.radioButtonLettersFirst.UseVisualStyleBackColor = true;
			this.radioButtonLettersFirst.CheckedChanged += new System.EventHandler(this.radioButtonLettersFirst_CheckedChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(30, 118);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(87, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Rename Pattern:";
			// 
			// listViewNames
			// 
			this.listViewNames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listViewNames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.listViewNames.FullRowSelect = true;
			this.listViewNames.GridLines = true;
			this.listViewNames.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewNames.HideSelection = false;
			this.listViewNames.Location = new System.Drawing.Point(12, 12);
			this.listViewNames.MultiSelect = false;
			this.listViewNames.Name = "listViewNames";
			this.listViewNames.ShowGroups = false;
			this.listViewNames.Size = new System.Drawing.Size(460, 140);
			this.listViewNames.TabIndex = 1;
			this.listViewNames.UseCompatibleStateImageBehavior = false;
			this.listViewNames.View = System.Windows.Forms.View.Details;
			this.listViewNames.Resize += new System.EventHandler(this.listViewNames_Resize);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Old Name";
			this.columnHeader1.Width = 158;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "New Name";
			this.columnHeader2.Width = 209;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(382, 425);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(90, 25);
			this.buttonCancel.TabIndex = 28;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(286, 425);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(90, 25);
			this.buttonOk.TabIndex = 27;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// BulkRename
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(484, 462);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.listViewNames);
			this.Controls.Add(this.groupBox1);
			this.DoubleBuffered = true;
			this.MinimumSize = new System.Drawing.Size(500, 500);
			this.Name = "BulkRename";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Bulk Rename";
			this.Load += new System.EventHandler(this.BulkRename_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartNumber)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ListView listViewNames;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.RadioButton radioButtonNumbersFirst;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown numericUpDownStartNumber;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBoxLetters;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxPattern;
		private System.Windows.Forms.RadioButton radioButtonLettersFirst;
		private System.Windows.Forms.Label label1;
	}
}