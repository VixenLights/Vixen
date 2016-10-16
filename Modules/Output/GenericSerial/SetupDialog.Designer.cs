namespace VixenModules.Output.GenericSerial
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupDialog));
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOkay = new System.Windows.Forms.Button();
			this.gbPacketData = new System.Windows.Forms.GroupBox();
			this.tbFooter = new System.Windows.Forms.TextBox();
			this.cbFooter = new System.Windows.Forms.CheckBox();
			this.tbHeader = new System.Windows.Forms.TextBox();
			this.cbHeader = new System.Windows.Forms.CheckBox();
			this.btnPortSetup = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lblSettings = new System.Windows.Forms.Label();
			this.lblSettingsLbl = new System.Windows.Forms.Label();
			this.gbPacketData.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(257, 269);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(87, 27);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.btnCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// btnOkay
			// 
			this.btnOkay.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOkay.Location = new System.Drawing.Point(162, 269);
			this.btnOkay.Name = "btnOkay";
			this.btnOkay.Size = new System.Drawing.Size(87, 27);
			this.btnOkay.TabIndex = 2;
			this.btnOkay.Text = "OK";
			this.btnOkay.UseVisualStyleBackColor = true;
			this.btnOkay.Click += new System.EventHandler(this.btnOkay_Click);
			this.btnOkay.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.btnOkay.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// gbPacketData
			// 
			this.gbPacketData.Controls.Add(this.tbFooter);
			this.gbPacketData.Controls.Add(this.cbFooter);
			this.gbPacketData.Controls.Add(this.tbHeader);
			this.gbPacketData.Controls.Add(this.cbHeader);
			this.gbPacketData.Location = new System.Drawing.Point(16, 102);
			this.gbPacketData.Name = "gbPacketData";
			this.gbPacketData.Size = new System.Drawing.Size(328, 160);
			this.gbPacketData.TabIndex = 1;
			this.gbPacketData.TabStop = false;
			this.gbPacketData.Text = "Packet Data";
			this.gbPacketData.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// tbFooter
			// 
			this.tbFooter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbFooter.Location = new System.Drawing.Point(29, 121);
			this.tbFooter.Name = "tbFooter";
			this.tbFooter.Size = new System.Drawing.Size(291, 23);
			this.tbFooter.TabIndex = 3;
			// 
			// cbFooter
			// 
			this.cbFooter.AutoSize = true;
			this.cbFooter.Location = new System.Drawing.Point(8, 95);
			this.cbFooter.Name = "cbFooter";
			this.cbFooter.Size = new System.Drawing.Size(118, 19);
			this.cbFooter.TabIndex = 2;
			this.cbFooter.Text = "Send a text footer";
			this.cbFooter.UseVisualStyleBackColor = true;
			this.cbFooter.CheckedChanged += new System.EventHandler(this.cbFooter_CheckedChanged);
			// 
			// tbHeader
			// 
			this.tbHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbHeader.Location = new System.Drawing.Point(29, 51);
			this.tbHeader.Name = "tbHeader";
			this.tbHeader.Size = new System.Drawing.Size(291, 23);
			this.tbHeader.TabIndex = 1;
			// 
			// cbHeader
			// 
			this.cbHeader.AutoSize = true;
			this.cbHeader.Location = new System.Drawing.Point(8, 23);
			this.cbHeader.Name = "cbHeader";
			this.cbHeader.Size = new System.Drawing.Size(122, 19);
			this.cbHeader.TabIndex = 0;
			this.cbHeader.Text = "Send a text header";
			this.cbHeader.UseVisualStyleBackColor = true;
			this.cbHeader.CheckedChanged += new System.EventHandler(this.cbHeader_CheckedChanged);
			// 
			// btnPortSetup
			// 
			this.btnPortSetup.Location = new System.Drawing.Point(139, 42);
			this.btnPortSetup.Name = "btnPortSetup";
			this.btnPortSetup.Size = new System.Drawing.Size(182, 27);
			this.btnPortSetup.TabIndex = 0;
			this.btnPortSetup.Text = "Setup/Change Serial Port";
			this.btnPortSetup.UseVisualStyleBackColor = true;
			this.btnPortSetup.Click += new System.EventHandler(this.btnPortSetup_Click);
			this.btnPortSetup.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.btnPortSetup.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.lblSettings);
			this.groupBox1.Controls.Add(this.lblSettingsLbl);
			this.groupBox1.Controls.Add(this.btnPortSetup);
			this.groupBox1.Location = new System.Drawing.Point(16, 14);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(328, 81);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Serial Port Settings";
			this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// lblSettings
			// 
			this.lblSettings.AutoSize = true;
			this.lblSettings.Location = new System.Drawing.Point(75, 23);
			this.lblSettings.Name = "lblSettings";
			this.lblSettings.Size = new System.Drawing.Size(209, 15);
			this.lblSettings.TabIndex = 1;
			this.lblSettings.Text = "COM1: 115200, Space, 8, OnePointFive";
			// 
			// lblSettingsLbl
			// 
			this.lblSettingsLbl.AutoSize = true;
			this.lblSettingsLbl.Location = new System.Drawing.Point(8, 23);
			this.lblSettingsLbl.Name = "lblSettingsLbl";
			this.lblSettingsLbl.Size = new System.Drawing.Size(59, 15);
			this.lblSettingsLbl.TabIndex = 4;
			this.lblSettingsLbl.Text = "Currently:";
			// 
			// SetupDialog
			// 
			this.AcceptButton = this.btnOkay;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(363, 317);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.gbPacketData);
			this.Controls.Add(this.btnOkay);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(379, 355);
			this.Name = "SetupDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Generic Serial Configuration";
			this.gbPacketData.ResumeLayout(false);
			this.gbPacketData.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOkay;
		private System.Windows.Forms.GroupBox gbPacketData;
		private System.Windows.Forms.TextBox tbFooter;
		private System.Windows.Forms.CheckBox cbFooter;
		private System.Windows.Forms.TextBox tbHeader;
		private System.Windows.Forms.CheckBox cbHeader;
		private System.Windows.Forms.Button btnPortSetup;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label lblSettings;
		private System.Windows.Forms.Label lblSettingsLbl;
	}
}