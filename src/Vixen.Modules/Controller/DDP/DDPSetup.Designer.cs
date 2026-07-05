namespace VixenModules.Output.DDP
{
	partial class DDPSetup
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
			if (disposing && (components != null)) {
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonIPAddress = new System.Windows.Forms.RadioButton();
            this.radioButtonHostName = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxIPAddress = new System.Windows.Forms.TextBox();
            this.textBoxHostName = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            //
            // buttonCancel
            //
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(295, 178);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(106, 39);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            //
            // buttonOK
            //
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(182, 178);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(106, 39);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            //
            // groupBox1
            //
            this.groupBox1.Controls.Add(this.radioButtonIPAddress);
            this.groupBox1.Controls.Add(this.radioButtonHostName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBoxIPAddress);
            this.groupBox1.Controls.Add(this.textBoxHostName);
            this.groupBox1.Location = new System.Drawing.Point(16, 19);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(385, 130);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Configuration";
            this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
            //
            // radioButtonIPAddress
            //
            this.radioButtonIPAddress.AutoSize = true;
            this.radioButtonIPAddress.Checked = true;
            this.radioButtonIPAddress.Location = new System.Drawing.Point(24, 24);
            this.radioButtonIPAddress.Name = "radioButtonIPAddress";
            this.radioButtonIPAddress.Size = new System.Drawing.Size(94, 21);
            this.radioButtonIPAddress.TabIndex = 0;
            this.radioButtonIPAddress.TabStop = true;
            this.radioButtonIPAddress.Text = "IP Address";
            this.radioButtonIPAddress.UseVisualStyleBackColor = true;
            this.radioButtonIPAddress.CheckedChanged += new System.EventHandler(this.Radio_CheckedChanged);
            //
            // radioButtonHostName
            //
            this.radioButtonHostName.AutoSize = true;
            this.radioButtonHostName.Location = new System.Drawing.Point(140, 24);
            this.radioButtonHostName.Name = "radioButtonHostName";
            this.radioButtonHostName.Size = new System.Drawing.Size(94, 21);
            this.radioButtonHostName.TabIndex = 1;
            this.radioButtonHostName.Text = "Host Name";
            this.radioButtonHostName.UseVisualStyleBackColor = true;
            this.radioButtonHostName.CheckedChanged += new System.EventHandler(this.Radio_CheckedChanged);
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 20);
            this.label1.TabIndex = 10;
            this.label1.Text = "IP Address:";
            //
            // textBoxIPAddress
            //
            this.textBoxIPAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxIPAddress.Location = new System.Drawing.Point(138, 69);
            this.textBoxIPAddress.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxIPAddress.Name = "textBoxIPAddress";
            this.textBoxIPAddress.Size = new System.Drawing.Size(160, 27);
            this.textBoxIPAddress.TabIndex = 2;
            //
            // textBoxHostName
            //
            this.textBoxHostName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxHostName.Enabled = false;
            this.textBoxHostName.Location = new System.Drawing.Point(138, 69);
            this.textBoxHostName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxHostName.Name = "textBoxHostName";
            this.textBoxHostName.Size = new System.Drawing.Size(160, 27);
            this.textBoxHostName.TabIndex = 3;
            this.textBoxHostName.Text = "myhost.local";
            //
            // DDPSetup
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(421, 230);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(439, 277);
            this.Name = "DDPSetup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DDP Configuration";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonIPAddress;
		private System.Windows.Forms.RadioButton radioButtonHostName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxIPAddress;
		private System.Windows.Forms.TextBox textBoxHostName;
	}
}
