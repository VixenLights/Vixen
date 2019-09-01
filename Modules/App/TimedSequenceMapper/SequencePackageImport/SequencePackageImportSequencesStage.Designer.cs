namespace VixenModules.App.TimedSequenceMapper.SequencePackageImport
{
	partial class SequencePackageImportSequencesStage
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
			this.lblChooseSequences = new System.Windows.Forms.Label();
			this.selectSequencesDialog = new System.Windows.Forms.OpenFileDialog();
			this.grpSequences = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.lstSequences = new System.Windows.Forms.ListView();
			this.sequenceColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.grpSequences.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblChooseSequences
			// 
			this.lblChooseSequences.AutoSize = true;
			this.lblChooseSequences.Location = new System.Drawing.Point(11, 12);
			this.lblChooseSequences.Name = "lblChooseSequences";
			this.lblChooseSequences.Size = new System.Drawing.Size(193, 15);
			this.lblChooseSequences.TabIndex = 0;
			this.lblChooseSequences.Text = "Step 2:  Select sequences to import.";
			// 
			// selectSequencesDialog
			// 
			this.selectSequencesDialog.DefaultExt = "tim";
			this.selectSequencesDialog.Filter = "Sequence Files|*.tim";
			this.selectSequencesDialog.Multiselect = true;
			this.selectSequencesDialog.Title = "Select Sequences";
			this.selectSequencesDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
			// 
			// grpSequences
			// 
			this.grpSequences.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grpSequences.AutoSize = true;
			this.grpSequences.Controls.Add(this.groupBox2);
			this.grpSequences.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.grpSequences.Location = new System.Drawing.Point(3, 43);
			this.grpSequences.Name = "grpSequences";
			this.grpSequences.Size = new System.Drawing.Size(497, 310);
			this.grpSequences.TabIndex = 17;
			this.grpSequences.TabStop = false;
			this.grpSequences.Text = "Sequences";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.AutoSize = true;
			this.groupBox2.Controls.Add(this.lstSequences);
			this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.groupBox2.Location = new System.Drawing.Point(3, 0);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(491, 310);
			this.groupBox2.TabIndex = 17;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Sequences";
			this.groupBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.GroupBox_Paint);
			// 
			// lstSequences
			// 
			this.lstSequences.CheckBoxes = true;
			this.lstSequences.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.sequenceColumn});
			this.lstSequences.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstSequences.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lstSequences.HideSelection = false;
			this.lstSequences.Location = new System.Drawing.Point(3, 19);
			this.lstSequences.Name = "lstSequences";
			this.lstSequences.Size = new System.Drawing.Size(485, 288);
			this.lstSequences.TabIndex = 0;
			this.lstSequences.UseCompatibleStateImageBehavior = false;
			this.lstSequences.View = System.Windows.Forms.View.Details;
			this.lstSequences.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lstSequences_KeyUp);
			// 
			// sequenceColumn
			// 
			this.sequenceColumn.Text = "Sequence Names";
			// 
			// SequencePackageImportSequencesStage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.grpSequences);
			this.Controls.Add(this.lblChooseSequences);
			this.Name = "SequencePackageImportSequencesStage";
			this.Size = new System.Drawing.Size(511, 474);
			this.grpSequences.ResumeLayout(false);
			this.grpSequences.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblChooseSequences;
		private System.Windows.Forms.OpenFileDialog selectSequencesDialog;
		private System.Windows.Forms.GroupBox grpSequences;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ListView lstSequences;
		private System.Windows.Forms.ColumnHeader sequenceColumn;
	}
}
