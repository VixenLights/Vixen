namespace VixenModules.Preview.VixenPreview
{
    partial class ResizePreviewForm
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
            if (disposing && (components != null))
            {
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
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResizePreviewForm));
			groupBox1 = new GroupBox();
			labelWidth = new Label();
			labelHeight = new Label();
			label3 = new Label();
			label4 = new Label();
			groupBox2 = new GroupBox();
			numericHeight = new NumericUpDown();
			label1 = new Label();
			label2 = new Label();
			numericWidth = new NumericUpDown();
			buttonCancel = new Button();
			buttonOK = new Button();
			buttonHelp = new Button();
			imageListLocks = new ImageList(components);
			_scaleShapes = new CheckBox();
			pictureBoxLock = new PictureBox();
			groupBox1.SuspendLayout();
			groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)numericHeight).BeginInit();
			((System.ComponentModel.ISupportInitialize)numericWidth).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxLock).BeginInit();
			SuspendLayout();
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(labelWidth);
			groupBox1.Controls.Add(labelHeight);
			groupBox1.Controls.Add(label3);
			groupBox1.Controls.Add(label4);
			groupBox1.Location = new Point(14, 14);
			groupBox1.Margin = new Padding(4, 3, 4, 3);
			groupBox1.Name = "groupBox1";
			groupBox1.Padding = new Padding(4, 3, 4, 3);
			groupBox1.Size = new Size(279, 63);
			groupBox1.TabIndex = 0;
			groupBox1.TabStop = false;
			groupBox1.Text = "Existing Image Size";
			// 
			// labelWidth
			// 
			labelWidth.AutoSize = true;
			labelWidth.Location = new Point(55, 29);
			labelWidth.Margin = new Padding(4, 0, 4, 0);
			labelWidth.Name = "labelWidth";
			labelWidth.Size = new Size(42, 15);
			labelWidth.TabIndex = 5;
			labelWidth.Text = "Width:";
			// 
			// labelHeight
			// 
			labelHeight.AutoSize = true;
			labelHeight.Location = new Point(192, 29);
			labelHeight.Margin = new Padding(4, 0, 4, 0);
			labelHeight.Name = "labelHeight";
			labelHeight.Size = new Size(43, 15);
			labelHeight.TabIndex = 6;
			labelHeight.Text = "Height";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new Point(7, 29);
			label3.Margin = new Padding(4, 0, 4, 0);
			label3.Name = "label3";
			label3.Size = new Size(42, 15);
			label3.TabIndex = 3;
			label3.Text = "Width:";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new Point(145, 29);
			label4.Margin = new Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new Size(46, 15);
			label4.TabIndex = 4;
			label4.Text = "Height:";
			// 
			// groupBox2
			// 
			groupBox2.Controls.Add(pictureBoxLock);
			groupBox2.Controls.Add(numericHeight);
			groupBox2.Controls.Add(label1);
			groupBox2.Controls.Add(label2);
			groupBox2.Controls.Add(numericWidth);
			groupBox2.Location = new Point(14, 84);
			groupBox2.Margin = new Padding(4, 3, 4, 3);
			groupBox2.Name = "groupBox2";
			groupBox2.Padding = new Padding(4, 3, 4, 3);
			groupBox2.Size = new Size(279, 63);
			groupBox2.TabIndex = 1;
			groupBox2.TabStop = false;
			groupBox2.Text = "New Image Size";
			// 
			// numericHeight
			// 
			numericHeight.Location = new Point(203, 29);
			numericHeight.Margin = new Padding(4, 3, 4, 3);
			numericHeight.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
			numericHeight.Name = "numericHeight";
			numericHeight.Size = new Size(68, 23);
			numericHeight.TabIndex = 2;
			numericHeight.Value = new decimal(new int[] { 100, 0, 0, 0 });
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(7, 31);
			label1.Margin = new Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new Size(42, 15);
			label1.TabIndex = 0;
			label1.Text = "Width:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(153, 31);
			label2.Margin = new Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new Size(46, 15);
			label2.TabIndex = 2;
			label2.Text = "Height:";
			// 
			// numericWidth
			// 
			numericWidth.Location = new Point(51, 29);
			numericWidth.Margin = new Padding(4, 3, 4, 3);
			numericWidth.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
			numericWidth.Name = "numericWidth";
			numericWidth.Size = new Size(68, 23);
			numericWidth.TabIndex = 1;
			numericWidth.Value = new decimal(new int[] { 100, 0, 0, 0 });
			numericWidth.ValueChanged += numericWidth_ValueChanged;
			// 
			// buttonCancel
			// 
			buttonCancel.DialogResult = DialogResult.Cancel;
			buttonCancel.Location = new Point(205, 180);
			buttonCancel.Margin = new Padding(4, 3, 4, 3);
			buttonCancel.Name = "buttonCancel";
			buttonCancel.Size = new Size(88, 27);
			buttonCancel.TabIndex = 4;
			buttonCancel.Text = "Cancel";
			buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			buttonOK.Location = new Point(111, 180);
			buttonOK.Margin = new Padding(4, 3, 4, 3);
			buttonOK.Name = "buttonOK";
			buttonOK.Size = new Size(88, 27);
			buttonOK.TabIndex = 3;
			buttonOK.Text = "OK";
			buttonOK.UseVisualStyleBackColor = true;
			buttonOK.Click += buttonOK_Click;
			// 
			// buttonHelp
			// 
			buttonHelp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			buttonHelp.Image = (Image)resources.GetObject("buttonHelp.Image");
			buttonHelp.ImageAlign = ContentAlignment.MiddleLeft;
			buttonHelp.Location = new Point(14, 180);
			buttonHelp.Margin = new Padding(4, 3, 4, 3);
			buttonHelp.Name = "buttonHelp";
			buttonHelp.Size = new Size(70, 27);
			buttonHelp.TabIndex = 59;
			buttonHelp.Text = "Help";
			buttonHelp.TextAlign = ContentAlignment.MiddleRight;
			buttonHelp.UseVisualStyleBackColor = true;
			buttonHelp.Click += buttonHelp_Click;
			// 
			// imageListLocks
			// 
			imageListLocks.ColorDepth = ColorDepth.Depth8Bit;
			imageListLocks.ImageStream = (ImageListStreamer)resources.GetObject("imageListLocks.ImageStream");
			imageListLocks.TransparentColor = Color.Transparent;
			imageListLocks.Images.SetKeyName(0, "lock");
			imageListLocks.Images.SetKeyName(1, "lock_open");
			imageListLocks.Images.SetKeyName(2, "link");
			imageListLocks.Images.SetKeyName(3, "unlink");
			imageListLocks.Images.SetKeyName(4, "link_break");
			// 
			// _scaleShapes
			// 
			_scaleShapes.AutoSize = true;
			_scaleShapes.Location = new Point(14, 153);
			_scaleShapes.Margin = new Padding(4, 3, 4, 3);
			_scaleShapes.Name = "_scaleShapes";
			_scaleShapes.Size = new Size(93, 19);
			_scaleShapes.TabIndex = 60;
			_scaleShapes.Text = "Scale Shapes";
			_scaleShapes.UseVisualStyleBackColor = true;
			// 
			// pictureBoxLock
			// 
			pictureBoxLock.Image = (Image)resources.GetObject("pictureBoxLock.Image");
			pictureBoxLock.Location = new Point(125, 27);
			pictureBoxLock.Margin = new Padding(2, 3, 2, 3);
			pictureBoxLock.Name = "pictureBoxLock";
			pictureBoxLock.Size = new Size(22, 24);
			pictureBoxLock.SizeMode = PictureBoxSizeMode.StretchImage;
			pictureBoxLock.TabIndex = 3;
			pictureBoxLock.TabStop = false;
			pictureBoxLock.Click += pictureBoxLock_Click;
			// 
			// ResizePreviewForm
			// 
			AcceptButton = buttonOK;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			CancelButton = buttonCancel;
			ClientSize = new Size(307, 224);
			Controls.Add(_scaleShapes);
			Controls.Add(buttonHelp);
			Controls.Add(buttonOK);
			Controls.Add(buttonCancel);
			Controls.Add(groupBox2);
			Controls.Add(groupBox1);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			Margin = new Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "ResizePreviewForm";
			StartPosition = FormStartPosition.CenterParent;
			Text = "Resize Preview";
			Load += ResizePreviewForm_Load;
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)numericHeight).EndInit();
			((System.ComponentModel.ISupportInitialize)numericWidth).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxLock).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numericHeight;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericWidth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelWidth;
        private System.Windows.Forms.Label labelHeight;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.ImageList imageListLocks;
		private System.Windows.Forms.CheckBox _scaleShapes;
		private PictureBox pictureBoxLock;
	}
}