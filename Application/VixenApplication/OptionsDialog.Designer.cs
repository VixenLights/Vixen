namespace VixenApplication
{
	partial class OptionsDialog
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.ctlUpdateInteral = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.wasapiLatency = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.ctlUpdateInteral)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.wasapiLatency)).BeginInit();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(196, 133);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(87, 27);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			this.btnOK.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.btnOK.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(290, 133);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(87, 27);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.btnCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(14, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(346, 15);
			this.label1.TabIndex = 2;
			this.label1.Text = "This form lets you set various options that control V3\'s operation";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(14, 47);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(90, 15);
			this.label2.TabIndex = 3;
			this.label2.Text = "Update Interval:";
			// 
			// ctlUpdateInteral
			// 
			this.ctlUpdateInteral.Increment = new decimal(new int[] {
            23,
            0,
            0,
            0});
			this.ctlUpdateInteral.Location = new System.Drawing.Point(116, 44);
			this.ctlUpdateInteral.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
			this.ctlUpdateInteral.Name = "ctlUpdateInteral";
			this.ctlUpdateInteral.Size = new System.Drawing.Size(59, 23);
			this.ctlUpdateInteral.TabIndex = 0;
			this.ctlUpdateInteral.Value = new decimal(new int[] {
            23,
            0,
            0,
            0});
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(14, 80);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(92, 15);
			this.label3.TabIndex = 4;
			this.label3.Text = "Wasapi Latency:";
			// 
			// wasapiLatency
			// 
			this.wasapiLatency.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.wasapiLatency.Location = new System.Drawing.Point(116, 73);
			this.wasapiLatency.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
			this.wasapiLatency.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.wasapiLatency.Name = "wasapiLatency";
			this.wasapiLatency.Size = new System.Drawing.Size(59, 23);
			this.wasapiLatency.TabIndex = 5;
			this.wasapiLatency.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
			// 
			// OptionsDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(392, 170);
			this.Controls.Add(this.wasapiLatency);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.ctlUpdateInteral);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(408, 209);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(408, 209);
			this.Name = "OptionsDialog";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Options";
			this.Load += new System.EventHandler(this.OptionsDialog_Load);
			((System.ComponentModel.ISupportInitialize)(this.ctlUpdateInteral)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.wasapiLatency)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown ctlUpdateInteral;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown wasapiLatency;
	}
}