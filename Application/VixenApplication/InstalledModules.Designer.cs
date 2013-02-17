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
			this.buttonCopyModuleId = new System.Windows.Forms.Button();
			this.listViewModules = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonClose = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Controls.Add(this.buttonCopyModuleId);
			this.panel1.Controls.Add(this.listViewModules);
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(700, 309);
			this.panel1.TabIndex = 0;
			// 
			// buttonCopyModuleId
			// 
			this.buttonCopyModuleId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonCopyModuleId.Enabled = false;
			this.buttonCopyModuleId.Location = new System.Drawing.Point(12, 283);
			this.buttonCopyModuleId.Name = "buttonCopyModuleId";
			this.buttonCopyModuleId.Size = new System.Drawing.Size(105, 23);
			this.buttonCopyModuleId.TabIndex = 1;
			this.buttonCopyModuleId.Text = "Copy Module Id";
			this.buttonCopyModuleId.UseVisualStyleBackColor = true;
			this.buttonCopyModuleId.Click += new System.EventHandler(this.buttonCopyModuleId_Click);
			// 
			// listViewModules
			// 
			this.listViewModules.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewModules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
			this.listViewModules.FullRowSelect = true;
			this.listViewModules.Location = new System.Drawing.Point(12, 12);
			this.listViewModules.MultiSelect = false;
			this.listViewModules.Name = "listViewModules";
			this.listViewModules.Size = new System.Drawing.Size(676, 265);
			this.listViewModules.TabIndex = 0;
			this.listViewModules.UseCompatibleStateImageBehavior = false;
			this.listViewModules.View = System.Windows.Forms.View.Details;
			this.listViewModules.SelectedIndexChanged += new System.EventHandler(this.listViewModules_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Name";
			this.columnHeader1.Width = 150;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Description";
			this.columnHeader2.Width = 240;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Author";
			this.columnHeader3.Width = 100;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Version";
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "File";
			this.columnHeader5.Width = 100;
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonClose.Location = new System.Drawing.Point(613, 315);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(75, 23);
			this.buttonClose.TabIndex = 1;
			this.buttonClose.Text = "Close";
			this.buttonClose.UseVisualStyleBackColor = true;
			// 
			// InstalledModules
			// 
			this.AcceptButton = this.buttonClose;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(700, 350);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.panel1);
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
		private System.Windows.Forms.ListView listViewModules;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader5;
	}
}