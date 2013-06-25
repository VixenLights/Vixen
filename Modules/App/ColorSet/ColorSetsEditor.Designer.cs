namespace VixenModules.App.ColorSets
{
	partial class ColorSetsEditor
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
			this.listViewColorSets = new System.Windows.Forms.ListView();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxColorSetName = new System.Windows.Forms.TextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.listViewColorSetColors = new System.Windows.Forms.ListView();
			this.label3 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// listViewColorSets
			// 
			this.listViewColorSets.Location = new System.Drawing.Point(12, 34);
			this.listViewColorSets.Name = "listViewColorSets";
			this.listViewColorSets.Size = new System.Drawing.Size(165, 296);
			this.listViewColorSets.TabIndex = 0;
			this.listViewColorSets.UseCompatibleStateImageBehavior = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(58, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Color Sets:";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.textBox1);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.textBoxColorSetName);
			this.groupBox1.Controls.Add(this.panel1);
			this.groupBox1.Controls.Add(this.listViewColorSetColors);
			this.groupBox1.Location = new System.Drawing.Point(194, 34);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(476, 446);
			this.groupBox1.TabIndex = 6;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "groupBox1";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(131, 60);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(84, 13);
			this.label2.TabIndex = 9;
			this.label2.Text = "Color Set Name:";
			// 
			// textBoxColorSetName
			// 
			this.textBoxColorSetName.Location = new System.Drawing.Point(248, 60);
			this.textBoxColorSetName.Name = "textBoxColorSetName";
			this.textBoxColorSetName.Size = new System.Drawing.Size(100, 20);
			this.textBoxColorSetName.TabIndex = 8;
			this.textBoxColorSetName.TextChanged += new System.EventHandler(this.textBoxColorSetName_TextChanged);
			// 
			// panel1
			// 
			this.panel1.Location = new System.Drawing.Point(328, 278);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(80, 80);
			this.panel1.TabIndex = 7;
			// 
			// listViewColorSetColors
			// 
			this.listViewColorSetColors.Location = new System.Drawing.Point(134, 133);
			this.listViewColorSetColors.Name = "listViewColorSetColors";
			this.listViewColorSetColors.Size = new System.Drawing.Size(261, 110);
			this.listViewColorSetColors.TabIndex = 6;
			this.listViewColorSetColors.UseCompatibleStateImageBehavior = false;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(131, 100);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(39, 13);
			this.label3.TabIndex = 10;
			this.label3.Text = "Colors:";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(202, 291);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(100, 20);
			this.textBox1.TabIndex = 11;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(97, 294);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(84, 13);
			this.label4.TabIndex = 12;
			this.label4.Text = "Color Set Name:";
			// 
			// ColorSetsEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(695, 554);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listViewColorSets);
			this.Name = "ColorSetsEditor";
			this.Text = "ColorSetsEditor";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListView listViewColorSets;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxColorSetName;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ListView listViewColorSetColors;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBox1;
	}
}