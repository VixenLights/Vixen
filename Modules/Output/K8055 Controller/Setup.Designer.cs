namespace VixenModules.Output.K8055_Controller
{
	partial class Setup
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.driverVersionButton = new System.Windows.Forms.Button();
			this.searchDevicesButton = new System.Windows.Forms.Button();
			this.checkBoxDev3 = new System.Windows.Forms.CheckBox();
			this.checkBoxDev2 = new System.Windows.Forms.CheckBox();
			this.checkBoxDev1 = new System.Windows.Forms.CheckBox();
			this.checkBoxDev0 = new System.Windows.Forms.CheckBox();
			this.labelDev3 = new System.Windows.Forms.Label();
			this.labelDev2 = new System.Windows.Forms.Label();
			this.labelDev1 = new System.Windows.Forms.Label();
			this.labelDev0 = new System.Windows.Forms.Label();
			this.numericUpDownDev3 = new System.Windows.Forms.NumericUpDown();
			this.numericUpDownDev2 = new System.Windows.Forms.NumericUpDown();
			this.numericUpDownDev1 = new System.Windows.Forms.NumericUpDown();
			this.numericUpDownDev0 = new System.Windows.Forms.NumericUpDown();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownDev3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownDev2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownDev1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownDev0)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.driverVersionButton);
			this.groupBox1.Controls.Add(this.searchDevicesButton);
			this.groupBox1.Controls.Add(this.checkBoxDev3);
			this.groupBox1.Controls.Add(this.checkBoxDev2);
			this.groupBox1.Controls.Add(this.checkBoxDev1);
			this.groupBox1.Controls.Add(this.checkBoxDev0);
			this.groupBox1.Controls.Add(this.labelDev3);
			this.groupBox1.Controls.Add(this.labelDev2);
			this.groupBox1.Controls.Add(this.labelDev1);
			this.groupBox1.Controls.Add(this.labelDev0);
			this.groupBox1.Controls.Add(this.numericUpDownDev3);
			this.groupBox1.Controls.Add(this.numericUpDownDev2);
			this.groupBox1.Controls.Add(this.numericUpDownDev1);
			this.groupBox1.Controls.Add(this.numericUpDownDev0);
			this.groupBox1.Location = new System.Drawing.Point(14, 14);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(392, 198);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Velleman K8805";
			this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// driverVersionButton
			// 
			this.driverVersionButton.Location = new System.Drawing.Point(232, 157);
			this.driverVersionButton.Name = "driverVersionButton";
			this.driverVersionButton.Size = new System.Drawing.Size(135, 27);
			this.driverVersionButton.TabIndex = 9;
			this.driverVersionButton.Text = "K8055 driver version";
			this.driverVersionButton.UseVisualStyleBackColor = true;
			this.driverVersionButton.Click += new System.EventHandler(this.driverVersionButton_Click);
			this.driverVersionButton.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.driverVersionButton.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// searchDevicesButton
			// 
			this.searchDevicesButton.Location = new System.Drawing.Point(72, 157);
			this.searchDevicesButton.Name = "searchDevicesButton";
			this.searchDevicesButton.Size = new System.Drawing.Size(135, 27);
			this.searchDevicesButton.TabIndex = 8;
			this.searchDevicesButton.Text = "Search Devices";
			this.searchDevicesButton.UseVisualStyleBackColor = true;
			this.searchDevicesButton.Click += new System.EventHandler(this.searchDevicesButton_Click);
			this.searchDevicesButton.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.searchDevicesButton.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// checkBoxDev3
			// 
			this.checkBoxDev3.AutoSize = true;
			this.checkBoxDev3.Location = new System.Drawing.Point(20, 112);
			this.checkBoxDev3.Name = "checkBoxDev3";
			this.checkBoxDev3.Size = new System.Drawing.Size(225, 19);
			this.checkBoxDev3.TabIndex = 6;
			this.checkBoxDev3.Text = "Device Address 3 will handle channels";
			this.checkBoxDev3.UseVisualStyleBackColor = true;
			// 
			// checkBoxDev2
			// 
			this.checkBoxDev2.AutoSize = true;
			this.checkBoxDev2.Location = new System.Drawing.Point(20, 84);
			this.checkBoxDev2.Name = "checkBoxDev2";
			this.checkBoxDev2.Size = new System.Drawing.Size(225, 19);
			this.checkBoxDev2.TabIndex = 4;
			this.checkBoxDev2.Text = "Device Address 2 will handle channels";
			this.checkBoxDev2.UseVisualStyleBackColor = true;
			// 
			// checkBoxDev1
			// 
			this.checkBoxDev1.AutoSize = true;
			this.checkBoxDev1.Location = new System.Drawing.Point(20, 53);
			this.checkBoxDev1.Name = "checkBoxDev1";
			this.checkBoxDev1.Size = new System.Drawing.Size(225, 19);
			this.checkBoxDev1.TabIndex = 2;
			this.checkBoxDev1.Text = "Device Address 1 will handle channels";
			this.checkBoxDev1.UseVisualStyleBackColor = true;
			// 
			// checkBoxDev0
			// 
			this.checkBoxDev0.AutoSize = true;
			this.checkBoxDev0.Location = new System.Drawing.Point(20, 24);
			this.checkBoxDev0.Name = "checkBoxDev0";
			this.checkBoxDev0.Size = new System.Drawing.Size(225, 19);
			this.checkBoxDev0.TabIndex = 0;
			this.checkBoxDev0.Text = "Device Address 0 will handle channels";
			this.checkBoxDev0.UseVisualStyleBackColor = true;
			// 
			// labelDev3
			// 
			this.labelDev3.AutoSize = true;
			this.labelDev3.Location = new System.Drawing.Point(348, 115);
			this.labelDev3.Name = "labelDev3";
			this.labelDev3.Size = new System.Drawing.Size(33, 15);
			this.labelDev3.TabIndex = 7;
			this.labelDev3.Text = "to 32";
			// 
			// labelDev2
			// 
			this.labelDev2.AutoSize = true;
			this.labelDev2.Location = new System.Drawing.Point(348, 85);
			this.labelDev2.Name = "labelDev2";
			this.labelDev2.Size = new System.Drawing.Size(33, 15);
			this.labelDev2.TabIndex = 6;
			this.labelDev2.Text = "to 24";
			// 
			// labelDev1
			// 
			this.labelDev1.AutoSize = true;
			this.labelDev1.Location = new System.Drawing.Point(348, 55);
			this.labelDev1.Name = "labelDev1";
			this.labelDev1.Size = new System.Drawing.Size(33, 15);
			this.labelDev1.TabIndex = 5;
			this.labelDev1.Text = "to 16";
			// 
			// labelDev0
			// 
			this.labelDev0.AutoSize = true;
			this.labelDev0.Location = new System.Drawing.Point(348, 25);
			this.labelDev0.Name = "labelDev0";
			this.labelDev0.Size = new System.Drawing.Size(27, 15);
			this.labelDev0.TabIndex = 4;
			this.labelDev0.Text = "to 8";
			// 
			// numericUpDownDev3
			// 
			this.numericUpDownDev3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.numericUpDownDev3.Location = new System.Drawing.Point(269, 113);
			this.numericUpDownDev3.Name = "numericUpDownDev3";
			this.numericUpDownDev3.Size = new System.Drawing.Size(71, 23);
			this.numericUpDownDev3.TabIndex = 7;
			this.numericUpDownDev3.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
			this.numericUpDownDev3.ValueChanged += new System.EventHandler(this.numericUpDownDev3_ValueChanged);
			// 
			// numericUpDownDev2
			// 
			this.numericUpDownDev2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.numericUpDownDev2.Location = new System.Drawing.Point(269, 83);
			this.numericUpDownDev2.Name = "numericUpDownDev2";
			this.numericUpDownDev2.Size = new System.Drawing.Size(71, 23);
			this.numericUpDownDev2.TabIndex = 5;
			this.numericUpDownDev2.Value = new decimal(new int[] {
            17,
            0,
            0,
            0});
			this.numericUpDownDev2.ValueChanged += new System.EventHandler(this.numericUpDownDev2_ValueChanged);
			// 
			// numericUpDownDev1
			// 
			this.numericUpDownDev1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.numericUpDownDev1.Location = new System.Drawing.Point(269, 53);
			this.numericUpDownDev1.Name = "numericUpDownDev1";
			this.numericUpDownDev1.Size = new System.Drawing.Size(71, 23);
			this.numericUpDownDev1.TabIndex = 3;
			this.numericUpDownDev1.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
			this.numericUpDownDev1.ValueChanged += new System.EventHandler(this.numericUpDownDev1_ValueChanged);
			// 
			// numericUpDownDev0
			// 
			this.numericUpDownDev0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.numericUpDownDev0.Location = new System.Drawing.Point(269, 23);
			this.numericUpDownDev0.Name = "numericUpDownDev0";
			this.numericUpDownDev0.Size = new System.Drawing.Size(71, 23);
			this.numericUpDownDev0.TabIndex = 1;
			this.numericUpDownDev0.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownDev0.ValueChanged += new System.EventHandler(this.numericUpDownDev0_ValueChanged);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(318, 232);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(87, 27);
			this.cancelButton.TabIndex = 2;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.cancelButton.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(212, 232);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(87, 27);
			this.okButton.TabIndex = 1;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			this.okButton.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.okButton.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// Setup
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(427, 286);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(443, 324);
			this.Name = "Setup";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "K8055 Configuration";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownDev3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownDev2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownDev1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownDev0)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button driverVersionButton;
		private System.Windows.Forms.Button searchDevicesButton;
		private System.Windows.Forms.CheckBox checkBoxDev3;
		private System.Windows.Forms.CheckBox checkBoxDev2;
		private System.Windows.Forms.CheckBox checkBoxDev1;
		private System.Windows.Forms.CheckBox checkBoxDev0;
		private System.Windows.Forms.Label labelDev3;
		private System.Windows.Forms.Label labelDev2;
		private System.Windows.Forms.Label labelDev1;
		private System.Windows.Forms.Label labelDev0;
		private System.Windows.Forms.NumericUpDown numericUpDownDev3;
		private System.Windows.Forms.NumericUpDown numericUpDownDev2;
		private System.Windows.Forms.NumericUpDown numericUpDownDev1;
		private System.Windows.Forms.NumericUpDown numericUpDownDev0;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
	}
}