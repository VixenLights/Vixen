namespace VixenApplication
{
	partial class VixenApplication
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
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("asdfadsa");
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("rewqrewq");
			System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("vbcbxvxc");
			System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("gfdsgfsd");
			System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("ytreyre");
			System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("xvcbxvcx");
			this.contextMenuStripNewSequence = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.menuStripMain = new System.Windows.Forms.MenuStrip();
			this.vixenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.logsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewInstalledModulesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.executionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.channelGroupTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.label2 = new System.Windows.Forms.Label();
			this.buttonOpenSequence = new System.Windows.Forms.Button();
			this.buttonNewSequence = new System.Windows.Forms.Button();
			this.groupBoxSequences = new System.Windows.Forms.GroupBox();
			this.listViewRecentSequences = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.panel1 = new System.Windows.Forms.Panel();
			this.label3 = new System.Windows.Forms.Label();
			this.labelVersion = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBoxSystemConfig = new System.Windows.Forms.GroupBox();
			this.buttonSetupOutputPreviews = new System.Windows.Forms.Button();
			this.buttonSetupOutputControllers = new System.Windows.Forms.Button();
			this.buttonSetupChannels = new System.Windows.Forms.Button();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabelExecutionLight = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabelExecutionState = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_memory = new System.Windows.Forms.ToolStripStatusLabel();
			this.button1 = new System.Windows.Forms.Button();
			this.menuStripMain.SuspendLayout();
			this.groupBoxSequences.SuspendLayout();
			this.panel1.SuspendLayout();
			this.groupBoxSystemConfig.SuspendLayout();
			this.statusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenuStripNewSequence
			// 
			this.contextMenuStripNewSequence.Name = "contextMenuStripNewSequence";
			this.contextMenuStripNewSequence.ShowImageMargin = false;
			this.contextMenuStripNewSequence.Size = new System.Drawing.Size(36, 4);
			// 
			// openFileDialog
			// 
			this.openFileDialog.Multiselect = true;
			// 
			// menuStripMain
			// 
			this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vixenToolStripMenuItem});
			this.menuStripMain.Location = new System.Drawing.Point(0, 0);
			this.menuStripMain.Name = "menuStripMain";
			this.menuStripMain.Size = new System.Drawing.Size(461, 24);
			this.menuStripMain.TabIndex = 2;
			this.menuStripMain.Text = "menuStrip1";
			// 
			// vixenToolStripMenuItem
			// 
			this.vixenToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logsToolStripMenuItem,
            this.viewInstalledModulesToolStripMenuItem,
            this.executionToolStripMenuItem,
            this.channelGroupTestToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
			this.vixenToolStripMenuItem.Name = "vixenToolStripMenuItem";
			this.vixenToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
			this.vixenToolStripMenuItem.Text = "System";
			// 
			// logsToolStripMenuItem
			// 
			this.logsToolStripMenuItem.Name = "logsToolStripMenuItem";
			this.logsToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.logsToolStripMenuItem.Text = "Logs";
			// 
			// viewInstalledModulesToolStripMenuItem
			// 
			this.viewInstalledModulesToolStripMenuItem.Name = "viewInstalledModulesToolStripMenuItem";
			this.viewInstalledModulesToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.viewInstalledModulesToolStripMenuItem.Text = "View Installed Modules";
			this.viewInstalledModulesToolStripMenuItem.Click += new System.EventHandler(this.viewInstalledModulesToolStripMenuItem_Click);
			// 
			// executionToolStripMenuItem
			// 
			this.executionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem});
			this.executionToolStripMenuItem.Name = "executionToolStripMenuItem";
			this.executionToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.executionToolStripMenuItem.Text = "Execution Engine";
			// 
			// startToolStripMenuItem
			// 
			this.startToolStripMenuItem.Name = "startToolStripMenuItem";
			this.startToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
			this.startToolStripMenuItem.Text = "Start";
			this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
			// 
			// stopToolStripMenuItem
			// 
			this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
			this.stopToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
			this.stopToolStripMenuItem.Text = "Stop";
			this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
			// 
			// channelGroupTestToolStripMenuItem
			// 
			this.channelGroupTestToolStripMenuItem.Name = "channelGroupTestToolStripMenuItem";
			this.channelGroupTestToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.channelGroupTestToolStripMenuItem.Text = "Channel/Group Test";
			this.channelGroupTestToolStripMenuItem.Click += new System.EventHandler(this.channelGroupTestToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(192, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.exitToolStripMenuItem.Text = "Shutdown and Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(15, 100);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(102, 13);
			this.label2.TabIndex = 8;
			this.label2.Text = "Recent Sequences:";
			// 
			// buttonOpenSequence
			// 
			this.buttonOpenSequence.Location = new System.Drawing.Point(18, 59);
			this.buttonOpenSequence.Name = "buttonOpenSequence";
			this.buttonOpenSequence.Size = new System.Drawing.Size(180, 30);
			this.buttonOpenSequence.TabIndex = 6;
			this.buttonOpenSequence.Text = "Open Sequence...";
			this.buttonOpenSequence.UseVisualStyleBackColor = true;
			this.buttonOpenSequence.Click += new System.EventHandler(this.buttonOpenSequence_Click);
			// 
			// buttonNewSequence
			// 
			this.buttonNewSequence.Location = new System.Drawing.Point(18, 23);
			this.buttonNewSequence.Name = "buttonNewSequence";
			this.buttonNewSequence.Size = new System.Drawing.Size(180, 30);
			this.buttonNewSequence.TabIndex = 5;
			this.buttonNewSequence.Text = "New Sequence...";
			this.buttonNewSequence.UseVisualStyleBackColor = true;
			this.buttonNewSequence.Click += new System.EventHandler(this.buttonNewSequence_Click);
			// 
			// groupBoxSequences
			// 
			this.groupBoxSequences.Controls.Add(this.listViewRecentSequences);
			this.groupBoxSequences.Controls.Add(this.buttonNewSequence);
			this.groupBoxSequences.Controls.Add(this.buttonOpenSequence);
			this.groupBoxSequences.Controls.Add(this.label2);
			this.groupBoxSequences.Location = new System.Drawing.Point(12, 133);
			this.groupBoxSequences.Name = "groupBoxSequences";
			this.groupBoxSequences.Size = new System.Drawing.Size(218, 227);
			this.groupBoxSequences.TabIndex = 10;
			this.groupBoxSequences.TabStop = false;
			this.groupBoxSequences.Text = "Sequences";
			// 
			// listViewRecentSequences
			// 
			this.listViewRecentSequences.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.listViewRecentSequences.FullRowSelect = true;
			this.listViewRecentSequences.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewRecentSequences.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6});
			this.listViewRecentSequences.Location = new System.Drawing.Point(18, 116);
			this.listViewRecentSequences.MultiSelect = false;
			this.listViewRecentSequences.Name = "listViewRecentSequences";
			this.listViewRecentSequences.Size = new System.Drawing.Size(180, 100);
			this.listViewRecentSequences.TabIndex = 9;
			this.listViewRecentSequences.UseCompatibleStateImageBehavior = false;
			this.listViewRecentSequences.View = System.Windows.Forms.View.Details;
			this.listViewRecentSequences.DoubleClick += new System.EventHandler(this.listViewRecentSequences_DoubleClick);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Width = 150;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackColor = System.Drawing.Color.White;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.labelVersion);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Location = new System.Drawing.Point(0, 27);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(461, 100);
			this.panel1.TabIndex = 11;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.ForeColor = System.Drawing.Color.Gray;
			this.label3.Location = new System.Drawing.Point(139, 78);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(173, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "(Help us come up with a new logo!)";
			// 
			// labelVersion
			// 
			this.labelVersion.AutoSize = true;
			this.labelVersion.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelVersion.ForeColor = System.Drawing.Color.DarkGray;
			this.labelVersion.Location = new System.Drawing.Point(326, 38);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(63, 22);
			this.labelVersion.TabIndex = 1;
			this.labelVersion.Text = "[0.0.0]";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Arial", 36F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.Gray;
			this.label1.Location = new System.Drawing.Point(12, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(308, 55);
			this.label1.TabIndex = 0;
			this.label1.Text = "Vixen 3 Beta";
			// 
			// groupBoxSystemConfig
			// 
			this.groupBoxSystemConfig.Controls.Add(this.buttonSetupOutputPreviews);
			this.groupBoxSystemConfig.Controls.Add(this.buttonSetupOutputControllers);
			this.groupBoxSystemConfig.Controls.Add(this.buttonSetupChannels);
			this.groupBoxSystemConfig.Location = new System.Drawing.Point(236, 133);
			this.groupBoxSystemConfig.Name = "groupBoxSystemConfig";
			this.groupBoxSystemConfig.Size = new System.Drawing.Size(204, 142);
			this.groupBoxSystemConfig.TabIndex = 12;
			this.groupBoxSystemConfig.TabStop = false;
			this.groupBoxSystemConfig.Text = "System Configuration";
			// 
			// buttonSetupOutputPreviews
			// 
			this.buttonSetupOutputPreviews.Location = new System.Drawing.Point(12, 103);
			this.buttonSetupOutputPreviews.Name = "buttonSetupOutputPreviews";
			this.buttonSetupOutputPreviews.Size = new System.Drawing.Size(180, 30);
			this.buttonSetupOutputPreviews.TabIndex = 7;
			this.buttonSetupOutputPreviews.Text = "Configure Previews";
			this.buttonSetupOutputPreviews.UseVisualStyleBackColor = true;
			this.buttonSetupOutputPreviews.Click += new System.EventHandler(this.buttonSetupOutputPreviews_Click);
			// 
			// buttonSetupOutputControllers
			// 
			this.buttonSetupOutputControllers.Location = new System.Drawing.Point(12, 63);
			this.buttonSetupOutputControllers.Name = "buttonSetupOutputControllers";
			this.buttonSetupOutputControllers.Size = new System.Drawing.Size(180, 30);
			this.buttonSetupOutputControllers.TabIndex = 6;
			this.buttonSetupOutputControllers.Text = "Configure Output Controllers";
			this.buttonSetupOutputControllers.UseVisualStyleBackColor = true;
			this.buttonSetupOutputControllers.Click += new System.EventHandler(this.buttonSetupOutputControllers_Click);
			// 
			// buttonSetupChannels
			// 
			this.buttonSetupChannels.Location = new System.Drawing.Point(12, 23);
			this.buttonSetupChannels.Name = "buttonSetupChannels";
			this.buttonSetupChannels.Size = new System.Drawing.Size(180, 30);
			this.buttonSetupChannels.TabIndex = 5;
			this.buttonSetupChannels.Text = "Configure Channels && Groups";
			this.buttonSetupChannels.UseVisualStyleBackColor = true;
			this.buttonSetupChannels.Click += new System.EventHandler(this.buttonSetupChannels_Click);
			// 
			// statusStrip
			// 
			this.statusStrip.AutoSize = false;
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelExecutionLight,
            this.toolStripStatusLabel1,
            this.toolStripStatusLabelExecutionState,
            this.toolStripStatusLabel_memory});
			this.statusStrip.Location = new System.Drawing.Point(0, 380);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(461, 27);
			this.statusStrip.SizingGrip = false;
			this.statusStrip.TabIndex = 13;
			this.statusStrip.Text = "statusStrip";
			// 
			// toolStripStatusLabelExecutionLight
			// 
			this.toolStripStatusLabelExecutionLight.AutoSize = false;
			this.toolStripStatusLabelExecutionLight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.toolStripStatusLabelExecutionLight.Name = "toolStripStatusLabelExecutionLight";
			this.toolStripStatusLabelExecutionLight.Size = new System.Drawing.Size(22, 22);
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(13, 22);
			this.toolStripStatusLabel1.Text = "  ";
			// 
			// toolStripStatusLabelExecutionState
			// 
			this.toolStripStatusLabelExecutionState.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
			this.toolStripStatusLabelExecutionState.Name = "toolStripStatusLabelExecutionState";
			this.toolStripStatusLabelExecutionState.Size = new System.Drawing.Size(119, 22);
			this.toolStripStatusLabelExecutionState.Text = "Execution: Unknown";
			// 
			// toolStripStatusLabel_memory
			// 
			this.toolStripStatusLabel_memory.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripStatusLabel_memory.Name = "toolStripStatusLabel_memory";
			this.toolStripStatusLabel_memory.Size = new System.Drawing.Size(292, 22);
			this.toolStripStatusLabel_memory.Spring = true;
			this.toolStripStatusLabel_memory.Text = "Resource Usage";
			this.toolStripStatusLabel_memory.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(284, 314);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 14;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// VixenApplication
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(461, 407);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.groupBoxSystemConfig);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.groupBoxSequences);
			this.Controls.Add(this.menuStripMain);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MainMenuStrip = this.menuStripMain;
			this.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size(467, 434);
			this.Name = "VixenApplication";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Vixen Administration";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VixenApp_FormClosing);
			this.Load += new System.EventHandler(this.VixenApplication_Load);
			this.menuStripMain.ResumeLayout(false);
			this.menuStripMain.PerformLayout();
			this.groupBoxSequences.ResumeLayout(false);
			this.groupBoxSequences.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.groupBoxSystemConfig.ResumeLayout(false);
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ContextMenuStrip contextMenuStripNewSequence;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.MenuStrip menuStripMain;
		private System.Windows.Forms.ToolStripMenuItem vixenToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem executionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonOpenSequence;
		private System.Windows.Forms.Button buttonNewSequence;
		private System.Windows.Forms.GroupBox groupBoxSequences;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.GroupBox groupBoxSystemConfig;
		private System.Windows.Forms.Button buttonSetupOutputControllers;
		private System.Windows.Forms.Button buttonSetupChannels;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelExecutionState;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelExecutionLight;
		private System.Windows.Forms.ToolStripMenuItem logsToolStripMenuItem;
		private System.Windows.Forms.ListView listViewRecentSequences;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ToolStripMenuItem viewInstalledModulesToolStripMenuItem;
		private System.Windows.Forms.Label labelVersion;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ToolStripMenuItem channelGroupTestToolStripMenuItem;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_memory;
		private System.Windows.Forms.Button buttonSetupOutputPreviews;
		private System.Windows.Forms.Button button1;
	}
}

