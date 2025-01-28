﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace VixenApplication
{
	partial class VixenApplication
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private IContainer components = null;

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
			components = new Container();
			ListViewItem listViewItem1 = new ListViewItem("asdfadsa");
			ListViewItem listViewItem2 = new ListViewItem("rewqrewq");
			ListViewItem listViewItem3 = new ListViewItem("vbcbxvxc");
			ListViewItem listViewItem4 = new ListViewItem("gfdsgfsd");
			ListViewItem listViewItem5 = new ListViewItem("ytreyre");
			ListViewItem listViewItem6 = new ListViewItem("xvcbxvcx");
			contextMenuStripNewSequence = new ContextMenuStrip(components);
			openFileDialog = new OpenFileDialog();
			menuStripMain = new MenuStrip();
			vixenToolStripMenuItem = new ToolStripMenuItem();
			logsToolStripMenuItem = new ToolStripMenuItem();
			viewInstalledModulesToolStripMenuItem = new ToolStripMenuItem();
			profilesToolStripMenuItem = new ToolStripMenuItem();
			systemConfigurationToolStripMenuItem = new ToolStripMenuItem();
			setupDisplayToolStripMenuItem = new ToolStripMenuItem();
			setupPreviewsToolStripMenuItem = new ToolStripMenuItem();
			executionToolStripMenuItem = new ToolStripMenuItem();
			startToolStripMenuItem = new ToolStripMenuItem();
			stopToolStripMenuItem = new ToolStripMenuItem();
			toolStripSeparator1 = new ToolStripSeparator();
			exitToolStripMenuItem = new ToolStripMenuItem();
			contextMenuStripRecent = new ContextMenuStrip(components);
			toolStripItemClearSequences = new ToolStripMenuItem();
			toolStripStatusLabelExecutionLight = new ToolStripStatusLabel();
			toolStripStatusLabel1 = new ToolStripStatusLabel();
			toolStripStatusLabelExecutionState = new ToolStripStatusLabel();
			toolStripStatusLabel_memory = new ToolStripStatusLabel();
			statusStrip = new StatusStrip();
			toolStripStatusUpdates = new ToolStripStatusLabel();
			progressBar = new Common.Controls.TextProgressBar();
			groupBoxSequences = new GroupBox();
			tableLayoutPanel2 = new TableLayoutPanel();
			listViewRecentSequences = new ListView();
			columnHeader1 = new ColumnHeader();
			label2 = new Label();
			buttonOpenSequence = new Button();
			buttonNewSequence = new Button();
			groupBoxSystemConfig = new GroupBox();
			tableLayoutPanel4 = new TableLayoutPanel();
			buttonSetupOutputPreviews = new Button();
			buttonSetupDisplay = new Button();
			buttonPanel = new TableLayoutPanel();
			titlePanel = new TableLayoutPanel();
			labelRelease = new Label();
			labelBuild = new Label();
			logoImage = new PictureBox();
			labelVixen = new Label();
			mainLayoutPanel = new TableLayoutPanel();
			menuStripMain.SuspendLayout();
			contextMenuStripRecent.SuspendLayout();
			statusStrip.SuspendLayout();
			groupBoxSequences.SuspendLayout();
			tableLayoutPanel2.SuspendLayout();
			groupBoxSystemConfig.SuspendLayout();
			tableLayoutPanel4.SuspendLayout();
			buttonPanel.SuspendLayout();
			titlePanel.SuspendLayout();
			((ISupportInitialize)logoImage).BeginInit();
			mainLayoutPanel.SuspendLayout();
			SuspendLayout();
			// 
			// contextMenuStripNewSequence
			// 
			contextMenuStripNewSequence.ImageScalingSize = new Size(24, 24);
			contextMenuStripNewSequence.Name = "contextMenuStripNewSequence";
			contextMenuStripNewSequence.ShowImageMargin = false;
			contextMenuStripNewSequence.Size = new Size(36, 4);
			// 
			// openFileDialog
			// 
			openFileDialog.Multiselect = true;
			// 
			// menuStripMain
			// 
			menuStripMain.BackColor = Color.Transparent;
			menuStripMain.ImageScalingSize = new Size(24, 24);
			menuStripMain.Items.AddRange(new ToolStripItem[] { vixenToolStripMenuItem });
			menuStripMain.Location = new Point(0, 0);
			menuStripMain.Name = "menuStripMain";
			menuStripMain.Padding = new Padding(9, 3, 0, 3);
			menuStripMain.RenderMode = ToolStripRenderMode.Professional;
			menuStripMain.Size = new Size(459, 25);
			menuStripMain.TabIndex = 2;
			menuStripMain.Text = "menuStrip1";
			// 
			// vixenToolStripMenuItem
			// 
			vixenToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { logsToolStripMenuItem, viewInstalledModulesToolStripMenuItem, profilesToolStripMenuItem, systemConfigurationToolStripMenuItem, executionToolStripMenuItem, toolStripSeparator1, exitToolStripMenuItem });
			vixenToolStripMenuItem.ForeColor = Color.FromArgb(221, 221, 221);
			vixenToolStripMenuItem.Name = "vixenToolStripMenuItem";
			vixenToolStripMenuItem.Size = new Size(57, 19);
			vixenToolStripMenuItem.Text = "System";
			// 
			// logsToolStripMenuItem
			// 
			logsToolStripMenuItem.ForeColor = Color.FromArgb(221, 221, 221);
			logsToolStripMenuItem.Name = "logsToolStripMenuItem";
			logsToolStripMenuItem.Size = new Size(195, 22);
			logsToolStripMenuItem.Text = "Logs";
			// 
			// viewInstalledModulesToolStripMenuItem
			// 
			viewInstalledModulesToolStripMenuItem.ForeColor = Color.FromArgb(221, 221, 221);
			viewInstalledModulesToolStripMenuItem.Name = "viewInstalledModulesToolStripMenuItem";
			viewInstalledModulesToolStripMenuItem.Size = new Size(195, 22);
			viewInstalledModulesToolStripMenuItem.Text = "View Installed Modules";
			viewInstalledModulesToolStripMenuItem.Click += viewInstalledModulesToolStripMenuItem_Click;
			// 
			// profilesToolStripMenuItem
			// 
			profilesToolStripMenuItem.ForeColor = Color.FromArgb(221, 221, 221);
			profilesToolStripMenuItem.Name = "profilesToolStripMenuItem";
			profilesToolStripMenuItem.Size = new Size(195, 22);
			profilesToolStripMenuItem.Text = "Profiles...";
			profilesToolStripMenuItem.Click += profilesToolStripMenuItem_Click;
			// 
			// systemConfigurationToolStripMenuItem
			// 
			systemConfigurationToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { setupDisplayToolStripMenuItem, setupPreviewsToolStripMenuItem });
			systemConfigurationToolStripMenuItem.ForeColor = Color.FromArgb(221, 221, 221);
			systemConfigurationToolStripMenuItem.Name = "systemConfigurationToolStripMenuItem";
			systemConfigurationToolStripMenuItem.Size = new Size(195, 22);
			systemConfigurationToolStripMenuItem.Text = "System Configuration";
			// 
			// setupDisplayToolStripMenuItem
			// 
			setupDisplayToolStripMenuItem.ForeColor = Color.FromArgb(221, 221, 221);
			setupDisplayToolStripMenuItem.Name = "setupDisplayToolStripMenuItem";
			setupDisplayToolStripMenuItem.Size = new Size(153, 22);
			setupDisplayToolStripMenuItem.Text = "Setup Display";
			setupDisplayToolStripMenuItem.Click += setupDisplayToolStripMenuItem_Click;
			// 
			// setupPreviewsToolStripMenuItem
			// 
			setupPreviewsToolStripMenuItem.ForeColor = Color.FromArgb(221, 221, 221);
			setupPreviewsToolStripMenuItem.Name = "setupPreviewsToolStripMenuItem";
			setupPreviewsToolStripMenuItem.Size = new Size(153, 22);
			setupPreviewsToolStripMenuItem.Text = "Setup Previews";
			setupPreviewsToolStripMenuItem.Click += setupPreviewsToolStripMenuItem_Click;
			// 
			// executionToolStripMenuItem
			// 
			executionToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { startToolStripMenuItem, stopToolStripMenuItem });
			executionToolStripMenuItem.ForeColor = Color.FromArgb(221, 221, 221);
			executionToolStripMenuItem.Name = "executionToolStripMenuItem";
			executionToolStripMenuItem.Size = new Size(195, 22);
			executionToolStripMenuItem.Text = "Execution Engine";
			// 
			// startToolStripMenuItem
			// 
			startToolStripMenuItem.Name = "startToolStripMenuItem";
			startToolStripMenuItem.Size = new Size(98, 22);
			startToolStripMenuItem.Text = "Start";
			startToolStripMenuItem.Click += startToolStripMenuItem_Click;
			// 
			// stopToolStripMenuItem
			// 
			stopToolStripMenuItem.Name = "stopToolStripMenuItem";
			stopToolStripMenuItem.Size = new Size(98, 22);
			stopToolStripMenuItem.Text = "Stop";
			stopToolStripMenuItem.Click += stopToolStripMenuItem_Click;
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.ForeColor = SystemColors.ControlText;
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new Size(192, 6);
			// 
			// exitToolStripMenuItem
			// 
			exitToolStripMenuItem.ForeColor = Color.FromArgb(221, 221, 221);
			exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			exitToolStripMenuItem.Size = new Size(195, 22);
			exitToolStripMenuItem.Text = "Shutdown and E&xit";
			exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
			// 
			// contextMenuStripRecent
			// 
			contextMenuStripRecent.ImageScalingSize = new Size(20, 20);
			contextMenuStripRecent.Items.AddRange(new ToolStripItem[] { toolStripItemClearSequences });
			contextMenuStripRecent.Name = "contextMenuStripRecent";
			contextMenuStripRecent.Size = new Size(200, 26);
			// 
			// toolStripItemClearSequences
			// 
			toolStripItemClearSequences.DisplayStyle = ToolStripItemDisplayStyle.Text;
			toolStripItemClearSequences.Name = "toolStripItemClearSequences";
			toolStripItemClearSequences.Size = new Size(199, 22);
			toolStripItemClearSequences.Text = "Clear Recent Sequences";
			toolStripItemClearSequences.ToolTipText = "Clears the Recent Sequence list";
			// 
			// toolStripStatusLabelExecutionLight
			// 
			toolStripStatusLabelExecutionLight.AutoSize = false;
			toolStripStatusLabelExecutionLight.BackColor = Color.FromArgb(255, 128, 255);
			toolStripStatusLabelExecutionLight.Margin = new Padding(0, 1, 0, 1);
			toolStripStatusLabelExecutionLight.Name = "toolStripStatusLabelExecutionLight";
			toolStripStatusLabelExecutionLight.Size = new Size(22, 22);
			// 
			// toolStripStatusLabel1
			// 
			toolStripStatusLabel1.Margin = new Padding(0, 1, 0, 1);
			toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			toolStripStatusLabel1.Size = new Size(13, 22);
			toolStripStatusLabel1.Text = "  ";
			// 
			// toolStripStatusLabelExecutionState
			// 
			toolStripStatusLabelExecutionState.BorderSides = ToolStripStatusLabelBorderSides.Right;
			toolStripStatusLabelExecutionState.ForeColor = Color.FromArgb(221, 221, 221);
			toolStripStatusLabelExecutionState.Margin = new Padding(0, 1, 0, 1);
			toolStripStatusLabelExecutionState.Name = "toolStripStatusLabelExecutionState";
			toolStripStatusLabelExecutionState.Size = new Size(120, 22);
			toolStripStatusLabelExecutionState.Text = "Execution: Unknown";
			// 
			// toolStripStatusLabel_memory
			// 
			toolStripStatusLabel_memory.DisplayStyle = ToolStripItemDisplayStyle.Text;
			toolStripStatusLabel_memory.ForeColor = Color.FromArgb(221, 221, 221);
			toolStripStatusLabel_memory.Margin = new Padding(0, 1, 0, 1);
			toolStripStatusLabel_memory.Name = "toolStripStatusLabel_memory";
			toolStripStatusLabel_memory.Size = new Size(231, 22);
			toolStripStatusLabel_memory.Spring = true;
			toolStripStatusLabel_memory.Text = "Resource Usage";
			toolStripStatusLabel_memory.TextAlign = ContentAlignment.MiddleRight;
			// 
			// statusStrip
			// 
			statusStrip.AutoSize = false;
			statusStrip.GripMargin = new Padding(0);
			statusStrip.ImageScalingSize = new Size(24, 24);
			statusStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelExecutionLight, toolStripStatusLabel1, toolStripStatusLabelExecutionState, toolStripStatusUpdates, toolStripStatusLabel_memory });
			statusStrip.Location = new Point(0, 515);
			statusStrip.Name = "statusStrip";
			statusStrip.Padding = new Padding(2, 0, 21, 0);
			statusStrip.Size = new Size(459, 24);
			statusStrip.SizingGrip = false;
			statusStrip.TabIndex = 13;
			statusStrip.Text = "statusStrip";
			// 
			// toolStripStatusUpdates
			// 
			toolStripStatusUpdates.Name = "toolStripStatusUpdates";
			toolStripStatusUpdates.Size = new Size(50, 19);
			toolStripStatusUpdates.Text = "Updates";
			// 
			// progressBar
			// 
			buttonPanel.SetColumnSpan(progressBar, 2);
			progressBar.CustomText = "";
			progressBar.Dock = DockStyle.Fill;
			progressBar.Location = new Point(3, 300);
			progressBar.Name = "progressBar";
			progressBar.ProgressColor = Color.Lime;
			progressBar.Size = new Size(444, 34);
			progressBar.TabIndex = 17;
			progressBar.TextColor = Color.Black;
			progressBar.TextFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
			progressBar.VisualMode = Common.Controls.ProgressBarDisplayMode.CustomText;
			// 
			// groupBoxSequences
			// 
			groupBoxSequences.AutoSize = true;
			groupBoxSequences.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			groupBoxSequences.Controls.Add(tableLayoutPanel2);
			groupBoxSequences.Dock = DockStyle.Fill;
			groupBoxSequences.ForeColor = Color.FromArgb(221, 221, 221);
			groupBoxSequences.Location = new Point(4, 5);
			groupBoxSequences.Margin = new Padding(4, 5, 4, 5);
			groupBoxSequences.Name = "groupBoxSequences";
			groupBoxSequences.Padding = new Padding(4, 5, 4, 5);
			groupBoxSequences.Size = new Size(217, 287);
			groupBoxSequences.TabIndex = 0;
			groupBoxSequences.TabStop = false;
			groupBoxSequences.Text = "Sequences";
			groupBoxSequences.Paint += groupBoxes_Paint;
			// 
			// tableLayoutPanel2
			// 
			tableLayoutPanel2.ColumnCount = 1;
			tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			tableLayoutPanel2.Controls.Add(listViewRecentSequences, 0, 3);
			tableLayoutPanel2.Controls.Add(label2, 0, 2);
			tableLayoutPanel2.Controls.Add(buttonOpenSequence, 0, 1);
			tableLayoutPanel2.Controls.Add(buttonNewSequence, 0, 0);
			tableLayoutPanel2.Dock = DockStyle.Fill;
			tableLayoutPanel2.Location = new Point(4, 21);
			tableLayoutPanel2.Name = "tableLayoutPanel2";
			tableLayoutPanel2.RowCount = 4;
			tableLayoutPanel2.RowStyles.Add(new RowStyle());
			tableLayoutPanel2.RowStyles.Add(new RowStyle());
			tableLayoutPanel2.RowStyles.Add(new RowStyle());
			tableLayoutPanel2.RowStyles.Add(new RowStyle());
			tableLayoutPanel2.Size = new Size(209, 261);
			tableLayoutPanel2.TabIndex = 19;
			// 
			// listViewRecentSequences
			// 
			listViewRecentSequences.BackColor = Color.FromArgb(90, 90, 90);
			listViewRecentSequences.Columns.AddRange(new ColumnHeader[] { columnHeader1 });
			listViewRecentSequences.ContextMenuStrip = contextMenuStripRecent;
			listViewRecentSequences.Dock = DockStyle.Fill;
			listViewRecentSequences.ForeColor = Color.FromArgb(221, 221, 221);
			listViewRecentSequences.FullRowSelect = true;
			listViewRecentSequences.HeaderStyle = ColumnHeaderStyle.None;
			listViewRecentSequences.Items.AddRange(new ListViewItem[] { listViewItem1, listViewItem2, listViewItem3, listViewItem4, listViewItem5, listViewItem6 });
			listViewRecentSequences.Location = new Point(4, 97);
			listViewRecentSequences.Margin = new Padding(4, 5, 4, 5);
			listViewRecentSequences.MultiSelect = false;
			listViewRecentSequences.Name = "listViewRecentSequences";
			listViewRecentSequences.Size = new Size(201, 159);
			listViewRecentSequences.TabIndex = 0;
			listViewRecentSequences.UseCompatibleStateImageBehavior = false;
			listViewRecentSequences.View = View.Details;
			listViewRecentSequences.DoubleClick += listViewRecentSequences_DoubleClick;
			// 
			// columnHeader1
			// 
			columnHeader1.Width = 150;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.ForeColor = Color.FromArgb(221, 221, 221);
			label2.Location = new Point(4, 77);
			label2.Margin = new Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new Size(105, 15);
			label2.TabIndex = 8;
			label2.Text = "Recent Sequences:";
			// 
			// buttonOpenSequence
			// 
			buttonOpenSequence.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			buttonOpenSequence.AutoSize = true;
			buttonOpenSequence.BackgroundImageLayout = ImageLayout.Stretch;
			buttonOpenSequence.FlatAppearance.BorderColor = Color.FromArgb(40, 40, 40);
			buttonOpenSequence.FlatStyle = FlatStyle.Flat;
			buttonOpenSequence.ForeColor = Color.FromArgb(221, 221, 221);
			buttonOpenSequence.Location = new Point(4, 42);
			buttonOpenSequence.Margin = new Padding(4, 5, 4, 5);
			buttonOpenSequence.Name = "buttonOpenSequence";
			buttonOpenSequence.Size = new Size(201, 30);
			buttonOpenSequence.TabIndex = 2;
			buttonOpenSequence.Text = "Open Sequence...";
			buttonOpenSequence.UseVisualStyleBackColor = true;
			buttonOpenSequence.Click += buttonOpenSequence_Click;
			buttonOpenSequence.MouseLeave += buttonBackground_MouseLeave;
			buttonOpenSequence.MouseHover += buttonBackground_MouseHover;
			// 
			// buttonNewSequence
			// 
			buttonNewSequence.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			buttonNewSequence.AutoSize = true;
			buttonNewSequence.BackgroundImageLayout = ImageLayout.Stretch;
			buttonNewSequence.FlatAppearance.BorderColor = Color.FromArgb(40, 40, 40);
			buttonNewSequence.FlatStyle = FlatStyle.Flat;
			buttonNewSequence.ForeColor = Color.FromArgb(221, 221, 221);
			buttonNewSequence.Location = new Point(4, 5);
			buttonNewSequence.Margin = new Padding(4, 5, 4, 5);
			buttonNewSequence.Name = "buttonNewSequence";
			buttonNewSequence.Size = new Size(201, 27);
			buttonNewSequence.TabIndex = 1;
			buttonNewSequence.Text = "New Sequence...";
			buttonNewSequence.UseVisualStyleBackColor = true;
			buttonNewSequence.Click += buttonNewSequence_Click;
			buttonNewSequence.MouseLeave += buttonBackground_MouseLeave;
			buttonNewSequence.MouseHover += buttonBackground_MouseHover;
			// 
			// groupBoxSystemConfig
			// 
			groupBoxSystemConfig.AutoSize = true;
			groupBoxSystemConfig.Controls.Add(tableLayoutPanel4);
			groupBoxSystemConfig.Dock = DockStyle.Top;
			groupBoxSystemConfig.ForeColor = Color.FromArgb(221, 221, 221);
			groupBoxSystemConfig.Location = new Point(229, 5);
			groupBoxSystemConfig.Margin = new Padding(4, 5, 4, 5);
			groupBoxSystemConfig.Name = "groupBoxSystemConfig";
			groupBoxSystemConfig.Padding = new Padding(4, 5, 4, 5);
			groupBoxSystemConfig.Size = new Size(217, 106);
			groupBoxSystemConfig.TabIndex = 1;
			groupBoxSystemConfig.TabStop = false;
			groupBoxSystemConfig.Text = "System Configuration";
			groupBoxSystemConfig.Paint += groupBoxes_Paint;
			// 
			// tableLayoutPanel4
			// 
			tableLayoutPanel4.AutoSize = true;
			tableLayoutPanel4.ColumnCount = 1;
			tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			tableLayoutPanel4.Controls.Add(buttonSetupOutputPreviews, 0, 1);
			tableLayoutPanel4.Controls.Add(buttonSetupDisplay, 0, 0);
			tableLayoutPanel4.Dock = DockStyle.Fill;
			tableLayoutPanel4.Location = new Point(4, 21);
			tableLayoutPanel4.Name = "tableLayoutPanel4";
			tableLayoutPanel4.RowCount = 2;
			tableLayoutPanel4.RowStyles.Add(new RowStyle());
			tableLayoutPanel4.RowStyles.Add(new RowStyle());
			tableLayoutPanel4.Size = new Size(209, 80);
			tableLayoutPanel4.TabIndex = 19;
			// 
			// buttonSetupOutputPreviews
			// 
			buttonSetupOutputPreviews.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			buttonSetupOutputPreviews.AutoSize = true;
			buttonSetupOutputPreviews.BackgroundImageLayout = ImageLayout.Stretch;
			buttonSetupOutputPreviews.FlatAppearance.BorderColor = Color.FromArgb(40, 40, 40);
			buttonSetupOutputPreviews.FlatStyle = FlatStyle.Flat;
			buttonSetupOutputPreviews.ForeColor = Color.FromArgb(221, 221, 221);
			buttonSetupOutputPreviews.Location = new Point(4, 45);
			buttonSetupOutputPreviews.Margin = new Padding(4, 5, 4, 5);
			buttonSetupOutputPreviews.Name = "buttonSetupOutputPreviews";
			buttonSetupOutputPreviews.Size = new Size(201, 30);
			buttonSetupOutputPreviews.TabIndex = 4;
			buttonSetupOutputPreviews.Text = "Setup Previews";
			buttonSetupOutputPreviews.UseVisualStyleBackColor = true;
			buttonSetupOutputPreviews.Click += buttonSetupOutputPreviews_Click;
			buttonSetupOutputPreviews.MouseLeave += buttonBackground_MouseLeave;
			buttonSetupOutputPreviews.MouseHover += buttonBackground_MouseHover;
			// 
			// buttonSetupDisplay
			// 
			buttonSetupDisplay.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			buttonSetupDisplay.AutoSize = true;
			buttonSetupDisplay.BackgroundImageLayout = ImageLayout.Stretch;
			buttonSetupDisplay.FlatAppearance.BorderColor = Color.FromArgb(40, 40, 40);
			buttonSetupDisplay.FlatStyle = FlatStyle.Flat;
			buttonSetupDisplay.ForeColor = Color.FromArgb(221, 221, 221);
			buttonSetupDisplay.Location = new Point(4, 5);
			buttonSetupDisplay.Margin = new Padding(4, 5, 4, 5);
			buttonSetupDisplay.Name = "buttonSetupDisplay";
			buttonSetupDisplay.Size = new Size(201, 30);
			buttonSetupDisplay.TabIndex = 3;
			buttonSetupDisplay.Text = "Setup Display";
			buttonSetupDisplay.UseVisualStyleBackColor = true;
			buttonSetupDisplay.Click += buttonSetupDisplay_Click;
			buttonSetupDisplay.MouseLeave += buttonBackground_MouseLeave;
			buttonSetupDisplay.MouseHover += buttonBackground_MouseHover;
			// 
			// buttonPanel
			// 
			buttonPanel.ColumnCount = 2;
			buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			buttonPanel.Controls.Add(groupBoxSystemConfig, 1, 0);
			buttonPanel.Controls.Add(groupBoxSequences, 0, 0);
			buttonPanel.Controls.Add(progressBar, 0, 1);
			buttonPanel.Dock = DockStyle.Fill;
			buttonPanel.Location = new Point(3, 150);
			buttonPanel.Name = "buttonPanel";
			buttonPanel.RowCount = 2;
			buttonPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 88.35714F));
			buttonPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 11.6428566F));
			buttonPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
			buttonPanel.Size = new Size(450, 337);
			buttonPanel.TabIndex = 18;
			// 
			// titlePanel
			// 
			titlePanel.CausesValidation = false;
			titlePanel.ColumnCount = 4;
			titlePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
			titlePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
			titlePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
			titlePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
			titlePanel.Controls.Add(labelRelease, 2, 1);
			titlePanel.Controls.Add(labelBuild, 2, 2);
			titlePanel.Controls.Add(logoImage, 0, 0);
			titlePanel.Controls.Add(labelVixen, 1, 0);
			titlePanel.Dock = DockStyle.Fill;
			titlePanel.Location = new Point(0, 0);
			titlePanel.Margin = new Padding(0);
			titlePanel.Name = "titlePanel";
			titlePanel.RowCount = 3;
			titlePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
			titlePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
			titlePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
			titlePanel.Size = new Size(456, 147);
			titlePanel.TabIndex = 19;
			// 
			// labelRelease
			// 
			labelRelease.AutoSize = true;
			labelRelease.Dock = DockStyle.Fill;
			labelRelease.ForeColor = SystemColors.HighlightText;
			labelRelease.Location = new Point(299, 102);
			labelRelease.Name = "labelRelease";
			labelRelease.Size = new Size(108, 22);
			labelRelease.TabIndex = 2;
			labelRelease.Text = "Release ";
			labelRelease.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// labelBuild
			// 
			labelBuild.AutoSize = true;
			labelBuild.Dock = DockStyle.Fill;
			labelBuild.ForeColor = SystemColors.HighlightText;
			labelBuild.Location = new Point(299, 124);
			labelBuild.Name = "labelBuild";
			labelBuild.Size = new Size(108, 23);
			labelBuild.TabIndex = 3;
			labelBuild.Text = "Build";
			labelBuild.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// logoImage
			// 
			logoImage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			logoImage.BackgroundImageLayout = ImageLayout.None;
			logoImage.Image = Properties.Resources.V3Logo;
			logoImage.Location = new Point(3, 3);
			logoImage.Name = "logoImage";
			titlePanel.SetRowSpan(logoImage, 3);
			logoImage.Size = new Size(176, 141);
			logoImage.SizeMode = PictureBoxSizeMode.Zoom;
			logoImage.TabIndex = 4;
			logoImage.TabStop = false;
			// 
			// labelVixen
			// 
			labelVixen.AutoSize = true;
			titlePanel.SetColumnSpan(labelVixen, 3);
			labelVixen.Dock = DockStyle.Fill;
			labelVixen.ForeColor = SystemColors.HighlightText;
			labelVixen.Location = new Point(182, 0);
			labelVixen.Margin = new Padding(0);
			labelVixen.Name = "labelVixen";
			labelVixen.Size = new Size(274, 102);
			labelVixen.TabIndex = 5;
			labelVixen.Text = "Vixen";
			labelVixen.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// mainLayoutPanel
			// 
			mainLayoutPanel.ColumnCount = 1;
			mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			mainLayoutPanel.Controls.Add(titlePanel, 0, 0);
			mainLayoutPanel.Controls.Add(buttonPanel, 0, 1);
			mainLayoutPanel.Dock = DockStyle.Fill;
			mainLayoutPanel.Location = new Point(0, 25);
			mainLayoutPanel.Name = "mainLayoutPanel";
			mainLayoutPanel.RowCount = 2;
			mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
			mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
			mainLayoutPanel.Size = new Size(456, 490);
			mainLayoutPanel.TabIndex = 20;
			// 
			// VixenApplication
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = true;
			BackColor = Color.FromArgb(68, 68, 68);
			ClientSize = new Size(456, 539);
			Controls.Add(mainLayoutPanel);
			Controls.Add(statusStrip);
			Controls.Add(menuStripMain);
			DoubleBuffered = true;
			MainMenuStrip = menuStripMain;
			Margin = new Padding(4, 5, 4, 5);
			MaximizeBox = false;
			MinimumSize = new Size(472, 578);
			Name = "VixenApplication";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Vixen Administration";
			FormClosing += VixenApp_FormClosing;
			Load += VixenApplication_Load;
			Shown += VixenApplication_Shown;
			SizeChanged += VixenApplication_SizeChanged;
			menuStripMain.ResumeLayout(false);
			menuStripMain.PerformLayout();
			contextMenuStripRecent.ResumeLayout(false);
			statusStrip.ResumeLayout(false);
			statusStrip.PerformLayout();
			groupBoxSequences.ResumeLayout(false);
			tableLayoutPanel2.ResumeLayout(false);
			tableLayoutPanel2.PerformLayout();
			groupBoxSystemConfig.ResumeLayout(false);
			groupBoxSystemConfig.PerformLayout();
			tableLayoutPanel4.ResumeLayout(false);
			tableLayoutPanel4.PerformLayout();
			buttonPanel.ResumeLayout(false);
			buttonPanel.PerformLayout();
			titlePanel.ResumeLayout(false);
			titlePanel.PerformLayout();
			((ISupportInitialize)logoImage).EndInit();
			mainLayoutPanel.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private ContextMenuStrip contextMenuStripNewSequence;
		private OpenFileDialog openFileDialog;
		private MenuStrip menuStripMain;
		private ToolStripMenuItem vixenToolStripMenuItem;
		private ToolStripMenuItem executionToolStripMenuItem;
		private ToolStripMenuItem startToolStripMenuItem;
		private ToolStripMenuItem stopToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripMenuItem exitToolStripMenuItem;
		private ToolStripMenuItem logsToolStripMenuItem;
		private ToolStripMenuItem viewInstalledModulesToolStripMenuItem;
		private ToolStripMenuItem profilesToolStripMenuItem;
		private ToolStripMenuItem systemConfigurationToolStripMenuItem;
		private ToolStripMenuItem setupDisplayToolStripMenuItem;
		private ToolStripMenuItem setupPreviewsToolStripMenuItem;
		private ToolStripStatusLabel toolStripStatusLabelExecutionLight;
		private ToolStripStatusLabel toolStripStatusLabel1;
		private ToolStripStatusLabel toolStripStatusLabelExecutionState;
		private ToolStripStatusLabel toolStripStatusLabel_memory;
		private StatusStrip statusStrip;
		private ContextMenuStrip contextMenuStripRecent;
		private ToolStripMenuItem toolStripItemClearSequences;
		private ToolStripStatusLabel toolStripStatusUpdates;
		private Common.Controls.TextProgressBar progressBar;
		private TableLayoutPanel buttonPanel;
		private GroupBox groupBoxSystemConfig;
		private GroupBox groupBoxSequences;
		private Button buttonNewSequence;
		private Button buttonOpenSequence;
		private Label label2;
		private ListView listViewRecentSequences;
		private ColumnHeader columnHeader1;
		private TableLayoutPanel tableLayoutPanel2;
		private TableLayoutPanel tableLayoutPanel4;
		private Button buttonSetupOutputPreviews;
		private Button buttonSetupDisplay;
		private TableLayoutPanel titlePanel;
		private Label labelRelease;
		private Label labelBuild;
		private PictureBox logoImage;
		private Label labelVixen;
		private TableLayoutPanel mainLayoutPanel;
	}
}

