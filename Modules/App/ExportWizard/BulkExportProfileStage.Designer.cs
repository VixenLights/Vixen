namespace VixenModules.App.ExportWizard
{
	partial class BulkExportProfileStage
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
			this.btnDeleteProfile = new System.Windows.Forms.Button();
			this.btnAddProfile = new System.Windows.Forms.Button();
			this.lblSelectProfile = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.lblChooseProfile = new System.Windows.Forms.Label();
			this.comboProfiles = new System.Windows.Forms.ComboBox();
			this.btnRename = new System.Windows.Forms.Button();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonDeleteProfile
			// 
			this.btnDeleteProfile.Location = new System.Drawing.Point(499, 38);
			this.btnDeleteProfile.Name = "btnDeleteProfile";
			this.btnDeleteProfile.Size = new System.Drawing.Size(27, 27);
			this.btnDeleteProfile.TabIndex = 10;
			this.btnDeleteProfile.Text = "-";
			this.btnDeleteProfile.UseVisualStyleBackColor = true;
			this.btnDeleteProfile.Click += new System.EventHandler(this.buttonDeleteProfile_Click);
			// 
			// buttonAddProfile
			// 
			this.btnAddProfile.Location = new System.Drawing.Point(466, 38);
			this.btnAddProfile.Name = "btnAddProfile";
			this.btnAddProfile.Size = new System.Drawing.Size(27, 27);
			this.btnAddProfile.TabIndex = 9;
			this.btnAddProfile.Text = "+";
			this.btnAddProfile.UseVisualStyleBackColor = true;
			this.btnAddProfile.Click += new System.EventHandler(this.buttonAddProfile_Click);
			// 
			// lblSelectProfile
			// 
			this.lblSelectProfile.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblSelectProfile.AutoSize = true;
			this.lblSelectProfile.Location = new System.Drawing.Point(3, 44);
			this.lblSelectProfile.Name = "lblSelectProfile";
			this.lblSelectProfile.Size = new System.Drawing.Size(44, 15);
			this.lblSelectProfile.TabIndex = 8;
			this.lblSelectProfile.Text = "Profile:";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 5;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.lblChooseProfile, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.lblSelectProfile, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.btnAddProfile, 2, 2);
			this.tableLayoutPanel1.Controls.Add(this.btnDeleteProfile, 3, 2);
			this.tableLayoutPanel1.Controls.Add(this.comboProfiles, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.btnRename, 4, 2);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 4;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(562, 382);
			this.tableLayoutPanel1.TabIndex = 13;
			// 
			// lblChooseProfile
			// 
			this.lblChooseProfile.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.lblChooseProfile, 4);
			this.lblChooseProfile.Location = new System.Drawing.Point(3, 0);
			this.lblChooseProfile.Name = "lblChooseProfile";
			this.lblChooseProfile.Size = new System.Drawing.Size(116, 15);
			this.lblChooseProfile.TabIndex = 13;
			this.lblChooseProfile.Text = "Step 1:  Select profile";
			// 
			// comboProfiles
			// 
			this.comboProfiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboProfiles.FormattingEnabled = true;
			this.comboProfiles.Location = new System.Drawing.Point(53, 38);
			this.comboProfiles.Name = "comboProfiles";
			this.comboProfiles.Size = new System.Drawing.Size(407, 23);
			this.comboProfiles.TabIndex = 14;
			this.comboProfiles.SelectedIndexChanged += new System.EventHandler(this.comboProfiles_SelectedIndexChanged);
			// 
			// btnRename
			// 
			this.btnRename.Location = new System.Drawing.Point(532, 38);
			this.btnRename.Name = "btnRename";
			this.btnRename.Size = new System.Drawing.Size(27, 27);
			this.btnRename.TabIndex = 15;
			this.btnRename.UseVisualStyleBackColor = true;
			this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
			// 
			// BulkExportProfileStage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "BulkExportProfileStage";
			this.Size = new System.Drawing.Size(568, 385);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Button btnDeleteProfile;
		private System.Windows.Forms.Button btnAddProfile;
		private System.Windows.Forms.Label lblSelectProfile;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label lblChooseProfile;
		private System.Windows.Forms.ComboBox comboProfiles;
		private System.Windows.Forms.Button btnRename;
	}
}
