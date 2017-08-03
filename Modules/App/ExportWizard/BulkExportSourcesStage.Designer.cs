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
			this.panel1 = new System.Windows.Forms.Panel();
			this.toolStripSequences = new System.Windows.Forms.ToolStrip();
			this.btnAdd = new System.Windows.Forms.ToolStripButton();
			this.btnDelete = new System.Windows.Forms.ToolStripButton();
			this.panel1.SuspendLayout();
			this.toolStripSequences.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblChooseSequences
			// 
			this.lblChooseSequences.AutoSize = true;
			this.lblChooseSequences.Location = new System.Drawing.Point(11, 12);
			this.lblChooseSequences.Name = "lblChooseSequences";
			this.lblChooseSequences.Size = new System.Drawing.Size(190, 15);
			this.lblChooseSequences.TabIndex = 0;
			this.lblChooseSequences.Text = "Step 1:  Select sequences to export.";
			// 
			// lstSequences
			// 
			this.lstSequences.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.sequences});
			this.lstSequences.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lstSequences.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lstSequences.Location = new System.Drawing.Point(0, 28);
			this.lstSequences.Name = "lstSequences";
			this.lstSequences.Size = new System.Drawing.Size(340, 374);
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
			// panel1
			// 
			this.panel1.Controls.Add(this.toolStripSequences);
			this.panel1.Controls.Add(this.lstSequences);
			this.panel1.Location = new System.Drawing.Point(3, 49);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(340, 402);
			this.panel1.TabIndex = 2;
			// 
			// toolStripSequences
			// 
			this.toolStripSequences.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.toolStripSequences.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStripSequences.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStripSequences.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAdd,
            this.btnDelete});
			this.toolStripSequences.Location = new System.Drawing.Point(0, 0);
			this.toolStripSequences.Name = "toolStripSequences";
			this.toolStripSequences.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
			this.toolStripSequences.Size = new System.Drawing.Size(340, 25);
			this.toolStripSequences.TabIndex = 4;
			this.toolStripSequences.Text = "Colors";
			// 
			// btnAdd
			// 
			this.btnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(33, 22);
			this.btnAdd.Text = "Add";
			this.btnAdd.ToolTipText = "Add Sequences";
			this.btnAdd.Click += new System.EventHandler(this.btnSelectSequences_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnDelete.Enabled = false;
			this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(44, 22);
			this.btnDelete.Text = "Delete";
			this.btnDelete.ToolTipText = "Delete Selected";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// BulkExportSourcesStage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.lblChooseSequences);
			this.Name = "BulkExportSourcesStage";
			this.Size = new System.Drawing.Size(364, 474);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.toolStripSequences.ResumeLayout(false);
			this.toolStripSequences.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblChooseSequences;
		private System.Windows.Forms.ListView lstSequences;
		private System.Windows.Forms.OpenFileDialog selectSequencesDialog;
		private System.Windows.Forms.ColumnHeader sequences;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ToolStrip toolStripSequences;
		private System.Windows.Forms.ToolStripButton btnAdd;
		private System.Windows.Forms.ToolStripButton btnDelete;
	}
}
