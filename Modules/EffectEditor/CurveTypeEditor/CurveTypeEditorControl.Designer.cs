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
			this.buttonEditCurve = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonEditCurve
			// 
			this.buttonEditCurve.Location = new System.Drawing.Point(3, 3);
			this.buttonEditCurve.Name = "buttonEditCurve";
			this.buttonEditCurve.Size = new System.Drawing.Size(164, 64);
			this.buttonEditCurve.TabIndex = 0;
			this.buttonEditCurve.Text = "Edit Curve";
			this.buttonEditCurve.UseVisualStyleBackColor = true;
			this.buttonEditCurve.Click += new System.EventHandler(this.buttonEditCurve_Click);
			// 
			// CurveTypeEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.buttonEditCurve);
			this.Name = "CurveTypeEditorControl";
			this.Size = new System.Drawing.Size(170, 70);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonEditCurve;
	}
}
