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
			this.testButton = new System.Windows.Forms.Button();
			this.ipAddressTextBox = new System.Windows.Forms.TextBox();
			this.ipaddressLabel = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.minOnIntensityLabel = new System.Windows.Forms.Label();
			this.minIntensityLabel = new System.Windows.Forms.Label();
			this.sliderMinIntensityTrackBar = new System.Windows.Forms.TrackBar();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.portTextBox = new System.Windows.Forms.TextBox();
			this.portLabel = new System.Windows.Forms.Label();
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
			this.ethernetSettingsGroupBox.Location = new System.Drawing.Point(13, 13);
			this.ethernetSettingsGroupBox.Name = "ethernetSettingsGroupBox";
			this.ethernetSettingsGroupBox.Size = new System.Drawing.Size(278, 79);
			this.ethernetSettingsGroupBox.TabIndex = 0;
			this.ethernetSettingsGroupBox.TabStop = false;
			this.ethernetSettingsGroupBox.Text = "Ethernet Settings";
			// 
			// testButton
			// 
			this.testButton.Location = new System.Drawing.Point(197, 18);
			this.testButton.Name = "testButton";
			this.testButton.Size = new System.Drawing.Size(75, 48);
			this.testButton.TabIndex = 2;
			this.testButton.Text = "Test";
			this.testButton.UseVisualStyleBackColor = true;
			this.testButton.Click += new System.EventHandler(this.testButton_Click);
			// 
			// ipAddressTextBox
			// 
			this.ipAddressTextBox.Location = new System.Drawing.Point(72, 20);
			this.ipAddressTextBox.Name = "ipAddressTextBox";
			this.ipAddressTextBox.Size = new System.Drawing.Size(112, 20);
			this.ipAddressTextBox.TabIndex = 1;
			this.ipAddressTextBox.Text = "10.10.10.10";
			// 
			// ipaddressLabel
			// 
			this.ipaddressLabel.AutoSize = true;
			this.ipaddressLabel.Location = new System.Drawing.Point(8, 23);
			this.ipaddressLabel.Name = "ipaddressLabel";
			this.ipaddressLabel.Size = new System.Drawing.Size(58, 13);
			this.ipaddressLabel.TabIndex = 0;
			this.ipaddressLabel.Text = "IP Address";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.minOnIntensityLabel);
			this.groupBox1.Controls.Add(this.minIntensityLabel);
			this.groupBox1.Controls.Add(this.sliderMinIntensityTrackBar);
			this.groupBox1.Location = new System.Drawing.Point(12, 98);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(278, 91);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Advanced";
			// 
			// minOnIntensityLabel
			// 
			this.minOnIntensityLabel.AutoSize = true;
			this.minOnIntensityLabel.Location = new System.Drawing.Point(11, 16);
			this.minOnIntensityLabel.Name = "minOnIntensityLabel";
			this.minOnIntensityLabel.Size = new System.Drawing.Size(144, 13);
			this.minOnIntensityLabel.TabIndex = 2;
			this.minOnIntensityLabel.Text = "Minium \'ON\' Intensity (0-255):";
			// 
			// minIntensityLabel
			// 
			this.minIntensityLabel.AutoSize = true;
			this.minIntensityLabel.Location = new System.Drawing.Point(126, 67);
			this.minIntensityLabel.Name = "minIntensityLabel";
			this.minIntensityLabel.Size = new System.Drawing.Size(13, 13);
			this.minIntensityLabel.TabIndex = 1;
			this.minIntensityLabel.Text = "1";
			// 
			// sliderMinIntensityTrackBar
			// 
			this.sliderMinIntensityTrackBar.LargeChange = 16;
			this.sliderMinIntensityTrackBar.Location = new System.Drawing.Point(11, 35);
			this.sliderMinIntensityTrackBar.Maximum = 255;
			this.sliderMinIntensityTrackBar.Name = "sliderMinIntensityTrackBar";
			this.sliderMinIntensityTrackBar.Size = new System.Drawing.Size(254, 45);
			this.sliderMinIntensityTrackBar.TabIndex = 0;
			this.sliderMinIntensityTrackBar.TickFrequency = 16;
			this.sliderMinIntensityTrackBar.ValueChanged += new System.EventHandler(this.sliderMinIntensityTrackBar_ValueChanged);
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(10, 196);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 2;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.Location = new System.Drawing.Point(92, 195);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 3;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// portTextBox
			// 
			this.portTextBox.Location = new System.Drawing.Point(72, 46);
			this.portTextBox.Name = "portTextBox";
			this.portTextBox.Size = new System.Drawing.Size(112, 20);
			this.portTextBox.TabIndex = 4;
			this.portTextBox.Text = "2424";
			// 
			// portLabel
			// 
			this.portLabel.AutoSize = true;
			this.portLabel.Location = new System.Drawing.Point(40, 49);
			this.portLabel.Name = "portLabel";
			this.portLabel.Size = new System.Drawing.Size(26, 13);
			this.portLabel.TabIndex = 3;
			this.portLabel.Text = "Port";
			// 
			// SetupDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(303, 226);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.ethernetSettingsGroupBox);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SetupDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Elexol USB I/O 24 - Setup";
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