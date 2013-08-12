namespace VersionControl {
	partial class Versioning {
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.treeViewFiles = new System.Windows.Forms.TreeView();
			this.chkEnabled = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.chkEnabled);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(674, 439);
			this.splitContainer1.SplitterDistance = 40;
			this.splitContainer1.TabIndex = 0;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.treeViewFiles);
			this.splitContainer2.Size = new System.Drawing.Size(674, 395);
			this.splitContainer2.SplitterDistance = 274;
			this.splitContainer2.TabIndex = 0;
			// 
			// treeViewFiles
			// 
			this.treeViewFiles.BackColor = System.Drawing.SystemColors.Control;
			this.treeViewFiles.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.treeViewFiles.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewFiles.Location = new System.Drawing.Point(0, 0);
			this.treeViewFiles.Name = "treeViewFiles";
			this.treeViewFiles.Size = new System.Drawing.Size(274, 395);
			this.treeViewFiles.TabIndex = 0;
			this.treeViewFiles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewFiles_AfterSelect);
			// 
			// chkEnabled
			// 
			this.chkEnabled.AutoSize = true;
			this.chkEnabled.Location = new System.Drawing.Point(12, 12);
			this.chkEnabled.Name = "chkEnabled";
			this.chkEnabled.Size = new System.Drawing.Size(170, 17);
			this.chkEnabled.TabIndex = 0;
			this.chkEnabled.Text = "Enable Version Control System";
			this.chkEnabled.UseVisualStyleBackColor = true;
			// 
			// Versioning
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(674, 439);
			this.Controls.Add(this.splitContainer1);
			this.Name = "Versioning";
			this.Text = "Vixen 3 Version Control";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.CheckBox chkEnabled;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.TreeView treeViewFiles;
	}
}