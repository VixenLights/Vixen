namespace VixenModules.Output.E131
{
    using System.ComponentModel;
    using System.Windows.Forms;

    using VixenModules.Output.E131.Controls;

    public partial class SetupForm
    {
        // datagridview columns

        private Button cancelButton;

        private IContainer components;

        // other entry controls
        private TextBox eventRepeatCountTextBox;

        // our buttons
        private Button okButton;

        private int pluginChannelCount;

        // common contextmenustrip for row manipulation - added to most of the columns
        private ContextMenuStrip rowManipulationContextMenuStrip = new ContextMenuStrip();

        private CheckBox statisticsCheckBox;

        private DataGridViewNumbered univDGVN;

        // universe datagridview cell event arguments to track mouse entry
        private DataGridViewCellEventArgs univDGVNCellEventArgs;

        private CheckBox warningsCheckBox;

        private MenuStrip mainMenu;
        private ToolStripMenuItem mIHelp;


        private Label label;

		private void InitializeComponent()
		{
			components = new Container();
			DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
			ComponentResourceManager resources = new ComponentResourceManager(typeof(SetupForm));
			rowManipulationContextMenuStrip = new ContextMenuStrip(components);
			mIHelp = new ToolStripMenuItem();
			mainMenu = new MenuStrip();
			warningsCheckBox = new CheckBox();
			statisticsCheckBox = new CheckBox();
			eventRepeatCountTextBox = new TextBox();
			label = new Label();
			okButton = new Button();
			cancelButton = new Button();
			eventSuppressCountTextBox = new TextBox();
			label1 = new Label();
			autoPopulateStart = new CheckBox();
			btnDeleteUnicast = new Button();
			btnAddUnicast = new Button();
			btnDeleteUniverse = new Button();
			btnAddUniverse = new Button();
			lblDestination = new Label();
			comboDestination = new ComboBox();
			lblPriority = new Label();
			numericPriority = new NumericUpDown();
			chkBoxTransmitBlind = new CheckBox();
			tabControlEX1 = new TabControl();
			tabPageEX1 = new TabPage();
			btnDeleteAllUniverses = new Button();
			univDGVN = new DataGridViewNumbered();
			startColumn = new DataGridViewTextBoxColumn();
			activeColumn = new DataGridViewCheckBoxColumn();
			universeColumn = new DataGridViewTextBoxColumn();
			sizeColumn = new DataGridViewTextBoxColumn();
			tabPageEX2 = new TabPage();
			((ISupportInitialize)numericPriority).BeginInit();
			tabControlEX1.SuspendLayout();
			tabPageEX1.SuspendLayout();
			((ISupportInitialize)univDGVN).BeginInit();
			tabPageEX2.SuspendLayout();
			SuspendLayout();
			// 
			// rowManipulationContextMenuStrip
			// 
			rowManipulationContextMenuStrip.Name = "rowManipulationContextMenuStrip";
			rowManipulationContextMenuStrip.Size = new Size(61, 4);
			// 
			// mIHelp
			// 
			mIHelp.Name = "mIHelp";
			mIHelp.Size = new Size(32, 19);
			mIHelp.Text = "&Help";
			// 
			// mainMenu
			// 
			mainMenu.Location = new Point(0, 0);
			mainMenu.Name = "mainMenu";
			mainMenu.Size = new Size(200, 24);
			mainMenu.TabIndex = 0;
			// 
			// warningsCheckBox
			// 
			warningsCheckBox.AutoSize = true;
			warningsCheckBox.Location = new Point(35, 38);
			warningsCheckBox.Name = "warningsCheckBox";
			warningsCheckBox.Size = new Size(262, 19);
			warningsCheckBox.TabIndex = 51;
			warningsCheckBox.Text = "Display ALL Warnings/Errors and wait For OK";
			// 
			// statisticsCheckBox
			// 
			statisticsCheckBox.AutoSize = true;
			statisticsCheckBox.Location = new Point(35, 64);
			statisticsCheckBox.Name = "statisticsCheckBox";
			statisticsCheckBox.Size = new Size(263, 19);
			statisticsCheckBox.TabIndex = 52;
			statisticsCheckBox.Text = "Gather statistics and display at end of session";
			// 
			// eventRepeatCountTextBox
			// 
			eventRepeatCountTextBox.BorderStyle = BorderStyle.FixedSingle;
			eventRepeatCountTextBox.Location = new Point(42, 162);
			eventRepeatCountTextBox.MaxLength = 2;
			eventRepeatCountTextBox.Name = "eventRepeatCountTextBox";
			eventRepeatCountTextBox.Size = new Size(30, 23);
			eventRepeatCountTextBox.TabIndex = 55;
			eventRepeatCountTextBox.Text = "0";
			eventRepeatCountTextBox.TextAlign = HorizontalAlignment.Center;
			eventRepeatCountTextBox.Validating += eventRepeatCountTextBoxValidating;
			// 
			// label
			// 
			label.AutoSize = true;
			label.Location = new Point(78, 162);
			label.Name = "label";
			label.Size = new Size(192, 90);
			label.TabIndex = 5;
			label.Text = "Max Repeat Count\r\n\r\nSet to 0 to send all events (even 0s)\r\nto each universe, Set to >0 to stop\r\nrepeating frames after N duplicates\r\nare sent.";
			// 
			// okButton
			// 
			okButton.AutoSize = true;
			okButton.DialogResult = DialogResult.OK;
			okButton.Location = new Point(188, 447);
			okButton.Name = "okButton";
			okButton.Size = new Size(75, 25);
			okButton.TabIndex = 199;
			okButton.Text = "OK";
			// 
			// cancelButton
			// 
			cancelButton.AutoSize = true;
			cancelButton.DialogResult = DialogResult.Cancel;
			cancelButton.Location = new Point(281, 448);
			cancelButton.Name = "cancelButton";
			cancelButton.Size = new Size(75, 25);
			cancelButton.TabIndex = 200;
			cancelButton.Text = "Cancel";
			// 
			// eventSuppressCountTextBox
			// 
			eventSuppressCountTextBox.BorderStyle = BorderStyle.FixedSingle;
			eventSuppressCountTextBox.Location = new Point(42, 276);
			eventSuppressCountTextBox.MaxLength = 2;
			eventSuppressCountTextBox.Name = "eventSuppressCountTextBox";
			eventSuppressCountTextBox.Size = new Size(30, 23);
			eventSuppressCountTextBox.TabIndex = 56;
			eventSuppressCountTextBox.Text = "0";
			eventSuppressCountTextBox.TextAlign = HorizontalAlignment.Center;
			eventSuppressCountTextBox.Validating += eventSuppressCountTextBoxValidating;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(78, 276);
			label1.Name = "label1";
			label1.Size = new Size(182, 90);
			label1.TabIndex = 104;
			label1.Text = "Max Suppress Count\r\n\r\nOnly used if Max Repeat Count\r\nis not  0. Set to >0 to allow every\r\nNth duplicate frame in a universe\r\nto go out.";
			// 
			// autoPopulateStart
			// 
			autoPopulateStart.AutoSize = true;
			autoPopulateStart.Location = new Point(35, 15);
			autoPopulateStart.Name = "autoPopulateStart";
			autoPopulateStart.Size = new Size(186, 19);
			autoPopulateStart.TabIndex = 50;
			autoPopulateStart.Text = "Manually manage start values.";
			autoPopulateStart.CheckedChanged += autoPopulateStart_CheckedChanged;
			// 
			// btnDeleteUnicast
			// 
			btnDeleteUnicast.FlatAppearance.BorderSize = 0;
			btnDeleteUnicast.FlatStyle = FlatStyle.Flat;
			btnDeleteUnicast.Location = new Point(261, 337);
			btnDeleteUnicast.Name = "btnDeleteUnicast";
			btnDeleteUnicast.Size = new Size(27, 28);
			btnDeleteUnicast.TabIndex = 7;
			btnDeleteUnicast.UseVisualStyleBackColor = true;
			btnDeleteUnicast.Click += btnRemoveUnicast_Click;
			// 
			// btnAddUnicast
			// 
			btnAddUnicast.FlatAppearance.BorderSize = 0;
			btnAddUnicast.FlatStyle = FlatStyle.Flat;
			btnAddUnicast.Location = new Point(228, 337);
			btnAddUnicast.Name = "btnAddUnicast";
			btnAddUnicast.Size = new Size(27, 28);
			btnAddUnicast.TabIndex = 6;
			btnAddUnicast.UseVisualStyleBackColor = true;
			btnAddUnicast.Click += btnAddUnicast_Click_1;
			// 
			// btnDeleteUniverse
			// 
			btnDeleteUniverse.FlatAppearance.BorderSize = 0;
			btnDeleteUniverse.FlatStyle = FlatStyle.Flat;
			btnDeleteUniverse.Location = new Point(294, 44);
			btnDeleteUniverse.Name = "btnDeleteUniverse";
			btnDeleteUniverse.Size = new Size(27, 28);
			btnDeleteUniverse.TabIndex = 4;
			btnDeleteUniverse.UseVisualStyleBackColor = true;
			btnDeleteUniverse.Click += btnDelete_Click;
			// 
			// btnAddUniverse
			// 
			btnAddUniverse.FlatAppearance.BorderSize = 0;
			btnAddUniverse.FlatStyle = FlatStyle.Flat;
			btnAddUniverse.Location = new Point(294, 10);
			btnAddUniverse.Name = "btnAddUniverse";
			btnAddUniverse.Size = new Size(27, 28);
			btnAddUniverse.TabIndex = 3;
			btnAddUniverse.UseVisualStyleBackColor = true;
			btnAddUniverse.Click += btnAdd_Click;
			// 
			// lblDestination
			// 
			lblDestination.AutoSize = true;
			lblDestination.Location = new Point(15, 339);
			lblDestination.Name = "lblDestination";
			lblDestination.Size = new Size(67, 15);
			lblDestination.TabIndex = 3;
			lblDestination.Text = "Destination";
			// 
			// comboDestination
			// 
			comboDestination.DrawMode = DrawMode.OwnerDrawFixed;
			comboDestination.DropDownStyle = ComboBoxStyle.DropDownList;
			comboDestination.FormattingEnabled = true;
			comboDestination.Location = new Point(19, 368);
			comboDestination.Name = "comboDestination";
			comboDestination.Size = new Size(269, 24);
			comboDestination.TabIndex = 5;
			comboDestination.DrawItem += comboBox_DrawItem;
			comboDestination.SelectedIndexChanged += comboDestination_SelectedIndexChanged;
			// 
			// lblPriority
			// 
			lblPriority.AutoSize = true;
			lblPriority.Location = new Point(102, 119);
			lblPriority.Name = "lblPriority";
			lblPriority.Size = new Size(81, 15);
			lblPriority.TabIndex = 108;
			lblPriority.Text = "sACN priority.";
			// 
			// numericPriority
			// 
			numericPriority.BorderStyle = BorderStyle.FixedSingle;
			numericPriority.Location = new Point(35, 115);
			numericPriority.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
			numericPriority.Name = "numericPriority";
			numericPriority.Size = new Size(49, 23);
			numericPriority.TabIndex = 54;
			numericPriority.Value = new decimal(new int[] { 100, 0, 0, 0 });
			// 
			// chkBoxTransmitBlind
			// 
			chkBoxTransmitBlind.AutoSize = true;
			chkBoxTransmitBlind.Location = new Point(35, 88);
			chkBoxTransmitBlind.Name = "chkBoxTransmitBlind";
			chkBoxTransmitBlind.Size = new Size(130, 19);
			chkBoxTransmitBlind.TabIndex = 53;
			chkBoxTransmitBlind.Text = "Transmit blind data.";
			chkBoxTransmitBlind.UseVisualStyleBackColor = true;
			// 
			// tabControlEX1
			// 
			tabControlEX1.Controls.Add(tabPageEX1);
			tabControlEX1.Controls.Add(tabPageEX2);
			tabControlEX1.ItemSize = new Size(59, 18);
			tabControlEX1.Location = new Point(12, 12);
			tabControlEX1.Name = "tabControlEX1";
			tabControlEX1.SelectedIndex = 0;
			tabControlEX1.Size = new Size(351, 424);
			tabControlEX1.TabIndex = 201;
			// 
			// tabPageEX1
			// 
			tabPageEX1.Controls.Add(btnDeleteAllUniverses);
			tabPageEX1.Controls.Add(btnDeleteUnicast);
			tabPageEX1.Controls.Add(univDGVN);
			tabPageEX1.Controls.Add(btnAddUnicast);
			tabPageEX1.Controls.Add(comboDestination);
			tabPageEX1.Controls.Add(btnDeleteUniverse);
			tabPageEX1.Controls.Add(lblDestination);
			tabPageEX1.Controls.Add(btnAddUniverse);
			tabPageEX1.Location = new Point(4, 22);
			tabPageEX1.Name = "tabPageEX1";
			tabPageEX1.Size = new Size(343, 398);
			tabPageEX1.TabIndex = 0;
			tabPageEX1.Text = "Universes";
			tabPageEX1.UseVisualStyleBackColor = true;
			// 
			// btnDeleteAllUniverses
			// 
			btnDeleteAllUniverses.FlatAppearance.BorderSize = 0;
			btnDeleteAllUniverses.Location = new Point(294, 78);
			btnDeleteAllUniverses.Name = "btnDeleteAllUniverses";
			btnDeleteAllUniverses.Size = new Size(27, 28);
			btnDeleteAllUniverses.TabIndex = 8;
			btnDeleteAllUniverses.UseVisualStyleBackColor = true;
			btnDeleteAllUniverses.Click += btnDeleteAllUniverses_Click;
			// 
			// univDGVN
			// 
			univDGVN.AllowUserToAddRows = false;
			univDGVN.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			univDGVN.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
			univDGVN.BackgroundColor = SystemColors.Control;
			dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = SystemColors.Control;
			dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
			dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
			univDGVN.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			univDGVN.Columns.AddRange(new DataGridViewColumn[] { startColumn, activeColumn, universeColumn, sizeColumn });
			univDGVN.ContextMenuStrip = rowManipulationContextMenuStrip;
			dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle5.BackColor = SystemColors.Window;
			dataGridViewCellStyle5.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
			dataGridViewCellStyle5.ForeColor = SystemColors.ControlText;
			dataGridViewCellStyle5.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle5.WrapMode = DataGridViewTriState.False;
			univDGVN.DefaultCellStyle = dataGridViewCellStyle5;
			univDGVN.Location = new Point(19, 10);
			univDGVN.Name = "univDGVN";
			univDGVN.SelectionMode = DataGridViewSelectionMode.CellSelect;
			univDGVN.Size = new Size(269, 317);
			univDGVN.TabIndex = 2;
			univDGVN.CellEndEdit += UnivDgvnCellEndEdit;
			univDGVN.CellEnter += UnivDgvnCellEnter;
			univDGVN.CellMouseClick += UnivDgvnCellMouseClick;
			univDGVN.CellMouseEnter += UnivDgvnCellMouseEnter;
			univDGVN.CellValidated += univDGVN_CellValidated;
			univDGVN.CellValidating += UnivDgvnCellValidating;
			univDGVN.EditingControlShowing += UnivDgvnEditingControlShowing;
			univDGVN.UserAddedRow += UnivDgvnInsertRow;
			univDGVN.UserDeletedRow += UnivDgvnDeletedRow;
			univDGVN.KeyPress += univDGVN_KeyPress;
			univDGVN.KeyUp += univDGVN_KeyUp;
			// 
			// startColumn
			// 
			startColumn.ContextMenuStrip = rowManipulationContextMenuStrip;
			dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleRight;
			startColumn.DefaultCellStyle = dataGridViewCellStyle2;
			startColumn.HeaderText = "Start";
			startColumn.MaxInputLength = 5;
			startColumn.Name = "startColumn";
			startColumn.Resizable = DataGridViewTriState.False;
			startColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
			startColumn.ToolTipText = "Sort (LeftClick = Ascending, RightClick = Descending)";
			startColumn.Width = 37;
			// 
			// activeColumn
			// 
			activeColumn.ContextMenuStrip = rowManipulationContextMenuStrip;
			activeColumn.HeaderText = "On";
			activeColumn.Name = "activeColumn";
			activeColumn.Resizable = DataGridViewTriState.False;
			activeColumn.Width = 29;
			// 
			// universeColumn
			// 
			universeColumn.ContextMenuStrip = rowManipulationContextMenuStrip;
			dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleRight;
			universeColumn.DefaultCellStyle = dataGridViewCellStyle3;
			universeColumn.HeaderText = "Universe";
			universeColumn.MaxInputLength = 5;
			universeColumn.Name = "universeColumn";
			universeColumn.Resizable = DataGridViewTriState.False;
			universeColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
			universeColumn.ToolTipText = "Sort (LeftClick = Ascending, RightClick = Descending)";
			universeColumn.Width = 58;
			// 
			// sizeColumn
			// 
			sizeColumn.ContextMenuStrip = rowManipulationContextMenuStrip;
			dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleRight;
			sizeColumn.DefaultCellStyle = dataGridViewCellStyle4;
			sizeColumn.HeaderText = "Size";
			sizeColumn.MaxInputLength = 3;
			sizeColumn.Name = "sizeColumn";
			sizeColumn.Resizable = DataGridViewTriState.False;
			sizeColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
			sizeColumn.ToolTipText = "Sort (LeftClick = Ascending, RightClick = Descending)";
			sizeColumn.Width = 33;
			// 
			// tabPageEX2
			// 
			tabPageEX2.Controls.Add(lblPriority);
			tabPageEX2.Controls.Add(autoPopulateStart);
			tabPageEX2.Controls.Add(numericPriority);
			tabPageEX2.Controls.Add(warningsCheckBox);
			tabPageEX2.Controls.Add(chkBoxTransmitBlind);
			tabPageEX2.Controls.Add(eventSuppressCountTextBox);
			tabPageEX2.Controls.Add(label);
			tabPageEX2.Controls.Add(statisticsCheckBox);
			tabPageEX2.Controls.Add(label1);
			tabPageEX2.Controls.Add(eventRepeatCountTextBox);
			tabPageEX2.Location = new Point(4, 22);
			tabPageEX2.Name = "tabPageEX2";
			tabPageEX2.Size = new Size(343, 398);
			tabPageEX2.TabIndex = 1;
			tabPageEX2.Text = "Advanced Options";
			tabPageEX2.UseVisualStyleBackColor = true;
			// 
			// SetupForm
			// 
			AcceptButton = okButton;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = true;
			CancelButton = cancelButton;
			ClientSize = new Size(382, 485);
			Controls.Add(tabControlEX1);
			Controls.Add(cancelButton);
			Controls.Add(okButton);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MinimizeBox = false;
			MaximizeBox = false;
			MainMenuStrip = mainMenu;
			MinimumSize = new Size(397, 519);
			Name = "SetupForm";
			StartPosition = FormStartPosition.CenterParent;
			Text = "Streaming ACN (E1.31) Configuration";
			FormClosing += SetupForm_FormClosing;
			((ISupportInitialize)numericPriority).EndInit();
			tabControlEX1.ResumeLayout(false);
			tabPageEX1.ResumeLayout(false);
			tabPageEX1.PerformLayout();
			((ISupportInitialize)univDGVN).EndInit();
			tabPageEX2.ResumeLayout(false);
			tabPageEX2.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }

            base.Dispose(disposing);
        }
		private Label label1;
		private TextBox eventSuppressCountTextBox;
		private CheckBox autoPopulateStart;
        private Label lblDestination;
        private ComboBox comboDestination;
        private Label lblPriority;
        private NumericUpDown numericPriority;
        private CheckBox chkBoxTransmitBlind;
        private DataGridViewTextBoxColumn startColumn;
        private DataGridViewCheckBoxColumn activeColumn;
        private DataGridViewTextBoxColumn universeColumn;
        private DataGridViewTextBoxColumn sizeColumn;
        private Button btnAddUniverse;
        private Button btnDeleteUniverse;
        private Button btnDeleteUnicast;
        private Button btnAddUnicast;
		private TabControl tabControlEX1;
		private TabPage tabPageEX1;
		private TabPage tabPageEX2;
		private Button btnDeleteAllUniverses;
	}
}