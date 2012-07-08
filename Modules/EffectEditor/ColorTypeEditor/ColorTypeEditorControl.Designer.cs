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
			this.panelColor = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// panelColor
			// 
			this.panelColor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panelColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelColor.Location = new System.Drawing.Point(0, 0);
			this.panelColor.Margin = new System.Windows.Forms.Padding(0);
			this.panelColor.Name = "panelColor";
			this.panelColor.Size = new System.Drawing.Size(80, 40);
			this.panelColor.TabIndex = 2;
			this.panelColor.Click += new System.EventHandler(this.panelColor_Click);
			// 
			// ColorTypeEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panelColor);
			this.Name = "ColorTypeEditorControl";
			this.Size = new System.Drawing.Size(80, 40);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelColor;
	}
}
