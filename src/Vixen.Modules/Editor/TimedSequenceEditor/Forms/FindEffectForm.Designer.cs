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
			components = new System.ComponentModel.Container();
			linkLabel1 = new LinkLabel();
			comboBoxAvailableEffect = new ComboBox();
			toolTipFindEffects = new ToolTip(components);
			checkBoxCollapseAllGroups = new CheckBox();
			contextMenuStrip1 = new ContextMenuStrip(components);
			findAllSelectedEffectsToolStripMenuItem = new ToolStripMenuItem();
			comboBoxFind = new ComboBox();
			listViewEffectStartTime = new Common.Controls.ListViewEx();
			elementHeader = new ColumnHeader();
			startTimeHeader = new ColumnHeader();
			LayerEffectHeader = new ColumnHeader();
			contextMenuStrip1.SuspendLayout();
			SuspendLayout();
			// 
			// linkLabel1
			// 
			linkLabel1.AutoSize = true;
			linkLabel1.Location = new Point(48, 336);
			linkLabel1.Name = "linkLabel1";
			linkLabel1.Size = new Size(0, 15);
			linkLabel1.TabIndex = 4;
			// 
			// comboBoxAvailableEffect
			// 
			comboBoxAvailableEffect.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			comboBoxAvailableEffect.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBoxAvailableEffect.FormattingEnabled = true;
			comboBoxAvailableEffect.Location = new Point(12, 46);
			comboBoxAvailableEffect.Margin = new Padding(3, 2, 3, 2);
			comboBoxAvailableEffect.MaxDropDownItems = 20;
			comboBoxAvailableEffect.Name = "comboBoxAvailableEffect";
			comboBoxAvailableEffect.Size = new Size(288, 23);
			comboBoxAvailableEffect.TabIndex = 12;
			comboBoxAvailableEffect.SelectedIndexChanged += comboBoxAvailableEffect_SelectedIndexChanged;
			comboBoxAvailableEffect.Click += comboBoxAvailableEffect_Click;
			// 
			// checkBoxCollapseAllGroups
			// 
			checkBoxCollapseAllGroups.AutoSize = true;
			checkBoxCollapseAllGroups.Checked = true;
			checkBoxCollapseAllGroups.CheckState = CheckState.Checked;
			checkBoxCollapseAllGroups.Location = new Point(164, 10);
			checkBoxCollapseAllGroups.Margin = new Padding(3, 2, 3, 2);
			checkBoxCollapseAllGroups.Name = "checkBoxCollapseAllGroups";
			checkBoxCollapseAllGroups.Size = new Size(127, 19);
			checkBoxCollapseAllGroups.TabIndex = 20;
			checkBoxCollapseAllGroups.Text = "Collapse all Groups";
			toolTipFindEffects.SetToolTip(checkBoxCollapseAllGroups, "When enabled will collapse all groups prior to finding selected Effects.");
			checkBoxCollapseAllGroups.UseVisualStyleBackColor = true;
			// 
			// contextMenuStrip1
			// 
			contextMenuStrip1.ImageScalingSize = new Size(20, 20);
			contextMenuStrip1.Items.AddRange(new ToolStripItem[] { findAllSelectedEffectsToolStripMenuItem });
			contextMenuStrip1.Name = "contextMenuStrip1";
			contextMenuStrip1.Size = new Size(197, 26);
			// 
			// findAllSelectedEffectsToolStripMenuItem
			// 
			findAllSelectedEffectsToolStripMenuItem.Name = "findAllSelectedEffectsToolStripMenuItem";
			findAllSelectedEffectsToolStripMenuItem.Size = new Size(196, 22);
			findAllSelectedEffectsToolStripMenuItem.Text = "Find all selected effects";
			findAllSelectedEffectsToolStripMenuItem.Click += findAllSelectedEffectsToolStripMenuItem_Click;
			// 
			// comboBoxFind
			// 
			comboBoxFind.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBoxFind.FormattingEnabled = true;
			comboBoxFind.Items.AddRange(new object[] { "Effects", "Layers" });
			comboBoxFind.Location = new Point(13, 9);
			comboBoxFind.Margin = new Padding(3, 2, 3, 2);
			comboBoxFind.MaxDropDownItems = 20;
			comboBoxFind.Name = "comboBoxFind";
			comboBoxFind.Size = new Size(123, 23);
			comboBoxFind.TabIndex = 21;
			comboBoxFind.SelectedIndexChanged += comboBoxFind_SelectedIndexChanged;
			// 
			// listViewEffectStartTime
			// 
			listViewEffectStartTime.AllowDrop = true;
			listViewEffectStartTime.AllowRowReorder = false;
			listViewEffectStartTime.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			listViewEffectStartTime.Columns.AddRange(new ColumnHeader[] { elementHeader, startTimeHeader, LayerEffectHeader });
			listViewEffectStartTime.ContextMenuStrip = contextMenuStrip1;
			listViewEffectStartTime.FullRowSelect = true;
			listViewEffectStartTime.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			listViewEffectStartTime.Location = new Point(0, 83);
			listViewEffectStartTime.Margin = new Padding(3, 2, 3, 2);
			listViewEffectStartTime.Name = "listViewEffectStartTime";
			listViewEffectStartTime.OwnerDraw = true;
			listViewEffectStartTime.Size = new Size(308, 330);
			listViewEffectStartTime.TabIndex = 14;
			listViewEffectStartTime.UseCompatibleStateImageBehavior = false;
			listViewEffectStartTime.View = View.Details;
			listViewEffectStartTime.DoubleClick += listViewEffectStartTime_DoubleClick;
			listViewEffectStartTime.MouseUp += listViewEffectStartTime_MouseUp;
			// 
			// elementHeader
			// 
			elementHeader.Text = "Element";
			elementHeader.Width = 104;
			// 
			// startTimeHeader
			// 
			startTimeHeader.Text = "Effect Start Time";
			startTimeHeader.Width = 160;
			// 
			// LayerEffectHeader
			// 
			LayerEffectHeader.Text = "Layer";
			LayerEffectHeader.Width = 40;
			// 
			// FindEffectForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			BackColor = Color.FromArgb(68, 68, 68);
			ClientSize = new Size(311, 414);
			ControlBox = false;
			Controls.Add(comboBoxFind);
			Controls.Add(checkBoxCollapseAllGroups);
			Controls.Add(listViewEffectStartTime);
			Controls.Add(comboBoxAvailableEffect);
			Controls.Add(linkLabel1);
			Margin = new Padding(3, 2, 3, 2);
			Name = "FindEffectForm";
			Text = "Find Effects/Layers";
			Load += FindEffectForm_Load;
			Resize += FindEffectForm_Resize;
			contextMenuStrip1.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private LinkLabel linkLabel1;
		private ComboBox comboBoxAvailableEffect;
		private Common.Controls.ListViewEx listViewEffectStartTime;
		private ColumnHeader elementHeader;
		private ColumnHeader startTimeHeader;
		private ToolTip toolTipFindEffects;
		private CheckBox checkBoxCollapseAllGroups;
		private ContextMenuStrip contextMenuStrip1;
		private ToolStripMenuItem findAllSelectedEffectsToolStripMenuItem;
		private ColumnHeader LayerEffectHeader;
		private ComboBox comboBoxFind;

	}
}