namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class MixingFilterEditor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MixingFilterEditor));
			this.cboListViewCombo = new System.Windows.Forms.ComboBox();
			this.txtListViewEdit = new System.Windows.Forms.TextBox();
			this.lvMixingFilters = new Common.Controls.ListViewEx();
			this._layerNameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this._mixingTypeColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.toolStripEx1 = new Common.Controls.ToolStripEx();
			this.toolStripButtonAddLayer = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonRemoveLayer = new System.Windows.Forms.ToolStripButton();
			this.toolStripEx1.SuspendLayout();
			this.SuspendLayout();
			// 
			// cboListViewCombo
			// 
			this.cboListViewCombo.FormattingEnabled = true;
			this.cboListViewCombo.Location = new System.Drawing.Point(48, 317);
			this.cboListViewCombo.Name = "cboListViewCombo";
			this.cboListViewCombo.Size = new System.Drawing.Size(140, 23);
			this.cboListViewCombo.TabIndex = 3;
			this.cboListViewCombo.Visible = false;
			this.cboListViewCombo.SelectedValueChanged += new System.EventHandler(this.cboListViewCombo_SelectedValueChanged);
			this.cboListViewCombo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cboListViewCombo_KeyPress);
			this.cboListViewCombo.Leave += new System.EventHandler(this.cboListViewCombo_Leave);
			// 
			// txtListViewEdit
			// 
			this.txtListViewEdit.Location = new System.Drawing.Point(198, 309);
			this.txtListViewEdit.Name = "txtListViewEdit";
			this.txtListViewEdit.Size = new System.Drawing.Size(100, 23);
			this.txtListViewEdit.TabIndex = 4;
			this.txtListViewEdit.Visible = false;
			this.txtListViewEdit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtListViewEdit_KeyPress);
			this.txtListViewEdit.Leave += new System.EventHandler(this.txtListViewEdit_Leave);
			// 
			// lvMixingFilters
			// 
			this.lvMixingFilters.AllowDrop = true;
			this.lvMixingFilters.AllowRowReorder = true;
			this.lvMixingFilters.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lvMixingFilters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._layerNameColumn,
            this._mixingTypeColumn});
			this.lvMixingFilters.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvMixingFilters.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lvMixingFilters.Location = new System.Drawing.Point(0, 25);
			this.lvMixingFilters.MultiSelect = false;
			this.lvMixingFilters.Name = "lvMixingFilters";
			this.lvMixingFilters.OwnerDraw = true;
			this.lvMixingFilters.Size = new System.Drawing.Size(283, 302);
			this.lvMixingFilters.TabIndex = 2;
			this.lvMixingFilters.UseCompatibleStateImageBehavior = false;
			this.lvMixingFilters.View = System.Windows.Forms.View.Details;
			// 
			// _layerNameColumn
			// 
			this._layerNameColumn.Text = "Layer Name";
			// 
			// _mixingTypeColumn
			// 
			this._mixingTypeColumn.Text = "Mixing Type";
			this._mixingTypeColumn.Width = 223;
			// 
			// toolStripEx1
			// 
			this.toolStripEx1.ClickThrough = true;
			this.toolStripEx1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAddLayer,
            this.toolStripButtonRemoveLayer});
			this.toolStripEx1.Location = new System.Drawing.Point(0, 0);
			this.toolStripEx1.Name = "toolStripEx1";
			this.toolStripEx1.Size = new System.Drawing.Size(283, 25);
			this.toolStripEx1.TabIndex = 1;
			this.toolStripEx1.Text = "toolStripEx1";
			// 
			// toolStripButtonAddLayer
			// 
			this.toolStripButtonAddLayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonAddLayer.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAddLayer.Image")));
			this.toolStripButtonAddLayer.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonAddLayer.Name = "toolStripButtonAddLayer";
			this.toolStripButtonAddLayer.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonAddLayer.Text = "Add Layer";
			this.toolStripButtonAddLayer.Click += new System.EventHandler(this.toolStripButtonAddLayer_Click);
			// 
			// toolStripButtonRemoveLayer
			// 
			this.toolStripButtonRemoveLayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonRemoveLayer.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRemoveLayer.Image")));
			this.toolStripButtonRemoveLayer.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonRemoveLayer.Name = "toolStripButtonRemoveLayer";
			this.toolStripButtonRemoveLayer.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonRemoveLayer.Text = "Remove Layer";
			this.toolStripButtonRemoveLayer.Click += new System.EventHandler(this.toolStripButtonRemoveLayer_Click);
			// 
			// MixingFilterEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(283, 327);
			this.Controls.Add(this.txtListViewEdit);
			this.Controls.Add(this.cboListViewCombo);
			this.Controls.Add(this.lvMixingFilters);
			this.Controls.Add(this.toolStripEx1);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "MixingFilterEditor";
			this.Text = "Mixing Filter Editor";
			this.Load += new System.EventHandler(this.MixingFilterEditor_Load);
			this.toolStripEx1.ResumeLayout(false);
			this.toolStripEx1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private Common.Controls.ToolStripEx toolStripEx1;
		private System.Windows.Forms.ToolStripButton toolStripButtonAddLayer;
		private Common.Controls.ListViewEx lvMixingFilters;
		private System.Windows.Forms.ComboBox cboListViewCombo;
		private System.Windows.Forms.ColumnHeader _layerNameColumn;
		private System.Windows.Forms.ColumnHeader _mixingTypeColumn;
		private System.Windows.Forms.ToolStripButton toolStripButtonRemoveLayer;
		private System.Windows.Forms.TextBox txtListViewEdit;
	}
}