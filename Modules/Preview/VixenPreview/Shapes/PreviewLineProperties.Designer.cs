namespace VixenModules.Preview.VixenPreview.Shapes
{
    partial class PreviewLineProperties
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
            this.numericLightCount = new System.Windows.Forms.NumericUpDown();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioPixel = new System.Windows.Forms.RadioButton();
            this.radioMono = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listLinkedElements = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label4 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonLinkElements = new System.Windows.Forms.Button();
            this.treeElements = new System.Windows.Forms.TreeView();
            ((System.ComponentModel.ISupportInitialize)(this.numericLightCount)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 84);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Number of Lights:";
            // 
            // numericLightCount
            // 
            this.numericLightCount.Location = new System.Drawing.Point(101, 82);
            this.numericLightCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericLightCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericLightCount.Name = "numericLightCount";
            this.numericLightCount.Size = new System.Drawing.Size(46, 20);
            this.numericLightCount.TabIndex = 1;
            this.numericLightCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericLightCount.ValueChanged += new System.EventHandler(this.numericLightCount_ValueChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOK.Location = new System.Drawing.Point(566, 358);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(647, 358);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioPixel);
            this.groupBox1.Controls.Add(this.radioMono);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numericLightCount);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(256, 114);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Light Properties";
            // 
            // radioPixel
            // 
            this.radioPixel.AutoSize = true;
            this.radioPixel.Location = new System.Drawing.Point(101, 50);
            this.radioPixel.Name = "radioPixel";
            this.radioPixel.Size = new System.Drawing.Size(47, 17);
            this.radioPixel.TabIndex = 8;
            this.radioPixel.TabStop = true;
            this.radioPixel.Text = "Pixel";
            this.radioPixel.UseVisualStyleBackColor = true;
            this.radioPixel.CheckedChanged += new System.EventHandler(this.radioPixel_CheckedChanged);
            // 
            // radioMono
            // 
            this.radioMono.AutoSize = true;
            this.radioMono.Location = new System.Drawing.Point(101, 27);
            this.radioMono.Name = "radioMono";
            this.radioMono.Size = new System.Drawing.Size(150, 17);
            this.radioMono.TabIndex = 7;
            this.radioMono.TabStop = true;
            this.radioMono.Text = "Single Color or Dumb RGB";
            this.radioMono.UseVisualStyleBackColor = true;
            this.radioMono.CheckedChanged += new System.EventHandler(this.radioMono_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "String Type:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listLinkedElements);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.buttonLinkElements);
            this.groupBox2.Controls.Add(this.treeElements);
            this.groupBox2.Location = new System.Drawing.Point(274, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(448, 336);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Element Links";
            // 
            // listLinkedElements
            // 
            this.listLinkedElements.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listLinkedElements.FullRowSelect = true;
            this.listLinkedElements.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listLinkedElements.HideSelection = false;
            this.listLinkedElements.Location = new System.Drawing.Point(235, 43);
            this.listLinkedElements.Name = "listLinkedElements";
            this.listLinkedElements.Size = new System.Drawing.Size(208, 287);
            this.listLinkedElements.TabIndex = 6;
            this.listLinkedElements.UseCompatibleStateImageBehavior = false;
            this.listLinkedElements.View = System.Windows.Forms.View.Details;
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
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Linked Elements";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(205, 65);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(24, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "<";
            this.button2.UseVisualStyleBackColor = true;
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
            this.treeElements.Location = new System.Drawing.Point(6, 43);
            this.treeElements.Name = "treeElements";
            this.treeElements.Size = new System.Drawing.Size(193, 287);
            this.treeElements.TabIndex = 0;
            this.treeElements.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeElements_AfterSelect);
            this.treeElements.DoubleClick += new System.EventHandler(this.treeElements_DoubleClick);
            // 
            // PreviewLineProperties
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(734, 389);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "PreviewLineProperties";
            this.Text = "String Properties";
            this.Load += new System.EventHandler(this.PreviewLineProperties_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericLightCount)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericLightCount;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioPixel;
        private System.Windows.Forms.RadioButton radioMono;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonLinkElements;
        private System.Windows.Forms.TreeView treeElements;
        private System.Windows.Forms.ListView listLinkedElements;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}