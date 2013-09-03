namespace VixenModules.Output.RDSController
{
	partial class SetupForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;


		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupForm));
			this.groupPorts = new System.Windows.Forms.GroupBox();
			this.radioUSB = new System.Windows.Forms.RadioButton();
			this.radioCOM6 = new System.Windows.Forms.RadioButton();
			this.radioCOM4 = new System.Windows.Forms.RadioButton();
			this.radioCOM3 = new System.Windows.Forms.RadioButton();
			this.radioCOM2 = new System.Windows.Forms.RadioButton();
			this.radioCOM1 = new System.Windows.Forms.RadioButton();
			this.radioLPT1 = new System.Windows.Forms.RadioButton();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.chkSlow = new System.Windows.Forms.CheckBox();
			this.chkBiDirectional = new System.Windows.Forms.CheckBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.radioVFMT212R = new System.Windows.Forms.RadioButton();
			this.radioMRDS1322 = new System.Windows.Forms.RadioButton();
			this.radioMRDS192 = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnTX = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.txtPSInterface = new System.Windows.Forms.TextBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.StatusLbl1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.groupPorts.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupPorts
			// 
			this.groupPorts.Controls.Add(this.radioUSB);
			this.groupPorts.Controls.Add(this.radioCOM6);
			this.groupPorts.Controls.Add(this.radioCOM4);
			this.groupPorts.Controls.Add(this.radioCOM3);
			this.groupPorts.Controls.Add(this.radioCOM2);
			this.groupPorts.Controls.Add(this.radioCOM1);
			this.groupPorts.Controls.Add(this.radioLPT1);
			this.groupPorts.Location = new System.Drawing.Point(12, 12);
			this.groupPorts.Name = "groupPorts";
			this.groupPorts.Size = new System.Drawing.Size(77, 173);
			this.groupPorts.TabIndex = 13;
			this.groupPorts.TabStop = false;
			this.groupPorts.Text = "Port";
			// 
			// radioUSB
			// 
			this.radioUSB.AutoSize = true;
			this.radioUSB.Enabled = false;
			this.radioUSB.Location = new System.Drawing.Point(12, 150);
			this.radioUSB.Name = "radioUSB";
			this.radioUSB.Size = new System.Drawing.Size(47, 17);
			this.radioUSB.TabIndex = 6;
			this.radioUSB.Text = "USB";
			this.radioUSB.UseVisualStyleBackColor = true;
			// 
			// radioCOM6
			// 
			this.radioCOM6.AutoSize = true;
			this.radioCOM6.Location = new System.Drawing.Point(12, 128);
			this.radioCOM6.Name = "radioCOM6";
			this.radioCOM6.Size = new System.Drawing.Size(55, 17);
			this.radioCOM6.TabIndex = 5;
			this.radioCOM6.Text = "COM6";
			this.radioCOM6.UseVisualStyleBackColor = true;
			this.radioCOM6.CheckedChanged += new System.EventHandler(this.radioPorts_CheckedChanged);
			// 
			// radioCOM4
			// 
			this.radioCOM4.AutoSize = true;
			this.radioCOM4.Location = new System.Drawing.Point(12, 105);
			this.radioCOM4.Name = "radioCOM4";
			this.radioCOM4.Size = new System.Drawing.Size(55, 17);
			this.radioCOM4.TabIndex = 4;
			this.radioCOM4.Text = "COM4";
			this.radioCOM4.UseVisualStyleBackColor = true;
			this.radioCOM4.CheckedChanged += new System.EventHandler(this.radioPorts_CheckedChanged);
			// 
			// radioCOM3
			// 
			this.radioCOM3.AutoSize = true;
			this.radioCOM3.Location = new System.Drawing.Point(12, 82);
			this.radioCOM3.Name = "radioCOM3";
			this.radioCOM3.Size = new System.Drawing.Size(55, 17);
			this.radioCOM3.TabIndex = 3;
			this.radioCOM3.Text = "COM3";
			this.radioCOM3.UseVisualStyleBackColor = true;
			this.radioCOM3.CheckedChanged += new System.EventHandler(this.radioPorts_CheckedChanged);
			// 
			// radioCOM2
			// 
			this.radioCOM2.AutoSize = true;
			this.radioCOM2.Location = new System.Drawing.Point(12, 59);
			this.radioCOM2.Name = "radioCOM2";
			this.radioCOM2.Size = new System.Drawing.Size(55, 17);
			this.radioCOM2.TabIndex = 2;
			this.radioCOM2.Text = "COM2";
			this.radioCOM2.UseVisualStyleBackColor = true;
			this.radioCOM2.CheckedChanged += new System.EventHandler(this.radioPorts_CheckedChanged);
			// 
			// radioCOM1
			// 
			this.radioCOM1.AutoSize = true;
			this.radioCOM1.Checked = true;
			this.radioCOM1.Location = new System.Drawing.Point(12, 36);
			this.radioCOM1.Name = "radioCOM1";
			this.radioCOM1.Size = new System.Drawing.Size(55, 17);
			this.radioCOM1.TabIndex = 1;
			this.radioCOM1.TabStop = true;
			this.radioCOM1.Text = "COM1";
			this.radioCOM1.UseVisualStyleBackColor = true;
			this.radioCOM1.CheckedChanged += new System.EventHandler(this.radioPorts_CheckedChanged);
			// 
			// radioLPT1
			// 
			this.radioLPT1.AutoSize = true;
			this.radioLPT1.Location = new System.Drawing.Point(12, 13);
			this.radioLPT1.Name = "radioLPT1";
			this.radioLPT1.Size = new System.Drawing.Size(51, 17);
			this.radioLPT1.TabIndex = 0;
			this.radioLPT1.Text = "LPT1";
			this.radioLPT1.UseVisualStyleBackColor = true;
			this.radioLPT1.CheckedChanged += new System.EventHandler(this.radioPorts_CheckedChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.chkSlow);
			this.groupBox3.Controls.Add(this.chkBiDirectional);
			this.groupBox3.Location = new System.Drawing.Point(95, 19);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(110, 69);
			this.groupBox3.TabIndex = 15;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Options";
			// 
			// chkSlow
			// 
			this.chkSlow.AutoSize = true;
			this.chkSlow.Location = new System.Drawing.Point(9, 42);
			this.chkSlow.Name = "chkSlow";
			this.chkSlow.Size = new System.Drawing.Size(49, 17);
			this.chkSlow.TabIndex = 1;
			this.chkSlow.Text = "Slow";
			this.chkSlow.UseVisualStyleBackColor = true;
			this.chkSlow.CheckedChanged += new System.EventHandler(this.chkSlow_CheckedChanged);
			// 
			// chkBiDirectional
			// 
			this.chkBiDirectional.AutoSize = true;
			this.chkBiDirectional.Location = new System.Drawing.Point(9, 19);
			this.chkBiDirectional.Name = "chkBiDirectional";
			this.chkBiDirectional.Size = new System.Drawing.Size(85, 17);
			this.chkBiDirectional.TabIndex = 0;
			this.chkBiDirectional.Text = "BiDirectional";
			this.chkBiDirectional.UseVisualStyleBackColor = true;
			this.chkBiDirectional.CheckedChanged += new System.EventHandler(this.chkBiDirectional_CheckedChanged);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.radioVFMT212R);
			this.groupBox4.Controls.Add(this.radioMRDS1322);
			this.groupBox4.Controls.Add(this.radioMRDS192);
			this.groupBox4.Location = new System.Drawing.Point(95, 94);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(110, 91);
			this.groupBox4.TabIndex = 16;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Hardware";
			// 
			// radioVFMT212R
			// 
			this.radioVFMT212R.AutoSize = true;
			this.radioVFMT212R.Location = new System.Drawing.Point(9, 65);
			this.radioVFMT212R.Name = "radioVFMT212R";
			this.radioVFMT212R.Size = new System.Drawing.Size(83, 17);
			this.radioVFMT212R.TabIndex = 2;
			this.radioVFMT212R.Text = "V-FMT212R";
			this.radioVFMT212R.UseVisualStyleBackColor = true;
			this.radioVFMT212R.CheckedChanged += new System.EventHandler(this.radioVFMT212R_CheckedChanged);
			// 
			// radioMRDS1322
			// 
			this.radioMRDS1322.AutoSize = true;
			this.radioMRDS1322.Location = new System.Drawing.Point(9, 42);
			this.radioMRDS1322.Name = "radioMRDS1322";
			this.radioMRDS1322.Size = new System.Drawing.Size(81, 17);
			this.radioMRDS1322.TabIndex = 1;
			this.radioMRDS1322.Text = "MRDS1322";
			this.radioMRDS1322.UseVisualStyleBackColor = true;
			this.radioMRDS1322.CheckedChanged += new System.EventHandler(this.radioMRDS1322_CheckedChanged);
			// 
			// radioMRDS192
			// 
			this.radioMRDS192.AutoSize = true;
			this.radioMRDS192.Checked = true;
			this.radioMRDS192.Location = new System.Drawing.Point(9, 19);
			this.radioMRDS192.Name = "radioMRDS192";
			this.radioMRDS192.Size = new System.Drawing.Size(75, 17);
			this.radioMRDS192.TabIndex = 0;
			this.radioMRDS192.TabStop = true;
			this.radioMRDS192.Text = "MRDS192";
			this.radioMRDS192.UseVisualStyleBackColor = true;
			this.radioMRDS192.CheckedChanged += new System.EventHandler(this.radioMRDS192_CheckedChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.btnTX);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.txtPSInterface);
			this.groupBox2.Location = new System.Drawing.Point(12, 191);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(363, 65);
			this.groupBox2.TabIndex = 14;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Test Interface";
			// 
			// btnTX
			// 
			this.btnTX.Location = new System.Drawing.Point(268, 33);
			this.btnTX.Name = "btnTX";
			this.btnTX.Size = new System.Drawing.Size(75, 23);
			this.btnTX.TabIndex = 1;
			this.btnTX.Text = "Send";
			this.btnTX.Click += new System.EventHandler(this.btnTX_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(21, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "PS";
			// 
			// txtPSInterface
			// 
			this.txtPSInterface.Location = new System.Drawing.Point(12, 35);
			this.txtPSInterface.MaxLength = 8;
			this.txtPSInterface.Name = "txtPSInterface";
			this.txtPSInterface.Size = new System.Drawing.Size(250, 20);
			this.txtPSInterface.TabIndex = 4;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Enabled = false;
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(280, 75);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(56, 36);
			this.pictureBox1.TabIndex = 20;
			this.pictureBox1.TabStop = false;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLbl1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 258);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(382, 22);
			this.statusStrip1.TabIndex = 24;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// StatusLbl1
			// 
			this.StatusLbl1.ForeColor = System.Drawing.Color.Crimson;
			this.StatusLbl1.Name = "StatusLbl1";
			this.StatusLbl1.Size = new System.Drawing.Size(33, 17);
			this.StatusLbl1.Text = "DIYC";
			// 
			// SetupForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(382, 280);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.groupPorts);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.pictureBox1);
			this.Name = "SetupForm";
			this.Text = "RDS Configuration";
			this.groupPorts.ResumeLayout(false);
			this.groupPorts.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupPorts;
		private System.Windows.Forms.RadioButton radioCOM6;
		private System.Windows.Forms.RadioButton radioCOM4;
		private System.Windows.Forms.RadioButton radioCOM3;
		private System.Windows.Forms.RadioButton radioCOM2;
		private System.Windows.Forms.RadioButton radioCOM1;
		private System.Windows.Forms.RadioButton radioLPT1;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox chkSlow;
		private System.Windows.Forms.CheckBox chkBiDirectional;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.RadioButton radioVFMT212R;
		private System.Windows.Forms.RadioButton radioMRDS1322;
		private System.Windows.Forms.RadioButton radioMRDS192;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button btnTX;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtPSInterface;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.RadioButton radioUSB;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel StatusLbl1;
	}
}