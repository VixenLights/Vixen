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
			this.propertyGridEffectProperties.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
			this.propertyGridEffectProperties.CategoryForeColor = System.Drawing.Color.WhiteSmoke;
			this.propertyGridEffectProperties.CategorySplitterColor = System.Drawing.Color.Black;
			this.propertyGridEffectProperties.CommandsBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.propertyGridEffectProperties.CommandsBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.propertyGridEffectProperties.CommandsForeColor = System.Drawing.Color.Silver;
			this.propertyGridEffectProperties.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
			this.propertyGridEffectProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGridEffectProperties.HelpBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.propertyGridEffectProperties.HelpBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.propertyGridEffectProperties.HelpForeColor = System.Drawing.Color.Silver;
			this.propertyGridEffectProperties.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.propertyGridEffectProperties.Location = new System.Drawing.Point(0, 0);
			this.propertyGridEffectProperties.Name = "propertyGridEffectProperties";
			this.propertyGridEffectProperties.SelectedItemWithFocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.propertyGridEffectProperties.SelectedItemWithFocusForeColor = System.Drawing.Color.Gainsboro;
			this.propertyGridEffectProperties.Size = new System.Drawing.Size(367, 521);
			this.propertyGridEffectProperties.TabIndex = 2;
			this.propertyGridEffectProperties.ViewBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.propertyGridEffectProperties.ViewBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.propertyGridEffectProperties.ViewForeColor = System.Drawing.Color.Silver;
			// 
			// FormEffectEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.DimGray;
			this.ClientSize = new System.Drawing.Size(367, 521);
			this.ControlBox = false;
			this.Controls.Add(this.propertyGridEffectProperties);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ForeColor = System.Drawing.Color.Silver;
			this.Name = "FormEffectEditor";
			this.Text = "Effect Editor";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PropertyGrid propertyGridEffectProperties;

	}
}
