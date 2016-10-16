namespace VixenModules.Output.ElexolEtherIO
{
	partial class SetupDialog
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
			this.ethernetSettingsGroupBox = new System.Windows.Forms.GroupBox();
			this.portTextBox = new System.Windows.Forms.TextBox();
			this.portLabel = new System.Windows.Forms.Label();
			this.testButton = new System.Windows.Forms.Button();
			this.ipAddressTextBox = new System.Windows.Forms.TextBox();
			this.ipaddressLabel = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.minOnIntensityLabel = new System.Windows.Forms.Label();
			this.minIntensityLabel = new System.Windows.Forms.Label();
			this.sliderMinIntensityTrackBar = new System.Windows.Forms.TrackBar();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.ethernetSettingsGroupBox.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sliderMinIntensityTrackBar)).BeginInit();
			this.SuspendLayout();
			// 
			// ethernetSettingsGroupBox
			// 
			this.ethernetSettingsGroupBox.Controls.Add(this.portTextBox);
			this.ethernetSettingsGroupBox.Controls.Add(this.portLabel);
			this.ethernetSettingsGroupBox.Controls.Add(this.testButton);
			this.ethernetSettingsGroupBox.Controls.Add(this.ipAddressTextBox);
			this.ethernetSettingsGroupBox.Controls.Add(this.ipaddressLabel);
			this.ethernetSettingsGroupBox.Location = new System.Drawing.Point(15, 15);
			this.ethernetSettingsGroupBox.Name = "ethernetSettingsGroupBox";
			this.ethernetSettingsGroupBox.Size = new System.Drawing.Size(324, 91);
			this.ethernetSettingsGroupBox.TabIndex = 0;
			this.ethernetSettingsGroupBox.TabStop = false;
			this.ethernetSettingsGroupBox.Text = "Ethernet Settings";
			this.ethernetSettingsGroupBox.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// portTextBox
			// 
			this.portTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.portTextBox.Location = new System.Drawing.Point(84, 53);
			this.portTextBox.Name = "portTextBox";
			this.portTextBox.Size = new System.Drawing.Size(130, 23);
			this.portTextBox.TabIndex = 1;
			this.portTextBox.Text = "2424";
			// 
			// portLabel
			// 
			this.portLabel.AutoSize = true;
			this.portLabel.Location = new System.Drawing.Point(47, 57);
			this.portLabel.Name = "portLabel";
			this.portLabel.Size = new System.Drawing.Size(29, 15);
			this.portLabel.TabIndex = 3;
			this.portLabel.Text = "Port";
			// 
			// testButton
			// 
			this.testButton.Location = new System.Drawing.Point(230, 21);
			this.testButton.Name = "testButton";
			this.testButton.Size = new System.Drawing.Size(87, 55);
			this.testButton.TabIndex = 2;
			this.testButton.Text = "Test";
			this.testButton.UseVisualStyleBackColor = true;
			this.testButton.Click += new System.EventHandler(this.testButton_Click);
			this.testButton.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.testButton.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// ipAddressTextBox
			// 
			this.ipAddressTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.ipAddressTextBox.Location = new System.Drawing.Point(84, 23);
			this.ipAddressTextBox.Name = "ipAddressTextBox";
			this.ipAddressTextBox.Size = new System.Drawing.Size(130, 23);
			this.ipAddressTextBox.TabIndex = 0;
			this.ipAddressTextBox.Text = "10.10.10.10";
			// 
			// ipaddressLabel
			// 
			this.ipaddressLabel.AutoSize = true;
			this.ipaddressLabel.Location = new System.Drawing.Point(9, 27);
			this.ipaddressLabel.Name = "ipaddressLabel";
			this.ipaddressLabel.Size = new System.Drawing.Size(62, 15);
			this.ipaddressLabel.TabIndex = 0;
			this.ipaddressLabel.Text = "IP Address";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.minOnIntensityLabel);
			this.groupBox1.Controls.Add(this.minIntensityLabel);
			this.groupBox1.Controls.Add(this.sliderMinIntensityTrackBar);
			this.groupBox1.Location = new System.Drawing.Point(14, 113);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(324, 105);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Advanced";
			this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// minOnIntensityLabel
			// 
			this.minOnIntensityLabel.AutoSize = true;
			this.minOnIntensityLabel.Location = new System.Drawing.Point(13, 18);
			this.minOnIntensityLabel.Name = "minOnIntensityLabel";
			this.minOnIntensityLabel.Size = new System.Drawing.Size(167, 15);
			this.minOnIntensityLabel.TabIndex = 2;
			this.minOnIntensityLabel.Text = "Minium \'ON\' Intensity (0-255):";
			// 
			// minIntensityLabel
			// 
			this.minIntensityLabel.AutoSize = true;
			this.minIntensityLabel.Location = new System.Drawing.Point(147, 77);
			this.minIntensityLabel.Name = "minIntensityLabel";
			this.minIntensityLabel.Size = new System.Drawing.Size(13, 15);
			this.minIntensityLabel.TabIndex = 1;
			this.minIntensityLabel.Text = "1";
			// 
			// sliderMinIntensityTrackBar
			// 
			this.sliderMinIntensityTrackBar.LargeChange = 16;
			this.sliderMinIntensityTrackBar.Location = new System.Drawing.Point(13, 40);
			this.sliderMinIntensityTrackBar.Maximum = 255;
			this.sliderMinIntensityTrackBar.Name = "sliderMinIntensityTrackBar";
			this.sliderMinIntensityTrackBar.Size = new System.Drawing.Size(296, 45);
			this.sliderMinIntensityTrackBar.TabIndex = 0;
			this.sliderMinIntensityTrackBar.TickFrequency = 16;
			this.sliderMinIntensityTrackBar.ValueChanged += new System.EventHandler(this.sliderMinIntensityTrackBar_ValueChanged);
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(164, 227);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(87, 27);
			this.okButton.TabIndex = 2;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			this.okButton.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.okButton.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// cancelButton
			// 
			this.cancelButton.Location = new System.Drawing.Point(260, 226);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(87, 27);
			this.cancelButton.TabIndex = 3;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.cancelButton.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// SetupDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(358, 275);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.ethernetSettingsGroupBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(374, 313);
			this.Name = "SetupDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Elexol USB I/O 24 Configuration";
			this.ethernetSettingsGroupBox.ResumeLayout(false);
			this.ethernetSettingsGroupBox.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.sliderMinIntensityTrackBar)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox ethernetSettingsGroupBox;
		private System.Windows.Forms.Button testButton;
		private System.Windows.Forms.TextBox ipAddressTextBox;
		private System.Windows.Forms.Label ipaddressLabel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label minOnIntensityLabel;
		private System.Windows.Forms.Label minIntensityLabel;
		private System.Windows.Forms.TrackBar sliderMinIntensityTrackBar;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.TextBox portTextBox;
		private System.Windows.Forms.Label portLabel;
	}
}