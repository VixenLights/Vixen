namespace VixenModules.Output.DummyLighting
{
	partial class DummyLightingSetup
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
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.radioButtonMonochrome = new System.Windows.Forms.RadioButton();
			this.radioButtonMultiRGB = new System.Windows.Forms.RadioButton();
			this.radioButtonSingleRGB = new System.Windows.Forms.RadioButton();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(116, 146);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 0;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(197, 146);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// radioButtonMonochrome
			// 
			this.radioButtonMonochrome.AutoSize = true;
			this.radioButtonMonochrome.Location = new System.Drawing.Point(12, 12);
			this.radioButtonMonochrome.Name = "radioButtonMonochrome";
			this.radioButtonMonochrome.Size = new System.Drawing.Size(180, 17);
			this.radioButtonMonochrome.TabIndex = 2;
			this.radioButtonMonochrome.TabStop = true;
			this.radioButtonMonochrome.Text = "Set up as monochrome channels";
			this.radioButtonMonochrome.UseVisualStyleBackColor = true;
			this.radioButtonMonochrome.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
			// 
			// radioButtonMultiRGB
			// 
			this.radioButtonMultiRGB.AutoSize = true;
			this.radioButtonMultiRGB.Location = new System.Drawing.Point(12, 43);
			this.radioButtonMultiRGB.Name = "radioButtonMultiRGB";
			this.radioButtonMultiRGB.Size = new System.Drawing.Size(245, 43);
			this.radioButtonMultiRGB.TabIndex = 3;
			this.radioButtonMultiRGB.TabStop = true;
			this.radioButtonMultiRGB.Text = "Set up as RGB channels, with every 3 inputs\r\nmapping to the R, G, B components of" +
    " a single\r\nRGB output";
			this.radioButtonMultiRGB.UseVisualStyleBackColor = true;
			this.radioButtonMultiRGB.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
			// 
			// radioButtonSingleRGB
			// 
			this.radioButtonSingleRGB.AutoSize = true;
			this.radioButtonSingleRGB.Location = new System.Drawing.Point(12, 95);
			this.radioButtonSingleRGB.Name = "radioButtonSingleRGB";
			this.radioButtonSingleRGB.Size = new System.Drawing.Size(222, 30);
			this.radioButtonSingleRGB.TabIndex = 4;
			this.radioButtonSingleRGB.TabStop = true;
			this.radioButtonSingleRGB.Text = "Set up as RGB channels, with every input\r\nmapping to a full-color RGB channel\r\n";
			this.radioButtonSingleRGB.UseVisualStyleBackColor = true;
			this.radioButtonSingleRGB.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
			// 
			// DummyLightingSetup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 181);
			this.Controls.Add(this.radioButtonSingleRGB);
			this.Controls.Add(this.radioButtonMultiRGB);
			this.Controls.Add(this.radioButtonMonochrome);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Name = "DummyLightingSetup";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Dummy Lighting Setup";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.RadioButton radioButtonMonochrome;
		private System.Windows.Forms.RadioButton radioButtonMultiRGB;
		private System.Windows.Forms.RadioButton radioButtonSingleRGB;

	}
}