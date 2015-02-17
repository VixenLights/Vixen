namespace VixenModules.EffectEditor.CustomValueEditor
{
	partial class CustomValueEditorControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.radioButton8bit = new System.Windows.Forms.RadioButton();
			this.radioButton16bit = new System.Windows.Forms.RadioButton();
			this.radioButton32bit = new System.Windows.Forms.RadioButton();
			this.radioButton64bit = new System.Windows.Forms.RadioButton();
			this.radioButtonColor = new System.Windows.Forms.RadioButton();
			this.radioButtonString = new System.Windows.Forms.RadioButton();
			this.numericUpDown8bit = new System.Windows.Forms.NumericUpDown();
			this.numericUpDown16bit = new System.Windows.Forms.NumericUpDown();
			this.numericUpDown32bit = new System.Windows.Forms.NumericUpDown();
			this.numericUpDown64bit = new System.Windows.Forms.NumericUpDown();
			this.colorTypeEditorControlColor = new VixenModules.EffectEditor.ColorTypeEditor.ColorTypeEditorControl();
			this.textBoxStringValue = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown8bit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown16bit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown32bit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown64bit)).BeginInit();
			this.SuspendLayout();
			// 
			// radioButton8bit
			// 
			this.radioButton8bit.AutoSize = true;
			this.radioButton8bit.Location = new System.Drawing.Point(10, 11);
			this.radioButton8bit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioButton8bit.Name = "radioButton8bit";
			this.radioButton8bit.Size = new System.Drawing.Size(109, 24);
			this.radioButton8bit.TabIndex = 0;
			this.radioButton8bit.TabStop = true;
			this.radioButton8bit.Text = "8 bit value:";
			this.radioButton8bit.UseVisualStyleBackColor = true;
			this.radioButton8bit.CheckedChanged += new System.EventHandler(this.radioButtonTypes_CheckedChanged);
			// 
			// radioButton16bit
			// 
			this.radioButton16bit.AutoSize = true;
			this.radioButton16bit.Location = new System.Drawing.Point(10, 54);
			this.radioButton16bit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioButton16bit.Name = "radioButton16bit";
			this.radioButton16bit.Size = new System.Drawing.Size(118, 24);
			this.radioButton16bit.TabIndex = 2;
			this.radioButton16bit.TabStop = true;
			this.radioButton16bit.Text = "16 bit value:";
			this.radioButton16bit.UseVisualStyleBackColor = true;
			this.radioButton16bit.CheckedChanged += new System.EventHandler(this.radioButtonTypes_CheckedChanged);
			// 
			// radioButton32bit
			// 
			this.radioButton32bit.AutoSize = true;
			this.radioButton32bit.Location = new System.Drawing.Point(10, 97);
			this.radioButton32bit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioButton32bit.Name = "radioButton32bit";
			this.radioButton32bit.Size = new System.Drawing.Size(118, 24);
			this.radioButton32bit.TabIndex = 4;
			this.radioButton32bit.TabStop = true;
			this.radioButton32bit.Text = "32 bit value:";
			this.radioButton32bit.UseVisualStyleBackColor = true;
			this.radioButton32bit.CheckedChanged += new System.EventHandler(this.radioButtonTypes_CheckedChanged);
			// 
			// radioButton64bit
			// 
			this.radioButton64bit.AutoSize = true;
			this.radioButton64bit.Location = new System.Drawing.Point(10, 140);
			this.radioButton64bit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioButton64bit.Name = "radioButton64bit";
			this.radioButton64bit.Size = new System.Drawing.Size(118, 24);
			this.radioButton64bit.TabIndex = 6;
			this.radioButton64bit.TabStop = true;
			this.radioButton64bit.Text = "64 bit value:";
			this.radioButton64bit.UseVisualStyleBackColor = true;
			this.radioButton64bit.CheckedChanged += new System.EventHandler(this.radioButtonTypes_CheckedChanged);
			// 
			// radioButtonColor
			// 
			this.radioButtonColor.AutoSize = true;
			this.radioButtonColor.Location = new System.Drawing.Point(10, 183);
			this.radioButtonColor.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioButtonColor.Name = "radioButtonColor";
			this.radioButtonColor.Size = new System.Drawing.Size(116, 24);
			this.radioButtonColor.TabIndex = 8;
			this.radioButtonColor.TabStop = true;
			this.radioButtonColor.Text = "Color value:";
			this.radioButtonColor.UseVisualStyleBackColor = true;
			this.radioButtonColor.CheckedChanged += new System.EventHandler(this.radioButtonTypes_CheckedChanged);
			// 
			// radioButtonString
			// 
			this.radioButtonString.AutoSize = true;
			this.radioButtonString.Checked = true;
			this.radioButtonString.Location = new System.Drawing.Point(10, 226);
			this.radioButtonString.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.radioButtonString.Name = "radioButtonString";
			this.radioButtonString.Size = new System.Drawing.Size(121, 24);
			this.radioButtonString.TabIndex = 10;
			this.radioButtonString.TabStop = true;
			this.radioButtonString.Text = "String value:";
			this.radioButtonString.UseVisualStyleBackColor = true;
			this.radioButtonString.CheckedChanged += new System.EventHandler(this.radioButtonTypes_CheckedChanged);
			// 
			// numericUpDown8bit
			// 
			this.numericUpDown8bit.Location = new System.Drawing.Point(147, 11);
			this.numericUpDown8bit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.numericUpDown8bit.Maximum = new decimal(new int[] {
			255,
			0,
			0,
			0});
			this.numericUpDown8bit.Name = "numericUpDown8bit";
			this.numericUpDown8bit.Size = new System.Drawing.Size(120, 26);
			this.numericUpDown8bit.TabIndex = 1;
			// 
			// numericUpDown16bit
			// 
			this.numericUpDown16bit.Location = new System.Drawing.Point(147, 54);
			this.numericUpDown16bit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.numericUpDown16bit.Maximum = new decimal(new int[] {
			65535,
			0,
			0,
			0});
			this.numericUpDown16bit.Name = "numericUpDown16bit";
			this.numericUpDown16bit.Size = new System.Drawing.Size(120, 26);
			this.numericUpDown16bit.TabIndex = 3;
			// 
			// numericUpDown32bit
			// 
			this.numericUpDown32bit.Location = new System.Drawing.Point(147, 97);
			this.numericUpDown32bit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.numericUpDown32bit.Maximum = new decimal(new int[] {
			-1,
			0,
			0,
			0});
			this.numericUpDown32bit.Name = "numericUpDown32bit";
			this.numericUpDown32bit.Size = new System.Drawing.Size(120, 26);
			this.numericUpDown32bit.TabIndex = 5;
			// 
			// numericUpDown64bit
			// 
			this.numericUpDown64bit.Location = new System.Drawing.Point(147, 140);
			this.numericUpDown64bit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.numericUpDown64bit.Maximum = new decimal(new int[] {
			-1,
			-1,
			0,
			0});
			this.numericUpDown64bit.Name = "numericUpDown64bit";
			this.numericUpDown64bit.Size = new System.Drawing.Size(120, 26);
			this.numericUpDown64bit.TabIndex = 7;
			// 
			// colorTypeEditorControlColor
			// 
			this.colorTypeEditorControlColor.ColorValue = System.Drawing.Color.Empty;
			this.colorTypeEditorControlColor.EffectParameterValues = new object[] {
		((object)(System.Drawing.Color.Empty))};
			this.colorTypeEditorControlColor.Location = new System.Drawing.Point(147, 178);
			this.colorTypeEditorControlColor.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
			this.colorTypeEditorControlColor.Name = "colorTypeEditorControlColor";
			this.colorTypeEditorControlColor.Size = new System.Drawing.Size(120, 34);
			this.colorTypeEditorControlColor.TabIndex = 9;
			this.colorTypeEditorControlColor.TargetEffect = null;
			// 
			// textBoxStringValue
			// 
			this.textBoxStringValue.Location = new System.Drawing.Point(147, 222);
			this.textBoxStringValue.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.textBoxStringValue.Name = "textBoxStringValue";
			this.textBoxStringValue.Size = new System.Drawing.Size(238, 26);
			this.textBoxStringValue.TabIndex = 11;
			// 
			// CustomValueEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.textBoxStringValue);
			this.Controls.Add(this.colorTypeEditorControlColor);
			this.Controls.Add(this.numericUpDown64bit);
			this.Controls.Add(this.numericUpDown32bit);
			this.Controls.Add(this.numericUpDown16bit);
			this.Controls.Add(this.numericUpDown8bit);
			this.Controls.Add(this.radioButtonString);
			this.Controls.Add(this.radioButtonColor);
			this.Controls.Add(this.radioButton64bit);
			this.Controls.Add(this.radioButton32bit);
			this.Controls.Add(this.radioButton16bit);
			this.Controls.Add(this.radioButton8bit);
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.Name = "CustomValueEditorControl";
			this.Size = new System.Drawing.Size(399, 265);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown8bit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown16bit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown32bit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown64bit)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton radioButton8bit;
		private System.Windows.Forms.RadioButton radioButton16bit;
		private System.Windows.Forms.RadioButton radioButton32bit;
		private System.Windows.Forms.RadioButton radioButton64bit;
		private System.Windows.Forms.RadioButton radioButtonColor;
		private System.Windows.Forms.RadioButton radioButtonString;
		private System.Windows.Forms.NumericUpDown numericUpDown8bit;
		private System.Windows.Forms.NumericUpDown numericUpDown16bit;
		private System.Windows.Forms.NumericUpDown numericUpDown32bit;
		private System.Windows.Forms.NumericUpDown numericUpDown64bit;
		private ColorTypeEditor.ColorTypeEditorControl colorTypeEditorControlColor;
		private System.Windows.Forms.TextBox textBoxStringValue;
	}
}
