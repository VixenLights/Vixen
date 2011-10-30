namespace SampleEffectEditor {
	partial class SampleEffectEditorControl {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.numericUpDownLevel = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownLevel)).BeginInit();
			this.SuspendLayout();
			// 
			// numericUpDownLevel
			// 
			this.numericUpDownLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.numericUpDownLevel.DecimalPlaces = 2;
			this.numericUpDownLevel.Location = new System.Drawing.Point(7, 3);
			this.numericUpDownLevel.Name = "numericUpDownLevel";
			this.numericUpDownLevel.Size = new System.Drawing.Size(68, 20);
			this.numericUpDownLevel.TabIndex = 1;
			this.numericUpDownLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TestCommandEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.numericUpDownLevel);
			this.Name = "TestCommandEditorControl";
			this.Size = new System.Drawing.Size(78, 27);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownLevel)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.NumericUpDown numericUpDownLevel;
	}
}
