namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class FormEffectEditor
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
			if (disposing && (components != null))
			{
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
			this.propertyGridEffectProperties = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			// 
			// propertyGridEffectProperties
			// 
			this.propertyGridEffectProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGridEffectProperties.Location = new System.Drawing.Point(0, 0);
			this.propertyGridEffectProperties.Name = "propertyGridEffectProperties";
			this.propertyGridEffectProperties.Size = new System.Drawing.Size(257, 521);
			this.propertyGridEffectProperties.TabIndex = 2;
			// 
			// FormEffectEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(257, 521);
			this.Controls.Add(this.propertyGridEffectProperties);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "FormEffectEditor";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PropertyGrid propertyGridEffectProperties;

	}
}
