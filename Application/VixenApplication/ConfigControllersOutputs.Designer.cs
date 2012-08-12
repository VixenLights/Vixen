namespace VixenApplication
{
	partial class ConfigControllersOutputs
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
		private void InitializeComponent()
		{
			this.listViewOutputs = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.groupBox = new System.Windows.Forms.GroupBox();
			this.buttonUpdate = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonBulkRename = new System.Windows.Forms.Button();
			this.groupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// listViewOutputs
			// 
			this.listViewOutputs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.listViewOutputs.FullRowSelect = true;
			this.listViewOutputs.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewOutputs.HideSelection = false;
			this.listViewOutputs.Location = new System.Drawing.Point(12, 12);
			this.listViewOutputs.Name = "listViewOutputs";
			this.listViewOutputs.ShowGroups = false;
			this.listViewOutputs.Size = new System.Drawing.Size(333, 286);
			this.listViewOutputs.TabIndex = 0;
			this.listViewOutputs.UseCompatibleStateImageBehavior = false;
			this.listViewOutputs.View = System.Windows.Forms.View.Details;
			this.listViewOutputs.SelectedIndexChanged += new System.EventHandler(this.listViewOutputs_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Output #";
			this.columnHeader1.Width = 56;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Name";
			this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.columnHeader2.Width = 240;
			// 
			// groupBox
			// 
			this.groupBox.Controls.Add(this.buttonUpdate);
			this.groupBox.Controls.Add(this.label1);
			this.groupBox.Controls.Add(this.textBoxName);
			this.groupBox.Location = new System.Drawing.Point(12, 304);
			this.groupBox.Name = "groupBox";
			this.groupBox.Size = new System.Drawing.Size(333, 70);
			this.groupBox.TabIndex = 5;
			this.groupBox.TabStop = false;
			this.groupBox.Text = "Selected Output:";
			// 
			// buttonUpdate
			// 
			this.buttonUpdate.Location = new System.Drawing.Point(230, 22);
			this.buttonUpdate.Name = "buttonUpdate";
			this.buttonUpdate.Size = new System.Drawing.Size(80, 25);
			this.buttonUpdate.TabIndex = 12;
			this.buttonUpdate.Text = "Update";
			this.buttonUpdate.UseVisualStyleBackColor = true;
			this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(28, 29);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "Name:";
			// 
			// textBoxName
			// 
			this.textBoxName.Location = new System.Drawing.Point(74, 26);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(138, 20);
			this.textBoxName.TabIndex = 6;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(265, 380);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(80, 25);
			this.buttonOK.TabIndex = 12;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonBulkRename
			// 
			this.buttonBulkRename.Enabled = false;
			this.buttonBulkRename.Location = new System.Drawing.Point(12, 380);
			this.buttonBulkRename.Name = "buttonBulkRename";
			this.buttonBulkRename.Size = new System.Drawing.Size(123, 25);
			this.buttonBulkRename.TabIndex = 13;
			this.buttonBulkRename.Text = "Bulk Rename Outputs";
			this.buttonBulkRename.UseVisualStyleBackColor = true;
			this.buttonBulkRename.Click += new System.EventHandler(this.buttonBulkRename_Click);
			// 
			// ConfigControllersOutputs
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(357, 415);
			this.Controls.Add(this.buttonBulkRename);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.groupBox);
			this.Controls.Add(this.listViewOutputs);
			this.DoubleBuffered = true;
			this.Name = "ConfigControllersOutputs";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Configure Controller Outputs";
			this.Load += new System.EventHandler(this.ConfigControllersOutputs_Load);
			this.groupBox.ResumeLayout(false);
			this.groupBox.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView listViewOutputs;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.GroupBox groupBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxName;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonUpdate;
		private System.Windows.Forms.Button buttonBulkRename;
	}
}