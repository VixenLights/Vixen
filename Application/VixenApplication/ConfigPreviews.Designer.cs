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
			System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("ListViewGroup", System.Windows.Forms.HorizontalAlignment.Left);
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
			this.groupBoxSelectedController.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxSelectedController
			// 
			this.groupBoxSelectedController.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxSelectedController.Controls.Add(this.label1);
			this.groupBoxSelectedController.Controls.Add(this.buttonUpdate);
			this.groupBoxSelectedController.Controls.Add(this.label2);
			this.groupBoxSelectedController.Controls.Add(this.textBoxName);
			this.groupBoxSelectedController.Controls.Add(this.buttonConfigureController);
			this.groupBoxSelectedController.Location = new System.Drawing.Point(18, 423);
			this.groupBoxSelectedController.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBoxSelectedController.Name = "groupBoxSelectedController";
			this.groupBoxSelectedController.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBoxSelectedController.Size = new System.Drawing.Size(592, 243);
			this.groupBoxSelectedController.TabIndex = 1;
			this.groupBoxSelectedController.TabStop = false;
			this.groupBoxSelectedController.Text = "Selected Preview";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(194, 102);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(291, 20);
			this.label1.TabIndex = 33;
			this.label1.Text = "Configure details specific to the preview.";
			// 
			// buttonUpdate
			// 
			this.buttonUpdate.Location = new System.Drawing.Point(462, 32);
			this.buttonUpdate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.buttonUpdate.Name = "buttonUpdate";
			this.buttonUpdate.Size = new System.Drawing.Size(110, 38);
			this.buttonUpdate.TabIndex = 2;
			this.buttonUpdate.Text = "Update";
			this.buttonUpdate.UseVisualStyleBackColor = true;
			this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(21, 42);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(55, 20);
			this.label2.TabIndex = 26;
			this.label2.Text = "Name:";
			// 
			// textBoxName
			// 
			this.textBoxName.Location = new System.Drawing.Point(87, 37);
			this.textBoxName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(202, 26);
			this.textBoxName.TabIndex = 1;
			// 
			// buttonConfigureController
			// 
			this.buttonConfigureController.Location = new System.Drawing.Point(20, 92);
			this.buttonConfigureController.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.buttonConfigureController.Name = "buttonConfigureController";
			this.buttonConfigureController.Size = new System.Drawing.Size(165, 38);
			this.buttonConfigureController.TabIndex = 0;
			this.buttonConfigureController.Text = "Configure Preview";
			this.buttonConfigureController.UseVisualStyleBackColor = true;
			this.buttonConfigureController.Click += new System.EventHandler(this.buttonConfigureController_Click);
			// 
			// listViewControllers
			// 
			this.listViewControllers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.listViewControllers.CheckBoxes = true;
			this.listViewControllers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.columnHeader1,
			this.columnHeader2});
			this.listViewControllers.FullRowSelect = true;
			listViewGroup1.Header = "ListViewGroup";
			listViewGroup1.Name = "listViewGroup1";
			this.listViewControllers.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
			listViewGroup1});
			this.listViewControllers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewControllers.HideSelection = false;
			this.listViewControllers.Location = new System.Drawing.Point(18, 18);
			this.listViewControllers.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.listViewControllers.Name = "listViewControllers";
			this.listViewControllers.ShowGroups = false;
			this.listViewControllers.Size = new System.Drawing.Size(590, 316);
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
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(332, 685);
			this.buttonOk.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(135, 38);
			this.buttonOk.TabIndex = 4;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// buttonDeleteController
			// 
			this.buttonDeleteController.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonDeleteController.Location = new System.Drawing.Point(356, 363);
			this.buttonDeleteController.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.buttonDeleteController.Name = "buttonDeleteController";
			this.buttonDeleteController.Size = new System.Drawing.Size(180, 38);
			this.buttonDeleteController.TabIndex = 3;
			this.buttonDeleteController.Text = "Delete Selected";
			this.buttonDeleteController.UseVisualStyleBackColor = true;
			this.buttonDeleteController.Click += new System.EventHandler(this.buttonDeleteController_Click);
			// 
			// buttonAddController
			// 
			this.buttonAddController.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonAddController.Location = new System.Drawing.Point(93, 363);
			this.buttonAddController.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.buttonAddController.Name = "buttonAddController";
			this.buttonAddController.Size = new System.Drawing.Size(180, 38);
			this.buttonAddController.TabIndex = 2;
			this.buttonAddController.Text = "Add New Preview";
			this.buttonAddController.UseVisualStyleBackColor = true;
			this.buttonAddController.Click += new System.EventHandler(this.buttonAddController_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(476, 685);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(135, 38);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// ConfigPreviews
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(620, 758);
			this.Controls.Add(this.groupBoxSelectedController);
			this.Controls.Add(this.listViewControllers);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.buttonDeleteController);
			this.Controls.Add(this.buttonAddController);
			this.Controls.Add(this.buttonCancel);
			this.DoubleBuffered = true;
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(642, 3047);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(642, 770);
			this.Name = "ConfigPreviews";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Previews Configuration";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigPreviews_FormClosing);
			this.Load += new System.EventHandler(this.ConfigPreviews_Load);
			this.groupBoxSelectedController.ResumeLayout(false);
			this.groupBoxSelectedController.PerformLayout();
			this.ResumeLayout(false);

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
	}
}