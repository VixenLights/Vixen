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
			buttonCancel = new System.Windows.Forms.Button();
			buttonOk = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			textBoxGroupName = new System.Windows.Forms.TextBox();
			textBoxItemPrefix = new System.Windows.Forms.TextBox();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			numericUpDownItemCount = new System.Windows.Forms.NumericUpDown();
			inputPanel = new System.Windows.Forms.TableLayoutPanel();
			btnPanel = new System.Windows.Forms.FlowLayoutPanel();
			((System.ComponentModel.ISupportInitialize)numericUpDownItemCount).BeginInit();
			inputPanel.SuspendLayout();
			btnPanel.SuspendLayout();
			SuspendLayout();
			// 
			// buttonCancel
			// 
			buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
			buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			buttonCancel.Location = new System.Drawing.Point(230, 5);
			buttonCancel.Name = "buttonCancel";
			buttonCancel.Size = new System.Drawing.Size(105, 29);
			buttonCancel.TabIndex = 4;
			buttonCancel.Text = "Cancel";
			buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOk
			// 
			buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
			buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			buttonOk.Location = new System.Drawing.Point(119, 3);
			buttonOk.Name = "buttonOk";
			buttonOk.Size = new System.Drawing.Size(105, 31);
			buttonOk.TabIndex = 3;
			buttonOk.Text = "OK";
			buttonOk.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(13, 17);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(78, 15);
			label1.TabIndex = 5;
			label1.Text = "Group Name:";
			// 
			// textBoxGroupName
			// 
			textBoxGroupName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			textBoxGroupName.Location = new System.Drawing.Point(97, 13);
			textBoxGroupName.Name = "textBoxGroupName";
			textBoxGroupName.Size = new System.Drawing.Size(234, 23);
			textBoxGroupName.TabIndex = 0;
			textBoxGroupName.Text = "Minitrees";
			// 
			// textBoxItemPrefix
			// 
			textBoxItemPrefix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			textBoxItemPrefix.Location = new System.Drawing.Point(97, 42);
			textBoxItemPrefix.Name = "textBoxItemPrefix";
			textBoxItemPrefix.Size = new System.Drawing.Size(234, 23);
			textBoxItemPrefix.TabIndex = 1;
			textBoxItemPrefix.Text = "Tree";
			// 
			// label2
			// 
			label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(13, 46);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(66, 15);
			label2.TabIndex = 7;
			label2.Text = "Item Prefix:";
			// 
			// label3
			// 
			label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(13, 75);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(70, 15);
			label3.TabIndex = 9;
			label3.Text = "Item Count:";
			// 
			// numericUpDownItemCount
			// 
			numericUpDownItemCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			numericUpDownItemCount.Location = new System.Drawing.Point(97, 71);
			numericUpDownItemCount.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
			numericUpDownItemCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
			numericUpDownItemCount.Name = "numericUpDownItemCount";
			numericUpDownItemCount.Size = new System.Drawing.Size(91, 23);
			numericUpDownItemCount.TabIndex = 2;
			numericUpDownItemCount.Value = new decimal(new int[] { 1, 0, 0, 0 });
			// 
			// inputPanel
			// 
			inputPanel.ColumnCount = 2;
			inputPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			inputPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			inputPanel.Controls.Add(label1, 0, 0);
			inputPanel.Controls.Add(numericUpDownItemCount, 1, 2);
			inputPanel.Controls.Add(textBoxGroupName, 1, 0);
			inputPanel.Controls.Add(label3, 0, 2);
			inputPanel.Controls.Add(label2, 0, 1);
			inputPanel.Controls.Add(textBoxItemPrefix, 1, 1);
			inputPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			inputPanel.Location = new System.Drawing.Point(0, 0);
			inputPanel.Name = "inputPanel";
			inputPanel.Padding = new System.Windows.Forms.Padding(10, 10, 10, 0);
			inputPanel.RowCount = 4;
			inputPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			inputPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			inputPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			inputPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			inputPanel.Size = new System.Drawing.Size(344, 193);
			inputPanel.TabIndex = 10;
			// 
			// btnPanel
			// 
			btnPanel.AutoSize = true;
			btnPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			btnPanel.Controls.Add(buttonCancel);
			btnPanel.Controls.Add(buttonOk);
			btnPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			btnPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			btnPanel.Location = new System.Drawing.Point(0, 156);
			btnPanel.Name = "btnPanel";
			btnPanel.Padding = new System.Windows.Forms.Padding(0, 0, 6, 0);
			btnPanel.Size = new System.Drawing.Size(344, 37);
			btnPanel.TabIndex = 11;
			// 
			// NumberedGroup
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			AutoSize = true;
			ClientSize = new System.Drawing.Size(344, 193);
			Controls.Add(btnPanel);
			Controls.Add(inputPanel);
			DoubleBuffered = true;
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			MinimumSize = new System.Drawing.Size(360, 221);
			ShowInTaskbar = false;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Group Setup";
			FormClosed += NumberedGroup_FormClosed;
			Load += NumberedGroup_Load;
			((System.ComponentModel.ISupportInitialize)numericUpDownItemCount).EndInit();
			inputPanel.ResumeLayout(false);
			inputPanel.PerformLayout();
			btnPanel.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		private System.Windows.Forms.FlowLayoutPanel btnPanel;

		private System.Windows.Forms.TableLayoutPanel inputPanel;

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