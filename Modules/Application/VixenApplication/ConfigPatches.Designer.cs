namespace VixenApplication
{
	partial class ConfigPatches
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
			this.listViewChannels = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonOk = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.domainUpDownOutputNumber = new System.Windows.Forms.DomainUpDown();
			this.buttonAddPatch = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.listView2 = new System.Windows.Forms.ListView();
			this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonRemovePatch = new System.Windows.Forms.Button();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// listViewChannels
			// 
			this.listViewChannels.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
			this.listViewChannels.FullRowSelect = true;
			this.listViewChannels.GridLines = true;
			this.listViewChannels.Location = new System.Drawing.Point(56, 117);
			this.listViewChannels.MultiSelect = false;
			this.listViewChannels.Name = "listViewChannels";
			this.listViewChannels.Size = new System.Drawing.Size(508, 273);
			this.listViewChannels.TabIndex = 0;
			this.listViewChannels.UseCompatibleStateImageBehavior = false;
			this.listViewChannels.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Channel";
			this.columnHeader1.Width = 120;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Patches";
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Patched To...";
			this.columnHeader3.Width = 300;
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(533, 643);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(90, 30);
			this.buttonOk.TabIndex = 25;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.domainUpDownOutputNumber);
			this.groupBox2.Controls.Add(this.buttonAddPatch);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.listView2);
			this.groupBox2.Controls.Add(this.buttonRemovePatch);
			this.groupBox2.Controls.Add(this.listView1);
			this.groupBox2.Location = new System.Drawing.Point(38, 406);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(561, 211);
			this.groupBox2.TabIndex = 27;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Selected Channel Patches";
			// 
			// domainUpDownOutputNumber
			// 
			this.domainUpDownOutputNumber.Location = new System.Drawing.Point(331, 138);
			this.domainUpDownOutputNumber.Name = "domainUpDownOutputNumber";
			this.domainUpDownOutputNumber.Size = new System.Drawing.Size(64, 20);
			this.domainUpDownOutputNumber.TabIndex = 33;
			this.domainUpDownOutputNumber.Text = "0";
			// 
			// buttonAddPatch
			// 
			this.buttonAddPatch.Location = new System.Drawing.Point(401, 138);
			this.buttonAddPatch.Name = "buttonAddPatch";
			this.buttonAddPatch.Size = new System.Drawing.Size(107, 30);
			this.buttonAddPatch.TabIndex = 32;
			this.buttonAddPatch.Text = "Add Patch";
			this.buttonAddPatch.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(271, 140);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(52, 13);
			this.label2.TabIndex = 31;
			this.label2.Text = "Output #:";
			// 
			// listView2
			// 
			this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader6});
			this.listView2.FullRowSelect = true;
			this.listView2.GridLines = true;
			this.listView2.Location = new System.Drawing.Point(274, 19);
			this.listView2.MultiSelect = false;
			this.listView2.Name = "listView2";
			this.listView2.Size = new System.Drawing.Size(234, 97);
			this.listView2.TabIndex = 29;
			this.listView2.UseCompatibleStateImageBehavior = false;
			this.listView2.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader7
			// 
			this.columnHeader7.Text = "Controller";
			this.columnHeader7.Width = 101;
			// 
			// columnHeader8
			// 
			this.columnHeader8.Text = "Type";
			this.columnHeader8.Width = 66;
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "Channels";
			// 
			// buttonRemovePatch
			// 
			this.buttonRemovePatch.Location = new System.Drawing.Point(38, 131);
			this.buttonRemovePatch.Name = "buttonRemovePatch";
			this.buttonRemovePatch.Size = new System.Drawing.Size(120, 30);
			this.buttonRemovePatch.TabIndex = 23;
			this.buttonRemovePatch.Text = "Remove Patch";
			this.buttonRemovePatch.UseVisualStyleBackColor = true;
			// 
			// listView1
			// 
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5});
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.Location = new System.Drawing.Point(18, 19);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(171, 97);
			this.listView1.TabIndex = 22;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Controller";
			this.columnHeader4.Width = 97;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Output";
			this.columnHeader5.Width = 48;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(5, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(626, 13);
			this.label1.TabIndex = 28;
			this.label1.Text = "This form probably won\'t be used anymore. Patching can now be done in the channel" +
				" config, and seems to make more sense there.";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(168, 57);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(209, 13);
			this.label3.TabIndex = 29;
			this.label3.Text = "It hasn\'t had any functionality implemented.";
			// 
			// ConfigPatches
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(635, 685);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.listViewChannels);
			this.Name = "ConfigPatches";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Configure Patches";
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListView listViewChannels;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.DomainUpDown domainUpDownOutputNumber;
		private System.Windows.Forms.Button buttonAddPatch;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListView listView2;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.ColumnHeader columnHeader8;
		private System.Windows.Forms.Button buttonRemovePatch;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
	}
}