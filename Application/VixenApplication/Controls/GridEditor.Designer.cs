namespace VixenApplication.Controls {
	partial class GridEditor {
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
			this.textBoxWidth = new System.Windows.Forms.TextBox();
			this.textBoxHeight = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxColStart = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxRowStart = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textBoxFormat = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(23, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Width";
			// 
			// textBoxWidth
			// 
			this.textBoxWidth.Location = new System.Drawing.Point(147, 20);
			this.textBoxWidth.Name = "textBoxWidth";
			this.textBoxWidth.Size = new System.Drawing.Size(73, 20);
			this.textBoxWidth.TabIndex = 1;
			this.textBoxWidth.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxWidth_Validating);
			// 
			// textBoxHeight
			// 
			this.textBoxHeight.Location = new System.Drawing.Point(147, 46);
			this.textBoxHeight.Name = "textBoxHeight";
			this.textBoxHeight.Size = new System.Drawing.Size(73, 20);
			this.textBoxHeight.TabIndex = 3;
			this.textBoxHeight.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxHeight_Validating);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(23, 49);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(38, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Height";
			// 
			// textBoxColStart
			// 
			this.textBoxColStart.Location = new System.Drawing.Point(147, 121);
			this.textBoxColStart.Name = "textBoxColStart";
			this.textBoxColStart.Size = new System.Drawing.Size(73, 20);
			this.textBoxColStart.TabIndex = 7;
			this.textBoxColStart.Text = "1";
			this.textBoxColStart.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxColStart_Validating);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(23, 124);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(118, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Starting column number";
			// 
			// textBoxRowStart
			// 
			this.textBoxRowStart.Location = new System.Drawing.Point(147, 95);
			this.textBoxRowStart.Name = "textBoxRowStart";
			this.textBoxRowStart.Size = new System.Drawing.Size(73, 20);
			this.textBoxRowStart.TabIndex = 5;
			this.textBoxRowStart.Text = "1";
			this.textBoxRowStart.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxRowStart_Validating);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(23, 98);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(101, 13);
			this.label4.TabIndex = 4;
			this.label4.Text = "Starting row number";
			// 
			// textBoxFormat
			// 
			this.textBoxFormat.Location = new System.Drawing.Point(147, 168);
			this.textBoxFormat.Name = "textBoxFormat";
			this.textBoxFormat.Size = new System.Drawing.Size(177, 20);
			this.textBoxFormat.TabIndex = 9;
			this.textBoxFormat.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxFormat_Validating);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(23, 171);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(39, 13);
			this.label5.TabIndex = 8;
			this.label5.Text = "Format";
			// 
			// GridEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.textBoxFormat);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textBoxColStart);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textBoxRowStart);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textBoxHeight);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBoxWidth);
			this.Controls.Add(this.label1);
			this.Name = "GridEditor";
			this.Size = new System.Drawing.Size(364, 210);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxWidth;
		private System.Windows.Forms.TextBox textBoxHeight;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxColStart;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBoxRowStart;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBoxFormat;
		private System.Windows.Forms.Label label5;
	}
}
