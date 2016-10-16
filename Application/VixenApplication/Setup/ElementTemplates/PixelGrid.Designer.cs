using System.Windows.Forms;
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
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).BeginInit();
			this.groupBoxStringOrientation.SuspendLayout();
			this.groupBoxDimensions.SuspendLayout();
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
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(187, 200);
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
			this.buttonOk.Location = new System.Drawing.Point(74, 200);
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
			this.textBoxName.Location = new System.Drawing.Point(87, 20);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(200, 23);
			this.textBoxName.TabIndex = 0;
			this.textBoxName.Text = "Grid";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(14, 23);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(67, 15);
			this.label6.TabIndex = 24;
			this.label6.Text = "Grid Name:";
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
			this.radioButtonVerticalFirst.Size = new System.Drawing.Size(64, 19);
			this.radioButtonVerticalFirst.TabIndex = 4;
			this.radioButtonVerticalFirst.TabStop = true;
			this.radioButtonVerticalFirst.Text = "Vertical";
			this.radioButtonVerticalFirst.UseVisualStyleBackColor = true;
			// 
			// groupBoxStringOrientation
			// 
			this.groupBoxStringOrientation.AutoSize = true;
			this.groupBoxStringOrientation.Controls.Add(this.radioButtonHorizontalFirst);
			this.groupBoxStringOrientation.Controls.Add(this.radioButtonVerticalFirst);
			this.groupBoxStringOrientation.Location = new System.Drawing.Point(17, 51);
			this.groupBoxStringOrientation.Name = "groupBoxStringOrientation";
			this.groupBoxStringOrientation.Size = new System.Drawing.Size(270, 63);
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
			this.groupBoxDimensions.Location = new System.Drawing.Point(17, 114);
			this.groupBoxDimensions.Name = "groupBoxDimensions";
			this.groupBoxDimensions.Size = new System.Drawing.Size(270, 67);
			this.groupBoxDimensions.TabIndex = 29;
			this.groupBoxDimensions.TabStop = false;
			this.groupBoxDimensions.Text = "Dimensions";
			this.groupBoxDimensions.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// PixelGrid
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(304, 241);
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
			this.Text = "Pixel Grid Setup";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PixelGrid_FormClosed);
			this.Load += new System.EventHandler(this.PixelGrid_Load);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).EndInit();
			this.groupBoxStringOrientation.ResumeLayout(false);
			this.groupBoxStringOrientation.PerformLayout();
			this.groupBoxDimensions.ResumeLayout(false);
			this.groupBoxDimensions.PerformLayout();
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
	}
}