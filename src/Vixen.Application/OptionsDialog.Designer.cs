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
			cmbBoxVideoEffectCacheFileType = new ComboBox();
			chkBoxClearEffectCacheOnExit = new CheckBox();
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
			btnOK.TabIndex = 6;
			btnOK.Text = "OK";
			btnOK.UseVisualStyleBackColor = true;
			btnOK.Click += btnOK_Click;
			// 
			// btnCancel
			// 
			btnCancel.DialogResult = DialogResult.Cancel;
			btnCancel.Location = new Point(290, 255);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new Size(87, 27);
			btnCancel.TabIndex = 7;
			btnCancel.Text = "Cancel";
			btnCancel.UseVisualStyleBackColor = true;
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
			wasapiLatency.TabIndex = 3;
			wasapiLatency.Value = new decimal(new int[] { 25, 0, 0, 0 });
			// 
			// grpAudio
			// 
			grpAudio.Controls.Add(label3);
			grpAudio.Controls.Add(wasapiLatency);
			grpAudio.Location = new Point(17, 80);
			grpAudio.Name = "grpAudio";
			grpAudio.Size = new Size(343, 76);
			grpAudio.TabIndex = 2;
			grpAudio.TabStop = false;
			grpAudio.Text = "Audio";
			// 
			// grpVideoEffectCache
			// 
			grpVideoEffectCache.Controls.Add(label4);
			grpVideoEffectCache.Controls.Add(cmbBoxVideoEffectCacheFileType);
			grpVideoEffectCache.Location = new Point(17, 166);
			grpVideoEffectCache.Name = "grpVideoEffectCache";
			grpVideoEffectCache.Size = new Size(343, 76);
			grpVideoEffectCache.TabIndex = 4;
			grpVideoEffectCache.TabStop = false;
			grpVideoEffectCache.Text = "Video Effect Cache";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new Point(6, 31);
			label4.Name = "label4";
			label4.Size = new Size(91, 15);
			label4.TabIndex = 2;
			label4.Text = "Cache File Type:";
			// 
			// cmbBoxVideoEffectCacheFileType
			// 
			cmbBoxVideoEffectCacheFileType.FormattingEnabled = true;
			cmbBoxVideoEffectCacheFileType.Items.AddRange(new object[] { "bmp", "png" });
			cmbBoxVideoEffectCacheFileType.Location = new Point(99, 28);
			cmbBoxVideoEffectCacheFileType.Name = "cmbBoxVideoEffectCacheFileType";
			cmbBoxVideoEffectCacheFileType.Size = new Size(59, 23);
			cmbBoxVideoEffectCacheFileType.TabIndex = 5;
			cmbBoxVideoEffectCacheFileType.Text = "bmp";
			// 
			// chkBoxClearEffectCacheOnExit
			// 
			chkBoxClearEffectCacheOnExit.AutoSize = true;
			chkBoxClearEffectCacheOnExit.Checked = true;
			chkBoxClearEffectCacheOnExit.CheckState = CheckState.Checked;
			chkBoxClearEffectCacheOnExit.Location = new Point(216, 46);
			chkBoxClearEffectCacheOnExit.Name = "chkBoxClearEffectCacheOnExit";
			chkBoxClearEffectCacheOnExit.Size = new Size(161, 19);
			chkBoxClearEffectCacheOnExit.TabIndex = 1;
			chkBoxClearEffectCacheOnExit.Text = "Clear Effect Cache on Exit";
			chkBoxClearEffectCacheOnExit.UseVisualStyleBackColor = true;
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
			Controls.Add(chkBoxClearEffectCacheOnExit);
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
		private CheckBox chkBoxClearEffectCacheOnExit;
		private Label label4;
		private ComboBox cmbBoxVideoEffectCacheFileType;
	}
}