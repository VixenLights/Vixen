﻿using System.Windows.Forms;
using Common.Controls.Theme;
namespace VixenApplication.Setup.ElementTemplates
{
	partial class PixelGrid
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
			this.numericUpDownHeight = new System.Windows.Forms.NumericUpDown();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			this.numericUpDownWidth = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.radioButtonHorizontalFirst = new System.Windows.Forms.RadioButton();
			this.radioButtonVerticalFirst = new System.Windows.Forms.RadioButton();
			this.groupBoxStringOrientation = new System.Windows.Forms.GroupBox();
			this.groupBoxDimensions = new System.Windows.Forms.GroupBox();
			this.textBoxSecondPrefix = new System.Windows.Forms.TextBox();
			this.textBoxFirstPrefix = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lblEveryValue = new System.Windows.Forms.Label();
			this.chkZigZag = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.radioTopLeft = new System.Windows.Forms.RadioButton();
			this.radioBottomRight = new System.Windows.Forms.RadioButton();
			this.radioBottomLeft = new System.Windows.Forms.RadioButton();
			this.radioTopRight = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).BeginInit();
			this.groupBoxStringOrientation.SuspendLayout();
			this.groupBoxDimensions.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// numericUpDownHeight
			// 
			this.numericUpDownHeight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.numericUpDownHeight.Location = new System.Drawing.Point(55, 22);
			this.numericUpDownHeight.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownHeight.Name = "numericUpDownHeight";
			this.numericUpDownHeight.Size = new System.Drawing.Size(66, 23);
			this.numericUpDownHeight.TabIndex = 1;
			this.numericUpDownHeight.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
			this.numericUpDownHeight.ValueChanged += new System.EventHandler(this.numericUpDownHeight_ValueChanged);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(454, 283);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(105, 29);
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(342, 283);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(105, 29);
			this.buttonOk.TabIndex = 5;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonOk.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOk.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// numericUpDownWidth
			// 
			this.numericUpDownWidth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.numericUpDownWidth.Location = new System.Drawing.Point(184, 22);
			this.numericUpDownWidth.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownWidth.Name = "numericUpDownWidth";
			this.numericUpDownWidth.Size = new System.Drawing.Size(66, 23);
			this.numericUpDownWidth.TabIndex = 2;
			this.numericUpDownWidth.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
			this.numericUpDownWidth.ValueChanged += new System.EventHandler(this.numericUpDownWidth_ValueChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(139, 24);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(39, 15);
			this.label4.TabIndex = 27;
			this.label4.Text = "Width";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 24);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(43, 15);
			this.label5.TabIndex = 26;
			this.label5.Text = "Height";
			// 
			// textBoxName
			// 
			this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxName.Location = new System.Drawing.Point(62, 20);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(485, 23);
			this.textBoxName.TabIndex = 0;
			this.textBoxName.Text = "Grid 1";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(14, 23);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(39, 15);
			this.label6.TabIndex = 24;
			this.label6.Text = "Name";
			// 
			// radioButtonHorizontalFirst
			// 
			this.radioButtonHorizontalFirst.AutoSize = true;
			this.radioButtonHorizontalFirst.Location = new System.Drawing.Point(29, 22);
			this.radioButtonHorizontalFirst.Name = "radioButtonHorizontalFirst";
			this.radioButtonHorizontalFirst.Size = new System.Drawing.Size(80, 19);
			this.radioButtonHorizontalFirst.TabIndex = 3;
			this.radioButtonHorizontalFirst.TabStop = true;
			this.radioButtonHorizontalFirst.Text = "Horizontal";
			this.radioButtonHorizontalFirst.UseVisualStyleBackColor = true;
			// 
			// radioButtonVerticalFirst
			// 
			this.radioButtonVerticalFirst.AutoSize = true;
			this.radioButtonVerticalFirst.Location = new System.Drawing.Point(142, 22);
			this.radioButtonVerticalFirst.Name = "radioButtonVerticalFirst";
			this.radioButtonVerticalFirst.Size = new System.Drawing.Size(63, 19);
			this.radioButtonVerticalFirst.TabIndex = 4;
			this.radioButtonVerticalFirst.TabStop = true;
			this.radioButtonVerticalFirst.Text = "Vertical";
			this.radioButtonVerticalFirst.UseVisualStyleBackColor = true;
			this.radioButtonVerticalFirst.CheckedChanged += new System.EventHandler(this.radioButtonOrientation_CheckedChanged);
			// 
			// groupBoxStringOrientation
			// 
			this.groupBoxStringOrientation.AutoSize = true;
			this.groupBoxStringOrientation.Controls.Add(this.radioButtonHorizontalFirst);
			this.groupBoxStringOrientation.Controls.Add(this.radioButtonVerticalFirst);
			this.groupBoxStringOrientation.Location = new System.Drawing.Point(26, 51);
			this.groupBoxStringOrientation.Name = "groupBoxStringOrientation";
			this.groupBoxStringOrientation.Size = new System.Drawing.Size(261, 67);
			this.groupBoxStringOrientation.TabIndex = 28;
			this.groupBoxStringOrientation.TabStop = false;
			this.groupBoxStringOrientation.Text = "String Orientation";
			this.groupBoxStringOrientation.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// groupBoxDimensions
			// 
			this.groupBoxDimensions.AutoSize = true;
			this.groupBoxDimensions.Controls.Add(this.numericUpDownHeight);
			this.groupBoxDimensions.Controls.Add(this.label5);
			this.groupBoxDimensions.Controls.Add(this.numericUpDownWidth);
			this.groupBoxDimensions.Controls.Add(this.label4);
			this.groupBoxDimensions.Location = new System.Drawing.Point(293, 51);
			this.groupBoxDimensions.Name = "groupBoxDimensions";
			this.groupBoxDimensions.Size = new System.Drawing.Size(270, 67);
			this.groupBoxDimensions.TabIndex = 29;
			this.groupBoxDimensions.TabStop = false;
			this.groupBoxDimensions.Text = "Dimensions";
			this.groupBoxDimensions.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// textBoxSecondPrefix
			// 
			this.textBoxSecondPrefix.Location = new System.Drawing.Point(378, 130);
			this.textBoxSecondPrefix.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.textBoxSecondPrefix.Name = "textBoxSecondPrefix";
			this.textBoxSecondPrefix.Size = new System.Drawing.Size(120, 23);
			this.textBoxSecondPrefix.TabIndex = 33;
			this.textBoxSecondPrefix.Text = "C";
			// 
			// textBoxFirstPrefix
			// 
			this.textBoxFirstPrefix.Location = new System.Drawing.Point(111, 129);
			this.textBoxFirstPrefix.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.textBoxFirstPrefix.Name = "textBoxFirstPrefix";
			this.textBoxFirstPrefix.Size = new System.Drawing.Size(120, 23);
			this.textBoxFirstPrefix.TabIndex = 32;
			this.textBoxFirstPrefix.Text = "R";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(290, 132);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(68, 15);
			this.label3.TabIndex = 31;
			this.label3.Text = "Pixel Prefix:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(23, 132);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(74, 15);
			this.label2.TabIndex = 30;
			this.label2.Text = "String Prefix:";
			// 
			// groupBox1
			// 
			this.groupBox1.AutoSize = true;
			this.groupBox1.Controls.Add(this.lblEveryValue);
			this.groupBox1.Controls.Add(this.chkZigZag);
			this.groupBox1.Location = new System.Drawing.Point(293, 172);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(268, 88);
			this.groupBox1.TabIndex = 30;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Patching ";
			this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// lblEveryValue
			// 
			this.lblEveryValue.AutoSize = true;
			this.lblEveryValue.Location = new System.Drawing.Point(115, 37);
			this.lblEveryValue.Name = "lblEveryValue";
			this.lblEveryValue.Size = new System.Drawing.Size(13, 15);
			this.lblEveryValue.TabIndex = 29;
			this.lblEveryValue.Text = "0";
			// 
			// chkZigZag
			// 
			this.chkZigZag.AutoSize = true;
			this.chkZigZag.Location = new System.Drawing.Point(12, 36);
			this.chkZigZag.Name = "chkZigZag";
			this.chkZigZag.Size = new System.Drawing.Size(97, 19);
			this.chkZigZag.TabIndex = 28;
			this.chkZigZag.Text = "Zig Zag Every";
			this.chkZigZag.UseVisualStyleBackColor = true;
			this.chkZigZag.CheckedChanged += new System.EventHandler(this.chkZigZag_CheckedChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.radioTopLeft);
			this.groupBox2.Controls.Add(this.radioBottomRight);
			this.groupBox2.Controls.Add(this.radioBottomLeft);
			this.groupBox2.Controls.Add(this.radioTopRight);
			this.groupBox2.Location = new System.Drawing.Point(26, 172);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(261, 88);
			this.groupBox2.TabIndex = 34;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Wire Start Location";
			this.groupBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// radioTopLeft
			// 
			this.radioTopLeft.AutoSize = true;
			this.radioTopLeft.Location = new System.Drawing.Point(6, 22);
			this.radioTopLeft.Name = "radioTopLeft";
			this.radioTopLeft.Size = new System.Drawing.Size(67, 19);
			this.radioTopLeft.TabIndex = 30;
			this.radioTopLeft.TabStop = true;
			this.radioTopLeft.Text = "Top Left";
			this.radioTopLeft.UseVisualStyleBackColor = true;
			// 
			// radioBottomRight
			// 
			this.radioBottomRight.AutoSize = true;
			this.radioBottomRight.Location = new System.Drawing.Point(119, 47);
			this.radioBottomRight.Name = "radioBottomRight";
			this.radioBottomRight.Size = new System.Drawing.Size(96, 19);
			this.radioBottomRight.TabIndex = 33;
			this.radioBottomRight.TabStop = true;
			this.radioBottomRight.Text = "Bottom Right";
			this.radioBottomRight.UseVisualStyleBackColor = true;
			// 
			// radioBottomLeft
			// 
			this.radioBottomLeft.AutoSize = true;
			this.radioBottomLeft.Location = new System.Drawing.Point(6, 47);
			this.radioBottomLeft.Name = "radioBottomLeft";
			this.radioBottomLeft.Size = new System.Drawing.Size(88, 19);
			this.radioBottomLeft.TabIndex = 31;
			this.radioBottomLeft.TabStop = true;
			this.radioBottomLeft.Text = "Bottom Left";
			this.radioBottomLeft.UseVisualStyleBackColor = true;
			// 
			// radioTopRight
			// 
			this.radioTopRight.AutoSize = true;
			this.radioTopRight.Location = new System.Drawing.Point(119, 22);
			this.radioTopRight.Name = "radioTopRight";
			this.radioTopRight.Size = new System.Drawing.Size(75, 19);
			this.radioTopRight.TabIndex = 32;
			this.radioTopRight.TabStop = true;
			this.radioTopRight.Text = "Top Right";
			this.radioTopRight.UseVisualStyleBackColor = true;
			// 
			// PixelGrid
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(575, 326);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.textBoxSecondPrefix);
			this.Controls.Add(this.textBoxFirstPrefix);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.groupBoxDimensions);
			this.Controls.Add(this.groupBoxStringOrientation);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.textBoxName);
			this.Controls.Add(this.label6);
			this.DoubleBuffered = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PixelGrid";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Pixel Grid / Matrix Setup";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PixelGrid_FormClosed);
			this.Load += new System.EventHandler(this.PixelGrid_Load);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).EndInit();
			this.groupBoxStringOrientation.ResumeLayout(false);
			this.groupBoxStringOrientation.PerformLayout();
			this.groupBoxDimensions.ResumeLayout(false);
			this.groupBoxDimensions.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}

		#endregion

		private System.Windows.Forms.NumericUpDown numericUpDownHeight;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.NumericUpDown numericUpDownWidth;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBoxName;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.RadioButton radioButtonHorizontalFirst;
		private System.Windows.Forms.RadioButton radioButtonVerticalFirst;
		private System.Windows.Forms.GroupBox groupBoxStringOrientation;
		private System.Windows.Forms.GroupBox groupBoxDimensions;
		private TextBox textBoxSecondPrefix;
		private TextBox textBoxFirstPrefix;
		private Label label3;
		private Label label2;
		private GroupBox groupBox1;
		private CheckBox chkZigZag;
		private GroupBox groupBox2;
		private RadioButton radioBottomRight;
		private RadioButton radioTopRight;
		private RadioButton radioBottomLeft;
		private RadioButton radioTopLeft;
		private Label lblEveryValue;
	}
}