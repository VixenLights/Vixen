namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class FindEffectForm
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
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.comboBoxAvailableEffect = new System.Windows.Forms.ComboBox();
			this.toolTipFindEffects = new System.Windows.Forms.ToolTip(this.components);
			this.checkBoxCollapseAllGroups = new System.Windows.Forms.CheckBox();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.findAllSelectedEffectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.comboBoxFind = new System.Windows.Forms.ComboBox();
			this.listViewEffectStartTime = new Common.Controls.ListViewEx();
			this.elementHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.startTimeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.LayerEffectHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// linkLabel1
			// 
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Location = new System.Drawing.Point(55, 358);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(0, 17);
			this.linkLabel1.TabIndex = 4;
			// 
			// comboBoxAvailableEffect
			// 
			this.comboBoxAvailableEffect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxAvailableEffect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxAvailableEffect.FormattingEnabled = true;
			this.comboBoxAvailableEffect.Location = new System.Drawing.Point(14, 49);
			this.comboBoxAvailableEffect.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.comboBoxAvailableEffect.MaxDropDownItems = 20;
			this.comboBoxAvailableEffect.Name = "comboBoxAvailableEffect";
			this.comboBoxAvailableEffect.Size = new System.Drawing.Size(329, 24);
			this.comboBoxAvailableEffect.TabIndex = 12;
			this.comboBoxAvailableEffect.SelectedIndexChanged += new System.EventHandler(this.comboBoxAvailableEffect_SelectedIndexChanged);
			this.comboBoxAvailableEffect.Click += new System.EventHandler(this.comboBoxAvailableEffect_Click);
			// 
			// checkBoxCollapseAllGroups
			// 
			this.checkBoxCollapseAllGroups.AutoSize = true;
			this.checkBoxCollapseAllGroups.Checked = true;
			this.checkBoxCollapseAllGroups.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxCollapseAllGroups.Location = new System.Drawing.Point(188, 11);
			this.checkBoxCollapseAllGroups.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.checkBoxCollapseAllGroups.Name = "checkBoxCollapseAllGroups";
			this.checkBoxCollapseAllGroups.Size = new System.Drawing.Size(153, 21);
			this.checkBoxCollapseAllGroups.TabIndex = 20;
			this.checkBoxCollapseAllGroups.Text = "Collapse all Groups";
			this.toolTipFindEffects.SetToolTip(this.checkBoxCollapseAllGroups, "When enabled will collapse all groups prior to finding selected Effects.");
			this.checkBoxCollapseAllGroups.UseVisualStyleBackColor = true;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findAllSelectedEffectsToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(234, 28);
			// 
			// findAllSelectedEffectsToolStripMenuItem
			// 
			this.findAllSelectedEffectsToolStripMenuItem.Name = "findAllSelectedEffectsToolStripMenuItem";
			this.findAllSelectedEffectsToolStripMenuItem.Size = new System.Drawing.Size(233, 24);
			this.findAllSelectedEffectsToolStripMenuItem.Text = "Find all selected effects";
			this.findAllSelectedEffectsToolStripMenuItem.Click += new System.EventHandler(this.findAllSelectedEffectsToolStripMenuItem_Click);
			// 
			// comboBoxFind
			// 
			this.comboBoxFind.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxFind.FormattingEnabled = true;
			this.comboBoxFind.Items.AddRange(new object[] {
            "Effects",
            "Layers"});
			this.comboBoxFind.Location = new System.Drawing.Point(15, 10);
			this.comboBoxFind.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.comboBoxFind.MaxDropDownItems = 20;
			this.comboBoxFind.Name = "comboBoxFind";
			this.comboBoxFind.Size = new System.Drawing.Size(140, 24);
			this.comboBoxFind.TabIndex = 21;
			this.comboBoxFind.SelectedIndexChanged += new System.EventHandler(this.comboBoxFind_SelectedIndexChanged);
			// 
			// listViewEffectStartTime
			// 
			this.listViewEffectStartTime.AllowDrop = true;
			this.listViewEffectStartTime.AllowRowReorder = false;
			this.listViewEffectStartTime.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewEffectStartTime.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.elementHeader,
            this.startTimeHeader,
            this.LayerEffectHeader});
			this.listViewEffectStartTime.ContextMenuStrip = this.contextMenuStrip1;
			this.listViewEffectStartTime.FullRowSelect = true;
			this.listViewEffectStartTime.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewEffectStartTime.Location = new System.Drawing.Point(0, 89);
			this.listViewEffectStartTime.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.listViewEffectStartTime.Name = "listViewEffectStartTime";
			this.listViewEffectStartTime.OwnerDraw = true;
			this.listViewEffectStartTime.Size = new System.Drawing.Size(352, 352);
			this.listViewEffectStartTime.TabIndex = 14;
			this.listViewEffectStartTime.UseCompatibleStateImageBehavior = false;
			this.listViewEffectStartTime.View = System.Windows.Forms.View.Details;
			this.listViewEffectStartTime.DoubleClick += new System.EventHandler(this.listViewEffectStartTime_DoubleClick);
			this.listViewEffectStartTime.MouseEnter += new System.EventHandler(this.listViewEffectStartTime_UpdateListView);
			this.listViewEffectStartTime.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listViewEffectStartTime_MouseUp);
			// 
			// elementHeader
			// 
			this.elementHeader.Text = "Element";
			this.elementHeader.Width = 104;
			// 
			// startTimeHeader
			// 
			this.startTimeHeader.Text = "Effect Start Time";
			this.startTimeHeader.Width = 160;
			// 
			// LayerEffectHeader
			// 
			this.LayerEffectHeader.Text = "Layer";
			this.LayerEffectHeader.Width = 84;
			// 
			// FindEffectForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.ClientSize = new System.Drawing.Size(355, 442);
			this.ControlBox = false;
			this.Controls.Add(this.comboBoxFind);
			this.Controls.Add(this.checkBoxCollapseAllGroups);
			this.Controls.Add(this.listViewEffectStartTime);
			this.Controls.Add(this.comboBoxAvailableEffect);
			this.Controls.Add(this.linkLabel1);
			this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "FindEffectForm";
			this.Text = "Find Effects/Layers";
			this.Resize += new System.EventHandler(this.FindEffectForm_Resize);
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.ComboBox comboBoxAvailableEffect;
		private Common.Controls.ListViewEx listViewEffectStartTime;
		private System.Windows.Forms.ColumnHeader elementHeader;
		private System.Windows.Forms.ColumnHeader startTimeHeader;
		private System.Windows.Forms.ToolTip toolTipFindEffects;
		private System.Windows.Forms.CheckBox checkBoxCollapseAllGroups;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem findAllSelectedEffectsToolStripMenuItem;
		private System.Windows.Forms.ColumnHeader LayerEffectHeader;
		private System.Windows.Forms.ComboBox comboBoxFind;

	}
}