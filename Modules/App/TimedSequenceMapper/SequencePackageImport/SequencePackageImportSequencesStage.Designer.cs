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
			this.lstSequences = new System.Windows.Forms.ListView();
			this.sequenceColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
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
			// lstSequences
			// 
			this.lstSequences.CheckBoxes = true;
			this.lstSequences.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.sequenceColumn});
			this.lstSequences.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lstSequences.HideSelection = false;
			this.lstSequences.Location = new System.Drawing.Point(14, 47);
			this.lstSequences.Name = "lstSequences";
			this.lstSequences.Size = new System.Drawing.Size(485, 318);
			this.lstSequences.TabIndex = 1;
			this.lstSequences.UseCompatibleStateImageBehavior = false;
			this.lstSequences.View = System.Windows.Forms.View.Details;
			this.lstSequences.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lstSequences_ItemChecked);
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
			this.Controls.Add(this.lstSequences);
			this.Controls.Add(this.lblChooseSequences);
			this.Name = "SequencePackageImportSequencesStage";
			this.Size = new System.Drawing.Size(511, 474);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblChooseSequences;
		private System.Windows.Forms.ListView lstSequences;
		private System.Windows.Forms.ColumnHeader sequenceColumn;
	}
}
