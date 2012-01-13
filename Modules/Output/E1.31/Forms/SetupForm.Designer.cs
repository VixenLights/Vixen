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

        private ContextMenuStrip destinationContextMenuStrip;

        private ToolTip destinationToolTip;

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

        private MenuItem mIHelpAbout;

        private MenuItem mIHelpShowSys;

        private Label label;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.rowManipulationContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mIHelp = new System.Windows.Forms.MenuItem();
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.warningsCheckBox = new System.Windows.Forms.CheckBox();
            this.statisticsCheckBox = new System.Windows.Forms.CheckBox();
            this.eventRepeatCountTextBox = new System.Windows.Forms.TextBox();
            this.label = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.univDGVN = new VixenModules.Output.E131.Controls.DataGridViewNumbered();
            this.activeColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.universeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.startColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sizeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.destinationColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ttlColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.warningsCheckBox.Location = new System.Drawing.Point(10, 260);
            this.warningsCheckBox.Name = "warningsCheckBox";
            this.warningsCheckBox.Size = new System.Drawing.Size(241, 17);
            this.warningsCheckBox.TabIndex = 2;
            this.warningsCheckBox.Text = "Display ALL Warnings/Errors and wait For OK";
            // 
            // statisticsCheckBox
            // 
            this.statisticsCheckBox.AutoSize = true;
            this.statisticsCheckBox.Location = new System.Drawing.Point(10, 284);
            this.statisticsCheckBox.Name = "statisticsCheckBox";
            this.statisticsCheckBox.Size = new System.Drawing.Size(240, 17);
            this.statisticsCheckBox.TabIndex = 3;
            this.statisticsCheckBox.Text = "Gather statistics and display at end of session";
            // 
            // eventRepeatCountTextBox
            // 
            this.eventRepeatCountTextBox.Location = new System.Drawing.Point(10, 308);
            this.eventRepeatCountTextBox.MaxLength = 2;
            this.eventRepeatCountTextBox.Name = "eventRepeatCountTextBox";
            this.eventRepeatCountTextBox.Size = new System.Drawing.Size(30, 20);
            this.eventRepeatCountTextBox.TabIndex = 4;
            this.eventRepeatCountTextBox.Text = "0";
            this.eventRepeatCountTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(60, 308);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(346, 26);
            this.label.TabIndex = 5;
            this.label.Text = "Event Repeat Count: Set to 0 to send all events to each universe,\r\nset to > 0 to " +
    "skip \'x\' events if data is unchanged on a per universe basis.";
            // 
            // okButton
            // 
            this.okButton.AutoSize = true;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(423, 346);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 101;
            this.okButton.Text = "&OK";
            // 
            // cancelButton
            // 
            this.cancelButton.AutoSize = true;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(504, 346);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 102;
            this.cancelButton.Text = "&Cancel";
            // 
            // univDGVN
            // 
            this.univDGVN.AllowUserToDeleteRows = false;
            this.univDGVN.BackgroundColor = this.BackColor;
            this.univDGVN.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.univDGVN.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.activeColumn,
            this.universeColumn,
            this.startColumn,
            this.sizeColumn,
            this.destinationColumn,
            this.ttlColumn});
            this.univDGVN.Location = new System.Drawing.Point(1, 3);
            this.univDGVN.Name = "univDGVN";
            this.univDGVN.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.univDGVN.Size = new System.Drawing.Size(578, 251);
            this.univDGVN.TabIndex = 1;
            this.univDGVN.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.UnivDgvnCellMouseClick);
            // 
            // activeColumn
            // 
            this.activeColumn.ContextMenuStrip = this.rowManipulationContextMenuStrip;
            this.activeColumn.HeaderText = "Active";
            this.activeColumn.Name = "activeColumn";
            this.activeColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.activeColumn.Width = 25;
            // 
            // universeColumn
            // 
            this.universeColumn.ContextMenuStrip = this.rowManipulationContextMenuStrip;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.universeColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.universeColumn.HeaderText = "Universe";
            this.universeColumn.MaxInputLength = 5;
            this.universeColumn.Name = "universeColumn";
            this.universeColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.universeColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.universeColumn.ToolTipText = "Sort (LeftClick = Ascending, RightClick = Descending)";
            this.universeColumn.Width = 60;
            // 
            // startColumn
            // 
            this.startColumn.ContextMenuStrip = this.rowManipulationContextMenuStrip;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.startColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.startColumn.HeaderText = "Start";
            this.startColumn.MaxInputLength = 5;
            this.startColumn.Name = "startColumn";
            this.startColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.startColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.startColumn.ToolTipText = "Sort (LeftClick = Ascending, RightClick = Descending)";
            this.startColumn.Width = 60;
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
            // destinationColumn
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.destinationColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.destinationColumn.HeaderText = "Destination";
            this.destinationColumn.Name = "destinationColumn";
            this.destinationColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.destinationColumn.ToolTipText = "Sort (LeftClick = Ascending, RightClick = Descending)";
            this.destinationColumn.Width = 300;
            // 
            // ttlColumn
            // 
            this.ttlColumn.ContextMenuStrip = this.rowManipulationContextMenuStrip;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.ttlColumn.DefaultCellStyle = dataGridViewCellStyle5;
            this.ttlColumn.HeaderText = "TTL";
            this.ttlColumn.MaxInputLength = 2;
            this.ttlColumn.Name = "ttlColumn";
            this.ttlColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ttlColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ttlColumn.Width = 30;
            // 
            // SetupForm
            // 
            this.AcceptButton = this.okButton;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(582, 372);
            this.Controls.Add(this.warningsCheckBox);
            this.Controls.Add(this.statisticsCheckBox);
            this.Controls.Add(this.eventRepeatCountTextBox);
            this.Controls.Add(this.label);
            this.Controls.Add(this.univDGVN);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Menu = this.mainMenu;
            this.Name = "SetupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "J1Sys E1.31 Setup Form";
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

        private DataGridViewCheckBoxColumn activeColumn;
        private DataGridViewTextBoxColumn universeColumn;
        private DataGridViewTextBoxColumn startColumn;
        private DataGridViewTextBoxColumn sizeColumn;
        private DataGridViewComboBoxColumn destinationColumn;
        private DataGridViewTextBoxColumn ttlColumn;
    }
}