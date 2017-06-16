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
			this.label4 = new System.Windows.Forms.Label();
			this.toolTipFindEffects = new System.Windows.Forms.ToolTip(this.components);
			this.checkBoxCollapseAllGroups = new System.Windows.Forms.CheckBox();
			this.listViewEffectStartTime = new Common.Controls.ListViewEx();
			this.elementHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.startTimeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.findAllSelectedEffectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// linkLabel1
			// 
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Location = new System.Drawing.Point(41, 291);
			this.linkLabel1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(0, 13);
			this.linkLabel1.TabIndex = 4;
			// 
			// comboBoxAvailableEffect
			// 
			this.comboBoxAvailableEffect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxAvailableEffect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxAvailableEffect.FormattingEnabled = true;
			this.comboBoxAvailableEffect.Location = new System.Drawing.Point(9, 26);
			this.comboBoxAvailableEffect.Margin = new System.Windows.Forms.Padding(2);
			this.comboBoxAvailableEffect.MaxDropDownItems = 20;
			this.comboBoxAvailableEffect.Name = "comboBoxAvailableEffect";
			this.comboBoxAvailableEffect.Size = new System.Drawing.Size(248, 21);
			this.comboBoxAvailableEffect.TabIndex = 12;
			this.comboBoxAvailableEffect.SelectedIndexChanged += new System.EventHandler(this.comboBoxAvailableEffect_SelectedIndexChanged);
			this.comboBoxAvailableEffect.Click += new System.EventHandler(this.comboBoxAvailableEffect_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(9, 6);
			this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(43, 13);
			this.label4.TabIndex = 13;
			this.label4.Text = "Effects:";
			// 
			// checkBoxCollapseAllGroups
			// 
			this.checkBoxCollapseAllGroups.AutoSize = true;
			this.checkBoxCollapseAllGroups.Checked = true;
			this.checkBoxCollapseAllGroups.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxCollapseAllGroups.Location = new System.Drawing.Point(61, 5);
			this.checkBoxCollapseAllGroups.Margin = new System.Windows.Forms.Padding(2);
			this.checkBoxCollapseAllGroups.Name = "checkBoxCollapseAllGroups";
			this.checkBoxCollapseAllGroups.Size = new System.Drawing.Size(116, 17);
			this.checkBoxCollapseAllGroups.TabIndex = 20;
			this.checkBoxCollapseAllGroups.Text = "Collapse all Groups";
			this.toolTipFindEffects.SetToolTip(this.checkBoxCollapseAllGroups, "When enabled will collapse all groups prior to finding selected Effects.");
			this.checkBoxCollapseAllGroups.UseVisualStyleBackColor = true;
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
            this.startTimeHeader});
			this.listViewEffectStartTime.ContextMenuStrip = this.contextMenuStrip1;
			this.listViewEffectStartTime.FullRowSelect = true;
			this.listViewEffectStartTime.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewEffectStartTime.Location = new System.Drawing.Point(0, 58);
			this.listViewEffectStartTime.Margin = new System.Windows.Forms.Padding(2);
			this.listViewEffectStartTime.Name = "listViewEffectStartTime";
			this.listViewEffectStartTime.OwnerDraw = true;
			this.listViewEffectStartTime.Size = new System.Drawing.Size(265, 301);
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
			this.elementHeader.Width = 167;
			// 
			// startTimeHeader
			// 
			this.startTimeHeader.Text = "Effect Start Time";
			this.startTimeHeader.Width = 94;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findAllSelectedEffectsToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(197, 26);
			// 
			// findAllSelectedEffectsToolStripMenuItem
			// 
			this.findAllSelectedEffectsToolStripMenuItem.Name = "findAllSelectedEffectsToolStripMenuItem";
			this.findAllSelectedEffectsToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
			this.findAllSelectedEffectsToolStripMenuItem.Text = "Find all selected effects";
			this.findAllSelectedEffectsToolStripMenuItem.Click += new System.EventHandler(this.findAllSelectedEffectsToolStripMenuItem_Click);
			// 
			// FindEffectForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.ClientSize = new System.Drawing.Size(266, 359);
			this.ControlBox = false;
			this.Controls.Add(this.checkBoxCollapseAllGroups);
			this.Controls.Add(this.listViewEffectStartTime);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.comboBoxAvailableEffect);
			this.Controls.Add(this.linkLabel1);
			this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "FindEffectForm";
			this.Text = "Find Effects";
			this.Resize += new System.EventHandler(this.FindEffectForm_Resize);
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.ComboBox comboBoxAvailableEffect;
		private System.Windows.Forms.Label label4;
		private Common.Controls.ListViewEx listViewEffectStartTime;
		private System.Windows.Forms.ColumnHeader elementHeader;
		private System.Windows.Forms.ColumnHeader startTimeHeader;
		private System.Windows.Forms.ToolTip toolTipFindEffects;
		private System.Windows.Forms.CheckBox checkBoxCollapseAllGroups;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem findAllSelectedEffectsToolStripMenuItem;

	}
}