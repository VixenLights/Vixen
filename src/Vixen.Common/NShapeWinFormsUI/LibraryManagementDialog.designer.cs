namespace Dataweb.NShape.WinFormsUI {
	partial class LibraryManagementDialog {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
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
			this.addLibraryButton = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.okButton = new System.Windows.Forms.Button();
			this.libraryListView = new System.Windows.Forms.ListView();
			this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeaderVersion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeaderPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.removeLibraryButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// addLibraryButton
			// 
			this.addLibraryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.addLibraryButton.Location = new System.Drawing.Point(16, 266);
			this.addLibraryButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.addLibraryButton.Name = "addLibraryButton";
			this.addLibraryButton.Size = new System.Drawing.Size(153, 28);
			this.addLibraryButton.TabIndex = 4;
			this.addLibraryButton.Text = "Add Libraries...";
			this.addLibraryButton.UseVisualStyleBackColor = true;
			this.addLibraryButton.Click += new System.EventHandler(this.addLibraryButton_Click);
			// 
			// openFileDialog
			// 
			this.openFileDialog.FileName = "openFileDialog";
			this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog_FileOk);
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(528, 266);
			this.okButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(100, 28);
			this.okButton.TabIndex = 5;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// libraryListView
			// 
			this.libraryListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.libraryListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderVersion,
            this.columnHeaderPath});
			this.libraryListView.Location = new System.Drawing.Point(4, 2);
			this.libraryListView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.libraryListView.Name = "libraryListView";
			this.libraryListView.Size = new System.Drawing.Size(741, 245);
			this.libraryListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.libraryListView.TabIndex = 6;
			this.libraryListView.UseCompatibleStateImageBehavior = false;
			this.libraryListView.View = System.Windows.Forms.View.Details;
			// 
			// columnHeaderName
			// 
			this.columnHeaderName.Text = "Library Name";
			this.columnHeaderName.Width = 244;
			// 
			// columnHeaderVersion
			// 
			this.columnHeaderVersion.Text = "Version";
			this.columnHeaderVersion.Width = 54;
			// 
			// columnHeaderPath
			// 
			this.columnHeaderPath.Text = "Library Path";
			this.columnHeaderPath.Width = 254;
			// 
			// removeLibraryButton
			// 
			this.removeLibraryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.removeLibraryButton.Location = new System.Drawing.Point(177, 266);
			this.removeLibraryButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.removeLibraryButton.Name = "removeLibraryButton";
			this.removeLibraryButton.Size = new System.Drawing.Size(153, 28);
			this.removeLibraryButton.TabIndex = 7;
			this.removeLibraryButton.Text = "Remove Libraries";
			this.removeLibraryButton.UseVisualStyleBackColor = true;
			this.removeLibraryButton.Click += new System.EventHandler(this.removeLibraryButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(636, 266);
			this.cancelButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(100, 28);
			this.cancelButton.TabIndex = 8;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// LibraryManagementDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.okButton;
			this.ClientSize = new System.Drawing.Size(752, 306);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.removeLibraryButton);
			this.Controls.Add(this.libraryListView);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.addLibraryButton);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "LibraryManagementDialog";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Library Manager";
			this.Load += new System.EventHandler(this.LibraryManagementDialog_Load);
			this.Shown += new System.EventHandler(this.LibraryManagementDialog_Shown);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button addLibraryButton;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.ListView libraryListView;
		private System.Windows.Forms.ColumnHeader columnHeaderName;
		private System.Windows.Forms.ColumnHeader columnHeaderVersion;
		private System.Windows.Forms.ColumnHeader columnHeaderPath;
		private System.Windows.Forms.Button removeLibraryButton;
		private System.Windows.Forms.Button cancelButton;
	}
}