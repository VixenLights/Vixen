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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UnicastForm));
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.ipRadio = new System.Windows.Forms.RadioButton();
			this.networkNameRadio = new System.Windows.Forms.RadioButton();
			this.networkNameTextBox = new System.Windows.Forms.TextBox();
			this.ipTextBox = new VixenModules.Controller.E131.Controls.IpTextBox();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.AutoSize = true;
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(28, 80);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 25);
			this.okButton.TabIndex = 101;
			this.okButton.Text = "&OK";
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			this.okButton.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.okButton.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// cancelButton
			// 
			this.cancelButton.AutoSize = true;
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(117, 80);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 25);
			this.cancelButton.TabIndex = 102;
			this.cancelButton.Text = "&Cancel";
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			this.cancelButton.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.cancelButton.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// ipRadio
			// 
			this.ipRadio.AutoSize = true;
			this.ipRadio.Checked = true;
			this.ipRadio.Location = new System.Drawing.Point(26, 49);
			this.ipRadio.Name = "ipRadio";
			this.ipRadio.Size = new System.Drawing.Size(80, 19);
			this.ipRadio.TabIndex = 104;
			this.ipRadio.TabStop = true;
			this.ipRadio.Text = "IP Address";
			this.ipRadio.UseVisualStyleBackColor = true;
			this.ipRadio.CheckedChanged += new System.EventHandler(this.Radio_CheckedChanged);
			// 
			// networkNameRadio
			// 
			this.networkNameRadio.AutoSize = true;
			this.networkNameRadio.Location = new System.Drawing.Point(103, 49);
			this.networkNameRadio.Name = "networkNameRadio";
			this.networkNameRadio.Size = new System.Drawing.Size(105, 19);
			this.networkNameRadio.TabIndex = 105;
			this.networkNameRadio.Text = "Network Name";
			this.networkNameRadio.UseVisualStyleBackColor = true;
			this.networkNameRadio.CheckedChanged += new System.EventHandler(this.Radio_CheckedChanged);
			// 
			// networkNameTextBox
			// 
			this.networkNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.networkNameTextBox.Enabled = false;
			this.networkNameTextBox.Location = new System.Drawing.Point(45, 18);
			this.networkNameTextBox.Name = "networkNameTextBox";
			this.networkNameTextBox.Size = new System.Drawing.Size(139, 23);
			this.networkNameTextBox.TabIndex = 106;
			this.networkNameTextBox.Text = "myComputer";
			// 
			// ipTextBox
			// 
			this.ipTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ipTextBox.Location = new System.Drawing.Point(45, 18);
			this.ipTextBox.Name = "ipTextBox";
			this.ipTextBox.Size = new System.Drawing.Size(139, 20);
			this.ipTextBox.TabIndex = 103;
			this.ipTextBox.Text = "127.0.0.1";
			this.ipTextBox.TextChanged += new System.EventHandler(this.ipTextBox_TextChanged);
			// 
			// UnicastForm
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(227, 110);
			this.ControlBox = false;
			this.Controls.Add(this.networkNameTextBox);
			this.Controls.Add(this.networkNameRadio);
			this.Controls.Add(this.ipRadio);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.ipTextBox);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UnicastForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "New Unicast Destination";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.UnicastForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        private RadioButton ipRadio;
        private RadioButton networkNameRadio;
        private TextBox networkNameTextBox;

    }
}