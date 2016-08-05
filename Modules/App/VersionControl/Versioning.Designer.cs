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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Versioning));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.treeViewFiles = new System.Windows.Forms.TreeView();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.txtHidden = new System.Windows.Forms.TextBox();
			this.btnRestore = new System.Windows.Forms.Button();
			this.txtChangeMessage = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.txtChangeFileName = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.txtChangeHash = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtChangeUser = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtChangeDate = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.listBoxChangeHistory = new System.Windows.Forms.ListBox();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox1.SuspendLayout();
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
			this.splitContainer1.Panel1.Controls.Add(this.label7);
			this.splitContainer1.Panel1.Controls.Add(this.label6);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(786, 467);
			this.splitContainer1.SplitterDistance = 41;
			this.splitContainer1.SplitterWidth = 5;
			this.splitContainer1.TabIndex = 0;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(337, 6);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(312, 30);
			this.label7.TabIndex = 2;
			this.label7.Text = "When enabled, will keep version history of important files \r\nautomatically on eve" +
    "ry save";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(3, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(261, 24);
			this.label6.TabIndex = 0;
			this.label6.Text = "Automated Version Control";
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
			this.splitContainer2.Size = new System.Drawing.Size(786, 421);
			this.splitContainer2.SplitterDistance = 319;
			this.splitContainer2.SplitterWidth = 5;
			this.splitContainer2.TabIndex = 0;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.treeViewFiles);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(0, 0);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(319, 421);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Files under Version Control";
			this.groupBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// treeViewFiles
			// 
			this.treeViewFiles.BackColor = System.Drawing.SystemColors.Window;
			this.treeViewFiles.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.treeViewFiles.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewFiles.Location = new System.Drawing.Point(3, 19);
			this.treeViewFiles.Name = "treeViewFiles";
			this.treeViewFiles.Size = new System.Drawing.Size(313, 399);
			this.treeViewFiles.TabIndex = 0;
			this.treeViewFiles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewFiles_AfterSelect);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.txtHidden);
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
			this.groupBox3.Location = new System.Drawing.Point(0, 200);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(462, 221);
			this.groupBox3.TabIndex = 1;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Details";
			this.groupBox3.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// txtHidden
			// 
			this.txtHidden.Location = new System.Drawing.Point(84, 178);
			this.txtHidden.Name = "txtHidden";
			this.txtHidden.ReadOnly = true;
			this.txtHidden.Size = new System.Drawing.Size(111, 23);
			this.txtHidden.TabIndex = 11;
			this.txtHidden.Visible = false;
			// 
			// btnRestore
			// 
			this.btnRestore.Location = new System.Drawing.Point(346, 188);
			this.btnRestore.Name = "btnRestore";
			this.btnRestore.Size = new System.Drawing.Size(87, 27);
			this.btnRestore.TabIndex = 10;
			this.btnRestore.Text = "Restore";
			this.btnRestore.UseVisualStyleBackColor = true;
			this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
			// 
			// txtChangeMessage
			// 
			this.txtChangeMessage.Location = new System.Drawing.Point(128, 148);
			this.txtChangeMessage.Name = "txtChangeMessage";
			this.txtChangeMessage.ReadOnly = true;
			this.txtChangeMessage.Size = new System.Drawing.Size(305, 23);
			this.txtChangeMessage.TabIndex = 9;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(63, 148);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(53, 15);
			this.label5.TabIndex = 8;
			this.label5.Text = "Message";
			// 
			// txtChangeFileName
			// 
			this.txtChangeFileName.Location = new System.Drawing.Point(128, 28);
			this.txtChangeFileName.Name = "txtChangeFileName";
			this.txtChangeFileName.ReadOnly = true;
			this.txtChangeFileName.Size = new System.Drawing.Size(305, 23);
			this.txtChangeFileName.TabIndex = 7;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(55, 31);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(60, 15);
			this.label4.TabIndex = 6;
			this.label4.Text = "File Name";
			// 
			// txtChangeHash
			// 
			this.txtChangeHash.Location = new System.Drawing.Point(128, 118);
			this.txtChangeHash.Name = "txtChangeHash";
			this.txtChangeHash.ReadOnly = true;
			this.txtChangeHash.Size = new System.Drawing.Size(305, 23);
			this.txtChangeHash.TabIndex = 5;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(80, 121);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(34, 15);
			this.label3.TabIndex = 4;
			this.label3.Text = "Hash";
			// 
			// txtChangeUser
			// 
			this.txtChangeUser.Location = new System.Drawing.Point(128, 88);
			this.txtChangeUser.Name = "txtChangeUser";
			this.txtChangeUser.ReadOnly = true;
			this.txtChangeUser.Size = new System.Drawing.Size(305, 23);
			this.txtChangeUser.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(41, 91);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(74, 15);
			this.label2.TabIndex = 2;
			this.label2.Text = "Change User";
			// 
			// txtChangeDate
			// 
			this.txtChangeDate.Location = new System.Drawing.Point(128, 58);
			this.txtChangeDate.Name = "txtChangeDate";
			this.txtChangeDate.ReadOnly = true;
			this.txtChangeDate.Size = new System.Drawing.Size(305, 23);
			this.txtChangeDate.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(40, 61);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(75, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "Change Date";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.listBoxChangeHistory);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(462, 200);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Change History";
			this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// listBoxChangeHistory
			// 
			this.listBoxChangeHistory.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBoxChangeHistory.FormattingEnabled = true;
			this.listBoxChangeHistory.ItemHeight = 15;
			this.listBoxChangeHistory.Location = new System.Drawing.Point(3, 19);
			this.listBoxChangeHistory.Name = "listBoxChangeHistory";
			this.listBoxChangeHistory.Size = new System.Drawing.Size(456, 178);
			this.listBoxChangeHistory.TabIndex = 0;
			this.listBoxChangeHistory.SelectedIndexChanged += new System.EventHandler(this.listBoxChangeHistory_SelectedIndexChanged);
			// 
			// Versioning
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(786, 467);
			this.Controls.Add(this.splitContainer1);
			this.Name = "Versioning";
			this.Text = "Vixen 3 Version Control";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox1.ResumeLayout(false);
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
        private System.Windows.Forms.TextBox txtHidden;
		private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
	}
}