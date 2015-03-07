namespace VixenModules.EffectEditor.LevelTypeEditor
{
	partial class LevelTypeEditorControl
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
			if (disposing && (components != null)) {
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
			this.valueUpDown = new Common.Controls.ControlsEx.ValueControls.ValueUpDown();
			this.SuspendLayout();
			// 
			// valueUpDown
			// 
			this.valueUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left)));
			this.valueUpDown.Location = new System.Drawing.Point(3, 7);
			this.valueUpDown.Name = "valueUpDown";
			this.valueUpDown.Size = new System.Drawing.Size(88, 25);
			this.valueUpDown.TabIndex = 1;
			this.valueUpDown.TrackerOrientation = System.Windows.Forms.Orientation.Vertical;
			this.valueUpDown.Value = 100;
			// 
			// LevelTypeEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.valueUpDown);
			this.Name = "LevelTypeEditorControl";
			this.Size = new System.Drawing.Size(97, 39);
			this.ResumeLayout(false);

		}

		#endregion

		private Common.Controls.ControlsEx.ValueControls.ValueUpDown valueUpDown;
	}
}
