namespace VixenModules.Preview.TestPreview {
	partial class PositionValueControl {
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
			this.label1 = new System.Windows.Forms.Label();
			this.labelPosition = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(47, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Position:";
			// 
			// labelPosition
			// 
			this.labelPosition.AutoSize = true;
			this.labelPosition.Location = new System.Drawing.Point(57, 3);
			this.labelPosition.Name = "labelPosition";
			this.labelPosition.Size = new System.Drawing.Size(13, 13);
			this.labelPosition.TabIndex = 0;
			this.labelPosition.Text = "0";
			// 
			// PositionValueControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.labelPosition);
			this.Controls.Add(this.label1);
			this.Name = "PositionValueControl";
			this.Size = new System.Drawing.Size(199, 24);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelPosition;
	}
}
