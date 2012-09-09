namespace VixenModules.EffectEditor.PercentageTypeEditor {
	partial class PercentageTypeEditorControl {
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
			this.labelValue = new System.Windows.Forms.Label();
			this.trackBar = new System.Windows.Forms.TrackBar();
			this.panel1 = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelValue
			// 
			this.labelValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelValue.AutoSize = true;
			this.labelValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelValue.ForeColor = System.Drawing.Color.White;
			this.labelValue.Location = new System.Drawing.Point(3, 17);
			this.labelValue.Name = "labelValue";
			this.labelValue.Size = new System.Drawing.Size(28, 17);
			this.labelValue.TabIndex = 3;
			this.labelValue.Text = "0%";
			// 
			// trackBar
			// 
			this.trackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.trackBar.Location = new System.Drawing.Point(3, 3);
			this.trackBar.Maximum = 100;
			this.trackBar.Name = "trackBar";
			this.trackBar.Size = new System.Drawing.Size(367, 45);
			this.trackBar.TabIndex = 2;
			this.trackBar.TickFrequency = 10;
			this.trackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.trackBar.ValueChanged += new System.EventHandler(this.trackBar_ValueChanged);
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panel1.Controls.Add(this.labelValue);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(376, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(49, 50);
			this.panel1.TabIndex = 4;
			// 
			// PercentageTypeEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.trackBar);
			this.Name = "PercentageTypeEditorControl";
			this.Size = new System.Drawing.Size(425, 50);
			((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelValue;
		private System.Windows.Forms.TrackBar trackBar;
		private System.Windows.Forms.Panel panel1;
	}
}
