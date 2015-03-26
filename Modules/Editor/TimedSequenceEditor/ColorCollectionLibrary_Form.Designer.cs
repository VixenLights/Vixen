namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class ColorCollectionLibrary_Form
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
			this.listViewColors = new System.Windows.Forms.ListView();
			this.lblLibraryName = new System.Windows.Forms.Label();
			this.comboBoxCollections = new System.Windows.Forms.ComboBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxDescription = new System.Windows.Forms.TextBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.colorCollectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newCollectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteCollectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.exportCollectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.importCollectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.removeColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// listViewColors
			// 
			this.listViewColors.AllowDrop = true;
			this.listViewColors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.listViewColors.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listViewColors.Location = new System.Drawing.Point(22, 85);
			this.listViewColors.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.listViewColors.Name = "listViewColors";
			this.listViewColors.ShowItemToolTips = true;
			this.listViewColors.Size = new System.Drawing.Size(930, 538);
			this.listViewColors.TabIndex = 0;
			this.listViewColors.UseCompatibleStateImageBehavior = false;
			this.listViewColors.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewColors_ItemDrag);
			this.listViewColors.SelectedIndexChanged += new System.EventHandler(this.listViewColors_SelectedIndexChanged);
			this.listViewColors.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewColors_DragDrop);
			this.listViewColors.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewColors_DragEnter);
			// 
			// lblLibraryName
			// 
			this.lblLibraryName.AutoSize = true;
			this.lblLibraryName.Location = new System.Drawing.Point(20, 48);
			this.lblLibraryName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblLibraryName.Name = "lblLibraryName";
			this.lblLibraryName.Size = new System.Drawing.Size(119, 20);
			this.lblLibraryName.TabIndex = 1;
			this.lblLibraryName.Text = "Color Collection";
			// 
			// comboBoxCollections
			// 
			this.comboBoxCollections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxCollections.FormattingEnabled = true;
			this.comboBoxCollections.Location = new System.Drawing.Point(148, 43);
			this.comboBoxCollections.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.comboBoxCollections.Name = "comboBoxCollections";
			this.comboBoxCollections.Size = new System.Drawing.Size(308, 28);
			this.comboBoxCollections.TabIndex = 2;
			this.comboBoxCollections.SelectedIndexChanged += new System.EventHandler(this.comboBoxCollections_SelectedIndexChanged);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(637, 634);
			this.btnOK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(154, 35);
			this.btnOK.TabIndex = 8;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(800, 634);
			this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(154, 35);
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(490, 48);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(89, 20);
			this.label1.TabIndex = 11;
			this.label1.Text = "Description";
			// 
			// textBoxDescription
			// 
			this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxDescription.Enabled = false;
			this.textBoxDescription.Location = new System.Drawing.Point(590, 43);
			this.textBoxDescription.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.textBoxDescription.Name = "textBoxDescription";
			this.textBoxDescription.Size = new System.Drawing.Size(363, 26);
			this.textBoxDescription.TabIndex = 12;
			this.textBoxDescription.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxDescription_KeyUp);
			// 
			// menuStrip1
			// 
			this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.colorCollectionsToolStripMenuItem,
			this.addColorToolStripMenuItem,
			this.removeColorToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
			this.menuStrip1.Size = new System.Drawing.Size(979, 35);
			this.menuStrip1.TabIndex = 13;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// colorCollectionsToolStripMenuItem
			// 
			this.colorCollectionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.newCollectionToolStripMenuItem,
			this.deleteCollectionToolStripMenuItem,
			this.toolStripMenuItem1,
			this.exportCollectionToolStripMenuItem,
			this.importCollectionToolStripMenuItem});
			this.colorCollectionsToolStripMenuItem.Name = "colorCollectionsToolStripMenuItem";
			this.colorCollectionsToolStripMenuItem.Size = new System.Drawing.Size(158, 29);
			this.colorCollectionsToolStripMenuItem.Text = "Color Collections";
			// 
			// newCollectionToolStripMenuItem
			// 
			this.newCollectionToolStripMenuItem.Name = "newCollectionToolStripMenuItem";
			this.newCollectionToolStripMenuItem.Size = new System.Drawing.Size(230, 30);
			this.newCollectionToolStripMenuItem.Text = "New Collection";
			this.newCollectionToolStripMenuItem.Click += new System.EventHandler(this.newCollectionToolStripMenuItem_Click);
			// 
			// deleteCollectionToolStripMenuItem
			// 
			this.deleteCollectionToolStripMenuItem.Name = "deleteCollectionToolStripMenuItem";
			this.deleteCollectionToolStripMenuItem.Size = new System.Drawing.Size(230, 30);
			this.deleteCollectionToolStripMenuItem.Text = "Delete Collection";
			this.deleteCollectionToolStripMenuItem.Click += new System.EventHandler(this.deleteCollectionToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(227, 6);
			// 
			// exportCollectionToolStripMenuItem
			// 
			this.exportCollectionToolStripMenuItem.Name = "exportCollectionToolStripMenuItem";
			this.exportCollectionToolStripMenuItem.Size = new System.Drawing.Size(230, 30);
			this.exportCollectionToolStripMenuItem.Text = "Export Collections";
			this.exportCollectionToolStripMenuItem.Click += new System.EventHandler(this.exportCollectionToolStripMenuItem_Click);
			// 
			// importCollectionToolStripMenuItem
			// 
			this.importCollectionToolStripMenuItem.Name = "importCollectionToolStripMenuItem";
			this.importCollectionToolStripMenuItem.Size = new System.Drawing.Size(230, 30);
			this.importCollectionToolStripMenuItem.Text = "Import Collections";
			this.importCollectionToolStripMenuItem.Click += new System.EventHandler(this.importCollectionToolStripMenuItem_Click);
			// 
			// addColorToolStripMenuItem
			// 
			this.addColorToolStripMenuItem.Name = "addColorToolStripMenuItem";
			this.addColorToolStripMenuItem.Size = new System.Drawing.Size(106, 29);
			this.addColorToolStripMenuItem.Text = "Add Color";
			this.addColorToolStripMenuItem.Click += new System.EventHandler(this.addColorToolStripMenuItem_Click);
			// 
			// removeColorToolStripMenuItem
			// 
			this.removeColorToolStripMenuItem.Name = "removeColorToolStripMenuItem";
			this.removeColorToolStripMenuItem.Size = new System.Drawing.Size(137, 29);
			this.removeColorToolStripMenuItem.Text = "Remove Color";
			this.removeColorToolStripMenuItem.Click += new System.EventHandler(this.removeColorToolStripMenuItem_Click);
			// 
			// ColorCollectionLibrary_Form
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(979, 688);
			this.Controls.Add(this.textBoxDescription);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.comboBoxCollections);
			this.Controls.Add(this.lblLibraryName);
			this.Controls.Add(this.listViewColors);
			this.Controls.Add(this.menuStrip1);
			this.DoubleBuffered = true;
			this.MainMenuStrip = this.menuStrip1;
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(720, 487);
			this.Name = "ColorCollectionLibrary_Form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Color Collection Editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RandomColorLibrary_Form_FormClosing);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListView listViewColors;
		private System.Windows.Forms.Label lblLibraryName;
		private System.Windows.Forms.ComboBox comboBoxCollections;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxDescription;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem colorCollectionsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newCollectionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteCollectionToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem exportCollectionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importCollectionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addColorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem removeColorToolStripMenuItem;
	}
}