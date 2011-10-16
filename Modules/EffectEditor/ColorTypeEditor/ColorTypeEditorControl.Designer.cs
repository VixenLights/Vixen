namespace VixenModules.EffectEditor.ColorTypeEditor
{
	partial class ColorTypeEditorControl
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
			this.buttonEditColor = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonEditColor
			// 
			this.buttonEditColor.Location = new System.Drawing.Point(3, 3);
			this.buttonEditColor.Name = "buttonEditColor";
			this.buttonEditColor.Size = new System.Drawing.Size(164, 64);
			this.buttonEditColor.TabIndex = 1;
			this.buttonEditColor.Text = "Edit Color";
			this.buttonEditColor.UseVisualStyleBackColor = true;
			this.buttonEditColor.Click += new System.EventHandler(this.buttonEditColor_Click);
			// 
			// ColorTypeEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.buttonEditColor);
			this.Name = "ColorTypeEditorControl";
			this.Size = new System.Drawing.Size(170, 70);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonEditColor;
	}
}
