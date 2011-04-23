namespace TestEditor {
	partial class IntervalEditDialog {
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.label1 = new System.Windows.Forms.Label();
			this.listBoxIntervals = new System.Windows.Forms.ListBox();
			this.numericUpDownInterval = new System.Windows.Forms.NumericUpDown();
			this.buttonDone = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownInterval)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(22, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(47, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Intervals";
			// 
			// listBoxIntervals
			// 
			this.listBoxIntervals.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxIntervals.FormattingEnabled = true;
			this.listBoxIntervals.Location = new System.Drawing.Point(25, 37);
			this.listBoxIntervals.Name = "listBoxIntervals";
			this.listBoxIntervals.Size = new System.Drawing.Size(120, 264);
			this.listBoxIntervals.TabIndex = 1;
			this.listBoxIntervals.SelectedIndexChanged += new System.EventHandler(this.listBoxIntervals_SelectedIndexChanged);
			// 
			// numericUpDownInterval
			// 
			this.numericUpDownInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.numericUpDownInterval.Location = new System.Drawing.Point(195, 134);
			this.numericUpDownInterval.Name = "numericUpDownInterval";
			this.numericUpDownInterval.Size = new System.Drawing.Size(80, 20);
			this.numericUpDownInterval.TabIndex = 2;
			this.numericUpDownInterval.ValueChanged += new System.EventHandler(this.numericUpDownInterval_ValueChanged);
			// 
			// buttonDone
			// 
			this.buttonDone.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonDone.Location = new System.Drawing.Point(234, 275);
			this.buttonDone.Name = "buttonDone";
			this.buttonDone.Size = new System.Drawing.Size(75, 23);
			this.buttonDone.TabIndex = 3;
			this.buttonDone.Text = "Done";
			this.buttonDone.UseVisualStyleBackColor = true;
			this.buttonDone.Click += new System.EventHandler(this.buttonDone_Click);
			// 
			// IntervalEditDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(321, 310);
			this.Controls.Add(this.buttonDone);
			this.Controls.Add(this.numericUpDownInterval);
			this.Controls.Add(this.listBoxIntervals);
			this.Controls.Add(this.label1);
			this.Name = "IntervalEditDialog";
			this.Text = "IntervalEditDialog";
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownInterval)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListBox listBoxIntervals;
		private System.Windows.Forms.NumericUpDown numericUpDownInterval;
		private System.Windows.Forms.Button buttonDone;
	}
}