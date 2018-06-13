namespace VixenModules.App.LipSyncApp
{
    partial class LipSyncMapEditor
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
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.nameTextBox = new System.Windows.Forms.TextBox();
			this.buttonAssign = new System.Windows.Forms.Button();
			this.notesLabel = new System.Windows.Forms.Label();
			this.notesTextBox = new System.Windows.Forms.TextBox();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.tabControl = new Dotnetrix.Controls.TabControlEX();
			this.tabMouth = new Dotnetrix.Controls.TabPageEX();
			this.dataGridViewMouth = new System.Windows.Forms.DataGridView();
			this.tabOther = new Dotnetrix.Controls.TabPageEX();
			this.dataGridViewOther = new System.Windows.Forms.DataGridView();
			this.tableLayoutPanel2.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.tabMouth.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewMouth)).BeginInit();
			this.tabOther.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewOther)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonOK.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonOK.Location = new System.Drawing.Point(695, 406);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(93, 25);
			this.buttonOK.TabIndex = 7;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOK.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonCancel.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonCancel.Location = new System.Drawing.Point(794, 406);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(93, 25);
			this.buttonCancel.TabIndex = 8;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(305, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(55, 15);
			this.label1.TabIndex = 2;
			this.label1.Text = "Elements";
			// 
			// label2
			// 
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 15);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(39, 15);
			this.label2.TabIndex = 0;
			this.label2.Text = "Name";
			// 
			// nameTextBox
			// 
			this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.nameTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.nameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.nameTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.nameTextBox.Location = new System.Drawing.Point(55, 11);
			this.nameTextBox.Name = "nameTextBox";
			this.nameTextBox.Size = new System.Drawing.Size(244, 23);
			this.nameTextBox.TabIndex = 1;
			// 
			// buttonAssign
			// 
			this.buttonAssign.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonAssign.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonAssign.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonAssign.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonAssign.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAssign.Location = new System.Drawing.Point(366, 8);
			this.buttonAssign.Name = "buttonAssign";
			this.buttonAssign.Size = new System.Drawing.Size(93, 29);
			this.buttonAssign.TabIndex = 3;
			this.buttonAssign.Text = "Assign";
			this.buttonAssign.UseVisualStyleBackColor = true;
			this.buttonAssign.Click += new System.EventHandler(this.buttonAssign_Click);
			this.buttonAssign.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonAssign.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// notesLabel
			// 
			this.notesLabel.AutoSize = true;
			this.notesLabel.Location = new System.Drawing.Point(8, 363);
			this.notesLabel.Name = "notesLabel";
			this.notesLabel.Size = new System.Drawing.Size(41, 15);
			this.notesLabel.TabIndex = 28;
			this.notesLabel.Text = "Notes:";
			// 
			// notesTextBox
			// 
			this.notesTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.notesTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tableLayoutPanel2.SetColumnSpan(this.notesTextBox, 4);
			this.notesTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.notesTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.notesTextBox.Location = new System.Drawing.Point(55, 366);
			this.notesTextBox.Multiline = true;
			this.notesTextBox.Name = "notesTextBox";
			this.tableLayoutPanel2.SetRowSpan(this.notesTextBox, 2);
			this.notesTextBox.Size = new System.Drawing.Size(634, 65);
			this.notesTextBox.TabIndex = 6;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 7;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.Controls.Add(this.tabControl, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.nameTextBox, 1, 0);
			this.tableLayoutPanel2.Controls.Add(this.notesLabel, 0, 2);
			this.tableLayoutPanel2.Controls.Add(this.label1, 2, 0);
			this.tableLayoutPanel2.Controls.Add(this.notesTextBox, 1, 2);
			this.tableLayoutPanel2.Controls.Add(this.buttonAssign, 3, 0);
			this.tableLayoutPanel2.Controls.Add(this.buttonCancel, 6, 3);
			this.tableLayoutPanel2.Controls.Add(this.buttonOK, 5, 3);
			this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(5);
			this.tableLayoutPanel2.RowCount = 4;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(895, 439);
			this.tableLayoutPanel2.TabIndex = 30;
			// 
			// tabControl
			// 
			this.tableLayoutPanel2.SetColumnSpan(this.tabControl, 7);
			this.tabControl.Controls.Add(this.tabMouth);
			this.tabControl.Controls.Add(this.tabOther);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.ItemSize = new System.Drawing.Size(42, 18);
			this.tabControl.Location = new System.Drawing.Point(11, 46);
			this.tabControl.Margin = new System.Windows.Forms.Padding(6);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(873, 311);
			this.tabControl.TabIndex = 4;
			this.tabControl.UseVisualStyles = false;
			// 
			// tabMouth
			// 
			this.tabMouth.Controls.Add(this.dataGridViewMouth);
			this.tabMouth.Location = new System.Drawing.Point(4, 22);
			this.tabMouth.Margin = new System.Windows.Forms.Padding(6);
			this.tabMouth.Name = "tabMouth";
			this.tabMouth.Size = new System.Drawing.Size(865, 285);
			this.tabMouth.TabIndex = 1;
			this.tabMouth.Text = "Mouth";
			this.tabMouth.UseVisualStyleBackColor = true;
			// 
			// dataGridViewMouth
			// 
			this.dataGridViewMouth.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.dataGridViewMouth.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewMouth.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewMouth.Location = new System.Drawing.Point(0, 0);
			this.dataGridViewMouth.Name = "dataGridViewMouth";
			this.dataGridViewMouth.Size = new System.Drawing.Size(865, 285);
			this.dataGridViewMouth.TabIndex = 5;
			this.dataGridViewMouth.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellContentClick);
			this.dataGridViewMouth.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellDoubleClick);
			this.dataGridViewMouth.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridViewMouth_CellPainting);
			// 
			// tabOther
			// 
			this.tabOther.Controls.Add(this.dataGridViewOther);
			this.tabOther.Location = new System.Drawing.Point(4, 22);
			this.tabOther.Margin = new System.Windows.Forms.Padding(6);
			this.tabOther.Name = "tabOther";
			this.tabOther.Size = new System.Drawing.Size(865, 285);
			this.tabOther.TabIndex = 2;
			this.tabOther.Text = "Outlines/Eyes";
			// 
			// dataGridViewOther
			// 
			this.dataGridViewOther.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.dataGridViewOther.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewOther.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewOther.Location = new System.Drawing.Point(0, 0);
			this.dataGridViewOther.Name = "dataGridViewOther";
			this.dataGridViewOther.Size = new System.Drawing.Size(865, 285);
			this.dataGridViewOther.TabIndex = 22;
			this.dataGridViewOther.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellContentClick);
			this.dataGridViewOther.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellDoubleClick);
			this.dataGridViewOther.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridViewOther_CellPainting);
			// 
			// LipSyncMapEditor
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(895, 439);
			this.Controls.Add(this.tableLayoutPanel2);
			this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.MaximumSize = new System.Drawing.Size(1024, 1033);
			this.Name = "LipSyncMapEditor";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "LipSync Mapping";
			this.Load += new System.EventHandler(this.LipSyncMapSetup_Load);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.tabControl.ResumeLayout(false);
			this.tabMouth.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewMouth)).EndInit();
			this.tabOther.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewOther)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Button buttonAssign;
        private System.Windows.Forms.Label notesLabel;
        private System.Windows.Forms.TextBox notesTextBox;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private Dotnetrix.Controls.TabControlEX tabControl;
		private Dotnetrix.Controls.TabPageEX tabMouth;
		private Dotnetrix.Controls.TabPageEX tabOther;
		private System.Windows.Forms.DataGridView dataGridViewMouth;
		private System.Windows.Forms.DataGridView dataGridViewOther;
	}
}