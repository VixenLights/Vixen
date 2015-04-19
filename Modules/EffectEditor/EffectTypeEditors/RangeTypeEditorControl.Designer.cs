namespace VixenModules.EffectEditor.EffectTypeEditors
{
	partial class RangeTypeEditorControl
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
			this.trackBarRange = new Common.Controls.ControlsEx.ValueControls.HMiniTracker();
			this.SuspendLayout();
			// 
			// trackBarRange
			// 
			this.trackBarRange.Location = new System.Drawing.Point(3, 3);
			this.trackBarRange.Maximum = 5000;
			this.trackBarRange.Name = "trackBarRange";
			this.trackBarRange.Size = new System.Drawing.Size(183, 23);
			this.trackBarRange.TabIndex = 0;
			this.trackBarRange.Text = "Range";
			this.trackBarRange.Value = 100;
			// 
			// RangeTypeEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.trackBarRange);
			this.Name = "RangeTypeEditorControl";
			this.Size = new System.Drawing.Size(189, 35);
			this.ResumeLayout(false);

		}

		#endregion

		private Common.Controls.ControlsEx.ValueControls.HMiniTracker trackBarRange;

	}
}
