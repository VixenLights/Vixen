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
			this.valueUpDown = new CommonElements.ControlsEx.ValueControls.ValueUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// valueUpDown
			// 
			this.valueUpDown.Location = new System.Drawing.Point(51, 6);
			this.valueUpDown.Name = "valueUpDown";
			this.valueUpDown.Size = new System.Drawing.Size(58, 25);
			this.valueUpDown.TabIndex = 1;
			this.valueUpDown.TrackerOrientation = System.Windows.Forms.Orientation.Vertical;
			this.valueUpDown.Value = 100;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(36, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Level:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(111, 11);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(15, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "%";
			// 
			// LevelTypeEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.valueUpDown);
			this.Name = "LevelTypeEditorControl";
			this.Size = new System.Drawing.Size(139, 38);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private CommonElements.ControlsEx.ValueControls.ValueUpDown valueUpDown;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
	}
}
