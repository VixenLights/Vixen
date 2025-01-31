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
			buttonOk = new Button();
			groupBoxOptions = new GroupBox();
			panel1 = new Panel();
			colorPanelSingleColor = new ColorPanel();
			buttonColorSetsSetup = new Button();
			comboBoxColorSet = new ComboBox();
			radioButtonOptionFullColor = new RadioButton();
			radioButtonOptionMultiple = new RadioButton();
			radioButtonOptionSingle = new RadioButton();
			groupBoxOptions.SuspendLayout();
			panel1.SuspendLayout();
			SuspendLayout();
			// 
			// buttonOk
			// 
			buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonOk.DialogResult = DialogResult.OK;
			buttonOk.Location = new Point(356, 188);
			buttonOk.Name = "buttonOk";
			buttonOk.Size = new Size(105, 29);
			buttonOk.TabIndex = 27;
			buttonOk.Text = "OK";
			buttonOk.UseVisualStyleBackColor = false;
			// 
			// groupBoxOptions
			// 
			groupBoxOptions.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			groupBoxOptions.Controls.Add(panel1);
			groupBoxOptions.Location = new Point(14, 14);
			groupBoxOptions.Name = "groupBoxOptions";
			groupBoxOptions.Size = new Size(447, 167);
			groupBoxOptions.TabIndex = 29;
			groupBoxOptions.TabStop = false;
			groupBoxOptions.Text = "Element Color";
			groupBoxOptions.Paint += groupBoxes_Paint;
			// 
			// panel1
			// 
			panel1.Controls.Add(colorPanelSingleColor);
			panel1.Controls.Add(buttonColorSetsSetup);
			panel1.Controls.Add(comboBoxColorSet);
			panel1.Controls.Add(radioButtonOptionFullColor);
			panel1.Controls.Add(radioButtonOptionMultiple);
			panel1.Controls.Add(radioButtonOptionSingle);
			panel1.Location = new Point(10, 18);
			panel1.Name = "panel1";
			panel1.Size = new Size(429, 145);
			panel1.TabIndex = 7;
			// 
			// colorPanelSingleColor
			// 
			colorPanelSingleColor.BorderStyle = BorderStyle.FixedSingle;
			colorPanelSingleColor.Color = System.Drawing.Color.Maroon;
			colorPanelSingleColor.Location = new Point(208, 9);
			colorPanelSingleColor.Margin = new Padding(5);
			colorPanelSingleColor.Name = "colorPanelSingleColor";
			colorPanelSingleColor.Size = new Size(70, 34);
			colorPanelSingleColor.TabIndex = 6;
			// 
			// buttonColorSetsSetup
			// 
			buttonColorSetsSetup.Location = new Point(356, 59);
			buttonColorSetsSetup.Margin = new Padding(0);
			buttonColorSetsSetup.Name = "buttonColorSetsSetup";
			buttonColorSetsSetup.Size = new Size(40, 27);
			buttonColorSetsSetup.TabIndex = 5;
			buttonColorSetsSetup.Text = "......";
			buttonColorSetsSetup.UseVisualStyleBackColor = false;
			buttonColorSetsSetup.Click += buttonColorSetsSetup_Click;
			// 
			// comboBoxColorSet
			// 
			comboBoxColorSet.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBoxColorSet.FormattingEnabled = true;
			comboBoxColorSet.Location = new Point(208, 60);
			comboBoxColorSet.Name = "comboBoxColorSet";
			comboBoxColorSet.Size = new Size(140, 23);
			comboBoxColorSet.TabIndex = 4;
			comboBoxColorSet.SelectedIndexChanged += comboBoxColorSet_SelectedIndexChanged;
			// 
			// radioButtonOptionFullColor
			// 
			radioButtonOptionFullColor.AutoSize = true;
			radioButtonOptionFullColor.Location = new Point(10, 107);
			radioButtonOptionFullColor.Name = "radioButtonOptionFullColor";
			radioButtonOptionFullColor.Size = new Size(190, 19);
			radioButtonOptionFullColor.TabIndex = 2;
			radioButtonOptionFullColor.TabStop = true;
			radioButtonOptionFullColor.Text = "Any color (ie. fully mixing RGB)";
			radioButtonOptionFullColor.UseVisualStyleBackColor = true;
			radioButtonOptionFullColor.CheckedChanged += AnyRadioButtonCheckedChanged;
			// 
			// radioButtonOptionMultiple
			// 
			radioButtonOptionMultiple.AutoSize = true;
			radioButtonOptionMultiple.Location = new Point(10, 61);
			radioButtonOptionMultiple.Name = "radioButtonOptionMultiple";
			radioButtonOptionMultiple.Size = new Size(177, 19);
			radioButtonOptionMultiple.TabIndex = 1;
			radioButtonOptionMultiple.TabStop = true;
			radioButtonOptionMultiple.Text = "Multiple independent colors:";
			radioButtonOptionMultiple.UseVisualStyleBackColor = true;
			radioButtonOptionMultiple.CheckedChanged += AnyRadioButtonCheckedChanged;
			// 
			// radioButtonOptionSingle
			// 
			radioButtonOptionSingle.AutoSize = true;
			radioButtonOptionSingle.Location = new Point(10, 17);
			radioButtonOptionSingle.Name = "radioButtonOptionSingle";
			radioButtonOptionSingle.Size = new Size(90, 19);
			radioButtonOptionSingle.TabIndex = 0;
			radioButtonOptionSingle.TabStop = true;
			radioButtonOptionSingle.Text = "Single color:";
			radioButtonOptionSingle.UseVisualStyleBackColor = true;
			radioButtonOptionSingle.CheckedChanged += AnyRadioButtonCheckedChanged;
			// 
			// ColorSetupForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = true;
			ClientSize = new Size(475, 232);
			Controls.Add(groupBoxOptions);
			Controls.Add(buttonOk);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			MinimumSize = new Size(491, 270);
			Name = "ColorSetupForm";
			SizeGripStyle = SizeGripStyle.Hide;
			StartPosition = FormStartPosition.CenterParent;
			Text = "Element Color";
			Load += ColorSetupForm_Load;
			groupBoxOptions.ResumeLayout(false);
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.GroupBox groupBoxOptions;
		private System.Windows.Forms.RadioButton radioButtonOptionFullColor;
		private System.Windows.Forms.RadioButton radioButtonOptionMultiple;
		private System.Windows.Forms.RadioButton radioButtonOptionSingle;
		private System.Windows.Forms.ComboBox comboBoxColorSet;
		private System.Windows.Forms.Button buttonColorSetsSetup;
		private ColorPanel colorPanelSingleColor;
		private System.Windows.Forms.Panel panel1;
	}
}