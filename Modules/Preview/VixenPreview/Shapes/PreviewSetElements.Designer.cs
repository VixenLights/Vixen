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
			this.reverseElementLinkingInThisStringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.label6 = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonHelp = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.tblMain = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.tblLightCountControls = new System.Windows.Forms.TableLayoutPanel();
			this.tblStringToLink = new System.Windows.Forms.TableLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownLightCount)).BeginInit();
			this.contextMenuLinkedElements.SuspendLayout();
			this.tblMain.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.tblLightCountControls.SuspendLayout();
			this.tblStringToLink.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(1, 10);
			this.label1.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(125, 15);
			this.label1.TabIndex = 30;
			this.label1.Text = "Light Count for String:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// numericUpDownLightCount
			// 
			this.numericUpDownLightCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.numericUpDownLightCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.numericUpDownLightCount.Location = new System.Drawing.Point(129, 6);
			this.numericUpDownLightCount.Margin = new System.Windows.Forms.Padding(2);
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
			this.numericUpDownLightCount.Size = new System.Drawing.Size(60, 23);
			this.numericUpDownLightCount.TabIndex = 1;
			this.numericUpDownLightCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// buttonSetLightCount
			// 
			this.buttonSetLightCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.buttonSetLightCount.Location = new System.Drawing.Point(192, 6);
			this.buttonSetLightCount.Margin = new System.Windows.Forms.Padding(1);
			this.buttonSetLightCount.Name = "buttonSetLightCount";
			this.buttonSetLightCount.Size = new System.Drawing.Size(43, 24);
			this.buttonSetLightCount.TabIndex = 2;
			this.buttonSetLightCount.Text = "Set";
			this.buttonSetLightCount.UseVisualStyleBackColor = true;
			this.buttonSetLightCount.Click += new System.EventHandler(this.buttonSetLightCount_Click);
			this.buttonSetLightCount.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonSetLightCount.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// label11
			// 
			this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label11.AutoSize = true;
			this.tblMain.SetColumnSpan(this.label11, 2);
			this.label11.Location = new System.Drawing.Point(5, 494);
			this.label11.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(571, 15);
			this.label11.TabIndex = 25;
			this.label11.Text = "- Ctrl+Click or Shift+Click to select multiple elements to drag.";
			// 
			// label10
			// 
			this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label10.AutoSize = true;
			this.tblMain.SetColumnSpan(this.label10, 2);
			this.label10.Location = new System.Drawing.Point(5, 479);
			this.label10.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(571, 15);
			this.label10.TabIndex = 24;
			this.label10.Text = "- Drag a group with children to link them all.";
			// 
			// label9
			// 
			this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label9.AutoSize = true;
			this.tblMain.SetColumnSpan(this.label9, 2);
			this.label9.Location = new System.Drawing.Point(5, 464);
			this.label9.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(571, 15);
			this.label9.TabIndex = 23;
			this.label9.Text = "- Drag elements from left to right to link them.";
			// 
			// treeElements
			// 
			this.treeElements.AllowDrop = true;
			this.treeElements.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.treeElements.Cursor = System.Windows.Forms.Cursors.Default;
			this.treeElements.CustomDragCursor = null;
			this.treeElements.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeElements.DragDefaultMode = System.Windows.Forms.DragDropEffects.None;
			this.treeElements.DragDestinationNodeBackColor = System.Drawing.SystemColors.Highlight;
			this.treeElements.DragDestinationNodeForeColor = System.Drawing.SystemColors.HighlightText;
			this.treeElements.DragSourceNodeBackColor = System.Drawing.SystemColors.ControlLight;
			this.treeElements.DragSourceNodeForeColor = System.Drawing.SystemColors.ControlText;
			this.treeElements.HideSelection = false;
			this.treeElements.Location = new System.Drawing.Point(3, 65);
			this.treeElements.Margin = new System.Windows.Forms.Padding(3, 3, 5, 3);
			this.treeElements.Name = "treeElements";
			this.treeElements.SelectedNodes = ((System.Collections.Generic.List<System.Windows.Forms.TreeNode>)(resources.GetObject("treeElements.SelectedNodes")));
			this.treeElements.Size = new System.Drawing.Size(282, 396);
			this.treeElements.TabIndex = 3;
			this.treeElements.UsingCustomDragCursor = false;
			this.treeElements.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeElements_ItemDrag);
			this.treeElements.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.treeElements_MouseDoubleClick);
			// 
			// label7
			// 
			this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(3, 10);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(80, 15);
			this.label7.TabIndex = 19;
			this.label7.Text = "String to Link:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboStrings
			// 
			this.comboStrings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.comboStrings.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboStrings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboStrings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.comboStrings.FormattingEnabled = true;
			this.comboStrings.Location = new System.Drawing.Point(91, 6);
			this.comboStrings.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.comboStrings.Name = "comboStrings";
			this.comboStrings.Size = new System.Drawing.Size(188, 24);
			this.comboStrings.TabIndex = 0;
			this.comboStrings.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
			this.comboStrings.SelectedIndexChanged += new System.EventHandler(this.comboStrings_SelectedIndexChanged);
			// 
			// listLinkedElements
			// 
			this.listLinkedElements.AllowDrop = true;
			this.listLinkedElements.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listLinkedElements.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.listLinkedElements.ContextMenuStrip = this.contextMenuLinkedElements;
			this.listLinkedElements.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listLinkedElements.FullRowSelect = true;
			this.listLinkedElements.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listLinkedElements.HideSelection = false;
			this.listLinkedElements.Location = new System.Drawing.Point(295, 65);
			this.listLinkedElements.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
			this.listLinkedElements.Name = "listLinkedElements";
			this.listLinkedElements.Size = new System.Drawing.Size(283, 396);
			this.listLinkedElements.TabIndex = 4;
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
			this.contextMenuLinkedElements.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.contextMenuLinkedElements.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToAllElementsInThisStringToolStripMenuItem,
            this.copyToAllElementsAllStringsToolStripMenuItem,
            this.reverseElementLinkingInThisStringToolStripMenuItem});
			this.contextMenuLinkedElements.Name = "contextMenuLinkedElements";
			this.contextMenuLinkedElements.Size = new System.Drawing.Size(275, 70);
			// 
			// copyToAllElementsInThisStringToolStripMenuItem
			// 
			this.copyToAllElementsInThisStringToolStripMenuItem.Name = "copyToAllElementsInThisStringToolStripMenuItem";
			this.copyToAllElementsInThisStringToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
			this.copyToAllElementsInThisStringToolStripMenuItem.Text = "Copy to All Elements in This String";
			this.copyToAllElementsInThisStringToolStripMenuItem.Click += new System.EventHandler(this.copyToAllElementsInThisStringToolStripMenuItem_Click);
			// 
			// copyToAllElementsAllStringsToolStripMenuItem
			// 
			this.copyToAllElementsAllStringsToolStripMenuItem.Name = "copyToAllElementsAllStringsToolStripMenuItem";
			this.copyToAllElementsAllStringsToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
			this.copyToAllElementsAllStringsToolStripMenuItem.Text = "Copy to All Elements/All Strings";
			this.copyToAllElementsAllStringsToolStripMenuItem.Click += new System.EventHandler(this.copyToAllElementsAllStringsToolStripMenuItem_Click);
			// 
			// reverseElementLinkingInThisStringToolStripMenuItem
			// 
			this.reverseElementLinkingInThisStringToolStripMenuItem.Name = "reverseElementLinkingInThisStringToolStripMenuItem";
			this.reverseElementLinkingInThisStringToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
			this.reverseElementLinkingInThisStringToolStripMenuItem.Text = "Reverse Element Linking in This String";
			this.reverseElementLinkingInThisStringToolStripMenuItem.Click += new System.EventHandler(this.reverseElementLinkingInThisStringToolStripMenuItem_Click);
			// 
			// label6
			// 
			this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(3, 44);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(106, 15);
			this.label6.TabIndex = 2;
			this.label6.Text = "Available Elements";
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.Location = new System.Drawing.Point(96, 6);
			this.buttonOK.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(87, 27);
			this.buttonOK.TabIndex = 20;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			this.buttonOK.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOK.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(193, 6);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(87, 27);
			this.buttonCancel.TabIndex = 21;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonHelp
			// 
			this.buttonHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonHelp.AutoSize = true;
			this.buttonHelp.Image = ((System.Drawing.Image)(resources.GetObject("buttonHelp.Image")));
			this.buttonHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonHelp.Location = new System.Drawing.Point(5, 521);
			this.buttonHelp.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.buttonHelp.Name = "buttonHelp";
			this.buttonHelp.Size = new System.Drawing.Size(24, 27);
			this.buttonHelp.TabIndex = 22;
			this.buttonHelp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.buttonHelp.UseVisualStyleBackColor = true;
			this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
			// 
			// label2
			// 
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(295, 44);
			this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(119, 15);
			this.label2.TabIndex = 27;
			this.label2.Text = "#        Linked Element";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tblMain
			// 
			this.tblMain.AutoSize = true;
			this.tblMain.ColumnCount = 2;
			this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tblMain.Controls.Add(this.treeElements, 0, 2);
			this.tblMain.Controls.Add(this.listLinkedElements, 1, 2);
			this.tblMain.Controls.Add(this.buttonHelp, 0, 6);
			this.tblMain.Controls.Add(this.label11, 0, 5);
			this.tblMain.Controls.Add(this.label2, 1, 1);
			this.tblMain.Controls.Add(this.label6, 0, 1);
			this.tblMain.Controls.Add(this.label10, 0, 4);
			this.tblMain.Controls.Add(this.label9, 0, 3);
			this.tblMain.Controls.Add(this.tableLayoutPanel2, 1, 6);
			this.tblMain.Controls.Add(this.tblLightCountControls, 1, 0);
			this.tblMain.Controls.Add(this.tblStringToLink, 0, 0);
			this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tblMain.Location = new System.Drawing.Point(0, 0);
			this.tblMain.Name = "tblMain";
			this.tblMain.RowCount = 7;
			this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tblMain.Size = new System.Drawing.Size(581, 554);
			this.tblMain.TabIndex = 28;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.AutoSize = true;
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.Controls.Add(this.buttonOK, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.buttonCancel, 1, 0);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(293, 512);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 1;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(285, 39);
			this.tableLayoutPanel2.TabIndex = 28;
			// 
			// tblLightCountControls
			// 
			this.tblLightCountControls.AutoSize = true;
			this.tblLightCountControls.ColumnCount = 3;
			this.tblLightCountControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tblLightCountControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tblLightCountControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tblLightCountControls.Controls.Add(this.buttonSetLightCount, 2, 0);
			this.tblLightCountControls.Controls.Add(this.numericUpDownLightCount, 1, 0);
			this.tblLightCountControls.Controls.Add(this.label1, 0, 0);
			this.tblLightCountControls.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tblLightCountControls.Location = new System.Drawing.Point(293, 3);
			this.tblLightCountControls.Name = "tblLightCountControls";
			this.tblLightCountControls.RowCount = 1;
			this.tblLightCountControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tblLightCountControls.Size = new System.Drawing.Size(285, 36);
			this.tblLightCountControls.TabIndex = 29;
			// 
			// tblStringToLink
			// 
			this.tblStringToLink.AutoSize = true;
			this.tblStringToLink.ColumnCount = 2;
			this.tblStringToLink.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tblStringToLink.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tblStringToLink.Controls.Add(this.comboStrings, 1, 0);
			this.tblStringToLink.Controls.Add(this.label7, 0, 0);
			this.tblStringToLink.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tblStringToLink.Location = new System.Drawing.Point(3, 3);
			this.tblStringToLink.Name = "tblStringToLink";
			this.tblStringToLink.RowCount = 1;
			this.tblStringToLink.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tblStringToLink.Size = new System.Drawing.Size(284, 36);
			this.tblStringToLink.TabIndex = 30;
			// 
			// PreviewSetElements
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(581, 554);
			this.Controls.Add(this.tblMain);
			this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.Name = "PreviewSetElements";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Link Elements";
			this.Load += new System.EventHandler(this.PreviewSetElements_Load);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownLightCount)).EndInit();
			this.contextMenuLinkedElements.ResumeLayout(false);
			this.tblMain.ResumeLayout(false);
			this.tblMain.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tblLightCountControls.ResumeLayout(false);
			this.tblLightCountControls.PerformLayout();
			this.tblStringToLink.ResumeLayout(false);
			this.tblStringToLink.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private Common.Controls.MultiSelectTreeview treeElements;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox comboStrings;
		private System.Windows.Forms.ListView listLinkedElements;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.ContextMenuStrip contextMenuLinkedElements;
		private System.Windows.Forms.ToolStripMenuItem copyToAllElementsAllStringsToolStripMenuItem;
		private System.Windows.Forms.Button buttonHelp;
		private System.Windows.Forms.ToolStripMenuItem copyToAllElementsInThisStringToolStripMenuItem;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown numericUpDownLightCount;
		private System.Windows.Forms.Button buttonSetLightCount;
		private System.Windows.Forms.ToolStripMenuItem reverseElementLinkingInThisStringToolStripMenuItem;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TableLayoutPanel tblMain;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.TableLayoutPanel tblLightCountControls;
		private System.Windows.Forms.TableLayoutPanel tblStringToLink;
	}
}