namespace VixenModules.Property.Color
{
	partial class ColorSetupHelper
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
			buttonCancel = new Button();
			buttonOk = new Button();
			label1 = new Label();
			buttonColorSetsSetup = new Button();
			comboBoxColorSet = new ComboBox();
			radioButtonOptionFullColor = new RadioButton();
			radioButtonOptionMultiple = new RadioButton();
			radioButtonOptionSingle = new RadioButton();
			label2 = new Label();
			label3 = new Label();
			colorPanelSingleColor = new ColorPanel();
			comboBoxColorOrder = new ComboBox();
			label4 = new Label();
			SuspendLayout();
			// 
			// buttonCancel
			// 
			buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonCancel.DialogResult = DialogResult.Cancel;
			buttonCancel.Location = new Point(422, 329);
			buttonCancel.Name = "buttonCancel";
			buttonCancel.Size = new Size(105, 29);
			buttonCancel.TabIndex = 22;
			buttonCancel.Text = "Cancel";
			buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOk
			// 
			buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonOk.DialogResult = DialogResult.OK;
			buttonOk.Enabled = false;
			buttonOk.Location = new Point(310, 329);
			buttonOk.Name = "buttonOk";
			buttonOk.Size = new Size(105, 29);
			buttonOk.TabIndex = 21;
			buttonOk.Text = "OK";
			buttonOk.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
			label1.Location = new Point(14, 22);
			label1.Name = "label1";
			label1.Size = new Size(240, 16);
			label1.TabIndex = 23;
			label1.Text = "How do these items handle color?";
			// 
			// buttonColorSetsSetup
			// 
			buttonColorSetsSetup.Enabled = false;
			buttonColorSetsSetup.Location = new Point(276, 217);
			buttonColorSetsSetup.Name = "buttonColorSetsSetup";
			buttonColorSetsSetup.Size = new Size(87, 27);
			buttonColorSetsSetup.TabIndex = 28;
			buttonColorSetsSetup.Text = "Edit Colors";
			buttonColorSetsSetup.UseVisualStyleBackColor = true;
			buttonColorSetsSetup.Click += buttonColorSetsSetup_Click;
			// 
			// comboBoxColorSet
			// 
			comboBoxColorSet.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBoxColorSet.Enabled = false;
			comboBoxColorSet.FormattingEnabled = true;
			comboBoxColorSet.Location = new Point(115, 219);
			comboBoxColorSet.Name = "comboBoxColorSet";
			comboBoxColorSet.Size = new Size(140, 23);
			comboBoxColorSet.TabIndex = 27;
			// 
			// radioButtonOptionFullColor
			// 
			radioButtonOptionFullColor.AutoSize = true;
			radioButtonOptionFullColor.Location = new Point(42, 276);
			radioButtonOptionFullColor.Name = "radioButtonOptionFullColor";
			radioButtonOptionFullColor.Size = new Size(380, 19);
			radioButtonOptionFullColor.TabIndex = 26;
			radioButtonOptionFullColor.TabStop = true;
			radioButtonOptionFullColor.Text = "They can be any color: they are full RGB and mix to make any color.";
			radioButtonOptionFullColor.UseVisualStyleBackColor = true;
			radioButtonOptionFullColor.CheckedChanged += AnyRadioButtonCheckedChanged;
			// 
			// radioButtonOptionMultiple
			// 
			radioButtonOptionMultiple.AutoSize = true;
			radioButtonOptionMultiple.Location = new Point(42, 160);
			radioButtonOptionMultiple.Name = "radioButtonOptionMultiple";
			radioButtonOptionMultiple.Size = new Size(431, 19);
			radioButtonOptionMultiple.TabIndex = 25;
			radioButtonOptionMultiple.TabStop = true;
			radioButtonOptionMultiple.Text = "They can be multiple colors, but the colors are independent: they do not mix.";
			radioButtonOptionMultiple.UseVisualStyleBackColor = true;
			radioButtonOptionMultiple.CheckedChanged += AnyRadioButtonCheckedChanged;
			// 
			// radioButtonOptionSingle
			// 
			radioButtonOptionSingle.AutoSize = true;
			radioButtonOptionSingle.Location = new Point(42, 63);
			radioButtonOptionSingle.Name = "radioButtonOptionSingle";
			radioButtonOptionSingle.Size = new Size(377, 19);
			radioButtonOptionSingle.TabIndex = 24;
			radioButtonOptionSingle.TabStop = true;
			radioButtonOptionSingle.Text = "They are a single color, and do not change color at all. The color is:";
			radioButtonOptionSingle.UseVisualStyleBackColor = true;
			radioButtonOptionSingle.CheckedChanged += AnyRadioButtonCheckedChanged;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(63, 188);
			label2.Name = "label2";
			label2.Size = new Size(361, 15);
			label2.TabIndex = 30;
			label2.Text = "(eg. multiple strings of different colors wrapped around each item.)";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new Point(63, 223);
			label3.Name = "label3";
			label3.Size = new Size(44, 15);
			label3.TabIndex = 31;
			label3.Text = "Colors:";
			// 
			// colorPanelSingleColor
			// 
			colorPanelSingleColor.BackColor = System.Drawing.Color.RoyalBlue;
			colorPanelSingleColor.BorderStyle = BorderStyle.FixedSingle;
			colorPanelSingleColor.Color = System.Drawing.Color.RoyalBlue;
			colorPanelSingleColor.Enabled = false;
			colorPanelSingleColor.Location = new Point(68, 96);
			colorPanelSingleColor.Margin = new Padding(5);
			colorPanelSingleColor.Name = "colorPanelSingleColor";
			colorPanelSingleColor.Size = new Size(70, 34);
			colorPanelSingleColor.TabIndex = 29;
			// 
			// comboBoxColorOrder
			// 
			comboBoxColorOrder.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBoxColorOrder.Enabled = false;
			comboBoxColorOrder.FlatStyle = FlatStyle.Flat;
			comboBoxColorOrder.FormattingEnabled = true;
			comboBoxColorOrder.Items.AddRange(new object[] { "RGB", "RBG", "GBR", "GRB", "BRG", "BGR", "RGBW", "GRWB" });
			comboBoxColorOrder.Location = new Point(141, 301);
			comboBoxColorOrder.Name = "comboBoxColorOrder";
			comboBoxColorOrder.Size = new Size(114, 23);
			comboBoxColorOrder.TabIndex = 32;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new Point(63, 304);
			label4.Name = "label4";
			label4.Size = new Size(72, 15);
			label4.TabIndex = 33;
			label4.Text = "Color Order:";
			// 
			// ColorSetupHelper
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = true;
			ClientSize = new Size(541, 372);
			Controls.Add(label4);
			Controls.Add(comboBoxColorOrder);
			Controls.Add(label3);
			Controls.Add(label2);
			Controls.Add(colorPanelSingleColor);
			Controls.Add(buttonColorSetsSetup);
			Controls.Add(comboBoxColorSet);
			Controls.Add(radioButtonOptionFullColor);
			Controls.Add(radioButtonOptionMultiple);
			Controls.Add(radioButtonOptionSingle);
			Controls.Add(label1);
			Controls.Add(buttonCancel);
			Controls.Add(buttonOk);
			FormBorderStyle = FormBorderStyle.FixedSingle;
			MaximizeBox = false;
			MinimizeBox = false;
			MinimumSize = new Size(557, 410);
			Name = "ColorSetupHelper";
			SizeGripStyle = SizeGripStyle.Hide;
			StartPosition = FormStartPosition.CenterParent;
			Text = "Color Configuration";
			Load += ColorSetupHelper_Load;
			ResumeLayout(false);
			PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Label label1;
		private ColorPanel colorPanelSingleColor;
		private System.Windows.Forms.Button buttonColorSetsSetup;
		private System.Windows.Forms.ComboBox comboBoxColorSet;
		private System.Windows.Forms.RadioButton radioButtonOptionFullColor;
		private System.Windows.Forms.RadioButton radioButtonOptionMultiple;
		private System.Windows.Forms.RadioButton radioButtonOptionSingle;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboBoxColorOrder;
		private System.Windows.Forms.Label label4;
	}
}