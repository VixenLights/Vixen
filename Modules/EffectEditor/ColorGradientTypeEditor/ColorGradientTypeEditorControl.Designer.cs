namespace VixenModules.EffectEditor.ColorGradientTypeEditor
{
	partial class ColorGradientTypeEditorControl
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
			this.buttonEditColorGradient = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonEditColorGradient
			// 
			this.buttonEditColorGradient.Location = new System.Drawing.Point(3, 3);
			this.buttonEditColorGradient.Name = "buttonEditColorGradient";
			this.buttonEditColorGradient.Size = new System.Drawing.Size(164, 64);
			this.buttonEditColorGradient.TabIndex = 2;
			this.buttonEditColorGradient.Text = "Edit Gradient";
			this.buttonEditColorGradient.UseVisualStyleBackColor = true;
			this.buttonEditColorGradient.Click += new System.EventHandler(this.buttonEditColorGradient_Click);
			// 
			// ColorGradientTypeEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.buttonEditColorGradient);
			this.Name = "ColorGradientTypeEditorControl";
			this.Size = new System.Drawing.Size(170, 70);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonEditColorGradient;
	}
}
