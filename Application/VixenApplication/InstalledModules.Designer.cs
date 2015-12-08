namespace VixenApplication {
	partial class InstalledModules {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.panel1 = new System.Windows.Forms.Panel();
			this.listViewModules = new Common.Controls.ListViewEx();
			this.nameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.descriptionHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.authorHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.versionHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.fileHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonCopyModuleId = new System.Windows.Forms.Button();
			this.buttonClose = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Controls.Add(this.listViewModules);
			this.panel1.Controls.Add(this.buttonCopyModuleId);
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(895, 357);
			this.panel1.TabIndex = 0;
			// 
			// listViewModules
			// 
			this.listViewModules.AllowDrop = true;
			this.listViewModules.AllowRowReorder = true;
			this.listViewModules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameHeader,
            this.descriptionHeader,
            this.authorHeader,
            this.versionHeader,
            this.fileHeader});
			this.listViewModules.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewModules.Location = new System.Drawing.Point(14, 12);
			this.listViewModules.Name = "listViewModules";
			this.listViewModules.OwnerDraw = true;
			this.listViewModules.Size = new System.Drawing.Size(873, 309);
			this.listViewModules.TabIndex = 2;
			this.listViewModules.UseCompatibleStateImageBehavior = false;
			this.listViewModules.View = System.Windows.Forms.View.Details;
			this.listViewModules.SelectedIndexChanged += new System.EventHandler(this.listViewModules_SelectedIndexChanged);
			// 
			// nameHeader
			// 
			this.nameHeader.Text = "Name";
			// 
			// descriptionHeader
			// 
			this.descriptionHeader.Text = "Description";
			// 
			// authorHeader
			// 
			this.authorHeader.Text = "Author";
			// 
			// versionHeader
			// 
			this.versionHeader.Text = "Version";
			// 
			// fileHeader
			// 
			this.fileHeader.Text = "File";
			this.fileHeader.Width = 629;
			// 
			// buttonCopyModuleId
			// 
			this.buttonCopyModuleId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonCopyModuleId.Enabled = false;
			this.buttonCopyModuleId.Location = new System.Drawing.Point(14, 327);
			this.buttonCopyModuleId.Name = "buttonCopyModuleId";
			this.buttonCopyModuleId.Size = new System.Drawing.Size(122, 27);
			this.buttonCopyModuleId.TabIndex = 1;
			this.buttonCopyModuleId.Text = "Copy Module Id";
			this.buttonCopyModuleId.UseVisualStyleBackColor = true;
			this.buttonCopyModuleId.Click += new System.EventHandler(this.buttonCopyModuleId_Click);
			this.buttonCopyModuleId.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCopyModuleId.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonClose.Location = new System.Drawing.Point(793, 363);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(87, 27);
			this.buttonClose.TabIndex = 1;
			this.buttonClose.Text = "Close";
			this.buttonClose.UseVisualStyleBackColor = true;
			this.buttonClose.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonClose.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// InstalledModules
			// 
			this.AcceptButton = this.buttonClose;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(899, 418);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.panel1);
			this.MaximumSize = new System.Drawing.Size(915, 681);
			this.MinimumSize = new System.Drawing.Size(915, 456);
			this.Name = "InstalledModules";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Installed Modules";
			this.Load += new System.EventHandler(this.InstalledModules_Load);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.Button buttonCopyModuleId;
		private Common.Controls.ListViewEx listViewModules;
		private System.Windows.Forms.ColumnHeader nameHeader;
		private System.Windows.Forms.ColumnHeader descriptionHeader;
		private System.Windows.Forms.ColumnHeader authorHeader;
		private System.Windows.Forms.ColumnHeader versionHeader;
		private System.Windows.Forms.ColumnHeader fileHeader;
	}
}