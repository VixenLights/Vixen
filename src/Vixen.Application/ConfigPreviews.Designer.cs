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
			this.groupBoxSelectedPreview = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonUpdate = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.buttonConfigurePreview = new System.Windows.Forms.Button();
			this.listViewPreviews = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonDeletePreview = new System.Windows.Forms.Button();
			this.buttonAddPreview = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.buttonDuplicateSelected = new System.Windows.Forms.Button();
			this.groupBoxSelectedPreview.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxSelectedPreview
			// 
			this.groupBoxSelectedPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxSelectedPreview.Controls.Add(this.label1);
			this.groupBoxSelectedPreview.Controls.Add(this.buttonUpdate);
			this.groupBoxSelectedPreview.Controls.Add(this.label2);
			this.groupBoxSelectedPreview.Controls.Add(this.textBoxName);
			this.groupBoxSelectedPreview.Controls.Add(this.buttonConfigurePreview);
			this.groupBoxSelectedPreview.Location = new System.Drawing.Point(14, 317);
			this.groupBoxSelectedPreview.Name = "groupBoxSelectedPreview";
			this.groupBoxSelectedPreview.Size = new System.Drawing.Size(455, 182);
			this.groupBoxSelectedPreview.TabIndex = 1;
			this.groupBoxSelectedPreview.TabStop = false;
			this.groupBoxSelectedPreview.Text = "Selected Preview";
			this.groupBoxSelectedPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(150, 76);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(221, 15);
			this.label1.TabIndex = 33;
			this.label1.Text = "Configure details specific to the preview.";
			// 
			// buttonUpdate
			// 
			this.buttonUpdate.Location = new System.Drawing.Point(359, 24);
			this.buttonUpdate.Name = "buttonUpdate";
			this.buttonUpdate.Size = new System.Drawing.Size(85, 29);
			this.buttonUpdate.TabIndex = 2;
			this.buttonUpdate.Text = "Update";
			this.buttonUpdate.UseVisualStyleBackColor = false;
			this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(16, 31);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(42, 15);
			this.label2.TabIndex = 26;
			this.label2.Text = "Name:";
			// 
			// textBoxName
			// 
			this.textBoxName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxName.Location = new System.Drawing.Point(68, 28);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(158, 23);
			this.textBoxName.TabIndex = 1;
			// 
			// buttonConfigurePreview
			// 
			this.buttonConfigurePreview.Location = new System.Drawing.Point(15, 69);
			this.buttonConfigurePreview.Name = "buttonConfigurePreview";
			this.buttonConfigurePreview.Size = new System.Drawing.Size(128, 29);
			this.buttonConfigurePreview.TabIndex = 0;
			this.buttonConfigurePreview.Text = "Configure Preview";
			this.buttonConfigurePreview.UseVisualStyleBackColor = false;
			this.buttonConfigurePreview.Click += new System.EventHandler(this.buttonConfigurePreview_Click);
			// 
			// listViewPreviews
			// 
			this.listViewPreviews.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewPreviews.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listViewPreviews.CheckBoxes = true;
			this.listViewPreviews.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.listViewPreviews.FullRowSelect = true;
			listViewGroup2.Header = "ListViewGroup";
			listViewGroup2.Name = "listViewGroup1";
			this.listViewPreviews.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup2});
			this.listViewPreviews.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewPreviews.HideSelection = false;
			this.listViewPreviews.Location = new System.Drawing.Point(14, 35);
			this.listViewPreviews.Name = "listViewPreviews";
			this.listViewPreviews.ShowGroups = false;
			this.listViewPreviews.Size = new System.Drawing.Size(456, 219);
			this.listViewPreviews.TabIndex = 0;
			this.listViewPreviews.UseCompatibleStateImageBehavior = false;
			this.listViewPreviews.View = System.Windows.Forms.View.Details;
			this.listViewPreviews.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listViewPreviews_ItemCheck);
			this.listViewPreviews.SelectedIndexChanged += new System.EventHandler(this.listViewPreviews_SelectedIndexChanged);
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
			this.buttonOk.Location = new System.Drawing.Point(252, 516);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(105, 29);
			this.buttonOk.TabIndex = 4;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = false;
			// 
			// buttonDeletePreview
			// 
			this.buttonDeletePreview.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonDeletePreview.Location = new System.Drawing.Point(318, 272);
			this.buttonDeletePreview.Name = "buttonDeletePreview";
			this.buttonDeletePreview.Size = new System.Drawing.Size(140, 29);
			this.buttonDeletePreview.TabIndex = 3;
			this.buttonDeletePreview.Text = "Delete Selected";
			this.buttonDeletePreview.UseVisualStyleBackColor = false;
			this.buttonDeletePreview.Click += new System.EventHandler(this.buttonDeletePreview_Click);
			// 
			// buttonAddPreview
			// 
			this.buttonAddPreview.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonAddPreview.Location = new System.Drawing.Point(17, 272);
			this.buttonAddPreview.Name = "buttonAddPreview";
			this.buttonAddPreview.Size = new System.Drawing.Size(140, 29);
			this.buttonAddPreview.TabIndex = 2;
			this.buttonAddPreview.Text = "Add New Preview";
			this.buttonAddPreview.UseVisualStyleBackColor = false;
			this.buttonAddPreview.Click += new System.EventHandler(this.buttonAddPreview_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(364, 516);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(105, 29);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = false;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(14, 14);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(39, 15);
			this.label3.TabIndex = 6;
			this.label3.Text = "Name";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(230, 14);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(31, 15);
			this.label4.TabIndex = 7;
			this.label4.Text = "Type";
			// 
			// buttonDuplicateSelected
			// 
			this.buttonDuplicateSelected.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonDuplicateSelected.Location = new System.Drawing.Point(167, 272);
			this.buttonDuplicateSelected.Name = "buttonDuplicateSelected";
			this.buttonDuplicateSelected.Size = new System.Drawing.Size(140, 29);
			this.buttonDuplicateSelected.TabIndex = 8;
			this.buttonDuplicateSelected.Text = "Duplicate Selected";
			this.buttonDuplicateSelected.UseVisualStyleBackColor = false;
			this.buttonDuplicateSelected.Click += new System.EventHandler(this.buttonDuplicateSelected_Click);
			// 
			// ConfigPreviews
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(482, 569);
			this.Controls.Add(this.buttonDuplicateSelected);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.groupBoxSelectedPreview);
			this.Controls.Add(this.listViewPreviews);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.buttonDeletePreview);
			this.Controls.Add(this.buttonAddPreview);
			this.Controls.Add(this.buttonCancel);
			this.DoubleBuffered = true;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.MinimizeBox = false;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(498, 2281);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(498, 573);
			this.Name = "ConfigPreviews";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Setup Previews";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigPreviews_FormClosing);
			this.Load += new System.EventHandler(this.ConfigPreviews_Load);
			this.groupBoxSelectedPreview.ResumeLayout(false);
			this.groupBoxSelectedPreview.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBoxSelectedPreview;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonUpdate;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxName;
		private System.Windows.Forms.Button buttonConfigurePreview;
		private System.Windows.Forms.ListView listViewPreviews;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonDeletePreview;
		private System.Windows.Forms.Button buttonAddPreview;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button buttonDuplicateSelected;
	}
}
