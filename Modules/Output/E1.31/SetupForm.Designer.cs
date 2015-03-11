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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnAddUnicast = new System.Windows.Forms.Button();
            this.lblDestination = new System.Windows.Forms.Label();
            this.comboDestination = new System.Windows.Forms.ComboBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lblPriority = new System.Windows.Forms.Label();
            this.numericPriority = new System.Windows.Forms.NumericUpDown();
            this.chkBoxTransmitBlind = new System.Windows.Forms.CheckBox();
            this.univDGVN = new VixenModules.Output.E131.Controls.DataGridViewNumbered();
            this.startColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.activeColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.universeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sizeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericPriority)).BeginInit();
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
            this.warningsCheckBox.Location = new System.Drawing.Point(24, 45);
            this.warningsCheckBox.Name = "warningsCheckBox";
            this.warningsCheckBox.Size = new System.Drawing.Size(241, 17);
            this.warningsCheckBox.TabIndex = 2;
            this.warningsCheckBox.Text = "Display ALL Warnings/Errors and wait For OK";
            // 
            // statisticsCheckBox
            // 
            this.statisticsCheckBox.AutoSize = true;
            this.statisticsCheckBox.Location = new System.Drawing.Point(24, 71);
            this.statisticsCheckBox.Name = "statisticsCheckBox";
            this.statisticsCheckBox.Size = new System.Drawing.Size(240, 17);
            this.statisticsCheckBox.TabIndex = 3;
            this.statisticsCheckBox.Text = "Gather statistics and display at end of session";
            // 
            // eventRepeatCountTextBox
            // 
            this.eventRepeatCountTextBox.Location = new System.Drawing.Point(24, 157);
            this.eventRepeatCountTextBox.MaxLength = 2;
            this.eventRepeatCountTextBox.Name = "eventRepeatCountTextBox";
            this.eventRepeatCountTextBox.Size = new System.Drawing.Size(30, 20);
            this.eventRepeatCountTextBox.TabIndex = 4;
            this.eventRepeatCountTextBox.Text = "0";
            this.eventRepeatCountTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.eventRepeatCountTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.eventRepeatCountTextBoxValidating);
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(74, 157);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(360, 26);
            this.label.TabIndex = 5;
            this.label.Text = "Max Repeat Count:  Set to 0 to send all events (even 0s) to each universe,\r\nSet t" +
    "o >0 to stop repeating frames after N duplicates are sent.";
            // 
            // okButton
            // 
            this.okButton.AutoSize = true;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(292, 397);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 101;
            this.okButton.Text = "&OK";
            // 
            // cancelButton
            // 
            this.cancelButton.AutoSize = true;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(385, 398);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 102;
            this.cancelButton.Text = "&Cancel";
            // 
            // eventSuppressCountTextBox
            // 
            this.eventSuppressCountTextBox.Location = new System.Drawing.Point(24, 193);
            this.eventSuppressCountTextBox.MaxLength = 2;
            this.eventSuppressCountTextBox.Name = "eventSuppressCountTextBox";
            this.eventSuppressCountTextBox.Size = new System.Drawing.Size(30, 20);
            this.eventSuppressCountTextBox.TabIndex = 103;
            this.eventSuppressCountTextBox.Text = "0";
            this.eventSuppressCountTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.eventSuppressCountTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.eventSuppressCountTextBoxValidating);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(74, 193);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(324, 26);
            this.label1.TabIndex = 104;
            this.label1.Text = "Max Suppress Count:  Only used if Max Repeat Count is not  0. \r\nSet to >0 to allo" +
    "w every Nth duplicate frame in a universe to go out.";
            // 
            // autoPopulateStart
            // 
            this.autoPopulateStart.AutoSize = true;
            this.autoPopulateStart.Location = new System.Drawing.Point(24, 22);
            this.autoPopulateStart.Name = "autoPopulateStart";
            this.autoPopulateStart.Size = new System.Drawing.Size(169, 17);
            this.autoPopulateStart.TabIndex = 105;
            this.autoPopulateStart.Text = "Manually manage start values.";
            this.autoPopulateStart.CheckedChanged += new System.EventHandler(this.autoPopulateStart_CheckedChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 8);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(452, 376);
            this.tabControl1.TabIndex = 106;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.btnDelete);
            this.tabPage1.Controls.Add(this.btnAdd);
            this.tabPage1.Controls.Add(this.btnAddUnicast);
            this.tabPage1.Controls.Add(this.lblDestination);
            this.tabPage1.Controls.Add(this.comboDestination);
            this.tabPage1.Controls.Add(this.univDGVN);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(444, 350);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Universes";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(362, 45);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(27, 28);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "-";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(362, 11);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(27, 28);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnAddUnicast
            // 
            this.btnAddUnicast.Location = new System.Drawing.Point(331, 316);
            this.btnAddUnicast.Name = "btnAddUnicast";
            this.btnAddUnicast.Size = new System.Drawing.Size(105, 24);
            this.btnAddUnicast.TabIndex = 4;
            this.btnAddUnicast.Text = "Add Unicast IP";
            this.btnAddUnicast.UseVisualStyleBackColor = true;
            this.btnAddUnicast.Click += new System.EventHandler(this.btnAddUnicast_Click);
            // 
            // lblDestination
            // 
            this.lblDestination.AutoSize = true;
            this.lblDestination.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDestination.Location = new System.Drawing.Point(2, 314);
            this.lblDestination.Name = "lblDestination";
            this.lblDestination.Size = new System.Drawing.Size(107, 24);
            this.lblDestination.TabIndex = 3;
            this.lblDestination.Text = "Destination:";
            // 
            // comboDestination
            // 
            this.comboDestination.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDestination.FormattingEnabled = true;
            this.comboDestination.Location = new System.Drawing.Point(111, 317);
            this.comboDestination.Name = "comboDestination";
            this.comboDestination.Size = new System.Drawing.Size(214, 21);
            this.comboDestination.TabIndex = 2;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lblPriority);
            this.tabPage2.Controls.Add(this.numericPriority);
            this.tabPage2.Controls.Add(this.chkBoxTransmitBlind);
            this.tabPage2.Controls.Add(this.label);
            this.tabPage2.Controls.Add(this.autoPopulateStart);
            this.tabPage2.Controls.Add(this.eventRepeatCountTextBox);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.statisticsCheckBox);
            this.tabPage2.Controls.Add(this.eventSuppressCountTextBox);
            this.tabPage2.Controls.Add(this.warningsCheckBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(444, 350);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Advanced Options";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lblPriority
            // 
            this.lblPriority.AutoSize = true;
            this.lblPriority.Location = new System.Drawing.Point(91, 126);
            this.lblPriority.Name = "lblPriority";
            this.lblPriority.Size = new System.Drawing.Size(70, 13);
            this.lblPriority.TabIndex = 108;
            this.lblPriority.Text = "sACN priority.";
            // 
            // numericPriority
            // 
            this.numericPriority.Location = new System.Drawing.Point(24, 122);
            this.numericPriority.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numericPriority.Name = "numericPriority";
            this.numericPriority.Size = new System.Drawing.Size(49, 20);
            this.numericPriority.TabIndex = 107;
            this.numericPriority.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // chkBoxTransmitBlind
            // 
            this.chkBoxTransmitBlind.AutoSize = true;
            this.chkBoxTransmitBlind.Location = new System.Drawing.Point(24, 95);
            this.chkBoxTransmitBlind.Name = "chkBoxTransmitBlind";
            this.chkBoxTransmitBlind.Size = new System.Drawing.Size(118, 17);
            this.chkBoxTransmitBlind.TabIndex = 106;
            this.chkBoxTransmitBlind.Text = "Transmit blind data.";
            this.chkBoxTransmitBlind.UseVisualStyleBackColor = true;
            // 
            // univDGVN
            // 
            this.univDGVN.AllowUserToAddRows = false;
            this.univDGVN.BackgroundColor = this.BackColor;
            this.univDGVN.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.startColumn,
            this.activeColumn,
            this.universeColumn,
            this.sizeColumn});
            this.univDGVN.ContextMenuStrip = this.rowManipulationContextMenuStrip;
            this.univDGVN.Location = new System.Drawing.Point(87, 11);
            this.univDGVN.Name = "univDGVN";
            this.univDGVN.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.univDGVN.Size = new System.Drawing.Size(269, 295);
            this.univDGVN.TabIndex = 1;
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
            this.startColumn.Width = 60;
            // 
            // activeColumn
            // 
            this.activeColumn.ContextMenuStrip = this.rowManipulationContextMenuStrip;
            this.activeColumn.HeaderText = "On";
            this.activeColumn.Name = "activeColumn";
            this.activeColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.activeColumn.Width = 25;
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
            this.universeColumn.Width = 60;
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
            this.sizeColumn.Width = 60;
            // 
            // SetupForm
            // 
            this.AcceptButton = this.okButton;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(469, 433);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu;
            this.Name = "SetupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Streaming ACN (E1.31) Configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SetupForm_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericPriority)).EndInit();
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
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Label lblDestination;
        private ComboBox comboDestination;
        private Label lblPriority;
        private NumericUpDown numericPriority;
        private CheckBox chkBoxTransmitBlind;
        private DataGridViewTextBoxColumn startColumn;
        private DataGridViewCheckBoxColumn activeColumn;
        private DataGridViewTextBoxColumn universeColumn;
        private DataGridViewTextBoxColumn sizeColumn;
        private Button btnAddUnicast;
        private Button btnAdd;
        private Button btnDelete;
    }
}