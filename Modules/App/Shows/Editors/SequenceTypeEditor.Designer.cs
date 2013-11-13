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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SequenceTypeEditor));
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxSequence = new System.Windows.Forms.TextBox();
			this.buttonSelectSequence = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.labelSequence = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(1, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(59, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Sequence:";
			// 
			// textBoxSequence
			// 
			this.textBoxSequence.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxSequence.Location = new System.Drawing.Point(66, 3);
			this.textBoxSequence.Name = "textBoxSequence";
			this.textBoxSequence.Size = new System.Drawing.Size(193, 20);
			this.textBoxSequence.TabIndex = 1;
			this.textBoxSequence.TextChanged += new System.EventHandler(this.textBoxSequence_TextChanged);
			// 
			// buttonSelectSequence
			// 
			this.buttonSelectSequence.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSelectSequence.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonSelectSequence.BackgroundImage")));
			this.buttonSelectSequence.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonSelectSequence.Location = new System.Drawing.Point(260, 3);
			this.buttonSelectSequence.Name = "buttonSelectSequence";
			this.buttonSelectSequence.Size = new System.Drawing.Size(20, 20);
			this.buttonSelectSequence.TabIndex = 6;
			this.buttonSelectSequence.UseVisualStyleBackColor = true;
			this.buttonSelectSequence.Click += new System.EventHandler(this.buttonSelectSequence_Click);
			// 
			// openFileDialog
			// 
			this.openFileDialog.FileName = "openFileDialog";
			this.openFileDialog.Multiselect = true;
			// 
			// labelSequence
			// 
			this.labelSequence.AutoSize = true;
			this.labelSequence.Location = new System.Drawing.Point(66, 25);
			this.labelSequence.Name = "labelSequence";
			this.labelSequence.Size = new System.Drawing.Size(87, 13);
			this.labelSequence.TabIndex = 9;
			this.labelSequence.Text = "Sequence Name";
			// 
			// SequenceTypeEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.labelSequence);
			this.Controls.Add(this.buttonSelectSequence);
			this.Controls.Add(this.textBoxSequence);
			this.Controls.Add(this.label1);
			this.Name = "SequenceTypeEditor";
			this.Size = new System.Drawing.Size(285, 79);
			this.Load += new System.EventHandler(this.SequenceTypeEditor_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxSequence;
		private System.Windows.Forms.Button buttonSelectSequence;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.Label labelSequence;
	}
}
