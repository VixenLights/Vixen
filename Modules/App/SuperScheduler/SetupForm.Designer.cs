namespace VixenModules.App.SuperScheduler
{
	partial class SetupForm
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupForm));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonClose = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.buttonEditShow = new System.Windows.Forms.Button();
			this.listViewItems = new System.Windows.Forms.ListView();
			this.columnShow = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnSchedule = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonDeleteSchedule = new System.Windows.Forms.Button();
			this.buttonEditSchedule = new System.Windows.Forms.Button();
			this.buttonAddSchedule = new System.Windows.Forms.Button();
			this.buttonHelp = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.contextMenuStripList = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.addAScheduledShowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editTheSelectedScheduleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteTheSelectedScheduleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.editTheAssociatedShowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.contextMenuStripList.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(660, 66);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Shows";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(648, 45);
			this.label1.TabIndex = 0;
			this.label1.Text = resources.GetString("label1.Text");
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// buttonClose
			// 
			this.buttonClose.Location = new System.Drawing.Point(591, 377);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(75, 23);
			this.buttonClose.TabIndex = 6;
			this.buttonClose.Text = "Close";
			this.buttonClose.UseVisualStyleBackColor = true;
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.buttonEditShow);
			this.groupBox2.Controls.Add(this.listViewItems);
			this.groupBox2.Controls.Add(this.buttonDeleteSchedule);
			this.groupBox2.Controls.Add(this.buttonEditSchedule);
			this.groupBox2.Controls.Add(this.buttonAddSchedule);
			this.groupBox2.Location = new System.Drawing.Point(12, 84);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(660, 282);
			this.groupBox2.TabIndex = 7;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Scheduled Shows";
			// 
			// buttonEditShow
			// 
			this.buttonEditShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonEditShow.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonEditShow.BackgroundImage")));
			this.buttonEditShow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.buttonEditShow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonEditShow.Location = new System.Drawing.Point(9, 252);
			this.buttonEditShow.Name = "buttonEditShow";
			this.buttonEditShow.Size = new System.Drawing.Size(24, 24);
			this.buttonEditShow.TabIndex = 10;
			this.toolTip1.SetToolTip(this.buttonEditShow, "Edit Selected Show");
			this.buttonEditShow.UseVisualStyleBackColor = true;
			this.buttonEditShow.Click += new System.EventHandler(this.buttonEditShow_Click);
			// 
			// listViewItems
			// 
			this.listViewItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnShow,
            this.columnSchedule,
            this.columnStatus});
			this.listViewItems.ContextMenuStrip = this.contextMenuStripList;
			this.listViewItems.FullRowSelect = true;
			this.listViewItems.Location = new System.Drawing.Point(9, 19);
			this.listViewItems.MultiSelect = false;
			this.listViewItems.Name = "listViewItems";
			this.listViewItems.Size = new System.Drawing.Size(645, 227);
			this.listViewItems.TabIndex = 8;
			this.listViewItems.UseCompatibleStateImageBehavior = false;
			this.listViewItems.View = System.Windows.Forms.View.Details;
			this.listViewItems.DoubleClick += new System.EventHandler(this.listViewItems_DoubleClick);
			// 
			// columnShow
			// 
			this.columnShow.Text = "Show";
			this.columnShow.Width = 100;
			// 
			// columnSchedule
			// 
			this.columnSchedule.Text = "Schedule";
			this.columnSchedule.Width = 460;
			// 
			// columnStatus
			// 
			this.columnStatus.Text = "Status";
			// 
			// buttonDeleteSchedule
			// 
			this.buttonDeleteSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDeleteSchedule.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonDeleteSchedule.BackgroundImage")));
			this.buttonDeleteSchedule.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.buttonDeleteSchedule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonDeleteSchedule.Location = new System.Drawing.Point(630, 252);
			this.buttonDeleteSchedule.Name = "buttonDeleteSchedule";
			this.buttonDeleteSchedule.Size = new System.Drawing.Size(24, 24);
			this.buttonDeleteSchedule.TabIndex = 7;
			this.toolTip1.SetToolTip(this.buttonDeleteSchedule, "Delete the Selected Schedule");
			this.buttonDeleteSchedule.UseVisualStyleBackColor = true;
			this.buttonDeleteSchedule.Click += new System.EventHandler(this.buttonDeleteSchedule_Click);
			// 
			// buttonEditSchedule
			// 
			this.buttonEditSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonEditSchedule.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonEditSchedule.BackgroundImage")));
			this.buttonEditSchedule.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.buttonEditSchedule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonEditSchedule.Location = new System.Drawing.Point(605, 252);
			this.buttonEditSchedule.Name = "buttonEditSchedule";
			this.buttonEditSchedule.Size = new System.Drawing.Size(24, 24);
			this.buttonEditSchedule.TabIndex = 6;
			this.toolTip1.SetToolTip(this.buttonEditSchedule, "Edit the Selected Schedule");
			this.buttonEditSchedule.UseVisualStyleBackColor = true;
			this.buttonEditSchedule.Click += new System.EventHandler(this.buttonEditSchedule_Click);
			// 
			// buttonAddSchedule
			// 
			this.buttonAddSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonAddSchedule.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonAddSchedule.BackgroundImage")));
			this.buttonAddSchedule.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.buttonAddSchedule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAddSchedule.Location = new System.Drawing.Point(580, 252);
			this.buttonAddSchedule.Name = "buttonAddSchedule";
			this.buttonAddSchedule.Size = new System.Drawing.Size(24, 24);
			this.buttonAddSchedule.TabIndex = 5;
			this.toolTip1.SetToolTip(this.buttonAddSchedule, "Add a New Scheduled Show");
			this.buttonAddSchedule.UseVisualStyleBackColor = true;
			this.buttonAddSchedule.Click += new System.EventHandler(this.buttonAddSchedule_Click);
			// 
			// buttonHelp
			// 
			this.buttonHelp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.buttonHelp.Image = ((System.Drawing.Image)(resources.GetObject("buttonHelp.Image")));
			this.buttonHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonHelp.Location = new System.Drawing.Point(12, 377);
			this.buttonHelp.Name = "buttonHelp";
			this.buttonHelp.Size = new System.Drawing.Size(60, 23);
			this.buttonHelp.TabIndex = 61;
			this.buttonHelp.Tag = "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/nutcracker-eff" +
    "ects/";
			this.buttonHelp.Text = "Help";
			this.buttonHelp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.buttonHelp.UseVisualStyleBackColor = true;
			this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
			// 
			// contextMenuStripList
			// 
			this.contextMenuStripList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addAScheduledShowToolStripMenuItem,
            this.editTheSelectedScheduleToolStripMenuItem,
            this.deleteTheSelectedScheduleToolStripMenuItem,
            this.toolStripMenuItem1,
            this.editTheAssociatedShowToolStripMenuItem});
			this.contextMenuStripList.Name = "contextMenuStrip1";
			this.contextMenuStripList.Size = new System.Drawing.Size(226, 98);
			// 
			// addAScheduledShowToolStripMenuItem
			// 
			this.addAScheduledShowToolStripMenuItem.Name = "addAScheduledShowToolStripMenuItem";
			this.addAScheduledShowToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
			this.addAScheduledShowToolStripMenuItem.Text = "Add a Scheduled Show...";
			this.addAScheduledShowToolStripMenuItem.Click += new System.EventHandler(this.addAScheduledShowToolStripMenuItem_Click);
			// 
			// editTheSelectedScheduleToolStripMenuItem
			// 
			this.editTheSelectedScheduleToolStripMenuItem.Name = "editTheSelectedScheduleToolStripMenuItem";
			this.editTheSelectedScheduleToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
			this.editTheSelectedScheduleToolStripMenuItem.Text = "Edit the Selected Schedule...";
			this.editTheSelectedScheduleToolStripMenuItem.Click += new System.EventHandler(this.editTheSelectedScheduleToolStripMenuItem_Click);
			// 
			// deleteTheSelectedScheduleToolStripMenuItem
			// 
			this.deleteTheSelectedScheduleToolStripMenuItem.Name = "deleteTheSelectedScheduleToolStripMenuItem";
			this.deleteTheSelectedScheduleToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
			this.deleteTheSelectedScheduleToolStripMenuItem.Text = "Delete the Selected Schedule";
			this.deleteTheSelectedScheduleToolStripMenuItem.Click += new System.EventHandler(this.deleteTheSelectedScheduleToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(222, 6);
			// 
			// editTheAssociatedShowToolStripMenuItem
			// 
			this.editTheAssociatedShowToolStripMenuItem.Name = "editTheAssociatedShowToolStripMenuItem";
			this.editTheAssociatedShowToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
			this.editTheAssociatedShowToolStripMenuItem.Text = "Edit the Associated Show...";
			this.editTheAssociatedShowToolStripMenuItem.Click += new System.EventHandler(this.editTheAssociatedShowToolStripMenuItem_Click);
			// 
			// SetupForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(684, 412);
			this.Controls.Add(this.buttonHelp);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "SetupForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Scheduler";
			this.Load += new System.EventHandler(this.SetupForm_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.contextMenuStripList.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button buttonDeleteSchedule;
		private System.Windows.Forms.Button buttonEditSchedule;
		private System.Windows.Forms.Button buttonAddSchedule;
		private System.Windows.Forms.ListView listViewItems;
		private System.Windows.Forms.ColumnHeader columnStatus;
		private System.Windows.Forms.ColumnHeader columnSchedule;
		private System.Windows.Forms.Button buttonHelp;
		private System.Windows.Forms.ColumnHeader columnShow;
		private System.Windows.Forms.Button buttonEditShow;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripList;
		private System.Windows.Forms.ToolStripMenuItem addAScheduledShowToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editTheSelectedScheduleToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteTheSelectedScheduleToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem editTheAssociatedShowToolStripMenuItem;
	}
}