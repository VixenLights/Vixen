namespace VixenModules.Preview.TestPreview {
	partial class LightingValueControl {
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
			this.pictureBoxColor = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxColor)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBoxColor
			// 
			this.pictureBoxColor.Location = new System.Drawing.Point(3, 3);
			this.pictureBoxColor.Name = "pictureBoxColor";
			this.pictureBoxColor.Size = new System.Drawing.Size(16, 16);
			this.pictureBoxColor.TabIndex = 1;
			this.pictureBoxColor.TabStop = false;
			// 
			// LightingValueControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pictureBoxColor);
			this.Name = "LightingValueControl";
			this.Size = new System.Drawing.Size(150, 23);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxColor)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBoxColor;
	}
}
