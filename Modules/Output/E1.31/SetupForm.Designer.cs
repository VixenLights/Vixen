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

        private MainMenu mainMenu;
        private MenuItem mIHelp;


        private Label label;

        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupForm));
			this.rowManipulationContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mIHelp = new System.Windows.Forms.MenuItem();
			this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
			this.warningsCheckBox = new System.Windows.Forms.CheckBox();
			this.statisticsCheckBox = new System.Windows.Forms.CheckBox();
			this.eventRepeatCountTextBox = new System.Windows.Forms.TextBox();
			this.label = new System.Windows.Forms.Label();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.eventSuppressCountTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.autoPopulateStart = new System.Windows.Forms.CheckBox();
			this.btnDeleteUnicast = new System.Windows.Forms.Button();
			this.btnAddUnicast = new System.Windows.Forms.Button();
			this.btnDeleteUniverse = new System.Windows.Forms.Button();
			this.btnAddUniverse = new System.Windows.Forms.Button();
			this.lblDestination = new System.Windows.Forms.Label();
			this.comboDestination = new System.Windows.Forms.ComboBox();
			this.lblPriority = new System.Windows.Forms.Label();
			this.numericPriority = new System.Windows.Forms.NumericUpDown();
			this.chkBoxTransmitBlind = new System.Windows.Forms.CheckBox();
			this.tabControlEX1 = new Dotnetrix.Controls.TabControlEX();
			this.tabPageEX1 = new Dotnetrix.Controls.TabPageEX();
			this.tabPageEX2 = new Dotnetrix.Controls.TabPageEX();
			this.univDGVN = new VixenModules.Output.E131.Controls.DataGridViewNumbered();
			this.startColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.activeColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.universeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.sizeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.numericPriority)).BeginInit();
			this.tabControlEX1.SuspendLayout();
			this.tabPageEX1.SuspendLayout();
			this.tabPageEX2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.univDGVN)).BeginInit();
			this.SuspendLayout();
			// 
			// rowManipulationContextMenuStrip
			// 
			this.rowManipulationContextMenuStrip.Name = "rowManipulationContextMenuStrip";
			this.rowManipulationContextMenuStrip.Size = new System.Drawing.Size(61, 4);
			// 
			// mIHelp
			// 
			this.mIHelp.Index = -1;
			this.mIHelp.Text = "&Help";
			// 
			// warningsCheckBox
			// 
			this.warningsCheckBox.AutoSize = true;
			this.warningsCheckBox.Location = new System.Drawing.Point(35, 38);
			this.warningsCheckBox.Name = "warningsCheckBox";
			this.warningsCheckBox.Size = new System.Drawing.Size(262, 19);
			this.warningsCheckBox.TabIndex = 51;
			this.warningsCheckBox.Text = "Display ALL Warnings/Errors and wait For OK";
			// 
			// statisticsCheckBox
			// 
			this.statisticsCheckBox.AutoSize = true;
			this.statisticsCheckBox.Location = new System.Drawing.Point(35, 64);
			this.statisticsCheckBox.Name = "statisticsCheckBox";
			this.statisticsCheckBox.Size = new System.Drawing.Size(263, 19);
			this.statisticsCheckBox.TabIndex = 52;
			this.statisticsCheckBox.Text = "Gather statistics and display at end of session";
			// 
			// eventRepeatCountTextBox
			// 
			this.eventRepeatCountTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.eventRepeatCountTextBox.Location = new System.Drawing.Point(42, 162);
			this.eventRepeatCountTextBox.MaxLength = 2;
			this.eventRepeatCountTextBox.Name = "eventRepeatCountTextBox";
			this.eventRepeatCountTextBox.Size = new System.Drawing.Size(30, 23);
			this.eventRepeatCountTextBox.TabIndex = 55;
			this.eventRepeatCountTextBox.Text = "0";
			this.eventRepeatCountTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.eventRepeatCountTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.eventRepeatCountTextBoxValidating);
			// 
			// label
			// 
			this.label.AutoSize = true;
			this.label.Location = new System.Drawing.Point(78, 162);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(192, 90);
			this.label.TabIndex = 5;
			this.label.Text = "Max Repeat Count\r\n\r\nSet to 0 to send all events (even 0s)\r\nto each universe, Set " +
    "to >0 to stop\r\nrepeating frames after N duplicates\r\nare sent.";
			// 
			// okButton
			// 
			this.okButton.AutoSize = true;
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(188, 447);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 25);
			this.okButton.TabIndex = 199;
			this.okButton.Text = "OK";
			this.okButton.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.okButton.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// cancelButton
			// 
			this.cancelButton.AutoSize = true;
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(281, 448);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 25);
			this.cancelButton.TabIndex = 200;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.cancelButton.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// eventSuppressCountTextBox
			// 
			this.eventSuppressCountTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.eventSuppressCountTextBox.Location = new System.Drawing.Point(42, 276);
			this.eventSuppressCountTextBox.MaxLength = 2;
			this.eventSuppressCountTextBox.Name = "eventSuppressCountTextBox";
			this.eventSuppressCountTextBox.Size = new System.Drawing.Size(30, 23);
			this.eventSuppressCountTextBox.TabIndex = 56;
			this.eventSuppressCountTextBox.Text = "0";
			this.eventSuppressCountTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.eventSuppressCountTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.eventSuppressCountTextBoxValidating);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(78, 276);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(182, 90);
			this.label1.TabIndex = 104;
			this.label1.Text = "Max Suppress Count\r\n\r\nOnly used if Max Repeat Count\r\nis not  0. Set to >0 to allo" +
    "w every\r\nNth duplicate frame in a universe\r\nto go out.";
			// 
			// autoPopulateStart
			// 
			this.autoPopulateStart.AutoSize = true;
			this.autoPopulateStart.Location = new System.Drawing.Point(35, 15);
			this.autoPopulateStart.Name = "autoPopulateStart";
			this.autoPopulateStart.Size = new System.Drawing.Size(186, 19);
			this.autoPopulateStart.TabIndex = 50;
			this.autoPopulateStart.Text = "Manually manage start values.";
			this.autoPopulateStart.CheckedChanged += new System.EventHandler(this.autoPopulateStart_CheckedChanged);
			// 
			// btnDeleteUnicast
			// 
			this.btnDeleteUnicast.FlatAppearance.BorderSize = 0;
			this.btnDeleteUnicast.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnDeleteUnicast.Location = new System.Drawing.Point(261, 337);
			this.btnDeleteUnicast.Name = "btnDeleteUnicast";
			this.btnDeleteUnicast.Size = new System.Drawing.Size(27, 28);
			this.btnDeleteUnicast.TabIndex = 7;
			this.btnDeleteUnicast.Text = "-";
			this.btnDeleteUnicast.UseVisualStyleBackColor = true;
			this.btnDeleteUnicast.Click += new System.EventHandler(this.btnRemoveUnicast_Click);
			// 
			// btnAddUnicast
			// 
			this.btnAddUnicast.FlatAppearance.BorderSize = 0;
			this.btnAddUnicast.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAddUnicast.Location = new System.Drawing.Point(228, 337);
			this.btnAddUnicast.Name = "btnAddUnicast";
			this.btnAddUnicast.Size = new System.Drawing.Size(27, 28);
			this.btnAddUnicast.TabIndex = 6;
			this.btnAddUnicast.Text = "+";
			this.btnAddUnicast.UseVisualStyleBackColor = true;
			this.btnAddUnicast.Click += new System.EventHandler(this.btnAddUnicast_Click_1);
			// 
			// btnDeleteUniverse
			// 
			this.btnDeleteUniverse.FlatAppearance.BorderSize = 0;
			this.btnDeleteUniverse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnDeleteUniverse.Location = new System.Drawing.Point(294, 44);
			this.btnDeleteUniverse.Name = "btnDeleteUniverse";
			this.btnDeleteUniverse.Size = new System.Drawing.Size(27, 28);
			this.btnDeleteUniverse.TabIndex = 4;
			this.btnDeleteUniverse.Text = "-";
			this.btnDeleteUniverse.UseVisualStyleBackColor = true;
			this.btnDeleteUniverse.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnAddUniverse
			// 
			this.btnAddUniverse.FlatAppearance.BorderSize = 0;
			this.btnAddUniverse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAddUniverse.Location = new System.Drawing.Point(294, 10);
			this.btnAddUniverse.Name = "btnAddUniverse";
			this.btnAddUniverse.Size = new System.Drawing.Size(27, 28);
			this.btnAddUniverse.TabIndex = 3;
			this.btnAddUniverse.Text = "+";
			this.btnAddUniverse.UseVisualStyleBackColor = true;
			this.btnAddUniverse.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// lblDestination
			// 
			this.lblDestination.AutoSize = true;
			this.lblDestination.Location = new System.Drawing.Point(15, 339);
			this.lblDestination.Name = "lblDestination";
			this.lblDestination.Size = new System.Drawing.Size(67, 15);
			this.lblDestination.TabIndex = 3;
			this.lblDestination.Text = "Destination";
			// 
			// comboDestination
			// 
			this.comboDestination.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboDestination.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboDestination.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.comboDestination.FormattingEnabled = true;
			this.comboDestination.Location = new System.Drawing.Point(19, 368);
			this.comboDestination.Name = "comboDestination";
			this.comboDestination.Size = new System.Drawing.Size(269, 24);
			this.comboDestination.TabIndex = 5;
			this.comboDestination.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
			this.comboDestination.SelectedIndexChanged += new System.EventHandler(this.comboDestination_SelectedIndexChanged);
			// 
			// lblPriority
			// 
			this.lblPriority.AutoSize = true;
			this.lblPriority.Location = new System.Drawing.Point(102, 119);
			this.lblPriority.Name = "lblPriority";
			this.lblPriority.Size = new System.Drawing.Size(81, 15);
			this.lblPriority.TabIndex = 108;
			this.lblPriority.Text = "sACN priority.";
			// 
			// numericPriority
			// 
			this.numericPriority.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.numericPriority.Location = new System.Drawing.Point(35, 115);
			this.numericPriority.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
			this.numericPriority.Name = "numericPriority";
			this.numericPriority.Size = new System.Drawing.Size(49, 23);
			this.numericPriority.TabIndex = 54;
			this.numericPriority.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			// 
			// chkBoxTransmitBlind
			// 
			this.chkBoxTransmitBlind.AutoSize = true;
			this.chkBoxTransmitBlind.Location = new System.Drawing.Point(35, 88);
			this.chkBoxTransmitBlind.Name = "chkBoxTransmitBlind";
			this.chkBoxTransmitBlind.Size = new System.Drawing.Size(132, 19);
			this.chkBoxTransmitBlind.TabIndex = 53;
			this.chkBoxTransmitBlind.Text = "Transmit blind data.";
			this.chkBoxTransmitBlind.UseVisualStyleBackColor = true;
			// 
			// tabControlEX1
			// 
			this.tabControlEX1.Controls.Add(this.tabPageEX1);
			this.tabControlEX1.Controls.Add(this.tabPageEX2);
			this.tabControlEX1.ItemSize = new System.Drawing.Size(59, 18);
			this.tabControlEX1.Location = new System.Drawing.Point(12, 12);
			this.tabControlEX1.Name = "tabControlEX1";
			this.tabControlEX1.SelectedIndex = 0;
			this.tabControlEX1.Size = new System.Drawing.Size(351, 424);
			this.tabControlEX1.TabIndex = 201;
			this.tabControlEX1.UseVisualStyles = false;
			// 
			// tabPageEX1
			// 
			this.tabPageEX1.Controls.Add(this.btnDeleteUnicast);
			this.tabPageEX1.Controls.Add(this.univDGVN);
			this.tabPageEX1.Controls.Add(this.btnAddUnicast);
			this.tabPageEX1.Controls.Add(this.comboDestination);
			this.tabPageEX1.Controls.Add(this.btnDeleteUniverse);
			this.tabPageEX1.Controls.Add(this.lblDestination);
			this.tabPageEX1.Controls.Add(this.btnAddUniverse);
			this.tabPageEX1.Location = new System.Drawing.Point(4, 22);
			this.tabPageEX1.Name = "tabPageEX1";
			this.tabPageEX1.Size = new System.Drawing.Size(343, 398);
			this.tabPageEX1.TabIndex = 0;
			this.tabPageEX1.Text = "Universes";
			this.tabPageEX1.UseVisualStyleBackColor = true;
			// 
			// tabPageEX2
			// 
			this.tabPageEX2.Controls.Add(this.lblPriority);
			this.tabPageEX2.Controls.Add(this.autoPopulateStart);
			this.tabPageEX2.Controls.Add(this.numericPriority);
			this.tabPageEX2.Controls.Add(this.warningsCheckBox);
			this.tabPageEX2.Controls.Add(this.chkBoxTransmitBlind);
			this.tabPageEX2.Controls.Add(this.eventSuppressCountTextBox);
			this.tabPageEX2.Controls.Add(this.label);
			this.tabPageEX2.Controls.Add(this.statisticsCheckBox);
			this.tabPageEX2.Controls.Add(this.label1);
			this.tabPageEX2.Controls.Add(this.eventRepeatCountTextBox);
			this.tabPageEX2.Location = new System.Drawing.Point(4, 22);
			this.tabPageEX2.Name = "tabPageEX2";
			this.tabPageEX2.Size = new System.Drawing.Size(343, 398);
			this.tabPageEX2.TabIndex = 1;
			this.tabPageEX2.Text = "Advanced Options";
			this.tabPageEX2.UseVisualStyleBackColor = true;
			// 
			// univDGVN
			// 
			this.univDGVN.AllowUserToAddRows = false;
			this.univDGVN.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.univDGVN.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.univDGVN.BackgroundColor = this.BackColor;
			this.univDGVN.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.startColumn,
            this.activeColumn,
            this.universeColumn,
            this.sizeColumn});
			this.univDGVN.ContextMenuStrip = this.rowManipulationContextMenuStrip;
			this.univDGVN.Location = new System.Drawing.Point(19, 10);
			this.univDGVN.Name = "univDGVN";
			this.univDGVN.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.univDGVN.Size = new System.Drawing.Size(269, 317);
			this.univDGVN.TabIndex = 2;
			this.univDGVN.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.UnivDgvnCellEndEdit);
			this.univDGVN.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.UnivDgvnCellEnter);
			this.univDGVN.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.UnivDgvnCellMouseClick);
			this.univDGVN.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.UnivDgvnCellMouseEnter);
			this.univDGVN.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.univDGVN_CellValidated);
			this.univDGVN.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.UnivDgvnCellValidating);
			this.univDGVN.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.UnivDgvnEditingControlShowing);
			this.univDGVN.UserAddedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.UnivDgvnInsertRow);
			this.univDGVN.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.UnivDgvnDeletedRow);
			this.univDGVN.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.univDGVN_KeyPress);
			this.univDGVN.KeyUp += new System.Windows.Forms.KeyEventHandler(this.univDGVN_KeyUp);
			// 
			// startColumn
			// 
			this.startColumn.ContextMenuStrip = this.rowManipulationContextMenuStrip;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			this.startColumn.DefaultCellStyle = dataGridViewCellStyle1;
			this.startColumn.HeaderText = "Start";
			this.startColumn.MaxInputLength = 5;
			this.startColumn.Name = "startColumn";
			this.startColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.startColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.startColumn.ToolTipText = "Sort (LeftClick = Ascending, RightClick = Descending)";
			this.startColumn.Width = 37;
			// 
			// activeColumn
			// 
			this.activeColumn.ContextMenuStrip = this.rowManipulationContextMenuStrip;
			this.activeColumn.HeaderText = "On";
			this.activeColumn.Name = "activeColumn";
			this.activeColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.activeColumn.Width = 29;
			// 
			// universeColumn
			// 
			this.universeColumn.ContextMenuStrip = this.rowManipulationContextMenuStrip;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			this.universeColumn.DefaultCellStyle = dataGridViewCellStyle2;
			this.universeColumn.HeaderText = "Universe";
			this.universeColumn.MaxInputLength = 5;
			this.universeColumn.Name = "universeColumn";
			this.universeColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.universeColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.universeColumn.ToolTipText = "Sort (LeftClick = Ascending, RightClick = Descending)";
			this.universeColumn.Width = 58;
			// 
			// sizeColumn
			// 
			this.sizeColumn.ContextMenuStrip = this.rowManipulationContextMenuStrip;
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			this.sizeColumn.DefaultCellStyle = dataGridViewCellStyle3;
			this.sizeColumn.HeaderText = "Size";
			this.sizeColumn.MaxInputLength = 3;
			this.sizeColumn.Name = "sizeColumn";
			this.sizeColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.sizeColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.sizeColumn.ToolTipText = "Sort (LeftClick = Ascending, RightClick = Descending)";
			this.sizeColumn.Width = 33;
			// 
			// SetupForm
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(382, 485);
			this.Controls.Add(this.tabControlEX1);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Menu = this.mainMenu;
			this.MinimumSize = new System.Drawing.Size(397, 519);
			this.Name = "SetupForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Streaming ACN (E1.31) Configuration";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SetupForm_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.numericPriority)).EndInit();
			this.tabControlEX1.ResumeLayout(false);
			this.tabPageEX1.ResumeLayout(false);
			this.tabPageEX1.PerformLayout();
			this.tabPageEX2.ResumeLayout(false);
			this.tabPageEX2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.univDGVN)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

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
		private Dotnetrix.Controls.TabControlEX tabControlEX1;
		private Dotnetrix.Controls.TabPageEX tabPageEX1;
		private Dotnetrix.Controls.TabPageEX tabPageEX2;
    }
}