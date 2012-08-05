namespace Dataweb.NShape.Designer {

	partial class OpenAdoNetRepositoryDialog {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
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
		private void InitializeComponent() {
			this.label1 = new System.Windows.Forms.Label();
			this.serverNameTextBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.projectNameLabel = new System.Windows.Forms.Label();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.providerNameComboBox = new System.Windows.Forms.ComboBox();
			this.projectNameComboBox = new System.Windows.Forms.ComboBox();
			this.databaseNameComboBox = new System.Windows.Forms.ComboBox();
			this.createDbButton = new System.Windows.Forms.Button();
			this.dropDbButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 64);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(69, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Server Name";
			// 
			// serverNameTextBox
			// 
			this.serverNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.serverNameTextBox.Location = new System.Drawing.Point(12, 80);
			this.serverNameTextBox.Name = "serverNameTextBox";
			this.serverNameTextBox.Size = new System.Drawing.Size(306, 20);
			this.serverNameTextBox.TabIndex = 1;
			this.serverNameTextBox.TextChanged += new System.EventHandler(this.serverNameTextBox_TextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(9, 115);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(84, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Database Name";
			// 
			// projectNameLabel
			// 
			this.projectNameLabel.AutoSize = true;
			this.projectNameLabel.Location = new System.Drawing.Point(9, 166);
			this.projectNameLabel.Name = "projectNameLabel";
			this.projectNameLabel.Size = new System.Drawing.Size(71, 13);
			this.projectNameLabel.TabIndex = 4;
			this.projectNameLabel.Text = "Project Name";
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(243, 227);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 6;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(162, 227);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 7;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 13);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(77, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "Provider Name";
			// 
			// providerNameComboBox
			// 
			this.providerNameComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.providerNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.providerNameComboBox.Items.AddRange(new object[] {
            "SQL Server"});
			this.providerNameComboBox.Location = new System.Drawing.Point(12, 29);
			this.providerNameComboBox.Name = "providerNameComboBox";
			this.providerNameComboBox.Size = new System.Drawing.Size(306, 21);
			this.providerNameComboBox.TabIndex = 9;
			this.providerNameComboBox.SelectedIndexChanged += new System.EventHandler(this.providerNameComboBox_SelectedIndexChanged);
			// 
			// projectNameComboBox
			// 
			this.projectNameComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.projectNameComboBox.FormattingEnabled = true;
			this.projectNameComboBox.Location = new System.Drawing.Point(12, 182);
			this.projectNameComboBox.Name = "projectNameComboBox";
			this.projectNameComboBox.Size = new System.Drawing.Size(306, 21);
			this.projectNameComboBox.TabIndex = 10;
			this.projectNameComboBox.DropDown += new System.EventHandler(this.projectNameComboBox_DropDown);
			// 
			// databaseNameComboBox
			// 
			this.databaseNameComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.databaseNameComboBox.FormattingEnabled = true;
			this.databaseNameComboBox.Location = new System.Drawing.Point(12, 131);
			this.databaseNameComboBox.Name = "databaseNameComboBox";
			this.databaseNameComboBox.Size = new System.Drawing.Size(194, 21);
			this.databaseNameComboBox.TabIndex = 11;
			this.databaseNameComboBox.SelectedIndexChanged += new System.EventHandler(this.databaseNameComboBox_TextChanged);
			this.databaseNameComboBox.DropDown += new System.EventHandler(this.databaseNameComboBox_DropDown);
			this.databaseNameComboBox.TextChanged += new System.EventHandler(this.databaseNameComboBox_TextChanged);
			// 
			// createDbButton
			// 
			this.createDbButton.Location = new System.Drawing.Point(212, 129);
			this.createDbButton.Name = "createDbButton";
			this.createDbButton.Size = new System.Drawing.Size(50, 23);
			this.createDbButton.TabIndex = 12;
			this.createDbButton.Text = "Create";
			this.createDbButton.UseVisualStyleBackColor = true;
			this.createDbButton.Click += new System.EventHandler(this.createDbButton_Click);
			// 
			// dropDbButton
			// 
			this.dropDbButton.Location = new System.Drawing.Point(268, 129);
			this.dropDbButton.Name = "dropDbButton";
			this.dropDbButton.Size = new System.Drawing.Size(50, 23);
			this.dropDbButton.TabIndex = 13;
			this.dropDbButton.Text = "Drop";
			this.dropDbButton.UseVisualStyleBackColor = true;
			this.dropDbButton.Click += new System.EventHandler(this.dropDbButton_Click);
			// 
			// OpenAdoNetRepositoryDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(330, 262);
			this.Controls.Add(this.dropDbButton);
			this.Controls.Add(this.createDbButton);
			this.Controls.Add(this.databaseNameComboBox);
			this.Controls.Add(this.projectNameComboBox);
			this.Controls.Add(this.providerNameComboBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.projectNameLabel);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.serverNameTextBox);
			this.Controls.Add(this.label1);
			this.Name = "OpenAdoNetRepositoryDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select Database Project";
			this.Load += new System.EventHandler(this.OpenAdoNetRepositoryDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox serverNameTextBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label projectNameLabel;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox providerNameComboBox;
		private System.Windows.Forms.ComboBox projectNameComboBox;
		private System.Windows.Forms.ComboBox databaseNameComboBox;
		private System.Windows.Forms.Button createDbButton;
		private System.Windows.Forms.Button dropDbButton;
	}
}