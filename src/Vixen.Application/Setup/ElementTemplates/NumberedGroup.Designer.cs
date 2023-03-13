namespace VixenApplication.Setup.ElementTemplates
{
	partial class NumberedGroup
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
			this.textBoxGroupName = new System.Windows.Forms.TextBox();
			this.textBoxItemPrefix = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.numericUpDownItemCount = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownItemCount)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(225, 140);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(105, 29);
			this.buttonCancel.TabIndex = 4;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(113, 140);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(105, 29);
			this.buttonOk.TabIndex = 3;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonOk.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOk.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(14, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(78, 15);
			this.label1.TabIndex = 5;
			this.label1.Text = "Group Name:";
			// 
			// textBoxGroupName
			// 
			this.textBoxGroupName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxGroupName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxGroupName.Location = new System.Drawing.Point(114, 29);
			this.textBoxGroupName.Name = "textBoxGroupName";
			this.textBoxGroupName.Size = new System.Drawing.Size(215, 23);
			this.textBoxGroupName.TabIndex = 0;
			this.textBoxGroupName.Text = "Minitrees";
			// 
			// textBoxItemPrefix
			// 
			this.textBoxItemPrefix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxItemPrefix.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxItemPrefix.Location = new System.Drawing.Point(114, 63);
			this.textBoxItemPrefix.Name = "textBoxItemPrefix";
			this.textBoxItemPrefix.Size = new System.Drawing.Size(215, 23);
			this.textBoxItemPrefix.TabIndex = 1;
			this.textBoxItemPrefix.Text = "Tree";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(14, 67);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(66, 15);
			this.label2.TabIndex = 7;
			this.label2.Text = "Item Prefix:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(14, 97);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(70, 15);
			this.label3.TabIndex = 9;
			this.label3.Text = "Item Count:";
			// 
			// numericUpDownItemCount
			// 
			this.numericUpDownItemCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.numericUpDownItemCount.Location = new System.Drawing.Point(114, 93);
			this.numericUpDownItemCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownItemCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownItemCount.Name = "numericUpDownItemCount";
			this.numericUpDownItemCount.Size = new System.Drawing.Size(91, 23);
			this.numericUpDownItemCount.TabIndex = 2;
			this.numericUpDownItemCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// NumberedGroup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(344, 183);
			this.Controls.Add(this.numericUpDownItemCount);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textBoxItemPrefix);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBoxGroupName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(360, 221);
			this.Name = "NumberedGroup";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Group Setup";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NumberedGroup_FormClosed);
			this.Load += new System.EventHandler(this.NumberedGroup_Load);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownItemCount)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxGroupName;
		private System.Windows.Forms.TextBox textBoxItemPrefix;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown numericUpDownItemCount;
	}
}