namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class Form_Marks
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Marks));
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonEditMarkCollection = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonAddMarkCollection = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonDeleteMarkCollection = new System.Windows.Forms.ToolStripButton();
			this.listViewMarkCollections = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonEditMarkCollection,
            this.toolStripButtonAddMarkCollection,
            this.toolStripButtonDeleteMarkCollection});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(256, 25);
			this.toolStrip1.TabIndex = 4;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButtonEditMarkCollection
			// 
			this.toolStripButtonEditMarkCollection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonEditMarkCollection.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonEditMarkCollection.Image")));
			this.toolStripButtonEditMarkCollection.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonEditMarkCollection.Name = "toolStripButtonEditMarkCollection";
			this.toolStripButtonEditMarkCollection.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonEditMarkCollection.Text = "toolStripButtonEditMarkCollections";
			this.toolStripButtonEditMarkCollection.ToolTipText = "Edit Mark Collections";
			this.toolStripButtonEditMarkCollection.Click += new System.EventHandler(this.toolStripButtonEditMarkCollection_Click);
			// 
			// toolStripButtonAddMarkCollection
			// 
			this.toolStripButtonAddMarkCollection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonAddMarkCollection.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAddMarkCollection.Image")));
			this.toolStripButtonAddMarkCollection.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonAddMarkCollection.Name = "toolStripButtonAddMarkCollection";
			this.toolStripButtonAddMarkCollection.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonAddMarkCollection.ToolTipText = "New Mark Collection";
			this.toolStripButtonAddMarkCollection.Click += new System.EventHandler(this.toolStripButtonAddMarkCollection_Click);
			// 
			// toolStripButtonDeleteMarkCollection
			// 
			this.toolStripButtonDeleteMarkCollection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonDeleteMarkCollection.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDeleteMarkCollection.Image")));
			this.toolStripButtonDeleteMarkCollection.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonDeleteMarkCollection.Name = "toolStripButtonDeleteMarkCollection";
			this.toolStripButtonDeleteMarkCollection.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonDeleteMarkCollection.Text = "toolStripButtonDeleteMarkCollection";
			this.toolStripButtonDeleteMarkCollection.ToolTipText = "Delete Mark Collection";
			this.toolStripButtonDeleteMarkCollection.Click += new System.EventHandler(this.toolStripButtonDeleteMarkCollection_Click);
			// 
			// listViewMarkCollections
			// 
			this.listViewMarkCollections.CheckBoxes = true;
			this.listViewMarkCollections.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.listViewMarkCollections.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewMarkCollections.FullRowSelect = true;
			this.listViewMarkCollections.HideSelection = false;
			this.listViewMarkCollections.LabelEdit = true;
			this.listViewMarkCollections.Location = new System.Drawing.Point(0, 25);
			this.listViewMarkCollections.MultiSelect = false;
			this.listViewMarkCollections.Name = "listViewMarkCollections";
			this.listViewMarkCollections.Scrollable = false;
			this.listViewMarkCollections.Size = new System.Drawing.Size(256, 377);
			this.listViewMarkCollections.TabIndex = 5;
			this.listViewMarkCollections.UseCompatibleStateImageBehavior = false;
			this.listViewMarkCollections.View = System.Windows.Forms.View.Details;
			this.listViewMarkCollections.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listViewMarkCollections_AfterLabelEdit);
			this.listViewMarkCollections.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listViewMarkCollections_ItemCheck);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Mark Collections";
			this.columnHeader1.Width = 999;
			// 
			// Form_Marks
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(256, 402);
			this.ControlBox = false;
			this.Controls.Add(this.listViewMarkCollections);
			this.Controls.Add(this.toolStrip1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Form_Marks";
			this.Text = "Marks";
			this.Load += new System.EventHandler(this.Form_Effects_Load);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ListView listViewMarkCollections;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ToolStripButton toolStripButtonEditMarkCollection;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ToolStripButton toolStripButtonAddMarkCollection;
		private System.Windows.Forms.ToolStripButton toolStripButtonDeleteMarkCollection;
	}
}