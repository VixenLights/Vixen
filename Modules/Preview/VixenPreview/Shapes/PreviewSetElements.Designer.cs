namespace VixenModules.Preview.VixenPreview.Shapes
{
    partial class PreviewSetElements
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreviewSetElements));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panelSetLightCount = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownLightCount = new System.Windows.Forms.NumericUpDown();
            this.buttonSetLightCount = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.treeElements = new Common.Controls.MultiSelectTreeview();
            this.label7 = new System.Windows.Forms.Label();
            this.comboStrings = new System.Windows.Forms.ComboBox();
            this.listLinkedElements = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuLinkedElements = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToAllElementsInThisStringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToAllElementsAllStringsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.reverseElementLinkingInThisStringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox2.SuspendLayout();
            this.panelSetLightCount.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLightCount)).BeginInit();
            this.contextMenuLinkedElements.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panelSetLightCount);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.treeElements);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.comboStrings);
            this.groupBox2.Controls.Add(this.listLinkedElements);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(24, 23);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(6);
            this.groupBox2.Size = new System.Drawing.Size(858, 804);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Element Links";
            // 
            // panelSetLightCount
            // 
            this.panelSetLightCount.Controls.Add(this.label1);
            this.panelSetLightCount.Controls.Add(this.numericUpDownLightCount);
            this.panelSetLightCount.Controls.Add(this.buttonSetLightCount);
            this.panelSetLightCount.Location = new System.Drawing.Point(438, 21);
            this.panelSetLightCount.Name = "panelSetLightCount";
            this.panelSetLightCount.Size = new System.Drawing.Size(396, 63);
            this.panelSetLightCount.TabIndex = 26;
            this.panelSetLightCount.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(221, 25);
            this.label1.TabIndex = 30;
            this.label1.Text = "Light Count for String:";
            // 
            // numericUpDownLightCount
            // 
            this.numericUpDownLightCount.Location = new System.Drawing.Point(230, 14);
            this.numericUpDownLightCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownLightCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownLightCount.Name = "numericUpDownLightCount";
            this.numericUpDownLightCount.Size = new System.Drawing.Size(81, 31);
            this.numericUpDownLightCount.TabIndex = 29;
            this.numericUpDownLightCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // buttonSetLightCount
            // 
            this.buttonSetLightCount.Location = new System.Drawing.Point(317, 9);
            this.buttonSetLightCount.Name = "buttonSetLightCount";
            this.buttonSetLightCount.Size = new System.Drawing.Size(75, 40);
            this.buttonSetLightCount.TabIndex = 28;
            this.buttonSetLightCount.Text = "Set";
            this.buttonSetLightCount.UseVisualStyleBackColor = true;
            this.buttonSetLightCount.Click += new System.EventHandler(this.buttonSetLightCount_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(16, 767);
            this.label11.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(589, 25);
            this.label11.TabIndex = 25;
            this.label11.Text = "- Ctrl+Click or Shift+Click to select multiple elements to drag.";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 742);
            this.label10.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(426, 25);
            this.label10.TabIndex = 24;
            this.label10.Text = "- Drag a group with children to link them all.";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(14, 717);
            this.label9.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(441, 25);
            this.label9.TabIndex = 23;
            this.label9.Text = "- Drag elements from left to right to link them.";
            // 
            // treeElements
            // 
            this.treeElements.AllowDrop = true;
            this.treeElements.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeElements.CustomDragCursor = null;
            this.treeElements.DragDefaultMode = System.Windows.Forms.DragDropEffects.Move;
            this.treeElements.DragDestinationNodeBackColor = System.Drawing.SystemColors.Highlight;
            this.treeElements.DragDestinationNodeForeColor = System.Drawing.SystemColors.HighlightText;
            this.treeElements.DragSourceNodeBackColor = System.Drawing.SystemColors.ControlLight;
            this.treeElements.DragSourceNodeForeColor = System.Drawing.SystemColors.ControlText;
            this.treeElements.HideSelection = false;
            this.treeElements.Location = new System.Drawing.Point(18, 129);
            this.treeElements.Margin = new System.Windows.Forms.Padding(6);
            this.treeElements.Name = "treeElements";
            this.treeElements.SelectedNodes = ((System.Collections.Generic.List<System.Windows.Forms.TreeNode>)(resources.GetObject("treeElements.SelectedNodes")));
            this.treeElements.Size = new System.Drawing.Size(396, 573);
            this.treeElements.TabIndex = 22;
            this.treeElements.UsingCustomDragCursor = false;
            this.treeElements.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeElements_ItemDrag);
            this.treeElements.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.treeElements_MouseDoubleClick);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 40);
            this.label7.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(144, 25);
            this.label7.TabIndex = 19;
            this.label7.Text = "String to Link:";
            // 
            // comboStrings
            // 
            this.comboStrings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboStrings.FormattingEnabled = true;
            this.comboStrings.Location = new System.Drawing.Point(172, 35);
            this.comboStrings.Margin = new System.Windows.Forms.Padding(6);
            this.comboStrings.Name = "comboStrings";
            this.comboStrings.Size = new System.Drawing.Size(242, 33);
            this.comboStrings.TabIndex = 7;
            this.comboStrings.SelectedIndexChanged += new System.EventHandler(this.comboStrings_SelectedIndexChanged);
            // 
            // listLinkedElements
            // 
            this.listLinkedElements.AllowDrop = true;
            this.listLinkedElements.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listLinkedElements.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listLinkedElements.ContextMenuStrip = this.contextMenuLinkedElements;
            this.listLinkedElements.FullRowSelect = true;
            this.listLinkedElements.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listLinkedElements.HideSelection = false;
            this.listLinkedElements.Location = new System.Drawing.Point(438, 129);
            this.listLinkedElements.Margin = new System.Windows.Forms.Padding(6);
            this.listLinkedElements.Name = "listLinkedElements";
            this.listLinkedElements.Size = new System.Drawing.Size(396, 573);
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
            this.columnHeader1.Width = 30;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Linked Element";
            this.columnHeader2.Width = 150;
            // 
            // contextMenuLinkedElements
            // 
            this.contextMenuLinkedElements.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToAllElementsInThisStringToolStripMenuItem,
            this.copyToAllElementsAllStringsToolStripMenuItem,
            this.reverseElementLinkingInThisStringToolStripMenuItem});
            this.contextMenuLinkedElements.Name = "contextMenuLinkedElements";
            this.contextMenuLinkedElements.Size = new System.Drawing.Size(438, 150);
            // 
            // copyToAllElementsInThisStringToolStripMenuItem
            // 
            this.copyToAllElementsInThisStringToolStripMenuItem.Name = "copyToAllElementsInThisStringToolStripMenuItem";
            this.copyToAllElementsInThisStringToolStripMenuItem.Size = new System.Drawing.Size(437, 34);
            this.copyToAllElementsInThisStringToolStripMenuItem.Text = "Copy to All Elements in This String";
            this.copyToAllElementsInThisStringToolStripMenuItem.Click += new System.EventHandler(this.copyToAllElementsInThisStringToolStripMenuItem_Click);
            // 
            // copyToAllElementsAllStringsToolStripMenuItem
            // 
            this.copyToAllElementsAllStringsToolStripMenuItem.Name = "copyToAllElementsAllStringsToolStripMenuItem";
            this.copyToAllElementsAllStringsToolStripMenuItem.Size = new System.Drawing.Size(437, 34);
            this.copyToAllElementsAllStringsToolStripMenuItem.Text = "Copy to All Elements/All Strings";
            this.copyToAllElementsAllStringsToolStripMenuItem.Click += new System.EventHandler(this.copyToAllElementsAllStringsToolStripMenuItem_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(432, 98);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(171, 25);
            this.label5.TabIndex = 5;
            this.label5.Text = "Linked Elements";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 98);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(195, 25);
            this.label6.TabIndex = 2;
            this.label6.Text = "Available Elements";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(576, 854);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(6);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(150, 44);
            this.buttonOK.TabIndex = 20;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(738, 854);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(6);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(150, 44);
            this.buttonCancel.TabIndex = 21;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonHelp
            // 
            this.buttonHelp.Image = ((System.Drawing.Image)(resources.GetObject("buttonHelp.Image")));
            this.buttonHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonHelp.Location = new System.Drawing.Point(24, 854);
            this.buttonHelp.Margin = new System.Windows.Forms.Padding(6);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(108, 44);
            this.buttonHelp.TabIndex = 22;
            this.buttonHelp.Text = "Help";
            this.buttonHelp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // reverseElementLinkingInThisStringToolStripMenuItem
            // 
            this.reverseElementLinkingInThisStringToolStripMenuItem.Name = "reverseElementLinkingInThisStringToolStripMenuItem";
            this.reverseElementLinkingInThisStringToolStripMenuItem.Size = new System.Drawing.Size(437, 34);
            this.reverseElementLinkingInThisStringToolStripMenuItem.Text = "Reverse Element Linking in This String";
            this.reverseElementLinkingInThisStringToolStripMenuItem.Click += new System.EventHandler(this.reverseElementLinkingInThisStringToolStripMenuItem_Click);
            // 
            // PreviewSetElements
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(912, 921);
            this.Controls.Add(this.buttonHelp);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "PreviewSetElements";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Link Elements";
            this.Load += new System.EventHandler(this.PreviewSetElements_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panelSetLightCount.ResumeLayout(false);
            this.panelSetLightCount.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLightCount)).EndInit();
            this.contextMenuLinkedElements.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private Common.Controls.MultiSelectTreeview treeElements;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboStrings;
        private System.Windows.Forms.ListView listLinkedElements;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ContextMenuStrip contextMenuLinkedElements;
        private System.Windows.Forms.ToolStripMenuItem copyToAllElementsAllStringsToolStripMenuItem;
        private System.Windows.Forms.Button buttonHelp;
		private System.Windows.Forms.ToolStripMenuItem copyToAllElementsInThisStringToolStripMenuItem;
        private System.Windows.Forms.Panel panelSetLightCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownLightCount;
        private System.Windows.Forms.Button buttonSetLightCount;
        private System.Windows.Forms.ToolStripMenuItem reverseElementLinkingInThisStringToolStripMenuItem;
    }
}