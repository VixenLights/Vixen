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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResizePreviewForm));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.labelWidth = new System.Windows.Forms.Label();
			this.labelHeight = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.pictureBoxLock = new System.Windows.Forms.PictureBox();
			this.numericHeight = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.numericWidth = new System.Windows.Forms.NumericUpDown();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonHelp = new System.Windows.Forms.Button();
			this.imageListLocks = new System.Windows.Forms.ImageList(this.components);
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxLock)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericHeight)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericWidth)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.labelWidth);
			this.groupBox1.Controls.Add(this.labelHeight);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(239, 55);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Existing Image Size";
			// 
			// labelWidth
			// 
			this.labelWidth.AutoSize = true;
			this.labelWidth.Location = new System.Drawing.Point(47, 25);
			this.labelWidth.Name = "labelWidth";
			this.labelWidth.Size = new System.Drawing.Size(38, 13);
			this.labelWidth.TabIndex = 5;
			this.labelWidth.Text = "Width:";
			// 
			// labelHeight
			// 
			this.labelHeight.AutoSize = true;
			this.labelHeight.Location = new System.Drawing.Point(165, 25);
			this.labelHeight.Name = "labelHeight";
			this.labelHeight.Size = new System.Drawing.Size(38, 13);
			this.labelHeight.TabIndex = 6;
			this.labelHeight.Text = "Height";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 25);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(38, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "Width:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(124, 25);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(41, 13);
			this.label4.TabIndex = 4;
			this.label4.Text = "Height:";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.pictureBoxLock);
			this.groupBox2.Controls.Add(this.numericHeight);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.numericWidth);
			this.groupBox2.Location = new System.Drawing.Point(12, 73);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(239, 55);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "New Image Size";
			// 
			// pictureBoxLock
			// 
			this.pictureBoxLock.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxLock.Image")));
			this.pictureBoxLock.Location = new System.Drawing.Point(107, 23);
			this.pictureBoxLock.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.pictureBoxLock.Name = "pictureBoxLock";
			this.pictureBoxLock.Size = new System.Drawing.Size(19, 21);
			this.pictureBoxLock.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBoxLock.TabIndex = 3;
			this.pictureBoxLock.TabStop = false;
			this.pictureBoxLock.Click += new System.EventHandler(this.pictureBoxLock_Click);
			// 
			// numericHeight
			// 
			this.numericHeight.Location = new System.Drawing.Point(170, 25);
			this.numericHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.numericHeight.Name = "numericHeight";
			this.numericHeight.Size = new System.Drawing.Size(58, 20);
			this.numericHeight.TabIndex = 2;
			this.numericHeight.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 27);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Width:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(127, 27);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(41, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Height:";
			// 
			// numericWidth
			// 
			this.numericWidth.Location = new System.Drawing.Point(44, 25);
			this.numericWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.numericWidth.Name = "numericWidth";
			this.numericWidth.Size = new System.Drawing.Size(58, 20);
			this.numericWidth.TabIndex = 1;
			this.numericWidth.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.numericWidth.ValueChanged += new System.EventHandler(this.numericWidth_ValueChanged);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(176, 138);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 4;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(95, 138);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 3;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			this.buttonOK.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
			this.buttonOK.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOK.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonHelp
			// 
			this.buttonHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonHelp.Image = ((System.Drawing.Image)(resources.GetObject("buttonHelp.Image")));
			this.buttonHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonHelp.Location = new System.Drawing.Point(12, 138);
			this.buttonHelp.Name = "buttonHelp";
			this.buttonHelp.Size = new System.Drawing.Size(60, 23);
			this.buttonHelp.TabIndex = 59;
			this.buttonHelp.Text = "Help";
			this.buttonHelp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.buttonHelp.UseVisualStyleBackColor = true;
			this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
			this.buttonHelp.Paint += new System.Windows.Forms.PaintEventHandler(this.buttonHelp_Paint);
			this.buttonHelp.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonHelp.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// imageListLocks
			// 
			this.imageListLocks.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListLocks.ImageStream")));
			this.imageListLocks.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListLocks.Images.SetKeyName(0, "lock");
			this.imageListLocks.Images.SetKeyName(1, "lock_open");
			this.imageListLocks.Images.SetKeyName(2, "link");
			this.imageListLocks.Images.SetKeyName(3, "unlink");
			this.imageListLocks.Images.SetKeyName(4, "link_break");
			// 
			// ResizePreviewForm
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(263, 170);
			this.Controls.Add(this.buttonHelp);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ResizePreviewForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Resize Preview";
			this.Load += new System.EventHandler(this.ResizePreviewForm_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxLock)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericHeight)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericWidth)).EndInit();
			this.ResumeLayout(false);

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
        private System.Windows.Forms.PictureBox pictureBoxLock;
        private System.Windows.Forms.ImageList imageListLocks;
    }
}