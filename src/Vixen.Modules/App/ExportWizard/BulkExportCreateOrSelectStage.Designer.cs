namespace VixenModules.App.ExportWizard
{
	partial class BulkExportCreateOrSelectStage
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.lblStep1 = new System.Windows.Forms.Label();
			this.radioCreateNew = new System.Windows.Forms.RadioButton();
			this.radioSelectExisting = new System.Windows.Forms.RadioButton();
			this.comboProfiles = new System.Windows.Forms.ComboBox();
			this.lblSelect = new System.Windows.Forms.Label();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.lblStep1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.radioCreateNew, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.radioSelectExisting, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.comboProfiles, 1, 5);
			this.tableLayoutPanel1.Controls.Add(this.lblSelect, 0, 5);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 7;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(472, 347);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// lblStep1
			// 
			this.lblStep1.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.lblStep1, 2);
			this.lblStep1.Location = new System.Drawing.Point(3, 0);
			this.lblStep1.Name = "lblStep1";
			this.lblStep1.Size = new System.Drawing.Size(166, 15);
			this.lblStep1.TabIndex = 0;
			this.lblStep1.Text = "Step 1: Configuration settings.";
			// 
			// radioCreateNew
			// 
			this.radioCreateNew.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.radioCreateNew, 2);
			this.radioCreateNew.Location = new System.Drawing.Point(3, 38);
			this.radioCreateNew.Name = "radioCreateNew";
			this.radioCreateNew.Size = new System.Drawing.Size(195, 19);
			this.radioCreateNew.TabIndex = 1;
			this.radioCreateNew.TabStop = true;
			this.radioCreateNew.Text = "Create new export configuration";
			this.radioCreateNew.UseVisualStyleBackColor = true;
			this.radioCreateNew.CheckedChanged += new System.EventHandler(this.radioCreateNew_CheckedChanged);
			// 
			// radioSelectExisting
			// 
			this.radioSelectExisting.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.radioSelectExisting, 2);
			this.radioSelectExisting.Location = new System.Drawing.Point(3, 63);
			this.radioSelectExisting.Name = "radioSelectExisting";
			this.radioSelectExisting.Size = new System.Drawing.Size(188, 19);
			this.radioSelectExisting.TabIndex = 2;
			this.radioSelectExisting.TabStop = true;
			this.radioSelectExisting.Text = "Use saved export configuration";
			this.radioSelectExisting.UseVisualStyleBackColor = true;
			this.radioSelectExisting.CheckedChanged += new System.EventHandler(this.radioSelectExisting_CheckedChanged);
			// 
			// comboProfiles
			// 
			this.comboProfiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboProfiles.FormattingEnabled = true;
			this.comboProfiles.Location = new System.Drawing.Point(47, 108);
			this.comboProfiles.Name = "comboProfiles";
			this.comboProfiles.Size = new System.Drawing.Size(422, 23);
			this.comboProfiles.TabIndex = 15;
			this.comboProfiles.SelectedIndexChanged += new System.EventHandler(this.comboProfiles_SelectedIndexChanged);
			// 
			// lblSelect
			// 
			this.lblSelect.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.lblSelect.AutoSize = true;
			this.lblSelect.Location = new System.Drawing.Point(3, 112);
			this.lblSelect.Name = "lblSelect";
			this.lblSelect.Size = new System.Drawing.Size(38, 15);
			this.lblSelect.TabIndex = 16;
			this.lblSelect.Text = "Select";
			// 
			// BulkExportCreateOrSelectStage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "BulkExportCreateOrSelectStage";
			this.Size = new System.Drawing.Size(475, 350);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label lblStep1;
		private System.Windows.Forms.RadioButton radioCreateNew;
		private System.Windows.Forms.RadioButton radioSelectExisting;
		private System.Windows.Forms.ComboBox comboProfiles;
		private System.Windows.Forms.Label lblSelect;
	}
}
