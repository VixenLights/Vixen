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
			this.buttonOk = new System.Windows.Forms.Button();
			this.groupBoxOptions = new System.Windows.Forms.GroupBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.colorPanelSingleColor = new VixenModules.Property.Color.ColorPanel();
			this.buttonColorSetsSetup = new System.Windows.Forms.Button();
			this.comboBoxColorSet = new System.Windows.Forms.ComboBox();
			this.radioButtonOptionFullColor = new System.Windows.Forms.RadioButton();
			this.radioButtonOptionMultiple = new System.Windows.Forms.RadioButton();
			this.radioButtonOptionSingle = new System.Windows.Forms.RadioButton();
			this.groupBoxOptions.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.BackColor = System.Drawing.SystemColors.Control;
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(356, 188);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(105, 29);
			this.buttonOk.TabIndex = 27;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = false;
			this.buttonOk.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOk.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// groupBoxOptions
			// 
			this.groupBoxOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxOptions.Controls.Add(this.panel1);
			this.groupBoxOptions.Location = new System.Drawing.Point(14, 14);
			this.groupBoxOptions.Name = "groupBoxOptions";
			this.groupBoxOptions.Size = new System.Drawing.Size(447, 167);
			this.groupBoxOptions.TabIndex = 29;
			this.groupBoxOptions.TabStop = false;
			this.groupBoxOptions.Text = "Element Color";
			this.groupBoxOptions.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.colorPanelSingleColor);
			this.panel1.Controls.Add(this.buttonColorSetsSetup);
			this.panel1.Controls.Add(this.comboBoxColorSet);
			this.panel1.Controls.Add(this.radioButtonOptionFullColor);
			this.panel1.Controls.Add(this.radioButtonOptionMultiple);
			this.panel1.Controls.Add(this.radioButtonOptionSingle);
			this.panel1.Location = new System.Drawing.Point(10, 18);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(429, 145);
			this.panel1.TabIndex = 7;
			// 
			// colorPanelSingleColor
			// 
			this.colorPanelSingleColor.BackColor = System.Drawing.Color.Maroon;
			this.colorPanelSingleColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.colorPanelSingleColor.Color = System.Drawing.Color.Maroon;
			this.colorPanelSingleColor.Location = new System.Drawing.Point(208, 9);
			this.colorPanelSingleColor.Margin = new System.Windows.Forms.Padding(5);
			this.colorPanelSingleColor.Name = "colorPanelSingleColor";
			this.colorPanelSingleColor.Size = new System.Drawing.Size(70, 34);
			this.colorPanelSingleColor.TabIndex = 6;
			// 
			// buttonColorSetsSetup
			// 
			this.buttonColorSetsSetup.BackColor = System.Drawing.SystemColors.Control;
			this.buttonColorSetsSetup.FlatAppearance.BorderSize = 0;
			this.buttonColorSetsSetup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonColorSetsSetup.Location = new System.Drawing.Point(356, 59);
			this.buttonColorSetsSetup.Margin = new System.Windows.Forms.Padding(0);
			this.buttonColorSetsSetup.Name = "buttonColorSetsSetup";
			this.buttonColorSetsSetup.Size = new System.Drawing.Size(40, 27);
			this.buttonColorSetsSetup.TabIndex = 5;
			this.buttonColorSetsSetup.Text = "......";
			this.buttonColorSetsSetup.UseVisualStyleBackColor = false;
			this.buttonColorSetsSetup.Click += new System.EventHandler(this.buttonColorSetsSetup_Click);
			// 
			// comboBoxColorSet
			// 
			this.comboBoxColorSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxColorSet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.comboBoxColorSet.FormattingEnabled = true;
			this.comboBoxColorSet.Location = new System.Drawing.Point(208, 60);
			this.comboBoxColorSet.Name = "comboBoxColorSet";
			this.comboBoxColorSet.Size = new System.Drawing.Size(140, 23);
			this.comboBoxColorSet.TabIndex = 4;
			this.comboBoxColorSet.SelectedIndexChanged += new System.EventHandler(this.comboBoxColorSet_SelectedIndexChanged);
			// 
			// radioButtonOptionFullColor
			// 
			this.radioButtonOptionFullColor.AutoSize = true;
			this.radioButtonOptionFullColor.Location = new System.Drawing.Point(10, 107);
			this.radioButtonOptionFullColor.Name = "radioButtonOptionFullColor";
			this.radioButtonOptionFullColor.Size = new System.Drawing.Size(189, 19);
			this.radioButtonOptionFullColor.TabIndex = 2;
			this.radioButtonOptionFullColor.TabStop = true;
			this.radioButtonOptionFullColor.Text = "Any color (ie. fully mixing RGB)";
			this.radioButtonOptionFullColor.UseVisualStyleBackColor = true;
			this.radioButtonOptionFullColor.CheckedChanged += new System.EventHandler(this.AnyRadioButtonCheckedChanged);
			// 
			// radioButtonOptionMultiple
			// 
			this.radioButtonOptionMultiple.AutoSize = true;
			this.radioButtonOptionMultiple.Location = new System.Drawing.Point(10, 61);
			this.radioButtonOptionMultiple.Name = "radioButtonOptionMultiple";
			this.radioButtonOptionMultiple.Size = new System.Drawing.Size(177, 19);
			this.radioButtonOptionMultiple.TabIndex = 1;
			this.radioButtonOptionMultiple.TabStop = true;
			this.radioButtonOptionMultiple.Text = "Multiple independent colors:";
			this.radioButtonOptionMultiple.UseVisualStyleBackColor = true;
			this.radioButtonOptionMultiple.CheckedChanged += new System.EventHandler(this.AnyRadioButtonCheckedChanged);
			// 
			// radioButtonOptionSingle
			// 
			this.radioButtonOptionSingle.AutoSize = true;
			this.radioButtonOptionSingle.Location = new System.Drawing.Point(10, 17);
			this.radioButtonOptionSingle.Name = "radioButtonOptionSingle";
			this.radioButtonOptionSingle.Size = new System.Drawing.Size(90, 19);
			this.radioButtonOptionSingle.TabIndex = 0;
			this.radioButtonOptionSingle.TabStop = true;
			this.radioButtonOptionSingle.Text = "Single color:";
			this.radioButtonOptionSingle.UseVisualStyleBackColor = true;
			this.radioButtonOptionSingle.CheckedChanged += new System.EventHandler(this.AnyRadioButtonCheckedChanged);
			// 
			// ColorSetupForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(475, 232);
			this.Controls.Add(this.groupBoxOptions);
			this.Controls.Add(this.buttonOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(491, 270);
			this.Name = "ColorSetupForm";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Element Color";
			this.Load += new System.EventHandler(this.ColorSetupForm_Load);
			this.groupBoxOptions.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

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