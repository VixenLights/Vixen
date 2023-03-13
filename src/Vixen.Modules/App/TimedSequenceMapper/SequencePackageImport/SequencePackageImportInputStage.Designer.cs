namespace VixenModules.App.TimedSequenceMapper.SequencePackageImport
{
	partial class SequencePackageImportInputStage
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lblChooseOutputFormat = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnEditMap = new System.Windows.Forms.Button();
			this.btnCreateMap = new System.Windows.Forms.Button();
			this.txtProfileMap = new System.Windows.Forms.TextBox();
			this.txtMapFile = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnMapFile = new System.Windows.Forms.Button();
			this.grpSequence = new System.Windows.Forms.GroupBox();
			this.txtPackageFile = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.btnOuputFolderSelect = new System.Windows.Forms.Button();
			this.tableLayoutPanel1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.grpSequence.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblChooseOutputFormat
			// 
			this.lblChooseOutputFormat.AutoSize = true;
			this.lblChooseOutputFormat.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblChooseOutputFormat.Location = new System.Drawing.Point(3, 0);
			this.lblChooseOutputFormat.Name = "lblChooseOutputFormat";
			this.lblChooseOutputFormat.Size = new System.Drawing.Size(504, 25);
			this.lblChooseOutputFormat.TabIndex = 15;
			this.lblChooseOutputFormat.Text = "Step 1:   Select the Sequence Package to import";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.grpSequence, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.lblChooseOutputFormat, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 5;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(510, 510);
			this.tableLayoutPanel1.TabIndex = 24;
			// 
			// groupBox1
			// 
			this.groupBox1.AutoSize = true;
			this.groupBox1.Controls.Add(this.btnEditMap);
			this.groupBox1.Controls.Add(this.btnCreateMap);
			this.groupBox1.Controls.Add(this.txtProfileMap);
			this.groupBox1.Controls.Add(this.txtMapFile);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.btnMapFile);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.groupBox1.Location = new System.Drawing.Point(3, 136);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(504, 170);
			this.groupBox1.TabIndex = 16;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Profile Map Configuration";
			this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// btnEditMap
			// 
			this.btnEditMap.Location = new System.Drawing.Point(106, 125);
			this.btnEditMap.Name = "btnEditMap";
			this.btnEditMap.Size = new System.Drawing.Size(75, 23);
			this.btnEditMap.TabIndex = 27;
			this.btnEditMap.Text = "Edit Map";
			this.btnEditMap.UseVisualStyleBackColor = true;
			this.btnEditMap.Click += new System.EventHandler(this.btnEditMap_Click);
			this.btnEditMap.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.btnEditMap.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// btnCreateMap
			// 
			this.btnCreateMap.Location = new System.Drawing.Point(25, 125);
			this.btnCreateMap.Name = "btnCreateMap";
			this.btnCreateMap.Size = new System.Drawing.Size(75, 23);
			this.btnCreateMap.TabIndex = 26;
			this.btnCreateMap.Text = "Create Map";
			this.btnCreateMap.UseVisualStyleBackColor = true;
			this.btnCreateMap.Click += new System.EventHandler(this.btnCreateMap_Click);
			this.btnCreateMap.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.btnCreateMap.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// txtProfileMap
			// 
			this.txtProfileMap.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtProfileMap.Location = new System.Drawing.Point(25, 23);
			this.txtProfileMap.Multiline = true;
			this.txtProfileMap.Name = "txtProfileMap";
			this.txtProfileMap.ReadOnly = true;
			this.txtProfileMap.Size = new System.Drawing.Size(468, 34);
			this.txtProfileMap.TabIndex = 25;
			this.txtProfileMap.Text = "Select the map file locaton if you have previously mapped this pacakge file. If y" +
    "ou have not previously mapped it, use the Create Map button to create one.";
			// 
			// txtMapFile
			// 
			this.txtMapFile.Location = new System.Drawing.Point(55, 88);
			this.txtMapFile.Name = "txtMapFile";
			this.txtMapFile.ReadOnly = true;
			this.txtMapFile.Size = new System.Drawing.Size(438, 23);
			this.txtMapFile.TabIndex = 23;
			this.txtMapFile.Leave += new System.EventHandler(this.txtMapFile_Leave);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(22, 70);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(89, 15);
			this.label1.TabIndex = 24;
			this.label1.Text = "Profile Map File";
			// 
			// btnMapFile
			// 
			this.btnMapFile.Location = new System.Drawing.Point(25, 88);
			this.btnMapFile.Name = "btnMapFile";
			this.btnMapFile.Size = new System.Drawing.Size(24, 23);
			this.btnMapFile.TabIndex = 22;
			this.btnMapFile.Text = "Map File";
			this.btnMapFile.UseVisualStyleBackColor = true;
			this.btnMapFile.Click += new System.EventHandler(this.btnMapFile_Click);
			// 
			// grpSequence
			// 
			this.grpSequence.AutoSize = true;
			this.grpSequence.Controls.Add(this.txtPackageFile);
			this.grpSequence.Controls.Add(this.label2);
			this.grpSequence.Controls.Add(this.btnOuputFolderSelect);
			this.grpSequence.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grpSequence.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.grpSequence.Location = new System.Drawing.Point(3, 48);
			this.grpSequence.Name = "grpSequence";
			this.grpSequence.Size = new System.Drawing.Size(504, 82);
			this.grpSequence.TabIndex = 15;
			this.grpSequence.TabStop = false;
			this.grpSequence.Text = "Package Location";
			this.grpSequence.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// txtPackageFile
			// 
			this.txtPackageFile.Location = new System.Drawing.Point(55, 37);
			this.txtPackageFile.Name = "txtPackageFile";
			this.txtPackageFile.ReadOnly = true;
			this.txtPackageFile.Size = new System.Drawing.Size(438, 23);
			this.txtPackageFile.TabIndex = 9;
			this.txtPackageFile.Leave += new System.EventHandler(this.txtPackageFile_Leave);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(22, 19);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(126, 15);
			this.label2.TabIndex = 21;
			this.label2.Text = "Sequence Package File";
			// 
			// btnOuputFolderSelect
			// 
			this.btnOuputFolderSelect.Location = new System.Drawing.Point(25, 37);
			this.btnOuputFolderSelect.Name = "btnOuputFolderSelect";
			this.btnOuputFolderSelect.Size = new System.Drawing.Size(24, 23);
			this.btnOuputFolderSelect.TabIndex = 8;
			this.btnOuputFolderSelect.Text = "Sequence Package File";
			this.btnOuputFolderSelect.UseVisualStyleBackColor = true;
			this.btnOuputFolderSelect.Click += new System.EventHandler(this.btnOutputFolderSelect_Click);
			// 
			// SequencePackageImportInputStage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "SequencePackageImportInputStage";
			this.Size = new System.Drawing.Size(510, 510);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.grpSequence.ResumeLayout(false);
			this.grpSequence.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label lblChooseOutputFormat;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox grpSequence;
		private System.Windows.Forms.TextBox txtPackageFile;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnOuputFolderSelect;
		private System.Windows.Forms.TextBox txtMapFile;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnMapFile;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox txtProfileMap;
		private System.Windows.Forms.Button btnCreateMap;
		private System.Windows.Forms.Button btnEditMap;
	}
}
