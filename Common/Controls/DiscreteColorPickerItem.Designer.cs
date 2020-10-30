namespace Common.Controls
{
	partial class DiscreteColorPickerItem
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
			this.checkBoxSelected = new System.Windows.Forms.CheckBox();
			this.panelColor = new System.Windows.Forms.Panel();
			this.radioButtonSelected = new System.Windows.Forms.RadioButton();
			this.SuspendLayout();
			// 
			// checkBoxSelected
			// 
			this.checkBoxSelected.AutoSize = true;
			this.checkBoxSelected.Location = new System.Drawing.Point(8, 17);
			this.checkBoxSelected.Name = "checkBoxSelected";
			this.checkBoxSelected.Size = new System.Drawing.Size(15, 14);
			this.checkBoxSelected.TabIndex = 0;
			this.checkBoxSelected.UseVisualStyleBackColor = true;
			this.checkBoxSelected.CheckedChanged += new System.EventHandler(this.checkBoxSelected_CheckedChanged);
			// 
			// panelColor
			// 
			this.panelColor.BackColor = System.Drawing.Color.White;
			this.panelColor.Location = new System.Drawing.Point(32, 3);
			this.panelColor.Name = "panelColor";
			this.panelColor.Size = new System.Drawing.Size(40, 40);
			this.panelColor.TabIndex = 1;
			this.panelColor.Click += new System.EventHandler(this.panelColor_Click);
			// 
			// radioButtonSelected
			// 
			this.radioButtonSelected.AutoSize = true;
			this.radioButtonSelected.Location = new System.Drawing.Point(8, 17);
			this.radioButtonSelected.Name = "radioButtonSelected";
			this.radioButtonSelected.Size = new System.Drawing.Size(14, 13);
			this.radioButtonSelected.TabIndex = 2;
			this.radioButtonSelected.TabStop = true;
			this.radioButtonSelected.UseVisualStyleBackColor = true;
			this.radioButtonSelected.Visible = false;
			this.radioButtonSelected.CheckedChanged += new System.EventHandler(this.radioButtonSelected_CheckedChanged);
			// 
			// DiscreteColorPickerItem
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.radioButtonSelected);
			this.Controls.Add(this.panelColor);
			this.Controls.Add(this.checkBoxSelected);
			this.DoubleBuffered = true;
			this.Name = "DiscreteColorPickerItem";
			this.Size = new System.Drawing.Size(76, 46);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBoxSelected;
		private System.Windows.Forms.Panel panelColor;
		private System.Windows.Forms.RadioButton radioButtonSelected;
	}
}
