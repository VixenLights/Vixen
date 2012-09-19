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
			this.comboBoxRuleTypes = new System.Windows.Forms.ComboBox();
			this.buttonAddNewRule = new System.Windows.Forms.Button();
			this.buttonDeleteRule = new System.Windows.Forms.Button();
			this.listViewGenerators = new System.Windows.Forms.ListView();
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonMoveRuleDown = new System.Windows.Forms.Button();
			this.buttonMoveRuleUp = new System.Windows.Forms.Button();
			this.panelRuleConfig = new System.Windows.Forms.Panel();
			this.numericUpDownItemCount = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.comboBoxTemplates = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxNameFormat = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBoxSelectedNamingRule.SuspendLayout();
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
			this.listViewNames.GridLines = true;
			this.listViewNames.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewNames.HideSelection = false;
			this.listViewNames.Location = new System.Drawing.Point(12, 12);
			this.listViewNames.MultiSelect = false;
			this.listViewNames.Name = "listViewNames";
			this.listViewNames.ShowGroups = false;
			this.listViewNames.Size = new System.Drawing.Size(460, 231);
			this.listViewNames.TabIndex = 1;
			this.listViewNames.UseCompatibleStateImageBehavior = false;
			this.listViewNames.View = System.Windows.Forms.View.Details;
			this.listViewNames.Resize += new System.EventHandler(this.listViewNames_Resize);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Old Name";
			this.columnHeader1.Width = 158;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "New Name";
			this.columnHeader2.Width = 209;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(382, 523);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(90, 25);
			this.buttonCancel.TabIndex = 28;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(286, 523);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(90, 25);
			this.buttonOk.TabIndex = 27;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// groupBoxSelectedNamingRule
			// 
			this.groupBoxSelectedNamingRule.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxSelectedNamingRule.Controls.Add(this.comboBoxRuleTypes);
			this.groupBoxSelectedNamingRule.Controls.Add(this.buttonAddNewRule);
			this.groupBoxSelectedNamingRule.Controls.Add(this.buttonDeleteRule);
			this.groupBoxSelectedNamingRule.Controls.Add(this.listViewGenerators);
			this.groupBoxSelectedNamingRule.Controls.Add(this.buttonMoveRuleDown);
			this.groupBoxSelectedNamingRule.Controls.Add(this.buttonMoveRuleUp);
			this.groupBoxSelectedNamingRule.Controls.Add(this.panelRuleConfig);
			this.groupBoxSelectedNamingRule.Location = new System.Drawing.Point(12, 331);
			this.groupBoxSelectedNamingRule.Name = "groupBoxSelectedNamingRule";
			this.groupBoxSelectedNamingRule.Size = new System.Drawing.Size(460, 178);
			this.groupBoxSelectedNamingRule.TabIndex = 31;
			this.groupBoxSelectedNamingRule.TabStop = false;
			this.groupBoxSelectedNamingRule.Text = "Naming Rules";
			// 
			// comboBoxRuleTypes
			// 
			this.comboBoxRuleTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.comboBoxRuleTypes.FormattingEnabled = true;
			this.comboBoxRuleTypes.Location = new System.Drawing.Point(16, 144);
			this.comboBoxRuleTypes.Name = "comboBoxRuleTypes";
			this.comboBoxRuleTypes.Size = new System.Drawing.Size(110, 21);
			this.comboBoxRuleTypes.TabIndex = 44;
			this.comboBoxRuleTypes.SelectedIndexChanged += new System.EventHandler(this.comboBoxRuleTypes_SelectedIndexChanged);
			// 
			// buttonAddNewRule
			// 
			this.buttonAddNewRule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonAddNewRule.Location = new System.Drawing.Point(132, 141);
			this.buttonAddNewRule.Name = "buttonAddNewRule";
			this.buttonAddNewRule.Size = new System.Drawing.Size(80, 25);
			this.buttonAddNewRule.TabIndex = 45;
			this.buttonAddNewRule.Text = "Add Rule";
			this.buttonAddNewRule.UseVisualStyleBackColor = true;
			this.buttonAddNewRule.Click += new System.EventHandler(this.buttonAddNewRule_Click);
			// 
			// buttonDeleteRule
			// 
			this.buttonDeleteRule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDeleteRule.Location = new System.Drawing.Point(294, 141);
			this.buttonDeleteRule.Name = "buttonDeleteRule";
			this.buttonDeleteRule.Size = new System.Drawing.Size(80, 25);
			this.buttonDeleteRule.TabIndex = 34;
			this.buttonDeleteRule.Text = "Delete Rule";
			this.buttonDeleteRule.UseVisualStyleBackColor = true;
			this.buttonDeleteRule.Click += new System.EventHandler(this.buttonDeleteRule_Click);
			// 
			// listViewGenerators
			// 
			this.listViewGenerators.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listViewGenerators.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
			this.listViewGenerators.HideSelection = false;
			this.listViewGenerators.Location = new System.Drawing.Point(16, 27);
			this.listViewGenerators.MultiSelect = false;
			this.listViewGenerators.Name = "listViewGenerators";
			this.listViewGenerators.Size = new System.Drawing.Size(110, 106);
			this.listViewGenerators.TabIndex = 43;
			this.listViewGenerators.UseCompatibleStateImageBehavior = false;
			this.listViewGenerators.View = System.Windows.Forms.View.List;
			this.listViewGenerators.SelectedIndexChanged += new System.EventHandler(this.listViewGenerators_SelectedIndexChanged);
			// 
			// columnHeader3
			// 
			this.columnHeader3.Width = 120;
			// 
			// buttonMoveRuleDown
			// 
			this.buttonMoveRuleDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonMoveRuleDown.Image = global::Common.Controls.Icons.DownArrowShort_Blue_16x16;
			this.buttonMoveRuleDown.Location = new System.Drawing.Point(416, 141);
			this.buttonMoveRuleDown.Name = "buttonMoveRuleDown";
			this.buttonMoveRuleDown.Size = new System.Drawing.Size(30, 25);
			this.buttonMoveRuleDown.TabIndex = 36;
			this.buttonMoveRuleDown.UseVisualStyleBackColor = true;
			this.buttonMoveRuleDown.Click += new System.EventHandler(this.buttonMoveRuleDown_Click);
			// 
			// buttonMoveRuleUp
			// 
			this.buttonMoveRuleUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonMoveRuleUp.Image = global::Common.Controls.Icons.UpArrowShort_Blue_16x16;
			this.buttonMoveRuleUp.Location = new System.Drawing.Point(380, 141);
			this.buttonMoveRuleUp.Name = "buttonMoveRuleUp";
			this.buttonMoveRuleUp.Size = new System.Drawing.Size(30, 25);
			this.buttonMoveRuleUp.TabIndex = 35;
			this.buttonMoveRuleUp.UseVisualStyleBackColor = true;
			this.buttonMoveRuleUp.Click += new System.EventHandler(this.buttonMoveRuleUp_Click);
			// 
			// panelRuleConfig
			// 
			this.panelRuleConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelRuleConfig.Location = new System.Drawing.Point(132, 27);
			this.panelRuleConfig.Name = "panelRuleConfig";
			this.panelRuleConfig.Size = new System.Drawing.Size(312, 106);
			this.panelRuleConfig.TabIndex = 30;
			// 
			// numericUpDownItemCount
			// 
			this.numericUpDownItemCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.numericUpDownItemCount.Location = new System.Drawing.Point(392, 299);
			this.numericUpDownItemCount.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.numericUpDownItemCount.Name = "numericUpDownItemCount";
			this.numericUpDownItemCount.Size = new System.Drawing.Size(50, 20);
			this.numericUpDownItemCount.TabIndex = 34;
			this.numericUpDownItemCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(286, 302);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 13);
			this.label1.TabIndex = 35;
			this.label1.Text = "Names to generate:";
			// 
			// comboBoxTemplates
			// 
			this.comboBoxTemplates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.comboBoxTemplates.FormattingEnabled = true;
			this.comboBoxTemplates.Location = new System.Drawing.Point(103, 262);
			this.comboBoxTemplates.Name = "comboBoxTemplates";
			this.comboBoxTemplates.Size = new System.Drawing.Size(165, 21);
			this.comboBoxTemplates.TabIndex = 36;
			this.comboBoxTemplates.SelectedIndexChanged += new System.EventHandler(this.comboBoxTemplates_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(25, 265);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 13);
			this.label2.TabIndex = 37;
			this.label2.Text = "Use template:";
			// 
			// textBoxNameFormat
			// 
			this.textBoxNameFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxNameFormat.Location = new System.Drawing.Point(103, 299);
			this.textBoxNameFormat.Name = "textBoxNameFormat";
			this.textBoxNameFormat.Size = new System.Drawing.Size(165, 20);
			this.textBoxNameFormat.TabIndex = 38;
			this.textBoxNameFormat.TextChanged += new System.EventHandler(this.textBoxNameFormat_TextChanged);
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(27, 302);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(70, 13);
			this.label3.TabIndex = 39;
			this.label3.Text = "Name format:";
			// 
			// NameGenerator
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(484, 560);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textBoxNameFormat);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboBoxTemplates);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.numericUpDownItemCount);
			this.Controls.Add(this.groupBoxSelectedNamingRule);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.listViewNames);
			this.DoubleBuffered = true;
			this.MinimumSize = new System.Drawing.Size(500, 500);
			this.Name = "NameGenerator";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Name Generation";
			this.Load += new System.EventHandler(this.BulkRename_Load);
			this.groupBoxSelectedNamingRule.ResumeLayout(false);
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
	}
}