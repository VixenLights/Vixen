﻿namespace Common.Controls {
	partial class SerialPortConfig {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.groupBox = new System.Windows.Forms.GroupBox();
			this.comboBoxBaudRate = new System.Windows.Forms.ComboBox();
			this.comboBoxStopBits = new System.Windows.Forms.ComboBox();
			this.comboBoxParity = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textBoxDataBits = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.comboBoxPortName = new System.Windows.Forms.ComboBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.currentConfiguredPortLabel = new System.Windows.Forms.Label();
			this.configuredPortValueLabel = new System.Windows.Forms.Label();
			this.groupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox
			// 
			this.groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox.Controls.Add(this.comboBoxBaudRate);
			this.groupBox.Controls.Add(this.comboBoxStopBits);
			this.groupBox.Controls.Add(this.comboBoxParity);
			this.groupBox.Controls.Add(this.label2);
			this.groupBox.Controls.Add(this.label5);
			this.groupBox.Controls.Add(this.label3);
			this.groupBox.Controls.Add(this.label4);
			this.groupBox.Controls.Add(this.textBoxDataBits);
			this.groupBox.Controls.Add(this.label1);
			this.groupBox.Controls.Add(this.comboBoxPortName);
			this.groupBox.Location = new System.Drawing.Point(12, 46);
			this.groupBox.Name = "groupBox";
			this.groupBox.Size = new System.Drawing.Size(243, 175);
			this.groupBox.TabIndex = 0;
			this.groupBox.TabStop = false;
			this.groupBox.Text = "Port Details";
			this.groupBox.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// comboBoxBaudRate
			// 
			this.comboBoxBaudRate.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboBoxBaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxBaudRate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.comboBoxBaudRate.FormattingEnabled = true;
			this.comboBoxBaudRate.Items.AddRange(new object[] {
            "2400",
            "4800",
            "9600",
            "19200",
            "38400",
            "57600",
            "115200",
            "230400",
            "460800",
            "921600",
            "1048576"});
			this.comboBoxBaudRate.Location = new System.Drawing.Point(103, 54);
			this.comboBoxBaudRate.Name = "comboBoxBaudRate";
			this.comboBoxBaudRate.Size = new System.Drawing.Size(95, 21);
			this.comboBoxBaudRate.TabIndex = 10;
			this.comboBoxBaudRate.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
			// 
			// comboBoxStopBits
			// 
			this.comboBoxStopBits.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboBoxStopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxStopBits.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.comboBoxStopBits.FormattingEnabled = true;
			this.comboBoxStopBits.Location = new System.Drawing.Point(103, 134);
			this.comboBoxStopBits.Name = "comboBoxStopBits";
			this.comboBoxStopBits.Size = new System.Drawing.Size(95, 21);
			this.comboBoxStopBits.TabIndex = 16;
			this.comboBoxStopBits.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
			// 
			// comboBoxParity
			// 
			this.comboBoxParity.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboBoxParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxParity.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.comboBoxParity.FormattingEnabled = true;
			this.comboBoxParity.Location = new System.Drawing.Point(103, 81);
			this.comboBoxParity.Name = "comboBoxParity";
			this.comboBoxParity.Size = new System.Drawing.Size(95, 21);
			this.comboBoxParity.TabIndex = 12;
			this.comboBoxParity.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(39, 62);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 13);
			this.label2.TabIndex = 9;
			this.label2.Text = "Baud rate";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(39, 89);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(33, 13);
			this.label5.TabIndex = 11;
			this.label5.Text = "Parity";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(39, 115);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(49, 13);
			this.label3.TabIndex = 13;
			this.label3.Text = "Data bits";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(39, 142);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(48, 13);
			this.label4.TabIndex = 15;
			this.label4.Text = "Stop bits";
			// 
			// textBoxDataBits
			// 
			this.textBoxDataBits.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxDataBits.Location = new System.Drawing.Point(103, 108);
			this.textBoxDataBits.MaxLength = 1;
			this.textBoxDataBits.Name = "textBoxDataBits";
			this.textBoxDataBits.Size = new System.Drawing.Size(95, 20);
			this.textBoxDataBits.TabIndex = 14;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(39, 35);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(55, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Port name";
			// 
			// comboBoxPortName
			// 
			this.comboBoxPortName.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboBoxPortName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxPortName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.comboBoxPortName.FormattingEnabled = true;
			this.comboBoxPortName.Location = new System.Drawing.Point(103, 27);
			this.comboBoxPortName.Name = "comboBoxPortName";
			this.comboBoxPortName.Size = new System.Drawing.Size(95, 21);
			this.comboBoxPortName.TabIndex = 0;
			this.comboBoxPortName.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(99, 227);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			this.buttonOK.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOK.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(180, 227);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// currentConfiguredPortLabel
			// 
			this.currentConfiguredPortLabel.AutoSize = true;
			this.currentConfiguredPortLabel.Location = new System.Drawing.Point(12, 18);
			this.currentConfiguredPortLabel.Name = "currentConfiguredPortLabel";
			this.currentConfiguredPortLabel.Size = new System.Drawing.Size(120, 13);
			this.currentConfiguredPortLabel.TabIndex = 3;
			this.currentConfiguredPortLabel.Text = "Current Configured Port:";
			// 
			// configuredPortValueLabel
			// 
			this.configuredPortValueLabel.AutoSize = true;
			this.configuredPortValueLabel.Location = new System.Drawing.Point(139, 18);
			this.configuredPortValueLabel.Name = "configuredPortValueLabel";
			this.configuredPortValueLabel.Size = new System.Drawing.Size(51, 13);
			this.configuredPortValueLabel.TabIndex = 4;
			this.configuredPortValueLabel.Text = "portname";
			// 
			// SerialPortConfig
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(267, 262);
			this.Controls.Add(this.configuredPortValueLabel);
			this.Controls.Add(this.currentConfiguredPortLabel);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.groupBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SerialPortConfig";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Serial Port Config";
			this.groupBox.ResumeLayout(false);
			this.groupBox.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxPortName;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.ComboBox comboBoxBaudRate;
		private System.Windows.Forms.ComboBox comboBoxStopBits;
		private System.Windows.Forms.ComboBox comboBoxParity;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBoxDataBits;
		private System.Windows.Forms.Label currentConfiguredPortLabel;
		private System.Windows.Forms.Label configuredPortValueLabel;
	}
}