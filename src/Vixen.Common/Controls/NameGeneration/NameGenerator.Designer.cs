namespace Common.Controls
{
	partial class NameGenerator
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
			this.mainLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.contentLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.configurationLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.itemCountLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.numericUpDownItemCount = new System.Windows.Forms.NumericUpDown();
			this.nameFormatLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxNameFormat = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBoxSelectedNamingRule = new System.Windows.Forms.GroupBox();
			this.rulesLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.ruleTypeLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.comboBoxRuleTypes = new System.Windows.Forms.ComboBox();
			this.ruleCommandsLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.buttonAddNewRule = new System.Windows.Forms.Button();
			this.buttonDeleteRule = new System.Windows.Forms.Button();
			this.listViewGenerators = new System.Windows.Forms.ListView();
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.moveRuleButtonsLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.buttonMoveRuleUp = new System.Windows.Forms.Button();
			this.buttonMoveRuleDown = new System.Windows.Forms.Button();
			this.panelRuleConfig = new System.Windows.Forms.Panel();
			this.previewLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.previewHeaderLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.labelColumnHeader1 = new System.Windows.Forms.Label();
			this.labelColumnHeader2 = new System.Windows.Forms.Label();
			this.listViewNames = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.footerLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.templateLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.label2 = new System.Windows.Forms.Label();
			this.comboBoxTemplates = new System.Windows.Forms.ComboBox();
			this.dialogButtonsLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			this.mainLayoutPanel.SuspendLayout();
			this.contentLayoutPanel.SuspendLayout();
			this.configurationLayoutPanel.SuspendLayout();
			this.itemCountLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownItemCount)).BeginInit();
			this.nameFormatLayoutPanel.SuspendLayout();
			this.groupBoxSelectedNamingRule.SuspendLayout();
			this.rulesLayoutPanel.SuspendLayout();
			this.ruleTypeLayoutPanel.SuspendLayout();
			this.ruleCommandsLayoutPanel.SuspendLayout();
			this.moveRuleButtonsLayoutPanel.SuspendLayout();
			this.previewLayoutPanel.SuspendLayout();
			this.previewHeaderLayoutPanel.SuspendLayout();
			this.footerLayoutPanel.SuspendLayout();
			this.templateLayoutPanel.SuspendLayout();
			this.dialogButtonsLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			//
			// mainLayoutPanel
			//
			this.mainLayoutPanel.ColumnCount = 1;
			this.mainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.mainLayoutPanel.Controls.Add(this.contentLayoutPanel, 0, 0);
			this.mainLayoutPanel.Controls.Add(this.footerLayoutPanel, 0, 1);
			this.mainLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this.mainLayoutPanel.Name = "mainLayoutPanel";
			this.mainLayoutPanel.Padding = new System.Windows.Forms.Padding(10);
			this.mainLayoutPanel.RowCount = 2;
			this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainLayoutPanel.Size = new System.Drawing.Size(699, 646);
			this.mainLayoutPanel.TabIndex = 0;
			//
			// contentLayoutPanel
			//
			this.contentLayoutPanel.ColumnCount = 2;
			this.contentLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
			this.contentLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55F));
			this.contentLayoutPanel.Controls.Add(this.configurationLayoutPanel, 0, 0);
			this.contentLayoutPanel.Controls.Add(this.previewLayoutPanel, 1, 0);
			this.contentLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.contentLayoutPanel.Location = new System.Drawing.Point(13, 13);
			this.contentLayoutPanel.Name = "contentLayoutPanel";
			this.contentLayoutPanel.RowCount = 1;
			this.contentLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.contentLayoutPanel.Size = new System.Drawing.Size(673, 584);
			this.contentLayoutPanel.TabIndex = 0;
			//
			// configurationLayoutPanel
			//
			this.configurationLayoutPanel.ColumnCount = 1;
			this.configurationLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.configurationLayoutPanel.Controls.Add(this.itemCountLayoutPanel, 0, 0);
			this.configurationLayoutPanel.Controls.Add(this.nameFormatLayoutPanel, 0, 1);
			this.configurationLayoutPanel.Controls.Add(this.label4, 0, 2);
			this.configurationLayoutPanel.Controls.Add(this.groupBoxSelectedNamingRule, 0, 3);
			this.configurationLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.configurationLayoutPanel.Location = new System.Drawing.Point(3, 3);
			this.configurationLayoutPanel.Name = "configurationLayoutPanel";
			this.configurationLayoutPanel.Padding = new System.Windows.Forms.Padding(0, 0, 6, 0);
			this.configurationLayoutPanel.RowCount = 4;
			this.configurationLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.configurationLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.configurationLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.configurationLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.configurationLayoutPanel.Size = new System.Drawing.Size(296, 578);
			this.configurationLayoutPanel.TabIndex = 0;
			//
			// itemCountLayoutPanel
			//
			this.itemCountLayoutPanel.AutoSize = true;
			this.itemCountLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.itemCountLayoutPanel.ColumnCount = 2;
			this.itemCountLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.itemCountLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.itemCountLayoutPanel.Controls.Add(this.label1, 0, 0);
			this.itemCountLayoutPanel.Controls.Add(this.numericUpDownItemCount, 1, 0);
			this.itemCountLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.itemCountLayoutPanel.Location = new System.Drawing.Point(3, 3);
			this.itemCountLayoutPanel.Name = "itemCountLayoutPanel";
			this.itemCountLayoutPanel.RowCount = 1;
			this.itemCountLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.itemCountLayoutPanel.Size = new System.Drawing.Size(284, 29);
			this.itemCountLayoutPanel.TabIndex = 0;
			//
			// label1
			//
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 7);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(130, 15);
			this.label1.TabIndex = 35;
			this.label1.Text = "Total Number of Items:";
			//
			// numericUpDownItemCount
			//
			this.numericUpDownItemCount.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.numericUpDownItemCount.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.numericUpDownItemCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownItemCount.Name = "numericUpDownItemCount";
			this.numericUpDownItemCount.Size = new System.Drawing.Size(61, 23);
			this.numericUpDownItemCount.TabIndex = 0;
			this.numericUpDownItemCount.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.numericUpDownItemCount.ValueChanged += new System.EventHandler(this.numericUpDownItemCount_ValueChanged);
			//
			// nameFormatLayoutPanel
			//
			this.nameFormatLayoutPanel.AutoSize = true;
			this.nameFormatLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.nameFormatLayoutPanel.ColumnCount = 2;
			this.nameFormatLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.nameFormatLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.nameFormatLayoutPanel.Controls.Add(this.label3, 0, 0);
			this.nameFormatLayoutPanel.Controls.Add(this.textBoxNameFormat, 1, 0);
			this.nameFormatLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.nameFormatLayoutPanel.Location = new System.Drawing.Point(3, 38);
			this.nameFormatLayoutPanel.Name = "nameFormatLayoutPanel";
			this.nameFormatLayoutPanel.RowCount = 1;
			this.nameFormatLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.nameFormatLayoutPanel.Size = new System.Drawing.Size(284, 29);
			this.nameFormatLayoutPanel.TabIndex = 1;
			//
			// label3
			//
			this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(3, 7);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(81, 15);
			this.label3.TabIndex = 39;
			this.label3.Text = "Name format:";
			//
			// textBoxNameFormat
			//
			this.textBoxNameFormat.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxNameFormat.Location = new System.Drawing.Point(90, 3);
			this.textBoxNameFormat.Name = "textBoxNameFormat";
			this.textBoxNameFormat.Size = new System.Drawing.Size(191, 23);
			this.textBoxNameFormat.TabIndex = 1;
			this.textBoxNameFormat.TextChanged += new System.EventHandler(this.textBoxNameFormat_TextChanged);
			//
			// label4
			//
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(3, 70);
			this.label4.Margin = new System.Windows.Forms.Padding(3, 0, 3, 6);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(189, 15);
			this.label4.TabIndex = 40;
			this.label4.Text = "Example: \"Tree - <1> - <2> - <3>\"";
			//
			// groupBoxSelectedNamingRule
			//
			this.groupBoxSelectedNamingRule.Controls.Add(this.rulesLayoutPanel);
			this.groupBoxSelectedNamingRule.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBoxSelectedNamingRule.Location = new System.Drawing.Point(3, 94);
			this.groupBoxSelectedNamingRule.Name = "groupBoxSelectedNamingRule";
			this.groupBoxSelectedNamingRule.Padding = new System.Windows.Forms.Padding(6);
			this.groupBoxSelectedNamingRule.Size = new System.Drawing.Size(284, 481);
			this.groupBoxSelectedNamingRule.TabIndex = 2;
			this.groupBoxSelectedNamingRule.TabStop = false;
			this.groupBoxSelectedNamingRule.Text = "Naming Rules";
			this.groupBoxSelectedNamingRule.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			//
			// rulesLayoutPanel
			//
			this.rulesLayoutPanel.ColumnCount = 2;
			this.rulesLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.rulesLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.rulesLayoutPanel.Controls.Add(this.ruleTypeLayoutPanel, 0, 0);
			this.rulesLayoutPanel.Controls.Add(this.listViewGenerators, 0, 1);
			this.rulesLayoutPanel.Controls.Add(this.moveRuleButtonsLayoutPanel, 1, 1);
			this.rulesLayoutPanel.Controls.Add(this.panelRuleConfig, 0, 2);
			this.rulesLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rulesLayoutPanel.Location = new System.Drawing.Point(6, 22);
			this.rulesLayoutPanel.Name = "rulesLayoutPanel";
			this.rulesLayoutPanel.RowCount = 3;
			this.rulesLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.rulesLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.rulesLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.rulesLayoutPanel.Size = new System.Drawing.Size(272, 453);
			this.rulesLayoutPanel.TabIndex = 0;
			this.rulesLayoutPanel.SetColumnSpan(this.ruleTypeLayoutPanel, 2);
			this.rulesLayoutPanel.SetColumnSpan(this.panelRuleConfig, 2);
			//
			// ruleTypeLayoutPanel
			//
			this.ruleTypeLayoutPanel.AutoSize = true;
			this.ruleTypeLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ruleTypeLayoutPanel.Controls.Add(this.comboBoxRuleTypes);
			this.ruleTypeLayoutPanel.Controls.Add(this.ruleCommandsLayoutPanel);
			this.ruleTypeLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ruleTypeLayoutPanel.Location = new System.Drawing.Point(3, 3);
			this.ruleTypeLayoutPanel.Name = "ruleTypeLayoutPanel";
			this.ruleTypeLayoutPanel.Size = new System.Drawing.Size(266, 34);
			this.ruleTypeLayoutPanel.TabIndex = 0;
			this.ruleTypeLayoutPanel.WrapContents = false;
			//
			// comboBoxRuleTypes
			//
			this.comboBoxRuleTypes.FormattingEnabled = true;
			this.comboBoxRuleTypes.Location = new System.Drawing.Point(3, 5);
			this.comboBoxRuleTypes.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
			this.comboBoxRuleTypes.Name = "comboBoxRuleTypes";
			this.comboBoxRuleTypes.Size = new System.Drawing.Size(170, 23);
			this.comboBoxRuleTypes.TabIndex = 0;
			this.comboBoxRuleTypes.SelectedIndexChanged += new System.EventHandler(this.comboBoxRuleTypes_SelectedIndexChanged);
			//
			// ruleCommandsLayoutPanel
			//
			this.ruleCommandsLayoutPanel.AutoSize = true;
			this.ruleCommandsLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ruleCommandsLayoutPanel.Controls.Add(this.buttonAddNewRule);
			this.ruleCommandsLayoutPanel.Controls.Add(this.buttonDeleteRule);
			this.ruleCommandsLayoutPanel.Location = new System.Drawing.Point(179, 3);
			this.ruleCommandsLayoutPanel.Name = "ruleCommandsLayoutPanel";
			this.ruleCommandsLayoutPanel.Size = new System.Drawing.Size(68, 28);
			this.ruleCommandsLayoutPanel.TabIndex = 1;
			this.ruleCommandsLayoutPanel.WrapContents = false;
			//
			// buttonAddNewRule
			//
			this.buttonAddNewRule.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this.buttonAddNewRule.Name = "buttonAddNewRule";
			this.buttonAddNewRule.Size = new System.Drawing.Size(28, 28);
			this.buttonAddNewRule.TabIndex = 3;
			this.buttonAddNewRule.TabStop = false;
			this.buttonAddNewRule.Text = "+";
			this.buttonAddNewRule.UseVisualStyleBackColor = false;
			this.buttonAddNewRule.Click += new System.EventHandler(this.buttonAddNewRule_Click);
			//
			// buttonDeleteRule
			//
			this.buttonDeleteRule.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this.buttonDeleteRule.Name = "buttonDeleteRule";
			this.buttonDeleteRule.Size = new System.Drawing.Size(28, 28);
			this.buttonDeleteRule.TabIndex = 4;
			this.buttonDeleteRule.TabStop = false;
			this.buttonDeleteRule.Text = "-";
			this.buttonDeleteRule.UseVisualStyleBackColor = false;
			this.buttonDeleteRule.Click += new System.EventHandler(this.buttonDeleteRule_Click);
			//
			// listViewGenerators
			//
			this.listViewGenerators.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
			this.listViewGenerators.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewGenerators.HideSelection = false;
			this.listViewGenerators.Location = new System.Drawing.Point(3, 43);
			this.listViewGenerators.MultiSelect = false;
			this.listViewGenerators.Name = "listViewGenerators";
			this.listViewGenerators.OwnerDraw = true;
			this.listViewGenerators.Size = new System.Drawing.Size(226, 200);
			this.listViewGenerators.TabIndex = 5;
			this.listViewGenerators.TabStop = false;
			this.listViewGenerators.UseCompatibleStateImageBehavior = false;
			this.listViewGenerators.View = System.Windows.Forms.View.List;
			this.listViewGenerators.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listViewGenerators_Highlight);
			this.listViewGenerators.SelectedIndexChanged += new System.EventHandler(this.listViewGenerators_SelectedIndexChanged);
			//
			// columnHeader3
			//
			this.columnHeader3.Width = 190;
			//
			// moveRuleButtonsLayoutPanel
			//
			this.moveRuleButtonsLayoutPanel.AutoSize = true;
			this.moveRuleButtonsLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.moveRuleButtonsLayoutPanel.Controls.Add(this.buttonMoveRuleUp);
			this.moveRuleButtonsLayoutPanel.Controls.Add(this.buttonMoveRuleDown);
			this.moveRuleButtonsLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.moveRuleButtonsLayoutPanel.Location = new System.Drawing.Point(235, 43);
			this.moveRuleButtonsLayoutPanel.Name = "moveRuleButtonsLayoutPanel";
			this.moveRuleButtonsLayoutPanel.Size = new System.Drawing.Size(34, 64);
			this.moveRuleButtonsLayoutPanel.TabIndex = 1;
			this.moveRuleButtonsLayoutPanel.WrapContents = false;
			//
			// buttonMoveRuleUp
			//
			this.buttonMoveRuleUp.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.buttonMoveRuleUp.Name = "buttonMoveRuleUp";
			this.buttonMoveRuleUp.Size = new System.Drawing.Size(34, 26);
			this.buttonMoveRuleUp.TabIndex = 6;
			this.buttonMoveRuleUp.TabStop = false;
			this.buttonMoveRuleUp.Text = "U";
			this.buttonMoveRuleUp.UseVisualStyleBackColor = false;
			this.buttonMoveRuleUp.Click += new System.EventHandler(this.buttonMoveRuleUp_Click);
			//
			// buttonMoveRuleDown
			//
			this.buttonMoveRuleDown.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.buttonMoveRuleDown.Name = "buttonMoveRuleDown";
			this.buttonMoveRuleDown.Size = new System.Drawing.Size(34, 26);
			this.buttonMoveRuleDown.TabIndex = 7;
			this.buttonMoveRuleDown.TabStop = false;
			this.buttonMoveRuleDown.Text = "D";
			this.buttonMoveRuleDown.UseVisualStyleBackColor = false;
			this.buttonMoveRuleDown.Click += new System.EventHandler(this.buttonMoveRuleDown_Click);
			//
			// panelRuleConfig
			//
			this.panelRuleConfig.AutoScroll = true;
			this.panelRuleConfig.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelRuleConfig.Location = new System.Drawing.Point(3, 249);
			this.panelRuleConfig.Name = "panelRuleConfig";
			this.panelRuleConfig.Size = new System.Drawing.Size(266, 201);
			this.panelRuleConfig.TabIndex = 1;
			this.panelRuleConfig.TabStop = true;
			//
			// previewLayoutPanel
			//
			this.previewLayoutPanel.ColumnCount = 1;
			this.previewLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.previewLayoutPanel.Controls.Add(this.previewHeaderLayoutPanel, 0, 0);
			this.previewLayoutPanel.Controls.Add(this.listViewNames, 0, 1);
			this.previewLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.previewLayoutPanel.Location = new System.Drawing.Point(305, 3);
			this.previewLayoutPanel.Name = "previewLayoutPanel";
			this.previewLayoutPanel.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
			this.previewLayoutPanel.RowCount = 2;
			this.previewLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.previewLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.previewLayoutPanel.Size = new System.Drawing.Size(365, 578);
			this.previewLayoutPanel.TabIndex = 1;
			//
			// previewHeaderLayoutPanel
			//
			this.previewHeaderLayoutPanel.AutoSize = true;
			this.previewHeaderLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.previewHeaderLayoutPanel.ColumnCount = 2;
			this.previewHeaderLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.previewHeaderLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.previewHeaderLayoutPanel.Controls.Add(this.labelColumnHeader1, 0, 0);
			this.previewHeaderLayoutPanel.Controls.Add(this.labelColumnHeader2, 1, 0);
			this.previewHeaderLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.previewHeaderLayoutPanel.Location = new System.Drawing.Point(9, 3);
			this.previewHeaderLayoutPanel.Name = "previewHeaderLayoutPanel";
			this.previewHeaderLayoutPanel.RowCount = 1;
			this.previewHeaderLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.previewHeaderLayoutPanel.Size = new System.Drawing.Size(353, 21);
			this.previewHeaderLayoutPanel.TabIndex = 0;
			//
			// labelColumnHeader1
			//
			this.labelColumnHeader1.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelColumnHeader1.AutoSize = true;
			this.labelColumnHeader1.Location = new System.Drawing.Point(3, 3);
			this.labelColumnHeader1.Name = "labelColumnHeader1";
			this.labelColumnHeader1.Size = new System.Drawing.Size(97, 15);
			this.labelColumnHeader1.TabIndex = 41;
			this.labelColumnHeader1.Text = "Column Header1";
			//
			// labelColumnHeader2
			//
			this.labelColumnHeader2.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelColumnHeader2.AutoSize = true;
			this.labelColumnHeader2.Location = new System.Drawing.Point(179, 3);
			this.labelColumnHeader2.Name = "labelColumnHeader2";
			this.labelColumnHeader2.Size = new System.Drawing.Size(97, 15);
			this.labelColumnHeader2.TabIndex = 42;
			this.labelColumnHeader2.Text = "Column Header2";
			//
			// listViewNames
			//
			this.listViewNames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.listViewNames.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewNames.FullRowSelect = true;
			this.listViewNames.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewNames.HideSelection = false;
			this.listViewNames.Location = new System.Drawing.Point(9, 30);
			this.listViewNames.MultiSelect = false;
			this.listViewNames.Name = "listViewNames";
			this.listViewNames.ShowGroups = false;
			this.listViewNames.Size = new System.Drawing.Size(353, 545);
			this.listViewNames.TabIndex = 1;
			this.listViewNames.TabStop = false;
			this.listViewNames.UseCompatibleStateImageBehavior = false;
			this.listViewNames.View = System.Windows.Forms.View.Details;
			this.listViewNames.Resize += new System.EventHandler(this.listViewNames_Resize);
			//
			// columnHeader1
			//
			this.columnHeader1.Text = "Old Name";
			this.columnHeader1.Width = 120;
			//
			// columnHeader2
			//
			this.columnHeader2.Text = "New Name";
			this.columnHeader2.Width = 120;
			//
			// footerLayoutPanel
			//
			this.footerLayoutPanel.AutoSize = true;
			this.footerLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.footerLayoutPanel.ColumnCount = 2;
			this.footerLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.footerLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.footerLayoutPanel.Controls.Add(this.templateLayoutPanel, 0, 0);
			this.footerLayoutPanel.Controls.Add(this.dialogButtonsLayoutPanel, 1, 0);
			this.footerLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.footerLayoutPanel.Location = new System.Drawing.Point(13, 603);
			this.footerLayoutPanel.Name = "footerLayoutPanel";
			this.footerLayoutPanel.RowCount = 1;
			this.footerLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.footerLayoutPanel.Size = new System.Drawing.Size(673, 30);
			this.footerLayoutPanel.TabIndex = 1;
			//
			// templateLayoutPanel
			//
			this.templateLayoutPanel.AutoSize = true;
			this.templateLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.templateLayoutPanel.Controls.Add(this.label2);
			this.templateLayoutPanel.Controls.Add(this.comboBoxTemplates);
			this.templateLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.templateLayoutPanel.Location = new System.Drawing.Point(3, 3);
			this.templateLayoutPanel.Name = "templateLayoutPanel";
			this.templateLayoutPanel.Size = new System.Drawing.Size(433, 24);
			this.templateLayoutPanel.TabIndex = 0;
			this.templateLayoutPanel.WrapContents = false;
			//
			// label2
			//
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label2.AutoSize = true;
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(79, 15);
			this.label2.TabIndex = 37;
			this.label2.Text = "Use template:";
			this.label2.Visible = false;
			//
			// comboBoxTemplates
			//
			this.comboBoxTemplates.FormattingEnabled = true;
			this.comboBoxTemplates.Name = "comboBoxTemplates";
			this.comboBoxTemplates.Size = new System.Drawing.Size(192, 23);
			this.comboBoxTemplates.TabIndex = 36;
			this.comboBoxTemplates.Visible = false;
			this.comboBoxTemplates.SelectedIndexChanged += new System.EventHandler(this.comboBoxTemplates_SelectedIndexChanged);
			//
			// dialogButtonsLayoutPanel
			//
			this.dialogButtonsLayoutPanel.AutoSize = true;
			this.dialogButtonsLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.dialogButtonsLayoutPanel.Controls.Add(this.buttonCancel);
			this.dialogButtonsLayoutPanel.Controls.Add(this.buttonOk);
			this.dialogButtonsLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.dialogButtonsLayoutPanel.Location = new System.Drawing.Point(442, 3);
			this.dialogButtonsLayoutPanel.Name = "dialogButtonsLayoutPanel";
			this.dialogButtonsLayoutPanel.Size = new System.Drawing.Size(228, 24);
			this.dialogButtonsLayoutPanel.TabIndex = 1;
			this.dialogButtonsLayoutPanel.WrapContents = false;
			//
			// buttonCancel
			//
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(105, 29);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			//
			// buttonOk
			//
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(105, 29);
			this.buttonOk.TabIndex = 2;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			//
			// NameGenerator
			//
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(699, 646);
			this.Controls.Add(this.mainLayoutPanel);
			this.DoubleBuffered = true;
			this.MinimumSize = new System.Drawing.Size(715, 684);
			this.Name = "NameGenerator";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Create/Modify Multiple Items";
			this.Load += new System.EventHandler(this.BulkRename_Load);
			this.mainLayoutPanel.ResumeLayout(false);
			this.mainLayoutPanel.PerformLayout();
			this.contentLayoutPanel.ResumeLayout(false);
			this.configurationLayoutPanel.ResumeLayout(false);
			this.configurationLayoutPanel.PerformLayout();
			this.itemCountLayoutPanel.ResumeLayout(false);
			this.itemCountLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownItemCount)).EndInit();
			this.nameFormatLayoutPanel.ResumeLayout(false);
			this.nameFormatLayoutPanel.PerformLayout();
			this.groupBoxSelectedNamingRule.ResumeLayout(false);
			this.rulesLayoutPanel.ResumeLayout(false);
			this.rulesLayoutPanel.PerformLayout();
			this.ruleTypeLayoutPanel.ResumeLayout(false);
			this.ruleTypeLayoutPanel.PerformLayout();
			this.ruleCommandsLayoutPanel.ResumeLayout(false);
			this.moveRuleButtonsLayoutPanel.ResumeLayout(false);
			this.previewLayoutPanel.ResumeLayout(false);
			this.previewLayoutPanel.PerformLayout();
			this.previewHeaderLayoutPanel.ResumeLayout(false);
			this.previewHeaderLayoutPanel.PerformLayout();
			this.footerLayoutPanel.ResumeLayout(false);
			this.footerLayoutPanel.PerformLayout();
			this.templateLayoutPanel.ResumeLayout(false);
			this.templateLayoutPanel.PerformLayout();
			this.dialogButtonsLayoutPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel mainLayoutPanel;
		private System.Windows.Forms.TableLayoutPanel contentLayoutPanel;
		private System.Windows.Forms.TableLayoutPanel configurationLayoutPanel;
		private System.Windows.Forms.TableLayoutPanel itemCountLayoutPanel;
		private System.Windows.Forms.TableLayoutPanel nameFormatLayoutPanel;
		private System.Windows.Forms.TableLayoutPanel rulesLayoutPanel;
		private System.Windows.Forms.FlowLayoutPanel ruleTypeLayoutPanel;
		private System.Windows.Forms.FlowLayoutPanel ruleCommandsLayoutPanel;
		private System.Windows.Forms.FlowLayoutPanel moveRuleButtonsLayoutPanel;
		private System.Windows.Forms.TableLayoutPanel previewLayoutPanel;
		private System.Windows.Forms.TableLayoutPanel previewHeaderLayoutPanel;
		private System.Windows.Forms.TableLayoutPanel footerLayoutPanel;
		private System.Windows.Forms.FlowLayoutPanel templateLayoutPanel;
		private System.Windows.Forms.FlowLayoutPanel dialogButtonsLayoutPanel;
		private System.Windows.Forms.ListView listViewNames;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.GroupBox groupBoxSelectedNamingRule;
		private System.Windows.Forms.Button buttonMoveRuleDown;
		private System.Windows.Forms.Button buttonMoveRuleUp;
		private System.Windows.Forms.Button buttonDeleteRule;
		private System.Windows.Forms.Panel panelRuleConfig;
		private System.Windows.Forms.NumericUpDown numericUpDownItemCount;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxTemplates;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxNameFormat;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboBoxRuleTypes;
		private System.Windows.Forms.Button buttonAddNewRule;
		private System.Windows.Forms.ListView listViewGenerators;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label labelColumnHeader1;
		private System.Windows.Forms.Label labelColumnHeader2;
	}
}
