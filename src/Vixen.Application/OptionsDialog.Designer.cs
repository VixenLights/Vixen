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
			btnOK = new Button();
			btnCancel = new Button();
			label1 = new Label();
			label2 = new Label();
			ctlUpdateInteral = new NumericUpDown();
			label3 = new Label();
			wasapiLatency = new NumericUpDown();
			grpAudio = new GroupBox();
			grpVideoEffectCache = new GroupBox();
			label4 = new Label();
			cmbBoxCacheFileType = new ComboBox();
			chkBoxClearCacheOnExit = new CheckBox();
			((System.ComponentModel.ISupportInitialize)ctlUpdateInteral).BeginInit();
			((System.ComponentModel.ISupportInitialize)wasapiLatency).BeginInit();
			grpAudio.SuspendLayout();
			grpVideoEffectCache.SuspendLayout();
			SuspendLayout();
			// 
			// btnOK
			// 
			btnOK.DialogResult = DialogResult.OK;
			btnOK.Location = new Point(196, 255);
			btnOK.Name = "btnOK";
			btnOK.Size = new Size(87, 27);
			btnOK.TabIndex = 1;
			btnOK.Text = "OK";
			btnOK.UseVisualStyleBackColor = true;
			btnOK.Click += btnOK_Click;
			btnOK.MouseLeave += buttonBackground_MouseLeave;
			btnOK.MouseHover += buttonBackground_MouseHover;
			// 
			// btnCancel
			// 
			btnCancel.DialogResult = DialogResult.Cancel;
			btnCancel.Location = new Point(290, 255);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new Size(87, 27);
			btnCancel.TabIndex = 2;
			btnCancel.Text = "Cancel";
			btnCancel.UseVisualStyleBackColor = true;
			btnCancel.MouseLeave += buttonBackground_MouseLeave;
			btnCancel.MouseHover += buttonBackground_MouseHover;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(14, 10);
			label1.Name = "label1";
			label1.Size = new Size(346, 15);
			label1.TabIndex = 2;
			label1.Text = "This form lets you set various options that control V3's operation";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(14, 47);
			label2.Name = "label2";
			label2.Size = new Size(90, 15);
			label2.TabIndex = 3;
			label2.Text = "Update Interval:";
			// 
			// ctlUpdateInteral
			// 
			ctlUpdateInteral.Increment = new decimal(new int[] { 23, 0, 0, 0 });
			ctlUpdateInteral.Location = new Point(116, 44);
			ctlUpdateInteral.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
			ctlUpdateInteral.Name = "ctlUpdateInteral";
			ctlUpdateInteral.Size = new Size(59, 23);
			ctlUpdateInteral.TabIndex = 0;
			ctlUpdateInteral.Value = new decimal(new int[] { 23, 0, 0, 0 });
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new Point(6, 31);
			label3.Name = "label3";
			label3.Size = new Size(51, 15);
			label3.TabIndex = 4;
			label3.Text = "Latency:";
			// 
			// wasapiLatency
			// 
			wasapiLatency.Increment = new decimal(new int[] { 10, 0, 0, 0 });
			wasapiLatency.Location = new Point(99, 29);
			wasapiLatency.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
			wasapiLatency.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
			wasapiLatency.Name = "wasapiLatency";
			wasapiLatency.Size = new Size(59, 23);
			wasapiLatency.TabIndex = 5;
			wasapiLatency.Value = new decimal(new int[] { 25, 0, 0, 0 });
			// 
			// grpAudio
			// 
			grpAudio.Controls.Add(label3);
			grpAudio.Controls.Add(wasapiLatency);
			grpAudio.Location = new Point(17, 80);
			grpAudio.Name = "grpAudio";
			grpAudio.Size = new Size(343, 76);
			grpAudio.TabIndex = 7;
			grpAudio.TabStop = false;
			grpAudio.Text = "Audio";
			// 
			// grpVideoEffectCache
			// 
			grpVideoEffectCache.Controls.Add(label4);
			grpVideoEffectCache.Controls.Add(cmbBoxCacheFileType);
			grpVideoEffectCache.Controls.Add(chkBoxClearCacheOnExit);
			grpVideoEffectCache.Location = new Point(17, 166);
			grpVideoEffectCache.Name = "grpVideoEffectCache";
			grpVideoEffectCache.Size = new Size(343, 76);
			grpVideoEffectCache.TabIndex = 8;
			grpVideoEffectCache.TabStop = false;
			grpVideoEffectCache.Text = "Video Effect Cache";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new Point(203, 23);
			label4.Name = "label4";
			label4.Size = new Size(91, 15);
			label4.TabIndex = 2;
			label4.Text = "Cache File Type:";
			// 
			// cmbBoxCacheFileType
			// 
			cmbBoxCacheFileType.FormattingEnabled = true;
			cmbBoxCacheFileType.Items.AddRange(new object[] { "bmp", "png" });
			cmbBoxCacheFileType.Location = new Point(203, 39);
			cmbBoxCacheFileType.Name = "cmbBoxCacheFileType";
			cmbBoxCacheFileType.Size = new Size(121, 23);
			cmbBoxCacheFileType.TabIndex = 1;
			cmbBoxCacheFileType.Text = "bmp";
			// 
			// chkBoxClearCacheOnExit
			// 
			chkBoxClearCacheOnExit.AutoSize = true;
			chkBoxClearCacheOnExit.Checked = true;
			chkBoxClearCacheOnExit.CheckState = CheckState.Checked;
			chkBoxClearCacheOnExit.Location = new Point(6, 34);
			chkBoxClearCacheOnExit.Name = "chkBoxClearCacheOnExit";
			chkBoxClearCacheOnExit.Size = new Size(128, 19);
			chkBoxClearCacheOnExit.TabIndex = 0;
			chkBoxClearCacheOnExit.Text = "Clear Cache on Exit";
			chkBoxClearCacheOnExit.UseVisualStyleBackColor = true;
			// 
			// OptionsDialog
			// 
			AcceptButton = btnOK;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			CancelButton = btnCancel;
			ClientSize = new Size(392, 293);
			Controls.Add(grpVideoEffectCache);
			Controls.Add(grpAudio);
			Controls.Add(ctlUpdateInteral);
			Controls.Add(label2);
			Controls.Add(label1);
			Controls.Add(btnCancel);
			Controls.Add(btnOK);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			MinimumSize = new Size(408, 39);
			Name = "OptionsDialog";
			SizeGripStyle = SizeGripStyle.Hide;
			StartPosition = FormStartPosition.CenterParent;
			Text = "Options";
			Load += OptionsDialog_Load;
			((System.ComponentModel.ISupportInitialize)ctlUpdateInteral).EndInit();
			((System.ComponentModel.ISupportInitialize)wasapiLatency).EndInit();
			grpAudio.ResumeLayout(false);
			grpAudio.PerformLayout();
			grpVideoEffectCache.ResumeLayout(false);
			grpVideoEffectCache.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown ctlUpdateInteral;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown wasapiLatency;
		private System.Windows.Forms.GroupBox grpAudio;
		private GroupBox grpVideoEffectCache;
		private CheckBox chkBoxClearCacheOnExit;
		private Label label4;
		private ComboBox cmbBoxCacheFileType;
	}
}