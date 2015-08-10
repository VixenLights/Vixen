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
			this.textBoxWindowTitle = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(183, 191);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 4;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
			this.buttonOK.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOK.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(264, 191);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// radioButtonMonochrome
			// 
			this.radioButtonMonochrome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.radioButtonMonochrome.AutoSize = true;
			this.radioButtonMonochrome.Location = new System.Drawing.Point(12, 63);
			this.radioButtonMonochrome.Name = "radioButtonMonochrome";
			this.radioButtonMonochrome.Size = new System.Drawing.Size(180, 17);
			this.radioButtonMonochrome.TabIndex = 1;
			this.radioButtonMonochrome.TabStop = true;
			this.radioButtonMonochrome.Text = "Set up as monochrome channels";
			this.radioButtonMonochrome.UseVisualStyleBackColor = true;
			this.radioButtonMonochrome.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
			// 
			// radioButtonMultiRGB
			// 
			this.radioButtonMultiRGB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.radioButtonMultiRGB.Location = new System.Drawing.Point(12, 92);
			this.radioButtonMultiRGB.Name = "radioButtonMultiRGB";
			this.radioButtonMultiRGB.Size = new System.Drawing.Size(313, 43);
			this.radioButtonMultiRGB.TabIndex = 2;
			this.radioButtonMultiRGB.TabStop = true;
			this.radioButtonMultiRGB.Text = "Set up as RGB channels, with every 3 inputs mapping to the R, G, B components of " +
    "a single RGB output";
			this.radioButtonMultiRGB.UseVisualStyleBackColor = true;
			this.radioButtonMultiRGB.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
			// 
			// radioButtonSingleRGB
			// 
			this.radioButtonSingleRGB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.radioButtonSingleRGB.AutoSize = true;
			this.radioButtonSingleRGB.Location = new System.Drawing.Point(12, 145);
			this.radioButtonSingleRGB.Name = "radioButtonSingleRGB";
			this.radioButtonSingleRGB.Size = new System.Drawing.Size(222, 30);
			this.radioButtonSingleRGB.TabIndex = 3;
			this.radioButtonSingleRGB.TabStop = true;
			this.radioButtonSingleRGB.Text = "Set up as RGB channels, with every input\r\nmapping to a full-color RGB channel\r\n";
			this.radioButtonSingleRGB.UseVisualStyleBackColor = true;
			this.radioButtonSingleRGB.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
			// 
			// textBoxWindowTitle
			// 
			this.textBoxWindowTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxWindowTitle.Location = new System.Drawing.Point(92, 24);
			this.textBoxWindowTitle.Name = "textBoxWindowTitle";
			this.textBoxWindowTitle.Size = new System.Drawing.Size(165, 20);
			this.textBoxWindowTitle.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 27);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 13);
			this.label1.TabIndex = 6;
			this.label1.Text = "Window Title:";
			// 
			// DummyLightingSetup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(355, 237);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxWindowTitle);
			this.Controls.Add(this.radioButtonSingleRGB);
			this.Controls.Add(this.radioButtonMultiRGB);
			this.Controls.Add(this.radioButtonMonochrome);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.MaximumSize = new System.Drawing.Size(371, 276);
			this.MinimumSize = new System.Drawing.Size(371, 276);
			this.Name = "DummyLightingSetup";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Dummy Lighting Configuration";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.RadioButton radioButtonMonochrome;
		private System.Windows.Forms.RadioButton radioButtonMultiRGB;
		private System.Windows.Forms.RadioButton radioButtonSingleRGB;
		private System.Windows.Forms.TextBox textBoxWindowTitle;
		private System.Windows.Forms.Label label1;

	}
}