namespace VixenModules.Controller.E131
{
    using System.Drawing;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Text;
    using System.Windows.Forms;

    using VixenModules.Controller.E131.Controls;

    public partial class UnicastForm
    {
        private Button cancelButton;

        private IpTextBox ipTextBox;

        private Button okButton;

		private void InitializeComponent()
		{
			ComponentResourceManager resources = new ComponentResourceManager(typeof(UnicastForm));
			okButton = new Button();
			cancelButton = new Button();
			ipRadio = new RadioButton();
			networkNameRadio = new RadioButton();
			networkNameTextBox = new TextBox();
			ipTextBox = new IpTextBox();
			SuspendLayout();
			// 
			// okButton
			// 
			okButton.AutoSize = true;
			okButton.DialogResult = DialogResult.OK;
			okButton.Location = new Point(28, 80);
			okButton.Name = "okButton";
			okButton.Size = new Size(75, 25);
			okButton.TabIndex = 101;
			okButton.Text = "&OK";
			okButton.Click += okButton_Click;
			okButton.MouseLeave += buttonBackground_MouseLeave;
			okButton.MouseHover += buttonBackground_MouseHover;
			// 
			// cancelButton
			// 
			cancelButton.AutoSize = true;
			cancelButton.DialogResult = DialogResult.Cancel;
			cancelButton.Location = new Point(117, 80);
			cancelButton.Name = "cancelButton";
			cancelButton.Size = new Size(75, 25);
			cancelButton.TabIndex = 102;
			cancelButton.Text = "&Cancel";
			cancelButton.Click += cancelButton_Click;
			cancelButton.MouseLeave += buttonBackground_MouseLeave;
			cancelButton.MouseHover += buttonBackground_MouseHover;
			// 
			// ipRadio
			// 
			ipRadio.AutoSize = true;
			ipRadio.Checked = true;
			ipRadio.Location = new Point(26, 49);
			ipRadio.Name = "ipRadio";
			ipRadio.Size = new Size(80, 19);
			ipRadio.TabIndex = 104;
			ipRadio.TabStop = true;
			ipRadio.Text = "IP Address";
			ipRadio.UseVisualStyleBackColor = true;
			ipRadio.CheckedChanged += Radio_CheckedChanged;
			// 
			// networkNameRadio
			// 
			networkNameRadio.AutoSize = true;
			networkNameRadio.Location = new Point(103, 49);
			networkNameRadio.Name = "networkNameRadio";
			networkNameRadio.Size = new Size(105, 19);
			networkNameRadio.TabIndex = 105;
			networkNameRadio.Text = "Network Name";
			networkNameRadio.UseVisualStyleBackColor = true;
			networkNameRadio.CheckedChanged += Radio_CheckedChanged;
			// 
			// networkNameTextBox
			// 
			networkNameTextBox.BorderStyle = BorderStyle.FixedSingle;
			networkNameTextBox.Enabled = false;
			networkNameTextBox.Location = new Point(45, 18);
			networkNameTextBox.Name = "networkNameTextBox";
			networkNameTextBox.Size = new Size(139, 23);
			networkNameTextBox.TabIndex = 106;
			networkNameTextBox.Text = "myComputer";
			// 
			// ipTextBox
			// 
			ipTextBox.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
			ipTextBox.Location = new Point(45, 18);
			ipTextBox.Name = "ipTextBox";
			ipTextBox.Size = new Size(139, 20);
			ipTextBox.TabIndex = 103;
			ipTextBox.Text = "127.0.0.1";
			ipTextBox.TextChanged += ipTextBox_TextChanged;
			// 
			// UnicastForm
			// 
			AcceptButton = okButton;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = true;
			CancelButton = cancelButton;
			ClientSize = new Size(227, 110);
			Controls.Add(networkNameTextBox);
			Controls.Add(networkNameRadio);
			Controls.Add(ipRadio);
			Controls.Add(okButton);
			Controls.Add(cancelButton);
			Controls.Add(ipTextBox);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			Icon = (Icon)resources.GetObject("$this.Icon");
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "UnicastForm";
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.CenterParent;
			Text = "New Unicast Destination";
			TopMost = true;
			Load += UnicastForm_Load;
			ResumeLayout(false);
			PerformLayout();
		}

		private RadioButton ipRadio;
        private RadioButton networkNameRadio;
        private TextBox networkNameTextBox;

    }
}