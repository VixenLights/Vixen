namespace VixenModules.Preview.VixenPreview.Shapes
{
    partial class PreviewSingleProperties
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
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownPixelSize = new System.Windows.Forms.NumericUpDown();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listLinkedElements = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonLinkElements = new System.Windows.Forms.Button();
            this.treeElements = new System.Windows.Forms.TreeView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.trackBarAlpha = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.pictureSample = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPixelSize)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAlpha)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureSample)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Light Size:";
            // 
            // numericUpDownPixelSize
            // 
            this.numericUpDownPixelSize.Location = new System.Drawing.Point(88, 20);
            this.numericUpDownPixelSize.Maximum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.numericUpDownPixelSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownPixelSize.Name = "numericUpDownPixelSize";
            this.numericUpDownPixelSize.Size = new System.Drawing.Size(53, 20);
            this.numericUpDownPixelSize.TabIndex = 1;
            this.numericUpDownPixelSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownPixelSize.ValueChanged += new System.EventHandler(this.numericUpDownPixelSize_ValueChanged);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(557, 322);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(476, 322);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listLinkedElements);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.buttonLinkElements);
            this.groupBox2.Controls.Add(this.treeElements);
            this.groupBox2.Location = new System.Drawing.Point(184, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(448, 292);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Element Links";
            // 
            // listLinkedElements
            // 
            this.listLinkedElements.AllowDrop = true;
            this.listLinkedElements.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listLinkedElements.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listLinkedElements.FullRowSelect = true;
            this.listLinkedElements.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listLinkedElements.HideSelection = false;
            this.listLinkedElements.Location = new System.Drawing.Point(235, 43);
            this.listLinkedElements.Name = "listLinkedElements";
            this.listLinkedElements.Size = new System.Drawing.Size(208, 243);
            this.listLinkedElements.TabIndex = 6;
            this.listLinkedElements.UseCompatibleStateImageBehavior = false;
            this.listLinkedElements.View = System.Windows.Forms.View.Details;
            this.listLinkedElements.DragDrop += new System.Windows.Forms.DragEventHandler(this.listLinkedElements_DragDrop);
            this.listLinkedElements.DragEnter += new System.Windows.Forms.DragEventHandler(this.listLinkedElements_DragEnter);
            this.listLinkedElements.DragOver += new System.Windows.Forms.DragEventHandler(this.listLinkedElements_DragOver);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "#";
            this.columnHeader1.Width = 25;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Element";
            this.columnHeader2.Width = 150;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(235, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Linked Element";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Available Elements";
            // 
            // buttonLinkElements
            // 
            this.buttonLinkElements.Location = new System.Drawing.Point(205, 43);
            this.buttonLinkElements.Name = "buttonLinkElements";
            this.buttonLinkElements.Size = new System.Drawing.Size(24, 23);
            this.buttonLinkElements.TabIndex = 1;
            this.buttonLinkElements.Text = ">";
            this.buttonLinkElements.UseVisualStyleBackColor = true;
            this.buttonLinkElements.Click += new System.EventHandler(this.buttonLinkElements_Click);
            // 
            // treeElements
            // 
            this.treeElements.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeElements.Location = new System.Drawing.Point(6, 43);
            this.treeElements.Name = "treeElements";
            this.treeElements.Size = new System.Drawing.Size(193, 243);
            this.treeElements.TabIndex = 0;
            this.treeElements.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeElements_ItemDrag);
            this.treeElements.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeElements_AfterSelect);
            this.treeElements.DoubleClick += new System.EventHandler(this.treeElements_DoubleClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.trackBarAlpha);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numericUpDownPixelSize);
            this.groupBox1.Location = new System.Drawing.Point(12, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(153, 120);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Light Properties";
            // 
            // trackBarAlpha
            // 
            this.trackBarAlpha.LargeChange = 10;
            this.trackBarAlpha.Location = new System.Drawing.Point(9, 69);
            this.trackBarAlpha.Maximum = 255;
            this.trackBarAlpha.Name = "trackBarAlpha";
            this.trackBarAlpha.Size = new System.Drawing.Size(135, 45);
            this.trackBarAlpha.TabIndex = 3;
            this.trackBarAlpha.TickFrequency = 25;
            this.trackBarAlpha.Scroll += new System.EventHandler(this.trackBarAlpha_Scroll);
            this.trackBarAlpha.ValueChanged += new System.EventHandler(this.trackBarAlpha_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Max Intensity:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.pictureSample);
            this.groupBox3.Location = new System.Drawing.Point(12, 139);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(153, 166);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Sample";
            // 
            // pictureSample
            // 
            this.pictureSample.BackColor = System.Drawing.Color.Black;
            this.pictureSample.Location = new System.Drawing.Point(9, 19);
            this.pictureSample.Name = "pictureSample";
            this.pictureSample.Size = new System.Drawing.Size(135, 135);
            this.pictureSample.TabIndex = 5;
            this.pictureSample.TabStop = false;
            // 
            // PreviewSingleProperties
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(643, 359);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "PreviewSingleProperties";
            this.Text = "Single Light Properties";
            this.Load += new System.EventHandler(this.PreviewSingleProperties_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPixelSize)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAlpha)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureSample)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownPixelSize;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListView listLinkedElements;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonLinkElements;
        private System.Windows.Forms.TreeView treeElements;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TrackBar trackBarAlpha;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.PictureBox pictureSample;
    }
}