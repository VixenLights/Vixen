namespace VixenModules.App.Shows
{
	partial class PauseTypeEditor
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
			this.components = new System.ComponentModel.Container();
			this.label1 = new System.Windows.Forms.Label();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.numericUpDownPauseSeconds = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPauseSeconds)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(1, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(89, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Puase (seconds):";
			// 
			// openFileDialog
			// 
			this.openFileDialog.FileName = "openFileDialog";
			this.openFileDialog.Multiselect = true;
			// 
			// numericUpDownPauseSeconds
			// 
			this.numericUpDownPauseSeconds.Location = new System.Drawing.Point(96, 4);
			this.numericUpDownPauseSeconds.Name = "numericUpDownPauseSeconds";
			this.numericUpDownPauseSeconds.Size = new System.Drawing.Size(47, 20);
			this.numericUpDownPauseSeconds.TabIndex = 1;
			this.numericUpDownPauseSeconds.ValueChanged += new System.EventHandler(this.numericUpDownPauseSeconds_ValueChanged);
			// 
			// PauseTypeEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.numericUpDownPauseSeconds);
			this.Controls.Add(this.label1);
			this.Name = "PauseTypeEditor";
			this.Size = new System.Drawing.Size(285, 79);
			this.Load += new System.EventHandler(this.SequenceTypeEditor_Load);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPauseSeconds)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.NumericUpDown numericUpDownPauseSeconds;
	}
}
