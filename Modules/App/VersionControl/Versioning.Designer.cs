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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.listBoxChangeHistory = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtChangeDate = new System.Windows.Forms.TextBox();
            this.txtChangeUser = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtChangeHash = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtChangeFileName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtChangeMessage = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnRestore = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
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
            this.splitContainer2.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox3);
            this.splitContainer2.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer2.Size = new System.Drawing.Size(674, 395);
            this.splitContainer2.SplitterDistance = 274;
            this.splitContainer2.TabIndex = 0;
            // 
            // treeViewFiles
            // 
            this.treeViewFiles.BackColor = System.Drawing.SystemColors.Window;
            this.treeViewFiles.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewFiles.Location = new System.Drawing.Point(3, 16);
            this.treeViewFiles.Name = "treeViewFiles";
            this.treeViewFiles.Size = new System.Drawing.Size(268, 376);
            this.treeViewFiles.TabIndex = 0;
            this.treeViewFiles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewFiles_AfterSelect);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBoxChangeHistory);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(396, 173);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Change History";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.treeViewFiles);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(274, 395);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Files under Version Control";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnRestore);
            this.groupBox3.Controls.Add(this.txtChangeMessage);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.txtChangeFileName);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.txtChangeHash);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.txtChangeUser);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.txtChangeDate);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 173);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(396, 222);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Details";
            // 
            // listBoxChangeHistory
            // 
            this.listBoxChangeHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxChangeHistory.FormattingEnabled = true;
            this.listBoxChangeHistory.Location = new System.Drawing.Point(3, 16);
            this.listBoxChangeHistory.Name = "listBoxChangeHistory";
            this.listBoxChangeHistory.Size = new System.Drawing.Size(390, 154);
            this.listBoxChangeHistory.TabIndex = 0;
            this.listBoxChangeHistory.SelectedIndexChanged += new System.EventHandler(this.listBoxChangeHistory_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Change Date";
            // 
            // txtChangeDate
            // 
            this.txtChangeDate.Location = new System.Drawing.Point(110, 50);
            this.txtChangeDate.Name = "txtChangeDate";
            this.txtChangeDate.ReadOnly = true;
            this.txtChangeDate.Size = new System.Drawing.Size(262, 20);
            this.txtChangeDate.TabIndex = 1;
            // 
            // txtChangeUser
            // 
            this.txtChangeUser.Location = new System.Drawing.Point(110, 76);
            this.txtChangeUser.Name = "txtChangeUser";
            this.txtChangeUser.ReadOnly = true;
            this.txtChangeUser.Size = new System.Drawing.Size(262, 20);
            this.txtChangeUser.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Change User";
            // 
            // txtChangeHash
            // 
            this.txtChangeHash.Location = new System.Drawing.Point(110, 102);
            this.txtChangeHash.Name = "txtChangeHash";
            this.txtChangeHash.ReadOnly = true;
            this.txtChangeHash.Size = new System.Drawing.Size(262, 20);
            this.txtChangeHash.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(69, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Hash";
            // 
            // txtChangeFileName
            // 
            this.txtChangeFileName.Location = new System.Drawing.Point(110, 24);
            this.txtChangeFileName.Name = "txtChangeFileName";
            this.txtChangeFileName.ReadOnly = true;
            this.txtChangeFileName.Size = new System.Drawing.Size(262, 20);
            this.txtChangeFileName.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(47, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "File Name";
            // 
            // txtChangeMessage
            // 
            this.txtChangeMessage.Location = new System.Drawing.Point(110, 128);
            this.txtChangeMessage.Name = "txtChangeMessage";
            this.txtChangeMessage.ReadOnly = true;
            this.txtChangeMessage.Size = new System.Drawing.Size(262, 20);
            this.txtChangeMessage.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(54, 128);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Message";
            // 
            // btnRestore
            // 
            this.btnRestore.Location = new System.Drawing.Point(297, 163);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(75, 23);
            this.btnRestore.TabIndex = 10;
            this.btnRestore.Text = "Restore";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // Versioning
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(674, 439);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "Versioning";
            this.Text = "Vixen 3 Version Control";
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TreeView treeViewFiles;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox listBoxChangeHistory;
        private System.Windows.Forms.TextBox txtChangeHash;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtChangeUser;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtChangeDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtChangeFileName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtChangeMessage;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnRestore;
	}
}