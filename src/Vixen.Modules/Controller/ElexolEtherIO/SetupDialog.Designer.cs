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
			ethernetSettingsGroupBox = new System.Windows.Forms.GroupBox();
			portTextBox = new System.Windows.Forms.TextBox();
			portLabel = new System.Windows.Forms.Label();
			testButton = new System.Windows.Forms.Button();
			ipAddressTextBox = new System.Windows.Forms.TextBox();
			ipaddressLabel = new System.Windows.Forms.Label();
			groupBox1 = new System.Windows.Forms.GroupBox();
			minOnIntensityLabel = new System.Windows.Forms.Label();
			minIntensityLabel = new System.Windows.Forms.Label();
			sliderMinIntensityTrackBar = new System.Windows.Forms.TrackBar();
			okButton = new System.Windows.Forms.Button();
			cancelButton = new System.Windows.Forms.Button();
			ethernetSettingsGroupBox.SuspendLayout();
			groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)sliderMinIntensityTrackBar).BeginInit();
			SuspendLayout();
			// 
			// ethernetSettingsGroupBox
			// 
			ethernetSettingsGroupBox.Controls.Add(portTextBox);
			ethernetSettingsGroupBox.Controls.Add(portLabel);
			ethernetSettingsGroupBox.Controls.Add(testButton);
			ethernetSettingsGroupBox.Controls.Add(ipAddressTextBox);
			ethernetSettingsGroupBox.Controls.Add(ipaddressLabel);
			ethernetSettingsGroupBox.Location = new System.Drawing.Point(15, 15);
			ethernetSettingsGroupBox.Name = "ethernetSettingsGroupBox";
			ethernetSettingsGroupBox.Size = new System.Drawing.Size(324, 91);
			ethernetSettingsGroupBox.TabIndex = 0;
			ethernetSettingsGroupBox.TabStop = false;
			ethernetSettingsGroupBox.Text = "Ethernet Settings";
			ethernetSettingsGroupBox.Paint += groupBoxes_Paint;
			// 
			// portTextBox
			// 
			portTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			portTextBox.Location = new System.Drawing.Point(84, 53);
			portTextBox.Name = "portTextBox";
			portTextBox.Size = new System.Drawing.Size(130, 23);
			portTextBox.TabIndex = 1;
			portTextBox.Text = "2424";
			// 
			// portLabel
			// 
			portLabel.AutoSize = true;
			portLabel.Location = new System.Drawing.Point(47, 57);
			portLabel.Name = "portLabel";
			portLabel.Size = new System.Drawing.Size(29, 15);
			portLabel.TabIndex = 3;
			portLabel.Text = "Port";
			// 
			// testButton
			// 
			testButton.Location = new System.Drawing.Point(230, 21);
			testButton.Name = "testButton";
			testButton.Size = new System.Drawing.Size(87, 55);
			testButton.TabIndex = 2;
			testButton.Text = "Test";
			testButton.UseVisualStyleBackColor = true;
			testButton.Click += testButton_Click;
			testButton.MouseLeave += buttonBackground_MouseLeave;
			testButton.MouseHover += buttonBackground_MouseHover;
			// 
			// ipAddressTextBox
			// 
			ipAddressTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			ipAddressTextBox.Location = new System.Drawing.Point(84, 23);
			ipAddressTextBox.Name = "ipAddressTextBox";
			ipAddressTextBox.Size = new System.Drawing.Size(130, 23);
			ipAddressTextBox.TabIndex = 0;
			ipAddressTextBox.Text = "10.10.10.10";
			// 
			// ipaddressLabel
			// 
			ipaddressLabel.AutoSize = true;
			ipaddressLabel.Location = new System.Drawing.Point(9, 27);
			ipaddressLabel.Name = "ipaddressLabel";
			ipaddressLabel.Size = new System.Drawing.Size(62, 15);
			ipaddressLabel.TabIndex = 0;
			ipaddressLabel.Text = "IP Address";
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(minOnIntensityLabel);
			groupBox1.Controls.Add(minIntensityLabel);
			groupBox1.Controls.Add(sliderMinIntensityTrackBar);
			groupBox1.Location = new System.Drawing.Point(14, 113);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(324, 105);
			groupBox1.TabIndex = 1;
			groupBox1.TabStop = false;
			groupBox1.Text = "Advanced";
			groupBox1.Paint += groupBoxes_Paint;
			// 
			// minOnIntensityLabel
			// 
			minOnIntensityLabel.AutoSize = true;
			minOnIntensityLabel.Location = new System.Drawing.Point(13, 18);
			minOnIntensityLabel.Name = "minOnIntensityLabel";
			minOnIntensityLabel.Size = new System.Drawing.Size(167, 15);
			minOnIntensityLabel.TabIndex = 2;
			minOnIntensityLabel.Text = "Minium 'ON' Intensity (0-255):";
			// 
			// minIntensityLabel
			// 
			minIntensityLabel.AutoSize = true;
			minIntensityLabel.Location = new System.Drawing.Point(147, 77);
			minIntensityLabel.Name = "minIntensityLabel";
			minIntensityLabel.Size = new System.Drawing.Size(13, 15);
			minIntensityLabel.TabIndex = 1;
			minIntensityLabel.Text = "1";
			// 
			// sliderMinIntensityTrackBar
			// 
			sliderMinIntensityTrackBar.LargeChange = 16;
			sliderMinIntensityTrackBar.Location = new System.Drawing.Point(13, 40);
			sliderMinIntensityTrackBar.Maximum = 255;
			sliderMinIntensityTrackBar.Name = "sliderMinIntensityTrackBar";
			sliderMinIntensityTrackBar.Size = new System.Drawing.Size(296, 45);
			sliderMinIntensityTrackBar.TabIndex = 0;
			sliderMinIntensityTrackBar.TickFrequency = 16;
			sliderMinIntensityTrackBar.ValueChanged += sliderMinIntensityTrackBar_ValueChanged;
			// 
			// okButton
			// 
			okButton.Location = new System.Drawing.Point(164, 227);
			okButton.Name = "okButton";
			okButton.Size = new System.Drawing.Size(87, 27);
			okButton.TabIndex = 2;
			okButton.Text = "OK";
			okButton.UseVisualStyleBackColor = true;
			okButton.Click += okButton_Click;
			okButton.MouseLeave += buttonBackground_MouseLeave;
			okButton.MouseHover += buttonBackground_MouseHover;
			// 
			// cancelButton
			// 
			cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			cancelButton.Location = new System.Drawing.Point(260, 226);
			cancelButton.Name = "cancelButton";
			cancelButton.Size = new System.Drawing.Size(87, 27);
			cancelButton.TabIndex = 3;
			cancelButton.Text = "Cancel";
			cancelButton.UseVisualStyleBackColor = true;
			cancelButton.MouseLeave += buttonBackground_MouseLeave;
			cancelButton.MouseHover += buttonBackground_MouseHover;
			// 
			// SetupDialog
			// 
			AcceptButton = okButton;
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			AutoSize = true;
			CancelButton = cancelButton;
			ClientSize = new System.Drawing.Size(358, 275);
			Controls.Add(cancelButton);
			Controls.Add(okButton);
			Controls.Add(groupBox1);
			Controls.Add(ethernetSettingsGroupBox);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			MinimumSize = new System.Drawing.Size(374, 313);
			Name = "SetupDialog";
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Elexol USB I/O 24 Configuration";
			ethernetSettingsGroupBox.ResumeLayout(false);
			ethernetSettingsGroupBox.PerformLayout();
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)sliderMinIntensityTrackBar).EndInit();
			ResumeLayout(false);
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