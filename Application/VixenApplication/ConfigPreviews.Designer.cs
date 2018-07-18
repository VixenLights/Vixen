namespace VixenApplication {
	partial class ConfigPreviews {
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
			System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("ListViewGroup", System.Windows.Forms.HorizontalAlignment.Left);
			this.groupBoxSelectedController = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonUpdate = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.buttonConfigureController = new System.Windows.Forms.Button();
			this.listViewControllers = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonDeleteController = new System.Windows.Forms.Button();
			this.buttonAddController = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBoxSelectedController.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxSelectedController
			// 
			this.groupBoxSelectedController.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxSelectedController.BackColor = System.Drawing.SystemColors.Control;
			this.groupBoxSelectedController.Controls.Add(this.label1);
			this.groupBoxSelectedController.Controls.Add(this.buttonUpdate);
			this.groupBoxSelectedController.Controls.Add(this.label2);
			this.groupBoxSelectedController.Controls.Add(this.textBoxName);
			this.groupBoxSelectedController.Controls.Add(this.buttonConfigureController);
			this.groupBoxSelectedController.ForeColor = System.Drawing.SystemColors.ControlText;
			this.groupBoxSelectedController.Location = new System.Drawing.Point(12, 275);
			this.groupBoxSelectedController.Name = "groupBoxSelectedController";
			this.groupBoxSelectedController.Size = new System.Drawing.Size(390, 158);
			this.groupBoxSelectedController.TabIndex = 1;
			this.groupBoxSelectedController.TabStop = false;
			this.groupBoxSelectedController.Text = "Selected Preview";
			this.groupBoxSelectedController.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.BackColor = System.Drawing.SystemColors.Control;
			this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label1.Location = new System.Drawing.Point(129, 66);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(197, 13);
			this.label1.TabIndex = 33;
			this.label1.Text = "Configure details specific to the preview.";
			// 
			// buttonUpdate
			// 
			this.buttonUpdate.BackColor = System.Drawing.SystemColors.Control;
			this.buttonUpdate.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonUpdate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonUpdate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonUpdate.ForeColor = System.Drawing.SystemColors.ControlText;
			this.buttonUpdate.Location = new System.Drawing.Point(308, 21);
			this.buttonUpdate.Name = "buttonUpdate";
			this.buttonUpdate.Size = new System.Drawing.Size(73, 25);
			this.buttonUpdate.TabIndex = 2;
			this.buttonUpdate.Text = "Update";
			this.buttonUpdate.UseVisualStyleBackColor = false;
			this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
			this.buttonUpdate.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonUpdate.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.BackColor = System.Drawing.SystemColors.Control;
			this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label2.Location = new System.Drawing.Point(14, 27);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(38, 13);
			this.label2.TabIndex = 26;
			this.label2.Text = "Name:";
			// 
			// textBoxName
			// 
			this.textBoxName.BackColor = System.Drawing.SystemColors.Control;
			this.textBoxName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxName.ForeColor = System.Drawing.SystemColors.ControlText;
			this.textBoxName.Location = new System.Drawing.Point(58, 24);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(136, 20);
			this.textBoxName.TabIndex = 1;
			// 
			// buttonConfigureController
			// 
			this.buttonConfigureController.BackColor = System.Drawing.SystemColors.Control;
			this.buttonConfigureController.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonConfigureController.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonConfigureController.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonConfigureController.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonConfigureController.ForeColor = System.Drawing.SystemColors.ControlText;
			this.buttonConfigureController.Location = new System.Drawing.Point(13, 60);
			this.buttonConfigureController.Name = "buttonConfigureController";
			this.buttonConfigureController.Size = new System.Drawing.Size(110, 25);
			this.buttonConfigureController.TabIndex = 0;
			this.buttonConfigureController.Text = "Configure Preview";
			this.buttonConfigureController.UseVisualStyleBackColor = false;
			this.buttonConfigureController.Click += new System.EventHandler(this.buttonConfigureController_Click);
			this.buttonConfigureController.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonConfigureController.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// listViewControllers
			// 
			this.listViewControllers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewControllers.BackColor = System.Drawing.SystemColors.Window;
			this.listViewControllers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listViewControllers.CheckBoxes = true;
			this.listViewControllers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.listViewControllers.ForeColor = System.Drawing.SystemColors.ControlText;
			this.listViewControllers.FullRowSelect = true;
			listViewGroup2.Header = "ListViewGroup";
			listViewGroup2.Name = "listViewGroup1";
			this.listViewControllers.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup2});
			this.listViewControllers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewControllers.HideSelection = false;
			this.listViewControllers.Location = new System.Drawing.Point(12, 30);
			this.listViewControllers.Name = "listViewControllers";
			this.listViewControllers.ShowGroups = false;
			this.listViewControllers.Size = new System.Drawing.Size(391, 190);
			this.listViewControllers.TabIndex = 0;
			this.listViewControllers.UseCompatibleStateImageBehavior = false;
			this.listViewControllers.View = System.Windows.Forms.View.Details;
			this.listViewControllers.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listViewControllers_ItemCheck);
			this.listViewControllers.SelectedIndexChanged += new System.EventHandler(this.listViewControllers_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Name";
			this.columnHeader1.Width = 181;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Type";
			this.columnHeader2.Width = 182;
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.BackColor = System.Drawing.Color.Transparent;
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonOk.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonOk.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonOk.ForeColor = System.Drawing.SystemColors.ControlText;
			this.buttonOk.Location = new System.Drawing.Point(216, 447);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(90, 25);
			this.buttonOk.TabIndex = 4;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = false;
			this.buttonOk.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOk.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonDeleteController
			// 
			this.buttonDeleteController.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonDeleteController.BackColor = System.Drawing.Color.Transparent;
			this.buttonDeleteController.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonDeleteController.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonDeleteController.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonDeleteController.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonDeleteController.ForeColor = System.Drawing.SystemColors.ControlText;
			this.buttonDeleteController.Location = new System.Drawing.Point(237, 236);
			this.buttonDeleteController.Name = "buttonDeleteController";
			this.buttonDeleteController.Size = new System.Drawing.Size(120, 25);
			this.buttonDeleteController.TabIndex = 3;
			this.buttonDeleteController.Text = "Delete Selected";
			this.buttonDeleteController.UseVisualStyleBackColor = false;
			this.buttonDeleteController.Click += new System.EventHandler(this.buttonDeleteController_Click);
			this.buttonDeleteController.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonDeleteController.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonAddController
			// 
			this.buttonAddController.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonAddController.BackColor = System.Drawing.Color.Transparent;
			this.buttonAddController.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.buttonAddController.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonAddController.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonAddController.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonAddController.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAddController.ForeColor = System.Drawing.SystemColors.ControlText;
			this.buttonAddController.Location = new System.Drawing.Point(62, 236);
			this.buttonAddController.Name = "buttonAddController";
			this.buttonAddController.Size = new System.Drawing.Size(120, 25);
			this.buttonAddController.TabIndex = 2;
			this.buttonAddController.Text = "Add New Preview";
			this.buttonAddController.UseVisualStyleBackColor = false;
			this.buttonAddController.Click += new System.EventHandler(this.buttonAddController_Click);
			this.buttonAddController.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonAddController.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.BackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonCancel.ForeColor = System.Drawing.SystemColors.ControlText;
			this.buttonCancel.Location = new System.Drawing.Point(312, 447);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(90, 25);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = false;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label3.Location = new System.Drawing.Point(12, 12);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(35, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Name";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label4.Location = new System.Drawing.Point(197, 12);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(31, 13);
			this.label4.TabIndex = 7;
			this.label4.Text = "Type";
			// 
			// ConfigPreviews
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(413, 493);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.groupBoxSelectedController);
			this.Controls.Add(this.listViewControllers);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.buttonDeleteController);
			this.Controls.Add(this.buttonAddController);
			this.Controls.Add(this.buttonCancel);
			this.DoubleBuffered = true;
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(429, 1982);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(429, 502);
			this.Name = "ConfigPreviews";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Setup Previews";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigPreviews_FormClosing);
			this.Load += new System.EventHandler(this.ConfigPreviews_Load);
			this.groupBoxSelectedController.ResumeLayout(false);
			this.groupBoxSelectedController.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBoxSelectedController;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonUpdate;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxName;
		private System.Windows.Forms.Button buttonConfigureController;
		private System.Windows.Forms.ListView listViewControllers;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonDeleteController;
		private System.Windows.Forms.Button buttonAddController;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
	}
}