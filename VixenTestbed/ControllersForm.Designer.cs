namespace VixenTestbed {
	partial class ControllersForm {
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
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.buttonDeleteController = new System.Windows.Forms.Button();
			this.comboBoxOutputModule = new System.Windows.Forms.ComboBox();
			this.numericUpDownOutputCount = new System.Windows.Forms.NumericUpDown();
			this.buttonControllerSetup = new System.Windows.Forms.Button();
			this.buttonLinkController = new System.Windows.Forms.Button();
			this.buttonRemoveControllerLink = new System.Windows.Forms.Button();
			this.comboBoxLinkedTo = new System.Windows.Forms.ComboBox();
			this.label11 = new System.Windows.Forms.Label();
			this.buttonUpdateController = new System.Windows.Forms.Button();
			this.listViewControllers = new System.Windows.Forms.ListView();
			this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonAddController = new System.Windows.Forms.Button();
			this.textBoxControllerName = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.buttonDone = new System.Windows.Forms.Button();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownOutputCount)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.buttonDeleteController);
			this.groupBox2.Controls.Add(this.comboBoxOutputModule);
			this.groupBox2.Controls.Add(this.numericUpDownOutputCount);
			this.groupBox2.Controls.Add(this.buttonControllerSetup);
			this.groupBox2.Controls.Add(this.buttonLinkController);
			this.groupBox2.Controls.Add(this.buttonRemoveControllerLink);
			this.groupBox2.Controls.Add(this.comboBoxLinkedTo);
			this.groupBox2.Controls.Add(this.label11);
			this.groupBox2.Controls.Add(this.buttonUpdateController);
			this.groupBox2.Controls.Add(this.listViewControllers);
			this.groupBox2.Controls.Add(this.buttonAddController);
			this.groupBox2.Controls.Add(this.textBoxControllerName);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Location = new System.Drawing.Point(12, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(470, 358);
			this.groupBox2.TabIndex = 0;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Controller";
			// 
			// label9
			// 
			this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(38, 239);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(79, 13);
			this.label9.TabIndex = 5;
			this.label9.Text = "Output module:";
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(45, 212);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(72, 13);
			this.label8.TabIndex = 3;
			this.label8.Text = "Output count:";
			// 
			// buttonDeleteController
			// 
			this.buttonDeleteController.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonDeleteController.Location = new System.Drawing.Point(288, 320);
			this.buttonDeleteController.Name = "buttonDeleteController";
			this.buttonDeleteController.Size = new System.Drawing.Size(75, 23);
			this.buttonDeleteController.TabIndex = 14;
			this.buttonDeleteController.Text = "Delete";
			this.buttonDeleteController.UseVisualStyleBackColor = true;
			this.buttonDeleteController.Click += new System.EventHandler(this.buttonDeleteController_Click);
			// 
			// comboBoxOutputModule
			// 
			this.comboBoxOutputModule.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxOutputModule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxOutputModule.FormattingEnabled = true;
			this.comboBoxOutputModule.Location = new System.Drawing.Point(126, 236);
			this.comboBoxOutputModule.Name = "comboBoxOutputModule";
			this.comboBoxOutputModule.Size = new System.Drawing.Size(216, 21);
			this.comboBoxOutputModule.TabIndex = 6;
			// 
			// numericUpDownOutputCount
			// 
			this.numericUpDownOutputCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.numericUpDownOutputCount.Location = new System.Drawing.Point(126, 210);
			this.numericUpDownOutputCount.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
			this.numericUpDownOutputCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownOutputCount.Name = "numericUpDownOutputCount";
			this.numericUpDownOutputCount.Size = new System.Drawing.Size(58, 20);
			this.numericUpDownOutputCount.TabIndex = 4;
			this.numericUpDownOutputCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// buttonControllerSetup
			// 
			this.buttonControllerSetup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonControllerSetup.Enabled = false;
			this.buttonControllerSetup.Location = new System.Drawing.Point(6, 320);
			this.buttonControllerSetup.Name = "buttonControllerSetup";
			this.buttonControllerSetup.Size = new System.Drawing.Size(75, 23);
			this.buttonControllerSetup.TabIndex = 11;
			this.buttonControllerSetup.Text = "Setup";
			this.buttonControllerSetup.UseVisualStyleBackColor = true;
			this.buttonControllerSetup.Click += new System.EventHandler(this.buttonControllerSetup_Click);
			// 
			// buttonLinkController
			// 
			this.buttonLinkController.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonLinkController.Location = new System.Drawing.Point(308, 288);
			this.buttonLinkController.Name = "buttonLinkController";
			this.buttonLinkController.Size = new System.Drawing.Size(75, 23);
			this.buttonLinkController.TabIndex = 9;
			this.buttonLinkController.Text = "Set";
			this.buttonLinkController.UseVisualStyleBackColor = true;
			this.buttonLinkController.Visible = false;
			this.buttonLinkController.Click += new System.EventHandler(this.buttonLinkController_Click);
			// 
			// buttonRemoveControllerLink
			// 
			this.buttonRemoveControllerLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRemoveControllerLink.Location = new System.Drawing.Point(389, 288);
			this.buttonRemoveControllerLink.Name = "buttonRemoveControllerLink";
			this.buttonRemoveControllerLink.Size = new System.Drawing.Size(75, 23);
			this.buttonRemoveControllerLink.TabIndex = 10;
			this.buttonRemoveControllerLink.Text = "Remove";
			this.buttonRemoveControllerLink.UseVisualStyleBackColor = true;
			this.buttonRemoveControllerLink.Visible = false;
			this.buttonRemoveControllerLink.Click += new System.EventHandler(this.buttonRemoveControllerLink_Click);
			// 
			// comboBoxLinkedTo
			// 
			this.comboBoxLinkedTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxLinkedTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxLinkedTo.FormattingEnabled = true;
			this.comboBoxLinkedTo.Location = new System.Drawing.Point(126, 290);
			this.comboBoxLinkedTo.Name = "comboBoxLinkedTo";
			this.comboBoxLinkedTo.Size = new System.Drawing.Size(174, 21);
			this.comboBoxLinkedTo.TabIndex = 8;
			this.comboBoxLinkedTo.Visible = false;
			// 
			// label11
			// 
			this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(63, 293);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(54, 13);
			this.label11.TabIndex = 7;
			this.label11.Text = "Linked to:";
			this.label11.Visible = false;
			// 
			// buttonUpdateController
			// 
			this.buttonUpdateController.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonUpdateController.Location = new System.Drawing.Point(207, 320);
			this.buttonUpdateController.Name = "buttonUpdateController";
			this.buttonUpdateController.Size = new System.Drawing.Size(75, 23);
			this.buttonUpdateController.TabIndex = 13;
			this.buttonUpdateController.Text = "Update";
			this.buttonUpdateController.UseVisualStyleBackColor = true;
			this.buttonUpdateController.Click += new System.EventHandler(this.buttonUpdateController_Click);
			// 
			// listViewControllers
			// 
			this.listViewControllers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listViewControllers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
			this.listViewControllers.FullRowSelect = true;
			this.listViewControllers.Location = new System.Drawing.Point(20, 19);
			this.listViewControllers.MultiSelect = false;
			this.listViewControllers.Name = "listViewControllers";
			this.listViewControllers.Size = new System.Drawing.Size(432, 159);
			this.listViewControllers.TabIndex = 0;
			this.listViewControllers.TabStop = false;
			this.listViewControllers.UseCompatibleStateImageBehavior = false;
			this.listViewControllers.View = System.Windows.Forms.View.Details;
			this.listViewControllers.SelectedIndexChanged += new System.EventHandler(this.listViewControllers_SelectedIndexChanged);
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Name";
			this.columnHeader4.Width = 173;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Outputs";
			this.columnHeader5.Width = 63;
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "Output module";
			this.columnHeader6.Width = 176;
			// 
			// buttonAddController
			// 
			this.buttonAddController.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonAddController.Location = new System.Drawing.Point(126, 320);
			this.buttonAddController.Name = "buttonAddController";
			this.buttonAddController.Size = new System.Drawing.Size(75, 23);
			this.buttonAddController.TabIndex = 12;
			this.buttonAddController.Text = "Add";
			this.buttonAddController.UseVisualStyleBackColor = true;
			this.buttonAddController.Click += new System.EventHandler(this.buttonAddController_Click);
			// 
			// textBoxControllerName
			// 
			this.textBoxControllerName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxControllerName.Location = new System.Drawing.Point(126, 184);
			this.textBoxControllerName.Name = "textBoxControllerName";
			this.textBoxControllerName.Size = new System.Drawing.Size(326, 20);
			this.textBoxControllerName.TabIndex = 2;
			// 
			// label10
			// 
			this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(79, 187);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(38, 13);
			this.label10.TabIndex = 1;
			this.label10.Text = "Name:";
			// 
			// buttonDone
			// 
			this.buttonDone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDone.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonDone.Location = new System.Drawing.Point(407, 376);
			this.buttonDone.Name = "buttonDone";
			this.buttonDone.Size = new System.Drawing.Size(75, 23);
			this.buttonDone.TabIndex = 1;
			this.buttonDone.Text = "Done";
			this.buttonDone.UseVisualStyleBackColor = true;
			// 
			// ControllersForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonDone;
			this.ClientSize = new System.Drawing.Size(494, 411);
			this.Controls.Add(this.buttonDone);
			this.Controls.Add(this.groupBox2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ControllersForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Controllers";
			this.Load += new System.EventHandler(this.ControllersForm_Load);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownOutputCount)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button buttonDeleteController;
		private System.Windows.Forms.ComboBox comboBoxOutputModule;
		private System.Windows.Forms.NumericUpDown numericUpDownOutputCount;
		private System.Windows.Forms.Button buttonControllerSetup;
		private System.Windows.Forms.Button buttonLinkController;
		private System.Windows.Forms.Button buttonRemoveControllerLink;
		private System.Windows.Forms.ComboBox comboBoxLinkedTo;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Button buttonUpdateController;
		private System.Windows.Forms.ListView listViewControllers;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.Button buttonAddController;
		private System.Windows.Forms.TextBox textBoxControllerName;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Button buttonDone;
	}
}