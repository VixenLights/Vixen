namespace VixenModules.App.ExportWizard
{
	partial class BulkExportSourcesStage
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
			this.lstSequences = new System.Windows.Forms.ListView();
			this.sequences = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.selectSequencesDialog = new System.Windows.Forms.OpenFileDialog();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnAddAll = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblChooseSequences
			// 
			this.lblChooseSequences.AutoSize = true;
			this.lblChooseSequences.Location = new System.Drawing.Point(11, 12);
			this.lblChooseSequences.Name = "lblChooseSequences";
			this.lblChooseSequences.Size = new System.Drawing.Size(190, 15);
			this.lblChooseSequences.TabIndex = 0;
			this.lblChooseSequences.Text = "Step 2:  Select sequences to export.";
			// 
			// lstSequences
			// 
			this.lstSequences.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.sequences});
			this.lstSequences.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lstSequences.Location = new System.Drawing.Point(14, 77);
			this.lstSequences.Name = "lstSequences";
			this.lstSequences.Size = new System.Drawing.Size(468, 374);
			this.lstSequences.TabIndex = 1;
			this.lstSequences.UseCompatibleStateImageBehavior = false;
			this.lstSequences.View = System.Windows.Forms.View.Details;
			this.lstSequences.SelectedIndexChanged += new System.EventHandler(this.lstSequences_SelectedIndexChanged);
			this.lstSequences.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lstSequences_KeyUp);
			// 
			// sequences
			// 
			this.sequences.Text = "Sequences";
			// 
			// selectSequencesDialog
			// 
			this.selectSequencesDialog.DefaultExt = "tim";
			this.selectSequencesDialog.Filter = "Sequence Files|*.tim";
			this.selectSequencesDialog.Multiselect = true;
			this.selectSequencesDialog.Title = "Select Sequences";
			this.selectSequencesDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(14, 48);
			this.btnAdd.Margin = new System.Windows.Forms.Padding(3, 3, 1, 3);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(28, 23);
			this.btnAdd.TabIndex = 2;
			this.btnAdd.Text = "button1";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnSelectSequences_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.Location = new System.Drawing.Point(76, 48);
			this.btnDelete.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(28, 23);
			this.btnDelete.TabIndex = 3;
			this.btnDelete.Text = "button2";
			this.btnDelete.UseVisualStyleBackColor = true;
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnAddAll
			// 
			this.btnAddAll.Location = new System.Drawing.Point(44, 48);
			this.btnAddAll.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
			this.btnAddAll.Name = "btnAddAll";
			this.btnAddAll.Size = new System.Drawing.Size(28, 23);
			this.btnAddAll.TabIndex = 4;
			this.btnAddAll.Text = "button2";
			this.btnAddAll.UseVisualStyleBackColor = true;
			this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
			// 
			// BulkExportSourcesStage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnAddAll);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.lstSequences);
			this.Controls.Add(this.lblChooseSequences);
			this.Name = "BulkExportSourcesStage";
			this.Size = new System.Drawing.Size(502, 474);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblChooseSequences;
		private System.Windows.Forms.ListView lstSequences;
		private System.Windows.Forms.OpenFileDialog selectSequencesDialog;
		private System.Windows.Forms.ColumnHeader sequences;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Button btnAddAll;
	}
}
