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
			if (panelGradient!=null && panelGradient.BackgroundImage != null)
				panelGradient.BackgroundImage.Dispose();

			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.panelGradient = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// panelGradient
			// 
			this.panelGradient.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panelGradient.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelGradient.Location = new System.Drawing.Point(0, 0);
			this.panelGradient.Name = "panelGradient";
			this.panelGradient.Size = new System.Drawing.Size(150, 80);
			this.panelGradient.TabIndex = 0;
			this.panelGradient.Click += new System.EventHandler(this.panelGradient_Click);
			// 
			// ColorGradientTypeEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panelGradient);
			this.Name = "ColorGradientTypeEditorControl";
			this.Size = new System.Drawing.Size(150, 80);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelGradient;

	}
}
