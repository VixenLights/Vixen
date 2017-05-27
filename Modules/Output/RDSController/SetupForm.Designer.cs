namespace VixenModules.Output.CommandController
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkSlow = new System.Windows.Forms.CheckBox();
            this.chkBiDirectional = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboPortName = new System.Windows.Forms.ComboBox();
            this.chkRequiresAuthentication = new System.Windows.Forms.CheckBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.txtHttpPassword = new System.Windows.Forms.TextBox();
            this.txtHttpUsername = new System.Windows.Forms.TextBox();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblUrl = new System.Windows.Forms.Label();
            this.radioHttp = new System.Windows.Forms.RadioButton();
            this.radioVFMT212R = new System.Windows.Forms.RadioButton();
            this.radioMRDS1322 = new System.Windows.Forms.RadioButton();
            this.radioMRDS192 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnTX = new System.Windows.Forms.Button();
            this.txtPSInterface = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLbl1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkHideLaunchedWindows = new System.Windows.Forms.CheckBox();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkSlow);
            this.groupBox3.Controls.Add(this.chkBiDirectional);
            this.groupBox3.Location = new System.Drawing.Point(531, 272);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox3.Size = new System.Drawing.Size(207, 69);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Options";
            this.groupBox3.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
            // 
            // chkSlow
            // 
            this.chkSlow.AutoSize = true;
            this.chkSlow.Location = new System.Drawing.Point(134, 29);
            this.chkSlow.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkSlow.Name = "chkSlow";
            this.chkSlow.Size = new System.Drawing.Size(63, 24);
            this.chkSlow.TabIndex = 1;
            this.chkSlow.Text = "Slow";
            this.chkSlow.UseVisualStyleBackColor = true;
            this.chkSlow.CheckedChanged += new System.EventHandler(this.chkSlow_CheckedChanged);
            // 
            // chkBiDirectional
            // 
            this.chkBiDirectional.AutoSize = true;
            this.chkBiDirectional.Location = new System.Drawing.Point(11, 29);
            this.chkBiDirectional.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkBiDirectional.Name = "chkBiDirectional";
            this.chkBiDirectional.Size = new System.Drawing.Size(117, 24);
            this.chkBiDirectional.TabIndex = 0;
            this.chkBiDirectional.Text = "BiDirectional";
            this.chkBiDirectional.UseVisualStyleBackColor = true;
            this.chkBiDirectional.CheckedChanged += new System.EventHandler(this.chkBiDirectional_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.cboPortName);
            this.groupBox4.Controls.Add(this.chkRequiresAuthentication);
            this.groupBox4.Controls.Add(this.lblPassword);
            this.groupBox4.Controls.Add(this.lblUserName);
            this.groupBox4.Controls.Add(this.txtHttpPassword);
            this.groupBox4.Controls.Add(this.txtHttpUsername);
            this.groupBox4.Controls.Add(this.txtUrl);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.lblUrl);
            this.groupBox4.Controls.Add(this.radioHttp);
            this.groupBox4.Controls.Add(this.radioVFMT212R);
            this.groupBox4.Controls.Add(this.radioMRDS1322);
            this.groupBox4.Controls.Add(this.radioMRDS192);
            this.groupBox4.Location = new System.Drawing.Point(12, 13);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox4.Size = new System.Drawing.Size(507, 328);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "RDS Hardware";
            this.groupBox4.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(323, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 20);
            this.label1.TabIndex = 15;
            this.label1.Text = "Serial Port";
            // 
            // cboPortName
            // 
            this.cboPortName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPortName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboPortName.FormattingEnabled = true;
            this.cboPortName.Location = new System.Drawing.Point(328, 59);
            this.cboPortName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cboPortName.Name = "cboPortName";
            this.cboPortName.Size = new System.Drawing.Size(101, 28);
            this.cboPortName.TabIndex = 4;
            this.cboPortName.SelectedIndexChanged += new System.EventHandler(this.cboPortName_SelectedIndexChanged);
            // 
            // chkRequiresAuthentication
            // 
            this.chkRequiresAuthentication.AutoSize = true;
            this.chkRequiresAuthentication.Enabled = false;
            this.chkRequiresAuthentication.Location = new System.Drawing.Point(55, 231);
            this.chkRequiresAuthentication.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkRequiresAuthentication.Name = "chkRequiresAuthentication";
            this.chkRequiresAuthentication.Size = new System.Drawing.Size(244, 24);
            this.chkRequiresAuthentication.TabIndex = 6;
            this.chkRequiresAuthentication.Text = "HTTP(s) Requires Authentication";
            this.chkRequiresAuthentication.UseVisualStyleBackColor = true;
            this.chkRequiresAuthentication.CheckedChanged += new System.EventHandler(this.chkRequiresAuthentication_CheckedChanged);
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Enabled = false;
            this.lblPassword.Location = new System.Drawing.Point(290, 271);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(70, 20);
            this.lblPassword.TabIndex = 13;
            this.lblPassword.Text = "Password";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Enabled = false;
            this.lblUserName.Location = new System.Drawing.Point(82, 271);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(78, 20);
            this.lblUserName.TabIndex = 12;
            this.lblUserName.Text = "UserName";
            // 
            // txtHttpPassword
            // 
            this.txtHttpPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHttpPassword.Enabled = false;
            this.txtHttpPassword.Location = new System.Drawing.Point(362, 267);
            this.txtHttpPassword.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtHttpPassword.MaxLength = 8;
            this.txtHttpPassword.Name = "txtHttpPassword";
            this.txtHttpPassword.Size = new System.Drawing.Size(137, 27);
            this.txtHttpPassword.TabIndex = 8;
            this.txtHttpPassword.TextChanged += new System.EventHandler(this.txtHttpPassword_TextChanged);
            // 
            // txtHttpUsername
            // 
            this.txtHttpUsername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHttpUsername.Enabled = false;
            this.txtHttpUsername.Location = new System.Drawing.Point(167, 267);
            this.txtHttpUsername.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtHttpUsername.MaxLength = 8;
            this.txtHttpUsername.Name = "txtHttpUsername";
            this.txtHttpUsername.Size = new System.Drawing.Size(116, 27);
            this.txtHttpUsername.TabIndex = 7;
            this.txtHttpUsername.TextChanged += new System.EventHandler(this.txtHttpUsername_TextChanged);
            // 
            // txtUrl
            // 
            this.txtUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUrl.Location = new System.Drawing.Point(55, 100);
            this.txtUrl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(444, 27);
            this.txtUrl.TabIndex = 5;
            this.txtUrl.TextChanged += new System.EventHandler(this.txtUrl_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(175, 191);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "{text}, {Time}";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(59, 191);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "Macro Fields:";
            // 
            // lblUrl
            // 
            this.lblUrl.AutoSize = true;
            this.lblUrl.Location = new System.Drawing.Point(8, 108);
            this.lblUrl.Name = "lblUrl";
            this.lblUrl.Size = new System.Drawing.Size(35, 20);
            this.lblUrl.TabIndex = 6;
            this.lblUrl.Text = "URL";
            // 
            // radioHttp
            // 
            this.radioHttp.AutoSize = true;
            this.radioHttp.Location = new System.Drawing.Point(11, 64);
            this.radioHttp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioHttp.Name = "radioHttp";
            this.radioHttp.Size = new System.Drawing.Size(83, 24);
            this.radioHttp.TabIndex = 2;
            this.radioHttp.Text = "HTTP(S)";
            this.radioHttp.UseVisualStyleBackColor = true;
            this.radioHttp.CheckedChanged += new System.EventHandler(this.radioHttp_CheckedChanged);
            // 
            // radioVFMT212R
            // 
            this.radioVFMT212R.AutoSize = true;
            this.radioVFMT212R.Location = new System.Drawing.Point(160, 64);
            this.radioVFMT212R.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioVFMT212R.Name = "radioVFMT212R";
            this.radioVFMT212R.Size = new System.Drawing.Size(106, 24);
            this.radioVFMT212R.TabIndex = 3;
            this.radioVFMT212R.Text = "V-FMT212R";
            this.radioVFMT212R.UseVisualStyleBackColor = true;
            this.radioVFMT212R.CheckedChanged += new System.EventHandler(this.radioVFMT212R_CheckedChanged);
            // 
            // radioMRDS1322
            // 
            this.radioMRDS1322.AutoSize = true;
            this.radioMRDS1322.Location = new System.Drawing.Point(160, 29);
            this.radioMRDS1322.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioMRDS1322.Name = "radioMRDS1322";
            this.radioMRDS1322.Size = new System.Drawing.Size(103, 24);
            this.radioMRDS1322.TabIndex = 1;
            this.radioMRDS1322.Text = "MRDS1322";
            this.radioMRDS1322.UseVisualStyleBackColor = true;
            this.radioMRDS1322.CheckedChanged += new System.EventHandler(this.radioMRDS1322_CheckedChanged);
            // 
            // radioMRDS192
            // 
            this.radioMRDS192.AutoSize = true;
            this.radioMRDS192.Checked = true;
            this.radioMRDS192.Location = new System.Drawing.Point(11, 29);
            this.radioMRDS192.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioMRDS192.Name = "radioMRDS192";
            this.radioMRDS192.Size = new System.Drawing.Size(95, 24);
            this.radioMRDS192.TabIndex = 0;
            this.radioMRDS192.TabStop = true;
            this.radioMRDS192.Text = "MRDS192";
            this.radioMRDS192.UseVisualStyleBackColor = true;
            this.radioMRDS192.CheckedChanged += new System.EventHandler(this.radioMRDS192_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnTX);
            this.groupBox2.Controls.Add(this.txtPSInterface);
            this.groupBox2.Location = new System.Drawing.Point(12, 351);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Size = new System.Drawing.Size(507, 72);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Test RDS Interface";
            this.groupBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
            // 
            // btnTX
            // 
            this.btnTX.Location = new System.Drawing.Point(400, 27);
            this.btnTX.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnTX.Name = "btnTX";
            this.btnTX.Size = new System.Drawing.Size(99, 36);
            this.btnTX.TabIndex = 1;
            this.btnTX.Text = "Send";
            this.btnTX.Click += new System.EventHandler(this.btnTX_Click);
            this.btnTX.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
            this.btnTX.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
            // 
            // txtPSInterface
            // 
            this.txtPSInterface.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPSInterface.Location = new System.Drawing.Point(16, 31);
            this.txtPSInterface.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtPSInterface.MaxLength = 64;
            this.txtPSInterface.Name = "txtPSInterface";
            this.txtPSInterface.Size = new System.Drawing.Size(376, 27);
            this.txtPSInterface.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Enabled = false;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(531, 13);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(207, 184);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 20;
            this.pictureBox1.TabStop = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLbl1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 427);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 18, 0);
            this.statusStrip1.Size = new System.Drawing.Size(756, 25);
            this.statusStrip1.TabIndex = 24;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StatusLbl1
            // 
            this.StatusLbl1.ForeColor = System.Drawing.Color.Crimson;
            this.StatusLbl1.Name = "StatusLbl1";
            this.StatusLbl1.Size = new System.Drawing.Size(40, 20);
            this.StatusLbl1.Text = "DIYC";
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(532, 362);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(206, 61);
            this.button1.TabIndex = 4;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
            this.button1.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkHideLaunchedWindows);
            this.groupBox1.Location = new System.Drawing.Point(531, 208);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(207, 60);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Launcher Options";
            this.groupBox1.Visible = false;
            this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
            // 
            // chkHideLaunchedWindows
            // 
            this.chkHideLaunchedWindows.AutoSize = true;
            this.chkHideLaunchedWindows.Location = new System.Drawing.Point(11, 29);
            this.chkHideLaunchedWindows.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkHideLaunchedWindows.Name = "chkHideLaunchedWindows";
            this.chkHideLaunchedWindows.Size = new System.Drawing.Size(195, 24);
            this.chkHideLaunchedWindows.TabIndex = 0;
            this.chkHideLaunchedWindows.Text = "Hide Launched Windows";
            this.chkHideLaunchedWindows.UseVisualStyleBackColor = true;
            this.chkHideLaunchedWindows.CheckedChanged += new System.EventHandler(this.chkHideLaunchedWindows_CheckedChanged);
            // 
            // SetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 452);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "SetupForm";
            this.Text = "Launcher Commands and RDS Configuration";
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox chkSlow;
		private System.Windows.Forms.CheckBox chkBiDirectional;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.RadioButton radioVFMT212R;
		private System.Windows.Forms.RadioButton radioMRDS1322;
		private System.Windows.Forms.RadioButton radioMRDS192;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button btnTX;
		private System.Windows.Forms.TextBox txtPSInterface;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel StatusLbl1;
		private System.Windows.Forms.Label lblUrl;
		private System.Windows.Forms.RadioButton radioHttp;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox txtUrl;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox chkHideLaunchedWindows;
		private System.Windows.Forms.TextBox txtHttpPassword;
		private System.Windows.Forms.TextBox txtHttpUsername;
		private System.Windows.Forms.Label lblPassword;
		private System.Windows.Forms.Label lblUserName;
		private System.Windows.Forms.CheckBox chkRequiresAuthentication;
		private System.Windows.Forms.ComboBox cboPortName;
		private System.Windows.Forms.Label label1;
	}
}