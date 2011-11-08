namespace VixenModules.Property.RGB
{
	partial class RGBSetup
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
			this.radioButtonSingleChannel = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.radioButtonMultiChannel = new System.Windows.Forms.RadioButton();
			this.groupBoxComponents = new System.Windows.Forms.GroupBox();
			this.comboBoxB = new System.Windows.Forms.ComboBox();
			this.comboBoxG = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.comboBoxR = new System.Windows.Forms.ComboBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBoxComponents.SuspendLayout();
			this.SuspendLayout();
			// 
			// radioButtonSingleChannel
			// 
			this.radioButtonSingleChannel.AutoSize = true;
			this.radioButtonSingleChannel.Location = new System.Drawing.Point(23, 45);
			this.radioButtonSingleChannel.Name = "radioButtonSingleChannel";
			this.radioButtonSingleChannel.Size = new System.Drawing.Size(261, 30);
			this.radioButtonSingleChannel.TabIndex = 0;
			this.radioButtonSingleChannel.TabStop = true;
			this.radioButtonSingleChannel.Text = "Each channel will display the full color: controllers\r\nwill receive full color co" +
				"mmands for every channel.";
			this.radioButtonSingleChannel.UseVisualStyleBackColor = true;
			this.radioButtonSingleChannel.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(20, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(258, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Please select how this channel will handle RGB data.";
			// 
			// radioButtonMultiChannel
			// 
			this.radioButtonMultiChannel.AutoSize = true;
			this.radioButtonMultiChannel.Location = new System.Drawing.Point(23, 93);
			this.radioButtonMultiChannel.Name = "radioButtonMultiChannel";
			this.radioButtonMultiChannel.Size = new System.Drawing.Size(257, 30);
			this.radioButtonMultiChannel.TabIndex = 2;
			this.radioButtonMultiChannel.TabStop = true;
			this.radioButtonMultiChannel.Text = "The color should be broken up into Red, Green &&\r\nBlue components, and displayed " +
				"in subchannels:";
			this.radioButtonMultiChannel.UseVisualStyleBackColor = true;
			// 
			// groupBoxComponents
			// 
			this.groupBoxComponents.Controls.Add(this.comboBoxB);
			this.groupBoxComponents.Controls.Add(this.comboBoxG);
			this.groupBoxComponents.Controls.Add(this.label4);
			this.groupBoxComponents.Controls.Add(this.label3);
			this.groupBoxComponents.Controls.Add(this.label2);
			this.groupBoxComponents.Controls.Add(this.comboBoxR);
			this.groupBoxComponents.Location = new System.Drawing.Point(23, 129);
			this.groupBoxComponents.Name = "groupBoxComponents";
			this.groupBoxComponents.Size = new System.Drawing.Size(280, 105);
			this.groupBoxComponents.TabIndex = 3;
			this.groupBoxComponents.TabStop = false;
			this.groupBoxComponents.Text = "Color Components";
			// 
			// comboBoxB
			// 
			this.comboBoxB.FormattingEnabled = true;
			this.comboBoxB.Location = new System.Drawing.Point(66, 69);
			this.comboBoxB.Name = "comboBoxB";
			this.comboBoxB.Size = new System.Drawing.Size(200, 21);
			this.comboBoxB.TabIndex = 5;
			// 
			// comboBoxG
			// 
			this.comboBoxG.FormattingEnabled = true;
			this.comboBoxG.Location = new System.Drawing.Point(66, 44);
			this.comboBoxG.Name = "comboBoxG";
			this.comboBoxG.Size = new System.Drawing.Size(200, 21);
			this.comboBoxG.TabIndex = 4;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(16, 72);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(28, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "Blue";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(16, 47);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(36, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Green";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(16, 22);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(27, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Red";
			// 
			// comboBoxR
			// 
			this.comboBoxR.FormattingEnabled = true;
			this.comboBoxR.Location = new System.Drawing.Point(66, 19);
			this.comboBoxR.Name = "comboBoxR";
			this.comboBoxR.Size = new System.Drawing.Size(200, 21);
			this.comboBoxR.TabIndex = 0;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(147, 247);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 4;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(228, 247);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// RGBSetup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(317, 282);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.groupBoxComponents);
			this.Controls.Add(this.radioButtonMultiChannel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.radioButtonSingleChannel);
			this.Name = "RGBSetup";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "RGB Channel Setup";
			this.groupBoxComponents.ResumeLayout(false);
			this.groupBoxComponents.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton radioButtonSingleChannel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton radioButtonMultiChannel;
		private System.Windows.Forms.GroupBox groupBoxComponents;
		private System.Windows.Forms.ComboBox comboBoxB;
		private System.Windows.Forms.ComboBox comboBoxG;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBoxR;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
	}
}