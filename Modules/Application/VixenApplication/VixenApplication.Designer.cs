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
			this.contextMenuStripNewSequence = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.menuStripMain = new System.Windows.Forms.MenuStrip();
			this.vixenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.executionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.label2 = new System.Windows.Forms.Label();
			this.listBoxRecentSequences = new System.Windows.Forms.ListBox();
			this.buttonOpenSequence = new System.Windows.Forms.Button();
			this.buttonNewSequence = new System.Windows.Forms.Button();
			this.groupBoxSequences = new System.Windows.Forms.GroupBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupBoxSystemConfig = new System.Windows.Forms.GroupBox();
			this.buttonSetupOutputControllers = new System.Windows.Forms.Button();
			this.buttonSetupChannels = new System.Windows.Forms.Button();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabelExecutionState = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabelExecutionLight = new System.Windows.Forms.ToolStripStatusLabel();
			this.menuStripMain.SuspendLayout();
			this.groupBoxSequences.SuspendLayout();
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
			this.menuStripMain.Size = new System.Drawing.Size(451, 24);
			this.menuStripMain.TabIndex = 2;
			this.menuStripMain.Text = "menuStrip1";
			// 
			// vixenToolStripMenuItem
			// 
			this.vixenToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.executionToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
			this.vixenToolStripMenuItem.Name = "vixenToolStripMenuItem";
			this.vixenToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
			this.vixenToolStripMenuItem.Text = "Vixen";
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.aboutToolStripMenuItem.Text = "About...";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// executionToolStripMenuItem
			// 
			this.executionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem});
			this.executionToolStripMenuItem.Name = "executionToolStripMenuItem";
			this.executionToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
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
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(161, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.exitToolStripMenuItem.Text = "Exit";
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
			// listBoxRecentSequences
			// 
			this.listBoxRecentSequences.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listBoxRecentSequences.FormattingEnabled = true;
			this.listBoxRecentSequences.Items.AddRange(new object[] {
            "dummy sequence 1",
            "dummy sequence 2",
            "dummy sequence 3",
            ""});
			this.listBoxRecentSequences.Location = new System.Drawing.Point(18, 116);
			this.listBoxRecentSequences.Name = "listBoxRecentSequences";
			this.listBoxRecentSequences.Size = new System.Drawing.Size(180, 93);
			this.listBoxRecentSequences.TabIndex = 7;
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
			this.groupBoxSequences.Controls.Add(this.buttonNewSequence);
			this.groupBoxSequences.Controls.Add(this.buttonOpenSequence);
			this.groupBoxSequences.Controls.Add(this.label2);
			this.groupBoxSequences.Controls.Add(this.listBoxRecentSequences);
			this.groupBoxSequences.Location = new System.Drawing.Point(12, 133);
			this.groupBoxSequences.Name = "groupBoxSequences";
			this.groupBoxSequences.Size = new System.Drawing.Size(218, 227);
			this.groupBoxSequences.TabIndex = 10;
			this.groupBoxSequences.TabStop = false;
			this.groupBoxSequences.Text = "Sequences";
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.panel1.Location = new System.Drawing.Point(0, 27);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(451, 100);
			this.panel1.TabIndex = 11;
			// 
			// groupBoxSystemConfig
			// 
			this.groupBoxSystemConfig.Controls.Add(this.buttonSetupOutputControllers);
			this.groupBoxSystemConfig.Controls.Add(this.buttonSetupChannels);
			this.groupBoxSystemConfig.Location = new System.Drawing.Point(236, 133);
			this.groupBoxSystemConfig.Name = "groupBoxSystemConfig";
			this.groupBoxSystemConfig.Size = new System.Drawing.Size(204, 105);
			this.groupBoxSystemConfig.TabIndex = 12;
			this.groupBoxSystemConfig.TabStop = false;
			this.groupBoxSystemConfig.Text = "System Configuration";
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
            this.toolStripStatusLabelExecutionState,
            this.toolStripStatusLabelExecutionLight});
			this.statusStrip.Location = new System.Drawing.Point(0, 370);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(451, 27);
			this.statusStrip.TabIndex = 13;
			this.statusStrip.Text = "statusStrip";
			// 
			// toolStripStatusLabelExecutionState
			// 
			this.toolStripStatusLabelExecutionState.AutoSize = false;
			this.toolStripStatusLabelExecutionState.Name = "toolStripStatusLabelExecutionState";
			this.toolStripStatusLabelExecutionState.Size = new System.Drawing.Size(110, 22);
			this.toolStripStatusLabelExecutionState.Text = "Execution: Unknown";
			// 
			// toolStripStatusLabelExecutionLight
			// 
			this.toolStripStatusLabelExecutionLight.AutoSize = false;
			this.toolStripStatusLabelExecutionLight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.toolStripStatusLabelExecutionLight.Name = "toolStripStatusLabelExecutionLight";
			this.toolStripStatusLabelExecutionLight.Size = new System.Drawing.Size(22, 22);
			// 
			// VixenApplication
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(451, 397);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.groupBoxSystemConfig);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.groupBoxSequences);
			this.Controls.Add(this.menuStripMain);
			this.DoubleBuffered = true;
			this.MainMenuStrip = this.menuStripMain;
			this.MinimumSize = new System.Drawing.Size(467, 435);
			this.Name = "VixenApplication";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Vixen";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VixenApp_FormClosing);
			this.Load += new System.EventHandler(this.VixenApplication_Load);
			this.menuStripMain.ResumeLayout(false);
			this.menuStripMain.PerformLayout();
			this.groupBoxSequences.ResumeLayout(false);
			this.groupBoxSequences.PerformLayout();
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
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox listBoxRecentSequences;
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
	}
}

