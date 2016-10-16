namespace VixenModules.Output.ElexolUSBIO
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
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lblSettings = new System.Windows.Forms.Label();
			this.lblSettingsLbl = new System.Windows.Forms.Label();
			this.buttonPortSetup = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.lblMinIntensity = new System.Windows.Forms.Label();
			this.sliderMinIntensity = new System.Windows.Forms.TrackBar();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sliderMinIntensity)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(152, 228);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(87, 27);
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			this.buttonOK.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOK.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(246, 228);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(87, 27);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 29);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(178, 15);
			this.label2.TabIndex = 5;
			this.label2.Text = "Minimum \'ON\' Intensity (0-255):";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.lblSettings);
			this.groupBox1.Controls.Add(this.lblSettingsLbl);
			this.groupBox1.Controls.Add(this.buttonPortSetup);
			this.groupBox1.Location = new System.Drawing.Point(17, 14);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(315, 74);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Serial Port Settings";
			this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// lblSettings
			// 
			this.lblSettings.AutoSize = true;
			this.lblSettings.Location = new System.Drawing.Point(76, 18);
			this.lblSettings.Name = "lblSettings";
			this.lblSettings.Size = new System.Drawing.Size(209, 15);
			this.lblSettings.TabIndex = 1;
			this.lblSettings.Text = "COM1: 115200, Space, 8, OnePointFive";
			// 
			// lblSettingsLbl
			// 
			this.lblSettingsLbl.AutoSize = true;
			this.lblSettingsLbl.Location = new System.Drawing.Point(9, 18);
			this.lblSettingsLbl.Name = "lblSettingsLbl";
			this.lblSettingsLbl.Size = new System.Drawing.Size(59, 15);
			this.lblSettingsLbl.TabIndex = 6;
			this.lblSettingsLbl.Text = "Currently:";
			// 
			// buttonPortSetup
			// 
			this.buttonPortSetup.Location = new System.Drawing.Point(7, 40);
			this.buttonPortSetup.Name = "buttonPortSetup";
			this.buttonPortSetup.Size = new System.Drawing.Size(166, 27);
			this.buttonPortSetup.TabIndex = 0;
			this.buttonPortSetup.Text = "Setup/Change Serial Port";
			this.buttonPortSetup.UseVisualStyleBackColor = true;
			this.buttonPortSetup.Click += new System.EventHandler(this.buttonPortSetup_Click);
			this.buttonPortSetup.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonPortSetup.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.lblMinIntensity);
			this.groupBox2.Controls.Add(this.sliderMinIntensity);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Location = new System.Drawing.Point(17, 100);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(315, 118);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Pixel";
			this.groupBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// lblMinIntensity
			// 
			this.lblMinIntensity.AutoSize = true;
			this.lblMinIntensity.Location = new System.Drawing.Point(149, 84);
			this.lblMinIntensity.Name = "lblMinIntensity";
			this.lblMinIntensity.Size = new System.Drawing.Size(38, 15);
			this.lblMinIntensity.TabIndex = 7;
			this.lblMinIntensity.Text = "label3";
			// 
			// sliderMinIntensity
			// 
			this.sliderMinIntensity.LargeChange = 16;
			this.sliderMinIntensity.Location = new System.Drawing.Point(16, 47);
			this.sliderMinIntensity.Maximum = 255;
			this.sliderMinIntensity.Minimum = 1;
			this.sliderMinIntensity.Name = "sliderMinIntensity";
			this.sliderMinIntensity.Size = new System.Drawing.Size(290, 45);
			this.sliderMinIntensity.TabIndex = 0;
			this.sliderMinIntensity.TickFrequency = 16;
			this.sliderMinIntensity.Value = 1;
			this.sliderMinIntensity.ValueChanged += new System.EventHandler(this.sliderMinIntensity_ValueChanged);
			// 
			// SetupDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(346, 279);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(362, 317);
			this.Name = "SetupDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Elexol USB I/O 24 Configuration";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.sliderMinIntensity)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TrackBar sliderMinIntensity;
		private System.Windows.Forms.Label lblMinIntensity;
		private System.Windows.Forms.Button buttonPortSetup;
		private System.Windows.Forms.Label lblSettings;
		private System.Windows.Forms.Label lblSettingsLbl;
	}
}