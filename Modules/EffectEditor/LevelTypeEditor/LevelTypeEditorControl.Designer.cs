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
			this.label2 = new System.Windows.Forms.Label();
			this.valueUpDown = new Common.Controls.ControlsEx.ValueControls.ValueUpDown();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(69, 12);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(15, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "%";
			// 
			// valueUpDown
			// 
			this.valueUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.valueUpDown.Location = new System.Drawing.Point(9, 7);
			this.valueUpDown.Name = "valueUpDown";
			this.valueUpDown.Size = new System.Drawing.Size(58, 25);
			this.valueUpDown.TabIndex = 1;
			this.valueUpDown.TrackerOrientation = System.Windows.Forms.Orientation.Vertical;
			this.valueUpDown.Value = 100;
			// 
			// LevelTypeEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.valueUpDown);
			this.Name = "LevelTypeEditorControl";
			this.Size = new System.Drawing.Size(90, 39);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Common.Controls.ControlsEx.ValueControls.ValueUpDown valueUpDown;
		private System.Windows.Forms.Label label2;
	}
}
