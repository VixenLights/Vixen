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
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonColorSetsSetup = new System.Windows.Forms.Button();
			this.comboBoxColorSet = new System.Windows.Forms.ComboBox();
			this.radioButtonOptionFullColor = new System.Windows.Forms.RadioButton();
			this.radioButtonOptionMultiple = new System.Windows.Forms.RadioButton();
			this.radioButtonOptionSingle = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.colorPanelSingleColor = new VixenModules.Property.Color.ColorPanel();
			this.comboBoxColorOrder = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(422, 329);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(105, 29);
			this.buttonCancel.TabIndex = 22;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Enabled = false;
			this.buttonOk.Location = new System.Drawing.Point(310, 329);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(105, 29);
			this.buttonOk.TabIndex = 21;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonOk.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOk.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(14, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(241, 16);
			this.label1.TabIndex = 23;
			this.label1.Text = "How do these items handle color?";
			// 
			// buttonColorSetsSetup
			// 
			this.buttonColorSetsSetup.Enabled = false;
			this.buttonColorSetsSetup.Location = new System.Drawing.Point(276, 217);
			this.buttonColorSetsSetup.Name = "buttonColorSetsSetup";
			this.buttonColorSetsSetup.Size = new System.Drawing.Size(87, 27);
			this.buttonColorSetsSetup.TabIndex = 28;
			this.buttonColorSetsSetup.Text = "Edit Colors";
			this.buttonColorSetsSetup.UseVisualStyleBackColor = true;
			this.buttonColorSetsSetup.Click += new System.EventHandler(this.buttonColorSetsSetup_Click);
			this.buttonColorSetsSetup.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonColorSetsSetup.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// comboBoxColorSet
			// 
			this.comboBoxColorSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxColorSet.Enabled = false;
			this.comboBoxColorSet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.comboBoxColorSet.FormattingEnabled = true;
			this.comboBoxColorSet.Location = new System.Drawing.Point(115, 219);
			this.comboBoxColorSet.Name = "comboBoxColorSet";
			this.comboBoxColorSet.Size = new System.Drawing.Size(140, 23);
			this.comboBoxColorSet.TabIndex = 27;
			// 
			// radioButtonOptionFullColor
			// 
			this.radioButtonOptionFullColor.AutoSize = true;
			this.radioButtonOptionFullColor.Location = new System.Drawing.Point(42, 276);
			this.radioButtonOptionFullColor.Name = "radioButtonOptionFullColor";
			this.radioButtonOptionFullColor.Size = new System.Drawing.Size(380, 19);
			this.radioButtonOptionFullColor.TabIndex = 26;
			this.radioButtonOptionFullColor.TabStop = true;
			this.radioButtonOptionFullColor.Text = "They can be any color: they are full RGB and mix to make any color.";
			this.radioButtonOptionFullColor.UseVisualStyleBackColor = true;
			this.radioButtonOptionFullColor.CheckedChanged += new System.EventHandler(this.AnyRadioButtonCheckedChanged);
			// 
			// radioButtonOptionMultiple
			// 
			this.radioButtonOptionMultiple.AutoSize = true;
			this.radioButtonOptionMultiple.Location = new System.Drawing.Point(42, 160);
			this.radioButtonOptionMultiple.Name = "radioButtonOptionMultiple";
			this.radioButtonOptionMultiple.Size = new System.Drawing.Size(431, 19);
			this.radioButtonOptionMultiple.TabIndex = 25;
			this.radioButtonOptionMultiple.TabStop = true;
			this.radioButtonOptionMultiple.Text = "They can be multiple colors, but the colors are independent: they do not mix.";
			this.radioButtonOptionMultiple.UseVisualStyleBackColor = true;
			this.radioButtonOptionMultiple.CheckedChanged += new System.EventHandler(this.AnyRadioButtonCheckedChanged);
			// 
			// radioButtonOptionSingle
			// 
			this.radioButtonOptionSingle.AutoSize = true;
			this.radioButtonOptionSingle.Location = new System.Drawing.Point(42, 63);
			this.radioButtonOptionSingle.Name = "radioButtonOptionSingle";
			this.radioButtonOptionSingle.Size = new System.Drawing.Size(377, 19);
			this.radioButtonOptionSingle.TabIndex = 24;
			this.radioButtonOptionSingle.TabStop = true;
			this.radioButtonOptionSingle.Text = "They are a single color, and do not change color at all. The color is:";
			this.radioButtonOptionSingle.UseVisualStyleBackColor = true;
			this.radioButtonOptionSingle.CheckedChanged += new System.EventHandler(this.AnyRadioButtonCheckedChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(63, 188);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(361, 15);
			this.label2.TabIndex = 30;
			this.label2.Text = "(eg. multiple strings of different colors wrapped around each item.)";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(63, 223);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(44, 15);
			this.label3.TabIndex = 31;
			this.label3.Text = "Colors:";
			// 
			// colorPanelSingleColor
			// 
			this.colorPanelSingleColor.BackColor = System.Drawing.Color.RoyalBlue;
			this.colorPanelSingleColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.colorPanelSingleColor.Color = System.Drawing.Color.RoyalBlue;
			this.colorPanelSingleColor.Enabled = false;
			this.colorPanelSingleColor.Location = new System.Drawing.Point(68, 96);
			this.colorPanelSingleColor.Margin = new System.Windows.Forms.Padding(5);
			this.colorPanelSingleColor.Name = "colorPanelSingleColor";
			this.colorPanelSingleColor.Size = new System.Drawing.Size(70, 34);
			this.colorPanelSingleColor.TabIndex = 29;
			// 
			// comboBoxColorOrder
			// 
			this.comboBoxColorOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxColorOrder.Enabled = false;
			this.comboBoxColorOrder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.comboBoxColorOrder.FormattingEnabled = true;
			this.comboBoxColorOrder.Items.AddRange(new object[] {
            "RGB",
            "RBG",
            "GBR",
            "GRB",
            "BRG",
            "BGR"});
			this.comboBoxColorOrder.Location = new System.Drawing.Point(141, 301);
			this.comboBoxColorOrder.Name = "comboBoxColorOrder";
			this.comboBoxColorOrder.Size = new System.Drawing.Size(114, 23);
			this.comboBoxColorOrder.TabIndex = 32;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(63, 304);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 15);
			this.label4.TabIndex = 33;
			this.label4.Text = "Color Order:";
			// 
			// ColorSetupHelper
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(541, 372);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.comboBoxColorOrder);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.colorPanelSingleColor);
			this.Controls.Add(this.buttonColorSetsSetup);
			this.Controls.Add(this.comboBoxColorSet);
			this.Controls.Add(this.radioButtonOptionFullColor);
			this.Controls.Add(this.radioButtonOptionMultiple);
			this.Controls.Add(this.radioButtonOptionSingle);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(557, 410);
			this.Name = "ColorSetupHelper";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Color Configuration";
			this.Load += new System.EventHandler(this.ColorSetupHelper_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

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