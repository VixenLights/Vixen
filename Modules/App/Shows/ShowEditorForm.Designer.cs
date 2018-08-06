namespace VixenModules.App.Shows
{
	partial class ShowEditorForm
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
			this.buttonDeleteItem = new System.Windows.Forms.Button();
			this.buttonAddItem = new System.Windows.Forms.Button();
			this.groupBoxAction = new System.Windows.Forms.GroupBox();
			this.comboBoxActions = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBoxItemEdit = new System.Windows.Forms.GroupBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonHelp = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxShowName = new System.Windows.Forms.TextBox();
			this.listViewShowItems = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.labelHelp = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.buttonCancel = new System.Windows.Forms.Button();
			this.tabControlShowItems = new Dotnetrix.Controls.TabControlEX();
			this.tabPageStartup = new Dotnetrix.Controls.TabPageEX();
			this.tabPageBackground = new Dotnetrix.Controls.TabPageEX();
			this.tabPageSequential = new Dotnetrix.Controls.TabPageEX();
			this.tabPageShutdown = new Dotnetrix.Controls.TabPageEX();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.groupBoxAction.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.tabControlShowItems.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonDeleteItem
			// 
			this.buttonDeleteItem.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.buttonDeleteItem.Location = new System.Drawing.Point(318, 4);
			this.buttonDeleteItem.Name = "buttonDeleteItem";
			this.buttonDeleteItem.Size = new System.Drawing.Size(28, 28);
			this.buttonDeleteItem.TabIndex = 7;
			this.buttonDeleteItem.Text = "-";
			this.buttonDeleteItem.UseVisualStyleBackColor = true;
			this.buttonDeleteItem.Click += new System.EventHandler(this.buttonDeleteItem_Click);
			// 
			// buttonAddItem
			// 
			this.buttonAddItem.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.buttonAddItem.Location = new System.Drawing.Point(352, 4);
			this.buttonAddItem.Name = "buttonAddItem";
			this.buttonAddItem.Size = new System.Drawing.Size(28, 28);
			this.buttonAddItem.TabIndex = 5;
			this.buttonAddItem.Text = "+";
			this.buttonAddItem.UseVisualStyleBackColor = true;
			this.buttonAddItem.Click += new System.EventHandler(this.buttonAddItem_Click);
			// 
			// groupBoxAction
			// 
			this.groupBoxAction.AutoSize = true;
			this.groupBoxAction.Controls.Add(this.comboBoxActions);
			this.groupBoxAction.Controls.Add(this.label3);
			this.groupBoxAction.Location = new System.Drawing.Point(402, 82);
			this.groupBoxAction.Name = "groupBoxAction";
			this.groupBoxAction.Size = new System.Drawing.Size(516, 68);
			this.groupBoxAction.TabIndex = 11;
			this.groupBoxAction.TabStop = false;
			this.groupBoxAction.Text = "Action";
			this.groupBoxAction.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// comboBoxActions
			// 
			this.comboBoxActions.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboBoxActions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxActions.FormattingEnabled = true;
			this.comboBoxActions.Location = new System.Drawing.Point(61, 22);
			this.comboBoxActions.Name = "comboBoxActions";
			this.comboBoxActions.Size = new System.Drawing.Size(140, 24);
			this.comboBoxActions.TabIndex = 1;
			this.comboBoxActions.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
			this.comboBoxActions.SelectedIndexChanged += new System.EventHandler(this.comboBoxActions_SelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(7, 25);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(45, 15);
			this.label3.TabIndex = 0;
			this.label3.Text = "Action:";
			// 
			// groupBoxItemEdit
			// 
			this.groupBoxItemEdit.Location = new System.Drawing.Point(402, 149);
			this.groupBoxItemEdit.Name = "groupBoxItemEdit";
			this.groupBoxItemEdit.Size = new System.Drawing.Size(516, 330);
			this.groupBoxItemEdit.TabIndex = 1;
			this.groupBoxItemEdit.TabStop = false;
			this.groupBoxItemEdit.Text = "Item Information";
			this.groupBoxItemEdit.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(736, 526);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(87, 27);
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			this.buttonOK.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOK.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonHelp
			// 
			this.buttonHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonHelp.Location = new System.Drawing.Point(3, 40);
			this.buttonHelp.Name = "buttonHelp";
			this.buttonHelp.Size = new System.Drawing.Size(70, 27);
			this.buttonHelp.TabIndex = 62;
			this.buttonHelp.Tag = "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/nutcracker-eff" +
    "ects/";
			this.buttonHelp.Text = "Help";
			this.buttonHelp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.buttonHelp.UseVisualStyleBackColor = true;
			this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.textBoxShowName);
			this.groupBox2.Location = new System.Drawing.Point(14, 8);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(904, 50);
			this.groupBox2.TabIndex = 63;
			this.groupBox2.TabStop = false;
			this.groupBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(74, 15);
			this.label1.TabIndex = 2;
			this.label1.Text = "Show Name:";
			// 
			// textBoxShowName
			// 
			this.textBoxShowName.Location = new System.Drawing.Point(93, 18);
			this.textBoxShowName.Name = "textBoxShowName";
			this.textBoxShowName.Size = new System.Drawing.Size(229, 23);
			this.textBoxShowName.TabIndex = 1;
			// 
			// listViewShowItems
			// 
			this.listViewShowItems.AllowDrop = true;
			this.listViewShowItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.listViewShowItems.FullRowSelect = true;
			this.listViewShowItems.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewShowItems.HideSelection = false;
			this.listViewShowItems.Location = new System.Drawing.Point(12, 104);
			this.listViewShowItems.MultiSelect = false;
			this.listViewShowItems.Name = "listViewShowItems";
			this.listViewShowItems.OwnerDraw = true;
			this.listViewShowItems.Size = new System.Drawing.Size(383, 374);
			this.listViewShowItems.TabIndex = 64;
			this.listViewShowItems.Tag = "";
			this.toolTip1.SetToolTip(this.listViewShowItems, "Left click Sequence and drag to re-arrange show order");
			this.listViewShowItems.UseCompatibleStateImageBehavior = false;
			this.listViewShowItems.View = System.Windows.Forms.View.Details;
			this.listViewShowItems.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listViewShowItems_AfterLabelEdit);
			this.listViewShowItems.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listViewShowItems_Highlight);
			this.listViewShowItems.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewShowItems_ItemDrag);
			this.listViewShowItems.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewShowItems_ItemSelectionChanged);
			this.listViewShowItems.SelectedIndexChanged += new System.EventHandler(this.listViewShowItems_SelectedIndexChanged);
			this.listViewShowItems.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewShowItems_DragDrop);
			this.listViewShowItems.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewShowItems_DragEnter);
			this.listViewShowItems.DragOver += new System.Windows.Forms.DragEventHandler(this.listViewShowItems_DragOver);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Item";
			this.columnHeader1.Width = 300;
			// 
			// labelHelp
			// 
			this.labelHelp.Location = new System.Drawing.Point(3, 0);
			this.labelHelp.Name = "labelHelp";
			this.labelHelp.Size = new System.Drawing.Size(309, 32);
			this.labelHelp.TabIndex = 65;
			this.labelHelp.Text = "Help";
			// 
			// toolTip1
			// 
			this.toolTip1.AutomaticDelay = 300;
			this.toolTip1.AutoPopDelay = 10000;
			this.toolTip1.InitialDelay = 300;
			this.toolTip1.ReshowDelay = 60;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Location = new System.Drawing.Point(835, 526);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(2);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(87, 25);
			this.buttonCancel.TabIndex = 66;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// tabControlShowItems
			// 
			this.tabControlShowItems.Controls.Add(this.tabPageStartup);
			this.tabControlShowItems.Controls.Add(this.tabPageBackground);
			this.tabControlShowItems.Controls.Add(this.tabPageSequential);
			this.tabControlShowItems.Controls.Add(this.tabPageShutdown);
			this.tabControlShowItems.ItemSize = new System.Drawing.Size(46, 18);
			this.tabControlShowItems.Location = new System.Drawing.Point(12, 82);
			this.tabControlShowItems.Margin = new System.Windows.Forms.Padding(1);
			this.tabControlShowItems.Name = "tabControlShowItems";
			this.tabControlShowItems.Padding = new System.Drawing.Point(2, 2);
			this.tabControlShowItems.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.tabControlShowItems.SelectedIndex = 3;
			this.tabControlShowItems.Size = new System.Drawing.Size(384, 23);
			this.tabControlShowItems.TabIndex = 0;
			this.tabControlShowItems.UseVisualStyles = false;
			this.tabControlShowItems.SelectedIndexChanged += new System.EventHandler(this.tabControlShowItems_SelectedIndexChanged);
			// 
			// tabPageStartup
			// 
			this.tabPageStartup.Location = new System.Drawing.Point(4, 22);
			this.tabPageStartup.Margin = new System.Windows.Forms.Padding(1);
			this.tabPageStartup.Name = "tabPageStartup";
			this.tabPageStartup.Size = new System.Drawing.Size(376, -3);
			this.tabPageStartup.TabIndex = 0;
			this.tabPageStartup.Tag = "These items are run once, in order, during startup.";
			this.tabPageStartup.Text = "Startup";
			this.tabPageStartup.UseVisualStyleBackColor = true;
			// 
			// tabPageBackground
			// 
			this.tabPageBackground.Location = new System.Drawing.Point(4, 22);
			this.tabPageBackground.Margin = new System.Windows.Forms.Padding(1);
			this.tabPageBackground.Name = "tabPageBackground";
			this.tabPageBackground.Size = new System.Drawing.Size(376, -3);
			this.tabPageBackground.TabIndex = 1;
			this.tabPageBackground.Tag = "These items are always run in the background.";
			this.tabPageBackground.Text = "Background";
			this.tabPageBackground.UseVisualStyleBackColor = true;
			// 
			// tabPageSequential
			// 
			this.tabPageSequential.Location = new System.Drawing.Point(4, 22);
			this.tabPageSequential.Margin = new System.Windows.Forms.Padding(1);
			this.tabPageSequential.Name = "tabPageSequential";
			this.tabPageSequential.Size = new System.Drawing.Size(376, -3);
			this.tabPageSequential.TabIndex = 2;
			this.tabPageSequential.Tag = "These items are run, in order, and re-started while the show is running.";
			this.tabPageSequential.Text = "Sequential";
			this.tabPageSequential.UseVisualStyleBackColor = true;
			// 
			// tabPageShutdown
			// 
			this.tabPageShutdown.Location = new System.Drawing.Point(4, 22);
			this.tabPageShutdown.Margin = new System.Windows.Forms.Padding(1);
			this.tabPageShutdown.Name = "tabPageShutdown";
			this.tabPageShutdown.Size = new System.Drawing.Size(376, 0);
			this.tabPageShutdown.TabIndex = 4;
			this.tabPageShutdown.Tag = "These items are run, in order, at the end of the show.";
			this.tabPageShutdown.Text = "Shutdown";
			this.tabPageShutdown.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.buttonHelp, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.labelHelp, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.buttonDeleteItem, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.buttonAddItem, 2, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 484);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(383, 75);
			this.tableLayoutPanel1.TabIndex = 67;
			// 
			// ShowEditorForm
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(932, 567);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.tabControlShowItems);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.groupBoxAction);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.groupBoxItemEdit);
			this.Controls.Add(this.listViewShowItems);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ShowEditorForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Setup a Show";
			this.Load += new System.EventHandler(this.ShowEditorForm_Load);
			this.groupBoxAction.ResumeLayout(false);
			this.groupBoxAction.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.tabControlShowItems.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.GroupBox groupBoxItemEdit;
		private System.Windows.Forms.Button buttonDeleteItem;
		private System.Windows.Forms.Button buttonAddItem;
        private System.Windows.Forms.Button buttonHelp;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBoxAction;
		private System.Windows.Forms.ComboBox comboBoxActions;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ListView listViewShowItems;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.Label labelHelp;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxShowName;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button buttonCancel;
		private Dotnetrix.Controls.TabControlEX tabControlShowItems;
		private Dotnetrix.Controls.TabPageEX tabPageStartup;
		private Dotnetrix.Controls.TabPageEX tabPageBackground;
		private Dotnetrix.Controls.TabPageEX tabPageSequential;
		private Dotnetrix.Controls.TabPageEX tabPageShutdown;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}