namespace VixenModules.EffectEditor.CurveTypeEditor
{
	partial class CurveTypeEditorControl
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
			this.panelCurve = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// panelCurve
			// 
			this.panelCurve.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panelCurve.Location = new System.Drawing.Point(0, 0);
			this.panelCurve.Name = "panelCurve";
			this.panelCurve.Size = new System.Drawing.Size(150, 80);
			this.panelCurve.TabIndex = 0;
			this.panelCurve.Click += new System.EventHandler(this.panelCurve_Click);
			// 
			// CurveTypeEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panelCurve);
			this.Name = "CurveTypeEditorControl";
			this.Size = new System.Drawing.Size(150, 80);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelCurve;

	}
}
