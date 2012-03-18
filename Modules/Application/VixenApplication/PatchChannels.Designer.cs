namespace VixenApplication {
	partial class PatchChannels {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
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
			this.label1 = new System.Windows.Forms.Label();
			this.listBoxChannels = new System.Windows.Forms.ListBox();
			this.buttonConfigurePatchingRule = new System.Windows.Forms.Button();
			this.comboBoxPatchingMethod = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.buttonPreview = new System.Windows.Forms.Button();
			this.listViewPreview = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonCommit = new System.Windows.Forms.Button();
			this.checkBoxApplyFilterTemplate = new System.Windows.Forms.CheckBox();
			this.comboBoxFilterTemplate = new System.Windows.Forms.ComboBox();
			this.buttonCreateFilterTemplate = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(18, 27);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(123, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Channels to be patched:";
			// 
			// listBoxChannels
			// 
			this.listBoxChannels.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listBoxChannels.FormattingEnabled = true;
			this.listBoxChannels.Location = new System.Drawing.Point(21, 53);
			this.listBoxChannels.Name = "listBoxChannels";
			this.listBoxChannels.Size = new System.Drawing.Size(161, 264);
			this.listBoxChannels.TabIndex = 1;
			// 
			// buttonConfigurePatchingRule
			// 
			this.buttonConfigurePatchingRule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonConfigurePatchingRule.Location = new System.Drawing.Point(502, 25);
			this.buttonConfigurePatchingRule.Name = "buttonConfigurePatchingRule";
			this.buttonConfigurePatchingRule.Size = new System.Drawing.Size(75, 23);
			this.buttonConfigurePatchingRule.TabIndex = 10;
			this.buttonConfigurePatchingRule.Text = "Configure";
			this.buttonConfigurePatchingRule.UseVisualStyleBackColor = true;
			this.buttonConfigurePatchingRule.Click += new System.EventHandler(this.buttonConfigurePatchingRule_Click);
			// 
			// comboBoxPatchingMethod
			// 
			this.comboBoxPatchingMethod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxPatchingMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxPatchingMethod.FormattingEnabled = true;
			this.comboBoxPatchingMethod.Location = new System.Drawing.Point(221, 53);
			this.comboBoxPatchingMethod.Name = "comboBoxPatchingMethod";
			this.comboBoxPatchingMethod.Size = new System.Drawing.Size(356, 21);
			this.comboBoxPatchingMethod.TabIndex = 9;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(216, 30);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(126, 13);
			this.label3.TabIndex = 8;
			this.label3.Text = "Patch them according to:";
			// 
			// buttonPreview
			// 
			this.buttonPreview.Location = new System.Drawing.Point(219, 144);
			this.buttonPreview.Name = "buttonPreview";
			this.buttonPreview.Size = new System.Drawing.Size(75, 23);
			this.buttonPreview.TabIndex = 11;
			this.buttonPreview.Text = "Preview";
			this.buttonPreview.UseVisualStyleBackColor = true;
			this.buttonPreview.Click += new System.EventHandler(this.buttonPreview_Click);
			// 
			// listViewPreview
			// 
			this.listViewPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewPreview.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.listViewPreview.FullRowSelect = true;
			this.listViewPreview.Location = new System.Drawing.Point(221, 173);
			this.listViewPreview.Name = "listViewPreview";
			this.listViewPreview.Size = new System.Drawing.Size(356, 144);
			this.listViewPreview.TabIndex = 12;
			this.listViewPreview.UseCompatibleStateImageBehavior = false;
			this.listViewPreview.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Channel";
			this.columnHeader1.Width = 147;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Will Patch To";
			this.columnHeader2.Width = 180;
			// 
			// buttonCommit
			// 
			this.buttonCommit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCommit.Location = new System.Drawing.Point(502, 329);
			this.buttonCommit.Name = "buttonCommit";
			this.buttonCommit.Size = new System.Drawing.Size(75, 23);
			this.buttonCommit.TabIndex = 13;
			this.buttonCommit.Text = "Commit";
			this.buttonCommit.UseVisualStyleBackColor = true;
			this.buttonCommit.Click += new System.EventHandler(this.buttonCommit_Click);
			// 
			// checkBoxApplyFilterTemplate
			// 
			this.checkBoxApplyFilterTemplate.AutoSize = true;
			this.checkBoxApplyFilterTemplate.Location = new System.Drawing.Point(219, 84);
			this.checkBoxApplyFilterTemplate.Name = "checkBoxApplyFilterTemplate";
			this.checkBoxApplyFilterTemplate.Size = new System.Drawing.Size(117, 17);
			this.checkBoxApplyFilterTemplate.TabIndex = 14;
			this.checkBoxApplyFilterTemplate.Text = "Apply filter template";
			this.checkBoxApplyFilterTemplate.UseVisualStyleBackColor = true;
			this.checkBoxApplyFilterTemplate.CheckedChanged += new System.EventHandler(this.checkBoxApplyFilterTemplate_CheckedChanged);
			// 
			// comboBoxFilterTemplate
			// 
			this.comboBoxFilterTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxFilterTemplate.Enabled = false;
			this.comboBoxFilterTemplate.FormattingEnabled = true;
			this.comboBoxFilterTemplate.Location = new System.Drawing.Point(342, 82);
			this.comboBoxFilterTemplate.Name = "comboBoxFilterTemplate";
			this.comboBoxFilterTemplate.Size = new System.Drawing.Size(155, 21);
			this.comboBoxFilterTemplate.TabIndex = 15;
			// 
			// buttonCreateFilterTemplate
			// 
			this.buttonCreateFilterTemplate.Enabled = false;
			this.buttonCreateFilterTemplate.Location = new System.Drawing.Point(503, 80);
			this.buttonCreateFilterTemplate.Name = "buttonCreateFilterTemplate";
			this.buttonCreateFilterTemplate.Size = new System.Drawing.Size(52, 23);
			this.buttonCreateFilterTemplate.TabIndex = 16;
			this.buttonCreateFilterTemplate.Text = "New";
			this.buttonCreateFilterTemplate.UseVisualStyleBackColor = true;
			this.buttonCreateFilterTemplate.Click += new System.EventHandler(this.buttonCreateFilterTemplate_Click);
			// 
			// PatchChannels
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(623, 358);
			this.Controls.Add(this.buttonCreateFilterTemplate);
			this.Controls.Add(this.comboBoxFilterTemplate);
			this.Controls.Add(this.checkBoxApplyFilterTemplate);
			this.Controls.Add(this.buttonCommit);
			this.Controls.Add(this.listViewPreview);
			this.Controls.Add(this.buttonPreview);
			this.Controls.Add(this.buttonConfigurePatchingRule);
			this.Controls.Add(this.comboBoxPatchingMethod);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.listBoxChannels);
			this.Controls.Add(this.label1);
			this.Name = "PatchChannels";
			this.Text = "PatchChannels";
			this.Load += new System.EventHandler(this.PatchChannels_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListBox listBoxChannels;
		private System.Windows.Forms.Button buttonConfigurePatchingRule;
		private System.Windows.Forms.ComboBox comboBoxPatchingMethod;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button buttonPreview;
		private System.Windows.Forms.ListView listViewPreview;
		private System.Windows.Forms.Button buttonCommit;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.CheckBox checkBoxApplyFilterTemplate;
		private System.Windows.Forms.ComboBox comboBoxFilterTemplate;
		private System.Windows.Forms.Button buttonCreateFilterTemplate;
	}
}