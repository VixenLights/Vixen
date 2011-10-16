namespace VixenModules.EffectEditor.SetLevelEffectEditor
{
	partial class SetLevelEffectEditorControl
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
			this.levelTypeEditorControl = new VixenModules.EffectEditor.LevelTypeEditor.LevelTypeEditorControl();
			this.colorTypeEditorControl = new VixenModules.EffectEditor.ColorTypeEditor.ColorTypeEditorControl();
			this.SuspendLayout();
			// 
			// levelTypeEditorControl
			// 
			this.levelTypeEditorControl.Location = new System.Drawing.Point(3, 3);
			this.levelTypeEditorControl.Name = "levelTypeEditorControl";
			this.levelTypeEditorControl.Size = new System.Drawing.Size(139, 38);
			this.levelTypeEditorControl.TabIndex = 0;
			// 
			// colorTypeEditorControl
			// 
			this.colorTypeEditorControl.ColorValue = System.Drawing.Color.Empty;
			this.colorTypeEditorControl.EffectParameterValues = new object[] {
        ((object)(System.Drawing.Color.Empty))};
			this.colorTypeEditorControl.Location = new System.Drawing.Point(87, 3);
			this.colorTypeEditorControl.Name = "colorTypeEditorControl";
			this.colorTypeEditorControl.Size = new System.Drawing.Size(174, 70);
			this.colorTypeEditorControl.TabIndex = 1;
			// 
			// SetLevelEffectEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.colorTypeEditorControl);
			this.Controls.Add(this.levelTypeEditorControl);
			this.Name = "SetLevelEffectEditorControl";
			this.Size = new System.Drawing.Size(263, 77);
			this.ResumeLayout(false);

		}

		#endregion

		private LevelTypeEditor.LevelTypeEditorControl levelTypeEditorControl;
		private ColorTypeEditor.ColorTypeEditorControl colorTypeEditorControl;
	}
}
