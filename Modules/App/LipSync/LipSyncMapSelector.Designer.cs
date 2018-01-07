namespace VixenModules.App.LipSyncApp
{
    partial class LipSyncMapSelector
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;



        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonEditMap = new System.Windows.Forms.Button();
            this.buttonDeleteMap = new System.Windows.Forms.Button();
            this.buttonNewMap = new System.Windows.Forms.Button();
            this.buttonCloneMap = new System.Windows.Forms.Button();
            this.mappingsListView = new System.Windows.Forms.ListView();
            this.newContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.stringMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pixelMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.BackColor = System.Drawing.Color.Transparent;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonCancel.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCancel.Location = new System.Drawing.Point(355, 310);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(93, 29);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = false;
            this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
            this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.BackColor = System.Drawing.Color.Transparent;
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonOK.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOK.Location = new System.Drawing.Point(256, 310);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(93, 29);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = false;
            this.buttonOK.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
            this.buttonOK.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
            // 
            // buttonEditMap
            // 
            this.buttonEditMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEditMap.BackColor = System.Drawing.Color.Transparent;
            this.buttonEditMap.Enabled = false;
            this.buttonEditMap.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonEditMap.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonEditMap.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonEditMap.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonEditMap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEditMap.Location = new System.Drawing.Point(380, 45);
            this.buttonEditMap.Name = "buttonEditMap";
            this.buttonEditMap.Size = new System.Drawing.Size(68, 27);
            this.buttonEditMap.TabIndex = 7;
            this.buttonEditMap.Text = "Edit";
            this.buttonEditMap.UseVisualStyleBackColor = false;
            this.buttonEditMap.Click += new System.EventHandler(this.buttonEditMap_Click);
            this.buttonEditMap.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
            this.buttonEditMap.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
            // 
            // buttonDeleteMap
            // 
            this.buttonDeleteMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDeleteMap.BackColor = System.Drawing.Color.Transparent;
            this.buttonDeleteMap.Enabled = false;
            this.buttonDeleteMap.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonDeleteMap.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonDeleteMap.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonDeleteMap.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonDeleteMap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDeleteMap.Location = new System.Drawing.Point(380, 78);
            this.buttonDeleteMap.Name = "buttonDeleteMap";
            this.buttonDeleteMap.Size = new System.Drawing.Size(68, 27);
            this.buttonDeleteMap.TabIndex = 8;
            this.buttonDeleteMap.Text = "Remove";
            this.buttonDeleteMap.UseVisualStyleBackColor = false;
            this.buttonDeleteMap.Click += new System.EventHandler(this.buttonDeleteMapping_Click);
            this.buttonDeleteMap.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
            this.buttonDeleteMap.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
            // 
            // buttonNewMap
            // 
            this.buttonNewMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNewMap.BackColor = System.Drawing.Color.Transparent;
            this.buttonNewMap.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonNewMap.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonNewMap.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonNewMap.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonNewMap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNewMap.Location = new System.Drawing.Point(380, 12);
            this.buttonNewMap.Name = "buttonNewMap";
            this.buttonNewMap.Size = new System.Drawing.Size(68, 27);
            this.buttonNewMap.TabIndex = 9;
            this.buttonNewMap.Text = "New";
            this.buttonNewMap.UseVisualStyleBackColor = false;
            this.buttonNewMap.Click += new System.EventHandler(this.buttonNewMap_Click);
            this.buttonNewMap.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
            this.buttonNewMap.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
            // 
            // buttonCloneMap
            // 
            this.buttonCloneMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCloneMap.BackColor = System.Drawing.Color.Transparent;
            this.buttonCloneMap.Enabled = false;
            this.buttonCloneMap.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonCloneMap.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonCloneMap.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonCloneMap.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonCloneMap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCloneMap.Location = new System.Drawing.Point(380, 111);
            this.buttonCloneMap.Name = "buttonCloneMap";
            this.buttonCloneMap.Size = new System.Drawing.Size(68, 27);
            this.buttonCloneMap.TabIndex = 10;
            this.buttonCloneMap.Text = "Clone";
            this.buttonCloneMap.UseVisualStyleBackColor = false;
            this.buttonCloneMap.Click += new System.EventHandler(this.buttonCloneMap_Click);
            this.buttonCloneMap.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
            this.buttonCloneMap.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
            // 
            // mappingsListView
            // 
            this.mappingsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mappingsListView.FullRowSelect = true;
            this.mappingsListView.LabelEdit = true;
            this.mappingsListView.Location = new System.Drawing.Point(26, 12);
            this.mappingsListView.MaximumSize = new System.Drawing.Size(524, 377);
            this.mappingsListView.Name = "mappingsListView";
            this.mappingsListView.Size = new System.Drawing.Size(348, 273);
            this.mappingsListView.TabIndex = 12;
            this.mappingsListView.UseCompatibleStateImageBehavior = false;
            this.mappingsListView.View = System.Windows.Forms.View.Details;
            this.mappingsListView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.mappingsListView_AfterLabelEdit);
            this.mappingsListView.BeforeLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.mappingsListView_BeforeLabelEdit);
            this.mappingsListView.SelectedIndexChanged += new System.EventHandler(this.mappingsListView_SelectedIndexChanged);
            this.mappingsListView.DoubleClick += new System.EventHandler(this.mappingsListView_DoubleClick);
            this.mappingsListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mappingsListView_KeyDown);
            // 
            // newContextMenu
            // 
            this.newContextMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.newContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stringMapToolStripMenuItem,
            this.pixelMapToolStripMenuItem});
            this.newContextMenu.Name = "newContextMenu";
            this.newContextMenu.ShowImageMargin = false;
            this.newContextMenu.Size = new System.Drawing.Size(108, 48);
            this.newContextMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.newContextMenu_ItemClicked);
            // 
            // stringMapToolStripMenuItem
            // 
            this.stringMapToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.stringMapToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.stringMapToolStripMenuItem.Name = "stringMapToolStripMenuItem";
            this.stringMapToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.stringMapToolStripMenuItem.Tag = "false";
            this.stringMapToolStripMenuItem.Text = "String Map";
            // 
            // pixelMapToolStripMenuItem
            // 
            this.pixelMapToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.pixelMapToolStripMenuItem.Name = "pixelMapToolStripMenuItem";
            this.pixelMapToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.pixelMapToolStripMenuItem.Tag = "true";
            this.pixelMapToolStripMenuItem.Text = "Pixel Map";
            // 
            // LipSyncMapSelector
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(462, 356);
            this.Controls.Add(this.mappingsListView);
            this.Controls.Add(this.buttonCloneMap);
            this.Controls.Add(this.buttonNewMap);
            this.Controls.Add(this.buttonDeleteMap);
            this.Controls.Add(this.buttonEditMap);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.KeyPreview = true;
            this.MaximumSize = new System.Drawing.Size(640, 480);
            this.MinimumSize = new System.Drawing.Size(375, 300);
            this.Name = "LipSyncMapSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "LipSync Maps";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LipSyncMapSelector_FormClosing);
            this.Load += new System.EventHandler(this.LipSyncMapSelector_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LipSyncMapSelector_KeyDown);
            this.Resize += new System.EventHandler(this.LipSyncMapSelector_Resize);
            this.newContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonEditMap;
        private System.Windows.Forms.Button buttonDeleteMap;
        private System.Windows.Forms.Button buttonNewMap;
        private System.Windows.Forms.Button buttonCloneMap;
        private System.Windows.Forms.ListView mappingsListView;
        private System.Windows.Forms.ContextMenuStrip newContextMenu;
        private System.Windows.Forms.ToolStripMenuItem stringMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pixelMapToolStripMenuItem;
    }
}