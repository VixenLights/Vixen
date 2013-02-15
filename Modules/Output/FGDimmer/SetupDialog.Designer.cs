namespace VixenModules.Output.FGDimmer
{
    partial class SetupDialog {
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.comboBoxModule4 = new System.Windows.Forms.ComboBox();
			this.checkBoxModule4 = new System.Windows.Forms.CheckBox();
			this.comboBoxModule3 = new System.Windows.Forms.ComboBox();
			this.checkBoxModule3 = new System.Windows.Forms.CheckBox();
			this.comboBoxModule2 = new System.Windows.Forms.ComboBox();
			this.checkBoxModule2 = new System.Windows.Forms.CheckBox();
			this.comboBoxModule1 = new System.Windows.Forms.ComboBox();
			this.checkBoxModule1 = new System.Windows.Forms.CheckBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.checkBoxHoldPort = new System.Windows.Forms.CheckBox();
			this.buttonSerialSetup = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.radioButtonAC = new System.Windows.Forms.RadioButton();
			this.radioButtonPWM = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.comboBoxModule4);
			this.groupBox1.Controls.Add(this.checkBoxModule4);
			this.groupBox1.Controls.Add(this.comboBoxModule3);
			this.groupBox1.Controls.Add(this.checkBoxModule3);
			this.groupBox1.Controls.Add(this.comboBoxModule2);
			this.groupBox1.Controls.Add(this.checkBoxModule2);
			this.groupBox1.Controls.Add(this.comboBoxModule1);
			this.groupBox1.Controls.Add(this.checkBoxModule1);
			this.groupBox1.Location = new System.Drawing.Point(16, 145);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox1.Size = new System.Drawing.Size(380, 172);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Modules";
			// 
			// comboBoxModule4
			// 
			this.comboBoxModule4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxModule4.FormattingEnabled = true;
			this.comboBoxModule4.Location = new System.Drawing.Point(288, 121);
			this.comboBoxModule4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.comboBoxModule4.Name = "comboBoxModule4";
			this.comboBoxModule4.Size = new System.Drawing.Size(73, 24);
			this.comboBoxModule4.TabIndex = 7;
			// 
			// checkBoxModule4
			// 
			this.checkBoxModule4.AutoSize = true;
			this.checkBoxModule4.Location = new System.Drawing.Point(24, 123);
			this.checkBoxModule4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.checkBoxModule4.Name = "checkBoxModule4";
			this.checkBoxModule4.Size = new System.Drawing.Size(256, 21);
			this.checkBoxModule4.TabIndex = 6;
			this.checkBoxModule4.Text = "Using module 4, starting at channel";
			this.checkBoxModule4.UseVisualStyleBackColor = true;
			// 
			// comboBoxModule3
			// 
			this.comboBoxModule3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxModule3.FormattingEnabled = true;
			this.comboBoxModule3.Location = new System.Drawing.Point(288, 92);
			this.comboBoxModule3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.comboBoxModule3.Name = "comboBoxModule3";
			this.comboBoxModule3.Size = new System.Drawing.Size(73, 24);
			this.comboBoxModule3.TabIndex = 5;
			// 
			// checkBoxModule3
			// 
			this.checkBoxModule3.AutoSize = true;
			this.checkBoxModule3.Location = new System.Drawing.Point(24, 95);
			this.checkBoxModule3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.checkBoxModule3.Name = "checkBoxModule3";
			this.checkBoxModule3.Size = new System.Drawing.Size(256, 21);
			this.checkBoxModule3.TabIndex = 4;
			this.checkBoxModule3.Text = "Using module 3, starting at channel";
			this.checkBoxModule3.UseVisualStyleBackColor = true;
			// 
			// comboBoxModule2
			// 
			this.comboBoxModule2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxModule2.FormattingEnabled = true;
			this.comboBoxModule2.Location = new System.Drawing.Point(288, 64);
			this.comboBoxModule2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.comboBoxModule2.Name = "comboBoxModule2";
			this.comboBoxModule2.Size = new System.Drawing.Size(73, 24);
			this.comboBoxModule2.TabIndex = 3;
			// 
			// checkBoxModule2
			// 
			this.checkBoxModule2.AutoSize = true;
			this.checkBoxModule2.Location = new System.Drawing.Point(24, 66);
			this.checkBoxModule2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.checkBoxModule2.Name = "checkBoxModule2";
			this.checkBoxModule2.Size = new System.Drawing.Size(256, 21);
			this.checkBoxModule2.TabIndex = 2;
			this.checkBoxModule2.Text = "Using module 2, starting at channel";
			this.checkBoxModule2.UseVisualStyleBackColor = true;
			// 
			// comboBoxModule1
			// 
			this.comboBoxModule1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxModule1.FormattingEnabled = true;
			this.comboBoxModule1.Location = new System.Drawing.Point(288, 36);
			this.comboBoxModule1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.comboBoxModule1.Name = "comboBoxModule1";
			this.comboBoxModule1.Size = new System.Drawing.Size(73, 24);
			this.comboBoxModule1.TabIndex = 1;
			// 
			// checkBoxModule1
			// 
			this.checkBoxModule1.AutoSize = true;
			this.checkBoxModule1.Location = new System.Drawing.Point(24, 38);
			this.checkBoxModule1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.checkBoxModule1.Name = "checkBoxModule1";
			this.checkBoxModule1.Size = new System.Drawing.Size(256, 21);
			this.checkBoxModule1.TabIndex = 0;
			this.checkBoxModule1.Text = "Using module 1, starting at channel";
			this.checkBoxModule1.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(188, 442);
			this.buttonOK.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(100, 28);
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(296, 442);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(100, 28);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.checkBoxHoldPort);
			this.groupBox2.Controls.Add(this.buttonSerialSetup);
			this.groupBox2.Location = new System.Drawing.Point(16, 15);
			this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox2.Size = new System.Drawing.Size(380, 123);
			this.groupBox2.TabIndex = 0;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Port";
			// 
			// checkBoxHoldPort
			// 
			this.checkBoxHoldPort.Location = new System.Drawing.Point(23, 66);
			this.checkBoxHoldPort.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.checkBoxHoldPort.Name = "checkBoxHoldPort";
			this.checkBoxHoldPort.Size = new System.Drawing.Size(340, 49);
			this.checkBoxHoldPort.TabIndex = 1;
			this.checkBoxHoldPort.Text = "Hold port open during the duration of the sequence execution.";
			this.checkBoxHoldPort.UseVisualStyleBackColor = true;
			// 
			// buttonSerialSetup
			// 
			this.buttonSerialSetup.Location = new System.Drawing.Point(140, 21);
			this.buttonSerialSetup.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.buttonSerialSetup.Name = "buttonSerialSetup";
			this.buttonSerialSetup.Size = new System.Drawing.Size(100, 28);
			this.buttonSerialSetup.TabIndex = 0;
			this.buttonSerialSetup.Text = "Serial Setup";
			this.buttonSerialSetup.UseVisualStyleBackColor = true;
			this.buttonSerialSetup.Click += new System.EventHandler(this.buttonSerialSetup_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.radioButtonAC);
			this.groupBox3.Controls.Add(this.radioButtonPWM);
			this.groupBox3.Location = new System.Drawing.Point(16, 325);
			this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox3.Size = new System.Drawing.Size(380, 110);
			this.groupBox3.TabIndex = 4;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Operation";
			// 
			// radioButtonAC
			// 
			this.radioButtonAC.AutoSize = true;
			this.radioButtonAC.Location = new System.Drawing.Point(44, 69);
			this.radioButtonAC.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.radioButtonAC.Name = "radioButtonAC";
			this.radioButtonAC.Size = new System.Drawing.Size(115, 21);
			this.radioButtonAC.TabIndex = 1;
			this.radioButtonAC.TabStop = true;
			this.radioButtonAC.Text = "AC operation";
			this.radioButtonAC.UseVisualStyleBackColor = true;
			// 
			// radioButtonPWM
			// 
			this.radioButtonPWM.AutoSize = true;
			this.radioButtonPWM.Location = new System.Drawing.Point(44, 33);
			this.radioButtonPWM.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.radioButtonPWM.Name = "radioButtonPWM";
			this.radioButtonPWM.Size = new System.Drawing.Size(132, 21);
			this.radioButtonPWM.TabIndex = 0;
			this.radioButtonPWM.TabStop = true;
			this.radioButtonPWM.Text = "PWM operation";
			this.radioButtonPWM.UseVisualStyleBackColor = true;
			// 
			// SetupDialog
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(412, 485);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "SetupDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Setup";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxModule1;
        private System.Windows.Forms.ComboBox comboBoxModule4;
        private System.Windows.Forms.CheckBox checkBoxModule4;
        private System.Windows.Forms.ComboBox comboBoxModule3;
        private System.Windows.Forms.CheckBox checkBoxModule3;
        private System.Windows.Forms.ComboBox comboBoxModule2;
        private System.Windows.Forms.CheckBox checkBoxModule2;
        private System.Windows.Forms.ComboBox comboBoxModule1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonSerialSetup;
        private System.Windows.Forms.CheckBox checkBoxHoldPort;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioButtonAC;
        private System.Windows.Forms.RadioButton radioButtonPWM;
    }
}