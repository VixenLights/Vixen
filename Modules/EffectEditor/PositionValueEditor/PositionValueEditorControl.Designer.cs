namespace VixenModules.EffectEditor.PositionValueEditor {
	partial class PositionValueEditorControl {
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
			this.trackBar = new System.Windows.Forms.TrackBar();
			this.labelValue = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
			this.SuspendLayout();
			// 
			// trackBar
			// 
			this.trackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.trackBar.Location = new System.Drawing.Point(3, 3);
			this.trackBar.Maximum = 100;
			this.trackBar.Name = "trackBar";
			this.trackBar.Size = new System.Drawing.Size(323, 45);
			this.trackBar.TabIndex = 0;
			this.trackBar.TickFrequency = 10;
			this.trackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.trackBar.ValueChanged += new System.EventHandler(this.trackBar_ValueChanged);
			// 
			// labelValue
			// 
			this.labelValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelValue.AutoSize = true;
			this.labelValue.Location = new System.Drawing.Point(332, 18);
			this.labelValue.Name = "labelValue";
			this.labelValue.Size = new System.Drawing.Size(21, 13);
			this.labelValue.TabIndex = 1;
			this.labelValue.Text = "0%";
			// 
			// PositionValueEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.labelValue);
			this.Controls.Add(this.trackBar);
			this.Name = "PositionValueEditorControl";
			this.Size = new System.Drawing.Size(370, 49);
			((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TrackBar trackBar;
		private System.Windows.Forms.Label labelValue;
	}
}
