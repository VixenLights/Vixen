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
			this.listViewNames = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			this.groupBoxSelectedNamingRule = new System.Windows.Forms.GroupBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonAddNewRule = new System.Windows.Forms.Button();
			this.buttonDeleteRule = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.buttonMoveRuleDown = new System.Windows.Forms.Button();
			this.buttonMoveRuleUp = new System.Windows.Forms.Button();
			this.comboBoxRuleTypes = new System.Windows.Forms.ComboBox();
			this.listViewGenerators = new System.Windows.Forms.ListView();
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.panelRuleConfig = new System.Windows.Forms.Panel();
			this.numericUpDownItemCount = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.comboBoxTemplates = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxNameFormat = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.labelColumnHeader1 = new System.Windows.Forms.Label();
			this.labelColumnHeader2 = new System.Windows.Forms.Label();
			this.groupBoxSelectedNamingRule.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownItemCount)).BeginInit();
			this.SuspendLayout();
			// 
			// listViewNames
			// 
			this.listViewNames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewNames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.listViewNames.FullRowSelect = true;
			this.listViewNames.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewNames.HideSelection = false;
			this.listViewNames.Location = new System.Drawing.Point(322, 43);
			this.listViewNames.MultiSelect = false;
			this.listViewNames.Name = "listViewNames";
			this.listViewNames.ShowGroups = false;
			this.listViewNames.Size = new System.Drawing.Size(362, 551);
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
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(556, 601);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(105, 29);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(414, 602);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(105, 29);
			this.buttonOk.TabIndex = 2;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonOk.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOk.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// groupBoxSelectedNamingRule
			// 
			this.groupBoxSelectedNamingRule.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBoxSelectedNamingRule.Controls.Add(this.panel1);
			this.groupBoxSelectedNamingRule.Controls.Add(this.panel2);
			this.groupBoxSelectedNamingRule.Controls.Add(this.comboBoxRuleTypes);
			this.groupBoxSelectedNamingRule.Controls.Add(this.listViewGenerators);
			this.groupBoxSelectedNamingRule.Controls.Add(this.panelRuleConfig);
			this.groupBoxSelectedNamingRule.Location = new System.Drawing.Point(21, 107);
			this.groupBoxSelectedNamingRule.Name = "groupBoxSelectedNamingRule";
			this.groupBoxSelectedNamingRule.Size = new System.Drawing.Size(294, 487);
			this.groupBoxSelectedNamingRule.TabIndex = 2;
			this.groupBoxSelectedNamingRule.TabStop = false;
			this.groupBoxSelectedNamingRule.Text = "Naming Rules";
			this.groupBoxSelectedNamingRule.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.buttonAddNewRule);
			this.panel1.Controls.Add(this.buttonDeleteRule);
			this.panel1.Location = new System.Drawing.Point(187, 18);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(100, 44);
			this.panel1.TabIndex = 33;
			// 
			// buttonAddNewRule
			// 
			this.buttonAddNewRule.BackColor = System.Drawing.Color.Transparent;
			this.buttonAddNewRule.FlatAppearance.BorderSize = 0;
			this.buttonAddNewRule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAddNewRule.Location = new System.Drawing.Point(17, 6);
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
			this.buttonDeleteRule.BackColor = System.Drawing.Color.Transparent;
			this.buttonDeleteRule.FlatAppearance.BorderSize = 0;
			this.buttonDeleteRule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonDeleteRule.Location = new System.Drawing.Point(58, 6);
			this.buttonDeleteRule.Name = "buttonDeleteRule";
			this.buttonDeleteRule.Size = new System.Drawing.Size(28, 28);
			this.buttonDeleteRule.TabIndex = 4;
			this.buttonDeleteRule.TabStop = false;
			this.buttonDeleteRule.Text = "-";
			this.buttonDeleteRule.UseVisualStyleBackColor = false;
			this.buttonDeleteRule.Click += new System.EventHandler(this.buttonDeleteRule_Click);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.buttonMoveRuleDown);
			this.panel2.Controls.Add(this.buttonMoveRuleUp);
			this.panel2.Location = new System.Drawing.Point(241, 51);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(45, 95);
			this.panel2.TabIndex = 32;
			// 
			// buttonMoveRuleDown
			// 
			this.buttonMoveRuleDown.BackColor = System.Drawing.Color.Transparent;
			this.buttonMoveRuleDown.FlatAppearance.BorderSize = 0;
			this.buttonMoveRuleDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonMoveRuleDown.Location = new System.Drawing.Point(1, 53);
			this.buttonMoveRuleDown.Name = "buttonMoveRuleDown";
			this.buttonMoveRuleDown.Size = new System.Drawing.Size(35, 29);
			this.buttonMoveRuleDown.TabIndex = 7;
			this.buttonMoveRuleDown.TabStop = false;
			this.buttonMoveRuleDown.Text = "D";
			this.buttonMoveRuleDown.UseVisualStyleBackColor = false;
			this.buttonMoveRuleDown.Click += new System.EventHandler(this.buttonMoveRuleDown_Click);
			// 
			// buttonMoveRuleUp
			// 
			this.buttonMoveRuleUp.BackColor = System.Drawing.Color.Transparent;
			this.buttonMoveRuleUp.FlatAppearance.BorderSize = 0;
			this.buttonMoveRuleUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonMoveRuleUp.Location = new System.Drawing.Point(1, 18);
			this.buttonMoveRuleUp.Name = "buttonMoveRuleUp";
			this.buttonMoveRuleUp.Size = new System.Drawing.Size(35, 29);
			this.buttonMoveRuleUp.TabIndex = 6;
			this.buttonMoveRuleUp.TabStop = false;
			this.buttonMoveRuleUp.Text = "U";
			this.buttonMoveRuleUp.UseVisualStyleBackColor = false;
			this.buttonMoveRuleUp.Click += new System.EventHandler(this.buttonMoveRuleUp_Click);
			// 
			// comboBoxRuleTypes
			// 
			this.comboBoxRuleTypes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.comboBoxRuleTypes.FormattingEnabled = true;
			this.comboBoxRuleTypes.Location = new System.Drawing.Point(10, 25);
			this.comboBoxRuleTypes.Name = "comboBoxRuleTypes";
			this.comboBoxRuleTypes.Size = new System.Drawing.Size(170, 23);
			this.comboBoxRuleTypes.TabIndex = 0;
			this.comboBoxRuleTypes.SelectedIndexChanged += new System.EventHandler(this.comboBoxRuleTypes_SelectedIndexChanged);
			// 
			// listViewGenerators
			// 
			this.listViewGenerators.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listViewGenerators.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
			this.listViewGenerators.HideSelection = false;
			this.listViewGenerators.Location = new System.Drawing.Point(7, 69);
			this.listViewGenerators.MultiSelect = false;
			this.listViewGenerators.Name = "listViewGenerators";
			this.listViewGenerators.OwnerDraw = true;
			this.listViewGenerators.Size = new System.Drawing.Size(224, 197);
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
			// panelRuleConfig
			// 
			this.panelRuleConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.panelRuleConfig.Location = new System.Drawing.Point(10, 273);
			this.panelRuleConfig.Name = "panelRuleConfig";
			this.panelRuleConfig.Size = new System.Drawing.Size(269, 207);
			this.panelRuleConfig.TabIndex = 1;
			this.panelRuleConfig.TabStop = true;
			// 
			// numericUpDownItemCount
			// 
			this.numericUpDownItemCount.Location = new System.Drawing.Point(254, 22);
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
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(14, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(130, 15);
			this.label1.TabIndex = 35;
			this.label1.Text = "Total Number of Items:";
			// 
			// comboBoxTemplates
			// 
			this.comboBoxTemplates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.comboBoxTemplates.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.comboBoxTemplates.FormattingEnabled = true;
			this.comboBoxTemplates.Location = new System.Drawing.Point(125, 607);
			this.comboBoxTemplates.Name = "comboBoxTemplates";
			this.comboBoxTemplates.Size = new System.Drawing.Size(192, 23);
			this.comboBoxTemplates.TabIndex = 36;
			this.comboBoxTemplates.Visible = false;
			this.comboBoxTemplates.SelectedIndexChanged += new System.EventHandler(this.comboBoxTemplates_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(28, 608);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(79, 15);
			this.label2.TabIndex = 37;
			this.label2.Text = "Use template:";
			this.label2.Visible = false;
			// 
			// textBoxNameFormat
			// 
			this.textBoxNameFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.textBoxNameFormat.Location = new System.Drawing.Point(105, 62);
			this.textBoxNameFormat.Name = "textBoxNameFormat";
			this.textBoxNameFormat.Size = new System.Drawing.Size(209, 23);
			this.textBoxNameFormat.TabIndex = 1;
			this.textBoxNameFormat.TextChanged += new System.EventHandler(this.textBoxNameFormat_TextChanged);
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(16, 66);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(81, 15);
			this.label3.TabIndex = 39;
			this.label3.Text = "Name format:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(115, 89);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(189, 15);
			this.label4.TabIndex = 40;
			this.label4.Text = "Example: \"Tree - <1> - <2> - <3>\"";
			// 
			// labelColumnHeader1
			// 
			this.labelColumnHeader1.AutoSize = true;
			this.labelColumnHeader1.Location = new System.Drawing.Point(325, 24);
			this.labelColumnHeader1.Name = "labelColumnHeader1";
			this.labelColumnHeader1.Size = new System.Drawing.Size(97, 15);
			this.labelColumnHeader1.TabIndex = 41;
			this.labelColumnHeader1.Text = "Column Header1";
			// 
			// labelColumnHeader2
			// 
			this.labelColumnHeader2.AutoSize = true;
			this.labelColumnHeader2.Location = new System.Drawing.Point(493, 24);
			this.labelColumnHeader2.Name = "labelColumnHeader2";
			this.labelColumnHeader2.Size = new System.Drawing.Size(97, 15);
			this.labelColumnHeader2.TabIndex = 42;
			this.labelColumnHeader2.Text = "Column Header2";
			// 
			// NameGenerator
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(699, 646);
			this.Controls.Add(this.labelColumnHeader2);
			this.Controls.Add(this.labelColumnHeader1);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textBoxNameFormat);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboBoxTemplates);
			this.Controls.Add(this.numericUpDownItemCount);
			this.Controls.Add(this.groupBoxSelectedNamingRule);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.listViewNames);
			this.DoubleBuffered = true;
			this.MinimumSize = new System.Drawing.Size(715, 684);
			this.Name = "NameGenerator";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Create/Modify Multiple Items";
			this.Load += new System.EventHandler(this.BulkRename_Load);
			this.groupBoxSelectedNamingRule.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownItemCount)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

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
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel1;
	}
}