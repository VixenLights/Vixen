namespace VixenModules.App.Shows
{
	partial class ShowListForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShowListForm));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.listViewShows = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonDelete = new System.Windows.Forms.Button();
			this.buttonEdit = new System.Windows.Forms.Button();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.buttonHelp = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.listViewShows);
			this.groupBox1.Controls.Add(this.buttonDelete);
			this.groupBox1.Controls.Add(this.buttonEdit);
			this.groupBox1.Controls.Add(this.buttonAdd);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(280, 210);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// listViewShows
			// 
			this.listViewShows.AutoArrange = false;
			this.listViewShows.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.listViewShows.FullRowSelect = true;
			this.listViewShows.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewShows.HideSelection = false;
			this.listViewShows.LabelEdit = true;
			this.listViewShows.Location = new System.Drawing.Point(10, 16);
			this.listViewShows.MultiSelect = false;
			this.listViewShows.Name = "listViewShows";
			this.listViewShows.Size = new System.Drawing.Size(260, 156);
			this.listViewShows.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listViewShows.TabIndex = 0;
			this.listViewShows.UseCompatibleStateImageBehavior = false;
			this.listViewShows.View = System.Windows.Forms.View.Details;
			this.listViewShows.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listViewShows_AfterLabelEdit);
			this.listViewShows.DoubleClick += new System.EventHandler(this.listViewShows_DoubleClick);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Shows";
			this.columnHeader1.Width = 235;
			// 
			// buttonDelete
			// 
			this.buttonDelete.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonDelete.BackgroundImage")));
			this.buttonDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonDelete.Location = new System.Drawing.Point(246, 178);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(24, 24);
			this.buttonDelete.TabIndex = 14;
			this.buttonDelete.UseVisualStyleBackColor = true;
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// buttonEdit
			// 
			this.buttonEdit.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonEdit.BackgroundImage")));
			this.buttonEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonEdit.Location = new System.Drawing.Point(221, 178);
			this.buttonEdit.Name = "buttonEdit";
			this.buttonEdit.Size = new System.Drawing.Size(24, 24);
			this.buttonEdit.TabIndex = 13;
			this.buttonEdit.UseVisualStyleBackColor = true;
			this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
			// 
			// buttonAdd
			// 
			this.buttonAdd.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonAdd.BackgroundImage")));
			this.buttonAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAdd.Location = new System.Drawing.Point(196, 178);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(24, 24);
			this.buttonAdd.TabIndex = 12;
			this.buttonAdd.UseVisualStyleBackColor = true;
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// buttonHelp
			// 
			this.buttonHelp.Image = ((System.Drawing.Image)(resources.GetObject("buttonHelp.Image")));
			this.buttonHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonHelp.Location = new System.Drawing.Point(12, 235);
			this.buttonHelp.Name = "buttonHelp";
			this.buttonHelp.Size = new System.Drawing.Size(60, 23);
			this.buttonHelp.TabIndex = 63;
			this.buttonHelp.Tag = "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/nutcracker-eff" +
    "ects/";
			this.buttonHelp.Text = "Help";
			this.buttonHelp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.buttonHelp.UseVisualStyleBackColor = true;
			this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(217, 235);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 64;
			this.buttonOK.Text = "Close";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// ShowListForm
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(304, 270);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonHelp);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ShowListForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Shows";
			this.Load += new System.EventHandler(this.ShowListForm_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Button buttonEdit;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonHelp;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.ListView listViewShows;
		private System.Windows.Forms.ColumnHeader columnHeader1;
	}
}