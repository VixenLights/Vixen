namespace VixenModules.Property.Color
{
	partial class ColorSetupForm
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
			this.fontDialog1 = new System.Windows.Forms.FontDialog();
			this.buttonOk = new System.Windows.Forms.Button();
			this.groupBoxOptions = new System.Windows.Forms.GroupBox();
			this.buttonColorSetsSetup = new System.Windows.Forms.Button();
			this.comboBoxColorSet = new System.Windows.Forms.ComboBox();
			this.radioButtonOptionFullColor = new System.Windows.Forms.RadioButton();
			this.radioButtonOptionMultiple = new System.Windows.Forms.RadioButton();
			this.radioButtonOptionSingle = new System.Windows.Forms.RadioButton();
			this.colorPanelSingleColor = new VixenModules.Property.Color.ColorPanel();
			this.groupBoxOptions.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(305, 163);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(90, 25);
			this.buttonOk.TabIndex = 27;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// groupBoxOptions
			// 
			this.groupBoxOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxOptions.Controls.Add(this.colorPanelSingleColor);
			this.groupBoxOptions.Controls.Add(this.buttonColorSetsSetup);
			this.groupBoxOptions.Controls.Add(this.comboBoxColorSet);
			this.groupBoxOptions.Controls.Add(this.radioButtonOptionFullColor);
			this.groupBoxOptions.Controls.Add(this.radioButtonOptionMultiple);
			this.groupBoxOptions.Controls.Add(this.radioButtonOptionSingle);
			this.groupBoxOptions.Location = new System.Drawing.Point(12, 12);
			this.groupBoxOptions.Name = "groupBoxOptions";
			this.groupBoxOptions.Size = new System.Drawing.Size(383, 145);
			this.groupBoxOptions.TabIndex = 29;
			this.groupBoxOptions.TabStop = false;
			this.groupBoxOptions.Text = "Element Color";
			// 
			// buttonColorSetsSetup
			// 
			this.buttonColorSetsSetup.Location = new System.Drawing.Point(314, 67);
			this.buttonColorSetsSetup.Name = "buttonColorSetsSetup";
			this.buttonColorSetsSetup.Size = new System.Drawing.Size(25, 23);
			this.buttonColorSetsSetup.TabIndex = 5;
			this.buttonColorSetsSetup.Text = "...";
			this.buttonColorSetsSetup.UseVisualStyleBackColor = true;
			this.buttonColorSetsSetup.Click += new System.EventHandler(this.buttonColorSetsSetup_Click);
			// 
			// comboBoxColorSet
			// 
			this.comboBoxColorSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxColorSet.FormattingEnabled = true;
			this.comboBoxColorSet.Location = new System.Drawing.Point(187, 68);
			this.comboBoxColorSet.Name = "comboBoxColorSet";
			this.comboBoxColorSet.Size = new System.Drawing.Size(121, 21);
			this.comboBoxColorSet.TabIndex = 4;
			this.comboBoxColorSet.SelectedIndexChanged += new System.EventHandler(this.comboBoxColorSet_SelectedIndexChanged);
			// 
			// radioButtonOptionFullColor
			// 
			this.radioButtonOptionFullColor.AutoSize = true;
			this.radioButtonOptionFullColor.Location = new System.Drawing.Point(18, 109);
			this.radioButtonOptionFullColor.Name = "radioButtonOptionFullColor";
			this.radioButtonOptionFullColor.Size = new System.Drawing.Size(168, 17);
			this.radioButtonOptionFullColor.TabIndex = 2;
			this.radioButtonOptionFullColor.TabStop = true;
			this.radioButtonOptionFullColor.Text = "Any color (ie. fully mixing RGB)";
			this.radioButtonOptionFullColor.UseVisualStyleBackColor = true;
			this.radioButtonOptionFullColor.CheckedChanged += new System.EventHandler(this.AnyRadioButtonCheckedChanged);
			// 
			// radioButtonOptionMultiple
			// 
			this.radioButtonOptionMultiple.AutoSize = true;
			this.radioButtonOptionMultiple.Location = new System.Drawing.Point(18, 69);
			this.radioButtonOptionMultiple.Name = "radioButtonOptionMultiple";
			this.radioButtonOptionMultiple.Size = new System.Drawing.Size(157, 17);
			this.radioButtonOptionMultiple.TabIndex = 1;
			this.radioButtonOptionMultiple.TabStop = true;
			this.radioButtonOptionMultiple.Text = "Multiple independent colors:";
			this.radioButtonOptionMultiple.UseVisualStyleBackColor = true;
			this.radioButtonOptionMultiple.CheckedChanged += new System.EventHandler(this.AnyRadioButtonCheckedChanged);
			// 
			// radioButtonOptionSingle
			// 
			this.radioButtonOptionSingle.AutoSize = true;
			this.radioButtonOptionSingle.Location = new System.Drawing.Point(18, 31);
			this.radioButtonOptionSingle.Name = "radioButtonOptionSingle";
			this.radioButtonOptionSingle.Size = new System.Drawing.Size(83, 17);
			this.radioButtonOptionSingle.TabIndex = 0;
			this.radioButtonOptionSingle.TabStop = true;
			this.radioButtonOptionSingle.Text = "Single color:";
			this.radioButtonOptionSingle.UseVisualStyleBackColor = true;
			this.radioButtonOptionSingle.CheckedChanged += new System.EventHandler(this.AnyRadioButtonCheckedChanged);
			// 
			// colorPanelSingleColor
			// 
			this.colorPanelSingleColor.BackColor = System.Drawing.Color.Maroon;
			this.colorPanelSingleColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.colorPanelSingleColor.Color = System.Drawing.Color.Maroon;
			this.colorPanelSingleColor.Location = new System.Drawing.Point(187, 24);
			this.colorPanelSingleColor.Name = "colorPanelSingleColor";
			this.colorPanelSingleColor.Size = new System.Drawing.Size(60, 30);
			this.colorPanelSingleColor.TabIndex = 6;
			this.colorPanelSingleColor.ColorChanged += new System.EventHandler<VixenModules.Property.Color.ColorPanelEventArgs>(this.colorPanelSingleColor_ColorChanged);
			// 
			// ColorSetupForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(407, 200);
			this.Controls.Add(this.groupBoxOptions);
			this.Controls.Add(this.buttonOk);
			this.Name = "ColorSetupForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Element Color";
			this.Load += new System.EventHandler(this.ColorSetupForm_Load);
			this.groupBoxOptions.ResumeLayout(false);
			this.groupBoxOptions.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.FontDialog fontDialog1;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.GroupBox groupBoxOptions;
		private System.Windows.Forms.RadioButton radioButtonOptionFullColor;
		private System.Windows.Forms.RadioButton radioButtonOptionMultiple;
		private System.Windows.Forms.RadioButton radioButtonOptionSingle;
		private System.Windows.Forms.ComboBox comboBoxColorSet;
		private System.Windows.Forms.Button buttonColorSetsSetup;
		private ColorPanel colorPanelSingleColor;
	}
}