namespace VixenModules.OutputFilter.ColorBreakdown
{
	partial class ColorBreakdownItemControl
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
			this.panelColor = new System.Windows.Forms.Panel();
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// panelColor
			// 
			this.panelColor.BackColor = System.Drawing.Color.White;
			this.panelColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelColor.Location = new System.Drawing.Point(3, 3);
			this.panelColor.Name = "panelColor";
			this.panelColor.Size = new System.Drawing.Size(60, 40);
			this.panelColor.TabIndex = 0;
			this.panelColor.Click += new System.EventHandler(this.panelColor_Click);
			// 
			// textBoxName
			// 
			this.textBoxName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxName.Location = new System.Drawing.Point(112, 13);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(100, 20);
			this.textBoxName.TabIndex = 1;
			this.textBoxName.Text = "New Color";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(71, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Name:";
			// 
			// buttonDelete
			// 
			this.buttonDelete.Location = new System.Drawing.Point(218, 10);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(75, 25);
			this.buttonDelete.TabIndex = 3;
			this.buttonDelete.Text = "Delete";
			this.buttonDelete.UseVisualStyleBackColor = true;
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			this.buttonDelete.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
			this.buttonDelete.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonDelete.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// ColorBreakdownItemControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.buttonDelete);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxName);
			this.Controls.Add(this.panelColor);
			this.DoubleBuffered = true;
			this.Name = "ColorBreakdownItemControl";
			this.Size = new System.Drawing.Size(300, 46);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel panelColor;
		private System.Windows.Forms.TextBox textBoxName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonDelete;
	}
}
