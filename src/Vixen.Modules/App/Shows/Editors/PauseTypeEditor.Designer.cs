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
			components = new System.ComponentModel.Container();
			label1 = new Label();
			openFileDialog = new OpenFileDialog();
			toolTip1 = new ToolTip(components);
			numericUpDownPauseSeconds = new NumericUpDown();
			((System.ComponentModel.ISupportInitialize)numericUpDownPauseSeconds).BeginInit();
			SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(1, 7);
			label1.Margin = new Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new Size(95, 15);
			label1.TabIndex = 0;
			label1.Text = "Pause (seconds):";
			// 
			// openFileDialog
			// 
			openFileDialog.FileName = "openFileDialog";
			openFileDialog.Multiselect = true;
			// 
			// numericUpDownPauseSeconds
			// 
			numericUpDownPauseSeconds.Location = new Point(112, 5);
			numericUpDownPauseSeconds.Margin = new Padding(4, 3, 4, 3);
			numericUpDownPauseSeconds.Maximum = new decimal(new int[] { 900, 0, 0, 0 });
			numericUpDownPauseSeconds.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
			numericUpDownPauseSeconds.Name = "numericUpDownPauseSeconds";
			numericUpDownPauseSeconds.Size = new Size(55, 23);
			numericUpDownPauseSeconds.TabIndex = 1;
			numericUpDownPauseSeconds.Value = new decimal(new int[] { 1, 0, 0, 0 });
			numericUpDownPauseSeconds.ValueChanged += numericUpDownPauseSeconds_ValueChanged;
			// 
			// PauseTypeEditor
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(numericUpDownPauseSeconds);
			Controls.Add(label1);
			Margin = new Padding(4, 3, 4, 3);
			Name = "PauseTypeEditor";
			Size = new Size(332, 91);
			Load += SequenceTypeEditor_Load;
			((System.ComponentModel.ISupportInitialize)numericUpDownPauseSeconds).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.NumericUpDown numericUpDownPauseSeconds;
	}
}
