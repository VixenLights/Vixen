namespace VixenModules.App.Shows
{
	partial class SequenceTypeEditor
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
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxSequence = new System.Windows.Forms.TextBox();
			this.buttonSelectSequence = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.labelSequence = new System.Windows.Forms.Label();
			this.labelName = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 30);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(51, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Location:";
			// 
			// textBoxSequence
			// 
			this.textBoxSequence.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxSequence.Location = new System.Drawing.Point(50, 27);
			this.textBoxSequence.Multiline = true;
			this.textBoxSequence.Name = "textBoxSequence";
			this.textBoxSequence.ReadOnly = true;
			this.textBoxSequence.Size = new System.Drawing.Size(260, 20);
			this.textBoxSequence.TabIndex = 1;
			// 
			// buttonSelectSequence
			// 
			this.buttonSelectSequence.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSelectSequence.Location = new System.Drawing.Point(316, 27);
			this.buttonSelectSequence.Name = "buttonSelectSequence";
			this.buttonSelectSequence.Size = new System.Drawing.Size(20, 20);
			this.buttonSelectSequence.TabIndex = 6;
			this.buttonSelectSequence.Text = "S";
			this.buttonSelectSequence.UseVisualStyleBackColor = true;
			this.buttonSelectSequence.Click += new System.EventHandler(this.buttonSelectSequence_Click);
			// 
			// openFileDialog
			// 
			this.openFileDialog.FileName = "openFileDialog";
			this.openFileDialog.Multiselect = true;
			this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenFileDialog_FileOk);
			// 
			// labelSequence
			// 
			this.labelSequence.AutoSize = true;
			this.labelSequence.Location = new System.Drawing.Point(47, 6);
			this.labelSequence.Name = "labelSequence";
			this.labelSequence.Size = new System.Drawing.Size(87, 13);
			this.labelSequence.TabIndex = 9;
			this.labelSequence.Text = "Sequence Name";
			// 
			// labelName
			// 
			this.labelName.AutoSize = true;
			this.labelName.Location = new System.Drawing.Point(3, 6);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(38, 13);
			this.labelName.TabIndex = 10;
			this.labelName.Text = "Name:";
			// 
			// SequenceTypeEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.labelSequence);
			this.Controls.Add(this.buttonSelectSequence);
			this.Controls.Add(this.textBoxSequence);
			this.Controls.Add(this.label1);
			this.Name = "SequenceTypeEditor";
			this.Size = new System.Drawing.Size(352, 79);
			this.Load += new System.EventHandler(this.SequenceTypeEditor_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonSelectSequence;
		private System.Windows.Forms.Label labelSequence;
		private System.Windows.Forms.TextBox textBoxSequence;
		private System.Windows.Forms.Label labelName;
	}
}
