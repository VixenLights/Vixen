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
			this.colorPanelSingleColor = new VixenModules.Property.Color.ColorPanel();
			this.buttonColorSetsSetup = new System.Windows.Forms.Button();
			this.comboBoxColorSet = new System.Windows.Forms.ComboBox();
			this.radioButtonOptionFullColor = new System.Windows.Forms.RadioButton();
			this.radioButtonOptionMultiple = new System.Windows.Forms.RadioButton();
			this.radioButtonOptionSingle = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(362, 285);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(90, 25);
			this.buttonCancel.TabIndex = 22;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Enabled = false;
			this.buttonOk.Location = new System.Drawing.Point(266, 285);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(90, 25);
			this.buttonOk.TabIndex = 21;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(12, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(241, 16);
			this.label1.TabIndex = 23;
			this.label1.Text = "How do these items handle color?";
			// 
			// colorPanelSingleColor
			// 
			this.colorPanelSingleColor.BackColor = System.Drawing.Color.RoyalBlue;
			this.colorPanelSingleColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.colorPanelSingleColor.Color = System.Drawing.Color.RoyalBlue;
			this.colorPanelSingleColor.Enabled = false;
			this.colorPanelSingleColor.Location = new System.Drawing.Point(58, 83);
			this.colorPanelSingleColor.Margin = new System.Windows.Forms.Padding(4);
			this.colorPanelSingleColor.Name = "colorPanelSingleColor";
			this.colorPanelSingleColor.Size = new System.Drawing.Size(60, 30);
			this.colorPanelSingleColor.TabIndex = 29;
			// 
			// buttonColorSetsSetup
			// 
			this.buttonColorSetsSetup.Enabled = false;
			this.buttonColorSetsSetup.Location = new System.Drawing.Point(237, 188);
			this.buttonColorSetsSetup.Name = "buttonColorSetsSetup";
			this.buttonColorSetsSetup.Size = new System.Drawing.Size(75, 23);
			this.buttonColorSetsSetup.TabIndex = 28;
			this.buttonColorSetsSetup.Text = "Edit Colors";
			this.buttonColorSetsSetup.UseVisualStyleBackColor = true;
			this.buttonColorSetsSetup.Click += new System.EventHandler(this.buttonColorSetsSetup_Click);
			// 
			// comboBoxColorSet
			// 
			this.comboBoxColorSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxColorSet.Enabled = false;
			this.comboBoxColorSet.FormattingEnabled = true;
			this.comboBoxColorSet.Location = new System.Drawing.Point(99, 190);
			this.comboBoxColorSet.Name = "comboBoxColorSet";
			this.comboBoxColorSet.Size = new System.Drawing.Size(121, 21);
			this.comboBoxColorSet.TabIndex = 27;
			// 
			// radioButtonOptionFullColor
			// 
			this.radioButtonOptionFullColor.AutoSize = true;
			this.radioButtonOptionFullColor.Location = new System.Drawing.Point(36, 239);
			this.radioButtonOptionFullColor.Name = "radioButtonOptionFullColor";
			this.radioButtonOptionFullColor.Size = new System.Drawing.Size(346, 17);
			this.radioButtonOptionFullColor.TabIndex = 26;
			this.radioButtonOptionFullColor.TabStop = true;
			this.radioButtonOptionFullColor.Text = "They can be any color: they are full RGB and mix to make any color.";
			this.radioButtonOptionFullColor.UseVisualStyleBackColor = true;
			this.radioButtonOptionFullColor.CheckedChanged += new System.EventHandler(this.AnyRadioButtonCheckedChanged);
			// 
			// radioButtonOptionMultiple
			// 
			this.radioButtonOptionMultiple.AutoSize = true;
			this.radioButtonOptionMultiple.Location = new System.Drawing.Point(36, 139);
			this.radioButtonOptionMultiple.Name = "radioButtonOptionMultiple";
			this.radioButtonOptionMultiple.Size = new System.Drawing.Size(384, 17);
			this.radioButtonOptionMultiple.TabIndex = 25;
			this.radioButtonOptionMultiple.TabStop = true;
			this.radioButtonOptionMultiple.Text = "They can be multiple colors, but the colors are independent: they do not mix.";
			this.radioButtonOptionMultiple.UseVisualStyleBackColor = true;
			this.radioButtonOptionMultiple.CheckedChanged += new System.EventHandler(this.AnyRadioButtonCheckedChanged);
			// 
			// radioButtonOptionSingle
			// 
			this.radioButtonOptionSingle.AutoSize = true;
			this.radioButtonOptionSingle.Location = new System.Drawing.Point(36, 55);
			this.radioButtonOptionSingle.Name = "radioButtonOptionSingle";
			this.radioButtonOptionSingle.Size = new System.Drawing.Size(343, 17);
			this.radioButtonOptionSingle.TabIndex = 24;
			this.radioButtonOptionSingle.TabStop = true;
			this.radioButtonOptionSingle.Text = "They are a single color, and do not change color at all. The color is:";
			this.radioButtonOptionSingle.UseVisualStyleBackColor = true;
			this.radioButtonOptionSingle.CheckedChanged += new System.EventHandler(this.AnyRadioButtonCheckedChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(54, 163);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(315, 13);
			this.label2.TabIndex = 30;
			this.label2.Text = "(eg. multiple strings of different colors wrapped around each item.)";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(54, 193);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(39, 13);
			this.label3.TabIndex = 31;
			this.label3.Text = "Colors:";
			// 
			// ColorSetupHelper
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(464, 322);
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
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(480, 360);
			this.MinimizeBox = false;
			this.Name = "ColorSetupHelper";
			this.ShowIcon = false;
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
	}
}