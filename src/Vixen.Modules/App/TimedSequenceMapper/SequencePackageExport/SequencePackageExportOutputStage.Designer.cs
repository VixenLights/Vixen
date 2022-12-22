namespace VixenModules.App.TimedSequenceMapper.SequencePackageExport
{
	partial class SequencePackageExportOutputStage
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
			this.grpSequence = new System.Windows.Forms.GroupBox();
			this.txtOutputFolder = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.chkIncludeAudio = new System.Windows.Forms.CheckBox();
			this.btnOuputFolderSelect = new System.Windows.Forms.Button();
			this.tableLayoutPanel1.SuspendLayout();
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
			this.lblChooseOutputFormat.Text = "Step 2:   Configure the Output Folder";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.grpSequence, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.lblChooseOutputFormat, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(510, 510);
			this.tableLayoutPanel1.TabIndex = 24;
			// 
			// grpSequence
			// 
			this.grpSequence.AutoSize = true;
			this.grpSequence.Controls.Add(this.txtOutputFolder);
			this.grpSequence.Controls.Add(this.label2);
			this.grpSequence.Controls.Add(this.chkIncludeAudio);
			this.grpSequence.Controls.Add(this.btnOuputFolderSelect);
			this.grpSequence.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.grpSequence.Location = new System.Drawing.Point(3, 48);
			this.grpSequence.Name = "grpSequence";
			this.grpSequence.Size = new System.Drawing.Size(499, 117);
			this.grpSequence.TabIndex = 15;
			this.grpSequence.TabStop = false;
			this.grpSequence.Text = "Export Configuration";
			this.grpSequence.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// txtOutputFolder
			// 
			this.txtOutputFolder.Location = new System.Drawing.Point(55, 37);
			this.txtOutputFolder.Name = "txtOutputFolder";
			this.txtOutputFolder.ReadOnly = true;
			this.txtOutputFolder.Size = new System.Drawing.Size(438, 23);
			this.txtOutputFolder.TabIndex = 9;
			this.txtOutputFolder.Leave += new System.EventHandler(this.txtOutputFolder_Leave);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(22, 19);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(144, 15);
			this.label2.TabIndex = 21;
			this.label2.Text = "Sequence Export Location";
			// 
			// chkIncludeAudio
			// 
			this.chkIncludeAudio.AutoSize = true;
			this.chkIncludeAudio.Location = new System.Drawing.Point(25, 76);
			this.chkIncludeAudio.Name = "chkIncludeAudio";
			this.chkIncludeAudio.Size = new System.Drawing.Size(100, 19);
			this.chkIncludeAudio.TabIndex = 5;
			this.chkIncludeAudio.Text = "Include Audio";
			this.chkIncludeAudio.UseVisualStyleBackColor = true;
			this.chkIncludeAudio.CheckedChanged += new System.EventHandler(this.chkIncludeAudio_CheckedChanged);
			// 
			// btnOuputFolderSelect
			// 
			this.btnOuputFolderSelect.Location = new System.Drawing.Point(25, 37);
			this.btnOuputFolderSelect.Name = "btnOuputFolderSelect";
			this.btnOuputFolderSelect.Size = new System.Drawing.Size(24, 23);
			this.btnOuputFolderSelect.TabIndex = 8;
			this.btnOuputFolderSelect.Text = "Output Folder";
			this.btnOuputFolderSelect.UseVisualStyleBackColor = true;
			this.btnOuputFolderSelect.Click += new System.EventHandler(this.btnOutputFolderSelect_Click);
			// 
			// SequencePackageExportOutputStage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "SequencePackageExportOutputStage";
			this.Size = new System.Drawing.Size(510, 510);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.grpSequence.ResumeLayout(false);
			this.grpSequence.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label lblChooseOutputFormat;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox grpSequence;
		private System.Windows.Forms.TextBox txtOutputFolder;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox chkIncludeAudio;
		private System.Windows.Forms.Button btnOuputFolderSelect;
	}
}
