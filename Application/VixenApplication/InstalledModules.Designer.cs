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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Controls.Add(this.label5);
			this.panel1.Controls.Add(this.label4);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.buttonCopyModuleId);
			this.panel1.Controls.Add(this.listViewModules);
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1150, 475);
			this.panel1.TabIndex = 0;
			// 
			// buttonCopyModuleId
			// 
			this.buttonCopyModuleId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonCopyModuleId.Enabled = false;
			this.buttonCopyModuleId.Location = new System.Drawing.Point(18, 435);
			this.buttonCopyModuleId.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.buttonCopyModuleId.Name = "buttonCopyModuleId";
			this.buttonCopyModuleId.Size = new System.Drawing.Size(158, 35);
			this.buttonCopyModuleId.TabIndex = 1;
			this.buttonCopyModuleId.Text = "Copy Module Id";
			this.buttonCopyModuleId.UseVisualStyleBackColor = true;
			this.buttonCopyModuleId.Click += new System.EventHandler(this.buttonCopyModuleId_Click);
			this.buttonCopyModuleId.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
			this.buttonCopyModuleId.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCopyModuleId.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
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
			this.listViewModules.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewModules.Location = new System.Drawing.Point(18, 41);
			this.listViewModules.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.listViewModules.MultiSelect = false;
			this.listViewModules.Name = "listViewModules";
			this.listViewModules.Size = new System.Drawing.Size(1112, 383);
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
			this.buttonClose.Location = new System.Drawing.Point(1020, 485);
			this.buttonClose.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(112, 35);
			this.buttonClose.TabIndex = 1;
			this.buttonClose.Text = "Close";
			this.buttonClose.UseVisualStyleBackColor = true;
			this.buttonClose.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
			this.buttonClose.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonClose.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(24, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(51, 20);
			this.label1.TabIndex = 2;
			this.label1.Text = "Name";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(251, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(89, 20);
			this.label2.TabIndex = 3;
			this.label2.Text = "Description";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(608, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(57, 20);
			this.label3.TabIndex = 4;
			this.label3.Text = "Author";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(754, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(63, 20);
			this.label4.TabIndex = 5;
			this.label4.Text = "Version";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(892, 16);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(34, 20);
			this.label5.TabIndex = 6;
			this.label5.Text = "File";
			// 
			// InstalledModules
			// 
			this.AcceptButton = this.buttonClose;
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1150, 538);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.panel1);
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.MaximumSize = new System.Drawing.Size(1172, 894);
			this.MinimumSize = new System.Drawing.Size(1172, 594);
			this.Name = "InstalledModules";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Installed Modules";
			this.Load += new System.EventHandler(this.InstalledModules_Load);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
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
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
	}
}