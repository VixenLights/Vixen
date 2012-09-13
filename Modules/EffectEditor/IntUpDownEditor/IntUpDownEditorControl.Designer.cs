namespace VixenModules.EffectEditor.IntUpDownEditor {
	partial class IntUpDownEditorControl {
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
			this.nudValue = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.nudValue)).BeginInit();
			this.SuspendLayout();
			// 
			// nudValue
			// 
			this.nudValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.nudValue.Location = new System.Drawing.Point(12, 12);
			this.nudValue.Maximum = new decimal(new int[] {
            32768,
            0,
            0,
            0});
			this.nudValue.Name = "nudValue";
			this.nudValue.Size = new System.Drawing.Size(120, 20);
			this.nudValue.TabIndex = 0;
			// 
			// IntUpDownEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.nudValue);
			this.Name = "IntUpDownEditorControl";
			this.Size = new System.Drawing.Size(143, 43);
			((System.ComponentModel.ISupportInitialize)(this.nudValue)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.NumericUpDown nudValue;
	}
}
